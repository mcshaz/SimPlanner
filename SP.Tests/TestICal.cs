using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SP.DataAccess;
using SP.Web.UserEmails;
using SP.Web.Controllers.Helpers;
using System.Linq;
using System.IO;
using SP.Dto.Utilities;
using System.Security.Principal;

namespace SP.Tests
{
    [TestClass]
    public class TestICal
    {
        internal static IPrincipal GetTestAllRolesPrincipal()
        {
            return new GenericPrincipal(new GenericIdentity("brentm@adhb.govt.nz"), RoleConstants.RoleNames.Values.ToArray());
        }
        [TestMethod]
        public void TestCreateIcal()
        {
            using (var db = new MedSimDbContext())
            {
                var bm = GetTestAllRolesPrincipal();
                var course = db.Courses.Find(Guid.Parse("0ca5d24f-292e-4004-bb08-096db4b440ad"));
                using (var appt = new AppointmentStream())
                {
                    using (var cal = Appointment.CreateCourseAppointment(course, bm.Identity))
                    {
                        appt.Add(cal);
                        using (var fileStream = File.Create(AppDomain.CurrentDomain.BaseDirectory + "calendar.ics"))
                        {
                            appt.GetStreams().First().CopyTo(fileStream);
                        }
                    }
                    using (var cal = Appointment.CreateFacultyCalendar(course))
                    {
                        appt.Replace(cal);
                        using (var fileStream = File.Create(AppDomain.CurrentDomain.BaseDirectory + "calendar2Appt.ics"))
                        {
                            appt.GetStreams().First().CopyTo(fileStream);
                        }
                    }
                }

            }
        }
    }
}
