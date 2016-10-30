using Durados.Web.Mvc.SocialLogin.Facebook;
using Durados.Web.Mvc.UI.Helpers;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Web.Script.Serialization;

namespace Durados.Web.Mvc.SocialLogin
{
    public class AzureAdSocialProvider : AdfsSocialProvider
    {
        
        protected override string ProviderName
        {
            get { return "azuread"; }
        }

        protected override string ConfigPrefix
        {
            get
            {
                return "azuread";
            }
        }

        protected override SocialProfile GetNewSocialProfile(Dictionary<string, object> dictionary)
        {
            return new AzureAdProfile(dictionary);
        }

        protected override string GetAccessTokenData(string code, string clientId, string redirectUri, string resource = null)
        {
            if (string.IsNullOrEmpty(resource))
            {
                resource = "https://graph.windows.net/";
            }

            return string.Format("grant_type=authorization_code&code={0}&client_id={1}&redirect_uri={2}&resource={3}", code, clientId, redirectUri, resource);
        }

        protected override SocialApplicationKeys GetSocialKeysFromDatabase(Map map)
        {
            return new AdfsApplicationKeys { ClientId = map.Database.AzureAdClientId ?? System.Configuration.ConfigurationManager.AppSettings["AzureAdClientId"], Resource = map.Database.AzureAdResource ?? System.Configuration.ConfigurationManager.AppSettings["AzureAdResource"], Host = map.Database.AzureAdHost ?? System.Configuration.ConfigurationManager.AppSettings["AzureAdHost"], ClientSecret = string.Empty };
        }
    }
}
