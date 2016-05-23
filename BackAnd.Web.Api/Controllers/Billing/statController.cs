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
    public class statController : apiController
    {

        [Route("~/1/billing/stat")]
        [HttpPost]
        public IHttpActionResult Post()
        {
            try
            {
                string json = System.Web.HttpContext.Current.Server.UrlDecode(Request.Content.ReadAsStringAsync().Result.Replace("%22", "%2522").Replace("%2B", "%252B").Replace("+", "%2B"));

                if (string.IsNullOrEmpty(json))
                {
                    return ResponseMessage(Request.CreateResponse(HttpStatusCode.NotFound, Messages.MissingObjectToUpdate));
                }


                Dictionary<string, object> values = Durados.Web.Mvc.UI.Json.JsonSerializer.Deserialize(json);

                DateTime date = DateTime.Today;
                if (values.ContainsKey("date"))
                {
                    date = (DateTime)values["date"];
                }

                bool reloadCache = false;
                if (values.ContainsKey("reloadCache"))
                {
                    reloadCache = (bool)values["reloadCache"];
                }

                string[] apps = null;

                if (values.ContainsKey("apps"))
                {
                    try
                    {
                        apps = (string[])Array.ConvertAll<object, string>((object[])values["apps"], System.Convert.ToString);
                    }
                    catch (Exception exception)
                    {
                        return ResponseMessage(Request.CreateResponse(HttpStatusCode.ExpectationFailed, "Fail to get apps " + exception.Message));
                    }
                }

                MeasurementType? measurementType = null;

                string measurment = null;

                if (values.ContainsKey("measurment"))
                {
                    measurment = values["measurment"].ToString();
                    MeasurementType type;
                    if (Enum.TryParse<MeasurementType>(measurment, out type))
                    {
                        measurementType = type;
                    }
                    else
                    {
                        return ResponseMessage(Request.CreateResponse(HttpStatusCode.NotFound, string.Format("The measurement {0} was not found.", measurment)));
                    }
                }

                Producer producer = new Producer();

                return Ok(producer.Produce(date, measurementType, apps, reloadCache));
            }
            catch (Exception exception)
            {
                throw new BackAndApiUnexpectedResponseException(exception, this);

            }
        }


    }
}
