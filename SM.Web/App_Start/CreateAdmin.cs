using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using SM.DataAccess;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SM.Web.Models
{
    internal static class CreateAdmin
    {
        internal static void Create(MedSimDbContext context)
        {
#if !DEBUG
            throw new NotImplementedException("this should not be being used in a production environment - security changes required");
            
#endif
            if (!context.Roles.Any())
            {
                var roleStore = new RoleStore<AspNetRole,Guid,AspNetUserRole>(context);
                var roleManager = new RoleManager<AspNetRole,Guid>(roleStore);
                var role = new AspNetRole
                {
                    Id = Guid.NewGuid(),
                    Name = RoleConstants.Admin
                };
                roleManager.Create(role);

                var userStore = new CustomUserStore(context);
                var userManager = new ApplicationUserManager(userStore);

                var user = context.Users.FirstOrDefault(u=>u.Email=="brentm@adhb.govt.nz");
                var result = userManager.AddPassword(userId: user.Id, password: "Admin_1");
                if (result.Succeeded)
                {
                    userManager.AddToRole(user.Id, RoleConstants.Admin);
                }
                else
                {
                    throw new DbSeedException(result.Errors);
                }
            }


        }

    }
    [Serializable]
    public class DbSeedException : Exception
    {
        public DbSeedException(IEnumerable<string> errors) :
            base("An exception has occured seeding the database:" + string.Join(Environment.NewLine, errors))
        { }
    }
}
