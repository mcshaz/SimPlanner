using Microsoft.Owin;
using Owin;

[assembly: OwinStartup(typeof(SM.Web.Startup))]

namespace SM.Web
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
