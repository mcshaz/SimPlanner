using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using SP.DataAccess;
using SP.Dto.Utilities;

namespace SP.Tests
{
    [TestClass]
    public class TestDocxCreation
    {
        [TestMethod]
        public void TestCreateTimetableDocx()
        {
            const string templ = @"C:\Users\OEM\Documents\Visual Studio 2015\Projects\SimPlanner\SP.Web\App_Data\CourseTimeTableTemplate.docx";

            Course course;
            using (var db = new MedSimDbContext())
            {
                course = CreateDocxTimetable.GetCourseWithIncludes(Guid.Parse("b15b8615-5883-4438-983b-45bd5a4deea8"),db);
                
                using (var stream = CreateDocxTimetable.CreateTimetableDocx(course, templ))
                {
                    using (var fileStream = new FileStream("testCourseTimetable.docx", FileMode.Create))
                    {
                        stream.WriteTo(fileStream);
                    }
                }
            }

        }
    }
}
