namespace SM.Web.UserEmails
{
    using System.Threading;
    using System.Web;
    public partial class EmailTemplate
    {
        public EmailTemplate()
        {
            var url = HttpContext.Current.Request.Url;
            BaseUrl = url.Scheme + "://" + url.Authority + "/";
            ToStringHelper.FormatProvider = Thread.CurrentThread.CurrentCulture;
        }
        public string Body { get; set; }
        public string Title { get; set; }
        public readonly string BaseUrl;

        public IMailBody BodyTemplate
        {
            set
            {
                value.BaseUrl = BaseUrl;
                value.ToStringFormatProvider = ToStringHelper.FormatProvider;
                //must be assigned first!
                Body = value.TransformText();
                Title = value.Title;
            }
        }
    }
}
