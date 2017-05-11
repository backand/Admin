using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Durados.Web.Mvc.UI.Helpers
{
    public class LambdaFunctionSelectionNotFound : DuradosException
    {
        public LambdaFunctionSelectionNotFound(string name)
            : base("The function " + name + " was not found.")
        {

        }
    }
}
