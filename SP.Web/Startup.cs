using Hangfire;
using Hangfire.Dashboard;
using Microsoft.Owin;
using Owin;

[assembly: OwinStartup(typeof(SP.Web.Startup))]

namespace SP.Web
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            
            GlobalConfiguration.Configuration.UseSqlServerStorage("DefaultConnection");
            app.UseHangfireServer();

            ConfigureAuth(app);

            var options = new DashboardOptions
            {
                Authorization = new[] { new HangfireAuthorizationFilter() }
            };
            app.UseHangfireDashboard("/hangfire", options);
        }
    }
}
