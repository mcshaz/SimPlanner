using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using SP.Web.UserEmails;

namespace SimPlanner.Tests
{
    [TestClass]
    public class TestPremailer
    {
        //commented out as need to alter method or MOQ request current
        //[TestMethod]
        public void TestEmailTemplate()
        {
            var template = new EmailTemplate() { Body = "Body", Title = "Title" };

            var text = template.TransformText();

            var pm = new PreMailer.Net.PreMailer(text);

            var path = GetEmailFolderPath() + "app.css";

            var css = File.ReadAllText(path);

            var il = pm.MoveCssInline(css: css);

            var res = il.Html;
        }

        string GetEmailFolderPath()
        {
            var currentPath = AppDomain.CurrentDomain.BaseDirectory;
            int indx = currentPath.LastIndexOf(@"\SP.Tests\");
            return currentPath.Substring(0, indx) + @"\SP.Web\UserEmails\";
        }
    }
}
