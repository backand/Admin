using System;
using System.Data;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Durados.Web.Mvc.UI.Json;
using System.Web;
using System.Web.Mvc;

using Durados.DataAccess;
using System.Data.SqlClient;
using System.IO;
using Durados.Web.Mvc.Controllers;
using System.Configuration;

namespace Durados.Web.Mvc.UI.Helpers
{
    public static class PlugInHelper
    {
        public static readonly int FreePlan = 3;

        public static void UpdateSiteInfo(this Database database, string siteId, string siteInfo, string url)
        {
            try
            {
                string sql = "update durados_PlugInSite set Url = @Url, Info = @Info where siteId = @SiteId ";


                if (url.Length > 500)
                {
                    url = url.Substring(0, 500);
                }

                if (siteInfo.Length > 4000)
                {
                    siteInfo = siteInfo.Substring(0, 4000);
                }

                Dictionary<string, object> parameters = new Dictionary<string, object>();
                parameters.Add("@Url", url);
                parameters.Add("@Info", string.Empty);
                parameters.Add("@SiteId", siteId);

                SqlAccess.ExecuteNonQuery(database.ConnectionString, sql, parameters, null);
            }
            catch (Exception exception)
            {
                database.Logger.Log("PlugIn", "UpdateSiteInfo", "update durados_PlugInSite", exception, 1, "Failed to update site info. siteId: " + siteId + ", url: " + url);
    
            }
        }

        public static void UpdatePlan(this Database database, int appId, int planId, string siteId)
        {
            bool newPlan = IsNewPlan(database, planId, siteId);

            int id = UpdateSiteApp(database, appId, planId, siteId);

            if (newPlan)
            {
                int[] appIds = GetSiteApps(database, id);

                foreach (int appId2 in appIds)
                {
                    UpdatePlan(database, appId2, planId, true);
                }
            }
            else
            {
                UpdatePlan(database, appId, planId, true);
            }
        }

        private static int[] GetSiteApps(Database database, int id)
        {
            string sql = string.Format("select appId from durados_PlugInSiteApp where PlugInSiteId = {0}", id);
            DataTable table = SqlAccess.ExecuteTable(database.ConnectionString, sql, null, CommandType.Text);
            List<int> ids = new List<int>();

            foreach (DataRow row in table.Rows)
            {
                ids.Add((int)row[0]);
            }

            return ids.ToArray();
        }

        private static int UpdateSiteApp(Database database, int appId, int planId, string siteId)
        {
            int id = UpdateSite(database, appId, planId, siteId);

            string sql = string.Format("if not exists (select top(1) appId from durados_PlugInSiteApp WITH (NOLOCK) where appId='{0}') insert into durados_PlugInSiteApp (appId, PlugInSiteId) values ({0}, {1}) ", appId, id);

            SqlAccess.ExecuteNonQuery(database.ConnectionString, sql);

            return id;
        }

        private static int UpdateSite(Database database, int appId, int planId, string siteId)
        {
            string sql = string.Format("if not exists (select top(1) siteId from durados_PlugInSite WITH (NOLOCK) where siteId='{0}') insert into durados_PlugInSite (siteId, planId) values ('{0}', {1}) else update durados_PlugInSite set planId = {1} where siteId = '{0}' ", siteId, planId);
            
            SqlAccess.ExecuteNonQuery(database.ConnectionString, sql);

            sql = string.Format("select id from durados_PlugInSite where siteId = '{0}'", siteId);

            object scalar = SqlAccess.ExecuteScalar(database.ConnectionString, sql);

            return Convert.ToInt32(scalar);
        }

        private static bool IsNewPlan(Database database, int planId, string siteId)
        {
            string sql = string.Format("select id from durados_PlugInSite WITH (NOLOCK) where SiteId = '{0}' and PlanId = {1}", siteId, planId);

            object scalar = SqlAccess.ExecuteScalar(database.ConnectionString, sql);

            return (scalar == null || scalar == DBNull.Value);
            
        }

        public static void UpdatePlan(this Database database, int appId, int planId, bool updateCache)
        {
            string sql = "if not exists (select * from (select top(1) PlanId from durados_AppPlan WITH (NOLOCK) where Appid=" + appId + " order by PurchaseDate desc) as a where PlanId=" + planId + ") insert into durados_AppPlan (AppId, PlanId) values(" + appId + "," + planId + ")";

            SqlAccess.ExecuteNonQuery(database.ConnectionString, sql);

            if (updateCache)
            {
                string appName = Maps.Instance.GetAppRow(appId).Name;

                Map map = Maps.Instance.GetMap(appName);

                map.Plan = planId;
            }
        }

        public static Option[] GetViewNameDisplayList(this Database database)
        {
            List<Option> views = new List<Option>();

            foreach (Durados.View view in database.Views.Values.Where(v => !v.SystemView).OrderBy(v => v.DisplayName))
            {
                views.Add(new Option(){ Value = view.Name, Text = view.DisplayName });
            }

            return views.ToArray();
        }

        public static Option[] GetStyleNameDisplayList(this Database database)
        {
            List<Option> styles = new List<Option>();

            foreach (string style in Enum.GetNames(typeof(Style)))
            {
                styles.Add(new Option (){ Value = style, Text = style.GetDecamal() });
            }

            return styles.ToArray();
        }

        //public static void SaveSelectedInstance(this Database database, string instanceId, int? sampleAppId, PlugInType plugInType, string appName, string viewName, string userId)
        //{
        //    int? appId = Maps.Instance.AppExists(appName);

        //    if (!appId.HasValue)
        //    {
        //        throw new DuradosException("The app " + appName + " could not be found. Make sure the app was not deleted or changed.");
        //    }

        //    SaveSelectedInstance(database, instanceId, sampleAppId, plugInType, appId.Value, viewName, userId);
        //}

        public static DataRow GetMyInstanceRow(this Database database, string instanceId, PlugInType plugInType, HttpRequestBase request)
        {
            if (IsRegistered(database, GetPlugInUserId(plugInType, request)))
            {
                return GetInstanceRow(database, instanceId, null);
            }
            else
            {
                return null;
            }
        }

        public static DataRow GetInstanceRow(this Database database, string instanceId, int? sampleAppId)
        {
            const string InstanceTableName = "durados_PlugInInstance";
            const string InstanceIdColumnName = "InstanceId";
            const string SampleAppIdColumnName = "SampleAppId";

            if (!(database.Map is DuradosMap))
                throw new DuradosException("Must be DuradosMap to get samples");

            if (!(database.Views.ContainsKey(InstanceTableName)))
                throw new DuradosException("DuradosMap does not contains samples for plug-in, check web.config for privateCloud = true");

            View instanceView = (View)database.Views[InstanceTableName];
            int rowCount = 0;
            Dictionary<string, object> filterValues = new Dictionary<string, object>();
            filterValues.Add(InstanceIdColumnName, instanceId);
            DataView dataView = instanceView.FillPage(1, 10000, filterValues, null, null, out rowCount, null, null);

            if (dataView.Count == 0)
                return null;

            foreach (System.Data.DataRowView row in dataView)
            {
                if (!sampleAppId.HasValue && row.Row.IsNull(SampleAppIdColumnName))
                    return row.Row;
                else if (sampleAppId.HasValue && row.Row[SampleAppIdColumnName].Equals(sampleAppId.Value))
                    return row.Row;
            }

            return null;
        }

        public static DataRow GetSelectedInstanceRow(this Database database, string instanceId, PlugInType plugInType)
        {
            const string InstanceTableName = "durados_PlugInInstance";
            const string InstanceIdColumnName = "InstanceId";
            const string SelectedColumnName = "Selected";
            
            if (!(database.Map is DuradosMap))
                throw new DuradosException("Must be DuradosMap to get samples");

            if (!(database.Views.ContainsKey(InstanceTableName)))
                throw new DuradosException("DuradosMap does not contains samples for plug-in, check web.config for privateCloud = true");

            View instanceView = (View)database.Views[InstanceTableName];
            int rowCount = 0;
            Dictionary<string, object> filterValues = new Dictionary<string, object>();
            filterValues.Add(InstanceIdColumnName, instanceId);
            filterValues.Add(SelectedColumnName, true);
            DataView dataView = instanceView.FillPage(1, 10000, filterValues, null, null, out rowCount, null, null);

            if (dataView.Count == 0)
                return null;

            return dataView[0].Row;
        }

        

        public static string CreatePlugInUser(this Database database, string id, PlugInType plugInType, Guid plugInUserGuid)
        {
            return plugInUserGuid.ToString();
        }

        //public static DataRow CreateMyInstanceRow(this Database database, string id, PlugInType plugInType, int appId, string viewName, string userId, string plugInUserId)
        //{
        //    const string InstanceTableName = "durados_PlugInInstance";

        //    if (!(database.Map is DuradosMap))
        //        throw new DuradosException("Must be DuradosMap to get samples");

        //    if (!(database.Views.ContainsKey(InstanceTableName)))
        //        throw new DuradosException("DuradosMap does not contains samples for plug-in, check web.config for privateCloud = true");

        //    View instanceView = (View)database.Views[InstanceTableName];

        //    SaveSelectedInstance(database, id, null, plugInType, appId, viewName, userId, plugInUserId);

        //    return GetSelectedInstanceRow(database, id, plugInType);
        //}

        public static string GetCreatorGuid2(this Database database, int appId)
        {
            string username = GetCreatorUsername(database, appId);

            DataRow row = database.GetUserRow(username);

            if (row == null)
                return null;

            return row["Guid"].ToString().ToUpper();
        }

        public static string GetCreatorGuid(this Database database, int appId)
        {
            string username = GetCreatorUsername(database, appId);

            string appName = Maps.Instance.GetAppRow(appId).Name;
            
            Map map = Maps.Instance.GetMap(appName);

            DataRow row = map.Database.GetUserRow(username);

            if (row == null)
                return null;

            return row["Guid"].ToString().ToUpper();
        }

        public static int? GetCreator(this Database database, int appId)
        {
            string sql = "SELECT Creator FROM dbo.durados_App WITH (NOLOCK) WHERE dbo.durados_App.Id = " + appId;

            string scalar = SqlAccess.ExecuteScalar(database.ConnectionString, sql);

            if (string.IsNullOrEmpty(scalar))
                return null;

            return Convert.ToInt32(scalar);
        }

        public static string GetCreatorUsername(this Database database, int appId)
        {
            string sql = "SELECT dbo.durados_User.[Username] FROM dbo.durados_App WITH (NOLOCK) INNER JOIN dbo.durados_User WITH (NOLOCK) ON dbo.durados_App.Creator = dbo.durados_User.ID WHERE dbo.durados_App.Id = " + appId;

            return SqlAccess.ExecuteScalar(database.ConnectionString, sql);
        }

        public static void HandleDatabaseInventory(this Database database, int templateAppId, int sampleAppId, PlugInType plugInType)
        {
            int count = 0;
            bool addToInventory = AddToInventory(database, templateAppId, plugInType, out count);

            if (!addToInventory)
            {
                return;
            }

            int dbCount = 0;
            string newDatabaseName = GetNewDatabaseName(database, templateAppId, plugInType, out dbCount);

            UpdateDatabaseName(database, templateAppId, plugInType, newDatabaseName, dbCount);
            CreateDatabase(database, newDatabaseName);
            string guid = Maps.Instance.DuradosMap.Database.GetUserGuid();

            string url = Maps.GetMainAppUrl() + "/" + plugInType.ToString() + "PlugIn/GenerateDB?sampleAppId=" + sampleAppId + "&count=" + Maps.Instance.PluginsCache[plugInType].Batch + "&" + System.Web.HttpContext.Current.Request.QueryString + "&id=" + guid;
            try
            {
                Infrastructure.ISendAsyncErrorHandler SendAsyncErrorHandler = null;
                Infrastructure.Http.AsynWebRequest(url, SendAsyncErrorHandler);
            }
            catch (Exception exception)
            {
                DuradosException duradosException = new DuradosException("HandleDatabaseInventory Failed, Failed to run async call url= " + url, exception);
                Maps.Instance.DuradosMap.Logger.Log("AppsGenerator", "HandleDatabaseInventory", DateTime.Now.Millisecond.ToString(), duradosException, 1, url);
                //throw duradosException;
                
            }
        }

        private static SqlAccess _sqlAccess = null;

        private static SqlAccess SqlAccess
        {
            get
            {
                if (_sqlAccess == null)
                    _sqlAccess = new SqlAccess();

                return _sqlAccess;
            }
        }

        private static string GetNewDatabaseName(Database database, int templateAppId, PlugInType plugInType, out int dbCount)
        {
            string sql = "select DatabaseName, DbCount from durados_SampleApp WITH (NOLOCK) where PlugInId = " + ((int)plugInType) + " and AppId = " + templateAppId;

            DataTable table = SqlAccess.ExecuteTable(database.ConnectionString, sql, null, CommandType.Text);

            if (table.Rows.Count != 1)
            {
                throw new DuradosException("Could not find the sample app " + templateAppId);
            }

            DataRow row = table.Rows[0];

            string currentName = row["DatabaseName"].ToString();

            dbCount = (int)row["DbCount"] + 1;

            return currentName.Split('_')[0] + "_" + dbCount.ToString().PadLeft(5, '0');
        }

        private static void CreateDatabase(Database database, string newDatabaseName)
        {
            SqlAccess.CreateDatabase(database.ConnectionString, newDatabaseName);
        }

        private static void UpdateDatabaseName(Database database, int templateAppId, PlugInType plugInType, string newDatabaseName, int dbCount)
        {
            string sql = "update durados_SampleApp set DatabaseName = '" + newDatabaseName + "', DbCount = " + dbCount + " where PlugInId = " + ((int)plugInType) + " and AppId = " + templateAppId;

            SqlAccess.ExecuteNonQuery(database.ConnectionString, sql);

        }

        public static bool AddToInventory(Database database, int templateAppId, PlugInType plugInType, out int count)
        {
            //if (!Maps.Instance.PluginsCache[plugInType].IsPassMaxCount(templateAppId))
            //    return false;

            string sql = "select count(*) from durados_App WITH (NOLOCK) where Creator is null and TemplateId = " + templateAppId;

            string scalar = SqlAccess.ExecuteScalar(database.ConnectionString, sql);

            count = 0;
            if (!string.IsNullOrEmpty(scalar))
                count = Convert.ToInt32(scalar);


            return count <= Maps.Instance.PluginsCache[plugInType].Remains;
        }

        public static DataRow CreateSampleInstanceRow(this Database database, string id, PlugInType plugInType, int? sampleAppId, string plugInUserId)
        {
            const string AppIdColumnName = "AppId";
            const string ViewNameColumnName = "ViewName";
            const string InstanceTableName = "durados_PlugInInstance";

            if (!(database.Map is DuradosMap))
                throw new DuradosException("Must be DuradosMap to get samples");

            if (!(database.Views.ContainsKey(InstanceTableName)))
                throw new DuradosException("DuradosMap does not contains samples for plug-in, check web.config for privateCloud = true");

            View instanceView = (View)database.Views[InstanceTableName];

            if (!sampleAppId.HasValue)
                return null;

            DataRow sampleRow = GetSampleRow(database, sampleAppId.Value);
            if (sampleRow == null)
                return null;

            int templateAppId = (int)sampleRow[AppIdColumnName];
            HandleDatabaseInventory(database, templateAppId, sampleAppId.Value, plugInType);
            
            int newUser = GetOrCreateNewUser(database, plugInUserId, plugInType);

            int? newAppId = GetAvailableApp(database, templateAppId, newUser);
            
            if (!newAppId.HasValue)
                return null;

            AddUserToApp(newAppId.Value, newUser, GetSampleCreatorRole());

           
            string viewName = (string)sampleRow[ViewNameColumnName] + newAppId;

            /** start temporary code **/
            try
            {
                AppsGenerator appsGenerator = new AppsGenerator();
            }
            catch
            {
                viewName = (string)sampleRow[ViewNameColumnName];
            }
            /** end temporary code **/

            SaveSelectedInstance(database, id, sampleAppId, plugInType, newAppId.Value, viewName, newUser, plugInUserId);

            return GetSelectedInstanceRow(database, id, plugInType);
        }

        public static int GetOrCreateNewUser(Database database, string plugInUserId, PlugInType plugInType)
        {
            int? userId = GetNotRegisterUser(database, plugInUserId);

            if (userId.HasValue)
                return userId.Value;

            return CreateNewUser(plugInUserId, plugInType, "wix");
        }

        public static int? GetNotRegisterUser(this Database database, string plugInUserId)
        {
            string scalar = SqlAccess.ExecuteScalar(database.ConnectionString, "select NotRegisteredUserId from durados_PlugInNotRegisteredUser WITH (NOLOCK) where PlugInUserId = '" + plugInUserId + "'");

            if (string.IsNullOrEmpty(scalar))
                return null;

            return Convert.ToInt32(scalar);
        }

        private static string GetSampleCreatorRole()
        {
            return "View Owner";
        }

        public static void AddUserToApp(int newAppId, int newUser, string role)
        {
            string appName = Maps.Instance.GetAppRow(newAppId).Name;

            Map map = Maps.Instance.GetMap(appName);

            DataRow userRow = Maps.Instance.DuradosMap.Database.GetUserRow(newUser);


            string sql = "if not exists(select * from durados_User WITH (NOLOCK) where username = @username) insert into durados_User (Username, FirstName, LastName, Email, Role, Guid, IsApproved) values(@Username, @FirstName, @LastName, @Email, @Role, @Guid, @IsApproved)";

            Dictionary<string, object> parameters = new Dictionary<string, object>();
            parameters.Add("Username", userRow["Username"]);
            parameters.Add("FirstName", userRow["FirstName"]);
            parameters.Add("LastName", userRow["LastName"]);
            parameters.Add("Email", userRow["Email"]);
            parameters.Add("Role", role);
            parameters.Add("Guid", userRow["Guid"]);
            parameters.Add("IsApproved", true);


            SqlAccess.ExecuteNonQuery(map.Database.SysDbConnectionString, sql, parameters, null);

            
        }

        private static int GetAvailableAppCount(Database database, int templateAppId)
        {
            string scalar = SqlAccess.ExecuteScalar(database.ConnectionString, "select count(*) from durados_App WITH (NOLOCK) where Creator is null and TemplateId = " + templateAppId);

            if (string.IsNullOrEmpty(scalar))
                return 0;

            return Convert.ToInt32(scalar);
        }

        private static int? GetAvailableApp(Database database, int templateAppId, int newUser)
        {
            //SqlAccess sqlAcces = new SqlAccess();

            //Dictionary<string, object> parameters = new Dictionary<string, object>();
            //parameters.Add("@TemplateId", templateAppId);
            //parameters.Add("@UserId", newUser);
            //parameters.Add("@AppId", 0);

            //System.Data.SqlClient.SqlParameter[] returnedParameters = sqlAcces.ExecuteProcedure(database.ConnectionString, "durados_GelAvailableApp", parameters, null);

            //if (returnedParameters[2]
            //return null;

            System.Data.SqlClient.SqlParameter outputParameter = null;

            using (System.Data.SqlClient.SqlConnection connection = new System.Data.SqlClient.SqlConnection(database.ConnectionString))
            {
                connection.Open();
                using (System.Data.SqlClient.SqlCommand command = new System.Data.SqlClient.SqlCommand())
                {
                    command.Connection = connection;
                    command.CommandType = CommandType.StoredProcedure;
                    command.CommandText = "durados_GelAvailableApp";
                    command.Parameters.AddWithValue("@TemplateId", templateAppId);
                    command.Parameters.AddWithValue("@UserId", newUser);
                    outputParameter = command.Parameters.AddWithValue("@AppId", 0);

                    outputParameter.Direction = ParameterDirection.InputOutput;

                    command.ExecuteNonQuery();
                }
            }

            if (!outputParameter.Value.Equals(0))
                return (int) outputParameter.Value;

            return null;

        }

        public static int CreateNewUser(string plugInUserId, PlugInType plugInType, string prefix, bool createPersistentCookie = true)
        {
            Dictionary<string, object> parameters = new Dictionary<string, object>();

            string email = prefix + "@" + Durados.Database.LongProductName + ".com";

            parameters.Add("@Email", email);


            parameters.Add("@Username", email);


            string firstName = prefix;
            parameters.Add("@FirstName", firstName);

            string lastName = prefix;
            parameters.Add("@LastName", lastName);


            string role = GetSampleCreatorRole();
            parameters.Add("@Role", role);
            
            //bool plugInUser = true;
            //parameters.Add("@" + PlugInUserFlag, plugInUser);

            Guid guid = Guid.NewGuid();
            parameters.Add("@Guid", guid);

            object scalar = SqlAccess.ExecuteScalar(Maps.Instance.DuradosMap.Database.ConnectionString, "INSERT INTO [durados_User] ([Username],[FirstName],[LastName],[Email],[Role],[Guid]) VALUES (@Username,@FirstName,@LastName,@Email,@Role,@Guid); SELECT IDENT_CURRENT(N'[durados_User]') AS ID ", parameters);

            

            parameters = new Dictionary<string, object>();
            email = prefix + scalar.ToString() + "@" + Durados.Database.LongProductName + ".com";
            parameters.Add("@Username", email);
            parameters.Add("@Id", scalar);

            SqlAccess.ExecuteNonQuery(Maps.Instance.DuradosMap.Database.ConnectionString, "update [durados_User] set [username] = @Username where id = @Id", parameters, null  );

            System.Web.Security.MembershipCreateStatus createStatus = (new Durados.Web.Mvc.Controllers.AccountMembershipService()).CreateUser(email, "123456", email);
            if (createStatus == System.Web.Security.MembershipCreateStatus.Success)
            {
                if( (int)plugInType == (int)PlugInType.Wix )
                {
                    createPersistentCookie = false;/***Force Logout due to the need to check preview user without Admin credentials*/
                }

                (new Durados.Web.Mvc.Controllers.FormsAuthenticationService()).SignIn(email, createPersistentCookie);
                System.Web.Security.Roles.AddUserToRole(email, role);

            }

            parameters = new Dictionary<string, object>();
            parameters.Add("@PlugInUserId", plugInUserId);
            parameters.Add("@PlugInId", (int)plugInType);
            parameters.Add("@NotRegisteredUserId", Convert.ToInt32(scalar));
            SqlAccess.ExecuteNonQuery(Maps.Instance.DuradosMap.Database.ConnectionString, "insert into durados_PlugInNotRegisteredUser (PlugInUserId, PlugInId, NotRegisteredUserId) values (@PlugInUserId, @PlugInId, @NotRegisteredUserId)", parameters, null);


            return Convert.ToInt32(scalar);
        }

        public static void SignOut()
        {
            (new Durados.Web.Mvc.Controllers.FormsAuthenticationService()).SignOut();
            System.Web.Security.Roles.DeleteCookie();
            System.Web.HttpContext.Current.Session.Clear();


            //if (System.Web.HttpContext.Current.Request.Browser.Browser.ToLower() == "ie")
            //{
            //    System.Web.HttpContext.Current.Session.Abandon();

            //    HttpCookie rFormsCookie = new HttpCookie(System.Web.Security.FormsAuthentication.FormsCookieName, "");
            //    rFormsCookie.Expires = DateTime.Now.AddYears(-1);
            //    System.Web.HttpContext.Current.Response.Cookies.Add(rFormsCookie);

            //    // Clear session cookie 
            //    HttpCookie rSessionCookie = new HttpCookie("ASP.NET_SessionId", "");
            //    rSessionCookie.Expires = DateTime.Now.AddYears(-1);
            //    System.Web.HttpContext.Current.Response.Cookies.Add(rSessionCookie);
            //}
        }

        public static void SignIn(string username, bool createPersistentCookie = true)
        {
            (new Durados.Web.Mvc.Controllers.FormsAuthenticationService()).SignIn(username, createPersistentCookie);
        }

        public static int? GetFirstSampleId(this Database database, PlugInType plugInType)
        {
            DataRow sampleRow = GetFirstSampleRow(database, plugInType);

            if (sampleRow == null)
                return null;

            return (int)sampleRow["Id"];
        }

        public static DataRow GetFirstSampleRow(Database database, PlugInType plugInType)
        {
            const string SampleAppTableName = "durados_SampleApp";
            const string PlugInIdColumnName = "PlugInId";

            List<Option> samples = new List<Option>();

            if (!(database.Map is DuradosMap))
                throw new DuradosException("Must be DuradosMap to get samples");

            if (!(database.Views.ContainsKey(SampleAppTableName)))
                throw new DuradosException("DuradosMap does not contains samples for plug-in, check web.config for privateCloud = true");

            View samplesView = (View)database.Views[SampleAppTableName];

            int rowCount = 0;
            Dictionary<string, object> filterValues = new Dictionary<string, object>();
            filterValues.Add(PlugInIdColumnName, ((int)plugInType).ToString());
            Dictionary<string, SortDirection> sortColumns = new Dictionary<string, SortDirection>();
            sortColumns.Add("Ordinal", SortDirection.Asc);
            DataView dataView = samplesView.FillPage(1, 10000, filterValues, null, sortColumns, out rowCount, null, null);

            if (dataView.Count == 0)
                return null;

            return dataView[0].Row;
        }

        private static DataRow GetSampleRow(Database database, int sampleAppId)
        {
            const string SampleAppTableName = "durados_SampleApp";
            //const string PlugInIdColumnName = "PlugInId";

            List<Option> samples = new List<Option>();

            if (!(database.Map is DuradosMap))
                throw new DuradosException("Must be DuradosMap to get samples");

            if (!(database.Views.ContainsKey(SampleAppTableName)))
                throw new DuradosException("DuradosMap does not contains samples for plug-in, check web.config for privateCloud = true");

            View samplesView = (View)database.Views[SampleAppTableName];

            return samplesView.GetDataRow(sampleAppId.ToString());
            //int rowCount = 0;
            //Dictionary<string, object> filterValues = new Dictionary<string, object>();
            //filterValues.Add(PlugInIdColumnName, ((int)plugInType).ToString());
            //Dictionary<string, SortDirection> sortColumns = new Dictionary<string, SortDirection>();
            //sortColumns.Add("Ordinal", SortDirection.Asc);
            //DataView dataView = samplesView.FillPage(1, 10000, filterValues, null, sortColumns, out rowCount, null, null);

            //if (dataView.Count == 0)
            //    return null;

            //return dataView[0].Row;
        }

        //public static int GetAppId(this Database database, string id, PlugInType plugInType, DataRow instanceRow, string userId)
        //{
        //    int appId = (int)instanceRow["AppId"];

        //    string basedOnSampleAppName = string.Empty;
        //    if (IsSampleApp(database, appId, id, plugInType, out basedOnSampleAppName))
        //    {
        //        int? basedOnSampleAppId = Maps.Instance.AppExists(basedOnSampleAppName);
        //        if (basedOnSampleAppId.HasValue)
        //            appId = basedOnSampleAppId.Value;
        //    }

        //    return appId;
        //}

        //public static bool IsSampleApp(this Database database, int appId, string viewName, string id, PlugInType plugInType, out string basedOnSampleAppName)
        //{
        //    const string SampleAppTableName = "durados_SampleApp";
        //    const string SampleNameColumnName = "Name";
        //    const string SamplePrefixColumnName = "AppNamePrefix";
        //    const string AppIdColumnName = "AppId";
        //    const string ViewNameColumnName = "ViewName";
        //    const string AppTitleColumnName = "Title";
        //    const string AppNameColumnName = "Name";
        //    const string SampleAppAppRelationName = "SampleAppApp";
        //    const string PlugInIdColumnName = "PlugInId";

        //    List<Option> samples = new List<Option>();

        //    if (!(database.Map is DuradosMap))
        //        throw new DuradosException("Must be DuradosMap to get samples");

        //    if (!(database.Views.ContainsKey(SampleAppTableName)))
        //        throw new DuradosException("DuradosMap does not contains samples for plug-in, check web.config for privateCloud = true");

        //    View samplesView = (View)database.Views[SampleAppTableName];
        //    int rowCount = 0;
        //    Dictionary<string, object> filterValues = new Dictionary<string, object>();
        //    filterValues.Add(AppIdColumnName, appId.ToString());
        //    filterValues.Add(ViewNameColumnName, viewName);
        //    filterValues.Add(PlugInIdColumnName, ((int)plugInType).ToString());
        //    Dictionary<string, SortDirection> sortColumns = new Dictionary<string, SortDirection>();
        //    sortColumns.Add("Ordinal", SortDirection.Asc);
        //    DataView dataView = samplesView.FillPage(1, 10000, filterValues, null, sortColumns, out rowCount, null, null);

        //    basedOnSampleAppName = string.Empty;
        //    if (dataView.Count != 1)
        //        return false;

        //    System.Data.DataRowView row = dataView[0];

        //    string samplePrefix = row[SamplePrefixColumnName].ToString();

        //    basedOnSampleAppName = samplePrefix + id.Replace("-", "");

        //    return true;
        //}

        public static int? GetRegisteredUserId(this Database database, string plugInUserId)
        {
            string scalar = SqlAccess.ExecuteScalar(database.ConnectionString, "select RegisteredUserId from durados_PlugInRegisteredUser WITH (NOLOCK) where PlugInUserId = '" + plugInUserId + "' and Selected = 1");

            if (string.IsNullOrEmpty(scalar))
                return null;

            return Convert.ToInt32(scalar);
        }


        public static void SaveSelectedInstance(this Database database, string instanceId, int? sampleAppId, PlugInType plugInType, int appId, string viewName, int userId, string plugInUserId)
        {
            //const string InstanceTableName = "durados_PlugInInstance";
            const string AppIdColumnName = "AppId";
            const string SampleAppIdColumnName = "SampleAppId";
            const string ViewNameColumnName = "ViewName";
            const string PlugInIdColumnName = "PlugInId";
            const string UserIdColumnName = "UserId";
            const string InstanceIdColumnName = "InstanceId";
            const string PlugInUserIdColumnName = "PlugInUserId";
            //const string RegisteredUserIdColumnName = "RegisteredUserId";


            Dictionary<string, object> parameters = new Dictionary<string, object>();
            parameters.Add(AppIdColumnName, appId);
            parameters.Add(SampleAppIdColumnName, sampleAppId);
            parameters.Add(ViewNameColumnName, viewName);
            parameters.Add(PlugInIdColumnName, (int)plugInType);
            parameters.Add(InstanceIdColumnName, instanceId);
            parameters.Add(UserIdColumnName, userId);
            parameters.Add(PlugInUserIdColumnName, plugInUserId);
            //parameters.Add(RegisteredUserIdColumnName, registeredUserId);

            SqlAccess.ExecuteProcedure(database.ConnectionString, "durados_SaveSelectedInstance", parameters, null);

            //if (!(database.Map is DuradosMap))
            //    throw new DuradosException("Must be DuradosMap to get samples");

            //if (!(database.Views.ContainsKey(InstanceTableName)))
            //    throw new DuradosException("DuradosMap does not contains samples for plug-in, check web.config for privateCloud = true");

            //View instanceView = (View)database.Views[InstanceTableName];

            //Field appField = instanceView.GetFieldByColumnNames(AppIdColumnName);
            //Field plugInField = instanceView.GetFieldByColumnNames(PlugInIdColumnName);
            //Field userField = instanceView.GetFieldByColumnNames(UserIdColumnName);

            //Dictionary<string, object> values = new Dictionary<string, object>();

            //values.Add(appField.Name, appId.ToString());
            //values.Add(ViewNameColumnName, viewName);
            //values.Add(userField.Name, userId);

            //if (instanceView.GetDataRow(instanceId) == null)
            //{
            //    values.Add("Id", instanceId);
            //    values.Add(plugInField.Name, ((int)plugInType).ToString());
            //    instanceView.Create(values);
            // }
            //else
            //{
            //    instanceView.Edit(values, instanceId, null, null, null, null);
            //}
        }

        public static Option[] GetInstanceAndSamples(this Database database, PlugInType plugInType, string id, HttpRequestBase request)
        {
            Option instance = GetInstance(database, id, plugInType, request);
            Option[] sampleOptions = GetSamples(database, plugInType);
            DataRow instanceRow = GetSelectedInstanceRow(database, id, plugInType);
            int? sampleAppId = null;
            if (instanceRow != null)
            {
                if (!instanceRow.IsNull("SampleAppId"))
                {
                    sampleAppId = (int?)instanceRow["SampleAppId"];
                }
            }
            List<Option> options = new List<Option>();
            
            foreach (Option option in sampleOptions)
            {
                if (sampleAppId.HasValue && option.Value.Equals(sampleAppId.Value.ToString()))
                    option.Selected = true;
                options.Add(option);
            }
            if (instance != null)
            {
                if (instanceRow != null && !sampleAppId.HasValue)
                {
                    instance.Selected = true;
                }
                options.Insert(0, new Option() { Text = "----------", Value = "-", Selected = false });
                options.Insert(0, instance);

            }
            
            return options.ToArray();
        }

        public static bool IsBasedOnSample(this Option option, Option sampleOption)
        {
            string[] sampleValues = sampleOption.Value.Split(',');
            string sampleAppName = sampleValues[0];
            string sampleViewName = sampleValues[1];
            string samplePrefix = sampleOption.Tag.ToString();

            string[] values = sampleOption.Value.Split(',');
            string appName = values[0];
            string viewName = values[1];

            return (viewName.Equals(sampleViewName) && appName.StartsWith(samplePrefix));

        }

        public static Option GetInstance(this Database database, string id, PlugInType plugInType, HttpRequestBase request)
        {
            const string InstanceTableName = "durados_PlugInInstance";
            const string AppIdColumnName = "AppId";
            const string ViewNameColumnName = "ViewName";
            const string AppTitleColumnName = "Title";
            const string AppNameColumnName = "Name";
            const string InstanceAppRelationName = "InstanceApp";
            

            if (!(database.Map is DuradosMap))
                throw new DuradosException("Must be DuradosMap to get samples");

            if (!(database.Views.ContainsKey(InstanceTableName)))
                throw new DuradosException("DuradosMap does not contains samples for plug-in, check web.config for privateCloud = true");

            View instanceView = (View)database.Views[InstanceTableName];

            DataRow row = GetMyInstanceRow(database, id, plugInType, request);

            if (row == null)
                return null;

            string appId = row[AppIdColumnName].ToString();
            string appTitle = row.GetParentRow(InstanceAppRelationName)[AppTitleColumnName].ToString();
            string appName = row.GetParentRow(InstanceAppRelationName)[AppNameColumnName].ToString();
            string viewName = row[ViewNameColumnName].ToString();
            Map map = Maps.Instance.GetMap(appName);
            if (map.Database.Views.ContainsKey(viewName))
            {
                string viewDisplayName = map.Database.Views[viewName].DisplayName;
                return new Option() { Value = string.Empty, Text = appTitle + " - " + viewDisplayName };
            }

            return null;
        }

        public static int[] GetSamplesId(this Database database, PlugInType plugInType)
        {
            List<int> samples = new List<int>();

            foreach (Option option in GetSamples(database, plugInType))
            {
                samples.Add(Convert.ToInt32(option.Value));
            }

            return samples.ToArray();
        }

        public static Option[] GetSamples(this Database database, PlugInType plugInType)
        {
            const string SampleAppTableName = "durados_SampleApp";
            const string SampleNameColumnName = "Name";
            const string SamplePrefixColumnName = "AppNamePrefix";
            const string AppIdColumnName = "AppId";
            const string ViewNameColumnName = "ViewName";
            const string AppTitleColumnName = "Title";
            const string AppNameColumnName = "Name";
            const string SampleAppAppRelationName = "SampleAppApp";
            const string PlugInIdColumnName = "PlugInId";

            List<Option> samples = new List<Option>();

            if (!(database.Map is DuradosMap))
                throw new DuradosException("Must be DuradosMap to get samples");

            if (!(database.Views.ContainsKey(SampleAppTableName)))
                throw new DuradosException("DuradosMap does not contains samples for plug-in, check web.config for privateCloud = true");

            View samplesView = (View)database.Views[SampleAppTableName];
            int rowCount = 0;
            Dictionary<string, object> filterValues = new Dictionary<string, object>();
            filterValues.Add(PlugInIdColumnName, ((int)plugInType).ToString());
            Dictionary<string, SortDirection> sortColumns = new Dictionary<string, SortDirection>();
            sortColumns.Add("Ordinal", SortDirection.Asc);
            DataView dataView = samplesView.FillPage(1, 10000, filterValues, null, sortColumns, out rowCount, null, null);

            //ParentField appField = (ParentField)samplesView.GetFieldByColumnNames(AppIdColumnName);

            foreach (System.Data.DataRowView row in dataView)
            {
                string id = row["Id"].ToString();
                string appId = row[AppIdColumnName].ToString();
                string sampleName = row[SampleNameColumnName].ToString();
                string samplePrefix = row[SamplePrefixColumnName].ToString();
                string appTitle = row.Row.GetParentRow(SampleAppAppRelationName)[AppTitleColumnName].ToString();
                string appName = row.Row.GetParentRow(SampleAppAppRelationName)[AppNameColumnName].ToString();
                string viewName = row[ViewNameColumnName].ToString();
                Map sampleMap = Maps.Instance.GetMap(appName);
                if (sampleMap.Database.Views.ContainsKey(viewName))
                {
                    string viewDisplayName = sampleMap.Database.Views[viewName].DisplayName;
                    samples.Add(new Option() { Value = id, Text = sampleName, Tag = samplePrefix });
                }
            }

            return samples.ToArray();
        }

        public static bool IsRegistered(this Database database, string plugInUserId)
        {
            return GetRegisteredUserId(database, plugInUserId).HasValue;
        }

        //public static readonly string PlugInUserFlag = "NewUser";
        //public static bool IsPlugInUser(this Database database, string plugInUserId)
        //{
        //    DataRow dataRow = database.GetUserRow();

        //    if (dataRow == null)
        //        return false;



        //    return (!dataRow.IsNull(PlugInUserFlag) && dataRow[PlugInUserFlag].Equals(true));
        //}

        public static string GetPlugInUserId(PlugInType plugInType, HttpRequestBase request)
        {
            switch (plugInType)
            {
                case PlugInType.Wix:
                    return WixPlugInHelper.GetPlugInUserId(request);

                default:
                    throw new NotImplementedException();
            }
        }

        public static void AddSamplesToRegisteredUsers(this Database database, PlugInType plugInType, HttpRequestBase request)
        {
            string plugInUserId = GetPlugInUserId(plugInType, request);
            int[] registeredUsers = GetRegisteredUsers(database, plugInUserId);
            int[] sampleApps = GetSampleApps(database, plugInUserId);
            foreach (int registeredUser in registeredUsers)
            {
                DataRow userRow = database.GetUserRow(registeredUser);
                string username = userRow["Username"].ToString();
                string firstName = userRow["FirstName"].ToString();
                string lastName = userRow["LastName"].ToString();

                AddSamplesToRegisteredUser(database, plugInType, sampleApps, username, firstName, lastName);
            }
        }

        private static int[] GetRegisteredUsers(this Database database, string plugInUserId)
        {
            string sql = "select RegisteredUserId from durados_PlugInRegisteredUser WITH (NOLOCK) where PlugInUserId = '" + plugInUserId + "'";

            DataTable table = SqlAccess.ExecuteTable(database.ConnectionString, sql, null, CommandType.Text);

            List<int> list = new List<int>();

            foreach (DataRow row in table.Rows)
            {
                list.Add((int)row["RegisteredUserId"]);
            }

            return list.ToArray();
        }

        private static int[] GetSampleApps(this Database database, string plugInUserId)
        {
            string sql = "select AppId from durados_PlugInInstance WITH (NOLOCK) where PlugInUserId = '" + plugInUserId + "'";

            DataTable table = SqlAccess.ExecuteTable(database.ConnectionString, sql, null, CommandType.Text);

            List<int> list = new List<int>();

            foreach (DataRow row in table.Rows)
            {
                list.Add((int)row["AppId"]);
            }

            return list.ToArray();
        }

        public static void AddSamplesToRegisteredUser(this Database database, PlugInType plugInType, int[] sampleApps, string username, string firstName, string lastName)
        {
            foreach (int appId in sampleApps)
            {
                AddSampleToRegisteredUser(database, plugInType, appId, username, firstName, lastName);
            }
        }

        public static void AddSampleToRegisteredUser(this Database database, PlugInType plugInType, int appId, string username, string firstName, string lastName)
        {
            string connectionString = GetConnectionString(database, appId);

            string sql = "if not exists(select * from durados_User WITH (NOLOCK) where username = @username) insert into durados_User (Username, FirstName, LastName, Email, Role, Guid, IsApproved) values(@Username, @FirstName, @LastName, @Email, @Role, @Guid, @IsApproved)";

            Dictionary<string, object> parameters = new Dictionary<string, object>();
            parameters.Add("Username", username);
            parameters.Add("FirstName", firstName);
            parameters.Add("LastName", lastName);
            parameters.Add("Email", username);
            parameters.Add("Role", GetSampleCreatorRole());
            parameters.Add("Guid", Guid.NewGuid());
            parameters.Add("IsApproved", true);

            SqlAccess sqlAccess = new SqlAccess();
            sqlAccess.ExecuteNonQuery(connectionString, sql, parameters, null);

        }

        private static string GetConnectionString(this Database database, int appId)
        {
            IPersistency sqlPersistency = new SqlPersistency();
            sqlPersistency.ConnectionString = System.Configuration.ConfigurationManager.ConnectionStrings["MapsConnectionString"].ConnectionString;
            System.Data.SqlClient.SqlConnectionStringBuilder builder = new System.Data.SqlClient.SqlConnectionStringBuilder();
            builder.ConnectionString = sqlPersistency.ConnectionString;
            View appView = (View)database.Views["durados_App"];
            Field idField = appView.Fields["Id"];
            MapDataSet.durados_AppRow appRow = (MapDataSet.durados_AppRow)appView.GetDataRow(idField, appId.ToString(), false);
         
            return sqlPersistency.GetSystemConnection(appRow, builder).ToString();

        }

        public static void Logoff(this Database database, PlugInType plugInType, string plugInUserId)
        {
            SetCurrentPlugInRegisteredUser(database, plugInType, plugInUserId, null, true);
        }

        public static void SetCurrentPlugInRegisteredUser(this Database database, PlugInType plugInType, string plugInUserId, int? registeredUserId, bool logoff)
        {
            const string plugInIdColumnName = "plugInId";
            const string PlugInUserIdColumnName = "PlugInUserId";
            const string RegisteredUserIdColumnName = "RegisteredUserId";


            Dictionary<string, object> parameters = new Dictionary<string, object>();
            parameters.Add(plugInIdColumnName, (int)plugInType);
            parameters.Add(PlugInUserIdColumnName, plugInUserId);
            parameters.Add(RegisteredUserIdColumnName, registeredUserId);
            parameters.Add("@Logoff", logoff);

            SqlAccess.ExecuteProcedure(database.ConnectionString, "durados_SetCurrentPlugInRegisteredUser", parameters, null);

        }

        public static string GetLogOnPath()
        {
            return "~/Views/Account/LogOn.aspx";
        }

        public static void DeleteInstanceApp(this Database database, string instanceId, string controllerName)
        {
            string cnn = database.ConnectionString;

            string sql = "UPDATE dbo.durados_App SET [DeletedDate]=GETDATE(),[Deleted]=1 WHERE id in(SELECT DISTINCT appId FROM dbo.durados_PlugInInstance WITH(NOLOCK) WHERE InstanceId=@instanceId )";

            Dictionary<string, object> parameters = new Dictionary<string, object>();
            
            parameters.Add("@instanceId", instanceId);

            try
            {
                SqlAccess.ExecuteNonQuery(cnn, sql, parameters, null);
            }
            catch (Exception ex)
            {
                database.Logger.Log(controllerName, "RemoveWidget", "", ex, 3, null);
            }
           
        }
    }

    public enum PlugInType
    {
        app = 0,
        BackAnd = 1,
        Wix = 2,
        Heroku =3,
    }

    public static class WixPlugInHelper
    {

        public static WixInstance GetInstance(string instance)
        {
            //string instance = null;
            //if (string.IsNullOrEmpty(Request.QueryString["instance"]))
            //    return null;

            //instance = Request.QueryString["instance"];
            if (!ValidateSignedRequest(instance))
                throw new DuradosException();
            else
            {
                string json = DecodeFrom64(instance.Split('.')[1]);
                json = json.Replace("\"uid\":null,", "");
                WixInstance wixInstance = UI.Json.JsonSerializer.Deserialize<WixInstance>(json);

                return wixInstance;
            }

        }

        private static string DecodeFrom64(string encodedData)
        {

            return System.Text.Encoding.UTF8.GetString(Convert.FromBase64String(encodedData.PadRight(encodedData.Length + (4 - encodedData.Length % 4) % 4, '=')));


        }


        public static bool IsOwner(string instance)
        {
            return GetInstance(instance).permissions == "OWNER";
        }

        public static string GetPlugInUserId(HttpRequestBase request)
        {
            return GetPlugInUserId(request.QueryString["instance"]);
        }

        public static string GetPlugInUserId(string instance)
        {
            return GetInstance(instance).uid.ToString().ToUpper();
        }

        public static bool ValidateSignedRequest(string instance)
        {
            if (string.IsNullOrEmpty(instance))
                return false;
            string applicationSecret = System.Configuration.ConfigurationManager.AppSettings["wixkey"];
            string[] signedRequest = instance.Split('.');
            string expectedSignature = signedRequest[0];
            string payload = signedRequest[1];

            // Attempt to get same hash
            var Hmac = SignWithHmac(System.Text.UTF8Encoding.UTF8.GetBytes(payload), System.Text.UTF8Encoding.UTF8.GetBytes(applicationSecret));
            var HmacBase64 = ToUrlBase64String(Hmac);

            return (HmacBase64 == expectedSignature);
        }

        private static string ToUrlBase64String(byte[] Input)
        {
            return Convert.ToBase64String(Input).Replace("=", String.Empty)
                                                .Replace('+', '-')
                                                .Replace('/', '_');
        }

        private static byte[] SignWithHmac(byte[] dataToSign, byte[] keyBody)
        {
            using (var hmacAlgorithm = new System.Security.Cryptography.HMACSHA256(keyBody))
            {
                hmacAlgorithm.ComputeHash(dataToSign);
                return hmacAlgorithm.Hash;
            }
        }

        public static string GetWebAccessUrl(string mapUrl, string viewName, string userName)
        {
            string templateUrl = "/Home/Index/{0}?mainPage=True&userName={1}";
            return mapUrl + string.Format(templateUrl, viewName, userName);
        }

        public static int GetPlanId(string instance)
        {
            if (!string.IsNullOrEmpty(System.Configuration.ConfigurationManager.AppSettings["planId"]))
            {
                return Convert.ToInt32(System.Configuration.ConfigurationManager.AppSettings["planId"]);
            }
            string vendorProductId = WixPlugInHelper.GetInstance(instance).vendorProductId;
            return string.IsNullOrEmpty(vendorProductId) ? PlugInHelper.FreePlan : Convert.ToInt32(vendorProductId);
        }

        /// <summary>
        /// Get details of current web master access settings- By check if master user is exists
        /// </summary>
        /// <param name="map"></param>
        /// <param name="instance"></param>
        /// <param name="viewName"></param>
        /// <returns></returns>
        public static WebMasterAccess GetWebMasterAccessDetails(Map map, string instance, string viewName)
        {

            AccountMembershipService MembershipService = new AccountMembershipService();
            WebMasterAccess webMasterAccess = null;
            DataRow userRow = Maps.Instance.DuradosMap.Database.GetUserRow();
            
            if (userRow == null)
            {
                throw new DuradosException("User not registered");
            }
            
            string userId = Convert.ToString(userRow["ID"]);
            string userNameFormat = ConfigurationManager.AppSettings["MasterUserNameFormat"] ?? "master{0}@" + Durados.Database.LongProductName + ".com";
            string userName = string.Format(userNameFormat, userId);
            
            bool existInUsersTable = Maps.Instance.DuradosMap.Database.GetUserRow(userName) != null;
            bool existInCurrentAppUsersTable = map.Database.GetUserRow(userName) != null;
            bool existInMembership = MembershipService.ValidateUserExists(userName);

            //If web master access is exist- Init password and url
            if (existInUsersTable && existInMembership && existInCurrentAppUsersTable)
            {
                DataRow masterUserName = Maps.Instance.DuradosMap.Database.GetUserRow(userName);
                string password = CryptorHelper.Decrypt(Convert.ToString(masterUserName["Password"]), true);
                webMasterAccess = new WebMasterAccess() { Password = password, Url = GetWebAccessUrl(map.Url, viewName, userName) };
            }
            //If web master access is NOT exist- Init default password
            else
            {
                string defaultPassword = new AccountMembershipService().GetRandomPassword(12);
                webMasterAccess = new WebMasterAccess() { Password = defaultPassword, Url = string.Empty };
            }

            //Set plan id and plan element
            int planId = GetPlanId(instance);
            webMasterAccess.PlanId = planId;
            if (planId == 3)
            {
                webMasterAccess.UpgradePlanContent = map.Database.GetPlanContent();
            }

            return webMasterAccess;
        }
        
    }

    public class WebMasterAccess
    {
        public string Password { get; set; }
        public string Url { get; set; }
        public int PlanId { get; set; }
        public string UpgradePlanContent { get; set; }
    }

    public class WixInstance
    {
        public Guid instanceId { get; set; }
        public string signDate { get; set; }
        public Guid uid { get; set; }
        public string permissions { get; set; }
        public string ipAndPort { get; set; }
        public bool demoMode { get; set; }
        public string vendorProductId { get; set; }
    }

    public class AppsGenerator
    {
        //Consts
        const string AppTableName = "durados_App";
        const string IdColumnName = "Id";
        const string CreatedDateColumnName = "CreatedDate";
        const string CreatorColumnName = "Creator";
        const string NameColumnName = "Name";
        const string UrlColumnName = "Url";
        const string ImageColumnName = "Image";
        const string SystemSqlConnectionColumnName = "SystemSqlConnectionId";
        const string SecuritySqlConnectionColumnName = "SecuritySqlConnectionId";

        SqlAccess sqlAccess = new SqlAccess();

        public AppsGenerator()
        {
        }

        public void GenerateAllSamples(int? sleep)
        {
            const string SampleAppTableName = "durados_SampleApp";
            const string AppIdColumnName = "AppId";

            foreach (int sampleAppId in Maps.Instance.DuradosMap.Database.GetSamplesId(PlugInType.Wix))
            {
                DataRow sampleRow = Maps.Instance.DuradosMap.Database.Views[SampleAppTableName].GetDataRow(sampleAppId.ToString());
                int templateId = Convert.ToInt32(sampleRow[AppIdColumnName]);

                int remains = 0;
                bool addToInventory = PlugInHelper.AddToInventory(Maps.Instance.DuradosMap.Database, templateId, PlugInType.Wix, out remains);
                Maps.Instance.DuradosMap.Logger.Log("PlugIn", "Generation", "GenerateAllSamples", null, 3, "Start generate semple id = " + sampleAppId + ", remains = " + remains, DateTime.Now);
                    
                if (addToInventory)
                {
                    int count = Maps.PlugInSampleGenerationCount;
                    Maps.Instance.DuradosMap.Logger.Log("PlugIn", "Generation", "GenerateAllSamples", null, 3, "Add generate semple id = " + sampleAppId + ", count = " + count, DateTime.Now);
                    Generate(sampleAppId, count, sleep);
                    Maps.Instance.DuradosMap.Logger.Log("PlugIn", "Generation", "GenerateAllSamples", null, 3, "Finish generate semple id = " + sampleAppId + ", count = " + count, DateTime.Now);
                }
            }
        }

        public void Generate(int sampleAppId, int count)
        {
            Generate(sampleAppId, count, null);
        }

        /// <summary>
        /// Generate durados apps by duplicate schema in existing DB. Copy configuration from existing app.
        /// </summary>
        /// <param name="sampleAppId"></param>
        /// <param name="count"></param>
        public void Generate(int sampleAppId, int count, int? sleep)
        {
            Maps.Instance.DuradosMap.Logger.Log("AppsGenerator", "Start Generate", DateTime.Now.Millisecond.ToString(), null, 3, "sample id: " + sampleAppId + ", count: " + count);
            
            
            const string SampleAppTableName = "durados_SampleApp";
            const string AppIdColumnName = "AppId";
            const string AppNamePrefixColumnName = "AppNamePrefix";
            const string GenerationScriptColumnName = "GenerationScript";
            const string ViewName = "ViewName";

            Database mainDatabase = Maps.Instance.DuradosMap.Database;
            DataRow sampleRow = mainDatabase.Views[SampleAppTableName].GetDataRow(sampleAppId.ToString());
            SqlConnectionStringBuilder scsb = new SqlConnectionStringBuilder(Maps.Instance.DuradosMap.connectionString);
            scsb.InitialCatalog = sampleRow["DatabaseName"].ToString();

            if (!IsDatabaseExists(scsb.InitialCatalog,Maps.Instance.DuradosMap.connectionString))
            {
                CreateDatabase(scsb.InitialCatalog, Maps.Instance.DuradosMap.connectionString);
            }

            string connectionString = scsb.ConnectionString;
            ConnectionStringHelper.ValidateConnectionString(connectionString);
            int templateId = Convert.ToInt32(sampleRow[AppIdColumnName]);
            string generationScript = Convert.ToString(sampleRow[GenerationScriptColumnName]);
            string appNamePrefix = Convert.ToString(sampleRow[AppNamePrefixColumnName]);
            string viewName = Convert.ToString(sampleRow[ViewName]);

            Generate(mainDatabase, templateId, connectionString, generationScript, appNamePrefix, count, viewName, sleep);
            Maps.Instance.DuradosMap.Logger.Log("AppsGenerator", "End Generate", DateTime.Now.Millisecond.ToString(), null, 3, "sample id: " + sampleAppId + ", count: " + count);
            
        }

        private bool IsDatabaseExists(string databaseName, string connectionString)
        {
            SqlSchema sqlSchema = new SqlSchema();

            return sqlSchema.IsDatabaseExists(databaseName, connectionString);
        }

        private void CreateDatabase(string databaseName, string connectionString)
        {
            sqlAccess.CreateDatabase(connectionString, databaseName);
        }


        private SqlRequest GetInsertAppRequest(Database mainDatabase, int templateId)
        {
            //Prepare app row sql, parameters
            Dictionary<string, object> values = GetValuesForCreateApp(mainDatabase, templateId);
            string sql = SqlGeneralAccess.GetSqlInsertStatement(values.Keys.ToList(), AppTableName, true, null);
            IList<SqlParameter> parameters = SqlGeneralAccess.GetParemeters(values);

            SqlRequest sqlRequest = new SqlRequest(sql, parameters);

            return sqlRequest;
        }

        private SqlRequest GetInsertConnRequest(string connectionString)
        {
            //Prepare app row sql, parameters
            Dictionary<string, object> values = GetValuesForCreateConnection(connectionString);
            List<string> encryptedcolumnNamesList = new List<string>();
            encryptedcolumnNamesList.Add("Password");

            string sql = SqlGeneralAccess.GetSqlInsertStatement(values.Keys.ToList(), "durados_SqlConnection", true, encryptedcolumnNamesList);
            IList<SqlParameter> parameters = SqlGeneralAccess.GetParemeters(values);

            SqlRequest sqlRequest = new SqlRequest(sql, parameters);

            return sqlRequest;
        }

        /// <summary>
        /// Generate durados apps by duplicate schema in existing DB. Copy configuration from existing app.
        /// </summary>
        /// <param name="mainDatabase"></param>
        /// <param name="templateId"></param>
        /// <param name="connectionString"></param>
        /// <param name="generationScriptFileName"></param>
        /// <param name="appNamePrefix"></param>
        /// <param name="count"></param>
        private void Generate(Database mainDatabase, int templateId, string connectionString, string generationScriptFileName, string appNamePrefix, int count, string viewName, int? sleep)
        {

            //Only generate apps if abouve remains
            int remains = 0;
            bool addToInventory = PlugInHelper.AddToInventory(mainDatabase, templateId, PlugInType.Wix, out remains);
            if (!addToInventory)
                return;

            Maps.Instance.DuradosMap.Logger.Log("PlugIn", "Generate", "CreateApps", null, 3, "CreateApp template id = " + templateId + ", count = " + count, DateTime.Now);
            //Prepare Insert requests before the loop- because it simmilar for all generated apps.
            SqlRequest insertAppRequest = GetInsertAppRequest(mainDatabase, templateId);
            SqlRequest insertConnRequest = GetInsertConnRequest(connectionString);

            for (int i = 0; i < count; i++)
            {
                if (sleep.HasValue)
                {
                    System.Threading.Thread.Sleep(sleep.Value);
                }
                CreateApp(mainDatabase, templateId, connectionString, generationScriptFileName, appNamePrefix, insertAppRequest, insertConnRequest, viewName);
            }
        }

        /// <summary>
        /// Create new app by tenplate app
        /// </summary>
        /// <param name="mainDatabase"></param>
        /// <param name="templateId"></param>
        /// <param name="templateMap"></param>
        /// <param name="connectionString"></param>
        /// <param name="generationScriptFileName"></param>
        /// <param name="appNamePrefix"></param>
        private void CreateApp(Database mainDatabase, int templateId, string connectionString, string generationScriptFileName, string appNamePrefix, SqlRequest insertAppRequest, SqlRequest insertConnRequest, string viewName)
        {
            //Get template map
            string templateAppName = Maps.Instance.GetAppRow(templateId).Name;
            Map templateMap = Maps.Instance.GetMap(templateAppName);

            //Create app row
            int appId = CreateAppRow(mainDatabase, templateId, appNamePrefix, connectionString, insertAppRequest, insertConnRequest);

            //Handle generationScript
            string updatedGenerationScript = GetUpdatedGenerationScript(appId, templateMap, generationScriptFileName);
            RunGenerationScript(connectionString, updatedGenerationScript);

            //Handle configuration file
            string appName = Maps.Instance.GetAppRow(appId).Name;
            string configurationFilePath = CopyConfiguration(appId, templateId, templateMap, appName);
            UpdateConfiguration(configurationFilePath, appId, templateMap);

            CopyAzureContainer(appId, templateId);

            //Update the Custom views values in the System app
            UpdateCustomViews(viewName, appId, templateId);

            mainDatabase.UpdatePlan(appId, 3, false);

            //for each App make Asyn call to enable it
            //CallApp(appId); we need to find better way to this

        }

        private void CopyAzureContainer(int appId, int templateId)
        {
            string containerName = Maps.AzureAppPrefix + templateId;
            string newContainerName = Maps.AzureAppPrefix + appId;
            if (AzureHelper.DoesDefaultContainerExist(containerName))
                AzureHelper.GetDefaultContainerReference(templateId).Duplicate(newContainerName);
            else if (AzureHelper.DoesContainerExist(AzureHelper.GetDefaultBlobClient(), "general"))
                AzureHelper.GetContainerReference(AzureHelper.GetDefaultBlobClient(), "general").Duplicate(newContainerName);
            
        }

        private void CallApp(int appId)
        {
            string url = Maps.Instance.GetAppRow(appId).Url;
            //Infrastructure.ISendAsyncErrorHandler SendAsyncErrorHandler = null;
            Infrastructure.Http.CallWebRequest(url.Split('|')[2]);
        }

        /// <summary>
        /// Update the CustomViews for the new created view
        /// </summary>
        /// <param name="origView"></param>
        /// <param name="newAppId"></param>
        /// <param name="templateId"></param>
        private void UpdateCustomViews(string origView, int newAppId, int templateId)
        {
            string newView = origView + newAppId.ToString();

            string sql = string.Format("insert into [durados_CustomViews] ([UserId], [ViewName], [CustomView]) select [UserId], '{0}',[CustomView] from [durados_CustomViews] WITH (NOLOCK) where [ViewName] = '{1}'", newView, origView);

            //string systemConnection = insertAppRequest.Parameters.FirstOrDefault(parameter => parameter.ParameterName == SystemSqlConnectionColumnName).Value.ToString();

            string appName = Maps.Instance.GetAppRow(templateId).Name;

            Map map = Maps.Instance.GetMap(appName);

            SqlAccess s = new SqlAccess();
            s.ExecuteNonQuery(map.Database.SysDbConnectionString, sql);

        }

        
        /// <summary>
        /// Update a specific file by template scheme: Change views names to [viewName+appId]
        /// </summary>
        /// <param name="filePath"></param>
        /// <param name="appId"></param>
        /// <param name="templateMap"></param>
        /// <param name="needWriteFileContent"></param>
        /// <returns></returns>
        private string updatedFileByTemplateSchema(string filePath, int appId, Map templateMap, bool needWriteFileContent, string prefix, string suffix, bool wrapReplacement, bool replaceFields)
        {
            FileInfo file = new FileInfo(filePath);
            string content = File.ReadAllText(filePath);
            string uploadFolder = templateMap.Database.UploadFolder;
            string newUploadFolder = uploadFolder.Replace(templateMap.Id, appId.ToString());
            string oldAzureContainer = Maps.AzureAppPrefix + templateMap.Id + "</DirectoryVirtualPath>";
            string oldAzurePath = "<DirectoryBasePath>" + Maps.AzureAppPrefix + templateMap.Id + "</DirectoryBasePath>";
            string newAzurePath = "<DirectoryBasePath>" + Maps.AzureAppPrefix + appId.ToString() + "</DirectoryBasePath>";
            string oldGeneralAzureContainer = "general</DirectoryVirtualPath>";
            string newAzureContainer = Maps.AzureAppPrefix + appId + "</DirectoryVirtualPath>";

            //Replace views names
            foreach (View view in templateMap.Database.Views.Values.Where(view => !view.SystemView))
            {
                string viewName = view.Name;
                string replacement = string.Empty;
                if (!string.IsNullOrEmpty(view.EditableTableName))
                {
                    replacement = view.EditableTableName + appId;
                    if (wrapReplacement)
                        replacement = WrapToken(replacement, prefix, suffix);
                    content = content.Replace(WrapToken(view.EditableTableName, prefix, suffix), replacement);
                }
                replacement = viewName + appId;
                if (wrapReplacement)
                    replacement = WrapToken(replacement, prefix, suffix);
                content = content.Replace(WrapToken(viewName, prefix, suffix), replacement);

                if (replaceFields)
                {
                    foreach (Field field in view.Fields.Values)
                    {
                        if (field is ParentField && !string.IsNullOrEmpty(field.RelatedViewName))
                        {
                            string newFieldName = GetRelationName(viewName + appId, field.RelatedViewName + appId);
                            
                            if (wrapReplacement)
                                replacement = WrapToken(newFieldName, prefix, suffix);
                            content = content.Replace(WrapToken(field.Name, prefix, suffix), replacement);
                        }
                    }
                }
            }

            //Replace uploadFolder name
            content = content.Replace(uploadFolder, newUploadFolder);

            //Replace Azure Container
            content = content.Replace(oldAzureContainer, newAzureContainer);
            content = content.Replace(oldGeneralAzureContainer, newAzureContainer);
            content = content.Replace(oldAzurePath, newAzurePath);

            //Write to file if neccessary
            if (needWriteFileContent)
            {
                File.WriteAllText(filePath, content);
            }

            return content;
        }

        private string GetRelationName(string viewName, string relatedViewName)
        {
            string relationName = string.Format("FK_{0}_{1}_Parent", relatedViewName, viewName);

            return relationName;
        }

        const string DatabasePrefixToken = "$";
        const string DatabaseSuffixToken = "$";
        const string XmlPrefixToken = ">";
        const string XmlSuffixToken = "<";
        const string XmlNamePrefixToken = "<Name>";
        const string XmlNameSuffixToken = "</Name>";
        const string XmlBaseNamePrefixToken = "<BaseName>";
        const string XmlBaseNameSuffixToken = "</BaseName>";
        const string XmlRelatedViewNamePrefixToken = "<RelatedViewName>";
        const string XmlRelatedViewNameSuffixToken = "</RelatedViewName>";
        const string XmlEditableTableNamePrefixToken = "<EditableTableName>";
        const string XmlEditableTableNameSuffixToken = "</EditableTableName>";
        private string WrapToken(string objectName, string prefix, string suffix)
        {
            return prefix + objectName + suffix;
        }

        /// <summary>
        /// Update new configuration files content by template schema
        /// </summary>
        /// <param name="configurationFilePath"></param>
        /// <param name="appId"></param>
        /// <param name="templateMap"></param>
        private void UpdateConfiguration(string configurationFilePath, int appId, Map templateMap)
        {
            updatedFileByTemplateSchema(configurationFilePath, appId, templateMap, true, XmlNamePrefixToken, XmlNameSuffixToken, true, true);
            updatedFileByTemplateSchema(configurationFilePath, appId, templateMap, true, XmlRelatedViewNamePrefixToken, XmlRelatedViewNameSuffixToken, true, false);
            updatedFileByTemplateSchema(configurationFilePath, appId, templateMap, true, XmlBaseNamePrefixToken, XmlBaseNameSuffixToken, true, false);
            updatedFileByTemplateSchema(configurationFilePath, appId, templateMap, true, XmlEditableTableNamePrefixToken, XmlEditableTableNameSuffixToken, true, false);
            updatedFileByTemplateSchema(configurationFilePath + ".xml", appId, templateMap, true, XmlPrefixToken, XmlSuffixToken, true, false);
            
        }

        /// <summary>
        /// Copy configuration files from template application path to new  application path
        /// </summary>
        /// <param name="appId"></param>
        /// <param name="templateId"></param>
        /// <param name="templateMap"></param>
        /// <param name="appName"></param>
        /// <returns></returns>
        private string CopyConfiguration(int appId, int templateId, Map templateMap, string appName)
        {
            string oldConfigFileName = templateMap.ConfigFileName;
            string newConfigFileName = Maps.GetConfigPath(Maps.DuradosAppPrefix + appId + ".xml");
            string oldSchemaFileName = oldConfigFileName + ".xml";
            string newSchemaFileName = newConfigFileName + ".xml";

            System.IO.File.Copy(oldConfigFileName, newConfigFileName);
            System.IO.File.Copy(oldSchemaFileName, newSchemaFileName);

            return newConfigFileName;
        }

        /// <summary>
        /// Run script which generate new scema into existing database
        /// </summary>
        /// <param name="connectionString"></param>
        /// <param name="updatedGenerationScript"></param>
        private void RunGenerationScript(string connectionString, string updatedGenerationScript)
        {
            SqlCommand command = new SqlCommand();
            using (SqlConnection connection = new SqlConnection(connectionString))
            {

                command.Connection = connection;
                connection.Open();
                sqlAccess.RunScriptText(updatedGenerationScript, command);
            }
        }

        /// <summary>
        /// Copy app image from source application to target application.
        /// </summary>
        /// <param name="sourceImageField"></param>
        /// <param name="targetAppId"></param>
        /// <param name="sourceImageName"></param>
        /// <param name="sourceAppId"></param>
        /// <returns></returns>
        private string copyAppImage(Database mainDatabase, int targetAppId, int sourceAppId)
        {
            View appView = mainDatabase.Views[AppTableName] as View;
            ColumnField sourceImageField = appView.Fields[ImageColumnName] as ColumnField;
            string sourceImageName = appView.Fields[ImageColumnName].GetValue(sourceAppId.ToString());
            string uploadPath = sourceImageField.GetUploadPath();
            string oldImagePath = uploadPath + sourceImageName;

            if (sourceImageName.StartsWith(sourceAppId.ToString()))
            {
                sourceImageName = sourceImageName.Remove(0, sourceAppId.ToString().Length + 1);
            }

            string newImageRelativePath = targetAppId + "/" + sourceImageName;
            string newImagePath = Path.Combine(uploadPath, newImageRelativePath);

            System.IO.FileInfo fileInfo = new System.IO.FileInfo(newImagePath);
            fileInfo.Directory.Create();

            try
            {
                System.IO.File.Copy(oldImagePath, newImagePath);
            }
            catch { }

            return newImageRelativePath;
        }

        /// <summary>
        /// Get dictionary values by template id for create new app 
        /// </summary>
        /// <param name="mainDatabase"></param>
        /// <param name="templateId"></param>
        /// <returns></returns>
        private Dictionary<string, object> GetValuesForCreateApp(Database mainDatabase, int templateId)
        {
            View appView = mainDatabase.Views[AppTableName] as View;
            DataRow templateAppRow = appView.GetDataRow(templateId.ToString());
            DataRow templateRow = appView.GetDataRow(templateId.ToString());
            Dictionary<string, object> values = new Dictionary<string, object>();

            //Add values by template app row
            values.Add("UsesSpecificBinary", templateRow["UsesSpecificBinary"]);
            values.Add("ExcelFileName", templateRow["ExcelFileName"]);
            values.Add("SpecificDOTNET", templateRow["SpecificDOTNET"]);
            values.Add("SpecificJS", templateRow["SpecificJS"]);
            values.Add("SpecificCss", templateRow["SpecificCss"]);
            values.Add("UseAsTemplate", templateRow["UseAsTemplate"]);
            values.Add("Description", templateRow["Description"]);
            values.Add("TemplateFile", templateRow["TemplateFile"]);
            values.Add("PrivateAuthentication", templateRow["PrivateAuthentication"]);
            values.Add("Title", templateRow["Title"]);

            //Add extra values
            values.Add("ToDelete", false);
            values.Add("DataSourceTypeId", 4);
            values.Add("SqlConnectionId", -1);
            values.Add("Name", string.Empty);
            values.Add("TemplateId", templateId);

            //Add systemSqlConnectionId value if exist in template app row
            string systemSqlConnectionId = templateRow[SystemSqlConnectionColumnName].ToString();
            if (!string.IsNullOrEmpty(systemSqlConnectionId))
            {
                values.Add("SystemSqlConnectionId", systemSqlConnectionId);
            }

            ////Add securitySqlConnectionRelatedColumnName value if exist in template app row
            string securitySqlConnectionId = templateRow[SecuritySqlConnectionColumnName].ToString();
            if (!string.IsNullOrEmpty(securitySqlConnectionId))
            {
                values.Add("SecuritySqlConnectionId", securitySqlConnectionId);
            }

            return values;
        }

        /// <summary>
        /// Insert row for new application in table "durados_App"
        /// </summary>
        /// <param name="mainDatabase"></param>
        /// <param name="templateId"></param>
        /// <param name="sqlConnectionId"></param>
        /// <param name="connection"></param>
        /// <param name="transaction"></param>
        /// <returns></returns>
        private int InsertAppRow(Database mainDatabase, int templateId, int sqlConnectionId, SqlConnection connection, SqlTransaction transaction, SqlRequest insertAppRequest)
        {
            object newAppObjId = null;
            int newAppId;

            insertAppRequest.Parameters.FirstOrDefault(parameter => parameter.ParameterName == "SqlConnectionId").Value = sqlConnectionId;
            insertAppRequest.Parameters.FirstOrDefault(parameter => parameter.ParameterName == "Name").Value = Guid.NewGuid().ToString();

            //Create app row
            newAppObjId = SqlGeneralAccess.Create(insertAppRequest, connection, transaction);

            //Return app id
            if (Int32.TryParse(newAppObjId.ToString(), out newAppId))
                return newAppId;
            else
                throw new DuradosException("Failed to get application id");
        }

        private Dictionary<string, object> GetValuesForCreateConnection(string connectionString)
        {
            Dictionary<string, object> values = new Dictionary<string, object>();
            SqlConnectionStringBuilder SqlConnectionBuilder = new SqlConnectionStringBuilder(connectionString);

            //Prepare connection row values
            values.Add("ServerName", SqlConnectionBuilder.DataSource);
            values.Add("Catalog", SqlConnectionBuilder.InitialCatalog);
            values.Add("Username", SqlConnectionBuilder.UserID);
            values.Add("IntegratedSecurity", SqlConnectionBuilder.IntegratedSecurity);
            values.Add("Password", SqlConnectionBuilder.Password);

            return values;
        }

        /// <summary>
        /// Insert connection row in table "durados_SqlConnection"
        /// </summary>
        /// <param name="connectionString"></param>
        /// <param name="connection"></param>
        /// <param name="transaction"></param>
        /// <returns></returns>
        private int InsertConnRow(string connectionString, SqlConnection connection, SqlTransaction transaction, SqlRequest insertConnRequest)
        {
            object sqlConnectionObjId = null;
            int sqlConnectionId;

            //Create connection row
            sqlConnectionObjId = SqlGeneralAccess.Create(insertConnRequest, connection, transaction);

            //Return connection id
            if (Int32.TryParse(sqlConnectionObjId.ToString(), out sqlConnectionId))
                return sqlConnectionId;
            else
                throw new DuradosException("Failed to get connection id");
        }

        /// <summary>
        /// Create row on durados_app table
        /// </summary>
        /// <param name="mainDatabase"></param>
        /// <param name="templateId"></param>
        /// <param name="appNamePrefix"></param>
        /// <param name="connectionString"></param>
        /// <returns></returns>
        private int CreateAppRow(Database mainDatabase, int templateId, string appNamePrefix, string connectionString, SqlRequest insertAppRequest, SqlRequest insertConnRequest)
        {
            int newAppId;

            using (SqlConnection connection = new SqlConnection(mainDatabase.ConnectionString))
            {
                connection.Open();

                SqlTransaction transaction = connection.BeginTransaction(IsolationLevel.ReadCommitted);

                try
                {
                    //Insert connection row and app row
                    int sqlConnectionId = InsertConnRow(connectionString, connection, transaction, insertConnRequest);
                    newAppId = InsertAppRow(mainDatabase, templateId, sqlConnectionId, connection, transaction, insertAppRequest);

                    //Copy image from template app directory
                    string imagePath = copyAppImage(mainDatabase, newAppId, templateId);

                    //Update app row for columns: Name, Url, Image
                    UpdateAppRow(imagePath, templateId, newAppId, appNamePrefix, connection, transaction);
                }
                catch (Exception exception)
                {
                    transaction.Rollback();
                    throw new DuradosException(string.Format("Create application by template {0} failed.", templateId), exception);
                }
                transaction.Commit();
            }

            //Create dns
            string appName = appNamePrefix + newAppId.ToString();
            CreateDns(appName);

            return newAppId;
        }

        /// <summary>
        //Update app row for columns: Name, Url, Image
        /// </summary>
        /// <param name="mainDatabase"></param>
        /// <param name="templateId"></param>
        /// <param name="newAppId"></param>
        /// <param name="appNamePrefix"></param>
        /// <param name="connection"></param>
        /// <param name="transaction"></param>
        private void UpdateAppRow(string imagePath, int templateId, int newAppId, string appNamePrefix, SqlConnection connection, SqlTransaction transaction)
        {
            string appName = appNamePrefix + newAppId;
            string url = Maps.GetAppUrl(appName, true);

            //Prepare updated values
            Dictionary<string, object> updatedValues = new Dictionary<string, object>();
            updatedValues.Add(NameColumnName, appName);
            updatedValues.Add(UrlColumnName, url);
            updatedValues.Add(ImageColumnName, imagePath);

            //Update app row
            SqlGeneralAccess.Update(updatedValues, AppTableName, "Id=" + newAppId, connection, transaction);
        }

        /// <summary>
        /// Get generationScript after update views names to views names with appIdPostfix
        /// </summary>
        /// <param name="appIdPostfix"></param>
        /// <param name="templateMap"></param>
        /// <param name="generationScriptFileName"></param>
        /// <returns></returns>
        private string GetUpdatedGenerationScript(int appIdPostfix, Map templateMap, string generationScriptFileName)
        {
            string generationScriptPath = Maps.GetDeploymentPath("Sql/SampleApp/" + generationScriptFileName);

            return updatedFileByTemplateSchema(generationScriptPath, appIdPostfix, templateMap, false, DatabasePrefixToken, DatabaseSuffixToken, false, false);
        }

        /// <summary>
        /// Create dns in debug mode
        /// </summary>
        /// <param name="name"></param>
        private void CreateDns(string name)
        {
            if (Maps.Debug)
            {
                try
                {
                    string windowsPath = System.Environment.GetEnvironmentVariable("windir");

                    using (System.IO.StreamWriter sw = new System.IO.StreamWriter(windowsPath + @"\system32\drivers\etc\hosts", true))
                    {
                        sw.WriteLine(string.Format("127.0.0.1   {0}", name + "." + Maps.Host));

                        sw.Close();
                    }
                }
                catch { }
            }


        }
    }

    public class Cleaner
    {
        public int CleanDeletedApps(int limit)
        {
            DataTable apps = GetDeletedApps(limit);

            int success = 0;
            int failure = 0;
            string message = string.Empty;

            foreach (DataRow row in apps.Rows)
            {
                int id = (int)row["Id"];
                string catalog = (string)row["Catalog"];
                try
                {
                    CleanDeletedApp(id, catalog);
                    success++;
                }
                catch (Exception excpetion)
                {
                    message += id + " failed: " + excpetion.Message + "<br>";
                    failure++;
                }
                UpdateAppRow(id);
            }

            if (message != string.Empty)
                throw new DuradosException(success + " successes, " + failure + " failures: " + "<br>" + message);

            return success;
        }

        private void CleanDeletedApp(int id, string catalog)
        {
            CleanXml(id);
            CleanDatabase(id, catalog);
            
        }

        private void UpdateAppRow(int id)
        {
            SqlAccess sqlAccess = new SqlAccess();
            string sql = "update durados_App set ToDelete=1 where id=" + id;

            sqlAccess.ExecuteNonQuery(Maps.Instance.DuradosMap.connectionString, sql);
        }

        private void CleanDatabase(int id, string catalog)
        {
            
        }

        private void CleanXml(int id)
        {
            string xmlFileName = Maps.GetConfigPath(Maps.DuradosAppPrefix + id + ".xml");
            string xmlxmlFileName = Maps.GetConfigPath(Maps.DuradosAppPrefix + id + ".xml.xml");

            if (File.Exists(xmlFileName))
            {
                File.Delete(xmlFileName);
            }
            else
            {
                throw new FileNotFoundException("File not found " + xmlFileName);
            }

            if (File.Exists(xmlxmlFileName))
            {
                File.Delete(xmlxmlFileName);
            }
            else
            {
                throw new FileNotFoundException("File not found " + xmlxmlFileName);
            }
        }

        private DataTable GetDeletedApps(int limit)
        {
            SqlAccess sqlAccess = new SqlAccess();

            string top = string.Empty;

            if (limit > -1)
            {
                top = "top(" + limit + ")";
            }

            string sql = "SELECT  " + top + "   dbo.durados_App.Id, dbo.durados_SqlConnection.ServerName, dbo.durados_SqlConnection.Catalog " +
                        " FROM         dbo.durados_App INNER JOIN " +
                            "dbo.durados_SqlConnection ON dbo.durados_App.SqlConnectionId = dbo.durados_SqlConnection.Id INNER JOIN " +
                            "dbo.durados_User ON dbo.durados_App.Creator = dbo.durados_User.ID " +
                        " WHERE     (dbo.durados_User.Username LIKE N'wix%') AND (dbo.durados_App.Deleted = 1) AND (dbo.durados_App.ToDelete = 0)";

            return sqlAccess.ExecuteTable(Maps.Instance.DuradosMap.connectionString, sql, null, CommandType.Text);
        }
    }
}
