﻿using System;
using System.Data;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Durados;
using Durados.DataAccess;
using Durados.Web.Mvc.UI.Helpers;
using System.Diagnostics;
using Microsoft.WindowsAzure;
using Microsoft.WindowsAzure.StorageClient;
using System.Web.Security;
using System.Data.SqlClient;
using System.Net;
using System.IO;
using Durados.Web.Mvc.Infrastructure;
using System.Threading;
using System.Web.Mvc;
using System.Security.Claims;

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

           string blobName = Maps.GetStorageBlobName(fileName);
           if (Maps.Instance.StorageCache.ContainsKey(blobName))
           {
               Maps.Instance.StorageCache.Remove(blobName);
           }
           blobName += "xml";
           if (Maps.Instance.StorageCache.ContainsKey(blobName))
           {
               Maps.Instance.StorageCache.Remove(blobName);
           }
       }

       public static void AddStat(Dictionary<string, object> item, string appName)
       {
           Stat stat = StatFactory.GetState(Maps.Instance.GetMap(appName).SystemSqlProduct);
          
           stat.AddStat(item, appName);
       }

       public static Dictionary<string, Dictionary<string, object>> ReferenceTableToDictionary(View view, DataView dataView, bool deep = false, bool descriptive = true)
       {
           return new DictionaryConverter().ReferenceTableToDictionary(view, dataView, deep, descriptive);
       }


       public static Dictionary<string, object>[] TableToDictionary(View view, DataView dataView, bool deep = false, bool descriptive = true)
       {
           return new DictionaryConverter().TableToDictionary(view, dataView, deep, descriptive);
       }

       public static Dictionary<string, object> RowToDictionary(View view, DataRow row, string pk, bool deep, bool displayParentValue = false)
       {
           return GetDictionaryConverter(view).RowToDictionary(view, row, pk, deep, displayParentValue);

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

       public static string Create(this View view, Dictionary<string, object>[] values, bool deep, BeforeCreateEventHandler beforeCreateCallback, BeforeCreateInDatabaseEventHandler beforeCreateInDatabaseEventHandler, AfterCreateEventHandler afterCreateBeforeCommitCallback, AfterCreateEventHandler afterCreateAfterCommitCallback)
       {
           return GetRest(view).Create(view, values, deep, beforeCreateCallback, beforeCreateInDatabaseEventHandler, afterCreateBeforeCommitCallback, afterCreateAfterCommitCallback);
       }

       public static void Update(this View view, string json, string pk, bool deep, BeforeEditEventHandler beforeEditCallback, BeforeEditInDatabaseEventHandler beforeEditInDatabaseCallback, AfterEditEventHandler afteEditBeforeCommitCallback, AfterEditEventHandler afterEditAfterCommitCallback)
       {
           Update(view, Deserialize(view, json), pk, deep, beforeEditCallback, beforeEditInDatabaseCallback, afteEditBeforeCommitCallback, afterEditAfterCommitCallback);
       }

       public static void Update(this View view, Dictionary<string, object> values, string pk, bool deep, BeforeEditEventHandler beforeEditCallback, BeforeEditInDatabaseEventHandler beforeEditInDatabaseCallback, AfterEditEventHandler afteEditBeforeCommitCallback, AfterEditEventHandler afterEditAfterCommitCallback, BeforeCreateEventHandler beforeCreateCallback = null, BeforeCreateInDatabaseEventHandler beforeCreateInDatabaseCallback = null, AfterCreateEventHandler afterCreateBeforeCommitCallback = null, AfterCreateEventHandler afterCreateAfterCommitCallback = null, bool overwrite = false, BeforeDeleteEventHandler beforeDeleteCallback = null, AfterDeleteEventHandler afterDeleteBeforeCommitCallback = null, AfterDeleteEventHandler afterDeleteAfterCommitCallback = null)
       {
           GetRest(view).Update(view, values, pk, deep, beforeEditCallback, beforeEditInDatabaseCallback, afteEditBeforeCommitCallback, afterEditAfterCommitCallback, beforeCreateCallback, beforeCreateInDatabaseCallback, afterCreateBeforeCommitCallback, afterCreateAfterCommitCallback, overwrite, beforeDeleteCallback, afterDeleteBeforeCommitCallback, afterDeleteAfterCommitCallback);
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
           foreach (ColumnField field in view.Fields.Values.Where(f => f.FieldType == FieldType.Column))
           {
               if (field.IsDateTime)
               {
                   if (values.ContainsKey(field.JsonName))
                   {
                       values.Remove(field.JsonName);
                   }
               }
           }

           System.Web.Script.Serialization.JavaScriptSerializer javaScriptSerializer = new System.Web.Script.Serialization.JavaScriptSerializer();
           foreach (ColumnField field in view.Fields.Values.Where(f=>f.FieldType == FieldType.Column))
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

       public static void Delete(this View view, string pk, bool deep, BeforeDeleteEventHandler beforeDeleteCallback, AfterDeleteEventHandler afteDeleteBeforeCommitCallback, AfterDeleteEventHandler afterDeleteAfterCommitCallback, Dictionary<string, object> values = null)
       {
           GetRest(view).Delete(view, pk, deep, beforeDeleteCallback, afteDeleteBeforeCommitCallback, afterDeleteAfterCommitCallback, null, null, values);
       }

       public static object Get(this Query query, int page, int pageSize, Dictionary<string, object> values, bool dataSeries = true)
       {
           string sql = query.SQL.Replace(Durados.Workflow.Engine.AsToken(values));

           string currentUsetId = query.Database.GetUserID();
           string currentUsername = query.Database.GetCurrentUsername();
           string currentUserRole = ((Database) query.Database).GetUserRole();

           sql = sql.Replace(Durados.Database.UserPlaceHolder, currentUsetId, false).Replace(Durados.Database.SysUserPlaceHolder.AsToken(), currentUsetId, false)
                 .Replace(Durados.Database.UsernamePlaceHolder, currentUsername, false).Replace(Durados.Database.SysUsernamePlaceHolder.AsToken(), currentUsername)
                 .Replace(Durados.Database.RolePlaceHolder, currentUserRole, false).Replace(Durados.Database.SysRolePlaceHolder.AsToken(), currentUserRole);

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
                       if (!row.IsNull(column))
                           jsonRow.Add(column.ColumnName, row[column]);
                   }
                   list.Add(jsonRow);
               }
           }


           return list;
       }

       public static object Get(this View view, bool withSelectOptions, bool withFilterOptions, int page, int pageSize, Dictionary<string, object>[] filter, string search, Dictionary<string, object>[] sort, out int rowCount, bool deep, BeforeSelectEventHandler beforeSelectCallback, AfterSelectEventHandler afterSelectCallback, bool returnDataView = false, bool descriptive = true)
       {
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

           DataView dataView = view.FillPage(page, pageSize, values, isSearch, sortColumns, out rowCount, beforeSelectCallback, afterSelectCallback);
           if (returnDataView)
               return dataView;

           Dictionary<string, object> json = new Dictionary<string, object>();
           json.Add("totalRows", rowCount);
           json.Add("data", TableToDictionary(view, dataView, deep, descriptive));

           if (deep)
           {
               Dictionary<string, Dictionary<string, Dictionary<string, object>>> referenceTables = new Dictionary<string, Dictionary<string, Dictionary<string, object>>>(); 
               foreach (DataTable table in dataView.Table.DataSet.Tables)
               {
                   if (table.TableName != dataView.Table.TableName && table.Rows.Count > 0)
                   {
                       View referenceView = (View)view.Database.Views[table.TableName];
                       referenceTables.Add(referenceView.JsonName, ReferenceTableToDictionary(referenceView, new DataView(table), deep));
                   }
               }
               json.Add("relatedTables", referenceTables);
           }
               
           if (withSelectOptions)
           {
               json.Add("selectOptions", GetSelectOptions(view));
           }
           if (withFilterOptions)
           {
               json.Add("filterOptions", GetFilterOptions(view));
           }
           return json;
           
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

       public static Dictionary<string, object> Get(this View view, string pk, bool deep, BeforeSelectEventHandler beforeSelectCallback, AfterSelectEventHandler afterSelectCallback, bool displayParentValue = false, bool ignoreConfig = false)
       {
           if (view.Database.IsConfig && !ignoreConfig)
           {
               return GetConfig(view, pk, deep, beforeSelectCallback, afterSelectCallback, displayParentValue);
           }

           var map = view.Database.Map;
           map.Logger.Log(view.Name, DateTime.Now.Ticks.ToString(), "before get row", null, 5, pk);
 
           DataRow dataRow = view.GetDataRow(pk, null, beforeSelectCallback, afterSelectCallback, true);

           map.Logger.Log(view.Name, DateTime.Now.Ticks.ToString(), "after get row", null, 5, pk);
 
           if (dataRow == null)
               return null;

           map.Logger.Log(view.Name, pk, "before row to json", null, 5, null);
           
           Dictionary<string, object> dic = RowToDictionary(view, dataRow, pk, deep);

           map.Logger.Log(view.Name, pk, "after row to json", null, 5, null);
           
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
                        string localDatabaseHost = GetLocalDatabaseHost();
                        if (map.connectionString.Contains(localDatabaseHost))
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

       public static string GetLocalDatabaseHost()
       {
           return (System.Web.HttpContext.Current.Items["localDatabaseHost"] ?? "yrv-dev.czvbzzd4kpof.eu-central-1.rds.amazonaws.com").ToString();
       }

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

       public static string GetAppUrl(string appname, bool ishttp = false)
       {
           string host = System.Configuration.ConfigurationManager.AppSettings["durados_host"];
           string port = System.Configuration.ConfigurationManager.AppSettings["durados_port"];
           string http = "https://";
           if (Maps.Debug || ishttp)
               http = "http://";

           string url = http + appname + "." + host + (string.IsNullOrEmpty(port) ? string.Empty : ":" + port);
           return url;
       }

       public static string GetCurrentAdminAppUrl()
       {
           return GetAppUrl((System.Web.HttpContext.Current.Items[Database.AppName] ?? "www").ToString());
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
               if (filename == null || filename.EndsWith(defaulLogo) || filename.ToLower().EndsWith("backand.png") || filename.ToLower().EndsWith("modubiz.png"))
               {
                   return GetAppUrl("www") + "/Content/Images/" + defaulLogo;
               }
               string folder = Maps.AzureAppPrefix + pk;
               return string.Format(Maps.AzureStorageUrl + "/{2}", Maps.AzureStorageAccountName, folder, filename);
           }
           catch
           {
               try
               {
                   return GetAppUrl("www") + "/Content/Images/" + defaulLogo;
               }
               catch
               {
                   return string.Empty;
               }
           }
       }

       public static Dictionary<string, object> GetUser(Database database)
       {
           Dictionary<string, object> user = new Dictionary<string, object>();

           user.Add("username", database.GetCurrentUsername());
           user.Add("fullName", database.GetUserFullName());
           user.Add("role", database.GetUserRole());

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


       private static SqlAccess GetSqlAccess(SqlProduct sqlProduct)
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

       public static Dictionary<string,object> GetDbStat(Database database)
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

       public static object GetKeys(string appName)
       {
           Map map = Maps.Instance.GetMap(appName);
           if (map == null || map is DuradosMap)
           {
               throw new DuradosException("App not found");
           }

           string role = map.Database.GetUserRole(map.Database.GetCurrentUsername());
           if (!(role == "Admin" || role == "Developer"))
               throw new DuradosException("Only admin can get keys");

           string sql = string.Format("SELECT * FROM [durados_app] WITH(NOLOCK)  WHERE [Name] = '{0}'", appName);

           Dictionary<string, object> keys = new Dictionary<string, object>();
           using (System.Data.SqlClient.SqlConnection cnn = new System.Data.SqlClient.SqlConnection(Maps.Instance.DuradosMap.Database.ConnectionString))
           {
               using (System.Data.SqlClient.SqlCommand command = new System.Data.SqlClient.SqlCommand(sql, cnn))
               {
                   cnn.Open();
                   using (System.Data.SqlClient.SqlDataReader reader = command.ExecuteReader())
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
               base.AddToDictionary(dictionary, field, ((Dictionary<string,object>)((Dictionary<string,object>)value)["__metadata"])["id"]);
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
               if (columnField.Name == "HideInCreate")
               {
                   Field realField = GetFieldFromFieldRow(view, dataRow);
                   
                   return !realField.IsVisibleForCreate();
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
                                   return Convert.ToDateTime(value).ToUniversalTime().ToString("u");
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
                               if (value.Equals("0") || value.ToString().ToLower() == "true" || value.ToString().ToLower() == "yes")
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
            "Formula","InlineSearch","Name","MultiFilter","Precedent","Width",
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

       protected override void AddAdditionalProperties(View view, DataRow dataRow, bool withChildren, HashSet<string> pks, Dictionary<string, object> dictionary)
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
        
               foreach (Category category in realView.Categories.Values.OrderBy(c=>c.Ordinal))
               {
                   Dictionary<string, object> categoryDictionary = new Dictionary<string, object>();
                   categoryDictionary.Add("name", category.Name);
                   if (category.Fields.Count() == category.Fields.Where(f=>f.FieldType == FieldType.Children).Count())
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
            string role = database.GetUserRole(database.GetCurrentUsername());

            return role == "Admin" || role == "Developer";
           
       }

       public Dictionary<string, object>[] TableToDictionary(View view, DataView dataView, bool deep = false, bool descriptive = true)
       {
           List<Dictionary<string, object>> list = new List<Dictionary<string, object>>();

           Durados.Web.Mvc.UI.TableViewer tableViewer = new Durados.Web.Mvc.UI.TableViewer();
           tableViewer.DataView = dataView;

           //System.Data.DataTable table = tableViewer.GetDataView(dataView, view, string.Empty).Table;

           foreach (System.Data.DataRowView row in dataView)
           {
               list.Add(RowToDictionary(view, dataView, tableViewer, row, deep, descriptive));
           }
           return list.ToArray();
       }

       public Dictionary<string, Dictionary<string, object>> ReferenceTableToDictionary(View view, DataView dataView, bool deep = false, bool descriptive = true)
       {
           Dictionary<string, Dictionary<string, object>> dictionary = new Dictionary<string, Dictionary<string, object>>();

           Durados.Web.Mvc.UI.TableViewer tableViewer = new Durados.Web.Mvc.UI.TableViewer();
           tableViewer.DataView = dataView;

           //System.Data.DataTable table = tableViewer.GetDataView(dataView, view, string.Empty).Table;

           foreach (System.Data.DataRowView row in dataView)
           {
               dictionary.Add(view.GetPkValue(row.Row), RowToDictionary(view, dataView, tableViewer, row, deep, descriptive));
           }
           return dictionary;
       }

       private Dictionary<string, object> RowToDictionary(View view, DataView dataView, Durados.Web.Mvc.UI.TableViewer tableViewer, System.Data.DataRowView row, bool deep = false, bool descriptive = true)
       {
           Dictionary<string, object> dictionary = new Dictionary<string, object>();
           dictionary.Add(Database.__metadata, GetRowMetadata(view, row.Row, tableViewer, descriptive));

           foreach (Durados.Field field in view.VisibleFieldsForTableFirstRow)
           {
               object value = null;
               try
               {
                   //if (deep)
                   //{
                       if (field.FieldType == FieldType.Children && !field.IsCheckList())
                           continue;

                       if (field.FieldType == FieldType.Column && (field.IsDate || field.IsBoolean || field.IsNumeric))
                           value = row.Row[((ColumnField)field).DataColumn.ColumnName];
                       else if (field.FieldType == FieldType.Children && field.IsCheckList())
                           value = tableViewer.GetFieldValue(field, row.Row);
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

           string username = row["Username"].ToString();

           System.Web.Security.MembershipUser existingUser = System.Web.Security.Membership.Provider.GetUser(username, false);

           bool readyToSignin = existingUser != null;

           dictionary.Add("readyToSignin", readyToSignin);
       }

       protected virtual void AddFieldValue(Field field, DataRow row, string name, object value, Dictionary<string, object> dictionary)
       {
           if (!HandleHistory(field, row, name, value, dictionary))
               dictionary.Add(name, value);
       }

       private bool HandleHistory(Field field, DataRow row, string name, object value, Dictionary<string, object> dictionary)
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

       private Dictionary<string, object> GetRowMetadata(View view, DataRow row, Durados.Web.Mvc.UI.TableViewer tableViewer = null, bool descriptive = true)
       {

           Dictionary<string, object> metadata = new Dictionary<string, object>();
           metadata.Add("id", view.GetPkValue(row));

           if (!view.Database.IsConfig)
           {
               if (descriptive)
               {
                   Dictionary<string, object> descriptives = new Dictionary<string, object>();
                   //foreach (ParentField field in view.Fields.Values.Where(f => f.GetHtmlControlType() == HtmlControlType.Autocomplete))
                   foreach (ParentField field in view.Fields.Values.Where(f => f.FieldType == FieldType.Parent))
                   {
                       string value = field.GetValue(row);
                       string label  = string.Empty;
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
                           label = tableViewer.GetFieldDisplayValue(field, row, false).Replace(",",", ");
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

           return metadata;
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

       public Dictionary<string, object> RowToDictionary(View view, DataRow row, string pk, bool deep, bool displayParentValue)
       {
           if (deep)
               return RowToDeepDictionary(view, row);
           else
               return RowToShallowDictionary(view, row, pk, displayParentValue);

       }

       public Dictionary<string, object> RowToDeepDictionary(View view, DataRow row)
       {
           return RowToDeepDictionary(view, row, true, new HashSet<string>());
       }

       public virtual Dictionary<string, object> RowToDeepDictionary(View view, DataRow dataRow, bool withChildren, HashSet<string> pks)
       {
           if (dataRow == null)
               return null;

           string pk = view.Name + view.GetPkValue(dataRow);

           if (pks.Contains(pk))
               return null;
           else
               pks.Add(pk);

           Dictionary<string, object> dictionary = new Dictionary<string, object>();
           dictionary.Add(Database.__metadata, GetRowMetadata(view, dataRow));

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
                   
                   List<Dictionary<string, object>> list = new List<Dictionary<string, object>>();
                   foreach (System.Data.DataRowView childRow in childTable)
                   {
                       if (!IsJsonable(childrenField, childRow.Row))
                           continue;
                       
                       Dictionary<string, object> childDictionary = RowToDeepDictionary(childrenView, childRow.Row, true, pks);
                       
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

                   Dictionary<string, object> parentDictionary = RowToDeepDictionary(parentView, parentRow, false, pks);
                   
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

       protected virtual void AddAdditionalProperties(View view, DataRow dataRow, bool withChildren, HashSet<string> pks, Dictionary<string, object> dictionary)
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

       public Dictionary<string, object> RowToShallowDictionary(View view, DataRow row, string pk, bool displayParentValue)
       {
           Json.View jsonView = view.GetJsonViewNotSerialized(DataAction.Edit, pk, row, string.Empty);

           Dictionary<string, object> dictionary = new Dictionary<string, object>();
           dictionary.Add(Database.__metadata, GetRowMetadata(view, row));

           foreach (string fieldName in jsonView.Fields.Keys)
           {
               Json.Field JsonField = jsonView.Fields[fieldName];
               Field field = view.Fields[fieldName];

               bool isJsonable = IsJsonable(field, row);
               if (!isJsonable)
                   continue;

               string name = GetName(field);
               if (!dictionary.ContainsKey(name))
               {
                   if (field.FieldType == FieldType.Column && (field.IsDate || field.IsBoolean))
                       dictionary.Add(name, row[((ColumnField)field).DataColumn.ColumnName]);
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
                       value = param["value"].ToString();
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
                       if (value.ToLower() == "yes" || value.ToLower() == "true" )
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

                   if (value == emptyValueIn)
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

               if (outOperator != null && (!string.IsNullOrEmpty(value) || inOperator == FilterOperandType.notEmpty.ToString() || inOperator == FilterOperandType.empty.ToString()))
               {
                   if (filterOut.ContainsKey(fieldName))
                   {
                       throw new FilterException("The simple filter can only contain one item per field. Please Use a Query.");
                   }
                   filterOut.Add(fieldName, outOperator + space + value);
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
       public void AddStat(Dictionary<string, object> item, string appName)
       {
           if (item.ContainsKey("durados_App_Stat"))
               item.Remove("durados_App_Stat");

           object stat = null;
           if (!cache.ContainsKey(appName) || cache[appName].Time < DateTime.Now.Subtract(new TimeSpan(1, 0, 0)))
           {
               if (cache.ContainsKey(appName))
                   cache.Remove(appName);
               try
               {
                   cache.Add(appName, new StatAndTime() { Time = DateTime.Now, Stat = GetStat(appName) });
               }
               catch { }
           }
           stat = cache[appName].Stat;
           item.Add("stat", stat);
       }

       class StatAndTime
       {
           public DateTime Time;
           public object Stat;
       }
       private static Dictionary<string, StatAndTime> cache = new Dictionary<string, StatAndTime>();
       

       private object GetStat(string appName)
       {
           string sysConnectionString = GetSystemConnectionString(appName);

           if (string.IsNullOrEmpty(sysConnectionString))
               return null;

           object scalar =  GetLast24Hours(sysConnectionString);
           int? last24 = null;
           if (scalar != null) last24 = Convert.ToInt32(scalar);

           scalar = GetLast48Hours(sysConnectionString);
           int? last48 = null;
           if (scalar != null) last48 = Convert.ToInt32(scalar);

           int? diff24 = null;
           if (last24.HasValue && last48.HasValue)
           {
               int today = last24.Value;
               int yesterday = last48.Value - today;

               if (today == yesterday)
               {
                   diff24 = 0;
               }
               else if (yesterday == 0)
               {
                   diff24 = null;
               }
               else
               {
                   diff24 = (100 * (today - yesterday)) / yesterday;
               }
           }

           scalar = GetLast30Days(sysConnectionString);
           int? last30 = null;
           if (scalar != null) last30 = Convert.ToInt32(scalar);

           scalar = GetLast60Days(sysConnectionString);
           int? last60 = null;
           if (scalar != null) last60 = Convert.ToInt32(scalar);

           int? diff30 = null;
           if (last30.HasValue && last60.HasValue)
           {
               int thisMonth = last30.Value;
               int prevMonth = last60.Value - thisMonth;

               if (thisMonth == prevMonth)
               {
                   diff30 = 0;
               }
               else if (prevMonth == 0)
               {
                   diff30 = null;
               }
               else
               {
                   diff30 = (100 * (thisMonth - prevMonth)) / prevMonth;
               }
           }

           int? size = GetSize(appName, GetConnectionString(appName));

           return new Dictionary<string, object>() { { "last24hours", last24 }, { "diffLastDaytoYesterday", diff24 }, { "last30days", last30 }, { "diffLast30DaysToPrev", diff30 }, { "sizeInMb", size } };
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

       private string GetSql(SqlProduct sqlProduct)
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
    public class MySqlState:Stat
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
            if(systemSqlProduct==SqlProduct.MySql)
                return new MySqlState();
            return new Stat();
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

           if (i["deletedTables"] > 0 )
           {
               Initiate(map);
           }

           return result;
       }

       public virtual Dictionary<string,object> AddNewViews(Map map, out string errorMessage)
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
                       try
                       {
                           map.Logger.WriteToEventLog("The view " + viewName + " was synced by " + map.Database.GetCurrentUsername(), System.Diagnostics.EventLogEntryType.Error, 2030);
                       }
                       catch { }
                   }
                   Initiate(map);
               }
           }
           catch (Exception exception)
           {
               throw new DuradosException("Failed to sync all", exception);
           }
       }

       protected virtual void Initiate(Map map)
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
               string label = row.Row["Tag"].ToString();
               string token = row.Row["Token"].ToString();
               if (!IsExcluded(view, label))
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
           string cacheKey = null;
           string viewDictionary = "viewDictionary";
           if (view != null)
               cacheKey = dictionaryType.ToString() + "__" + view.Name;

           if (view != null && view.Database.Map.AllKindOfCache.ContainsKey(viewDictionary) && view.Database.Map.AllKindOfCache[viewDictionary].ContainsKey(cacheKey))
           {
               return (DataView)view.Database.Map.AllKindOfCache[viewDictionary][cacheKey];
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
                   string pk = GetFirstPK(view);
                   dataView = GetDisplayNameDictionary(view, pk);
                   break;
           }

           if (view != null)
           {
               if (!view.Database.Map.AllKindOfCache.ContainsKey(viewDictionary))
               {
                   view.Database.Map.AllKindOfCache.Add(viewDictionary, new Dictionary<string, object>());
               }
               if (view.Database.Map.AllKindOfCache[viewDictionary].ContainsKey(cacheKey))
               {
                   view.Database.Map.AllKindOfCache[viewDictionary].Remove(cacheKey);
               }
               view.Database.Map.AllKindOfCache[viewDictionary].Add(cacheKey, dataView);
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
           dt.Rows.Add("User Id", Durados.Database.SysUserPlaceHolder, DataType.Numeric);
           dt.Rows.Add("User Role", Durados.Database.SysRolePlaceHolder, DataType.ShortText);
           dt.Rows.Add("Username", Durados.Database.SysUsernamePlaceHolder, DataType.ShortText);
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

   public class Account
   {
       object controller = null;
       public Account(object controller)
       {
           this.controller = controller;
       }

       private Map GetMap(string appName)
       {
           Map map = Maps.Instance.GetMap(appName);

           if (map == null || map is DuradosMap)
               throw new AppDoesNotExistException(appName);

           return map;
       }

       private Map GetDuradosMap()
       {
           return Maps.Instance.DuradosMap;
       }

       protected virtual bool VerifyPassword(string username, string password)
       {
           Durados.Web.Mvc.Controllers.AccountMembershipService accountMembershipService = new Durados.Web.Mvc.Controllers.AccountMembershipService();
           return accountMembershipService.AuthenticateUser(username, password);
       }

       

       private static MembershipProvider _provider = System.Web.Security.Membership.Provider;

       public virtual void ChangePassword(string username, string password)
       {
           _provider.UnlockUser(username);
           string oldPassword = _provider.ResetPassword(username, null);
           _provider.ChangePassword(username, oldPassword, password);
       }

       public virtual void ChangePassword(string username, string newPassword, string oldPassword)
       {
           if (!VerifyPassword(username, oldPassword))
               throw new DuradosException("The old password is incorrect");

           ChangePassword(username, newPassword);
       }

       public static bool IsValidRole(string appName, string role)
       {
           if (role == null)
               return true;

           Map map = Maps.Instance.GetMap(appName);

           if (map == null)
               return false;

           return map.Database.GetRoleRow(role) != null;
       }

       public virtual SignUpResults SignUp(string appName, string firstName, string lastName, string username, string role, bool byAdmin, string password, string confirmPassword, bool? isSignupEmailVerification, Dictionary<string, object> parameters, BeforeCreateEventHandler beforeCreateCallback, BeforeCreateInDatabaseEventHandler beforeCreateInDatabaseEventHandler, AfterCreateEventHandler afterCreateBeforeCommitCallback, AfterCreateEventHandler afterCreateAfterCommitCallback, BeforeEditEventHandler beforeEditCallback, BeforeEditInDatabaseEventHandler beforeEditInDatabaseCallback, AfterEditEventHandler afteEditBeforeCommitCallback, AfterEditEventHandler afterEditAfterCommitCallback)
       {
           try
           {
               System.Web.HttpContext.Current.Items[Database.Username] = username;

               if (!password.Equals(confirmPassword))
               {
                   throw new PasswordConfirmationException();
               }

               if (!IsAppExists(appName))
               {
                   throw new AppDoesNotExistException(appName);
               }

               bool isPrivate = IsPrivate(appName);

               bool isInvited = IsInvited(username, appName);

               if (isPrivate && !byAdmin)
               {
                   if (!isInvited)
                   {
                       throw new OnlyInvitedUsersAreAllowedException();
                   }
               }

               bool isActive = IsActive(username, appName);

               if (isActive)
               {
                   bool isApproved = IsApproved(username, appName);
                   if (isApproved)
                   {
                       throw new AlreadySignedUpToAppException();
                   }
                   else
                   {
                       return new SignUpResults() { AppName = appName, Username = username, Status = SignUpStatus.PendingAdminApproval };
                   }
               }

               bool isAuthenticated = IsAuthenticated(username, appName);

               bool isPending = true;
               if (!isAuthenticated)
               {
                   AddToAuthenticatedUsers(appName, firstName, lastName, username, password, !isPending, GetDefaultUserRole());
               }
               else
               {
                   isPending = IsPending(username);
               }

               if (!isInvited)
               {
                   Invite(appName, firstName, lastName, username, role, parameters, beforeCreateCallback, beforeCreateInDatabaseEventHandler, afterCreateBeforeCommitCallback, afterCreateAfterCommitCallback);
               }

               SignUpResults signUpResults = new SignUpResults() { AppName = appName, Username = username };
               if (isPending)
               {
                   if (IsSignupEmailVerification(appName, isSignupEmailVerification))
                   {
                       SendVerificationEmail(username, appName, firstName, lastName);
                       signUpResults.Status = SignUpStatus.PengingVerification;
                       return signUpResults;
                   }
                   else
                   {
                       return Verified(appName, signUpResults, beforeEditCallback, beforeEditInDatabaseCallback, afteEditBeforeCommitCallback, afterEditAfterCommitCallback);
                   }
               }
               else
               {
                   Activate(username, appName);

                   bool isManualApprove = IsManualApprove(appName);
                   signUpResults.Status = SignUpStatus.PendingAdminApproval;

                   if (!isManualApprove)
                   {
                       Approve(username, appName, beforeEditCallback, beforeEditInDatabaseCallback, afteEditBeforeCommitCallback, afterEditAfterCommitCallback);
                       signUpResults.Status = SignUpStatus.Ready;
                   }
                   signUpResults.Redirect = GetRedirectToSignIn(signUpResults.AppName);
               }

               return signUpResults;

           }
           catch (SignUpException exception)
           {
               throw exception;
           }
           catch (Exception exception)
           {
               throw new DuradosException("An unexpected signup  exception occured", exception);
           }
       }

       private bool IsSignupEmailVerification(string appName, bool? isSignupEmailVerification)
       {
           if (appName == Maps.DuradosAppName)
               return false;
           if (isSignupEmailVerification.HasValue)
               return isSignupEmailVerification.Value;
           Database database = GetMap(appName).Database;
           return database.SignupEmailVerification;
       }

       private bool IsApproved(string username, string appName)
       {
           Database database = GetMap(appName).Database;
           DataRow userRow = database.GetUserView().GetDataRow(database.GetUsernameField(), username, false);
           if (userRow == null)
               return false;
           if (userRow.IsNull("IsApproved"))
               return false;
           return (bool)userRow["IsApproved"];
       }

       protected virtual Durados.Web.Mvc.Workflow.Engine CreateWorkflowEngine()
       {
           return new Durados.Web.Mvc.Workflow.Engine();
       }

       private void SendVerificationEmail(string username, string appName, string firstName, string lastName)
       {
           string encrypted = GetVerificationParametersToken(appName, username);
           string token = System.Web.HttpContext.Current.Server.UrlEncode(encrypted);
           Durados.Web.Mvc.Workflow.Engine wfe = CreateWorkflowEngine();

           Map map = GetMap(appName);
           if (map == null || map is DuradosMap)
           {
               throw new AppDoesNotExistException(appName);
           }

           Durados.View view = map.Database.GetUserView();
           if (view == null)
               throw new ViewNotFoundExeption("user");

           DataRow row = map.Database.GetUserRow(username);
           if (row == null)
               throw new UserDoesNotExistException(username);
           Dictionary<string, object> values = new Dictionary<string, object>();
           string id = row["ID"].ToString();
           values.Add("token".AsToken(), token);
           string siteWithoutQueryString = System.Web.HttpContext.Current.Request.Url.Scheme + "://" + System.Web.HttpContext.Current.Request.Url.Authority + (System.Web.HttpContext.Current.Request.Url.ToString().Contains("backapi") ? "/backapi" : "");
           values.Add("apiPath".AsToken(), siteWithoutQueryString);
           values.Add("appName".AsToken(), appName);
           values.Add("firstName".AsToken(), row.IsNull("FirstName") ? username : row["FirstName"].ToString());


           wfe.PerformActions(this.controller, view, Durados.TriggerDataAction.OnDemand, values, id, row, map.Database.ConnectionString, Convert.ToInt32(id), row["Role"].ToString(), null, "newUserVerification");


           Debug.WriteLine(encrypted);
           Debug.WriteLine(token);
       }

       private void SendApprovalEmail(string username, string appName)
       {
           string token = System.Web.HttpContext.Current.Server.UrlEncode(GetVerificationParametersToken(appName, username));
           Durados.Web.Mvc.Workflow.Engine wfe = CreateWorkflowEngine();

           Map map = GetMap(appName);
           if (map == null || map is DuradosMap)
           {
               throw new AppDoesNotExistException(appName);
           }

           Durados.View view = map.Database.GetUserView();
           if (view == null)
               throw new ViewNotFoundExeption("user");

           DataRow row = map.Database.GetUserRow(username);
           if (row == null)
               throw new UserDoesNotExistException(username);
           Dictionary<string, object> values = new Dictionary<string, object>();
           string id = row["ID"].ToString();
           string siteWithoutQueryString = System.Web.HttpContext.Current.Request.Url.Scheme + "://" + System.Web.HttpContext.Current.Request.Url.Authority + (System.Web.HttpContext.Current.Request.Url.ToString().Contains("backapi") ? "/backapi" : "");
           values.Add("signInUrl".AsToken(), map.Database.SignInRedirectUrl);
           values.Add("appName".AsToken(), appName);
           values.Add("firstName".AsToken(), row.IsNull("FirstName") ? username : row["FirstName"].ToString());


           wfe.PerformActions(this.controller, view, Durados.TriggerDataAction.OnDemand, values, id, row, map.Database.ConnectionString, Convert.ToInt32(id), row["Role"].ToString(), null, "userApproval");


           Debug.WriteLine(token);
       }

       private string GetVerificationParametersToken(string appName, string username)
       {
           Map map = GetMap(appName);
           string userToken = Durados.Web.Mvc.UI.Helpers.SecurityHelper.GetTmpUserGuidFromGuid(map.Database.GetGuidByUsername(username));
           return Encrypt(appName, userToken);
       }

       private string Encrypt(string appName, string userToken)
       {
           Map duradosMap = GetDuradosMap();
           string parameters = string.Format("appName={0}&userToken={1}", appName, userToken);
           return Durados.Security.CipherUtility.Encrypt<System.Security.Cryptography.AesManaged>(parameters, duradosMap.Database.DefaultMasterKeyPassword, duradosMap.Database.Salt);
       }

       private bool IsManualApprove(string appName)
       {
           Map map = GetMap(appName);
           return map.Database.ApproveNewUsersManually;
       }

       private void Approve(string username, string appName, BeforeEditEventHandler beforeEditCallback, BeforeEditInDatabaseEventHandler beforeEditInDatabaseCallback, AfterEditEventHandler afterEditBeforeCommitCallback, AfterEditEventHandler afterEditAfterCommitCallback)
       {
           Map map = GetMap(appName);

           Dictionary<string, object> values = new Dictionary<string, object>();
           values.Add("IsApproved", true);

           DataRow row = map.Database.GetUserRow(username);
           if (row == null)
               throw new DuradosException("In RestHelper.Approve could not find user row, GetUserRow, for username: " + username);

           string userId = row["ID"].ToString();

           map.Database.GetUserView().Edit(values, userId, beforeEditCallback, beforeEditInDatabaseCallback, afterEditBeforeCommitCallback, afterEditAfterCommitCallback);

           //SendApprovalEmail(username, appName);
       }

       protected virtual void Activate(string username, string appName)
       {
           Activate(username, appName, GetDefaultUserRole());
       }
       protected virtual void Activate(string username, string appName, string role)
       {
           Map map = GetMap(appName);
           Durados.DataAccess.SqlAccess sql = new Durados.DataAccess.SqlAccess();
           Dictionary<string, object> parameters = new Dictionary<string, object>();
           parameters.Add("@UserId", map.Database.GetUserID(username));
           parameters.Add("@AppId", map.Id);
           parameters.Add("@Role", role);
           sql.ExecuteNonQuery(Maps.Instance.DuradosMap.Database.ConnectionString, "INSERT INTO [durados_UserApp] ([UserId],[AppId],[Role]) VALUES (@UserId,@AppId,@Role)", parameters, null);
       }

       public virtual void ActivateAdmin(string username, string appName)
       {
           bool isAuthenticated = IsAuthenticated(username, appName);

           if (isAuthenticated)
           {
               Activate(username, appName, "Admin");
           }
           else
           {
               InviteAdminBeforeAignUp(username, appName);
           }
           
       }

       private void InviteAdminBeforeAignUp(string username, string appName)
       {
           SqlAccess sqlAccess = new SqlAccess(); 

           try
           {
               Map map = GetDuradosMap();
               string appId = GetMap(appName).Id;
               sqlAccess.ExecuteNonQuery(map.connectionString, string.Format("insert into durados_Invite (username, appId) values ('{0}', {1})", username, appId));
           }
           catch (Exception exception)
           {
               throw new DuradosException("Failed to invite admin before signup", exception);
           }
       }

       public virtual void InviteAdminAfterSignUp(string username)
       {
           try
           {
               Map map = GetDuradosMap();
               int userId = GetDuradosMap().Database.GetUserID(username);

               using (SqlConnection connection = new SqlConnection(map.connectionString))
               {
                   connection.Open();

                   SqlTransaction transaction = connection.BeginTransaction(IsolationLevel.ReadCommitted);

                   try
                   {
                       using (SqlCommand command = new SqlCommand())
                       {

                           command.Connection = connection;
                           command.Transaction = transaction;

                           command.CommandText = string.Format("select appId from durados_Invite where username = '{0}'", username);
                           List<string> apps = new List<string>();
                           using (IDataReader reader = command.ExecuteReader())
                           {
                               while (reader.Read())
                               {
                                   string appId = reader["appId"].ToString();

                                   apps.Add(appId);
                               }
                           }

                           foreach(string appId in apps)
                           {
                               command.CommandText = string.Format("insert into durados_UserApp (UserId, AppId, Role) values ({0},{1},'{2}')", userId, appId, "Admin");
                               command.ExecuteNonQuery();
                           }
                           command.CommandText = string.Format("delete durados_Invite where username = '{0}'", username);
                           command.ExecuteNonQuery();
                           transaction.Commit();

                       }
                   }
                   catch
                   {
                       try
                       {
                           transaction.Rollback();
                       }
                       catch { }
                   }

               }
               
           }
           catch (Exception exception)
           {
               throw new DuradosException("Failed to invite admin after signup", exception);
           }
       }

       protected virtual void Invite(string appName, string firstName, string lastName, string username, string role, Dictionary<string, object> parameters, BeforeCreateEventHandler beforeCreateCallback, BeforeCreateInDatabaseEventHandler beforeCreateInDatabaseEventHandler, AfterCreateEventHandler afterCreateBeforeCommitCallback, AfterCreateEventHandler afterCreateAfterCommitCallback)
       {
           Map map = GetMap(appName);
           if (map is DuradosMap)
               return;

           View userView = (View)map.Database.GetUserView();

           Dictionary<string, object> values = new Dictionary<string, object>();
           values.Add("Username", username);
           values.Add("FirstName", firstName);
           values.Add("LastName", lastName);
           values.Add("Email", username);
           values.Add("IsApproved", false);
           values.Add(userView.GetFieldByColumnNames("Role").Name, role ?? GetDefaultUserRole(appName));

           if (parameters != null)
           {
               foreach (string key in parameters.Keys)
               {
                   if (!values.ContainsKey(key))
                       values.Add(key.AsToken(), parameters[key]);
               }
           }

           userView.Create(values, null, beforeCreateCallback, beforeCreateInDatabaseEventHandler, afterCreateBeforeCommitCallback, afterCreateAfterCommitCallback);
       }

       protected virtual bool IsPending(string username)
       {
           System.Web.Security.MembershipUser existingUser = System.Web.Security.Membership.Provider.GetUser(username, false);

           if (existingUser == null)
           {
               throw new NotSignedUpToBackandException();
           }

           return !existingUser.IsApproved;
       }

       //protected virtual void AddToAuthenticatedUsers(string appName, string firstName, string lastName, string username, string password, bool isPending)
       //{
       //    string defaultUserRole = role ?? GetDefaultUserRole();
       //    AddToAuthenticatedUsers(appName, firstName, lastName, username, password, !isPending, defaultUserRole);
       //}

       protected virtual string GetDefaultUserRole()
       {
           return "User";
       }

       protected virtual string GetDefaultUserRole(string appName)
       {
           Map map = GetMap(appName);

           string role = map.Database.NewUserDefaultRole;

           if (string.IsNullOrEmpty(role))
           {
               throw new MissingDefaultSignupRoleBackandException();
           }

           return role;
       }

       protected virtual void AddToAuthenticatedUsers(string appName, string firstName, string lastName, string username, string password, bool isApproved, string role)
       {
           Dictionary<string, object> parameters = new Dictionary<string, object>();
           parameters.Add("Username", username);
           parameters.Add("Password", password);
           parameters.Add("FirstName", firstName);
           parameters.Add("LastName", lastName);
           parameters.Add("Email", username);
           parameters.Add("Role", role);
           parameters.Add("Guid", Guid.NewGuid());
           Durados.DataAccess.SqlAccess sql = new Durados.DataAccess.SqlAccess();
           sql.ExecuteNonQuery(GetDuradosMap().Database.GetUserView().ConnectionString, "INSERT INTO [" + GetDuradosMap().Database.GetUserView().GetTableName() + "] ([Username],[FirstName],[LastName],[Email],[Role],[Guid]) VALUES (@Username,@FirstName,@LastName,@Email,@Role,@Guid)", parameters, AddToAuthenticatedUsersCallback);

       }

       protected virtual string AddToAuthenticatedUsersCallback(Dictionary<string, object> paraemeters)
       {
           CreateMembership(paraemeters["Username"].ToString(), paraemeters["Password"].ToString(), paraemeters["Role"].ToString());
           return "success";
       }
       protected virtual void CreateMembership(string username, string password, string role)
       {
           
           System.Web.Security.MembershipUser existingUser = System.Web.Security.Membership.Provider.GetUser(username, false);
           if (existingUser != null)
           {
               if (!System.Web.Security.Membership.Provider.ValidateUser(username, password))
               {
                   throw new AlreadySignedUpToBackandException();
               }
           }

           System.Web.Security.MembershipCreateStatus status = CreateUser(username, password, username);

           if (status == MembershipCreateStatus.Success)
           {

               //System.Web.Security.Roles.AddUserToRole(username, role);

               System.Web.Security.MembershipUser user = System.Web.Security.Membership.Provider.GetUser(username, false);
               user.IsApproved = false;
               System.Web.Security.Membership.UpdateUser(user);
           }
           else if (status == MembershipCreateStatus.DuplicateUserName)
           {
           }
           else
           {
               throw new MembershipException(status);
           }
       }

       protected virtual MembershipCreateStatus CreateUser(string userName, string password, string email)
       {
           MembershipCreateStatus status;
           System.Web.Security.Membership.Provider.CreateUser(userName, password, email, null, null, true, null, out status);
           return status;
       }

       protected virtual bool IsAuthenticated(string username, string appName)
       {
           return GetDuradosMap().Database.GetUserRow(username) != null;
       }

       protected virtual bool IsActive(string username, string appName)
       {
           if (appName == Maps.DuradosAppName)
               return false;

           Map map = GetMap(appName);

           return Maps.Instance.AppExists(appName, map.Database.GetUserID(username)).HasValue;
       }

       protected virtual bool IsInvited(string username, string appName)
       {
           if (appName == Maps.DuradosAppName)
           {
               return true;
           }

           Map map = GetMap(appName);
           return map.Database.GetUserRow(username) != null;
       }

       public SignUpResults Verify(string appName, string token, BeforeEditEventHandler beforeEditCallback, BeforeEditInDatabaseEventHandler beforeEditInDatabaseCallback, AfterEditEventHandler afteEditBeforeCommitCallback, AfterEditEventHandler afterEditAfterCommitCallback)
       {
           SignUpResults signUpResults = null;

           try
           {
               signUpResults = Decrypt(token);
           }
           catch (Exception exception)
           {
               try
               {
                   GetMap(appName).Logger.Log("signup", "verify", string.Empty, exception, 2, string.Empty);
               }
               catch { };
               signUpResults = new SignUpResults() { AppName = appName };
               signUpResults.Redirect = GetRedirectToSignUpWithError(appName) + "&message=" + exception.Message;
               return signUpResults;
           }

           return Verified(appName, signUpResults, beforeEditCallback, beforeEditInDatabaseCallback, afteEditBeforeCommitCallback, afterEditAfterCommitCallback);
       }

       private SignUpResults Verified(string appName, SignUpResults signUpResults, BeforeEditEventHandler beforeEditCallback, BeforeEditInDatabaseEventHandler beforeEditInDatabaseCallback, AfterEditEventHandler afteEditBeforeCommitCallback, AfterEditEventHandler afterEditAfterCommitCallback)
       {
           FinishPendingUser(signUpResults.Username);

           if (appName == Maps.DuradosAppName)
           {
               signUpResults.Status = SignUpStatus.Ready;
               return signUpResults;
           }

           Activate(signUpResults.Username, signUpResults.AppName);

           bool isManualApprove = IsManualApprove(signUpResults.AppName);
           signUpResults.Status = SignUpStatus.PendingAdminApproval;

           if (!isManualApprove)
           {
               Approve(signUpResults.Username, signUpResults.AppName, beforeEditCallback, beforeEditInDatabaseCallback, afteEditBeforeCommitCallback, afterEditAfterCommitCallback);
               signUpResults.Status = SignUpStatus.Ready;
           }

           signUpResults.Redirect = GetRedirectToSignIn(signUpResults.AppName);

           return signUpResults;
       }

       public virtual string GetSignUpStatusMessage(SignUpStatus status)
       {
           if (status == SignUpStatus.PendingAdminApproval)
               return "The user signed up and is now waiting for an administrator approval.";
           else if (status == SignUpStatus.PengingVerification)
               return "The system is now waiting for the user to responed a verification email.";
           else
               return "The user is ready to sign in";
       }

       private string GetRedirectToSignIn(string appName)
       {
           Map map = GetMap(appName);
           return map.Database.SignInRedirectUrl;
       }

       private string GetRedirectToSignUpWithError(string appName)
       {
           Map map = GetMap(appName);
           string url = map.Database.RegistrationRedirectUrl;

           string appendSign = "?";
           if (url.Contains(appendSign))
           {
               appendSign = "&";
           }
           return url + appendSign + "verificationError=true";
       }

       private void FinishPendingUser(string username)
       {
           System.Web.Security.MembershipUser user = System.Web.Security.Membership.Provider.GetUser(username, false);
           user.IsApproved = true;
           System.Web.Security.Membership.UpdateUser(user);
       }

       private SignUpResults Decrypt(string token)
       {
           Map duradosMap = GetDuradosMap();
           string text = null;
           try
           {
               text = Durados.Security.CipherUtility.Decrypt<System.Security.Cryptography.AesManaged>(token, duradosMap.Database.DefaultMasterKeyPassword, duradosMap.Database.Salt);
           }
           catch
           {
               text = Durados.Security.CipherUtility.Decrypt<System.Security.Cryptography.AesManaged>(System.Web.HttpContext.Current.Server.UrlDecode(token), duradosMap.Database.DefaultMasterKeyPassword, duradosMap.Database.Salt);
           }

           string appName = null;
           string userToken = null;
           foreach (string parameter in text.Split('&'))
           {
               string[] keyValue = parameter.Split('=');
               string key = keyValue[0];
               string val = keyValue[1];
               if (key == "appName")
               {
                   appName = val;
               }
               else if (key == "userToken")
               {
                   userToken = val;
               }
           }

           if (appName == null)
           {
               throw new TokenDecryptionException("Missing app name");
           }
           if (userToken == null)
           {
               throw new TokenDecryptionException("user token decryption");
           }

           Map map = GetMap(appName);

           string userGuid = Durados.Web.Mvc.UI.Helpers.SecurityHelper.GetUserGuidFromTmpGuid(userToken);

           string username = map.Database.GetUsernameByGuid(userGuid);
           if (username == null)
           {
               throw new TokenDecryptionException("temp guid failure");
           }

           return new SignUpResults() { Username = username, AppName = appName};

       }

       protected virtual bool IsPrivate(string appName)
       {
           if (appName == Maps.DuradosAppName)
               return true;
           Map map = GetMap(appName);
           return !map.Database.EnableUserRegistration;
       }

       protected virtual bool IsAppExists(string appName)
       {
           return Maps.Instance.AppInCach(appName) || Maps.Instance.AppExists(appName).HasValue;
       }

       public class SignUpException : DuradosException
       {
           public SignUpException(string message, Exception innerException) : base(message, innerException) { }
           public SignUpException(Exception exception)
               : base("An unexpected error has occured during the signup.", exception)
           {

           }

           public SignUpException(string message)
               : base(message)
           {

           }
       }

       public class PasswordConfirmationException : SignUpException
       {
           public PasswordConfirmationException()
               : base("The password does not match with the confirmation.")
           {

           }
       }

       public class AppDoesNotExistException : SignUpException
       {
           public AppDoesNotExistException(string appName)
               : base(string.Format("The app {0} does not exist.", appName))
           {

           }
       }

       public class UserDoesNotExistException : SignUpException
       {
           public UserDoesNotExistException(string username)
               : base(string.Format("The user {0} does not exist.", username))
           {

           }
       }

       public class OnlyInvitedUsersAreAllowedException : SignUpException
       {
           public OnlyInvitedUsersAreAllowedException()
               : base("Only invited users are allowed.")
           {

           }
       }

       public class TokenDecryptionException : SignUpException
       {
           public TokenDecryptionException(string message)
               : base("Verification token decryption failure: " + message)
           {

           }
       }


       public class AlreadySignedUpToAppException : SignUpException
       {
           public AlreadySignedUpToAppException()
               : base("The user is already signed up to this app")
           {

           }
       }

       public class AlreadySignedUpToBackandException : SignUpException
       {
           public AlreadySignedUpToBackandException()
               : base("The user is already signed up to this app")
           {

           }
       }

       public class NotSignedUpToBackandException : SignUpException
       {
           public NotSignedUpToBackandException()
               : base("The user is not signed up to backand")
           {

           }
       }

       public class MissingDefaultSignupRoleBackandException : SignUpException
       {
           public MissingDefaultSignupRoleBackandException()
               : base("Missing default signup role")
           {

           }
       }

       public class MembershipException : SignUpException
       {
           public MembershipException(MembershipCreateStatus status)
               : base("Membership failure:" + status.ToString())
           {

           }
       }
       

       public class SignUpResults
       {
           public string Username { get; set; }
           public string AppName { get; set; }
           public string Redirect { get; set; }

           public SignUpStatus Status { get; set; }
       }

       public enum SignUpStatus
       {
           Ready = 1,
           PengingVerification = 2,
           PendingAdminApproval = 3
       }

       public object[] GetListOfPossibleStatus()
       {
           List<object> statuses = new List<object>();

           foreach(SignUpStatus status in Enum.GetValues(typeof(SignUpStatus)))
           {
               statuses.Add(new { status = status, message = GetSignUpStatusMessage(status) });
           }

           return statuses.ToArray();
       }

       public void SendForgotPasswordToken(string appName, string username)
       {
           Durados.Web.Mvc.Workflow.Engine wfe = CreateWorkflowEngine();
           string guid = GetUserGuid(username);

           string token = SecurityHelper.GetTmpUserGuidFromGuid(guid);

           Map map = Maps.Instance.GetMap(appName);

           Durados.View view = map.Database.GetUserView();
           DataRow row = map.Database.GetUserRow(username);
           if (row == null)
               throw new UserDoesNotExistException(username);

           string mainAppUrl = System.Configuration.ConfigurationManager.AppSettings["forgotPasswordUrl"] ?? Maps.GetAppUrl("www") + "/Account/ChangePassword?id=" + token + "&isReset=true";
           string currentAppUrl = map.Database.ForgotPasswordUrl;
           if (!string.IsNullOrEmpty(currentAppUrl))
               currentAppUrl += "?token=" + token;

           Dictionary<string, object> values = new Dictionary<string, object>();
           string id = row["ID"].ToString();
           values.Add("token".AsToken(), token);
           values.Add("ForgotPasswordUrl".AsToken(), string.IsNullOrEmpty(currentAppUrl) ? mainAppUrl : currentAppUrl);
           values.Add("AppName".AsToken(), appName);
           values.Add("FirstName".AsToken(), row.IsNull("FirstName") ? username : row["FirstName"].ToString());


           wfe.PerformActions(this.controller, view, Durados.TriggerDataAction.OnDemand, values, id, row, map.Database.ConnectionString, Convert.ToInt32(id), row["Role"].ToString(), null, "requestResetPassword");

           Debug.WriteLine(token);
   }

       private string GetUserGuid(string userName)
       {
           try
           {
               Durados.DataAccess.SqlAccess sql = new Durados.DataAccess.SqlAccess();

               Dictionary<string, object> parameters = new Dictionary<string, object>();

               parameters.Add("@username", userName);
               object guid = sql.ExecuteScalar(Maps.Instance.DuradosMap.connectionString, "SELECT TOP 1 [durados_user].[guid] FROM durados_user WITH(NOLOCK)  WHERE [durados_user].[username]=@username", parameters);

               if (guid == null || guid == DBNull.Value)
                   throw new DuradosException("Username has no uniqe guid ,password canot be reset.");

               return guid.ToString();
           }
           catch (Exception ex)
           {
               throw new DuradosException("User guid was not found.", ex);
           }

       }



       public void ChangePassword(Guid token, string password)
       {
           string userSysGuid = SecurityHelper.GetUserGuidFromTmpGuid(token.ToString());
           if (string.IsNullOrEmpty(userSysGuid))
           {
               throw new DuradosException("User identification is invalid.");
           }
           string username = GetUsername(userSysGuid);
           if (string.IsNullOrEmpty(username))// &&  guid.Equals(userSysGuid)
           {
               throw new DuradosException("User data is invalid.");

           }

           ChangePassword(username, password);
       }

       private string GetUsername(string guid)
       {
           Durados.DataAccess.SqlAccess sqlAccess = new Durados.DataAccess.SqlAccess();

           Dictionary<string, object> parameters = new Dictionary<string, object>();

           parameters.Add("@guid", guid);

           string sqlDuradosSys = string.Format("SELECT TOP 1 username FROM durados_user WITH(NOLOCK)  WHERE guid=@guid");

           return sqlAccess.ExecuteScalar(Maps.Instance.ConnectionString, sqlDuradosSys, parameters);
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
                  method= (string)request["method"];
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
                   headers.Add("AppName", appName);
               }

               int index2 = index;

               tasks.Add(Task.Factory.StartNew(() =>
               {
                   var responseStatusAndData = GetWebResponse(method, url, data, parameters, headers, index2);
                   responses[responseStatusAndData.index] = responseStatusAndData;
                   return responseStatusAndData.data;
               }));


               
               //At this point tasks[0].Result will be the result (The Response) of the first task
               //tasks[1].Result will be the result of the second task and so on.
              
           }
           Task.WaitAll(tasks.ToArray());
               
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
               bytes = System.Text.Encoding.ASCII.GetBytes(data);
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
                   return new ResponseStatusAndData() { status = (int)((HttpWebResponse)responseTask.Result).StatusCode, data = reader.ReadToEnd(), index = index };
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
                           message = webException.Message;
                       }
                   }
               }
               if (string.IsNullOrEmpty(message))
                   throw exception;

               return new ResponseStatusAndData() { status = status, data = message, index = index };
           }
           
       }

       public class ResponseStatusAndData
       {
           public int status { get; set; }
           public string data { get; set; }
           internal int index { get; set; }
           
       }
       
   }


   public class ExternalLoginData
   {
       public string LoginProvider { get; set; }
       public string ProviderKey { get; set; }
       public string UserName { get; set; }

       public IList<Claim> GetClaims()
       {
           IList<Claim> claims = new List<Claim>();
           claims.Add(new Claim(ClaimTypes.NameIdentifier, ProviderKey, null, LoginProvider));

           if (UserName != null)
           {
               claims.Add(new Claim(ClaimTypes.Name, UserName, null, LoginProvider));
           }

           return claims;
       }
       public static ExternalLoginData FromIdentity(ClaimsIdentity identity)
       {
           if (identity == null)
           {
               return null;
           }

           Claim providerKeyClaim = identity.FindFirst(ClaimTypes.NameIdentifier);

           if (providerKeyClaim == null || String.IsNullOrEmpty(providerKeyClaim.Issuer)
               || String.IsNullOrEmpty(providerKeyClaim.Value))
           {
               return null;
           }

           if (providerKeyClaim.Issuer == ClaimsIdentity.DefaultIssuer)
           {
               return null;
           }

           return new ExternalLoginData
           {
               LoginProvider = providerKeyClaim.Issuer,
               ProviderKey = providerKeyClaim.Value,
               UserName = identity.Claims.FirstOrDefault().Value//FindFirstValue(ClaimTypes.Name)
           };
       }
   }


   
   
}
