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
    public class filesController : apiController
    {

        [Route("~/1/files/folder")]
        [HttpGet]
        public IHttpActionResult smartListFolder(string path = null)
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

                data.Add("bucket", Maps.S3FilesBucket);
                data.Add("folder", appName);
                data.Add("pathInFolder", path);


                request.setRequestHeader("content-type", "application/json");

                System.Web.Script.Serialization.JavaScriptSerializer jss = new System.Web.Script.Serialization.JavaScriptSerializer();
                request.send(jss.Serialize(data));

                if (request.status != 200)
                {
                    Maps.Instance.DuradosMap.Logger.Log("files", "folder", request.responseText, null, 1, "status: " + request.status);
                    return ResponseMessage(Request.CreateResponse(HttpStatusCode.InternalServerError, request.responseText));
                }

                Dictionary<string, object>[] response = null;
                try
                {
                    response = jss.Deserialize<Dictionary<string, object>[]>(request.responseText);
                }
                catch (Exception exception)
                {
                    Maps.Instance.DuradosMap.Logger.Log("files", "folder", exception.Source, exception, 1, request.responseText);
                    return ResponseMessage(Request.CreateResponse(HttpStatusCode.InternalServerError, "Could not parse upload response: " + request.responseText + ". With the following error: " +  exception.Message));
                }

                
                return Ok(response);
            }
            catch (Exception exception)
            {
                throw new BackAndApiUnexpectedResponseException(exception, this);

            }
        }

        
        
    }
}
