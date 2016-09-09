using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Net;
using System.Net.Configuration;
using System.Net.Http;
using System.Net.Mail;
using System.Threading.Tasks;
using System.Web;

namespace SP.Web.Controllers.Helpers
{
    public static class EmailHelpers
    {
        public static List<SmtpAttempt> SendTestEmails(HttpRequestMessage request, string to = "brent@focused-light.net")
        {
            var host = request.RequestUri.Host;

            host = "mail." + host;

            var smtpSection = (SmtpSection)ConfigurationManager.GetSection("system.net/mailSettings/smtp");

            var credentials = new NetworkCredential(smtpSection.Network.UserName, smtpSection.Network.Password);

            var ports = new[] { 25, 587 };

            var ssl = new[] { true };

            var capacity =  ports.Length * ssl.Length;

            var returnVar = new List<SmtpAttempt>(capacity);
            var taskList = new List<Task>(capacity);
            var smtpClientList = new List<SmtpClient>(capacity);

            foreach (var p in ports)
            {
                foreach (var s in ssl)
                {
                    var a = new SmtpAttempt
                    {
                        Host = host,
                        Port = p,
                        SSL = s
                    };

                    SmtpClient mailer = new SmtpClient(host, p);

                    mailer.UseDefaultCredentials = false;
                    mailer.Credentials = credentials;
                    mailer.EnableSsl = s;
                    try
                    {
                        returnVar.Add(a);
                        mailer.Send(smtpSection.From, to, "test email", "testing server settings\r\n" + a.ToString());
                        //var sm = mailer.SendMailAsync(smtpSection.From, to, "test email", "testing server settings\r\n" + a.ToString());
                        //taskList.Add(sm);
                        smtpClientList.Add(mailer);
                    }
                    catch(Exception e)
                    {
                        a.ExceptionMsg = e.Message;
                        LogErrorManually(e);
                        mailer.Dispose();
                    }
                }
            }
            try
            {
                // wait for all of them to complete
                //await Task.WhenAll(taskList); 
            }
            catch (Exception)
            {
                for (int i = 0; i< taskList.Count;i++)
                {
                    if (taskList[i].Exception != null)
                    {
                        var sa = returnVar[i];
                        sa.ExceptionMsg = taskList[i].Exception.Message;
                        LogErrorManually(taskList[i].Exception);
                    }
                }
            }
            smtpClientList.ForEach(s => s.Dispose());
            return returnVar;
        }

        private static void SmtpClientSendCompleted(object sender, AsyncCompletedEventArgs e)
        {
            var smtpClient = (SmtpClient)sender;
            var userAsyncState = (SmtpAttempt)e.UserState;
            smtpClient.SendCompleted -= SmtpClientSendCompleted;

            if (e.Error != null)
            {
                e.Error.Data.Add("Host", userAsyncState.Host);
                e.Error.Data.Add("Port", userAsyncState.Port);
                e.Error.Data.Add("SSL", userAsyncState.SSL);
                LogErrorManually(e.Error);
            }
            smtpClient.Dispose();
        }

        public static void LogErrorManually(Exception ex)
        {
            if (HttpContext.Current != null)//website is logging the error
            {
                var elmahCon = Elmah.ErrorSignal.FromCurrentContext();
                elmahCon.Raise(ex);
            }
            else//non website, probably an agent
            {
                var elmahCon = Elmah.ErrorLog.GetDefault(null);
                elmahCon.Log(new Elmah.Error(ex));
            }
        }

    }

    public class SmtpAttempt
    {
        public string Host { get; set; }
        public int Port { get; set; }
        public bool SSL { get; set; }
        public string ExceptionMsg { get; set; }
        public override string ToString()
        {
            return $"{Host}:{Port} SSL:{SSL}";
        }
    }
}