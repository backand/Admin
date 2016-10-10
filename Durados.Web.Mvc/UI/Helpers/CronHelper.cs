using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Durados.DataAccess;
using System.Data;
using Backand;
using System.Collections;

namespace Durados.Web.Mvc.UI.Helpers
{
    public static class CronHelper
    {
        #region Consts

        private const string WHERE = "CronName= '{0}' and AppId = {1}";
        private const string TABLE_NAME = "durados_Cron";

        #endregion

        public static bool IsCronExists(string name)
        {
            string appId = Maps.Instance.GetMap().Id;
            string connectionString = Maps.Instance.DuradosMap.connectionString;
            string whereStatement = string.Format(WHERE, name, appId);
            Dictionary<string, object> values = new Dictionary<string, object>();
            return SqlGeneralAccess.Select(values, TABLE_NAME, whereStatement, connectionString).Rows.Count > 0;
        }

        public static void Create(string name, string cycle)
        {
            Dictionary<string, object> values = new Dictionary<string, object>();
            string appId = Maps.Instance.GetMap().Id;
            string connectionString = Maps.Instance.DuradosMap.connectionString;

            int cycleNumber = System.Convert.ToInt32(Enum.Parse(typeof(CycleEnum), cycle));
            values.Add("CronName", name);
            values.Add("Cycle", cycleNumber);
            values.Add("AppId", appId);
            SqlGeneralAccess.Create(values, TABLE_NAME, true, connectionString);
        }

        public static void Edit(string name, string cycle, string prevName)
        {
            Dictionary<string, object> values = new Dictionary<string, object>();
            string appId = Maps.Instance.GetMap().Id;

            int cycleNumber = System.Convert.ToInt32(Enum.Parse(typeof(CycleEnum), cycle));
            values.Add("CronName", name);
            values.Add("Cycle", cycleNumber);
            string whereStatement = string.Format(WHERE, prevName, appId);
            string connectionString = Maps.Instance.DuradosMap.connectionString;

            SqlGeneralAccess.Update(values, TABLE_NAME, whereStatement, connectionString);
        }

        public static void Delete(string name)
        {
            string appId = Maps.Instance.GetMap().Id;
            string connectionString = Maps.Instance.DuradosMap.connectionString;
            string whereStatment = string.Format(WHERE, name, appId);
            SqlGeneralAccess.Delete(TABLE_NAME, whereStatment, connectionString);
        }

        public static DataTable GetCycleCrons(int cycle)
        {
            DataTable cronsTable = new DataTable();
            string connectionString = Maps.Instance.DuradosMap.connectionString;
            string whereStatement = string.Format(" Cycle = {0}", cycle);
            Dictionary<string, object> values = new Dictionary<string, object>();
            values.Add("AppId", null);
            cronsTable = SqlGeneralAccess.Select(values, TABLE_NAME, whereStatement, connectionString);
            return cronsTable;
        }

        public static string GetAppName(int appId)
        {
            string appName = string.Empty;
            string connectionString = Maps.Instance.DuradosMap.connectionString;
            string sql = string.Format("select Name from [durados_App] where Id= {0}", appId);

            SqlAccess sqlAccess = new SqlAccess();
            appName = sqlAccess.ExecuteScalar(connectionString, sql);
            return appName;
        }

        const string GET = "GET";
        const string POST = "POST";


        public static CronRequestInfo GetRequestInfo(Cron cron, bool test)
        {

            CronRequestInfo requestInfo = new CronRequestInfo();

            if (cron.CronType == CronType.External)
                requestInfo.url = cron.ExternalUrl;
            else if (cron.CronType == CronType.Action)
                requestInfo.url = GetActionUrl(cron.EntityId, cron.QueryString);
            else if (cron.CronType == CronType.Query)
                requestInfo.url = GetQueryUrl(cron.EntityId, cron.QueryString);


            requestInfo.method = GetMethod(cron.Method);

            if (requestInfo.method == POST)
                requestInfo.data = cron.Data;

            requestInfo.headers = GetHeaders(cron.Headers);

            SetAuthorization(requestInfo, test);

            return requestInfo;
        }

        private static void SetAuthorization(CronRequestInfo requestInfo, bool test)
        {
            const string AUTHORIZATION = "AUTHORIZATION";
            const string AppId = "AppId";
           
            if (requestInfo.url.ToUpper().Contains(AUTHORIZATION) || (requestInfo.headers != null && requestInfo.headers.Keys.Where(k=>k.ToUpper() == AUTHORIZATION).FirstOrDefault() != null))
                return;

            if (System.Web.HttpContext.Current.Request.QueryString[AUTHORIZATION] != null)
            {
                requestInfo.url += "&authorization=" + System.Web.HttpContext.Current.Request.QueryString[AUTHORIZATION];
                return;
            }

            if (requestInfo.headers == null)
                requestInfo.headers = new Dictionary<string, object>();
            
            if (test)
            {
                string appId = Maps.Instance.GetMap().Id;
                if (!requestInfo.headers.ContainsKey("authorization"))
                    requestInfo.headers.Add("authorization", Maps.CronAuthorizationHeader);
                if (!requestInfo.headers.ContainsKey(AppId))
                    requestInfo.headers.Add(AppId, appId);
                return;
            }
            else if (System.Web.HttpContext.Current.Request.Headers[AUTHORIZATION] != null)
            {
                if (!requestInfo.headers.ContainsKey("authorization"))
                    requestInfo.headers.Add("authorization", System.Web.HttpContext.Current.Request.Headers[AUTHORIZATION]);
                if (System.Web.HttpContext.Current.Request.Headers[AppId] != null)
                    if (!requestInfo.headers.ContainsKey(AppId))
                        requestInfo.headers.Add(AppId, System.Web.HttpContext.Current.Request.Headers[AppId]);
                return;
            }

            throw new CronAuthorizationWasNotSuppliedException();
        }

        private static string GetQueryUrl(int queryId, string queryString)
        {
            Query query = GetQuery(queryId);

            string hostAndProtocol = GetHostAndProtocol();

            return hostAndProtocol + "/1/query/data/" + query.Name + "?" + queryString;
        }

        private static Query GetQuery(int queryId)
        {
            Map map = Maps.Instance.GetMap();
            Query query = null;

            query = (Query)map.Database.Queries.Values.Where(q => q.ID == queryId).FirstOrDefault();
            if (query == null)
            {
                throw new QueryNotFoundException(queryId);
            }

            return query;
        }

        private static string GetActionUrl(int actionId, string queryString)
        {
            Dictionary<string, object> ruleAndAction = GetRuleAndAction(actionId);
            Rule action = (Rule)ruleAndAction["rule"];
            View view = (View)ruleAndAction["view"];
            
            string hostAndProtocol = GetHostAndProtocol();

            return hostAndProtocol + "/1/objects/action/" + view.JsonName + "?name=" + action.Name + "&" + queryString;
        }

        private static string GetHostAndProtocol()
        {
            return System.Web.HttpContext.Current.Request.Url.Scheme + "://" + System.Web.HttpContext.Current.Request.Url.Host + ":" + System.Web.HttpContext.Current.Request.Url.Port;
        }

        public static Dictionary<string, object> GetRuleAndAction(int actionId)
        {
            Map map = Maps.Instance.GetMap();

            ConfigAccess configAccess = new ConfigAccess();
            DataRow row = configAccess.GetRow("Rule", "ID", actionId.ToString(), map.GetConfigDatabase().ConnectionString);

            if (row == null)
            {
                throw new ActionNotFoundException(actionId);
            }
            int viewId = System.Convert.ToInt32(row["Rules"]);

            string name = row["Name"].ToString();

            View view = null;

            view = (View)map.Database.Views.Values.Where(v => v.ID == viewId).FirstOrDefault();
            if (view == null)
            {
                throw new ActionNotFoundException(actionId, viewId);
            }
            
            Rule rule = view.Rules[name];

            return new Dictionary<string, object>() { { "view", view }, { "rule", rule } };
        }

        private static string GetMethod(string method)
        {
            if (string.IsNullOrEmpty(method))
                return GET;
            else if (method.ToUpper() != GET && method.ToUpper() != POST)
                throw new InvalidHttpMethodException(method);
            else
                return method.ToUpper();

        }

        private static Dictionary<string, object> GetHeaders(string headers)
        {
            System.Web.Script.Serialization.JavaScriptSerializer jss = new System.Web.Script.Serialization.JavaScriptSerializer();

            if (string.IsNullOrEmpty(headers))
                return null;

            try
            {
                return jss.Deserialize<Dictionary<string, object>>(headers);
            }
            catch (Exception exception)
            {
                throw new InvalidCronHeadersJsonException(exception);
            }
        }

        private static string BaseUrl = System.Configuration.ConfigurationManager.AppSettings["nodeHost"] ?? "http://127.0.0.1:9000";
        private static string LambdaArn = Maps.LambdaArn;
        private static string CronPrefix = Maps.CronPrefix;
        public static void putCron(Cron cron)
        {

            Dictionary<string, object> input = GetInput(cron);
            string name = GetName(cron);

            putCron(name, cron.Schedule, input, cron.Active, cron.Description);
        }

        private static string GetName(Cron cron)
        {
            return GetNamePrefix() + cron.ID;
        }

        private static string GetNamePrefix(string appId = null)
        {
            return CronPrefix + "-" + (appId ?? Maps.Instance.GetMap().Id) + "-";
        }

        private static Dictionary<string, object> GetInput(Cron cron)
        {
            const string CronId = "cronId";
            const string AppId = "appId";
            
            Dictionary<string, object> input = new Dictionary<string, object>();
            
            input.Add(CronId, cron.ID);
            input.Add(AppId, Maps.Instance.GetMap().Id);
            
            return input;
        }

        private static void putCron(string name, string schedule, Dictionary<string, object> input, bool? active, string description)
        {
            string url = BaseUrl + "/putCron";
            XMLHttpRequest request = new XMLHttpRequest();
            request.open("POST", url, false);
            Dictionary<string, object> data = new Dictionary<string, object>();

            data.Add("name", name);
            data.Add("schedule", HandleSchedule(schedule));
            data.Add("lambdaArn", LambdaArn);
            data.Add("input", input);
            if (active.HasValue)
            {
                data.Add("active", active.Value);
            }
            else
            {
                data.Add("active", null);
            }
            data.Add("description", description);
            
            request.setRequestHeader("content-type", "application/json");

            System.Web.Script.Serialization.JavaScriptSerializer jss = new System.Web.Script.Serialization.JavaScriptSerializer();
            request.send(jss.Serialize(data));

            if (request.status != 200)
            {
                string message = request.responseText;
                if (request.responseText.Contains("Parameter ScheduleExpression is not valid"))
                {
                    message = "The schedule expression '" + schedule + "' is not valid";
                }
                throw new Durados.DuradosException(message);
            }

            Dictionary<string, object> response = null;
            try
            {
                response = jss.Deserialize<Dictionary<string, object>>(request.responseText);
            }
            catch (Exception exception)
            {
                throw new Durados.DuradosException("Could not parse putCron response", exception);
            }

        }

        private static object HandleSchedule(string schedule)
        {
            CronScheduler.Scheduler scheduler = new CronScheduler.AwsScheduler();
            return scheduler.Standardize(schedule);
        }

        public static void deleteCron(Cron cron)
        {

            string name = GetName(cron);

            deleteCron(name);
        }

        private static void deleteCron(string name)
        {
            string url = BaseUrl + "/deleteCron";
            XMLHttpRequest request = new XMLHttpRequest();
            request.open("POST", url, false);
            Dictionary<string, object> data = new Dictionary<string, object>();

            data.Add("name", name);
            
            request.setRequestHeader("content-type", "application/json");

            System.Web.Script.Serialization.JavaScriptSerializer jss = new System.Web.Script.Serialization.JavaScriptSerializer();
            request.send(jss.Serialize(data));

            if (request.status != 200)
            {
                throw new Durados.DuradosException("Server return status " + request.status + ", " + request.responseText);
            }

            Dictionary<string, object> response = null;
            try
            {
                response = jss.Deserialize<Dictionary<string, object>>(request.responseText);
            }
            catch (Exception exception)
            {
                throw new Durados.DuradosException("Could not parse deleteCron response", exception);
            }

        }

        const string RULES = "Rules";
        const string NAME = "Name";
              
        public static Dictionary<string, Dictionary<string, object>> getCron(Cron cron = null)
        {
            
            string prefix = cron == null ? GetNamePrefix() : GetName(cron);
            Dictionary<string, object> crons = getCron(prefix);
            Dictionary<string, Dictionary<string, object>> cronsDictionay = new Dictionary<string, Dictionary<string, object>>();

            if (!crons.ContainsKey(RULES))
            {
                return cronsDictionay;
            }

            ArrayList cronsArray = (ArrayList)crons[RULES];

            
            if (cron != null)
            {
                if (cronsArray.Count == 1)
                {
                    cronsDictionay.Add(cron.ID.ToString(), (Dictionary<string, object>)cronsArray[0]);
                }
            }
            else
            {
                foreach (Dictionary<string, object> c in cronsArray)
                {
                    string name = GetCronName(c[NAME].ToString());
                    cronsDictionay.Add(name, c);
                }
            }
            return cronsDictionay;
        }

        private static string GetCronName(string name)
        {
            return name.Replace(GetNamePrefix(), string.Empty);
        }

        private static Dictionary<string, object> getCron(string prefix)
        {
            string url = BaseUrl + "/getCron";
            XMLHttpRequest request = new XMLHttpRequest();
            request.open("POST", url, false);
            Dictionary<string, object> data = new Dictionary<string, object>();

            data.Add("namePrefix", prefix);

            request.setRequestHeader("content-type", "application/json");

            System.Web.Script.Serialization.JavaScriptSerializer jss = new System.Web.Script.Serialization.JavaScriptSerializer();
            request.send(jss.Serialize(data));

            if (request.status != 200)
            {
                throw new Durados.DuradosException("Server return status " + request.status + ", " + request.responseText);
            }

            Dictionary<string, object> response = null;
            try
            {
                response = jss.Deserialize<Dictionary<string, object>>(request.responseText);
            }
            catch (Exception exception)
            {
                throw new Durados.DuradosException("Could not parse getCron response", exception);
            }

            return response;
        }


        public static void DeleteAllCrons(string appId)
        {
            Dictionary<string, object> crons = getCron(GetNamePrefix(appId));
            if (!crons.ContainsKey(RULES))
            {
                return;
            }
             ArrayList cronsArray = (ArrayList)crons[RULES];

             foreach (Dictionary<string, object> cron in cronsArray)
             {
                 string name = cron[NAME].ToString();
                 deleteCron(name);
             }
        }

        public static object GetTestRequest(CronRequestInfo requestInfo)
        {
            Dictionary<string, object> dic = new Dictionary<string, object>();

            if (requestInfo.data != null)
            {
                dic.Add("data", requestInfo.data);
            }
            if (requestInfo.url != null)
            {
                dic.Add("url", requestInfo.url);
            }
            if (requestInfo.method != null)
            {
                dic.Add("method", requestInfo.method);
            }
            return dic;
        }
    }
}
