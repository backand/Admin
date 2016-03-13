using System.Collections.Generic;
using System.Linq;

namespace Durados.Web.Mvc.SocialLogin.Facebook
{
    public class FacebookProfile : SocialProfile
    {
        public FacebookProfile(Dictionary<string, object> dictionary) :
            base(dictionary)
        {

        }

        protected virtual string GetPart(string key)
        {
            if (dictionary.ContainsKey(key))
            {
                return dictionary[key].ToString();
            }
            else
            {
                if (key != "email")
                {
                    if (string.IsNullOrEmpty(email))
                    {
                        return null;
                    }
                    else
                    {
                        return email.Split('@').FirstOrDefault();
                    }
                }
                else
                {
                    return null;
                }
            }
        }

        protected override string firstNameInner
        {
            get { return GetPart("first_name"); }
        }

        protected override string lastNameInner
        {
            get { return GetPart("last_name"); }
        }

        protected override string emailInner
        {
            get { return GetPart("email"); }
        }

        public override string Provider
        {
            get { return "facebook"; }
        }
    }
}
