using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.Owin;
using Microsoft.Owin.Infrastructure;
using Microsoft.Owin.Logging;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.Infrastructure;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Threading;
using System.Configuration;

namespace Owin.Security.Providers.GitHub
{
    public class GitHubAuthenticationHandler : AuthenticationHandler<GitHubAuthenticationOptions>
    {
        private const string XmlSchemaString = "http://www.w3.org/2001/XMLSchema#string";

        private readonly ILogger logger;
        private readonly HttpClient httpClient;

        public static string RedirectUri = ConfigurationManager.AppSettings["MvcServerAddress"]  +  @"/account/signingithubback";
        public static GitHubAuthenticationOptions sOption;
        public static HttpClient sHttpClient;
        public GitHubAuthenticationHandler(HttpClient httpClient, ILogger logger)
        {
            this.httpClient = httpClient;
            this.logger = logger;
        }

        public GitHubAuthenticationHandler()
        {
            this.httpClient = sHttpClient;
        }

        public AuthenticationTicket ChallengeResult(string code, string state)
        {
            //var tt = new GitHubAuthenticationOptions();
            var properties = sOption.StateDataFormat.Unprotect(state);
            if (properties == null)
            {
                return null;
            }
            
            // OAuth2 10.12 CSRF
            //if (!ValidateCorrelationId(properties, logger))
//            {
//                return null;
//            }


            string redirectUri = RedirectUri;// requestPrefix + Request.PathBase + Options.CallbackPath;

            // Build up the body for the token request
            var body = new List<KeyValuePair<string, string>>();
            body.Add(new KeyValuePair<string, string>("code", code));
            body.Add(new KeyValuePair<string, string>("redirect_uri", redirectUri));
            body.Add(new KeyValuePair<string, string>("client_id", sOption.ClientId));
            body.Add(new KeyValuePair<string, string>("client_secret", sOption.ClientSecret));

            // Request the token
            var requestMessage = new HttpRequestMessage(HttpMethod.Post, sOption.Endpoints.TokenEndpoint);
            requestMessage.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            requestMessage.Content = new FormUrlEncodedContent(body);
            HttpResponseMessage tokenResponse =  httpClient.SendAsync(requestMessage).Result;
            tokenResponse.EnsureSuccessStatusCode();
            string text = tokenResponse.Content.ReadAsStringAsync().Result;

            // Deserializes the token response
            dynamic response = JsonConvert.DeserializeObject<dynamic>(text);
            string accessToken = (string)response.access_token;

            // Get the GitHub user
            HttpRequestMessage userRequest = new HttpRequestMessage(HttpMethod.Get, sOption.Endpoints.UserInfoEndpoint + "?access_token=" + Uri.EscapeDataString(accessToken));
            userRequest.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            var aaa = new CancellationToken();
            HttpResponseMessage userResponse = httpClient.SendAsync(userRequest, aaa).Result;
            userResponse.EnsureSuccessStatusCode();
            text =  userResponse.Content.ReadAsStringAsync().Result;
            JObject user = JObject.Parse(text);

            // Get the GitHub email
            HttpRequestMessage emailRequest = new HttpRequestMessage(HttpMethod.Get, sOption.Endpoints.EmailEndpoint + "?access_token=" + Uri.EscapeDataString(accessToken));
            emailRequest.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            HttpResponseMessage emailResponse =  httpClient.SendAsync(emailRequest, aaa).Result;
            emailResponse.EnsureSuccessStatusCode();
            text =  emailResponse.Content.ReadAsStringAsync().Result;

            var allEmails = JArray.Parse(text).Children<JObject>();

            string primaryEmail = "";
            foreach (var email in allEmails)
            {
                if (GitHubAuthenticatedContext.TryGetValue(email, "primary") == "True")
                {
                    primaryEmail = GitHubAuthenticatedContext.TryGetValue(email, "email");
                    break;
                }
            }
            var context = new GitHubAuthenticatedContext(Context, user, accessToken);
            context.Identity = new ClaimsIdentity(
                sOption.AuthenticationType,
                ClaimsIdentity.DefaultNameClaimType,
                ClaimsIdentity.DefaultRoleClaimType);
            if (!string.IsNullOrEmpty(context.Id))
            {
                context.Identity.AddClaim(new Claim(ClaimTypes.NameIdentifier, context.Id, XmlSchemaString, sOption.AuthenticationType));
            }
            if (!string.IsNullOrEmpty(context.UserName))
            {
                context.Identity.AddClaim(new Claim(ClaimsIdentity.DefaultNameClaimType, context.UserName, XmlSchemaString, sOption.AuthenticationType));
            }
            if (!string.IsNullOrEmpty(context.Name))
            {
                context.Identity.AddClaim(new Claim("urn:github:name", context.Name, XmlSchemaString, sOption.AuthenticationType));
            }
            if (!string.IsNullOrEmpty(context.Link))
            {
                context.Identity.AddClaim(new Claim("urn:github:url", context.Link, XmlSchemaString, sOption.AuthenticationType));
            }
            if (!string.IsNullOrEmpty(primaryEmail))
            {
                context.Identity.AddClaim(new Claim(ClaimTypes.Email, primaryEmail, XmlSchemaString, sOption.AuthenticationType));
            }
            context.Properties = properties;

            return new AuthenticationTicket(context.Identity, context.Properties);

        }

        protected override async Task<AuthenticationTicket> AuthenticateCoreAsync()
        {
            AuthenticationProperties properties = null;

            try
            {
                string code = null;
                string state = null;

                IReadableStringCollection query = Request.Query;
                IList<string> values = query.GetValues("code");
                if (values != null && values.Count == 1)
                {
                    code = values[0];
                }
                values = query.GetValues("state");
                if (values != null && values.Count == 1)
                {
                    state = values[0];
                }

                properties = Options.StateDataFormat.Unprotect(state);
/*                if (properties == null)
                {
                    return null;
                }

                // OAuth2 10.12 CSRF
                if (!ValidateCorrelationId(properties, logger))
                {
                    return new AuthenticationTicket(null, properties);
                }*/

                string requestPrefix = Request.Scheme + "://" + Request.Host;
                string redirectUri = requestPrefix + Request.PathBase + Options.CallbackPath;

                // Build up the body for the token request
                var body = new List<KeyValuePair<string, string>>();
                body.Add(new KeyValuePair<string, string>("code", code));
                body.Add(new KeyValuePair<string, string>("redirect_uri", redirectUri));
                body.Add(new KeyValuePair<string, string>("client_id", Options.ClientId));
                body.Add(new KeyValuePair<string, string>("client_secret", Options.ClientSecret));

                // Request the token
                var requestMessage = new HttpRequestMessage(HttpMethod.Post, Options.Endpoints.TokenEndpoint);
                requestMessage.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                requestMessage.Content = new FormUrlEncodedContent(body);
                HttpResponseMessage tokenResponse = await httpClient.SendAsync(requestMessage);
                tokenResponse.EnsureSuccessStatusCode();
                string text = await tokenResponse.Content.ReadAsStringAsync();

                // Deserializes the token response
                dynamic response = JsonConvert.DeserializeObject<dynamic>(text);
                string accessToken = (string)response.access_token;

                // Get the GitHub user
                HttpRequestMessage userRequest = new HttpRequestMessage(HttpMethod.Get, Options.Endpoints.UserInfoEndpoint + "?access_token=" + Uri.EscapeDataString(accessToken));
                userRequest.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                HttpResponseMessage userResponse = await httpClient.SendAsync(userRequest, Request.CallCancelled);
                userResponse.EnsureSuccessStatusCode();
                text = await userResponse.Content.ReadAsStringAsync();
                JObject user = JObject.Parse(text);

                // Get the GitHub email
                HttpRequestMessage emailRequest = new HttpRequestMessage(HttpMethod.Get, Options.Endpoints.EmailEndpoint + "?access_token=" + Uri.EscapeDataString(accessToken));
                emailRequest.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                HttpResponseMessage emailResponse = await httpClient.SendAsync(emailRequest, Request.CallCancelled);
                emailResponse.EnsureSuccessStatusCode();
                text = await emailResponse.Content.ReadAsStringAsync();

                var allEmails = JArray.Parse(text).Children<JObject>();

                string primaryEmail = "";
                foreach (var email in allEmails)
                {
                    if (GitHubAuthenticatedContext.TryGetValue(email,"primary") == "True")
                    {
                        primaryEmail = GitHubAuthenticatedContext.TryGetValue(email,"email");
                        break;
                    }
                }
                var context = new GitHubAuthenticatedContext(Context, user, accessToken);
                context.Identity = new ClaimsIdentity(
                    Options.AuthenticationType,
                    ClaimsIdentity.DefaultNameClaimType,
                    ClaimsIdentity.DefaultRoleClaimType);
                if (!string.IsNullOrEmpty(context.Id))
                {
                    context.Identity.AddClaim(new Claim(ClaimTypes.NameIdentifier, context.Id, XmlSchemaString, Options.AuthenticationType));
                }
                if (!string.IsNullOrEmpty(context.UserName))
                {
                    context.Identity.AddClaim(new Claim(ClaimsIdentity.DefaultNameClaimType, context.UserName, XmlSchemaString, Options.AuthenticationType));
                }
                if (!string.IsNullOrEmpty(context.Name))
                {
                    context.Identity.AddClaim(new Claim("urn:github:name", context.Name, XmlSchemaString, Options.AuthenticationType));
                }
                if (!string.IsNullOrEmpty(context.Link))
                {
                    context.Identity.AddClaim(new Claim("urn:github:url", context.Link, XmlSchemaString, Options.AuthenticationType));
                }
                if (!string.IsNullOrEmpty(primaryEmail))
                {
                    context.Identity.AddClaim(new Claim(ClaimTypes.Email,primaryEmail, XmlSchemaString, Options.AuthenticationType));
                }
                context.Properties = properties;

                await Options.Provider.Authenticated(context);

                return new AuthenticationTicket(context.Identity, context.Properties);
            }
            catch (Exception ex)
            {
                logger.WriteError(ex.Message);
            }
            return new AuthenticationTicket(null, properties);
        }

        protected override Task ApplyResponseChallengeAsync()
        {
            if (Response.StatusCode != 401)
            {
                return Task.FromResult<object>(null);
            }

            AuthenticationResponseChallenge challenge = Helper.LookupChallenge(Options.AuthenticationType, Options.AuthenticationMode);

            if (challenge != null)
            {
                string baseUri =
                    Request.Scheme +
                    Uri.SchemeDelimiter +
                    Request.Host +
                    Request.PathBase;

                string currentUri =
                    baseUri +
                    Request.Path +
                    Request.QueryString;

                string redirectUri = RedirectUri;

                AuthenticationProperties properties = challenge.Properties;
                if (string.IsNullOrEmpty(properties.RedirectUri))
                {
                    properties.RedirectUri = currentUri;
                }

                // OAuth2 10.12 CSRF
                GenerateCorrelationId(properties);

                // comma separated
                string scope = string.Join(",", Options.Scope);

                string state = Options.StateDataFormat.Protect(properties);

                

                string authorizationEndpoint =
                    Options.Endpoints.AuthorizationEndpoint +
                        "?client_id=" + Uri.EscapeDataString(Options.ClientId) +
                        "&redirect_uri=" + Uri.EscapeDataString(redirectUri) +
                        "&scope=" + Uri.EscapeDataString(scope) +
                        "&state=" + Uri.EscapeDataString(state);

                Response.Redirect(authorizationEndpoint);
            }

            return Task.FromResult<object>(null);
        }

        public override async Task<bool> InvokeAsync()
        {
            return await InvokeReplyPathAsync();
        }

        private async Task<bool> InvokeReplyPathAsync()
        {
            if (Options.CallbackPath.HasValue && Options.CallbackPath == Request.Path)
            {
                // TODO: error responses

                AuthenticationTicket ticket = await AuthenticateAsync();
                if (ticket == null)
                {
                    logger.WriteWarning("Invalid return state, unable to redirect.");
                    Response.StatusCode = 500;
                    return true;
                }

                var context = new GitHubReturnEndpointContext(Context, ticket);
                context.SignInAsAuthenticationType = Options.SignInAsAuthenticationType;
                context.RedirectUri = ticket.Properties.RedirectUri;

                await Options.Provider.ReturnEndpoint(context);

                if (context.SignInAsAuthenticationType != null &&
                    context.Identity != null)
                {
                    ClaimsIdentity grantIdentity = context.Identity;
                    if (!string.Equals(grantIdentity.AuthenticationType, context.SignInAsAuthenticationType, StringComparison.Ordinal))
                    {
                        grantIdentity = new ClaimsIdentity(grantIdentity.Claims, context.SignInAsAuthenticationType, grantIdentity.NameClaimType, grantIdentity.RoleClaimType);
                    }
                    Context.Authentication.SignIn(context.Properties, grantIdentity);
                }

                if (!context.IsRequestCompleted && context.RedirectUri != null)
                {
                    string redirectUri = context.RedirectUri;
                    if (context.Identity == null)
                    {
                        // add a redirect hint that sign-in failed in some way
                        redirectUri = WebUtilities.AddQueryString(redirectUri, "error", "access_denied");
                    }
                    Response.Redirect(redirectUri);
                    context.RequestCompleted();
                }

                return context.IsRequestCompleted;
            }
            return false;
        }
    }
}