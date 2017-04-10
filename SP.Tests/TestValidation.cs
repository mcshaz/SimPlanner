using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SP.Dto.ProcessBreezeRequests;
using SP.DTOs.Utilities;
using System.Linq;

namespace SP.Tests
{
    [TestClass]
    public class TestValidation
    {
        //[TestMethod]
        public void AddDialCode()
        {
            const string sp = "            ";
            foreach (var i in ISO3166.GetCollection())
            {

                var dc = CountryDialCodes.GetDialCode(i.Alpha2);
                Console.WriteLine($"{sp}yield return new ISO3166Country(\"{i.Name}\", \"{i.Alpha2}\", \"{i.Alpha3}\", {i.NumericCode + (dc==null?string.Empty:", new [] {\"" + string.Join("\",\"",CountryDialCodes.GetDialCode(i.Alpha2)) + "\"}")});");
            }
            
        }
        //[TestMethod]
        public void DialCodeLength()
        {
            Console.WriteLine(ISO3166.GetCollection().Max(i => i.DialCodes?.Max(d => d.Length) ?? 0));
        }
    }
}
