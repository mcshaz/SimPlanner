using System.Linq;
using System.Web.Http;
using Breeze.WebApi2;
using Newtonsoft.Json.Linq;
using SP.Dto;
using Breeze.ContextProvider;
using System.Web.Http.OData.Query;
using SP.Web.Controllers.Helpers;
using System.Collections.Generic;
using System.Net.Http;
using Microsoft.AspNet.Identity.Owin;
using SP.Web.UserEmails;
using System.Net;
using System.Threading.Tasks;
using System;
using Microsoft.AspNet.Identity;

namespace SP.Web.Controllers
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
                return _repository ?? (_repository = new MedSimDtoRepository(User, Request.GetOwinContext().Get<DataAccess.MedSimDbContext>())
                {
                    AfterBookingChange = MailExtensions.SendBookingNotifications,
                    AfterNewUnapprovedUser = MailExtensions.SendNewUserRequest,
                    AfterUserApproved = MailExtensions.SendNewUserApproved,
                    AfterNewCourseParticipant = cps => MailExtensions.SendNewCourseParticipantNotifications(cps, _repository.Context, User),
                    AfterCourseDateChange = (courseId, oldDate) => {
                        var course = MailExtensions.GetCourseIncludes(_repository.Context)
                            .First(c=>c.Id == courseId);
                        if (oldDate.HasValue)
                        {
                            Task.Run(() => CreateParticipantEmails.SendCourseEmail(course, oldDate));
                        }
                        CreateParticipantEmails.RescheduleReadings(course, _repository.Context);
                    },
                    PasswordHasher = PasswordHasher
                });
            }
        }

        ApplicationUserManager _userManager;
        private string PasswordHasher(string plainTextPwd)
        {
            return (_userManager ?? (_userManager = Request.GetOwinContext().GetUserManager<ApplicationUserManager>()))
                .PasswordHasher.HashPassword(plainTextPwd);
        }

        [HttpPost]
        [AllowAnonymous]
        public SaveResult SaveChanges(JObject saveBundle)
        {
            return Repo.SaveChanges(saveBundle);
        }
        [HttpGet, EnableMappedBreezeQuery]
		public IQueryable<ActivityDto> Activities(ODataQueryOptions options)
        {
            var iso = new IncludeSelectOptions(options);
            return Repo.Activities(iso.Includes, iso.Selects, IncludeSelectOptions.Seperator);
        }
        [HttpGet, EnableMappedBreezeQuery]
		public IQueryable<ParticipantDto> Participants(ODataQueryOptions options)
        {
            var iso = new IncludeSelectOptions(options);
            return Repo.GetParticipants(iso.Includes, iso.Selects, IncludeSelectOptions.Seperator);
        }
        [HttpGet, EnableMappedBreezeQuery(MaxExpansionDepth = 3)]
        public IQueryable<CultureDto> Cultures(ODataQueryOptions options)
        {
            var iso = new IncludeSelectOptions(options);
            return Repo.GetCultures(iso.Includes, iso.Selects, IncludeSelectOptions.Seperator); ;
        }
        [HttpGet, EnableMappedBreezeQuery]
        public IQueryable<CourseActivityDto> CourseActivities(ODataQueryOptions options) {
            var iso = new IncludeSelectOptions(options);
            return Repo.GetCourseActivities(iso.Includes, iso.Selects, IncludeSelectOptions.Seperator);
        }
        [HttpGet, EnableMappedBreezeQuery]
        public IQueryable<ManikinServiceDto> ManikinServices(ODataQueryOptions options)
        {
            var iso = new IncludeSelectOptions(options);
            return Repo.GetManikinServices(iso.Includes, iso.Selects, IncludeSelectOptions.Seperator);
        }
        [HttpGet, EnableMappedBreezeQuery]
        [AllowAnonymous]
        public IQueryable<DepartmentDto> Departments(ODataQueryOptions options)
        {
            var iso = new IncludeSelectOptions(options);
            AllowedAnonIncludes(iso, "Institution/Culture");
            return Repo.GetDepartments(iso.Includes, iso.Selects, IncludeSelectOptions.Seperator);
        }

        [HttpGet]
        [AllowAnonymous]
        public IQueryable<HotDrinkDto> HotDrinks() { return Repo.HotDrinks; }

        [HttpGet, EnableMappedBreezeQuery(MaxExpansionDepth = 4)]
        [AllowAnonymous]
        public IQueryable<InstitutionDto> Institutions(ODataQueryOptions options)
        {
            var iso = new IncludeSelectOptions(options);
            AllowedAnonIncludes(iso, "Departments");
            return Repo.GetInstitutions(iso.Includes, iso.Selects, IncludeSelectOptions.Seperator);
        }
        [HttpGet]
		public IQueryable<FacultyScenarioRoleDto> SenarioRoles(){ return Repo.SenarioRoles; } 
        [HttpGet, EnableMappedBreezeQuery(MaxExpansionDepth = 4)]
		public IQueryable<ManikinDto> Manikins(ODataQueryOptions options)
        {
            var iso = new IncludeSelectOptions(options);
            return Repo.GetManikins(iso.Includes, iso.Selects, IncludeSelectOptions.Seperator);
        }
        [HttpGet]
        public IQueryable<ManikinModelDto> ManikinModels() { return Repo.ManikinModels; }
        [HttpGet]
        [AllowAnonymous]
		public IQueryable<ProfessionalRoleDto> ProfessionalRoles(ODataQueryOptions options)
        {
            var iso = new IncludeSelectOptions(options);
            return Repo.GetProfessionalRoles(iso.Includes, iso.Selects, IncludeSelectOptions.Seperator);
        }
        [HttpGet]
        public IQueryable<ProfessionalRoleInstitutionDto> ProfessionalRoleInstitutions() { return Repo.ProfessionalRoleInstitutions; }
        [HttpGet, EnableMappedBreezeQuery]
        public IQueryable<ScenarioDto> Scenarios(ODataQueryOptions options)
        {
            var iso = new IncludeSelectOptions(options);
            return Repo.GetScenarios(iso.Includes, iso.Selects, IncludeSelectOptions.Seperator);
        }

        [HttpGet, EnableMappedBreezeQuery]
        public IQueryable<CourseSlotDto> CourseSlots(ODataQueryOptions options)
        {
            var iso = new IncludeSelectOptions(options);
            return Repo.GetCourseSlots(iso.Includes, iso.Selects, IncludeSelectOptions.Seperator);
        }

        [HttpGet, EnableMappedBreezeQuery]
        public IQueryable<CourseFormatDto> CourseFormats(ODataQueryOptions options)
        {
            var iso = new IncludeSelectOptions(options);
            return Repo.GetCourseFormats(iso.Includes, iso.Selects, IncludeSelectOptions.Seperator);
        }

        [HttpGet, EnableMappedBreezeQuery]
        public IQueryable<CourseDayDto> CourseDays(ODataQueryOptions options)
        {
            var iso = new IncludeSelectOptions(options);
            return Repo.GetCourseDays(iso.Includes, iso.Selects, IncludeSelectOptions.Seperator);
        }

        [HttpGet]
        public IQueryable<CourseTypeScenarioRoleDto> CourseTypeScenarioRoles()
        {
            return Repo.CourseTypeScenarioRoles;
        }

        [HttpGet]
        public IQueryable<FacultyScenarioRoleDto> FacultyScenarioRoles()
        {
            return Repo.FacultyScenarioRoles;
        }

        [HttpGet, EnableMappedBreezeQuery(MaxExpansionDepth = 4)]
		public IQueryable<CourseDto> Courses(ODataQueryOptions options)
        {
            var iso = new IncludeSelectOptions(options);
            var returnVar = Repo.GetCourses(iso.Includes, iso.Selects, IncludeSelectOptions.Seperator);
            //???working now with EnableMappedBreezeQuery????
            //hack alert - currently options.applyto does not handle includes which are deeper than a null parent
            //the following hack works around this for a very specific use scenario
            //this is about as ugly as hacks get, so steel yourself before reading ahead
            /*
            if (iso.Includes != null && iso.Includes.Any(i=>i.Contains("Activity/")))
            {
                var exp = (BinaryOperatorNode)options.Filter.FilterClause.Expression;
                System.Diagnostics.Debug.Assert(exp.OperatorKind == Microsoft.Data.OData.Query.BinaryOperatorKind.Equal);
                System.Diagnostics.Debug.Assert(((SingleValuePropertyAccessNode)exp.Left).Property.Name == "Id");
                Guid id = (Guid)((ConstantNode)exp.Right).Value;
                //todo fix this method up so that the query isn't being applied twice
                return returnVar.Where(c=>c.Id==id).ToList().AsQueryable();
            }
            */
            return returnVar;
        }

        private void AllowedAnonIncludes(IncludeSelectOptions iso, params string[] allowed)
        {
            var emptyString = new string[0];
            if (!User.Identity.IsAuthenticated && ((iso.Includes ?? emptyString).Any(i => !allowed.Contains(i))) 
                || (iso.Selects ?? emptyString).Any(s => !allowed.Contains(s)))
            {
                throw new HttpResponseException(HttpStatusCode.Unauthorized);
            }
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
                        Includes =  se.RawExpand.Split(splitter);
                    }
                    if (se.RawSelect != null)
                    {
                        Selects = se.RawSelect.Split(splitter);
                    }
                }
                if (options.Filter != null)
                {
                    var navs = FindNavigationFilterOptions.GetPaths(options.Filter, Seperator.ToString());
                    if (navs.Any())
                    {
                        Includes = (Includes ?? new string[0]).Union(navs).ToArray();
                    }
                }
                if (options.OrderBy != null)
                {
                    var orderProps = new HashSet<string>(Includes ?? new string[0]);
                    foreach (var n in options.OrderBy.RawValue.Split(splitter))
                    {
                        int i = n.LastIndexOf(Seperator);
                        if (i>-1) {
                            orderProps.Add(n.Substring(0,i));
                        }
                    }
                    Includes = orderProps.ToArray();
                }
            }
            public readonly string[] Includes;
            public readonly string[] Selects;
            public const char Seperator = '/';
            const char splitter = ',';
        }
        [HttpGet, EnableMappedBreezeQuery(MaxExpansionDepth = 3)]
        public IQueryable<CourseTypeDto> CourseTypes(ODataQueryOptions options)
        {
            var iso = new IncludeSelectOptions(options);
            return Repo.GetCourseTypes(iso.Includes, iso.Selects, IncludeSelectOptions.Seperator);
        }

        [HttpGet]
        public LookupBundle Lookups()
        {
            var emptyString = new string[0];
            return new LookupBundle
            {
                Institutions = Repo.GetInstitutions(includes: new[] { "Departments.Rooms", "ProfessionalRoleInstitutions", "Culture", "Departments.Manikins" }).ToList(),
                CourseTypes = Repo.GetCourseTypes(includes: new[] { "CourseFormats" }).ToList(),
                ProfessionalRoles = Repo.GetProfessionalRoles(emptyString, emptyString).ToList(),
                ManikinManufacturers = Repo.ManikinManufacturers.ToList(),
                Manikins = Repo.Manikins.ToList()
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