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
    [BackAnd.Web.Api.Controllers.Filters.BackAndAuthorize]
    public class queryDataController : apiController
    {

        protected virtual bool IsAllow(Durados.Query query)
        {
            return query.IsAllow();
        }

        [Route("queries/data/{name}")]
        [Route("query/data/{name}")]
        [HttpGet]
        [HttpPost]
        public IHttpActionResult Get(string name, int? pageNumber = null, int? pageSize = null, bool dataSeries = false, string parameters = null)
        {
            try
            {
                if (string.IsNullOrEmpty(name))
                {
                    return ResponseMessage(Request.CreateResponse(HttpStatusCode.NotFound, Messages.ViewNameIsMissing));
                }
                Durados.Query query = GetQuery(name);
                if (query == null)
                {
                    return ResponseMessage(Request.CreateResponse(HttpStatusCode.NotFound, string.Format(Messages.ViewNameNotFound, name)));
                }
                if (!IsAllow(query))
                {
                    return ResponseMessage(Request.CreateResponse(HttpStatusCode.Forbidden, Messages.ActionIsUnauthorized));
                }

                Dictionary<string, object> values = null;
                if (!string.IsNullOrEmpty(parameters))
                {
                    values = Durados.Web.Mvc.UI.Json.JsonSerializer.Deserialize((System.Web.HttpContext.Current.Server.UrlDecode(parameters)));
                }
                else if (Request.Method == HttpMethod.Post)
                {
                    string json = Request.Content.ReadAsStringAsync().Result;
                    if (!string.IsNullOrEmpty(json))
                    {
                        values = Durados.Web.Mvc.UI.Json.JsonSerializer.Deserialize((System.Web.HttpContext.Current.Server.UrlDecode(json)));
                        if (values.ContainsKey("parameters") && values["parameters"] is Dictionary<string, object>)
                        {
                            values = (Dictionary<string, object>)values["parameters"];
                        }
                    }
                }
                var data = query.Get(pageNumber ?? 0, pageSize ?? 1000, values, dataSeries);

                return Ok(data);
            }
            catch (Exception exception)
            {
                throw new BackAndApiUnexpectedResponseException(exception, this);

            }
        }


        private Durados.Query GetQuery(string name)
        {
            return Map.Database.Queries.Values.Where(q => q.Name == name).FirstOrDefault();
        }
        
        
    }

}
