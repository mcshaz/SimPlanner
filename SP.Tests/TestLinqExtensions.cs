using System;
using System.Text;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SP.Dto.Utilities;
using System.Linq;

namespace SimPlanner.Tests
{
    /// <summary>
    /// Summary description for TestLinqExtensions
    /// </summary>
    [TestClass]
    public class TestLinqExtensions
    {
        public TestLinqExtensions()
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
        public void TestAllButLast()
        {
            var emptyInt = new int[0];
            CollectionAssert.AreEqual((new[] { 1, 2, 3, 4 }).AllButLast().ToList(), new[] { 1, 2, 3 });
            CollectionAssert.AreEqual((new[] { 1, 2 }).AllButLast().ToList(), new[] { 1 });
            CollectionAssert.AreEqual((new[] { 1 }).AllButLast().ToList(), emptyInt);
            CollectionAssert.AreEqual((emptyInt ).AllButLast().ToList(), emptyInt);
            //
            // TODO: Add test logic here
            //
        }
    }
}
