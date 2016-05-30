using System;

namespace SM.Web.UserEmails
{
    public interface IMailBody
    {
        string TransformText();
        string Title { get; }
        string BaseUrl { set; }
        IFormatProvider ToStringFormatProvider { set; }
    }

}
