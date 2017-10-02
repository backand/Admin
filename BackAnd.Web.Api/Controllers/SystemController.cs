
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;


namespace BackAnd.Web.Api.Controllers
{
    
    public class systemController : apiController
    {
        public IHttpActionResult Get()
        {
            try
            {
                string json = "{\"newSchema\":[],\"oldSchema\":[],\"severity\":0}";
                string node = "node is running";
                string nodeVersion = "Failed to get the node version";
                string instanceName = "unknown";

                bool hasError = false;
                var a = Task.Factory.StartNew(() =>
                {
                    try
                    {
                        Dictionary<string, object> transformResult = Transform(json, false);
                    }
                    catch (Exception exception)
                    {
                        hasError = true;
                        node = exception.InnerException == null ? exception.Message : exception.InnerException.Message;
                    }
                });

                var b = Task.Factory.StartNew(() =>
                {
                    try
                    {
                        nodeVersion = GetNodeVersion();
                    }
                    catch (Exception exception)
                    {
                        // In production, Server B don't have socket server, so that will fail.
                        //hasError = true;
                        nodeVersion += ": " + exception.Message;
                    }
                });

               
                var c = Task.Factory.StartNew(() =>
                {
                    try
                    {
                        instanceName = GetInstance();
                    }
                    catch (Exception exception)
                    {
                        hasError = true;
                        instanceName += ": " + exception.Message;
                    }
                });
                
                var tasks  = new Task[] {a ,b, c};

                // wait all tasks name
                Task.WaitAll(tasks);

                var response = new HealthCheckResponse 
                { 
                    version = Durados.Web.Mvc.Infrastructure.General.Version(), 
                    node = node, 
                    nodeVersion = nodeVersion, 
                    instance = instanceName,
                    time = DateTime.Now
                };

                if (hasError)
                {
                    return BadRequest(JsonConvert.SerializeObject(response));
                }

                return Ok(response);
      
            }
            catch (Exception exception)
            {
                throw new BackAndApiUnexpectedResponseException(exception, this);
            }
        }

        private string GetInstance()
        {
            return (System.Web.HttpContext.Current != null) ? System.Web.HttpContext.Current.Server.MachineName : System.Environment.MachineName;
        }

        private string GetNodeVersion()
        {
            string url = GetNodeUrl();
            var httpWebRequest = (HttpWebRequest)WebRequest.Create(url);
            httpWebRequest.Method = "GET";
            httpWebRequest.Timeout = 150;
            var response = httpWebRequest.GetResponse();
            Stream dataStream = response.GetResponseStream();
            // Open the stream using a StreamReader for easy access.
            StreamReader reader = new StreamReader(dataStream);
            // Read the content.
            string responseFromServer = reader.ReadToEnd();
            return responseFromServer;
        }

        private string GetSocketUrl()
        {
            string http = "http";
            if (HttpContext.Current != null && HttpContext.Current.Request != null && HttpContext.Current.Request.IsSecureConnection)
            {
                http = "https";
            }
            return System.Configuration.ConfigurationManager.AppSettings["socketUrl"] ?? http + "://localhost:4000";
 
        }
        
    }

    public class HealthCheckResponse
    {
        public string version { get; set; }

        public string node { get; set; }

        public string nodeVersion { get; set; }

        public string instance { get; set; }

        public DateTime time { get; set; }
    }

    
}
