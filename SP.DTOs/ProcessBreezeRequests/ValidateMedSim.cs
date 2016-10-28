﻿using Breeze.ContextProvider;
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
//using Ent = System.Data.Entity;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Security.Principal;

namespace SP.Dto.ProcessBreezeRequests
{
    internal sealed class ValidateMedSim : IDisposable
    {
        private MedSimDbContext _context;
        private readonly string _userName;
        private Participant _user;

        public ValidateMedSim(IPrincipal user)
        {
            if (user.Identity.IsAuthenticated)
            {
                if (user.IsInRole(RoleConstants.AccessAllData))
                {
                    _adminLevel = AdminLevels.AllData;
                }
                else if (user.IsInRole(RoleConstants.AccessInstitution))
                {
                    _adminLevel = AdminLevels.InstitutionAdmin;
                }
                else if (user.IsInRole(RoleConstants.AccessDepartment))
                {
                    _adminLevel = AdminLevels.DepartmentAdmin;
                }
                _userName = user.Identity.Name;
            }
        }

        private MedSimDbContext Context
        {
            get
            {
                return _context ?? (_context = new MedSimDbContext());
            }
        }
        private Participant User
        {
            get
            {
                return _user ?? (_user = Context.Users.First(u=>u.UserName == _userName)); //?include("Department")
            }
        }

        private enum AdminLevels { None = default(int), AllData, InstitutionAdmin, DepartmentAdmin };
        private readonly AdminLevels _adminLevel;
        private AdminLevels AdminLevel { get { return _adminLevel; } }

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

        public IEnumerable<EntityError> ValidateDto(Dictionary<Type, List<EntityInfo>> saveMap)
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

            return errors;
        }
        bool HasCoursePermission(Guid courseId)
        {
            return HasDepartmentPermission((from c in Context.Courses
                                            where c.Id == courseId
                                            select c.DepartmentId).First());
        }

        bool HasCourseTypePermission(Guid courseTypeId)
        {
            return HasDepartmentPermission((from c in Context.CourseTypeDepartments
                                            where c.CourseTypeId == courseTypeId
                                            select c.DepartmentId).ToList());
        }
        bool HasDepartmentPermission(Guid departmentId)
        {
            return HasDepartmentPermission(new[] { departmentId });
        }

        bool HasDepartmentPermission(IEnumerable<Guid> departmentIds)
        {
            switch (AdminLevel)
            {
                case AdminLevels.AllData:
                    return true;
                case AdminLevels.None:
                    return false;
                case AdminLevels.DepartmentAdmin:
                    return departmentIds.Contains(User.DefaultDepartmentId);
                case AdminLevels.InstitutionAdmin:
                    return departmentIds.Contains(User.DefaultDepartmentId) || User.Department.Institution.Departments.Any(d=>d.InstitutionId == User.Department.InstitutionId && departmentIds.Contains(d.Id));
            }
                throw new UnauthorizedAccessException("Unknown Admin Level");
        }

        bool HasInstitutionPermission(Guid institutionId)
        {
            switch (AdminLevel)
            {
                case AdminLevels.AllData:
                    return true;
                case AdminLevels.None:
                case AdminLevels.DepartmentAdmin:
                    return false;
                case AdminLevels.InstitutionAdmin:
                    return User.Department.InstitutionId == institutionId;
            }
            throw new UnauthorizedAccessException("Unknown Admin Level");
        }


        private static IEnumerable<EntityError> PermissionErrors<T>(Dictionary<Type, List<EntityInfo>> saveMap, Func<T,bool> isPermitted, string propertyName = null)
        {
            List<EntityInfo> eis;
            if (saveMap.TryGetValue(typeof(T), out eis))
            {
                return (from ei in eis
                        let e = (T)ei.Entity
                        where !isPermitted(e)
                        select MappedEFEntityError.Create(e, "insufficientPermission","you do not have permission to update this property",propertyName));
            }
            return Enumerable.Empty<EntityError>();
        }

        public IEnumerable<EntityError> ValidatePermission(Dictionary<Type, List<EntityInfo>> saveMap)
        {
            return PermissionErrors<ActivityDto>(saveMap,
                a => HasDepartmentPermission((from ca in Context.CourseActivities
                                              where ca.Id == a.CourseActivityId
                                              from ctd in ca.CourseType.CourseTypeDepartments
                                              select ctd.DepartmentId).ToList()))
                .Concat(PermissionErrors<CandidatePrereadingDto>(saveMap,
                c => HasCourseTypePermission(c.CourseTypeId)))
                .Concat(PermissionErrors<CultureDto>(saveMap,
                e => AdminLevel == AdminLevels.InstitutionAdmin))
                .Concat(PermissionErrors<CourseDto>(saveMap,
                e => HasDepartmentPermission(e.DepartmentId)))
                .Concat(PermissionErrors<CourseActivityDto>(saveMap,
                e => HasCourseTypePermission(e.CourseTypeId)))
                .Concat(PermissionErrors<CourseFormatDto>(saveMap,
                e => HasCourseTypePermission(e.CourseTypeId)))
                .Concat(PermissionErrors<CourseParticipantDto>(saveMap,
                e => HasCoursePermission(e.CourseId)))
                .Concat(PermissionErrors<CourseScenarioFacultyRoleDto>(saveMap,
                e => HasCoursePermission(e.CourseId)))
                .Concat(PermissionErrors<CourseSlotDto>(saveMap,
                e => HasDepartmentPermission((from c in Context.CourseFormats
                                              where c.Id == e.CourseFormatId
                                              from ctd in c.CourseType.CourseTypeDepartments
                                              select ctd.DepartmentId).ToList())))
                .Concat(PermissionErrors<CourseSlotActivityDto>(saveMap,
                e => HasCoursePermission(e.CourseId)))
                .Concat(PermissionErrors<CourseSlotManikinDto>(saveMap,
                e => HasCoursePermission(e.CourseId)))
                .Concat(PermissionErrors<CourseSlotPresenterDto>(saveMap,
                e => HasCoursePermission(e.CourseId)))
                .Concat(PermissionErrors<CourseTypeDto>(saveMap,
                e => HasCourseTypePermission(e.Id)))
                //issue of who can share a course type with others
                //perhaps a primary creator property should be added
                .Concat(PermissionErrors<CourseTypeDepartmentDto>(saveMap,
                e => HasCourseTypePermission(e.CourseTypeId)))
                .Concat(PermissionErrors<CourseTypeScenarioRoleDto>(saveMap,
                e => HasCourseTypePermission(e.CourseTypeId)))
                .Concat(PermissionErrors<DepartmentDto>(saveMap,
                e => HasDepartmentPermission(e.Id)))
                .Concat(PermissionErrors<FacultyScenarioRoleDto>(saveMap,
                e => HasDepartmentPermission((from ctsr in Context.CourseTypeScenarioRoles
                                              where ctsr.FacultyScenarioRoleId == e.Id
                                              from ctd in ctsr.CourseType.CourseTypeDepartments
                                              select ctd.DepartmentId).ToList())))
                .Concat(PermissionErrors<HotDrinkDto>(saveMap,
                e => AdminLevel == AdminLevels.InstitutionAdmin))
                .Concat(PermissionErrors<InstitutionDto>(saveMap,
                e => HasInstitutionPermission(e.Id)))
                .Concat(PermissionErrors<ManikinDto>(saveMap,
                e => HasDepartmentPermission(e.ModelId)))
                .Concat(PermissionErrors<ManikinManufacturerDto>(saveMap,
                e => AdminLevel == AdminLevels.InstitutionAdmin))
                .Concat(PermissionErrors<ProfessionalRoleDto>(saveMap,
                e => AdminLevel == AdminLevels.InstitutionAdmin))
                .Concat(PermissionErrors<ProfessionalRoleInstitutionDto>(saveMap,
                e => HasInstitutionPermission(e.InstitutionId)))
                .Concat(PermissionErrors<RoleDto>(saveMap,
                e => false))
                .Concat(PermissionErrors<RoomDto>(saveMap,
                e => HasDepartmentPermission(e.DepartmentId)))
                .Concat(PermissionErrors<ScenarioDto>(saveMap,
                e => HasDepartmentPermission(e.DepartmentId)))
                .Concat(PermissionErrors<ScenarioResourceDto>(saveMap,
                e => HasDepartmentPermission((from s in Context.Scenarios
                                              where s.Id == e.ScenarioId
                                              select s.DepartmentId).First())));
        }

        public static Dictionary<Type, List<EntityInfo>> MapDtoToServerType(Dictionary<Type, List<EntityInfo>> saveMap)
        {
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

        public void AfterSave(Dictionary<Type, List<EntityInfo>> saveMap, List<KeyMapping> maps)
        {
            List<EntityInfo> ei;
            if (saveMap.TryGetValue(typeof(CourseSlot), out ei))
            {
                UpdateICourseDays(ei.Select(e=> (CourseSlot)e.Entity));
            }
            var iAssocFiles = (from s in saveMap
                               where s.Key.IsAssignableFrom(typeof(IAssociateFile))
                               from v in s.Value
                               select v).ToLookup(k => k.EntityState, v => (IAssociateFile)v.Entity);

            foreach (var i in iAssocFiles[EntityState.Deleted]) {
                i.DeleteFile();
            }

            foreach (var i in iAssocFiles[EntityState.Added].Concat(iAssocFiles[EntityState.Modified]))
            {
                if (i.File != null)
                {
                    var o = i as IAssociateFileOptional;
                    if (o == null)
                    {
                        ((IAssociateFileRequired)i).StoreFile();
                    }
                    else
                    {
                        o.StoreFile();
                    }
                    i.File = null;
                }
            }
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
