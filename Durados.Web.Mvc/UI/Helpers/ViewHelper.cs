using System;
using System.Data;
using System.Data.SqlClient;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using Durados.DataAccess;
using System.Reflection;

namespace Durados.Web.Mvc.UI.Helpers
{
    public static class ViewHelper
    {
        public static Map Map
        {
            get
            {
                return Maps.Instance.GetMap();
            }
        }

        private static ViewViewer viewViewer = new ViewViewer();
        
        public static string GetIndexUrl(this View view)
        {
            return viewViewer.GetIndexUrl(view);
        }

        public static Durados.Web.Mvc.View GetView(string viewName)
        {
            if (Map.Database.Views.ContainsKey(viewName))
                return (Durados.Web.Mvc.View)Map.Database.Views[viewName];
            else if (Map.Database.Views.ContainsKey(viewName))
                return (Durados.Web.Mvc.View)Map.Database.Views[viewName];
            else if (Map.GetConfigDatabase().Views.ContainsKey(viewName))
                return (Durados.Web.Mvc.Config.View)Map.GetConfigDatabase().Views[viewName];
            else if (Maps.Instance.DuradosMap.Database.Views.ContainsKey(viewName))
                return (Durados.Web.Mvc.View)Maps.Instance.DuradosMap.Database.Views[viewName];
            else if (Map.GetLocalizationDatabase() != null && Map.GetLocalizationDatabase().Views.ContainsKey(viewName))
                return (Durados.Web.Mvc.View)Map.GetLocalizationDatabase().Views[viewName];
            else
                return null;

        }

        public static Durados.Web.Mvc.View GetViewForRest(string viewName)
        {
            if (Map.Database.Views.ContainsKey(viewName))
                return (Durados.Web.Mvc.View)Map.Database.Views[viewName];
            else if (Map.Database.Views.ContainsKey(viewName))
                return (Durados.Web.Mvc.View)Map.Database.Views[viewName];
            else if (Map.GetConfigDatabase().Views.ContainsKey(viewName))
                return (Durados.Web.Mvc.Config.View)Map.GetConfigDatabase().Views[viewName];
            else if (Map.GetLocalizationDatabase() != null && Map.GetLocalizationDatabase().Views.ContainsKey(viewName))
                return (Durados.Web.Mvc.View)Map.GetLocalizationDatabase().Views[viewName];
            else
                return null;

        }

        public static string GetAutoCompleteUrl(this View view)
        {
            return viewViewer.GetAutoCompleteUrl(view);
        }

        public static string GetSetLanguageUrl(this View view)
        {
            return viewViewer.GetSetLanguageUrl(view);
        }

        public static string GetChangePageSizeUrl(this View view)
        {
            return viewViewer.GetChangePageSizeUrl(view);
        }

        public static string GetDataActionUrl(this View view, DataAction dataAction)
        {
            if (dataAction == DataAction.Create)
                return viewViewer.GetCreateOnlyUrl(view);
            if (dataAction == DataAction.Edit)
                return viewViewer.GetEditOnlyUrl(view);

            throw new NotSupportedException();
        }

        public static string GetCreateUrl(this View view)
        {
            return viewViewer.GetCreateUrl(view);
        }

        public static string GetCreateOnlyUrl(this View view)
        {
            return viewViewer.GetCreateOnlyUrl(view);
        }

        public static string GetEditUrl(this View view)
        {
            return viewViewer.GetEditUrl(view);
        }

        public static string GetDuplicateUrl(this View view)
        {
            return viewViewer.GetDuplicateUrl(view);
        }

        public static string GetSelectListUrl(this View view)
        {
            return viewViewer.GetSelectListUrl(view);
        }

        public static string GetEditRichUrl(this View view)
        {
            return viewViewer.GetEditRichUrl(view);
        }

        public static string GetRichUrl(this View view)
        {
            return viewViewer.GetRichUrl(view);
        }

        public static string GetJsonViewUrl(this View view)
        {
            return viewViewer.GetJsonViewUrl(view);
        }

        public static string GetJsonViewInlineAddingUrl(this View view)
        {
            return viewViewer.GetJsonViewInlineAddingUrl(view);
        }

        public static string GetJsonViewInlineEditingUrl(this View view)
        {
            return viewViewer.GetJsonViewInlineEditingUrl(view);
        }

        public static string GetInlineAddingDialogUrl(this View view)
        {
            return viewViewer.GetInlineAddingDialogUrl(view);
        }

        public static string GetInlineEditingDialogUrl(this View view)
        {
            return viewViewer.GetInlineEditingDialogUrl(view);
        }

        public static string GetInlineAddingCreateUrl(this View view)
        {
            return viewViewer.GetInlineAddingCreateUrl(view);
        }

        public static string GetInlineEditingEditUrl(this View view)
        {
            return viewViewer.GetInlineEditingEditUrl(view);
        }

        public static string GetInlineDuplicateDialogUrl(this View view)
        {
            return viewViewer.GetInlineDuplicateDialogUrl(view);
        }

        public static string GetInlineDuplicateUrl(this View view)
        {
            return viewViewer.GetInlineDuplicateUrl(view);
        }

        public static string GetInlineSearchDialogUrl(this View view)
        {
            return viewViewer.GetInlineSearchDialogUrl(view);
        }

        public static string GetDeleteUrl(this View view)
        {
            return viewViewer.GetDeleteUrl(view);
        }

        public static string GetRefreshUrl(this View view)
        {
            return viewViewer.GetRefreshUrl(view);
        }

        public static string GetDeleteSelectionUrl(this View view)
        {
            return viewViewer.GetDeleteSelectionUrl(view);
        }

        public static string GetEditSelectionUrl(this View view)
        {
            return viewViewer.GetEditSelectionUrl(view);
        }

        public static string GetFilterUrl(this View view)
        {
            return viewViewer.GetFilterUrl(view);
        }

        public static string GetUploadUrl(this View view)
        {
            return viewViewer.GetUploadUrl(view);
        }

        public static string GetExportToCsvUrl(this View view)
        {
            return viewViewer.GetExportToCsvUrl(view);
        }

        public static string GetAllFilterValuesUrl(this View view)
        {
            return viewViewer.GetAllFilterValuesUrl(view);
        }

        public static string GetPrintUrl(this View view)
        {
            return viewViewer.GetPrintUrl(view);
        }

        //public static string GetClientFormCollection(this View view)
        //{
        //    return viewViewer.GetClientFormCollection(view);
        //}

        //public static string GetFieldValuesFromTableIntoDialog(this View view)
        //{
        //    return viewViewer.GetFieldValuesFromTableIntoDialog(view);
        //}

        public static string GetFieldValuesFromDialogIntoTable(this View view, DataAction dataAction)
        {
            return viewViewer.GetFieldValuesFromDialogIntoTable(view, dataAction);
        }

        public static string GetAutocompleteClientControlInitiation(this View view)
        {
            return viewViewer.GetAutocompleteClientControlInitiation(view);
        }

        public static string GetJsonFilter(this View view, string guid)
        {
            return Json.JsonSerializer.Serialize<Json.Filter>(viewViewer.GetJsonFilter(view, guid));
        }


        //public static string GetValidation(this View view, DataAction dataAction, string guid)
        //{
        //    return viewViewer.GetValidation(view, dataAction, guid);

        //}

        //public static string GetRow(this View view, DataRow row)
        //{
        //    return viewViewer.GetRow(view, row);

        //}

        public static System.Collections.Specialized.NameValueCollection GetSessionState(string guid)
        {
            //if (HttpContext.Current.Session[guid] != null && HttpContext.Current.Session[guid].ToString() != string.Empty)
            //{
            //    //filter = Json.JsonSerializer.Deserialize<Json.Filter>(collection["jsonFilter"]);
                
            //    return (System.Collections.Specialized.NameValueCollection)HttpContext.Current.Session[guid];
            //}
            if (Map.Session[guid] != null)
            {
                System.Collections.Specialized.NameValueCollection c = (new UI.Json.NameValueCollectionSerializer(Map.Session[guid].ToString())).NameValueCollection;
                //HttpContext.Current.Session[guid] = c;
                return c;
            }
            return new System.Collections.Specialized.NameValueCollection();
        }

        public static void SetSessionState(string guid, System.Collections.Specialized.NameValueCollection queryString)
        {
            //HttpContext.Current.Session[guid] = queryString;
            Map.Session[guid] = new UI.Json.NameValueCollectionSerializer(queryString).ToString();
        }

        public static string GetSessionString(string guid)
        {
            if (HttpContext.Current.Session[guid] != null)
                return HttpContext.Current.Session[guid].ToString();

            if (Map.Session[guid] != null)
            {
                string c = Map.Session[guid].ToString();
                HttpContext.Current.Session[guid] = c;
                return c;
            }
            return string.Empty;
        }

        public static void SetSessionState(string guid, string value)
        {
            HttpContext.Current.Session[guid] = value;
            Map.Session[guid] = value;
        }

        public static Json.Filter ConvertQueryStringToJsonFilter(string guid)
        {
            Json.Filter filter = new Durados.Web.Mvc.UI.Json.Filter();
            System.Collections.Specialized.NameValueCollection queryString =
                    GetSessionState(guid + "PageFilterState");
            if (queryString != null)
            {
                foreach (string key in queryString.Keys)
                {
                    if (key != "menuId")
                    {
                        string value = System.Web.HttpContext.Current.Server.UrlDecode(queryString[key]);
                        if (filter.Fields.ContainsKey(key))
                        {
                            filter.Fields[key].Value = value;
                        }
                        else
                        {
                            filter.Fields.Add(key, new Json.Field() { Name = key, Value = value });
                        }
                    }
                }
            }

            return filter;
        }

        private static void MergeJsonFilters(Json.Filter filter1, Json.Filter filter2)
        {
            foreach (string key in filter2.Fields.Keys)
            {
                object value = filter2.Fields[key].Value;
                if (!filter1.Fields.ContainsKey(key))
                {
                    filter1.Fields.Add(key, new Json.Field() { Name = key, Value = value });
                }
            }
        }

        public static DataView ApplyFilter(this View view, int page, int pageSize, FormCollection collection, bool search, string SortColumn, Durados.SortDirection direction, out int rowCount, out Json.Filter filter, string guid, BeforeSelectEventHandler beforeSelectCallback, AfterSelectEventHandler afterSelectCallback)
        {
            return ApplyFilter(view, page, pageSize, collection, search, SortColumn, direction, out rowCount, out filter, guid, beforeSelectCallback, afterSelectCallback, null);
        }

        public static Json.Filter GetJsonFilter(FormCollection collection, string guid)
        {
            Json.Filter jsonFilter = ConvertQueryStringToJsonFilter(guid);
            if (jsonFilter.Fields.ContainsKey("guid"))
                jsonFilter.Fields.Remove("guid");
            if (jsonFilter.Fields.ContainsKey("isMainPage"))
                jsonFilter.Fields.Remove("isMainPage");

            Json.Filter filter = null;
            if (collection != null && collection["jsonFilter"] != null)
            {
                filter = Json.JsonSerializer.Deserialize<Json.Filter>(collection["jsonFilter"]);
                MergeJsonFilters(jsonFilter, filter);
            }
            return jsonFilter;
        }

        public static DataView ApplyFilter(this View view, int page, int pageSize, FormCollection collection, bool search, string SortColumn, Durados.SortDirection direction, out int rowCount, out Json.Filter filter, string guid, BeforeSelectEventHandler beforeSelectCallback, AfterSelectEventHandler afterSelectCallback, TableViewer tableViewer)
        {
            Json.Filter jsonFilter = ConvertQueryStringToJsonFilter(guid);
            if (jsonFilter.Fields.ContainsKey("guid"))
                jsonFilter.Fields.Remove("guid");
            if (jsonFilter.Fields.ContainsKey("isMainPage"))
                jsonFilter.Fields.Remove("isMainPage");

            filter = null;
            if (collection != null && collection["jsonFilter"] != null)
            {
                try
                {
                    filter = Json.JsonSerializer.Deserialize<Json.Filter>(collection["jsonFilter"]);
                    MergeJsonFilters(jsonFilter, filter);
                }
                catch (Exception ex)
                {
                    Map.Logger.Log(string.Empty, string.Empty, "ApplyFilter", ex, 3, string.Empty);
                }
            }
            if (tableViewer != null)
            {
                tableViewer.HandleFilter(jsonFilter);
            }
            return viewViewer.FillPage(view, page, pageSize, jsonFilter, search, SortColumn, direction, out rowCount, beforeSelectCallback, afterSelectCallback);
        }

        public static Json.Filter GetPageFilter(this View view, string guid)
        {
            Json.Filter filter = new Durados.Web.Mvc.UI.Json.Filter();

            System.Collections.Specialized.NameValueCollection queryString =
                    GetSessionState(guid + "PageFilterState");
            if (queryString != null)
            {
                foreach (string key in queryString.Keys)
                {
                    if (key.StartsWith("__") && key.EndsWith("__"))
                    {
                        string value = queryString[key];
                        filter.Fields.Add(key, new Json.Field() { Name = key.Trim('_'), Value = value });
                        break;
                    }
                }
            }

            return filter;
        }

        public static DataRow CreateRow(this View view, FormCollection collection, string insertAbovePK, BeforeCreateEventHandler beforeCreateCallback, BeforeCreateInDatabaseEventHandler beforeCreateInDatabaseCallback, AfterCreateEventHandler afterCreateBeforeCommitCallback, AfterCreateEventHandler afterCreateAfterCommitCallback)
        {
            Json.View jsonView = Json.JsonSerializer.Deserialize<Json.View>(collection["jsonView"]);
            return viewViewer.CreateRow(view, insertAbovePK, jsonView, beforeCreateCallback, beforeCreateInDatabaseCallback, afterCreateBeforeCommitCallback, afterCreateAfterCommitCallback);
        }

        public static DataRow CreateRow(this View view, Json.View jsonView, string insertAbovePK, BeforeCreateEventHandler beforeCreateCallback, BeforeCreateInDatabaseEventHandler beforeCreateInDatabaseCallback, AfterCreateEventHandler afterCreateBeforeCommitCallback, AfterCreateEventHandler afterCreateAfterCommitCallback)
        {
            return viewViewer.CreateRow(view, insertAbovePK, jsonView, beforeCreateCallback, beforeCreateInDatabaseCallback, afterCreateBeforeCommitCallback, afterCreateAfterCommitCallback);
        }

        public static void Delete(this View view, string fk, string fkField, BeforeDeleteEventHandler beforeDeleteCallback, AfterDeleteEventHandler afterDeleteBeforeCommitCallback, AfterDeleteEventHandler afterDeleteAfterCommitCallback)
        {
            viewViewer.Delete(view, fk, fkField, beforeDeleteCallback, afterDeleteBeforeCommitCallback, afterDeleteAfterCommitCallback);
        }

        public static void EditRow(this View view, FormCollection collection, string pk, bool ignoreNull, BeforeEditEventHandler beforeEditCallback, BeforeEditInDatabaseEventHandler beforeEditInDatabaseCallback, AfterEditEventHandler afterEditBeforeCommitCallback, AfterEditEventHandler afterEditAfterCommitCallback)
        {
            Json.View jsonView = Json.JsonSerializer.Deserialize<Json.View>(collection["jsonView"]);
            viewViewer.EditRow(view, jsonView, pk, ignoreNull, beforeEditCallback, beforeEditInDatabaseCallback, afterEditBeforeCommitCallback, afterEditAfterCommitCallback);
        }

        public static void CopyPaste(this View view, FormCollection collection, BeforeEditEventHandler beforeEditCallback, BeforeEditInDatabaseEventHandler beforeEditInDatabaseCallback, AfterEditEventHandler afterEditBeforeCommitCallback, AfterEditEventHandler afterEditAfterCommitCallback)
        {
            Json.CopyPaste jsonView = Json.JsonSerializer.Deserialize<Json.CopyPaste>(collection["jsonView"]);
            viewViewer.CopyPasteView(view, jsonView, beforeEditCallback, beforeEditInDatabaseCallback, afterEditBeforeCommitCallback, afterEditAfterCommitCallback);
        }

        public static void EditField(this View view, string fieldName, object fieldValue, string pk, BeforeEditEventHandler beforeEditCallback, BeforeEditInDatabaseEventHandler beforeEditInDatabaseCallback, AfterEditEventHandler afterEditBeforeCommitCallback, AfterEditEventHandler afterEditAfterCommitCallback)
        {
            viewViewer.EditField(view, fieldName, fieldValue, pk, beforeEditCallback, beforeEditInDatabaseCallback, afterEditBeforeCommitCallback, afterEditAfterCommitCallback);
        }

        public static string GetDateFormat(this View view)
        {
            return view.Database.DateFormat;
        }

        public static Json.View GetJsonViewNotSerialized(this View view, DataAction dataAction, string guid)
        {
            return viewViewer.GetJsonView(view, dataAction, guid);
        }

        public static string GetJsonView(this View view, DataAction dataAction, string guid)
        {
            return Json.JsonSerializer.Serialize<Json.View>(GetJsonViewNotSerialized(view, dataAction, guid));
        }

        public static string GetJsonViewSerialized(this View view, Json.View jsonView)
        {
            return Json.JsonSerializer.Serialize<Json.View>(jsonView);
        }

        public static Json.View GetJsonViewNotSerialized(this View view, DataAction dataAction, string pk, DataRow dataRow, string guid)
        {
            return viewViewer.GetJsonView(view, dataAction, pk, dataRow, guid);
        }

        public static Json.View GetJsonViewNotSerialized(this View view, DataAction dataAction, string pk, DataRow dataRow, string guid, BeforeSelectEventHandler beforeSelectCallback, AfterSelectEventHandler afterSelectCallback)
        {
            return viewViewer.GetJsonView(view, dataAction, pk, dataRow, guid, beforeSelectCallback, afterSelectCallback);
        }

        public static string GetJsonView(this View view, DataAction dataAction, string pk, DataRow dataRow, string guid)
        {
            return Json.JsonSerializer.Serialize<Json.View>(viewViewer.GetJsonView(view, dataAction, pk, dataRow, guid));
        }

        public static object GetJsonDisplayValue(this View view, DataRow dataRow)
        {
            return Json.JsonSerializer.Serialize<Json.View>(viewViewer.GetJsonDisplayValue(view, dataRow));
        }


        //public static bool ViewHasMenu(Menu menu)
        //{
        //    foreach (Durados.Web.Mvc.View view in menu.Views)
        //    {
        //        if (view.IsVisible())
        //            return true;
        //    }
        //    return false;
        //}
       

        //public static string GetAutoCompleteAction(this View view)
        //{
        //    return view.au;
        //} 

        public static string AddRow(string tableName, List<d_Field> fields)
        {
            using (SqlConnection connection = new SqlConnection(Map.Database.ConnectionString))
            {
                connection.Open();
                SqlCommand command = new SqlCommand();
                command.Connection = connection;

                bool rollback = false;

                return AddRow(tableName, fields, command, out rollback);
            }
        }

        public static string AddRow(string tableName, List<d_Field> fields, System.Data.SqlClient.SqlCommand command, out bool rollback)
        {
            DataAccess.SqlAccess dataAccess = new Durados.DataAccess.SqlAccess();
            Durados.Web.Mvc.View view = (Durados.Web.Mvc.View)Map.Database.GetView(tableName);

            Json.View jsonView = new Durados.Web.Mvc.UI.Json.View();

            foreach (d_Field field in fields)
            {
                string name = view.GetFieldByColumnNames(field.Name).Name;
                jsonView.Fields.Add(name, new Json.Field() { Name = name, Value = field.Value });
            }
            foreach (Durados.Field field in view.Fields.Values.Where(f => f.FieldType != FieldType.Children && !f.IsExcludedInInsert()))
            {
                if (!jsonView.Fields.ContainsKey(field.Name))
                {
                    jsonView.Fields.Add(field.Name, new Json.Field() { Name = field.Name, Value = string.Empty });
                }
            }

            Dictionary<string, object> values = new Dictionary<string, object>();
            foreach (Json.Field jsonField in jsonView.Fields.Values)
                values.Add(jsonField.Name, LocalizeValue(view, jsonField.Name, jsonField.Value));

            int? pk = dataAccess.Create(view, values, false, null, null, null, null, null, command, out rollback);

            if (pk.HasValue)
                return pk.Value.ToString();
            else
                return null;

            //DataRow row = view.CreateRow(jsonView, null, null, null);

            //return view.GetPkValue(row);
        }

        private static string LocalizeValue(View view, string fieldName, object value)
        {
            Field field = view.Fields[fieldName];

            if (value == null)
                return string.Empty;
            if (field.FieldType == FieldType.Column)
            {
                return field.ConvertFromString(value.ToString()).ToString();
            }
            else
            {
                return value.ToString();
            }
        }


        public static List<d_Field> GetFieldsCollection(string tableName)
        {
            Durados.Web.Mvc.View view = (Durados.Web.Mvc.View)Map.Database.GetView(tableName);

            List<d_Field> fields = new List<d_Field>();


            foreach (Durados.Field field in view.VisibleFieldsForCreate.Where(f => f.FieldType != FieldType.Children))
            {
                fields.Add(new d_Field() { Name = field.Name, Value = string.Empty });
            }

            return fields;
        }

        public static string GetScripts(this View view)
        {
            return GetReferences(view, ReferenceType.JavaScript);
        }

        public static string GetStyleSheets(this Durados.Services.IStyleable styleable)
        {
            return GetReferences(styleable, ReferenceType.StyleSheet);
        }

        /// <summary>
        /// Get dictionary values from a specific row
        /// </summary>
        /// <param name="view"></param>
        /// <param name="row"></param>
        /// <param name="fkField"></param>
        /// <param name="fkValue"></param>
        /// <returns></returns>
        public static Dictionary<string, object> GetDictionaryFromRow(this View view, DataRow row, Field fkField, string fkValue)
        {
            Dictionary<string, object> dictionary = new Dictionary<string, object>();

            foreach (Field field in view.Fields.Values.Where(f => f.FieldType != FieldType.Children))
            {
                if (field.FieldType == FieldType.Parent)
                    if (field.Equals(fkField))
                        dictionary.Add(field.Name, fkValue);
                    else
                        dictionary.Add(field.Name, field.GetValue(row));
                else
                    dictionary.Add(field.Name, row[((ColumnField)field).DataColumn.ColumnName]);
            }

            return dictionary;
        }

        public static string GetReferences(this Durados.Services.IStyleable view, ReferenceType referenceType)
        {
            string references = string.Empty;
            if (referenceType == ReferenceType.JavaScript)
                references = view.JavaScripts;
            else if (referenceType == ReferenceType.StyleSheet)
                references = view.StyleSheets;

            if (string.IsNullOrEmpty(references))
                return string.Empty;

            string[] scripts = references.Split(';');
            string s = string.Empty;

            foreach (string script in scripts)
            {

                //string rootPath = Durados.Web.Mvc.Infrastructure.General.GetRootPath();
                //string script2 = script;
                //if (string.IsNullOrEmpty(rootPath))
                //    rootPath = "/";
                //if (!rootPath.StartsWith("/"))
                //    rootPath = "/" + rootPath;
                //if (!rootPath.EndsWith("/"))
                //    rootPath = rootPath + "/";

                //if (script2.StartsWith("/"))
                //    script2 = script2.Remove(0, 1);

                //string r = string.Empty;
                //if (rootPath.Length > 1 && script2.StartsWith(rootPath.Remove(0, 1)))
                //    r = script2;
                //else
                //    r = Durados.Web.Mvc.Infrastructure.General.GetRootPath() + script2;


                if (referenceType == ReferenceType.JavaScript)
                {
                    s += "<script src='" + script + "' type=\"text/javascript\"></script>";
                }
                else if (referenceType == ReferenceType.StyleSheet)
                {
                    s += "<link rel=\"stylesheet\" type=\"text/css\" href=\"" + script + "\" />";
                }
            }

            return s;
        }

        public static string GetTooltip(this View view)
        {
            return GetTooltip(view.DisplayName, view.Description);
        }

        public static string GetActionTooltipTitle(this View view, ToolbarActionNames action)
        {
            if (action == ToolbarActionNames.NEW)
            {
                if (!string.IsNullOrEmpty(view.NewButtonName))
                    return Map.Database.Localizer.Translate(view.NewButtonName);
            }
            if (action == ToolbarActionNames.EDIT)
            {
                if (!string.IsNullOrEmpty(view.EditButtonName))
                    return Map.Database.Localizer.Translate(view.EditButtonName);
            }
            if (action == ToolbarActionNames.DELETE)
            {
                if (!string.IsNullOrEmpty(view.DeleteButtonName))
                    return Map.Database.Localizer.Translate(view.DeleteButtonName);
            }
            if (action == ToolbarActionNames.DUPLICATE)
            {
                if (!string.IsNullOrEmpty(view.DuplicateButtonName))
                    return Map.Database.Localizer.Translate(view.DuplicateButtonName);
            }

            if (action == ToolbarActionNames.ADD_ITEMS)
            {
                if (!string.IsNullOrEmpty(view.AddItemsButtonName))
                    return Map.Database.Localizer.Translate(view.AddItemsButtonName);
            }


            return Map.Database.Localizer.Translate(view.Database.Tooltips[action.ToString()].Title);
        }

        public static string GetActionTooltipDescription(this View view, ToolbarActionNames action)
        {
            if (action == ToolbarActionNames.NEW)
            {
                if (!string.IsNullOrEmpty(view.NewButtonDescription))
                    return Map.Database.Localizer.Translate(view.NewButtonDescription);
            }
            if (action == ToolbarActionNames.EDIT)
            {
                if (!string.IsNullOrEmpty(view.EditButtonDescription))
                    return Map.Database.Localizer.Translate(view.EditButtonDescription);
            }
            if (action == ToolbarActionNames.DUPLICATE)
            {
                if (!string.IsNullOrEmpty(view.DuplicateButtonDescription))
                    return Map.Database.Localizer.Translate(view.DuplicateButtonDescription);
            }
            if (action == ToolbarActionNames.ADD_ITEMS)
            {
                if (!string.IsNullOrEmpty(view.AddItemsButtonDescription))
                    return Map.Database.Localizer.Translate(view.AddItemsButtonDescription);
            }

            return Map.Database.Localizer.Translate(view.Database.Tooltips[action.ToString()].Description);
        }


        public static string GetActionTooltip(this View view, ToolbarActionNames action)
        {
            return GetTooltip(GetActionTooltipTitle(view, action), GetActionTooltipDescription(view, action));
        }

        public static string GetTooltip(string title, string description)
        {
            
            if (string.IsNullOrEmpty(description))
                return string.Empty;

            string tooltip = string.Empty;
            string seperator = "|";

            if (string.IsNullOrEmpty(Map.Database.Localizer.Translate(title)))
            {
                title = " ";
            }

            tooltip = title + seperator;

            if (string.IsNullOrEmpty(Map.Database.Localizer.Translate(description)))
            {
                description = " ";
            }

            tooltip += description;

            return tooltip.Replace('"', '\'');

        }

        public static string GetLink(this View view, DataRow row, bool rich)
        {
            string link;
            string url = null;
            string host = null;
            if (Maps.MultiTenancy)
                host = Maps.Instance.GetMap().Url;
            else
                host = System.Web.HttpContext.Current.Request.Url.Scheme + "://" + System.Web.HttpContext.Current.Request.Url.Authority;

            url = host + "/" + view.Controller + "/" + view.IndexAction + "/" + view.Name + "?pk=" + view.GetPkValue(row) + "&guid=" + view.Name;
            
            if (rich)
                link = "<a href=\"" + url + "\">" + view.GetDisplayValue(row) + "</a>";
            else
                link = view.GetDisplayValue(row) + ": " + url;

            return link;

        }

        public static string GetAppBasePath(this HtmlHelper helper)
        {
            //var helper = new UrlHelper(this.ControllerContext.RequestContext); //In controller
            //this.Url.RouteUrl(this.ViewContext.RouteData.Values); // in view ?

            UrlHelper urlHelper = new UrlHelper(helper.ViewContext.RequestContext);

            var request = HttpContext.Current.Request;

            string host= request.Url.Authority;
            if (Maps.MultiTenancy)
                host = System.Web.HttpContext.Current.Request.Headers["Host"];

            return string.Format("{0}://{1}{2}", request.Url.Scheme, host, urlHelper.Content("~"));
        }

        public static string GetAppBasePath(System.Web.Routing.RequestContext context)
        {
            //var helper = new UrlHelper(this.ControllerContext.RequestContext); //In controller

            UrlHelper urlHelper = new UrlHelper(context);

            var request = HttpContext.Current.Request;

            return string.Format("{0}://{1}{2}", request.Url.Scheme, request.Url.Authority, urlHelper.Content("~"));
        }


        public static Json.CustomView GetDesrializedCustomView(string viewName, bool defaults = false)
        {
            string jsonView = GetCustomView(viewName, defaults);

            if (jsonView != string.Empty) 
            {
                try 
                {
                    return Json.JsonSerializer.Deserialize<Json.CustomView>(jsonView);
                } 
                catch (Exception) 
                {
                    //TODO - Log exeption
                }
            }
            return null;
        }


        public static string GetCustomView(string viewName, bool defaults)
        {
            //Durados.Web.Mvc.View view = (Durados.Web.Mvc.View)Map.Database.Views["durados_CustomViews"];            

            string sql = GetSelectForCustomeViewTable();

            int UserId = -1;
            if (!defaults) {
                UserId = Convert.ToInt32(Map.Database.GetUserID());
            }

            Dictionary<string, object> parameters = new Dictionary<string, object>();

            parameters.Add("UserId", UserId);
            parameters.Add("ViewName", viewName);

            IDataTableAccess da = GetSystemSqlAccess();

            string result = da.ExecuteScalar(Map.Database.SystemConnectionString ?? Map.Database.ConnectionString, sql, parameters);

            if (result == string.Empty && UserId != -1) 
            {
                parameters["UserId"] = -1;
                result = da.ExecuteScalar(Map.Database.SystemConnectionString ?? Map.Database.ConnectionString, sql, parameters);

            }

            return result;
        }

        private static string GetSelectForCustomeViewTable()
        {
           return DataAccessHelper.GetDataTableAccess(Map.systemConnectionString).GetNewSqlSchema().GetSelectForCustomeViewTable();
            //
        }

        private static IDataTableAccess GetSystemSqlAccess()
        {
            return DataAccessHelper.GetDataTableAccess(Map.systemConnectionString);
          //  return new DataAccess.SqlAccess();
        }

        public static string GetJsonDatesFormats()
        {
            return  Json.JsonSerializer.Serialize<List<DatesFormatStrings>>(DateFormatsMapper.getLegalDateFormats());
        }

        public static string GetJsonTimesFormats()
        {
            return Json.JsonSerializer.Serialize<List<DatesFormatStrings>>(DateFormatsMapper.getLegalTimeFormats());
        }

        public static bool IsPartOfPk(this View configView, DataRow row)
        {
            bool isPkField = false;

            if (configView.Name == "Field" && configView.DataTable.Columns.Contains("Name") && configView.DataTable.Columns.Contains("Fields"))
            {
                string fieldName = row["Name"].ToString();
                string viewId = row["Fields"].ToString();

                Durados.DataAccess.ConfigAccess configAccess = new Durados.DataAccess.ConfigAccess();
                string viewName = configAccess.GetViewNameByPK(viewId, Map.GetConfigDatabase().ConnectionString);

                if (viewName == null)
                    return false;

                if (Map.Database.Views.ContainsKey(viewName))
                {
                    Durados.View dbView = Map.Database.Views[viewName];
                    if (dbView.Fields.ContainsKey(fieldName))
                    {
                        Durados.Field field = dbView.Fields[fieldName];

                        isPkField = field.IsPartOfPK();
                    }
                }
            }

            return isPkField;
        }

        public static void Initiate()
        {
            lock (Map)
            {
                string fileName = Map.GetConfigDatabase().ConnectionString;
                Database configDatabase = Map.GetConfigDatabase();
                Map.Database.SetNextMinorConfigVersion();
                Durados.DataAccess.ConfigAccess.UpdateVersion(Map.Database.ConfigVersion, fileName);
                Durados.DataAccess.ConfigAccess.SaveConfigDataset(configDatabase.ConnectionString, Map.Logger);
                Map.SaveDynamicMapping();
                Map.Initiate();
                ConfigAccess.Refresh(fileName);
                configDatabase = Map.GetConfigDatabase();
                Map.JsonConfigCache.Clear();
                
            }
        }

        private static string GetNewViewMenuPK(ConfigAccess configAccess, Database configDatabase)
        {
            string pk = configAccess.GetMenuPK("New Views", configDatabase.ConnectionString);
            if (pk != null)
                return pk;
            View menuView = (View)configDatabase.Views["Menu"];
            Dictionary<string, object> values = new Dictionary<string, object>();
            values.Add("Name", "New Views");
            values.Add("WorkspaceID", Map.Database.GetDefaultWorkspace().ID);
            values.Add("Ordinal", "1000");
            //values.Add("Root", false);
            System.Data.DataRow row = menuView.Create(values, null, null, null, null, null);

            return menuView.GetPkValue(row);
        }

        private static void CacheForeignKeys(Database database)
        {
            database.Map.ForeignKeys.Clear();

            IDataTableAccess sqlAccess = DataAccessHelper.GetDataTableAccess(database.ConnectionString);

            sqlAccess.LoadForeignKeys(database.ConnectionString, database.SqlProduct, database.Map.ForeignKeys);
        }

        public static int AddViews(this Database database, string pks, System.Data.DataView dataView, bool autoGeneration, out string errorMessages)
        {
            CacheForeignKeys(database);

            const string EditableTableName = "EditableTableName";
            const string Name = "Name";
            const string viewName = "View";

            errorMessages = string.Empty;
            int added = 0;

            Durados.Database configDatabase = Map.GetConfigDatabase();

            string[] pkArray = pks.Split(',');

            Dictionary<string, string> viewNames = new Dictionary<string, string>();

            foreach (string pk in pkArray)
            {
                System.Data.DataRow row = dataView.Table.Rows.Find(pk);
                if (row != null)
                {
                    string name = row[Name].ToString();
                    string editableTableName = row[EditableTableName].ToString();
                    try
                    {
                        Map.CreateView(name, null, editableTableName, (View)configDatabase.Views[viewName]);
                        viewNames.Add(name, editableTableName);
                        added++;
                        
                    }
                    catch (Exception exception)
                    {
                        errorMessages += "Failed to add '" + name + "': " + exception.Message + "\n\r";
                    }
                }
            }
            Database db = Map.Database;
            Initiate();
            // update default config
            ConfigAccess configAccess = new ConfigAccess();
            string filename = configDatabase.ConnectionString;
            View configView = (View)configDatabase.Views["View"];
            View configField = (View)configDatabase.Views["Field"];
            string menuPk = GetNewViewMenuPK(configAccess, (Database)configDatabase);
            IDataTableAccess sqlAccess = DataAccessHelper.GetDataTableAccess(db.ConnectionString);
            int autoCompleteLimit = 500;
            int lastOrder = 10;
            View lastView = (View)Map.Database.Views.Values.OrderByDescending(v => v.Order).FirstOrDefault();
            if (lastView != null)
                lastOrder = lastView.Order;
            foreach (string name in viewNames.Keys)
            {
                lastOrder += 10;
                string configViewPk = configAccess.GetViewPK(name, filename);
                Dictionary<string, object> values = new Dictionary<string, object>();
                values.Add("GridEditable", true);
                values.Add("GridEditableEnabled", true);
                values.Add("MultiSelect", true);
                //values.Add("WorkspaceID", db.GetAdminWorkspaceId().ToString());
                values.Add("WorkspaceID", db.GetDefaultWorkspace().ID.ToString());
                values.Add("Menu_Parent", menuPk);
                values.Add("Order", lastOrder.ToString());
                string editableTableName = viewNames[name];
                if (!string.IsNullOrEmpty(editableTableName))
                    values.Add("EditableTableName", editableTableName);
                configAccess.Edit(configView, values, configViewPk, null, null, null, null);

                System.Data.DataRow[] fieldsRows = configAccess.GetFieldsRows(name, filename);
                if (fieldsRows != null)
                {
                    Durados.View view = (Durados.View)db.Views[name];
                    int defaultInDashboard = 3;
                    int inDashboard = 0;

                    foreach (System.Data.DataRow fieldRow in fieldsRows)
                    {
                        string configFieldPK = fieldRow["ID"].ToString();
                        string configFieldName = fieldRow["Name"].ToString();

                        bool ispk = false;

                        if (view.Fields.ContainsKey(configFieldName))
                        {
                            Field field = view.Fields[configFieldName];
                            ispk = field.IsPartOfPK();
                            values = new Dictionary<string, object>();

                            values.Add("Required", field.GetDbRequired());

                            bool notInEditable = field.GetDbNotEditable();

                            if (notInEditable)
                            {
                                values.Add("HideInCreate", true);
                                values.Add("ExcludeInInsert", true);
                                if (!ispk)
                                    values.Add("HideInEdit", true);
                                values.Add("ExcludeInUpdate", true);
                            }

                            int len = ((Durados.ColumnField)field).DataColumn.MaxLength;
                            if (field.FieldType == FieldType.Column && ((Durados.ColumnField)field).DataColumn.DataType.Equals(typeof(string)) && (len > 500 || len == -1))
                            {
                                values.Add("DataType", "LongText");
                                values.Add("DisplayFormat", "MultiLines");
                                values.Add("TextHtmlControlType", "TextArea");
                                values.Add("ColSpanInDialog", "2");
                            }
                            else if (field.FieldType == FieldType.Column && ((Durados.ColumnField)field).DataColumn.DataType.Equals(typeof(DateTime)))
                            {
                                DataColumn column = ((Durados.ColumnField)field).DataColumn;

                                DisplayFormat displayFormat = DisplayFormat.Date_Custom;
                                string dateFormat = configField.Database.DateFormat;
                                bool notDateOnly = column.ExtendedProperties.ContainsKey("dataType") && !column.ExtendedProperties["dataType"].ToString().ToLower().Equals("date");


                                if (dateFormat == "MM/dd/yyyy")
                                {
                                    if (notDateOnly)
                                    {
                                        displayFormat = DisplayFormat.Date_mm_dd_24;
                                    }
                                    else
                                    {
                                        displayFormat = DisplayFormat.Date_mm_dd;
                                    }
                                }
                                else if (dateFormat == "dd/MM/yyyy")
                                {
                                    if (notDateOnly)
                                    {
                                        displayFormat = DisplayFormat.Date_dd_mm_24;
                                    }
                                    else
                                    {
                                        displayFormat = DisplayFormat.Date_dd_mm;
                                    }
                                }

                                if (notDateOnly && !dateFormat.Contains(":"))
                                {
                                    dateFormat += " hh:mm:ss";
                                }

                                values.Add("DisplayFormat", displayFormat);
                                values.Add("Format", dateFormat);
                            }

                            configAccess.Edit(configField, values, configFieldPK, null, null, null, null);
                        }

                        string fieldType = fieldRow["FieldType"].ToString();

                        if (fieldType != "Children" && !ispk && inDashboard < defaultInDashboard)
                        {
                            values = new Dictionary<string, object>();
                            values.Add("Dashboard", true);
                            configAccess.Edit(configField, values, configFieldPK, null, null, null, null);
                            inDashboard++;
                        }

                        if (fieldType == "Parent")
                        {
                            if (!database.NoSysIndex)
                            {
                                try
                                {
                                    if (!fieldRow.IsNull("RelatedViewName"))
                                    {
                                        string relatedViewName = fieldRow["RelatedViewName"].ToString();
                                        Durados.View relatedView = (Durados.View)db.Views[relatedViewName];
                                        int rowCount = sqlAccess.RowCount(relatedView);
                                        if (rowCount > autoCompleteLimit)
                                        {
                                            values = new Dictionary<string, object>();
                                            values.Add("ParentHtmlControlType", "Autocomplete");
                                            values.Add("DisplayFormat", "AutoCompleteStratWith");
                                            values.Add("AutocompleteFilter", true);
                                            configAccess.Edit(configField, values, configFieldPK, null, null, null, null);

                                        }
                                    }
                                }
                                catch { }
                            }
                        }
                    }


                   

                }

                try
                {
                    Durados.View view = Map.Database.Views[name];
                    int? categoryId = ConfigAccess.GetCategoryId("General", Map.GetConfigDatabase().ConnectionString);
                    if (categoryId.HasValue)
                    {
                        foreach (Field field in view.Fields.Values)
                        {
                            if (!(field.FieldType == FieldType.Children && !field.IsCheckList()))
                            {

                                string configFieldPK = configAccess.GetFieldPK(view.Name, field.Name, Map.GetConfigDatabase().ConnectionString);
                                values = new Dictionary<string, object>();
                                values.Add("Category_Parent", categoryId.ToString());
                                configAccess.Edit(configField, values, configFieldPK, null, null, null, null);

                            }
                        }
                    }
                }
                catch { }

                if (!database.NoSysIndex)
                {
                    try
                    {
                        Durados.View view = Map.Database.Views[name];

                        foreach (ChildrenField field in view.Fields.Values.Where(f => f.FieldType == FieldType.Children && ((ChildrenField)f).ChildrenHtmlControlType == ChildrenHtmlControlType.CheckList))
                        {
                            Durados.View relatedView = field.ChildrenView;
                            if (relatedView.Fields.Values.Where(f => f.FieldType == FieldType.Parent).Count() == 2)
                            {
                                Field[] parents = relatedView.Fields.Values.Where(f => f.FieldType == FieldType.Parent).ToArray();

                                ParentField parent = null;

                                if (((ParentField)parents[0]).ParentView.Equals(view))
                                {
                                    parent = (ParentField)parents[1];
                                }
                                else if (((ParentField)parents[1]).ParentView.Equals(view))
                                {
                                    parent = (ParentField)parents[0];
                                }

                                relatedView = parent.ParentView;

                                int rowCount = sqlAccess.RowCount(relatedView);
                                string configFieldPK = configAccess.GetFieldPK(view.Name, field.Name, Map.GetConfigDatabase().ConnectionString);
                                if (rowCount > autoCompleteLimit)
                                {
                                    values = new Dictionary<string, object>();
                                    values.Add("ChildrenHtmlControlType", "Hide");
                                    configAccess.Edit(configField, values, configFieldPK, null, null, null, null);

                                }
                            }
                        }

                    }
                    catch { }
                }
            }

            if (Map.Database.Views.Values.Where(v => !v.SystemView).Count() == 0 && viewNames.Keys.Count() > 0)
            {
                return -100;
            }

            configDatabase = Map.GetConfigDatabase();
            database = Map.Database;
                
            Dictionary<string, long> rowCounts = GetRowCounts(database);

            Durados.View[] views = null;

            if (autoGeneration)
            {
                if (!(Map is DuradosMap))
                {
                    InitAddViewsParameters();


                    SetMenu(rowCounts, configDatabase, database);

                }
            }
            else
            {
                SetTopMenus(viewNames);
            }

            try
            {
                WriteStatistics(GetFeatures(rowCounts, views, database));
            }
            catch { }

            Initiate();

            return added;
        }

        public static void SetTopMenus(Dictionary<string, string> viewNames)
        {
            Database database = Map.Database;
            Database configDatabase = Map.GetConfigDatabase();

            string workspaceId = database.GetPublicWorkspaceId().ToString();
            ConfigAccess configAccess = new ConfigAccess();
            View configView = (View)configDatabase.Views["View"];
           
            foreach (string viewName in viewNames.Keys)
            {
                View view = (View)Map.Database.Views[viewName];
                string url = "/" + view.Controller + "/" + view.IndexAction + "/" + view.Name;
                SetTopMenu(view.Name, view.Name, configDatabase, url, workspaceId, LinkType.View, view.Order);
                SetViewWorkspace(view, configAccess, configView, configDatabase.ConnectionString, workspaceId);
            }
        }

        private static void SetMenu(Dictionary<string, long> rowCounts, Durados.Database configDatabase, Database database)
        {
            SetHomeMenu(configDatabase, database);

            Durados.View[] views = SetMainInterestsMenu(rowCounts, configDatabase, database);

            SetDashboardMenu(configDatabase, database);

            SetSettingsMenus(views, configDatabase, database);

        }

        private static void SetDashboardMenu(Durados.Database configDatabase, Database database)
        {
            string dashboardId = CreatBasicDashboard();
            string workspaceId = database.GetPublicWorkspaceId().ToString();
            string menuName = Map.Database.Localizer.Translate("Dashboard");
            string url = Map.Database.GetDashboardUrl() + dashboardId ;

            string parentMenu = SetTopMenu(menuName, string.Empty, configDatabase, url, workspaceId, LinkType.MyCharts, 10);

            string pageName = Map.Database.Localizer.Translate("New Page");
            string content = "new content page";
            string pageId = CreateContentPage(configDatabase, pageName, workspaceId, content);
            url = "/Home/Page?pageId=" + pageId;

            SetChildMenu(pageName, parentMenu, pageId, configDatabase, url, workspaceId, LinkType.Page, 100);

            pageName = Map.Database.Localizer.Translate("My Website");
            string externalNewPage = "www.backand.com|_blank|http://www.backand.com";
            pageId = CreateExternalPage(configDatabase, pageName, workspaceId, externalNewPage, true);
            url = "/Home/Page?pageId=" + pageId;

            SetChildMenu(pageName, parentMenu, pageId, configDatabase, url, workspaceId, LinkType.Page, 100);



        }

        private static string CreatBasicDashboard()
        {
            string dashboardId = Map.CreateDashboard("My Charts");
            Dictionary<string, object> values = new Dictionary<string, object>();
            string sql = "SELECT top(10) substring(Controller,1,10) as Controller, COUNT(*) as [Count]  FROM [Durados_Log]  where logtype<=3 group by Controller Order By [Count] desc ";
            values.Add("Name", "Activity");
            values.Add("SubTitle", "Example");
            values.Add("SQL", sql);
            values.Add("XField", "Controller");
            values.Add("YField", "Count");
            values.Add("XTitle", "Durados Views");
            values.Add("YTitle", "Count Activity");
            values.Add("Height", 340);
            //Charts.Add(2, new Chart() { ID = 2, ChartType = ChartType.Line, Name = "Activity", SubTitle = "Example", SQL = sql, XField = "Controller", YField = "Count", XTitle = "Durados Views", YTitle = "Count Activity", Height = 340, Align = ChartAlignment.Left, Ordinal = 2, Database = Database });
            //Charts.Add(3, new Chart() { ID = 3, ChartType = ChartType.Bar, Name = "Activity", SubTitle = "Example", SQL = sql, XField = "Controller", YField = "Count", XTitle = "Durados Views", YTitle = "Count Activity", Height = 340, Align = ChartAlignment.Right, Ordinal = 1, Database = Database });
            //Charts.Add(4, new Chart() { ID = 4, Height = 340, Align = ChartAlignment.Right, Ordinal = 2 });

            values["ChartType"] = ChartType.Pie;
            Map.AddNewChartToDashboard(dashboardId, 1, 1, new Dictionary<string, object>(values), false);
            values["ChartType"] = ChartType.bubble;
            Map.AddNewChartToDashboard(dashboardId, 2, 2, new Dictionary<string, object>(values), false);
            values["ChartType"] = ChartType.Bar;
            Map.AddNewChartToDashboard(dashboardId, 3, 1, new Dictionary<string, object>(values), false);

            Map.AddNewChartToDashboard(dashboardId, 4, 2, null, false);

            return dashboardId;
        }

        private static void SetHomeMenu(Durados.Database configDatabase, Database database)
        {

            string workspaceId = database.GetPublicWorkspaceId().ToString();
            string pageName = Map.Database.Localizer.Translate("Home");
            string content = Map.GetWorkspaceDefaultDescription(); //"<style>.admin-main-content {margin-left: 20px;} .admin-button {color:#c4f434;}.admin-button:link, .admin-button:visited{text-decoration:none;}  .admin-button:hover {text-decoration:underline; color:White;}  .admin-button span{display: block;width: 150px;background-image: linear-gradient(rgb(66,65,66) 0%, rgb(100,100,100) 100%);border-radius: 3px;text-align: center;height:30px; padding-top:10px;font-size:18px;font-family: Arial, Helvetica, sans-serif;}</style><div class=\"admin-main-content\"><h1>    Admin workspace landing page</h1><h2>    The main functionality of this page:</h2><div><div style=\"display: inline; height: 450px; width: 200px; float: left;\"><p><a href=\"/Admin/Index/Menu\" class=\"admin-button\"><span>Menus</span></a></p><p><a href=\"/Admin/Index/View\" class=\"admin-button\"><span>Tables / Views</span></a></p><p><a href=\"/DuradosUser/Index/v_durados_User\" class=\"admin-button\"><span>Users</span></a></p><p><a href=\"/Admin/Index/Page\" class=\"admin-button\"><span>Pages</span></a></p></div><div style=\"display: inline;\"><ul><li><font size=\"4\"><b>Design</b></font></li><ul><li style=\"font-size: 18px;\"><b>- Defaults</b>: Control the application's default&nbsp;behavior</li></ul><ul><li style=\"font-size: 18px;\"><b>- Views</b>: A list of all the tables and views that selected from the database. You can add new tables by selecting \"<b>Add Views</b>\". To view the <b>ERD </b>of the&nbsp;database&nbsp;click on the summary icon</li><li style=\"font-size: 18px;\">- <b>Fields</b>: A list of all the fields in the entire database</li><li style=\"font-size: 18px;\">- <b>Menus</b>: Manage the menu in the work-space</li><li style=\"font-size: 18px;\">- <b>Categories</b>: A category is used to create Tab style in form view</li><li style=\"font-size: 18px;\">- <b>FTP Upload</b>: Manage all the FTP locations that host files</li><li style=\"font-size: 18px;\">- <b>Content</b>: A content management page for emails, user notifications and more</li><li style=\"font-size: 18px;\">- <b>Charts</b>: Manage the details in the chart dashboard</li></ul></ul><ul><li style=\"font-size: 18px;\"><b>Users &amp; Roles</b></li><ul><li style=\"font-size: 18px;\"><b>Users:&nbsp;</b>&nbsp;Manage the list of users and their roles</li><li style=\"font-size: 18px;\"><b>Roles</b>:&nbsp;Manage the&nbsp;roles</li><li style=\"font-size: 18px;\"><b>Workspaces</b>: Manage the works-paces content page and security access list</li></ul></ul><ul><li><font size=\"4\"><b>Trace</b></font></li><ul><li><font size=\"4\"><b>Monitor</b>: Log all the activity and exceptions</font></li><li><font size=\"4\"><b>History</b>: Display all the recorded changes made by user. In order to record the changes, enable the History flag for each views</font></li></ul></ul><ul><li><font size=\"4\"><b>New Views</b></font></li><ul><li><font size=\"4\">Shows all the new views that have recently been added to the application</font></li></ul></ul></div></div></div>";
            string pageId = CreateContentPage(configDatabase, pageName, workspaceId, content);
            string url = "/Home/Page?pageId=" + pageId;

            string parentMenu = SetTopMenu(pageName, pageId, configDatabase, url, workspaceId, LinkType.Page, 0);

            Dictionary<string, object> values = new Dictionary<string, object>();
            values.Add("HomePage", parentMenu);

            configDatabase.Views["Workspace"].Edit(values, workspaceId, null, null, null, null);
        }

        private static string CreateExternalPage(Durados.Database configDatabase, string pageName, string workspaceId, string externalNewPage, bool newTab)
        {
            View view = (View)configDatabase.Views["Page"];
            Dictionary<string, object> values = new Dictionary<string, object>();
            PageType pageType = PageType.External;
            values.Add("PageType", pageType.ToString());
            values.Add("Pages_Parent", 0);
            values.Add("WorkspaceID", workspaceId);
            values.Add("Title", pageName);
            values.Add("ExternalNewPage", externalNewPage);
            values.Add("NewTab", newTab);

            DataRow row = view.Create(values, null, null, null, null, null);
            string pk = view.GetPkValue(row);
            return pk;
        }

        private static string CreateContentPage(Durados.Database configDatabase, string pageName, string workspaceId, string content)
        {
            View view = (View)configDatabase.Views["Page"];
            Dictionary<string, object> values = new Dictionary<string, object>();
            PageType pageType = PageType.Content;
            values.Add("PageType", pageType.ToString());
            values.Add("Pages_Parent", 0);
            values.Add("WorkspaceID", workspaceId);
            values.Add("Title", pageName);
            values.Add("Content", content);
            
            DataRow row = view.Create(values, null, null, null, null, null);
            string pk = view.GetPkValue(row);
            return pk;
        }

        private static void WriteStatistics(Dictionary<string, Dictionary<string, object>> dictionary)
        {
            StringBuilder sb = new StringBuilder();
            //int parentCount = 0;
            //long maxRowCount = 0;

            foreach (Dictionary<string, object> features in dictionary.Values)
            {
                foreach (string name in features.Keys)
                {
                    sb.Append(name);
                    sb.Append(":");

                    sb.Append(features[name]);
                    if (features.Keys.Last() != name)
                        sb.Append(",");

                }
                //parentCount += (int)features["ParentCount"];
                //maxRowCount = Math.Max(maxRowCount, (long)features["RowCount"]);
                sb.Append(";");
            }

            Maps.Instance.DuradosMap.Logger.Log("Basic", "", "", "", "", 3, sb.ToString(), DateTime.Now);

            string total = "Table Count: " + dictionary.Count;


            Maps.Instance.DuradosMap.Logger.Log("BasicTotal", "", "", "", "", 3, total, DateTime.Now);
        }

        private static Dictionary<string, Dictionary<string, object>> GetFeatures(Dictionary<string, long> rowCounts, Durados.View[] views, Database database)
        {
            Dictionary<string, Dictionary<string, object>> featuresList = new Dictionary<string, Dictionary<string, object>>();
            foreach (string viewName in rowCounts.Keys)
            {
                Dictionary<string, object> features = new Dictionary<string, object>();

                Durados.View view = database.Views[viewName];
                features.Add("ViewName", viewName);
                features.Add("RowCount", rowCounts[viewName]);
                features.Add("ParentCount", view.ParentCount);
                features.Add("Interest", 0);

                featuresList.Add(viewName, features);
            }

            if (views != null)
            {
                for (int i = 0; i < views.Length; i++)
                {
                    if (featuresList.ContainsKey(views[i].Name))
                    {
                        featuresList[views[i].Name]["Interest"] = i + 1;
                    }
                }
            }

            return featuresList;
        }

        private static Dictionary<string, long> GetRowCounts(Database database)
        {
            IDataTableAccess sqlAccess = DataAccessHelper.GetDataTableAccess(database.ConnectionString);
            Dictionary<string, long> rowCounts = new Dictionary<string, long>();
            foreach (Durados.View view in database.Views.Values.Where(v => !v.SystemView))
            {
                rowCounts.Add(view.Name, sqlAccess.RowCount(view));
            }
            return rowCounts;
        }

        private static Dictionary<string, object> addViewsParameters = null;

        private static void InitAddViewsParameters()
        {
            addViewsParameters = new Dictionary<string, object>();
            addViewsParameters.Add("ViewsInMenu", Convert.ToInt32(System.Configuration.ConfigurationManager.AppSettings["AddViewsParameters_ViewsInMenu"] ?? "20"));
            addViewsParameters.Add("MenuPrefix", System.Configuration.ConfigurationManager.AppSettings["AddViewsParameters_MenuPrefix"] ?? "Menu");
            addViewsParameters.Add("NonEmptyViewsRatioLimit", Convert.ToDouble(System.Configuration.ConfigurationManager.AppSettings["AddViewsParameters_NonEmptyViewsRatioLimit"] ?? "0.5"));
            addViewsParameters.Add("MainInterestsCount", System.Configuration.ConfigurationManager.AppSettings["AddViewsParameters_MainInterestsCount"] ?? "0,0;1,1;20,2;200,3;10000000,4");

            
        }

        private static void SetSettingsMenus(Durados.View[] views, Durados.Database configDatabase, Database database)
        {
            SetSettingsMenus(views, (int)addViewsParameters["ViewsInMenu"], configDatabase, database);
        }

        private static void SetSettingsMenus(Durados.View[] views, int itemsCount, Durados.Database configDatabase, Database database)
        {

            string workspaceId = database.GetPublicWorkspaceId().ToString();
            
            






            View configView = (View)configDatabase.Views["View"];
            View menuView = (View)configDatabase.Views["Menu"];
            ConfigAccess configAccess = new ConfigAccess();
            string filename = configDatabase.ConnectionString;
            int i = 0;
            string menuPrefix = (string)addViewsParameters["MenuPrefix"];
            int count = 0;
            string menuName = string.Empty;
            int ordinal = 100;
            string menuId = null;

            string parentMenu = null;
            foreach (View view in database.Views.Values.Where(v=>!v.SystemView && !views.Contains(v)))
            {
                if (i % itemsCount == 0)
                {
                    count++;
                    ordinal += 10;
                    menuName = menuPrefix + count;

                    parentMenu = SetTopMenu(menuName, string.Empty, configDatabase, string.Empty, workspaceId, LinkType.View, ordinal);

                    //menuId = CreateSettingsMenu(menuView, configAccess, menuName, ordinal);
                }
                string url = "/" + view.Controller + "/" + view.IndexAction + "/" + view.Name;
                SetViewWorkspace(view, configAccess, configView, configDatabase.ConnectionString, workspaceId);

                SetChildMenu(view.Name, parentMenu, view.Name, configDatabase, url, workspaceId, LinkType.View, ordinal);
                
                //SetSettingsMenu(view, configAccess, configView, filename, menuId, database);
                i++;
            }
        }

        private static string CreateSettingsMenu(View view, ConfigAccess configAccess, string name, int ordinal)
        {
            
            Dictionary<string, object> values = new Dictionary<string, object>();
            values.Add("Name", name);
            values.Add("Ordinal", ordinal.ToString());
            DataRow dr = view.Create(values, null, null, null, null, null);

            return dr["ID"].ToString();
        }

        private static void SetViewWorkspace(View view, ConfigAccess configAccess, View configView, string filename, string workspaceID)
        {
            string configViewPk = configAccess.GetViewPK(view.Name, filename);
            Dictionary<string, object> values = new Dictionary<string, object>();
            values.Add("WorkspaceID", workspaceID);
            configAccess.Edit(configView, values, configViewPk, null, null, null, null);

        }

        private static void SetSettingsMenu(View view, ConfigAccess configAccess, View configView, string filename, string menuId, Database database)
        {
            string configViewPk = configAccess.GetViewPK(view.Name, filename);
            Dictionary<string, object> values = new Dictionary<string, object>();
            values.Add("Menu_Parent", menuId);
            values.Add("WorkspaceID", database.GetPublicWorkspaceId().ToString());
            configAccess.Edit(configView, values, configViewPk, null, null, null, null);

        }

        private static Durados.View[] SetMainInterestsMenu(Dictionary<string, long> rowCounts, Durados.Database configDatabase, Database database)
        {
            Durados.View[] views = GetMainInterests(rowCounts, database).ToArray();
            int i = 1;
            string workspaceId = database.GetPublicWorkspaceId().ToString();
            ConfigAccess configAccess = new ConfigAccess();
            View configView = (View)configDatabase.Views["View"];
            
            foreach (View view in views)
            {
                string url = "/" + view.Controller + "/" + view.IndexAction + "/" + view.Name;
                SetTopMenu(view.Name, view.Name, configDatabase, url, workspaceId, LinkType.View, i++);
                SetViewWorkspace(view, configAccess, configView, configDatabase.ConnectionString, workspaceId);
            }
            return views;
        }

        private static string SetTopMenu(string menuName, string viewName, Durados.Database configDatabase, string url, string workspaceId, LinkType linkType, int ordinal)
        {
            return SetSpecialMenu(menuName, null, viewName, configDatabase, url, workspaceId, linkType, ordinal);
        }

        private static string SetSpecialMenu(string menuName, string parentMenu, string viewName, Durados.Database configDatabase, string url, string workspaceId, LinkType linkType, int ordinal)
        {
            View view = (View)configDatabase.Views["SpecialMenu"];
            
            Dictionary<string, object> values = new Dictionary<string, object>();
            values.Add("WorkspaceID", workspaceId);
            if (string.IsNullOrEmpty(parentMenu))
            {
                values.Add("SpecialMenus_Parent", workspaceId);
            }
            else
            {
                values.Add("Menus_Parent", parentMenu);
            }
            values.Add("Name", menuName);
            values.Add("ViewName", viewName);
            values.Add("Url", url);
            values.Add("Ordinal", ordinal);
            values.Add("LinkType", linkType.ToString());


            DataRow row = view.Create(values, null, null, null, null, null);

            string id = view.GetPkValue(row);
            return id;
        }

        private static string SetChildMenu(string menuName, string parentMenu, string viewName, Durados.Database configDatabase, string url, string workspaceId, LinkType linkType, int ordinal)
        {
            return SetSpecialMenu(menuName, parentMenu, viewName, configDatabase, url, workspaceId, linkType, ordinal);

        }

        private static void SetMainInterestMenu(View view, ConfigAccess configAccess, View configView, string filename, Database database)
        {
            string configViewPk = configAccess.GetViewPK(view.Name, filename);
            Dictionary<string, object> values = new Dictionary<string, object>();
            values.Add("Menu_Parent", string.Empty);
            values.Add("WorkspaceID", database.GetPublicWorkspaceId().ToString());
            configAccess.Edit(configView, values, configViewPk, null, null, null, null);

        }


        private static Durados.View[] GetMainInterests(Dictionary<string, long> rowCounts, Database database)
        {
            int mainInterestsCount = GetMainInterestsCount(database);


            if (rowCounts == null)
            {
                return database.Views.Values.Where(v => !v.SystemView).OrderByDescending(v => v.ParentCount).Take(mainInterestsCount).ToArray();
            }
            else
            {
                double totalViews = rowCounts.Count;
                double totalviewsMoreThan0 = rowCounts.Values.Where(i => i > 0).Count();

                double nonEmptyViewsRatioLimit = (double)addViewsParameters["NonEmptyViewsRatioLimit"];

                if (totalviewsMoreThan0 / totalViews > nonEmptyViewsRatioLimit)
                {
                    return database.Views.Values.Where(v => !v.SystemView && rowCounts[v.Name] > 0).OrderByDescending(v => v.ParentCount).Take(mainInterestsCount).ToArray();
                }
                else
                {
                    return database.Views.Values.Where(v => !v.SystemView).OrderByDescending(v => v.ParentCount).Take(mainInterestsCount).ToArray();
                }
            }
        }

        private static int GetMainInterestsCount(Database database)
        {
            SortedList<int, int> mainInterestsCountRange = GetMainInterestsCountRange();

            int viewCount = database.Views.Count;

            int prevRange = -1;
            foreach (int range in mainInterestsCountRange.Keys)
            {
                if (viewCount > prevRange && viewCount <= range)
                    return mainInterestsCountRange[range];

                prevRange = range;
            }

            return 0;
        }

        private static SortedList<int, int> GetMainInterestsCountRange()
        {
            SortedList<int, int> mainInterestsCountRange = new SortedList<int, int>();

            string mainInterestsCountString = (string)addViewsParameters["MainInterestsCount"];

            string[] mainInterestsCount1 = mainInterestsCountString.Split(';');

            foreach (string s in mainInterestsCount1)
            {
                string[] mainInterestsCount2 = s.Split(',');
                if (mainInterestsCount2.Length == 2)
                {
                    int viewCount = Convert.ToInt32(mainInterestsCount2[0]);
                    int interestCount = Convert.ToInt32(mainInterestsCount2[1]);

                    mainInterestsCountRange.Add(viewCount, interestCount);
                }
            }

            //mainInterestsCountRange.Add(0, 0);
            //mainInterestsCountRange.Add(1, 1);
            //mainInterestsCountRange.Add(20, 2);
            //mainInterestsCountRange.Add(200, 3);
            //mainInterestsCountRange.Add(int.MaxValue, 4);

            return mainInterestsCountRange;
        }

        public static string GetEncryptedUrl(this Database database, string url)
        {
            string key = database.Guid.ToString();
            if (url.IndexOf(Database.UserPlaceHolder, StringComparison.OrdinalIgnoreCase) >= 0)
            {
                string userId = database.GetUserID();
                string encryptedUserId = CryptorHelper.EncryptUrl(userId, true, key);

                url = url.Replace(Database.UserPlaceHolder, encryptedUserId,false);
            }
            if (url.IndexOf(Database.UsernamePlaceHolder, StringComparison.OrdinalIgnoreCase) >= 0)
            {
                string username = string.Empty;
                if (System.Web.HttpContext.Current.User != null && System.Web.HttpContext.Current.User.Identity != null && System.Web.HttpContext.Current.User.Identity.Name != null)
                    username = System.Web.HttpContext.Current.User.Identity.Name;
                string encryptedUsername = CryptorHelper.EncryptUrl(username, true, key);

                //string decrptedUsername = CryptorHelper.DecryptUrl(encryptedUsername, true, key);

                url = url.Replace(Database.UsernamePlaceHolder, encryptedUsername,false);
            }
            if (url.IndexOf(Database.RolePlaceHolder, StringComparison.OrdinalIgnoreCase) >= 0)
            {
                string role = database.GetUserRole();
                string encryptedRole = CryptorHelper.EncryptUrl(role, true, key);

                url = url.Replace(Database.RolePlaceHolder, encryptedRole,false);
            }

            return url; 
        }
        public static List<Dictionary<object, object>> GetTableForJson(this  DataView dv, View view, int rowCount, int pageNumber)
        {

            List<Dictionary<object, object>> rows = new List<Dictionary<object, object>>();
            Dictionary<object, object> row = null;
            int drNum = 1;
            foreach (DataRow dr in dv.Table.Rows)
            {
                row = new Dictionary<object, object>();
                foreach (Field field in view.VisibleFieldsForTable)
                {
                    try
                    {
                        row.Add(field.DisplayName, dr[field.GetColumnsNames()[0]]);
                    }
                    catch (Exception ex)
                    {
                        throw new DuradosException(string.Format("Error on {0}, row number {1}, Column {2}", view.Name, drNum, field.DisplayName), ex);
                    }
                }
                rows.Add(row);
                drNum++;
            }

            return rows;
        }

        public static string GetJson(this View view)
        {
            string pk = null;
            IDataTableAccess sqlAccess = DataAccessHelper.GetDataTableAccess(view.ConnectionString);
            if (Map.Database.Views.ContainsKey(view.Name))
                pk = sqlAccess.GetFirstPK(Map.Database.Views[view.Name]);
            else
                return null;
            DataRow row = view.GetDataRow(pk);

           DatasetToJson datasetToJson = new DatasetToJson();

           return datasetToJson.GetJson(view);
        }
    }

    public class DatasetToJson
    {
        const string Space = "&nbsp;";
        const int SpaceNum = 3;

        public string GetJson(View view)
        {
            IDataTableAccess sqlAccess = DataAccessHelper.GetDataTableAccess(view.ConnectionString);
            string pk  = sqlAccess.GetFirstPK(view); 
             DataRow row = view.GetDataRow(pk);

            StringBuilder sb = new StringBuilder();
            GetJson(view, row, new HashSet<string>(), sb, 0);

            return sb.ToString();
        }

        private string GetJson(View view, DataRow row, string spaces)
        {
            string s = spaces + "{<br/>";

            foreach (ColumnField f in view.Fields.Values.Where(f => f.FieldType == FieldType.Column))
            {

                s += spaces + f.Name + ":";
                if (f.DataType == DataType.Numeric)
                    s += f.GetValue(row);
                else
                    s += "\"" + f.GetValue(row) + "\"";

                s += ",</br>";
            
            }

            return s.TrimEnd(",</br>".ToArray());
        }

        private void GetJson(View view, DataRow row, HashSet<string> pks, StringBuilder sb, int indent)
        {
            string spaces = string.Concat(Enumerable.Repeat(Space, indent * SpaceNum));
            string pk = view.Name + view.GetPkValue(row);

            indent += 1;

            sb.Append(GetJson(view, row, spaces));

            if (pks.Contains(pk))
            {
                sb.Append(spaces + "</br>" + spaces + "}");
                return;
            }

            pks.Add(pk);
            var childrenFields = view.Fields.Values.Where(f => f.FieldType == FieldType.Children && ((ChildrenField)f).LoadForBlockTemplate);
            foreach (ChildrenField field in childrenFields)
            {
                sb.Append(spaces + ",</br>");
                sb.Append(spaces + field.Name);
                sb.Append(":</br>" + spaces + "[</br>");
                
                DataTable table = field.GetDataView(row).Table;
                View childrenView = (View)field.ChildrenView;
                int i = 0;
                foreach(DataRow childRow in table.Rows)
                {

                    GetJson(childrenView, childRow, pks, sb, indent);
                    if (i < 1)
                        sb.Append(",</br>");
                    if (i == 1)
                        break;
                    i++;
                }
                sb.Append("</br>" + spaces + "]");
                
            }

            string parentSpaces = string.Concat(Enumerable.Repeat(Space, indent * SpaceNum));
           
            foreach (ParentField field in view.Fields.Values.Where(f => f.FieldType == FieldType.Parent))
            {
                DataRow parentRow = row.GetParentRow(field.DataRelation.RelationName);
                View parentView = (View)field.ParentView;
                if (parentRow == null)
                {
                    string key = field.GetValue(row);
                    if (!string.IsNullOrEmpty(key))
                        parentRow = parentView.GetDataRow(key, row.Table.DataSet);
                }
                if (parentRow != null)
                {


                    sb.Append(spaces + ",</br>");
                    sb.Append(spaces + field.Name);
                    sb.Append(":</br>");
                    GetJson(parentView, parentRow, pks, sb, indent);
                    //sb.Append("</br>" + parentSpaces + "}");
           

                }
            
            }

            sb.Append(spaces + "</br>" + spaces + "}");
           
        }
    }

    [Serializable]
    public class d_Field
    {
        public string Name;
        public string Value;

    }

    public enum ReferenceType
    {
        JavaScript,
        StyleSheet
    }
}
