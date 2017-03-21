using Durados.Web.Mvc.SocialLogin.Google;
using Durados.Web.Mvc.UI.Helpers;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Web;

namespace Durados.Web.Mvc.SocialLogin
{
    public class TwitterSocialProvider : AbstractSocialProvider
    {
        protected override SocialProfile GetNewProfile(Dictionary<string, object> dictionary)
        {
            return new TwitterProfile(dictionary);
        }

        protected override string ProviderName
        {
            get { return "twitter"; }
        }

        protected override string ConfigPrefix
        {
            get
            {
                return "Twitter";
            }
        }


        protected override string GetAuthUrl(string appName, string returnAddress, string parameters, string activity, string email, bool signupIfNotSignedIn, bool useHashRouting)
        {
            var socialKeys = GetSocialKeys(appName);
            string clientId = socialKeys.ClientId;
            string redirectUri = GetRedirectUrl();

            var requestToken = ObtainRequestTokenAsync(clientId, socialKeys.ClientSecret, redirectUri);



            if (requestToken.CallbackConfirmed)
            {

                // set cookie
                AddCookie(new TwitterRequestUserData { Activity = activity, Email = email, AppName = appName, ReturnAddress = returnAddress, Token = requestToken, signupIfNotSignedIn = signupIfNotSignedIn });
                string twitterAuthenticationEndpoint = AuthenticationEndpoint + requestToken.Token;
                return twitterAuthenticationEndpoint;
            }


            // todo : handle error
            return string.Empty;
        }


        private static TwitterRequestUserData GetFromCookie()
        {
            var data = HttpContext.Current.Request.Cookies["_twitter"];

            if (data == null)
            {
                throw new SocialException("can't find twitter cookie in request");
            }

            return JsonConvert.DeserializeObject<TwitterRequestUserData>(data.Value);
        }

        private static void AddCookie(TwitterRequestUserData requestToken)
        {
            HttpCookie serverCookie = new HttpCookie
                                              ("_twitter", JsonConvert.SerializeObject(requestToken));
            serverCookie.Expires = DateTime.Now.AddDays(2);
            serverCookie.Domain = HttpContext.Current.Request.Url.Host;
            serverCookie.Path = "/";

            HttpContext.Current.Response.Cookies.Add(serverCookie);

            //string AccessToken = BackAnd.Web.Api.Startup.OAuthBearerOptions.AccessTokenFormat.Protect(ticket);
        }

        private const string RequestTokenEndpoint = "https://api.twitter.com/oauth/request_token";
        private const string AuthenticationEndpoint = "http://twitter.com/oauth/authorize?oauth_token=";
        private const string AccessTokenEndpoint = "https://api.twitter.com/oauth/access_token";


        private RequestToken ObtainRequestTokenAsync(string consumerKey, string consumerSecret, string callBackUri)
        {

            string nonce = Guid.NewGuid().ToString("N");

            var authorizationParts = new SortedDictionary<string, string>
            {
                { "oauth_callback", callBackUri },
                { "oauth_consumer_key", consumerKey },
                { "oauth_nonce", nonce },
                { "oauth_signature_method", "HMAC-SHA1" },
                { "oauth_timestamp", GenerateTimeStamp() },
                { "oauth_version", "1.0" }
            };

            var parameterBuilder = new StringBuilder();
            foreach (var authorizationKey in authorizationParts)
            {
                parameterBuilder.AppendFormat("{0}={1}&", Uri.EscapeDataString(authorizationKey.Key), Uri.EscapeDataString(authorizationKey.Value));
            }
            parameterBuilder.Length--;
            string parameterString = parameterBuilder.ToString();

            var canonicalizedRequestBuilder = new StringBuilder();
            canonicalizedRequestBuilder.Append(HttpMethod.Post.Method);
            canonicalizedRequestBuilder.Append("&");
            canonicalizedRequestBuilder.Append(Uri.EscapeDataString(RequestTokenEndpoint));
            canonicalizedRequestBuilder.Append("&");
            canonicalizedRequestBuilder.Append(Uri.EscapeDataString(parameterString));

            string signature = ComputeSignature(consumerSecret, null, canonicalizedRequestBuilder.ToString());
            authorizationParts.Add("oauth_signature", signature);

            var authorizationHeaderBuilder = new StringBuilder();
            authorizationHeaderBuilder.Append("OAuth ");
            foreach (var authorizationPart in authorizationParts)
            {
                authorizationHeaderBuilder.AppendFormat(
                    "{0}=\"{1}\", ", authorizationPart.Key, Uri.EscapeDataString(authorizationPart.Value));
            }
            authorizationHeaderBuilder.Length = authorizationHeaderBuilder.Length - 2;

            var request = new HttpRequestMessage(HttpMethod.Post, RequestTokenEndpoint);
            request.Headers.Add("Authorization", authorizationHeaderBuilder.ToString());


            //  string response = Durados.Web.Mvc.Infrastructure.Http.GetWebRequest(RequestTokenEndpoint, accessTokenData);
            var _httpClient = new HttpClient();
            HttpResponseMessage response = _httpClient.SendAsync(request).Result;

            response.EnsureSuccessStatusCode();
            string responseText = response.Content.ReadAsStringAsync().Result;

            var responseParameters = HttpUtility.ParseQueryString(responseText);
            if (string.Equals(responseParameters["oauth_callback_confirmed"].ToString(), "true", StringComparison.InvariantCulture))
            {
                return new RequestToken { Token = Uri.UnescapeDataString(responseParameters["oauth_token"].ToString()), TokenSecret = Uri.UnescapeDataString(responseParameters["oauth_token_secret"].ToString()), CallbackConfirmed = true };
            }

            return new RequestToken();
        }

        private string ObtainAccessTokenAsync(string consumerKey, string consumerSecret, RequestToken token, string verifier)
        {
            // https://dev.twitter.com/docs/api/1/post/oauth/access_token


            string nonce = Guid.NewGuid().ToString("N");

            var authorizationParts = new SortedDictionary<string, string>
            {
                { "oauth_consumer_key", consumerKey },
                { "oauth_nonce", nonce },
                { "oauth_signature_method", "HMAC-SHA1" },
                { "oauth_token", token.Token },
                { "oauth_timestamp", GenerateTimeStamp() },
                { "oauth_verifier", verifier },
                { "oauth_version", "1.0" },
            };

            var parameterBuilder = new StringBuilder();
            foreach (var authorizationKey in authorizationParts)
            {
                parameterBuilder.AppendFormat("{0}={1}&", Uri.EscapeDataString(authorizationKey.Key), Uri.EscapeDataString(authorizationKey.Value));
            }
            parameterBuilder.Length--;
            string parameterString = parameterBuilder.ToString();

            var canonicalizedRequestBuilder = new StringBuilder();
            canonicalizedRequestBuilder.Append(HttpMethod.Post.Method);
            canonicalizedRequestBuilder.Append("&");
            canonicalizedRequestBuilder.Append(Uri.EscapeDataString(AccessTokenEndpoint));
            canonicalizedRequestBuilder.Append("&");
            canonicalizedRequestBuilder.Append(Uri.EscapeDataString(parameterString));

            string signature = ComputeSignature(consumerSecret, token.TokenSecret, canonicalizedRequestBuilder.ToString());
            authorizationParts.Add("oauth_signature", signature);
            authorizationParts.Remove("oauth_verifier");

            var authorizationHeaderBuilder = new StringBuilder();
            authorizationHeaderBuilder.Append("OAuth ");
            foreach (var authorizationPart in authorizationParts)
            {
                authorizationHeaderBuilder.AppendFormat(
                    "{0}=\"{1}\", ", authorizationPart.Key, Uri.EscapeDataString(authorizationPart.Value));
            }
            authorizationHeaderBuilder.Length = authorizationHeaderBuilder.Length - 2;

            var request = new HttpRequestMessage(HttpMethod.Post, AccessTokenEndpoint);
            request.Headers.Add("Authorization", authorizationHeaderBuilder.ToString());

            var formPairs = new List<KeyValuePair<string, string>>()
            {
                new KeyValuePair<string, string>("oauth_verifier", verifier)
            };

            var _httpClient = new HttpClient();
            request.Content = new FormUrlEncodedContent(formPairs);

            HttpResponseMessage response = _httpClient.SendAsync(request).Result;

            if (!response.IsSuccessStatusCode)
            {
                response.EnsureSuccessStatusCode(); // throw
            }

            string responseText = response.Content.ReadAsStringAsync().Result;

            var responseParameters = HttpUtility.ParseQueryString(responseText);


            return Uri.UnescapeDataString(responseParameters["user_id"]);
            /*  return new AccessToken
              {
                  Token = Uri.UnescapeDataString(responseParameters["oauth_token"]),
                  TokenSecret = Uri.UnescapeDataString(responseParameters["oauth_token_secret"]),
                  UserId = Uri.UnescapeDataString(responseParameters["user_id"]),
                  ScreenName = Uri.UnescapeDataString(responseParameters["screen_name"])
              };*/
        }


        public override SocialProfile Authenticate()
        {
            try
            {
                //get the code from Google and request from access token
                string token = System.Web.HttpContext.Current.Request.QueryString["oauth_token"];
                string verifier = System.Web.HttpContext.Current.Request.QueryString["oauth_verifier"];

                var dataFromCookie = GetFromCookie();

                var keys = GetSocialKeys(dataFromCookie.AppName);

                var twitterId = ObtainAccessTokenAsync(keys.ClientId, keys.ClientSecret, dataFromCookie.Token, verifier);

                if (string.IsNullOrEmpty(twitterId))
                {
                    throw new SocialException("can't fetch id from twitter");
                }


                return new TwitterProfile(dataFromCookie.Email, twitterId, dataFromCookie.ReturnAddress, dataFromCookie.Activity, dataFromCookie.AppName, null, dataFromCookie.signupIfNotSignedIn, true);
                //return FetchProfileByCode(code, appName, returnAddress, activity, parameters, null);

            }
            catch (Exception exception)
            {
                throw new TwitterException(exception.Message, exception);
            }

        }

        protected override SocialProfile FetchProfileByCode(string code, string appName, string returnUrl, string activity,
            string parameters, string redirectUrl, string email, bool signupIfNotSignedIn, bool useHashRouting)
        {
            //build the URL to send to Google
            string urlAccessToken = "http://twitter.com/oauth/authenticate?oauth_token=";

            var keys = GetSocialKeys(appName);
            string clientId = keys.ClientId;
            string clientSecret = keys.ClientSecret;

            string redirectUri = string.IsNullOrEmpty(redirectUrl) ? GetRedirectUrl() : redirectUrl;

            string accessTokenData = string.Format("scope=&code={0}&client_id={1}&client_secret={2}&redirect_uri={3}&grant_type=authorization_code", code, clientId, clientSecret, redirectUri);
            string response = Durados.Web.Mvc.Infrastructure.Http.PostWebRequest(urlAccessToken, accessTokenData);

            //get the access token from the return JSON
            //JavaScriptSerializer jsonSerializer = new JavaScriptSerializer();
            //AuthResponse validateResponse = (AuthResponse)jsonSerializer.Deserialize<AuthResponse>(response);

            Dictionary<string, object> validateResponse = Durados.Web.Mvc.UI.Json.JsonSerializer.Deserialize(response);

            string accessToken = validateResponse["access_token"].ToString();

            var googleProfile = GetProfile(appName, accessToken, null, redirectUri, activity, parameters, email, signupIfNotSignedIn, useHashRouting);

            return googleProfile;
        }



        protected override string FetchProfileFromService(string accessToken)
        {
            string profileUrl = "https://www.googleapis.com/plus/v1/people/me";
            string profileHeader = "Authorization: Bearer " + accessToken;
            string profiel = Durados.Web.Mvc.Infrastructure.Http.GetWebRequest(profileUrl, profileHeader);
            return profiel;
        }

        private void EnrichProfile(Dictionary<string, object> dataFromProvider)
        {
        }

        protected override SocialApplicationKeys GetSocialKeysFromDatabase(Map map)
        {
            return new SocialApplicationKeys
            {
                ClientId = map.Database.TwitterClientId,
                ClientSecret = map.Database.TwitterClientSecret
            };
        }


        private static string GenerateTimeStamp()
        {
            TimeSpan secondsSinceUnixEpocStart = DateTime.UtcNow - Epoch;
            return Convert.ToInt64(secondsSinceUnixEpocStart.TotalSeconds).ToString(CultureInfo.InvariantCulture);
        }

        private static readonly DateTime Epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);


        private static string ComputeSignature(string consumerSecret, string tokenSecret, string signatureData)
        {
            using (var algorithm = new HMACSHA1())
            {
                algorithm.Key = Encoding.ASCII.GetBytes(
                    string.Format(CultureInfo.InvariantCulture,
                        "{0}&{1}",
                        Uri.EscapeDataString(consumerSecret),
                        string.IsNullOrEmpty(tokenSecret) ? string.Empty : Uri.EscapeDataString(tokenSecret)));
                byte[] hash = algorithm.ComputeHash(Encoding.ASCII.GetBytes(signatureData));
                return Convert.ToBase64String(hash);
            }
        }

        public override string GetLogOutRedirectUrl(string appName, string redirectUri = null)
        {
            throw new NotImplementedException();
        }

    }

    public class RequestToken
    {
        /// <summary>
        /// Gets or sets the Twitter token
        /// </summary>
        public string Token { get; set; }

        /// <summary>
        /// Gets or sets the Twitter token secret
        /// </summary>
        public string TokenSecret { get; set; }

        public bool CallbackConfirmed { get; set; }

        /// <summary>
        /// Gets or sets a property bag for common authentication properties
        /// </summary>
        //public AuthenticationProperties Properties { get; set; }
    }

    public class TwitterRequestUserData
    {
        public string AppName { get; set; }

        public string ReturnAddress { get; set; }

        public string Activity { get; set; }

        public string Email { get; set; }

        public RequestToken Token { get; set; }

        public bool signupIfNotSignedIn { get; set; }
    }
}
