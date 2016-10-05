﻿using SP.DataAccess;
using SP.Dto.Utilities;
using SP.Web.Models;
using SP.Web.UserEmails;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net.Mail;
using System.Threading.Tasks;
using System.Web.Http;

namespace SP.Web.Controllers
{
    [Authorize]
    [RoutePrefix("api/CoursePlanning")]
    public class CoursePlanningController : ApiController
    {
        private MedSimDbContext _repository; // not populating in constructor as I believe this may be too early
        private MedSimDbContext Repo
        {
            get
            {
                return _repository ?? (_repository = new MedSimDbContext());
            }
        }

        //[Route("EmailAll")]
        [HttpPost]
        public async Task<IHttpActionResult> EmailAll(EmailAllBindingModel model)
        {
            var course = await Repo.Courses.Include("CourseParticipants.Participant").Include("CourseParticipants.Department.Institution").Include("CourseFormat.CourseType").Include("Room").Include("FacultyMeetingRoom")
                .FirstOrDefaultAsync(cp => cp.Id == model.CourseId);
            //Thread.CurrentThread.CurrentCulture = System.Globalization.CultureInfo.GetCultureInfo(course.Department.Institution.LocaleCode);

            var now = DateTime.UtcNow;
            if (course==null) { return NotFound(); }
            if (course.LastDay().StartUtc < now)
            {
                return Ok("The course finish must be after now");
            }
            course.EmailSequence++;
            course.EmailTimeStamp = DateTime.UtcNow;
            course.SystemChangesOnly = true;

            await SendEmail(course);

            await Repo.SaveChangesAsync();
            return Ok();
        }
        //move to a service layer
        private async Task SendEmail(Course course)
        {
            using (var cal = Appointment.CreateCourseAppointment(course, User.Identity))
            {
                using (var appt = new AppointmentStream(cal))
                {
                    var faculty = course.CourseParticipants.Where(cp => !cp.EmailTimeStamp.HasValue || cp.EmailTimeStamp.Value < course.LastModifiedUtc)
                        .ToLookup(cp => cp.IsFaculty);
                    IEnumerable<Attachment> attachments = new Attachment[0];
                    using (var client = new SmtpClient())
                    {
                        var mailMessages = new List<MailMessage>();
                        var sendMail = new Func<CourseParticipant, Task>(async (cp) =>
                        {
                            var mail = new MailMessage();
                            mailMessages.Add(mail);
                            mail.To.AddParticipants(cp.Participant);
                            var confirmEmail = new CourseInvite { CourseParticipant = cp };
                            mail.CreateHtmlBody(confirmEmail);
                            appt.AddAppointmentsTo(mail);
                            foreach (var a in attachments)
                            {
                                a.ContentStream.Position = 0;
                                mail.Attachments.Add(a);
                            }

                            await client.SendMailAsync(mail);
                        });

                        foreach (var cp in faculty[false])
                        {
                            await sendMail(cp);
                        }
                        if (faculty[true].Any())
                        {
                            if (course.FacultyMeetingUtc.HasValue && course.FacultyMeetingUtc > course.EmailTimeStamp)
                            {
                                using (var fm = Appointment.CreateFacultyCalendar(course))
                                {
                                    appt.Add(fm);
                                }
                            }
                            attachments = course.GetFilePaths().Select(fp => new Attachment(fp.Value, "application/zip") { Name = fp.Key })
                                .Concat(new[] { new Attachment(CreateDocxTimetable.CreateTimetableDocx(course, System.Web.Hosting.HostingEnvironment.MapPath("~/App_Data/CourseTimeTableTemplate.docx")), "application/vnd.openxmlformats-officedocument.wordprocessingml.document") { Name = $"Timetable for {course.CourseFormat.CourseType.Abbreviation}.docx" } });
                            foreach (var cp in faculty[true])
                            {
                                await sendMail(cp);
                            }
                        }
                        mailMessages.ForEach(mm => mm.Dispose());
                    }
                }
            }
        }

        [Route("Rsvp")]
        [HttpPost]
        [AllowAnonymous]
        public async Task<IHttpActionResult> Rsvp(RsvpBindingModel model)
        {
            var find = model.Auth.HasValue ? new[] { model.Auth.Value, model.ParticipantId }
                : new[] { model.ParticipantId };
            var cps = await Repo.CourseParticipants.Include("Participant").Include("Course")
                .Where(cp=>cp.CourseId == model.CourseId && find.Contains(cp.ParticipantId)).ToListAsync();

            var part = cps.First(cp => cp.ParticipantId == model.ParticipantId);

            CourseParticipant auth = null;
            if (model.Auth.HasValue)
            {
                auth = cps.First(cp => cp.ParticipantId == model.Auth && cp.IsOrganiser);
            }

            if (part.Course.StartUtc < DateTime.UtcNow)
            {
                return Ok("Confirmation status cannot be changed after the course has commenced.");
            }

            string userName = part.Participant.FullName;
            var returnString = "registered as " + (model.IsAttending ? "attending" : "unable to attend");
            if (model.IsAttending == part.IsConfirmed && !model.Auth.HasValue)
            {
                return Ok(userName + " is already " + returnString);
            }
            if (model.Auth.HasValue)
            {
                bool wasConfirmed = part.IsConfirmed.Value;
                part.IsConfirmed = model.IsAttending;
                Repo.SaveChanges();
                using (var client = new SmtpClient())
                {
                    using (var mail = new MailMessage())
                    {
                        mail.To.AddParticipants(part.Participant);
                        mail.CC.AddParticipants((from cp in part.Course.CourseParticipants
                                                    where cp.IsOrganiser
                                                    select cp.Participant).ToArray());
                        var confirmEmail = new AuthConfirmationResult
                        {
                            CourseParticipant = part,
                            Auth = auth.Participant,
                            IsChanged = wasConfirmed != model.IsAttending
                        };
                        mail.CreateHtmlBody(confirmEmail);
                        await client.SendMailAsync(mail);
                    }
                }
                return Ok(userName + " is now " + returnString + ". A confirmation email has been sent, with copies to the course organisers.");
            }
            if (!part.IsConfirmed.HasValue || part.IsOrganiser)
            {
                part.IsConfirmed = model.IsAttending;
                await Repo.SaveChangesAsync();
                return Ok(userName + " is now " + returnString);
            }

            using (var client = new SmtpClient())
            {
                foreach (var org in part.Course.CourseParticipants.Where(p=>p.IsOrganiser))
                {
                    using (var mail = new MailMessage())
                    {
                        mail.To.AddParticipants(org.Participant);
                        var confirmEmail = new ReverseConfirmation(org.ParticipantId.ToString()) { CourseParticipant = part };
                        mail.CreateHtmlBody(confirmEmail);
                        await client.SendMailAsync(mail);
                    }
                }
            }
            return Ok(userName + " had been confirmed as " + (part.IsConfirmed.Value ? "attending" : "not attending")
                + ", but would like to change to being " + returnString + ". An email has been sent to the course organiser(s), who will be able to confirm this change.");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (_repository != null)
                {
                    _repository.Dispose();
                }
            }
            base.Dispose(disposing);
        }
    }
}
