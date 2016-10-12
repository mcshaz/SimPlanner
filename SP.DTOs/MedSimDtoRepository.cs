using Breeze.ContextProvider;
using Breeze.ContextProvider.EF6;
using Newtonsoft.Json.Linq;
using SP.DataAccess;
using SP.Dto.Maps;
using SP.Dto.ProcessBreezeRequests;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;

namespace SP.Dto
{
    public class MedSimDtoRepository : IDisposable
    {
        private readonly EFContextProvider<MedSimDbContext> _contextProvider;
        private readonly ValidateMedSim _validationHelper;
        private readonly IPrincipal _user;

        internal MedSimDbContext Context
        {
            get { return _contextProvider.Context; }
        }

        public MedSimDtoRepository(IPrincipal user)
        {

            _contextProvider = new EFContextProvider<MedSimDbContext>(/*user , allowedRoles: new[] { RoleConstants.AccessAllData } */);
            _validationHelper = new ValidateMedSim(user);
            _contextProvider.BeforeSaveEntitiesDelegate += MapToServerTypes; 
            _contextProvider.AfterSaveEntitiesDelegate += _validationHelper.PostValidation;
            _user = user;
        }

        Dictionary<Type, List<EntityInfo>> MapToServerTypes(Dictionary<Type, List<EntityInfo>> dtos)
        {
            _validationHelper.Validate(dtos);
            var returnVar = new Dictionary<Type, List<EntityInfo>>();
            foreach (var kv in dtos)
            {
                Type serverType = MapperConfig.GetServerModelType(kv.Key);
                var mapper = MapperConfig.GetFromDtoMapper(kv.Key);
                var vals = kv.Value.Select(d => _contextProvider.CreateEntityInfo(mapper(d.Entity), d.EntityState)).ToList();
                returnVar.Add(serverType, vals);
            }
            return returnVar;
        }

        public static string GetEdmxMetadata()
        {
            return new EFContextProvider<MedSimDtoContextPretender>().Metadata();
        }

        public IQueryable<ActivityDto> Activities(string[] includes, string[] selects, char sepChar)
        {
            return Context.Activities.ProjectToDto<Activity, ActivityDto>(includes, selects, sepChar);
        }

        public SaveResult SaveChanges(JObject saveBundle)
        {
            // save with server model's "real" contextProvider
            var returnVar = _contextProvider.SaveChanges(saveBundle);
            Remap(returnVar);
            return returnVar;
        }

        static void Remap(SaveResult result)
        {
            result.Entities = result.Entities.Select(o=> MapperConfig.GetToDtoLambda(MapperConfig.GetDtoType(o.GetType())).Compile().DynamicInvoke(o)).ToList();
        }

        public IQueryable<CourseFormatDto> GetCourseFormats(string[] includes, string[] selects, char sepChar)
        {
            return Context.CourseFormats.ProjectToDto<CourseFormat, CourseFormatDto>(includes, selects, sepChar);
        }

        public IQueryable<CourseDayDto> GetCourseDays(string[] includes, string[] selects, char sepChar)
        {
            return Context.CourseDays.ProjectToDto<CourseDay, CourseDayDto>(includes, selects, sepChar);
        }

        public IQueryable<CourseSlotDto> GetCourseSlots(string[] includes, string[] selects, char sepChar)
        {
            return Context.CourseSlots.ProjectToDto<CourseSlot, CourseSlotDto>(includes, selects, sepChar);
        }

        [Serializable]
        public class UnknownPropertyException : Exception
        {
            public UnknownPropertyException() : base() { }
            public UnknownPropertyException(string msg) : base(msg) { }
        }

        //https://github.com/AutoMapper/AutoMapper/wiki/Queryable-Extensions
        public IQueryable<InstitutionDto> GetInstitutions(string[] includes = null, string[] selects = null, char sepChar = '.')
        {
            IQueryable<Institution> returnVar = Context.Institutions;
            if (!_user.IsInRole(RoleConstants.AccessAllData))
            {
                returnVar = returnVar.Where(i => i.Departments.Any(d => d.Participants.Any(p => p.UserName == _user.Identity.Name)));
            }
            //currently allowing users to view all departmetns within their institution - but only edit thseir department
            return returnVar.ProjectToDto<Institution,InstitutionDto>(includes, selects,sepChar);
        }

        public IQueryable<ParticipantDto> GetParticipants(string[] includes = null, string[] selects = null, char sepChar = '.')
        {
            return Context.Users.ProjectToDto<Participant, ParticipantDto>(includes, selects, sepChar);
            /*
            if (include.Length > 0)
            {
                return Context.Courses.Project<Course,CourseDto>(parameters: null, membersToExpand: include);
            }
            */
            //return Context.Courses.Include("")
            //filteredQuery.Select(CourseMaps.MapFromDomain).ToList().AsQueryable();
        }

        public IQueryable<CultureDto> GetCultures(string[] includes, string[] selects, char sepChar)
        {
            return Context.Cultures.ProjectToDto<Culture, CultureDto>(includes, selects, sepChar);
        }

        public IQueryable<DepartmentDto> Departments { get { return Context.Departments.ProjectToDto<Department,DepartmentDto>(); } }

        public IQueryable<HotDrinkDto> HotDrinks { get { return Context.HotDrinks.ProjectToDto<HotDrink, HotDrinkDto>(); } }

        public IQueryable<CourseTypeDto> GetCourseTypes(string[] includes = null, string[] selects = null, char sepChar = '.')
        {
            return Context.CourseTypes.ProjectToDto<CourseType, CourseTypeDto>(includes, selects, sepChar);
        }

        public IQueryable<ScenarioDto> GetScenarios(string[] includes, string[] selects, char sepChar = '.')
        {
            return Context.Scenarios.ProjectToDto<Scenario, ScenarioDto>(includes, selects, sepChar);
        }

        public IQueryable<ManikinServiceDto> GetManikinServices(string[] includes, string[] selects, char sepChar = '.')
        {
            return Context.ManikinServices.ProjectToDto<ManikinService, ManikinServiceDto>(includes, selects, sepChar);

        }

        public IQueryable<FacultyScenarioRoleDto> SenarioRoles { get { return Context.FacultyScenarioRoles.ProjectToDto<FacultyScenarioRole,FacultyScenarioRoleDto>(); } }

        public IQueryable<InstitutionDto> Hospitals { get { return Context.Institutions.ProjectToDto<Institution,InstitutionDto>(); } }

        public IQueryable<ManikinDto> Manikins
        {
            get
            {
                IQueryable<Manikin> returnVar = Context.Manikins.Where(m => m.Department.Institution.Departments.Any(d => d.Participants.Any(p => p.UserName == _user.Identity.Name)));
                return returnVar.ProjectToDto<Manikin, ManikinDto>();
            }
        }

        public IQueryable<ProfessionalRoleDto> ProfessionalRoles { get { return Context.ProfessionalRoles.ProjectToDto<ProfessionalRole,ProfessionalRoleDto>(); } }

        public IQueryable<ScenarioResourceDto> ScenarioResources { get { return Context.ScenarioResources.ProjectToDto<ScenarioResource,ScenarioResourceDto>(); } }

        public IQueryable<ManikinManufacturerDto> ManikinManufacturers
        {
            get
            {
                return Context.ManikinManufacturers.ProjectToDto<ManikinManufacturer, ManikinManufacturerDto>(includes: new[] { "ManikinModels" });
            }
        }

        public IQueryable<ManikinModelDto> ManikinModels { get { return Context.ManikinModels.ProjectToDto<ManikinModel, ManikinModelDto>(); } }

        public IQueryable<ProfessionalRoleInstitutionDto> ProfessionalRoleInstitutions { get { return Context.ProfessionalRoleInstitutions.ProjectToDto<ProfessionalRoleInstitution, ProfessionalRoleInstitutionDto>(); } }

        public IQueryable<FacultyScenarioRoleDto> FacultyScenarioRoles { get {
                return Context.FacultyScenarioRoles.ProjectToDto<FacultyScenarioRole, FacultyScenarioRoleDto>();
        } }

        public IQueryable<CourseTypeScenarioRoleDto> CourseTypeScenarioRoles
        {
            get
            {
                return Context.CourseTypeScenarioRoles.ProjectToDto<CourseTypeScenarioRole, CourseTypeScenarioRoleDto>();
            }
        }

        public IQueryable<CourseActivityDto> GetCourseActivities(string[] includes = null, string[] selects = null, char sepChar = '.') {
            return Context.CourseActivities.ProjectToDto<CourseActivity, CourseActivityDto>(includes, selects, sepChar);
        }


        //might eventually run the visitor like so: http://stackoverflow.com/questions/18879779/select-and-expand-break-odataqueryoptions-how-to-fix
        public IQueryable<CourseDto> GetCourses(string[] includes = null, string[] selects = null, char sepChar = '.')
        {
            return Context.Courses.ProjectToDto<Course,CourseDto>(includes, selects, sepChar);
            /*
            if (include.Length > 0)
            {
                return Context.Courses.Project<Course,CourseDto>(parameters: null, membersToExpand: include);
            }
            */
            //return Context.Courses.Include("")
            //filteredQuery.Select(CourseMaps.MapFromDomain).ToList().AsQueryable();
        }

        public IQueryable<CourseTypeDto> GetCourseTypes()
        {
            return Context.CourseTypes.ProjectToDto<CourseType, CourseTypeDto>(includes: new[] { "CourseFormats" });
        }

        #region IDisposable

        bool _disposed;

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        ~MedSimDtoRepository()
        {
            Dispose(false);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (_disposed)
                return;

            if (disposing)
            {
                // free other managed objects that implement
                // IDisposable only
                _contextProvider.Context.Dispose();
                //if (_validationHelper != null) {
                _validationHelper.Dispose();
                //}
            }

            // release any unmanaged objects
            // set the object references to null

            _disposed = true;
        }
        #endregion //IDisposable
    }
}
