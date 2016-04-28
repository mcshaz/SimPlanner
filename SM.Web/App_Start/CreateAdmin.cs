using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using SM.DataAccess;
using SM.Dto;
using System;
using System.Collections.Generic;
using System.Linq;

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
                    Name = RoleConstants.AccessAllData
                };
                roleManager.Create(role);
                role = new AspNetRole
                {
                    Id = Guid.NewGuid(),
                    Name = RoleConstants.AccessInstitution
                };
                roleManager.Create(role);
                role = new AspNetRole
                {
                    Id = Guid.NewGuid(),
                    Name = RoleConstants.SiteAdmin
                };
                roleManager.Create(role);

                var userStore = new CustomUserStore(context);
                var userManager = new ApplicationUserManager(userStore);

                foreach(var user in context.Users.Where(u=>u.Department.Institution.Name== "Starship").ToList())
                {
                    var result = userManager.AddPassword(userId: user.Id, password: "Admin_1");
                    if (result.Succeeded)
                    {
                        userManager.AddToRole(user.Id, RoleConstants.AccessAllData);
                    }
                    else
                    {
                        throw new DbSeedException(result.Errors);
                    }
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
