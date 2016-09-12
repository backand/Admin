using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BackAnd.Web.Api.Controllers
{
    class UserNotSignedUpSocialException : Durados.Web.Mvc.SocialLogin.SocialException
    {
        public UserNotSignedUpSocialException(string appName) : base("The user is not signed up to " + appName)
        {

        }
    }
}
