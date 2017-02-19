using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BackAnd.Web.Api.Controllers.Filters
{
    public class UnauthorizedSocialProviderException : Durados.DuradosException
    {
        public UnauthorizedSocialProviderException(string provider)
            : base("The " + provider + " provider is unauthorized.")
        {

        }
    }
}
