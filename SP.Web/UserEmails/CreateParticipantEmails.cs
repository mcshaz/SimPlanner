using Hangfire;
using Hangfire.Server;
using SP.DataAccess;
using SP.Dto;
using SP.Dto.Maps;
using SP.Dto.Utilities;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.IO.Compression;
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
        /// <returns>a lookup true = successfully emailed, false = email failed</returns>
        public static async Task<EmailResult<CourseParticipant>> SendCourseEmail(Course course, DateTime? originalDate = null)
        {
            var map = ((Expression<Func<CourseParticipant, CourseParticipantDto>>)new CourseParticipantMaps().MapToDto).Compile();
            var faculty = course.CourseParticipants.Where(cp => map(cp).IsEmailed == originalDate.HasValue)
                .ToLookup(cp => cp.IsFaculty);
            var attachments = new List<Attachment>();
            string timetableName = CreateDocxTimetable.TimetableName(course);
            var participantTimetable = CreateDocxTimetable.CreateTimetableDocx(course, WebApiConfig.DefaultTimetableTemplatePath,false);
            if (course.CourseFormat.CourseType.SendCandidateTimetable)
            {
                attachments.Add(new Attachment(Stream.Synchronized(participantTimetable), OpenXmlDocxExtensions.DocxMimeType) { Name = timetableName });
            }
            
            attachments.AddRange(GetInviteReadings(course));

            var success = new ConcurrentBag<CourseParticipant>();
            var fail = new ConcurrentBag<CourseParticipant>();

            using (var parallelEmails = new ParallelSmtpEmails(disposeMsgOnComplete: false))
            {
                List<MailMessage> messages = new List<MailMessage>(course.CourseParticipants.Count);
                var sendMail = new Action<CourseParticipant>(cp =>
                {
                    var mail = new MailMessage();
                    messages.Add(mail);
                    mail.To.AddParticipants(cp.Participant);
                    var confirmEmail = new CourseInvite { CourseParticipant = cp, OldStart = originalDate };
                    mail.CreateHtmlBody(confirmEmail);
                    foreach (var a in attachments)
                    {
                        //a.ContentStream.Position = 0;
                        mail.Attachments.Add(a);
                    }
                    parallelEmails.Send(mail, s =>
                    {
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

                MemoryStream facultyTimetable = null;
                if (faculty[true].Any())
                {
                    attachments = new List<Attachment>();
                    facultyTimetable = CreateDocxTimetable.CreateTimetableDocx(course, WebApiConfig.DefaultTimetableTemplatePath, true);
                    attachments.Add(new Attachment(Stream.Synchronized(facultyTimetable), OpenXmlDocxExtensions.DocxMimeType) { Name = timetableName });

                    attachments.AddRange(GetFilePaths(course)
                        .Select(fp => new Attachment(Stream.Synchronized(fp.Value.ToStream()), System.Net.Mime.MediaTypeNames.Application.Zip) { Name = fp.Key }));
                    foreach (var f in faculty[true])
                    {
                        sendMail(f);
                    }
                }
                await parallelEmails.SendingComplete();
                messages.ForEach(m => m.Dispose());
                participantTimetable.Dispose();
                facultyTimetable?.Dispose();
            }
            return new EmailResult<CourseParticipant> {
                SuccessRecipients = success.ToArray(),
                FailRecipients = fail.ToArray()
            };
        }

        public static async Task<EmailResult<CourseFacultyInvite>> SendMultiInvites(ICollection<Guid> courseIds, ICollection<Guid> participantIds, IPrincipal currentPrincipal, MedSimDbContext repo)
        {
            var existing = await repo.CourseFacultyInvites.Where(cfi => courseIds.Contains(cfi.CourseId) && participantIds.Contains(cfi.ParticipantId))
                .ToHashSetAsync(cfi => Tuple.Create(cfi.CourseId, cfi.ParticipantId));
            existing.UnionWith((await repo.CourseParticipants
                .Where(cp => courseIds.Contains(cp.CourseId) && participantIds.Contains(cp.ParticipantId)).ToListAsync())
                .Select(cfi => Tuple.Create(cfi.CourseId, cfi.ParticipantId)));
            var allInvites = new List<CourseFacultyInvite>(courseIds.Count * participantIds.Count - existing.Count);
            foreach (var c in courseIds)
            {
                foreach (var p in participantIds)
                {
                    if (!existing.Contains(Tuple.Create(c, p)))
                    {
                        allInvites.Add(new CourseFacultyInvite { CourseId = c, ParticipantId = p });
                    }
                }
            }
            var success = new ConcurrentBag<CourseFacultyInvite>();
            var fail = new ConcurrentBag<CourseFacultyInvite>();

            var allCourseIds = allInvites.ToHashSet(i => i.CourseId);
            var courses = await repo.Courses.Include("CourseFormat.CourseType").Where(c => allCourseIds.Contains(c.Id)).ToDictionaryAsync(u=>u.Id);
            var allInvitees = allInvites.ToLookup(i => i.ParticipantId);
            var allInviteeIds = allInvitees.Select(i=>i.Key);
            var userRepo = (DbSet<Participant>)repo.Users;
            var users = await userRepo.Include("Department.Institution.Culture").Where(p => allInviteeIds.Contains(p.Id)).ToDictionaryAsync(u=>u.Id);

            var currentUser = await userRepo.Include("Department").Include("ProfessionalRole")
                .SingleAsync(u => u.UserName == currentPrincipal.Identity.Name);

            using (var parallelEmails = new ParallelSmtpEmails())
            {
                foreach (var g in allInvitees)
                {
                    var mail = new MailMessage();

                    var recipient = users[g.Key];
                    mail.To.AddParticipants(recipient);
                    var requestEmail = new MultiCourseInvite
                    {
                        PersonRequesting = currentUser,
                        Courses = g.Select(c => courses[c.CourseId]),
                        Recipient = recipient
                    };
                    mail.CreateHtmlBody(requestEmail);
                    parallelEmails.Send(mail, s => {
                        if (s == null)
                        {
                            foreach (var ci in g)
                            {
                                success.Add(ci);
                            }
                        }
                        else
                        {
                            foreach (var ci in g)
                            {
                                fail.Add(ci);
                            }
                        }
                    });
                }
                await parallelEmails.SendingComplete();
            }
            return new EmailResult<CourseFacultyInvite>
            {
                SuccessRecipients = success.ToArray(),
                FailRecipients = fail.ToArray()
            };
        }

        public static IEnumerable<Attachment> GetInviteReadings(Course course)
        {
            var now = DateTime.UtcNow;
            var relevantReadings = course.CourseFormat.CourseType.CandidatePrereadings
                .Where(cp => !cp.SendRelativeToCourse.HasValue || (course.StartFacultyUtc.AddDays(cp.SendRelativeToCourse.Value) <= now));
            return GetPrereadings(relevantReadings);
        }

        public static IEnumerable<Attachment> GetSupplementReadings(Course course, DateTime schedule)
        {
            var scheduleDay = schedule.Date;
            var relevantReadings = course.CourseFormat.CourseType.CandidatePrereadings
                .Where(cp => cp.SendRelativeToCourse.HasValue && (course.StartFacultyUtc.Date.AddDays(cp.SendRelativeToCourse.Value) == scheduleDay));
            return GetPrereadings(relevantReadings);
        }

        private static IEnumerable<Attachment> GetPrereadings(IEnumerable<CandidatePrereading> prereadings)
        {

            var readingDict = prereadings.ToDictionary(rr=>rr.FileName);
            var returnVar = new List<Attachment>();
            if (readingDict.Count == 0)
            {
                return returnVar;
            }

            var path = readingDict.Values.First().GetServerPath();
            using (ZipArchive archive = ZipFile.Open(path, ZipArchiveMode.Read))
            {
                foreach (var f in archive.Entries)
                {
                    CandidatePrereading cp;
                    if (readingDict.TryGetValue(f.Name, out cp))
                    {
                        returnVar.Add(new Attachment(f.Open(), f.Name));
                    }
                }
            }
            return returnVar;
        }

        public static void RescheduleReadings(Course course, MedSimDbContext context)
        {
            var existingJobs = context.CourseHangfireJobs.Where(ch=>ch.CourseId == course.Id).ToList();
            foreach (var ej in existingJobs)
            {
                BackgroundJob.Delete(ej.HangfireId);
                context.CourseHangfireJobs.Remove(ej);
            }
            context.SaveChanges();
            
            context.CourseHangfireJobs.AddRange(EnqueReading(course));
            context.SaveChanges();
        }

        public static IEnumerable<CourseHangfireJob> EnqueReading(Course course)
        {
            var now = DateTime.UtcNow;
            var dates = (from cp in course.CourseFormat.CourseType.CandidatePrereadings
                         where cp.SendRelativeToCourse.HasValue
                         let d = course.StartFacultyUtc.AddDays(cp.SendRelativeToCourse.Value)
                         where d > now
                         select d).ToHashSet();

            var returnVar = new List<CourseHangfireJob>();
            foreach (var d in dates)
            {
                returnVar.Add(new CourseHangfireJob
                {
                    HangfireId = BackgroundJob.Schedule(() => SendReading(course.Id, d, null), d),
                    CourseId = course.Id
                });
            }
            return returnVar;
        }

        public static void SendReading(Guid courseId, DateTime schedule,PerformContext context)
        {
            using (var db = new MedSimDbContext())
            {
                var course = db.Courses.Include("CourseFormat.CourseType.CandidatePrereadings").Include("CourseParticipants.Participant").Include("HangfireJobs")
                    .First(c=>c.Id == courseId);

                var readings = GetSupplementReadings(course, schedule);
                using (var mail = new MailMessage())
                {
                    mail.To.AddParticipants(from cp in course.CourseParticipants
                                            where !cp.IsFaculty
                                            select cp.Participant);
                    var confirmEmail = new CandidateReadingMessage
                    {
                        Course = course,
                    };
                    mail.CreateHtmlBody(confirmEmail);
                    foreach (var a in GetSupplementReadings(course, schedule))
                    {
                        mail.Attachments.Add(a);
                    }
                    using (var smtp = new SmtpClient())
                    {
                        smtp.Send(mail);
                    }
                }

                db.CourseHangfireJobs.RemoveRange(course.HangfireJobs.Where(hj=>hj.HangfireId == context.BackgroundJob.Id));
                db.SaveChanges();
            }
        }

        private static IEnumerable<KeyValuePair<string, string>> GetFilePaths(Course course)
        {
            return (from s in course.CourseSlotActivities
                    where s.ScenarioId.HasValue
                    let sr = s.Scenario.ScenarioResources.FirstOrDefault(ssr => ssr.FileName != null)
                    where sr != null
                    select new KeyValuePair<string, string>(s.Scenario.BriefDescription, sr.GetServerPath()))
                   .Concat(from ctr in course.CourseSlotActivities
                           let atr = ctr.Activity
                           where atr?.FileName != null
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
    public class EmailResult<T>
    {
        public IEnumerable<T> SuccessRecipients { get; set; }
        public IEnumerable<T> FailRecipients { get; set; }
    }
}