using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SP.DataAccess;
using SP.DataAccess.Migrations;

namespace SP.Tests
{
    [TestClass]
    public class SqlHelperTests
    {
        [TestMethod]
        public void TestCreateUniqueConstraint()
        {
            Console.WriteLine(SqlHelpers.CreateUniqueConstraint<Scenario>("Scenarios", e => e.DepartmentId, e => e.BriefDescription));
        }
    }
}
