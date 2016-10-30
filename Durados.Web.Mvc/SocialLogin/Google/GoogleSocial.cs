using Durados.Web.Mvc.SocialLogin.Google;
using Durados.Web.Mvc.UI.Helpers;
using System;
using System.Collections.Generic;
using System.Web.Script.Serialization;

namespace Durados.Web.Mvc.SocialLogin
{
    public class GoogleSocialProvider : AbstractSocialProvider
    {
        protected override SocialProfile GetNewProfile(Dictionary<string, object> dictionary)
        {
            return new GoogleProfile(dictionary);
        }

        protected override string ProviderName
        {
            get { return "google"; }
        }

        protected override string ConfigPrefix
        {
            get
            {
                return "Google";
            }
        }

        public override string GetAuthUrl(string appName, string returnAddress, string parameters, string activity, string email, bool signupIfNotSignedIn, bool useHashRouting)
        {
            var socialKeys = GetSocialKeys(appName);
            string clientId = socialKeys.ClientId;
            string redirectUri = GetRedirectUrl();
            string scope = "https://www.googleapis.com/auth/userinfo.email+https://www.googleapis.com/auth/userinfo.profile";

            //var state = Durados.Web.Mvc.UI.Json.JsonSerializer.Serialize(new Dictionary<string, object>() { { "appName", appName }, { "returnAddress", System.Web.HttpContext.Current.Server.UrlEncode(returnAddress) } });
            var state = new { appName = appName, returnAddress = System.Web.HttpContext.Current.Server.UrlEncode(returnAddress), activity = activity, parameters = parameters ?? string.Empty, email = email, signupIfNotSignedIn = signupIfNotSignedIn };
            var jss = new JavaScriptSerializer();

            string url = string.Format("https://accounts.google.com/o/oauth2/auth?scope={0}&client_id={1}&redirect_uri={2}&response_type=code&access_type=offline&state={3}", scope, clientId, redirectUri, jss.Serialize(state));
            return url;
        }




        public override SocialProfile Authenticate()
        {
            //get the code from Google and request from access token
            string code = System.Web.HttpContext.Current.Request.QueryString["code"];
            string error = System.Web.HttpContext.Current.Request.QueryString["error"];

            if (code == null || error != null)
            {
                throw new GoogleException(error);
            }
            try
            {
                string state = System.Web.HttpContext.Current.Request.QueryString["state"];
                var jss = new JavaScriptSerializer();

                Dictionary<string, object> stateObject = Durados.Web.Mvc.UI.Json.JsonSerializer.Deserialize(state);

                if (!stateObject.ContainsKey("appName"))
                {
                    throw new GoogleException("Could not find the app name");
                }
                string appName = stateObject["appName"].ToString();
                if (!stateObject.ContainsKey("returnAddress"))
                {
                    throw new GoogleException("Could not find the return address");
                }
                string returnAddress = stateObject["returnAddress"].ToString();
                if (!stateObject.ContainsKey("activity"))
                {
                    throw new GoogleException("Could not find the activity");
                }
                string activity = stateObject["activity"].ToString();
                if (!stateObject.ContainsKey("parameters"))
                {
                    throw new GoogleException("Could not find the parameters");
                }
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

                string parameters = stateObject["parameters"].ToString();

                string email = null;

                if (stateObject.ContainsKey("email") && stateObject["email"] != null && !string.IsNullOrEmpty(stateObject["email"].ToString()))
                {
                    email = stateObject["email"].ToString();
                }

                return FetchProfileByCode(code, appName, returnAddress, activity, parameters, null, email, signupIfNotSignedIn, useHashRouting);

            }
            catch (Exception exception)
            {
                throw new GoogleException(exception.Message, exception);
            }

        }

        protected override SocialProfile FetchProfileByCode(string code, string appName, string returnUrl, string activity, string parameters, string redirectUrl, string email, bool signupIfNotSignedIn, bool useHashRouting)
        {
            //build the URL to send to Google
            string urlAccessToken = "https://accounts.google.com/o/oauth2/token";

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

            var googleProfile = GetProfile(appName, accessToken, returnUrl, activity, parameters, email, signupIfNotSignedIn, useHashRouting);

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
            return new SocialApplicationKeys {
                ClientId = map.Database.GoogleClientId,
                ClientSecret = map.Database.GoogleClientSecret };
        }



    }
}
