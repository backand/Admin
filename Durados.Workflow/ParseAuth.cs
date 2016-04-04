using System;
using System.Collections.Generic;

namespace Backand
{
    public class ParseAuth
    {
        private string BaseUrl = System.Configuration.ConfigurationManager.AppSettings["nodeHost"] ?? "http://127.0.0.1:9000";
        public object compare(string password, string hashedPassword)
        {
            string url = BaseUrl + "/parseCheckPassword";
            XMLHttpRequest request = new XMLHttpRequest();
            request.open("POST", url, false);
            Dictionary<string, object> data = new Dictionary<string, object>();
            
            data.Add("password", password);
            data.Add("hashedPassword", hashedPassword);
            
            request.setRequestHeader("content-type", "application/json");

            System.Web.Script.Serialization.JavaScriptSerializer jss = new System.Web.Script.Serialization.JavaScriptSerializer();
            request.send(jss.Serialize(data));

            if (request.status == 200)
            {
                return new Dictionary<string, object>() { { "result", true } };
            }
            else if (request.status == 401)
            {
                return new Dictionary<string, object>() { { "result", false } };
            }
            
            else
            {
                throw new Durados.DuradosException("Server return status " + request.status + ", " + request.responseText);
            }
        }
    }
}
