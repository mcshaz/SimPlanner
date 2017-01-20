using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace SP.Tests
{
    [TestClass]
    public class CSharpImplementationTests
    {
        [TestMethod]
        public void TestDateComparison()
        {
            var testDate = new DateTime(2012, 3, 3, 0, 0, 0, DateTimeKind.Unspecified);
            Assert.AreEqual(DateTime.SpecifyKind(testDate, DateTimeKind.Utc), testDate);
        }
    }
}
