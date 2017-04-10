using Hangfire;
using Hangfire.Dashboard;
using Microsoft.Owin;
using Owin;
using SP.DTOs.Utilities;

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
                Authorization = new[] { new HangfireAuthorizationFilter() },
                AppPath = System.Web.VirtualPathUtility.ToAbsolute("~")
            };
            app.UseHangfireDashboard("/hangfire", options);

            InitializeJobs();
        }

        public static void InitializeJobs()
        {
            RecurringJob.AddOrUpdate(() => AutomatedDbMaintenance.DeleteOrphans(), Cron.Daily(12));
        }
    }
}
