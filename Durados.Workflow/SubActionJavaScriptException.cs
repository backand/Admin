﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Durados.Workflow
{
    public class SubActionJavaScriptException : DoNotLogException
    {
        public SubActionJavaScriptException(string message, Exception innerException = null)
            : base(message, innerException)
        {

        }
    }
}
