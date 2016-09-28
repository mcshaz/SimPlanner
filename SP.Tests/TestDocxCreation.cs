using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using SP.DataAccess;
using SP.Dto.Utilities;
using System.Collections.Generic;

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
                course = CreateDocxTimetable.GetCourseWithIncludes(Guid.Parse("0ca5d24f-292e-4004-bb08-096db4b440ad"),db);
                
                using (var stream = CreateDocxTimetable.CreateTimetableDocx(course, templ))
                {
                    using (var fileStream = new FileStream("testCourseTimetable.docx", FileMode.Create))
                    {
                        stream.WriteTo(fileStream);
                    }
                }
            }

        }

        [TestMethod]
        public void TestSplitAndInclude()
        {
            var anyOf = new[] { '\t', '\n' };
            CollectionAssert.AreEqual(new[] { "asdewrr" }, (List<string>)"asdewrr".SplitAndInclude(anyOf));
            CollectionAssert.AreEqual(new[] { "asdewrr","\t","\n" }, (List<string>)"asdewrr\t\n".SplitAndInclude(anyOf));
            CollectionAssert.AreEqual(new[] { "asdewr","\n","\t","r" }, (List<string>)"asdewr\n\tr".SplitAndInclude(anyOf));
            CollectionAssert.AreEqual(new[] { "as","\t","de","\n","wrr" }, (List<string>)"as\tde\nwrr".SplitAndInclude(anyOf));
            CollectionAssert.AreEqual(new[] { "\t","asdewrr","\n" }, (List<string>)"\tasdewrr\n".SplitAndInclude(anyOf));
        }
    }
}
