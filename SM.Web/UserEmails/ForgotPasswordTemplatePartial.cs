using System;
using System.Web;

namespace SM.Web.UserEmails
{
    public partial class ForgotPasswordTemplate : IMailBody
    {

        public string BaseUrl { get; set; }
        public string Title { get { return "SimManager Password Reset"; } }
        public string Token { get; set; }
        public Guid UserId { get; set; }
        public IFormatProvider ToStringFormatProvider
        {
            set
            {
                ToStringHelper.FormatProvider = value;
            }
        }

        public string AsQueryString()
        {
            var url = HttpContext.Current.Request.Url;

            return BaseUrl + "index.html#/resetPassword?token=" + HttpUtility.UrlEncode(Token) + "&userId=" + UserId.ToString();
        }
    }
}