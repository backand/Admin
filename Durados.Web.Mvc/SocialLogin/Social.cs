using Durados.Web.Mvc.SocialLogin;
using System;
using System.Collections.Generic;

namespace Durados.Web.Mvc.UI.Helpers
{
    public abstract class AbstractSocialProvider
    {
        public abstract string GetAuthUrl(string appName, string returnAddress, string parameters, string activity, string email);

        public abstract SocialProfile Authenticate();

        protected abstract SocialProfile GetNewProfile(Dictionary<string, object> dictionary);

        protected abstract string ProviderName
        {
            get;
        }

        protected abstract SocialProfile FetchProfileByCode(string code, string appName, string returnUrl, string activity, string parameters, string redirectUrl, string email);

        protected abstract string ConfigPrefix { get; }

        protected abstract SocialApplicationKeys GetSocialKeysFromDatabase(Map map);

        public SocialProfile Authenticate(string appName, string code, string returnUri)
        {
            return FetchProfileByCode(code, appName, "dummy", returnUri, string.Empty, null, null);
        }

        public SocialProfile Authenticate(SopcialLoginUserData data)
        {
            return FetchProfileByCode(data.code, data.appName, "dummy", string.Empty, string.Empty, data.redirectUri, null);
        }

        public SocialProfile GetProfile(string appName, string accessToken)
        {
            return GetProfileUnsafe(appName, accessToken, "dummy", "signin", string.Empty, null);
        }


        public SocialProfile GetProfile(string appName, string accessToken, string email)
        {
            return GetProfileUnsafe(appName, accessToken, "dummy", "signin", string.Empty, email);
        }

        protected SocialProfile GetProfile(string appName, string accessToken, string returnAddress, string activity, string parameters, string email)
        {
            try
            {
                return GetProfileUnsafe(appName, accessToken, returnAddress, activity, parameters, email);
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        protected virtual string GetRedirectUrl()
        {
            return System.Web.HttpContext.Current.Request.Url.Scheme + "://" + System.Web.HttpContext.Current.Request.Url.Authority + "/1/user/" + ProviderName + "/auth";
        }

        protected SocialApplicationKeys GetSocialKeys(string appName)
        {
            if (appName == Maps.DuradosAppName)
            {
                return GetDefaultApplicationKeys();
            }

            Map map = Maps.Instance.GetMap(appName);

            // application not exist strange case
            if (map == null)
            {
                return GetDefaultApplicationKeys();
            }

            var keys = GetSocialKeysFromDatabase(map);

            // don't have custom keys, use Backand global keys            
            if (keys.ClientId == null || keys.ClientSecret == null)
            {
                keys = GetDefaultApplicationKeys();
            }

            return keys;
        }

        protected SocialApplicationKeys GetDefaultApplicationKeys()
        {
            string clientId = System.Configuration.ConfigurationManager.AppSettings[ConfigPrefix + "ClientId"];
            string clientSecret = System.Configuration.ConfigurationManager.AppSettings[ConfigPrefix + "ClientSecret"];
            var defaultKeys = new SocialApplicationKeys() { ClientId = clientId, ClientSecret = clientSecret };

            return defaultKeys;
        }

        protected SocialProfile GetProfileUnsafe(string appName, string accessToken, string returnAddress, string activity, string parameters, string email)
        {
            //get the Google user profile using the access token
            string profiel = FetchProfileFromService(accessToken);

            //get the user email out of goolge profile
            //GoogleProfile googleProfile = (GoogleProfile)jsonSerializer.Deserialize<GoogleProfile>(profiel); ;
            Dictionary<string, object> profile = Durados.Web.Mvc.UI.Json.JsonSerializer.Deserialize(profiel);
            profile.Add("appName", appName);
            profile.Add("returnAddress", returnAddress);
            profile.Add("activity", activity);
            profile.Add("parameters", parameters);

            if (!string.IsNullOrEmpty(email))
            {
                if (!profile.ContainsKey("email"))
                {
                    profile.Add("email", "");
                }

                profile["email"] = email;
            }

            return GetNewProfile(profile);
        }

        protected abstract string FetchProfileFromService(string accessToken);
    }
}
