using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Durados.Web.Mvc.SocialLogin
{
    public class RedirectState
    {
        public string AppName { get; set; }
        public string ReturnAddress { get; set; }
        public string Activity { get; set; }
        public string Parameters { get; set; }
        public bool SignupIfNotSignedIn { get; set; }
        public string Email { get; set; }

    }
}
