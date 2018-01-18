using System.Web.Http;
using Microsoft.Owin.Security.OAuth;
using System.Web.Http.ExceptionHandling;
using SP.Web.LogHelpers;

namespace SP.Web
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            // Web API configuration and services
            // Configure Web API to use only bearer token authentication.
            config.SuppressDefaultHostAuthentication();
            config.Filters.Add(new HostAuthenticationFilter(OAuthDefaults.AuthenticationType));
            //config.Filters.Add(new LoggingActionFilter());
            config.Services.Add(typeof(IExceptionLogger), new NLogExceptionLogger());

            // Web API routes
            config.MapHttpAttributeRoutes();

            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{action}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );
            /*
            config.Routes.MapHttpRoute(
                name: "DefaultVideo",
                routeTemplate: "api/{controller}/{ext}/{filename}"
            );
            */
            /*
            config.Routes.MapHttpRoute(
                name: "NotFound",
                routeTemplate: "{*path}",
                defaults: new { controller = "Error", action = "NotFound" }
            );
            */

            System.Net.ServicePointManager.ServerCertificateValidationCallback = CertificateValidationCallBack;
        }
        private static bool CertificateValidationCallBack(
            object sender,
            System.Security.Cryptography.X509Certificates.X509Certificate certificate,
            System.Security.Cryptography.X509Certificates.X509Chain chain,
            System.Net.Security.SslPolicyErrors sslPolicyErrors)
        {
            return (sslPolicyErrors == System.Net.Security.SslPolicyErrors.None
                || (sslPolicyErrors == System.Net.Security.SslPolicyErrors.RemoteCertificateNameMismatch
                    && certificate.Subject.Contains("CN=*.openhost.net.nz")));
        }

        static string _defaultTimetableTemplatePath;
        internal static string DefaultTimetableTemplatePath{
            get{
                return _defaultTimetableTemplatePath
                    ?? (_defaultTimetableTemplatePath = System.Web.Hosting.HostingEnvironment.MapPath("~/App_Data/CourseTimeTableTemplate.docx"));
            }    
        }

        /*
        static string _defaultCertificateTemplatePath;
        internal static string DefaultCertificateTemplatePath
        {
            get
            {
                return _defaultCertificateTemplatePath
                    ?? (_defaultCertificateTemplatePath = System.Web.Hosting.HostingEnvironment.MapPath("~/App_Data/CertificateTemplate.pptx"));
            }
        }
        */
    }
}
