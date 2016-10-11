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
using System.Net.Http.Headers;
using Microsoft.AspNet.Identity.Owin;
using System.Threading.Tasks;
using SP.Web.Models;
using System.Web;
using SP.Web.Controllers.Helpers;
using SP.Dto;
using System.Data.Entity;

namespace SP.Web.Controllers
{
    [RoutePrefix("api/utilities")]
    public class UtilitiesController : ApiController
    {
        private List<Stream> _streamsToDispose = new List<Stream>();

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

        MedSimDtoRepository _context;
        private MedSimDtoRepository Context
        {
            get { return _context ?? (_context = new MedSimDtoRepository(User)); }
            set { _context = value; }
        }

        /*
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
                User = new RequestOnlyPrincipal(appUser.UserName, await UserManager.GetRolesAsync(userId));
            }
            return returnVar;
        }

        [HttpGet]
        public IEnumerable<KeyValuePair<string, string>> CultureFormats() {
            return (from c in CultureInfo.GetCultures(CultureTypes.AllCultures)
                    let indx = c.Name.LastIndexOf('-')
                    where  indx != -1 && indx == c.Name.Length-3
                    select new KeyValuePair<string, string>(c.Name, c.DisplayName));
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

            var scenario = await Context.GetScenarios(_emptyString, _emptyString).FirstAsync(s=>s.Id == model.EntitySetId);

            var path = ResourceExtensions.ScenarioResourceToPath(scenario.DepartmentId, scenario.Id);

            return StreamToResponse(new FileStream(path, FileMode.Open), scenario.BriefDescription + ".zip");
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

            var course = await CreateDocxTimetable.GetCourseIncludes(Context)
                .FirstOrDefaultAsync(c => c.Id == model.EntitySetId);
            return StreamToResponse(
                CreateDocxTimetable.CreateTimetableDocx(course, WebApiConfig.DefaultTimetableTemplatePath),
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
            var course = await CreateCertificates.GetCourseIncludes(Context)
                    .FirstOrDefaultAsync(c => c.Id == model.EntitySetId);
            return StreamToResponse(
                CreateCertificates.CreatePptxCertificates(course, WebApiConfig.DefaultCertificateTemplatePath),
                CreateCertificates.CertificateName(course));
        }

        #region Helpers

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

        private HttpResponseMessage StreamToResponse(Stream stream, string fileName){
            _streamsToDispose.Add(stream);
            stream.Position = 0;
            var result = new HttpResponseMessage()
            {
                Content = new StreamContent(stream)
            };
            result.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment")
            {
                //?urlencode
                FileName = fileName
            };
            result.Content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
            result.Content.Headers.ContentLength = stream.Length;
            return result;
        }
        #endregion //Helpers

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (_context != null)
                {
                    _context.Dispose();
                }
                _streamsToDispose.ForEach(s=>s.Dispose());
            }
            base.Dispose(disposing);
        }
    }
}