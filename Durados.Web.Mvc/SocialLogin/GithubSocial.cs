using Durados.Web.Mvc.UI.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Script.Serialization;

namespace Durados.Web.Mvc.SocialLogin
{
    public class GithubSocialProvider : SocialProvider
    {
        protected override SocialProfile GetNewProfile(Dictionary<string, object> dictionary)
        {
            return new GithubProfile(dictionary);
        }

        protected override string ProviderName
        {
            get { return "github"; }
        }

        public override string GetAuthUrl(string appName, string returnAddress, string parameters, string activity)
        {
            Dictionary<string, object> keys = GetKeys(appName);
            string clientId = keys["ClientId"].ToString();
            string redirectUri = GetRedirectUrl();

            //var state = Durados.Web.Mvc.UI.Json.JsonSerializer.Serialize(new Dictionary<string, object>() { { "appName", appName }, { "returnAddress", System.Web.HttpContext.Current.Server.UrlEncode(returnAddress) } });
            var state = new { appName = appName, returnAddress = System.Web.HttpContext.Current.Server.UrlEncode(returnAddress), activity = activity, parameters = parameters ?? string.Empty };
            var jss = new JavaScriptSerializer();

            string url = string.Format("https://github.com/login/oauth/authorize?scope=user:email&client_id={0}&redirect_uri={1}?appName={3}&state={2}", clientId, redirectUri, jss.Serialize(state), appName);
            return url;
        }


        public override SocialProfile Authenticate()
        {
            //get the code from Google and request from access token
            string code = System.Web.HttpContext.Current.Request.QueryString["code"];
            string error = System.Web.HttpContext.Current.Request.QueryString["error"];


            if (code == null || error != null)
            {
                throw new GithubException(error);
            }
            try
            {

                string state = System.Web.HttpContext.Current.Request.QueryString["state"];
                var jss = new JavaScriptSerializer();
                Dictionary<string, object> stateObject = Durados.Web.Mvc.UI.Json.JsonSerializer.Deserialize(state);
                if (!stateObject.ContainsKey("appName"))
                {
                    throw new GithubException("Could not find the app name");
                }
                if (!stateObject.ContainsKey("returnAddress"))
                {
                    throw new GithubException("Could not find the return address");
                }
                if (!stateObject.ContainsKey("activity"))
                {
                    throw new GithubException("Could not find the activity");
                }

                if (!stateObject.ContainsKey("parameters"))
                {
                    throw new GithubException("Could not find the parameters");
                }

                string appName = stateObject["appName"].ToString();
                string returnAddress = stateObject["returnAddress"].ToString();
                string activity = stateObject["activity"].ToString();
                string parameters = stateObject["parameters"].ToString();

                return FetchProfileByCode(code, appName, returnAddress, activity, parameters, null);

            }
            catch (Exception exception)
            {
                throw new GithubException(exception.Message, exception);
            }

        }

        protected override SocialProfile FetchProfileByCode(string code, string appName, string returnAddress, string activity, string parameters, string redirectUrl)
        {

            string urlAccessToken = "https://github.com/login/oauth/access_token";
            //build the URL to send to Google
            Dictionary<string, object> keys = GetKeys(appName);
            string clientId = keys["ClientId"].ToString();
            string clientSecret = keys["ClientSecret"].ToString();

            string redirectUri = string.IsNullOrEmpty(redirectUrl) ? GetRedirectUrl() : redirectUrl;

            string accessTokenData = string.Format("code={0}&client_id={1}&client_secret={2}&redirect_uri={3}", code, clientId, clientSecret, redirectUri);
            string response = Infrastructure.Http.PostWebRequest(urlAccessToken, accessTokenData, "", "application/json");

            //get the access token from the return JSON
            Dictionary<string, object> accessObject = Durados.Web.Mvc.UI.Json.JsonSerializer.Deserialize(response);
            string accessToken = accessObject["access_token"].ToString();

            return GetProfile(appName, accessToken,  returnAddress, activity, parameters );
        }

        protected override SocialProfile GetProfileUnsafe(string appName, string accessToken, string returnUrl, string activity, string parameters)
        {
            //get the Google user profile using the access token
            //string profileUrl = "https://api.github.com/user";
            string profileUrl = "https://api.github.com/user/emails";
            string profileHeader = "Authorization: Bearer " + accessToken;
            string profiel = Infrastructure.Http.GetWebRequest(profileUrl, profileHeader, "https://api.github.com/meta");

            //get the user email out of goolge profile
            //need to make the JSON more statndrd for us
            profiel = "{\"data\":" + profiel + "}";
            Dictionary<string, object> profile = Durados.Web.Mvc.UI.Json.JsonSerializer.Deserialize(profiel);

            profile.Add("appName", appName);
            profile.Add("returnAddress", returnUrl);
            profile.Add("activity", activity);
            profile.Add("parameters", parameters);

            return GetNewProfile(profile);
        }

        protected override Dictionary<string, object> GetKeys(string appName)
        {
            return GetGithubKeys(appName);
        }

        private Dictionary<string, object> GetGithubKeys(string appName)
        {
            string clientId = System.Configuration.ConfigurationManager.AppSettings["GithubClientId"];
            string clientSecret = System.Configuration.ConfigurationManager.AppSettings["GithubClientSecret"];
            Dictionary<string, object> keys = new Dictionary<string, object>();
            keys.Add("ClientId", clientId);
            keys.Add("ClientSecret", clientSecret);

            if (appName == Maps.DuradosAppName)
                return keys;

            Map map = Maps.Instance.GetMap(appName);
            if (map == null)
                return keys;

            if (!string.IsNullOrEmpty(map.Database.GithubClientId))
            {
                keys["ClientId"] = map.Database.GithubClientId;
            }
            if (!string.IsNullOrEmpty(map.Database.GithubClientSecret))
            {
                keys["ClientSecret"] = map.Database.GithubClientSecret;
            }

            return keys;
        }



    }

    public class GithubProfile : SocialProfile
    {
        public GithubProfile(Dictionary<string, object> dictionary) :
            base(dictionary)
        {

        }

        protected virtual string GetNamePart(string key)
        {
            try
            {
                return ((Dictionary<string, object>)dictionary["data"])[key].ToString();
            }
            catch
            {
                return null;
            }
        }

        public override string firstName
        {
            get { return email.Split('@').FirstOrDefault(); }
        }

        public override string lastName
        {
            get { return string.Empty; }
        }

        public override string email
        {
            get
            {
                foreach (Dictionary<string, object> emailItem in (object[])dictionary["data"])
                {
                    if (emailItem["primary"].Equals(true))
                    {
                        return emailItem["email"].ToString();
                    }
                }
                return null;
            }
        }
        public override string Provider
        {
            get { return "github"; }
        }

    }

    public class GithubException : SocialException
    {
        public GithubException(string message)
            : base(message)
        {

        }

        public GithubException(string message, Exception innerException)
            : base(message)
        {

        }
    }
}
