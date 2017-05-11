using System.Collections.Generic;
using System.Linq;

namespace Durados.Web.Mvc.SocialLogin.Google
{
    public class TwitterProfile : SocialProfile
    {
        public TwitterProfile(Dictionary<string, object> dictionary) :
            base(dictionary)
        {

        }

        public TwitterProfile(string email, string id, string returnUrl, string activity, string appName, string parameters, bool signupIfNotSignedIn, bool useHashRouting) :
            base(string.IsNullOrEmpty(email) ? "" : email.Split('@').FirstOrDefault() // if email exist, first name can be the prefix part of email address
                , "", email, id, appName, returnUrl, activity, parameters, signupIfNotSignedIn, useHashRouting)
        {
            dictionary = new Dictionary<string, object>() { { "provider", Provider } };
        }

        protected override string firstNameInner
        {
            get { return null; }
        }

        protected override string lastNameInner
        {
            get { return null; }
        }

        protected override string emailInner
        {
            get { return null; }
        }

        public override string Provider
        {
            get { return "twitter"; }
        }
    }

}
