using System;
using System.Collections.Generic;

namespace Backand
{
    public class security
    {
        private string BaseUrl = System.Configuration.ConfigurationManager.AppSettings["nodeHost"] ?? "http://127.0.0.1:9000";
        public object compare(string password, string hashedPassword)
        {
            string url = BaseUrl + "/security/compare";
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
                return true;
            }
            else if (request.status == 401)
            {
                return false;
            }
            
            else
            {
                throw new Durados.DuradosException("Server return status " + request.status + ", " + request.responseText);
            }
        }
        public object hash(string password)
        {
            return hash(password, null);
        }
        public object hash(string password, string salt)
        {
            const string encrypted = "encrypted";

            string url = BaseUrl + "/security/hash";
            XMLHttpRequest request = new XMLHttpRequest();
            request.open("POST", url, false);
            Dictionary<string, object> data = new Dictionary<string, object>();

            data.Add("password", password);
            if (!string.IsNullOrEmpty(salt))
                data.Add("salt", salt);

            request.setRequestHeader("content-type", "application/json");

            System.Web.Script.Serialization.JavaScriptSerializer jss = new System.Web.Script.Serialization.JavaScriptSerializer();
            request.send(jss.Serialize(data));

            if (request.status == 200)
            {
                Dictionary<string, object> response = null;
                try
                {
                    response = jss.Deserialize<Dictionary<string, object>>(request.responseText);
                }
                catch (Exception exception)
                {
                    throw new Durados.DuradosException("Failed to deserialize hash response " + request.status + ", " + request.responseText, exception);
                }
                if (!response.ContainsKey(encrypted))
                {
                    throw new Durados.DuradosException("hash response not contain encrypted " + request.status + ", " + request.responseText);
                }
                return response[encrypted];
            }
            else
            {
                throw new Durados.DuradosException("Server return status " + request.status + ", " + request.responseText);
            }
        }
    }
}
