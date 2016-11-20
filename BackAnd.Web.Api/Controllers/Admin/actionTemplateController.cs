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
    [RoutePrefix("admin/actionTemplate")]
    [BackAnd.Web.Api.Controllers.Filters.BackAndAuthorize]
    public class actionTemplateController : apiController
    {
        string viewName = "backand_ActionTemplate";

        [Route("{id}")]
        [HttpGet]
        public virtual IHttpActionResult Get(string id)
        {
            try
            {
                Durados.Web.Mvc.View view = (Durados.Web.Mvc.View)Maps.Instance.DuradosMap.Database.Views[viewName];
                if (view == null)
                {
                    return ResponseMessage(Request.CreateResponse(HttpStatusCode.NotFound, string.Format(Messages.ViewNameNotFound, viewName)));
                }

                if (string.IsNullOrEmpty(id))
                {
                    return ResponseMessage(Request.CreateResponse(HttpStatusCode.NotFound, Messages.IdIsMissing));
                }

                if (!view.IsAllow())
                {
                    return ResponseMessage(Request.CreateResponse(HttpStatusCode.Forbidden, Messages.ViewIsUnauthorized));
                }

                var item = RestHelper.Get(view, id, false, view_BeforeSelect, view_AfterSelect, false, false, true);

                if (item == null)
                {
                    return ResponseMessage(Request.CreateResponse(HttpStatusCode.NotFound, string.Format(Messages.ItemWithIdNotFound, id, viewName)));
                }

                return Ok(item);


            }
            catch (Exception exception)
            {
                throw new BackAndApiUnexpectedResponseException(exception, this);

            }
        }

        protected virtual bool IsAllow(Durados.Web.Mvc.View view)
        {
            return view.IsAllow();
        }


        [Route("")]
        [HttpGet]
        public virtual IHttpActionResult Get(int? pageNumber = null, int? pageSize = null, string filter = null, string sort = null, string search = null, bool? deep = null, bool descriptive = true)
        {

            try
            {
                Durados.Web.Mvc.View view = (Durados.Web.Mvc.View)Maps.Instance.DuradosMap.Database.Views[viewName];
                if (view == null)
                {
                    return ResponseMessage(Request.CreateResponse(HttpStatusCode.NotFound, string.Format(Messages.ViewNameNotFound, viewName)));
                }
                if (!IsAllow(view))
                {
                    return ResponseMessage(Request.CreateResponse(HttpStatusCode.Forbidden, Messages.ViewIsUnauthorized));
                }

                int rowCount = 0;

                Dictionary<string, object>[] filterArray = null;

                if (!string.IsNullOrEmpty(filter) && filter != "filter" && filter != "false" && filter != "null" && filter != "undefined" && filter != "[{}]")
                {
                    if (filter == "(Collection)")
                    {
                        filter = "[" + System.Web.HttpContext.Current.Request.Params["filter"] + "]";
                    }
                    if (filter.StartsWith("{"))
                    {
                        filter = "[" + filter + "]";
                    }
                    try
                    {
                        filterArray = JsonConverter.DeserializeArray(filter);
                    }
                    catch (Exception exception)
                    {
                        Map.Logger.Log(GetControllerNameForLog(this.ControllerContext), "get", exception.Source, exception, 1, "Deserialize filter " + filter + ", original: " + System.Web.HttpContext.Current.Request.Params["filter"]);
                        return ResponseMessage(Request.CreateResponse(HttpStatusCode.NotAcceptable, Messages.StringifyFilter));
                    }
                }

                Dictionary<string, object>[] sortArray = null;

                if (!string.IsNullOrEmpty(sort) && sort != "sort" && sort != "false" && sort != "null" && sort != "undefined" && sort != "[{}]")
                {
                    if (sort == "(Collection)")
                    {
                        sort = "[" + System.Web.HttpContext.Current.Request.Params["sort"] + "]";
                    }
                    if (sort.StartsWith("{"))
                    {
                        sort = "[" + sort + "]";
                    }
                    try
                    {
                        sortArray = JsonConverter.DeserializeArray(sort);
                    }
                    catch (Exception exception)
                    {
                        Map.Logger.Log(GetControllerNameForLog(this.ControllerContext), "get", exception.Source, exception, 1, "Deserialize sort " + sort + ", original: " + System.Web.HttpContext.Current.Request.Params["sort"]);
                        return ResponseMessage(Request.CreateResponse(HttpStatusCode.NotAcceptable, Messages.StringifySort));
                    }
                }

                if (search == "null" || search == "undefined")
                    search = null;

                var items = RestHelper.Get(view, false, false, pageNumber ?? 1, pageSize ?? 200, filterArray, search, sortArray, out rowCount, deep ?? false, view_BeforeSelect, view_AfterSelect, false, descriptive, true);

                return Ok(items);

            }
            catch (FilterException exception)
            {
                return ResponseMessage(Request.CreateResponse(HttpStatusCode.NotAcceptable, exception.Message));
            }
            catch (SortException exception)
            {
                return ResponseMessage(Request.CreateResponse(HttpStatusCode.NotAcceptable, exception.Message));
            }
            catch (Exception exception)
            {
                throw new BackAndApiUnexpectedResponseException(exception, this);

            }
        }


        [Route("")]
        [HttpPost]
        [BackAnd.Web.Api.Controllers.Filters.ResponseHeaderFilter]
        public virtual IHttpActionResult Post(bool? returnObject = null, string parameters = null)
        {
            try
            {
                Durados.Web.Mvc.View view = (Durados.Web.Mvc.View)Maps.Instance.DuradosMap.Database.Views[viewName];
                if (view == null)
                {
                    return ResponseMessage(Request.CreateResponse(HttpStatusCode.NotFound, string.Format(Messages.ViewNameNotFound, viewName)));
                }
                if (!view.IsCreatable() && !view.IsDuplicatable())
                {
                    return ResponseMessage(Request.CreateResponse(HttpStatusCode.Forbidden, Messages.ActionIsUnauthorized));
                }

                string json = System.Web.HttpContext.Current.Server.UrlDecode(Request.Content.ReadAsStringAsync().Result.Replace("%22", "%2522").Replace("%2B", "%252B").Replace("+", "%2B"));

                Dictionary<string, object>[] values = GetParameters(parameters, view, json);

                string pk = view.Create(values, false, view_BeforeCreate, view_BeforeCreateInDatabase, view_AfterCreateBeforeCommit, view_AfterCreateAfterCommit, true);

                string[] pkArray = pk.Split(';');
                int pkArrayLength = pkArray.Length;

                if (returnObject.HasValue && returnObject.Value && pkArrayLength == 1)
                {
                    var item = RestHelper.Get(view, pk, false, view_BeforeSelect, view_AfterSelect);
                    return Ok(item);
                }
                else if (returnObject.HasValue && returnObject.Value && pkArrayLength > 1 && pkArrayLength <= 100)
                {
                    List<Dictionary<string, object>> data = new List<Dictionary<string, object>>();
                    foreach (string key in pkArray)
                    {
                        var item = RestHelper.Get(view, key, false, view_BeforeSelect, view_AfterSelect);
                        data.Add(item);
                    }

                    Dictionary<string, object> items = new Dictionary<string, object>();
                    items.Add("totalRows", pkArrayLength);
                    items.Add("data", data.ToArray());

                    return Ok(items);
                }

                object id = pk;
                if (pkArrayLength > 1)
                {
                    id = pkArray;
                }
                return Ok(new { __metadata = new { id = id } });
            }
            catch (Exception exception)
            {
                throw new BackAndApiUnexpectedResponseException(exception, this);

            }
        }

        private static Dictionary<string, object>[] GetParameters(string parameters, Durados.Web.Mvc.View view, string json)
        {
            if (string.IsNullOrEmpty(json))
            {
                return new Dictionary<string, object>[1] { new Dictionary<string, object>() };
            }
            if (json.StartsWith("["))
            {
                Dictionary<string, object>[] valuesArray = JsonConverter.DeserializeArray(json);

                List<Dictionary<string, object>> list = new List<Dictionary<string, object>>();

                foreach (var values in valuesArray)
                {
                    Dictionary<string, object> valuesWithParameters = GetParameters(parameters, view, values);
                    list.Add(valuesWithParameters);
                }

                return list.ToArray();

            }
            else
            {
                Dictionary<string, object> values = view.Deserialize(json);

                return new Dictionary<string, object>[1] { GetParameters(parameters, view, values) };
            }
        }

        private static Dictionary<string, object> GetParameters(string parameters, Durados.Web.Mvc.View view, Dictionary<string, object> values)
        {
            Dictionary<string, object> values2 = new Dictionary<string, object>();
            if (!string.IsNullOrEmpty(parameters))
            {
                Dictionary<string, object> rulesParameters = view.Deserialize(System.Web.HttpContext.Current.Server.UrlDecode(parameters));
                foreach (string key in rulesParameters.Keys)
                {
                    //if (values.ContainsKey(key))
                    //{
                    //    throw new Durados.DuradosException("The name of a parameter cannot be the same as a field.");
                    //}
                    values2.Add(key.AsToken(), rulesParameters[key]);
                }
            }
            foreach (string key in values.Keys)
            {
                values2.Add(key, values[key]);
            }
            return values2;
        }


        [Route("{id}")]
        [HttpPut]
        [BackAnd.Web.Api.Controllers.Filters.ResponseHeaderFilter]
        public virtual IHttpActionResult Put(string id, bool? returnObject = null, string parameters = null)
        {
            try
            {

                if (string.IsNullOrEmpty(id) || id.Equals("undefined"))
                {
                    return ResponseMessage(Request.CreateResponse(HttpStatusCode.NotFound, Messages.IdIsMissing));
                }

                Durados.Web.Mvc.View view = (Durados.Web.Mvc.View)Maps.Instance.DuradosMap.Database.Views[viewName];
                if (view == null)
                {
                    return ResponseMessage(Request.CreateResponse(HttpStatusCode.NotFound, string.Format(Messages.ViewNameNotFound, viewName)));
                }
                
                if (!view.IsEditable())
                {
                    return ResponseMessage(Request.CreateResponse(HttpStatusCode.Forbidden, Messages.ActionIsUnauthorized));
                }

                string json = System.Web.HttpContext.Current.Server.UrlDecode(Request.Content.ReadAsStringAsync().Result.Replace("%22", "%2522").Replace("%2B", "%252B").Replace("+", "%2B"));

                if (string.IsNullOrEmpty(json))
                {
                    return ResponseMessage(Request.CreateResponse(HttpStatusCode.NotFound, Messages.MissingObjectToUpdate));
                }

                Dictionary<string, object> values2 = view.Deserialize(json);

                Dictionary<string, object> values = GetParameters(parameters, view, values2);

                view.Update(values, id, false, view_BeforeEdit, view_BeforeEditInDatabase, view_AfterEditBeforeCommit, view_AfterEditAfterCommit, view_BeforeCreate, view_BeforeCreateInDatabase, view_AfterCreateBeforeCommit, view_AfterCreateAfterCommit, false, view_BeforeDelete, view_AfterDeleteBeforeCommit, view_AfterDeleteAfterCommit, true);

                if (returnObject.HasValue && returnObject.Value)
                {
                    var item = RestHelper.Get(view, id, false, view_BeforeSelect, view_AfterSelect);
                    return Ok(item);
                }

                return Ok();
            }
            catch (Exception exception)
            {
                throw new BackAndApiUnexpectedResponseException(exception, this);

            }
        }

        [Route("{id}")]
        [HttpDelete]
        [BackAnd.Web.Api.Controllers.Filters.ResponseHeaderFilter]
        public virtual IHttpActionResult Delete(string id, string parameters = null)
        {
            try
            {
                if (string.IsNullOrEmpty(id))
                {
                    return ResponseMessage(Request.CreateResponse(HttpStatusCode.NotFound, Messages.IdIsMissing));
                }

                Durados.Web.Mvc.View view = (Durados.Web.Mvc.View)Maps.Instance.DuradosMap.Database.Views[viewName];
                if (view == null)
                {
                    return ResponseMessage(Request.CreateResponse(HttpStatusCode.NotFound, string.Format(Messages.ViewNameNotFound, viewName)));
                }
                
                if (!view.IsDeletable())
                {
                    return ResponseMessage(Request.CreateResponse(HttpStatusCode.NotFound, Messages.ViewIsUnauthorized));
                }

                Dictionary<string, object> values = null;

                if (!string.IsNullOrEmpty(parameters))
                {
                    values = new Dictionary<string, object>();
                    Dictionary<string, object> rulesParameters = view.Deserialize(System.Web.HttpContext.Current.Server.UrlDecode(parameters));
                    foreach (string key in rulesParameters.Keys)
                    {
                        if (!values.ContainsKey(key))
                            values.Add(key.AsToken(), rulesParameters[key]);
                    }
                }

                view.Delete(id, false, view_BeforeDelete, view_AfterDeleteBeforeCommit, view_AfterDeleteAfterCommit, values, true);


                return Ok(new { __metadata = new { id = id } });
            }
            catch (RowNotFoundException exception)
            {
                return ResponseMessage(Request.CreateResponse(HttpStatusCode.NotFound, exception.Message));
            }
            catch (System.Data.Common.DbException exception)
            {
                string message = exception.Message;
                if (message.StartsWith("The DELETE statement conflicted with the REFERENCE constraint"))
                    message = Messages.ForeignKeyDeleteViolation;
                return ResponseMessage(Request.CreateResponse(HttpStatusCode.ExpectationFailed, message));
            }

            catch (Exception exception)
            {
                throw new BackAndApiUnexpectedResponseException(exception, this);


            }
        }
    }
}
