using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Durados.Web.Mvc.Logging
{
   
    public class StashLogMessage
    {
        public string ID { get; set; }
        public string ApplicationName { get; set; }
        public string Username { get; set; }
        public string MachineName { get; set; }
        public string Time { get; set; }
        public string Controller { get; set; }
        public string Action { get; set; }
        public string MethodName { get; set; }
        public string LogType { get; set; }
        public string ExceptionMessage { get; set; }
        public string Trace { get; set; }
        public string FreeText { get; set; }
        public string Guid { get; set; }

        public string ClientInfo { get; set; }

        public string ClientIP { get; set; }

        public string Source = "WebApi";
    }
}
