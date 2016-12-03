using Microsoft.AspNet.Identity.Owin;
using SP.DataAccess;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web;
using System.Web.Http;

namespace SP.Web.Controllers
{
    public class StreamControllerBase : ApiController
    {
        private List<Stream> _streamsToDispose = new List<Stream>();

        MedSimDbContext _repo;
        protected MedSimDbContext Repo
        {
            get { return _repo ?? (_repo = HttpContext.Current.GetOwinContext().Get<MedSimDbContext>()); }
        }

        protected HttpResponseMessage StreamToResponse(Stream stream, string fileName)
        {
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

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (_repo != null)
                {
                    _repo.Dispose();
                }
                _streamsToDispose.ForEach(s => s.Dispose());
            }
            base.Dispose(disposing);
        }
    }
}