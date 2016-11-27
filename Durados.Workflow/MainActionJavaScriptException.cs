using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Durados.Workflow
{
    public class MainActionJavaScriptException : DuradosException
    {
        public MainActionJavaScriptException(string message, Exception innerException = null)
            : base(message, innerException)
        {

        }
    }
}
