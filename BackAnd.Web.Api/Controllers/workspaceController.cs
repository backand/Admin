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
    [RoutePrefix("1/workspace")]
    [BackAnd.Web.Api.Controllers.Filters.BackAndAuthorize]
    public class workspaceController : configurationTableController
    {
        [Route("{id}")]
        public  IHttpActionResult Get(string id)
        {
            return GetItem(id, false);


        }
        protected override string GetConfigViewName()
        {
            return "Workspace";
        }
        [Route("")]
        [HttpGet]
        public IHttpActionResult Get(bool? withSelectOptions = null, int? pageNumber = null, int? pageSize = null, string filter = null, string sort = null, string search = null)
        {
            string strFilter= "{fieldName:'Name', operator:'notEquals', value:'Admin'}";
            if (string.IsNullOrEmpty(filter))
                filter = "[" + strFilter + "]";
            else
                filter = filter.TrimEnd(']') + "," + strFilter + "]";
            return base.Get(withSelectOptions, pageNumber, pageSize, filter, sort, search);
        }
         [Route("{id}")]
        [BackAnd.Web.Api.Controllers.Filters.ConfigBackupFilter]
        public IHttpActionResult Put(string id)
        {
            return base.Put(id);
        }

         [Route("")]
         [BackAnd.Web.Api.Controllers.Filters.ConfigBackupFilter]
         public virtual IHttpActionResult Post()
         {
             return base.Post();
         }
         protected override IHttpActionResult ValidateInputForUpdate(string id, View view, Dictionary<string, object> values)
         {
             string workspaceName = values["Name"].ToString();

             if (string.IsNullOrEmpty(workspaceName))
             {
                 return ResponseMessage(Request.CreateResponse(HttpStatusCode.NotFound, string.Format(Messages.WorkspaceNameMissing)));
             }


             if (IsAdminWorkspace(id) && workspaceName != "Admin")
             {
                 return ResponseMessage(Request.CreateResponse(HttpStatusCode.Conflict, string.Format(Messages.ChangeAdminWorkspaceNameNotAllowed)));
             }

             if (view.GetDataRow(id) == null)
             {
                 return ResponseMessage(Request.CreateResponse(HttpStatusCode.NotFound, string.Format(Messages.RuleNotFound)));
             }

             if (Map.Database.Workspaces.Values.Where(w => w.Name == workspaceName && w.ID.ToString() != id).FirstOrDefault() != null)
             {
                 return ResponseMessage(Request.CreateResponse(HttpStatusCode.Conflict, string.Format(Messages.WorkspaceWithNameAlreadyExists, workspaceName)));

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

         private bool IsAdminWorkspace(string id)
         {
             return (id == "1");
         }
         protected override void HandelChildViewsForUpdate(string id, Dictionary<string, object> values)
         { }
         private bool IsWorkspaceAlreadyExists(View view, string workspaceName, bool update)
        {
            int count = 0;
            view.FillPage(1, 1000, new Dictionary<string, object>() { { "Name", workspaceName }, }, false,false, null, out count, null, null);

            int c = update ? 1 : 0;
           
            return count > c;
        }

         protected override bool IsDeep()
         {
             return false;
         }

        protected override IHttpActionResult ValidateInputForPost(View view, Dictionary<string, object> values)
        {
            string workspaceName = values["Name"].ToString();

            if(IsPassWorkspaceLimit(view))
                return ResponseMessage(Request.CreateResponse(HttpStatusCode.Conflict, Messages.WorkspaceLimit));
            if (IsWorkspaceAlreadyExists(view, workspaceName, false))
            {
                return ResponseMessage(Request.CreateResponse(HttpStatusCode.Conflict, string.Format(Messages.WorkspaceWithNameAlreadyExists, workspaceName)));
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

        const string NAME = "Name";
        const string DATABASE = "Workspaces_Parent";
        

        private bool IsPassWorkspaceLimit(View view)
        {
            return view.DataTable.Rows.Count > 20;
        }
        [Route("{id}")]
        [HttpDelete]
        [BackAnd.Web.Api.Controllers.Filters.ConfigBackupFilter]
        public virtual IHttpActionResult Delete(string id)
        {
            return ResponseMessage(Request.CreateResponse(HttpStatusCode.NotImplemented,Messages.NotImplemented));
        }

        
    }

}
