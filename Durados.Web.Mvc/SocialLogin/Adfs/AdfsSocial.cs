using Durados.Web.Mvc.SocialLogin.Facebook;
using Durados.Web.Mvc.UI.Helpers;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Web.Script.Serialization;

namespace Durados.Web.Mvc.SocialLogin
{
    public class AdfsSocialProvider : AbstractSocialProvider
    {
        protected override SocialProfile GetNewProfile(Dictionary<string, object> dictionary)
        {
            if (!(dictionary.ContainsKey("upn") || dictionary.ContainsKey("email")))
            {
                throw new AdfsException("adfs/oauth2/authorize response must contain upn or email");
            }
            if (!dictionary.ContainsKey("id"))
            {
                if (dictionary.ContainsKey("upn"))
                    dictionary.Add("id", dictionary["upn"]);
                else
                    dictionary.Add("id", dictionary["email"]);
            }

            return GetNewSocialProfile(dictionary);
        }

        protected virtual SocialProfile GetNewSocialProfile(Dictionary<string, object> dictionary)
        {
            return new AdfsProfile(dictionary); 
        }

        protected override string ProviderName
        {
            get { return "adfs"; }
        }

        protected override string ConfigPrefix
        {
            get
            {
                return "adfs";
            }
        }

        public override string GetAuthUrl(string appName, string returnAddress, string parameters, string activity, string email, bool signupIfNotSignedIn, bool useHashRouting)
        {
            AdfsApplicationKeys keys = (AdfsApplicationKeys)GetSocialKeys(appName);
            string clientId = keys.ClientId;
            string oauth2EndPoint = keys.Host;
            string resource = keys.Resource;
            Dictionary<string, object> state = new Dictionary<string, object>()
            { 
                { "stam", "stam" },
                { "appName", appName },
                { "returnAddress", System.Web.HttpContext.Current.Server.UrlEncode(returnAddress) },
                { "activity", activity },
                { "parameters", parameters ?? string.Empty },
                { "email", email ?? string.Empty },
                {"signupIfNotSignedIn", true},
                {"useHashRouting", useHashRouting}
            };
            string redirectUri = GetRedirectUrl(appName);

            string qsState = ConvertDictionaryToQueryString(state);
            //var jss = new JavaScriptSerializer();

            string authorizationEndpoint =
               oauth2EndPoint + "/authorize" +
                    "?response_type=code" +
                    "&client_id=" + Uri.EscapeDataString(clientId) +
                    "&redirect_uri=" + Uri.EscapeDataString(redirectUri) + // + "?state=" + jss.Serialize(state)) +
                    (string.IsNullOrEmpty(resource) ? string.Empty : ("&resource=" + Uri.EscapeDataString(resource))) +
                    "&state=" + Uri.EscapeDataString(qsState);

            return authorizationEndpoint;
        }

        private string ConvertDictionaryToQueryString(Dictionary<string, object> dictionary)
        {
            var parameters = new List<string>();
            foreach (var item in dictionary)
            {
                if (item.Value != null)
                {
                    parameters.Add(item.Key + "=" + item.Value.ToString());
                }
            }

            return String.Join("&", parameters);

        }

        private Dictionary<string, object> ConvertQueryStringToDictionary(string qs)
        {
            Dictionary<string, object> dictionary = new Dictionary<string, object>();
            var parameters =  qs.Split('&');

            foreach (var parameter in parameters)
            {
                var pair = parameter.Split('=');

                if (pair.Length == 2)
                {
                    dictionary.Add(pair[0], pair[1]);
                }

            }

            return dictionary;
        }

        
        public override SocialProfile Authenticate()
        {
            //get the code from Google and request from access token

            var qs = System.Web.HttpContext.Current.Request.QueryString;

            string code = qs["code"];
            string error = qs["error"];

            if (code == null || error != null)
            {
                throw new AdfsException(error);
            }
            try
            {

                RedirectState redirectState = GetRedirectState();

                return FetchProfileByCode(code, redirectState.AppName, redirectState.ReturnAddress, redirectState.Activity, redirectState.Parameters, null, redirectState.Email, redirectState.SignupIfNotSignedIn, redirectState.UseHashRouting);


               
            }
            catch (Exception exception)
            {
                throw new AdfsException(exception.Message, exception);
            }



        }

        private RedirectState GetRedirectState()
        {
            var qs = System.Web.HttpContext.Current.Request.QueryString;
            if (qs["appName"] == null)
            {
                return GetRedirectStateFromState();
            }
            else
            {
                return GetRedirectStateDirect();
            }
        }
        private RedirectState GetRedirectStateDirect()
        {
            RedirectState redirectState = new SocialLogin.RedirectState();
            var qs = System.Web.HttpContext.Current.Request.QueryString;
            if (qs["appName"] == null)
            {
                throw new AdfsException("Could not find the app name");
            }
            redirectState.AppName = qs["appName"].ToString();
            if (qs["returnAddress"] == null)
            {
                throw new AdfsException("Could not find the return address");
            }
            redirectState.ReturnAddress = qs["returnAddress"];
            if (qs["activity"] == null)
            {
                throw new AdfsException("Could not find the activity");
            }
            redirectState.Activity = qs["activity"];
            if (qs["parameters"] == null)
            {
                throw new AdfsException("Could not find the parameters");
            }
            redirectState.Parameters = qs["parameters"];
            redirectState.UseHashRouting = true;
            if (qs["useHashRouting"].ToLower() == "false")
            {
                redirectState.UseHashRouting = false;
            }
            redirectState.SignupIfNotSignedIn = true;
            redirectState.Email = null;

            return redirectState;
        }
        private RedirectState GetRedirectStateFromState()
        {
            var qs = System.Web.HttpContext.Current.Request.QueryString;
            RedirectState redirectState = new SocialLogin.RedirectState();

            if (qs["state"] == null)
            {
                throw new AdfsException("Could not find the state");
            }

            string[] state = qs["state"].Split('&');

            redirectState.UseHashRouting = true;

            foreach (string keyValue in state)
            {
                string[] keyValueArray = keyValue.Split('=');

                 if (keyValueArray.Length == 2)
                {
                    string key = keyValueArray[0];
                    string value = keyValueArray[1];

                    
                    if (key == "appName")
                    {
                        redirectState.AppName = value;
                    }

                    if (key == "returnAddress")
                    {
                        redirectState.ReturnAddress = System.Web.HttpContext.Current.Server.UrlDecode(value);
                    }
                    if (key == "useHashRouting" && value.ToLower() == "false")
                    {
                        redirectState.UseHashRouting = false;
                    }

                    if (key == "activity")
                    {
                        redirectState.Activity = value;
                    }

                    if (key == "parameters")
                    {
                        redirectState.Parameters = value;
                    }
                    
                }
            }

            if (redirectState.AppName == null)
            {
                throw new AdfsException("Could not find the app name");
            }
            
            
            redirectState.SignupIfNotSignedIn = true;
            redirectState.Email = null;

            return redirectState;
        }

        private string GetRedirectUrl(string appName)
        {
            return GetRedirectUrl();// +"/" + appName;
        }

        public override SocialProfile FetchProfileByRefreshToken(string refreshToken, string appName)
        {

            AdfsApplicationKeys keys = (AdfsApplicationKeys)GetSocialKeys(appName);
            string clientId = keys.ClientId;
            string oauth2EndPoint = keys.Host;
            string urlAccessToken = oauth2EndPoint + "/token";

            string redirectUri = GetRedirectUrl(appName);

            string accessTokenData = GetRefreshTokenData(refreshToken, clientId, redirectUri, keys.Resource);

            string response = null;

            try
            {
                response = Durados.Web.Mvc.Infrastructure.Http.PostWebRequest(urlAccessToken, accessTokenData);
            }
            catch (System.Net.WebException exception)
            {
                try
                {
                    string errorDescription = new System.IO.StreamReader((exception).Response.GetResponseStream()).ReadToEnd();
                    throw new AdfsException(errorDescription);
                }
                catch { }
                throw exception;
            }
            //get the access token from the return JSON
            //JavaScriptSerializer jsonSerializer = new JavaScriptSerializer();
            //AuthResponse validateResponse = (AuthResponse)jsonSerializer.Deserialize<AuthResponse>(response);

            Dictionary<string, object> validateResponse = Durados.Web.Mvc.UI.Json.JsonSerializer.Deserialize(response);

            string newRefreshToken = validateResponse["refresh_token"].ToString();
            string accessToken = validateResponse["access_token"].ToString();

            var profile = GetProfile(appName, accessToken, newRefreshToken, null, null, null, null, false, false);

            return profile;
        }

        protected override SocialProfile FetchProfileByCode(string code, string appName, string returnUrl, string activity, string parameters, string redirectUrl, string email, bool signupIfNotSignedIn, bool useHashRouting)
        {

            AdfsApplicationKeys keys = (AdfsApplicationKeys)GetSocialKeys(appName);
            string clientId = keys.ClientId;
            string oauth2EndPoint = keys.Host;
            string urlAccessToken = oauth2EndPoint + "/token";
            
            string redirectUri = GetRedirectUrl(appName);

            string accessTokenData = GetAccessTokenData(code, clientId, redirectUri, keys.Resource);

            string response = null;

            try
            {
                response = Durados.Web.Mvc.Infrastructure.Http.PostWebRequest(urlAccessToken, accessTokenData);
            }
            catch (System.Net.WebException exception)
            {
                try
                {
                    string errorDescription = new System.IO.StreamReader((exception).Response.GetResponseStream()).ReadToEnd();
                    throw new AdfsException(errorDescription);
                }
                catch { }
                throw exception;
            }
            //get the access token from the return JSON
            //JavaScriptSerializer jsonSerializer = new JavaScriptSerializer();
            //AuthResponse validateResponse = (AuthResponse)jsonSerializer.Deserialize<AuthResponse>(response);

            Dictionary<string, object> validateResponse = Durados.Web.Mvc.UI.Json.JsonSerializer.Deserialize(response);

            string accessToken = validateResponse["access_token"].ToString();
            string refreshToken = validateResponse["refresh_token"].ToString();

            var profile = GetProfile(appName, accessToken, refreshToken, returnUrl, activity, parameters, email, signupIfNotSignedIn, useHashRouting);

            return profile;
        }

        protected virtual string GetAccessTokenData(string code, string clientId, string redirectUri, string resource)
        {
            string accessTokenData = string.Format("grant_type=authorization_code&code={0}&client_id={1}&redirect_uri={2}", code, clientId, redirectUri);
            return accessTokenData;
        }

        protected virtual string GetRefreshTokenData(string refreshToken, string clientId, string redirectUri, string resource)
        {
            string accessTokenData = string.Format("grant_type=refresh_token&refresh_token={0}&client_id={1}&redirect_uri={2}", refreshToken, clientId, redirectUri);
            return accessTokenData;
        }

        protected override SocialApplicationKeys GetSocialKeysFromDatabase(Map map)
        {
            return new AdfsApplicationKeys { ClientId = map.Database.AdfsClientId ?? System.Configuration.ConfigurationManager.AppSettings["AdfsClientId"], Resource = map.Database.AdfsResource ?? System.Configuration.ConfigurationManager.AppSettings["AdfsResource"], Host = map.Database.AdfsHost ?? System.Configuration.ConfigurationManager.AppSettings["AdfsHost"], ClientSecret = string.Empty };
        }

        protected override string FetchProfileFromService(string accessToken)
        {
            var array = accessToken.Split('.');
            if (array.Length != 3)
            {
                throw new AdfsException("adfs access token does not contain the jwt part.");
            }
            string base64 = array[1];

            string jwt = null;
            try
            {
                jwt = base64.FromBase64String();
            }
            catch (Exception exception)
            {
                throw new AdfsException("Fail to convert from base64: " + base64, exception);
            }
            
            return jwt;
        }

        public override string GetLogOutRedirectUrl(string appName, string redirectUri = null)
        {
            AdfsApplicationKeys keys = (AdfsApplicationKeys)GetSocialKeys(appName);
            string clientId = keys.ClientId;
            string oauth2EndPoint = keys.Host;
            
            string url = oauth2EndPoint + "/logout";
            if (!string.IsNullOrEmpty(redirectUri))
            {
                url += "?post_logout_redirect_uri=" + redirectUri;
            }

            return url;
        }
    }
}
