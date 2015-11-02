﻿using BackAnd.Web.Api.Providers;
using Durados.Web.Mvc.UI.Helpers;
using Microsoft.Owin;
using Microsoft.Owin.Security.Cookies;
using Microsoft.Owin.Security.Facebook;
using Microsoft.Owin.Security.OAuth;
using Newtonsoft.Json.Linq;
using Owin;
//using AngularJSAuthentication.API.Providers.BackAnd.Web.Api.Providers;
using Owin.Security.Providers.GitHub;
using System;
using System.Configuration;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;

[assembly: OwinStartup(typeof(BackAnd.Web.Api.Startup))]

namespace BackAnd.Web.Api
{
    public class Startup
    {
        //public const string ExternalCookieAuthenticationType = CookieAuthenticationDefaults.ExternalAuthenticationType;
        public const string ExternalCookieAuthenticationType = "External";
        public const string ExternalOAuthAuthenticationType = "ExternalToken";
        //public const string ExternalOAuthAuthenticationType = "ExternalToken";
        public static OAuthBearerAuthenticationOptions OAuthBearerOptions;
        //public const string ExternalCookieAuthenticationType = CookieAuthenticationDefaults.ExternalAuthenticationType;
        public void Configuration(IAppBuilder app)
        {

            HttpConfiguration config = new HttpConfiguration();

            ConfigureOAuth(app);

            WebApiConfig.Register(config);
            app.UseCors(Microsoft.Owin.Cors.CorsOptions.AllowAll);
            app.UseWebApi(config);

            FarmCaching.Instance.AppStarted();

            var context = new OwinContext(app.Properties);
            var token = context.Get<System.Threading.CancellationToken>("host.OnAppDisposing");
            if (token != System.Threading.CancellationToken.None)
            {
                token.Register(() =>
                {
                    FarmCaching.Instance.AppEnded();

                });
            }

            Analytics.Init();

        }

        public static OAuthAuthorizationServerOptions OAuthOptions { get; private set; }

        public static string PublicClientId = "self";

        public static CookieAuthenticationOptions CookieOptions { get; private set; }
        public void ConfigureOAuth(IAppBuilder app)
        {

            OAuthOptions = new OAuthAuthorizationServerOptions()
            {

                AllowInsecureHttp = true,
                TokenEndpointPath = new PathString("/token"),
                AccessTokenExpireTimeSpan = TimeSpan.FromDays(1),
                Provider = new SimpleAuthorizationServerProvider(),
                RefreshTokenProvider = new SimpleRefreshTokenProvider(),
            };

            OAuthBearerOptions = new OAuthBearerAuthenticationOptions
            {
                Provider = new OAuthTokenProvider()
            };

            app.UseOAuthBearerAuthentication(OAuthBearerOptions);

            // Token Generation
            app.UseOAuthAuthorizationServer(OAuthOptions);
            
            // Enable the application to use cookies to authenticate users
            CookieOptions = new CookieAuthenticationOptions();
            app.UseCookieAuthentication(CookieOptions);

            // Enable the application to use a cookie to store temporary information about a user logging in with a third party login provider
            app.UseExternalSignInCookie(ExternalCookieAuthenticationType);
            SetUpGithubAuth(app);
            SetUpGoogleAuth(app);

            SetUpFacebookAuth(app);
        }

        String XmlSchemaString = "http://schemas.xmlsoap.org/ws/2005/05/identity/claims";
       
        private void SetUpGithubAuth(IAppBuilder app)
        {
            if (ConfigurationManager.AppSettings["GithubClientId"] != null && ConfigurationManager.AppSettings["GithubClientSecret"] != null)
            {
                GitHubAuthenticationOptions options = new GitHubAuthenticationOptions
                {
                    ClientId = ConfigurationManager.AppSettings["GithubClientId"],
                    ClientSecret = ConfigurationManager.AppSettings["GithubClientSecret"],
                    //AuthorizationCallbackBaseURL = ConfigurationManager.AppSettings["GithubRedirectBaseUri"]
                };
                options.Scope.RemoveAt(0);
                options.Scope.Add("user:email");
                app.UseGitHubAuthentication(options);
            }
        }

        private void SetUpFacebookAuth(IAppBuilder app)
        {
            if (ConfigurationManager.AppSettings["FacebookAppId"] != null && ConfigurationManager.AppSettings["FacebookSecret"] != null)
            {
                var facebook = new FacebookAuthenticationOptions()
                {
                    AppId = ConfigurationManager.AppSettings["FacebookAppId"], //"1581103388791711",
                    AppSecret = ConfigurationManager.AppSettings["FacebookSecret"], //"6bb26860f2bcfaaee1e26a470f2b6d48",
                    Provider = new Microsoft.Owin.Security.Facebook.FacebookAuthenticationProvider()
                    {
                        OnAuthenticated = (context) =>
                        {
                            JObject wholeUser = context.User;

                            context.Identity.AddClaim(new System.Security.Claims.Claim("urn:facebook:access_token", context.AccessToken, XmlSchemaString, "Facebook"));
                            context.Identity.AddClaim(new System.Security.Claims.Claim("urn:facebook:email", context.Email, XmlSchemaString, "Facebook"));
                            return Task.FromResult(0);
                        }
                    }
                };

                facebook.Scope.Add("email");
                app.UseFacebookAuthentication(facebook);
            }
        }

        private static void SetUpGoogleAuth(IAppBuilder app)
        {

            if (ConfigurationManager.AppSettings["GoogleClientId"] != null && ConfigurationManager.AppSettings["GoogleClientSecret"] != null)

                app.UseGoogleAuthentication(
                        clientId: ConfigurationManager.AppSettings["GoogleClientId"],
                        clientSecret: ConfigurationManager.AppSettings["GoogleClientSecret"]
                        );
        }
    }

    public class OAuthTokenProvider : OAuthBearerAuthenticationProvider
    {
        private readonly Regex _bearerRegex = new Regex("((B|b)earer\\s)");
        private const string AuthHeader = "Authorization";

        /// <summary>
        /// By Default the Token will be searched for on the "Authorization" header.
        /// <para> pass additional getters that might return a token string</para>
        /// </summary>
        /// <param name="locations"></param>
        public OAuthTokenProvider()
        {
        }

        private const string key = "authorization";

        public override Task RequestToken(OAuthRequestTokenContext context)
        {
            if (!context.Request.Headers.ContainsKey(AuthHeader))
            {
                if (context.Request.QueryString.HasValue)
                {
                    var queryString = HttpUtility.ParseQueryString(context.Request.QueryString.Value);
                    var token = queryString[key];
                    context.Token = token;
                }
            }

            return Task.FromResult<object>(null);
        }
    }
}
