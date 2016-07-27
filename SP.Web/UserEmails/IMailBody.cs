using System;

namespace SP.Web.UserEmails
{
    public interface IMailBody
    {
        string TransformText();
        string Title { get; }
        string BaseUrl { set; }
        IFormatProvider ToStringFormatProvider { get; set; }
    }

}
