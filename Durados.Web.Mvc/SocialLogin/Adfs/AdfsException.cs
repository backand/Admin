using System;

namespace Durados.Web.Mvc.SocialLogin.Facebook
{
    public class AdfsException : SocialException
    {
        public AdfsException(string message)
            : base(message)
        {

        }

        public AdfsException(string message, Exception innerException)
            : base(message)
        {

        }
    }
}
