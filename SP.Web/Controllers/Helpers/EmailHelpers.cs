using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Configuration;
using System.Net.Http;
using System.Net.Mail;

namespace SP.Web.Controllers.Helpers
{
    public static class EmailHelpers
    {
        public static List<SmtpAttempt> SendTestEmails(HttpRequestMessage request, string to = "brent@focused-light.net")
        {
            var host = request.RequestUri.Host;
            List<string> list = new List<string>();
            try
            {
                list.AddRange(Dns.GetHostEntry(host).AddressList.Select(a => a.ToString()).ToList());
            }
            catch(Exception) { }

            host = "mail." + host;

            try
            {
                list.Add(host);
                list.AddRange(Dns.GetHostEntry(host).AddressList.Select(a => a.ToString()));
            }
            catch (Exception) { }

            var smtpSection = (SmtpSection)ConfigurationManager.GetSection("system.net/mailSettings/smtp");

            var credentials = new NetworkCredential(smtpSection.Network.UserName, smtpSection.Network.Password);

            var ports = new[] { 25, 143, 993, 110, 995, 587, 465 };

            var ssl = new[] { true, false };

            var returnVar = new List<SmtpAttempt>(list.Count * ports.Length* ssl.Length);

            foreach (var h in list)
            {
                foreach (var p in ports)
                {
                    foreach (var s in ssl)
                    {
                        var a = new SmtpAttempt
                        {
                            Host = h,
                            Port = p,
                            SSL = s
                        };
                        returnVar.Add(a);
                        try
                        {
                            using (SmtpClient mailer = new SmtpClient(h, p))
                            { 
                                mailer.Credentials = credentials;
                                mailer.EnableSsl = s;

                                mailer.Send(smtpSection.From, to, "test email", "testing server settings\r\n" + a.ToString());
                            }
                        }
                        catch (Exception e)
                        {
                            a.ExceptionMsg = e.Message;
                        }
                    }

                }

            }
            return returnVar;
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
            return $"{Host}:{Port} SSL:{SSL} Exception:{ExceptionMsg}";
        }
    }
}