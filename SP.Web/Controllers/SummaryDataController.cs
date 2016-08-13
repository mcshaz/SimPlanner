using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using SP.DataAccess;
using SP.DTOs.ParticipantSummary;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Web;
using System.Web.Http;

namespace SP.Web.Controllers
{
    [Authorize]
    [RoutePrefix("api/ActivitySummary")]
    public class SummaryDataController : ApiController
    {
        [Route("UserInfo")]
        public IHttpActionResult GetUserInfo(DateTime? after=null)
        {

            var userId = Guid.Parse(User.Identity.GetUserId());
            var owinContext = HttpContext.Current.GetOwinContext().Get<MedSimDbContext>();
            return Json(ParticipantSummaryServices.GetParticipantSummary(owinContext, userId, after));
        }
    }
    public class ActivitySummary
    {
        public IEnumerable<KeyValuePair<Guid,ActivityCount>> DepartmentActivities { get; set; }
        public IEnumerable<KeyValuePair<string, int>> RolesUndertaken { get; set; }
    }
    public class ActivityCount
    {
        public int InstructorCourses { get; set; }
        public int ParticipantCourses { get; set; }
    }
}
