using Breeze.ContextProvider;
using LinqKit;
using Microsoft.AspNet.Identity.EntityFramework;
using SP.DataAccess;
using SP.DataAccess.Data.Interfaces;
using SP.Dto.Maps;
using SP.Dto.Utilities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Globalization;
using System.Linq;
using System.Reflection;
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

        private static readonly Type[] _associatedFiles = new[] {
            typeof(Institution),
            typeof(Room),
            typeof(ScenarioResource),
            typeof(CandidatePrereading),
            typeof(Activity)
        };

        private static IEnumerable<PropertyInfo> _identityUserDbProperties;
        private static IEnumerable<PropertyInfo> IdentityUserDbProperties
        {
            get
            {
                return _identityUserDbProperties ??
                    (_identityUserDbProperties = typeof(IdentityUser<Guid, AspNetUserLogin, AspNetUserRole, AspNetUserClaim>)
                        .GetProperties(BindingFlags.DeclaredOnly |
                                           BindingFlags.Public |
                                           BindingFlags.Instance));
            }
        }

        public Dictionary<Type, List<EntityInfo>> ValidateAndMapToServer(Dictionary<Type, List<EntityInfo>> saveMap)
        {
            IEnumerable<EntityError> errors = new EntityError[0];

            List<EntityInfo> currentInfos;
            if (saveMap.TryGetValue(typeof(CourseFormatDto), out currentInfos))
            {
                errors = errors.Concat(GetCourseFormatErrors(currentInfos));
            }

            if (saveMap.TryGetValue(typeof(InstitutionDto), out currentInfos))
            {
                errors = errors.Concat(GetInstitutionErrors(currentInfos));
            }

            if (saveMap.TryGetValue(typeof(CandidatePrereadingDto), out currentInfos))
            {
                errors = errors.Concat(GetCandidatePrereadingErrors(currentInfos));
            }

            if (saveMap.TryGetValue(typeof(RoomDto), out currentInfos))
            {
                errors = errors.Concat(GetRoomErrors(currentInfos));
            }

            if (saveMap.TryGetValue(typeof(ActivityDto), out currentInfos))
            {
                errors = errors.Concat(GetActivityErrors(currentInfos));
            }

            if (saveMap.TryGetValue(typeof(ScenarioResourceDto), out currentInfos))
            {
                errors = errors.Concat(GetScenarioResourceErrors(currentInfos));
            }

            if (errors.Any())
            {
                throw new EntityErrorsException(errors);
            }

            //avoid clobbering details sitting in out database not mapped to the DTO
            if(saveMap.TryGetValue(typeof(ParticipantDto), out currentInfos))
            {
                MapIdentityUser(currentInfos);
            }


            //map to server model
            var returnVar = new Dictionary<Type, List<EntityInfo>>();

            foreach (var kv in saveMap)
            {
                Type serverType = MapperConfig.GetServerModelType(kv.Key);
                var mapper = MapperConfig.GetFromDtoMapper(kv.Key);
                var vals = kv.Value.Select(d => d.ContextProvider.CreateEntityInfo(mapper(d.Entity), d.EntityState)).ToList();
                returnVar.Add(serverType, vals);
            }

            return returnVar;
        }

        public void PostValidation(Dictionary<Type, List<EntityInfo>> saveMap, List<KeyMapping> maps)
        {
            List<EntityInfo> ei;
            if (saveMap.TryGetValue(typeof(CourseSlot), out ei))
            {
                UpdateICourseDays(ei.Select(e=> (CourseSlot)e.Entity));
            }
            DeleteFilesFromFileSystem(saveMap.TryGetValues(_associatedFiles).SelectMany(f=>f));
        }

        void MapIdentityUser(List<EntityInfo> currentInfos)
        {
            var breezeParticipants = from ci in currentInfos
                                     where ci.EntityState == EntityState.Modified
                                     select (Participant)ci.Entity;
            var ids = breezeParticipants.Select(p => p.Id);
            var usrs = Context.Users.Where(u => ids.Contains(u.Id)).ToDictionary(u=>u.Id);
            foreach (var p in breezeParticipants)
            {
                Participant u = usrs[p.Id];
                foreach (var pi in IdentityUserDbProperties)
                {
                    pi.SetValue(p, pi.GetValue(u));
                }
            }

        }

        IEnumerable<EntityError> GetCourseFormatErrors(List<EntityInfo> currentInfos)
        {
            var cfs = TypedEntityInfo<CourseFormatDto>.GetTyped(currentInfos);

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
                    .Select(i => MappedEFEntityError.Create(cfs.First(ci => ci.Entity.Id == i.Id).Entity,
                        "RepeatWithinGroup",
                        string.Format("Each course format description must be unique within course type. [{0}]", i.Description),
                        "Description")).ToList(); //tolist so it throws here if a problem
        }

        IEnumerable<EntityError> GetParticipantErrors(List<EntityInfo> currentInfos)
        {
            var ps = TypedEntityInfo<ParticipantDto>.GetTyped(currentInfos);

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
            List<EntityError> returnVar = new List<EntityError>();
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
                    returnVar.Add(MappedEFEntityError.Create(p.Entity,
                        "DuplicateUser",
                        "2 users with the same name, department and profession",
                        "FullName"));
                }

            }
            return returnVar;

        }

        IEnumerable<EntityError> GetInstitutionErrors(List<EntityInfo> currentInfos)
        {
            var insts = TypedEntityInfo<InstitutionDto>.GetTyped(currentInfos);
            List<EntityError> returnVar = new List<EntityError>();

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
                    returnVar.Add(MappedEFEntityError.Create(i.Entity,
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
                        returnVar.Add(MappedEFEntityError.Create(i.Entity,
                            "UnknownTimeZone",
                            "The Time Zone specified is not valid",
                            "StandardTimeZone"));
                    }
                }
                returnVar.AddRange(GetFileErrors(i.Entity.File, i, () => Context.Institutions.Find(i.Entity.Id)));
            }
            return returnVar;
        }

        IEnumerable<EntityError> GetCandidatePrereadingErrors(List<EntityInfo> currentInfos)
        {
            var prereadings = TypedEntityInfo<CandidatePrereadingDto>.GetTyped(currentInfos);
            List<EntityError> returnVar = new List<EntityError>();

            foreach (var p in prereadings)
            {
                returnVar.AddRange(GetFileErrors(p.Entity.File, p, () => Context.CandidatePrereadings.Find(p.Entity.Id)));
            }
            return returnVar;
        }

        IEnumerable<EntityError> GetRoomErrors(List<EntityInfo> currentInfos)
        {
            var rooms = TypedEntityInfo<RoomDto>.GetTyped(currentInfos);
            List<EntityError> returnVar = new List<EntityError>();

            foreach (var r in rooms)
            {
                returnVar.AddRange(GetFileErrors(r.Entity.File, r, () => Context.Rooms.Find(r.Entity.Id)));
            }
            return returnVar;
        }

        IEnumerable<EntityError> GetScenarioResourceErrors(List<EntityInfo> currentInfos)
        {
            var resources = TypedEntityInfo<ScenarioResourceDto>.GetTyped(currentInfos);
            List<EntityError> returnVar = new List<EntityError>();

            foreach (var r in resources)
            {
                returnVar.AddRange(GetFileErrors(r.Entity.File, r, () => Context.ScenarioResources.Find(r.Entity.Id)));
            }
            return returnVar;
        }

        IEnumerable<EntityError> GetActivityErrors(List<EntityInfo> currentInfos)
        {
            var activities = TypedEntityInfo<ActivityDto>.GetTyped(currentInfos);
            List<EntityError> returnVar = new List<EntityError>();

            foreach (var a in activities)
            {
                returnVar.AddRange(GetFileErrors(a.Entity.File, a, () => Context.Activities.Find(a.Entity.Id)));
            }
            return returnVar;
        }

        IEnumerable<EntityError> GetFileErrors<T>(byte[] file, TypedEntityInfo<T> entityInfo, Func<IAssociateFileRequired> getExistingEntity) where T : class, IAssociateFileRequired
        {
            var returnVar = GetBaseErrors(file, entityInfo, entityInfo.Entity.FileSize, entityInfo.Entity.FileModified, () => getExistingEntity().AsOptional());
            if (entityInfo.Info.EntityState == EntityState.Added)
            {
                if (file == null)
                {
                    returnVar.Add(MappedEFEntityError.Create(entityInfo.Entity,
                        "FileNull",
                        "A file must be supplied for upload",
                        "File"));
                }
            }
            
            return returnVar;
        }

        List<EntityError> GetBaseErrors<T>(byte[] file, TypedEntityInfo<T> entityInfo, long? fileSize, DateTime? fileModified, Func<IAssociateFileOptional> getExistingEntity) where T:class, IAssociateFile
        {
            var returnVar = new List<EntityError>();
            if (file != null && fileSize != file.Length)
            {
                returnVar.Add(MappedEFEntityError.Create(entityInfo.Entity,
                    "FileSizeDifference",
                    "The file size stated is different to the size of the file being uploaded",
                    "FileSize"));
            }
            if (fileModified == default(DateTime))
            {
                returnVar.Add(MappedEFEntityError.Create(entityInfo.Entity,
                    "FileModifiedDefaultDate",
                    $"The file modified must not be the default value for a date ({(default(DateTime)):D})",
                    "FileModified"));
            }
            if (entityInfo.Info.EntityState == EntityState.Modified && file == null)
            {
                var existingEntity = getExistingEntity();
                if (entityInfo.Entity.FileName != existingEntity.FileName)
                {
                    returnVar.Add(MappedEFEntityError.Create(entityInfo.Entity,
                    "FileNameDifferWithExisting",
                    "The filename is different to the existing filename, but no new file is being uploaded",
                    "FileName"));
                }
                if (fileModified != existingEntity.FileModified)
                {
                    returnVar.Add(MappedEFEntityError.Create(entityInfo.Entity,
                    "FileModifiedDifferWithExisting",
                    "The file modified date is different to the existing date modified, but no new file is being uploaded",
                    "FileModified"));
                }
                if (fileSize != existingEntity.FileSize)
                {
                    returnVar.Add(MappedEFEntityError.Create(entityInfo.Entity,
                    "FileSizeDifferWithExisting",
                    "The file size is different to the existing file size, but no new file is being uploaded",
                    "FileSize"));
                }
            }
            return returnVar;
        }

        IEnumerable<EntityError> GetFileErrors<T>(byte[] file, TypedEntityInfo<T> entityInfo, Func<IAssociateFileOptional> getExistingEntity) where T: class, IAssociateFileOptional
        {
            var returnVar = GetBaseErrors(file, entityInfo, entityInfo.Entity.FileSize, entityInfo.Entity.FileModified, getExistingEntity);

            if ((entityInfo.Entity.FileModified == null) != (entityInfo.Entity.FileName == null) || (entityInfo.Entity.FileName == null) != (entityInfo.Entity.FileSize == null))
            {
                if (entityInfo.Entity.FileName == null)
                {
                    returnVar.Add(MappedEFEntityError.Create(entityInfo.Entity,
                        "FileNamePropertiesDiscordant",
                        "Filename is null but file size and date modified are not also null",
                        "FileName"));
                }
                else
                {
                    returnVar.Add(MappedEFEntityError.Create(entityInfo.Entity,
                        "FileNamePropertiesDiscordant",
                        "Filename is not null but file size or date modified are null",
                        "FileName"));
                }
            }
            return returnVar;
        }

        void DeleteFilesFromFileSystem(IEnumerable<EntityInfo> fileEntities)
        {
            foreach (var f in (from ei in fileEntities
                                let fr = ei.Entity as IAssociateFile
                                where fr != null && ei.EntityState == EntityState.Deleted
                                select fr))
            {
                f.DeleteFile();
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
                var days = course.AllDays().ToDictionary(k=>k.Day);

                for (int i = 1; i <= course.CourseFormat.DaysDuration; i++)
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
                foreach (var k in days.Keys.Where(d=>d> course.CourseFormat.DaysDuration))
                {
                    days[k].DurationMins = 0;
                }
            }
            Context.SaveChanges();
        }
        //not great separation of concerns here- this is not a buisness logic problem 
        /*
        IEnumerable<MappedEFEntityError> GetCourseSlotErrors(List<EntityInfo> currentInfos)
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

        class TypedEntityInfo<T>
        {
            internal T Entity;
            internal EntityInfo Info;

            internal static IEnumerable<TypedEntityInfo<T>> GetTyped(IEnumerable<EntityInfo> info)
            {
                return info.Select(i => new TypedEntityInfo<T> { Info = i, Entity = (T)i.Entity }).ToList();
            }
        }

        class MappedEFEntityError : EntityError
        {
            /// <summary>
            /// 
            /// </summary>
            /// <param name="entityInfo"></param>
            /// <param name="errorName"></param>
            /// <param name="errorMessage"></param>
            /// <param name="propertyName"></param>
            /// <param name="dtoType">If not specified, the TypeName from</param>
            internal MappedEFEntityError() { }
            public static MappedEFEntityError Create<T>(T entity, string errorName, string errorMessage, string propertyName)
            {
                return new MappedEFEntityError
                {
                    EntityTypeName = typeof(T).FullName,
                    KeyValues = GetKeyValues(entity),

                    ErrorName = errorName,
                    ErrorMessage = errorMessage,
                    PropertyName = propertyName,
                };
            }

            private static object[] GetKeyValues<T>(T entity)
            {
                //all primary key mapping is on the metadata type - could argue we should somehow be working on a version of the ContextPretender here!

                Type metaTypeFromAttr = ((MetadataTypeAttribute)typeof(T).GetCustomAttributes(typeof(MetadataTypeAttribute), false).Single()).MetadataClassType;
                var keyProps = metaTypeFromAttr.GetProperties().Where(prop => Attribute.IsDefined(prop, typeof(KeyAttribute)))
                    .Select(vp=>typeof(T).GetProperty(vp.Name)).ToList();
                if (keyProps.Count == 1)
                {
                    return new[] { keyProps[0].GetValue(entity) };
                }
                return keyProps.Select(kp => new {
                    value = kp.GetValue(entity),
                    order = kp.GetCustomAttributes(typeof(ColumnAttribute), true).Cast<ColumnAttribute>().Select(ca => ca.Order)
                }).OrderBy(a => a.order).Select(a => a.value).ToArray();

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
