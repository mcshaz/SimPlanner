using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Jint;
using System.Linq;
using System.Diagnostics;

namespace SimManager.Tests
{
    [TestClass]
    public class JIntDateTests
    {
        [TestMethod]
        public void TestToDateStrings()
        {
            var testDates = new[] { new DateTime(2000,1,1), new DateTime(2000, 1, 1, 0, 15, 15, 15), new DateTime(1900, 1, 1, 0, 15, 15, 15), new DateTime(1999, 6, 1, 0, 15, 15, 15) };
            foreach(var tzId in new[] { "Pacific Standard Time", "New Zealand Standard Time", "India Standard Time" })
            {
                Debug.WriteLine(tzId);
                TimeZoneInfo tzi = TimeZoneInfo.FindSystemTimeZoneById(tzId); 
                var engine = new Engine(ctx => ctx.LocalTimeZone(tzi));
                foreach (var d in testDates)
                {
                    var td = new DateTimeOffset(d, tzi.GetUtcOffset(d));
                    engine.Execute(
                        string.Format("var d = new Date({0},{1},{2},{3},{4},{5},{6});", td.Year, td.Month - 1, td.Day, td.Hour, td.Minute, td.Second, td.Millisecond));
                    
                    Assert.AreEqual(td.ToString("ddd MMM dd yyyy HH:mm:ss 'GMT'zzz"), engine.Execute("d.toString();").GetCompletionValue().ToString());

                    Assert.AreEqual(td.UtcDateTime.ToString("yyyy-MM-dd'T'HH:mm:ss.fff'Z'"), engine.Execute("d.toISOString();").GetCompletionValue().ToString());
                }
            }
        }
    }
}
