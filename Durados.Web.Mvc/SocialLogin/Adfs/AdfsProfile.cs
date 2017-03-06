using System.Collections.Generic;
using System.Linq;

namespace Durados.Web.Mvc.SocialLogin.Adfs
{
    public class AdfsProfile : SocialProfile
    {
        public AdfsProfile(Dictionary<string, object> dictionary) :
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
                    if (string.IsNullOrEmpty(emailInner))
                    {
                        return null;
                    }
                    else
                    {
                        return emailInner.Split('@').FirstOrDefault();
                    }
                }
                else
                {
                    return dictionary["upn"].ToString();
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
            get { return "adfs"; }
        }
    }
}
