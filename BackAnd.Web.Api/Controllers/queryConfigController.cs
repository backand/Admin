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
    [RoutePrefix("1/query/config")]
    [BackAnd.Web.Api.Controllers.Filters.BackAndAuthorize]
    public class queryConfigController : configurationTableController
    {
        [Route("{id}")]
        public new IHttpActionResult Get(string id)
        {
            return GetItem(id, false);


        }
        protected override string GetConfigViewName()
        {
            return "Query";
        }
        [Route("")]
        [HttpGet]
        public new IHttpActionResult Get(bool? withSelectOptions = null, int? pageNumber = null, int? pageSize = null, string filter = null, string sort = null, string search = null)
        {
            return base.Get(withSelectOptions, pageNumber, pageSize, filter, sort, search);
        }
         [Route("{id}")]
        public override IHttpActionResult Put(string id)
        {
            return base.Put(id);
        }

         [Route("")]
         public override IHttpActionResult Post()
         {
             return base.Post();
         }
        

        
         protected override bool IsDeep()
         {
             return false;
         }

        
        [Route("{id}")]
        [HttpDelete]
        public override IHttpActionResult Delete(string id)
        {
            return base.Delete(id);
        }

        const string NAME = "Name";
        const string DATABASE = "Queries_Parent"; 
        protected override IHttpActionResult ValidateInputForPost(View view, Dictionary<string, object> values)
        {
            if (values.ContainsKey(NAME))
            {
                string name = values[NAME].ToString();
                if (Map.Database.Queries.Values.Where(q => q.Name == name).FirstOrDefault() != null)
                {
                    return ResponseMessage(Request.CreateResponse(HttpStatusCode.Conflict, string.Format(Messages.QueryWithNameAlreadyExists, name)));
 
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

        protected override IHttpActionResult ValidateInputForUpdate(string id, View view, Dictionary<string, object> values)
        {
            if (values.ContainsKey(NAME))
            {
                string name = values[NAME].ToString();
                if (Map.Database.Queries.Values.Where(q => q.Name == name && q.ID.ToString() != id).FirstOrDefault() != null)
                {
                    return ResponseMessage(Request.CreateResponse(HttpStatusCode.Conflict, string.Format(Messages.QueryWithNameAlreadyExists, name)));

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
    }

}
