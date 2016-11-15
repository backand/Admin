using Jint.Parser.Ast;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jint.Runtime.Debug
{
    public class StatementEvaluation
    {
        internal Statement Statement { get; private set; }
        internal int Index { get; private set; }

        private List<ExpressionEvaluation> expressionEvaluationList;
        public Completion Completion { get; internal set; }
        
        public StatementEvaluation StatementParent { get; private set; }

        public StatementEvaluation(Statement statement, int index, StatementEvaluation statementEvaluationParent = null)
        {
            Statement = statement;
            Index = index;
            StatementParent = statementEvaluationParent;
            expressionEvaluationList = new List<ExpressionEvaluation>();
        }

        internal void AddExpressionEvaluation(ExpressionEvaluation expressionEvaluation)
        {
            expressionEvaluationList.Add(new ExpressionEvaluation(expressionEvaluation.Expression, expressionEvaluation.JsValue, null));
        }
        public ExpressionEvaluation[] Expressions
        {
            get
            {
                return expressionEvaluationList.ToArray();
            }
        }
    }
}
