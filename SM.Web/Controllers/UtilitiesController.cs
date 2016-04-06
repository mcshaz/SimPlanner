using System.Linq;
using System.Web.Http;
using System;
using System.Collections.Generic;

namespace SM.Web.Controllers
{
    [Authorize]
    public class UtilitiesController : ApiController
    {
        // Todo: inject via an interface rather than "new" the concrete class
  
        [HttpGet]
		public IEnumerable<string> ProfessionalRoles(){
            return TimeZoneInfo.GetSystemTimeZones().Select(tz=>tz.Id);
        } 
    }
}