using Ical.Net.Serialization.iCalendar.Serializers;
using SP.DataAccess;
using SP.Dto;
using SP.Dto.Utilities;
using SP.Web.Models;
using SP.Web.UserEmails;
using System;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net.Http;
using System.Net.Mail;
using System.Threading.Tasks;
using System.Web.Http;

namespace SP.Web.Controllers
{
    [Authorize]
    [RoutePrefix("api/CoursePlanning")]
    public class CoursePlanningController : StreamControllerBase
    {
        /*
        private MedSimDtoRepository _repository; // not populating in constructor as I believe this may be too early
        private MedSimDtoRepository Repo
        {
            get
            {
                return _repository ?? (_repository = new MedSimDtoRepository(User));
            }
        }
        */

        //[Route("EmailAll")]
        [HttpPost]
        public async Task<IHttpActionResult> EmailAll(EmailAllBindingModel model)
        {
            var course = await GetCourseIncludes(Repo)
                .FirstOrDefaultAsync(cp => cp.Id == model.CourseId);
            //Thread.CurrentThread.CurrentCulture = System.Globalization.CultureInfo.GetCultureInfo(course.Department.Institution.LocaleCode);

            if (course==null) { return NotFound(); }
            if (course.LastDay().StartUtc < DateTime.UtcNow)
            {
                return Ok("The course finish must be after now");
            }

            var result = await CreateParticipantEmails.SendEmail(course, User);

            DateTime now = DateTime.UtcNow;
            foreach (var cp in result.SuccessRecipients) {
                cp.EmailedUtc = now;
            }
            await Repo.SaveChangesAsync();

            return Ok(new
            {
                SuccessRecipients = result.SuccessRecipients.Select(sr=>sr.ParticipantId),
                FailRecipients = result.FailRecipients.Select(sr => sr.ParticipantId)
            });
        }

        public static DbQuery<Course> GetCourseIncludes(MedSimDbContext repo)
        {
            return CreateDocxTimetable.GetCourseIncludes(repo)
                .Include("CourseParticipants.Department.Institution.Culture").Include("Room").Include("FacultyMeetingRoom");
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
                await Repo.SaveChangesAsync();
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
                        var confirmEmail = new ReverseConfirmation
                        {
                            CourseParticipant = part,
                            AuthorizationToken = org.ParticipantId.ToString("N")
                        };
                        mail.CreateHtmlBody(confirmEmail);
                        await client.SendMailAsync(mail);
                    }
                }
            }
            return Ok(userName + " had been confirmed as " + (part.IsConfirmed.Value ? "attending" : "not attending")
                + ", but would like to change to being " + returnString + ". An email has been sent to the course organiser(s), who will be able to confirm this change.");
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<HttpResponseMessage> MyCalendar(string id)
        {
            const string calExt = ".ics";

            if (!id.EndsWith(calExt)) {
                id += calExt;
            }
            Guid userId = Guid.Parse(id.Substring(0, id.Length - calExt.Length));
            var courseParticipants = await (from cp in Repo.CourseParticipants.Include(cp=>cp.Course.Department.Institution.Culture)
                                            let c = cp.Course
                                            where cp.ParticipantId == userId && cp.IsConfirmed != false && DbFunctions.AddDays(c.StartUtc, c.CourseFormat.DaysDuration + 1) > DateTime.UtcNow
                                            select cp).ToListAsync();

            var evts = Appointment.MapCoursesToEvents(courseParticipants.Select(cp => cp.Course));
            evts.AddRange(from cp in courseParticipants
                          where cp.IsFaculty && cp.Course.FacultyMeetingUtc.HasValue
                          select Appointment.CreateFacultyMeetingEvent(cp.Course));
            var cal = Appointment.CreateCal(evts);
            var serializer = new CalendarSerializer();
            var stream = serializer.SerializeToString(cal).ToStream();
            //should work but truncating the stream at the moment - ? flush needed in the ical.net source code
            return StreamToResponse(stream, id);
        }
    }
}
