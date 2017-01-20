using Hangfire.Dashboard;
using SP.DataAccess;
using System.Web;

namespace SP.Web
{
    internal class HangfireAuthorizationFilter : IDashboardAuthorizationFilter
    {
        public bool Authorize(DashboardContext context)
        {
            return HttpContext.Current.User.IsInRole(RoleConstants.SiteAdmin);
        }
    }
}