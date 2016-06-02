using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using System;
using SM.DataAccess;

namespace SM.Web.Models
{
    // You can add profile data for the user by adding more properties to your ApplicationUser class, please visit http://go.microsoft.com/fwlink/?LinkID=317594 to learn more.
    public static class ApplicationUser 
    {
        public static async Task<ClaimsIdentity> GenerateUserIdentityAsync(this Participant usr,ApplicationUserManager manager, string authenticationType)
        {
            // Note the authenticationType must match the one defined in CookieAuthenticationOptions.AuthenticationType
            var userIdentity = await manager.CreateIdentityAsync(usr, authenticationType);
            // Add custom user claims here
            return userIdentity;
        }
    }
    /*
    public class MedSimDbContext : IdentityDbContext<ApplicationUser, CustomRole,
                Guid, CustomUserLogin, CustomUserRole, CustomUserClaim>
    {
        public MedSimDbContext()
            : base("DefaultConnection")
        {
        }

        static MedSimDbContext()
        {
            // Set the database intializer which is run once during application start
            // This seeds the database with admin user credentials and admin role
            System.Data.Entity.Database.SetInitializer(new ApplicationDbInitializer());
        }
        
        public static MedSimDbContext Create()
        {
            return new MedSimDbContext();
        }
    }
    */
}