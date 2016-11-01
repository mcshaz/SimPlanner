using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.Cookies;
using Microsoft.Owin.Security.OAuth;
using SP.Web.Models;
using SP.DataAccess;

namespace SP.Web.Providers
{
    public class ApplicationOAuthProvider : OAuthAuthorizationServerProvider
    {
        private readonly string _publicClientId;

        public ApplicationOAuthProvider(string publicClientId)
        {
            if (publicClientId == null)
            {
                throw new ArgumentNullException("publicClientId");
            }

            _publicClientId = publicClientId;
        }

        public override async Task GrantResourceOwnerCredentials(OAuthGrantResourceOwnerCredentialsContext context)
        {
            var userManager = context.OwinContext.GetUserManager<ApplicationUserManager>();

            Participant user = await userManager.FindAsync(context.UserName, context.Password);

            if (user == null)
            {
                context.SetError("invalid_grant", "The user name or password is incorrect.");
                return;
            }
            if (!user.AdminApproved)
            {
                context.SetError("invalid_grant", "Your account is waiting for administrator approval.");
                return;
            }

            ClaimsIdentity oAuthIdentity = await user.GenerateUserIdentityAsync(userManager,
               OAuthDefaults.AuthenticationType);
            ClaimsIdentity cookiesIdentity = await user.GenerateUserIdentityAsync(userManager,
                CookieAuthenticationDefaults.AuthenticationType);

            AuthenticationProperties properties = CreateProperties(user, await userManager.GetRolesAsync(user.Id));
            AuthenticationTicket ticket = new AuthenticationTicket(oAuthIdentity, properties);
            context.Validated(ticket);
            context.Request.Context.Authentication.SignIn(cookiesIdentity);
        }

        public override Task TokenEndpoint(OAuthTokenEndpointContext context)
        {
            foreach (KeyValuePair<string, string> property in context.Properties.Dictionary)
            {
                context.AdditionalResponseParameters.Add(property.Key, property.Value);
            }

            return Task.FromResult<object>(null);
        }

        public override Task ValidateClientAuthentication(OAuthValidateClientAuthenticationContext context)
        {
            // Resource owner password credentials does not provide a client ID.
            if (context.ClientId == null)
            {
                context.Validated();
            }

            return Task.FromResult<object>(null);
        }

        /*
        public override Task RequestToken(OAuthRequestTokenContext context)
        {
            var value = context.Request.Query.Get("access_token");
            var value = context.Request.Headers.Get(_name);
            if (!string.IsNullOrEmpty(value))
            {
                context.Token = value;
            }
            return Task.FromResult<object>(null);
        }
        */

        public override Task ValidateClientRedirectUri(OAuthValidateClientRedirectUriContext context)
        {
            if (context.ClientId == _publicClientId)
            {
                Uri expectedRootUri = new Uri(context.Request.Uri, "/");

                if (context.RedirectUri.StartsWith(expectedRootUri.AbsoluteUri))
                {
                    context.Validated();
                }
            }

            return Task.FromResult<object>(null);
        }

        public override Task AuthorizationEndpointResponse(OAuthAuthorizationEndpointResponseContext context)
        {
            var props = context.OwinContext.Authentication.AuthenticationResponseGrant.Properties.Dictionary;
            foreach (var k in props.Keys)
            {
                if (k[0] != '.' && !string.Equals(k,"client_id",StringComparison.OrdinalIgnoreCase))
                {
                    context.AdditionalResponseParameters.Add(k, props[k]);
                }
            }
            return base.AuthorizationEndpointResponse(context);
        }

        public static AuthenticationProperties CreateProperties(Participant user, IEnumerable<string> roles)
        {
            IDictionary<string, string> data = new Dictionary<string, string>
            {
                { "userId", user.Id.ToString() },
                { "fullName", user.FullName },
                { "userRoles", string.Join(",",roles) },
                { "locale", user.Department.Institution.LocaleCode },
                { "departmentId", user.DefaultDepartmentId.ToString() }
            };
            return new AuthenticationProperties(data);
        }
    }
}