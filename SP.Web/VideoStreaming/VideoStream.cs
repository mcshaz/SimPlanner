using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Web;

namespace SP.Web.VideoStreaming
{
    //http://www.strathweb.com/2013/01/asynchronously-streaming-video-with-asp-net-web-api/
    public class VideoStream
    {
        private readonly string _filename;

        public VideoStream(string filename, string ext)
        {
            _filename = @"C:UsersFilipDownloads" + filename + "." + ext;
        }

        public async void WriteToStream(Stream outputStream, HttpContent content, TransportContext context)
        {
            try
            {
                var buffer = new byte[65536];

                using (var video = File.Open(_filename, FileMode.Open, FileAccess.Read))
                {
                    var length = (int)video.Length;
                    var bytesRead = 1;

                    while (length > 0 && bytesRead > 0)
                    {
                        bytesRead = video.Read(buffer, 0, Math.Min(length, buffer.Length));
                        await outputStream.WriteAsync(buffer, 0, bytesRead);
                        length -= bytesRead;
                    }
                }
            }
            catch (HttpException)
            {
                return;
            }
            finally
            {
                outputStream.Close();
            }
        }
    }
}