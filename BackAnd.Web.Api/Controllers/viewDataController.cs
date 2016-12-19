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
    [RoutePrefix("1/view/data")]
    [BackAnd.Web.Api.Controllers.Filters.BackAndAuthorize]
    public class viewDataController : wfController
    {
        #region data

        public virtual IHttpActionResult Get(string name, string id, string collection, bool? withSelectOptions = null, bool? withFilterOptions = null, int? pageNumber = null, int? pageSize = null, string filter = null, string sort = null, string search = null, bool? deep = null, bool descriptive = true, bool? relatedObjects = false, string exclude = null)
        {
            try
            {
                if (string.IsNullOrEmpty(name))
                {
                    return ResponseMessage(Request.CreateResponse(HttpStatusCode.NotFound, Messages.ViewNameIsMissing));
                }
                View view = GetView(name);
                if (view == null)
                {
                    return ResponseMessage(Request.CreateResponse(HttpStatusCode.NotFound, string.Format(Messages.ViewNameNotFound, name)));
                }
                if (!IsAllow(view))
                {
                    return ResponseMessage(Request.CreateResponse(HttpStatusCode.Forbidden, Messages.ViewIsUnauthorized));
                }

                if (string.IsNullOrEmpty(id))
                {
                    return ResponseMessage(Request.CreateResponse(HttpStatusCode.NotFound, Messages.IdIsMissing));
                }

                if (string.IsNullOrEmpty(collection))
                {
                    return ResponseMessage(Request.CreateResponse(HttpStatusCode.NotFound, Messages.CollectionIsMissing));
                }

                Durados.Field[] fileds = view.GetFieldsByJsonName(collection);
                if (fileds.Length == 0)
                {
                    return ResponseMessage(Request.CreateResponse(HttpStatusCode.NotFound, Messages.CollectionNotFound));
                }
                if (fileds.Length > 1)
                {
                    return ResponseMessage(Request.CreateResponse(HttpStatusCode.NotFound, Messages.DuplicateCollectionName));
                }

                ChildrenField childrenField = (ChildrenField)fileds[0];

                View childrenView = (View)childrenField.ChildrenView;
                if (!IsAllow(childrenView))
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

                Filter inFilter = null;

                object filterValue = id;

                if (IsFilter(id))
                {
                    Dictionary<string, object>[] routeFilterArray = null;
                    string routeFilter = System.Web.HttpContext.Current.Request.Params[id];
                    if (string.IsNullOrEmpty(routeFilter))
                    {
                        return ResponseMessage(Request.CreateResponse(HttpStatusCode.NotAcceptable, Messages.StringifyFilter));
                    }
                    if (routeFilter.StartsWith("{"))
                    {
                        routeFilter = "[" + routeFilter + "]";
                    }
                    try
                    {
                        routeFilterArray = JsonConverter.DeserializeArray(routeFilter);
                    }
                    catch (Exception exception)
                    {
                        Map.Logger.Log(GetControllerNameForLog(this.ControllerContext), "get", exception.Source, exception, 1, "Deserialize route filter " + filter + ", original: " + id);
                        return ResponseMessage(Request.CreateResponse(HttpStatusCode.NotAcceptable, Messages.StringifyFilter));
                    }
                    try
                    {
                        inFilter = GetFilter(view, routeFilterArray, collection);
                        filterValue = inFilter;
                    }
                    catch (Exception exception)
                    {
                        Map.Logger.Log(GetControllerNameForLog(this.ControllerContext), "get", exception.Source, exception, 1, "Get route filter " + filter + ", original: " + id);
                        return ResponseMessage(Request.CreateResponse(HttpStatusCode.NotAcceptable, Messages.StringifyFilter));
                    }
                }
                
                Dictionary<string, object> collectionFilter = new Dictionary<string, object>();
                string parentFieldName = childrenField.GetEquivalentParentField().JsonName;
                collectionFilter.Add("fieldName", parentFieldName);
                collectionFilter.Add("operator", "in");
                collectionFilter.Add("value", filterValue);
                
                if (filterArray == null)
                {
                    filterArray = new Dictionary<string, object>[1] { collectionFilter };
                }
                else
                {
                    List<Dictionary<string, object>> filterList = filterArray.ToList();
                    for (int i = 0; i < filterList.Count; i++)
                    {
                        if (filterList[i].ContainsKey("fieldName") && filterList[i]["fieldName"].Equals(collectionFilter["fieldName"]))
                        {
                            return ResponseMessage(Request.CreateResponse(HttpStatusCode.NotAcceptable, "Ambiguous filter. Please remove the filter item with the fieldName \"" + parentFieldName + "\"."));
                        }
                    }
                    filterList.Add(collectionFilter);
                    filterArray = filterList.ToArray();
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

                bool hideMetadata = exclude != null && exclude.ToLower().Contains("metadata");
                bool hideTotalRows = exclude != null && exclude.ToLower().Contains("totalRows");

                var items = RestHelper.Get(childrenView, withSelectOptions ?? false, withFilterOptions ?? false, pageNumber ?? 1, pageSize ?? childrenView.Database.DefaultPageSize, filterArray, search, sortArray, out rowCount, deep ?? false, view_BeforeSelect, view_AfterSelect, false, descriptive, false, relatedObjects ?? false, null, hideMetadata, hideTotalRows);
                
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

        private Filter GetFilter(View view, Dictionary<string, object>[] filter, string childrenFieldName)
        {
            return RestHelper.GetFilter(view, filter, childrenFieldName);
        }

        private bool IsFilter(string filter)
        {
            return (filter.StartsWith("filter"));
        }

        public virtual IHttpActionResult Get(string name, string id, bool? deep = null, int? level = null, string exclude = null, string fields = null)
        {
            try
            {
                if (string.IsNullOrEmpty(name))
                {
                    return ResponseMessage(Request.CreateResponse(HttpStatusCode.NotFound, Messages.ViewNameIsMissing));
                }
                View view = GetView(name);
                if (view == null)
                {
                    return ResponseMessage(Request.CreateResponse(HttpStatusCode.NotFound, string.Format(Messages.ViewNameNotFound, name)));
                }

                if (string.IsNullOrEmpty(id))
                {
                    return ResponseMessage(Request.CreateResponse(HttpStatusCode.NotFound, Messages.IdIsMissing));
                }
                
                if (!view.IsAllow())
                {
                    return ResponseMessage(Request.CreateResponse(HttpStatusCode.Forbidden, Messages.ViewIsUnauthorized));
                }

                if (level.HasValue && level <= 0)
                {
                    return ResponseMessage(Request.CreateResponse(HttpStatusCode.NotAcceptable, "The level must be more than 0."));
                }

                string[] fieldsArray = null;
                if (!string.IsNullOrEmpty(fields) && fields != "fields" && fields != "all" && fields != "false" && fields != "null" && fields != "undefined" && fields != "[]")
                {
                    if (!(fields.Contains('\'') || fields.Contains('\"')))
                    {
                        return ResponseMessage(Request.CreateResponse(HttpStatusCode.NotAcceptable, Messages.StringifyFields));
                    }
                    try
                    {
                        fieldsArray = JsonConverter.DeserializeStringArray(fields);
                    }
                    catch (Exception exception)
                    {
                        Map.Logger.Log(GetControllerNameForLog(this.ControllerContext), "get", exception.Source, exception, 1, "Deserialize fields " + fields + ", original: " + System.Web.HttpContext.Current.Request.Params["fields"]);
                        return ResponseMessage(Request.CreateResponse(HttpStatusCode.NotAcceptable, Messages.StringifyFields));
                    }
                }

                if (view.Database.DefaultLevelOfDept <= 0)
                    view.Database.DefaultLevelOfDept = 3;

                bool hideMetadata = exclude != null && exclude.ToLower().Contains("metadata");
                var item = RestHelper.Get(view, id, deep ?? false, view_BeforeSelect, view_AfterSelect, false, false, false, level ?? view.Database.DefaultLevelOfDept, hideMetadata, fieldsArray);
                
                if (item == null)
                {
                    return ResponseMessage(Request.CreateResponse(HttpStatusCode.NotFound, string.Format(Messages.ItemWithIdNotFound, id, name)));
                }

                return Ok(item);
                
                
            }
            catch (Exception exception)
            {
                throw new BackAndApiUnexpectedResponseException(exception, this);
                
            }
        }

        protected virtual bool IsAllow(View view)
        {
            if (view.SystemView && view.Database.Map.IsMainMap)
            {
                return false;
            }
            if (Map.Database.GetCurrentUsername() == Durados.Database.GuestUsername && (view.Database.IsConfig || view.SystemView))
                return false;
            return view.IsAllow();
        }

        [Route("export/{name}")]
        [HttpGet]
        public HttpResponseMessage export(string name, int? pageNumber = null, int? pageSize = null, string filter = null, string sort = null, string search = null, string fileType = "csv")
        {
            if (string.IsNullOrEmpty(name))
            {
                return new HttpResponseMessage(HttpStatusCode.NotFound);
            }
            View view = GetView(name);
            if (view == null)
            {
                return new HttpResponseMessage(HttpStatusCode.NotFound);
            }
            if (!IsAllow(view))
            {
                return new HttpResponseMessage(HttpStatusCode.Forbidden);
            }

            int rowCount = 0;

            Dictionary<string, object>[] filterArray = null;
            if (!string.IsNullOrEmpty(filter) && filter != "filter")
            {
                filterArray = JsonConverter.DeserializeArray(filter);
            }

            Dictionary<string, object>[] sortArray = null;
            if (!string.IsNullOrEmpty(sort))
            {
                sortArray = JsonConverter.DeserializeArray(sort);
            }

            if (search == "null" || search == "undefined")
                search = null;

            System.Data.DataView dataView = (System.Data.DataView)RestHelper.Get(view, false, false, pageNumber ?? 1, pageSize ?? 60000, filterArray, search, sortArray, out rowCount, false, view_BeforeSelect, view_AfterSelect, true);

            Durados.ExportFileType exportFileType = (Durados.ExportFileType)Enum.Parse(typeof(Durados.ExportFileType), fileType, true);

            HttpResponseMessage result = new HttpResponseMessage(HttpStatusCode.OK);

            switch (exportFileType)
            {
                case Durados.ExportFileType.Csv:
                    Durados.DataAccess.Csv csv = new Durados.DataAccess.Csv();
                    Durados.Web.Mvc.UI.TableViewer tableViewer = GetNewTableViewer();
                    tableViewer.DataView = dataView;

                    string content = csv.Export(dataView, view, SecurityHelper.GetCurrentUserRoles(), tableViewer, "ex");


                    result.Content = new StringContent(content);
                    //a text file is actually an octet-stream (pdf, etc)
                    //result.Content.Headers.Add("ContentEncoding", System.Text.Encoding.GetEncoding("windows-1255").ToString());

                    result.Content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/octet-stream");
                    //we used attachment to force download
                    result.Content.Headers.ContentDisposition = new System.Net.Http.Headers.ContentDispositionHeaderValue("attachment");
                    result.Content.Headers.ContentDisposition.FileName = view.GetFileDisplayName() + ".csv";
                    break;
                default:
                    return new HttpResponseMessage(HttpStatusCode.NotImplemented);
            }

            return result;
        }


        public virtual IHttpActionResult Get(string name, bool? withSelectOptions = null, bool? withFilterOptions = null, int? pageNumber = null, int? pageSize = null, string filter = null, string sort = null, string search = null, bool? deep = null, bool descriptive = true, bool? relatedObjects = false, string exclude = null, string fields = null)
        {
            
            try
            {
                if (string.IsNullOrEmpty(name))
                {
                    return ResponseMessage(Request.CreateResponse(HttpStatusCode.NotFound, Messages.ViewNameIsMissing));
                }
                View view = GetView(name);
                if (view == null)
                {
                    return ResponseMessage(Request.CreateResponse(HttpStatusCode.NotFound, string.Format(Messages.ViewNameNotFound, name)));
                }
                if (!IsAllow(view))
                {
                    return ResponseMessage(Request.CreateResponse(HttpStatusCode.Forbidden, Messages.ViewIsUnauthorized));
                }

                int rowCount = 0;

                Dictionary<string, object>[] filterArray = null;
                isNosqlFilter = IsNosqlFilter(filter);
                if (isNosqlFilter)
                {
                    where = GetWhere(view, filter);
                }

                else if (!string.IsNullOrEmpty(filter) && filter != "filter" && filter != "false" && filter != "null" && filter != "undefined" && filter != "[{}]")
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

                string[] fieldsArray = null;
                if (!string.IsNullOrEmpty(fields) && fields != "fields" && fields != "all" && fields != "false" && fields != "null" && fields != "undefined" && fields != "[]")
                {
                    if (!(fields.Contains('\'') || fields.Contains('\"')))
                    {
                        return ResponseMessage(Request.CreateResponse(HttpStatusCode.NotAcceptable, Messages.StringifyFields));
                    }
                    try
                    {
                        fieldsArray = JsonConverter.DeserializeStringArray(fields);
                    }
                    catch (Exception exception)
                    {
                        Map.Logger.Log(GetControllerNameForLog(this.ControllerContext), "get", exception.Source, exception, 1, "Deserialize fields " + fields + ", original: " + System.Web.HttpContext.Current.Request.Params["fields"]);
                        return ResponseMessage(Request.CreateResponse(HttpStatusCode.NotAcceptable, Messages.StringifyFields));
                    }
                }


                bool hideMetadata = exclude != null && exclude.ToLower().Contains("metadata");
                bool hideTotalRows = exclude != null && exclude.ToLower().Contains("totalrows");
                var items = RestHelper.Get(view, withSelectOptions ?? false, withFilterOptions ?? false, pageNumber ?? 1, pageSize ?? view.Database.DefaultPageSize, filterArray, search, sortArray, out rowCount, deep ?? false, view_BeforeSelect, view_AfterSelect, false, descriptive, false, relatedObjects ?? false, where, hideMetadata, hideTotalRows, fieldsArray);
                
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
            catch (System.Data.Common.DbException exception)
            {
                return ResponseMessage(Request.CreateResponse(HttpStatusCode.NotAcceptable, "Check the security Pre-defined Filter for the following sql errors: " + exception.Message));

            }
            catch (AggregateException exception)
            {
                if (exception.InnerException != null)
                    throw new BackAndApiUnexpectedResponseException(exception.InnerException, this);
                else
                    throw new BackAndApiUnexpectedResponseException(exception, this);
            }
            catch (Exception exception)
            {
                throw new BackAndApiUnexpectedResponseException(exception, this);

            }
        }

        string where = null;
        bool isNosqlFilter = false;

        protected override void SetPermanentFilter(Durados.Web.Mvc.View view, Durados.DataAccess.Filter filter)
        {
            if (where != null)
            {
                filter.WhereStatement = " where " + where;
                filter.IsNosqlFilter = isNosqlFilter;
            }
            base.SetPermanentFilter(view, filter); ;
            
        }

        private string GetWhere(View view, string json, bool useCache = false)
        {
            if (useCache)
            {
                try
                {
                    var cacheResponse = GetWhereFromCache(view, json);

                    if (cacheResponse != null)
                    {
                        ReplaceVariables(cacheResponse);
                        return cacheResponse["where"].ToString();
                    }
                }
                catch { }
            }

            JavaScriptSerializer jss = new JavaScriptSerializer();

            var data = jss.Deserialize<Dictionary<string, object>>(json);

            if (!(data.Count == 1 && data.ContainsKey("q")))
            {
                throw new Durados.DuradosException("filter must start with \"q\"");
            }

            if (!data.ContainsKey("object"))
            {
                var data2 = new Dictionary<string, object>();
                data2.Add("object", view.JsonName);
                data2.Add("q", data["q"]);
                data = data2;
            }


            if (!data.ContainsKey("json"))
            {
                var data2 = new Dictionary<string, object>();
                data2.Add("json", data);
                data = data2;
            }

            data.Add("isFilter", true);
            
            json = jss.Serialize(data);

            var response = transformJson(json, useCache);

            if (!response.ContainsKey("where"))
            {
                throw new Durados.DuradosException("Failed to transform nosql filter");
            }

            if (useCache)
            {
                SetWhereToCache(view, json, response);
            }

            if (useCache)
            {
                ReplaceVariables(response);
            }
            

            return response["where"].ToString();

        }

        private void SetWhereToCache(View view, string json, Dictionary<string, object> response)
        {
            NoSqlFilterCache.Set(Map.AppName, view, json, response);
        }

        
        private Dictionary<string, object> GetWhereFromCache(View view, string json)
        {
            return NoSqlFilterCache.Get(Map.AppName, view, json);
        }

        private bool IsNosqlFilter(string filter)
        {
            return !string.IsNullOrEmpty(filter) && filter.Contains("\"q\":");
        }

        [BackAnd.Web.Api.Controllers.Filters.ResponseHeaderFilter]
        public virtual IHttpActionResult Post(string name, bool? deep = null, bool? returnObject = null, string parameters = null, string exclude = null)
        {
            Dictionary<string, object>[] values = null;
            try
            {
                if (string.IsNullOrEmpty(name))
                {
                    return ResponseMessage(Request.CreateResponse(HttpStatusCode.NotFound, Messages.ViewNameIsMissing));
                }
                View view = GetView(name);
                if (view == null)
                {
                    return ResponseMessage(Request.CreateResponse(HttpStatusCode.NotFound, string.Format(Messages.ViewNameNotFound, name)));
                }
                if (!view.IsCreatable() && !view.IsDuplicatable())
                {
                    return ResponseMessage(Request.CreateResponse(HttpStatusCode.Forbidden, Messages.ActionIsUnauthorized));
                }

                string json = System.Web.HttpContext.Current.Server.UrlDecode(Request.Content.ReadAsStringAsync().Result.Replace("%22", "%2522").Replace("%2B", "%252B").Replace("+", "%2B"));

                values = GetParameters(parameters, view, json, deep ?? false);

                string pk = view.Create(values, deep ?? false, view_BeforeCreate, view_BeforeCreateInDatabase, view_AfterCreateBeforeCommit, view_AfterCreateAfterCommit);

                string[] pkArray = pk.Split(';');
                int pkArrayLength = pkArray.Length;

                bool hideMetadata = exclude != null && exclude.ToLower().Contains("metadata");
                
                if (returnObject.HasValue && returnObject.Value && pkArrayLength == 1)
                {
                    var item = RestHelper.Get(view, pk, deep ?? false, view_BeforeSelect, view_AfterSelect, false, false, false, 3, hideMetadata);
                    return Ok(item);
                }
                else if (returnObject.HasValue && returnObject.Value && pkArrayLength > 1 && pkArrayLength <= 100)
                {
                    List<Dictionary<string, object>> data = new List<Dictionary<string, object>>();
                    foreach (string key in pkArray)
                    {
                        var item = RestHelper.Get(view, key, deep ?? false, view_BeforeSelect, view_AfterSelect, false, false, false, 3, hideMetadata);
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
            catch (RowNotFoundException)
            {
                return ResponseMessage(Request.CreateResponse(HttpStatusCode.Forbidden, Messages.PostContradictsPredefinedFilter));
            }
            catch (Durados.Workflow.DoNotLogException exception)
            {
                return DoNotLogExceptionHandler(exception);
            }
            catch (Exception exception)
            {

                //if (System.Web.HttpContext.Current.Items.Contains(GuidKey))
                //{
                //    string actionHeaderGuidValue = System.Web.HttpContext.Current.Items[GuidKey].ToString();
                //    this.ActionContext.Response.Headers.Add(actionHeaderGuidName, actionHeaderGuidValue);
                //}

                //if (values[0].ContainsKey(Durados.Workflow.JavaScript.ReturnedValueKey))
                //{
                //    var returnedValue = values[0][Durados.Workflow.JavaScript.ReturnedValueKey];
                //    return Ok(returnedValue);
                //}
                //else
                //{
                    throw new BackAndApiUnexpectedResponseException(exception, this);
                //}

                
                
            }
        }

        private IHttpActionResult DoNotLogExceptionHandler(Durados.Workflow.DoNotLogException exception)
        {
            var response = Request.CreateResponse(HttpStatusCode.ExpectationFailed, exception.Message);

            if (System.Web.HttpContext.Current.Items.Contains(GuidKey))
            {
                string actionHeaderGuidValue = System.Web.HttpContext.Current.Items[GuidKey].ToString();
                if (!response.Headers.Contains(actionHeaderGuidName))
                    response.Headers.Add(actionHeaderGuidName, actionHeaderGuidValue);
            }

            if (exception.InnerException != null && exception.InnerException is TimeoutException)
            {
                map.Logger.Log(GetControllerNameForLog(ControllerContext), Request.Method.Method, exception.Source, exception, 1, null);

            }

            return ResponseMessage(response);
        }

        internal static Dictionary<string, object>[] GetParameters(string parameters, View view, string json, bool deep)
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
                    Dictionary<string, object> valuesWithParameters = null;
                    if (deep)
                    {
                        valuesWithParameters = GetParametersForUpdateDeep(parameters, view, values);
                    }
                    else
                    {
                        valuesWithParameters = GetParameters(parameters, view, values);
                    }
                        
                    list.Add(valuesWithParameters);
                }

                return list.ToArray();

            }
            else
            {
                Dictionary<string, object> values = view.Deserialize(json);
                Dictionary<string, object> valuesWithParameters = null;
                if (deep)
                {
                    valuesWithParameters = GetParametersForUpdateDeep(parameters, view, values);
                }
                else
                {
                    valuesWithParameters = GetParameters(parameters, view, values);
                }
                return new Dictionary<string, object>[1] { valuesWithParameters };
            }
        }

        internal static Dictionary<string, object> GetParametersForUpdateDeep(string parameters, View view, Dictionary<string, object> values)
        {
            if (!string.IsNullOrEmpty(parameters))
            {
               
                Dictionary<string, object> rulesParameters = view.Deserialize(System.Web.HttpContext.Current.Server.UrlDecode(parameters));
                GetParametersForUpdateDeep(rulesParameters, view, values);
                
               
            }

            return values;
            
        }

        internal static Dictionary<string, object> GetParametersForUpdateDeep(Dictionary<string, object> rulesParameters, View view, Dictionary<string, object> values)
        {
            foreach (string key in rulesParameters.Keys)
            {
                if (!values.ContainsKey(key.AsToken()))
                {
                    values.Add(key.AsToken(), rulesParameters[key]);
                }
            }

            foreach (Durados.Field field in view.Fields.Values)
            {
                if (field.FieldType == Durados.FieldType.Children && !field.IsCheckList())
                {

                    ChildrenField childrenField = (ChildrenField)field;
                    View childrenView = (View)childrenField.ChildrenView;
                    var children = (object[])values[field.JsonName];
                    foreach (Dictionary<string, object> child in children)
                    {
                        GetParametersForUpdateDeep(rulesParameters, childrenView, child);
                    }
                }
                            
            }

            return values;
        }

        internal static Dictionary<string, object> GetParameters(string parameters, View view, Dictionary<string, object> values)
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

        [BackAnd.Web.Api.Controllers.Filters.ResponseHeaderFilter]
        public virtual IHttpActionResult Put(string name, string id, bool? deep = null, bool? returnObject = null, string parameters = null, bool? overwrite = null, string exclude = null)
        {
            try
            {

                if (string.IsNullOrEmpty(id) || id.Equals("undefined"))
                {
                    return ResponseMessage(Request.CreateResponse(HttpStatusCode.NotFound, Messages.IdIsMissing));
                }

                if (string.IsNullOrEmpty(name))
                {
                    return ResponseMessage(Request.CreateResponse(HttpStatusCode.NotFound, Messages.ViewNameIsMissing));
                }
                View view = GetView(name);
                if (view == null)
                {
                    return ResponseMessage(Request.CreateResponse(HttpStatusCode.NotFound, string.Format(Messages.ViewNameNotFound, name)));
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


                Dictionary<string, object> values = null;
                if (deep ?? false)
                {
                    values = GetParametersForUpdateDeep(parameters, view, values2);
                }
                else
                {
                    values = GetParameters(parameters, view, values2);
                }

                if (values.Count == 0 || (values.Count == 1 && values.ContainsKey("__metadata")))
                {
                    return ResponseMessage(Request.CreateResponse(HttpStatusCode.NotFound, string.Format(Messages.ItemWithNoFieldsToUpdate, id, name)));
                }
                    

                int affected = view.Update(values, id, deep ?? false, view_BeforeEdit, view_BeforeEditInDatabase, view_AfterEditBeforeCommit, view_AfterEditAfterCommit, view_BeforeCreate, view_BeforeCreateInDatabase, view_AfterCreateBeforeCommit, view_AfterCreateAfterCommit, overwrite ?? false, view_BeforeDelete, view_AfterDeleteBeforeCommit, view_AfterDeleteAfterCommit);

                if (affected == 0)
                {
                    return ResponseMessage(Request.CreateResponse(HttpStatusCode.NotFound, string.Format(Messages.ItemWithIdNotFound, id, name)));
                }

                bool hideMetadata = exclude != null && exclude.ToLower().Contains("metadata");
                
                if (returnObject.HasValue && returnObject.Value)
                {
                    var item = RestHelper.Get(view, id, deep ?? false, view_BeforeSelect, view_AfterSelect, false, false, false, 3, hideMetadata);
                    return Ok(item);
                }

                return Ok();
            }
            catch (Durados.Workflow.DoNotLogException exception)
            {
                return DoNotLogExceptionHandler(exception);
            }
            catch (Exception exception)
            {
                throw new BackAndApiUnexpectedResponseException(exception, this);
                
            }
        }

        [BackAnd.Web.Api.Controllers.Filters.ResponseHeaderFilter]
        public virtual IHttpActionResult Delete(string name, string id, bool? deep = null, string parameters = null)
        {
            try
            {
                if (string.IsNullOrEmpty(id))
                {
                    return ResponseMessage(Request.CreateResponse(HttpStatusCode.NotFound, Messages.IdIsMissing));
                }

                if (string.IsNullOrEmpty(name))
                {
                    return ResponseMessage(Request.CreateResponse(HttpStatusCode.NotFound, Messages.ViewNameIsMissing));
                }
                View view = GetView(name);
                if (view == null)
                {
                    return ResponseMessage(Request.CreateResponse(HttpStatusCode.NotFound, string.Format(Messages.ViewNameNotFound, name)));
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

                int affected = view.Delete(id, deep ?? false, view_BeforeDelete, view_AfterDeleteBeforeCommit, view_AfterDeleteAfterCommit, values);

                if (affected == 0)
                {
                    return ResponseMessage(Request.CreateResponse(HttpStatusCode.NotFound, string.Format(Messages.ItemWithIdNotFound, id, name)));
 
                }

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
            catch (Durados.Workflow.DoNotLogException exception)
            {
                return DoNotLogExceptionHandler(exception);
            }
            
            catch (Exception exception)
            {
                throw new BackAndApiUnexpectedResponseException(exception, this);
                

            }
        }

        [System.Web.Http.HttpGet]
        public virtual IHttpActionResult autocomplete(string viewName, string fieldName, string term = null, int limit = 20)
        {
            if (string.IsNullOrEmpty(viewName))
            {
                return ResponseMessage(Request.CreateResponse(HttpStatusCode.NotFound, Messages.ViewNameIsMissing));
            }

            Durados.Web.Mvc.View view = GetView(viewName);
            if (view == null)
            {
                return ResponseMessage(Request.CreateResponse(HttpStatusCode.NotFound, string.Format(Messages.ViewNameNotFound, viewName)));
            }
                
            if (string.IsNullOrEmpty(fieldName))
            {
                return ResponseMessage(Request.CreateResponse(HttpStatusCode.NotFound, Messages.FieldNameIsMissing));
            }

            Durados.Field[] fields = view.GetFieldsByJsonName(fieldName);
            if (fields.Length == 0)
            {
                return ResponseMessage(Request.CreateResponse(HttpStatusCode.NotFound, string.Format(Messages.FieldNameNotFound, fieldName)));
            }

            Durados.Field field = fields[0];

            if (field.FieldType == Durados.FieldType.Column)
            {
                return ResponseMessage(Request.CreateResponse(HttpStatusCode.NotFound, string.Format(Messages.FieldShouldBeParent, fieldName)));
            }

            //if (field.DisplayFormat == Durados.DisplayFormat.DropDown)
            //{
            //    return ResponseMessage(Request.CreateResponse(HttpStatusCode.NotFound, string.Format(Messages.FieldShouldBeAutoComplete, fieldName)));
            //}

            return Ok(view.GetAutoCompleteValues(field.Name, limit, term, true));
        }

        [Route("1/view/data/selectOptions/{viewName}/{fieldName}")]
        [Route("1/table/data/selectOptions/{viewName}/{fieldName}")]
        [System.Web.Http.HttpGet]
        public virtual IHttpActionResult selectOptions(string viewName, string fieldName = null)
        {
            if (string.IsNullOrEmpty(viewName))
            {
                return ResponseMessage(Request.CreateResponse(HttpStatusCode.NotFound, Messages.ViewNameIsMissing));
            }

            Durados.Web.Mvc.View view = GetView(viewName);
            if (view == null)
            {
                return Ok(view.GetSelectOptions());
            }

            if (string.IsNullOrEmpty(fieldName))
            {
                return ResponseMessage(Request.CreateResponse(HttpStatusCode.NotFound, Messages.FieldNameIsMissing));
            }

            Durados.Field[] fields = view.GetFieldsByJsonName(fieldName);
            if (fields.Length == 0)
            {
                return ResponseMessage(Request.CreateResponse(HttpStatusCode.NotFound, string.Format(Messages.FieldNameNotFound, fieldName)));
            }

            Durados.Field field = fields[0];

            return Ok(field.GetSelectOptions());
        }

        [Route("~/1/view/template/data")]
        [HttpPost]
        public virtual IHttpActionResult TemplateData()
        {
            string json = System.Web.HttpContext.Current.Server.UrlDecode(Request.Content.ReadAsStringAsync().Result);

            if (string.IsNullOrEmpty(json))
            {
                return ResponseMessage(Request.CreateResponse(HttpStatusCode.NotFound, Messages.MissingObjectToUpdate));
            }
            try
            {
                Dictionary<string, object>[] data = JsonConverter.DeserializeArray(json);


                foreach (Dictionary<string, object> table in data)
                {
                    string name = table["name"].ToString();
                    View view = GetView(name);

                    List<Dictionary<string, object>> list = new List<Dictionary<string, object>>(((System.Collections.ArrayList)table["data"]).Count);
                    foreach (var instance in ((System.Collections.ArrayList)table["data"]))
                    {
                        list.Add((Dictionary<string, object>)instance);
                    }
                    
                  
                    view.Create(list.ToArray(), false, view_BeforeCreate, view_BeforeCreateInDatabase, view_AfterCreateBeforeCommit, view_AfterCreateAfterCommit);
                }
             
            }
            catch (Exception exception)
            {
                throw new BackAndApiUnexpectedResponseException(exception, this);
            }
            //string name, bool? deep = null, bool? returnObject = null, string parameters = null
            return Ok();
        }


        [Route("~/1/bulk")]
        [HttpPost]
        // [{method:"string", url:"string", data:"string", parameters:"string", headers:{"string":"string"}}]
        public virtual IHttpActionResult Bulk()
        {
            string json = System.Web.HttpContext.Current.Server.UrlDecode(Request.Content.ReadAsStringAsync().Result);

            Dictionary<string, object>[] requests = null;

            if (!string.IsNullOrEmpty(json) && json != "false" && json != "null" && json != "undefined" && json != "[{}]")
            {
                if (json.StartsWith("{"))
                {
                    json = "[" + json + "]";
                }
                try
                {
                    requests = JsonConverter.DeserializeArray(json);
                }
                catch (Exception exception)
                {
                    Map.Logger.Log("viewData", "bulk", exception.Source, exception, 1, "Deserialize requests " + json);
                    return ResponseMessage(Request.CreateResponse(HttpStatusCode.NotAcceptable, Messages.StringifyFilter));
                }
            }

            if (requests.Length > 1000)
            {
                Map.Logger.Log("viewData", "bulk", "", null, 2, "more than 1000 requests");
                return ResponseMessage(Request.CreateResponse(HttpStatusCode.NotAcceptable, "bulk is limited to 1000 requests at a time"));
            }
            //IEnumerable<string> headers = Request.Headers.GetValues("Authorization");
            //string authorization = headers.FirstOrDefault();
            //headers = Request.Headers.GetValues("AppName");
            //string appName = headers.FirstOrDefault();
            //var responses = new bulk().Run(requests, authorization, appName);
            var responses = new Bulk().Run(requests, Map, DataHandler);
            return Ok(responses);
        }

       


        #endregion data

    }

}
