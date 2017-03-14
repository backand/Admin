using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Backand
{
    public class console
    {
        System.Web.Script.Serialization.JavaScriptSerializer javaScriptSerializer = new System.Web.Script.Serialization.JavaScriptSerializer();
        public void log(object message)
        {
            log(message, null);
        }
        public void log(object message, object o1)
        {
            log(message, o1, null);
        }
        public void log(object message, object o1, object o2)
        {
            log(message, o1, o2, null);
        }
        public void log(object message, object o1, object o2, object o3)
        {
            log(message, o1, o2, o3, null);
        }
        public void log(object message, object o1, object o2, object o3, object o4)
        {
            log(message, o1, o2, o3, o4, null);
        }
        public void log(object message, object o1, object o2, object o3, object o4, object o5)
        {
            log(message, o1, o2, o3, o4, o5, null);
        }
        public void log(object message, object o1, object o2, object o3, object o4, object o5, object o6)
        {
            log(message, o1, o2, o3, o4, o5, o6, null);
        }
        public void log(object message, object o1, object o2, object o3, object o4, object o5, object o6, object o7)
        {
            log(message, o1, o2, o3, o4, o5, o6, o7, null);
        }
        public void log(object message, object o1, object o2, object o3, object o4, object o5, object o6, object o7, object o8)
        {
            Log(message, o1, o2, o3, o4, o5, o6, o7, o8, 500);

        }

        public void error(object message)
        {
            error(message, null);
        }
        public void error(object message, object o1)
        {
            error(message, o1, null);
        }
        public void error(object message, object o1, object o2)
        {
            error(message, o1, o2, null);
        }
        public void error(object message, object o1, object o2, object o3)
        {
            error(message, o1, o2, o3, null);
        }
        public void error(object message, object o1, object o2, object o3, object o4)
        {
            error(message, o1, o2, o3, o4, null);
        }
        public void error(object message, object o1, object o2, object o3, object o4, object o5)
        {
            error(message, o1, o2, o3, o4, o5, null);
        }
        public void error(object message, object o1, object o2, object o3, object o4, object o5, object o6)
        {
            error(message, o1, o2, o3, o4, o5, o6, null);
        }
        public void error(object message, object o1, object o2, object o3, object o4, object o5, object o6, object o7)
        {
            error(message, o1, o2, o3, o4, o5, o6, o7, null);
        }
        public void error(object message, object o1, object o2, object o3, object o4, object o5, object o6, object o7, object o8)
        {
           Log(message, o1, o2, o3, o4, o5, o6, o7, o8, 501);
        }

        private void Log(object message, object o1, object o2, object o3, object o4, object o5, object o6, object o7, object o8, int logType)
        {
            string messageString = string.Empty;

            if (message is string)
            {
                messageString = (string)message;
            }
            else if (message is Jint.Native.Error.ErrorInstance)
            {
                messageString = message.ToString();
            }
            else
            {
                try
                {
                    if (message is IDictionary<string, object> && ((IDictionary<string, object>)message).Count == 1 && (((IDictionary<string, object>)message)).Values.First() is Jint.Native.Error.ErrorInstance)
                    {
                        messageString = (((IDictionary<string, object>)message)).Values.First().ToString();
                    }
                    else
                    {
                        messageString = Newtonsoft.Json.JsonConvert.SerializeObject(message);
                    }
                }
                catch
                {
                    messageString = message.ToString();
                }
            }
            
            object[] list = new object[8] { o1, o2, o3, o4, o5, o6, o7, o8 };
            foreach (object o in list)
            {
                if (o != null)
                {
                    if (o is string)
                    {
                        message += o.ToString();
                    }
                    else
                    {
                        messageString = Newtonsoft.Json.JsonConvert.SerializeObject(o);
                    }
                   
                }
            }

            Logger.Log(messageString, logType);
        }
    }

    internal class Logger
    {

        internal static void Log(string message, int logType = 500)
        {
            Guid requestGuid = (Guid)(Durados.Workflow.JavaScript.GetCacheInCurrentRequest(Durados.Workflow.JavaScript.GuidKey) ?? Guid.NewGuid());
            Durados.Database database = Durados.Workflow.Engine.GetCurrentDatabase();
            if (database == null) 
                return;
            database.Logger.Log("", "", Durados.Database.LogMessage, "", "", logType, message, DateTime.Now, requestGuid);

            if (!IsDebug())
                return;

            CheckLimit();

            database.Logger.Log("", "", "", "", "", logType, message, DateTime.Now, requestGuid);
            //using (System.Data.SqlClient.SqlConnection connection = new System.Data.SqlClient.SqlConnection(Durados.Workflow.JavaScript.GetCacheInCurrentRequest(Durados.Workflow.JavaScript.ConnectionStringKey).ToString()))
            //{
            //    connection.Open();
            //    string sql = "insert into durados_log ([Time], [LogType], [FreeText], [Guid]) values(@Time,@LogType,@FreeText,@Guid)";
            //    using (System.Data.SqlClient.SqlCommand command = new System.Data.SqlClient.SqlCommand(sql, connection))
            //    {
            //        command.Parameters.Add(new System.Data.SqlClient.SqlParameter("Time", DateTime.Now));
            //        command.Parameters.Add(new System.Data.SqlClient.SqlParameter("LogType", logType));
            //        command.Parameters.Add(new System.Data.SqlClient.SqlParameter("FreeText", message));
            //        command.Parameters.Add(new System.Data.SqlClient.SqlParameter("Guid", Durados.Workflow.JavaScript.GetCacheInCurrentRequest(Durados.Workflow.JavaScript.GuidKey)));
            //        command.ExecuteNonQuery();
            //    }
            //}
        }

        private static bool IsDebug()
        {
            return System.Convert.ToBoolean(Durados.Workflow.JavaScript.GetCacheInCurrentRequest(Durados.Workflow.JavaScript.Debug) ?? false) || System.Web.HttpContext.Current.Request.QueryString["$$debug$$"] != null;
        }

        private static void CheckLimit()
        {
            int counter = System.Convert.ToInt32(Durados.Workflow.JavaScript.GetCacheInCurrentRequest(Durados.Workflow.JavaScript.LogCounter) ?? 0);
            counter++;
            if (counter > Durados.Workflow.JavaScript.LogLimit)
            {
                throw new Durados.DuradosException(string.Format("Logging exceeds the {0} writings limit per a single excecution, please limit your writings or check for endless loops.", Durados.Workflow.JavaScript.LogLimit));
            }
            Durados.Workflow.JavaScript.SetCacheInCurrentRequest(Durados.Workflow.JavaScript.LogCounter, counter);
        }
    }
}
