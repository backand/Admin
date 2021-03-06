﻿using System.Collections.Generic;
using System.Linq;

namespace Durados.Web.Mvc.SocialLogin.Google
{
    public class GoogleProfile : SocialProfile
    {
        public GoogleProfile(Dictionary<string, object> dictionary) :
            base(dictionary)
        {

        }

        protected virtual string GetNamePart(string key)
        {
            if (dictionary.ContainsKey("name"))
            {
                Dictionary<string, object> names = (Dictionary<string, object>)dictionary["name"];
                if (names.ContainsKey(key))
                {
                    return names[key].ToString();
                }
                else if (!string.IsNullOrEmpty(emailInner))
                {
                    return emailInner.Split('@').FirstOrDefault();
                }
                return null;
            }
            else
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
        }

        protected override string firstNameInner
        {
            get { return GetNamePart("givenName"); }
        }

        protected override string lastNameInner
        {
            get { return GetNamePart("familyName"); }
        }

        protected override string emailInner
        {
            get
            {
                if (dictionary.ContainsKey("emails"))
                {
                    return ((Dictionary<string, object>)((object[])dictionary["emails"])[0])["value"].ToString();
                }

                return null;
            }
        }

        public override string Provider
        {
            get { return "google"; }
        }
    }

}
