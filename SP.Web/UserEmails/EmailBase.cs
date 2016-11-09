using SP.DataAccess;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace SP.Web.UserEmails
{
    public class EmailBase : RazorGenerator.Templating.RazorTemplateBase
    {
        private static object _lock = new object();
        private static string _baseUrl;

        public IFormatProvider FormatProvider { get; protected set; }
        
        public string BaseUrl
        {
            get
            { 
                if (_baseUrl == null)
                {
                    lock (_lock)
                    {
                        var url = HttpContext.Current.Request.Url;
                        _baseUrl = url.Scheme + "://" + url.Authority + '/';
                    }
                }
                return _baseUrl;
            }
        }

        public static string GetMailTo(Participant participant)
        {
            string returnVar = "mailto:" + participant.Email;
            if (!string.IsNullOrEmpty(participant.AlternateEmail))
            {
                returnVar += "?cc=" + participant.AlternateEmail;
            }
            return returnVar;
        }
    }
}
