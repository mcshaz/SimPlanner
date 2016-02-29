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
using System.Web.Http.OData;
using System.Net.Http;
using System.Web.Http.OData.Extensions;
using Microsoft.Data.OData.Query.SemanticAst;

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
        [HttpGet]
		public IQueryable<CourseDto> Courses(ODataQueryOptions options)
        {
            return Repo.GetCourses(GetAndStripIncludes(options));
        }
        private string[] GetAndStripIncludes(ODataQueryOptions options)
        {
            //a hack to use breeze instead of learning .net odata implementation
            //http://www.asp.net/web-api/overview/odata-support-in-aspnet-web-api/odata-v4/create-an-odata-v4-endpoint
            if (options.SelectExpand != null)
            {
                var se = options.SelectExpand;
                string[] returnVar = se.RawExpand.Split(',');

                //we'll deal with the expand, breeze deals with the 
                Request.ODataProperties().SelectExpandClause = new SelectExpandQueryOption(
                    se.RawSelect,null,se.Context).SelectExpandClause;

            }
            return null;
        }
        [HttpGet]
		public IQueryable<CourseTypeDto> CourseTypes()
        {
            return Repo.CourseTypes;
        }

        [HttpGet]
        public LookupBundle Lookups()
        {
            return new LookupBundle
            {
                CourseTypes = Repo.CourseTypes.ToList(),
                //Institutions = Repo.Institutions.ToList(),
                //ProfessionalRoles = Repo.ProfessionalRoles.ToList()
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