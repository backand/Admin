using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Durados.Workflow
{
    public class NodeJsException : Durados.DuradosException
    {
        public NodeJsException(string message)
            : base(message)
        {

        }
    }
}
