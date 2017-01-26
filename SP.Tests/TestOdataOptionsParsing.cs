using Microsoft.Data.OData.Query;
using Microsoft.Data.OData.Query.SemanticAst;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SP.DataAccess;
using SP.Web.Controllers.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http.OData;
using System.Web.Http.OData.Builder;
using System.Web.Http.OData.Query;

namespace SimPlanner.Tests
{
    /// <summary>
    /// Summary description for TestOdatOptionsParsing
    /// </summary>
    [TestClass]
    public class TestOdataOptionsParsing
    {
        public TestOdataOptionsParsing()
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

            FilterQueryOption f = new FilterQueryOption("startswith(BarString,'b') eq true", context);
            var pathsString = FindNavigationFilterOptions.GetPaths(f).ToList();
            CollectionAssert.AreEqual(pathsString, new string[0]);

            f = new FilterQueryOption("F/FooId eq 1", context);
            pathsString = FindNavigationFilterOptions.GetPaths(f).ToList();
            CollectionAssert.AreEqual(pathsString, new[] { "F" });

            f = new FilterQueryOption("Foos/any(x1: x1/B/BarId eq 1)", context);
            pathsString = FindNavigationFilterOptions.GetPaths(f).ToList();
            CollectionAssert.AreEqual(pathsString, new[] { "Foos.B" });

            f = new FilterQueryOption("(startswith(BarString,'b') eq true) and (not (Foos/any(x1: x1/B/BarId eq 1)))", context);
            pathsString = FindNavigationFilterOptions.GetPaths(f).ToList();
            CollectionAssert.AreEqual(pathsString, new[] { "Foos.B" });

            f = new FilterQueryOption("Foos/any(x1: x1/B/F/FooId eq 1)", context);
            pathsString = FindNavigationFilterOptions.GetPaths(f).ToList();
            CollectionAssert.AreEqual(pathsString, new[] { "Foos.B.F" });

            f = new FilterQueryOption("F/Bars/any(x1: x1/BarId eq 1)", context);
            pathsString = FindNavigationFilterOptions.GetPaths(f).ToList();
            CollectionAssert.AreEqual(pathsString, new[] { "F.Bars" });

            f = new FilterQueryOption("F/Bars/any(x1: ((x1/BarId eq 1) and (x1/BarString eq 'abc')) and (x1/F/FooId lt 3))", context);
            pathsString = FindNavigationFilterOptions.GetPaths(f).ToList();
            CollectionAssert.AreEqual(pathsString, new[] { "F.Bars.F" });



        }
    }


}
