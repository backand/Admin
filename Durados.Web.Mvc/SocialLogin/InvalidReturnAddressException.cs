using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Durados.Web.Mvc.SocialLogin
{
    public class InvalidReturnAddressException : DuradosException
    {
        public InvalidReturnAddressException(string appName, string returnAddress)
            : base("The Return Address is invalid. Please set the app " + appName + " with the value " + returnAddress + " into the returnAddressURIs field.")
        {

        }
    }
}
