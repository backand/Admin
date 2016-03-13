using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Durados.Web.Mvc.SocialLogin
{
    public class SocialException : DuradosException
    {
        public SocialException(string message)
            : base(message)
        {

        }

        public SocialException(string message, Exception innerException)
            : base(message)
        {

        }
    }
}
