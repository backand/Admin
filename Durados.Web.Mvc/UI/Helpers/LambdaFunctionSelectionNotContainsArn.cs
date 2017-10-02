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
    class FunctionCloudNotExists : DuradosException
    {
        public FunctionCloudNotExists(string name, int cloudId)
            : base("The function " + name + " cloudId " + cloudId.ToString() + " reference dose not exists")
        {

        }
    }
}
