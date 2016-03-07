using Microsoft.AspNet.Identity.EntityFramework;
using SM.DataAccess;
using SM.MyIdentityProviders;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SM.Web.Models
{

    public class CustomUserStore : MyUserStore<ApplicationUser, CustomRole, Guid,
        CustomUserLogin, CustomUserRole, CustomUserClaim>
    {
        public CustomUserStore(MedSimDbContext context)
            : base(context)
        {
        }

        public override Task<ApplicationUser> FindByEmailAsync(string email)
        {
            return ((MedSimDbContext)Context).Users
                .FirstOrDefaultAsync(
                        u=>u.Email==email || u.AlternateEmail==email
                );
        }

        public override Task CreateAsync(ApplicationUser user)
        {
            if (user.Id == Guid.Empty)
            {
                user.Id = Guid.NewGuid();
            }
            return base.CreateAsync(user);
        }
    }

    public class CustomRoleStore : RoleStore<AspNetRole, Guid,>
    {
        public CustomRoleStore(MedSimDbContext context)
            : base(context)
        {
        }
    }
}
