using Ical.Net.Serialization.iCalendar.Serializers;
using Microsoft.AspNet.Identity.Owin;
using SP.DataAccess;
using SP.Dto;
using SP.Dto.Utilities;
using SP.Web.Controllers.Helpers;
using SP.Web.Models;
using SP.Web.UserEmails;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
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
        private ApplicationUserManager _userManager;
        private ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? (_userManager = Request.GetOwinContext().GetUserManager<ApplicationUserManager>());
            }
        }
        //[Route("EmailAll")]
        [HttpPost]
        public async Task<IHttpActionResult> EmailAll(EmailAllBindingModel model)
        {
            var course = await MailExtensions.GetCourseIncludes(Repo)
                .FirstOrDefaultAsync(cp => cp.Id == model.CourseId);
            //Thread.CurrentThread.CurrentCulture = System.Globalization.CultureInfo.GetCultureInfo(course.Department.Institution.LocaleCode);

            if (course==null) { return NotFound(); }
            if (course.LastDay().StartFacultyUtc < DateTime.UtcNow)
            {
                return Ok("The course finish must be after now");
            }

            var result = await CreateParticipantEmails.SendCourseEmail(course /*, User */, UserManager);

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

        [HttpPost]
        public async Task<IHttpActionResult> MultiInvite(MultiInviteBindingModel model)
        {
            var result = await CreateParticipantEmails.SendMultiInvites(model.Courses, model.Invitees, User, Repo);
            Repo.CourseFacultyInvites.AddRange(result.SuccessRecipients);
            await Repo.SaveChangesAsync();
            return Ok(result.SuccessRecipients);
        }

        [Route("Rsvp")]
        [HttpPost]
        [AllowAnonymous]
        public async Task<IHttpActionResult> Rsvp(RsvpBindingModel model)
        {
            var find = model.Auth.HasValue 
                ? new[] { model.Auth.Value, model.ParticipantId }
                : new[] { model.ParticipantId };
            var cps = await Repo.CourseParticipants.Include("Participant").Include("Course")
                .Where(cp=>cp.CourseId == model.CourseId && find.Contains(cp.ParticipantId)).ToListAsync();

            var part = cps.First(cp => cp.ParticipantId == model.ParticipantId);

            TimeSpan timeToStart = part.Course.StartParticipantUtc() - DateTime.UtcNow;
            string purpose = CreateParticipantEmails.InvitePurpose(model.CourseId);
            var verification = model.Auth.HasValue
                ? UserManager.VerifyUserTokenAsync(model.Auth.Value, purpose, model.Token, timeToStart)
                : UserManager.VerifyUserTokenAsync(model.ParticipantId, purpose, model.Token, timeToStart);
            if (!await verification)
            {
                return Unauthorized();
            }

            CourseParticipant auth = null;
            if (model.Auth.HasValue)
            {
                auth = cps.First(cp => cp.ParticipantId == model.Auth && cp.IsOrganiser);
            }

            if (part.Course.StartFacultyUtc < DateTime.UtcNow)
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
                            OrganiserId = org.ParticipantId,
                            Token = await UserManager.GenerateUserTokenAsync(purpose, org.ParticipantId),
                            CourseParticipant = part
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
                                            where cp.ParticipantId == userId && cp.IsConfirmed != false && DbFunctions.AddDays(c.StartFacultyUtc, c.CourseFormat.DaysDuration + 1) > DateTime.UtcNow
                                            select cp).ToListAsync();

            var evts = Appointment.MapCoursesToEvents(courseParticipants);
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
