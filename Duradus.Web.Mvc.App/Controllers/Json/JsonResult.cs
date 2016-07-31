using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Durados.Web.Mvc.App.Controllers.Json
{
    public class JsonResult
    {
        public string Message { get; private set; }
        public bool Success { get; private set; }
        public string FailureType { get; private set; }

        public JsonResult(string message, bool success)
            : this(message, success, null)
        {
        }

        public JsonResult(string message, bool success, string failureType)
        {
            Message = message;
            Success = success;
            FailureType = failureType;
        }
    }
}
