using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Http;
using System.Net;
using System.Net.Http;
using System.Collections.Specialized;
using System.Web.Script.Serialization;
using Durados.Web.Mvc.UI.Helpers;

using Durados.Web.Mvc;
using Durados.DataAccess;
using Durados.Web.Mvc.Controllers.Api;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Security;
using Durados.Web.Mvc.Infrastructure;
using MySql.Data.MySqlClient;
using Durados;
using System.Collections;
using System.Data;
using Backand;
/*
 HTTP Verb	|Entire Collection (e.g. /customers)	                                                        |Specific Item (e.g. /customers/{id})
-----------------------------------------------------------------------------------------------------------------------------------------------
GET	        |200 (OK), list data items. Use pagination, sorting and filtering to navigate big lists.	    |200 (OK), single data item. 404 (Not Found), if ID not found or invalid.
PUT	        |404 (Not Found), unless you want to update/replace every resource in the entire collection.	|200 (OK) or 204 (No Content). 404 (Not Found), if ID not found or invalid.
POST	    |201 (Created), 'Location' header with link to /customers/{id} containing new ID.	            |404 (Not Found).
DELETE	    |404 (Not Found), unless you want to delete the whole collection—not often desirable.	        |200 (OK). 404 (Not Found), if ID not found or invalid.
 
 */

namespace BackAnd.Web.Api.Controllers
{
    [RoutePrefix("1")]
    [BackAnd.Web.Api.Controllers.Filters.BackAndAuthorize("Admin,Developer")]
    public class hostingController : apiController
    {

        [Route("~/1/hosting/folder")]
        [Route("~/1/nodejs/folder")]
        [HttpGet]
        public IHttpActionResult smartListFolder(string path = null, string objectName = null, string actionName = null)
        {
            try
            {
                if (path == null)
                    path = string.Empty;

                string url = GetNodeUrl() + "/smartListFolder";
                XMLHttpRequest request = new XMLHttpRequest();
                request.open("POST", url, false);
                Dictionary<string, object> data = new Dictionary<string, object>();
                string appName = Map.AppName;

                string bucket = Maps.S3Bucket;
                string folder = appName;
                if (Request.GetRouteData().Route.RouteTemplate == "1/nodejs/folder")
                {
                    bucket = Maps.NodeJSBucket;
                    if (string.IsNullOrEmpty(path))
                        folder = appName + "/" + objectName + "/" + actionName;
                
                }

                data.Add("bucket", bucket);
                data.Add("folder", folder);
                data.Add("pathInFolder", path);


                request.setRequestHeader("content-type", "application/json");

                System.Web.Script.Serialization.JavaScriptSerializer jss = new System.Web.Script.Serialization.JavaScriptSerializer();
                request.send(jss.Serialize(data));

                if (request.status != 200)
                {
                    Maps.Instance.DuradosMap.Logger.Log("hosting", "folder", request.responseText, null, 1, "status: " + request.status);
                    return ResponseMessage(Request.CreateResponse(HttpStatusCode.InternalServerError, request.responseText));
                }

                Dictionary<string, object>[] response = null;
                try
                {
                    response = jss.Deserialize<Dictionary<string, object>[]>(request.responseText);
                }
                catch (Exception exception)
                {
                    Maps.Instance.DuradosMap.Logger.Log("hosting", "folder", exception.Source, exception, 1, request.responseText);
                    return ResponseMessage(Request.CreateResponse(HttpStatusCode.InternalServerError, "Could not parse upload response: " + request.responseText + ". With the following error: " +  exception.Message));
                }

                
                return Ok(response);
            }
            catch (Exception exception)
            {
                throw new BackAndApiUnexpectedResponseException(exception, this);

            }
        }

        
        [Route("~/1/hosting")]
        [HttpPost]
        public IHttpActionResult hosting()
        {
            try
            {
                string dir = Map.AppName;
                string bucket = Maps.S3Bucket;
                JavaScriptSerializer jss = new JavaScriptSerializer();
                var data = new Dictionary<string, object>() { { "bucket", bucket }, { "dir", dir } };
                string json = jss.Serialize(data);
                Dictionary<string, object> credentials = bucketCredentials(json);
                credentials.Add("Info", new Dictionary<string, object>() { { "Bucket", bucket }, { "Dir", dir } });
                return Ok(credentials);
            }
            catch (Exception exception)
            {
                return Ok(new { valid = "never", warnings = new string[1] { exception.Message } });
                
            }
        }

        [Route("~/1/syncInfo")]
        [HttpPost]
        public IHttpActionResult syncInfo()
        {
            try
            {
                string dir = Map.AppName;
                
                Dictionary<string, string> buckets = new Dictionary<string, string>() { { "hosting", Maps.S3Bucket }, { "nodejs", Maps.NodeJSBucket } };

                Dictionary<string, object> services = new Dictionary<string, object>();

                foreach (string key in buckets.Keys)
                {
                    string bucket = buckets[key];
                    Dictionary<string, object> credentials = GetInfo(dir, bucket);
                    Dictionary<string, object> adjustedCredentials = AdjustCredentials(credentials);

                    services.Add(key, adjustedCredentials);
                }

                return Ok(services);
            }
            catch (Exception exception)
            {
                return Ok(new { valid = "never", warnings = new string[1] { exception.Message } });

            }
        }

        private Dictionary<string, object> AdjustCredentials(Dictionary<string, object> dic)
        {
            Dictionary<string, object> lowerDic = LowerKeys(dic);
            ((Dictionary<string, object>)lowerDic["credentials"]).Remove("expiration");
            return lowerDic;
        }

        private Dictionary<string, object> LowerKeys(Dictionary<string, object> dic)
        {
            Dictionary<string, object> lowerDic = new Dictionary<string, object>();

            foreach (string key in dic.Keys)
            {
                string lower = key.Substring(0, 1).ToLower() + key.Substring(1);
                object value = dic[key];
                if (value is Dictionary<string, object>)
                {
                    lowerDic.Add(lower, LowerKeys((Dictionary<string, object>)value));
                }
                else
                {
                    lowerDic.Add(lower, value);
                }
            }

            return lowerDic;
        }

        private Dictionary<string, object> GetInfo(string dir, string bucket)
        {
            JavaScriptSerializer jss = new JavaScriptSerializer();
            var data = new Dictionary<string, object>() { { "bucket", bucket }, { "dir", dir } };
            string json = jss.Serialize(data);
            Dictionary<string, object> credentials = bucketCredentials(json);
            credentials.Add("Info", new Dictionary<string, object>() { { "Bucket", bucket }, { "Dir", dir } });
            return credentials;
        }


        protected virtual Dictionary<string, object> bucketCredentials(string json)
        {
            string getNodeUrl = GetNodeUrl() + "/bucketCredentials";

            bulk bulk = new Durados.Web.Mvc.UI.Helpers.bulk();

            JavaScriptSerializer jss = new JavaScriptSerializer();

            var tasks = new List<Task<string>>();
            object responses = null;
            tasks.Add(Task.Factory.StartNew(() =>
            {
                //, { "Authorization", Request.Headers.Authorization.ToString() }
                var responseStatusAndData = bulk.GetWebResponse("POST", getNodeUrl, json, null, new Dictionary<string, object>() { { "Content-Type", "application/json" }, { "Authorization", Request.Headers.Authorization.ToString() } }, 0);
                responses = responseStatusAndData.data;
                if (string.IsNullOrEmpty(responseStatusAndData.data))
                {
                    if (responseStatusAndData.GetHeaders()["error"] != null && !string.IsNullOrEmpty(responseStatusAndData.GetHeaders()["error"].ToString()))
                    {
                        throw new DuradosException(responseStatusAndData.GetHeaders()["error"].ToString());
                    }
                }
                return responseStatusAndData.data;
            }));

            Task.WaitAll(tasks.ToArray());

            Dictionary<string, object> result = null;
            try
            {
                result = jss.Deserialize<Dictionary<string, object>>(responses.ToString());
            }
            catch
            {
                throw new DuradosException(responses.ToString());
            }

            return result;
        }

        [Route("~/1/template/{service}")]
        [HttpGet]
        public IHttpActionResult template(string service, string template = "template")
        {
            try
            {
                Dictionary<string, object> response = new Dictionary<string, object>();
                switch (service)
                {
                    case "nodejs":
                        response.Add("url", "http://s3.amazonaws.com/templates.backand.net/action/nodejs/1.0/" + template + ".zip");
                        break;

                    default:
                        throw new DuradosException("There is no tempalte for this service " + service);
                }
                return Ok(response);
            }
            catch (Exception exception)
            {
                return Ok(new { valid = "never", warnings = new string[1] { exception.Message } });

            }
        }
    }
}
