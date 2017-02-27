using SP.Web.VideoStreaming;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web;
using System.Web.Http;

namespace SP.Web.Controllers
{
    [RoutePrefix("api/videos")]
    public class VideosController : ApiController
    {
        [HttpGet]
        [ActionName("stream")]
        public HttpResponseMessage Stream(string id, [FromUri]string ext)
        {
            var video = new VideoStream(id, ext);

            var response = Request.CreateResponse();
            response.Content = new PushStreamContent(video.WriteToStream, new MediaTypeHeaderValue("video/" + ext));

            return response;
        }
        /*
        [HttpGet]
        [ActionName("dash")]
        public HttpResponseMessage Dash(string id)
        {
            var video = new VideoStream(id);

            var response = Request.CreateResponse();
            response.Content = new PushStreamContent(video.WriteToStream, new MediaTypeHeaderValue("video/mp4"));

            return response;
        }
        */
    }
}