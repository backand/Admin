using System;

namespace Durados.Web.Mvc.SocialLogin.Google
{
    public class TwitterException : SocialException
    {
        public TwitterException(string message)
            : base(message)
        {

        }

        public TwitterException(string message, Exception innerException)
            : base(message)
        {

        }
    }
}
