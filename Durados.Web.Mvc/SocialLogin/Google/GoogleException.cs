using System;

namespace Durados.Web.Mvc.SocialLogin.Google
{
    public class GoogleException : SocialException
    {
        public GoogleException(string message)
            : base(message)
        {

        }

        public GoogleException(string message, Exception innerException)
            : base(message)
        {

        }
    }
}
