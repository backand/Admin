using System.Collections.Generic;

namespace Durados.Web.Mvc.SocialLogin
{
    public abstract class SocialProfile
    {
        protected Dictionary<string, object> dictionary = null;

        public SocialProfile(Dictionary<string, object> dictionary)
        {
            if (dictionary == null ||
                !dictionary.ContainsKey("id") ||
                string.IsNullOrEmpty(dictionary["id"].ToString()))
            {
                throw new SocialException("Social Profile must have an id");
            }

            this.dictionary = dictionary;

            this.firstName = firstNameInner;
            this.lastName = lastNameInner;
            this.email = dictionary.ContainsKey("email") ? dictionary["email"].ToString() : emailInner;
            this.id = dictionary["id"].ToString();
            this.appName = dictionary["appName"].ToString();
            this.returnAddress = dictionary["returnAddress"].ToString();
            this.activity = dictionary["activity"].ToString();
            this.parameters = dictionary["parameters"].ToString();
        }

        public SocialProfile(string firstName, string lastName, string email, string id,
            string appName, string returnAddress, string activity, string parameters)
        {
            this.firstName = firstName;
            this.lastName = lastName;
            this.email = email;
            this.id = id;
            this.appName = appName;
            this.returnAddress = returnAddress;
            this.activity = activity;
            this.parameters = parameters;
        }

        public string id
        {
            get; set;
        }


        public string firstName { get; set; }

        public string lastName { get; set; }

        public string email { get; set; }

        protected abstract string firstNameInner { get; }

        protected abstract string lastNameInner { get; }

        protected abstract string emailInner { get; }


        public virtual Dictionary<string, object> additionalValues
        {
            get
            {
                return dictionary;
            }
        }

        public string appName;

        public string returnAddress;

        public string activity;

        public string parameters;

        public abstract string Provider
        {
            get;
        }
    }
}
