namespace SP.Web.UserEmails
{
    using System.Net.Mail;
    using System.Text;
    using System.Net.Mime;
    using System.Collections.Generic;
    using Dto.ProcessBreezeRequests;
    using System.Threading.Tasks;

    public static class MailExtensions
    {
        public static void CreateHtmlBody(this MailMessage message, EmailBase template)
        {
            var html = template.TransformText();
            message.Subject = ((LayoutTemplate)template.Layout)?.Title ?? "Sim-planner email";
            message.Body = message.Subject + new string('-', message.Subject.Length) + "\r\n\r\n" + HtmlToText.ConvertHtml(html);
            message.AlternateViews.Add(AlternateView.CreateAlternateViewFromString(html, Encoding.UTF8, MediaTypeNames.Text.Html));
        }

        public static void SendBookingNotifications(IEnumerable<BookingChangeDetails> details)
        {
            using (var client = new SmtpClient())
            {
                foreach (var d in details)
                {
                    var n = new NotifyBooking
                    {
                        Course = d.RelevantCourse,
                        PersonBooking = d.PersonBooking,
                        ManikinsBooked = d.AddedManikinBookings,
                        ManikinsCancelled = d.RemovedManikinBookings,
                        RoomBooked = d.AddedRoomBooking,
                        RoomCancelled = d.RemovedRoomBooking
                    };
                    using (var mail = new MailMessage())
                    {
                        mail.CreateHtmlBody(n);
                        client.Send(mail);
                    }
                }
            }
        }

    }
}
