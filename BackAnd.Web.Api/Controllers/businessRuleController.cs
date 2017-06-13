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
using Durados.Workflow;
using System.Data;

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
    [BackAnd.Web.Api.Controllers.Filters.BackAndAuthorize]
    public class businessRuleController : configurationTableController
    {
         [Route("businessrule/{id}")]
         [Route("action/config/{id}")]
        public IHttpActionResult Get(string id)
        {
            return GetItem(id,true);
        }

         [Route("action/config/{objectName}/{actionName}")]
         [HttpGet]
         public virtual IHttpActionResult Get(string objectName, string actionName)
         {
             if (!map.Database.Views.ContainsKey(objectName))
             {
                 return ResponseMessage(Request.CreateResponse(HttpStatusCode.NotFound, Messages.ViewNameNotFound));
             }

             View view = (View)map.Database.Views[objectName];

             Durados.Rule rule = view.GetRules().FirstOrDefault(r => r.Name == actionName);

             if (rule == null)
             {
                 return ResponseMessage(Request.CreateResponse(HttpStatusCode.NotFound, Messages.RuleNotFound));
             }

             string id = rule.ID.ToString();

             return GetItem(id, true);
         }

        protected override string GetConfigViewName()
        {
            return "Rule";
        }

        [Route("businessrule")]
        [Route("action/config")]
        [HttpGet]
        public IHttpActionResult Get(bool? withSelectOptions = null, int? pageNumber = null, int? pageSize = null, string filter = null, string sort = null, string search = null)
        {

            return GetList(withSelectOptions, pageNumber, pageSize, filter, sort, search, true);
        }

        protected override void AdjustItems(object items, Dictionary<string, object>[] filterArray, Dictionary<string, object>[] sortArray)
        {
            AdjustItems((Dictionary<string, object>)items, filterArray, sortArray);
        }

        protected virtual void AdjustItems(Dictionary<string, object> items, Dictionary<string, object>[] filterArray, Dictionary<string, object>[] sortArray)
        {
            if (!items.ContainsKey("data"))
                return;

            if (!(items["data"] is Dictionary<string,object>[]))
                return;

            Dictionary<string,object>[] rules = (Dictionary<string,object>[])items["data"];

            HashSet<string> categories = new HashSet<string>();

            foreach(Dictionary<string, object> rule in rules)
            {
                if (rule.ContainsKey("category"))
                {
                    string category = rule["category"].ToString();

                    if (!categories.Contains(category))
                    {
                        categories.Add(category);
                    }
                }
            }

            if (!IsAdmin())
            {
                RemoveNotAllowedRules(items);
            }

            Dictionary<string, object> relatedObjects = null;

            if (items.ContainsKey("relatedObjects"))
            {
                if (items["relatedObjects"] is Dictionary<string, object>)
                {
                    relatedObjects = ((Dictionary<string, object>)items["relatedObjects"]);
                }
                else
                {
                    throw new Durados.DuradosException("relatedObjects is not a Dictionary");
                }
            }
            else
            {
                relatedObjects = new Dictionary<string, object>();
                items.Add("relatedObjects", relatedObjects);
            }

            
            relatedObjects.Add("categories", categories.ToArray());
        }

        private void RemoveNotAllowedRules(Dictionary<string, object> items)
        {
            HashSet<string> allowedRuleNames = ((View)map.Database.Views["_root"]).GetAllAllowedRuleNames();
            Dictionary<string,object>[] currentRules = (Dictionary<string,object>[])items["data"];
            List<Dictionary<string,object>> allowedRules = new List<Dictionary<string,object>>();


            foreach (Dictionary<string, object> rule in currentRules)
            {
                string ruleName = rule["name"].ToString();

                if (allowedRuleNames.Contains(ruleName))
                {
                    allowedRules.Add(rule);
                }
            }

            items["data"] = allowedRules.ToArray();
        }

        protected override void AdjustSortItem(Dictionary<string, object>[] sortArray)
        {
            foreach (Dictionary<string, object> sortItem in sortArray)
            {
                if (sortItem.ContainsKey("fieldName") && sortItem["fieldName"].Equals("viewTable"))
                {
                    sortItem["fieldName"] = "Rules_Parent";
                }
            }
        }

        protected override void AdjustFilterItem(Dictionary<string, object>[] filterArray)
        {
            foreach (Dictionary<string, object> filterItem in filterArray)
            {
                if (filterItem.ContainsKey("fieldName") && filterItem["fieldName"].Equals("viewTable"))
                {
                    filterItem["fieldName"] = "Rules_Parent";
                }
            }
        }

        [Route("action/config/getActionId")]
        [HttpGet]
        public virtual IHttpActionResult GetActionId(string objectName, string actionName)
        {
            if (!IsAdmin())
            {
                return ResponseMessage(Request.CreateResponse(HttpStatusCode.Forbidden, Messages.ActionIsUnauthorized));
            }

           
            if (!map.Database.Views.ContainsKey(objectName))
            {
                return ResponseMessage(Request.CreateResponse(HttpStatusCode.NotFound, Messages.RuleNotFound));
            }

            View view = (View)map.Database.Views[objectName];

            Durados.Rule rule = view.GetRules().Where(r => r.Name == actionName).FirstOrDefault();

            if (rule == null)
            {
                return ResponseMessage(Request.CreateResponse(HttpStatusCode.NotFound, Messages.RuleNotFound));
            }

            return Ok(new { id = rule.ID });
        }

        [Route("action/config/GetSecurityActionId")]
        [HttpGet]
        public virtual IHttpActionResult GetSecurityActionId(string actionName)
        {
            if (!IsAdmin())
            {
                return ResponseMessage(Request.CreateResponse(HttpStatusCode.Forbidden, Messages.ActionIsUnauthorized));
            }


            View view = (View)map.Database.GetUserView();

            Durados.Rule rule = view.GetRules().Where(r => r.Name == actionName).FirstOrDefault();

            if (rule == null)
            {
                return ResponseMessage(Request.CreateResponse(HttpStatusCode.NotFound, Messages.RuleNotFound));
            }

            return Ok(new { id = rule.ID });
        }

        [Route("businessrule/{id}")]
        [Route("action/config/{id}")]
        [BackAnd.Web.Api.Controllers.Filters.ConfigBackupFilter]
        [HttpPut]
        public virtual IHttpActionResult Put(string id)
        {
            return base.Put(id);
        }

        [Route("action/config/{objectName}/{actionName}")]
        [BackAnd.Web.Api.Controllers.Filters.ConfigBackupFilter]
        [HttpPut]
        public virtual IHttpActionResult Put(string objectName, string actionName)
        {
            if (!map.Database.Views.ContainsKey(objectName))
            {
                return ResponseMessage(Request.CreateResponse(HttpStatusCode.NotFound, Messages.RuleNotFound));
            }

            View view = (View)map.Database.Views[objectName];

            Durados.Rule rule = view.GetRules().Where(r => r.Name == actionName).FirstOrDefault();

            if (rule == null)
            {
                return ResponseMessage(Request.CreateResponse(HttpStatusCode.NotFound, Messages.RuleNotFound));
            }

            string id = rule.ID.ToString();

            return base.Put(id);
        }

         protected virtual bool IsDeep()
         {
             return true;
         }

        protected virtual void HandelChildViewsForUpdate(string id, Dictionary<string, object> values)
        {
            string Parameters = "Parameters_Children";
            if (values.ContainsKey(Parameters))
            {
                UpdateParameters(id, (object[])values[Parameters]);
            }
        }

        protected override IHttpActionResult ValidateInputForUpdate(string id, View view, Dictionary<string, object> values)
        {
            string ruleName = null;
            if (values.ContainsKey(Name))
                ruleName = values[Name].ToString();
            string viewId = null;
            if (values.ContainsKey("Rules_Parent"))
                viewId = values["Rules_Parent"].ToString();
            string viewName = null;

            if (viewId != null)
            {
                viewName = new ConfigAccess().GetViewNameByPK(viewId, Map.GetConfigDatabase().ConnectionString);

                if (string.IsNullOrEmpty(viewName))
                {
                    return ResponseMessage(Request.CreateResponse(HttpStatusCode.NotFound, string.Format(Messages.ViewNameNotFound, viewId)));
                }
            }

            System.Data.DataRow ruleRow = view.GetDataRow(id);

            if (ruleRow == null)
            {
                return ResponseMessage(Request.CreateResponse(HttpStatusCode.NotFound, string.Format(Messages.RuleNotFound)));
            }

            if (ruleName != null)
            {
                string viewIdColumnName = "Rules";
                if (viewId == null)
                {
                    if (ruleRow.IsNull(viewIdColumnName))
                    {
                        return ResponseMessage(Request.CreateResponse(HttpStatusCode.NotFound, string.Format(Messages.ViewNameNotFound, viewId)));
                    }
                    viewId = ruleRow[viewIdColumnName].ToString();
                }
                if (viewName == null)
                {
                    viewName = new ConfigAccess().GetViewNameByPK(viewId, Map.GetConfigDatabase().ConnectionString);
                }
                if (Map.Database.Views[viewName].GetRules().Where(r => r.Name == ruleName && r.ID.ToString() != id).FirstOrDefault() != null)
                {
                    string message;
                    if (viewName == "_root")
                        message = string.Format(Messages.FunctionWithNameAlreadyExists, ruleName);
                    else
                        message = string.Format(Messages.ActionWithNameAlreadyExists, ruleName, viewName);
                    return ResponseMessage(Request.CreateResponse(HttpStatusCode.Conflict, message));

                }
                
            }

            return null;
        }

        private bool IsRuleWithNameAlreadyExists(View view, string name, string viewId, bool update)
        {
            int count = 0;
            view.FillPage(1, 1000, new Dictionary<string, object>() { { "Name", name }, { "Rules_Parent", viewId } }, false, null, out count, null, null);

            int c = update ? 1 : 0;
            return count > c;
        }




        [Route("businessrule")]
        [Route("action/config")]
        [BackAnd.Web.Api.Controllers.Filters.ConfigBackupFilter]
        public virtual IHttpActionResult Post()
        {
            return base.Post();
            
        }

        protected override Dictionary<string, object> GetAdjustedValues(Durados.Web.Mvc.View view, Dictionary<string, object> values)
        {
            const string viewTable = "viewTable";
            if (values.ContainsKey(viewTable))
            {
                string viewName = values[viewTable].ToString();
                if (Map.Database.Views.ContainsKey(viewName))
                {
                    values[viewTable] = Map.Database.Views[viewName].ID;
                }
            }

            return base.GetAdjustedValues(view, values);
        }

        private void HandelChiledViewsForPost(Dictionary<string, object> values, string pk)
        {

            string Parameters = "Parameters_Children";
            if (values.ContainsKey(Parameters))
            {
                UpdateParameters(pk, (object[])values[Parameters]);
            }
        }

        protected override IHttpActionResult ValidateInputForPost(View view, Dictionary<string, object> values)
        {
            string ruleName = null;
            if (values.ContainsKey(Name))
                ruleName = values[Name].ToString();
            string viewId = null;
            if (values.ContainsKey("Rules_Parent"))
                viewId = values["Rules_Parent"].ToString();
            string viewName = null;

            if (viewId != null)
            {
                viewName = new ConfigAccess().GetViewNameByPK(viewId, Map.GetConfigDatabase().ConnectionString);

                if (string.IsNullOrEmpty(viewName))
                {
                    return ResponseMessage(Request.CreateResponse(HttpStatusCode.NotFound, string.Format(Messages.ViewNameNotFound, viewId)));
                }
            }

            if (string.IsNullOrEmpty(ruleName))
            {
                return ResponseMessage(Request.CreateResponse(HttpStatusCode.NotFound, string.Format("Action name is missing", viewId)));
            }
            
            if (string.IsNullOrEmpty(viewName))
            {
                return ResponseMessage(Request.CreateResponse(HttpStatusCode.NotFound, string.Format(Messages.ViewNameNotFound, viewId)));
            }

            if (Map.Database.Views[viewName].GetRules().Where(r => r.Name == ruleName).FirstOrDefault() != null)
            {
                return ResponseMessage(Request.CreateResponse(HttpStatusCode.Conflict, string.Format(Messages.ActionWithNameAlreadyExists, ruleName, viewName)));
            }
            return null;
        }

        


        protected virtual void UpdateParameters(string ruleId, object[] parameters)
        {
            DeleteParameters(ruleId);
            CreateParameters(ruleId, parameters);
        }

        private void CreateParameters(string ruleId, object[] parameters)
        {
            View parameterView = GetView("Parameter");
            foreach (Dictionary<string, object> parameter in parameters)
            {
                Dictionary<string, object> adjustedParameter = GetAdjustedValues(parameterView, parameter);
                if (!adjustedParameter.ContainsKey("Parameters_Parent"))
                {
                    adjustedParameter.Add("Parameters_Parent", ruleId);
                }
                parameterView.Create(adjustedParameter, null, null, null, null, null);
            }
        }

        private void DeleteParameters(string ruleId)
        {
            View ruleView = GetView(GetConfigViewName());
            View parameterView = GetView("Parameter");

            System.Data.DataRow row = ruleView.GetDataRow(ruleId);

            foreach (System.Data.DataRow parameterRow in row.GetChildRows("Parameters"))
            {
                string parameterId = parameterRow["ID"].ToString();
                parameterView.Delete(parameterId, null, null, null);
            }

        }


        [Route("businessrule/{id}")]
        [Route("action/config/{id}")]
        [BackAnd.Web.Api.Controllers.Filters.ConfigBackupFilter]
        public virtual IHttpActionResult Delete(string id)
        {
            return base.Delete(id);
        }


        protected override void BeforeCreate(Durados.CreateEventArgs e)
        {
            HandleFriendlyName(e);

            if (IsNodeJSFunction(e))
            {
                CreateNodeJSFunction(e);
            }
            WriteToAnalytics(e);

            base.BeforeCreate(e);
        }

        const string friendlyNameFieldName = "friendlyName";
        const string FriendlyNameFieldName = "FriendlyName";
        private void HandleFriendlyName(Durados.CreateEventArgs e)
        {
            string friendlyName = GetFriendlyName(e.Values);

            if (e.View.Name == "Rule")
            {
                if (e.Values.ContainsKey(friendlyNameFieldName) && (e.Values[friendlyNameFieldName] == null || e.Values[friendlyNameFieldName].ToString().Equals(string.Empty)))
                {
                    e.Values[friendlyNameFieldName] = friendlyName;
                }
                else if (e.Values.ContainsKey(FriendlyNameFieldName) && (e.Values[FriendlyNameFieldName] == null || e.Values[FriendlyNameFieldName].ToString().Equals(string.Empty)))
                {
                    e.Values[FriendlyNameFieldName] = friendlyName;
                }
                else
                {
                    e.Values.Add(FriendlyNameFieldName, friendlyName);
                }
            }
            
        }
        const string nameFieldName = "name";
        const string NameFieldName = "Name";
        

        private string GetFriendlyName(Dictionary<string, object> values)
        {
            if (values.ContainsKey(nameFieldName) && values[nameFieldName] != null)
                return values[nameFieldName].ToString().GetDecamal();
            else if (values.ContainsKey(NameFieldName) && values[NameFieldName] != null)
                return values[NameFieldName].ToString().GetDecamal();
            else
                return string.Empty;
        }

        private void WriteToAnalytics(Durados.EditEventArgs e)
        {
            try
            {
                string username = GetUsername();

                Durados.WorkflowAction workflowAction = GetActionType(e);
                string name = GetActionName(e);

                string type = IsFunction(e) ? "function" : "action";

                switch (workflowAction)
                {
                    case Durados.WorkflowAction.NodeJS:
                        SendAnalyticsInfo(username, "LambdaDeployed", new Dictionary<string, object>() { { "rule", name }, { "type", type } });
                        break;

                    default:
                        break;
                }
            }
            catch { }

        }

        private void WriteToAnalytics(Durados.CreateEventArgs e)
        {
            try
            {
                string username = GetUsername();

                Durados.WorkflowAction workflowAction = GetActionType(e);
                string name = GetActionName(e);

                string type = IsFunction(e) ? "function" : "action";

                switch (workflowAction)
                {
                    case Durados.WorkflowAction.NodeJS:
                        SendAnalyticsInfo(username, "LambdaAdded", new Dictionary<string, object>() { { "rule", name }, { "type", type } }); 
                        break;

                    case Durados.WorkflowAction.Lambda:
                        SendAnalyticsInfo(username, "LambdaLinked", new Dictionary<string, object>() { { "rule", name }, { "type", type } });
                        break;

                    //case Durados.WorkflowAction.JavaScript:
                    //    if (IsFunction(e))
                    //    {
                    //        SendAnalyticsInfo(username, "AddedFunction", new Dictionary<string, object>() { { "rule", name } });
                    //    }
                    //    else if (IsIntegration(e))
                    //    {
                    //        SendAnalyticsInfo(username, "AddedIntegration", new Dictionary<string, object>() { { "rule", name } });
                    //    }
                    //    else
                    //    {
                    //        SendAnalyticsInfo(username, "AddedRule", new Dictionary<string, object>() { { "rule", name } });
                    //    }
                    //    break;


                    default:
                        break;
                }
            }
            catch { }
            
        }

        const string ActionType = "ActionType";

        private void SendAnalyticsInfo(string username,  string eventName, Dictionary<string, object> contextInfo)
        {
            string writeKey = Convert.ToString(System.Configuration.ConfigurationManager.AppSettings["SegmentWriteKey"] ?? "W21J7U7JO9IXajS1w292q1atMOGklmPi");
            if (Segment.Analytics.Client == null)
                Segment.Analytics.Initialize(writeKey, new Segment.Config().SetAsync(false));

            //Segment.Analytics.Client.Track(ruleType, ruleName);

            //Segment.Analytics
            var req = System.Web.HttpContext.Current.Request;
            
            var options = new Segment.Model.Options();
            var properties = new Segment.Model.Properties();

            Uri referrer = req.UrlReferrer;
            string userAgent = req.UserAgent;


            string ip = Durados.Web.Mvc.Logging.Logger.GetUserIP();
            
            const string utm_content = "utm_content";
            const string utm_campaign = "utm_campaign";
            const string utm_medium = "utm_medium";
            const string utm_source = "utm_source";
            const string utm_term = "utm_term";

            var campaign = new Segment.Model.Dict();


            if (referrer != null)
            {
                var query = referrer.ParseQueryString();
                if (query[utm_content] != null) campaign.Add("content", query[utm_content]);
                if (query[utm_campaign] != null) campaign.Add("name", query[utm_campaign]);
                if (query[utm_medium] != null) campaign.Add("medium", query[utm_medium]);
                if (query[utm_source] != null) campaign.Add("source", query[utm_source]);
                if (query[utm_term] != null) campaign.Add("keyword", query[utm_term]);

                properties.Add("query", referrer.Query);
                properties.Add("path", referrer.PathAndQuery);
                properties.Add("host", referrer.Host);
                properties.Add("url", referrer.ToString());
            }
            foreach (string key in contextInfo.Keys)
            {
                properties.Add(key, contextInfo[key]);

            }
            /* ++ any custom props (eg. title) */

            var context = new Segment.Model.Context();
            context.Add("campaign", campaign);
            context.Add("userAgent", userAgent);
            context.Add("ip", Durados.Web.Mvc.Logging.Logger.UserIPAddress);
            
            
            options.SetContext(context);

            Segment.Analytics.Client.Track(username, eventName, properties, options);

        }

        private bool IsIntegration(Durados.CreateEventArgs e)
        {
            return e.Values.ContainsKey(ActionType) && e.Values[ActionType] != null && e.Values[ActionType].Equals("Integration");
        }

        private bool IsFunction(Durados.DataActionEventArgs e)
        {
            return e.Values.ContainsKey(ActionType) && e.Values[ActionType] != null && e.Values[ActionType].Equals("Function");
        }

        private string GetActionName(Durados.DataActionEventArgs e)
        {
            return e.Values["Name"].ToString();
        }

        private Durados.WorkflowAction GetActionType(Durados.DataActionEventArgs e)
        {
            return (Durados.WorkflowAction)Enum.Parse(typeof(Durados.WorkflowAction), e.Values["WorkflowAction"].ToString());
        }

        protected override void BeforeDelete(Durados.DeleteEventArgs e)
        {
            if (IsNodeJSFunction(e))
            {
                DeleteNodeJSFunction(e);
            }
            base.BeforeDelete(e);
        }

        private void DeleteNodeJSFunction(Durados.DeleteEventArgs e)
        {
            NodeJS nodeJS = new NodeJS();

            string ruleId = e.PrimaryKey;
            DataRow ruleRow = Map.GetConfigDatabase().Views[Rule].GetDataRow(ruleId);
            string viewId = ruleRow[Rules].ToString();
            string viewName = null;

            if (viewId != null)
            {
                viewName = new ConfigAccess().GetViewNameByPK(viewId, Map.GetConfigDatabase().ConnectionString);

                if (string.IsNullOrEmpty(viewName))
                {
                    throw new Durados.DuradosException(string.Format(Messages.ViewNameNotFound, viewId));
                }

            }
            else
            {
                throw new Durados.DuradosException(string.Format(Messages.ViewNameNotFound, string.Empty));
            }

            string actionName = ruleRow[Name].ToString();
            string functionName = Map.AppName + "_" + viewName + "_" + actionName;
            string folder = Map.AppName + "/" + viewName + "/" + actionName;
            nodeJS.Delete(folder, functionName);
        }

        protected override void BeforeEdit(Durados.EditEventArgs e)
        {
            if (IsNodeJSFunction(e))
            {
                UpdateNodeJSFunction(e);
            }

            WriteToAnalytics(e);

            base.BeforeEdit(e);
        }

        const string FileName = "FileName";
        const string Rule = "Rule";
        const string Rules = "Rules";
        const string Name = "Name";
            
        private void UpdateNodeJSFunction(Durados.EditEventArgs e)
        {
            
            NodeJS nodeJS = new NodeJS();

            string fileName = null;
            if (e.Values.ContainsKey(FileName))
                fileName = e.Values[FileName].ToString();

            string ruleId = e.PrimaryKey;
            DataRow ruleRow = Map.GetConfigDatabase().Views[Rule].GetDataRow(ruleId);
            string viewId = ruleRow[Rules].ToString();
            string viewName = null;

            if (viewId != null)
            {
                viewName = new ConfigAccess().GetViewNameByPK(viewId, Map.GetConfigDatabase().ConnectionString);

                if (string.IsNullOrEmpty(viewName))
                {
                    throw new Durados.DuradosException(string.Format(Messages.ViewNameNotFound, viewId));
                }

            }
            else
            {
                throw new Durados.DuradosException(string.Format(Messages.ViewNameNotFound, string.Empty));
            }

            string actionName = ruleRow[Name].ToString();
            if (fileName == null)
            {
                fileName = actionName + ".zip";
            }
            string functionName = Map.AppName + "_" + viewName + "_" + actionName;
            string folder = Map.AppName + "/" + viewName + "/" + actionName;
            nodeJS.Update(Maps.NodeJSBucket, folder, fileName, functionName);
        }

        private void CreateNodeJSFunction(Durados.CreateEventArgs e)
        {
            
            NodeJS nodeJS = new NodeJS();

            string fileName = null;
            if (e.Values.ContainsKey(FileName))
                fileName = e.Values[FileName].ToString();

            string viewId = null;
            if (e.Values.ContainsKey("Rules_Parent"))
                viewId = e.Values["Rules_Parent"].ToString();
            string viewName = null;

            if (viewId != null)
            {
                viewName = new ConfigAccess().GetViewNameByPK(viewId, Map.GetConfigDatabase().ConnectionString);

                if (string.IsNullOrEmpty(viewName))
                {
                    throw new Durados.DuradosException(string.Format(Messages.ViewNameNotFound, viewId));
                }
              
            }

            string actionName = e.Values[Name].ToString();
            if (fileName == null)
            {
                fileName = actionName + ".zip";
            }
            string functionName = Map.AppName + "_" + viewName + "_" + actionName;
            string folder = Map.AppName + "/" + viewName + "/" + actionName;
            nodeJS.Create(Maps.NodeJSBucket, folder, fileName, functionName, "handler", "handler");
        }

        private bool IsNodeJSFunction(Durados.CreateEventArgs e)
        {
            return IsNodeJSFunction(e.Values["WorkflowAction"]);
        }

        private bool IsNodeJSFunction(Durados.DeleteEventArgs e)
        {
            DataRow ruleRow = Map.GetConfigDatabase().Views["Rule"].GetDataRow(e.PrimaryKey);

            return IsNodeJSFunction(ruleRow["WorkflowAction"]);
        }

        private bool IsNodeJSFunction(Durados.EditEventArgs e)
        {
            DataRow ruleRow = e.PrevRow;

            if (ruleRow == null)
            {
                ruleRow = Map.GetConfigDatabase().Views["Rule"].GetDataRow(e.PrimaryKey);
            }

            return IsNodeJSFunction(ruleRow["WorkflowAction"]);
        }


        private bool IsNodeJSFunction(object value)
        {
            return value != null && value.Equals(Durados.WorkflowAction.NodeJS.ToString());
        }

        [HttpGet]
        [HttpPost]
        public virtual IHttpActionResult Download(string functionName)
        {
            try
            {
                View view = GetView("_root");
                Durados.Rule rule = GetRule(view, functionName);

                if (rule == null)
                {
                    return ResponseMessage(Request.CreateResponse(HttpStatusCode.NotFound, Messages.RuleNotFound));
                }

                if (!(rule.WorkflowAction == Durados.WorkflowAction.Lambda || rule.WorkflowAction == Durados.WorkflowAction.NodeJS))
                {
                    return ResponseMessage(Request.CreateResponse(HttpStatusCode.BadRequest, "not a lambda function"));
                }

                NodeJS nodejs = new NodeJS();
                return Ok(nodejs.Download(view.GetRuleCredentials(rule), rule.LambdaName));
            }
            catch (Exception exception)
            {
                throw new BackAndApiUnexpectedResponseException(exception, this);

            }
        }

        private Durados.Rule GetRule(View view, string functionName)
        {
            return view.GetRules().Where(r => r.Name == functionName).FirstOrDefault();
        }

        
 
    }

}
