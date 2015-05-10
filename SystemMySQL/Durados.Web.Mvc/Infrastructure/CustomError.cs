using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Durados.Web.Mvc.Infrastructure
{
    public class CustomError
    {
        public string Title { get; set; }
        public string Message { get; set; }
        public string ViewName { get; set; }
        public Durados.Diagnostics.ILog Log { get; private set; }

        public CustomError(Durados.Diagnostics.ILog log, string viewName)
        {
            Title = "Oops, an unexpected error occurred";
            if (log!=null)
                Message = log.ExceptionMessage;

            ViewName = viewName;
            Log = log;
        }

        public string GetTitle()
        {
            string appName = string.Empty;

            
                try
                {
                    appName = Durados.Web.Mvc.Maps.GetCurrentAppName();
                }
                catch
                {
                    appName = "Initiation";
                }
            
            return appName + " - Exception";
        }
    }
}
