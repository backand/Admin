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
        [Route("~/1/billing/stat/async")]
        [HttpPost]
        public IHttpActionResult PostAsync()
        {
            if (!Maps.IsDevUser())
            {
                return ResponseMessage(Request.CreateResponse(HttpStatusCode.Unauthorized, Messages.ActionIsUnauthorized));
            }
            try
            {
                string json = System.Web.HttpContext.Current.Server.UrlDecode(Request.Content.ReadAsStringAsync().Result.Replace("%22", "%2522").Replace("%2B", "%252B").Replace("+", "%2B"));

                if (string.IsNullOrEmpty(json))
                {
                    return ResponseMessage(Request.CreateResponse(HttpStatusCode.NotFound, Messages.MissingObjectToUpdate));
                }


                
                string url = GetUrl();
                Dictionary<string, string> headers = GetHeaders();


                new Thread(() =>
                {
                    Thread.CurrentThread.IsBackground = true;
                    /* run your code here */
                    Durados.Web.Mvc.Infrastructure.Http.PostWebRequest2(url, json, string.Empty, null, headers, 60 * 60 * 1000);
                }).Start();
                
                
                return Ok();
            }
            catch (Exception exception)
            {
                try
                {
                    string[] emails = GetEmails();
                    SendError(emails, exception);
                }
                catch { }
                throw new BackAndApiUnexpectedResponseException(exception, this);
            }
        }

        private Dictionary<string, string> GetHeaders()
        {
            return new Dictionary<string, string>() { { "Authorization", this.Request.Headers.Authorization.ToString() } };
        }

        private string GetUrl()
        {
            return this.Request.RequestUri.OriginalString.TrimEnd("/async".ToCharArray());
        }

        private string[] GetEmails()
        {
            return Maps.DevUsers;
        }

       
        [Route("~/1/billing/stat")]
        [HttpPost]
        public IHttpActionResult Post()
        {
            System.Web.HttpContext.Current.Items[Durados.Database.EnableDecryptionKey] = true;

            string[] emails = null;
            if (!Maps.IsDevUser())
            {
                return ResponseMessage(Request.CreateResponse(HttpStatusCode.Unauthorized, Messages.ActionIsUnauthorized));
            }
            try
            {
                string json = System.Web.HttpContext.Current.Server.UrlDecode(Request.Content.ReadAsStringAsync().Result.Replace("%22", "%2522").Replace("%2B", "%252B").Replace("+", "%2B"));

                if (string.IsNullOrEmpty(json))
                {
                    return ResponseMessage(Request.CreateResponse(HttpStatusCode.NotFound, Messages.MissingObjectToUpdate));
                }


                Dictionary<string, object> values = Durados.Web.Mvc.UI.Json.JsonSerializer.Deserialize(json);

                if (values.ContainsKey("emails"))
                {
                    try
                    {
                        emails = (string[])Array.ConvertAll<object, string>((object[])values["emails"], System.Convert.ToString);
                    }
                    catch (Exception exception)
                    {
                        return ResponseMessage(Request.CreateResponse(HttpStatusCode.ExpectationFailed, "Fail to get emails " + exception.Message));
                    }
                }

                DateTime date = DateTime.Today;
                if (values.ContainsKey("date"))
                {
                    try
                    {
                        string dateString = values["date"].ToString();

                        string[] dateParts = dateString.Split('-');

                        int year = System.Convert.ToInt32(dateParts[0]);
                        int month = System.Convert.ToInt32(dateParts[1]);
                        int day = System.Convert.ToInt32(dateParts[2]);

                        date = new DateTime(year, month, day);
                    }
                    catch (Exception exception)
                    {
                        return ResponseMessage(Request.CreateResponse(HttpStatusCode.ExpectationFailed, "Could not parse date: " + exception.Message));
                    }
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

                string sql = null;

                if (values.ContainsKey("sql"))
                {
                    sql = values["sql"].ToString();
                }

                if (apps == null && sql == null)
                {
                    return ResponseMessage(Request.CreateResponse(HttpStatusCode.ExpectationFailed, "This action input must contains either apps or sql, it contains neither."));
                }
                else if (apps != null && sql != null)
                {
                    return ResponseMessage(Request.CreateResponse(HttpStatusCode.ExpectationFailed, "This action input must contains either apps or sql, it contains both."));
                }

                List<MeasurementType> measurementTypes = new List<MeasurementType>();

                string[] measurements = null;

                if (values.ContainsKey("measurements"))
                {
                    try
                    {
                        measurements = (string[])Array.ConvertAll<object, string>((object[])values["measurements"], System.Convert.ToString);
                    }
                    catch (Exception exception)
                    {
                        return ResponseMessage(Request.CreateResponse(HttpStatusCode.ExpectationFailed, "Fail to get measurements " + exception.Message));
                    }

                    foreach (string measurement in measurements)
                    {
                        MeasurementType measurementType;
                        if (!Enum.TryParse<MeasurementType>(measurement, out measurementType))
                        {
                            return ResponseMessage(Request.CreateResponse(HttpStatusCode.NotFound, string.Format("The measurement {0} was not found.", measurement)));
                        }
                        measurementTypes.Add(measurementType);
                    }

                }

                int bulk = 100;
                if (values.ContainsKey("bulk"))
                {
                    bulk = System.Convert.ToInt32(values["bulk"]);
                }

                int sleep = 1000;
                if (values.ContainsKey("sleep"))
                {
                    sleep = System.Convert.ToInt32(values["sleep"]);
                }

                Producer producer = new Producer();

                DateTime started = DateTime.Now;

                var result = producer.Produce(date, measurementTypes.ToArray(), apps, sql, reloadCache, bulk, sleep);

                TimeSpan took = DateTime.Now.Subtract(started);

                string duration = "date: " + date.ToShortDateString() + "; took: " + took.ToString();

                if (emails != null && emails.Length > 0)
                {
                    var errors = (Dictionary<string, Dictionary<string, object>>)((Dictionary<string, object>)result)["errors"];
                    var successes = (Dictionary<string, Dictionary<string, object>>)((Dictionary<string, object>)result)["apps"];
                    var warnings = (Dictionary<string, Dictionary<string, object>>)((Dictionary<string, object>)result)["warnings"];
                    
                    if (errors.Count == 0)
                    {
                        Send(emails, "Billing Stat, " + duration + ", " + successes.Count + " Apps Without Errors", "");
                    }
                    else
                    {
                        Send(emails, "Billing Stat, " + duration + ", " + successes.Count + " Apps With " + errors.Count + " Errors", GetMessage(errors, warnings));
                    }
                }

                return Ok(result);
            }
            catch (Exception exception)
            {
                if (emails != null && emails.Length > 0)
                {
                    SendError(emails, exception);
                }
                throw new BackAndApiUnexpectedResponseException(exception, this);

            }
        }

        private string GetMessage(Dictionary<string, Dictionary<string, object>> errors, Dictionary<string, Dictionary<string, object>> warnings)
        {
            StringBuilder sb = new StringBuilder();
            if (errors.Count > 0)
            {
                sb.AppendLine(GetMessageSection(errors, "errors"));
                sb.AppendLine();
            }
            if (warnings.Count > 0)
            {
                sb.AppendLine(GetMessageSection(warnings, "warnings"));
            }
            return sb.ToString();
        }
        private string GetMessageSection(Dictionary<string, Dictionary<string, object>> sections, string sectionName)
        {
            StringBuilder sb = new StringBuilder();

            sb.AppendFormat("The following {0} occured:", sectionName);
            sb.AppendLine();
            foreach (string key in sections.Keys)
            {
                Dictionary<string, object> appIssues = (Dictionary<string, object>)sections[key];
                
                sb.Append(key);
                sb.AppendLine(": <br>");
                sb.AppendLine(GetAppIssues(appIssues) + "<br>");
            }

            return sb.ToString();
        }

        private string GetAppIssues(Dictionary<string, object> appIssues)
        {
            StringBuilder sb = new StringBuilder();

            foreach (string key in appIssues.Keys)
            {
                sb.Append("&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;");
                sb.Append(key);
                sb.Append(" - ");
                sb.Append(appIssues[key]);
                sb.AppendLine(",<br>");
            }


            return sb.ToString();
        }

        private void Send(string[] emails, string subject, string message)
        {
            string host = System.Convert.ToString(System.Configuration.ConfigurationManager.AppSettings["host"]);
            int port = System.Convert.ToInt32(System.Configuration.ConfigurationManager.AppSettings["port"]);
            string username = System.Convert.ToString(System.Configuration.ConfigurationManager.AppSettings["username"]);
            string password = System.Convert.ToString(System.Configuration.ConfigurationManager.AppSettings["password"]);
            string from = System.Convert.ToString(System.Configuration.ConfigurationManager.AppSettings["fromAlert"]);

            Durados.Cms.DataAccess.Email.Send(host, Maps.Instance.DuradosMap.Database.UseSmtpDefaultCredentials, port, username, password, false, emails, null, null, subject, message, from, null, null, false, null, Maps.Instance.DuradosMap.Database.Logger, true);
        }
        private void SendError(string[] emails, Exception exception)
        {
            Send(emails, "Billing Stat Error", exception.Message);
        }
    }
}
