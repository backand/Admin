﻿using System;
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
using Durados.Web.Mvc.UI.Helpers.CallStack;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
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
    public class actionsCallStackController : apiController
    {
        [Route("~/1/actions/CallStack/{guid}")]
        [HttpGet]
        public IHttpActionResult Get(string guid, int limit = 1000)
        {
            JavaScriptSerializer jss = new JavaScriptSerializer();
            List<IActionEvent> events = new List<IActionEvent>();
            try
            {
                Durados.Web.Mvc.View view = GetView("durados_Log");

                int rowCount = -1;

                Dictionary<string, object>[] filterArray = new Dictionary<string, object>[2] { new Dictionary<string, object>() { { "fieldName", "Guid" }, { "operator", FilterOperandType.equals.ToString() }, { "value", guid } }, new Dictionary<string, object>() { { "fieldName", "LogType" }, { "operator", FilterOperandType.greaterThan.ToString() }, { "value", 501 } } };
                Dictionary<string, object>[] sortArray = new Dictionary<string, object>[1] { new Dictionary<string, object>() { { "fieldName", "Time" }, { "order", "asc" } } };

                var items = (Dictionary<string, object>)RestHelper.Get(view, false, false, 1, limit * 2 + 1, filterArray, null, sortArray, out rowCount, false, view_BeforeSelect, view_AfterSelect, false, false, false, false, null, true, false);

                if (rowCount > limit)
                {
                    return ResponseMessage(Request.CreateResponse(HttpStatusCode.PreconditionFailed, "Number of actions is more than " + limit));
                }

                var data = (Dictionary<string, object>[])items["data"];

                
                foreach (Dictionary<string, object> item in data)
                {
                    var actionEventData = jss.Deserialize<Dictionary<string, object>>(item["FreeText"].ToString());
                    IActionEvent actionEvent = new ActionEvent((int)actionEventData["time"], (Event)Enum.Parse(typeof(Event), actionEventData["event"].ToString(), true), actionEventData["objectName"].ToString(), actionEventData["actionName"].ToString(), actionEventData["id"].ToString(), actionEventData["data"]);
                    events.Add(actionEvent);
                }

                CallStackConverter callStackConverter = new CallStackConverter();

                var result = callStackConverter.ChronologicalListToTree(events.OrderBy(e => e.Time).ToList());
                if (result == null)
                    return Ok(new { });
                return Json(result, ViewHelpers.CamelCase);
            }
            catch (Exception exception)
            {
                string eventsJson = jss.Serialize(events);
                Maps.Instance.DuradosMap.Logger.Log("", "", "", exception, 1, eventsJson);
                throw new BackAndApiUnexpectedResponseException(exception, this);
            }
        }


        internal static class ViewHelpers
        {
            public static JsonSerializerSettings CamelCase
            {
                get
                {
                    return new JsonSerializerSettings
                    {
                        ContractResolver = new CamelCasePropertyNamesContractResolver()
                    };
                }
            }
        }
    }
}
