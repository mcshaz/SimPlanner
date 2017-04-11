using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SP.Dto.ProcessBreezeRequests;
using System.Linq;
using System.Diagnostics;

namespace SP.Tests
{
    [TestClass]
    public class PerformanceTests
    {
        [TestMethod]
        public void YieldReturnVsArray()
        {
            var yrTimer = new Stopwatch();
            var arTimer = new Stopwatch();
            foreach (var a in ISO3166.GetArray().Select(a => a.Alpha2).ToArray())
            {
                yrTimer.Start();
                for (int i = 0; i < 10000; i++)
                {
                    ISO3166.GetCollection().FirstOrDefault(y => y.Alpha2 == a);
                }
                yrTimer.Stop();
                Console.WriteLine("Search {0} Cumulative yield return: {1} ms", a, yrTimer.ElapsedMilliseconds);

                arTimer.Start();
                for (int i = 0; i < 10000; i++)
                {
                   ISO3166.GetArray().FirstOrDefault(y => y.Alpha2 == a);
                }
                arTimer.Stop();
                Console.WriteLine("Search {0} Cumulative array: {1} ms", a, arTimer.ElapsedMilliseconds);
            }

            //results:
            //Search ZW Cumulative yield return: 112389 ms
            //Search ZW Cumulative array: 53051 ms
            //cumulative time becomes faster for array from KZ onwards
        }
    }
}
