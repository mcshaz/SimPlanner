using Microsoft.VisualStudio.TestTools.UnitTesting;
using SP.DataAccess;
using SP.DataAccess.Metadata.Attributes;
using SP.Metadata;
using System;
using System.ComponentModel.DataAnnotations;
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
        public void TestCreateDatabase()
        {
            MedSimDbContext db = null;
            try 
            {
                db = new MedSimDbContext();
                Assert.IsTrue(db.Cultures.Any(), "MedSimDbContext accessed, but no cultures seeded");
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
        public void TestFilenameAttribute()
        {
            var rxa = new ValidFileNameAttribute();
            Assert.IsFalse(rxa.IsValid("pptx."));
            Assert.IsFalse(rxa.IsValid("pp.tx."));
            Assert.IsFalse(rxa.IsValid("."));
            Assert.IsFalse(rxa.IsValid(".pp.tx"));
            Assert.IsFalse(rxa.IsValid(".pptx"));
            Assert.IsFalse(rxa.IsValid("pptx"));
            Assert.IsFalse(rxa.IsValid("a/abc.pptx"));
            Assert.IsFalse(rxa.IsValid("a\\abc.pptx"));
            Assert.IsFalse(rxa.IsValid("c:abc.pptx"));
            Assert.IsFalse(rxa.IsValid("c<abc.pptx"));
            Assert.IsTrue(rxa.IsValid("abc.pptx"));
            rxa = new ValidFileNameAttribute { AllowedExtensions = ".pptx" };
            Assert.IsFalse(rxa.IsValid("abc.docx"));
            Assert.IsTrue(rxa.IsValid("abc.pptx"));
        }
    }
}
