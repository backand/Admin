using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Durados.Workflow
{
    public class DoNotLogException : DuradosException
    {
        public DoNotLogException(string message, Exception innerException = null)
            : base(message, innerException)
        {

        }
    }
}
