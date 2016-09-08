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
    public class cronController : configurationTableController
    {

        [Route("~/1/cron/run/{id}")]
        [HttpGet]
        public IHttpActionResult run(int id)
        {
            try
            {
                if (!Map.Database.Crons.ContainsKey(id))
                {
                    return ResponseMessage(Request.CreateResponse(HttpStatusCode.NotFound, "The Cron " + id + " does not exist"));
                }
                Cron cron = Map.Database.Crons[id];
                

                CronRequestInfo cronInfo = GetRequestInfo(cron);
                

                XMLHttpRequest request = new XMLHttpRequest();
                request.open(cronInfo.method, cronInfo.url, false);
                string appName = Map.AppName;

                request.setRequestHeader("content-type", "application/json");
                if (cronInfo.headers != null)
                {
                    foreach (string key in cronInfo.headers.Keys)
                    {
                        try
                        {
                            request.setRequestHeader(key, cronInfo.headers[key].ToString());
                        }
                        catch { }
                    }
                }

                System.Web.Script.Serialization.JavaScriptSerializer jss = new System.Web.Script.Serialization.JavaScriptSerializer();
                string data = cronInfo.data;
                
                request.send(data);

                if (request.status != 200)
                {
                    Maps.Instance.DuradosMap.Logger.Log("cron", "run", "failed", new Exception(request.responseText), 1, "status: " + request.status);
                    return ResponseMessage(Request.CreateResponse(HttpStatusCode.InternalServerError, request.responseText));
                }

                    
                string response = request.responseText;
                Maps.Instance.DuradosMap.Logger.Log("cron", "run", "success", null, 3, response.Length > 4000 ? response.Substring(0, 4000) : response);
                
                
                return Ok(response);
            }
            catch (ActionNotFoundException exception)
            {
                return ResponseMessage(Request.CreateResponse(HttpStatusCode.NotFound, exception.Message));
            }
            catch (QueryNotFoundException exception)
            {
                return ResponseMessage(Request.CreateResponse(HttpStatusCode.NotFound, exception.Message));
            }
            catch (Exception exception)
            {
                throw new BackAndApiUnexpectedResponseException(exception, this);
            }
        }

        private CronRequestInfo GetRequestInfo(Cron cron)
        {
            return CronHelper.GetRequestInfo(cron);
        }

        [Route("~/1/cron/run/{id}/async")]
        [HttpGet]
        public IHttpActionResult runAsync(int id)
        {
            try
            {
                string url = GetUrl();
                Dictionary<string, string> headers = GetHeaders();


                new Thread(() =>
                {
                    Thread.CurrentThread.IsBackground = true;
                    /* run your code here */
                    Durados.Web.Mvc.Infrastructure.Http.GetWebRequest(url, string.Empty, string.Empty, null, headers);
                }).Start();


                return Ok();
            }
            catch (Exception exception)
            {
                throw new BackAndApiUnexpectedResponseException(exception, this);
            }
        }

        private Dictionary<string, string> GetHeaders()
        {
            if (this.Request.Headers.Authorization == null)
                return null;
            return new Dictionary<string, string>() { { "Authorization", this.Request.Headers.Authorization.ToString() } };
        }
        private string GetUrl()
        {
            return this.Request.RequestUri.AbsoluteUri.Replace("/async", string.Empty);
        }

        protected override string GetConfigViewName()
        {
            return "Cron";
        }

        [Route("~/1/cron")]
        [HttpGet]
        public new IHttpActionResult Get(bool? withSelectOptions = null, int? pageNumber = null, int? pageSize = null, string filter = null, string sort = null, string search = null)
        {
            return GetList(withSelectOptions, pageNumber, pageSize, filter, sort, search);
        }

        protected override System.Web.Http.Results.OkNegotiatedContentResult<T> Ok<T>(T content)
        {
            const string DATA = "data";
            const string ID = "iD";
            const string STATE = "State";
            const string ENABLED = "ENABLED";
            const string ACTIVE = "active";

            IDictionary<string, object> response = (dynamic)content;
            Dictionary<string, Dictionary<string, object>> crons = null;


            if (response.Count == 2 && response.ContainsKey(DATA))
            {
                crons = CronHelper.getCron();
                foreach (Dictionary<string, object> item in (IEnumerable)response[DATA])
                {
                    bool active = false;
                    if (crons.ContainsKey(item[ID].ToString()))
                    {
                        active = ((Dictionary<string, object>)crons[item[ID].ToString()])[STATE].ToString() == ENABLED;
                    }
                    if (item.ContainsKey(ACTIVE))
                        item[ACTIVE] = active;
                    else
                        item.Add(ACTIVE, active);
                }
            }
            else if (response.ContainsKey(ID))
            {
                crons = CronHelper.getCron(new Cron() { ID = System.Convert.ToInt32(response[ID]) });
                bool active = false;
                if (crons.ContainsKey(response[ID].ToString()))
                {
                    active = ((Dictionary<string, object>)crons[response[ID].ToString()])[STATE].ToString() == ENABLED;
                }
                if (response.ContainsKey(ACTIVE))
                    response[ACTIVE] = active;
                else
                    response.Add(ACTIVE, active);
            }

            return base.Ok<T>(content);
        }

        [Route("~/1/cron/{id}")]
        [HttpGet]
        public new IHttpActionResult Get(string id)
        {
            return GetItem(id, true);
        }

        [Route("~/1/cron")]
        [HttpPost]
        [BackAnd.Web.Api.Controllers.Filters.ConfigBackupFilter]
        public override IHttpActionResult Post()
        {
            return base.Post();
        }

        const string NAME = "Name";
        const string DATABASE = "Crons_Parent";
        protected override IHttpActionResult ValidateInputForPost(Durados.Web.Mvc.View view, Dictionary<string, object> values)
        {
            if (values.ContainsKey(NAME))
            {
                string name = values[NAME].ToString();
                if (Map.Database.Crons.Values.Where(q => q.Name == name).FirstOrDefault() != null)
                {
                    return ResponseMessage(Request.CreateResponse(HttpStatusCode.Conflict, string.Format(Messages.CronWithNameAlreadyExists, name)));

                }
            }

            if (values.ContainsKey(DATABASE))
            {
                values[DATABASE] = 0;
            }
            else
            {
                values.Add(DATABASE, 0);
            }

            return null;
        }

        protected override IHttpActionResult ValidateInputForUpdate(string id, Durados.Web.Mvc.View view, Dictionary<string, object> values)
        {
            if (values.ContainsKey(NAME))
            {
                string name = values[NAME].ToString();
                if (Map.Database.Crons.Values.Where(q => q.Name == name && q.ID.ToString() != id).FirstOrDefault() != null)
                {
                    return ResponseMessage(Request.CreateResponse(HttpStatusCode.Conflict, string.Format(Messages.CronWithNameAlreadyExists, name)));

                }
            }

            if (values.ContainsKey(DATABASE))
            {
                values[DATABASE] = 0;
            }
            else
            {
                values.Add(DATABASE, 0);
            }

            return null;
        }

        protected override void AfterCreateAfterCommit(CreateEventArgs e)
        {
            base.AfterCreateAfterCommit(e);
            CreateOrUpdateAwsCron(e);
            
        }

        private void CreateOrUpdateAwsCron(DataActionEventArgs e)
        {
            Cron cron = null;
            int cronId = System.Convert.ToInt32(e.PrimaryKey);
            cron = Map.Database.Crons[cronId];
            if (e.Values.ContainsKey("active"))
            {
                if (e.Values["active"] is bool)
                    cron.Active = System.Convert.ToBoolean(e.Values["active"]);
            }
            CreateOrUpdateAwsCron(cron);
        }

        private void CreateOrUpdateAwsCron(Cron cron)
        {
            CronHelper.putCron(cron);
        }

        [Route("~/1/cron/{id}")]
        [HttpPut]
        [BackAnd.Web.Api.Controllers.Filters.ConfigBackupFilter]
        public override IHttpActionResult Put(string id)
        {
            return base.Put(id);
        }

        protected override void AfterEditAfterCommit(EditEventArgs e)
        {
            base.AfterEditAfterCommit(e);
            CreateOrUpdateAwsCron(e);

        }

        [Route("~/1/cron/{id}")]
        [HttpDelete]
        [BackAnd.Web.Api.Controllers.Filters.ConfigBackupFilter]
        public override IHttpActionResult Delete(string id)
        {
            return base.Delete(id);
        }

        protected override void BeforeDelete(DeleteEventArgs e)
        {
            Cron cron = null;
            int cronId = System.Convert.ToInt32(e.PrimaryKey);
            cron = Map.Database.Crons[cronId];
            DeleteAwsCron(cron);
            base.BeforeDelete(e);
        }

        private void DeleteAwsCron(Cron cron)
        {
            CronHelper.deleteCron(cron);
        }
    }
}
