using System;
using System.Web;

namespace SM.Web.UserEmails
{
    public partial class ForgotPasswordTemplate : IMailBody
    {
        public string Title { get { return "SimManager Password Reset"; } }
        public string Token { get; set; }
        public Guid UserId { get; set; }

        public string AsQueryString()
        {
            var url = HttpContext.Current.Request.Url;

            return url.Scheme + "://" + url.Authority + "/index.html#/resetPassword?token=" + HttpUtility.UrlEncode(Token) + "&userId=" + UserId.ToString();
        }
    }
}