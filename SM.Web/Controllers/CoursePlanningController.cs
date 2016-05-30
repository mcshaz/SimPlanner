using SM.DataAccess;
using SM.Web.Models;
using SM.Web.UserEmails;
using System;
using System.Linq;
using System.Net.Mail;
using System.Threading;
using System.Web.Http;

namespace SM.Web.Controllers
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
            Thread.CurrentThread.CurrentCulture = System.Globalization.CultureInfo.GetCultureInfo(course.Department.Institution.LocaleCode);

            var now = DateTime.UtcNow;
            if (course==null) { return NotFound(); }
            if (course.StartTime < now)
            {
                return Ok("The course start must be after now");
            }

            course.EmailSequence++;
            Repo.SaveChanges();

            using (var cal = Appointment.CreateiCalendar(course, User.Identity))
            {
                var faculty = course.CourseParticipants.Where(cp=>!cp.IsConfirmed.HasValue).ToLookup(cp => cp.IsFaculty);
                using (var client = new SmtpClient())
                {
                    
                    var sendMail = new Action<CourseParticipant>(cp => {
                        using (var mail = new MailMessage())
                        {
                            mail.To.Add(cp.Participant.Email);
                            if (cp.Participant.AlternateEmail != null)
                            {
                                mail.To.Add(cp.Participant.AlternateEmail);
                            }
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
                    if (course.FacultyMeetingTime.HasValue && course.FacultyMeetingTime > now)
                    {
                        Appointment.AddFacultyMeeting(cal.Ical, course);
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
            var cp = Repo.CourseParticipants.Find(new[] { model.CourseId, model.ParticipantId});
            if (cp==null)
            {
                return NotFound();
            }
            var returnString = "registered as " + (model.IsAttending ? "attending" : "unable to attend");
            if (!cp.IsConfirmed.HasValue)
            {
                cp.IsConfirmed = model.IsAttending;
                Repo.SaveChanges();
                return Ok(returnString);
            }
            if (model.IsAttending == cp.IsConfirmed) { return Ok("already " + returnString); }
            returnString = "had been confirmed as " + (cp.IsConfirmed.Value ? "attending" : "not attending")
                + "but would like to be changed to being " + returnString;
            //todo send email to organisers
            cp.IsConfirmed = null;

            return Ok("you " + returnString + ". An email will be sent to the course organiser(s), who will be able to confirm this change.");
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
