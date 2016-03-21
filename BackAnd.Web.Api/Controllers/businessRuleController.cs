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
    [BackAnd.Web.Api.Controllers.Filters.BackAndAuthorize]
    public class businessRuleController : configurationTableController
    {
         [Route("businessrule/{id}")]
         [Route("action/config/{id}")]
        public IHttpActionResult Get(string id)
        {
            return GetItem(id,true);
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

            return GetList(withSelectOptions, pageNumber, pageSize, filter, sort, search);
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
            if (values.ContainsKey("Name"))
                ruleName = values["Name"].ToString();
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
                    return ResponseMessage(Request.CreateResponse(HttpStatusCode.Conflict, string.Format(Messages.RuleWithNameAlreadyExists, ruleName, viewName)));

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
            if (values.ContainsKey("Name"))
                ruleName = values["Name"].ToString();
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
                return ResponseMessage(Request.CreateResponse(HttpStatusCode.Conflict, string.Format(Messages.RuleWithNameAlreadyExists, ruleName, viewName)));
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

         

    }

}
