using Microsoft.VisualStudio.TestTools.UnitTesting;
using SM.Web.Controllers.Helpers;
using System.Collections.Generic;
using System.Web.Http.OData;
using System.Web.Http.OData.Builder;
using System.Web.Http.OData.Query;

namespace SimManager.Tests
{
    /// <summary>
    /// Summary description for TestOdatOptionsParsing
    /// </summary>
    [TestClass]
    public class TestOdatOptionsParsing
    {
        public TestOdatOptionsParsing()
        {
            //
            // TODO: Add constructor logic here
            //
        }

        private TestContext testContextInstance;

        /// <summary>
        ///Gets or sets the test context which provides
        ///information about and functionality for the current test run.
        ///</summary>
        public TestContext TestContext
        {
            get
            {
                return testContextInstance;
            }
            set
            {
                testContextInstance = value;
            }
        }

        #region Additional test attributes
        //
        // You can use the following additional attributes as you write your tests:
        //
        // Use ClassInitialize to run code before running the first test in the class
        // [ClassInitialize()]
        // public static void MyClassInitialize(TestContext testContext) { }
        //
        // Use ClassCleanup to run code after all tests in a class have run
        // [ClassCleanup()]
        // public static void MyClassCleanup() { }
        //
        // Use TestInitialize to run code before running each test 
        // [TestInitialize()]
        // public void MyTestInitialize() { }
        //
        // Use TestCleanup to run code after each test has run
        // [TestCleanup()]
        // public void MyTestCleanup() { }
        //
        #endregion

        [TestMethod]
        public void TestFilterVisitor()
        {
            ODataConventionModelBuilder builder = new ODataConventionModelBuilder();
            builder.EntitySet<Bar>("Bar");

            ODataQueryContext context = new ODataQueryContext(builder.GetEdmModel(), typeof(Bar));
            //ODataQueryOptions<Customer> query = new ODataQueryOptions<Customer>(context, new HttpRequestMessage(HttpMethod.Get, "http://server/?$top=10"));

            FilterQueryOption f = new FilterQueryOption("(startswith(BarString,'b') eq true) and (not (Foos/any(x1: x1/B/BarId eq 1)))", context);
            var fo = new FindAnyAllFilterOptions();
            fo.Find(f.FilterClause.Expression);
            CollectionAssert.AreEqual((List<string>)fo.Paths, new[] { "Foos.B" });
        }
    }
}
