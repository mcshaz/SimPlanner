using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using System.Net.Mail;
using System.Text.RegularExpressions;
using System.Web;
using SP.Web.UserEmails;
using System.Linq;
using System.Net.Mime;
using SP.DataAccess;
using SP.Web.Controllers;
using System;

namespace SP.Tests
{
    [TestClass]
    public class EmailTests
    {
        [TestMethod]
        public void TestCourseInvite()
        {
            Course course;
            using (var context = new MedSimDbContext())
            {
                var courseQueryable = CoursePlanningController.GetCourseIncludes(context);
                course = courseQueryable.AsNoTracking().First(c => c.CourseParticipants.Any(cp=>cp.IsFaculty) && c.CourseParticipants.Any(cp => !cp.IsFaculty));
            }

            WriteMail(new CourseInvite { CourseParticipant = course.CourseParticipants.First() });
        }

        public void WriteMail(EmailBase template)
        {
            HttpContext.Current = new HttpContext(
                new HttpRequest(null, "https://sim-planner.com", null),
                new HttpResponse(new StringWriter())
                );

            // User is logged in
            HttpContext.Current.User = TestICal.GetTestAllRolesPrincipal();

            using (var msg = new MailMessage("info@sim-planner.com", HttpContext.Current.User.Identity.Name))
            {
                string dir = Path.GetFullPath("TestEmails");
                Directory.CreateDirectory(dir);
                msg.CreateHtmlBody(template);
                using (SmtpClient client = new SmtpClient("mysmtphost"))
                {
                    client.DeliveryMethod = SmtpDeliveryMethod.SpecifiedPickupDirectory;
                    client.PickupDirectoryLocation = dir;
                    client.Send(msg);
                }
                string fileName = ((object)template).GetType().Name;
                File.WriteAllText(Path.Combine(dir, fileName + ".txt"), msg.Body);
                var htmlStream = msg.AlternateViews.First(av => av.ContentType.MediaType == MediaTypeNames.Text.Html).ContentStream;
                using (var fileStream = File.Create(Path.Combine(dir, fileName + ".html")))
                {
                    htmlStream.Seek(0, SeekOrigin.Begin);
                    htmlStream.CopyTo(fileStream);
                }
            }

        }

    }
}
