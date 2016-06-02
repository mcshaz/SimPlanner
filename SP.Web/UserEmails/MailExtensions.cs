namespace SP.Web.UserEmails
{
    using System.Net.Mail;
    using System.Text;
    using PreMailer.Net;
    using System.IO;
    using System.Web;
    using System.Net.Mime;
    using System.Text.RegularExpressions;
    public static class MailExtensions
    {
        internal static void CreateHtmlBody(this MailMessage message, IMailBody template)
        {
            var mailTemplate = new EmailTemplate() { BodyTemplate = template };

            var pm = new PreMailer(mailTemplate.TransformText());

            string output = pm.MoveCssInline(css: File.ReadAllText(HttpContext.Current.Server.MapPath(@"~/UserEmails/app.css"))).Html;

            var html = AlternateView.CreateAlternateViewFromString(RemoveWhiteSpace(output), Encoding.UTF8, MediaTypeNames.Text.Html);

            message.Subject = mailTemplate.Title;

            message.Body = mailTemplate.Title + new string('-', mailTemplate.Title.Length) + "\r\n\r\n" + HtmlToText.ConvertHtml(mailTemplate.Body);

            message.AlternateViews.Add(html);
        }

        static string RemoveWhiteSpace(string inpt)
        {
            bool lastWS = false;
            StringBuilder returnVar = new StringBuilder(inpt.Length);
            for (int i = 0; i < inpt.Length; i++)
            {
                if (!char.IsWhiteSpace(inpt[i]))
                {
                    returnVar.Append(inpt[i]);
                    lastWS = false;
                }
                else if (!lastWS)
                {
                    lastWS = true;
                    returnVar.Append(inpt[i]);
                }
            }
            return returnVar.ToString();
        }
    }
}
