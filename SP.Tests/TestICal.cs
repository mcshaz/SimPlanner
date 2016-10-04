using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SP.DataAccess;
using SP.Web.UserEmails;
using SP.Web.Controllers.Helpers;
using System.Linq;
using System.IO;

namespace SP.Tests
{
    [TestClass]
    public class TestICal
    {

        [TestMethod]
        public void TestCreateIcal()
        {
            using (var db = new MedSimDbContext())
            {
                var bm = new RequestOnlyPrincipal("brentm@adhb.govt.nz", db.Roles.Select(r=>r.Name).ToList());
                var course = db.Courses.Find(Guid.Parse("0ca5d24f-292e-4004-bb08-096db4b440ad"));
                using (var cal = Appointment.CreateiCalendar(course, bm.Identity))
                {
                    string calString = cal.IcalString();
                    File.WriteAllText(AppDomain.CurrentDomain.BaseDirectory + "calendar.ics", calString);
                    Appointment.AddFacultyMeeting(cal.Cal, course);
                    File.WriteAllText(AppDomain.CurrentDomain.BaseDirectory + "calendar2Appt.ics", calString);
                }
            }
        }
    }
}
