using Microsoft.VisualStudio.TestTools.UnitTesting;
using SM.DataAccess;
using System.Data;
using System.Data.Entity.Validation;
using System.Diagnostics;
using System.Linq;

namespace ServerSideUnitTests
{
    [TestClass]
    public class TestEntitities
    {
        [TestMethod]
        public void TestCreate()
        {
            MedSimDbContext db = null;
            try 
            {
                db = new MedSimDbContext();
                Assert.IsTrue(db.Countries.Any());
            }
            catch(DataException e)
            {
                var inner = e.InnerException as DbEntityValidationException;
                if (inner!=null)
                {
                    Debug.Write(string.Join("\r\n", inner.EntityValidationErrors.Select(i=>string.Join(";",i.ValidationErrors.Select(v=>v.ErrorMessage)))));
                }
                throw;

            }
            finally
            {
                if (db!=null) db.Dispose();
            }
        }
        [TestMethod]
        public void TestMetadata()
        {
            //Console.Write(CreateMetaDataJSON.CreateMetadata());
        }
    }
}
