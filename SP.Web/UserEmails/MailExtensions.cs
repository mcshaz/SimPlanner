namespace SP.Web.UserEmails
{
    using System.Net.Mail;
    using System.Text;
    using System.Net.Mime;
    using System.Collections.Generic;
    using Dto.ProcessBreezeRequests;
    //using System.Threading.Tasks;
    using DataAccess;
    using System.Linq;

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

        public static void SendNewUserRequest(UserRequestingApproval newUser)
        {
            var n = new NotifyNewRegistrationRequest
            {
                PersonRequesting = newUser.User
            };
            using (var client = new SmtpClient())
            {
                using (var mail = new MailMessage())
                {
                    foreach (var ad in newUser.Administrators.Select(a=>new MailAddress(a.Email, a.FullName)))
                    {
                        mail.To.Add(ad);
                    }
                    foreach (var ad in from a in newUser.Administrators
                                       where !string.IsNullOrEmpty(a.AlternateEmail)
                                       select new MailAddress(a.AlternateEmail, a.FullName))
                    {
                        mail.CC.Add(ad);
                    }
                    mail.CreateHtmlBody(n);
                    client.Send(mail);
                }
            }
        }

        public static void SendNewUserApproved(Participant newUser)
        {
            var n = new ApplicationApproved
            {
                PersonRequesting = newUser
            };
            using (var client = new SmtpClient())
            {
                using (var mail = new MailMessage())
                {
                    mail.To.Add(new MailAddress(newUser.Email, newUser.FullName));
                    if (!string.IsNullOrEmpty(newUser.AlternateEmail))
                    {
                        mail.CC.Add(new MailAddress(newUser.AlternateEmail, newUser.FullName));
                    }
                    mail.CreateHtmlBody(n);
                    client.Send(mail);
                }
            }
        }
    }
}
