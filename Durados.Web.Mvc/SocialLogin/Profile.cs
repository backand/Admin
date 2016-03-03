using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Durados.Web.Mvc.SocialLogin
{
    public abstract class SocialProfile
    {
        protected Dictionary<string, object> dictionary = null;

        public SocialProfile(Dictionary<string, object> dictionary)
        {
            this.dictionary = dictionary;
        }

        public abstract string firstName { get; }

        public abstract string lastName { get; }

        public abstract string email { get; }

        public virtual Dictionary<string, object> additionalValues
        {
            get
            {
                return dictionary;
            }
        }


        public virtual string appName
        {
            get { return dictionary["appName"].ToString(); }
        }

        public virtual string returnAddress
        {
            get { return dictionary["returnAddress"].ToString(); }
        }

        public virtual string activity
        {
            get { return dictionary["activity"].ToString(); }
        }

        public virtual string parameters
        {
            get { return dictionary["parameters"].ToString(); }
        }
        public abstract string Provider
        {
            get;
        }
    }
}
