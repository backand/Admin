using Durados.Web.Mvc.UI.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Script.Serialization;

namespace Durados.Web.Mvc.SocialLogin
{
    public class GoogleSocialProvider : SocialProvider
    {
        protected override SocialProfile GetNewProfile(Dictionary<string, object> dictionary)
        {
            return new GoogleProfile(dictionary);
        }

        protected override string ProviderName
        {
            get { return "google"; }
        }

        public override string GetAuthUrl(string appName, string returnAddress, string parameters, string activity)
        {
            Dictionary<string, object> keys = GetKeys(appName);
            string clientId = keys["ClientId"].ToString();
            string redirectUri = GetRedirectUrl();
            string scope = "https://www.googleapis.com/auth/userinfo.email+https://www.googleapis.com/auth/userinfo.profile";

            //var state = Durados.Web.Mvc.UI.Json.JsonSerializer.Serialize(new Dictionary<string, object>() { { "appName", appName }, { "returnAddress", System.Web.HttpContext.Current.Server.UrlEncode(returnAddress) } });
            var state = new { appName = appName, returnAddress = System.Web.HttpContext.Current.Server.UrlEncode(returnAddress), activity = activity, parameters = parameters ?? string.Empty };
            var jss = new JavaScriptSerializer();

            string url = string.Format("https://accounts.google.com/o/oauth2/auth?scope={0}&client_id={1}&redirect_uri={2}&response_type=code&access_type=offline&state={3}", scope, clientId, redirectUri, jss.Serialize(state));
            return url;
        }

        protected override Dictionary<string, object> GetKeys(string appName)
        {
            return GetGoogleKeys(appName);
        }

        private Dictionary<string, object> GetGoogleKeys(string appName)
        {
            string GoogleClientId = System.Configuration.ConfigurationManager.AppSettings["GoogleClientId"];
            string GoogleClientSecret = System.Configuration.ConfigurationManager.AppSettings["GoogleClientSecret"];
            Dictionary<string, object> keys = new Dictionary<string, object>();
            keys.Add("ClientId", GoogleClientId);
            keys.Add("ClientSecret", GoogleClientSecret);

            if (appName == Maps.DuradosAppName)
                return keys;

            Map map = Maps.Instance.GetMap(appName);
            if (map == null)
                return keys;

            if (!string.IsNullOrEmpty(map.Database.GoogleClientId))
            {
                keys["ClientId"] = map.Database.GoogleClientId;
            }
            if (!string.IsNullOrEmpty(map.Database.GoogleClientSecret))
            {
                keys["ClientSecret"] = map.Database.GoogleClientSecret;
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
                string parameters = stateObject["parameters"].ToString();

                return FetchProfileByCode(code, appName, returnAddress, activity, parameters , null);

            }
            catch (Exception exception)
            {
                throw new GoogleException(exception.Message, exception);
            }

        }

        protected override SocialProfile FetchProfileByCode(string code, string appName, string returnUrl, string activity, string parameters, string redirectUrl)
        {
            //build the URL to send to Google
            string urlAccessToken = "https://accounts.google.com/o/oauth2/token";

            Dictionary<string, object> keys = GetKeys(appName);
            string clientId = keys["ClientId"].ToString();
            string clientSecret = keys["ClientSecret"].ToString();

            string redirectUri = string.IsNullOrEmpty(redirectUrl) ? GetRedirectUrl() : redirectUrl;

            string accessTokenData = string.Format("scope=&code={0}&client_id={1}&client_secret={2}&redirect_uri={3}&grant_type=authorization_code", code, clientId, clientSecret, redirectUri);
            string response = Durados.Web.Mvc.Infrastructure.Http.PostWebRequest(urlAccessToken, accessTokenData);

            //get the access token from the return JSON
            //JavaScriptSerializer jsonSerializer = new JavaScriptSerializer();
            //AuthResponse validateResponse = (AuthResponse)jsonSerializer.Deserialize<AuthResponse>(response);

            Dictionary<string, object> validateResponse = Durados.Web.Mvc.UI.Json.JsonSerializer.Deserialize(response);

            string accessToken = validateResponse["access_token"].ToString();

            var googleProfile = GetProfile(appName, accessToken, redirectUri, activity, parameters);

            return googleProfile;
        }

        protected override SocialProfile GetProfileUnsafe(string appName, string accessToken, string returnAddress, string activity, string parameters)
        {
            //get the Google user profile using the access token
            string profileUrl = "https://www.googleapis.com/plus/v1/people/me";
            string profileHeader = "Authorization: Bearer " + accessToken;
            string profiel = Durados.Web.Mvc.Infrastructure.Http.GetWebRequest(profileUrl, profileHeader);

            //get the user email out of goolge profile
            //GoogleProfile googleProfile = (GoogleProfile)jsonSerializer.Deserialize<GoogleProfile>(profiel); ;
            Dictionary<string, object> googleProfile = Durados.Web.Mvc.UI.Json.JsonSerializer.Deserialize(profiel);
            googleProfile.Add("appName", appName);
            googleProfile.Add("returnAddress", returnAddress);
            googleProfile.Add("activity", activity);
            googleProfile.Add("parameters", parameters);
            return GetNewProfile(googleProfile);
        }



        public class GoogleProfile : SocialProfile
        {
            public GoogleProfile(Dictionary<string, object> dictionary) :
                base(dictionary)
            {

            }

            protected virtual string GetNamePart(string key)
            {
                if (dictionary.ContainsKey("name"))
                {
                    Dictionary<string, object> names = (Dictionary<string, object>)dictionary["name"];
                    if (names.ContainsKey(key))
                        return names[key].ToString();
                    else
                        return email.Split('@').FirstOrDefault();
                }
                else
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
            }

            public override string firstName
            {
                get { return GetNamePart("givenName"); }
            }

            public override string lastName
            {
                get { return GetNamePart("familyName"); }
            }

            public override string email
            {
                get { return ((Dictionary<string, object>)((object[])dictionary["emails"])[0])["value"].ToString(); }
            }

            public override string Provider
            {
                get { return "google"; }
            }
        }

        public class GoogleException : SocialException
        {
            public GoogleException(string message)
                : base(message)
            {

            }

            public GoogleException(string message, Exception innerException)
                : base(message)
            {

            }
        }
    }
}
