using SP.DataAccess;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;

namespace SP.Web.Controllers
{
    [Authorize]
    [RoutePrefix("api/ActivitySummary")]
    public class SummaryDataController : ApiController
    {
        [Route("UserInfo")]
        public IHttpActionResult GetUserInfo(DateTime? from=null)
        {

            var userName =User.Identity.Name;
            using (var db = new MedSimDbContext())//to do - get from 1 per owin context
            {

            }
            throw new NotImplementedException();
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
