using System.Diagnostics;
using System.Web.Script.Serialization;

namespace Jint.Parser.Ast
{
    public class SyntaxNode
    {
        [ScriptIgnore]
        public SyntaxNodes Type;
        public int[] Range;
        public Location Location;

        [DebuggerStepThrough]
        public T As<T>() where T : SyntaxNode
        {
            return (T)this;
        }
    }
}
