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
            return Ok(ParticipantSummaryServices.GetParticipantSummary(Context, userId, after));
        }

        [Route("PriorExposure")]
        [HttpGet]
        public IHttpActionResult PriorExposure(Guid courseId)
        {
            return Ok(ParticipantSummaryServices.GetExposures(Context, courseId));
        }


        [Route("ManikinBookings")]
        [HttpGet]
        public IHttpActionResult ManikinBookings([FromUri]Guid[] departmentIds, Guid courseId)
        {
            return Ok(ParticipantSummaryServices.GetBookedManikins(User, courseId, departmentIds));
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
