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
            if (!dictionary.ContainsKey("upn"))
            {
                throw new AdfsException("adfs/oauth2/authorize response must contain upn");
            }
            if (!dictionary.ContainsKey("id"))
            {
                dictionary.Add("id", dictionary["upn"]);
            }
            
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

        public override string GetAuthUrl(string appName, string returnAddress, string parameters, string activity, string email, bool signupIfNotSignedIn)
        {
            AdfsApplicationKeys keys = (AdfsApplicationKeys)GetSocialKeys(appName);
            string clientId = keys.ClientId;
            string host = keys.Host;
            string resource = keys.Resource;
            Dictionary<string, object> state = new Dictionary<string, object>()
            { 
                { "stam", "stam" },
                { "appName", appName },
                { "returnAddress", System.Web.HttpContext.Current.Server.UrlEncode(returnAddress) },
                { "activity", activity },
                { "parameters", parameters ?? string.Empty },
                { "email", email ?? string.Empty },
                {"signupIfNotSignedIn", true}
            };
            string redirectUri = GetRedirectUrl(appName);

            string qsState = ConvertDictionaryToQueryString(state);
            //var jss = new JavaScriptSerializer();

            string authorizationEndpoint =
               host + "/adfs/oauth2/authorize" +
                    "?response_type=code" +
                    "&client_id=" + Uri.EscapeDataString(clientId) +
                    "&redirect_uri=" + Uri.EscapeDataString(redirectUri) + // + "?state=" + jss.Serialize(state)) +
                    "&resource=" + Uri.EscapeDataString(resource) +
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
            string code = System.Web.HttpContext.Current.Request.QueryString["code"];
            string error = System.Web.HttpContext.Current.Request.QueryString["error"];

            if (code == null || error != null)
            {
                throw new AdfsException(error);
            }
            try
            {

                if (System.Web.HttpContext.Current.Request.QueryString["appName"] == null)
                {
                    throw new AdfsException("Could not find the app name");
                }
                string appName = System.Web.HttpContext.Current.Request.QueryString["appName"].ToString();
                if (System.Web.HttpContext.Current.Request.QueryString["returnAddress"] == null)
                {
                    throw new AdfsException("Could not find the return address");
                }
                string returnAddress = System.Web.HttpContext.Current.Request.QueryString["returnAddress"];
                if (System.Web.HttpContext.Current.Request.QueryString["activity"] == null)
                {
                    throw new AdfsException("Could not find the activity");
                }
                string activity = System.Web.HttpContext.Current.Request.QueryString["activity"];
                if (System.Web.HttpContext.Current.Request.QueryString["parameters"] == null)
                {
                    throw new AdfsException("Could not find the parameters");
                }
                string parameters = System.Web.HttpContext.Current.Request.QueryString["parameters"];
                bool signupIfNotSignedIn = true;
                string email = null;

                return FetchProfileByCode(code, appName, returnAddress, activity, parameters, null, email, signupIfNotSignedIn);


               
            }
            catch (Exception exception)
            {
                throw new AdfsException(exception.Message, exception);
            }



        }

        private string GetRedirectUrl(string appName)
        {
            return GetRedirectUrl();// +"/" + appName;
        }

        protected override SocialProfile FetchProfileByCode(string code, string appName, string returnUrl, string activity, string parameters, string redirectUrl, string email, bool signupIfNotSignedIn)
        {

            AdfsApplicationKeys keys = (AdfsApplicationKeys)GetSocialKeys(appName);
            string clientId = keys.ClientId;
            string host = keys.Host;
            string urlAccessToken = host + "/adfs/oauth2/token";

            string redirectUri = GetRedirectUrl(appName);
            
            string accessTokenData = string.Format("grant_type=authorization_code&code={0}&client_id={1}&redirect_uri={2}", code, clientId, redirectUri);
            string response = Durados.Web.Mvc.Infrastructure.Http.PostWebRequest(urlAccessToken, accessTokenData);

            //get the access token from the return JSON
            //JavaScriptSerializer jsonSerializer = new JavaScriptSerializer();
            //AuthResponse validateResponse = (AuthResponse)jsonSerializer.Deserialize<AuthResponse>(response);

            Dictionary<string, object> validateResponse = Durados.Web.Mvc.UI.Json.JsonSerializer.Deserialize(response);

            string accessToken = validateResponse["access_token"].ToString();

            var profile = GetProfile(appName, accessToken, returnUrl, activity, parameters, email, signupIfNotSignedIn);

            return profile;
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

            string jwt = System.Text.Encoding.UTF8.GetString(System.Convert.FromBase64String(base64.PadRight(base64.Length + (4 - base64.Length % 4) % 4, '=')));

            
            return jwt;
        }
    }
}
