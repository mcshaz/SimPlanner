using System;
using System.Net;
using System.Threading.Tasks;

namespace SP.Dto.ProcessBreezeRequests
{
    public static class WebValidation
    {
        public static bool IsAccessible(string url) //optimal to use async Task<
        {
            if (url == null)
            {
                throw new ArgumentNullException("url");
            }

            if (!Uri.IsWellFormedUriString(url, UriKind.Absolute))
            {
                return false;
            }
            var request = (HttpWebRequest)WebRequest.Create(url);
            request.Method = "HEAD";

            try
            {
                using (var response = request.GetResponse() as HttpWebResponse) //await getResponseAsync
                {
                    return response?.StatusCode == HttpStatusCode.OK;
                }
            }
            catch (WebException)
            {
                return false;
            }
        }
    }
}
