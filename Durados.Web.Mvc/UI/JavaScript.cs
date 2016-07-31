using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Durados.Web.Mvc.UI
{
    public static class JavaScript
    {
        public static string ConvertToLegalVariable(string variable)
        {
            return variable.Replace("-", "_");
        }
    }
}
