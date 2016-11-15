using Jint.Native;
using Jint.Parser.Ast;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jint.Runtime.Debug
{
    public class ExpressionEvaluation
    {
        internal ExpressionEvaluation(Expression expression, JsValue jsValue, StatementEvaluation statementEvaluation)
        {
            Expression = expression;
            JsValue = jsValue;
            Value = new DebugValue(jsValue.Type.ToString(), jsValue.ToString());
            Statement = statementEvaluation;
        }

        public Expression Expression { get; private set; }
        internal JsValue JsValue { get; private set; }

        public DebugValue Value { get; private set; }
        public StatementEvaluation Statement { get; private set; }
       
    }
}
