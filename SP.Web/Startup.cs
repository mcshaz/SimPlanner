using Microsoft.Owin;
using Owin;

[assembly: OwinStartup(typeof(SP.Web.Startup))]

namespace SP.Web
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
