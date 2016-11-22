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
using System.Reflection;
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
    public class ruleController : businessRuleController
    {

        //[BackAnd.Web.Api.Controllers.Filters.TimeoutFilter(30000)]
        [Route("table/rule/{viewName}")]
        [Route("table/action/{viewName}")]
        [Route("objects/action/{viewName}")]
        [Route("table/rule/{viewName}/{id}")]
        [Route("table/action/{viewName}/{id}")]
        [Route("objects/action/{viewName}/{id}")]
        [HttpDelete]
        [HttpPut]
        [HttpPost]
        [HttpGet]
        public virtual HttpResponseMessage Perform(string viewName, string id = null, string name = null, string actionName = null, string parameters = null)
         {
             try
             {
                 if (string.IsNullOrEmpty(viewName))
                 {
                     return Request.CreateResponse(HttpStatusCode.NotFound, Messages.ViewNameIsMissing);
                 }
                 View view = GetView(viewName);
                 if (view == null)
                 {
                     return Request.CreateResponse(HttpStatusCode.NotFound, string.Format(Messages.ViewNameNotFound, viewName));
                 }

                 if (!IsAllow(view))
                 {
                     return Request.CreateResponse(HttpStatusCode.Forbidden, Messages.ActionIsUnauthorized);
                 }

                
                 System.Data.DataRow row = null;

                 if (!string.IsNullOrEmpty(id))
                 {
                     row = view.GetDataRow(id);

                     if (row == null)
                     {
                         return Request.CreateResponse(HttpStatusCode.NotFound, string.Format(Messages.ItemWithIdNotFound, id, viewName));
                     }
                 }

                 IEnumerable<Durados.Rule> rules = view.GetRules().Where(r => r.ShouldTrigger(Durados.TriggerDataAction.OnDemand)).OrderBy(r => r.Name).ToList();
                 if (string.IsNullOrEmpty(name) && string.IsNullOrEmpty(actionName))
                 {
                     return Request.CreateResponse(HttpStatusCode.NotFound, "actionName is missing");
                 }
                 if (string.IsNullOrEmpty(name))
                     name = actionName;

                 if (rules.Where(r => r.Name.Equals(name)).Count() == 0)
                 {
                     return Request.CreateResponse(HttpStatusCode.NotFound, "Action not found, or is not on demand");
                 }


                 Dictionary<string, object> values = null;
                 values = new Dictionary<string, object>();
                 if (!string.IsNullOrEmpty(parameters))
                 {
                     string json = System.Web.HttpContext.Current.Server.UrlDecode(parameters);

                     try
                     {
                         Dictionary<string, object> rulesParameters = view.Deserialize(System.Web.HttpContext.Current.Server.UrlDecode(parameters));
                         foreach (string key in rulesParameters.Keys)
                         {
                             if (!values.ContainsKey(key))
                                 values.Add(key.AsToken(), rulesParameters[key]);
                         }
                     }
                     catch (Exception exception)
                     {
                         Map.Logger.Log(GetControllerName(), GetActionName(), "get parameters json", exception, 2, string.Empty);
                         return Request.CreateResponse(HttpStatusCode.ExpectationFailed, Messages.FailedToGetJsonFromParameters);
                     }
                 }

                 try
                 {
                     string jsonPost = Request.Content.ReadAsStringAsync().Result;
                     if (jsonPost != "")
                     {
                         Dictionary<string, object> jsonPostDict = view.Deserialize(jsonPost);
                         foreach (string key in jsonPostDict.Keys)
                         {
                             if (!values.ContainsKey(key))
                                 values.Add(key, jsonPostDict[key]);
                         }
                     }
                     if (!string.IsNullOrEmpty(jsonPost))
                     {
                         if (System.Web.HttpContext.Current.Items.Contains("body"))
                         {
                             System.Web.HttpContext.Current.Items["body"] = jsonPost;
                         }
                         else
                         {
                             System.Web.HttpContext.Current.Items.Add("body", jsonPost);
                         }
                     }
                 }
                 catch (Exception exception)
                 {
                     throw new Durados.DuradosException("POST body processing error", exception);
                 }
                 using (System.Data.IDbConnection connection = GetConnection(Map.Database.SqlProduct, Map.Database.ConnectionString ))
                 {
                     using (System.Data.IDbCommand command = GetCommand(Map.Database.SqlProduct))
                     {
                         command.Connection = connection;
                         int userID = -1;
                         try
                         {
                             userID = Convert.ToInt32(((Durados.Web.Mvc.Database)view.Database).GetUserID());
                         }
                         catch { }
                         if (wfe == null)
                             wfe = CreateWorkflowEngine();

                         System.Data.IDbCommand sysCommand = null;
                         System.Data.IDbTransaction sysTransaction = null; 
                         System.Data.IDbConnection sysConnection = null;
                         try
                         {
                             sysCommand = GetCommand(view.Database.SystemSqlProduct);
                             sysConnection = SqlAccess.GetNewConnection(view.Database.SystemSqlProduct, view.Database.SystemConnectionString);
                             sysConnection.Open();
                             sysTransaction = sysConnection.BeginTransaction(System.Data.IsolationLevel.ReadCommitted);
                             sysCommand.Connection = sysConnection;
                             sysCommand.Transaction = sysTransaction;
                         }
                         catch { }
                         wfe.PerformActions(this, view, Durados.TriggerDataAction.OnDemand, values, id, row, Map.Database.ConnectionString, userID, ((Durados.Web.Mvc.Database)view.Database).GetUserRole(), command, sysCommand, name);

                         try
                         {
                             sysTransaction.Commit();
                             sysConnection.Close();
                         }
                         catch { }
                     }
                 }
                 HttpResponseMessage response = null;

                 if (values.ContainsKey(Durados.Workflow.JavaScript.ReturnedValueKey))
                 {
                     var returnedValue = values[Durados.Workflow.JavaScript.ReturnedValueKey];
                     response = Request.CreateResponse(HttpStatusCode.OK, returnedValue);
                 }
                 else
                 {
                     response = Request.CreateResponse(HttpStatusCode.OK);
                 }

                 if (System.Web.HttpContext.Current.Items.Contains(GuidKey))
                 {
                     string actionHeaderGuidValue = System.Web.HttpContext.Current.Items[GuidKey].ToString();
                     response.Headers.Add(actionHeaderGuidName, actionHeaderGuidValue);
                 }

                 return response;
             }
             catch (Exception exception)
             {
                 Dictionary<string, string> responseHeaders = null;
                 try
                 {
                     if (System.Web.HttpContext.Current.Items.Contains(GuidKey))
                     {
                         responseHeaders = new Dictionary<string, string>();
                         string actionHeaderGuidValue = System.Web.HttpContext.Current.Items[GuidKey].ToString();
                         responseHeaders.Add(actionHeaderGuidName, actionHeaderGuidValue);
                     }
                 }
                 catch { }
                 throw new BackAndApiUnexpectedResponseException(exception, this, responseHeaders);

             }
         }


    }

}
