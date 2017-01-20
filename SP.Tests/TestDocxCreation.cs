using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using SP.DataAccess;
using SP.Dto.Utilities;
using System.Linq;

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
                var testId = Guid.Parse("f1afbbbb-b72f-43f4-8b36-7837fe8d1b80");
                course = CreateDocxTimetable.GetCourseIncludes(db).First(c=>c.Id == testId);
                
                using (var stream = CreateDocxTimetable.CreateFullTimetableDocx(course, templ))
                {
                    using (var fileStream = new FileStream("testCourseTimetable.docx", FileMode.Create))
                    {
                        stream.WriteTo(fileStream);
                    }
                }
            }

        }

        [TestMethod]
        public void TestSplitAndIncludeChar()
        {
            var anyOf = new[] { '\t', '\n' };
            CollectionAssert.AreEqual(new[] { "asdewrr" }, "asdewrr".SplitAndInclude(anyOf));
            CollectionAssert.AreEqual(new[] { "a" }, "a".SplitAndInclude(anyOf));
            CollectionAssert.AreEqual(new[] { "asdewrr","\t","\n" }, "asdewrr\t\n".SplitAndInclude(anyOf));
            CollectionAssert.AreEqual(new[] { "asdewr","\n","\t","r" }, "asdewr\n\tr".SplitAndInclude(anyOf));
            CollectionAssert.AreEqual(new[] { "as","\t","de","\n","wrr" }, "as\tde\nwrr".SplitAndInclude(anyOf));
            CollectionAssert.AreEqual(new[] { "\t","asdewrr","\n" }, "\tasdewrr\n".SplitAndInclude(anyOf));
            CollectionAssert.AreEqual(new string[0] , "".SplitAndInclude(anyOf));
            CollectionAssert.AreEqual(new[] { "\t" }, "\t".SplitAndInclude(anyOf));
        }

        [TestMethod]
        public void TestFirstWord()
        {
            string testString = null;
            Assert.AreEqual(null, testString.FirstWord());

            Assert.AreEqual(string.Empty, string.Empty.FirstWord());
            Assert.AreEqual(string.Empty, "   ".FirstWord());

            Assert.AreEqual("Hello", "Hello".FirstWord());
            Assert.AreEqual("Hello", "   Hello".FirstWord());
            Assert.AreEqual("Hello", "Hello   ".FirstWord());
            Assert.AreEqual("Hello", "  Hello   ".FirstWord());
            Assert.AreEqual("Hello", "Hello There".FirstWord());
            Assert.AreEqual("Hello", "  Hello There  ".FirstWord());
        }

        [TestMethod]
        public void TestSplitAndIncludeString()
        {
            var anyOf = new[] { "\t", "\n" };
            CollectionAssert.AreEqual(new[] { "asdewrr" }, "asdewrr".SplitAndInclude(anyOf));
            CollectionAssert.AreEqual(new[] { "a" }, "a".SplitAndInclude(anyOf));
            CollectionAssert.AreEqual(new[] { "asdewrr", "\t", "\n" }, "asdewrr\t\n".SplitAndInclude(anyOf));
            CollectionAssert.AreEqual(new[] { "asdewr", "\n", "\t", "r" }, "asdewr\n\tr".SplitAndInclude(anyOf));
            CollectionAssert.AreEqual(new[] { "as", "\t", "de", "\n", "wrr" }, "as\tde\nwrr".SplitAndInclude(anyOf));
            CollectionAssert.AreEqual(new[] { "\t", "asdewrr", "\n" }, "\tasdewrr\n".SplitAndInclude(anyOf));
            CollectionAssert.AreEqual(new string[0], "".SplitAndInclude(anyOf));
            CollectionAssert.AreEqual(new[] { "\t" }, "\t".SplitAndInclude(anyOf));

            anyOf = new[] { "\t\n" };
            CollectionAssert.AreEqual(new[] { "asdewrr", "\t\n" }, "asdewrr\t\n".SplitAndInclude(anyOf));
            CollectionAssert.AreEqual(new[] {  "\t\n", "asdewrr" }, "\t\nasdewrr".SplitAndInclude(anyOf));
            CollectionAssert.AreEqual(new[] { "asd", "\t\n", "ewrr" }, "asd\t\newrr".SplitAndInclude(anyOf));
        }

        [TestMethod]
        public void MultiToSingleWhitespace()
        {
            Assert.AreEqual("a r e", "a\r\n\t r     e".MultiToSingleWhitespace());
        }
    }
}
