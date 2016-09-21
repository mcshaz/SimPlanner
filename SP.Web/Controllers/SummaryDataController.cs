using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using SP.DataAccess;
using SP.Dto.ParticipantSummary;
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
        public SummaryDataController() {
        }
        public SummaryDataController(MedSimDbContext context)
        {
            _context = context;
        }
        MedSimDbContext _context;
        private MedSimDbContext Context
        {
            get { return _context ?? (_context = HttpContext.Current.GetOwinContext().Get<MedSimDbContext>());  }
        } 
        [Route("UserInfo")]
        [HttpGet]
        public IHttpActionResult GetUserInfo(DateTime? after=null)
        {
            var userId = Guid.Parse(User.Identity.GetUserId());
            return Json(ParticipantSummaryServices.GetParticipantSummary(Context, userId, after));
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
