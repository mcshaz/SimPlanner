using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Text.RegularExpressions;

namespace SimManager.Tests
{
    [TestClass]
    public class CourseNamingTests
    {
        [TestMethod]
        public void TestAcceptableCourseNames()
        {
            const string courseName = @"(\bcourse\b)|(\bsim(ulation)?\b)";
            foreach (var s in new[] { "xyz course", "xyz Course", "xyz Sim", "xyz simulation" })
            {
                Assert.IsTrue(Regex.IsMatch(s, courseName, RegexOptions.IgnoreCase));
            }
            foreach (var s in new[] { "xyz" })
            {
                Assert.IsFalse(Regex.IsMatch(s, courseName, RegexOptions.IgnoreCase));
            }
        }

    }
}
