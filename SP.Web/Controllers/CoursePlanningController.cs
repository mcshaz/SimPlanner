using SP.DataAccess;
using SP.Web.Models;
using SP.Web.UserEmails;
using System;
using System.Linq;
using System.Net.Mail;
using System.Threading;
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
        public IHttpActionResult EmailAll(EmailAllBindingModel model)
        {
            var course = Repo.Courses.Include("CourseParticipants.Participant").Include("CourseFormat.CourseType").Include("Department.Institution").Include("Room").Include("FacultyMeetingRoom")
                .FirstOrDefault(cp => cp.Id == model.CourseId);
            //Thread.CurrentThread.CurrentCulture = System.Globalization.CultureInfo.GetCultureInfo(course.Department.Institution.LocaleCode);

            var now = DateTime.UtcNow;
            if (course==null) { return NotFound(); }
            if (course.StartTimeUtc < now)
            {
                return Ok("The course start must be after now");
            }

            course.EmailSequence++;
            Repo.SaveChanges();

            using (var cal = Appointment.CreateiCalendar(course, User.Identity))
            {
                var faculty = course.CourseParticipants.Where(cp=>!cp.IsConfirmed.HasValue || cp.IsOrganiser).ToLookup(cp => cp.IsFaculty);
                using (var client = new SmtpClient())
                {
                    
                    var sendMail = new Action<CourseParticipant>(cp => {
                        using (var mail = new MailMessage())
                        {
                            mail.To.AddParticipants(cp.Participant);
                            var confirmEmail = new CourseInvite { CourseParticipant = cp };
                            mail.CreateHtmlBody(confirmEmail);
                            cal.AddAppointment(mail);
                            client.Send(mail);
                        }
                    });

                    foreach (var cp in faculty[false])
                    {
                        sendMail(cp);
                    }
                    if (course.FacultyMeetingTimeUtc.HasValue && course.FacultyMeetingTimeUtc > now)
                    {
                        Appointment.AddFacultyMeeting(cal.Cal, course);
                    }
                    foreach (var cp in faculty[true])
                    {
                        sendMail(cp);
                    }
                }
            }
            return Ok();
        }

        [Route("Rsvp")]
        [HttpPost]
        [AllowAnonymous]
        public IHttpActionResult Rsvp(RsvpBindingModel model)
        {
            var find = model.Auth.HasValue ? new[] { model.Auth.Value, model.ParticipantId }
                : new[] { model.ParticipantId };
            var cps = Repo.CourseParticipants.Include("Participant").Include("Course").Where(cp=>cp.CourseId == model.CourseId && find.Contains(cp.ParticipantId)).ToList();

            var part = cps.First(cp => cp.ParticipantId == model.ParticipantId);

            CourseParticipant auth = null;
            if (model.Auth.HasValue)
            {
                auth = cps.First(cp => cp.ParticipantId == model.Auth && cp.IsOrganiser);
            }

            if (part.Course.StartTimeUtc < DateTime.UtcNow)
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
                        client.Send(mail);
                    }
                }
                return Ok(userName + " is now " + returnString + ". A confirmation email has been sent, with copies to the course organisers.");
            }
            if (!part.IsConfirmed.HasValue || part.IsOrganiser)
            {
                part.IsConfirmed = model.IsAttending;
                Repo.SaveChanges();
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
                        client.Send(mail);
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
