using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using System;
using System.Threading.Tasks;

namespace SP.Web.Controllers.Helpers
{
    public static class TimeSpanTokenExtensions
    {
        //a bit of a hack really
        public static async Task<bool> VerifyUserTokenAsync<TUser, TKey>(this UserManager<TUser, TKey> manager, TKey userId, string purpose, string token, TimeSpan tokenLifeSpan) 
            where TUser : class, IUser<TKey>
            where TKey : IEquatable<TKey>
        {
            var provider = (DataProtectorTokenProvider<TUser, TKey>)manager.UserTokenProvider;
            TimeSpan defaultSpan = provider.TokenLifespan;
            provider.TokenLifespan = tokenLifeSpan;
            var returnVar = await manager.VerifyUserTokenAsync(userId, purpose, token);
            provider.TokenLifespan = defaultSpan;
            return returnVar;
        }
    }
}