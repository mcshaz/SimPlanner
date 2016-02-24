﻿using Breeze.ContextProvider;
using Breeze.ContextProvider.EF6;
using Newtonsoft.Json.Linq;
using SM.DataAccess;
using System;
using System.Collections.Generic;
using System.Linq;

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

        public IQueryable<InstitutionDto> Institutions
        {
            get
            {
                if (UserRoles.Contains(RoleConstants.AccessAllData))
                {
                    return Context.Institutions.Select(InstitutionDto.mapFromRepo);
                }
                //currently allowing users to view all departmetns within their institution - but only edit thseir department
                return Context.Institutions
                    .Where(i => i.Departments.Any(d => d.Participants.Any(p => p.Id == _userId)))
                    .Select(InstitutionDto.mapFromRepo);

            }
        }

        public IQueryable<ParticipantDto> Participants { get { return Context.Users.Select(ParticipantDto.mapFromRepo); } }

        public IQueryable<CountryDto> Countries { get { return Context.Countries.Select(CountryDto.mapFromRepo); } }

        public IQueryable<DepartmentDto> Departments { get { return Context.Departments.Select(DepartmentDto.mapFromRepo); } }

        public IQueryable<ScenarioRoleDescriptionDto> SenarioRoles { get { return Context.SenarioRoles.Select(ScenarioRoleDescriptionDto.mapFromRepo); } }

        public IQueryable<InstitutionDto> Hospitals { get { return Context.Institutions.Select(InstitutionDto.mapFromRepo); } }

        public IQueryable<ManequinDto> Manequins { get { return Context.Manequins.Select(ManequinDto.mapFromRepo); } }

        public IQueryable<ProfessionalRoleDto> ProfessionalRoles { get { return Context.ProfessionalRoles.Select(ProfessionalRoleDto.mapFromRepo); } }

        public IQueryable<ScenarioDto> Scenarios { get { return Context.Scenarios.Select(ScenarioDto.mapFromRepo); } }

        public IQueryable<ScenarioResourceDto> ScenarioResources { get { return Context.ScenarioResources.Select(ScenarioResourceDto.mapFromRepo); } }

        public IQueryable<CourseDto> Courses { get { return Context.Courses.Select(CourseDto.mapFromRepo); } }

        public IQueryable<CourseTypeDto> CourseTypes
        {
            get
            {
                return Context.CourseTypes.Select(CourseTypeDto.mapFromRepo);
            }
        }

    }
}
