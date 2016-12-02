using Durados.Web.Mvc.SocialLogin.Github;
using Durados.Web.Mvc.UI.Helpers;
using System;
using System.Collections.Generic;
using System.Web.Script.Serialization;

namespace Durados.Web.Mvc.SocialLogin
{
    public class GithubSocialProvider : AbstractSocialProvider
    {
        protected override SocialProfile GetNewProfile(Dictionary<string, object> dictionary)
        {
            dictionary.Add("id", "dummy");
            var data = new GithubProfile(dictionary);
            data.id = data.email;
            return data;
        }

        protected override string ProviderName
        {
            get { return "github"; }
        }

        public override string GetAuthUrl(string appName, string returnAddress, string parameters, string activity, string email, bool signupIfNotSignedIn, bool useHashRouting)
        {
            var socialKeys = GetSocialKeys(appName);
            string clientId = socialKeys.ClientId;
            string redirectUri = GetRedirectUrl();

            //var state = Durados.Web.Mvc.UI.Json.JsonSerializer.Serialize(new Dictionary<string, object>() { { "appName", appName }, { "returnAddress", System.Web.HttpContext.Current.Server.UrlEncode(returnAddress) } });

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

                bool signupIfNotSignedIn = false;
                if (stateObject.ContainsKey("signupIfNotSignedIn"))
                {
                    try
                    {
                        signupIfNotSignedIn = System.Convert.ToBoolean(stateObject["signupIfNotSignedIn"]);
                    }
                    catch { }
                }
                bool useHashRouting = false;
                if (stateObject.ContainsKey("useHashRouting"))
                {
                    try
                    {
                        useHashRouting = System.Convert.ToBoolean(stateObject["useHashRouting"]);
                    }
                    catch { }
                }
                string email = null;

                if (stateObject.ContainsKey("email") && stateObject["email"] != null && !string.IsNullOrEmpty(stateObject["email"].ToString()))
                {
                    email = stateObject["email"].ToString();
                }

                return FetchProfileByCode(code, appName, returnAddress, activity, parameters, null, email, signupIfNotSignedIn, useHashRouting);

            }
            catch (Exception exception)
            {
                throw new GithubException(exception.Message, exception);
            }

        }

        protected override SocialProfile FetchProfileByCode(string code, string appName, string returnAddress, string activity, string parameters, string redirectUrl, string email, bool signupIfNotSignedIn, bool useHashRouting)
        {

            string urlAccessToken = "https://github.com/login/oauth/access_token";
            //build the URL to send to Google
            var keys = GetSocialKeys(appName);
            string clientId = keys.ClientId;
            string clientSecret = keys.ClientSecret;

            string redirectUri = string.IsNullOrEmpty(redirectUrl) ? GetRedirectUrl() : redirectUrl;

            string accessTokenData = string.Format("code={0}&client_id={1}&client_secret={2}&redirect_uri={3}", code, clientId, clientSecret, redirectUri);
            string response = Infrastructure.Http.PostWebRequest(urlAccessToken, accessTokenData, "", "application/json");

            //get the access token from the return JSON
            Dictionary<string, object> accessObject = Durados.Web.Mvc.UI.Json.JsonSerializer.Deserialize(response);
            if (!accessObject.ContainsKey("access_token"))
            {
                if (accessObject.ContainsKey("error") && accessObject.ContainsKey("error_description"))
                {
                    throw new DuradosException(accessObject["error"].ToString() + ":" + accessObject["error_description"].ToString());
                }
                else
                {
                    throw new DuradosException(response);
                }
            }
            string accessToken = accessObject["access_token"].ToString();

            return GetProfile(appName, accessToken, null, returnAddress, activity, parameters, email, signupIfNotSignedIn, useHashRouting);
        }

        protected override SocialApplicationKeys GetSocialKeysFromDatabase(Map map)
        {
            return new SocialApplicationKeys { ClientId = map.Database.GithubClientId, ClientSecret = map.Database.GithubClientSecret };
        }

        protected override string FetchProfileFromService(string accessToken)
        {
            string profileUrl = "https://api.github.com/user/emails";
            string profileHeader = "Authorization: Bearer " + accessToken;
            string profiel = Infrastructure.Http.GetWebRequest(profileUrl, profileHeader, "https://api.github.com/meta");

            //get the user email out of goolge profile
            //need to make the JSON more statndrd for us
            profiel = "{\"data\":" + profiel + "}";

            return profiel;
        }

        protected override string ConfigPrefix
        {
            get
            {
                return "Github";
            }
        }

        public override string GetLogOutRedirectUrl(string appName, string redirectUri = null)
        {
            throw new NotImplementedException();
        }

    }




}
