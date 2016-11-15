using Jint.Native;
using Jint.Runtime.Debug;
using System.Web.Script.Serialization;

namespace Jint.Runtime
{
    /// <summary>
    /// http://www.ecma-international.org/ecma-262/5.1/#sec-8.9
    /// </summary>
    public class Completion
    {
        public static string Normal = "normal";
        public static string Break = "break";
        public static string Continue = "continue";
        public static string Return = "return";
        public static string Throw = "throw";
        
        public Completion(string type, JsValue? value, string identifier)
        {
            Type = type;
            Value = value;
            Identifier = identifier;
        }

        public string Type { get; private set; }
        [ScriptIgnore]
        public JsValue? Value { get; private set; }
        [ScriptIgnore]
        public string Identifier { get; private set; }

        public Debug.DebugValue Val
        {
            get
            {
                if (!Value.HasValue)
                    return null;
                return new DebugValue(Value.Value);
            }
        }
        public JsValue GetValueOrDefault()
        {
            return Value.HasValue ? Value.Value : Undefined.Instance;
        }
    }
}
