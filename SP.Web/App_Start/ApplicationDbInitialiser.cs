using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using SM.Web.Models;
using SM.DataAccess;
using System.Data.Entity.Validation;

namespace SM.Web
{
    public static class ApplicationDbInitializer
    {
        public static void Seed(MedSimDbContext context)
        {
#if !DEBUG
            throw new NotImplementedException("this should not be being used in a production environment - security changes required");
            
#endif
            try
            {
                if (!context.Roles.Any())
                {
                    //not in production
                    //context.Database.ExecuteSqlCommand(TransactionalBehavior.DoNotEnsureTransaction,
                    //    "alter database [" + context.Database.Connection.Database + "] set single_user with rollback immediate");
                    //
                    var roleStore = new RoleStore<AspNetRole, Guid, AspNetUserRole>(context);
                    var roleManager = new RoleManager<AspNetRole, Guid>(roleStore);
                    var role = new AspNetRole
                    {
                        Id = Guid.NewGuid(),
                        Name = RoleConstants.Admin
                    };
                    roleManager.Create(role);
                }

                if (!context.Users.Any())
                {
                    var userStore = new CustomUserStore(context);
                    var userManager = new ApplicationUserManager(userStore);

                    var user = new AspNetUser
                    {
                        Email = "foo@bar.com",
                        UserName = "admin"
                    };
                    var result = userManager.Create(user, password: "Admin_1");
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
            catch (DbEntityValidationException ex)
            {
                // Retrieve the error messages as a list of strings.
                var errorMessages = ex.EntityValidationErrors
                        .SelectMany(x => x.ValidationErrors)
                        .Select(x => x.ErrorMessage);

                // Join the list to a single string.
                var fullErrorMessage = string.Join("; ", errorMessages);

                // Combine the original exception message with the new one.
                var exceptionMessage = string.Concat(ex.Message, " The validation errors are: ", fullErrorMessage);

                // Throw a new DbEntityValidationException with the improved exception message.
                throw new DbEntityValidationException(exceptionMessage, ex.EntityValidationErrors);
            }
        }
    }
    public class DbSeedException:Exception
    {
        public DbSeedException(IEnumerable<string> errors):
            base("An exception has occured seeding the database:" + string.Join(Environment.NewLine,errors))
        {}
    }
}
