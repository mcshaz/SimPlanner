namespace SP.Web.UserEmails
{
    using System.Net.Mail;
    using System.Text;
    using System.Net.Mime;

    public static class MailExtensions
    {
        public static void CreateHtmlBody(this MailMessage message, EmailBase template)
        {
            var html = template.TransformText();

            message.Subject = ((LayoutTemplate)template.Layout)?.Title ?? "Sim-planner email";

            message.Body = message.Subject + new string('-', message.Subject.Length) + "\r\n\r\n" + HtmlToText.ConvertHtml(html);

            message.AlternateViews.Add(AlternateView.CreateAlternateViewFromString(html, Encoding.UTF8, MediaTypeNames.Text.Html));
        }

    }
}
