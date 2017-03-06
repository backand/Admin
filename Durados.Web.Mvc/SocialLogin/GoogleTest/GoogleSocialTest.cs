using Durados.Web.Mvc.SocialLogin.Google;
using Durados.Web.Mvc.UI.Helpers;
using System;
using System.Collections.Generic;
using System.Web.Script.Serialization;

namespace Durados.Web.Mvc.SocialLogin
{
    public class GoogleSocialProviderTest : GoogleSocialProvider
    {
        protected override SocialProfile GetNewProfile(Dictionary<string, object> dictionary)
        {
            return new GoogleProfile(dictionary);
        }

        protected override string ProviderName
        {
            get { return "googletest"; }
        }

        protected override string ConfigPrefix
        {
            get
            {
                return "GoogleTest";
            }
        }

        
        protected override SocialProfile FetchProfileByCode(string code, string appName, string returnUrl, string activity, string parameters, string redirectUrl, string email, bool signupIfNotSignedIn, bool useHashRouting)
        {
            if (!IsTestApp(appName))
            {
                throw new DuradosException("not a test app");
            }
            var testProfile = GetTestProfile();
            return new GoogleProfile(testProfile);
        }

        private Dictionary<string, object> GetTestProfile()
        {
            var jss = new JavaScriptSerializer();
            string qsValue = System.Web.HttpContext.Current.Request.QueryString["test"];
            if (string.IsNullOrEmpty(qsValue))
            {
                throw new DuradosException("missing test in query string");
            }
            return jss.Deserialize<Dictionary<string, object>>(qsValue);
        }



        
    }
}
