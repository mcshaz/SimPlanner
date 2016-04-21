using Breeze.ContextProvider;
using Breeze.ContextProvider.EF6;
using Newtonsoft.Json.Linq;
using SM.DataAccess;
using SM.Dto.Maps;
using SM.DTOs.ProcessBreezeRequests;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Security.Principal;

namespace SM.Dto
{
    public class MedSimDtoRepository : IDisposable
    {
        private readonly EFContextProvider<MedSimDbContext> _contextProvider;
        private readonly ValidateMedSim _validationHelper;
        private readonly IPrincipal _user;

        private MedSimDbContext Context
        {
            get { return _contextProvider.Context; }
        }

        public MedSimDtoRepository(IPrincipal user)
        {

            _contextProvider = new EFContextProvider<MedSimDbContext>(/*user , allowedRoles: new[] { RoleConstants.AccessAllData } */);
            _validationHelper = new ValidateMedSim(user);
            _contextProvider.BeforeSaveEntitiesDelegate += _validationHelper.Process;
            _user = user;
        }

        public static string GetEdmxMetadata()
        {
            return new EFContextProvider<MedSimDtoContextPretender>().Metadata();
        }

        public IQueryable<ActivityTeachingResourceDto> ActivityTeachingResources(string[] includes, string[] selects, char sepChar)
        {
            return Context.ActivityTeachingResources.Project<ActivityTeachingResource, ActivityTeachingResourceDto>(includes, selects, sepChar);
        }

        public SaveResult SaveChanges(JObject saveBundle)
        {
            // save with server model's "real" contextProvider
            MapFromDto(saveBundle);

            var returnVar = _contextProvider.SaveChanges(saveBundle);
            Remap(returnVar);
            return returnVar;
        }

        static void Remap(SaveResult result)
        {
            result.Entities = result.Entities.Select(o=> MapperConfig.GetLambda(o.GetType().Name, null,null,'.').Compile().DynamicInvoke(o)).ToList();
        }

        static void MapFromDto(JObject savebundle)
        {
            var dtoAssembly = typeof(ParticipantDto).Assembly;
            //da for data access
            var daNamespace = typeof(Participant).Namespace;
            const string entityAspectName = "entityAspect";
            const string entityTypeName = "entityTypeName";
            const string namespaceSep = ":#";

            foreach (JToken ent in savebundle["entities"])
            {
                JToken entityAspect = ent[entityAspectName];
                string dtoTypeString = (string)entityAspect[entityTypeName];
                int hashIdx = dtoTypeString.IndexOf(namespaceSep);
                string typeName = dtoTypeString.Remove(hashIdx);
                Type dtoType = dtoAssembly.GetType(dtoTypeString.Substring(hashIdx + namespaceSep.Length) + '.' + typeName);
                List<string> props = dtoType
                    .GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.SetProperty)
                    .Select(pi => pi.Name).ToList();
                props.Add(entityAspectName);
                var unknownProps = ent.Select(e => ((JProperty)e).Name).Except(props);
                if (unknownProps.Any())
                {
                    throw new UnknownPropertyException(string.Join(";", unknownProps));
                }
                string daTypeName = typeName.Remove(typeName.Length - 3); //very application specific - remove Dto from the end of the type name
                entityAspect[entityTypeName] = JToken.FromObject(daTypeName + namespaceSep + daNamespace);
            }
        }

        public IQueryable<CourseFormatDto> GetCourseFormats(string[] includes, string[] selects, char sepChar)
        {
            return Context.CourseFormats.Project<CourseFormat, CourseFormatDto>(includes, selects, sepChar);
        }

        public IQueryable<CourseSlotDto> GetCourseSlots(string[] includes, string[] selects, char sepChar)
        {
            return Context.CourseSlots.Project<CourseSlot, CourseSlotDto>(includes, selects, sepChar);
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
            return returnVar.Project<Institution,InstitutionDto>(includes, selects,sepChar);
        }

        public IQueryable<ParticipantDto> GetParticipants(string[] includes = null, string[] selects = null, char sepChar = '.')
        {
            return Context.Users.Project<Participant, ParticipantDto>(includes, selects, sepChar);
            /*
            if (include.Length > 0)
            {
                return Context.Courses.Project<Course,CourseDto>(parameters: null, membersToExpand: include);
            }
            */
            //return Context.Courses.Include("")
            //filteredQuery.Select(CourseMaps.mapFromRepo).ToList().AsQueryable();
        }

        public IQueryable<CultureDto> GetCultures(string[] includes, string[] selects, char sepChar)
        {
            return Context.Cultures.Project<Culture, CultureDto>(includes, selects, sepChar);
        }

        public IQueryable<DepartmentDto> Departments { get { return Context.Departments.Project<Department,DepartmentDto>(); } }

        public IQueryable<CourseTypeDto> GetCourseTypes(string[] includes, string[] selects, char sepChar)
        {
            return Context.CourseTypes.Project<CourseType, CourseTypeDto>(includes, selects, sepChar);
        }

        public IQueryable<FacultyScenarioRoleDto> SenarioRoles { get { return Context.FacultyScenarioRoles.Project<FacultyScenarioRole,FacultyScenarioRoleDto>(); } }

        public IQueryable<InstitutionDto> Hospitals { get { return Context.Institutions.Project<Institution,InstitutionDto>(); } }

        public IQueryable<ManequinDto> Manequins
        {
            get
            {
                IQueryable<Manequin> returnVar = Context.Manequins.Where(m => m.Department.Institution.Departments.Any(d => d.Participants.Any(p => p.UserName == _user.Identity.Name)));
                return returnVar.Project<Manequin, ManequinDto>();
            }
        }

        public IQueryable<ProfessionalRoleDto> ProfessionalRoles { get { return Context.ProfessionalRoles.Project<ProfessionalRole,ProfessionalRoleDto>(); } }

        public IQueryable<ScenarioDto> Scenarios { get { return Context.Scenarios.Project<Scenario,ScenarioDto>(); } }

        public IQueryable<ScenarioResourceDto> ScenarioResources { get { return Context.ScenarioResources.Project<ScenarioResource,ScenarioResourceDto>(); } }

        public IQueryable<ManequinManufacturerDto> ManequinManufacturers
        {
            get
            {
                return Context.ManequinManufacturers.Project<ManequinManufacturer, ManequinManufacturerDto>(includes: new[] { "ManequinModels" });
            }
        }

        public IQueryable<ManequinModelDto> ManequinModels { get { return Context.ManequinModels.Project<ManequinModel, ManequinModelDto>(); } }

        public IQueryable<ProfessionalRoleInstitutionDto> ProfessionalRoleInstitutions { get { return Context.ProfessionalRoleInstitutions.Project<ProfessionalRoleInstitution, ProfessionalRoleInstitutionDto>(); } }

        public IQueryable<FacultyScenarioRoleDto> FacultyScenarioRoles { get {
                return Context.FacultyScenarioRoles.Project<FacultyScenarioRole, FacultyScenarioRoleDto>();
        } }

        public IQueryable<CourseTypeScenarioRoleDto> CourseTypeScenarioRoles
        {
            get
            {
                return Context.CourseTypeScenarioRoles.Project<CourseTypeScenarioRole, CourseTypeScenarioRoleDto>();
            }
        }

        public IQueryable<CourseActivityDto> GetCourseActivities(string[] includes = null, string[] selects = null, char sepChar = '.') {
            return Context.CourseActivities.Project<CourseActivity, CourseActivityDto>(includes, selects, sepChar);
        }


        //might eventually run the visitor like so: http://stackoverflow.com/questions/18879779/select-and-expand-break-odataqueryoptions-how-to-fix
        public IQueryable<CourseDto> GetCourses(string[] includes = null, string[] selects = null, char sepChar = '.')
        {
            return Context.Courses.Project<Course,CourseDto>(includes, selects, sepChar);
            /*
            if (include.Length > 0)
            {
                return Context.Courses.Project<Course,CourseDto>(parameters: null, membersToExpand: include);
            }
            */
            //return Context.Courses.Include("")
            //filteredQuery.Select(CourseMaps.mapFromRepo).ToList().AsQueryable();
        }

        public IQueryable<CourseTypeDto> GetCourseTypes()
        {
            return Context.CourseTypes.Project<CourseType, CourseTypeDto>(includes: new[] { "CourseFormats" });
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
