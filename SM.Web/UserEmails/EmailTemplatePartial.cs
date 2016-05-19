namespace SM.Web.UserEmails
{
    using System;
    using System.Linq;

    public partial class EmailTemplate
    {
        public string Body { get; set; }
        public string Title { get; set; }

        public IMailBody BodyTemplate
        {
            set
            {
                Body = value.TransformText();
                Title = value.Title;
            }
        }
    }
}
