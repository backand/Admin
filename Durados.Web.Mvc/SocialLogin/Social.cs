using Durados.Web.Mvc.SocialLogin;
using System;
using System.Collections.Generic;

namespace Durados.Web.Mvc.UI.Helpers
{
    public abstract class SocialProvider
    {
        public abstract string GetAuthUrl(string appName, string returnAddress, string parameters, string activity);

        public abstract SocialProfile Authenticate();

        protected abstract SocialProfile GetNewProfile(Dictionary<string, object> dictionary);

        protected abstract SocialProfile GetProfileUnsafe(string appName, string accessToken, string returnAddress, string activity, string parameters);

        protected abstract Dictionary<string, object> GetKeys(string appName);

        protected abstract string ProviderName
        {
            get;
        }

        protected abstract SocialProfile FetchProfileByCode(string code, string appName, string returnUrl, string activity, string parameters, string redirectUrl);

        public SocialProfile Authenticate(string appName, string code, string returnUri)
        {
            return FetchProfileByCode(code, appName, "dummy", returnUri, string.Empty, null);
        }

        public SocialProfile Authenticate(SopcialLoginUserData data)
        {
            return FetchProfileByCode(data.code, data.appName, "dummy", string.Empty, string.Empty, data.redirectUri);
        }

        public SocialProfile GetProfile(string appName, string accessToken)
        {
            return GetProfileUnsafe(appName, accessToken, "dummy", "signin", string.Empty);
        }

        protected SocialProfile GetProfile(string appName, string accessToken, string returnAddress, string activity, string parameters)
        {
            try
            {
                return GetProfileUnsafe(appName, accessToken, returnAddress, activity, parameters);
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
    }
}
