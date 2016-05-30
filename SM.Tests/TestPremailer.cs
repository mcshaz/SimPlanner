using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using SM.Web.UserEmails;

namespace SimManager.Tests
{
    [TestClass]
    public class TestPremailer
    {
        [TestMethod]
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
            int indx = currentPath.LastIndexOf(@"\SM.Tests\");
            return currentPath.Substring(0, indx) + @"\SM.Web\UserEmails\";
        }
    }
}
