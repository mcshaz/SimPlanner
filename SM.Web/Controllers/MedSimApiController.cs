using System.Linq;
using System.Web.Http;
using Breeze.ContextProvider;
using Breeze.WebApi2;
using SM.DataAccess;
using Newtonsoft.Json.Linq;
using Breeze.ContextProvider.EF6;

namespace SM.Web.Controllers
{
    [BreezeController]
    public class MedSimApiController : ApiController
    {
        // Todo: inject via an interface rather than "new" the concrete class
        readonly EFContextProvider<MedSimDbContext> _repository = new EFContextProvider<MedSimDbContext>();

        [HttpGet]
        public string Metadata()
        {
            //todo when database finalised - refactor to static file
            return _repository.Metadata();
        }

        [HttpPost]
        public SaveResult SaveChanges(JObject saveBundle)
        {
            return _repository.SaveChanges(saveBundle);
        }

        [HttpGet]
		public IQueryable<Participant> Participants(){ return _repository.Context.Users; } 
        [HttpGet]
		public IQueryable<Country> Countries(){ return _repository.Context.Countries; } 
        [HttpGet]
		public IQueryable<Department> Departments(){ return _repository.Context.Departments; } 
        [HttpGet]
		public IQueryable<ScenarioRoleDescription> SenarioRoles(){ return _repository.Context.SenarioRoles; } 
        [HttpGet]
		public IQueryable<Institution> Hospitals(){ return _repository.Context.Institutions; } 
        [HttpGet]
		public IQueryable<Manequin> Manequins(){ return _repository.Context.Manequins; } 
        [HttpGet]
		public IQueryable<ProfessionalRole> ProfessionalRoles(){ return _repository.Context.ProfessionalRoles; } 
        [HttpGet]
		public IQueryable<Scenario> Scenarios(){ return _repository.Context.Scenarios; } 
        [HttpGet]
		public IQueryable<ScenarioResource> ScenarioResources(){ return _repository.Context.ScenarioResources; } 
        [HttpGet]
		public IQueryable<Course> Courses()
        {
            return _repository.Context.Courses;
        } 
        [HttpGet]
		public IQueryable<CourseType> CourseTypes()
        {
            return _repository.Context.CourseTypes;
        }

        [HttpGet]
        public LookupBundle Lookups()
        {
            return new LookupBundle
            {
                CourseTypes = _repository.Context.CourseTypes.ToList(),
                //TODO - get user institution (add DTO)
                UserInstitution = _repository.Context.Institutions.Include("Departments").First(),
                ProfessionalRoles = _repository.Context.ProfessionalRoles.ToList()
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