using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Durados.Web.Mvc.UI.Helpers
{
    public class LambdaFunctionSelectionAlreadyExists : DuradosException
    {
        public LambdaFunctionSelectionAlreadyExists(string name)
            : base("The function " + name + " already exists.")
        {

        }
    }
}
