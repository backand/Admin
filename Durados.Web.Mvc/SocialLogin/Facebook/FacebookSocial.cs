using Durados.Web.Mvc.SocialLogin.Facebook;
using Durados.Web.Mvc.UI.Helpers;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Web.Script.Serialization;

namespace Durados.Web.Mvc.SocialLogin
{
    public class FacebookSocialProvider : AbstractSocialProvider
    {
        protected override SocialProfile GetNewProfile(Dictionary<string, object> dictionary)
        {
            return new FacebookProfile(dictionary);
        }

        protected override string ProviderName
        {
            get { return "facebook"; }
        }

        protected override string ConfigPrefix
        {
            get
            {
                return "Facebook";
            }
        }

        protected override string GetAuthUrl(string appName, string returnAddress, string parameters, string activity, string email, bool signupIfNotSignedIn, bool useHashRouting)
        {
            var keys = GetSocialKeys(appName);
            string clientId = keys.ClientId;
            string redirectUri = GetRedirectUrl();
            var state = new
            {
                appName = appName,
                returnAddress = System.Web.HttpContext.Current.Server.UrlEncode(returnAddress),
                activity = activity,
                parameters = parameters ?? string.Empty,
                email = email,
                signupIfNotSignedIn = signupIfNotSignedIn
            };

            var jss = new JavaScriptSerializer();

            // OAuth2 10.12 CSRF
            //GenerateCorrelationId(properties);

            string scope = GetScope(appName);
           
            string authorizationEndpoint =
                "https://www.facebook.com/dialog/oauth" +
                    "?response_type=code" +
                    "&client_id=" + Uri.EscapeDataString(clientId) +
                    "&redirect_uri=" + Uri.EscapeDataString(redirectUri) +
                    "&scope=" + Uri.EscapeDataString(scope) +
                    "&state=" + Uri.EscapeDataString(jss.Serialize(state));

            return authorizationEndpoint;
        }

        private string GetScope(string appName)
        {
            Map map = Maps.Instance.GetMap(appName);
            return map.Database.FacebookScope ?? "email";
            
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
                bool signupIfNotSignedIn = false;
                if (stateObject.ContainsKey("signupIfNotSignedIn"))
                {
                    try
                    {
                        signupIfNotSignedIn = System.Convert.ToBoolean(stateObject["signupIfNotSignedIn"]);
                    }
                    catch { }
                }

                bool useHashRouting = true;
                if (stateObject.ContainsKey("useHashRouting"))
                {
                    try
                    {
                        useHashRouting = System.Convert.ToBoolean(stateObject["useHashRouting"]);
                    }
                    catch { }
                }
                
                
                if (!stateObject.ContainsKey("parameters"))
                {
                    throw new FacebookException("Could not find the parameters");
                }
                string parameters = stateObject["parameters"].ToString();


                string email = null;

                if (stateObject.ContainsKey("email") && stateObject["email"] != null && !string.IsNullOrEmpty(stateObject["email"].ToString()))
                {
                    email = stateObject["email"].ToString();
                }

                return FetchProfileByCode(code, appName, returnAddress, activity, parameters, null, email, signupIfNotSignedIn, useHashRouting);


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

        protected override SocialProfile FetchProfileByCode(string code, string appName, string returnAddress, string activity, string parameters, string redirectUrl, string email, bool signupIfNotSignedIn, bool useHashRouting)
        {
            var socialKeys = GetSocialKeys(appName);
            string clientId = socialKeys.ClientId;
            string clientSecret = socialKeys.ClientSecret;

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

            SocialProfile profile = GetProfile(appName, accessToken, null, returnAddress, activity, parameters, email, signupIfNotSignedIn, useHashRouting);

            // don't email more socialLogin V2
            //if (string.IsNullOrEmpty(profile.email))
            //{
            //    Exception exception = new FacebookException("Facebook account authenticated but did not return an email.");
            //    Maps.Instance.DuradosMap.Logger.Log("userController", "Authenticate", "email", exception, 1, "");
            //    throw exception;
            //}

            return profile;
        }

        protected override SocialApplicationKeys GetSocialKeysFromDatabase(Map map)
        {
            return new SocialApplicationKeys { ClientId = map.Database.FacebookClientId, ClientSecret = map.Database.FacebookClientSecret };
        }

        protected override string FetchProfileFromService(string accessToken)
        {
            string GraphApiEndpoint = "https://graph.facebook.com/me?fields=id,email,name,first_name,last_name";
            string graphAddress = GraphApiEndpoint + "&access_token=" + Uri.EscapeDataString(accessToken);

            string response = Durados.Web.Mvc.Infrastructure.Http.GetWebRequest(graphAddress);

            return response;
        }

        public override string GetLogOutRedirectUrl(string appName, string redirectUri = null)
        {
            throw new NotImplementedException();
        }
    }
}
