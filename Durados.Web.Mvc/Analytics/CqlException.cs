using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Durados.Web.Mvc.Analytics
{
    public class CqlException : DuradosException
    {
        public CqlException(string message) : base(message) { }
        public CqlException(string message, Exception innerException) : base(message, innerException) { }
    }
}
