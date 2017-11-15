using Durados.DataAccess;
using Durados.Web.Mvc.SocialLogin;
using Microsoft.WindowsAzure;
using Microsoft.WindowsAzure.StorageClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Durados.Data;
using System.Runtime.Caching;

namespace Durados.Web.Mvc.UI.Helpers
{
    public static class RestHelper
    {
        //private static IRest rest = null;

        private static IRest GetRest(View view)
        {
            return new Rest();
        }

        public static void Refresh(string appname)
        {
            string fileName = Maps.Instance.GetMap(appname).GetConfigDatabase().ConnectionString;
            Maps.Instance.Restart(appname);
            Durados.DataAccess.ConfigAccess.Restart(fileName);

            if (Workflow.Engine.CurrentDatabases != null && Workflow.Engine.CurrentDatabases.Contains(appname))
            {
                try
                {
                    Workflow.Engine.CurrentDatabases.Remove(appname);
                }
                catch { }
            }

            string blobName = Maps.GetStorageBlobName(fileName);

            if (Maps.Instance.StorageCache.ContainsKey(blobName))
            {
                Maps.Instance.StorageCache.Remove(blobName);

                blobName += "xml";

                if (Maps.Instance.StorageCache.ContainsKey(blobName))
                {
                    Maps.Instance.StorageCache.Remove(blobName);
                }
            }
        }

        public static void AddStat(Dictionary<string, object> item, string appName, bool reset)
        {
            Stat stat = StatFactory.GetState(Maps.Instance.GetMap(appName).SystemSqlProduct);

            stat.AddStat(item, appName, reset);
        }

        public static Dictionary<string, Dictionary<string, object>> ReferenceTableToDictionary(View view, DataView dataView, bool deep = false, bool descriptive = true, bool hideMetadata = false)
        {
            return new DictionaryConverter().ReferenceTableToDictionary(view, dataView, deep, descriptive, hideMetadata);
        }


        public static Dictionary<string, object>[] TableToDictionary(View view, DataView dataView, bool deep = false, bool descriptive = true, bool relatedObjects = false, bool hideMetadata = false)
        {
            return new DictionaryConverter().TableToDictionary(view, dataView, deep, descriptive, relatedObjects, 3, hideMetadata);
        }

        public static Dictionary<string, object> RowToDictionary(View view, DataRow row, string pk, bool deep, bool displayParentValue = false, int level = 3, bool hideMetadata = false)
        {
            return GetDictionaryConverter(view).RowToDictionary(view, row, pk, deep, displayParentValue, level, hideMetadata);

        }

        private static DictionaryConverter GetDictionaryConverter(View view)
        {
            if (view.Database.IsConfig && view.Name == "View")
                return new ViewDictionaryConverter();
            else if (view.Database.IsConfig && view.Name == "MyCharts")
                return new DashboardDictionaryConverter();
            else if (view.Database.IsConfig && view.Name == "Chart")
                return new ChartDictionaryConverter();
            else if (view.Database.IsConfig && view.Name == "Database")
                return new DatabaseDictionaryConverter();
            else if (view.Database.IsConfig && view.Name == "Page")
                return new ContentDictionaryConverter();
            else if (view.Database.IsConfig && view.Name == "Rule")
                return new RuleDictionaryConverter();

            return new DictionaryConverter();
        }


        public static string Create(this View view, string json, bool deep, BeforeCreateEventHandler beforeCreateCallback, BeforeCreateInDatabaseEventHandler beforeCreateInDatabaseEventHandler, AfterCreateEventHandler afterCreateBeforeCommitCallback, AfterCreateEventHandler afterCreateAfterCommitCallback)
        {
            return Create(view, Deserialize(view, json), deep, beforeCreateCallback, beforeCreateInDatabaseEventHandler, afterCreateBeforeCommitCallback, afterCreateAfterCommitCallback);
        }
        public static string Create(this View view, Dictionary<string, object> values, bool deep, BeforeCreateEventHandler beforeCreateCallback, BeforeCreateInDatabaseEventHandler beforeCreateInDatabaseEventHandler, AfterCreateEventHandler afterCreateBeforeCommitCallback, AfterCreateEventHandler afterCreateAfterCommitCallback)
        {
            return GetRest(view).Create(view, values, deep, beforeCreateCallback, beforeCreateInDatabaseEventHandler, afterCreateBeforeCommitCallback, afterCreateAfterCommitCallback);
        }

        public static string Create(this View view, Dictionary<string, object>[] values, bool deep, BeforeCreateEventHandler beforeCreateCallback, BeforeCreateInDatabaseEventHandler beforeCreateInDatabaseEventHandler, AfterCreateEventHandler afterCreateBeforeCommitCallback, AfterCreateEventHandler afterCreateAfterCommitCallback, bool clearCache = false, IDbCommand command = null, IDbCommand sysCommand = null)
        {
            if (clearCache)
            {
                ClearCache();
            }
            return GetRest(view).Create(view, values, deep, beforeCreateCallback, beforeCreateInDatabaseEventHandler, afterCreateBeforeCommitCallback, afterCreateAfterCommitCallback, command, sysCommand);
        }

        const string RestDataCache = "RestDataCache";
        private static void ClearCache()
        {
            try
            {
                Map map = Maps.Instance.GetMap();
                //if (map.AllKindOfCache.ContainsKey(RestDataCache))
                //{
                //    ((Dictionary<string, object>)map.AllKindOfCache[RestDataCache]).Clear();
                //}
                if (map.AllKindOfCache.Contains(RestDataCache))
                {
                    map.AllKindOfCache[RestDataCache] = new MemoryCache(RestDataCache);
                }
            }
            catch { }
        }

        public static void Update(this View view, string json, string pk, bool deep, BeforeEditEventHandler beforeEditCallback, BeforeEditInDatabaseEventHandler beforeEditInDatabaseCallback, AfterEditEventHandler afteEditBeforeCommitCallback, AfterEditEventHandler afterEditAfterCommitCallback)
        {
            Update(view, Deserialize(view, json), pk, deep, beforeEditCallback, beforeEditInDatabaseCallback, afteEditBeforeCommitCallback, afterEditAfterCommitCallback);
        }

        public static int Update(this View view, Dictionary<string, object> values, string pk, bool deep, BeforeEditEventHandler beforeEditCallback, BeforeEditInDatabaseEventHandler beforeEditInDatabaseCallback, AfterEditEventHandler afteEditBeforeCommitCallback, AfterEditEventHandler afterEditAfterCommitCallback, BeforeCreateEventHandler beforeCreateCallback = null, BeforeCreateInDatabaseEventHandler beforeCreateInDatabaseCallback = null, AfterCreateEventHandler afterCreateBeforeCommitCallback = null, AfterCreateEventHandler afterCreateAfterCommitCallback = null, bool overwrite = false, BeforeDeleteEventHandler beforeDeleteCallback = null, AfterDeleteEventHandler afterDeleteBeforeCommitCallback = null, AfterDeleteEventHandler afterDeleteAfterCommitCallback = null, bool clearCache = false, IDbCommand command = null, IDbCommand sysCommand = null)
        {
            if (clearCache)
            {
                ClearCache();
            }

            return GetRest(view).Update(view, values, pk, deep, beforeEditCallback, beforeEditInDatabaseCallback, afteEditBeforeCommitCallback, afterEditAfterCommitCallback, beforeCreateCallback, beforeCreateInDatabaseCallback, afterCreateBeforeCommitCallback, afterCreateAfterCommitCallback, overwrite, beforeDeleteCallback, afterDeleteBeforeCommitCallback, afterDeleteAfterCommitCallback, command, sysCommand);
        }

        public static Dictionary<string, object> Deserialize(this View view, string json)
        {
            Dictionary<string, object> values = Json.JsonSerializer.Deserialize(json);

            foreach (Field field in view.Fields.Values.Where(f => !f.Excluded))
            {
                if (!field.IsAutoIdentity && !field.IsGuidIdentity && field.Required && values.ContainsKey(field.JsonName) && (values[field.JsonName] == null || string.IsNullOrEmpty(values[field.JsonName].ToString())))
                {
                    throw new DuradosException("The value can not be NULL in the field '" + field.JsonName + "'");
                }
            }

            // temporary ignore datetime
            //foreach (ColumnField field in view.Fields.Values.Where(f => f.FieldType == FieldType.Column))
            //{
            //    if (field.IsDateTime)
            //    {
            //        if (values.ContainsKey(field.JsonName))
            //        {
            //            values.Remove(field.JsonName);
            //        }
            //    }
            //}

            //System.Web.Script.Serialization.JavaScriptSerializer javaScriptSerializer = new System.Web.Script.Serialization.JavaScriptSerializer();
            foreach (ColumnField field in view.Fields.Values.Where(f => f.FieldType == FieldType.Column))
            {
                if (field.DataColumn.DataType.Equals(typeof(DateTime)) || field.DataColumn.DataType.Equals(typeof(DateTimeOffset)))
                {
                    if (values.ContainsKey(field.JsonName) && values[field.JsonName] != null && !string.IsNullOrEmpty(values[field.JsonName].ToString()))
                    {
                        try
                        {
                            values[field.JsonName] = DateTime.Parse(values[field.JsonName].ToString()); //javaScriptSerializer.Deserialize<DateTime>(values[field.JsonName].ToString()).ToString("MM/dd/yyyy hh:mm:ss tt");//DateTime.ParseExact(values[field.JsonName].ToString(), field.Format, System.Globalization.CultureInfo.InvariantCulture).ToString("MM/dd/yyyy hh:mm:ss tt");
                        }
                        catch (Exception exception)
                        {
                            throw new DuradosException("Invalid json date", exception);
                        }
                    }
                }
            }



            return values;
        }

        public static int Delete(this View view, string pk, bool deep, BeforeDeleteEventHandler beforeDeleteCallback, AfterDeleteEventHandler afteDeleteBeforeCommitCallback, AfterDeleteEventHandler afterDeleteAfterCommitCallback, Dictionary<string, object> values = null, bool clearCache = false, IDbCommand command = null, IDbCommand sysCommand = null)
        {
            if (clearCache)
            {
                ClearCache();
            }
            return GetRest(view).Delete(view, pk, deep, beforeDeleteCallback, afteDeleteBeforeCommitCallback, afterDeleteAfterCommitCallback, command, sysCommand, values);
        }

        public static object Get(this Query query, int page, int pageSize, Dictionary<string, object> values, bool dataSeries = true)
        {
            string sql = query.SQL.Replace(Durados.Workflow.Engine.AsToken(values));

            string currentUsetId = query.Database.GetUserID() ?? string.Empty;
            string currentUsername = query.Database.GetCurrentUsername() ?? string.Empty;
            string currentUserRole = ((Database)query.Database).GetUserRole() ?? string.Empty;

            sql = sql.Replace(Durados.Database.UserPlaceHolder, currentUsetId, false).Replace(Durados.Database.SysUserPlaceHolder.AsToken(), currentUsetId, false)
                  .Replace(Durados.Database.UsernamePlaceHolder, currentUsername, false).Replace(Durados.Database.SysUsernamePlaceHolder.AsToken(), currentUsername)
                  .Replace(Durados.Database.RolePlaceHolder, currentUserRole, false).Replace(Durados.Database.SysRolePlaceHolder.AsToken(), currentUserRole);

            //sql = sql.ReplaceGlobals(query.Database);
            sql = sql.ReplaceConfig(query.Database);

            SqlAccess sqlAccess = GetSqlAccess(query.Database.SqlProduct);

            DataTable table = sqlAccess.ExecuteTable(query.Database.ConnectionString, sql, null, CommandType.Text);

            return table.TableToJson(dataSeries);
        }


        private static string GetSelectStatementWithPaging(Durados.Database database, string sql, int page, int pageSize)
        {
            ISqlTextBuilder sqlTextBuilder = database.GetSqlTextBuilder();
            return "SELECT " + sqlTextBuilder.AllColumns + " FROM (" + sql + sqlTextBuilder.GetRowNumber("{1}") + " FROM " + sqlTextBuilder.EscapeDbObject("{0}") + sqlTextBuilder.WithNolock + sqlTextBuilder.GetPageOrder("{1}") + " ) " + sqlTextBuilder.DbAs + sqlTextBuilder.EscapeDbObject("{0}") + " WHERE 1=1 " + sqlTextBuilder.GetPage(page, pageSize);
        }
        public static object TableToJson(this DataTable table, bool dataSeries = true)
        {
            List<object> list = new List<object>();


            if (dataSeries)
            {
                foreach (DataRow row in table.Rows)
                {
                    list.Add(row.ItemArray);
                }
            }
            else
            {
                foreach (DataRow row in table.Rows)
                {
                    Dictionary<string, object> jsonRow = new Dictionary<string, object>();
                    foreach (DataColumn column in table.Columns)
                    {
                        jsonRow.Add(column.ColumnName, row.IsNull(column) ? null : row[column]);
                    }
                    list.Add(jsonRow);
                }
            }


            return list;
        }

        private static string GetRestDataCacheKey()
        {
            return System.Web.HttpContext.Current.Request.Url.PathAndQuery;
        }

        public static object Get(this View view, bool withSelectOptions, bool withFilterOptions, int page, int pageSize, Dictionary<string, object>[] filter, string search, Dictionary<string, object>[] sort, out int rowCount, bool deep, BeforeSelectEventHandler beforeSelectCallback, AfterSelectEventHandler afterSelectCallback, bool returnDataView = false, bool descriptive = true, bool useCache = false, bool relatedObjects = false, string where = null, bool hideMetadata = false, bool hideTotalRows = false, string[] fields = null)
        {
            if (useCache)
            {
                object cachedObject = null;
                if (ExistsInCache(out cachedObject))
                {
                    rowCount = 0;
                    return cachedObject;
                }
            }
            Dictionary<string, object> values = null;
            if (filter != null)
            {
                values = (new FilterAdapter()).ReplaceFilterOperands(view, filter);
            }

            Dictionary<string, SortDirection> sortColumns = null;
            if (sort != null)
            {
                sortColumns = (new SortAdapter()).GetSortColumns(view, sort);
            }

            bool isSearch = !string.IsNullOrEmpty(search);
            if (isSearch)
            {
                if (values == null)
                    values = new Dictionary<string, object>();
                AddSearch(view, values, search);
            }

            if (fields != null && fields.Length > 0 && System.Web.HttpContext.Current != null)
            {
                HashSet<string> fieldsHash = new HashSet<string>(fields);
                System.Web.HttpContext.Current.Items.Add("fieldsHash", fieldsHash);
            }

            DataView dataView = view.FillPage(page, pageSize, values, isSearch, sortColumns, out rowCount, beforeSelectCallback, afterSelectCallback);
            if (returnDataView)
                return dataView;

            Dictionary<string, object> json = new Dictionary<string, object>();
            if (!hideTotalRows)
                json.Add("totalRows", rowCount);
            if (!relatedObjects)
                relatedObjects = deep;
            json.Add("data", TableToDictionary(view, dataView, deep, descriptive, relatedObjects, hideMetadata));

            if (relatedObjects)
            {
                Dictionary<string, Dictionary<string, Dictionary<string, object>>> referenceTables = new Dictionary<string, Dictionary<string, Dictionary<string, object>>>();
                foreach (DataTable table in dataView.Table.DataSet.Tables)
                {
                    if (table.TableName != dataView.Table.TableName && table.Rows.Count > 0)
                    {
                        View referenceView = (View)view.Database.Views[table.TableName];
                        referenceTables.Add(referenceView.JsonName, ReferenceTableToDictionary(referenceView, new DataView(table), deep, descriptive, hideMetadata));
                    }
                }
                json.Add("relatedObjects", referenceTables);
            }

            if (withSelectOptions)
            {
                json.Add("selectOptions", GetSelectOptions(view));
            }
            if (withFilterOptions)
            {
                json.Add("filterOptions", GetFilterOptions(view));
            }

            if (useCache)
            {
                SetInCache(json);
            }

            return json;

        }

        private static void SetInCache(object json)
        {
            try
            {
                Map map = Maps.Instance.GetMap();
                if (!map.AllKindOfCache.Contains(RestDataCache))
                {
                    map.AllKindOfCache[RestDataCache] = new MemoryCache(RestDataCache);
                }
                //if (!map.AllKindOfCache.ContainsKey(RestDataCache))
                //{
                //    map.AllKindOfCache.Add(RestDataCache, new Dictionary<string, object>());
                //}

                MemoryCache restDataCache = (MemoryCache)map.AllKindOfCache[RestDataCache];
                restDataCache[GetRestDataCacheKey()] = json;
            }
            catch { }

        }

        private static bool ExistsInCache(out object cachedObject)
        {
            cachedObject = null;

            try
            {

                Map map = Maps.Instance.GetMap();
                //if (map.AllKindOfCache.ContainsKey(RestDataCache))
                //{
                //    Dictionary<string, object> restDataCacheDictionary = (Dictionary<string, object>)map.AllKindOfCache[RestDataCache];
                //    if (restDataCacheDictionary.ContainsKey(GetRestDataCacheKey()))
                //    {
                //        cachedObject = restDataCacheDictionary[GetRestDataCacheKey()];
                //        return true;
                //    }
                //}
                if (map.AllKindOfCache.Contains(RestDataCache))
                {
                    MemoryCache restDataCacheDictionary = (MemoryCache)map.AllKindOfCache[RestDataCache];
                    string restDataCacheKey = GetRestDataCacheKey();
                    if (restDataCacheDictionary.Contains(restDataCacheKey))
                    {
                        cachedObject = restDataCacheDictionary[restDataCacheKey];
                        return true;
                    }
                }
                return false;
            }
            catch
            {
                return false;
            }
        }

        private static void AddSearch(View view, Dictionary<string, object> values, string search)
        {
            foreach (Field field in view.Fields.Values)
            {
                if (field.FieldType == FieldType.Column && (field.GetColumnFieldType() == ColumnFieldType.String || field.GetColumnFieldType() == ColumnFieldType.DateTime))
                {
                    if (!values.ContainsKey(field.Name))
                    {
                        values.Add(field.Name, search);
                    }
                }
            }
        }

        public static object GetFilterOptions(this View view)
        {
            DictionaryConverter dictionaryConverter = new Helpers.DictionaryConverter();

            List<object> filterOptionsList = new List<object>();
            foreach (Field field in view.VisibleFieldsForFilter)
            {
                string fieldName = dictionaryConverter.GetName(field);
                string label = field.DisplayName;
                string operand = FilterOperandType.equals.ToString();
                string fieldType = null;

                if (field.FieldType == FieldType.Column)
                {
                    ColumnField columnField = (ColumnField)field;

                    switch (columnField.GetColumnFieldType())
                    {
                        case ColumnFieldType.Boolean:
                            fieldType = "boolean";
                            break;

                        case ColumnFieldType.DateTime:
                            fieldType = "date";
                            break;

                        case ColumnFieldType.Integer:
                        case ColumnFieldType.Real:
                            fieldType = "numeric";
                            break;

                        default:
                            operand = columnField.View.Database.InsideTextSearch ? FilterOperandType.contains.ToString() : FilterOperandType.startsWith.ToString();
                            fieldType = "text";

                            break;
                    }
                    filterOptionsList.Add(new { fieldName = fieldName, label = label, fieldType = fieldType, @operator = operand, value = string.Empty });
                }
                else if (field.FieldType == FieldType.Parent)
                {
                    operand = "in";
                    fieldType = "relation";

                    var selectOptions = ((ParentField)field).GetSelectOptions(view, string.Empty, null, true);
                    var options = new List<object>();
                    options.Add(new { name = string.Empty, value = string.Empty });
                    foreach (string key in selectOptions.Keys)
                    {
                        options.Add(new { name = selectOptions[key], value = key });
                    }
                    filterOptionsList.Add(new { fieldName = fieldName, label = label, fieldType = fieldType, @operator = operand, value = string.Empty, selectOptions = options });
                }
            }


            return filterOptionsList;
        }

        public static object GetSelectOptions(this View view)
        {
            DictionaryConverter dictionaryConverter = new Helpers.DictionaryConverter();

            Dictionary<string, object> selectOptionsList = new Dictionary<string, object>();
            foreach (ParentField field in view.Fields.Values.Where(f => f.FieldType == FieldType.Parent && (f.DisplayFormat == DisplayFormat.DropDown || f.DisplayFormat == DisplayFormat.None)))
            {
                var selectOptions = field.GetSelectOptions(view, string.Empty, null, false);
                var options = new List<object>();
                options.Add(new { name = string.Empty, value = string.Empty });
                foreach (string key in selectOptions.Keys)
                {
                    options.Add(new { name = selectOptions[key], value = key });
                }
                if (!selectOptionsList.ContainsKey(dictionaryConverter.GetName(field)))
                {
                    selectOptionsList.Add(dictionaryConverter.GetName(field), options);
                }
            }
            foreach (ChildrenField field in view.Fields.Values.Where(f => f.FieldType == FieldType.Children && f.IsCheckList()))
            {
                var selectOptions = field.GetSelectList(null, false);
                var options = new List<object>();
                foreach (System.Web.Mvc.SelectListItem item in selectOptions)
                {
                    options.Add(new { name = item.Text, value = item.Value });
                }
                if (!selectOptionsList.ContainsKey(dictionaryConverter.GetName(field)))
                {
                    selectOptionsList.Add(dictionaryConverter.GetName(field), options);
                }
            }

            return selectOptionsList;

        }

        public static object GetSelectOptions(this Field field)
        {
            DictionaryConverter dictionaryConverter = new Helpers.DictionaryConverter();

            var options = new List<object>();
            if (field.FieldType == FieldType.Parent)
            {
                var selectOptions = ((ParentField)field).GetSelectOptions((View)field.View, string.Empty, null, false);
                options.Add(new { name = string.Empty, value = string.Empty });
                foreach (string key in selectOptions.Keys)
                {
                    options.Add(new { name = selectOptions[key], value = key });
                }

            }
            else if (field.FieldType == FieldType.Children && field.IsCheckList())
            {
                var selectOptions = ((ChildrenField)field).GetSelectList(null, false);
                foreach (System.Web.Mvc.SelectListItem item in selectOptions)
                {
                    options.Add(new { name = item.Text, value = item.Value });
                }

            }

            return options;

        }

        public static Dictionary<string, object> Get(this View view, string pk, bool deep, BeforeSelectEventHandler beforeSelectCallback, AfterSelectEventHandler afterSelectCallback, bool displayParentValue = false, bool ignoreConfig = false, bool useCache = false, int level = 3, bool hideMetadata = false, string[] fields = null)
        {
            try
            {
                if (useCache && !view.Database.Map.HasAuthApp)
                {
                    object cachedObject = null;
                    if (ExistsInCache(out cachedObject))
                    {
                        return (Dictionary<string, object>)cachedObject;
                    }
                }
            }
            catch { }
            if (view.Database.IsConfig && !ignoreConfig)
            {
                return GetConfig(view, pk, deep, beforeSelectCallback, afterSelectCallback, displayParentValue);
            }

            if (fields != null && fields.Length > 0 && System.Web.HttpContext.Current != null)
            {
                HashSet<string> fieldsHash = new HashSet<string>(fields);
                System.Web.HttpContext.Current.Items.Add("fieldsHash", fieldsHash);
            }

            var map = view.Database.Map;
            map.Logger.Log(view.Name, DateTime.Now.Ticks.ToString(), "before get row", null, 5, pk);

            DataRow dataRow = view.GetDataRow(pk, null, beforeSelectCallback, afterSelectCallback, true);

            map.Logger.Log(view.Name, DateTime.Now.Ticks.ToString(), "after get row", null, 5, pk);

            if (dataRow == null)
                return null;

            map.Logger.Log(view.Name, pk, "before row to json", null, 5, null);

            Dictionary<string, object> dic = null;
            try
            {
                dic = RowToDictionary(view, dataRow, pk, deep, false, level, hideMetadata);
            }
            catch (NoLongerChecklistException exception)
            {
                exception.ChildrenField.ChildrenHtmlControlType = ChildrenHtmlControlType.Grid;
                dic = RowToDictionary(view, dataRow, pk, deep, false, level, hideMetadata);
            }

            map.Logger.Log(view.Name, pk, "after row to json", null, 5, null);

            if (useCache)
            {
                SetInCache(dic);
            }

            return dic;
        }

        public static string GetConnectionSource(string appName)
        {
            try
            {
                if (Maps.Instance.AppInCach(appName))
                {
                    Map map = Maps.Instance.GetMap(appName);
                    if (map == null || map is DuradosMap)
                    {
                        return "unknown";
                    }
                    else
                    {
                        //string localDatabaseHost = GetLocalDatabaseHost();
                        if (map.HostedByUs)//(map.connectionString.Contains(localDatabaseHost))
                        {
                            return "local";
                        }
                        else
                        {
                            return "external";
                        }
                    }
                }
                else
                {
                    return "unknown";
                }
            }
            catch
            {
                return "unknown";
            }
        }

        ////public static string GetLocalDatabaseHost()
        ////{

        ////    return (System.Configuration.ConfigurationManager.AppSettings["localDatabaseHost"] ?? "yrv-dev.czvbzzd4kpof.eu-central-1.rds.amazonaws.com").ToString();
        ////}

        private static Dictionary<string, object> GetConfig(View view, string pk, bool deep, BeforeSelectEventHandler beforeSelectCallback, AfterSelectEventHandler afterSelectCallback, bool displayParentValue = false)
        {
            var map = view.Database.Map;
            string name = GetCacheName(view, pk);

            if (!map.JsonConfigCache.ContainsKey(name))
            {
                map.JsonConfigCache.Set(name, Get(view, pk, deep, beforeSelectCallback, afterSelectCallback, displayParentValue, true));
            }

            return map.JsonConfigCache.Get(name);
        }

        private static string GetCacheName(View view, string pk)
        {
            string role = view.Database.Map.Database.GetUserRole();

            return view.Name + "_" + pk + "_" + role;
        }

        public static Dictionary<string, object> GetApp(this View view, Workspace workspace, Database database, bool ignoreCache)
        {
            var map = view.Database.Map;
            string name = GetCacheName(view, workspace.ID.ToString());

            if (!map.JsonConfigCache.ContainsKey(name))
            {
                map.JsonConfigCache.Set(name, GetApp(view, workspace, database));
            }

            Dictionary<string, object> app = map.JsonConfigCache.Get(name);
            Dictionary<string, object> user = GetUser(database);
            if (app.ContainsKey("user"))
            {
                app.Remove("user");
            }
            app.Add("user", user);

            return app;
        }

        public static Dictionary<string, object> GetApp(this View view, Workspace workspace, Database database)
        {
            const string Workspaces = "workspace";
            Dictionary<string, object> dictionary = Get(view, "0", false, null, null, false, true);

            Dictionary<string, object> workspaceDictionary = GetWorkspace(workspace);

            dictionary.Add(Workspaces, workspaceDictionary);

            Dictionary<string, object>[] pages = GetPages(workspace, database);

            workspaceDictionary.Add("pages", pages.ToArray());

            Dictionary<string, object> user = GetUser(database);

            dictionary.Add("user", user);

            Dictionary<string, object> company = GetCompany(database);
            string title = GetTitle(database);

            dictionary.Add("company", company);
            dictionary.Add("title", title);
            dictionary.Add("additionalWorkspaces", GetAdditionalWorkspacesNames(workspace, database));

            return dictionary;
        }

        private static List<object> GetAdditionalWorkspacesNames(Workspace currentWorkspace, Database database)
        {
            List<Durados.Workspace> workspaces = GetWorkspaces(database);
            List<object> additionalWorkspacesNames = new List<object>();

            foreach (Workspace additionalWorkspace in workspaces.Where(w => w.ID != database.GetAdminWorkspaceId()).OrderBy(w => w.Ordinal))
            {
                if (!additionalWorkspace.Equals(currentWorkspace))
                {
                    additionalWorkspacesNames.Add(new { id = additionalWorkspace.ID, name = additionalWorkspace.Name });
                }
            }

            return additionalWorkspacesNames;
        }

        private static List<Durados.Workspace> GetWorkspaces(Database database)
        {
            return database.Workspaces.Values.Where(w => !SecurityHelper.IsDenied(w.GetDenySelectRoles(database), w.GetAllowSelectRoles(database))).OrderBy(w => w.Ordinal).ToList();
        }

        private static string GetTitle(Database database)
        {
            return database.SiteInfo.Product;
        }

        private static Dictionary<string, object> GetCompany(Database database)
        {
            Dictionary<string, object> company = new Dictionary<string, object>();

            company.Add("title", database.SiteInfo.Product);
            company.Add("logo", GetLogoUrl(database.Map.GetLogoSrc()));
            company.Add("url", database.SiteInfo.LogoHref);

            return company;
        }

        //public static string GetAppUrl(string appname, bool ishttp = false)
        //{
        //    string host = System.Configuration.ConfigurationManager.AppSettings["durados_host"];
        //    string port = System.Configuration.ConfigurationManager.AppSettings["durados_port"];
        //    string http = "https://";
        //    if (Maps.Debug || ishttp)
        //        http = "http://";

        //    string url = http + appname + "." + host + (string.IsNullOrEmpty(port) ? string.Empty : ":" + port);
        //    return url;
        //}

        public static string GetRemoteAdminUrl(string appname, bool ishttp = false)
        {
            string http = "https://";
            if (Maps.Debug || ishttp)
                http = "http://";

            string remoteAdminUrl = http + appname + "." + System.Configuration.ConfigurationManager.AppSettings["remoteAdminUrl"];
            return remoteAdminUrl;
        }


        public static string GetCurrentUsername()
        {
            return (System.Web.HttpContext.Current.Items[Database.Username] ?? "").ToString();
        }

        private static string GetLogoUrl(string url)
        {
            string defaulLogo = string.Empty;
            try
            {
                System.Collections.Specialized.NameValueCollection query = System.Web.HttpUtility.ParseQueryString(url.Replace("&amp;", "&"));

                string filename = query["fileName"];
                string pk = query["pk"];

                string imageFieldName = "Image";
                try
                {
                    defaulLogo = Maps.Instance.DuradosMap.Database.Views["durados_App"].Fields[imageFieldName].DefaultValue.ToString();
                }
                catch { }
                //if (filename == null || filename.EndsWith(defaulLogo) || filename.ToLower().EndsWith("backand.png") || filename.ToLower().EndsWith("modubiz.png"))
                //{
                //    return GetAppUrl("www") + "/Content/Images/" + defaulLogo;
                //}
                string folder = Maps.AzureAppPrefix + pk;
                return string.Format(Maps.AzureStorageUrl + "/{2}", Maps.AzureStorageAccountName, folder, filename);
            }
            catch
            {
                //try
                //{
                //    return GetAppUrl("www") + "/Content/Images/" + defaulLogo;
                //}
                //catch
                //{
                //    return string.Empty;
                //}
                return string.Empty;
            }
        }

        public static Dictionary<string, object> GetUser(Database database)
        {
            Dictionary<string, object> user = new Dictionary<string, object>();

            string username = database.GetCurrentUsername();
            user.Add("username", username);
            user.Add("fullName", database.GetUserFullName2(username));
            user.Add("role", database.GetUserRole2(username));
            user.Add("appName", database.Map.AppName);

            return user;
        }

        private static Dictionary<string, object> GetWorkspace(Workspace workspace)
        {
            Dictionary<string, object> workspaceDictionary = new Dictionary<string, object>();
            workspaceDictionary.Add("id", workspace.ID);
            workspaceDictionary.Add("name", workspace.Name);
            workspaceDictionary.Add("homePage", workspace.HomePage);

            return workspaceDictionary;
        }

        private static Dictionary<string, object>[] GetPages(Workspace workspace, Database database, IEnumerable<Menu> menus)
        {
            List<Dictionary<string, object>> pages = new List<Dictionary<string, object>>();


            foreach (SpecialMenu menu in menus)
            {
                if (database.IsAllowMenu(menu))
                {
                    Dictionary<string, object> page = GetPage(workspace, database, menu);
                    pages.Add(page);
                }
            }

            return pages.ToArray();
        }

        private static Dictionary<string, object>[] GetPages(Workspace workspace, Database database)
        {
            return GetPages(workspace, database, workspace.SpecialMenus.Values.Where(m => !m.HideFromMenu).OrderBy(m => m.Ordinal));
        }

        private static string GetDashboardId(Database database, string url)
        {
            string id = null;

            string[] split1 = url.Split("?".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
            if (split1.Length == 2)
            {
                string[] split2 = split1[1].Split("=".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
                if (split2.Length == 2)
                {
                    id = split2[1];
                }
            }

            return id;
        }

        private static string GetViewJsonName(Database database, string viewName)
        {
            if (string.IsNullOrEmpty(viewName) || !database.Views.ContainsKey(viewName))
                return viewName;
            View view = (View)database.Views[viewName];
            return view.JsonName;
        }

        private static Dictionary<string, object> GetPage(Workspace workspace, Database database, SpecialMenu menu)
        {
            Dictionary<string, object> page = new Dictionary<string, object>();

            page.Add("id", menu.ID);
            page.Add("name", menu.Name);
            page.Add("partType", LinkTypeString(menu.LinkType));
            if (menu.LinkType == LinkType.MyCharts)
            {
                page.Add("partId", GetDashboardId(database, menu.Url));
            }
            else
            {
                page.Add("partId", GetViewJsonName(database, menu.ViewName));
            }
            IEnumerable<Menu> menus = menu.Menus.Values.Where(m => database.IsAllowMenu(m) && !m.HideFromMenu);
            if (menus.Count() > 0 && database.MenuType == MenuType.PullDown)
            {
                page.Add("pages", GetPages(workspace, database, menus));
            }

            return page;
        }

        private static string LinkTypeString(LinkType linkType)
        {
            switch (linkType)
            {
                case LinkType.MyCharts:
                    return "dashboard";
                case LinkType.View:
                    return "table";
                default:
                    return "content";
            }
        }


        public static bool IsLoginFailureException(this Database database, Exception exception)
        {
            SqlAccess sqlAccess = GetSqlAccess(database.SqlProduct);
            bool failure = sqlAccess.IsLoginFailureException(exception);

            if (failure)
            {
                return !sqlAccess.TestConnection(database.ConnectionString, database.SqlProduct);
            }

            return failure;
        }

        public static bool TestConnection(this Database database)
        {
            SqlAccess sqlAccess = GetSqlAccess(database.SqlProduct);
            bool success = sqlAccess.TestConnection(database.ConnectionString, database.SqlProduct);
            return success;
        }


        public static SqlAccess GetSqlAccess(SqlProduct sqlProduct)
        {
            SqlAccess sqlAccess = null;

            switch (sqlProduct)
            {
                case SqlProduct.MySql:
                    sqlAccess = new MySqlAccess();
                    break;
                case SqlProduct.Postgre:
                    sqlAccess = new PostgreAccess();
                    break;
                case SqlProduct.Oracle:
                    sqlAccess = new OracleAccess();
                    break;

                default:
                    sqlAccess = new SqlAccess();
                    break;
            }

            return sqlAccess;
        }

        public static Dictionary<string, object> GetDbStat(Database database)
        {
            SqlProduct? sqlProduct = database.SqlProduct;

            if (sqlProduct == SqlProduct.Oracle)
                return null;

            SqlAccess sqlAccess = GetSqlAccess(sqlProduct.Value);
            SqlSchema schema = sqlAccess.GetNewSqlSchema();

            string sql = "select Count(*) from (" + schema.GetTableNamesSelectStatement() + ") db_tables";

            string scalar = sqlAccess.ExecuteScalar(database.ConnectionString, sql);

            int tableCount = Convert.ToInt32(scalar);

            return new Dictionary<string, object>() { { "tableCount", tableCount } };

        }

        #region Keys

        public static object GetUserKey(string username)
        {
            Map map = Maps.Instance.GetMap();
            if (map == null || map is DuradosMap)
            {
                throw new DuradosException("App not found");
            }

            string role = map.Database.GetUserRole(map.Database.GetCurrentUsername());
            if (!(role == "Admin" || role == "Developer"))
                throw new DuradosException("Only admin can get keys");

            return map.Database.GetUserRow(username)["Guid"].ToString();

        }

        public static object ResetUserKey(string username, BeforeEditInDatabaseEventHandler view_BeforeEditInDatabase, Map map = null)
        {
            if (map == null)
                map = Maps.Instance.GetMap();
            if (map == null || map is DuradosMap)
            {
                throw new DuradosException("App not found");
            }

            string guid = Guid.NewGuid().ToString();
            int pk = map.Database.GetUserID(username);
            if (pk == -1)
            {
                throw new UserNotFoundException(username);
            }
            map.Database.GetUserView().Edit(new Dictionary<string, object>() { { "Guid", guid } }, pk.ToString(), null, view_BeforeEditInDatabase, null, null);

            RefreshToken.Clear();
            return guid;
        }

        public static object GetKeys(string appName)
        {
            Map map = Maps.Instance.GetMap(appName);

            if (map == null || map is DuradosMap)
            {
                throw new DuradosException("App not found");
            }

            string role = map.Database.GetUserRole(map.Database.GetCurrentUsername());

            if (string.IsNullOrEmpty(role))
            {
                int? appId = Maps.Instance.AppExists(appName);

                if (!appId.HasValue)
                {
                    throw new AppNotFoundException(appName);
                }
                else if (Maps.Instance.GetOnBoardingStatus(appId.Value.ToString()) != OnBoardingStatus.Ready)
                {
                    throw new AppNotReadyException(appName);
                }
                else
                {
                    throw new DuradosException("Unexpected error when get app keys");
                }
            }

            if (!(role == "Admin" || role == "Developer"))
                throw new DuradosException("Only admin can get keys. your role is " + role);

            string sql = Maps.MainAppSchema.GetAppRowByNameSql(appName);

            Dictionary<string, object> keys = new Dictionary<string, object>();
            using (IDbConnection cnn = Maps.MainAppSchema.GetNewConnection(Maps.Instance.ConnectionString))
            {
                using (IDbCommand command = cnn.CreateCommand())
                {
                    command.CommandText = sql;
                    cnn.Open();
                    using (IDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            string general = reader["Guid"].ToString();
                            keys.Add("general", general);
                            string signup = reader["SignUpToken"].ToString();
                            keys.Add("signup", signup);
                            string anonymousToken = reader["AnonymousToken"].ToString();
                            keys.Add("anonymous", anonymousToken);
                        }
                    }
                }
            }

            return keys;
        }

        public static object ResetKey(string appName, string key)
        {
            Map map = Maps.Instance.GetMap(appName);
            if (map == null || map is DuradosMap)
            {
                throw new DuradosException("App not found");
            }

            string role = map.Database.GetUserRole(map.Database.GetCurrentUsername());
            if (!(role == "Admin" || role == "Developer"))
                throw new DuradosException("Only admin can get keys");


            string columnName = null;
            if (key == "general")
            {
                columnName = "Guid";
            }
            else if (key == "signup")
            {
                columnName = "SignUpToken";
            }
            else if (key == "anonymous")
            {
                columnName = "AnonymousToken";
            }
            else
            {
                throw new DuradosException("Key not found");
            }


            string val = null;

            using (System.Data.SqlClient.SqlConnection cnn = new System.Data.SqlClient.SqlConnection(Maps.Instance.DuradosMap.Database.ConnectionString))
            {
                using (System.Data.SqlClient.SqlCommand command = new System.Data.SqlClient.SqlCommand("", cnn))
                {
                    cnn.Open();

                    command.CommandText = string.Format("update [durados_app] set [{1}] = @newGuid  WHERE [Name] = '{0}'", appName, columnName);
                    command.Parameters.AddWithValue("newGuid", Guid.NewGuid());
                    command.ExecuteNonQuery();

                    command.Parameters.Clear();
                    command.CommandText = string.Format("SELECT * FROM [durados_app] WITH(NOLOCK)  WHERE [Name] = '{0}'", appName);
                    using (System.Data.SqlClient.SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            val = reader[columnName].ToString();

                        }
                    }
                }
            }

            return val;
        }

        #endregion

        public static Durados.DataAccess.Filter GetFilter(View view, Dictionary<string, object>[] filter, string childrenFieldName)
        {
            SqlAccess sql = GetSqlAccess(view.Database.SqlProduct);

            return sql.GetInFilter(view, (new FilterAdapter()).ReplaceFilterOperands(view, filter), childrenFieldName);
        }
    }

    public class ContentDictionaryConverter : DictionaryConverter
    {
        private PageType GetPageType(DataRow dataRow)
        {
            if (dataRow.IsNull("PageType"))
                return PageType.Content;

            return (PageType)Enum.Parse(typeof(PageType), dataRow["PageType"].ToString());
        }

        Dictionary<string, PageType> pageTypeFields = new Dictionary<string, PageType>() { { "Content", PageType.Content }, { "ExternalNewPage", PageType.External }, { "Target", PageType.External }, { "NewTab", PageType.External }, { "ExternalPage", PageType.IFrame }, { "Scroll", PageType.IFrame }, { "Width", PageType.IFrame }, { "Height", PageType.IFrame }, { "ReportName", PageType.ReportingServices }, { "ReportDisplayName", PageType.ReportingServices } };
        protected override bool IsJsonable(Field field, DataRow dataRow)
        {
            PageType pageType = GetPageType(dataRow);

            if (field.Name == "PageType")
            {
                return true;
            }
            else if (pageTypeFields.ContainsKey(field.Name))
            {
                return (pageTypeFields[field.Name].Equals(pageType));
            }
            else
            {
                return false;
            }
        }

        protected override object GetColumnValue(View view, DataRow dataRow, ColumnField columnField)
        {
            object val = base.GetColumnValue(view, dataRow, columnField);
            if (columnField.GetHtmlControlType() == HtmlControlType.Url)
            {
                if (val != null && !val.Equals(string.Empty))
                {
                    string[] segments = val.ToString().Split('|');
                    if (segments.Length == 3)
                        return segments[2];
                }
            }
            return val;
        }
    }

    public class DashboardDictionaryConverter : ChartDictionaryConverter
    {
        protected override bool IsJsonable(Field field, DataRow dataRow)
        {
            if (field.Name == "Dashboards_Parent")
                return false;

            if (field.FieldType == FieldType.Children && ((ChildrenField)field).ChildrenView.Name == "Chart" && dataRow.Table.TableName == "Chart")
            {
                if ((dataRow.IsNull("SQL")) || string.IsNullOrEmpty(dataRow["SQL"].ToString()) || dataRow["ChartType"].ToString().ToLower() == "gauge")
                    return false;
                else
                {
                    Chart chart = GetRealChart(field, dataRow);
                    if (chart == null)
                        return false;
                    return chart.IsVisible(((Database)field.View.Database).Map.Database.GetUserRole());
                }
            }



            return base.IsJsonable(field, dataRow);


        }

        private Chart GetRealChart(Field field, DataRow dataRow)
        {
            int chartId = (int)dataRow["ID"];
            int? dashboardId = ((Database)field.View.Database).Map.Database.GetDashboardPK(chartId);

            if (dashboardId.HasValue)
            {
                MyCharts dashboard = ((Database)field.View.Database).Map.Database.Dashboards[dashboardId.Value];
                if (!dashboard.Charts.ContainsKey(chartId))
                    return null;
                return dashboard.Charts[chartId];
            }

            return null;
        }


    }

    public class ChartDictionaryConverter : DictionaryConverter
    {
        protected HashSet<string> gaugeFields = new HashSet<string>() { "GaugeGreen", "GaugeYellow", "GaugeRed", "RefreshInterval", "GaugeMinValue", "GaugeMaxValue" };
        protected HashSet<string> chartExcludeFieldsFromJson = new HashSet<string>(StringComparer.InvariantCultureIgnoreCase) { "SQL", "WorkspaceID", "Precedent", "AllowSelectRoles" };
        protected override bool IsJsonable(Field field, DataRow dataRow)
        {
            if (field.View.Name == "Chart" && !dataRow.IsNull("ChartType") && !dataRow["ChartType"].ToString().Equals(ChartType.Gauge.ToString()) && gaugeFields.Contains(field.Name))
            {
                return false;
            }
            if (field.View.Name == "Chart" && chartExcludeFieldsFromJson.Contains(field.Name))
            {
                return false;
            }

            return true;
        }

        public override string GetName(Field field)
        {
            string name = base.GetName(field);

            if (name.ToLower() == "height(pixels)")
                return "height";

            return name;
        }
    }

    public class DatabaseDictionaryConverter : DictionaryConverter
    {
        // && !appExcludeFieldsFromJson.Contains(field.Name);
        HashSet<string> appExcludeFieldsFromJson = new HashSet<string>() {
            "AdminEmail","DefaultAllowCreateRoles","DefaultAllowDeleteRoles","DefaultAllowEditRoles","DefaultAllowSelectRoles","DateFormat","DefaultGuestRole","DefaultLast","DefaultPageSize",
            "InsideTextSearch","DefaultWorkspaceId","DefaultDenyCreateRoles","DefaultDenyEditRoles","DefaultDenyDeleteRoles","DefaultDenySelectRoles","DoLocalizeAdmin","SsrsDomain",
            "StyleSheets","HideMyStuff","IsMultiLanguages","SsrsPassword","SsrsPath","GrobootNotificationAccessKey","SecureLevel","SsrsReportServerUrl","RequiresSSL","UserGuidFieldName",
            "SsrsUsername","ViewOwnerRoles"
        };

        HashSet<string> authAppFields = new HashSet<string>() {
            "GoogleClientId","GoogleClientSecret","TwitterClientId","TwitterClientSecret","GithubClientId","GithubClientSecret","FacebookClientId","FacebookClientSecret","FacebookScope",
            "EnableGithub","EnableGoogle","EnableTwitter","EnableFacebook",
            "EnableAdfs","EnableAzureAd",
            "AdfsClientId","AdfsResource","AdfsHost","AzureAdClientId","AzureAdResource","AzureAdHost",
            "NewUserDefaultRole",
            "EnableUserRegistration",
            "SignupEmailVerification",
            "TokenExpiration"
        };

        HashSet<string> appExcludeFieldsForAdminFromJson = new HashSet<string>();
        protected override bool IsJsonable(Field field, DataRow dataRow)
        {
            if (field.View.Name == "Database")
            {
                if (IsAdmin(((Database)field.View.Database).Map.Database))
                {
                    return base.IsJsonable(field, dataRow) && !appExcludeFieldsForAdminFromJson.Contains(field.Name);
                }
                else
                {
                    return base.IsJsonable(field, dataRow) && !appExcludeFieldsFromJson.Contains(field.Name);
                }
            }

            return base.IsJsonable(field, dataRow);
        }

        protected override object GetColumnValue(View view, DataRow dataRow, ColumnField columnField)
        {
            string name = columnField.Name;
            if (HasAuthApp(view) && authAppFields.Contains(name))
            {
                return GetAuthValue(view, name);
            }
            return base.GetColumnValue(view, dataRow, columnField);
        }

        private bool HasAuthApp(View view)
        {
            return ((View)view).Database.Map.HasAuthApp;
        }
        

        private object GetAuthValue(View view, string name)
        {
            return ((View)view).Database.Map.Database[name];
        }
    }

    public class RuleDictionaryConverter : DictionaryConverter
    {
        //protected override bool IsJsonable(Field field, DataRow dataRow)
        //{
        //    return !field.HideInTable;
        //}

        protected override void AddToDictionary(Dictionary<string, object> dictionary, Field field, object value)
        {
            if (GetName(field) == "viewTable" && value is Dictionary<string, object>)
            {
                base.AddToDictionary(dictionary, field, ((Dictionary<string, object>)((Dictionary<string, object>)value)["__metadata"])["id"]);
            }
            else
            {
                base.AddToDictionary(dictionary, field, value);
            }
        }

        public override string GetName(Field field)
        {
            if (field.View.Name == "Rule" && field.View.GetFieldsByDisplayName(field.DisplayName).Length > 1)
            {
                string name = field.Name;
                return name.ReplaceNonAlphaNumeric2(string.Empty).LowerFirstChar();

            }
            else
            {
                return base.GetName(field);
            }
        }
    }

    public class ViewDictionaryConverter : DictionaryConverter
    {
        protected override object GetColumnValue(View view, DataRow dataRow, ColumnField columnField)
        {
            if (!view.Database.IsConfig)
                return base.GetColumnValue(view, dataRow, columnField);

            object value = base.GetColumnValue(view, dataRow, columnField);

            if (IsView(view))
            {
                if (columnField.Name == "AllowCreate")
                {
                    View realView = GetViewFromViewRow(view, dataRow);

                    bool isCreatable = realView.IsCreatable();

                    return isCreatable;
                }
                else if (columnField.Name == "AllowEdit")
                {
                    View realView = GetViewFromViewRow(view, dataRow);

                    return realView.IsEditable();
                }
                else if (columnField.Name == "AllowDelete")
                {
                    View realView = GetViewFromViewRow(view, dataRow);

                    return realView.IsDeletable();
                }
                else if (columnField.Name == "HideInMenu")
                {
                    View realView = GetViewFromViewRow(view, dataRow);

                    return realView.IsAllow();
                }
                else if (columnField.Name == "DisplayColumn")
                {
                    View realView = GetViewFromViewRow(view, dataRow);

                    if (realView.DisplayField == null)
                        return value;
                    return realView.DisplayField.JsonName;
                }
            }
            else if (IsField(view))
            {
                if (columnField.Name == "DataType")
                {
                    Field realField = GetFieldFromFieldRow(view, dataRow);
                    if (realField.IsPoint)
                    {
                        return "Point";
                    }
                    return value;
                }
                if (columnField.Name == "HideInCreate")
                {
                    Field realField = GetFieldFromFieldRow(view, dataRow);

                    return !realField.IsVisibleForCreate();
                }
                if (columnField.Name == "ExcludeInInsert" || columnField.Name == "ExcludeInUpdate")
                {
                    Field realField = GetFieldFromFieldRow(view, dataRow);
                    if (realField.FieldType == FieldType.Column)
                        if (((ColumnField)realField).DataColumn.AutoIncrement || ((ColumnField)realField).IsAutoGuid)
                            return true;
                }
                if (columnField.Name == "Required")
                {
                    Field realField = GetFieldFromFieldRow(view, dataRow);
                    return realField.Required;
                }
                if (columnField.Name == "Unique")
                {
                    Field realField = GetFieldFromFieldRow(view, dataRow);
                    return realField.Unique;
                }
                else if (columnField.Name == "HideInEdit")
                {
                    Field realField = GetFieldFromFieldRow(view, dataRow);

                    return !realField.IsVisibleForEdit();
                }
                else if (columnField.Name == "HideInTable")
                {
                    Field realField = GetFieldFromFieldRow(view, dataRow);

                    return !realField.IsVisibleForTable();
                }

                else if (columnField.Name == "DisplayFormat")
                {
                    Field realField = GetFieldFromFieldRow(view, dataRow);
                    if (realField.FieldType == FieldType.Children)
                        return ((ChildrenField)realField).ChildrenHtmlControlType.ToString();
                }
                else if (columnField.Name == "RelatedViewName")
                {
                    if (value != null && !value.Equals(string.Empty))
                    {
                        Field realField = GetFieldFromFieldRow(view, dataRow);
                        if (realField != null && realField.View.Database.Views.ContainsKey(value.ToString()))
                        {
                            return realField.View.Database.Views[value.ToString()].JsonName;
                        }
                    }
                }
                else if (columnField.Name == "DefaultValue")
                {
                    Field realField = GetFieldFromFieldRow(view, dataRow);
                    if (value != null && !value.Equals(string.Empty))
                    {
                        if (realField.IsNumeric)
                        {
                            try
                            {
                                return Convert.ToDecimal(value);
                            }
                            catch (Exception exception)
                            {
                                view.Database.Map.Logger.Log("RestHelper", "ViewDictionaryConverter", "GetColumnValue", exception, 1, string.Format("The default value for '{0}' should be numeric", realField.JsonName));
                                //throw new DuradosException(string.Format("The default value for '{0}' should be numeric", realField.JsonName), exception);
                            }
                        }
                        if (realField.FieldType == FieldType.Column && realField.IsDate)
                        {
                            try
                            {
                                if (value.ToString().ToLower() != "now")
                                {
                                    return Convert.ToDateTime(value).ToString("u").TrimEnd('Z');
                                }
                            }
                            catch (Exception exception)
                            {
                                view.Database.Map.Logger.Log("RestHelper", "ViewDictionaryConverter", "GetColumnValue", exception, 1, string.Format("The default value for '{0}' should be date", realField.JsonName));
                                //throw new DuradosException(string.Format("The default value for '{0}' should be date", realField.JsonName), exception);
                                return "now";
                            }
                        }
                        else if (realField.GetColumnFieldType() == ColumnFieldType.Boolean)
                        {
                            try
                            {
                                if (value.Equals("0") || value.ToString().ToLower() == "false" || value.ToString().ToLower() == "no")
                                {
                                    return false;
                                }
                                else
                                {
                                    return true;
                                }
                            }
                            catch (Exception exception)
                            {
                                view.Database.Map.Logger.Log("RestHelper", "ViewDictionaryConverter", "GetColumnValue", exception, 1, string.Format("The default value for '{0}' should be boolean: either false or true", realField.JsonName));
                                //throw new DuradosException(string.Format("The default value for '{0}' should be boolean: either false or true", realField.JsonName), exception);
                            }
                        }
                        else if (realField.FieldType == FieldType.Parent && realField.DisplayFormat == DisplayFormat.AutoCompleteMatchAny || realField.DisplayFormat == DisplayFormat.AutoCompleteStratWith)
                        {
                            try
                            {
                                View parentView = (View)((ParentField)realField).ParentView;
                                DataRow row = parentView.GetDataRow(value.ToString());
                                if (row != null)
                                {
                                    string displayValue = parentView.GetDisplayValue(row);
                                    if (displayValue != null)
                                    {
                                        value = new Dictionary<string, object>() { { "label", displayValue }, { "value", value } };
                                    }
                                    else
                                    {
                                        value = string.Empty;
                                    }
                                }
                                else
                                {
                                    value = string.Empty;
                                }
                            }
                            catch
                            {
                                value = string.Empty;
                            }
                        }
                    }
                }

            }

            return value;
        }

        Dictionary<string, string> viewNames = new Dictionary<string, string>() { { "RowHeight", "rowHeightInPixels" }, { "", "" } };
        Dictionary<string, string> fieldNames = new Dictionary<string, string>() { { "", "" } };
        public override string GetName(Field field)
        {
            if (IsField(field.View))
            {
                if (fieldNames.ContainsKey(field.Name))
                {
                    return fieldNames[field.Name];
                }
            }
            else if (IsView(field.View))
            {
                if (viewNames.ContainsKey(field.Name))
                {
                    return viewNames[field.Name];
                }
            }

            return base.GetName(field);
        }
        private bool IsField(Durados.View view)
        {
            return view.Name == "Field";
        }

        private bool IsView(Durados.View view)
        {
            return view.Name == "View";
        }
        protected override void AddToDictionary(Dictionary<string, object> dictionary, Field field, object value)
        {
            if (IsField(field.View) || IsView(field.View))
            {
                if (field.Category == null)
                {
                    base.AddToDictionary(dictionary, field, value);
                }
                else
                {
                    string categoryName = field.Category.Name;
                    if (categoryName == "General" || categoryName == "Organize_Columns")
                    {
                        base.AddToDictionary(dictionary, field, value);
                    }
                    else
                    {
                        categoryName = categoryName.LowerFirstChar().ReplaceNonAlphaNumeric2().Replace("_", "");
                        if (!dictionary.ContainsKey(categoryName))
                        {
                            dictionary.Add(categoryName, new Dictionary<string, object>());
                        }
                        base.AddToDictionary((Dictionary<string, object>)dictionary[categoryName], field, value);
                    }
                }
            }
            else
            {
                base.AddToDictionary(dictionary, field, value);
            }

        }

        private Field GetFieldFromFieldRow(View view, DataRow dataRow)
        {
            string name = dataRow["Name"].ToString();
            DataRow fieldsRow = dataRow.GetParentRow("Fields");
            if (fieldsRow == null)
                return null;
            View realView = GetViewFromViewRow(view, fieldsRow);
            if (realView == null)
                return null;
            if (!realView.Fields.ContainsKey(name))
                return null;
            return realView.Fields[name];
        }

        View viewFromViewRow = null;
        private View GetViewFromViewRow(View view, DataRow dataRow)
        {
            if (viewFromViewRow == null)
                viewFromViewRow = (View)view.Database.Map.Database.Views[dataRow["Name"].ToString()];
            return viewFromViewRow;
        }

        HashSet<string> chartExcludeFieldsFromJson = new HashSet<string>() {
            "SQL","WorkspaceID","Precedent","AllowSelectRoles"
       };

        HashSet<string> viewExcludeFieldsFromJson = new HashSet<string>() {
            "ApplyColorDesignToAllViews","ApplySkinToAllViews","ToolBoxBackground","AlternateRowBackground","RowBackground","BorderColor","SendCc","Layout","Skin",
            "EditableTableName","GridEditable","GridEditableEnabled","HoverBackground","HoverTextColor","NotifyMessage","OpenDialogMax",
            "OpenSingleRow","Send","SendSubject","SystemView","FontColor","TextFontColor","AlterTextColor","SendTo","ViewOwnerRoles",
            "DenyCreateRoles","DenyDeleteRoles","DenyEditRoles","DenySelectRoles"
       };
        HashSet<string> fieldExcludeFieldsFromJson = new HashSet<string>() {
            "AdvancedFilter", "AutoIncrement","AutoIncrementSequanceName","DefaultFilter","Dashboard","Preview","GraphicProperties",
            "Formula","InlineSearch","Name","MultiFilter","Width",
            "ChildrenHtmlControlType","DenyCreateRoles","DenyEditRoles","DenySelectRoles"
       };

        protected override bool IsJsonable(Field field, DataRow dataRow)
        {
            if (!field.View.Database.IsConfig)
                return base.IsJsonable(field, dataRow);

            if (field.FieldType == FieldType.Children && dataRow.Table.TableName == "Field" && dataRow["Excluded"].Equals(true))
                return false;


            if (field.Name == "Views_Parent")
                return false;
            if (field.View.Name == "Category")
                return false;

            if (field.View.Name == "View")
                return base.IsJsonable(field, dataRow) && !viewExcludeFieldsFromJson.Contains(field.Name);
            else if (field.View.Name == "Field" && dataRow.Table.TableName == "Field")
            {
                if (!IsInFieldType(field.Name, dataRow["FieldType"].ToString()))
                    return false;

                return base.IsJsonable(field, dataRow) && !fieldExcludeFieldsFromJson.Contains(field.Name);
            }

            return base.IsJsonable(field, dataRow);

        }

        private bool IsInFieldType(string fieldName, string fieldType)
        {
            return Maps.Instance.FieldProperty.IsInType(fieldName, fieldType);
        }

        protected override void AddAdditionalProperties(View view, DataRow dataRow, bool withChildren, Dictionary<string, object> pks, Dictionary<string, object> dictionary)
        {
            base.AddAdditionalProperties(view, dataRow, withChildren, pks, dictionary);
            if (!view.Database.IsConfig)
                return;

            if (IsField(view) && dataRow["FieldType"].Equals(FieldType.Children.ToString()))
            {
                ChildrenField realField = (ChildrenField)GetFieldFromFieldRow(view, dataRow);
                ParentField relatedParentField = (ParentField)realField.GetEquivalentParentField();
                if (relatedParentField != null)
                {
                    dictionary.Add("relatedParentFieldName", GetName(relatedParentField));
                }


            }
            if (IsField(view))
            {
                Field realField = GetFieldFromFieldRow(view, dataRow);
                if (dictionary.ContainsKey("category"))
                    dictionary.Remove("category");
                if (dictionary.ContainsKey("categoryName"))
                    dictionary.Remove("categoryName");
                string categoryName = string.Empty;
                if (realField.Category == null)
                {
                    if (realField.FieldType == FieldType.Children && realField.IsSubGrid())
                    {
                        categoryName = ((ChildrenField)realField).ChildrenView.DisplayName;
                    }
                }
                else
                {
                    categoryName = realField.Category.Name;
                }
                dictionary.Add("categoryName", categoryName);
                if (realField.FieldType == FieldType.Column && ((ColumnField)realField).FtpUpload != null)
                {
                    dictionary.Add("urlPrefix", ((ColumnField)realField).FtpUpload.DirectoryVirtualPath);

                }
            }


            if (IsView(view))
            {
                View realView = GetViewFromViewRow(view, dataRow);


                List<Dictionary<string, object>> categories = new List<Dictionary<string, object>>();
                //public int ColumnsInDialog { get; set; }

                foreach (Category category in realView.Categories.Values.OrderBy(c => c.Ordinal))
                {
                    Dictionary<string, object> categoryDictionary = new Dictionary<string, object>();
                    categoryDictionary.Add("name", category.Name);
                    if (category.Fields.Count() == category.Fields.Where(f => f.FieldType == FieldType.Children).Count())
                    {
                        categoryDictionary.Add("columnsInDialog", 1);
                    }
                    else
                    {
                        categoryDictionary.Add("columnsInDialog", realView.GetColumnsInDialog(category));
                    }
                    categories.Add(categoryDictionary);
                }

                dictionary.Add("categories", categories.ToArray());
            }

            if (IsField(view))
            {
                AddColumnWidth(view, dataRow, dictionary);
            }
        }

        Json.CustomView cv = null;
        bool cvLoaded = false;

        private void AddColumnWidth(View view, DataRow dataRow, Dictionary<string, object> dictionary)
        {
            Field realField = GetFieldFromFieldRow(view, dataRow);
            if (realField == null)
                return;

            if (cv == null && !cvLoaded)
            {
                cvLoaded = true;
                cv = ViewHelper.GetDesrializedCustomView(realField.View.Name, true);
            }

            if (cv == null)
                return;

            if (cv.Fields.ContainsKey(realField.Name))
            {
                dictionary.Add("columnWidth", cv.Fields[realField.Name].width);
            }
        }

        //protected override List<Field> GetFields(View view)
        //{
        //    if (view.Name == "View" || view.Name == "Field")
        //        return view.Fields.Values.Where(f => f.IsAllowSelect()).ToList();
        //    return base.GetFields(view);
        //}
    }

    public class DictionaryConverter
    {
        protected virtual bool IsAdmin(Database database)
        {
            string role = null;
            try
            {
                if (System.Web.HttpContext.Current.Items["role"] != null)
                {
                    role = System.Web.HttpContext.Current.Items["role"].ToString();
                }
            }
            catch { }
            if (role == null)
            {
                role = database.GetUserRole(database.GetCurrentUsername());
                try
                {
                    System.Web.HttpContext.Current.Items["role"] = role;
                }
                catch { }
            }


            return role == "Admin" || role == "Developer";

        }

        public Dictionary<string, object>[] TableToDictionary(View view, DataView dataView, bool deep = false, bool descriptive = true, bool relatedObjects = false, int level = 3, bool hideMetadata = false)
        {
            List<Dictionary<string, object>> list = new List<Dictionary<string, object>>();

            Durados.Web.Mvc.UI.TableViewer tableViewer = new Durados.Web.Mvc.UI.TableViewer();
            tableViewer.DataView = dataView;

            //System.Data.DataTable table = tableViewer.GetDataView(dataView, view, string.Empty).Table;

            foreach (System.Data.DataRowView row in dataView)
            {
                if (deep && !relatedObjects)
                {
                    list.Add(RowToDictionary(view, row.Row, view.GetPkValue(row.Row), deep, false, level, hideMetadata));
                }
                else
                {
                    list.Add(RowToDictionary(view, dataView, tableViewer, row, deep, descriptive, hideMetadata));
                }
            }
            return list.ToArray();
        }

        public Dictionary<string, Dictionary<string, object>> ReferenceTableToDictionary(View view, DataView dataView, bool deep = false, bool descriptive = true, bool hideMetadata = false)
        {
            Dictionary<string, Dictionary<string, object>> dictionary = new Dictionary<string, Dictionary<string, object>>();

            Durados.Web.Mvc.UI.TableViewer tableViewer = new Durados.Web.Mvc.UI.TableViewer();
            tableViewer.DataView = dataView;

            //System.Data.DataTable table = tableViewer.GetDataView(dataView, view, string.Empty).Table;

            foreach (System.Data.DataRowView row in dataView)
            {
                dictionary.Add(view.GetPkValue(row.Row), RowToDictionary(view, dataView, tableViewer, row, deep, descriptive, hideMetadata));
            }
            return dictionary;
        }

        private Dictionary<string, object> RowToDictionary(View view, DataView dataView, Durados.Web.Mvc.UI.TableViewer tableViewer, System.Data.DataRowView row, bool deep = false, bool descriptive = true, bool hideMetadata = false)
        {
            Dictionary<string, object> dictionary = new Dictionary<string, object>();
            dictionary.Add(Database.__metadata, GetRowMetadata(view, row.Row, tableViewer, descriptive, hideMetadata));

            foreach (Durados.Field field in view.VisibleFieldsForTableFirstRow)
            {
                object value = null;
                try
                {
                    //if (deep)
                    //{
                    if (field.FieldType == FieldType.Children && !field.IsCheckList())
                    {
                        value = null;
                    }
                    //continue;

                    else if (field.FieldType == FieldType.Column && (field.IsDate || field.IsBoolean || field.IsNumeric))
                        value = row.Row[((ColumnField)field).DataColumn.ColumnName];
                    else if (field.FieldType == FieldType.Children && field.IsCheckList())
                        value = tableViewer.GetFieldValue(field, row.Row);
                    else if (field.FieldType == FieldType.Column && field.IsPoint)
                        value = GetPoint(field, row.Row);
                    else
                        value = System.Web.HttpContext.Current.Server.HtmlDecode(field.GetValue(row.Row));
                    //}
                    //else
                    //{
                    //    value = System.Web.HttpContext.Current.Server.HtmlDecode(tableViewer.GetDisplayValues(field, row.Row));
                    //}
                    //value = tableViewer.GetDisplayValues(field, row.Row);
                }
                catch (Exception exception)
                {
                    value = GetErrorValue(exception);
                }
                string name = GetName(field);
                if (!dictionary.ContainsKey(name))
                    AddFieldValue(field, row.Row, name, value, dictionary);
                //dictionary.Add(name, value);
            }

            AddSpecialValues(dictionary, view, row.Row, dataView);

            return dictionary;
        }

        private object GetPoint(Field field, DataRow dataRow)
        {
            try
            {
                string s = field.GetValue(dataRow);
                if (string.IsNullOrEmpty(s))
                    return null;
                string[] a = s.Split(",".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
                return new double[2] { Convert.ToDouble(a[0]), Convert.ToDouble(a[1]) };
            }
            catch (Exception exception)
            {
                throw new DuradosException("Fail to retrieve point value", exception);
            }
        }

        private void AddSpecialValues(Dictionary<string, object> dictionary, View view, System.Data.DataRow row, DataView dataView)
        {
            if (view == view.Database.GetUserView())
            {
                AddUserStatus(dictionary, view, row, dataView);
            }
        }

        private void AddUserStatus(Dictionary<string, object> dictionary, View view, System.Data.DataRow row, DataView dataView)
        {
            if (row == null)
                return;

            Map map = Maps.Instance.GetMap();

            var provider = (map is DuradosMap) ? System.Web.Security.Membership.Provider : map.GetMembershipProvider();

            string username = row["Username"].ToString();

            System.Web.Security.MembershipUser existingUser = provider.GetUser(username, false);
            
            Durados.Web.Mvc.Controllers.AccountMembershipService accountMembershipService = new Durados.Web.Mvc.Controllers.AccountMembershipService();
            bool valid = accountMembershipService.ValidateUser(username);

            if (map.HasAuthApp)
            {
                bool readyToSignin = existingUser != null || valid;

                dictionary.Add("readyToSignin", readyToSignin);
                if (dictionary.ContainsKey("IsApproved"))
                {
                    if (existingUser == null)
                    {
                        dictionary["IsApproved"] = false;
                    }
                }
            }
            else
            {
                bool readyToSignin = existingUser == null || valid;

                dictionary.Add("readyToSignin", readyToSignin);
            }
        }

        protected virtual void AddFieldValue(Field field, DataRow row, string name, object value, Dictionary<string, object> dictionary)
        {
            if (!HandleHistory(field, row, name, value, dictionary))
                dictionary.Add(name, value);
        }

        protected bool HandleHistory(Field field, DataRow row, string name, object value, Dictionary<string, object> dictionary)
        {
            try
            {
                if (field.View.Name == "durados_v_ChangeHistory")
                {
                    if (field.Name == "FieldName")
                    {
                        if (field.View.Database.Views.ContainsKey(row["ViewName"].ToString()))
                        {
                            Durados.View realView = field.View.Database.Views[row["ViewName"].ToString()];
                            if (realView.Fields.ContainsKey(value.ToString()))
                            {
                                value = GetName(realView.Fields[value.ToString()]);
                                dictionary.Add(name, value);
                                return true;
                            }
                        }
                    }
                    else if (field.Name == "ViewName")
                    {
                        if (field.View.Database.Views.ContainsKey(value.ToString()))
                        {
                            value = field.View.Database.Views[value.ToString()].JsonName;
                            dictionary.Add(name, value);
                            return true;
                        }
                    }
                }
            }
            catch { }
            return false;
        }

        private Dictionary<string, object> GetRowMetadata(View view, DataRow row, Durados.Web.Mvc.UI.TableViewer tableViewer = null, bool descriptive = true, bool hideMetadata = false)
        {

            Dictionary<string, object> metadata = new Dictionary<string, object>();
            metadata.Add("id", view.GetPkValue(row));
            if (!hideMetadata)
            {
                try
                {
                    metadata.Add("fields", GetFieldsTypes(view));
                }
                catch (Exception exception)
                {
                    Maps.Instance.DuradosMap.Logger.Log("RestHelper", "GetRowMetadata", "GetFieldsTypes", exception, 1, null);
                }
                if (!view.Database.IsConfig)
                {
                    if (descriptive)
                    {
                        Dictionary<string, object> descriptives = new Dictionary<string, object>();
                        //foreach (ParentField field in view.Fields.Values.Where(f => f.GetHtmlControlType() == HtmlControlType.Autocomplete))
                        foreach (ParentField field in view.Fields.Values.Where(f => f.FieldType == FieldType.Parent))
                        {
                            string value = field.GetValue(row);
                            string label = string.Empty;
                            if (row.GetParentRow(field.DataRelation.RelationName) == null)
                                label = field.GetDisplayValue(value);
                            else
                                label = field.ConvertToString(row);

                            descriptives.Add(GetName(field), new Dictionary<string, object>() { { "label", label }, { "value", value } });
                        }
                        foreach (ChildrenField field in view.Fields.Values.Where(f => f.FieldType == FieldType.Children && f.IsCheckList()))
                        {
                            string value = field.GetValue(row);
                            string label = null;
                            if (tableViewer != null)
                            {
                                value = tableViewer.GetFieldValue(field, row);
                                label = tableViewer.GetFieldDisplayValue(field, row, false).Replace(",", ", ");
                            }
                            else
                            {
                                string pk = field.View.GetPkValue(row);
                                value = field.GetSelectedChildrenPKDelimited(pk);
                                label = field.GetDisplayValue(pk);
                            }

                            descriptives.Add(GetName(field), new Dictionary<string, object>() { { "label", label }, { "value", value } });
                        }
                        metadata.Add("descriptives", descriptives);
                    }
                    System.Web.Script.Serialization.JavaScriptSerializer javaScriptSerializer = new System.Web.Script.Serialization.JavaScriptSerializer();

                    Dictionary<string, object> dates = new Dictionary<string, object>();
                    foreach (ColumnField field in view.Fields.Values.Where(f => f.FieldType == FieldType.Column && f.GetColumnFieldType() == ColumnFieldType.DateTime))
                    {
                        string value = row.IsNull(field.DataColumn.ColumnName) ? string.Empty : ((DateTime)row[field.DataColumn.ColumnName]).ToUniversalTime().ToString("u");
                        dates.Add(GetName(field), value);
                    }
                    metadata.Add("dates", dates);
                }

                HandleMetadataExceptions(metadata, view, row, tableViewer, descriptive);
            }
            return metadata;
        }

        private object GetFieldsTypes(View view)
        {
            return view.GetFieldsTypes();
        }

        private void HandleMetadataExceptions(Dictionary<string, object> metadata, View view, DataRow row, TableViewer tableViewer, bool descriptive)
        {
            if (view.Name == "durados_v_ChangeHistory")
            {
                HandleMetadataHistoryExceptions(metadata, view, row, tableViewer, descriptive);
            }
        }

        private void HandleMetadataHistoryExceptions(Dictionary<string, object> metadata, View view, DataRow row, TableViewer tableViewer, bool descriptive)
        {
            Dictionary<string, object> descriptives = (Dictionary<string, object>)metadata["descriptives"];

            Field field = view.Fields["PK"];
            string value = field.GetValue(row);
            string label = value;
            if (value == "0")
            {
                label = "settings";
            }
            else
            {
                if (!row.IsNull("ViewName"))
                {
                    string configViewName = row["ViewName"].ToString();
                    if (view.Database.Map.GetConfigDatabase().Views.ContainsKey(configViewName))
                    {
                        View configView = (View)view.Database.Map.GetConfigDatabase().Views[configViewName];
                        if (configView != null)
                        {
                            DataRow configRow = configView.GetDataRow(value);
                            if (configRow != null)
                            {
                                label = configView.GetDisplayValue(configRow);
                            }
                        }
                    }
                }
            }
            descriptives.Add(GetName(field), new Dictionary<string, object>() { { "label", label }, { "value", value } });

            field = view.Fields["ViewName"];
            value = field.GetValue(row);
            label = value;
            if (value == "View")
            {
                label = "Object";
            }
            else if (value == "Database")
            {
                label = "App";
            }
            else if (value == "Rule")
            {
                label = "Action";
            }
            descriptives.Add(GetName(field), new Dictionary<string, object>() { { "label", label }, { "value", value } });



        }

        public virtual string GetName(Durados.Field field)
        {
            if (field.View.Database.IsConfig)
            {
                string name = field.DisplayName;
                return name.ReplaceNonAlphaNumeric2(string.Empty).LowerFirstChar();
            }
            else
            {
                return field.JsonName;
            }
        }

        public static Durados.Field GetField(Durados.View view, string name)
        {
            if (view.Fields.ContainsKey(name))
                return view.Fields[name];
            else
            {
                var fields = view.GetFieldsByJsonName(name);
                if (fields.Length > 0)
                    return fields[0];
                else
                {
                    fields = view.GetFieldsByDisplayName(name);
                    if (fields.Length > 0)
                        return fields[0];
                    else
                    {
                        return view.GetFieldByColumnNames(name);
                    }
                }
            }
        }

        private string GetErrorValue(Exception exception)
        {
            return "Err!";
        }

        public Dictionary<string, object> RowToDictionary(View view, DataRow row, string pk, bool deep, bool displayParentValue, int level, bool hideMetadata)
        {
            if (deep)
                return RowToDeepDictionary(view, row, level, hideMetadata);
            else
                return RowToShallowDictionary(view, row, pk, displayParentValue, hideMetadata);

        }

        public Dictionary<string, object> RowToDeepDictionary(View view, DataRow row, int level, bool hideMetadata)
        {
            return RowToDeepDictionary(view, row, true, new Dictionary<string, object>(), level, 0, hideMetadata);
        }


        //System.Threading.Tasks.Task.Run(() =>Send(host, useDefaultCredentials, port, username, password, useSsl, to, cc, bcc, subject, message, fromEmail, fromNick, anonymousEmail, dontSend, files, logger, false));

        public virtual Dictionary<string, object> RowToDeepDictionary(View view, DataRow dataRow, bool withChildren, Dictionary<string, object> pks, int level, int currentLevel, bool hideMetadata)
        {
            if (dataRow == null)
                return null;

            string pk = view.Name + view.GetPkValue(dataRow);

            Dictionary<string, object> dictionary = new Dictionary<string, object>();

            if (pks.ContainsKey(pk)) { }
            //return ((Dictionary<string, object>)pks[pk]);
            else
                pks.Add(pk, dictionary);

            dictionary.Add(Database.__metadata, GetRowMetadata(view, dataRow, null, true, hideMetadata));

            var fields = GetFields(view);

            foreach (Field field in fields)
            {
                bool isJsonable = IsJsonable(field, dataRow);

                if (field.FieldType == FieldType.Column && isJsonable)
                {
                    AddToDictionary(dictionary, field, GetColumnValue(view, dataRow, (ColumnField)field));

                }
                else if (field.FieldType == FieldType.Children && withChildren)
                {
                    ChildrenField childrenField = (ChildrenField)field;
                    View childrenView = (View)childrenField.ChildrenView;

                    DataView childTable = childrenField.GetDataView(dataRow);

                    if (!IsJsonable(childrenField, dataRow))
                        continue;

                    if (currentLevel >= level)
                        continue;

                    List<Dictionary<string, object>> list = new List<Dictionary<string, object>>();
                    foreach (System.Data.DataRowView childRow in childTable)
                    {
                        if (!IsJsonable(childrenField, childRow.Row))
                            continue;


                        Dictionary<string, object> childDictionary = RowToDeepDictionary(childrenView, childRow.Row, true, pks, level, currentLevel + 1, hideMetadata);

                        if (childDictionary != null)
                        {
                            list.Add(childDictionary);
                        }


                    }

                    AddToDictionary(dictionary, field, list.ToArray());

                }
                else if (field.FieldType == FieldType.Parent)
                {
                    ParentField parentField = (ParentField)field;
                    View parentView = (View)parentField.ParentView;
                    DataRow parentRow = dataRow.GetParentRow(parentField.DataRelation.RelationName);

                    if (!IsJsonable(parentField, dataRow))
                        continue;

                    if (!IsJsonable(parentField, parentRow))
                        continue;

                    if (parentRow == null)
                    {
                        AddToDictionary(dictionary, parentField, parentField.GetValue(dataRow));
                        continue;
                    }

                    Dictionary<string, object> parentDictionary = RowToDeepDictionary(parentView, parentRow, false, pks, level, 0, hideMetadata);

                    if (parentDictionary != null)
                    {
                        AddToDictionary(dictionary, field, parentDictionary);
                    }
                }

            }

            AddAdditionalProperties(view, dataRow, withChildren, pks, dictionary);

            return dictionary;
        }

        protected virtual void AddToDictionary(Dictionary<string, object> dictionary, Field field, object value)
        {
            string fieldName = GetName(field);
            if (!dictionary.ContainsKey(fieldName))
            {
                dictionary.Add(fieldName, value);
            }
        }



        protected virtual List<Field> GetFields(View view)
        {
            return view.GetVisibleFieldsForRow(DataAction.Edit);
        }

        protected virtual void AddAdditionalProperties(View view, DataRow dataRow, bool withChildren, Dictionary<string, object> pks, Dictionary<string, object> dictionary)
        {

        }

        protected virtual bool IsJsonable(Field field, DataRow dataRow)
        {
            return true;
        }

        protected virtual object GetColumnValue(View view, DataRow dataRow, ColumnField columnField)
        {
            //string value= columnField.ConvertToString(dataRow);
            //if (columnField.DataColumn.DataType == typeof(bool) && string.IsNullOrEmpty(value))
            //    value = false.ToString();
            object value = dataRow[columnField.DataColumn.ColumnName];


            if (view is Durados.Config.IConfigView && columnField.Name == "Formula")
            {
                string parentViewname = dataRow.GetParentRow("Fields")["Name"].ToString();

                Map map = Maps.Instance.GetMap();

                View currentView = (View)map.Database.Views[parentViewname];

                value = DataAccessHelper.ReplaceFieldDisplayNames(value.ToString(), false, currentView);
            }

            if (value is DBNull || value.Equals("null"))
                value = string.Empty;
            else if (columnField.DataColumn.DataType == typeof(DateTime) || columnField.DataColumn.DataType == typeof(byte[]))
            {
                //value = columnField.ConvertToString(dataRow);
                value = dataRow[columnField.DataColumn.ColumnName];
            }


            if (columnField.Encrypted && columnField.SpecialColumn == SpecialColumn.Password)
            {
                value = columnField.View.Database.EncryptedPlaceHolder;
            }

            return value;
        }

        private string GetParentValue(View view, DataRow dataRow, ParentField parentField)
        {
            string fk = string.Empty;
            DataRow parentRow = null;
            try
            {
                parentRow = dataRow.GetParentRow(parentField.DataRelation.RelationName);
                fk = parentField.ParentView.GetPkValue(parentRow);
            }
            catch (Exception)
            {
                DataColumn[] columns = parentField.GetDataColumns();
                if (columns.Length == 1)
                {
                    fk = dataRow[columns[0].ColumnName].ToString();
                }
                else
                {
                    foreach (DataColumn column in columns)
                        fk += dataRow[column.ColumnName].ToString() + ',';
                    fk.TrimEnd(',');
                }

            }

            return fk;
        }

        public Dictionary<string, object> RowToShallowDictionary(View view, DataRow row, string pk, bool displayParentValue, bool hideMetadata)
        {
            Json.View jsonView = view.GetJsonViewNotSerialized(DataAction.Edit, pk, row, string.Empty);

            Dictionary<string, object> dictionary = new Dictionary<string, object>();
            dictionary.Add(Database.__metadata, GetRowMetadata(view, row, null, true, hideMetadata));

            foreach (string fieldName in jsonView.Fields.Keys)
            {
                Json.Field JsonField = jsonView.Fields[fieldName];
                Field field = view.Fields[fieldName];

                bool isJsonable = IsJsonable(field, row) && field.IsIncludeInRequest();
                if (!isJsonable)
                    continue;

                string name = GetName(field);
                if (!dictionary.ContainsKey(name))
                {
                    if (field.FieldType == FieldType.Column && (field.IsDate || field.IsBoolean))
                        dictionary.Add(name, row[((ColumnField)field).DataColumn.ColumnName]);
                    else if (field.IsPoint)
                        dictionary.Add(name, GetPoint(field, row));
                    else if (field.FieldType == FieldType.Children)
                        dictionary.Add(name, null);
                    else
                        dictionary.Add(name, JsonField.Value);
                }
            }

            if (displayParentValue)
            {
                foreach (string fieldName in jsonView.Fields.Keys)
                {
                    Field field = view.Fields[fieldName];
                    bool isJsonable = IsJsonable(field, row);
                    if (!isJsonable)
                        continue;

                    if (field.FieldType == FieldType.Parent)
                    {
                        dictionary[fieldName] = field.GetValue(row);
                    }
                }
            }

            return dictionary;
        }



    }

    public class SortAdapter
    {
        public Dictionary<string, SortDirection> GetSortColumns(View view, Dictionary<string, object>[] sortIn)
        {
            if (sortIn == null)
                return null;

            Dictionary<string, SortDirection> sortOut = new Dictionary<string, SortDirection>();

            foreach (Dictionary<string, object> param in sortIn)
            {
                if (!param.ContainsKey("fieldName"))
                    throw new SortException("The sort items must contain the property \"fieldName\".");

                string fieldName = param["fieldName"].ToString();

                Field field = DictionaryConverter.GetField(view, fieldName);
                if (field == null)
                    throw new FilterException("Could not find the field associate with \"" + fieldName + "\". Please check in Database Tables \"" + view.JsonName + "\" in the Fields tab for the appropriate field name.");

                fieldName = field.Name;

                //if (!view.Fields.ContainsKey(fieldName))
                //{
                //    Field[] fields = view.GetFieldsByDisplayName(fieldName);
                //    if (fields.Length == 0)
                //        continue;
                //    else
                //        fieldName = fields[0].Name;
                //}

                string order = string.Empty;
                if (param.ContainsKey("order"))
                    order = param["order"].ToString();

                sortOut.Add(fieldName, order.ToLower().Equals("desc") ? SortDirection.Desc : SortDirection.Asc);
            }

            return sortOut;
        }
    }

    public class FilterAdapter
    {
        Dictionary<string, string> textOperatorMap = new Dictionary<string, string>();
        Dictionary<string, string> numericOperatorMap = new Dictionary<string, string>();
        Dictionary<string, string> relationOperatorMap = new Dictionary<string, string>();

        string prefix = "&&%&";
        string postfix = "&&%&";
        char space = ' ';
        public FilterAdapter()
        {
            numericOperatorMap.Add(FilterOperandType.equals.ToString(), "=");
            numericOperatorMap.Add(FilterOperandType.notEquals.ToString(), "<>");
            numericOperatorMap.Add(FilterOperandType.greaterThan.ToString(), ">");
            numericOperatorMap.Add(FilterOperandType.greaterThanOrEqualsTo.ToString(), ">=");
            numericOperatorMap.Add(FilterOperandType.lessThan.ToString(), "<");
            numericOperatorMap.Add(FilterOperandType.lessThanOrEqualsTo.ToString(), "<=");
            numericOperatorMap.Add(FilterOperandType.empty.ToString(), "empty");
            numericOperatorMap.Add(FilterOperandType.notEmpty.ToString(), "!empty");

            textOperatorMap.Add(FilterOperandType.equals.ToString(), "=");
            textOperatorMap.Add(FilterOperandType.notEquals.ToString(), "<>");
            textOperatorMap.Add(FilterOperandType.startsWith.ToString(), "like%");
            textOperatorMap.Add(FilterOperandType.endsWith.ToString(), "%like");
            textOperatorMap.Add(FilterOperandType.contains.ToString(), "%like%");
            textOperatorMap.Add(FilterOperandType.notContains.ToString(), "not %like%");
            textOperatorMap.Add(FilterOperandType.empty.ToString(), "empty");
            textOperatorMap.Add(FilterOperandType.notEmpty.ToString(), "!empty");

            relationOperatorMap.Add("in", "");
        }

        public Dictionary<string, object> ReplaceFilterOperands(View view, Dictionary<string, object>[] filterIn)
        {
            if (filterIn == null)
                return null;

            Dictionary<string, object> filterOut = new Dictionary<string, object>();

            foreach (Dictionary<string, object> param in filterIn)
            {
                if (!param.ContainsKey("fieldName"))
                    throw new FilterException("The filter items must contain the property \"fieldName\".");

                string fieldName = param["fieldName"].ToString();

                if (!param.ContainsKey("operator"))
                    throw new FilterException("The filter items must contain the property \"operator\".");

                string inOperator = param["operator"].ToString();

                string value = string.Empty;

                if (!param.ContainsKey("value"))
                    throw new FilterException("The filter items must contain the property \"value\".");

                if (param.ContainsKey("value") && param["value"] != null)
                {
                    try
                    {
                        if (param["value"] is Durados.DataAccess.Filter)
                        {
                        }
                        else
                        {
                            value = param["value"].ToString();
                        }
                    }
                    catch
                    {
                        throw new FilterException("Could not get the value from the \"value\" property of the field \"" + fieldName + "\". Make sure to encode the values with encodeURIComponent.");
                    }
                }

                Field field = DictionaryConverter.GetField(view, fieldName);
                if (field == null)
                    throw new FilterException("Could not find the field associate with \"" + fieldName + "\". Please check in Database Tables \"" + view.JsonName + "\" in the Fields tab for the appropriate field name.");

                fieldName = field.Name;

                //if (!view.Fields.ContainsKey(fieldName))
                //{
                //    Field[] fields = view.GetFieldsByJsonName(fieldName);
                //    if (fields.Length == 0)
                //    {
                //        fields = view.GetFieldsByDisplayName(fieldName);
                //        if (fields.Length == 0)
                //        {
                //            continue;
                //        }
                //        else
                //            fieldName = fields[0].Name;
                //    }
                //    else
                //        fieldName = fields[0].Name;
                //}

                //Field field = view.Fields[fieldName];
                string outOperator = null;
                if (field.FieldType == FieldType.Column)
                {
                    if (field.GetColumnFieldType() == ColumnFieldType.DateTime || field.IsNumeric)
                    {
                        if (numericOperatorMap.ContainsKey(inOperator))
                        {
                            outOperator = Wrap(numericOperatorMap[inOperator]);
                        }
                        else
                        {
                            throw new FilterException("The operator \"" + inOperator + "\" can not be used to filter numeric or date fields. To filter numeric or date fields please use one of the following operators: " + string.Join(",", numericOperatorMap.Keys.ToArray()));
                        }
                    }
                    else if (field.GetColumnFieldType() == ColumnFieldType.Boolean)
                    {
                        outOperator = null;
                        if (value.ToLower() == "yes" || value.ToLower() == "true")
                            filterOut.Add(fieldName, true);
                        else
                            filterOut.Add(fieldName, false);
                    }
                    else if (field.GetColumnFieldType() == ColumnFieldType.String || field.GetColumnFieldType() == ColumnFieldType.Guid)
                    {
                        if (textOperatorMap.ContainsKey(inOperator))
                        {
                            outOperator = Wrap(textOperatorMap[inOperator]);
                        }
                        else
                        {
                            throw new FilterException("The operator \"" + inOperator + "\" can not be used to filter textual fields. To filter textual fields please use one of the following operators: " + string.Join(",", textOperatorMap.Keys.ToArray()));
                        }
                    }
                }
                else
                {
                    string emptyValueIn = "[null]";
                    string emptyValueOut = ",[null]";

                    if (value == emptyValueIn || value == string.Empty)
                    {
                        filterOut.Add(fieldName, emptyValueOut);
                    }
                    else if (relationOperatorMap.ContainsKey(inOperator))
                    {
                        outOperator = relationOperatorMap[inOperator];
                    }
                    else
                    {
                        throw new FilterException("The field \"" + fieldName + "\" is a relation field. To filter relation fields please use the operator \"in\"");
                    }
                }

                if (outOperator != null && (value != null || inOperator == FilterOperandType.notEmpty.ToString() || inOperator == FilterOperandType.empty.ToString()))
                {
                    if (filterOut.ContainsKey(fieldName))
                    {
                        throw new FilterException("The simple filter can only contain one item per field. Please Use a Query.");
                    }
                    filterOut.Add(fieldName, outOperator + space + value);
                }
                if (param["value"] is Durados.DataAccess.Filter)
                {
                    filterOut.Add(fieldName, param["value"]);
                }
            }

            return filterOut;
        }

        private string Wrap(string @operator)
        {
            return string.Format("{0}{1}{2}", prefix, @operator, postfix);
        }
    }

    public class ChartHelperForRest : ChartHelper
    {
        public ChartHelperForRest(Map map) : base(map) { }
        public virtual List<tblObject> GetDashboardData(string dashboardId, string queryString)
        {
            if (string.IsNullOrEmpty(dashboardId))
                throw new DashboardIdIsMissingException();

            int dashboardIdInt = 0;
            if (!Int32.TryParse(dashboardId, out dashboardIdInt))
                throw new DashboardIdNotFoundException(dashboardId);

            if (!Database.Dashboards.ContainsKey(dashboardIdInt))
                throw new DashboardIdNotFoundException(dashboardId);

            List<tblObject> charts = new List<tblObject>();

            foreach (int chartId in Map.Database.Dashboards[dashboardIdInt].Charts.Keys)
            {
                tblObject tblObject = null;
                try
                {
                    tblObject = GetChartData(chartId.ToString(), queryString);
                }
                catch (Exception exception)
                {
                    tblObject = GetChartJsonObjectWithException(dashboardIdInt, chartId, exception);
                }
                if (tblObject.Type != "gauge")
                    charts.Add(tblObject);

            }

            return charts;
        }

        public tblObject GetChartJsonObjectWithException(int dashboardId, int chartId, Exception exception)
        {
            Chart chart = Map.Database.Dashboards[dashboardId].Charts[chartId];

            switch (chart.ChartType)
            {

                case ChartType.Pie:
                    tblPieObjectForRest tblPieObjectForRest = new tblPieObjectForRest(chart.ID.ToString());
                    tblPieObjectForRest.__metadata.Add("err", exception.Message);
                    return tblPieObjectForRest;
                case ChartType.Gauge:
                    tblGaugeObjectForRest tblGaugeObjectForRest = new tblGaugeObjectForRest(chart);
                    tblGaugeObjectForRest.__metadata.Add("err", exception.Message);
                    return tblGaugeObjectForRest;
                default:
                    tblSeriesObjectForRest tblSeriesObjectForRest = new tblSeriesObjectForRest(chart.ID.ToString());
                    tblSeriesObjectForRest.__metadata.Add("err", exception.Message);
                    return tblSeriesObjectForRest;
            }
        }
        public override tblObject GetChartJsonObject(Chart chart)
        {
            switch (chart.ChartType)
            {

                case ChartType.Pie:
                    return new tblPieObjectForRest(chart.ID.ToString());
                case ChartType.Gauge:
                    return new tblGaugeObjectForRest(chart);
                default:
                    return new tblSeriesObjectForRest(chart.ID.ToString());

            }
        }
    }

    public class tblPieObjectForRest : tblPieObject
    {
        public Dictionary<string, object> __metadata { get; set; }
        public tblPieObjectForRest(string chartId)
            : base()
        {
            this.__metadata = new Dictionary<string, object>();
            this.__metadata.Add("id", chartId);

        }
    }

    public class tblGaugeObjectForRest : tblGaugeObject
    {
        public Dictionary<string, object> __metadata { get; set; }
        public tblGaugeObjectForRest(Chart chart)
            : base(chart)
        {
            this.__metadata = new Dictionary<string, object>();
            this.__metadata.Add("id", chart.ID.ToString());

        }
    }

    public class tblSeriesObjectForRest : tblSeriesObject
    {
        public Dictionary<string, object> __metadata { get; set; }
        public tblSeriesObjectForRest(string chartId)
            : base()
        {
            this.__metadata = new Dictionary<string, object>();
            this.__metadata.Add("id", chartId);

        }
    }

    public class FileHandler
    {
        protected virtual bool FtpUploadValidExtension(string extension)
        {
            HashSet<string> h = new HashSet<string>(new string[] { "ade", "adp", "app", "bas", "bat", "chm", "cmd", "cpl", "crt", "csh", "exe", "fxp", "hlp", "hta", "inf", "ins", "isp", "js", "jse", "ksh", "Lnk", "mda", "mdb", "mde", "mdt", "mdt", "mdw", "mdz", "msc", "msi", "msp", "mst", "ops", "pcd", "pif", "prf", "prg", "pst", "reg", "scf", "scr", "sct", "shb", "shs", "url", "vb", "vbe", "vbs", "wsc", "wsf", "wsh" });
            return !h.Contains(extension.ToLower());
        }

        protected virtual bool FtpUploadValidSize(ColumnField field, float fileSize)
        {
            return true;
        }

        protected virtual bool FtpUploadValidFolderSize(ColumnField field, float fileSize)
        {
            return true;
        }

        protected virtual object SaveUploadedFile(ColumnField field)
        {
            string strFileName = System.IO.Path.GetFileName(System.Web.HttpContext.Current.Request.Files[0].FileName);

            string strExtension = System.IO.Path.GetExtension(System.Web.HttpContext.Current.Request.Files[0].FileName).ToLower().TrimStart('.');
            if (!FtpUploadValidExtension(strExtension))
            {
                throw new Exception("Invalid file type");
            }

            if (!string.IsNullOrEmpty(field.FtpUpload.FileAllowedTypes))
            {
                string[] exts = field.FtpUpload.FileAllowedTypes.Split(',');

                bool valid = false;

                foreach (string ext in exts)
                {
                    if (ext.Trim().Equals(strExtension))
                    {
                        valid = true;
                        break;
                    }
                }
                if (!valid)
                {
                    throw new Exception("Invalid file type in field [" + field.DisplayName + "].<br><br>Allowed formats: " + field.FtpUpload.FileAllowedTypes);
                }
            }

            float fileSize = (System.Web.HttpContext.Current.Request.Files[0].ContentLength / 1024) / 1000;

            if (!FtpUploadValidSize(field, fileSize))
            {
                throw new Exception("The file has exceeded the size limit.");
            }

            if (!FtpUploadValidFolderSize(field, fileSize))
            {
                throw new Exception("The folder has exceeded the size limit.");
            }

            if (field.FtpUpload.FileMaxSize > 0)
            {

                if (fileSize > field.FtpUpload.FileMaxSize)
                {
                    throw new Exception("File too big in field [" + field.DisplayName + "].<br><br>Max Allowed size: " + field.FtpUpload.FileMaxSize + " MB");
                }
            }

            string strSaveLocation = string.Empty;

            string src = string.Empty;

            if (field.FtpUpload.StorageType == StorageType.Azure)
            {
                src = SaveUploadedFileToAzure(field, strFileName);
            }
            else if (field.FtpUpload.StorageType == StorageType.Aws)
            {
                src = SaveUploadedFileToAws(field, strFileName);
            }
            else
            {
                SaveUploadedFileToFtp(field, strFileName);

                if (field.FtpUpload.StorageType != StorageType.Azure)
                {
                    src = field.FtpUpload.DirectoryVirtualPath.TrimEnd('/') + "/" + ((string.IsNullOrEmpty(field.FtpUpload.DirectoryBasePath)) ? string.Empty : (field.FtpUpload.DirectoryBasePath.TrimStart('/').TrimEnd('/') + "/")) + strFileName;
                }
                else
                {
                    src = System.Web.HttpContext.Current.Server.UrlEncode((new UI.ColumnFieldViewer()).GetDownloadUrl(field, string.Empty));
                }
            }

            return new { FileName = strFileName, Path = src };
        }

        private void SaveUploadedFileToFtp(ColumnField field, string strFileName)
        {
            throw new NotImplementedException();
        }

        private string SaveUploadedFileToAws(ColumnField field, string strFileName)
        {
            throw new NotImplementedException();
        }

        protected virtual string SaveUploadedFileToAzure(ColumnField field, string strFileName)
        {
            return SaveUploadedFileToAzure(field.FtpUpload.AzureAccountName, field.FtpUpload.GetDecryptedAzureAccountKey(((Database)field.View.Database).Map.GetConfigDatabase()), field.FtpUpload.DirectoryBasePath, strFileName);
        }
        protected virtual string SaveUploadedFileToAzure(string accountName, string accountKey, string folder, string strFileName)
        {
            string fileUri = string.Empty;
            try
            {
                if (Maps.AzureStorageAccountKey == accountKey)
                    folder = Maps.AzureAppPrefix + Maps.Instance.GetCurrentAppId(); //Azure must have at least 3 chars

                // Variables for the cloud storage objects.
                CloudStorageAccount cloudStorageAccount;
                CloudBlobClient blobClient;
                CloudBlobContainer blobContainer;
                BlobContainerPermissions containerPermissions;
                CloudBlob blob;

                //cloudStorageAccount = CloudStorageAccount.Parse("DefaultEndpointsProtocol=http;AccountName=qacontent;AccountKey=6/OpnuGeLRm8REKhQ/5Led/WoJcHjmFJII4GM5HYJ120o7OxtgC1zCstw0kqyn05N2m1jGadY3aYFypOzD4L5A==");
                cloudStorageAccount = CloudStorageAccount.Parse(string.Format("DefaultEndpointsProtocol=http;AccountName={0};AccountKey={1}", accountName, accountKey));

                // Create the blob client, which provides
                // authenticated access to the Blob service.
                blobClient = cloudStorageAccount.CreateCloudBlobClient();

                // Get the container reference.
                //blobContainer = blobClient.GetContainerReference("general");
                blobContainer = blobClient.GetContainerReference(folder);

                // Create the container if it does not exist.
                if (Maps.AzureStorageAccountKey == accountKey)
                {
                    // Set permissions on the container.
                    blobContainer.CreateIfNotExist();
                    containerPermissions = new BlobContainerPermissions();
                    containerPermissions.PublicAccess = BlobContainerPublicAccessType.Blob;
                    blobContainer.SetPermissions(containerPermissions);
                }

                // Get a reference to the blob.
                blob = blobContainer.GetBlobReference(strFileName);

                blob.Properties.ContentType = System.Web.HttpContext.Current.Request.Files[0].ContentType;

                // Upload a file from the local system to the blob.
                //blob.UploadFile(Request.Files[0].FileName);  // File from emulated storage.
                blob.UploadFromStream(System.Web.HttpContext.Current.Request.Files[0].InputStream);
                fileUri = blob.Uri.ToString();


            }
            catch (Exception exception)
            {
                throw new DuradosException("Upload failed: " + exception.Message, exception);
            }

            return fileUri;

        }
    }

    public class Stat
    {
        protected SqlAccess sqlAccess = null;
        public Stat()
        {
            sqlAccess = new SqlAccess();
        }
        public void AddStat(Dictionary<string, object> item, string appName, bool reset)
        {
            if (item.ContainsKey("durados_App_Stat"))
                item.Remove("durados_App_Stat");

            Map map = Maps.Instance.GetMap(appName);

            if (map == null || map is DuradosMap)
            {
                return;
            }

            object stat = null;
            if (reset || map.Stat == null || map.Stat.Time < DateTime.Now.Subtract(new TimeSpan(1, 0, 0)))
            {
                map.Stat = new StatAndTime() { Time = DateTime.Now, Stat = GetStat(appName) };
            }
            stat = map.Stat.Stat;
            item.Add("stat", stat);
            if (item.ContainsKey("PaymentStatus"))
            {
                item["PaymentStatus"] = (int)map.PaymentStatus;
            }
            else
            {
                item.Add("PaymentStatus", (int)map.PaymentStatus);
            }
        }

        public class StatAndTime
        {
            public DateTime Time;
            public object Stat;
        }
        //private static Dictionary<string, StatAndTime> cache = new Dictionary<string, StatAndTime>();
        //private static MemoryCache cache = new MemoryCache("AppStat",);


        private object GetStat(string appName)
        {
            //string sysConnectionString = GetSystemConnectionString(appName);

            //if (string.IsNullOrEmpty(sysConnectionString))
            //    return null;

            //object scalar = GetLast24Hours(sysConnectionString);
            //int? last24 = null;
            //if (scalar != null) last24 = Convert.ToInt32(scalar);

            //scalar = GetLast48Hours(sysConnectionString);
            //int? last48 = null;
            //if (scalar != null) last48 = Convert.ToInt32(scalar);

            //int? diff24 = null;
            //if (last24.HasValue && last48.HasValue)
            //{
            //    int today = last24.Value;
            //    int yesterday = last48.Value - today;

            //    if (today == yesterday)
            //    {
            //        diff24 = 0;
            //    }
            //    else if (yesterday == 0)
            //    {
            //        diff24 = null;
            //    }
            //    else
            //    {
            //        diff24 = (100 * (today - yesterday)) / yesterday;
            //    }
            //}

            //scalar = GetLast30Days(sysConnectionString);
            //int? last30 = null;
            //if (scalar != null) last30 = Convert.ToInt32(scalar);

            //scalar = GetLast60Days(sysConnectionString);
            //int? last60 = null;
            //if (scalar != null) last60 = Convert.ToInt32(scalar);

            //int? diff30 = null;
            //if (last30.HasValue && last60.HasValue)
            //{
            //    int thisMonth = last30.Value;
            //    int prevMonth = last60.Value - thisMonth;

            //    if (thisMonth == prevMonth)
            //    {
            //        diff30 = 0;
            //    }
            //    else if (prevMonth == 0)
            //    {
            //        diff30 = null;
            //    }
            //    else
            //    {
            //        diff30 = (100 * (thisMonth - prevMonth)) / prevMonth;
            //    }
            //}

            //int? size = GetSize(appName, GetConnectionString(appName));

            Dictionary<string, object> totalRows = GetTotalRows(appName);

            Dictionary<string, object> authorizationSecurity = GetAuthorizationSecurity(appName);
            Dictionary<string, object> dataSecurity = GetDataSecurity(appName);

            Dictionary<string, object> stat = new Dictionary<string, object>() { 
            //{ "last24hours", last24 }, { "diffLastDaytoYesterday", diff24 }, { "last30days", last30 }, { "diffLast30DaysToPrev", diff30 }, { "sizeInMb", size }, 
            { "totalRows", totalRows }, { "authorizationSecurity", authorizationSecurity }, { "dataSecurity", dataSecurity } };

            
            return stat;
        }

        private Dictionary<string, object> GetDataSecurity(string appName)
        {
            Map map = Maps.Instance.GetMap(appName);
            if (map == null)
                return null;

            Dictionary<string, object> dataSecurity = new Dictionary<string, object>();

            foreach (View view in map.Database.Views.Values.Where(v => !v.SystemView && !v.IsCloned).OrderBy(v => v.JsonName))
            {
                dataSecurity.Add(view.JsonName, !string.IsNullOrEmpty(view.PermanentFilter));
            }

            return dataSecurity;
        }

        private Dictionary<string, object> GetAuthorizationSecurity(string appName)
        {
            Map map = Maps.Instance.GetMap(appName);
            if (map == null)
                return null;

            Dictionary<string, object> dataSecurity = new Dictionary<string, object>();

            foreach (View view in map.Database.Views.Values.Where(v => !v.SystemView && !v.IsCloned).OrderBy(v => v.JsonName))
            {
                if (!dataSecurity.ContainsKey(view.JsonName))
                {
                    dataSecurity.Add(view.JsonName, view.Precedent);
                }
            }

            return dataSecurity;
        }

        private Dictionary<string, object> GetTotalRows(string appName)
        {
            string connectionString = GetConnectionString(appName);

            SqlProduct? sqlProduct = GetSqlProduct(appName);

            if (!sqlProduct.HasValue || string.IsNullOrEmpty(connectionString))
                return null;

            if (sqlProduct != SqlProduct.MySql)
                return null;

            SqlAccess sqlAccess = GetSqlAccess(sqlProduct.Value);
            string sql = GetTotalRowsSql(sqlProduct.Value);

            try
            {
                DataTable table = sqlAccess.ExecuteTable(connectionString, sql, null, CommandType.Text);

                Dictionary<string, object> totals = new Dictionary<string, object>();

                foreach (DataRow row in table.Rows)
                {
                    string tableName = row["TableName"].ToString();
                    object total = row["TotalRows"];
                    if (!totals.ContainsKey(tableName))
                    {
                        try
                        {
                            totals.Add(tableName, total);
                        }
                        catch { }
                    }
                }

                return totals;
            }
            catch { }
            return null;
        }

        public static string GetTotalRowsSql(SqlProduct sqlProduct)
        {
            string sql = null;

            switch (sqlProduct)
            {
                case SqlProduct.MySql:
                    sql = "SELECT table_name as TableName, table_rows as TotalRows  FROM information_schema.tables where table_schema = DATABASE() order by table_name ";
                    break;
                case SqlProduct.Postgre:
                    break;
                case SqlProduct.Oracle:
                    break;

                default:
                    break;
            }

            return sql;
        }

        private int? GetSize(string appName, string connectionString)
        {
            SqlProduct? sqlProduct = GetSqlProduct(appName);

            if (!sqlProduct.HasValue || string.IsNullOrEmpty(connectionString))
                return null;

            if (sqlProduct == SqlProduct.Oracle)
                return null;

            SqlAccess sqlAccess = GetSqlAccess(sqlProduct.Value);
            string sql = GetSql(sqlProduct.Value);

            try
            {
                string scalar = sqlAccess.ExecuteScalar(connectionString, sql);

                decimal size = -1;
                if (decimal.TryParse(scalar, out size))
                    return Convert.ToInt32(size);
            }
            catch { }
            return null;
        }

        private SqlAccess GetSqlAccess(SqlProduct sqlProduct)
        {
            SqlAccess sqlAccess = null;

            switch (sqlProduct)
            {
                case SqlProduct.MySql:
                    sqlAccess = new MySqlAccess();
                    break;
                case SqlProduct.Postgre:
                    sqlAccess = new PostgreAccess();
                    break;
                case SqlProduct.Oracle:
                    sqlAccess = new OracleAccess();
                    break;

                default:
                    sqlAccess = new SqlAccess();
                    break;
            }

            return sqlAccess;
        }

        public static string GetSql(SqlProduct sqlProduct)
        {
            string sql = null;

            switch (sqlProduct)
            {
                case SqlProduct.MySql:
                    sql = "SELECT Round(Sum(data_length + index_length) / 1024 / 1024, 1) FROM   information_schema.tables GROUP  BY table_schema; ";
                    break;
                case SqlProduct.Postgre:
                    sql = "SELECT pg_database_size(current_DATABASE()) / 1024 / 1024;";
                    break;
                case SqlProduct.Oracle:
                    sql = "";
                    break;

                default:
                    sql = "SELECT row_size_mb = CAST(SUM(CASE WHEN type_desc = 'ROWS' THEN size END) * 8. / 1024 AS DECIMAL(8,2)) FROM sys.master_files WITH(NOWAIT) WHERE database_id = DB_ID() GROUP BY database_id";
                    break;
            }

            return sql;
        }

        private SqlProduct? GetSqlProduct(string appName)
        {
            //if (!Maps.Instance.AppInCach(appName))
            //    return null;

            Map map = Maps.Instance.GetMap(appName);
            if (map == null)
                return null;

            return map.Database.SqlProduct;
        }

        private string GetConnectionString(string appName)
        {
            //if (!Maps.Instance.AppInCach(appName))
            //    return null;

            Map map = Maps.Instance.GetMap(appName);
            if (map == null)
                return null;

            return map.connectionString;
        }

        private string GetSystemConnectionString(string appName)
        {
            //if (!Maps.Instance.AppInCach(appName))
            //    return null;

            Map map = Maps.Instance.GetMap(appName);
            if (map == null)
                return null;

            return map.systemConnectionString;
        }

        private object GetLast60Days(string connectionString)
        {
            string sql = GetLast60DaysSelect();

            return sqlAccess.ExecuteScalar(connectionString, sql);
        }

        private object GetLast30Days(string connectionString)
        {
            string sql = GetLast30DaysSelect();

            return sqlAccess.ExecuteScalar(connectionString, sql);
        }

        private object GetLast48Hours(string connectionString)
        {
            string sql = GetLast48HoursSelect();

            return sqlAccess.ExecuteScalar(connectionString, sql);
        }

        private object GetLast24Hours(string connectionString)
        {
            string sql = GetLast24HoursSelect();

            return sqlAccess.ExecuteScalar(connectionString, sql);

        }
        protected virtual string GetLast60DaysSelect()
        {
            return "select count(*) from dbo.Durados_Log with (nolock) where LogType = 3 and Username <> 'dev@devitout.com' and Time >= dateadd(day,-60,getdate())";
        }
        protected virtual string GetLast30DaysSelect()
        {
            return "select count(*) from dbo.Durados_Log with (nolock) where LogType = 3 and Username <> 'dev@devitout.com' and Time >= dateadd(day,-30,getdate())";
        }
        protected virtual string GetLast48HoursSelect()
        {
            return "select count(*) from dbo.Durados_Log with (nolock) where LogType = 3 and Username <> 'dev@devitout.com' and Time >= dateadd(hour,-48,getdate())";
        }
        protected virtual string GetLast24HoursSelect()
        {
            return "select count(*) from dbo.Durados_Log with (nolock) where LogType = 3 and Username <> 'dev@devitout.com' and Time >= dateadd(hour,-24,getdate())";
        }


    }

    public class MySqlState : Stat
    {
        public MySqlState()
        {
            sqlAccess = new MySqlAccess();
        }

        protected override string GetLast60DaysSelect()
        {
            return "select count(*) from Durados_Log  where LogType = 3 and Username <> 'dev@devitout.com' and Time >=  DATE_ADD(CURDATE(),INTERVAL -60 DAY)";
        }
        protected override string GetLast30DaysSelect()
        {
            return "select count(*) from Durados_Log  where LogType = 3 and Username <> 'dev@devitout.com' and Time >=  DATE_ADD(CURDATE(),INTERVAL -30 DAY)";
        }
        protected override string GetLast48HoursSelect()
        {
            return "select count(*) from Durados_Log  where LogType = 3 and Username <> 'dev@devitout.com' and Time >= DATE_ADD(CURDATE(),INTERVAL -48 HOUR)";
        }
        protected override string GetLast24HoursSelect()
        {

            return "select count(*) from Durados_Log  where LogType = 3 and Username <> 'dev@devitout.com' and Time >= DATE_ADD(CURDATE(),INTERVAL -24 HOUR)";
        }

    }

    public class StatFactory
    {
        public static Stat GetState(SqlProduct systemSqlProduct)
        {
            if (systemSqlProduct == SqlProduct.MySql)
                return new MySqlState();
            return new Stat();
        }
    }

    public static class Analytics
    {


        private static Durados.Web.Mvc.Logging.IExternalAnalytics xLogger;

        public static void Log(Durados.Web.Mvc.Logging.ExternalAnalyticsAction eventName, string username, Dictionary<string, object> dict, Dictionary<string, object> page = null, Dictionary<string, object> campaign = null, string userAgent = null)
        {
            try
            {
                if (!dict.ContainsKey(Database.AppName) || dict[Database.AppName].ToString() != Maps.DuradosAppName)
                {
                    Maps.Instance.DuradosMap.Logger.Log("Anaytics", "Log-Signup", "Log", "missing app name", null, 1, null, DateTime.Now);
                    return;
                }


                if (xLogger != null)
                    xLogger.Log(eventName, null, username, dict, true, page, campaign, userAgent);
            }
            catch (Exception exception)
            {
                Maps.Instance.DuradosMap.Logger.Log("Anaytics", "Log-Signup", "Log", exception, 1, null, DateTime.Now);
            }

        }

        public static void Init()
        {
            xLogger = Durados.Web.Mvc.Logging.ExternalAnalyticsFactory.GetLogger(Durados.Web.Mvc.Maps.Instance.DuradosMap.Logger, 1);
        }
    }

    public class Sync
    {
        public virtual Dictionary<string, object> AddNewViewsAndSyncAll(Map map)
        {
            string errorMessage = string.Empty;
            Dictionary<string, object> result = AddNewViews(map, out errorMessage);
            result.Add("errors", errorMessage);
            SyncAll(map);

            result = RemoveDropedViews(map, result, out errorMessage);
            result["errors"] += errorMessage;
            return result;
        }

        private Dictionary<string, object> RemoveDropedViews(Map map, Dictionary<string, object> result, out string errorMessage)
        {
            Dictionary<string, int> i = map.RemovedDeletedViews(map.Database, out errorMessage);

            result.Add("deletedTables", i["deletedTables"]);
            result.Add("removed", i["removed"]);

            if (i["deletedTables"] > 0)
            {
                Initiate(map);
            }

            return result;
        }

        public virtual Dictionary<string, object> AddNewViews(Map map, out string errorMessage)
        {
            try
            {
                DataView dataView = map.GetSchemaEntities();
                List<string> pkList = new List<string>();
                string tableEntity = map.DynamicMapper.GetTableEntity();
                foreach (System.Data.DataRowView row in dataView)
                {
                    string viewName = row["Name"].ToString();
                    if (!map.Database.Views.ContainsKey(viewName) && row["EntityType"].ToString().Equals(tableEntity))
                    {
                        pkList.Add(viewName);
                    }
                }
                string pks = pkList.ToArray().Delimited();
                int added = map.Database.AddViews(pks, dataView, false, out errorMessage);
                return new Dictionary<string, object>() { { "newTables", pkList.Count }, { "added", added } };
            }
            catch (Exception exception)
            {
                throw new DuradosException("Failed to add new views", exception);
            }
        }

        public virtual void SyncAll(Map map)
        {
            try
            {
                lock (map)
                {
                    Durados.DataAccess.ConfigAccess configAccess = new Durados.DataAccess.ConfigAccess();

                    foreach (View view in map.Database.Views.Values.Where(v => !v.SystemView && !v.IsCloned))
                    {
                        string viewName = view.Name;
                        string configViewPk = configAccess.GetViewPK(viewName, map.GetConfigDatabase().ConnectionString);
                        map.Sync(viewName, configViewPk);
                    }
                    Initiate(map);
                }
            }
            catch (Exception exception)
            {
                throw new DuradosException("Failed to sync all", exception);
            }
        }

        public virtual void Initiate(Map map)
        {
            lock (map)
            {
                string fileName = map.GetConfigDatabase().ConnectionString;
                Database configDatabase = map.GetConfigDatabase();
                map.Database.SetNextMinorConfigVersion();
                Durados.DataAccess.ConfigAccess.UpdateVersion(map.Database.ConfigVersion, fileName);
                Durados.DataAccess.ConfigAccess.SaveConfigDataset(configDatabase.ConnectionString, map.Logger);
                map.SaveDynamicMapping();
                map.Initiate();
                ConfigAccess.Refresh(fileName);
                configDatabase = map.GetConfigDatabase();
            }
        }
    }

    public class DataViewForDictionary
    {
        public Dictionary<string, object> GetDataViewForDictionary(View view, bool withSystemTokens, bool deep, Crud crud)
        {
            Dictionary<string, object> dictionary = new Dictionary<string, object>();
            if (withSystemTokens)
            {
                DataView dataView = GetDataViewForDictionary(null, DictionaryType.PlaceHolders);
                if (dataView != null)
                    dictionary.Add("systemTokens", GetDictionary(dataView));
            }

            if (view != null)
            {
                if (crud == Crud.create)
                {
                    dictionary.Add("userInput", GetDictionaryForCreate(view));
                }
                else if (crud == Crud.update)
                {
                    dictionary.Add("userInput", GetDictionaryForUpdate(view));
                }
                if (crud != Crud.create)
                {
                    DictionaryType dictionaryType = DictionaryType.InternalNames;
                    if (deep)
                    {
                        dictionaryType = DictionaryType.DisplayNames;
                    }
                    DataView dataView = GetDataViewForDictionary(view, dictionaryType);
                    dictionary.Add(view.JsonName, GetDictionary(dataView, view));
                }
            }

            return dictionary;
        }

        private object GetDictionaryForUpdate(View view)
        {
            return GetDictionaryForCrud(view, Crud.update);
        }

        private object GetDictionaryForCreate(View view)
        {
            return GetDictionaryForCrud(view, Crud.create);
        }

        private object GetDictionaryForCrud(View view, Crud crud)
        {
            List<Dictionary<string, object>> dictionary = new List<Dictionary<string, object>>();

            foreach (Field field in view.Fields.Values.Where(f => f.FieldType != FieldType.Children && !IsExcluded(f)))
            {
                string label = field.JsonName;
                string token = field.JsonName;
                dictionary.Add(new Dictionary<string, object>() { { "label", label }, { "token", token } });
            }

            return dictionary.ToArray();
        }

        protected virtual bool IsExcluded(Field field)
        {
            if (field.View == field.View.Database.GetUserView())
            {
                HashSet<string> exclude = new HashSet<string>() { "Password", "Guid", "Signature", "SignatureHTML", "NewUser", "Comments" };

                return exclude.Contains(field.Name);
            }
            return false;
        }

        private Dictionary<string, object>[] GetDictionary(DataView dataView, View view = null)
        {
            List<Dictionary<string, object>> dictionary = new List<Dictionary<string, object>>();

            foreach (System.Data.DataRowView row in dataView)
            {
                string label = null;
                if (!row.Row.IsNull("Tag"))
                    label = row.Row["Tag"].ToString();
                string token = null;
                if (!row.Row.IsNull("Token"))
                    token = row.Row["Token"].ToString();
                if (!IsExcluded(view, label) && label != null && token != null)
                {
                    dictionary.Add(new Dictionary<string, object>() { { "label", label }, { "token", token } });
                }
            }
            return dictionary.ToArray();
        }

        protected virtual bool IsExcluded(View view, string label)
        {
            if (view != null && view == view.Database.GetUserView())
            {
                HashSet<string> exclude = new HashSet<string>() { "User.Password", "User.Guid", "User.Signature", "User.Signature HTML", "User.New User", "User.Comments", "User.Role.Name", "User.Role.Description", "User.Role.First View" };

                return exclude.Contains(label);
            }
            return false;
        }

        private DataView GetDataViewForDictionary(View view, DictionaryType dictionaryType)
        {
            DataView dataView = null;
            string pk = null;
            string cacheKey = null;
            string viewDictionary = "viewDictionary";
            if (view != null)
                cacheKey = dictionaryType.ToString() + "__" + view.Name;

            //if (view != null && view.Database.Map.AllKindOfCache.ContainsKey(viewDictionary) && view.Database.Map.AllKindOfCache[viewDictionary].ContainsKey(cacheKey))
            //{
            //    return (DataView)view.Database.Map.AllKindOfCache[viewDictionary][cacheKey];
            //}
            if (view != null && view.Database.Map.AllKindOfCache.Contains(viewDictionary) && ((MemoryCache)view.Database.Map.AllKindOfCache[viewDictionary]).Contains(cacheKey))
            {
                return (DataView)((MemoryCache)view.Database.Map.AllKindOfCache[viewDictionary])[cacheKey];
            }

            switch (dictionaryType)
            {

                case DictionaryType.PlaceHolders:
                    dataView = GetPlaceHolderDictionary();
                    break;

                case DictionaryType.InternalNames:
                    dataView = GetInternalNameDictionary(view, null);
                    break;


                default:
                    pk = GetFirstPK(view);
                    dataView = GetDisplayNameDictionary(view, pk);
                    break;
            }

            if (view != null)
            {
                if (!view.Database.Map.AllKindOfCache.Contains(viewDictionary))
                {
                    view.Database.Map.AllKindOfCache[viewDictionary] = new MemoryCache(viewDictionary);
                }
                MemoryCache viewDictionaryCache = (MemoryCache)view.Database.Map.AllKindOfCache[viewDictionary];
                if (viewDictionaryCache.Contains(cacheKey))
                {
                    viewDictionaryCache.Remove(cacheKey);
                }
                if (pk != null)
                    viewDictionaryCache[cacheKey] = dataView;

                //if (!view.Database.Map.AllKindOfCache.ContainsKey(viewDictionary))
                //{
                //    view.Database.Map.AllKindOfCache.Add(viewDictionary, new Dictionary<string, object>());
                //}
                //if (view.Database.Map.AllKindOfCache[viewDictionary].ContainsKey(cacheKey))
                //{
                //    view.Database.Map.AllKindOfCache[viewDictionary].Remove(cacheKey);
                //}
                //if (pk != null)
                //    view.Database.Map.AllKindOfCache[viewDictionary].Add(cacheKey, dataView);
            }


            return dataView;
        }

        private string GetFirstPK(View view)
        {
            IDataTableAccess sqlAccess = DataAccessHelper.GetDataTableAccess(view.ConnectionString);
            return sqlAccess.GetFirstPK(view);
        }

        private static DataTable GetDictionarySchema()
        {

            DataTable dt = new DataTable("View");
            dt.Columns.Add("Tag", typeof(string));
            dt.Columns.Add("Token", typeof(string));
            dt.Columns.Add("DataType", typeof(DataType));
            return dt;
        }

        private DataView GetPlaceHolderDictionary()
        {
            DataTable dt = GetDictionarySchema();
            //dt.Rows.Add("User Id", Durados.Database.SysUserPlaceHolder, DataType.Numeric);
            dt.Rows.Add("User Role", Durados.Database.SysRolePlaceHolder, DataType.ShortText);
            dt.Rows.Add("Username", Durados.Database.SysUsernamePlaceHolder, DataType.ShortText);

            Map map = Maps.Instance.GetMap();
            if (map != null && !map.IsMainMap)
            {
                var dic = map.Database.GetConfigDictionary();
                if (dic != null)
                {
                    foreach (string key in dic.Keys)
                    {
                        string name = key;
                        object value = dic[key];

                        if (!(value is IDictionary<string, object>) && !(value is IEnumerable<object>))
                        {
                            dt.Rows.Add(name, Durados.Database.ConfigPlaceHolder + name, DataType.ShortText);
                        }
                    }
                }
            }
            //   dt.Rows.Add("Current Date", Durados.Database.CurrentDatePlaceHolder);
            DataView dataView = new DataView(dt);
            return dataView;
        }

        private DataView GetInternalNameDictionary(View view, string pk)
        {
            DataTable dt = GetDictionarySchema();
            string dynastyPath = view.DisplayNameForDynasty + ".";
            foreach (Field field in view.Fields.Values.Where(f => !f.Excluded && (f.FieldType == FieldType.Column || f.FieldType == FieldType.Parent)))
            {

                dt.Rows.Add(dynastyPath + field.DisplayNameForDynasty, field.Name, field.DataType);
            }

            DataView dataView = new DataView(dt);
            return dataView;
        }

        private DataView GetDisplayNameDictionary(View view, string pk)
        {
            View blockView = ViewHelper.GetView("Block");

            DataTable dt = GetDictionarySchema();
            dt.CaseSensitive = true;

            List<string> names = new List<string>();
            Dictionary<string, Durados.Workflow.DictionaryField> dicFields = new Dictionary<string, Durados.Workflow.DictionaryField>();
            LoadNames(names, view, null, view, view.DisplayName + ".", string.Empty, string.Empty, pk, dicFields, view.Name + ".");

            //dataTable.Rows.Clear();
            //foreach (string name in names)
            //{
            //    dt.Rows.Add( name, name );
            //}
            foreach (string name in dicFields.Keys)
            {
                dt.Rows.Add(dicFields[name].DisplayName, name, dicFields[name].Type);

            }
            DataView dataView = new DataView(dt);
            return dataView;
        }

        protected void LoadNames(List<string> names, View view, ParentField parentField, View rootView, string dynastyPath, string prefix, string postfix, string pk, Dictionary<string, Durados.Workflow.DictionaryField> dicFields, string internalDynastyPath)
        {
            if (pk == null)
            {
                LoadNames(names, view, parentField, rootView, dynastyPath, prefix, postfix);
            }
            else
            {
                DataRow row = view.GetDataRow(pk);
                Dictionary<string, object> values = new Dictionary<string, object>();
                LoadValues(values, row, view, parentField, rootView, dynastyPath, prefix, postfix, dicFields, internalDynastyPath);

                foreach (string name in values.Keys)
                {
                    names.Add(name);
                }
            }
        }

        protected void LoadNames(List<string> names, View view, ParentField parentField, View rootView, string dynastyPath, string prefix, string postfix)
        {
            if (view.Equals(rootView))
                dynastyPath = view.DisplayName + ".";

            foreach (Field field in view.Fields.Values.Where(f => f.FieldType == FieldType.Column))
            {
                LoadName(names, view, field, dynastyPath, prefix, postfix);
            }

            foreach (ChildrenField field in view.Fields.Values.Where(f => f.FieldType == FieldType.Children && ((ChildrenField)f).LoadForBlockTemplate))
            {
                LoadName(names, view, field, dynastyPath, prefix, postfix);
            }

            foreach (ParentField field in view.Fields.Values.Where(f => f.FieldType == FieldType.Parent))
            {
                if (view.Equals(rootView))
                    dynastyPath = view.DisplayName + ".";

                LoadName(names, view, field, dynastyPath, prefix, postfix);

                View parentView = (View)field.ParentView;

                if (parentView != rootView && !IsRecursive(dynastyPath + field.DisplayName))
                {
                    //dynastyPath += field.DisplayName + ".";
                    dynastyPath = GetDynastyPath(dynastyPath, parentField, field);
                    LoadNames(names, parentView, field, rootView, dynastyPath, prefix, postfix);
                }

            }
        }

        protected bool IsRecursive(string dynastyPath)
        {
            string[] s = dynastyPath.Split('.');

            int l = s.Length;

            for (int rank = 2; rank <= l / 2; rank++)
            {
                if (IsRecursive(dynastyPath, rank))
                    return true;
            }

            return false;
        }

        protected bool IsRecursive(string dynastyPath, int rank)
        {
            string[] s = dynastyPath.Split('.');

            string last = string.Empty;

            if (s.Length > rank)
            {
                for (int i = s.Length - 1; i >= s.Length - rank; i--)
                {
                    last += s[i] + ".";
                }

                return dynastyPath.Contains(last);
            }

            return false;
        }
        protected void LoadName(List<string> names, View view, Field field, string dynastyPath, string prefix, string postfix)
        {
            string name = prefix + dynastyPath + field.DisplayName + postfix;
            if (!names.Contains(name))
                names.Add(name);
        }

        protected virtual string GetViewDisplayName(View view)
        {
            return view.DisplayName;
        }

        private void LoadValue(Dictionary<string, object> values, DataRow dataRow, Durados.View view, Durados.Field field, string dynastyPath, string prefix, string postfix, Dictionary<string, Durados.Workflow.DictionaryField> dicFields, string internalDynastyPath)
        {
            string name = prefix + dynastyPath + field.DisplayName + postfix;
            string InternalName = prefix + internalDynastyPath + field.Name + postfix;
            string value = view.GetDisplayValue(field.Name, dataRow);
            if (!values.ContainsKey(name))
            {
                values.Add(name, value);
                dicFields.Add(InternalName, new Durados.Workflow.DictionaryField { DisplayName = name, Type = field.DataType, Value = value });

            }
            if (field.FieldType == FieldType.Column && ((ColumnField)field).Upload != null)
            {
                if (dataRow.Table.Columns.Contains(field.Name))
                {

                    dataRow.Table.Columns[field.Name].ExtendedProperties["ImagePath"] = ((ColumnField)field).GetUploadPath();
                }
            }
        }

        private void LoadValues(Dictionary<string, object> values, DataRow dataRow, Durados.View view, Durados.ParentField parentField, Durados.View rootView, string dynastyPath, string prefix, string postfix, Dictionary<string, Durados.Workflow.DictionaryField> dicFields, string internalDynastyPath)
        {
            if (view.Equals(rootView))
            {
                dynastyPath = GetViewDisplayName((View)view) + ".";
                internalDynastyPath = view.Name + ".";
            }
            foreach (Field field in view.Fields.Values.Where(f => f.FieldType == FieldType.Column))
            {
                LoadValue(values, dataRow, view, field, dynastyPath, prefix, postfix, dicFields, internalDynastyPath);
            }

            var childrenFields = view.Fields.Values.Where(f => f.FieldType == FieldType.Children && ((ChildrenField)f).LoadForBlockTemplate);
            foreach (ChildrenField field in childrenFields)
            {
                string name = prefix + dynastyPath + field.DisplayName + postfix;
                string internalName = prefix + internalDynastyPath + field.Name + postfix;
                DataView value = GetDataView(field, dataRow);
                if (!values.ContainsKey(name))
                {
                    values.Add(name, value);
                    dicFields.Add(internalDynastyPath, new Durados.Workflow.DictionaryField { DisplayName = field.DisplayName, Type = field.DataType, Value = value });
                }

                foreach (ColumnField columnField in field.ChildrenView.Fields.Values.Where(f => f.FieldType == FieldType.Column))
                {
                    if (columnField.Upload != null)
                    {
                        value.Table.Columns[columnField.Name].ExtendedProperties["ImagePath"] = columnField.GetUploadPath();
                    }
                }
            }

            foreach (ParentField field in view.Fields.Values.Where(f => f.FieldType == FieldType.Parent))
            {
                if (view.Equals(rootView))
                {
                    dynastyPath = view.DisplayName + ".";
                    internalDynastyPath = view.Name + ".";
                }
                LoadValue(values, dataRow, view, field, dynastyPath, prefix, postfix, dicFields, internalDynastyPath);



                DataRow parentRow = dataRow.GetParentRow(field.DataRelation.RelationName);
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
        protected virtual DataView GetDataView(ChildrenField childrenField, DataRow dataRow)
        {
            return childrenField.GetDataView(dataRow);
        }

        protected virtual string GetInternalDynastyPath(string dynastyPath, ParentField parentField, ParentField field)
        {
            if (parentField == null)
                return dynastyPath + field.Name + ".";

            string[] s = dynastyPath.Split('.');

            for (int i = s.Length - 1; i >= 0; i--)
            {
                if (s[i] == parentField.Name)
                {
                    string r = string.Empty;
                    for (int j = 0; j <= i; j++)
                    {
                        r += s[j] + ".";
                    }
                    return r + field.Name + ".";
                }
            }

            return dynastyPath += field.Name + ".";
        }
        protected string GetDynastyPath(string dynastyPath, ParentField parentField, ParentField field)
        {
            if (parentField == null)
                return dynastyPath + field.DisplayName + ".";

            string[] s = dynastyPath.Split('.');

            for (int i = s.Length - 1; i >= 0; i--)
            {
                if (s[i] == parentField.DisplayName)
                {
                    string r = string.Empty;
                    for (int j = 0; j <= i; j++)
                    {
                        r += s[j] + ".";
                    }
                    return r + field.DisplayName + ".";
                }
            }

            return dynastyPath += field.DisplayName + ".";
        }

    }

    public class SchemaGenerator
    {
        protected virtual Durados.Web.Mvc.View GetView(Map map, string viewName)
        {
            Durados.Web.Mvc.View view = (Durados.Web.Mvc.View)map.Database.GetViewByJsonName(viewName);
            if (view != null)
                return view;

            return ViewHelper.GetViewForRest(viewName);
        }
        private Dictionary<string, string> GetViewsNames(Map map)
        {
            Dictionary<string, string> viewsNames = new Dictionary<string, string>();

            foreach (View view in map.Database.Views.Values.Where(v => !v.SystemView))
            {
                viewsNames.Add(view.Name, view.Name);
            }
            return viewsNames;
        }
        public void Validate(Map map, IEnumerable<object> tables)
        {
            View view = GetView(map, "View");

            List<Dictionary<string, object>> fks = new List<Dictionary<string, object>>();

            Dictionary<string, string> viewNames = GetViewsNames(map);

            foreach (Dictionary<string, object> values in tables)
            {
                //Dictionary<string, object> values = view.Deserialize(table);


                if (!values.ContainsKey("DisplayName") && values.ContainsKey("name"))
                {
                    values.Add("DisplayName", values["name"].ToString().Replace('_', ' '));
                }
                if (!values.ContainsKey("WorkspaceID"))
                {
                    values.Add("WorkspaceID", "0");
                }



                ValidateInput(values);
            }
        }
        const string Name = "Name";

        private void ValidateInput(Dictionary<string, object> values)
        {
            if (!values.ContainsKey("name"))
            {
                throw new Durados.DuradosException("The table must have a name.");
            }
            if (!values.ContainsKey("fields"))
            {
                throw new Durados.DuradosException("The table " + values["name"].ToString() + " must have at least one field. Please make sure that the json input has a fields property.");
            }

            System.Collections.IEnumerable fields = (System.Collections.IEnumerable)values["fields"];
            foreach (Dictionary<string, object> field in fields)
            {
                if (!field.ContainsKey("name") && !field.ContainsKey(Name))
                {
                    throw new Durados.DuradosException("The field must have a name.");
                }

                if (!field.ContainsKey("type") && !field.ContainsKey("DataType"))
                {
                    throw new Durados.DuradosException("The field must have a type. The type can be one of the following: ShortText, LongText, Image, Url, Numeric, Boolean, DateTime, SingleSelect and MultiSelect");
                }

                if (string.IsNullOrEmpty(field["type"] as string))
                {
                    throw new Durados.DuradosException("The field type can not not be null or empty. The type can be one of the following: ShortText, LongText, Image, Url, Numeric, Boolean, DateTime, SingleSelect and MultiSelect");
                }
                if (!Enum.GetNames(typeof(Durados.DataType)).Where(n => n.ToLower() != "html" && n.ToLower() != "imagelist" && n.ToLower() != "email").Contains(field["type"].ToString()))
                {
                    throw new Durados.DuradosException("The field type [" + field["type"].ToString() + "]  is not defined. The type can be one of the following: ShortText, LongText, Image, Url, Numeric, Boolean, DateTime, SingleSelect and MultiSelect");
                }

            }

        }
    }



    public class AppNotReadyException : DuradosException
    {
        public AppNotReadyException(string appName)
            : base(string.Format("App is not ready yet: {0}", appName))
        {

        }

    }

    public class FilterException : DuradosException
    {
        public FilterException(string message)
            : base(message)
        {

        }

        public FilterException(string message, Exception innerException)
            : base(message, innerException)
        {

        }
    }

    public class SortException : DuradosException
    {
        public SortException(string message)
            : base(message)
        {

        }

        public SortException(string message, Exception innerException)
            : base(message, innerException)
        {

        }
    }

    public class bulk
    {

        public object[] Run(object[] requests, string authorization, string appName)
        {
            object[] responses = new object[requests.Length];

            ServicePointManager.DefaultConnectionLimit = 20;//Please test different numbers here
            var tasks = new List<Task<string>>();

            for (int index = 0; index < requests.Length; index++)
            {
                Dictionary<string, object> request = (Dictionary<string, object>)requests[index];


                string method = "GET";
                if (request.ContainsKey("method"))
                {
                    method = (string)request["method"];
                }
                if (!request.ContainsKey("url"))
                {
                    responses[index] = new ResponseStatusAndData() { status = 417, data = "Missing url in the request" };
                    continue;
                }
                string url = (string)request["url"];
                Dictionary<string, object> parameters = null;
                if (request.ContainsKey("parameters"))
                {
                    parameters = (Dictionary<string, object>)request["parameters"];
                }
                string data = null;
                if (request.ContainsKey("data"))
                {
                    data = new System.Web.Script.Serialization.JavaScriptSerializer().Serialize((Dictionary<string, object>)request["data"]);
                }

                Dictionary<string, object> headers = null;
                if (request.ContainsKey("headers"))
                {
                    headers = (Dictionary<string, object>)request["headers"];
                }
                if (headers == null)
                {
                    headers = new Dictionary<string, object>();
                }
                if (authorization != null)
                {

                    headers.Add("Authorization", authorization);
                }
                if (!string.IsNullOrEmpty(appName))
                {
                    headers.Add("appname", appName);
                }

                int index2 = index;
                //Maps.Instance.DuradosMap.Logger.Log("FarmCaching", "RefreshCash", "RunBulkIterate", null, 3, string.Format("method:{0}, url:{1},  headers:{2}", method, url,   string.Join(";",headers.Select(x=>x.Key+"="+x.Value ).ToArray())));
                request.Add("headers2", headers);

                tasks.Add(Task.Factory.StartNew(() =>
                {
                    var responseStatusAndData = GetWebResponse(method, url, data, parameters, headers, index2);
                    responses[responseStatusAndData.index] = responseStatusAndData;
                    return responseStatusAndData.data;
                }));



                //At this point tasks[0].Result will be the result (The Response) of the first task
                //tasks[1].Result will be the result of the second task and so on.

            }
            try
            {
                Task.WaitAll(tasks.ToArray());
                for (int index3 = 0; index3 < responses.Length; index3++)
                {
                    if (((int)HttpStatusCode.OK) != ((ResponseStatusAndData)responses[index3]).status && ((int)HttpStatusCode.NotFound) != ((ResponseStatusAndData)responses[index3]).status)
                    {
                        string msg = string.Format("Config cash refresh return {0} to server {1} ,requsts headers :{2}"
                            , ((ResponseStatusAndData)responses[index3]).status.ToString()
                            , ((Dictionary<string, object>)requests[index3])["url"].ToString()
                            , string.Join(";", ((Dictionary<string, object>)((Dictionary<string, object>)requests[index3])
                                ["headers2"]).Select(x => x.Key + "=" + x.Value).ToArray()));

                        throw new DuradosException(msg);
                    }
                }
            }
            catch (Exception ex)
            {
                string message = ex.Message + (ex.InnerException != null ? "." + ex.InnerException.Message : string.Empty);
                Maps.Instance.DuradosMap.Logger.Log(appName, "RefreshCash", "Task.Run", ex, 1, "");
                if (ex.InnerException != null)
                    Maps.Instance.DuradosMap.Logger.Log("FarmCaching", "RefreshCash", "Task.Run", ex.InnerException, 1, "");
                //  throw new DuradosException("failed to refresh farm instances cash." );
            }
            return responses;
        }

        private void setRequestHeader(WebRequest request, Dictionary<string, object> headers)
        {
            foreach (string header in headers.Keys)
            {
                setRequestHeader(request, header, headers[header].ToString());
            }
        }
        private void setRequestHeader(WebRequest request, string header, string value)
        {
            if (header.ToLower() == "content-type" || header.ToLower() == "accept")
                request.ContentType = value;
            else
                request.Headers.Add(header, value);

        }

        public ResponseStatusAndData GetWebResponse(string method, string url, string data, Dictionary<string, object> parameters, Dictionary<string, object> headers, int index)
        {
            if (parameters != null)
            {
                var sb = new StringBuilder();
                foreach (var key in parameters.Keys)
                    sb.Append(key + "=" + parameters[key] + "&");

                if (url.Contains("?"))
                {
                    url += "&";
                }
                else
                {
                    url += "?";
                }

                url += sb.ToString();
            }

            var httpWebRequest = (HttpWebRequest)WebRequest.Create(url);
            httpWebRequest.ContentType = "application/x-www-form-urlencoded";
            httpWebRequest.Method = method;

            if (headers != null)
            {
                setRequestHeader(httpWebRequest, headers);
            }

            if (!string.IsNullOrEmpty(data) && data != "null")
            {
                byte[] bytes;
                bytes = System.Text.Encoding.UTF8.GetBytes(data);
                httpWebRequest.ContentLength = bytes.Length;

                using (Stream requestStream = httpWebRequest.GetRequestStream())
                {
                    //Writes a sequence of bytes to the current stream 
                    requestStream.Write(bytes, 0, bytes.Length);
                    requestStream.Close();//Close stream
                }
            }

            Task<WebResponse> responseTask = null;
            try
            {
                responseTask = Task.Factory.FromAsync<WebResponse>(httpWebRequest.BeginGetResponse, httpWebRequest.EndGetResponse, null);
            }
            catch (Exception exception)
            {
                int status = 500;
                string message = exception.Message;
                if (exception is System.Net.WebException)
                {
                    status = (int)((System.Net.HttpWebResponse)(((System.Net.WebException)(exception)).Response)).StatusCode;
                }
                return new ResponseStatusAndData() { status = status, data = message, index = index };
            }

            try
            {
                using (var responseStream = responseTask.Result.GetResponseStream())
                {
                    var reader = new StreamReader(responseStream);
                    return new ResponseStatusAndData() { status = (int)((HttpWebResponse)responseTask.Result).StatusCode, data = reader.ReadToEnd(), index = index, headers = ((HttpWebResponse)responseTask.Result).Headers };
                }
            }
            catch (Exception exception)
            {
                int status = 500;

                Exception e = exception;
                string message = null;

                while (e.InnerException != null)
                {
                    e = e.InnerException;

                    if (e is WebException)
                    {
                        WebException webException = e as WebException;
                        if (webException.Response != null)
                        {
                            status = (int)((System.Net.HttpWebResponse)webException.Response).StatusCode;

                            try
                            {
                                using (var responseStream = ((System.Net.HttpWebResponse)webException.Response).GetResponseStream())
                                {
                                    var reader = new StreamReader(responseStream);
                                    message = reader.ReadToEnd();
                                    reader.Close();
                                }
                            }
                            catch
                            {
                                message = webException.Message;
                            }


                        }
                    }
                }
                if (string.IsNullOrEmpty(message))
                    message = (exception.InnerException ?? exception).Message;

                return new ResponseStatusAndData() { status = status, data = message, index = index };
            }

        }

        public class ResponseStatusAndData
        {
            public int status { get; set; }
            public string data { get; set; }
            internal int index { get; set; }
            internal System.Collections.Specialized.NameValueCollection headers { get; set; }

            public System.Collections.Specialized.NameValueCollection GetHeaders()
            {
                return headers;
            }
        }

    }

    public enum UserValidationError
    {
        Valid,
        IncorrectUsernameOrPassword,
        LockedOut,
        UserDoesNotBelongToApp,
        NotRegistered,
        NotApproved,
        Custom,
        Unknown
    }

    public class UserValidationErrorMessages
    {
        public static readonly string IncorrectUsernameOrPassword = "The user name or password is incorrect.";
        public static readonly string LockedOut = "The user is locked because of too many wrong passwords attempts. please contact the administrator.";
        public static readonly string UserDoesNotBelongToApp = "The user does not belong to this app.";
        public static readonly string NotRegistered = "The user is not registered to this app.";
        public static readonly string NotApproved = "The user is not approved for this app.";
        public static readonly string Unknown = "The server is busy. Please contact your administrator or try again later.";
        public static readonly string InvalidGrant = "invalid_grant";
        public static readonly string AppNameNotSupplied = "The app name was not supplied.";
        public static readonly string AppNameNotExists = "The app {0} does not exist.";
        public static readonly string AccessTokenNotAllowedToApp = "The access token is not not allowed to app {0}, that was provided in the header";
        public static string AppLocked
        {
            get
            {
                return Maps.AppLockedMessage;
            }
        }
        public static readonly string MissingUserIdOrAppId = "Missing userId or appId.";
        public static readonly string WrongUserIdOrAppId = "Wrong userId or appId.";
        public static readonly string EnableKeysAccess = "Enable keys access.";


        public static readonly string MissingAccessToken = "Missing accessToken.";
        public static readonly string InvalidAccessToken = "Invalid accessToken.";
        public static readonly string WrongAppName = "Wrong appName.";
        public static readonly string InvalidRefreshToken = "Invalid refreshToken.";

    }

    public class wf : Durados.Workflow.INotifier, Durados.Workflow.IExecuter
    {
        protected Map map = null;
        public Map Map
        {
            get
            {
                if (map == null)
                    map = Maps.Instance.GetMap();
                map.OpenSshSession();
                return map;
            }
        }

        protected virtual string GetUserID()
        {
            return Map.Database.GetUserID();
        }

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
            if (view.Database.IsApi())
                return null;

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
                Map.Logger.Log("", "", ex.Source, ex, 1, "Save Message Board View name: " + view.Name + ", pk: " + pk);
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

        protected virtual string GetViewDisplayName(Durados.Web.Mvc.View view)
        {
            return view.DisplayName;
        }
        protected virtual DataView GetDataView(Durados.Web.Mvc.ChildrenField childrenField, DataRow dataRow)
        {
            return childrenField.GetDataView(dataRow);
        }
        protected virtual string GetDynastyPath(string dynastyPath, Durados.Web.Mvc.ParentField parentField, Durados.Web.Mvc.ParentField field)
        {
            if (parentField == null)
                return dynastyPath + field.DisplayName + ".";

            string[] s = dynastyPath.Split('.');

            for (int i = s.Length - 1; i >= 0; i--)
            {
                if (s[i] == parentField.DisplayName)
                {
                    string r = string.Empty;
                    for (int j = 0; j <= i; j++)
                    {
                        r += s[j] + ".";
                    }
                    return r + field.DisplayName + ".";
                }
            }

            return dynastyPath += field.DisplayName + ".";
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
        protected virtual string GetInternalDynastyPath(string dynastyPath, Durados.Web.Mvc.ParentField parentField, Durados.Web.Mvc.ParentField field)
        {
            if (parentField == null)
                return dynastyPath + field.Name + ".";

            string[] s = dynastyPath.Split('.');

            for (int i = s.Length - 1; i >= 0; i--)
            {
                if (s[i] == parentField.Name)
                {
                    string r = string.Empty;
                    for (int j = 0; j <= i; j++)
                    {
                        r += s[j] + ".";
                    }
                    return r + field.Name + ".";
                }
            }

            return dynastyPath += field.Name + ".";
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

        protected virtual Durados.Web.Mvc.View GetView(string viewName)
        {
            Durados.Web.Mvc.View view = (Durados.Web.Mvc.View)Map.Database.GetViewByJsonName(viewName);
            if (view != null)
                return view;

            return ViewHelper.GetViewForRest(viewName);
        }
        public Durados.View GetNonConfigView(string viewName)
        {
            return GetView(viewName);
        }

        public string HtmlDecode(string text)
        {
            return System.Web.HttpUtility.HtmlDecode(text);
        }

        #endregion notifier

        #region executer
        public string[] GetFilterFieldValue(Durados.View view)
        {
            return null;
        }
        #endregion executer
    }

    public class DuradosAuthorizationHelper : wf
    {
        public bool IsAppExists(string appname)
        {
            if (Durados.Web.Mvc.Maps.Instance.AppInCach(appname))
                return true;
            else
                return Durados.Web.Mvc.Maps.Instance.AppExists(appname).HasValue;
        }

        public bool IsAppLocked(string appname)
        {
            return Durados.Web.Mvc.Maps.Instance.AppLocked(appname);
        }

        public bool IsValid(string appName, string username, string password, out UserValidationError userValidationError, out string customError, out bool hasCustomValidation, out bool customValid)
        {
            hasCustomValidation = false;
            customValid = false;
            if (HasCustomValidation())
            {
                hasCustomValidation = true;
                try
                {
                    bool? customValidation = CustomValidation(username, password, out userValidationError, out customError);

                    if (customValidation.HasValue)
                    {
                        customValid = true;
                        return customValidation.Value;
                    }
                }
                catch (Exception exception)
                {
                    map.Logger.Log("signin", "CustomValidation", "general", exception, 1, string.Empty);
                }
            }
            customError = null;
            Durados.Web.Mvc.Controllers.AccountMembershipService accountMembershipService = new Durados.Web.Mvc.Controllers.AccountMembershipService();
            bool valid = accountMembershipService.ValidateUser(username, password);
            if (valid)
            {
                userValidationError = UserValidationError.Valid;
            }
            else
            {
                if (map == null)
                {
                    map = Maps.Instance.GetMap(appName);
                }
                bool authenticated = accountMembershipService.AuthenticateUser(map, username, password);
                if (authenticated)
                {
                    bool belongToApp = accountMembershipService.ValidateUser(username);
                    if (belongToApp)
                    {
                        if (accountMembershipService.IsRegisterd(username))
                        {
                            userValidationError = UserValidationError.NotRegistered;
                        }
                        else
                        {
                            if (accountMembershipService.IsApproved(username))
                            {
                                userValidationError = UserValidationError.Unknown;
                            }
                            else
                            {
                                userValidationError = UserValidationError.NotApproved;
                            }
                        }
                    }
                    else
                    {
                        userValidationError = UserValidationError.UserDoesNotBelongToApp;
                    }
                }
                else
                {
                    if (accountMembershipService.IsLockedOut(username))
                    {
                        userValidationError = UserValidationError.LockedOut;
                    }
                    else if (!accountMembershipService.IsApproved(username))
                    {
                        userValidationError = UserValidationError.NotApproved;
                    }
                    else
                    {
                        userValidationError = UserValidationError.IncorrectUsernameOrPassword;
                    }
                }
            }

            return valid;
        }


        public bool IsSocialCustomDeny(SocialProfile profile, out string customError)
        {
            customError = null;

            if (HasSocialCustomValidation(profile.appName))
            {
                try
                {
                    bool? customValidation = SocialCustomValidation(profile, out customError);

                    if (customValidation.HasValue)
                    {
                        return !customValidation.Value;
                    }
                }
                catch (Exception exception)
                {
                    map.Logger.Log("signin", "SocialCustomValidation", "general", exception, 1, string.Empty);
                }
            }


            return false;

            
        }

        public bool IsAccessFilterDeny(string appName, string username, out string customError)
        {
            customError = null;

            if (HasAccessFilterValidation(appName))
            {
                try
                {
                    bool? customValidation = AccessFilterValidation(appName, username, out customError);

                    if (customValidation.HasValue)
                    {
                        return !customValidation.Value;
                    }
                }
                catch (Exception exception)
                {
                    map.Logger.Log("access token", "AccessFilterValidation", "general", exception, 1, string.Empty);
                }
            }


            return false;


        }

        private bool? AccessFilterValidation(string appName, string username, out string customError)
        {
            customError = null;

            Dictionary<string, object> values = null;
            values = new Dictionary<string, object>();
            values.Add("username", username);
            
            
            Map map = Maps.Instance.GetMap(appName);
            View view = (View)map.Database.GetUserView();
            Durados.Web.Mvc.Workflow.Engine wfe = CreateWorkflowEngine();
            try
            {
                wfe.PerformActions(this, view, Durados.TriggerDataAction.OnDemand, values, null, null, map.Database.ConnectionString, Convert.ToInt32(map.Database.GetUserID()), map.Database.GetUserRole(), null, null, Durados.Database.CustomAccessFilterActionName);

            }
            catch (Exception exception)
            {
                customError = exception.Message;
                return false;
            }
            if (values.ContainsKey(Durados.Workflow.JavaScript.ReturnedValueKey))
            {
                var returnedValue = values[Durados.Workflow.JavaScript.ReturnedValueKey];
                if (returnedValue is IDictionary<string, object>)
                {
                    const string Result = "result";
                    const string Allow = "allow";
                    const string Deny = "deny";
                    const string Ignore = "ignore";
                    const string Message = "message";
                    IDictionary<string, object> result = (IDictionary<string, object>)returnedValue;
                    if (result.ContainsKey(Result))
                    {
                        if (result[Result].Equals(Ignore))
                        {
                            return null;
                        }
                        else if (result[Result].Equals(Allow))
                        {
                            

                        }
                        else if (result[Result].Equals(Deny))
                        {
                            if (result.ContainsKey(Message))
                            {
                                customError = result[Message].ToString();
                                return false;
                            }
                            else
                            {
                                customError = Database.CustomValidationActionName + " did not return the expected result. Missing " + Message + " field in the returned object";
                                return false;
                            }
                        }
                        else
                        {
                            customError = Database.CustomValidationActionName + " did not return the expected result. " + Result + " must return either \"" + Allow + "\", \"" + Deny + "\" or \"" + Ignore + "\".";
                            return false;
                        }
                    }
                    else
                    {
                        customError = Database.CustomValidationActionName + " did not return the expected result. Missing " + Result + " field in the returned object.";
                        return false;
                    }
                }
                else
                {
                    customError = Database.CustomValidationActionName + " did not return the expected result.";
                    return false;
                }

            }

            
            return true;

        }

        private bool HasAccessFilterValidation(string appName)
        {
            return HasCustomAction(appName, Database.CustomAccessFilterActionName, Maps.Instance.GetCode(Database.CustomAccessFilterActionFileName));
        }

        private bool? SocialCustomValidation(SocialProfile profile, out string customError)
        {
            customError = null;
            
            Dictionary<string, object> values = null;
            values = new Dictionary<string, object>();
            values.Add("username", profile.email);
            values.Add("firstName", profile.firstName);
            values.Add("lastName", profile.lastName);
            values.Add("provider", profile.additionalValues["provider"]);
            if (profile.additionalValues.ContainsKey("providerAccessToken"))
            {
                values.Add("providerAccessToken", profile.additionalValues["providerAccessToken"]);
            }
            values.Add("id", profile.id);
            
            foreach (string key in profile.additionalValues.Keys)
            {
                if (!values.ContainsKey(key))
                    values.Add(key, profile.additionalValues[key]);
            }

            Map map = Maps.Instance.GetMap(profile.appName);
            View view = (View)map.Database.GetUserView();
            string parameters = profile.parameters;
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
                    map.Logger.Log("signin", "SocialCustomValidation", "get parameters json", exception, 2, string.Empty);
                }
            }
            Durados.Web.Mvc.Workflow.Engine wfe = CreateWorkflowEngine();
            try
            {
                wfe.PerformActions(this, view, Durados.TriggerDataAction.OnDemand, values, null, null, map.Database.ConnectionString, Convert.ToInt32(map.Database.GetUserID()), map.Database.GetUserRole(), null, null, Durados.Database.CustomSocialValidationActionName);

            }
            catch (Exception exception)
            {
                customError = exception.Message;
                return false;
            }
            if (values.ContainsKey(Durados.Workflow.JavaScript.ReturnedValueKey))
            {
                var returnedValue = values[Durados.Workflow.JavaScript.ReturnedValueKey];
                if (returnedValue is IDictionary<string, object>)
                {
                    const string Result = "result";
                    const string Allow = "allow";
                    const string Deny = "deny";
                    const string Ignore = "ignore";
                    const string Message = "message";
                    const string AdditionalTokenInfo = "additionalTokenInfo";
                    IDictionary<string, object> result = (IDictionary<string, object>)returnedValue;
                    if (result.ContainsKey(Result))
                    {
                        if (result[Result].Equals(Ignore))
                        {
                            return null;
                        }
                        else if (result[Result].Equals(Allow))
                        {
                            if (result.ContainsKey(AdditionalTokenInfo))
                            {
                                //System.Web.HttpContext.Current.Items.Add(Durados.Database.CustomTokenAttrKey, result[AdditionalTokenInfo]);
                                IDictionary<string, object> additionalTokenInfo = (IDictionary<string, object>)result[AdditionalTokenInfo];
                                const string ForToken = "forToken";
                                var additionalValuesForToken = new Dictionary<string, object>();
                                if (!profile.additionalValues.ContainsKey(ForToken))
                                {
                                    profile.additionalValues.Add(ForToken, additionalValuesForToken);
                                }
                                else
                                {
                                    additionalValuesForToken = (Dictionary<string, object>)profile.additionalValues[ForToken];
                                }
                                foreach (string key in additionalTokenInfo.Keys)
                                {
                                    additionalValuesForToken.Add(key, additionalTokenInfo[key]);
                                }
                            }
                            
                        }
                        else if (result[Result].Equals(Deny))
                        {
                            if (result.ContainsKey(Message))
                            {
                                customError = result[Message].ToString();
                                return false;
                            }
                            else
                            {
                                customError = Database.CustomValidationActionName + " did not return the expected result. Missing " + Message + " field in the returned object";
                                return false;
                            }
                        }
                        else
                        {
                            customError = Database.CustomValidationActionName + " did not return the expected result. " + Result + " must return either \"" + Allow + "\", \"" + Deny + "\" or \"" + Ignore + "\".";
                            return false;
                        }
                    }
                    else
                    {
                        customError = Database.CustomValidationActionName + " did not return the expected result. Missing " + Result + " field in the returned object.";
                        return false;
                    }
                }
                else
                {
                    customError = Database.CustomValidationActionName + " did not return the expected result.";
                    return false;
                }

            }

            string username = null;
            if (values.ContainsKey("Username"))
            {
                username = values["Username"].ToString();
            }
            if (values.ContainsKey("username"))
            {
                username = values["username"].ToString();
            }

            if (System.Web.HttpContext.Current != null && System.Web.HttpContext.Current.Items.Contains(Database.Username) && System.Web.HttpContext.Current.Items[Database.Username] != username)
            {
                System.Web.HttpContext.Current.Items[Database.Username] = username;
            }

            string firstName = null;
            if (values.ContainsKey("FirstName"))
            {
                firstName = (string)values["FirstName"];
            }
            if (values.ContainsKey("firstName"))
            {
                firstName = (string)values["firstName"];
            }

            string lastName = null;
            if (values.ContainsKey("LastName"))
            {
                lastName = (string)values["LastName"];
            }
            if (values.ContainsKey("lastName"))
            {
                lastName = (string)values["lastName"];
            }

            string role = null;
            if (values.ContainsKey("Role"))
            {
                role = (string)values["Role"];
            }
            if (values.ContainsKey("UserRole_Parent"))
            {
                role = (string)values["UserRole_Parent"];
            }

            string password = null;
            if (values.ContainsKey("password"))
            {
                password = (string)values["password"];
            }
            if (values.ContainsKey("Password"))
            {
                password = (string)values["Password"];
            }

            if (!IsUserExist(username))
            {
                try
                {
                    View roleView = Map.Database.GetRoleView();
                    if (!string.IsNullOrEmpty(role) && roleView.GetDataRow(role) == null)
                    {
                        roleView.Create(new Dictionary<string, object>() { { "Name", role }, { "Description", role } });
                    }
                    AddUser(username, password, firstName, lastName, role);
                }
                catch (Exception exception)
                {
                    customError = "Failed to add user: " + exception.Message;
                    return false;
                }
            }
            return true;

        }

        protected virtual Durados.Web.Mvc.Workflow.Engine CreateWorkflowEngine()
        {
            return new Durados.Web.Mvc.Workflow.Engine();
        }

        public bool? CustomChangePassword(string username, string oldPassword, string newPassword, out UserValidationError userValidationError, out string customError)
        {
            customError = null;
            userValidationError = UserValidationError.Custom;

            Dictionary<string, object> values = null;
            values = new Dictionary<string, object>();
            values.Add("username", username);
            values.Add("oldPassword", oldPassword);
            values.Add("newPassword", newPassword);

            Map map = Maps.Instance.GetMap();
            View view = (View)map.Database.GetUserView();
            string parameters = System.Web.HttpContext.Current.Request.QueryString["parameters"];
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
                    map.Logger.Log("ChangePassword", "CustomChangePassword", "get parameters json", exception, 2, string.Empty);
                }
            }
            Durados.Web.Mvc.Workflow.Engine wfe = CreateWorkflowEngine();
            try
            {
                wfe.PerformActions(this, view, Durados.TriggerDataAction.OnDemand, values, null, null, map.Database.ConnectionString, Convert.ToInt32(map.Database.GetUserID()), map.Database.GetUserRole(), null, null, Durados.Database.ChangePasswordOverride);

            }
            catch (Exception exception)
            {
                customError = exception.Message;
                return false;
            }
            if (values.ContainsKey(Durados.Workflow.JavaScript.ReturnedValueKey))
            {
                var returnedValue = values[Durados.Workflow.JavaScript.ReturnedValueKey];
                if (returnedValue is IDictionary<string, object>)
                {
                    const string Result = "result";
                    const string Allow = "allow";
                    const string Deny = "deny";
                    const string Ignore = "ignore";
                    const string Message = "message";
                    IDictionary<string, object> result = (IDictionary<string, object>)returnedValue;
                    if (result.ContainsKey(Result))
                    {
                        if (result[Result].Equals(Ignore))
                        {
                            return null;
                        }
                        else if (result[Result].Equals(Allow))
                        {
                            
                        }
                        else if (result[Result].Equals(Deny))
                        {
                            if (result.ContainsKey(Message))
                            {
                                customError = result[Message].ToString();
                                return false;
                            }
                            else
                            {
                                customError = Database.CustomValidationActionName + " did not return the expected result. Missing " + Message + " field in the returned object";
                                return false;
                            }
                        }
                        else
                        {
                            customError = Database.CustomValidationActionName + " did not return the expected result. " + Result + " must return either \"" + Allow + "\", \"" + Deny + "\" or \"" + Ignore + "\".";
                            return false;
                        }
                    }
                    else
                    {
                        customError = Database.CustomValidationActionName + " did not return the expected result. Missing " + Result + " field in the returned object.";
                        return false;
                    }
                }
                else
                {
                    customError = Database.CustomValidationActionName + " did not return the expected result.";
                    return false;
                }

            }
            
            return true;
        }


        private bool? CustomValidation(string username, string password, out UserValidationError userValidationError, out string customError)
        {
            customError = null;
            userValidationError = UserValidationError.Custom;

            Dictionary<string, object> values = null;
            values = new Dictionary<string, object>();
            values.Add("username", username);
            values.Add("password", password);

            Map map = Maps.Instance.GetMap();
            View view = (View)map.Database.GetUserView();
            string parameters = System.Web.HttpContext.Current.Request.QueryString["parameters"];
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
                    map.Logger.Log("signin", "CustomValidation", "get parameters json", exception, 2, string.Empty);
                }
            }
            Durados.Web.Mvc.Workflow.Engine wfe = CreateWorkflowEngine();
            try
            {
                wfe.PerformActions(this, view, Durados.TriggerDataAction.OnDemand, values, null, null, map.Database.ConnectionString, Convert.ToInt32(map.Database.GetUserID()), map.Database.GetUserRole(), null, null, Durados.Database.CustomValidationActionName);

            }
            catch (Exception exception)
            {
                customError = exception.Message;
                return false;
            }
            if (values.ContainsKey(Durados.Workflow.JavaScript.ReturnedValueKey))
            {
                var returnedValue = values[Durados.Workflow.JavaScript.ReturnedValueKey];
                if (returnedValue is IDictionary<string, object>)
                {
                    const string Result = "result";
                    const string Allow = "allow";
                    const string Deny = "deny";
                    const string Ignore = "ignore";
                    const string Message = "message";
                    const string AdditionalTokenInfo = "additionalTokenInfo";
                    IDictionary<string, object> result = (IDictionary<string, object>)returnedValue;
                    if (result.ContainsKey(Result))
                    {
                        if (result[Result].Equals(Ignore))
                        {
                            return null;
                        }
                        else if (result[Result].Equals(Allow))
                        {
                            if (result.ContainsKey(AdditionalTokenInfo))
                            {
                                const string ForToken = "forToken";
                                var additionalValuesForToken = new Dictionary<string, object>();
                                additionalValuesForToken.Add(ForToken, result[AdditionalTokenInfo]);
                                System.Web.HttpContext.Current.Items.Add(Durados.Database.CustomTokenAttrKey, additionalValuesForToken);
                            }
                        }
                        else if (result[Result].Equals(Deny))
                        {
                            if (result.ContainsKey(Message))
                            {
                                customError = result[Message].ToString();
                                return false;
                            }
                            else
                            {
                                customError = Database.CustomValidationActionName + " did not return the expected result. Missing " + Message + " field in the returned object";
                                return false;
                            }
                        }
                        else
                        {
                            customError = Database.CustomValidationActionName + " did not return the expected result. " + Result + " must return either \"" + Allow + "\", \"" + Deny + "\" or \"" + Ignore + "\".";
                            return false;
                        }
                    }
                    else
                    {
                        customError = Database.CustomValidationActionName + " did not return the expected result. Missing " + Result + " field in the returned object.";
                        return false;
                    }
                }
                else
                {
                    customError = Database.CustomValidationActionName + " did not return the expected result.";
                    return false;
                }

            }

            if (values.ContainsKey("Username"))
            {
                username = values["Username"].ToString();
            }
            if (values.ContainsKey("username"))
            {
                username = values["username"].ToString();
            }

            if (System.Web.HttpContext.Current != null && System.Web.HttpContext.Current.Items.Contains(Database.Username) && System.Web.HttpContext.Current.Items[Database.Username] != username)
            {
                System.Web.HttpContext.Current.Items[Database.Username] = username;
            }

            string firstName = null;
            if (values.ContainsKey("FirstName"))
            {
                firstName = values["FirstName"].ToString();
            }
            if (values.ContainsKey("firstName"))
            {
                firstName = values["firstName"].ToString();
            }

            string lastName = null;
            if (values.ContainsKey("LastName"))
            {
                lastName = values["LastName"].ToString();
            }
            if (values.ContainsKey("lastName"))
            {
                lastName = values["lastName"].ToString();
            }

            string role = null;
            if (values.ContainsKey("Role"))
            {
                role = values["Role"].ToString();
            }
            if (values.ContainsKey("UserRole_Parent"))
            {
                role = values["UserRole_Parent"].ToString();
            }

            if (values.ContainsKey("password"))
            {
                password = values["password"].ToString();
            }
            if (values.ContainsKey("Password"))
            {
                password = values["Password"].ToString();
            }

            if (!IsUserExist(username))
            {
                try
                {
                    View roleView = Map.Database.GetRoleView();
                    if (!string.IsNullOrEmpty(role) && roleView.GetDataRow(role) == null)
                    {
                        roleView.Create(new Dictionary<string, object>() { { "Name", role }, { "Description", role } });
                    }
                    AddUser(username, password, firstName, lastName, role);
                }
                catch (Exception exception)
                {
                    customError = "Failed to add user: " + exception.Message;
                    return false;
                }
            }
            return true;

        }

        public static string GeneratePassword(int lowercase, int uppercase, int numerics)
        {
            string lowers = "abcdefghijklmnopqrstuvwxyz";
            string uppers = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            string number = "0123456789";

            Random random = new Random();

            string generated = "!";
            for (int i = 1; i <= lowercase; i++)
                generated = generated.Insert(
                    random.Next(generated.Length),
                    lowers[random.Next(lowers.Length - 1)].ToString()
                );

            for (int i = 1; i <= uppercase; i++)
                generated = generated.Insert(
                    random.Next(generated.Length),
                    uppers[random.Next(uppers.Length - 1)].ToString()
                );

            for (int i = 1; i <= numerics; i++)
                generated = generated.Insert(
                    random.Next(generated.Length),
                    number[random.Next(number.Length - 1)].ToString()
                );

            return generated.Replace("!", string.Empty);

        }


        private void AddUser(string username, string password = null, string firstName = null, string lastName = null, string role = null)
        {
            AccountService account = new AccountService(this);

            Map map = Maps.Instance.GetMap();
            string password2 = password;
            if (string.IsNullOrEmpty(password) || password.Length < 6)
                password2 = GeneratePassword(4, 4, 4);
            Durados.Web.Mvc.UI.Helpers.AccountService.SignUpResults signUpResults = account.SignUp(map.AppName, firstName ?? username.Split('@').FirstOrDefault(), lastName ?? string.Empty, username, role, true, password2, password2, false, null, null, null, null, null, null, null, null, null);

        }

        private bool IsUserExist(string username)
        {
            Map map = Maps.Instance.GetMap();
            return map.Database.GetUserRow(username) != null;
        }

        private bool HasSocialCustomValidation(string appName)
        {
            return HasCustomAction(appName, Database.CustomSocialValidationActionName, Maps.Instance.GetCode(Database.CustomSocialValidationActionFileName));
        }

        private bool HasCustomValidation()
        {
            return HasCustomAction(Database.CustomValidationActionName, Map.EmptyCode);
        }

        private static bool HasCustomAction(string actionName, string emptyCode)
        {
            Map map = Maps.Instance.GetMap();
            return HasCustomAction(map.AppName, actionName, emptyCode);
        }

        private static bool HasCustomAction(string appName, string actionName, string emptyCode)
        {
            Map map = Maps.Instance.GetMap(appName);
            return map.HasRule(actionName) && (map.GetRule(actionName).Code != emptyCode);
        }
        public bool HasCustomChangePassword()
        {
            return false;
            //return HasCustomAction(Database.ChangePasswordOverride, Map.EmptyChangePasswordCode);
        }

        public bool ValidateLogOnAuthUrl(Durados.Web.Mvc.Map map, System.Collections.Specialized.NameValueCollection formCollecion)
        {
            try
            {
                System.Collections.Specialized.NameValueCollection nameValueCollection = new System.Collections.Specialized.NameValueCollection();

                foreach (string nv in map.Database.LogOnUrlAuthToken)
                {
                    nameValueCollection.Add(nv, formCollecion[nv]);
                }
                nameValueCollection.Add("username", formCollecion["username"]);

                string externalValidetionIdentificationKey = System.Configuration.ConfigurationManager.AppSettings["ExternalValidetionIdentificationKey"] ?? "auth";
                string externalValidetionIdentificationValue = System.Configuration.ConfigurationManager.AppSettings["ExternalValidetionIdentificationValue"] ?? true.ToString();

                nameValueCollection.Add(externalValidetionIdentificationKey, externalValidetionIdentificationValue);


                string url = map.Database.LogOnUrlAuthBase + AsEncodedQueryString(nameValueCollection);

                System.Net.HttpWebRequest request = System.Net.WebRequest.Create(url) as System.Net.HttpWebRequest;
                request.Method = Durados.DataAccess.HttpVerb.GET.ToString();

                using (System.Net.HttpWebResponse response = request.GetResponse() as System.Net.HttpWebResponse)
                {
                    if (response.StatusCode != System.Net.HttpStatusCode.OK)
                        throw new System.Exception(System.String.Format(
                        "Server error (HTTP {0}: {1}).",
                        response.StatusCode,
                        response.StatusDescription));
                    System.Web.Script.Serialization.JavaScriptSerializer jsonSerializer = new System.Web.Script.Serialization.JavaScriptSerializer();
                    string responseContent = new System.IO.StreamReader(response.GetResponseStream()).ReadToEnd();

                    SuccessMessageResponse validateResponse = (SuccessMessageResponse)jsonSerializer.Deserialize<SuccessMessageResponse>(responseContent); ;

                    if (validateResponse.Success)
                        return true;
                    else
                    {
                        //ModelState.AddModelError("_FORM", Map.Database.Localizer.Translate(validateResponse.Message));
                        return false;
                    };
                }
                // }
            }
            catch (System.Exception ex)
            {
                //map.Logger.Log(this.ControllerContext.RouteData.Values["controller"].ToString(), this.ControllerContext.RouteData.Values["action"].ToString(), "ValidateLogOnAuthUrl", "Fail to validate through LogOnAuthUrl", ex.StackTrace, 3, "", DateTime.Now);
                //ModelState.AddModelError("_FORM", Map.Database.Localizer.Translate(ex.Message));
                return false;
            }

        }

        private string AsEncodedQueryString(System.Collections.Specialized.NameValueCollection nvc)
        {
            return "?" + System.String.Join("&", from item in nvc.AllKeys select item + "=" + nvc[item]);

        }
        public class SuccessMessageResponse
        {

            public bool Success { get; set; }

            public string Message { get; set; }
        }


    }

    public class LambdaSelection
    {
        public int cloudId { get; set; }
        public string name { get; set; }
        public string arn { get; set; }
        public string description { get; set; }
        public Dictionary<string, object> lambdaProperties { get; set; }
        public string friendlyName { get; set; }
        public bool select { get; set; }
    }

    public class LambdaSelectionResult : LambdaSelection
    {
        public bool result { get; set; }
        public string error { get; set; }
        public int functionId { get; set; }
    }

    public class LambdaHelper
    {
        public Dictionary<string, object> Get()
        {
            Dictionary<string, object> json = new Dictionary<string, object>();

            json.Add("totalRows", json.Count);

            Dictionary<string, object>[] cloudsJson = GetClouds();

            foreach (Dictionary<string, object> cloudJson in cloudsJson)
            {
                int cloudId = Convert.ToInt32(cloudJson["id"]);
                try
                {
                    cloudJson["functions"] = GetLambdaListByGroups(Maps.Instance.GetMap().Database.Clouds[cloudId]); 
                }
                catch (Durados.Workflow.NodeJsLambdaListException ex)
                {
                    cloudJson.Add("error", new { message = ex.Message });
                }
                
            }
            
            json["totalRows"] = cloudsJson.Count();

            json.Add("data", cloudsJson);

            return json;
        }

        private Dictionary<string, object>[] GetClouds()
        {
            List<Dictionary<string, object>> cloudsJson = new List<Dictionary<string,object>>();
            
            foreach (Cloud cloud in Maps.Instance.GetMap().Database.Clouds.Values)
            {
                cloudsJson.Add(new Dictionary<string, object>() { { "id", cloud.Id }, { "name", cloud.Name }, { "accessKeyId", cloud.GetCloudDescriptor() }, { "cloudVendor", cloud.CloudVendor.ToString() }, { "functions", null } });
            }

            return cloudsJson.ToArray();
        }

        private Dictionary<string, object> GetLambdaListByGroups(Cloud cloud)
        {
            Dictionary<string, object> groups = new Dictionary<string, object>();
                            
            Durados.Workflow.NodeJS nodejs = new Durados.Workflow.NodeJS();

            Durados.Security.Cloud.ICloudCredentials[] credentials = cloud.GetCloudCredentials();
            foreach (Durados.Security.Cloud.ICloudCredentials credential in credentials)
            {
                
                Dictionary<string, Dictionary<string, object>[]>  funcListGroups = GetLambdaList(nodejs, credential);
                foreach (var funcList in funcListGroups)
                {
                    if (!groups.Keys.Contains(funcList.Key))
                            groups.Add(funcList.Key, funcList.Value);
                }
                
            }

            //Durados.Security.Aws.AwsCredentials credential = cloud.GetAwsCredentials();
            //regions.Add(credential.Region, nodejs.GetLambdaList(credential));

            return groups;
        }

     


        private Dictionary<string, Dictionary<string, object>[]> GetLambdaList(Durados.Workflow.NodeJS nodejs, Durados.Security.Cloud.ICloudCredentials credential)
        {
            var lambdaList = nodejs.GetLambdaList(credential);

           credential.Cloud.SetSelectedFunctions(lambdaList.Values, functionView);

            return lambdaList;
        }

        private void SetSelectedFunctions(Dictionary<string, Dictionary<string, object>[]>.ValueCollection valueCollection)
        {
            foreach (var lambdaList in valueCollection)
            {
                foreach (var lambdaFunction in lambdaList)
                {
                    const string FunctionId = "functionId";
                    const string ARN = "FunctionArn";
                    const string SELECTED = "selected";
                    if (!lambdaFunction.ContainsKey(ARN))
                        throw new DuradosException("ORM did not return lambda list with FunctionArn");
                    string arn = lambdaFunction[ARN].ToString();
                    Rule rule = GetRuleByArn(arn);
                    bool selected = (rule != null);
                    lambdaFunction.Add(SELECTED, selected);
                    if (rule != null)
                        lambdaFunction.Add(FunctionId, rule.ID);
                }
            }
        }

        

        

        private void SetSelectedFunctions( Dictionary<string, object>[] lambdaList)
        {
            foreach (var lambdaFunction in lambdaList)
            {
                const string FunctionId = "functionId";
                const string ARN = "FunctionArn";
                const string SELECTED = "selected";
                if (!lambdaFunction.ContainsKey(ARN))
                    throw new DuradosException("ORM did not return lambda list with FunctionArn");
                string arn = lambdaFunction[ARN].ToString();
                Rule rule = GetRuleByArn(arn);
                bool selected = (rule != null);
                lambdaFunction.Add(SELECTED, selected);
                if (rule != null)
                    lambdaFunction.Add(FunctionId, rule.ID);
            }
        }


        public LambdaSelectionResult[] Select(LambdaSelection[] selections, BeforeCreateEventHandler beforeCreateCallback, BeforeCreateInDatabaseEventHandler beforeCreateInDatabaseEventHandler, AfterCreateEventHandler afterCreateBeforeCommitCallback, AfterCreateEventHandler afterCreateAfterCommitCallback)
        {
            List<LambdaSelectionResult> results = new List<LambdaSelectionResult>();

            foreach (LambdaSelection selection in selections)
            {
                results.Add(Select(selection, beforeCreateCallback, beforeCreateInDatabaseEventHandler, afterCreateBeforeCommitCallback, afterCreateAfterCommitCallback));
            }

            return results.ToArray();
        }

        private LambdaSelectionResult Select(LambdaSelection selection, BeforeCreateEventHandler beforeCreateCallback, BeforeCreateInDatabaseEventHandler beforeCreateInDatabaseEventHandler, AfterCreateEventHandler afterCreateBeforeCommitCallback, AfterCreateEventHandler afterCreateAfterCommitCallback)
        {
            LambdaSelectionResult result = new LambdaSelectionResult() { cloudId = selection.cloudId, name = selection.name };
            
            Dictionary<int,Cloud> clouds = Maps.Instance.GetMap().Database.Clouds;
            if (clouds == null || clouds.Count() == 0 || !clouds.ContainsKey(selection.cloudId) || clouds[selection.cloudId] == null)
                 throw new FunctionCloudNotExists(selection.name, selection.cloudId);

            Cloud cloud = clouds[selection.cloudId];          
            int? id;
            try
            {
                if (selection.select)
                {
                    id = CreateAction(selection, cloud, beforeCreateCallback, beforeCreateInDatabaseEventHandler, afterCreateBeforeCommitCallback, afterCreateAfterCommitCallback);
                    result.select = true;
                }
                else
                {
                    

                    id = DeleteAction(selection, cloud);
                    result.select = false;
                }
                result.result = true;
                result.functionId = id.Value;
            }
            catch (Exception e)
            {
                result.result = false;
                result.error = e.Message;
            }

            return result;
        }

        View ruleView = (View)Maps.Instance.GetMap().GetConfigDatabase().Views["Rule"];
        View functionView = (View)Maps.Instance.GetMap().Database.Views["_root"];


        private int? DeleteAction(LambdaSelection selection, Cloud cloud)
        {
            if (string.IsNullOrEmpty(selection.arn))
                throw new LambdaFunctionSelectionNotContainsArn(selection.name);

            Rule rule = cloud.GetRuleByArn(selection.arn,selection.name, cloud.Id, functionView);

            if (rule == null)
                throw new LambdaFunctionSelectionNotFound(selection.name);

            ruleView.Delete(rule.ID.ToString(), null, null, null);

            return rule.ID;

        }

        private int? CreateAction(LambdaSelection selection, Cloud cloud, BeforeCreateEventHandler beforeCreateCallback, BeforeCreateInDatabaseEventHandler beforeCreateInDatabaseEventHandler, AfterCreateEventHandler afterCreateBeforeCommitCallback, AfterCreateEventHandler afterCreateAfterCommitCallback)
        {
            if (string.IsNullOrEmpty(selection.arn))
                throw new LambdaFunctionSelectionNotContainsArn(selection.name);

            Rule rule = GetRuleByArn(selection);

            if (rule != null)
                throw new LambdaFunctionSelectionAlreadyExists(selection.name);

            rule = GetRuleByName(selection.name);

            string newName = selection.name;
            if (rule != null)
            {
                newName = GetUniqueName(newName);
            }

            string functionObject = GetFilteredFunctionObject(selection, cloud);

            
            Dictionary<string, object> values = new Dictionary<string, object>();

            values.Add("Name", newName);
            values.Add("LambdaName", selection.name);
            values.Add("LambdaArn", selection.arn);
            values.Add("LambdaProperties", functionObject);
            values.Add("CloudSecurity", selection.cloudId);
            values.Add("ActionType", ActionType.Function.ToString());
            values.Add("WorkflowAction", WorkflowAction.Lambda.ToString());
            values.Add("DataAction", TriggerDataAction.OnDemand.ToString());
            values.Add("Rules_Parent", functionView.ID.ToString());
            values.Add("AdditionalView", "");
            values.Add("DatabaseViewName", "");
            values.Add("UseSqlParser", "false");
            values.Add("WhereCondition", "true");
            values.Add("Category", "general");
            values.Add("WorkspaceID", 0);
            values.Add("Description", selection.description);
                
            var dataRow = ruleView.Create(values, null, beforeCreateCallback, beforeCreateInDatabaseEventHandler, afterCreateBeforeCommitCallback, afterCreateAfterCommitCallback);

            return System.Convert.ToInt32(ruleView.GetPkValue(dataRow));
        }

        private static string GetFilteredFunctionObject(LambdaSelection selection, Cloud cloud)
        {
            string functionObject = null;
            Dictionary<string, object> functionObjectDic = cloud.GetFunctionObject(selection.lambdaProperties);
            if (functionObjectDic != null && functionObjectDic.Count > 0)
                functionObject = new System.Web.Script.Serialization.JavaScriptSerializer().Serialize(functionObjectDic);//Json.JsonSerializer.Serialize(functionObjectDic);
            return functionObject;
        }

        private string GetUniqueName(string newName)
        {
            for (int i = 1; i < 1000; i++)
            {
                if (GetRuleByName(newName + i) == null)
                    return newName + i;
            }

            throw new DuradosException("Over 1000 names");
        }

        private Rule GetRuleByArn(LambdaSelection selection)
        {
            return GetRuleByArn(selection.arn);
        }

        private Rule GetRuleByArn(string arn)
        {
            return functionView.GetRules().Where(r => r.LambdaArn == arn).FirstOrDefault();
        }

        private Rule GetRuleByName(string name)
        {
            return functionView.GetRules().Where(r => r.Name == name).FirstOrDefault();
        }

    }

    public class GoogleResult
    {
        public string issued_to { get; set; }
        public string audience { get; set; }
        public string user_id { get; set; }
        public string scope { get; set; }
        public int expires_in { get; set; }
        public string email { get; set; }
        public bool verified_email { get; set; }
        public string access_type { get; set; }
    }

    public class NoSqlFilterCache
    {
        static string key = "NoSqlFilter";
        static NoSqlFilterCache()
        {

        }

        private static Map GetMap(string appName)
        {
            Map map = null;

            if (appName == Maps.DuradosAppName)
            {
                map = Maps.Instance.DuradosMap;
            }
            else
            {
                map = Maps.Instance.GetMap(appName);
                if (map == null || map == Maps.Instance.DuradosMap)
                {
                    throw new AppNotFoundException(appName);
                }
            }

            return map;
        }

        public static Dictionary<string, object> Get(string appName, View view, string json)
        {
            Map map = GetMap(appName);
            if (!map.AllKindOfCache.Contains(key))
            {
                map.AllKindOfCache[key] = new MemoryCache(key);
            }

            MemoryCache NoSqlFilterCache = (MemoryCache)map.AllKindOfCache[key];
            if (!NoSqlFilterCache.Contains(view.JsonName))
            {
                ((MemoryCache)NoSqlFilterCache)[view.JsonName] = new MemoryCache(view.JsonName);
                return null;
            }
            if (((MemoryCache)((MemoryCache)NoSqlFilterCache)[view.JsonName]).Contains(System.Web.Helpers.Crypto.SHA256(json)))
            {
                return (Dictionary<string, object>)((MemoryCache)((MemoryCache)NoSqlFilterCache)[view.JsonName])[System.Web.Helpers.Crypto.SHA256(json)];
            }
            return null;

        }

        //public static Dictionary<string, object> Get(string appName, View view, string json)
        //{
        //    Map map = GetMap(appName);
        //    if (!map.AllKindOfCache.ContainsKey(key))
        //    {
        //        map.AllKindOfCache.Add(key, new Dictionary<string, object>());
        //    }
        //    if (!map.AllKindOfCache[key].ContainsKey(view.JsonName))
        //    {
        //        ((Dictionary<string, object>)map.AllKindOfCache[key]).Add(view.JsonName, new Dictionary<string, object>());
        //        return null;
        //    }
        //    if (((Dictionary<string, object>)((Dictionary<string, object>)map.AllKindOfCache[key])[view.JsonName]).ContainsKey(System.Web.Helpers.Crypto.SHA256(json)))
        //    {
        //        return (Dictionary<string, object>)((Dictionary<string, object>)((Dictionary<string, object>)map.AllKindOfCache[key])[view.JsonName])[System.Web.Helpers.Crypto.SHA256(json)];
        //    }
        //    return null;

        //}

        //public static void Set(string appName, View view, string json, Dictionary<string, object> result)
        //{
        //    Map map = GetMap(appName);
        //    if (!map.AllKindOfCache.ContainsKey(key))
        //    {
        //        map.AllKindOfCache.Add(key, new Dictionary<string, object>());
        //    }
        //    if (!map.AllKindOfCache[key].ContainsKey(view.JsonName))
        //    {
        //        ((Dictionary<string, object>)map.AllKindOfCache[key]).Add(view.JsonName, new Dictionary<string, object>());
        //    }
        //    if (!((Dictionary<string, object>)((Dictionary<string, object>)map.AllKindOfCache[key])[view.JsonName]).ContainsKey(System.Web.Helpers.Crypto.SHA256(json)))
        //    {
        //        ((Dictionary<string, object>)((Dictionary<string, object>)map.AllKindOfCache[key])[view.JsonName])
        //            .Add(System.Web.Helpers.Crypto.SHA256(json), result);
        //    }
        //}

        public static void Set(string appName, View view, string json, Dictionary<string, object> result)
        {
            Map map = GetMap(appName);
            if (!map.AllKindOfCache.Contains(key))
            {
                map.AllKindOfCache[key] = new MemoryCache(key);
            }
            MemoryCache NoSqlFilterCache = (MemoryCache)map.AllKindOfCache[key];

            if (!NoSqlFilterCache.Contains(view.JsonName))
            {
                ((MemoryCache)NoSqlFilterCache)[view.JsonName] = new MemoryCache(view.JsonName);
            }
            if (!((MemoryCache)((MemoryCache)NoSqlFilterCache)[view.JsonName]).Contains(System.Web.Helpers.Crypto.SHA256(json)))
            {
                ((Dictionary<string, object>)((MemoryCache)NoSqlFilterCache)[view.JsonName])
                    .Add(System.Web.Helpers.Crypto.SHA256(json), result);
            }
        }
    }

    public class ExternalAuthRefreshToken
    {
        public static bool IsExternalAuth(Map map, out string refreshToken)
        {
            refreshToken = null;
            if (map.Database.UseRefreshToken)
            {
                string refreshTokenAfterValidate = GetRefreshTokenAfterValidate();
                if (refreshTokenAfterValidate != null)
                {
                    refreshToken = refreshTokenAfterValidate;
                    return true;
                }

                AbstractSocialProvider social = null;
                if (IsSocial(map, SocialProviders.AzureAd))
                {
                    social = SocialProviderFactory.GetSocialProvider(SocialProviders.AzureAd.ToString().ToLower());

                }
                else if (IsSocial(map, SocialProviders.Adfs))
                {
                    social = SocialProviderFactory.GetSocialProvider(SocialProviders.Adfs.ToString().ToLower());

                }
                else
                {
                    return false;
                }

                string currentRefreshToken = GetCurrentRefreshToken();
                if (currentRefreshToken != null)
                {
                    SocialProfile profile = social.FetchProfileByRefreshToken(currentRefreshToken, map.AppName);
                    refreshToken = profile.additionalValues["refreshToken"].ToString();
                }
                return true;
            }
            else
            {
                return false;
            }
        }

        private static string GetRefreshTokenAfterValidate()
        {
            return System.Web.HttpContext.Current.Items["refreshToken"] == null ? null : System.Web.HttpContext.Current.Items["refreshToken"].ToString();
        }

        private static void SetRefreshTokenAfterValidate(string refreshToken)
        {
            System.Web.HttpContext.Current.Items["refreshToken"] = refreshToken;
        }

        public static bool IsExternalAuth(Map map, string refreshToken, out bool valid)
        {
            AbstractSocialProvider social = null;
            valid = false;
            if (IsSocial(map, SocialProviders.AzureAd))
            {
                social = SocialProviderFactory.GetSocialProvider(SocialProviders.AzureAd.ToString().ToLower());

            }
            else if (IsSocial(map, SocialProviders.Adfs))
            {
                social = SocialProviderFactory.GetSocialProvider(SocialProviders.Adfs.ToString().ToLower());

            }
            else
            {
                return false;
            }
            SocialProfile profile = social.FetchProfileByRefreshToken(refreshToken, map.AppName);
            if (!CompareRequestWithToken(profile.email))
            {
                throw new RefereshTokenException("The user in the token is different then the request.");
            }
            if (!UserBelongToApp(profile.email, map))
            {
                throw new RefereshTokenException("The user does not belong to the app.");
            }
            if (!IsApprovedUser(profile.email, map))
            {
                throw new RefereshTokenException("The user is not approved.");
            }
            
            string newRefreshToken = profile.additionalValues["refreshToken"].ToString();
            SetRefreshTokenAfterValidate(newRefreshToken);
            if (newRefreshToken != null)
            {
                valid = true;
            }

            return true;
        }
        static Durados.Web.Mvc.Controllers.AccountMembershipService accountMembershipService = new Durados.Web.Mvc.Controllers.AccountMembershipService();
            

        private static bool IsApprovedUser(string email, Map map)
        {
            return accountMembershipService.IsApproved(email);
        }

        private static bool UserBelongToApp(string email, Map map)
        {
            return accountMembershipService.ValidateUser(email);
        }

        private static bool CompareRequestWithToken(string email)
        {
            return System.Web.HttpContext.Current.Request.Form["username"].Equals(email);
        }

        private static string GetCurrentRefreshToken()
        {
            string oneTimeToken = GetOneTimeToken();
            return Durados.Web.Mvc.Farm.SharedMemorySingeltone.Instance.Get(oneTimeToken);
        }

        private static string GetOneTimeToken()
        {
            return System.Web.HttpContext.Current.Request.Form["accessToken"];
        }

        private static bool IsSocial(Map map, SocialProviders provider)
        {
            return map.Database.AzureAdClientId != null && provider.Equals(SocialProviders.AzureAd) ||
                map.Database.AdfsClientId != null && provider.Equals(SocialProviders.Adfs);
        }
    }

    public class RefereshTokenException : DuradosException
    {
        public RefereshTokenException(string message)
            : base(message)
        {

        }
    }

    public class RefreshToken
    {
        
        static string key = "RefreshToken";
        static RefreshToken()
        {
            //if (!Maps.Instance.DuradosMap.AllKindOfCache.ContainsKey(key))
            //{
            //    Maps.Instance.DuradosMap.AllKindOfCache.Add(key, new Dictionary<string, object>());
            //}
            if (!Maps.Instance.DuradosMap.AllKindOfCache.Contains(key))
            {
                Maps.Instance.DuradosMap.AllKindOfCache[key] = new MemoryCache(key);
            }
        }

        public static string Get(string appName, string username)
        {
            string refreshToken = null;
            Map map = GetMap(appName);
            if (ExternalAuthRefreshToken.IsExternalAuth(map, out refreshToken))
            {
                return refreshToken;
            }
            string appGuid = map.Guid.ToString();
            string userGuid = map.Database.GetGuidByUsername(username);
            refreshToken = System.Web.Helpers.Crypto.HashPassword(appGuid + userGuid);
            return refreshToken; 
        }

        

        private static Map GetMap(string appName)
        {
            Map map = null;

            if (appName == Maps.DuradosAppName)
            {
                map = Maps.Instance.DuradosMap;
            }
            else
            {
                map = Maps.Instance.GetMap(appName);
                if (map == null || map == Maps.Instance.DuradosMap)
                {
                    throw new AppNotFoundException(appName);
                }
            }

            return map;
        }

        public static bool Validate(string appName, string refreshToken, string username)
        {
            if (!((MemoryCache)Maps.Instance.DuradosMap.AllKindOfCache[key]).Contains(refreshToken))
            {
                Map map = GetMap(appName);
                if (!map.Database.UseRefreshToken && !map.Equals(Maps.Instance.DuradosMap))
                    return false;

                bool valid;
                if (ExternalAuthRefreshToken.IsExternalAuth(map, refreshToken, out valid))
                {
                    return valid;
                }
            
                string appGuid = map.Guid.ToString();
                string userGuid = map.Database.GetGuidByUsername(username);
                if (System.Web.Helpers.Crypto.VerifyHashedPassword(refreshToken, appGuid + userGuid))
                {
                    ((MemoryCache)Maps.Instance.DuradosMap.AllKindOfCache[key])[refreshToken] = new Dictionary<string, string>() { { "username", username }, { "appName", appName } };
                }
                else
                {
                    return false;
                }
            }
            return true;

            //if (!Maps.Instance.DuradosMap.AllKindOfCache[key].ContainsKey(refreshToken))
            //{
            //    Map map = GetMap(appName);
            //    if (!map.Database.UseRefreshToken && !map.Equals(Maps.Instance.DuradosMap))
            //        return false;

            //    bool valid;
            //    if (ExternalAuthRefreshToken.IsExternalAuth(map, refreshToken, out valid))
            //    {
            //        return valid;
            //    }

            //    string appGuid = map.Guid.ToString();
            //    string userGuid = map.Database.GetGuidByUsername(username);
            //    if (System.Web.Helpers.Crypto.VerifyHashedPassword(refreshToken, appGuid + userGuid))
            //    {
            //        Maps.Instance.DuradosMap.AllKindOfCache[key].Add(refreshToken, new Dictionary<string, string>() { { "username", username }, { "appName", appName } });
            //    }
            //    else
            //    {
            //        return false;
            //    }
            //}
            //return true;
        }

        public static void Clear()
        {
            Clear(Maps.Instance.GetMap().AppName);
        }
        public static void Clear(string appName)
        {
            Maps.Instance.DuradosMap.AllKindOfCache[key] = new MemoryCache(key);
            FarmCachingSingeltone.Instance.ClearMachinesCache(appName);
        }
    }

    public class UserRelation
    {
        public class UserRelationException : Durados.DuradosException
        {
            public UserRelationException(string message, Exception innerException) :
                base(message, innerException)
            {

            }
        }

        public class UserViewNotFound : UserRelationException
        {
            public UserViewNotFound() : base("The users object was not found", null) { }
        }

        public class EmailFieldNotFound : UserRelationException
        {
            public EmailFieldNotFound() : base("The email field was not found", null) { }
        }

        public class RelationNotFound : UserRelationException
        {
            public RelationNotFound() : base("A relation to users was not found", null) { }
        }

        const string SysUsername = "{{sys::username}}";
        const string SysRole = "{{sys::role}}";
        const string AdminRole = "Admin";
        private View GetUsersView(View view, string usersObjectName)
        {
            if (view.Name == usersObjectName)
                return view;
            else
                return GetUsersView(view.Database, usersObjectName);
        }
        private View GetUsersView(Database database, string usersObjectName)
        {
            if (database.Views.ContainsKey(usersObjectName))
                return (View)database.Views[usersObjectName];

            return null;
        }

        private bool HasEmailField(View userView, string emailFieldName)
        {
            return userView.Fields.ContainsKey(emailFieldName);
        }

        public Dictionary<string, object> GetWhere(View view, string usersObjectName, string emailFieldName, int maxLevel, bool showAllForAdmin)
        {
            View usersView = GetUsersView(view, usersObjectName);
            if (usersView == null)
                throw new UserViewNotFound();
            if (!HasEmailField(usersView, emailFieldName))
                throw new EmailFieldNotFound();

            string where = string.Empty;

            List<Field> fields = new List<Field>();

            Field field = GetUserRelatedField(usersView, view, fields, 0, maxLevel);
            if (field == null)
                throw new RelationNotFound();

            ISqlTextBuilder sqlTextBuilder = view.Database.GetSqlTextBuilder();

            string sql = GetWhere(fields, usersObjectName, emailFieldName, fields.Count - 1, string.Empty, sqlTextBuilder, showAllForAdmin);
            if (showAllForAdmin)
            {
                sql = "(" + string.Format(" '{0}' = '{1}'", AdminRole, SysRole) + ") or (" + sql + ")";
            }
            string nosql = "{" + GetNoSql(fields, usersObjectName, emailFieldName, fields.Count - 1, string.Empty, showAllForAdmin) + "}";
            return new Dictionary<string, object>() { { "sql", sql }, { "nosql", nosql } };
        }

        private string GetWhere(List<Field> fields, string usersObjectName, string emailFieldName, int i, string where, ISqlTextBuilder sqlTextBuilder, bool showAllForAdmin)
        {
            string next = null;
            if (i == 0)
            {
                next = string.Format("{0} = '{1}'", sqlTextBuilder.EscapeDbObject(usersObjectName) + sqlTextBuilder.DbTableColumnSeperator + sqlTextBuilder.EscapeDbObject(emailFieldName), SysUsername);

            }
            else
            {
                next = GetWhere(fields, usersObjectName, emailFieldName, i - 1, where, sqlTextBuilder, showAllForAdmin);
            }
            Field field = fields[i];
            if (field.FieldType == FieldType.Parent)
            {
                ParentField parentField = (ParentField)field;
                where += string.Format("{0} in (select {1} from {2}" + sqlTextBuilder.WithNolock + " where {3})", sqlTextBuilder.EscapeDbObject(parentField.View.Name) + sqlTextBuilder.DbTableColumnSeperator + sqlTextBuilder.EscapeDbObject(parentField.GetColumnsNames()[0]), sqlTextBuilder.EscapeDbObject(parentField.ParentView.Name) + sqlTextBuilder.DbTableColumnSeperator + sqlTextBuilder.EscapeDbObject(parentField.ParentView.GetPkColumnNames()[0]), sqlTextBuilder.EscapeDbObject(parentField.ParentView.Name), next);
            }
            else if (field.FieldType == FieldType.Children)
            {
                ChildrenField childrenField = (ChildrenField)field;
                //where += string.Format("{0} in (select {1} from {2}" + sqlTextBuilder.WithNolock + " inner join {3} on {4} = {5} where {6})", sqlTextBuilder.EscapeDbObject(childrenField.View.Name) + sqlTextBuilder.DbTableColumnSeperator + sqlTextBuilder.EscapeDbObject(childrenField.GetColumnsNames()[0]), sqlTextBuilder.EscapeDbObject(childrenField.ChildrenView.Name) + sqlTextBuilder.DbTableColumnSeperator + sqlTextBuilder.EscapeDbObject(childrenField.GetEquivalentParentField().GetColumnsNames()[0]), sqlTextBuilder.EscapeDbObject(childrenField.ChildrenView.Name), sqlTextBuilder.EscapeDbObject(childrenField.GetFirstNonEquivalentParentField().ParentView.Name), sqlTextBuilder.EscapeDbObject(childrenField.ChildrenView.Name) + sqlTextBuilder.DbTableColumnSeperator + sqlTextBuilder.EscapeDbObject(childrenField.GetFirstNonEquivalentParentField().GetColumnsNames()[0]), sqlTextBuilder.EscapeDbObject(childrenField.GetFirstNonEquivalentParentField().ParentView.Name) + sqlTextBuilder.DbTableColumnSeperator + sqlTextBuilder.EscapeDbObject(childrenField.GetFirstNonEquivalentParentField().ParentView.GetPkColumnNames()[0]), next);
                where += string.Format("{0} in (select {1} from {2}" + sqlTextBuilder.WithNolock + " where {3} in (select {4} from {5}  where {6}))",
                    sqlTextBuilder.EscapeDbObject(childrenField.View.Name) + sqlTextBuilder.DbTableColumnSeperator + sqlTextBuilder.EscapeDbObject(childrenField.GetColumnsNames()[0]),
                    sqlTextBuilder.EscapeDbObject(childrenField.ChildrenView.Name) + sqlTextBuilder.DbTableColumnSeperator + sqlTextBuilder.EscapeDbObject(childrenField.GetEquivalentParentField().GetColumnsNames()[0]),
                    sqlTextBuilder.EscapeDbObject(childrenField.ChildrenView.Name),
                    sqlTextBuilder.EscapeDbObject(childrenField.ChildrenView.Name) + sqlTextBuilder.DbTableColumnSeperator + sqlTextBuilder.EscapeDbObject(childrenField.GetFirstNonEquivalentParentField().GetColumnsNames()[0]),
                    sqlTextBuilder.EscapeDbObject(childrenField.GetFirstNonEquivalentParentField().ParentView.Name) + sqlTextBuilder.DbTableColumnSeperator + sqlTextBuilder.EscapeDbObject(childrenField.GetFirstNonEquivalentParentField().ParentView.GetPkColumnNames()[0]),
                    sqlTextBuilder.EscapeDbObject(childrenField.GetFirstNonEquivalentParentField().ParentView.Name),
                    next);
            }

            return where;
        }

        private string GetNoSql(List<Field> fields, string usersObjectName, string emailFieldName, int i, string nosql, bool showAllForAdmin)
        {
            string next = null;
            if (i == 0)
            {
                next = string.Format("\"object\":\"{0}\",\"q\":{{\"{1}\":{{\"$eq\":\"'{2}'\"}}}}", usersObjectName, emailFieldName, SysUsername);
            }
            else
            {
                next = GetNoSql(fields, usersObjectName, emailFieldName, i - 1, nosql, showAllForAdmin);
            }

            Field field = fields[i];
            if (field.FieldType == FieldType.Parent)
            {
                ParentField parentField = (ParentField)field;
                //where += string.Format("{0} in (select {1} from {2} where {3})", sqlTextBuilder.EscapeDbObject(parentField.View.Name) + sqlTextBuilder.DbTableColumnSeperator + sqlTextBuilder.EscapeDbObject(parentField.GetColumnsNames()[0]), sqlTextBuilder.EscapeDbObject(parentField.ParentView.Name) + sqlTextBuilder.DbTableColumnSeperator + sqlTextBuilder.EscapeDbObject(parentField.ParentView.GetPkColumnNames()[0]), sqlTextBuilder.EscapeDbObject(parentField.ParentView.Name), next);
                string q = string.Format("\"{0}\":{{\"$in\":{{{1},\"fields\":[\"{2}\"]}}}}",
                    parentField.JsonName,
                    next,
                    parentField.ParentView.GetFieldByColumnNames(parentField.ParentView.GetPkColumnNames()[0]).JsonName);

                if (showAllForAdmin)
                {
                    if (i == fields.Count - 1)
                    {
                        q = string.Format("\"$or\":[{{\"'{0}'\":\"'{1}'\"}}, {{", SysRole, AdminRole) + q + "}]";
                    }
                }

                nosql += string.Format("\"object\":\"{0}\",\"q\":{{{1}}}",
                    parentField.View.JsonName,
                    q);
            }
            else if (field.FieldType == FieldType.Children)
            {
                ChildrenField childrenField = (ChildrenField)field;

                string q = string.Format("\"{0}\":{{\"$in\":{{\"object\":\"{1}\",\"q\":{{\"{2}\":{{\"$in\":{{{3},\"fields\":[\"{4}\"]}}}}}},\"fields\":[\"{5}\"]}}}}",
                    childrenField.GetColumnsNames()[0],
                    childrenField.ChildrenView.Name,
                    childrenField.GetFirstNonEquivalentParentField().GetColumnsNames()[0],
                    next,
                    childrenField.GetFirstNonEquivalentParentField().ParentView.GetPkColumnNames()[0],
                    childrenField.GetEquivalentParentField().GetColumnsNames()[0]);

                if (showAllForAdmin)
                {
                    if (i == fields.Count - 1)
                    {
                        q = string.Format("\"$or\":[{{\"'{0}'\":\"'{1}'\"}}, {{", SysRole, AdminRole) + q + "}]";
                    }
                }

                nosql += string.Format("\"object\":\"{0}\",\"q\":{{{1}}}",
                    childrenField.View.JsonName,
                    q);

                //nosql += string.Format("{0} in (select {1} from {2} where {3} in (select {4} from {5}  where {6}))",
                //    childrenField.GetColumnsNames()[0],
                //    childrenField.GetEquivalentParentField().GetColumnsNames()[0],
                //    childrenField.ChildrenView.Name,
                //    childrenField.GetFirstNonEquivalentParentField().GetColumnsNames()[0],
                //    childrenField.GetFirstNonEquivalentParentField().ParentView.GetPkColumnNames()[0],
                //    childrenField.GetFirstNonEquivalentParentField().ParentView.Name,
                //    next);
            }

            return nosql;
        }

        private Field GetUserRelatedField(View usersView, View view, List<Field> fields, int level, int max)
        {
            if (level >= max)
                return null;

            foreach (ParentField parentField in view.Fields.Values.Where(f => f.FieldType == FieldType.Parent))
            {
                if (parentField.ParentView == usersView)
                {
                    fields.Add(parentField);
                    return parentField;
                }
            }

            foreach (ChildrenField childrenField in view.Fields.Values.Where(f => f.FieldType == FieldType.Children))
            {
                Durados.ParentField parentField = null;
                Durados.ParentField fkField = null;
                if (childrenField.GetOtherParentView(out parentField, out fkField) == usersView)
                {
                    fields.Add(childrenField);
                    return parentField;
                }
            }

            level++;

            foreach (ParentField parentField in view.Fields.Values.Where(f => f.FieldType == FieldType.Parent))
            {
                Field ancestorField = GetUserRelatedField(usersView, (View)parentField.ParentView, fields, level, max);
                if (ancestorField != null)
                {
                    fields.Add(parentField);
                    return ancestorField;
                }
            }

            foreach (ChildrenField childrenField in view.Fields.Values.Where(f => f.FieldType == FieldType.Children && f.IsCheckList()))
            {
                Durados.ParentField parentField = null;
                Durados.ParentField fkField = null;
                View parentView = (View)childrenField.GetOtherParentView(out parentField, out fkField);
                Field ancestorField = GetUserRelatedField(usersView, (View)parentField.ParentView, fields, level, max);
                if (ancestorField != null)
                {
                    fields.Add(childrenField);
                    return parentField;
                }
            }

            return null;
        }
    }

    public class AppsPool
    {
        public bool? Pop(string appName, string title, string username, out int? appId, string template, int? templateId, bool force)
        {
            int creator = GetCreator(username);
            return Pop(appName, title, username, creator, out appId, template, templateId, force);
        }

        private int GetCreator(string username)
        {
            return Maps.Instance.DuradosMap.Database.GetUserID(username);
        }

        public bool? Pop(string appName, string title, string username, int creator, out int? appId, string template, int? templateId, bool force)
        {
            Map mainMap = null;
            appId = null;
            if (force || template != "10")
            {
                return false;
            }
            if (!ShouldBeUsed())
                return false;

            try
            {
                mainMap = Maps.Instance.DuradosMap;
                int poolCreator = GetPoolCreator();
                if (poolCreator == creator)
                {
                    mainMap.Logger.Log("AppsPool", "Pop", "", string.Format("pool creator equals creator, id = {0}", creator), "", 8, string.Empty, DateTime.Now);
                    return false;
                }

                appId = FindAndUpdateAppInMain(appName, title, creator, mainMap.connectionString, templateId);
                if (!appId.HasValue)
                {
                    mainMap.Logger.Log("AppsPool", "Pop", "", string.Format("Could not find app in pool for user id {0} where pool creator is {1}", creator, poolCreator), "", 2, string.Empty, DateTime.Now);
                    return false;
                }

                RestHelper.Refresh(appName);

                System.Data.DataRow userMainRow = mainMap.Database.GetUserRow(username);
                string firstName = userMainRow["FirstName"].ToString();
                string lastName = userMainRow["LastName"].ToString();

                ReplaceUsernameInSysDb(mainMap, appName, username, firstName, lastName);
                //RegisterAdminToAppSecurity(mainMap, appName, username, firstName, lastName);
                ReplaceUsernameInUsers(mainMap, appName, username, firstName, lastName);

                mainMap.Logger.Log("AppsPool", "Pop", "", string.Format("success for creator id = {0}, pool creator = {1}, app id = {2}, appName = {3} ", creator, poolCreator, appId, appName), "", 3, string.Empty, DateTime.Now);

                return true;
            }
            catch (Exception exception)
            {
                if (appId.HasValue)
                {
                    try
                    {
                        DeleteBadApp(appId.Value, mainMap.connectionString);
                        RestHelper.Refresh(appName);
                        return null;
                    }
                    catch (Exception exception2)
                    {
                        mainMap.Logger.Log("AppsPool", "Pop", "fail to delete app id: " + appId, exception2, 1, string.Empty);
                    }
                }
                mainMap.Logger.Log("AppsPool", "Pop", "app id: " + appId, exception, 1, string.Empty);
                return false;
            }
        }

        //private void RegisterAdminToAppSecurity(Map mainMap, string appName, string username, string firstName, string lastName)
        //{
        //    (new AccountService(null)).CreateAdminMembership(appName, username, password);
        //}

        private void DeleteBadApp(int appId, string connectionString)
        {

            string sql = Maps.MainAppSchema.GetDeleteAppById(appId);
                
             Maps.MainAppSqlAccess.ExecuteNonQuery(connectionString, sql, new Dictionary<string, object>() { { "id", appId } }, null);

        }

        private void ReplaceUsernameInUsers(Map mainMap, string appName, string username, string firstName, string lastName)
        {
            Map map = Maps.Instance.GetMap(appName);
            View userView = (View)map.Database.GetView("users");
            if (userView == null)
                throw new Durados.DuradosException("users does not exist");
            int rowCount = 0;
            DataView dataView = userView.FillPage(1, 2, null, null, null, out rowCount, null, null);
            if (dataView.Count != 1)
                throw new Durados.DuradosException("users should contain one row for " + appName);
            System.Data.DataRow currentUserRow = dataView[0].Row;
            string pk = userView.GetPkValue(currentUserRow);

            SqlAccess sqlAccess = RestHelper.GetSqlAccess(userView.Database.SqlProduct);

            string sql = string.Format("update {0} set email = '{1}', firstName = '{2}', lastName = '{3}' where id = {4}", userView.Name, username, firstName, lastName, pk);
            try
            {
                sqlAccess.ExecuteNonQuery(userView.ConnectionString, sql, userView.Database.SqlProduct);
            }
            catch (Exception exception)
            {
                throw new DuradosException("Failed replace username in users", exception);
            }

            //userView.Edit(new Dictionary<string, object>() { { "email", username }, { "firstName", firstName }, { "lastName", lastName } }, pk, null, null, null, null);
        }

        private void ReplaceUsernameInSysDb(Map mainMap, string appName, string username, string firstName, string lastName)
        {
            Map map = Maps.Instance.GetMap(appName);
            View userView = (View)map.Database.GetUserView();
            int rowCount = 0;
            DataView dataView = userView.FillPage(1, 2, null, null, null, out rowCount, null, null);
            if (dataView.Count != 1)
                throw new Durados.DuradosException("system user should contain one row for " + appName);
            System.Data.DataRow currentUserRow = dataView[0].Row;
            string pk = userView.GetPkValue(currentUserRow);
            userView.Edit(new Dictionary<string, object>() { { "Username", username }, { "Email", username }, { "FirstName", firstName }, { "LastName", lastName } }, pk, null, null, null, null);
        }

        private int? FindAndUpdateAppInMain(string appName, string title, int creator, string connectionString, int? templateId)
        {
            return FindAndUpdateAppInMain(appName, title, creator, GetPoolCreator(), connectionString, templateId);
        }

        private int GetPoolCreator()
        {
            return Maps.PoolCreator;
        }

        private bool ShouldBeUsed()
        {
            return Maps.PoolShouldBeUsed;
        }

        private int? FindAndUpdateAppInMain(string appName, string title, int creator, int poolCreator, string connectionString, int? templateId)
        {
            SqlAccess sqlAccess = Maps.MainAppSqlAccess;
            string varConnectionString = string.Format("{0}{1};", connectionString, Maps.MainAppSchema.GetConnectionStringAllowVeriables());
            string sql = Maps.MainAppSchema.GetFindAndUpdateAppInMainSql(templateId); 
            string scalar = sqlAccess.ExecuteScalar(varConnectionString, sql, new Dictionary<string, object>() { { "poolCreator", poolCreator }, { "creator", creator }, { "CreatedDate", DateTime.Now }, { "Name", appName }, { "Title", title } });
            if (!string.IsNullOrEmpty(scalar))
            {
                return Convert.ToInt32(scalar);
            }
            return null;
        }
    }

    public class RestException : DuradosException
    {
        public RestException(string message, Exception innerException) : base(message, innerException) { }

        public RestException(string message)
            : base(message)
        {

        }

        public object GetJsonError(string error)
        {
            return new { error = error, error_description = this.Message };
        }
    }
}
