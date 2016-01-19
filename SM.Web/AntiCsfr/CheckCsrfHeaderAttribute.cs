using System.Linq;
using System.Net.Http;
using System.Web.Helpers;
using System.Web.Http;
using System.Web.Http.Controllers;

namespace SM.Web
{
    public class CheckCsrfHeaderAttribute : AuthorizeAttribute
    {
        //  http://stackoverflow.com/questions/11725988/problems-implementing-validatingantiforgerytoken-attribute-for-web-api-with-mvc
        protected override bool IsAuthorized(HttpActionContext context)
        {
            var owinContext = context.Request.GetOwinContext();
            var request = owinContext.Request;
            // get auth token from cookie
            var authCookie = request.Cookies[AngularCoookieAuthProvider.AngularCookieTokenName];
            var csrfToken = request.Headers.GetValues(AngularCoookieAuthProvider.AngularHeaderTokenName).FirstOrDefault();

            // Verify that csrf token was generated from auth token
            // Since the csrf token should have gone out as a cookie, only our site should have been able to get it (via javascript) and return it in a header. 
            // This proves that our site made the request.
            AntiForgery.Validate(csrfToken, authCookie);//should throw if a problem

            AngularCoookieAuthProvider.SetAntiCsfrTokens(owinContext.Response, authCookie);
            return true;
  
            //to do - update cookie
        }
    }
}
