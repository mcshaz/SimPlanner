using Breeze.ContextProvider;
using Breeze.ContextProvider.EF6;
using LinqKit;
using SP.DataAccess;
using SP.DataAccess.Data.Interfaces;
using SP.Dto.Utilities;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Security.Principal;

namespace SP.Dto.ProcessBreezeRequests
{
    internal sealed class ValidateMedSim : IDisposable
    {
        private readonly IPrincipal _user;
        private MedSimDbContext _context;

        public ValidateMedSim(IPrincipal user)
        {
            _user = user;
        }

        private MedSimDbContext Context
        {
            get
            {
                return _context ?? (_context = new MedSimDbContext());
            }
        }

        public Dictionary<Type, List<EntityInfo>> Validate(Dictionary<Type, List<EntityInfo>> saveMap)
        {
            IEnumerable<EFEntityError> errors = new EFEntityError[0];

            List<EntityInfo> currentInfos;
            if (saveMap.TryGetValue(typeof(CourseFormatDto), out currentInfos))
            {
                errors = errors.Concat(GetCourseFormatErrors(currentInfos));
            }

            if (saveMap.TryGetValue(typeof(InstitutionDto), out currentInfos))
            {
                errors = errors.Concat(GetInstitutionErrors(currentInfos));
            }

            if (errors.Any())
            {
                throw new EntityErrorsException(errors);
            }

            if (saveMap.TryGetValue(typeof(ScenarioResourceDto), out currentInfos))
            {
                SaveScenarioFileToFileSystem(currentInfos);
            }

            if (saveMap.TryGetValue(typeof(ActivityDto), out currentInfos))
            {
                SaveActivityFileToFileSystem(currentInfos);
            }

            return saveMap;
        }

        public void PostValidation(Dictionary<Type, List<EntityInfo>> saveMap, List<KeyMapping> maps)
        {
            List<EntityInfo> ei;
            if (saveMap.TryGetValue(typeof(CourseSlot), out ei))
            {
                UpdateICourseDays(ei.Select(e=> (CourseSlot)e.Entity));
            }

            if (saveMap.TryGetValue(typeof(ScenarioResource), out ei))
            {
                DeleteScenarioFromFileSystem(ei);
            }

            if (saveMap.TryGetValue(typeof(Activity), out ei))
            {
                DeleteActivityFromFileSystem(ei);
            }
        }

        IEnumerable<EFEntityError> GetCourseFormatErrors(List<EntityInfo> currentInfos)
        {
            var cfs = TypedEntityinfo<CourseFormatDto>.GetTyped(currentInfos);

            //multiple individual queries may be the way to go here
            var pred = cfs.Aggregate(PredicateBuilder.New<CourseFormat>(), (prev, cur) => prev.Or(
                c => cur.Entity.Id == c.Id && 
                    c.CourseTypeId != cur.Entity.CourseTypeId));
            if (Context.CourseFormats.Any(pred.Compile()))
            {
                throw new InvalidDataException();
            }

            var ids = cfs.Select(cf => cf.Entity.Id).ToList();
            var courseTypeIds = cfs.Select(cf => cf.Entity.CourseTypeId);

            var newFormatsForType = (from c in Context.CourseFormats
                                     where courseTypeIds.Contains(c.CourseTypeId) && !ids.Contains(c.Id)
                                     select new { c.Id, c.Description }).ToList();

            newFormatsForType.AddRange(cfs.Select(c => new { c.Entity.Id, c.Entity.Description }));

            return (from c in newFormatsForType
                    group c by c.Description into cg
                    where cg.Count() > 1
                    select cg).SelectMany(i => i)
                    .Where(i => ids.Contains(i.Id))
                    .Select(i => new EFEntityError(cfs.First(ci => ci.Entity.Id == i.Id).Info,
                        "RepeatWithinGroup",
                        string.Format("Each course format description must be unique within course type. [{0}]", i.Description),
                        "Description"));
        }

        IEnumerable<EFEntityError> GetParticipantErrors(List<EntityInfo> currentInfos)
        {
            var ps = TypedEntityinfo<ParticipantDto>.GetTyped(currentInfos);

            /* too dificult, and there are exceptions - had been trying to keep drs as drs etc
            var pred = PredicateBuilder.False<ProfessionalRole>();
            foreach (var p in ps)
            {
                object pr;
                if (p.Info.OriginalValuesMap.TryGetValue("ProfessionalRoleId", out pr) && !p.Entity.DefaultProfessionalRoleId.Equals(pr))
                {
                    Context.ProfessionalRoles.;
                }
            }
            */
            List<EFEntityError> returnVar = new List<EFEntityError>();
            foreach (var p in ps)
            {
                var dup = (from u in Context.Users
                           where p.Entity.Id != u.Id &&
                               p.Entity.FullName == u.FullName &&
                               p.Entity.DefaultDepartmentId == u.DefaultDepartmentId
                           select u.DefaultProfessionalRoleId).FirstOrDefault();
                if (dup != default(Guid) 
                    && ((dup == p.Entity.DefaultProfessionalRoleId 
                        || (from r in Context.ProfessionalRoles
                            where (new[] { dup, p.Entity.DefaultProfessionalRoleId}).Contains(r.Id)
                            group r by r.Description into c
                            select c).Count() == 1)))
                { 
                    returnVar.Add(new EFEntityError(p.Info,
                        "DuplicateUser",
                        "2 users with the same name, department and profession",
                        "FullName"));
                }

            }
            return returnVar;

        }

        IEnumerable<EFEntityError> GetInstitutionErrors(List<EntityInfo> currentInfos)
        {
            var insts = TypedEntityinfo<InstitutionDto>.GetTyped(currentInfos);
            List<EFEntityError> returnVar = new List<EFEntityError>();

            foreach (var i in insts)
            {
                try
                {
                    var ci = CultureInfo.GetCultureInfo(i.Entity.LocaleCode);
                    //not great separation of concerns here- this is not a buisness logic problem
                    if (i.Info.EntityState == EntityState.Added && !Context.Cultures.Any(c => c.LocaleCode == ci.Name))
                    {
                        CreateCulture(ci);
                    }
                }
                catch (CultureNotFoundException)
                {
                    returnVar.Add(new EFEntityError(i.Info,
                        "UnknownLocale",
                        "The Locale Code specified is not valid",
                        "LocaleCode"));
                }
                if (!string.IsNullOrWhiteSpace(i.Entity.StandardTimeZone))
                {
                    try
                    {
                        TimeZoneInfo.FindSystemTimeZoneById(i.Entity.StandardTimeZone);
                    }
                    catch (TimeZoneNotFoundException)
                    {
                        returnVar.Add(new EFEntityError(i.Info,
                            "UnknownTimeZone",
                            "The Time Zone specified is not valid",
                            "StandardTimeZone"));
                    }
                }
            }
            return returnVar;
        }

        void SaveScenarioFileToFileSystem(IEnumerable<EntityInfo> scenarioResources)
        {
            foreach (var sr in (from ei in scenarioResources
                                 let sr = (ScenarioResourceDto)ei.Entity
                                 where ei.EntityState == EntityState.Added || ei.EntityState == EntityState.Modified
                                    && sr.File != null
                                 select sr))
            {
                sr.CreateFile((from s in Context.Scenarios
                               where s.Id == sr.ScenarioId
                               select s.DepartmentId).First());
            }
        }

        void DeleteScenarioFromFileSystem(IEnumerable<EntityInfo> scenarioResources)
        {
            foreach (var sr in (from ei in scenarioResources
                                let sr = (ScenarioResource)ei.Entity
                                where ei.EntityState == EntityState.Deleted || sr.FileName == null
                                select sr))
            {
                sr.DeleteFile((from s in Context.Scenarios
                               where s.Id == sr.ScenarioId
                               select s.DepartmentId).First());
            }
        }

        void SaveActivityFileToFileSystem(IEnumerable<EntityInfo> activityResources)
        {
            foreach (var atr in (from ei in activityResources
                                let atr = (ActivityDto)ei.Entity
                                where (ei.EntityState == EntityState.Added || ei.EntityState == EntityState.Modified)
                                   && atr.File != null
                                select atr))
            {
                atr.CreateFile((from ca in Context.CourseActivities
                               where ca.Id == atr.CourseActivityId
                               select ca.CourseTypeId).First());
            }
        }

        void DeleteActivityFromFileSystem(IEnumerable<EntityInfo> activityResources)
        {
            foreach (var atr in (from ei in activityResources
                                let atr = (Activity)ei.Entity
                                 where ei.EntityState == EntityState.Deleted || atr.FileName == null
                                 select atr))
            {
                atr.DeleteFile((from ca in Context.CourseActivities
                               where ca.Id == atr.CourseActivityId
                               select ca.CourseTypeId).First());
            }
        }

        //if corseslots altered, need to update upcoming courses
        void UpdateICourseDays(IEnumerable<CourseSlot> alteredSlots)
        {
            var courseFormatIds = alteredSlots.Select(c => c.CourseFormatId).Distinct().ToList();
            var slotDays = (from cs in Context.CourseSlots
                            where courseFormatIds.Contains(cs.CourseFormatId) && cs.IsActive
                            group cs by cs.Day into g
                            select new { day = g.Key, minutes = g.Sum(m => m.MinutesDuration) })
                            .ToDictionary(d=>d.day,d=>d.minutes);

            foreach (var course in Context.Courses.Include("CourseDays").Include("CourseFormat")
                    .Where(c=>c.StartUtc > DateTime.UtcNow && courseFormatIds.Contains(c.CourseFormatId)))
            {
                var days = course.CourseDays.Cast<ICourseDay>().ToDictionary(k=>k.Day);
                days.Add(1, course);

                for (var i = 1; i <= course.CourseFormat.DaysDuration; i++)
                {
                    ICourseDay icd;
                    if (!days.TryGetValue(i,out icd)){
                        icd = new CourseDay {
                            Day=i,
                            Course = course,
                            StartUtc = days[i-1].StartUtc
                        };
                        Context.CourseDays.Add((CourseDay)icd);
                        days.Add(i, icd);
                    }
                    int duration;
                    slotDays.TryGetValue((byte)i,out duration);
                    icd.DurationMins = duration;
                }
                foreach (var i in days.Keys.Where(d=>d> course.CourseFormat.DaysDuration))
                {
                    days[i].DurationMins = 0;
                }
            }
            Context.SaveChanges();
        }
        //not great separation of concerns here- this is not a buisness logic problem 
        /*
        IEnumerable<EFEntityError> GetCourseSlotErrors(List<EntityInfo> currentInfos)
        {
            var insts = TypedEntityinfo<CourseSlot>.GetTyped(currentInfos);

            foreach (var i in insts)
            {
                object wasActive;
                if (!i.Entity.IsActive && i.Info.OriginalValuesMap.TryGetValue("IsActive", out wasActive) 
                    && (bool)wasActive)
                {
                    //could check all collections, but probably easiest to have a crack and see how we go
                    try
                    {
                        //need to do this as Participant of update
                    }
                }
            }
        }
        */

        private void CreateCulture(CultureInfo ci)
        {
            var ri = new RegionInfo(ci.LCID);
            var iso = ISO3166.FromAlpha2(ri.TwoLetterISORegionName);
            var c = new Culture
            {
                Name = ci.DisplayName,
                LocaleCode = ci.Name,
                CountryCode = iso.NumericCode
            };
            Context.Cultures.Add(c);
            Context.SaveChanges();
        }

        [Serializable]
        public class InvalidDataException : Exception
        {
            public InvalidDataException() : base() { }
            public InvalidDataException(string msg) : base(msg) { }
        }

        class TypedEntityinfo<T>
        {
            internal T Entity;
            internal EntityInfo Info;

            internal static IEnumerable<TypedEntityinfo<T>> GetTyped(IEnumerable<EntityInfo> info)
            {
                return info.Select(i => new TypedEntityinfo<T> { Info = i, Entity = (T)i.Entity }).ToList();
            }
        }

        #region IDisposable
        public void Dispose() {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        ~ValidateMedSim() { Dispose(false); }
        void Dispose(bool disposing)
        { // would be protected virtual if not sealed 
            if (disposing && _context!=null)
            { // only run this logic when Dispose is called
                _context.Dispose();
            }
        }
        #endregion //IDisposable
    }
}
