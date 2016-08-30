using System;
using Microsoft.AspNet.Identity;
using Microsoft.Owin;
using Microsoft.Owin.Security.Cookies;
using Microsoft.Owin.Security.Google;
using Microsoft.Owin.Security.OAuth;
using Owin;
using SP.Web.Providers;
using SP.Web.Models;
//using Owin.Security.Providers.LinkedIn;
using Microsoft.Owin.Security.Facebook;
using System.Threading.Tasks;
using SP.DataAccess;

namespace SP.Web
{
    public partial class Startup
    {
        public static OAuthAuthorizationServerOptions OAuthOptions { get; private set; }

        public static string PublicClientId { get; private set; }

        // For more information on configuring authentication, please visit http://go.microsoft.com/fwlink/?LinkId=301864
        public void ConfigureAuth(IAppBuilder app)
        {
            // Configure the db context and user manager to use a single instance per request
            app.CreatePerOwinContext(() => MedSimDbContext.Create());//CreateAdmin.Create(newDb);

            app.CreatePerOwinContext<ApplicationUserManager>(ApplicationUserManager.Create);

            // Enable the application to use a cookie to store information for the signed in user
            // and to use a cookie to temporarily store information about a user logging in with a third party login provider

            app.UseCookieAuthentication(new CookieAuthenticationOptions());
            app.UseExternalSignInCookie(DefaultAuthenticationTypes.ExternalCookie);

            // Configure the application for OAuth based flow
            PublicClientId = "SimPlanner";
            OAuthOptions = new OAuthAuthorizationServerOptions
            {
                TokenEndpointPath = new PathString("/Token"),
                Provider = new ApplicationOAuthProvider(PublicClientId),
                AuthorizeEndpointPath = new PathString("/api/Account/ExternalLogin"),
                AccessTokenExpireTimeSpan = TimeSpan.FromMinutes(90.0),
                // In production mode set AllowInsecureHttp = false
                AllowInsecureHttp = false
            };

            // Enable the application to use bearer tokens to authenticate users
            app.UseOAuthBearerTokens(OAuthOptions);

            // Uncomment the following lines to enable logging in with third party login providers
            //app.UseMicrosoftAccountAuthentication(
            //    clientId: "",
            //    clientSecret: "");

            /*app.UseTwitterAuthentication(
                consumerKey: "",
                consumerSecret: "");*/

            /*app.UseLinkedInAuthentication(
                clientId: "",
                clientSecret: "");*/

            app.UseFacebookAuthentication(
                new FacebookAuthenticationOptions { 
                    AppId = "1715030692085751",
                    AppSecret = "1a27773a799393e3571dec634b3a0487",
                    Scope = { "email" },
                    Provider = new FacebookAuthenticationProvider
                    {
                        OnAuthenticated = context =>
                        {
                            context.Identity.AddClaim(new System.Security.Claims.Claim("FacebookAccessToken", context.AccessToken));
                            return Task.FromResult(true);
                        }
                        /*, OnApplyRedirect = context =>
                        {
                            context.Response.Redirect(context.RedirectUri);
                        }, OnReturnEndpoint = context =>
                        {
                            return Task.FromResult(true);
                        }
                        */
                    }
                });

            app.UseGoogleAuthentication(new GoogleOAuth2AuthenticationOptions()
            {
                ClientId = "425898604892-b2rt4ta4lu0n2d8mi084jsigdgdjd016.apps.googleusercontent.com ",
                ClientSecret = "DQDWtwQgEAjVLyTrFHT0EBUC"
            });

        }
    }
}
