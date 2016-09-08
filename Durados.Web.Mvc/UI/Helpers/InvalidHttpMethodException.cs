using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Durados.Web.Mvc.UI.Helpers
{
    public class InvalidHttpMethodException : DuradosException
    {
        public InvalidHttpMethodException(string method)
            : base("The method " + method + " is invalid. Please enter GET or POST only.")
        {

        }
    }
}
