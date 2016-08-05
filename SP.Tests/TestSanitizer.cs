using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Ganss.XSS;

namespace SP.Tests
{
    [TestClass]
    public class TestSanitizer
    {
        [TestMethod]
        public void TestSanitizeSafeString()
        {
            var sanitizer = new HtmlSanitizer();
            const string safe = "<i>hello</i> there";
            Assert.AreEqual(safe, sanitizer.Sanitize(safe));
        }
    }
}
