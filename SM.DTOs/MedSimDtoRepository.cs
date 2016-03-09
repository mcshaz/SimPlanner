using Breeze.ContextProvider;
using Breeze.ContextProvider.EF6;
using Newtonsoft.Json.Linq;
using SM.DataAccess;
using SM.DTOs.Maps;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SM.Dto
{
    public class MedSimDtoRepository : IDisposable
    {
        private readonly EFContextProvider<MedSimDbContext> _contextProvider;
        private readonly Guid _userId;
        private readonly Func<IEnumerable<string>> _getUserRoles; // ? eventually async
        private IEnumerable<string> _userRoles;
        private IEnumerable<string> UserRoles
        {
            get
            {
                if (_userRoles == null)
                {
                    _userRoles = _getUserRoles();
                }
                return _userRoles;
            }
        }

        private MedSimDbContext Context
        {
            get { return _contextProvider.Context; }
        }

        public MedSimDtoRepository(Guid userId, Func<IEnumerable<string>> getUserRoles)
        {
            _userId = userId;
            _getUserRoles = getUserRoles;
            _contextProvider = new EFContextProvider<MedSimDbContext>();
            //_entitySaveGuard = new EntitySaveGuard();
            //_contextProvider.BeforeSaveEntityDelegate += _entitySaveGuard.BeforeSaveEntity;
        }

        public static string GetMetadata()
        {
            return new EFContextProvider<MedSimDtoContextPretender>().Metadata();
        }

        public SaveResult SaveChanges(JObject saveBundle)
        {
            // Todo: transform entities in saveBundle from DTO form into server-model form.
            // At least change the namespace from Northwind.DtoModels to Northwind.Models :-)
            // will fail until then

            // save with server model's "real" contextProvider
            return _contextProvider.SaveChanges(saveBundle);
        }

        //https://github.com/AutoMapper/AutoMapper/wiki/Queryable-Extensions
        public IQueryable<InstitutionDto> Institutions
        {
            get
            {
                IQueryable<Institution> returnVar = Context.Institutions;
                if (!UserRoles.Contains(RoleConstants.AccessAllData))
                {
                    returnVar = returnVar.Where(i => i.Departments.Any(d => d.Participants.Any(p => p.Id == _userId)));
                }
                //currently allowing users to view all departmetns within their institution - but only edit thseir department
                return returnVar.Project<Institution,InstitutionDto>(new MapperConfig.IncludeSelectOptions(new[] { "Departments.Rooms","ProfessionalRoles" }));

            }
        }

        public IQueryable<ParticipantDto> Participants { get { return Context.Users.Project<Participant,ParticipantDto>(); } }

        public IQueryable<CountryDto> Countries { get { return Context.Countries.Project<Country,CountryDto>(); } }

        public IQueryable<DepartmentDto> Departments { get { return Context.Departments.Project<Department,DepartmentDto>(); } }

        public IQueryable<ScenarioRoleDescriptionDto> SenarioRoles { get { return Context.SenarioRoles.Project<ScenarioRoleDescription,ScenarioRoleDescriptionDto>(); } }

        public IQueryable<InstitutionDto> Hospitals { get { return Context.Institutions.Project<Institution,InstitutionDto>(); } }

        public IQueryable<ManequinDto> Manequins { get { return Context.Manequins.Project<Manequin,ManequinDto>(); } }

        public IQueryable<ProfessionalRoleDto> ProfessionalRoles { get { return Context.ProfessionalRoles.Project<ProfessionalRole,ProfessionalRoleDto>(); } }

        public IQueryable<ScenarioDto> Scenarios { get { return Context.Scenarios.Project<Scenario,ScenarioDto>(); } }

        public IQueryable<ScenarioResourceDto> ScenarioResources { get { return Context.ScenarioResources.Project<ScenarioResource,ScenarioResourceDto>(); } }


        //might eventually run the visitor like so: http://stackoverflow.com/questions/18879779/select-and-expand-break-odataqueryoptions-how-to-fix
        public IQueryable<CourseDto> GetCourses(MapperConfig.IncludeSelectOptions options = null)
        {
            return Context.Courses.Project<Course,CourseDto>(options);
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
            return Context.CourseTypes.Project<CourseType, CourseTypeDto>();
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
            }

            // release any unmanaged objects
            // set the object references to null

            _disposed = true;
        }
        #endregion //IDisposable
    }
}
