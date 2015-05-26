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

        public virtual IHttpActionResult Get(string name, string id, bool? deep = null)
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

                var item = RestHelper.Get(view, id, deep ?? false, view_BeforeSelect, view_AfterSelect);
                
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


        public virtual IHttpActionResult Get(string name, bool? withSelectOptions = null, bool? withFilterOptions = null, int? pageNumber = null, int? pageSize = null, string filter = null, string sort = null, string search = null, bool? deep = null, bool descriptive = true)
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
                        Map.Logger.Log(GetControllerNameForLog(this.ControllerContext), this.ControllerContext.RouteData.Values["action"].ToString(), exception.Source, exception, 1, "Deserialize filter " + filter + ", original: " + System.Web.HttpContext.Current.Request.Params["filter"]);
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
                        Map.Logger.Log(GetControllerNameForLog(this.ControllerContext), this.ControllerContext.RouteData.Values["action"].ToString(), exception.Source, exception, 1, "Deserialize sort " + sort + ", original: " + System.Web.HttpContext.Current.Request.Params["sort"]);
                        return ResponseMessage(Request.CreateResponse(HttpStatusCode.NotAcceptable, Messages.StringifySort));
                    }
                }

                if (search == "null" || search == "undefined")
                    search = null;

                var items = RestHelper.Get(view, withSelectOptions ?? false, withFilterOptions ?? false, pageNumber ?? 1, pageSize ?? 20, filterArray, search, sortArray, out rowCount, deep ?? false, view_BeforeSelect, view_AfterSelect, false, descriptive);
                
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

        [BackAnd.Web.Api.Controllers.Filters.ResponseHeaderFilter]
        public virtual IHttpActionResult Post(string name, bool? deep = null, bool? returnObject = null, string parameters = null)
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
                if (!view.IsCreatable() && !view.IsDuplicatable())
                {
                    return ResponseMessage(Request.CreateResponse(HttpStatusCode.Forbidden, Messages.ActionIsUnauthorized));
                }

                string json = System.Web.HttpContext.Current.Server.UrlDecode(Request.Content.ReadAsStringAsync().Result);

                Dictionary<string, object>[] values = GetParameters(parameters, view, json);

                string pk = view.Create(values, deep ?? false, view_BeforeCreate, view_BeforeCreateInDatabase, view_AfterCreateBeforeCommit, view_AfterCreateAfterCommit);

                string[] pkArray = pk.Split(';');
                int pkArrayLength = pkArray.Length;

                if (returnObject.HasValue && returnObject.Value && pkArrayLength == 1)
                {
                    var item = RestHelper.Get(view, pk, deep ?? false, view_BeforeSelect, view_AfterSelect);
                    return Ok(item);
                }
                else if (returnObject.HasValue && returnObject.Value && pkArrayLength > 1 && pkArrayLength <= 100)
                {
                    List<Dictionary<string, object>> data = new List<Dictionary<string, object>>();
                    foreach (string key in pkArray)
                    {
                        var item = RestHelper.Get(view, key, deep ?? false, view_BeforeSelect, view_AfterSelect);
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

        private static Dictionary<string, object>[] GetParameters(string parameters, View view, string json)
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

        private static Dictionary<string, object> GetParameters(string parameters, View view, Dictionary<string, object> values)
        {
            Dictionary<string, object> values2 = new Dictionary<string, object>();
            if (!string.IsNullOrEmpty(parameters))
            {
                Dictionary<string, object> rulesParameters = view.Deserialize(System.Web.HttpContext.Current.Server.UrlDecode(parameters));
                foreach (string key in rulesParameters.Keys)
                {
                    if (values.ContainsKey(key))
                    {
                        throw new Durados.DuradosException("The name of a parameter cannot be the same as a field.");
                    }
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
        public virtual IHttpActionResult Put(string name, string id, bool? deep = null, bool? returnObject = null, string parameters = null, bool? overwrite = null)
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
                if (!view.IsEditable())
                {
                    return ResponseMessage(Request.CreateResponse(HttpStatusCode.Forbidden, Messages.ActionIsUnauthorized));
                }

                string json = System.Web.HttpContext.Current.Server.UrlDecode(Request.Content.ReadAsStringAsync().Result);

                if (string.IsNullOrEmpty(json))
                {
                    return ResponseMessage(Request.CreateResponse(HttpStatusCode.NotFound, Messages.MissingObjectToUpdate));
                }

                Dictionary<string, object> values2 = view.Deserialize(json);

                Dictionary<string, object> values = GetParameters(parameters, view, values2);

                view.Update(values, id, deep ?? false, view_BeforeEdit, view_BeforeEditInDatabase, view_AfterEditBeforeCommit, view_AfterEditAfterCommit, view_BeforeCreate, view_BeforeCreateInDatabase, view_AfterCreateBeforeCommit, view_AfterCreateAfterCommit, overwrite ?? false, view_BeforeDelete, view_AfterDeleteBeforeCommit, view_AfterDeleteAfterCommit);
                
                if (returnObject.HasValue && returnObject.Value)
                {
                    var item = RestHelper.Get(view, id, deep ?? false, view_BeforeSelect, view_AfterSelect);
                    return Ok(item);
                }

                return Ok();
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

                view.Delete(id, deep ?? false, view_BeforeDelete, view_AfterDeleteBeforeCommit, view_AfterDeleteAfterCommit, values);


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

            IEnumerable<string> headers = Request.Headers.GetValues("Authorization");
            string authorization = headers.FirstOrDefault();
            headers = Request.Headers.GetValues("AppName");
            string appName = headers.FirstOrDefault();
            var responses = new bulk().Run(requests, authorization, appName);

            return Ok(responses);
        }


        #endregion data


        
        /*
        #region workflow

        #region notifier

        public bool DontSend
        {
            get
            {
                return Convert.ToBoolean(System.Configuration.ConfigurationManager.AppSettings["DontSend"] ?? "false");
            }
        }

        public virtual string GetSiteWithoutQueryString()
        {
            return System.Web.HttpContext.Current.Request.Url.Scheme + "://" + System.Web.HttpContext.Current.Request.Url.Authority;
            //return Request.Url.Scheme + "://" + Request.Url.Authority;

        }

        public virtual string GetMainSiteWithoutQueryString()
        {
            //return System.Web.HttpContext.Current.Request.Url.Scheme + "://" + System.Web.HttpContext.Current.Request.Url.Authority;
            return Maps.Instance.DuradosMap.Url;

        }

        public string GetUrlAction(Durados.View view, string pk)
        {
            //return Url.Action(((View)view).IndexAction, ((View)view).Controller, new { viewName = view.Name, pk = pk });
            return string.Empty;

        }

        public virtual string SaveInMessageBoard(Dictionary<string, Durados.Parameter> parameters, Durados.View view, Dictionary<string, object> values, System.Data.DataRow prevRow, string pk, string siteWithoutQueryString, string urlAction, string subject, string message, int currentUserId, string currentUserRole, Dictionary<int, bool> recipients)
        {
            return SaveInMessageBoard(parameters, (View)view, values, prevRow, pk, siteWithoutQueryString, urlAction, subject, message, currentUserId, recipients);
        }

        public virtual void SaveMessageAction(View view, string pk, Durados.Web.Mvc.UI.Json.Field jsonField, Durados.Web.Mvc.Controllers.MessageBoardAction messageBoardAction)
        {
            SaveMessageAction(view, pk, ((ColumnField)view.Fields[messageBoardAction.ToString()]).ConvertFromString(jsonField.Value.ToString()), messageBoardAction.GetHashCode(), Convert.ToInt32(GetUserID()));
        }

        public virtual void SaveMessageAction(Durados.View view, string pk, object value, int messageBoardAction, int userId)
        {
            SqlAccess sqlAccess = new SqlAccess();
            Dictionary<string, object> parameters = new Dictionary<string, object>();
            parameters.Add("@UserId", userId);
            parameters.Add("@MessageId", Convert.ToInt32(pk));

            parameters.Add("@ActionId", messageBoardAction);
            parameters.Add("@ActionValue", value);
            sqlAccess.ExecuteProcedure(Map.Database.SysDbConnectionString, "Durados_MessageBoard_Action", parameters, null);

        }

        
        public virtual string SaveInMessageBoard(Dictionary<string, Durados.Parameter> parameters, View view, Dictionary<string, object> values, System.Data.DataRow prevRow, string pk, string siteWithoutQueryString, string urlAction, string subject, string message, int currentUserId, Dictionary<int, bool> recipients)
        {
            string id = null;
            try
            {
                View messageBoardView = (View)Map.Database.Views["durados_v_MessageBoard"];

                
                string sql = "INSERT INTO [durados_MessageBoard] ([Subject] ,[Message] ,[OriginatedUserId] ,[ViewName] ,[ViewDisplayName] ,[PK] ,[RowDisplayName] ,[CreatedDate] ,[RowLink] ,[ViewLink]) VALUES (";
                sql += "@Subject, @Message, @OriginatedUserId, @ViewName, @ViewDisplayName, @PK, @RowDisplayName, @CreatedDate, @RowLink, @ViewLink);";
                sql += "SELECT IDENT_CURRENT('[durados_MessageBoard]') AS ID";

                Dictionary<string, object> parameters2 = new Dictionary<string, object>();
                string rowDisplayValue = view.GetDisplayValue(pk, values, prevRow);
                parameters2.Add("Subject", subject);
                parameters2.Add("Message", message);
                parameters2.Add("OriginatedUserId", currentUserId);
                parameters2.Add("ViewName", view.Name);
                parameters2.Add("ViewDisplayName", view.DisplayName);
                parameters2.Add("PK", pk);
                parameters2.Add("RowDisplayName", rowDisplayValue);
                parameters2.Add("CreatedDate", DateTime.Now);
                parameters2.Add("RowLink", FieldHelper.GetUrlData(view.DisplayName + " - " + rowDisplayValue, urlAction));
                parameters2.Add("ViewLink", FieldHelper.GetUrlData(view.DisplayName + " - " + rowDisplayValue, urlAction));


                SqlAccess sqlAccess = new SqlAccess();
                id = sqlAccess.ExecuteScalar(Map.Database.SysDbConnectionString, sql, parameters2);

                View messageStatusView = (View)Map.Database.Views["durados_MessageStatus"];

                foreach (int recipient in recipients.Keys)
                {
                    SaveMessageAction(messageStatusView, id, recipients[recipient] ? "True" : "False", 4, recipient);
                }
            }
            catch (Exception ex)
            {
                Map.Logger.Log(GetControllerNameForLog(this.ControllerContext), this.ControllerContext.RouteData.Values["action"].ToString(), ex.Source, ex, 1, "Save Message Board View name: " + view.Name + ", pk: " + pk);
            }

            return id;
        }

        public virtual string GetFieldValue(Durados.Field field, string pk)
        {
            return field.GetValue(pk);
        }

        protected virtual Durados.Web.Mvc.UI.TableViewer GetNewTableViewer()
        {
            return new Durados.Web.Mvc.UI.TableViewer();
        }

        Durados.Web.Mvc.UI.TableViewer tableViewer = null;

        public ITableConverter GetTableViewer()
        {
            if (tableViewer == null)
                tableViewer = GetNewTableViewer();

            return tableViewer;
        }

        public void LoadValues(Dictionary<string, object> values, System.Data.DataRow dataRow, Durados.View view, Durados.ParentField parentField, Durados.View rootView, string dynastyPath, string prefix, string postfix, Dictionary<string, Durados.Workflow.DictionaryField> dicFields, string internalDynastyPath)
        {
            if (view.Equals(rootView))
            {
                dynastyPath = GetViewDisplayName((View)view) + ".";
                internalDynastyPath = view.Name + ".";
            }
            foreach (Durados.Field field in view.Fields.Values.Where(f => f.FieldType == Durados.FieldType.Column))
            {
                LoadValue(values, dataRow, view, field, dynastyPath, prefix, postfix, dicFields, internalDynastyPath);
            }

            var childrenFields = view.Fields.Values.Where(f => f.FieldType == Durados.FieldType.Children && ((ChildrenField)f).LoadForBlockTemplate);
            foreach (ChildrenField field in childrenFields)
            {
                string name = prefix + dynastyPath + field.DisplayName + postfix;
                string internalName = prefix + internalDynastyPath + field.Name + postfix;
                System.Data.DataView value = GetDataView(field, dataRow);
                if (!values.ContainsKey(name))
                {
                    values.Add(name, value);
                    dicFields.Add(internalDynastyPath, new Durados.Workflow.DictionaryField { DisplayName = field.DisplayName, Type = field.DataType, Value = value });
                }

                foreach (ColumnField columnField in field.ChildrenView.Fields.Values.Where(f => f.FieldType == Durados.FieldType.Column))
                {
                    if (columnField.Upload != null)
                    {
                        value.Table.Columns[columnField.Name].ExtendedProperties["ImagePath"] = columnField.GetUploadPath();
                    }
                }
            }

            foreach (ParentField field in view.Fields.Values.Where(f => f.FieldType == Durados.FieldType.Parent))
            {
                if (view.Equals(rootView))
                {
                    dynastyPath = view.DisplayName + ".";
                    internalDynastyPath = view.Name + ".";
                }
                LoadValue(values, dataRow, view, field, dynastyPath, prefix, postfix, dicFields, internalDynastyPath);



                System.Data.DataRow parentRow = dataRow.GetParentRow(field.DataRelation.RelationName);
                View parentView = (View)field.ParentView;
                if (parentRow == null)
                {
                    string key = field.GetValue(dataRow);
                    if (!string.IsNullOrEmpty(key))
                        parentRow = parentView.GetDataRow(key, dataRow.Table.DataSet);
                }
                if (parentRow != null && parentField != field)
                {
                    if (parentView != rootView)
                    {
                        //dynastyPath += field.DisplayName + ".";
                        dynastyPath = GetDynastyPath(dynastyPath, (ParentField)parentField, field);
                        internalDynastyPath = GetInternalDynastyPath(internalDynastyPath, (ParentField)parentField, field);
                        LoadValues(values, parentRow, parentView, field, rootView, dynastyPath, prefix, postfix, dicFields, internalDynastyPath);
                    }
                }
            }


        }

        public void LoadValue(Dictionary<string, object> values, System.Data.DataRow dataRow, Durados.View view, Durados.Field field, string dynastyPath, string prefix, string postfix, Dictionary<string, Durados.Workflow.DictionaryField> dicFields, string internalDynastyPath)
        {
            string name = prefix + dynastyPath + field.DisplayName + postfix;
            string InternalName = prefix + internalDynastyPath + field.Name + postfix;
            string value = view.GetDisplayValue(field.Name, dataRow);
            if (!values.ContainsKey(name))
            {
                values.Add(name, value);
                dicFields.Add(InternalName, new Durados.Workflow.DictionaryField { DisplayName = name, Type = field.DataType, Value = value });

            }
            if (field.FieldType == Durados.FieldType.Column && ((ColumnField)field).Upload != null)
            {
                if (dataRow.Table.Columns.Contains(field.Name))
                {

                    dataRow.Table.Columns[field.Name].ExtendedProperties["ImagePath"] = ((ColumnField)field).GetUploadPath();
                }
            }
        }


        public Durados.View GetNonConfigView(string viewName)
        {
            return GetView(viewName);
        }

        public string HtmlDecode(string text)
        {
            return HttpUtility.HtmlDecode(text);
        }

        #endregion notifier

        #region executer
        public string[] GetFilterFieldValue(Durados.View view)
        {
            return null;
        }
        #endregion executer

        #endregion workflow
        */
    }

}
