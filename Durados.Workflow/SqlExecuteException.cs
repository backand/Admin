using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Durados.Workflow
{
    public class SqlExecuteException : WorkflowEngineException
    {
        public SqlExecuteException(string message)
            : base(message)
        {

        }
    }
}


