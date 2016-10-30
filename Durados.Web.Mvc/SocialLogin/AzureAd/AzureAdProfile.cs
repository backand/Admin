using System.Collections.Generic;
using System.Linq;

namespace Durados.Web.Mvc.SocialLogin.Facebook
{
    public class AzureAdProfile : AdfsProfile
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
