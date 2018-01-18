using NLog;
using SP.Web.Models;
using System.Web.Http;

namespace SP.Web.Controllers
{
    public class ErrorController : ApiController
    {
        private static ILogger _logger = LogManager.GetCurrentClassLogger();

        [HttpGet, HttpPost, HttpPut, HttpDelete, HttpHead, HttpOptions]
        public IHttpActionResult NotFound(string path)
        {
        // log error to NLog
            _logger.Info(()=>"404 not found:" + path);

            // return 404
            return NotFound();
        }
    }
}
