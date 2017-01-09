using System;
using System.Collections.Generic;

namespace Backand
{
    public class socket : ISocket
    {

        public static List<object> SentMessagesMock;

        protected virtual string GetNodeUrl()
        {
            return System.Configuration.ConfigurationManager.AppSettings["nodeHost"] ?? "http://127.0.0.1:9000";
        }

        protected virtual string GetAppName()
        {
            return (System.Web.HttpContext.Current.Items[Durados.Database.AppName] ?? string.Empty).ToString();
        }

        /// <summary>
        /// Send message to all users in app
        /// </summary>
        /// <param name="eventName"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        public virtual object emitAll(string eventName, object data)
        {
            return emitAll(eventName, data, null);
        }


        public virtual object emitAll(string eventName, object data, string appName)
        {
            return EmitPrivate(eventName, data, appName, "All");

        }

        /// <summary>
        /// send message to users in app with specific role
        /// </summary>
        /// <param name="eventName"></param>
        /// <param name="data"></param>
        /// <param name="role"></param>
        /// <returns></returns>
        public virtual object emitRole(string eventName, object data, string role)
        {
            if (string.IsNullOrEmpty(role))
            {
                return new { status = 502, message = "if emitRole is called, you must specify a role as parameter " };
            }

            return EmitPrivate(eventName, data, null, "Role", role);
        }

        public virtual object emitRole(string eventName, object data, string role, string appName)
        {
            if (string.IsNullOrEmpty(role))
            {
                return new { status = 502, message = "if emitRole is called, you must specify a role as parameter " };
            }

            return EmitPrivate(eventName, data, appName, "Role", role);
        }

        /// <summary>
        /// send message to specific users
        /// </summary>
        /// <param name="eventName"></param>
        /// <param name="data"></param>
        /// <param name="users"></param>
        /// <returns></returns>
        public virtual object emitUsers(string eventName, object data, object users)
        {
            if (users == null)
            {
                return new { status = 502, message = "if emitUsers is called, you must specify users as array parameter " };
            }

            return EmitPrivate(eventName, data, null, "Users", null, users);
        }

        private object EmitPrivate(string eventName, object data, string appName, string mode, string role = "", object users = null)
        {
            try
            {
                XMLHttpRequest xmlHttpRequest = new XMLHttpRequest();
                xmlHttpRequest.open("POST", GetNodeUrl() + "/socket/emit", false);
                xmlHttpRequest.setRequestHeader("Content-Type", "application/json");
                xmlHttpRequest.setRequestHeader("app", appName ?? GetAppName());
                System.Web.Script.Serialization.JavaScriptSerializer serializer = new System.Web.Script.Serialization.JavaScriptSerializer();

                // data
                Dictionary<string, object> newData = new Dictionary<string, object>();
                newData.Add("eventName", eventName);
                newData.Add("mode", mode);
                

                switch (mode)
                {
                    case "Role":
                        newData.Add("role", role);
                        break;
                    case "Users":
                        newData.Add("users", users);
                        break;
                    default:
                        break;
                }


                object json = data;
                try
                {
                    if (data != null && data is string)
                        json = serializer.Deserialize<Dictionary<string, object>>((string)data);
                }
                catch
                {

                }
                
                newData.Add("data", json);

                // save requests for tests
                if (SentMessagesMock != null)
                {
                    SentMessagesMock.Add(xmlHttpRequest);
                }

                // sehd data to server
                xmlHttpRequest.send(serializer.Serialize(newData));

                if (Logger != null)
                    Logger.Log("socket", "emit", eventName, null, 3, "emit " + mode + " " + eventName, DateTime.Now, 0);

                // send response to user
                return new { status = xmlHttpRequest.status, message = xmlHttpRequest.responseText };
            }
            catch (Exception exception)
            {
                if (Logger != null)
                    Logger.Log("socket", "emit", eventName, exception, 1, "emit " + mode + " " + eventName, DateTime.Now, 0);
                return new { status = 502, message = exception.Message };
            }
        }

        private Durados.Diagnostics.ILogger Logger
        {
            get
            {
                Durados.Database dataabase = Durados.Workflow.Engine.GetCurrentDatabase();
                if (dataabase == null)
                    return null;
                return (Durados.Diagnostics.ILogger)dataabase.Logger;
            }
        }
        
    }

    public interface ISocket
    {
        object emitAll(string eventName, object data);

        object emitAll(string eventName, object data, string appName);

        object emitRole(string eventName, object data, string role);

        object emitUsers(string eventName, object data, object users);

    }
}
