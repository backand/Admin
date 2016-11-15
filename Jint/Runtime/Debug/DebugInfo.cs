using Jint.Native;
using Jint.Parser.Ast;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jint.Runtime.Debug
{
    public class DebugInfo
    {
        public DebugInfo(int customCodeStart)
        {
            expressionEvaluationList = new List<ExpressionEvaluation>();
            StatementEvaluationList = new Dictionary<int, StatementEvaluation>();
            CustomCodeStart = customCodeStart;
        }

        private List<ExpressionEvaluation> expressionEvaluationList;

        internal Dictionary<int, StatementEvaluation> StatementEvaluationList;

        internal void AddExpressionEvaluation(Expression expression, JsValue value, Statement statement, int statementIndex, Statement parentStatement, int? parentStatementIndex)
        {
            StatementEvaluation statementEvaluation = null;
            StatementEvaluation parentStatementEvaluation = null;
            if (parentStatementIndex.HasValue)
            {
                if (StatementEvaluationList.ContainsKey(parentStatementIndex.Value))
                {
                    parentStatementEvaluation = StatementEvaluationList[parentStatementIndex.Value];
                }
                else
                {
                    parentStatementEvaluation = new StatementEvaluation(parentStatement, parentStatementIndex.Value);
                    StatementEvaluationList.Add(parentStatementIndex.Value, parentStatementEvaluation);
                }
            }
            if (StatementEvaluationList.ContainsKey(statementIndex))
            {
                statementEvaluation = StatementEvaluationList[statementIndex];
            }
            else
            {
                statementEvaluation = new StatementEvaluation(statement, statementIndex, parentStatementEvaluation);
                StatementEvaluationList.Add(statementIndex, statementEvaluation);
            }

            ExpressionEvaluation expressionEvaluation = new ExpressionEvaluation(expression, value, statementEvaluation);

            //statementEvaluation.AddExpressionEvaluation(expressionEvaluation);

            AddExpressionEvaluation(expressionEvaluation);
        }

        private void AddExpressionEvaluation(ExpressionEvaluation expressionEvaluation)
        {
            expressionEvaluationList.Add(expressionEvaluation);
        }
        public ExpressionEvaluation[] Expressions
        {
            get
            {
                return expressionEvaluationList.ToArray();
            }
        }

        internal void SetStatementEvaluationCompletion(Completion complition, int index)
        {
            if (StatementEvaluationList.ContainsKey(index))
            {
                StatementEvaluationList[index].Completion = complition;
            }
        }

        internal int CustomCodeStart { get; private set; }
    }
}
