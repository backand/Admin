using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Durados.Web.Mvc.UI.Helpers
{
    class LambdaFunctionSelectionNotContainsArn : DuradosException
    {
        public LambdaFunctionSelectionNotContainsArn(string name)
            : base("The function " + name + " must have ARN.")
        {

        }
    }
}
