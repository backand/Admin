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

using Durados;
using Durados.Web.Mvc;
using Durados.Web.Mvc.Controllers.Api;
using Durados.DataAccess;
using System.Data;
using Durados.Web.Mvc.Controllers;
using BackAnd.Web.Api.Controllers.Filters;
using System.Threading.Tasks;
using System.Collections;

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
    //[System.Web.Http.Authorize]
    [LogExceptionFilter]
    [LogActionFilter]
    public class apiController : ApiController
    {
        protected const string JsonNull = "\"null\"";
        //
        // GET: /v1/
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

        public Durados.Database Database { get; private set; }
        protected Durados.Web.Mvc.Workflow.Engine wfe = null;
        public const string GuidKey = "JsGuid";
        public const string actionHeaderGuidName = "Action-Guid";

        public apiController()
            : base()
        {
            //Init();
        }

        protected virtual string GetWarnings(Dictionary<string, object> transformResult)
        {
            string warnings = string.Empty;
            try
            {

                if (!transformResult.ContainsKey("warnings"))
                {
                    return warnings;
                }

                int i = 1;
                foreach (object warning in (ArrayList)transformResult["warnings"])
                {

                    warnings += "(" + i++ + ") " + (warning is string ? warning.ToString() : new JavaScriptSerializer().Serialize(warning)) + ";\n";
                }
            }
            catch
            {
                warnings = new JavaScriptSerializer().Serialize(transformResult);
            }
            return warnings;
        }

        protected virtual Dictionary<string, object> transformJson(string json, bool shouldGeneralize = false)
        {
            string getNodeUrl = GetNodeUrl() + "/transformJson";

            bulk bulk = new Durados.Web.Mvc.UI.Helpers.bulk();

            JavaScriptSerializer jss = new JavaScriptSerializer();

            var data = jss.Deserialize<Dictionary<string, object>>(json);

            if (!data.ContainsKey("appName"))
            {
                string appName = Map.AppName;
                if (string.IsNullOrEmpty(appName))
                {
                    appName = (System.Web.HttpContext.Current.Items[Durados.Web.Mvc.Database.AppName] ?? string.Empty).ToString();
                }
                data.Add("appName", appName);
            }
            if (shouldGeneralize)
            {
                data.Add("shouldGeneralize", shouldGeneralize);
            }

            json = jss.Serialize(data);

            var tasks = new List<Task<string>>();
            object responses = null;
            tasks.Add(Task.Factory.StartNew(() =>
            {
                //, { "Authorization", Request.Headers.Authorization.ToString() }
                var responseStatusAndData = bulk.GetWebResponse("POST", getNodeUrl, json, null, new Dictionary<string, object>() { { "Content-Type", "application/json" }, { "Authorization", Request.Headers.Authorization.ToString() } }, 0);
                responses = responseStatusAndData.data;
                if (string.IsNullOrEmpty(responseStatusAndData.data))
                {
                    if (responseStatusAndData.GetHeaders()["error"] != null && !string.IsNullOrEmpty(responseStatusAndData.GetHeaders()["error"].ToString()))
                    {
                        throw new DuradosException(responseStatusAndData.GetHeaders()["error"].ToString());
                    }
                }
                return responseStatusAndData.data;
            }));

            Task.WaitAll(tasks.ToArray());

            Dictionary<string, object> result = null;
            try
            {
                result = jss.Deserialize<Dictionary<string, object>>(responses.ToString());
            }
            catch
            {
                throw new DuradosException(responses.ToString());
            }

            if (result == null)
            {
                LogModel(json, string.Empty, string.Empty, "nosql");
            }
            else
            {
                LogModel(json, new JavaScriptSerializer().Serialize(result), result.ContainsKey("valid") ? result["valid"].ToString() : string.Empty, "nosql");
            }

            return result;



        }

        protected void ReplaceVariables(Dictionary<string, object> result)
        {
            if (result.ContainsKey("values") && result.ContainsKey("variables") && result.ContainsKey("str") && result.ContainsKey("where"))
            {
                foreach (string variable in (ArrayList)result["variables"])
                {
                    string value = ((Dictionary<string, object>)result["values"])[variable].ToString();
                    if (!(value.StartsWith("'") && value.EndsWith("'")))
                    {
                        decimal d = 0;
                        if (!decimal.TryParse(value, out d))
                        {
                            value = value.Pad("'");
                        }
                    }
                    result["str"] = result["str"].ToString().Replace(variable, value);
                    result["where"] = result["where"].ToString().Replace(variable, value);
                }
            }
        }

        protected virtual Dictionary<string, object> Transform(string json, bool getOldSchema = true)
        {
            string getNodeUrl = GetNodeUrl() + "/transform";

            bulk bulk = new Durados.Web.Mvc.UI.Helpers.bulk();

            JavaScriptSerializer jss = new JavaScriptSerializer();

            var data = jss.Deserialize<Dictionary<string, object>>(json);

            if (!data.ContainsKey("oldSchema"))
            {
                data.Add("oldSchema", "");
            }

            if (getOldSchema)
            {
                data["oldSchema"] = GetBackandToObject();
            }

            json = jss.Serialize(data);

            var tasks = new List<Task<string>>();
            object responses = null;
            tasks.Add(Task.Factory.StartNew(() =>
            {
                //, { "Authorization", Request.Headers.Authorization.ToString() }
                var responseStatusAndData = bulk.GetWebResponse("POST", getNodeUrl, json, null, new Dictionary<string, object>() { { "Content-Type", "application/json" } }, 0);
                responses = responseStatusAndData.data;
                return responseStatusAndData.data;
            }));

            Task.WaitAll(tasks.ToArray());

            Dictionary<string, object> result = null;
            try
            {
                result = jss.Deserialize<Dictionary<string, object>>(responses.ToString());
            }
            catch
            {
                throw new DuradosException(responses.ToString());
            }

            LogModel(json, new JavaScriptSerializer().Serialize(result), result.ContainsKey("valid") ? result["valid"].ToString() : string.Empty);

            return result;



        }

        protected void LogModel(string input, string output, string valid, string action = "model")
        {
            try
            {
                string appName = Map.AppName;
                if (string.IsNullOrEmpty(appName))
                {
                    appName = (System.Web.HttpContext.Current.Items[Durados.Web.Mvc.Database.AppName] ?? string.Empty).ToString();
                }
                LogModel(appName, Map.Database.GetCurrentUsername(), DateTime.Now, input, output, valid, action);
            }
            catch (Exception exception)
            {
                Maps.Instance.DuradosMap.Logger.Log("Model", "Validate", "LogModel", exception, 1, null);
            }
        }

        protected void UpdateLogModel(Exception exception)
        {
            try
            {
                if (logModelId.HasValue)
                {
                    using (System.Data.SqlClient.SqlConnection connection = new System.Data.SqlClient.SqlConnection(Maps.Instance.DuradosMap.connectionString))
                    {
                        connection.Open();
                        string sql = "update [backand_model] set errorMessage = @errorMessage, errorTrace = @errorTrace where id=@id";

                        using (System.Data.SqlClient.SqlCommand command = new System.Data.SqlClient.SqlCommand(sql, connection))
                        {
                            command.Parameters.AddWithValue("errorMessage", exception.Message);
                            command.Parameters.AddWithValue("errorTrace", exception.StackTrace);
                            command.Parameters.AddWithValue("id", logModelId.Value);
                            command.ExecuteNonQuery();

                        }

                        connection.Close();
                    }
                }
            }
            catch (Exception exception2)
            {
                Maps.Instance.DuradosMap.Logger.Log("Model", "Validate", "LogModel", exception, 1, null);
                Maps.Instance.DuradosMap.Logger.Log("Model", "Validate", "LogModel", exception2, 1, null);
            }
        }

        protected int? logModelId = null;

        private void LogModel(string appName, string username, DateTime timestamp, string input, string output, string valid, string action)
        {
            using (System.Data.SqlClient.SqlConnection connection = new System.Data.SqlClient.SqlConnection(Maps.Instance.DuradosMap.connectionString))
            {
                connection.Open();
                string sql = "insert into [backand_model] ([appName], [username], [timestamp], [input], [output], [valid], [action]) values (@appName, @username, @timestamp, @input, @output, @valid, @action); SELECT IDENT_CURRENT(N'backand_model') AS ID";

                using (System.Data.SqlClient.SqlCommand command = new System.Data.SqlClient.SqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("appName", appName);
                    command.Parameters.AddWithValue("username", username);
                    command.Parameters.AddWithValue("timestamp", timestamp);
                    command.Parameters.AddWithValue("input", input);
                    command.Parameters.AddWithValue("output", output);
                    command.Parameters.AddWithValue("valid", valid);
                    command.Parameters.AddWithValue("action", action);
                    object scalar = command.ExecuteScalar();
                    logModelId = Convert.ToInt32(scalar);
                }

                connection.Close();
            }
        }

        protected virtual string GetNodeUrl()
        {
            return System.Configuration.ConfigurationManager.AppSettings["nodeHost"] ?? "http://127.0.0.1:9000";

        }

        protected virtual int CountViews()
        {
            return Map.Database.Views.Values.Where(v => !v.SystemView).Count();
        }
        
        protected virtual ArrayList GetBackandToObject()
        {
            return GetBackandToObject(Request.Headers.Authorization.ToString());
        }

        protected virtual string GetAppName()
        {
            string appName = Request.Headers.GetValues("AppName").FirstOrDefault();
            if (appName == null)
            {
                appName = Map.AppName;
            }

            return appName;
        }
        protected virtual ArrayList GetBackandToObject(string token)
        {
            if (CountViews() == 0)
                return new ArrayList();

            string getNodeUrl = GetNodeUrl() + "/json";

            bulk bulk = new Durados.Web.Mvc.UI.Helpers.bulk();


            var tasks = new List<Task<string>>();
            object responses = null;
            int status = 0;
            tasks.Add(Task.Factory.StartNew(() =>
            {
                var responseStatusAndData = bulk.GetWebResponse("POST", getNodeUrl, null, null, new Dictionary<string, object>() { { "Content-Type", "application/json" }, { "Authorization", Request.Headers.Authorization.ToString() }, { "AppName", GetAppName() } }, 0);
                responses = responseStatusAndData.data;
                status = responseStatusAndData.status;
                return responseStatusAndData.data;
            }));

            Task.WaitAll(tasks.ToArray());

            if (status >= 300 || status < 200)
            {
                throw new WebException(responses.ToString(), (WebExceptionStatus)status);
            }

            JavaScriptSerializer jss = new JavaScriptSerializer();
            var result = jss.Deserialize<ArrayList>(responses.ToString());

            return result;
        }

        protected virtual void RefreshOldAdmin(string appName)
        {
            string id = GetMasterGuid();

            string qstring = "id=" + id;
            string url = RestHelper.GetAppUrl(appName, Maps.OldAdminHttp) + "/Admin/Restart?" + qstring;

            Maps.Instance.DuradosMap.Logger.Log(GetControllerNameForLog(ControllerContext), GetActionName(), this.Request.Method.Method, "Refresh admin after Sync " + appName, url, 3, null, DateTime.Now);

            string response = string.Empty;
            try
            {
                Durados.Web.Mvc.Infrastructure.Http.AsynWebRequest(url, null);
            }
            catch { }
           
        }

        internal protected virtual void Init()
        {
            Database = GetDatabase();
            wfe = CreateWorkflowEngine();
        }

        protected void RefreshConfigCache()
        {
            lock (Map)
            {
                Durados.Web.Mvc.Database configDatabase = Map.GetConfigDatabase();
                Map.Database.SetNextMinorConfigVersion();
                Durados.DataAccess.ConfigAccess.UpdateVersion(Map.Database.ConfigVersion, Map.GetConfigDatabase().ConnectionString);
                Durados.DataAccess.ConfigAccess.SaveConfigDataset(configDatabase.ConnectionString, Map.Logger);
                Map.SaveDynamicMapping();
                Map.Refresh();
                Map.JsonConfigCache.Clear();
                Map.AllKindOfCache.Clear();
                RefreshOldAdmin(Map.AppName);
            }
            FarmCaching.Instance.ClearMachinesCache(Map.AppName);

        }

        protected void RefreshConfigCache(Map map)
        {
            lock (map)
            {
                Durados.Web.Mvc.Database configDatabase = map.GetConfigDatabase();
                map.Database.SetNextMinorConfigVersion();
                Durados.DataAccess.ConfigAccess.UpdateVersion(map.Database.ConfigVersion, map.GetConfigDatabase().ConnectionString);
                Durados.DataAccess.ConfigAccess.SaveConfigDataset(configDatabase.ConnectionString, map.Logger);
                map.SaveDynamicMapping();
                map.Refresh();
                Map.JsonConfigCache.Clear();
                try
                {
                    RefreshOldAdmin(map.AppName);
                }
                catch { }
            }
            FarmCaching.Instance.ClearMachinesCache(map.AppName);

        }

        protected bool IsAdmin()
        {
            string role = Map.Database.GetUserRole();
            return role == "Admin" || role == "Developer";
        }

        public string CurrentApplication
        {
            get
            {
                return Request.ToString().Split('/').FirstOrDefault();
            }
        }

        protected virtual Durados.Database GetDatabase()
        {
            return Map.Database;
        }

        protected virtual Durados.Web.Mvc.Workflow.Engine CreateWorkflowEngine()
        {
            return new Durados.Web.Mvc.Workflow.Engine();
        }

        protected virtual string GetUserID()
        {
            return Map.Database.GetUserID();
        }

        protected virtual string GetViewDisplayName(Durados.Web.Mvc.View view)
        {
            return view.DisplayName;
        }

        protected virtual DataView GetDataView(Durados.Web.Mvc.ChildrenField childrenField, DataRow dataRow)
        {
            return childrenField.GetDataView(dataRow);
        }

        protected virtual DataView GetDataView(Durados.ChildrenField childrenField, string pk)
        {
            return childrenField.GetDataView(pk);
        }

        private string GetUsername()
        {
            return Database.GetCurrentUsername();

        }

        protected virtual string GetSiteWithoutQueryString()
        {
            return System.Web.HttpContext.Current.Request.Url.Scheme + "://" + System.Web.HttpContext.Current.Request.Url.Authority;

        }

        protected virtual string GetMainSiteWithoutQueryString()
        {
            return Maps.Instance.DuradosMap.Url;

        }

        protected virtual string GetTempGuid()
        {
            string currentUser = Map.Database.GetCurrentUsername();
            System.Data.DataRow userRow = Maps.Instance.DuradosMap.Database.GetUserRow(currentUser);
            string guid = userRow["Guid"].ToString();
            return SecurityHelper.GetTmpUserGuidFromGuid(guid);

        }
        protected virtual string GetMasterGuid()
        {
            string currentUser = Maps.SuperDeveloper;
            System.Data.DataRow userRow = Maps.Instance.DuradosMap.Database.GetUserRow(currentUser);
            string guid = userRow["Guid"].ToString();
            return SecurityHelper.GetTmpUserGuidFromGuid(guid);

        }

        #region uri
        protected string GetControllerName()
        {
            return ControllerContext.RouteData.Values["controller"].ToString();
        }

        protected string GetActionName()
        {
            if (ControllerContext.RouteData.Values.ContainsKey("action"))
                return ControllerContext.RouteData.Values["action"].ToString();
            else
                return string.Empty;
        }

        public const char Slash = '/';
        protected virtual string ApiVersion
        {
            get
            {
                return "/1";
            }
        }

        protected string GetErrorUri(Exception exception)
        {
            return "/Error";
        }

        #endregion uri


        #region callbacks
        protected virtual History GetNewHistory()
        {
            if (MySqlAccess.IsMySqlConnectionString(Map.Database.SystemConnectionString))
                return new MySqlHistory();
            return new History();
        }

        protected virtual IDbCommand GetCommand(SqlProduct sqlProduct)
        {
            if (sqlProduct == SqlProduct.Postgre)
                return new Npgsql.NpgsqlCommand();
            else if (sqlProduct == SqlProduct.MySql)
                return new MySql.Data.MySqlClient.MySqlCommand();
            else
                return new System.Data.SqlClient.SqlCommand();
        }

        protected virtual IDbConnection GetConnection(SqlProduct sqlProduct, string connectionString)
        {
            if (sqlProduct == SqlProduct.Postgre)
            {
                Npgsql.NpgsqlConnection connection = new Npgsql.NpgsqlConnection(connectionString);
                connection.ValidateRemoteCertificateCallback += new Npgsql.ValidateRemoteCertificateCallback(connection_ValidateRemoteCertificateCallback);
                return connection;
            }
            else if (sqlProduct == SqlProduct.MySql)
                return new MySql.Data.MySqlClient.MySqlConnection(connectionString );
            else
                return new System.Data.SqlClient.SqlConnection(connectionString);
        }

        bool connection_ValidateRemoteCertificateCallback(System.Security.Cryptography.X509Certificates.X509Certificate cert, System.Security.Cryptography.X509Certificates.X509Chain chain, System.Net.Security.SslPolicyErrors errors)
        {
            return true;
        }

        #region select callbacks

        protected virtual void view_BeforeSelect(object sender, SelectEventArgs e)
        {
            if (string.IsNullOrEmpty(e.Sql))
                SetPermanentFilter((Durados.Web.Mvc.View)e.View, (Durados.DataAccess.Filter)e.Filter);
            else
                SetSql(e);
        }

        protected virtual void view_AfterSelect(object sender, SelectEventArgs e)
        {
        }

        protected virtual void SetPermanentFilter(Durados.Web.Mvc.View view, Durados.DataAccess.Filter filter)
        {
            if (view.Name == view.Database.RoleViewName)
                if (string.IsNullOrEmpty(filter.WhereStatement))
                    filter.WhereStatement = "WHERE  1=1 AND (durados_UserRole.Name <> 'Developer' AND durados_UserRole.Name <>'View Owner')";
                else
                    filter.WhereStatement += "AND (durados_UserRole.Name <> 'Developer' AND durados_UserRole.Name <>'View Owner')";
        }

        protected virtual void SetSql(SelectEventArgs e)
        {
        }

        #endregion select callbacks

        #region create callbacks

        protected void view_BeforeCreate(object sender, CreateEventArgs e)
        {
            try
            {
                BeforeCreate(e);
            }
            catch (Exception exception)
            {
                if (e.Command != null && e.Command.Transaction != null)
                    e.Command.Transaction.Rollback();
                //Map.Logger.Log(GetControllerNameForLog(this.ControllerContext), this.ControllerContext.RouteData.Values["action"].ToString(), exception.Source, exception, 1, null);
                throw exception;
            }
        }

        protected virtual void BeforeCreate(CreateEventArgs e)
        {

            LoadCreationSignature(e.View, e.Values);
            LoadModificationSignature(e.View, e.Values);
            HandleSpecialDefaults((Durados.Web.Mvc.View)e.View, e.Values);

            int currentUserId = Convert.ToInt32(Map.Database.GetUserID());
            string currentUserRole = null;

            try
            {
                currentUserRole = Map.Database.GetUserRole();
            }
            catch
            {
                currentUserRole = Map.Database.DefaultGuestRole ?? Map.Database.NewUserDefaultRole;
            }

            if (e.View.SaveHistory)
            {
                e.History = GetNewHistory();
                e.UserId = currentUserId;
            }

            CreateWorkflowEngine().PerformActions(this, e.View, TriggerDataAction.BeforeCreate, e.Values, e.PrimaryKey, null, Map.Database.ConnectionString, currentUserId, currentUserRole, e.Command);
        }

        private void LoadCreationSignature(Durados.View view, Dictionary<string, object> values)
        {
            Field createDate = view.CreateDate;
            Field createdBy = view.CreatedBy;

            LoadSignature(view, values, createDate, createdBy);

        }

        private void HandleSpecialDefaults(Durados.Web.Mvc.View view, Dictionary<string, object> values)
        {
            HandleSpecialDefaults(view, values, false);
        }

        private void HandleSpecialDefaults(Durados.Web.Mvc.View view, Dictionary<string, object> values, bool import)
        {
            HandleCurrentUserDefault(view, values, import);

            if (import)
            {
                HandleCreationDate(view, values);
            }
            //LoadCreationSignature(view, values);
            //LoadModificationSignature(view, values);
        }

        private void HandleCurrentUserDefault(Durados.Web.Mvc.View view, Dictionary<string, object> values)
        {
            HandleCurrentUserDefault(view, values, false);

            string userViewName = ((Durados.Web.Mvc.Database)Database).UserViewName;
            string usernameFieldName = ((Durados.Web.Mvc.Database)Database).UsernameFieldName;

            if (view.Name == userViewName)
            {
                string currentRole = "User";
                if (values.ContainsKey(usernameFieldName) && values[usernameFieldName] != null)
                {
                    string newUser = values[usernameFieldName].ToString();
                    HandleMultiTenancyUser(currentRole, newUser);
                }
            }
        }

        private void HandleCurrentUserDefault(Durados.Web.Mvc.View view, Dictionary<string, object> values, bool import)
        {
            var fields = view.Fields.Values.Where(f => f.FieldType == FieldType.Parent && f.DefaultValue != null && f.DefaultValue.ToString().ToLower() == Durados.Web.Mvc.Database.UserPlaceHolder.ToLower());
            foreach (Durados.Web.Mvc.ParentField field in fields)
            {
                if (values.ContainsKey(field.Name))
                {
                    if ((values[field.Name] == null || values[field.Name].ToString() == string.Empty || values[field.Name].ToString().ToLower() == Durados.Web.Mvc.Database.UserPlaceHolder.ToLower()))
                    {
                        values[field.Name] = GetUserID();
                    }
                }
                else
                {
                    //if (field.IsExcludedInInsert() || import)
                    //{
                    //    values.Add(field.Name, GetUserID());
                    //}
                    values.Add(field.Name, GetUserID());

                }
            }

        }
        protected virtual void HandleMultiTenancyUser(string currentRole, string newUser)
        {
            //string userViewName = ((Database)Database).UserViewName;
            //string usernameFieldName = ((Database)Database).UsernameFieldName;
            //string roleDbName = "Role";

            //if (e.View.Name == userViewName)
            //{
            //    string roleFieldName = ((Database)Database).GetUserView().GetFieldByColumnNames(roleDbName).Name;
            //    string currentRole = "User";
            //    if (e.Values.ContainsKey(roleFieldName) && !string.IsNullOrEmpty(e.Values[roleFieldName].ToString()))
            //        currentRole = e.Values[roleFieldName].ToString();
            //    string newUser = e.Values[usernameFieldName].ToString();
            string appName = Maps.GetCurrentAppName();
            Dictionary<string, object> parameters = new Dictionary<string, object>();
            parameters.Add("newUser", newUser);
            parameters.Add("appName", appName);
            parameters.Add("role", currentRole);
            SqlAccess sql = new SqlAccess();
            sql.ExecuteNonQuery(Maps.Instance.DuradosMap.connectionString, "durados_NewAppAsignment @newUser, @appName, @role", parameters, SendNewAppAsignment);
            //Maps.Instance.DuradosMap.Database.Views["durados_UserApp"].Create(values, null, view_BeforeCreate, view_BeforeCreateInDatabase, view_AfterCreateBeforeCommit, view_AfterCreateAfterCommit);
            //}
        }

        private void HandleCreationDate(Durados.View view, Dictionary<string, object> values)
        {

            if (view.CreateDate != null)
            {
                Field createDate = view.CreateDate;

                if (!values.ContainsKey(createDate.Name))
                {
                    values.Add(createDate.Name, DateTime.Now);
                }
                else
                {
                    values[createDate.Name] = DateTime.Now;
                }
            }

        }

        protected virtual string SendNewAppAsignment(Dictionary<string, object> parameters)
        {
            return "success";
        }


        protected void view_BeforeCreateInDatabase(object sender, CreateEventArgs e)
        {
            try
            {
                BeforeCreateInDatabase(e);
            }
            catch (Exception exception)
            {
                if (e.Command != null && e.Command.Transaction != null)
                    e.Command.Transaction.Rollback();
                //Map.Logger.Log(GetControllerNameForLog(this.ControllerContext), this.ControllerContext.RouteData.Values["action"].ToString(), exception.Source, exception, 1, null);
                throw exception;
            }
        }

        protected virtual void BeforeCreateInDatabase(CreateEventArgs e)
        {
        }

        virtual protected void view_AfterCreateBeforeCommit(object sender, CreateEventArgs e)
        {
            try
            {
                AfterCreateBeforeCommit(e);

                HandleUploadsSpecialPaths(e.View, DataAction.Create, e.Values, e.PrimaryKey, (DataActionEventArgs)e);

                HandleAdminInvetation(e);

                //if (Maps.MultiTenancy)
                //{
                //    HandleMultiTenancyUser(e);
                //}
            }
            catch (Exception exception)
            {
                if (e.Command != null && e.Command.Transaction != null)
                    e.Command.Transaction.Rollback();
                if (!Database.IdenticalSystemConnection && e.SysCommand != null && e.SysCommand.Transaction != null)
                    e.SysCommand.Transaction.Rollback();
                //Map.Logger.Log(GetControllerNameForLog(this.ControllerContext), this.ControllerContext.RouteData.Values["action"].ToString(), exception.Source, exception, 1, null);
                throw exception;
            }
        }

        protected virtual void HandleAdminInvetation(CreateEventArgs e)
        {
            string userViewName = ((Durados.Web.Mvc.Database)Database).UserViewName;
            if (e.View.Name == userViewName)
            {

               string roleFieldName = e.View.GetFieldByColumnNames("Role").Name;
               string role = e.Values[roleFieldName].ToString();
               bool isAdmin = role.Equals("Admin") || role.Equals("Developer");
               if (isAdmin)
               {
                   string appName = Map.AppName;
                   string username = e.Values["Username"].ToString();

                   Account account = new Account(this);
                   account.ActivateAdmin(username, appName);
               }
            }
        }

        protected virtual void AfterCreateBeforeCommit(CreateEventArgs e)
        {
            //Workflow.Engine wfe = CreateWorkflowEngine();

            if (Database == null)
                Init();

            string userViewName = ((Durados.Web.Mvc.Database)Database).UserViewName;
            if (e.View.Name == userViewName)
            {
                if (!e.Values.ContainsKey("appName".AsToken()))
                {
                    string appName = Map.AppName;
                    e.Values.Add("appName".AsToken(), appName);
                }

                if (!e.Values.ContainsKey("RegistrationRedirectUrl".AsToken()))
                {
                    e.Values.Add("RegistrationRedirectUrl".AsToken(), Map.Database.RegistrationRedirectUrl);
                }
                if (!e.Values.ContainsKey("SignInRedirectUrl".AsToken()))
                {
                    e.Values.Add("SignInRedirectUrl".AsToken(), Map.Database.SignInRedirectUrl);
                }
            }
            string currentUserRole = null;

            try
            {
                currentUserRole = Map.Database.GetUserRole();
            }
            catch
            {
                currentUserRole = Map.Database.DefaultGuestRole ?? Map.Database.NewUserDefaultRole;
            }

            wfe.PerformActions(this, e.View, TriggerDataAction.AfterCreateBeforeCommit, e.Values, e.PrimaryKey, null, Map.Database.ConnectionString, Convert.ToInt32(((Durados.Web.Mvc.Database)e.View.Database).GetUserID()), currentUserRole, e.Command);


        }

        protected void view_AfterCreateAfterCommit(object sender, CreateEventArgs e)
        {
            AfterCreateAfterCommit(e);
        }

        protected virtual void AfterCreateAfterCommit(CreateEventArgs e)
        {
            //Workflow.Engine wfe = CreateWorkflowEngine();
            string currentUserRole = null;

            try
            {
                currentUserRole = Map.Database.GetUserRole();
            }
            catch
            {
                currentUserRole = Map.Database.DefaultGuestRole ?? Map.Database.NewUserDefaultRole;
            }
            if (wfe == null)
                wfe = CreateWorkflowEngine();
            wfe.PerformActions(this, e.View, TriggerDataAction.AfterCreate, e.Values, e.PrimaryKey, null, Map.Database.ConnectionString, Convert.ToInt32(((Durados.Web.Mvc.Database)e.View.Database).GetUserID()), currentUserRole, e.Command);


        }

        #endregion create callbacks

        #region update callbacks

        protected virtual void view_BeforeEditInDatabase(object sender, EditEventArgs e)
        {
            try
            {
                BeforeEditInDatabase(e);
            }
            catch (Exception exception)
            {
                if (e.Command != null && e.Command.Transaction != null)
                    e.Command.Transaction.Rollback();
                //Map.Logger.Log(GetControllerNameForLog(this.ControllerContext), this.ControllerContext.RouteData.Values["action"].ToString(), exception.Source, exception, 1, null);
                throw exception;
            }
        }

        protected virtual void BeforeEditInDatabase(EditEventArgs e)
        {
        }

        protected virtual void view_BeforeEdit(object sender, EditEventArgs e)
        {
            try
            {
                BeforeEdit(e);
            }
            catch (Exception exception)
            {
                if (e.Command != null && e.Command.Transaction != null)
                    e.Command.Transaction.Rollback();
                //Map.Logger.Log(GetControllerNameForLog(this.ControllerContext), this.ControllerContext.RouteData.Values["action"].ToString(), exception.Source, exception, 1, null);
                throw exception;
            }
        }

        protected virtual void BeforeEdit(EditEventArgs e)
        {
            HandleEncryptedHiddenFields(e);

            LoadModificationSignature(e.View, e.Values);

            //if (IsApprovalProcessUserView(e.View))
            //{
            //    HandleApprovalProcess(e);
            //}

            int currentUserId = Convert.ToInt32(Map.Database.GetUserID());
            string currentUserRole = Map.Database.GetUserRole();
            
            if (e.View.SaveHistory)
            {
                e.History = GetNewHistory();
                e.UserId = currentUserId;
                if (e.View.Name == "Database" && e.View.Database.IsConfig && Map is DuradosMap)
                {
                    try
                    {
                        Map map = Maps.Instance.GetMap(System.Web.HttpContext.Current.Items[Durados.Database.AppName].ToString());
                        e.UserId = Convert.ToInt32(map.Database.GetUserID());
                    }
                    catch { }
                }
                if (e.View.Database is Durados.Web.Mvc.Config.Database)
                {
                    e.Command = GetCommand(Map.Database.SqlProduct);
                    e.Command.Connection = GetConnection(Map.Database.SqlProduct, Map.Database.ConnectionString);

                    try
                    {
                        if (Database == null)
                            Database = Maps.Instance.GetMap(System.Web.HttpContext.Current.Items[Durados.Database.AppName].ToString()).Database;
                        if (Database.IdenticalSystemConnection)
                        {
                            e.SysCommand = GetCommand(Map.SystemSqlProduct); ;
                        }
                        else
                        {
                            e.SysCommand = GetCommand(Map.Database.SystemSqlProduct);
                            e.SysCommand.Connection = GetConnection(Map.Database.SystemSqlProduct, Map.Database.SystemConnectionString);
                        }
                    }
                    catch { }
                }
            }
            if (e.View.GetRules().Count() > 0)
            {
                CreateWorkflowEngine().PerformActions(this, e.View, TriggerDataAction.BeforeEdit, e.Values, e.PrimaryKey, e.PrevRow, Map.Database.ConnectionString, currentUserId, currentUserRole, e.Command);
            }
        }

        protected virtual void HandleEncryptedHiddenFields(EditEventArgs e)
        {
            List<string> valuesToRemove = new List<string>();
            foreach (string name in e.Values.Keys)
            {
                if (e.View.Fields.ContainsKey(name) && e.View.Fields[name].FieldType == FieldType.Column)
                {
                    Durados.Web.Mvc.ColumnField columnField = (Durados.Web.Mvc.ColumnField)e.View.Fields[name];
                    if (columnField.Encrypted && columnField.SpecialColumn == SpecialColumn.Password)
                    {
                        if (e.Values[name].ToString().Equals(Map.Database.EncryptedPlaceHolder))
                        {
                            valuesToRemove.Add(name);
                        }
                    }
                }
            }

            foreach (string name in valuesToRemove)
                e.Values.Remove(name);
        }

        private void LoadModificationSignature(Durados.View view, Dictionary<string, object> values)
        {
            Field modifiedDate = view.ModifiedDate;
            Field modifiedBy = view.ModifiedBy;

            LoadSignature(view, values, modifiedDate, modifiedBy);


        }

        private void LoadSignature(Durados.View view, Dictionary<string, object> values, Field dateField, Field userField)
        {
            if (dateField != null)
            {
                dateField.ExcludeInInsert = false;
                dateField.ExcludeInUpdate = false;
                if (!values.ContainsKey(dateField.Name))
                {
                    values.Add(dateField.Name, DateTime.Now);
                }
                else
                {
                    values[dateField.Name] = DateTime.Now;
                }
            }

            if (userField != null)
            {
                userField.ExcludeInInsert = false;
                userField.ExcludeInUpdate = false;
                if (!values.ContainsKey(userField.Name))
                {
                    values.Add(userField.Name, Map.Database.GetUserID());
                }
                else
                {
                    values[userField.Name] = Map.Database.GetUserID();
                }
            }
        }

        protected virtual void view_AfterEditBeforeCommit(object sender, EditEventArgs e)
        {
            try
            {
                AfterEditBeforeCommit(e);

                //if (Maps.MultiTenancy)
                //{
                //    HandleMultiTenancyUser(e);
                //}
            }
            catch (Exception exception)
            {
                if (e.Command != null && e.Command.Transaction != null)
                    e.Command.Transaction.Rollback();
                if (!Database.IdenticalSystemConnection && e.SysCommand != null && e.SysCommand.Transaction != null)
                    e.SysCommand.Transaction.Rollback();
                // Map.Logger.Log(GetControllerNameForLog(this.ControllerContext), this.ControllerContext.RouteData.Values["action"].ToString(), exception.Source, exception, 1, null);
                throw exception;
            }
        }

        protected virtual void AfterEditBeforeCommit(EditEventArgs e)
        {

            //Workflow.Engine wfe = CreateWorkflowEngine();
            if (wfe == null)
                Init();

            int userId = - 1;
            string userIdStr = ((Durados.Web.Mvc.Database)e.View.Database).GetUserID();
            if (!string.IsNullOrEmpty(userIdStr))
            {
                userId = Convert.ToInt32(userIdStr);
            }
            else if (e.View == e.View.Database.GetUserView())
            {
                userId = Convert.ToInt32(e.PrimaryKey);
            }

            //if (e.View.Name == "durados_App" && e.View.Database.IsMain())
            //{
            //    try
            //    {
            //        Map map = Maps.Instance.GetMap(e.PrevRow["Name"].ToString());
            //        userId = Convert.ToInt32(map.Database.GetUserID());
            //    }
            //    catch { }
            //}

            wfe.PerformActions(this, e.View, TriggerDataAction.AfterEditBeforeCommit, e.Values, e.PrimaryKey, e.PrevRow, Map.Database.ConnectionString, userId, ((Durados.Web.Mvc.Database)e.View.Database).GetUserRole(), e.Command);

            HandleUploadsSpecialPaths(e.View, DataAction.Edit, e.Values, e.PrimaryKey, (DataActionEventArgs)e);

            HandleFieldsFromOtherViews(e);

            //CompleteApprovalProcess(e);


        }

        protected virtual void HandleUploadsSpecialPaths(Durados.View view, DataAction dataAction, Dictionary<string, object> values, string pk, DataActionEventArgs e)
        {
            bool valuesLoaded = true;

            if (dataAction == DataAction.Edit) valuesLoaded = false;

            foreach (Field field in view.Fields.Values.Where(f => f.FieldType == FieldType.Column && (((Durados.Web.Mvc.ColumnField)f).Upload != null && !string.IsNullOrEmpty(((Durados.Web.Mvc.ColumnField)f).Upload.TemplatePath) || ((Durados.Web.Mvc.ColumnField)f).FtpUpload != null && !string.IsNullOrEmpty(((Durados.Web.Mvc.ColumnField)f).FtpUpload.TemplatePath)) && !f.IsExcluded(dataAction) && f.IsDerivationEditable(values)))
            {

                if (!valuesLoaded)
                {
                    //DuradosController.LoadValues(view, values, e);
                    valuesLoaded = true;
                }

                if (!values.ContainsKey(field.Name) || values[field.Name] == null) continue;

                string fileName = values[field.Name].ToString().Trim();

                if (fileName == string.Empty) continue;

                if (dataAction == DataAction.Edit)
                {
                    if (fileName == field.ConvertToString(((EditEventArgs)e).PrevRow)) continue;
                }

                Durados.Web.Mvc.ColumnField cField = (Durados.Web.Mvc.ColumnField)field;

                IUpload upload = UploadFactory.GetUpload(cField);

                string oldPath;

                if (upload != null) oldPath = upload.GetBaseUploadPath(fileName);//cField.GetUploadPath();

                else throw new DuradosException("Upload is not recognized.");
                // else uploadPath = cField.FtpUpload.GetFtpBasePath(fileName);

                //string oldPath;
                //string oldPath = uploadBasePath + fileName;
                //string oldPath = upload.GetBaseUploadPath(fileName);

                if (!upload.IsFileExists(oldPath)) continue;


                string template = upload.TemplatePath;

                string newPath = string.Empty;

                string newDirPath = string.Empty;// DuradosController.CreateTemplatePath(values, pk, fileName, ref template, ref newPath);

                upload.CreateNewDirectory(newDirPath);

                string newPhisicalPath = newPath.Replace("/", "\\");

                upload.DeleteOldFile(newPhisicalPath);

                upload.CreateNewDirectory2(newPhisicalPath);

                MoveUploadedFile(upload, oldPath, newPhisicalPath);

                Dictionary<string, object> newFileNameValues = new Dictionary<string, object>();

                newFileNameValues.Add(cField.Name, newPath);

                SqlAccess sa = new SqlAccess();

                sa.Edit(view, newFileNameValues, pk, null, null, null, null, (System.Data.SqlClient.SqlCommand)e.Command, (System.Data.SqlClient.SqlCommand)e.SysCommand, null, null);
            }
        }

        protected virtual void MoveUploadedFile(IUpload upload, string oldPath, string newPhisicalPath)
        {
            upload.MoveUploadedFile(oldPath, newPhisicalPath);
        }

        protected virtual void HandleFieldsFromOtherViews(EditEventArgs e)
        {
            SqlAccess sa = new SqlAccess();

            int userId = Convert.ToInt32(GetUserID());

            foreach (Field field in e.View.Fields.Values.Where(f => f.IsFromOtherView()))
            {
                string pk = null;
                object v = null;

                if (e.Values.ContainsKey(field.OriginalParentRelatedFieldName))
                {
                    v = e.Values[field.OriginalParentRelatedFieldName];
                }
                else if (e.View.Fields.ContainsKey(field.OriginalParentRelatedFieldName))
                {
                    string fk = ((Durados.Web.Mvc.ParentField)e.View.Fields[field.OriginalParentRelatedFieldName]).DatabaseNames;

                    if (e.PrevRow != null && e.PrevRow.Table.Columns.Contains(fk))
                        v = e.PrevRow[fk];
                }
                else
                {
                    throw new DuradosException("Configuraion error: Parent Related Fieldname not exist in view");
                }

                if (v != null)
                {
                    pk = v.ToString();
                }

                if (string.IsNullOrEmpty(pk))
                {
                    throw new DuradosException("Foreign key to original view is invalid");
                }

                Dictionary<string, object> values = new Dictionary<string, object>();

                values.Add(field.OriginalFieldName, e.Values[field.Name]);

                Durados.View originalView = ((Durados.Web.Mvc.ParentField)e.View.Fields[field.OriginalParentRelatedFieldName]).ParentView;

                sa.Edit(originalView, values, pk, view_BeforeEdit, view_BeforeEditInDatabase, view_AfterEditBeforeCommit, view_AfterEditAfterCommit, (System.Data.SqlClient.SqlCommand)e.Command, (System.Data.SqlClient.SqlCommand)e.SysCommand, e.History, userId);

            }

        }

        public virtual void LoadValues(Dictionary<string, object> values, DataRow dataRow, Durados.Web.Mvc.View view, Durados.Web.Mvc.ParentField parentField, Durados.Web.Mvc.View rootView, string dynastyPath, string prefix, string postfix, Dictionary<string, Durados.Workflow.DictionaryField> dicFields, string internalDynastyPath)
        {
            if (view.Equals(rootView))
            {
                dynastyPath = GetViewDisplayName(view) + ".";
                internalDynastyPath = view.Name + ".";
            }
            foreach (Field field in view.Fields.Values.Where(f => f.FieldType == FieldType.Column))
            {
                LoadValue(values, dataRow, view, field, dynastyPath, prefix, postfix, dicFields, internalDynastyPath);
            }

            var childrenFields = view.Fields.Values.Where(f => f.FieldType == FieldType.Children && ((Durados.Web.Mvc.ChildrenField)f).LoadForBlockTemplate);
            foreach (Durados.Web.Mvc.ChildrenField field in childrenFields)
            {
                string name = prefix + dynastyPath + field.DisplayName + postfix;
                string internalName = prefix + internalDynastyPath + field.Name + postfix;
                DataView value = GetDataView(field, dataRow);
                if (!values.ContainsKey(name))
                {
                    values.Add(name, value);
                    dicFields.Add(internalDynastyPath, new Durados.Workflow.DictionaryField { DisplayName = field.DisplayName, Type = field.DataType, Value = value });
                }

                foreach (Durados.Web.Mvc.ColumnField columnField in field.ChildrenView.Fields.Values.Where(f => f.FieldType == FieldType.Column))
                {
                    if (columnField.Upload != null)
                    {
                        value.Table.Columns[columnField.Name].ExtendedProperties["ImagePath"] = columnField.GetUploadPath();
                    }
                }
            }

            foreach (Durados.Web.Mvc.ParentField field in view.Fields.Values.Where(f => f.FieldType == FieldType.Parent))
            {
                if (view.Equals(rootView))
                {
                    dynastyPath = view.DisplayName + ".";
                    internalDynastyPath = view.Name + ".";
                }
                LoadValue(values, dataRow, view, field, dynastyPath, prefix, postfix, dicFields, internalDynastyPath);



                DataRow parentRow = dataRow.GetParentRow(field.DataRelation.RelationName);
                Durados.Web.Mvc.View parentView = (Durados.Web.Mvc.View)field.ParentView;
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
                        dynastyPath = GetDynastyPath(dynastyPath, parentField, field);
                        internalDynastyPath = GetInternalDynastyPath(internalDynastyPath, parentField, field);
                        LoadValues(values, parentRow, parentView, field, rootView, dynastyPath, prefix, postfix, dicFields, internalDynastyPath);
                    }
                }
            }
        }

        protected void LoadValue(Dictionary<string, object> values, DataRow dataRow, Durados.Web.Mvc.View view, Field field, string dynastyPath, string prefix, string postfix, Dictionary<string, Durados.Workflow.DictionaryField> dicFields, string internalDynastyPath)
        {
            string name = prefix + dynastyPath + field.DisplayName + postfix;
            string InternalName = prefix + internalDynastyPath + field.Name + postfix;
            string value = view.GetDisplayValue(field.Name, dataRow);
            if (!values.ContainsKey(name))
            {
                values.Add(name, value);
                dicFields.Add(InternalName, new Durados.Workflow.DictionaryField { DisplayName = name, Type = field.DataType, Value = value });

            }
            if (field.FieldType == FieldType.Column && ((Durados.Web.Mvc.ColumnField)field).Upload != null)
            {
                if (dataRow.Table.Columns.Contains(field.Name))
                {

                    dataRow.Table.Columns[field.Name].ExtendedProperties["ImagePath"] = ((Durados.Web.Mvc.ColumnField)field).GetUploadPath();
                }
            }
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

        protected virtual void view_AfterEditAfterCommit(object sender, EditEventArgs e)
        {
            AfterEditAfterCommit(e);
        }

        protected virtual void AfterEditAfterCommit(EditEventArgs e)
        {
            //Workflow.Engine wfe = CreateWorkflowEngine();
            if (wfe == null)
                wfe = CreateWorkflowEngine();

            wfe.PerformActions(this, e.View, TriggerDataAction.AfterEdit, e.Values, e.PrimaryKey, e.PrevRow, Map.Database.ConnectionString, Convert.ToInt32(((Durados.Web.Mvc.Database)e.View.Database).GetUserID()), ((Durados.Web.Mvc.Database)e.View.Database).GetUserRole(), e.Command);
           
            wfe.Notifier.Notify((Durados.Web.Mvc.View)e.View, 1, GetUsername(), e.OldNewValues, e.PrimaryKey, e.PrevRow, this, e.Values, GetSiteWithoutQueryString(), GetMainSiteWithoutQueryString());
             
            const string Active = "Active";
            if (e.View.Name == "Durados_Language")
            {
                bool prevActive = !e.PrevRow.IsNull(Active) && Convert.ToBoolean(e.PrevRow[Active]);
                bool currActive = e.Values.ContainsKey(Active) && Convert.ToBoolean(e.Values[Active]);

                if (!prevActive && currActive)
                {
                    string code = e.PrevRow["Code"].ToString();
                    string scriptFile = Maps.GetDeploymentPath("Sql/Localization/" + code + "pack.sql");

                    SqlAccess sqlAcces = new SqlAccess();
                    sqlAcces.RunScriptFile(scriptFile, Map.GetLocalizationDatabase().ConnectionString);
                    Map.Database.Localizer.SetCurrentUserLanguageCode(code);
                }
            }
        }

        #endregion update callbacks

        #region delete callbacks

        protected void view_BeforeDelete(object sender, DeleteEventArgs e)
        {
            try
            {
                BeforeDelete(e);
            }
            catch (Exception exception)
            {
                if (e.Command != null && e.Command.Transaction != null)
                    e.Command.Transaction.Rollback();
                //Map.Logger.Log(GetControllerNameForLog(this.ControllerContext), this.ControllerContext.RouteData.Values["action"].ToString(), exception.Source, exception, 1, null);
                throw exception;
            }
        }

        protected virtual void BeforeDelete(DeleteEventArgs e)
        {
            int currentUserId = Convert.ToInt32(Map.Database.GetUserID());
            string currentUserRole = Map.Database.GetUserRole();

            if (e.View.Equals(e.View.Database.GetUserView()))
            {
                if (e.PrimaryKey.Equals(Map.Database.GetUserID()))
                {
                    throw new DuradosException("You can not delete yourself because you will not be able to sign in again");
                }
                if (Map.GetUserEmail(e.PrimaryKey).Equals(Maps.Instance.DuradosMap.Database.GetCreatorUsername(Convert.ToInt32(Map.Id))))
                {
                    throw new DuradosException("You can not delete the creator of the app.");
                }
            }

            if (e.View.SaveHistory)
            {
                e.History = GetNewHistory();
                e.UserId = currentUserId;
            }
            CreateWorkflowEngine().PerformActions(this, e.View, TriggerDataAction.BeforeDelete, e.Values, e.PrimaryKey, e.PrevRow, Map.Database.ConnectionString, currentUserId, currentUserRole, e.Command);
        }

        protected void view_AfterDeleteBeforeCommit(object sender, DeleteEventArgs e)
        {
            try
            {
                AfterDeleteBeforeCommit(e);

                //if (Maps.MultiTenancy)
                //{
                //    HandleMultiTenancyUser(e);
                //}
            }
            catch (Exception exception)
            {
                if (e.Command != null && e.Command.Transaction != null)
                    e.Command.Transaction.Rollback();
                //Map.Logger.Log(GetControllerNameForLog(this.ControllerContext), this.ControllerContext.RouteData.Values["action"].ToString(), exception.Source, exception, 1, null);
                throw exception;
            }
        }

        protected virtual void AfterDeleteBeforeCommit(DeleteEventArgs e)
        {
            //Workflow.Engine wfe = CreateWorkflowEngine();
            if (Database == null)
                Database = Map.Database;
            string userViewName = ((Durados.Web.Mvc.Database)Database).UserViewName;
            if (e.View.Name == userViewName)
            {
                try
                {
                    string deletedUsername = null;
                    try
                    {
                        deletedUsername = e.PrevRow["Username"].ToString();
                    }
                    catch { }
                    if (!string.IsNullOrEmpty(deletedUsername))
                    {
                        int userId = Map.Database.GetUserID(deletedUsername);
                        string appId = Map.Id;
                        SqlAccess sqlAccess = new SqlAccess();
                        sqlAccess.ExecuteNonQuery(Maps.Instance.DuradosMap.connectionString, string.Format("delete durados_UserApp where UserId = {0} and AppId = {1}", userId, appId));
                    }
                }
                catch { }
            }
            if (wfe == null)
                wfe = CreateWorkflowEngine();

            wfe.PerformActions(this, e.View, TriggerDataAction.AfterDeleteBeforeCommit, e.Values, e.PrimaryKey, e.PrevRow, Map.Database.ConnectionString, Convert.ToInt32(Map.Database.GetUserID()), ((Durados.Web.Mvc.Database)e.View.Database).GetUserRole(), e.Command);

        }

        protected void view_AfterDeleteAfterCommit(object sender, DeleteEventArgs e)
        {
            AfterDeleteAfterCommit(e);
        }

        protected virtual void AfterDeleteAfterCommit(DeleteEventArgs e)
        {

            wfe.PerformActions(this, e.View, TriggerDataAction.AfterDelete, e.Values, e.PrimaryKey, e.PrevRow, Map.Database.ConnectionString, Convert.ToInt32(Map.Database.GetUserID()), ((Durados.Web.Mvc.Database)e.View.Database).GetUserRole(), e.Command);
            //wfe.Notifier.Notify((View)e.View, 3, GetUsername(), null, e.PrimaryKey, e.PrevRow, this, null, GetSiteWithoutQueryString(), GetMainSiteWithoutQueryString());

        }

        #endregion delete callbacks

        #endregion callbacks


        protected Dictionary<string, object> GetItemFromRequest(string key = null)
        {
            string json = null;
            var httpContext = (HttpContextWrapper)Request.Properties["MS_HttpContext"];

            if (key == null)
            {
                json = System.Web.HttpContext.Current.Server.UrlDecode(httpContext.Request.Form.ToString());
            }
            else
            {
                json = System.Web.HttpContext.Current.Server.UrlDecode(httpContext.Request.Form[key]);

            }
            return JsonConverter.Deserialize(json);
        }

        //protected string GetJsonFromRequest(string key = null)
        //{
        //    string json = null;
        //    var httpContext = (HttpContextWrapper)Request.Properties["MS_HttpContext"];

        //    if (key == null)
        //    {
        //        json = System.Web.HttpContext.Current.Server.UrlDecode(httpContext.Request.Form.ToString());
        //    }
        //    else
        //    {
        //        json = System.Web.HttpContext.Current.Server.UrlDecode(httpContext.Request.Form[key]);

        //    }
        //    return json;
        //}

        protected virtual Durados.Web.Mvc.View GetView(string viewName)
        {
            Durados.Web.Mvc.View view = (Durados.Web.Mvc.View)Map.Database.GetViewByJsonName(viewName);
            if (view != null)
                return view;

            return ViewHelper.GetViewForRest(viewName);
        }

        internal protected virtual string GetViewNameForLog(System.Web.Http.Controllers.HttpControllerContext controllerContext)
        {
            string viewName = string.Empty;
            if (controllerContext.RouteData.Values.ContainsKey("viewName"))
            {
                viewName = controllerContext.RouteData.Values["viewName"].ToString();
            }
            return viewName;
        }
        internal protected virtual string GetControllerNameForLog(System.Web.Http.Controllers.HttpControllerContext controllerContext)
        {
            string viewName = GetViewNameForLog(controllerContext);

            string controller = string.Empty;
            if (controllerContext.RouteData.Values.ContainsKey("controller"))
                controller = controllerContext.RouteData.Values["controller"].ToString();
            if (!string.IsNullOrEmpty(viewName))
                controller += " - " + viewName;

            return controller;
        }

        protected virtual Dictionary<string, object> GetAdjustedValues(Durados.Web.Mvc.View view, Dictionary<string, object> values)
        {
            Dictionary<string, object> adjustedValues = new Dictionary<string, object>();

            foreach (string key in values.Keys)
            {
                string adjustedKey = GetAdjustedKey(view, key);
                adjustedValues.Add(adjustedKey, values[key]);
            }

            return adjustedValues;
        }

        protected virtual string GetAdjustedKey(Durados.Web.Mvc.View view, string key)
        {
            if (key == "viewTable")
                return "Rules_Parent";

            if (view.Fields.ContainsKey(key))
                return key;
            string upper = char.ToUpper(key[0]) + key.Substring(1);
            if (view.Fields.ContainsKey(upper))
                return upper;

            Durados.Field[] fields = view.GetFieldsByDisplayName(upper);
            if (fields != null && fields.Length == 1)
                return fields[0].Name;

            return key;
        }
        protected virtual void SetRequestItemCurrentAppName( string appName)
        {
            try
                {
                    if (System.Web.HttpContext.Current.Items.Contains(Durados.Database.CurAppName))
                        System.Web.HttpContext.Current.Items[Durados.Database.CurAppName] = appName;
                    else 
                        System.Web.HttpContext.Current.Items.Add(Durados.Database.CurAppName, appName);
                }
                catch { }
        }
    }


    public class BackAndApiResponseException : HttpResponseException
    {
        public BackAndApiResponseException(HttpResponseMessage httpResponseMessage)
            : base(httpResponseMessage)
        {

        }
    }

    public class BackAndApiUnexpectedResponseException : BackAndApiResponseException
    {
        public BackAndApiUnexpectedResponseException(Exception exception, apiController apiController)
            : base(new HttpResponseMessage()
            {
                StatusCode = exception is DuradosException ? HttpStatusCode.ExpectationFailed : HttpStatusCode.InternalServerError,
                Content = new StringContent(exception is DuradosException ? (exception.InnerException != null && exception.InnerException is Durados.Workflow.WorkflowEngineException ? exception.InnerException.Message : exception.Message) : string.Format(Messages.Unexpected, exception.Message)),
                ReasonPhrase = Messages.Critical
            })
        {
            Log(apiController, exception);
        }

        protected virtual void Log(apiController apiController, Exception exception)
        {
            try
            {
                Map map = apiController.Map;

                string exceptionSource = exception == null ? null : exception.Source;

                int logType = exception == null ? 3 : 1;

                string verb = apiController.Request.Method.Method;

                map.Logger.Log(apiController.GetControllerNameForLog(apiController.ControllerContext), verb, exceptionSource, exception, logType, null);

                Durados.Web.Mvc.Logging.Logger logger = new Durados.Web.Mvc.Logging.Logger();

                SendError(logType, exception, apiController.GetControllerNameForLog(apiController.ControllerContext), apiController.GetViewNameForLog(apiController.ControllerContext), logger, apiController.Map, apiController);

            }
            catch
            {

            }
        }

        protected virtual void SendError(int logType, Exception exception, string controller, string action, Durados.Web.Mvc.Logging.Logger logger, Map map, apiController apiController)
        {
            SendError(logType, exception, controller, action, logger, string.Empty, map, apiController);
        }

        protected virtual void SendError(int logType, Exception exception, string controller, string action, Durados.Web.Mvc.Logging.Logger logger, string moreInfo, Map map, apiController apiController)
        {
            try
            {
                bool sendError = Convert.ToBoolean(System.Configuration.ConfigurationManager.AppSettings["sendError"]) && logType == 1;
                if (sendError)
                {
                    string host = Convert.ToString(System.Configuration.ConfigurationManager.AppSettings["host"]);
                    int port = Convert.ToInt32(System.Configuration.ConfigurationManager.AppSettings["port"]);
                    string username = Convert.ToString(System.Configuration.ConfigurationManager.AppSettings["username"]);
                    string password = Convert.ToString(System.Configuration.ConfigurationManager.AppSettings["password"]);
                    string applicationName = string.Empty;
                    try
                    {
                        applicationName = System.Web.HttpContext.Current.Items[Durados.Database.AppName].ToString();
                    }
                    catch { }
                    string from = Convert.ToString(System.Configuration.ConfigurationManager.AppSettings["fromError"]);
                    string defaultTo = Convert.ToString(System.Configuration.ConfigurationManager.AppSettings["toError"]);
                    string[] to = !string.IsNullOrEmpty(map.Database.AdminEmail) ? map.Database.AdminEmail.Split(';') : null;
                    string[] cc = new string[1] { defaultTo };
                    if (to == null || to.Length == 0)
                    {
                        to = cc;
                        cc = null;
                    }



                    string message = "The following error occurred:\n\r" + exception.ToString();
                    if (!string.IsNullOrEmpty(moreInfo))
                    {
                        message += "\n\r\n\r\n\rMore info:\n\r" + moreInfo;
                    }

                    Durados.Cms.DataAccess.Email.Send(host, false, port, username, password, false, to, cc, null, applicationName + " error", "url: " + apiController.Request.RequestUri.ToString() + ", error: " + message, from, null, null, DontSend, logger);
                }
            }
            catch (Exception ex)
            {
                logger.Log(controller, action, exception.Source, ex, 1, "Error sending email when logging an exception");
            }
        }

        public bool DontSend
        {
            get
            {
                return Convert.ToBoolean(System.Configuration.ConfigurationManager.AppSettings["DontSend"] ?? "false");
            }
        }
    }

    public class Messages
    {
        public static readonly string FieldShouldBeParent = "The field \"{0}\" nust be a single-select or a multi-select type.";
        public static readonly string ForeignKeyDeleteViolation = "The row you are trying to delete is referenced in another table ,please check your table constraints";
        public static readonly string FieldShouldBeAutoComplete = "The field \"{0}\" nust be an autocomplete display format.";
        public static readonly string ViewNameIsMissing = "The Table name is missing.";
        public static readonly string FieldNameIsMissing = "The field name is missing.";
        public static readonly string IdIsMissing = "The id is missing.";
        public static readonly string CollectionIsMissing = "The collection is missing.";
        public static readonly string CollectionNotFound = "The collection was not found.";
        public static readonly string DuplicateCollectionName = "The collection name exists more than once.";
        public static readonly string ViewIsUnauthorized = "The Table is unauthorized for this current user role.";
        public static readonly string ViewNameNotFound = "The Table \"{0}\" was not found.";
        public static readonly string MissingObjectToUpdate = "The object to update is missing.";
        public static readonly string FieldNameNotFound = "The field \"{0}\" was not found.";
        public static readonly string TheFieldMustBeTextual = "The field must be textual.";
        public static readonly string ItemWithIdNotFound = "An item with id \"{0}\" was not found in Table \"{1}\".";
        public static readonly string AppNotFound = "The app \"{0}\" was not found.";
        public static readonly string ChartWithIdNotFound = "An chart with id \"{0}\" was not found.";
        public static readonly string Unexpected = "An error occurred, please try again or contact the administrator. Error details: {0}";
        public static readonly string Critical = "Critical Exception";
        public static readonly string ActionIsUnauthorized = "The action is unauthorized for this current user role.";
        public static readonly string WorkspaceNotFound = "The workspace was not found";
        public static readonly string WorkspaceNameMissing = "The workspace name is missing";
        public static readonly string WorkspaceWithNameAlreadyExists = "A Workspace with the name {0} already exists.";
        public static readonly string QueryWithNameAlreadyExists = "A Query with the name {0} already exists.";
        public static readonly string UploadWithNameAlreadyExists = "An Upload with the name {0} already exists.";
        public static readonly string ChangeAdminWorkspaceNameNotAllowed = "Changing Admin worksapces name is not allowed.";
        public static readonly string WorkspaceLimit = "You have reached workspaces limit";
        public static readonly string UploadNotFound = "The column has no upload configuration";
        public static readonly string InvalidFileType = "Invalid file type";
        public static readonly string InvalidFileType2 = "Invalid file type in field [{0}].<br><br>Allowed formats: {1}";
        public static readonly string AppNameAlreadyExists = "An application by this name already exists.";
        public static readonly string AppNameCannotBeNull = "App name cannot be empty.";
        public static readonly string AppNameInvalid = "App name must be alphanumeric.";
        public static readonly string RuleWithNameAlreadyExists = "A rule with the name {0} already exists for table {1}.";
        public static readonly string RuleNotFound = "The rule was not found.";
        public static readonly string NotImplemented = "The action is not implemented yet.";
        public static readonly string FailedToGetJsonFromParameters = "Failed to get json from parameters.";
        public static readonly string StringifyFilter = "Please JSON.stringify the filter parameter";
        public static readonly string GetFilterError = "Failed to translate filter";
        public static readonly string StringifySort = "Please JSON.stringify the sort parameter";
        public static readonly string InvalidSchema = "Invalid schema";
        
    }
}