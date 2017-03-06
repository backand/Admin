using System.Collections.Generic;
using System.Linq;

namespace Durados.Web.Mvc.SocialLogin.AzureAd
{
    public class AzureAdProfile : Durados.Web.Mvc.SocialLogin.Adfs.AdfsProfile
    {
        public AzureAdProfile(Dictionary<string, object> dictionary) :
            base(dictionary)
        {

        }

        
        public override string Provider
        {
            get { return "azuread"; }
        }
    }
}
