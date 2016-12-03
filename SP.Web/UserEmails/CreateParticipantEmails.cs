using SP.DataAccess;
using SP.Dto;
using SP.Dto.Maps;
using SP.Dto.Utilities;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
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
        public static async Task<EmailResult> SendEmail(Course course, IPrincipal user)
        {
            var map = ((Expression<Func<CourseParticipant, CourseParticipantDto>>)new CourseParticipantMaps().MapToDto).Compile();
            var faculty = course.CourseParticipants.Where(cp => !map(cp).IsEmailed)
                .ToLookup(cp => cp.IsFaculty);

            IEnumerable<Attachment> attachments = new Attachment[0];

            var success = new ConcurrentBag<CourseParticipant>();
            var fail = new ConcurrentBag<CourseParticipant>();

            using (var parallelEmails = new ParallelSmtpEmails())
            {
                List<MailMessage> messages = new List<MailMessage>(course.CourseParticipants.Count);
                var sendMail = new Action<CourseParticipant>(cp =>
                {
                    var mail = new MailMessage();
                    messages.Add(mail);
                    mail.To.AddParticipants(cp.Participant);
                    var confirmEmail = new CourseInvite { CourseParticipant = cp };
                    mail.CreateHtmlBody(confirmEmail);
                    foreach (var a in attachments)
                    {
                        //a.ContentStream.Position = 0;
                        mail.Attachments.Add(a);
                    }
                    parallelEmails.Send(mail, s=> {
                        if (s == null)
                        {
                            success.Add(cp);
                        }
                        else
                        {
                            fail.Add(cp);
                        }
                    });
                });

                foreach (var f in faculty[false])
                {
                    sendMail(f);
                }
                            

                if (faculty[true].Any())
                {
                    attachments = GetFilePaths(course)
                        .Select(fp => new Attachment(Stream.Synchronized(fp.Value.ToStream()), System.Net.Mime.MediaTypeNames.Application.Zip) { Name = fp.Key })
                        .Concat(new[] { new Attachment(Stream.Synchronized(CreateDocxTimetable.CreateTimetableDocx(course, WebApiConfig.DefaultTimetableTemplatePath)), OpenXmlDocxExtensions.DocxMimeType) { Name = CreateDocxTimetable.TimetableName(course) } }).ToList();
                    foreach (var f in faculty[true])
                    {
                        sendMail(f);
                    }
                }
                await parallelEmails.SendingComplete();
                messages.ForEach(m=>m.Dispose());
            }
            return new EmailResult {
                SuccessRecipients = success.ToArray(),
                FailRecipients = fail.ToArray()
            };
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