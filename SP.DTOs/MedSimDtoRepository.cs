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
        private readonly CurrentUser _currentUser;
        private readonly IPrincipal _user;

        public MedSimDbContext Context
        {
            get { return _contextProvider.Context; }
        }

        public MedSimDtoRepository(IPrincipal user, MedSimDbContext validationContext = null)
        {

            _contextProvider = new EFContextProvider<MedSimDbContext>(/*user , allowedRoles: new[] { RoleConstants.AccessAllData } */);
            _currentUser = new CurrentUser(user, validationContext);
            _validationHelper = new ValidateMedSim(_currentUser);
            _contextProvider.BeforeSaveEntitiesDelegate += BeforeSaveEntities; 
            _contextProvider.AfterSaveEntitiesDelegate += _validationHelper.AfterSave;
            _user = user;
        }

        Dictionary<Type, List<EntityInfo>> BeforeSaveEntities(Dictionary<Type, List<EntityInfo>> dtos)
        {
            var errors = _validationHelper.ValidateDto(dtos);
            if (errors.Any())
            {
                throw new EntityErrorsException(errors);
            }
            return ValidateMedSim.MapDtoToServerType(dtos);
        }

        public static string GetEdmxMetadata()
        {
            return new EFContextProvider<MedSimDtoContextPretender>().Metadata();
        }

        public IQueryable<ActivityDto> Activities(string[] includes, string[] selects, char sepChar)
        {
            return Context.Activities.ProjectToDto<Activity, ActivityDto>(_currentUser, includes, selects, sepChar);
        }

        public SaveResult SaveChanges(JObject saveBundle)
        {
            // save with server model's "real" contextProvider
            var returnVar = _contextProvider.SaveChanges(saveBundle);
            Remap(returnVar);
            return returnVar;
        }

        void Remap(SaveResult result)
        {
            result.Entities = result.Entities.Select(o=> {
                var treeTop = MapperConfig.GetToDtoLambda(MapperConfig.GetDtoType(o.GetType()), _currentUser);
                //possibly Assert(treeTop.WhereExpression.Compile().DynamicInvoke(o);)
                //possible bottleneck - could cast to all the different types
                return treeTop.SelectExpression.Compile().DynamicInvoke(o);
            }).ToList();
        }

        public IQueryable<CourseFormatDto> GetCourseFormats(string[] includes, string[] selects, char sepChar)
        {
            return Context.CourseFormats.ProjectToDto<CourseFormat, CourseFormatDto>(_currentUser, includes, selects, sepChar);
        }

        public IQueryable<CourseDayDto> GetCourseDays(string[] includes, string[] selects, char sepChar)
        {
            return Context.CourseDays.ProjectToDto<CourseDay, CourseDayDto>(_currentUser, includes, selects, sepChar);
        }

        public IQueryable<CourseSlotDto> GetCourseSlots(string[] includes, string[] selects, char sepChar)
        {
            return Context.CourseSlots.ProjectToDto<CourseSlot, CourseSlotDto>(_currentUser, includes, selects, sepChar);
        }

        public IQueryable<ManikinDto> GetManikins(string[] includes, string[] selects, char sepChar)
        {
            return Context.Manikins.ProjectToDto<Manikin, ManikinDto>(_currentUser, includes, selects, sepChar);
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
            return returnVar.ProjectToDto<Institution,InstitutionDto>(_currentUser, includes, selects,sepChar);
        }

        public IQueryable<ParticipantDto> GetParticipants(string[] includes = null, string[] selects = null, char sepChar = '.')
        {
            return Context.Users.ProjectToDto<Participant, ParticipantDto>(_currentUser, includes, selects, sepChar);
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
            return Context.Cultures.ProjectToDto<Culture, CultureDto>(_currentUser, includes, selects, sepChar);
        }

        public IQueryable<DepartmentDto> Departments { get { return Context.Departments.ProjectToDto<Department,DepartmentDto>(_currentUser); } }

        public IQueryable<HotDrinkDto> HotDrinks { get { return Context.HotDrinks.ProjectToDto<HotDrink, HotDrinkDto>(_currentUser); } }

        public IQueryable<CourseTypeDto> GetCourseTypes(string[] includes = null, string[] selects = null, char sepChar = '.')
        {
            return Context.CourseTypes.ProjectToDto<CourseType, CourseTypeDto>(_currentUser, includes, selects, sepChar);
        }

        public IQueryable<ScenarioDto> GetScenarios(string[] includes, string[] selects, char sepChar = '.')
        {
            return Context.Scenarios.ProjectToDto<Scenario, ScenarioDto>(_currentUser, includes, selects, sepChar);
        }

        public IQueryable<ManikinServiceDto> GetManikinServices(string[] includes, string[] selects, char sepChar = '.')
        {
            return Context.ManikinServices.ProjectToDto<ManikinService, ManikinServiceDto>(_currentUser, includes, selects, sepChar);

        }

        public IQueryable<FacultyScenarioRoleDto> SenarioRoles { get { return Context.FacultyScenarioRoles.ProjectToDto<FacultyScenarioRole,FacultyScenarioRoleDto>(_currentUser); } }

        public IQueryable<InstitutionDto> Hospitals { get { return Context.Institutions.ProjectToDto<Institution,InstitutionDto>(_currentUser); } }

        public IQueryable<ManikinDto> Manikins
        {
            get
            {
                IQueryable<Manikin> returnVar = Context.Manikins.Where(m => m.Department.Institution.Departments.Any(d => d.Participants.Any(p => p.UserName == _user.Identity.Name)));
                return returnVar.ProjectToDto<Manikin, ManikinDto>(_currentUser);
            }
        }

        public IQueryable<ProfessionalRoleDto> ProfessionalRoles { get { return Context.ProfessionalRoles.ProjectToDto<ProfessionalRole,ProfessionalRoleDto>(_currentUser); } }

        public IQueryable<ScenarioResourceDto> ScenarioResources { get { return Context.ScenarioResources.ProjectToDto<ScenarioResource,ScenarioResourceDto>(_currentUser); } }

        public IQueryable<ManikinManufacturerDto> ManikinManufacturers
        {
            get
            {
                return Context.ManikinManufacturers.ProjectToDto<ManikinManufacturer, ManikinManufacturerDto>(_currentUser, includes: new[] { "ManikinModels" });
            }
        }

        public IQueryable<ManikinModelDto> ManikinModels { get { return Context.ManikinModels.ProjectToDto<ManikinModel, ManikinModelDto>(_currentUser); } }

        public IQueryable<ProfessionalRoleInstitutionDto> ProfessionalRoleInstitutions { get { return Context.ProfessionalRoleInstitutions.ProjectToDto<ProfessionalRoleInstitution, ProfessionalRoleInstitutionDto>(_currentUser); } }

        public IQueryable<FacultyScenarioRoleDto> FacultyScenarioRoles { get {
                return Context.FacultyScenarioRoles.ProjectToDto<FacultyScenarioRole, FacultyScenarioRoleDto>(_currentUser);
        } }

        public IQueryable<CourseTypeScenarioRoleDto> CourseTypeScenarioRoles
        {
            get
            {
                return Context.CourseTypeScenarioRoles.ProjectToDto<CourseTypeScenarioRole, CourseTypeScenarioRoleDto>(_currentUser);
            }
        }

        public IQueryable<CourseActivityDto> GetCourseActivities(string[] includes = null, string[] selects = null, char sepChar = '.') {
            return Context.CourseActivities.ProjectToDto<CourseActivity, CourseActivityDto>(_currentUser, includes, selects, sepChar);
        }


        //might eventually run the visitor like so: http://stackoverflow.com/questions/18879779/select-and-expand-break-odataqueryoptions-how-to-fix
        public IQueryable<CourseDto> GetCourses(string[] includes = null, string[] selects = null, char sepChar = '.')
        {
            return Context.Courses.ProjectToDto<Course,CourseDto>(_currentUser, includes, selects, sepChar);
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
            return Context.CourseTypes.ProjectToDto<CourseType, CourseTypeDto>(_currentUser, includes: new[] { "CourseFormats" });
        }

        public int IncrementEmail(Guid courseId)
        {
            return (Context.Database.ExecuteSqlCommand($"UPDATE Courses SET EmailSequence = EmailSequence + 1 WHERE Id = {{guid '{courseId}'}}"));
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
                _currentUser.Dispose();
                //}
            }

            // release any unmanaged objects
            // set the object references to null

            _disposed = true;
        }
        #endregion //IDisposable
    }
}
