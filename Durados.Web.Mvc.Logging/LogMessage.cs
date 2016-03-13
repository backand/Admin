using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Durados.Web.Mvc.Logging
{
   
    public class StashLogMessage : Dictionary<string,string>
    {
        public string ID
        {
            set
            {
                this["ID"] = value;
            }
        }

        public string ApplicationName
        {
            set
            {
                this["ApplicationName"] = value;
            }
        }
        public string Username
        {
            
            set
            {
                this["Username"] = value;
            }
        }
        public string MachineName 
        {
           set 
           {
                this["MachineName"] = value;
            } 
        }

        public string Time 
        { 
            set 
            {
                this["Time"] = value;
            }
        }
        public string Controller
        {
            
            set
            {
                this["Controller"] = value;
            }
        }
        public string Action { 
            set 
            {
                this["Action"] = value;
            } 
        }
        public string MethodName { 
            set 
            {
                this["MethodName"] = value;
            } 
        }
        public string LogType
        {
            set
            {
                this["LogType"] = value;
            }
        }
        public string ExceptionMessage
        {
            set
            {
                this["ExceptionMessage"] = value;
            }
        }
        public string Trace
        {
            set
            {
                this["Trace"] = value;
            }
        }
        public string FreeText 
        { 
            set 
            {
                this["FreeText"] = value;
            }
        }
        public string Guid 
        { 
            set 
            {
                this["Guid"] = value;
            }
        }

        public string ClientInfo { 
            set 
            {
                this["ClientInfo"] = value;
            }
        }

        public string ClientIP
        {
            set
            {
                this["ClientIP"] = value;
            }
        }

        public int? RequestTime 
        { 
            set 
            {
                this["RequestTime"] = value.Value.ToString(); 
            }
        }


        public StashLogMessage()
        {
            this["Source"] = "WebApi";
        }
    }
}
