using Durados.Web.Mvc.UI.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web.Script.Serialization;

namespace Durados.Web.Mvc.SocialLogin
{
    public class FacebookSocialProvider : SocialProvider
    {
        protected override SocialProfile GetNewProfile(Dictionary<string, object> dictionary)
        {
            return new FacebookProfile(dictionary);
        }

        protected override string ProviderName
        {
            get { return "facebook"; }
        }

        public override string GetAuthUrl(string appName, string returnAddress, string parameters, string activity)
        {
            Dictionary<string, object> keys = GetKeys(appName);
            string clientId = keys["ClientId"].ToString();
            string redirectUri = GetRedirectUrl();
            var state = new { appName = appName, returnAddress = System.Web.HttpContext.Current.Server.UrlEncode(returnAddress), activity = activity, parameters = parameters ?? string.Empty };
            var jss = new JavaScriptSerializer();


            // OAuth2 10.12 CSRF
            //GenerateCorrelationId(properties);

            // comma separated
            string scope = "email";

            string authorizationEndpoint =
                "https://www.facebook.com/dialog/oauth" +
                    "?response_type=code" +
                    "&client_id=" + Uri.EscapeDataString(clientId) +
                    "&redirect_uri=" + Uri.EscapeDataString(redirectUri) +
                    "&scope=" + Uri.EscapeDataString(scope) +
                    "&state=" + Uri.EscapeDataString(jss.Serialize(state));
            return authorizationEndpoint;
        }

        protected override Dictionary<string, object> GetKeys(string appName)
        {
            return GetFacebookKeys(appName);
        }

        private Dictionary<string, object> GetFacebookKeys(string appName)
        {
            string GoogleClientId = System.Configuration.ConfigurationManager.AppSettings["FacebookClientId"];
            string GoogleClientSecret = System.Configuration.ConfigurationManager.AppSettings["FacebookClientSecret"];
            Dictionary<string, object> keys = new Dictionary<string, object>();
            keys.Add("ClientId", GoogleClientId);
            keys.Add("ClientSecret", GoogleClientSecret);

            if (appName == Maps.DuradosAppName)
                return keys;

            Map map = Maps.Instance.GetMap(appName);
            if (map == null)
                return keys;

            if (!string.IsNullOrEmpty(map.Database.FacebookClientId))
            {
                keys["ClientId"] = map.Database.FacebookClientId;
            }
            if (!string.IsNullOrEmpty(map.Database.FacebookClientSecret))
            {
                keys["ClientSecret"] = map.Database.FacebookClientSecret;
            }

            return keys;
        }



        public override SocialProfile Authenticate()
        {
            //get the code from Google and request from access token
            string code = System.Web.HttpContext.Current.Request.QueryString["code"];
            string error = System.Web.HttpContext.Current.Request.QueryString["error"];

            if (code == null || error != null)
            {
                throw new FacebookException(error);
            }
            try
            {

                var _httpClient = new HttpClient();
                // _httpClient.Timeout = Options.BackchannelTimeout;
                _httpClient.MaxResponseContentBufferSize = 1024 * 1024 * 10; // 10 MB


                string state = System.Web.HttpContext.Current.Request.QueryString["state"];
                var jss = new JavaScriptSerializer();
                Dictionary<string, object> stateObject = Durados.Web.Mvc.UI.Json.JsonSerializer.Deserialize(state);

                if (stateObject == null)
                {
                    return null;
                }

                if (!stateObject.ContainsKey("appName"))
                {
                    throw new FacebookException("Could not find the app name");
                }
                string appName = stateObject["appName"].ToString();
                if (!stateObject.ContainsKey("returnAddress"))
                {
                    throw new FacebookException("Could not find the return address");
                }
                string returnAddress = System.Web.HttpContext.Current.Server.UrlDecode(stateObject["returnAddress"].ToString());
                if (!stateObject.ContainsKey("activity"))
                {
                    throw new FacebookException("Could not find the activity");
                }
                string activity = stateObject["activity"].ToString();
                if (!stateObject.ContainsKey("parameters"))
                {
                    throw new FacebookException("Could not find the parameters");
                }
                string parameters = stateObject["parameters"].ToString();

                return FetchProfileByCode(code, appName, returnAddress, activity, parameters, null);


                //HttpResponseMessage graphResponse = await _httpClient.GetAsync(graphAddress, HttpCompletionOption.ResponseContentRead);
                //graphResponse.EnsureSuccessStatusCode();
                //text = await graphResponse.Content.ReadAsStringAsync();
                //JObject user = JObject.Parse(text);

                //var email = user["email"].Value<string>();
                //var redirectUrl = stateObject.RedirectUri;
                //var appName = stateObject.Dictionary["appname"];


            }
            catch (Exception exception)
            {
                throw new FacebookException(exception.Message, exception);
            }



        }

        protected override SocialProfile FetchProfileByCode(string code, string appName, string returnAddress, string activity, string parameters, string redirectUrl)
        {
            Dictionary<string, object> keys = GetKeys(appName);
            string clientId = keys["ClientId"].ToString();
            string clientSecret = keys["ClientSecret"].ToString();

            string redirectUri = string.IsNullOrEmpty(redirectUrl) ? GetRedirectUrl() : redirectUrl;

            string tokenRequest = "grant_type=authorization_code" +
                "&code=" + Uri.EscapeDataString(code) +
                "&redirect_uri=" + Uri.EscapeDataString(redirectUri) +
                "&client_id=" + Uri.EscapeDataString(clientId) +
                "&client_secret=" + Uri.EscapeDataString(clientSecret);

            string TokenEndpoint = "https://graph.facebook.com/oauth/access_token";

            string response = Infrastructure.Http.GetWebRequest(TokenEndpoint + "?" + tokenRequest);
            var validateResponse = System.Web.HttpUtility.ParseQueryString(response);

            string accessToken = validateResponse["access_token"].ToString();
            string expires = validateResponse["expires"].ToString();

            SocialProfile profile = GetProfile(appName, accessToken, returnAddress, activity, parameters);

            if (string.IsNullOrEmpty(profile.email))
            {
                Exception exception = new FacebookException("Facebook account authenticated but did not return an email.");
                Maps.Instance.DuradosMap.Logger.Log("userController", "Authenticate", "email", exception, 1, "");
                throw exception;
            }

            return profile;
        }

        protected override SocialProfile GetProfileUnsafe(string appName, string accessToken, string returnAddress, string activity, string parameters)
        {
            string GraphApiEndpoint = "https://graph.facebook.com/me?fields=id,email,name";
            string graphAddress = GraphApiEndpoint + "&access_token=" + Uri.EscapeDataString(accessToken);

            string profiel = Durados.Web.Mvc.Infrastructure.Http.GetWebRequest(graphAddress);
            Dictionary<string, object> facebookProfile = Durados.Web.Mvc.UI.Json.JsonSerializer.Deserialize(profiel);
            facebookProfile.Add("appName", appName);
            facebookProfile.Add("returnAddress", returnAddress);
            facebookProfile.Add("activity", activity);
            facebookProfile.Add("parameters", parameters);

            SocialProfile profile = GetNewProfile(facebookProfile);
            return profile;
        }



        public class FacebookProfile : SocialProfile
        {
            public FacebookProfile(Dictionary<string, object> dictionary) :
                base(dictionary)
            {

            }

            protected virtual string GetPart(string key)
            {
                if (dictionary.ContainsKey(key))
                {
                    return dictionary[key].ToString();
                }
                else
                {
                    if (key != "email")
                    {
                        if (string.IsNullOrEmpty(email))
                        {
                            return null;
                        }
                        else
                        {
                            return email.Split('@').FirstOrDefault();
                        }
                    }
                    else
                    {
                        return null;
                    }
                }
            }
            public override string firstName
            {
                get { return GetPart("first_name"); }
            }

            public override string lastName
            {
                get { return GetPart("last_name"); }
            }

            public override string email
            {
                get { return GetPart("email"); }
            }

            public override string Provider
            {
                get { return "facebook"; }
            }
        }

        public class FacebookException : SocialException
        {
            public FacebookException(string message)
                : base(message)
            {

            }

            public FacebookException(string message, Exception innerException)
                : base(message)
            {

            }
        }
    }
}
