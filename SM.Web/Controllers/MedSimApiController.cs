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

        [HttpGet]
		public IQueryable<ParticipantDto> Participants()
        {
            return Repo.Participants;
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
           return Repo.GetCourses(GetIncludes(options));
        }
        private MapperConfig.IncludeSelectOptions GetIncludes(ODataQueryOptions options)
        {
            //a hack to use breeze instead of learning .net odata implementation
            //http://www.asp.net/web-api/overview/odata-support-in-aspnet-web-api/odata-v4/create-an-odata-v4-endpoint
            //SelectExpand.RawExpand = "CourseParticipants" RawSelect= null;
            //SelectExpand = null
            var se = options.SelectExpand;
            if (se != null)
            {
                return new MapperConfig.IncludeSelectOptions((se.RawExpand==null)? null :se.RawExpand.Split(','), (se.RawSelect == null) ? null : se.RawSelect.Split(','), '/');
            }
            return null;
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