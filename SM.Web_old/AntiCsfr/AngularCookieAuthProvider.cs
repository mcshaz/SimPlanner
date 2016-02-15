using Microsoft.Owin;
using Microsoft.Owin.Security.Cookies;
using System.Web.Helpers;

namespace SM.Web
{
    public class AngularCoookieAuthProvider : CookieAuthenticationProvider
    {
        public const string AngularHeaderTokenName = "XSRF-TOKEN";
        public const string AngularCookieTokenName = "X-XSRF-TOKEN";
        public override void ResponseSignedIn(CookieResponseSignedInContext context)
        {
            SetAntiCsfrTokens(context.Response);
            base.ResponseSignedIn(context);
        }
        public override void ResponseSignOut(CookieResponseSignOutContext context)
        {
            context.Response.Cookies.Delete(AngularCookieTokenName);
            context.Response.Headers.Remove(AngularHeaderTokenName);
            base.ResponseSignOut(context);
        }
        internal static void SetAntiCsfrTokens(IOwinResponse response, string oldCookieToken=null)
        {
            string cookieToken;
            string formToken;
            AntiForgery.GetTokens(oldCookieToken, out cookieToken, out formToken);

            response.Cookies.Append(AngularCookieTokenName, cookieToken);
            response.Headers.Append(AngularHeaderTokenName, formToken);
        }
    }
}
