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
using Durados.Web.Mvc.Stat;
using System.IO;
using System.Threading;
using System.Text;
using Durados.Web.Mvc.Analytics;
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
    public class logController : apiController
    {
        [Route("~/1/log/{name}")]
        [HttpGet]
        public IHttpActionResult Get(string name, int? pageNumber = null, int? pageSize = null, string filter = null, string sort = null)
        {
            try
            {
                if (WriteToExternalLog())
                {
                    Cql cql = new Cql();
                    return Ok(cql.Get(name, pageNumber ?? 1, pageSize ?? 1000, filter, sort));
                }
                else
                {
                    return Redirect("/1/table/data/durados_Log?filter=filter&pageNumber=pageNumber&pageSize=pageSize&sort=sort");
                }
            }
            catch (Exception exception)
            {
                throw new BackAndApiUnexpectedResponseException(exception, this);
            }
        }

        private bool WriteToExternalLog()
        {
            return System.Convert.ToBoolean(System.Configuration.ConfigurationManager.AppSettings["writeToLogStash"] ?? "true");
        }

        private string BaseUrl = System.Configuration.ConfigurationManager.AppSettings["nodeHost"] ?? "http://127.0.0.1:9000";

        [Route("~/1/last2hoursLog")]
        [HttpGet]
        public IHttpActionResult Get(int pageNumber = 1, int pageSize = 100, int minutesAgo = 120)
        {
            try
            {
                if (pageNumber <= 0)
                    throw new Durados.DuradosException("pageNumber must be larger than 0");
                if (pageSize <= 0)
                    throw new Durados.DuradosException("pageSize must be larger than 0");
                if (minutesAgo <= 0)
                    throw new Durados.DuradosException("minutesAgo must be larger than 0");

                if (pageSize > 1000)
                    return ResponseMessage(Request.CreateResponse(HttpStatusCode.BadRequest, new { Messsage = "pageSize is limited to 1000" }));
                if (minutesAgo > 1200)
                    return ResponseMessage(Request.CreateResponse(HttpStatusCode.BadRequest, new { Messsage = "minutesAgo is limited to 1200" }));

                pageNumber = pageNumber - 1;

                Dictionary<string, object> data = new Dictionary<string, object>();

                data.Add("appName", Map.AppName);
                data.Add("fromTimeEpochTime", GetEpochTime(DateTime.Now.Subtract(new TimeSpan(0, minutesAgo, 0))));
                data.Add("toTimeEpochTime", GetEpochTime(DateTime.Now));
                data.Add("offset", pageNumber);
                data.Add("count", pageSize);

                string url = BaseUrl + "/lastHourExceptions";
                XMLHttpRequest request = new XMLHttpRequest();
                request.open("POST", url, false);
                request.setRequestHeader("content-type", "application/json");

                System.Web.Script.Serialization.JavaScriptSerializer jss = new System.Web.Script.Serialization.JavaScriptSerializer();
                request.send(jss.Serialize(data));

                Dictionary<string, object>[] response = null;
                if (request.status == 200)
                {
                    try
                    {
                        response = jss.Deserialize<Dictionary<string, object>[]>(request.responseText);
                    }
                    catch (Exception exception)
                    {
                        throw new Durados.DuradosException("Failed to deserialize response " + request.status + ", " + request.responseText, exception);
                    }
                }
                else
                {
                    throw new Durados.DuradosException("Status: " + request.status + ", " + request.responseText);
                }

                return Ok(response);
            }
            catch (Exception exception)
            {
                throw new BackAndApiUnexpectedResponseException(exception, this);
            }
        }

        private int GetEpochTime(DateTime dateTime)
        {
            TimeSpan t = dateTime - new DateTime(1970, 1, 1);
            return (int)t.TotalSeconds;
        }
    }
}
