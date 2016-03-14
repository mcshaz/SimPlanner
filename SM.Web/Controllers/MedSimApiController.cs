using System.Linq;
using System.Web.Http;
using Breeze.WebApi2;
using Newtonsoft.Json.Linq;
using Microsoft.AspNet.Identity;
using System.Security.Claims;
using System;
using SM.Dto;
using Breeze.ContextProvider;
using System.Web.Http.OData.Query;
using SM.DTOs.Maps;
using SM.Web.Controllers.Helpers;

namespace SM.Web.Controllers
{
    [BreezeController]
    [Authorize]
    public class MedSimApiController : ApiController
    {
        // Todo: inject via an interface rather than "new" the concrete class
        private MedSimDtoRepository _repository; // not populating in constructor as I believe this may be too early
        private MedSimDtoRepository Repo
        {
            get
            {
                return _repository ?? (_repository = new MedSimDtoRepository(
                    Guid.Parse(User.Identity.GetUserId()), 
                    () => ((ClaimsIdentity)User.Identity).Claims
                        .Where(c => c.Type == ClaimTypes.Role)
                        .Select(c => c.Value)));
            }
        }


        [HttpPost]
        public SaveResult SaveChanges(JObject saveBundle)
        {
            return Repo.SaveChanges(saveBundle);
        }

        [HttpGet, EnableBreezeQuery]
		public IQueryable<ParticipantDto> Participants(ODataQueryOptions options)
        {
            var iso = new IncludeSelectOptions(options);
            return Repo.GetParticipants(iso.Includes, iso.Selects, IncludeSelectOptions.Seperator);
        }
        [HttpGet]
		public IQueryable<CountryDto> Countries(){ return Repo.Countries; } 
        [HttpGet]
		public IQueryable<DepartmentDto> Departments(){ return Repo.Departments; } 
        [HttpGet]
		public IQueryable<ScenarioRoleDescriptionDto> SenarioRoles(){ return Repo.SenarioRoles; } 
        [HttpGet]
		public IQueryable<InstitutionDto> Hospitals(){ return Repo.Institutions; } 
        [HttpGet]
		public IQueryable<ManequinDto> Manequins(){ return Repo.Manequins; } 
        [HttpGet]
		public IQueryable<ProfessionalRoleDto> ProfessionalRoles(){ return Repo.ProfessionalRoles; } 
        [HttpGet]
		public IQueryable<ScenarioDto> Scenarios()
        {
            return Repo.Scenarios;
        } 
        [HttpGet]
		public IQueryable<ScenarioResourceDto> ScenarioResources()
        {
            return Repo.ScenarioResources;
        } 
        [HttpGet, EnableBreezeQuery]
		public IQueryable<CourseDto> Courses(ODataQueryOptions options)
        {
            var iso = new IncludeSelectOptions(options);
            return Repo.GetCourses(iso.Includes, iso.Selects, IncludeSelectOptions.Seperator);
        }

        private class IncludeSelectOptions
        {
            public IncludeSelectOptions(ODataQueryOptions options)
            {
                var se = options.SelectExpand;
                if (se != null)
                {
                    if (se.RawExpand != null)
                    {
                        Includes =  se.RawExpand.Split(',');
                    }
                     if (se.RawSelect != null)
                    {
                        Selects = se.RawSelect.Split(',');
                    }
                }
                if (options.Filter != null)
                {
                    var anyAlls = FindAnyAllFilterOptions.GetPaths(options.Filter);
                    if (anyAlls.Any())
                    {
                        Includes = (Includes ?? new string[0]).Concat(anyAlls).ToArray();
                    }
                }
            }
            public readonly string[] Includes;
            public readonly string[] Selects;
            public const char Seperator = '/';
        }
        [HttpGet]
		public IQueryable<CourseTypeDto> CourseTypes()
        {
            return Repo.GetCourseTypes();
        }

        [HttpGet]
        public LookupBundle Lookups()
        {
            return new LookupBundle
            {
                CourseTypes = Repo.GetCourseTypes().ToList(),
                Institutions = Repo.Institutions.ToList(),
                ProfessionalRoles = Repo.ProfessionalRoles.ToList()
            };
        }
        
        // Diagnostic
        [HttpGet]
        public string Ping()
        {
            return "pong";
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (_repository != null)
                {
                    _repository.Dispose();
                }
            }
            base.Dispose(disposing);
        }
    }
}