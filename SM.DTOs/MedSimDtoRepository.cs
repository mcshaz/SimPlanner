using AutoMapper.QueryableExtensions;
using Breeze.ContextProvider;
using Breeze.ContextProvider.EF6;
using Newtonsoft.Json.Linq;
using SM.DataAccess;
using SM.DTOs.Maps;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Web.Http.OData.Query;

namespace SM.Dto
{
    public class MedSimDtoRepository
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
                return returnVar.ProjectTo<InstitutionDto>(AutomapperConfig.GetConfig());

            }
        }

        public IQueryable<ParticipantDto> Participants { get { return Context.Users.ProjectTo<ParticipantDto>(AutomapperConfig.GetConfig()); } }

        public IQueryable<CountryDto> Countries { get { return Context.Countries.ProjectTo<CountryDto>(AutomapperConfig.GetConfig()); } }

        public IQueryable<DepartmentDto> Departments { get { return Context.Departments.ProjectTo<DepartmentDto>(AutomapperConfig.GetConfig()); } }

        public IQueryable<ScenarioRoleDescriptionDto> SenarioRoles { get { return Context.SenarioRoles.ProjectTo<ScenarioRoleDescriptionDto>(AutomapperConfig.GetConfig()); } }

        public IQueryable<InstitutionDto> Hospitals { get { return Context.Institutions.ProjectTo<InstitutionDto>(AutomapperConfig.GetConfig()); } }

        public IQueryable<ManequinDto> Manequins { get { return Context.Manequins.ProjectTo<ManequinDto>(AutomapperConfig.GetConfig()); } }

        public IQueryable<ProfessionalRoleDto> ProfessionalRoles { get { return Context.ProfessionalRoles.ProjectTo<ProfessionalRoleDto>(AutomapperConfig.GetConfig()); } }

        public IQueryable<ScenarioDto> Scenarios { get { return Context.Scenarios.ProjectTo<ScenarioDto>(AutomapperConfig.GetConfig()); } }

        public IQueryable<ScenarioResourceDto> ScenarioResources { get { return Context.ScenarioResources.ProjectTo<ScenarioResourceDto>(AutomapperConfig.GetConfig()); } }


        //might eventually run the visitor like so: http://stackoverflow.com/questions/18879779/select-and-expand-break-odataqueryoptions-how-to-fix
        public IQueryable<CourseDto> GetCourses(string[] include)
        {
            ValidateIncludes(include);
            return Context.Courses.Select(CourseMaps.mapFromRepo);
            /*
            if (include.Length > 0)
            {
                return Context.Courses.ProjectTo<CourseDto>(parameters: null, membersToExpand: include);
            }
            */
            //return Context.Courses.Include("")
            //filteredQuery.Select(CourseMaps.mapFromRepo).ToList().AsQueryable();
        } 

        public IQueryable<CourseTypeDto> CourseTypes
        {
            get
            {
                var mapper = AutomapperConfig.GetConfig().CreateMapper();
                return mapper.Map<IEnumerable<CourseTypeDto>>(Context.CourseTypes).AsQueryable();
            }
        }

        private static void ValidateIncludes(string[] includes)
        {
            if (includes.Any(i => i.Contains('.')))
            {
                throw new NotSupportedException("nested include properties are not supported");
            }
        }

    }
}
