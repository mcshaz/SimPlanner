namespace SM.Web.UserEmails
{
    using System.Net.Mail;
    using System.Text;
    using PreMailer.Net;
    using System.IO;
    using System.Web;
    public static class MailExtensions
    {
        public static void CreateHtmlBody(this MailMessage message, string title, object template)
        {
            var mailTemplate = new EmailTemplate { BodyTemplate = template, Title = title };

            var pm = new PreMailer(mailTemplate.TransformText());

            string output = pm.MoveCssInline(css: File.ReadAllText(HttpContext.Current.Server.MapPath(@"~/UserEmails/foundation.css"))).Html;

            var html = AlternateView.CreateAlternateViewFromString(output, Encoding.UTF8, "text/html");
            message.AlternateViews.Add(html);

            message.Body = title + "\r\n\r\n" + HtmlToText.ConvertHtml(mailTemplate.Body);
        }
    }
}
