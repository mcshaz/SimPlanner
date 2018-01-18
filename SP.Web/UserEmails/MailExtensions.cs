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
    using System.Data.Entity.Infrastructure;
    using Dto.Utilities;
    using Dto;
    using System;
    using System.Security.Principal;
    using System.Data.Entity;
    using System.Threading.Tasks;

    public static class MailExtensions
    {
        public static DbQuery<Course> GetCourseIncludes(MedSimDbContext repo)
        {
            return CreateDocxTimetable.GetCourseIncludes(repo)
                .Include("CourseParticipants.Department.Institution.Culture")
                .Include("Room")
                .Include("FacultyMeetingRoom")
                .Include("CourseFormat.CourseType.CandidatePrereadings");
        }

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
                        mail.To.AddParticipants(d.Notify);
                        client.Send(mail);
                    }
                }
            }
        }

        public static async Task SendNewCourseParticipantNotificationsAsync(IEnumerable<CourseParticipant> courseParticipants, MedSimDbContext repo, IPrincipal principal)
        {
            //logic:
            //get all courses hapening after now where the currentUser is not an organiser, and group by each organiser
            var currentUser = repo.Users.Include("Department").Include("ProfessionalRole").Single(u => u.UserName == principal.Identity.Name);
            var courseIds = courseParticipants.ToHashSet(cp => cp.CourseId);
            var organisers = repo.CourseParticipants.Include("Course.CourseFormat.CourseType")
                            .Include("Participant.Department.Institution")
                            .Include("Course.Department.Institution.Culture")
                            .Where(cp => courseIds.Contains(cp.CourseId) && cp.IsOrganiser && !cp.Course.CourseParticipants.Any(ap=>ap.IsOrganiser && ap.ParticipantId == currentUser.Id))
                            .ToLookup(cp => cp.Participant);
            
            using (var client = new ParallelSmtpEmails())
            {
                foreach (var o in organisers)
                {
                    var n = new MultiCourseInviteResonse
                    {
                        Courses = o.Select(cp=>cp.Course),
                        PersonResponding = currentUser
                    };
                    var mail = new MailMessage();
                    mail.To.AddParticipants(o.Key);
                    mail.CreateHtmlBody(n);
                    client.Send(mail);
                }
                await client.SendingComplete();
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
                    mail.To.AddParticipants(newUser.Administrators);
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
                    mail.To.AddParticipants(newUser);
                    mail.CreateHtmlBody(n);
                    client.Send(mail);
                }
            }
        }
    }
}
