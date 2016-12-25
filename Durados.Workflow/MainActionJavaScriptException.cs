using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Durados.Workflow
{
    public class MainActionJavaScriptException : DuradosException, IMainActionJavaScriptException
    {
        public string JintTrace { get; set; }
        public MainActionJavaScriptException(string message, Exception innerException = null)
            : base(message, innerException)
        {

        }
    }
}
