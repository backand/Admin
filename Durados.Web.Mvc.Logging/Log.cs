using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Durados.Web.Mvc.Logging
{
    public class Log : Durados.Diagnostics.ILog
    {
        public string MachineName { get; set; }
        public string Controller { get; set; }
        public string Action { get; set; }
        public string MethodName { get; set; }
        public string ExceptionMessage { get; set; }
        public string Trace { get; set; }
        public DateTime Time { get; set; }
    }
}
