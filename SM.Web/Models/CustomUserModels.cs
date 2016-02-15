using Microsoft.AspNet.Identity.EntityFramework;
using SM.DataAccess;
using System;
using System.Data.Entity;
using System.Threading.Tasks;

namespace SM.Web.Models
{

    public class CustomUserStore : UserStore<Participant, AspNetRole, Guid,
        AspNetUserLogin, AspNetUserRole, AspNetUserClaim>
    {
        public CustomUserStore(MedSimDbContext context)
            : base(context)
        {
        }

        public override Task<Participant> FindByEmailAsync(string email)
        {
            return ((MedSimDbContext)Context).Users
                    .FirstOrDefaultAsync(
                            u=>u.Email==email || u.AlternateEmail==email
                    );
        }

        public override Task CreateAsync(Participant user)
        {
            if (user.Id == Guid.Empty)
            {
                user.Id = Guid.NewGuid();
            }
            return base.CreateAsync(user);
        }
    }

}
