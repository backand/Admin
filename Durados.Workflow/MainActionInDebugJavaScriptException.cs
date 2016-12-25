using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Durados.Workflow
{
    public class MainActionInDebugJavaScriptException : DoNotLogException, IMainActionJavaScriptException
    {
        public string JintTrace { get; set; }
        public MainActionInDebugJavaScriptException(string message, Exception innerException = null)
            : base(message, innerException)
        {

        }
    }
}
