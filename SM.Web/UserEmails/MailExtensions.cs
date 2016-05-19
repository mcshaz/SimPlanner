namespace SM.Web.UserEmails
{
    using System.Net.Mail;
    using System.Text;
    using PreMailer.Net;
    using System.IO;
    using System.Web;
    public static class MailExtensions
    {
        internal static void CreateHtmlBody(this MailMessage message, IMailBody template)
        {
            var mailTemplate = new EmailTemplate { BodyTemplate = template };

            var pm = new PreMailer(mailTemplate.TransformText());

            string output = pm.MoveCssInline(css: File.ReadAllText(HttpContext.Current.Server.MapPath(@"~/UserEmails/foundation.css"))).Html;

            var html = AlternateView.CreateAlternateViewFromString(output, Encoding.UTF8, "text/html");
            message.AlternateViews.Add(html);

            message.Subject = mailTemplate.Title;

            message.Body = mailTemplate.Title + new string('-', mailTemplate.Title.Length) + "\r\n\r\n" + HtmlToText.ConvertHtml(mailTemplate.Body);
        }
    }
}
