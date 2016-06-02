using SP.DataAccess;
using System.Security.Claims;
using System.Threading.Tasks;

namespace SP.Web.Models
{
    internal static class UserExtensions
    {
        internal static async Task<ClaimsIdentity> GenerateUserIdentityAsync(this Participant user, ApplicationUserManager manager, string authenticationType)
        {
            // Note the authenticationType must match the one defined in CookieAuthenticationOptions.AuthenticationType
            var userIdentity = await manager.CreateIdentityAsync(user, authenticationType);
            // Add custom user claims here
            return userIdentity;
        }
    }
}
