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
using System.Threading;
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
    public class limitsController : apiController
    {

        

        [Route("~/1/admin/limits/{activity}")]
        [HttpGet]
        public IHttpActionResult Get(string activity)
        {
            try
            {
                Limits limits;

                if (!Enum.TryParse<Limits>(activity, true, out limits))
                {
                    return ResponseMessage(Request.CreateResponse(HttpStatusCode.NotFound, "activity not found"));
                }
                return Ok(new Dictionary<string, object>() { { "limit", Map.GetLimit(limits) }, { "count", GetCurrentCount(limits) } });
            }
            catch (Exception exception)
            {
                throw new BackAndApiUnexpectedResponseException(exception, this);
            }
        }

        private int? GetCurrentCount(Limits limit)
        {
            if (limit == Limits.Cron)
            {
                int count;

                Map.GetConfigDatabase().Views["Cron"].FillPage(1, 100000, null, null, null, out count, null, null);

                return count;
            }

            return null;
        }

        [Route("~/1/admin/limits")]
        [HttpGet]
        public IHttpActionResult Get()
        {
            try
            {
                Dictionary<string, object> result = new Dictionary<string, object>();

                foreach (Limits limits in Enum.GetValues(typeof(Limits)))
                {
                    result.Add(limits.ToString().LowerFirstChar(), new Dictionary<string, object>() { { "limit", Map.GetLimit(limits) }, { "count", GetCurrentCount(limits) } });
                }

                return Ok(result);
            }
            catch (Exception exception)
            {
                throw new BackAndApiUnexpectedResponseException(exception, this);
            }
        }

        [Route("~/1/admin/limits")]
        [HttpPut]
        public IHttpActionResult Put(string appName = null)
        {
            try
            {
                if (!Maps.IsDevUser())
                {
                    return ResponseMessage(Request.CreateResponse(HttpStatusCode.Unauthorized, Messages.ActionIsUnauthorized));
                }

                if (appName == null)
                    appName = Map.AppName;

                string json = Request.Content.ReadAsStringAsync().Result;

                if (string.IsNullOrEmpty(json))
                {
                    return ResponseMessage(Request.CreateResponse(HttpStatusCode.NotFound, Messages.MissingObjectToUpdate));
                }

                json = System.Web.HttpContext.Current.Server.UrlDecode(json.Replace("%22", "%2522").Replace("%2B", "%252B").Replace("+", "%2B"));

                Dictionary<string, object> values = Durados.Web.Mvc.UI.Json.JsonSerializer.Deserialize(json);

                int? id = Maps.Instance.AppExists(appName);

                if (!id.HasValue)
                {
                    return ResponseMessage(Request.CreateResponse(HttpStatusCode.NotFound, string.Format(Messages.AppNotFound, appName)));
                }

                foreach (string key in values.Keys)
                {
                    Limits limits;

                    if (!Enum.TryParse<Limits>(key, true, out limits))
                    {
                        return ResponseMessage(Request.CreateResponse(HttpStatusCode.NotFound, key + " limit not found"));
                    }

                    int limit = System.Convert.ToInt32(values[key]);

                    SqlAccess sa = Maps.MainAppSqlAccess;

                    string sql = Maps.MainAppSchema.GetInsertLimitsSql(limits, limit, id); 

                    sa.ExecuteNonQuery(Maps.Instance.DuradosMap.connectionString, sql);
                }

                Reload(appName);

                return Ok();
            }
            catch (Exception exception)
            {
                throw new BackAndApiUnexpectedResponseException(exception, this);
            }
            
        }

        private void Reload(string appName)
        {
            Maps.Instance.GetMap(appName).LoadLimits();
        }
    }
}
