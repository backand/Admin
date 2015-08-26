using System;
using System.Collections.Generic;

namespace Backand
{
    public class socket : ISocket
    {
        protected virtual string GetNodeUrl()
        {
            return System.Configuration.ConfigurationManager.AppSettings["nodeHost"] ?? "http://127.0.0.1:9000";
        }

        protected virtual string GetAppName()
        {
            return (System.Web.HttpContext.Current.Items[Durados.Database.AppName] ?? string.Empty).ToString();
        }

        public virtual object emit(string eventName, string data)
        {
            return emit(eventName, data);
        }

        public virtual object emit(string eventName, string data, string appName)
        {
            try
            {
                XMLHttpRequest xmlHttpRequest = new XMLHttpRequest();
                xmlHttpRequest.open("POST", GetNodeUrl() + "/socket/emit", false);
                xmlHttpRequest.setRequestHeader("Content-Type", "application/json");
                xmlHttpRequest.setRequestHeader("app", appName ?? GetAppName());
                System.Web.Script.Serialization.JavaScriptSerializer serializer = new System.Web.Script.Serialization.JavaScriptSerializer();
                Dictionary<string, object> newData = new Dictionary<string, object>();
                newData.Add("eventName", eventName);
                newData.Add("data", serializer.Deserialize<Dictionary<string, object>>(data));
                xmlHttpRequest.send(serializer.Serialize(newData));
                return new { status = xmlHttpRequest.status, message = xmlHttpRequest.responseText };
            }
            catch (Exception exception)
            {
                return new { status = 502, message = exception.Message };
            }
        }
    }

    public interface ISocket
    {
        object emit(string eventName, string data);
    }
}
