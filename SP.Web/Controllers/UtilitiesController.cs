using System.Linq;
using System.Web.Http;
using System.Collections.Generic;
using System.Globalization;
using NodaTime.TimeZones;
using System.Net.Http;
using System;
using SP.Dto.Utilities;
using System.IO;
using System.Net;
using Microsoft.AspNet.Identity.Owin;
using System.Threading.Tasks;
using SP.Web.Models;
using System.Web;
using SP.Web.Controllers.Helpers;
using System.Data.Entity;
using SP.Dto.Maps;
using System.Linq.Expressions;
using SP.Dto.ProcessBreezeRequests;
using System.Security.Principal;

namespace SP.Web.Controllers
{
    [RoutePrefix("api/utilities")]
    public class UtilitiesController : StreamControllerBase
    {
        private ApplicationUserManager _userManager;
        private ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? Request.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            set
            {
                _userManager = value;
            }
        }

        CurrentPrincipal _currentUser;
        private CurrentPrincipal CurrentUser
        {
            get { return _currentUser ?? ( _currentUser = new CurrentPrincipal(User, Repo)); }
        }
        /*
        MedSimDtoRepository _repo;
        private MedSimDtoRepository Repo
        {
            get { return _repo ?? (_repo = new MedSimDtoRepository(User)); }
            set { _repo = value; }
        }
        private async Task<bool> VerifyAccessTokenAsync(string access_token, Guid userId)
        {
            var user = UserManager.FindByIdAsync(userId);

            ClaimsIdentity oAuthIdentity = UserManager.;
            ClaimsIdentity cookieIdentity = await user.GenerateUserIdentityAsync(UserManager,
                CookieAuthenticationDefaults.AuthenticationType);

            //at preesent creating the properties has no effect on the fragment provided in the redirect response
            AuthenticationProperties properties = ApplicationOAuthProvider.CreateProperties(user, await UserManager.GetRolesAsync(user.Id));
            Authentication.SignIn(properties, oAuthIdentity, cookieIdentity);
        }
        */

        private async Task<bool> VerifyUserTokenAsync(string token, Guid userId)
        {
            var cookie = Request.Headers.GetCookies(AccountController.DownloadPurpose).FirstOrDefault();
            if (cookie == null)
            {
                return false;
            }
            string key = cookie[AccountController.DownloadPurpose].Value;
            var returnVar = await UserManager.VerifyUserTokenAsync(userId, key, token, TimeSpan.FromMinutes(1));
            if (returnVar && !User.Identity.IsAuthenticated)
            {
                //for using the logic to restrict access within our Dto layer
                var appUser = await UserManager.FindByIdAsync(userId);
                //hopefully changing this will not cause problems downstream - we do not want cookies going back and forward
                User = new GenericPrincipal(new GenericIdentity(appUser.UserName), (await UserManager.GetRolesAsync(userId)).ToArray());
            }
            return returnVar;
        }

        [HttpGet]
        public IEnumerable<CultureFormatModel> CultureFormats() {
            return (from c in CultureInfo.GetCultures(CultureTypes.AllCultures)
                    let indx = c.Name.LastIndexOf('-')
                    where  indx != -1 && indx == c.Name.Length-3
                    select new CultureFormatModel { LocaleCode = c.Name, DisplayName = c.DisplayName });
        }

        [HttpGet]
        public IEnumerable<string> TimeZones(string id)
        {
            int dashPos = id.IndexOf('-');
            if (dashPos != -1)
            {
                id = id.Substring(dashPos + 1);
            }
            HashSet<string> returnVar = new HashSet<string>();
            foreach (var t in TzdbDateTimeZoneSource.Default.ZoneLocations)
            {
                if (t.CountryCode == id)
                {
                    var mapZone = TzdbDateTimeZoneSource.Default.WindowsMapping.MapZones.FirstOrDefault(x => x.TzdbIds.Contains(t.ZoneId));
                    if (mapZone != null)
                    {
                        returnVar.Add(mapZone.WindowsId);
                    }
                }
            }
            return returnVar;
        }

        [HttpGet]
        public string CurrencyInfo(string id)
        {
            /*
            var ci = CultureInfo.GetCultureInfo(cultureCode);
            var returnVar = new Dictionary<string, object>();
            returnVar.Add("currencyDecimalDigits", ci.NumberFormat.CurrencyDecimalDigits);
            returnVar.Add("currencyDecimalSeparator", ci.NumberFormat.CurrencyDecimalSeparator);
            returnVar.Add("currencyGroupSeparator", ci.NumberFormat.CurrencyGroupSeparator);
            returnVar.Add("currencyGroupSizes", ci.NumberFormat.CurrencyGroupSizes);
            returnVar.Add("currencySymbol", ci.NumberFormat.CurrencySymbol);
            */
            var ri = new RegionInfo(id);

            return ri.ISOCurrencySymbol;
        }
        [Route("ScenarioResources")]
        [HttpGet]
        public async Task<HttpResponseMessage> GetResourcesForScenario([FromUri]DowloadFileSetModel model)
        {
            var validation = await ValidateInput(model);
            if (validation != null)
            {
                return validation;
            }

            var scenarios = GetPermittedEntity(Repo.Scenarios.Include(s => s.ScenarioResources));
            var scenario = await scenarios.FirstAsync(c => c.Id == model.EntitySetId);

            var sr = scenario.ScenarioResources.FirstOrDefault();
            if (sr == null)
            {
                ModelState.AddModelError("EntitySetId", "no resources found for the provided key");
                return Request.CreateResponse(HttpStatusCode.BadRequest, ModelState);
            }

            return StreamToResponse(new FileStream(sr.GetServerPath(), FileMode.Open), scenario.BriefDescription + ".zip");
        }

        [Route("CandidateReading")]
        [HttpGet]
        public async Task<HttpResponseMessage> GetCandidateReading([FromUri]DowloadFileSetModel model)
        {
            var validation = await ValidateInput(model);
            if (validation != null)
            {
                return validation;
            }

            var courseTypes = GetPermittedEntity(Repo.CourseTypes.Include(s => s.CandidatePrereadings));
            var courseType = await courseTypes.FirstAsync(c => c.Id == model.EntitySetId);

            var read = courseType.CandidatePrereadings.FirstOrDefault();
            if (read == null)
            {
                ModelState.AddModelError("EntitySetId", "no resources found for the provided key");
                return Request.CreateResponse(HttpStatusCode.BadRequest, ModelState);
            }

            return StreamToResponse(new FileStream(read.GetServerPath(), FileMode.Open), courseType.Abbreviation + " Reading.zip");
        }

        [Route("GetTimetable")]
        [HttpGet]
        public async Task<HttpResponseMessage> GetTimetableForCourse([FromUri]DowloadFileSetModel model)
        {
            var validation = await ValidateInput(model);
            if (validation != null)
            {
                return validation;
            }

            var courses = GetPermittedEntity(CreateDocxTimetable.GetCourseIncludes(Repo));
            var course = await courses.FirstAsync(c => c.Id == model.EntitySetId);
            //todo - check if a faculty member, otherwise download participant timetable
            return StreamToResponse(
                CreateDocxTimetable.CreateTimetableDocx(course, WebApiConfig.DefaultTimetableTemplatePath,true),
                CreateDocxTimetable.TimetableName(course));
        }

        [Route("GetCertificates")]
        [HttpGet]
        public async Task<HttpResponseMessage> GetCertificatesForCourse([FromUri]DowloadFileSetModel model)
        {
            var validation = await ValidateInput(model);
            if (validation != null)
            {
                return validation;
            }
            
            var courses = GetPermittedEntity(CreateCertificates.GetCourseIncludes(Repo));
            var course = await courses.FirstAsync(c => c.Id == model.EntitySetId);

            return StreamToResponse(
                CreateCertificates.CreatePptxCertificates(course, course.CourseFormat.CourseType.GetServerPath()),
                CreateCertificates.CertificateName(course));
        }

        [Route("CertificateTemplate")]
        [HttpGet]
        public async Task<HttpResponseMessage> GetCertificateTemplate([FromUri]DowloadFileSetModel model)
        {
            var validation = await ValidateInput(model);
            if (validation != null)
            {
                return validation;
            }

            var courseTypes = GetPermittedEntity(Repo.CourseTypes);
            var courseType = (await courseTypes.FirstOrDefaultAsync(c => c.Id == model.EntitySetId)) ?? new DataAccess.CourseType();
            string path = courseType.GetServerPath();
            return StreamToResponse(new FileStream(path, FileMode.Open), courseType.CertificateFileName ?? Path.GetFileName(path));
        }

        #region Helpers
        private IQueryable<T> GetPermittedEntity<T>(IQueryable<T> entitySet)
        {
            var permissionPredicate = (Expression<Func<T, bool>>)MapperConfig.GetWhereExpression(typeof(T), CurrentUser);
            if (permissionPredicate != null)
            {
                entitySet = entitySet.Where(permissionPredicate);
            }
            return entitySet;
        }

        readonly string[] _emptyString = new string[0];
        private async Task<HttpResponseMessage> ValidateInput(DowloadFileSetModel model)
        {
            if (!ModelState.IsValid)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, ModelState);
            }

            if (!await VerifyUserTokenAsync(model.Token, model.UserId))
            {
                return Request.CreateResponse(HttpStatusCode.Unauthorized);
            }
            return null;
        }
        #endregion //Helpers
    }
}