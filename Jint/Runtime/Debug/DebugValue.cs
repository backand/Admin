using Jint.Native;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Jint.Runtime.Debug
{
    public class DebugValue
    {
        public string Type { get; private set; }
        public string Value { get; private set; }

        public DebugValue(JsValue jsValue)
            : this(jsValue.Type.ToString(), jsValue.ToString())
        {

        }
        
        public DebugValue(string type, string value)
        {
            Type = type;
            Value = value;
        }
    }
}
