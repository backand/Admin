using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Durados.Web.Mvc.SocialLogin
{
    public class AdfsApplicationKeys : SocialApplicationKeys
    {
        public string Resource { get; set; }
        public string Host { get; set; }
    }
}
