using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Globalization;
using SP.DTOs.Utilities;
using System.Diagnostics;

namespace SP.Tests
{
    [TestClass]
    public class TestCultureExtensions
    {
        [TestMethod]
        public void TestCountryFullNames()
        {
            foreach (var c in CultureInfo.GetCultures(CultureTypes.AllCultures))
            {
                if (c.Name.IndexOf('-') != -1)
                {
                    Debug.WriteLine($"{CultureExtensions.GetCountryFullName(c)} - {c.Name} - {c.EnglishName}");
                }
                
            }
        }

    }
}
