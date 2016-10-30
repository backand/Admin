using Durados.Web.Mvc.UI.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Durados.Web.Mvc.SocialLogin
{
    public static class SocialProviderFactory
    {
        public static AbstractSocialProvider GetSocialProvider(string providerName)
        {
            switch (providerName.ToLower())
            {
                case "google":
                    return new GoogleSocialProvider();
                case "github":
                    return new GithubSocialProvider();
                case "facebook":
                    return new FacebookSocialProvider();
                case "twitter":
                    return new TwitterSocialProvider();
                case "adfs":
                    return new AdfsSocialProvider();
                case "azuread":
                    return new AzureAdSocialProvider();
                
                default:
                    return null;
            }
        }
    }
}
