using System.Linq;
using System.Web.Http;
using Breeze.WebApi2;
using Newtonsoft.Json.Linq;
using Microsoft.AspNet.Identity;
using System.Security.Claims;
using System;
using SM.Dto;
using Breeze.ContextProvider;

namespace SM.Web.Controllers
{
    [BreezeController]
    [Authorize]
    public class MedSimApiController : ApiController
    {
        // Todo: inject via an interface rather than "new" the concrete class
        readonly MedSimDtoRepository _repository;

        MedSimApiController() 
        {
            _repository = new MedSimDtoRepository(Guid.Parse(User.Identity.GetUserId()), () => ((ClaimsIdentity)User.Identity).Claims
                .Where(c => c.Type == ClaimTypes.Role)
                .Select(c => c.Value));
        }

        [HttpPost]
        public SaveResult SaveChanges(JObject saveBundle)
        {
            return _repository.SaveChanges(saveBundle);
        }

        [HttpGet]
		public IQueryable<ParticipantDto> Participants(){ return _repository.Participants; } 
        [HttpGet]
		public IQueryable<CountryDto> Countries(){ return _repository.Countries; } 
        [HttpGet]
		public IQueryable<DepartmentDto> Departments(){ return _repository.Departments; } 
        [HttpGet]
		public IQueryable<ScenarioRoleDescriptionDto> SenarioRoles(){ return _repository.SenarioRoles; } 
        [HttpGet]
		public IQueryable<InstitutionDto> Hospitals(){ return _repository.Institutions; } 
        [HttpGet]
		public IQueryable<ManequinDto> Manequins(){ return _repository.Manequins; } 
        [HttpGet]
		public IQueryable<ProfessionalRoleDto> ProfessionalRoles(){ return _repository.ProfessionalRoles; } 
        [HttpGet]
		public IQueryable<ScenarioDto> Scenarios()
        {
            return _repository.Scenarios;
        } 
        [HttpGet]
		public IQueryable<ScenarioResourceDto> ScenarioResources()
        {
            return _repository.ScenarioResources;
        } 
        [HttpGet]
		public IQueryable<CourseDto> Courses()
        {
            return _repository.Courses;
        } 
        [HttpGet]
		public IQueryable<CourseTypeDto> CourseTypes()
        {
            return _repository.CourseTypes;
        }

        [HttpGet]
        public LookupBundle Lookups()
        {
            return new LookupBundle
            {
                CourseTypes = _repository.CourseTypes.ToList(),
                //TODO - get user institution (add DTO)
                Institutions = _repository.Institutions.ToList(),
                ProfessionalRoles = _repository.ProfessionalRoles.ToList()
            };
        }
        
        // Diagnostic
        [HttpGet]
        public string Ping()
        {
            return "pong";
        }
    }
}