
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.VisualStudio.TestTools.UnitTesting.Web;

namespace SimPlanner.Tests
{
    [TestClass]
    public class TestWebConfig
    {
        [TestMethod]
        //[HostType("ASP.NET")]
        //[UrlToTest("http://localhost:1234/upload_file.aspx")]
        //[AspNetDevelopmentServer(@"https://localhost:44300/", @"C:\Users\OEM\Documents\Visual Studio 2015\Projects\SimPlanner\SP.Web")]
        public void TestMetadataScriptWriter()
        {
            SP.Web.App_Start.MetadataScriptWriter.Write();
        }
    }
}
