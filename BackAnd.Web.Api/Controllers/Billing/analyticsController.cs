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
using Durados.Web.Mvc.Analytics;
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
    public class analyticsController : apiController
    {
        [Route("~/1/analytics/{reportName}")]
        [HttpPost]
        [HttpGet]
        public IHttpActionResult Get(string reportName, string last = null, DateTime? earlierThan = null, DateTime? laterThan = null, string byThe = null)
        {
            try
            {
                ReportParameters reportParameters = null;
                if (Request.Method == HttpMethod.Post)
                {
                    string json = System.Web.HttpContext.Current.Server.UrlDecode(Request.Content.ReadAsStringAsync().Result.Replace("%22", "%2522").Replace("%2B", "%252B").Replace("+", "%2B"));

                    if (!string.IsNullOrEmpty(json))
                    {
                        reportParameters = (ReportParameters)Durados.Web.Mvc.UI.Json.JsonSerializer.Deserialize<ReportParameters>(json);
                    }
                    else
                    {
                        reportParameters = new ReportParameters();
                    }
                }
                else
                {
                    reportParameters = new ReportParameters() { last = last, byThe = byThe, earlierThan = earlierThan, laterThan = laterThan };
                }

                string appName = GetAppName();

                reportParameters.AppName = appName;
                reportParameters.ReportName = reportName;

                HttpResponseMessage message = reportParameters.Validate(Request);
                if (message != null)
                {
                    return ResponseMessage(message);
                }
                
                Report report = new Report();
                var result = report.Get(reportParameters);
         
                return Ok(result);
            }
            catch (Exception exception)
            {
                throw new BackAndApiUnexpectedResponseException(exception, this);
            }
        }

        [Route("~/1/analytics/embeddedReports")]
        [HttpPost]
        public IHttpActionResult embeddedReports()
        {
            try
            {
                string json = System.Web.HttpContext.Current.Server.UrlDecode(Request.Content.ReadAsStringAsync().Result.Replace("%22", "%2522").Replace("%2B", "%252B").Replace("+", "%2B"));

                if (string.IsNullOrEmpty(json))
                {
                    return ResponseMessage(Request.CreateResponse(HttpStatusCode.NotFound, "Missing request payload, check http://docs.cooladata.com/docs/setup/apis/embedded-report/"));
                }

                EmbeddedReports embeddedReport = new EmbeddedReports();

                return Ok(embeddedReport.GetUrl(json));
            }
            catch (Exception exception)
            {
                throw new BackAndApiUnexpectedResponseException(exception, this);
            }
        }

          
    }
}
