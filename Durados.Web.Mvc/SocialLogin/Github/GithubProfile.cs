using System.Collections.Generic;
using System.Linq;

namespace Durados.Web.Mvc.SocialLogin.Github
{
    public class GithubProfile : SocialProfile
    {
        public GithubProfile(Dictionary<string, object> dictionary) :
            base(dictionary)
        {

        }

        protected virtual string GetNamePart(string key)
        {
            try
            {
                return ((Dictionary<string, object>)dictionary["data"])[key].ToString();
            }
            catch
            {
                return null;
            }
        }

        protected override string firstNameInner
        {
            get { return emailInner.Split('@').FirstOrDefault(); }
        }

        protected override string lastNameInner
        {
            get { return string.Empty; }
        }

        protected override string emailInner
        {
            get
            {
                foreach (Dictionary<string, object> emailItem in (object[])dictionary["data"])
                {
                    if (emailItem["primary"].Equals(true))
                    {
                        return emailItem["email"].ToString();
                    }
                }
                return null;
            }
        }

        public override string Provider
        {
            get { return "github"; }
        }

    }
}
