using SM.DataAccess;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;

namespace SM.Web
{
    public class UserExposedContext : MedSimDbContext
    {
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Participant>()
                .Ignore(e => e.PasswordHash)
                .Ignore(e => e.AccessFailedCount)
                .Ignore(e => e.LockoutEnabled)
                .Ignore(e => e.LockoutEndDateUtc)
                .Ignore(e => e.SecurityStamp)
                .Ignore(e => e.TwoFactorEnabled);

            base.OnModelCreating(modelBuilder);
        }
    }
}
