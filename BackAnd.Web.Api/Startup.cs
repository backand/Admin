using BackAnd.Web.Api.Providers;
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
using System.Collections.Generic;
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

            var context = new OwinContext(app.Properties);
            var token = context.Get<System.Threading.CancellationToken>("host.OnAppDisposing");

            // load singetone 
            var u = FarmCachingSingeltone.Instance.ToString();

            if (token != System.Threading.CancellationToken.None)
            {
                token.Register(() =>
                {
                    FarmCachingSingeltone.Instance.AppEnded();

                });
            }

            Analytics.Init();

            var properties = new Dictionary<string, object>();
            properties.Add("AppName", "aaa");

            //pass any properties through the Owin context Environment
            app.Use(typeof(PropertiesMiddleware), new object[] { properties });
        }

        public class PropertiesMiddleware : OwinMiddleware
        {
            Dictionary<string, object> _properties = null;

            public PropertiesMiddleware(OwinMiddleware next, Dictionary<string, object> properties)
                : base(next)
            {
                _properties = properties;
            }

            public async override Task Invoke(IOwinContext context)
            {
                if (_properties != null)
                {
                    foreach (var prop in _properties)
                        if (context.Get<object>(prop.Key) == null)
                        {
                            context.Set<object>(prop.Key, prop.Value);
                        }
                }

                await Next.Invoke(context);
            }
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
            //SetUpGithubAuth(app);
            //SetUpGoogleAuth(app);

            //SetUpFacebookAuth(app);
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
        private const string BEARER = "bearer";

        public override Task RequestToken(OAuthRequestTokenContext context)
        {
            try
            {
                SetMainLoggerInRequest();
                if (!context.Request.Headers.ContainsKey(AuthHeader))
                {
                    if (context.Request.QueryString.HasValue)
                    {
                        var queryString = HttpUtility.ParseQueryString(context.Request.QueryString.Value);
                        var token = queryString[key];
                        if (token != null)
                        {
                            if (token.Contains(BEARER))
                            {
                                token = token.Replace(BEARER, "").Trim();
                            }
                            context.Token = token;
                            context.Request.Headers.Add(key, new string[1] { queryString[key] });
                        }
                        else
                        {
                            string anonymousToken = queryString[Durados.Database.AnonymousToken];
                            if (anonymousToken != null)
                            {
                                context.Request.Headers.Add(Durados.Database.AnonymousToken, new string[1] { anonymousToken });
                            }
                        }
                    }
                }
            }
            catch (Exception exception)
            {
                Durados.Web.Mvc.Maps.Instance.DuradosMap.Logger.Log("Startup", "RequestToken", "", exception, 1, null);
                throw exception;
            }

            return Task.FromResult<object>(null);
        }

        private void SetMainLoggerInRequest()
        {
            try
            {
                System.Web.HttpContext.Current.Items[Durados.Database.MainLogger] = Durados.Web.Mvc.Maps.Instance.DuradosMap.Logger;
            }
            catch { }
        }
    }
}
