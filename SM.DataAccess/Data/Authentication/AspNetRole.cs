namespace SM.DataAccess
{
    using Microsoft.AspNet.Identity.EntityFramework;
    using System;

    public class AspNetRole: IdentityRole<Guid,AspNetUserRole>
    {
    }
}
