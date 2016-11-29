using SP.DataAccess;
using SP.Dto;
using SP.Dto.Maps;
using SP.Dto.Utilities;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Net.Mail;
using System.Security.Principal;
using System.Threading.Tasks;

namespace SP.Web.UserEmails
{
    public static class CreateParticipantEmails
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="course"></param>
        /// <param name="user"></param>
        /// <returns>a lookup true = successfully emailed, false = email failed</returns>
        public static EmailResult SendEmail(Course course, IPrincipal user)
        {
            using (var cal = Appointment.CreateCourseAppointment(course, user.Identity))
            {
                using (var appt = new AppointmentStream(cal))
                {
                    var map = ((Expression<Func<CourseParticipant, CourseParticipantDto>>)new CourseParticipantMaps().MapToDto).Compile();
                    var faculty = course.CourseParticipants.Where(cp => !map(cp).IsEmailed)
                        .ToLookup(cp => cp.IsFaculty);

                    IEnumerable<Attachment> attachments = new Attachment[0];

                    var success = new ConcurrentBag<CourseParticipant>();
                    var fail = new ConcurrentBag<CourseParticipant>();
                    var sendMail = new Action<CourseParticipant>(cp =>
                    {
                        using (var mail = new MailMessage())
                        {
                            mail.To.AddParticipants(cp.Participant);
                            var confirmEmail = new CourseInvite { CourseParticipant = cp };
                            mail.CreateHtmlBody(confirmEmail);
                            appt.AddAppointmentsTo(mail);
                            foreach (var a in attachments)
                            {
                                a.ContentStream.Position = 0;
                                mail.Attachments.Add(a);
                            }
                            using (var client = new SmtpClient())
                            {
                                try
                                {
                                    client.Send(mail);
                                    success.Add(cp);
                                }
                                catch (SmtpFailedRecipientsException)
                                {
                                    fail.Add(cp);
                                }
                            }
                        }
                    });

                    Parallel.ForEach(faculty[false], sendMail); //new ParallelOptions { MaxDegreeOfParallelism = 5 }

                    if (faculty[true].Any())
                    {
                        if (course.FacultyMeetingUtc.HasValue)
                        {
                            using (var fm = Appointment.CreateFacultyCalendar(course))
                            {
                                appt.Add(fm);
                            }
                        }
                        attachments = GetFilePaths(course).Select(fp => new Attachment(fp.Value, System.Net.Mime.MediaTypeNames.Application.Zip) { Name = fp.Key })
                            .Concat(new[] { new Attachment(CreateDocxTimetable.CreateTimetableDocx(course, WebApiConfig.DefaultTimetableTemplatePath), OpenXmlDocxExtensions.DocxMimeType) { Name = CreateDocxTimetable.TimetableName(course)} });
                        Parallel.ForEach(faculty[true], sendMail);
                    }
                    return new EmailResult {
                        SuccessRecipients = success.ToArray(),
                        FailRecipients = fail.ToArray()
                    };
                }
            }
        }

        private static IEnumerable<KeyValuePair<string, string>> GetFilePaths(Course course)
        {
            return (from s in course.CourseSlotActivities
                    let sr = s.Scenario.ScenarioResources.FirstOrDefault(ssr => ssr.FileName != null)
                    where sr != null
                    select new KeyValuePair<string, string>(s.Scenario.BriefDescription, sr.GetServerPath()))
                   .Concat(from ctr in course.CourseSlotActivities
                           let atr = ctr.Activity
                           where atr.FileName != null
                           select new KeyValuePair<string, string>(atr.Description, atr.GetServerPath()));
        }
    }
    /*
    internal class ParticipantMail
    {
        public CourseParticipant CourseParticipant { get; set; }
        //public Task SendTask { get; set; }
        //public MailMessage Message { get; set; }
    }
    */
    public class EmailResult
    {
        public IEnumerable<CourseParticipant> SuccessRecipients { get; set; }
        public IEnumerable<CourseParticipant> FailRecipients { get; set; }
    }
}