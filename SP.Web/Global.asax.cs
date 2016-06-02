using System.Web.Http;
using System.Web.Mvc;
using System.Web.Routing;

namespace SP.Web
{
    public class WebApiApplication : System.Web.HttpApplication
    {
        protected void Application_Start(object sender, System.EventArgs e)
        {
            AreaRegistration.RegisterAllAreas();
            GlobalConfiguration.Configure(WebApiConfig.Register);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
        }
    }
}