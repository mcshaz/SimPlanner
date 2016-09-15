using Microsoft.Owin.Security.OAuth;
using System.Threading.Tasks;

namespace SP.Web
{
    internal class HeaderOrQueryStringOAuthBearerProvider : OAuthBearerAuthenticationProvider
    {
        public override Task RequestToken(OAuthRequestTokenContext context)
        {
            var value = context.Request.Headers["Authorization"].Substring("Bearer ".Length).Trim() 
                ?? context.Request.Query.Get("access_token");

            if (!string.IsNullOrEmpty(value))
            {
                context.Token = value;
            }

            return Task.FromResult<object>(null);
        }
    }

}