using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;

namespace Durados.Workflow
{
    public class NodeJsException : Durados.DuradosException
    {
        public bool JsonFormat { get; private set; }
        public HttpStatusCode HttpStatusCode { get; private set; }
        public NodeJsException(string message, bool jsonFormat = false, HttpStatusCode httpStatusCode = HttpStatusCode.InternalServerError)
            : base(message)
        {
            JsonFormat = jsonFormat;
            HttpStatusCode = httpStatusCode;
        }
    }
}
