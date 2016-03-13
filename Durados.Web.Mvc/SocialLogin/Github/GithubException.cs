using System;

namespace Durados.Web.Mvc.SocialLogin.Github
{
    public class GithubException : SocialException
    {
        public GithubException(string message)
            : base(message)
        {

        }

        public GithubException(string message, Exception innerException)
            : base(message)
        {

        }
    }
}
