using System;

namespace Durados.Web.Mvc.SocialLogin.Facebook
{
    public class FacebookException : SocialException
    {
        public FacebookException(string message)
            : base(message)
        {

        }

        public FacebookException(string message, Exception innerException)
            : base(message)
        {

        }
    }
}
