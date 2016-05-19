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

namespace BackAnd.Web.Api.Controllers.Billing
{
    [RoutePrefix("1")]
    [BackAnd.Web.Api.Controllers.Filters.BackAndAuthorize("Admin,Developer")]
    public class paymentStatusController : apiController
    {

        [Route("~/1/billing/payment/status/{appName}")]
        [HttpPut]
        public IHttpActionResult Put(string appName)
        {
            try
            {

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

                string pk = id.Value.ToString();

                Durados.Web.Mvc.View appsView = (Durados.Web.Mvc.View)Maps.Instance.DuradosMap.Database.Views["durados_App"];

                string sql = string.Empty;
                if (values.ContainsKey("PaymentStatus"))
                {
                    string PaymentStatus = values["PaymentStatus"].ToString();
                    sql = "update [durados_App] set [PaymentStatus] = " + PaymentStatus + " where [durados_App].[Id] = " + pk + ";";
               
                }
                if (values.ContainsKey("PaymentLocked"))
                {
                    string PaymentLocked = values["PaymentLocked"].ToString();
                    sql += "update [durados_App] set [PaymentLocked] = " + PaymentLocked + " where [durados_App].[Id] = " + pk + ";";

                }
                
                SqlAccess sa = new SqlAccess();
                
                sa.ExecuteNonQuery(Maps.Instance.DuradosMap.Database.ConnectionString, sql);

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
            RestHelper.Refresh(appName);
            try
            {
                new Sync().Initiate(Maps.Instance.GetMap(appName));
                new Sync().Initiate(Maps.Instance.GetMap(appName));
            }
            catch { }
        }
    }
}
