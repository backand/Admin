using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;

using Jint;
using System.IO;

namespace Durados.Workflow
{
    public class JavaScript
    {
        public delegate string btoaHandler(string text);
    

        #region consts

        public static readonly string ReturnedValueKey = "$$ReturnedValueKey$$";
        const string FILEDATA = "filedata";
        const string FILENAME = "filename";
        public static readonly string ActionInfo = "ActionInfo";
        public static readonly string ActionHeaderKey = "Backand-Action-Guid";
        public static readonly string StorageAccountsKey = "storage_accounts";
        #endregion

        private static string xhr = null;
        private string defaultJsInfrastructureFileName = Path.GetDirectoryName(AppDomain.CurrentDomain.BaseDirectory) + @"\deployment\jsInfrastructure.js";
        public string GetXhrWrapper()
        {
            return GetXhrWrapper(defaultJsInfrastructureFileName);
        }
        public static string GetXhrWrapper(string jsInfrastructureFileName)
        {
            if (xhr == null)
            {
                if (File.Exists(jsInfrastructureFileName))
                {
                    xhr = File.ReadAllText(jsInfrastructureFileName);
                }
                else
                {
                    throw new System.IO.FileNotFoundException("The js infrastructure file was not found", jsInfrastructureFileName);
                }
            }
            return xhr;
        }

        static int logLimit = -1;
        public static int LogLimit
        {
            get
            {
                if (logLimit == -1)
                {
                    logLimit = System.Convert.ToInt32(System.Configuration.ConfigurationManager.AppSettings["logLimit"] ?? "1000");
                }
                return logLimit;
            }
        }

        public static readonly string Debug = "Debug";
        public static readonly string LogCounter = "LogCounter";
        public static readonly string GuidKey = "JsGuid";
        public static readonly string ConnectionStringKey = "ConnectionStringKey";

        static System.Web.Script.Serialization.JavaScriptSerializer jss = new System.Web.Script.Serialization.JavaScriptSerializer();

        static JavaScript()
        {
            jss.MaxJsonLength = int.MaxValue;
        }
                
        public static void SetCacheInCurrentRequest(string key, object value)
        {
            if (key == Durados.Workflow.JavaScript.GuidKey && System.Web.HttpContext.Current.Request.Headers[Durados.Workflow.JavaScript.ActionHeaderKey] != null)
            {
                value = Guid.Parse(System.Web.HttpContext.Current.Request.Headers[Durados.Workflow.JavaScript.ActionHeaderKey]);
            }
            if (key == Durados.Workflow.JavaScript.GuidKey && System.Web.HttpContext.Current.Items.Contains(key))
                return;
            if (System.Web.HttpContext.Current.Items.Contains(key))
                System.Web.HttpContext.Current.Items[key] = value;
            else
                System.Web.HttpContext.Current.Items.Add(key, value);

        }

        public static object GetCacheInCurrentRequest(string key)
        {
            try
            {
                if (System.Web.HttpContext.Current != null && System.Web.HttpContext.Current.Items.Contains(key))
                    return System.Web.HttpContext.Current.Items[key];
            }
            catch { }

            return null;
        }

        public static bool IsDebug()
        {
            return System.Convert.ToBoolean(Durados.Workflow.JavaScript.GetCacheInCurrentRequest(Durados.Workflow.JavaScript.Debug) ?? false);
        }

        public static string HandleLineCodes(string message, string objectName, string actionName, View view, bool debug)
        {
            

            message = message.Replace(Jint.Engine.EscapeCurlyBrackets.Left, "{").Replace(Jint.Engine.EscapeCurlyBrackets.Right, "}").Replace("\\\"", "\"");
            Guid requestGuid = (Guid)Durados.Workflow.JavaScript.GetCacheInCurrentRequest(Durados.Workflow.JavaScript.GuidKey);
            string actionGuid = requestGuid.ToString();
            
            try
            {
                var json = CleanJson(message);

                string previousTrace = null;
                if (debug || true)
                {
                    previousTrace = GetPreviousTrace(view, actionGuid);

                    if (!string.IsNullOrEmpty(previousTrace))
                    {
                        json = MergeTrace(json, previousTrace);
                    }

                    SetCurrentTrace(view, json, actionGuid);
                }

                message = CleanMessage(json);
            }
            catch { }

            

            return message;

            // "The follwoing action: "aaa" failed to perform: Failed to load the javascript code: Line 166: Unexpected token }"
        }

        private static void SetCurrentTrace(View view, IDictionary<string, object> json, string actionGuid)
        {
            const int TenMinutes = 1000 * 60 * 10;

            view.Database.SetCacheValue(actionGuid, jss.Serialize(json), TenMinutes);
        }

        private static IDictionary<string, object> MergeTrace(IDictionary<string, object> json, string previousTrace)
        {
            var previousTraceJson = jss.Deserialize<Dictionary<string,object>>(previousTrace);
            IDictionary<string, object> child = previousTraceJson;
            
            bool eoj = false;
            while (!eoj)
            {

                if (child.Keys.Count == 2)
                {
                    string key = child.Keys.Where(k => k != "at").FirstOrDefault();
                    if (child[key] is IDictionary<string, object>)
                    {
                        child = (IDictionary<string, object>)child[key];
                    }
                    else
                    {
                        child[key] = json;
                        return previousTraceJson;
                    }
                }
                else
                {
                    eoj = true;
                }

            }
            return json;
        }

        private static string GetPreviousTrace(View view, string actionGuid)
        {
            string value = view.Database.GetCacheValue(actionGuid);
            return value;
        }

        private static string CleanMessage(IDictionary<string, object> json)
        {
            const string at = "at";
            StringBuilder sb = new StringBuilder();

            IDictionary<string, object> child = json;
            IDictionary<string, object> grandChild = null;
            
            bool eoj = false;
            while (!eoj)
            {
                bool nodeHasLocation = false;
                bool nodeHasChildren = false;
                object value = null;
                string key2 = string.Empty;
                object message2 = null;
                foreach (string key in child.Keys)
                {
                    value = child[key];

                    if (key == at && value is IDictionary<string, object>)
                    {
                        string location = GetLocation((IDictionary<string, object>)value);
                        sb.Insert(0, location);
                        nodeHasLocation = true;
                    }
                    //else if (value is IDictionary<string, object>)
                    else if (value is IDictionary<string, object> && ((IDictionary<string, object>)value).ContainsKey(at))
                    {
                        grandChild = (IDictionary<string, object>)value;
                        nodeHasChildren = true;
                        key2 = key;
                        message2 = value;
                    }
                    else
                    {
                        key2 = key;
                        message2 = value;
                    }
                }
                if (nodeHasChildren)
                {
                    child = grandChild;
                }
                if (!nodeHasLocation || !nodeHasChildren)
                {
                    string message = GetMessage(key2, message2);
                    sb.Insert(0, message);
                        
                    eoj = true;
                }
            }

            return sb.ToString().TrimStart("CriticalError:".ToCharArray());
        }

        private static string GetLocation(IDictionary<string, object> dictionary)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("\n\tat ");
            sb.Append("(");
            if (dictionary.ContainsKey("object"))
            {
                string objectName = dictionary["object"].ToString();
                sb.Append(objectName);
                sb.Append("/");
            }
            if (dictionary.ContainsKey("action"))
            {
                string actionName = dictionary["action"].ToString();
                sb.Append(actionName);
            }
            if (dictionary.ContainsKey("line"))
            {
                sb.Append(":");
                string line = dictionary["line"].ToString();
                sb.Append(line);
            }
            //if (dictionary.ContainsKey("column"))
            //{
            //    sb.Append(":");
            //    string column = dictionary["column"].ToString();
            //    sb.Append(column);
            //}
            sb.Append(")"); 
            
            return sb.ToString();
        }

        private static string GetMessage(string key, object value)
        {
            if (key == "417")
                key = "CriticalError";
            if (value is string)
                return key + ":" + value.ToString();

            return key + ":" + jss.Serialize(value);
        }

        private static IDictionary<string, object> CleanJson(string message)
        {

            var theJavaScriptSerializer = new System.Web.Script.Serialization.JavaScriptSerializer();
            theJavaScriptSerializer.MaxJsonLength = int.MaxValue;

            IDictionary<string, object> d2 = new Dictionary<string, object>();
            IDictionary<string, object> d = theJavaScriptSerializer.Deserialize<Dictionary<string, object>>(message);
            IDictionary<string, object> d2Child = d2;

            const string at = "at";

            bool eoj = false;
            while (!eoj)
            {
                eoj = true;
                bool hasLine = false;
                string errorKey = null;
                foreach (string key in d.Keys)
                {
                    if (key == at)
                    {
                        if (((IDictionary<string, object>)d[key]).ContainsKey("line"))
                        {
                            hasLine = true;
                        }
                    }
                    else
                    {
                        errorKey = key;

                        //if (d[key] is IDictionary<string, object>)
                        if (d[key] is IDictionary<string, object> && ((IDictionary<string, object>)d[key]).ContainsKey(at))
                        {
                            eoj = false;
                        }
                    }
                }
                if (hasLine)
                {
                    foreach (string key in d.Keys)
                    {
                        if (key == at || eoj)
                        {
                            d2Child.Add(key, d[key]);
                        }
                    }
                    if (!eoj)
                    {
                        d2Child.Add(errorKey, new Dictionary<string, object>());
                        d2Child = (IDictionary<string, object>)d2Child[errorKey];
                    }
                }
                else if (eoj && errorKey != null)
                {
                    d2Child.Add(errorKey, d[errorKey]);
                }
                if (!eoj)
                {
                    d = (IDictionary<string, object>)d[errorKey];
                }

            }

            return d2;



        }

        public static bool IsCrud(System.Net.WebRequest request)
        {
            string pathAndQuery = System.Web.HttpContext.Current.Request.Url.PathAndQuery.ToLower();
            if (System.Web.HttpContext.Current == null || System.Web.HttpContext.Current.Request.Headers["Authorization"] == null || (pathAndQuery.Contains("1/user") || pathAndQuery.Contains("/backandusers")))
                return false;
            if (System.Web.HttpContext.Current.Items.Contains(Database.SignupInProcess))
                return false;
            HashSet<string> methods = new HashSet<string>(StringComparer.OrdinalIgnoreCase) { "POST", "PUT", "DELETE" };
            string route = "/objects/";
            string actionRoute = "/objects/action/";

            return IsBackand(request) && request.RequestUri.AbsoluteUri.Contains(route) && !request.RequestUri.AbsoluteUri.Contains(actionRoute) && methods.Contains(request.Method.ToUpper());
        }

        private static bool IsBackandRequest
        {
            get
            {
                string localhost = "localhost";
                string backand = "backand";

                return System.Web.HttpContext.Current.Request.Url.AbsoluteUri.ToLower().Contains(localhost) || System.Web.HttpContext.Current.Request.Url.AbsoluteUri.ToLower().Contains(backand);
            }
        }

        private static bool IsBasic
        {
            get
            {
                return System.Web.HttpContext.Current.Request.Headers != null && System.Web.HttpContext.Current.Request.Headers["authorization"] != null && System.Web.HttpContext.Current.Request.Headers["authorization"].ToLower().StartsWith("basic");
            }
        }

        private static bool HasAppId
        {
            get
            {
                return System.Web.HttpContext.Current.Request.Headers != null && System.Web.HttpContext.Current.Request.Headers["AppId"] != null;
            }
        }

        private static bool IsBackand(System.Net.WebRequest request)
        {
            string localhost = "localhost";
            string backand = "backand";

            return request.RequestUri.AbsoluteUri.Contains(localhost) || request.RequestUri.AbsoluteUri.ToLower().Contains(backand);
        }

        public static string PerformCrud(System.Net.WebRequest request, string json)
        {
            //, { "parameters", parameters }, { "view", view }, { "values", values }, { "prevRow", prevRow }, { "pk", pk }
            Dictionary<string, object> executeArgs = (Dictionary<string, object>)GetCacheInCurrentRequest("js" + GetCacheInCurrentRequest(GuidKey));
            if (executeArgs == null)
            {
                throw new DuradosException("Not performing CRUD");
            }
            if (executeArgs.ContainsKey("view"))
                executeArgs["view"] = GetViewName(request);
            else
                executeArgs.Add("view", GetViewName(request));
            if (executeArgs.ContainsKey("pk"))
                executeArgs["pk"] = GetPk(request);
            else
                executeArgs.Add("pk", GetPk(request));
            if (executeArgs.ContainsKey("parameters"))
                executeArgs["parameters"] = GetParameters(request);
            else
                executeArgs.Add("parameters", GetParameters(request));


            return PerformCrud(request, json, executeArgs);
        }

        private static string GetViewName(System.Net.WebRequest request)
        {
            if (request.Method == "POST")
            {
                return request.RequestUri.Segments.LastOrDefault();
            }
            else if (request.Method == "PUT" || request.Method == "DELETE")
            {
                return request.RequestUri.Segments[request.RequestUri.Segments.Length - 2];
            }
            else
            {
                return null;
            }
        }

        private static string GetPk(System.Net.WebRequest request)
        {
            if (request.Method == "POST")
            {
                return null;
            }
            else if (request.Method == "PUT" || request.Method == "DELETE")
            {
                return request.RequestUri.Segments.LastOrDefault(); 
            }
            else
            {
                return null;
            }
        }

        private static string GetParameters(System.Net.WebRequest request)
        {
            System.Collections.Specialized.NameValueCollection qs = System.Web.HttpUtility.ParseQueryString(request.RequestUri.Query);
            return qs["Parameters"];
        }

        private static string PerformCrud(System.Net.WebRequest request, string json, Dictionary<string, object> executeArgs)
        {
            
            return PerformCrud(request, json, executeArgs, executeArgs["controller"] as Durados.Data.IData, (string)executeArgs["actionName"]);
        }

        private static string PerformCrud(System.Net.WebRequest request, string json, Dictionary<string, object> executeArgs, Durados.Data.IData controller, string actionName)
        {
            if (IsEndlessLoop(request, json))
            {
                throw new JavaScriptException(string.Format("The request '{0}' is repeating calling itself. Please change the action {1}", request.RequestUri.AbsoluteUri, actionName), new StackOverflowException());
            }
            if (controller == null || controller.DataHandler == null)
            {
                throw new DuradosException("Not performing CRUD");
            }
            return controller.DataHandler.PerformCrud(request, json, executeArgs);
        }

        private static bool IsEndlessLoop(System.Net.WebRequest request, string json)
        {

            string key = "endless" + GetActionName();
            if (GetCacheInCurrentRequest(key) == null)
                SetCacheInCurrentRequest(key, new HashSet<string>());
            HashSet<string> requests = (HashSet<string>)GetCacheInCurrentRequest(key);

            if (requests.Contains(request.RequestUri.AbsoluteUri + request.Method + json))
            {
                if (GetDataAction() != TriggerDataAction.OnDemand)
                {
                    return true;
                }
            }
            requests.Add(request.RequestUri.AbsoluteUri + request.Method + json);
            return false;

            
        }

        private static bool IsAction(System.Net.WebRequest request)
        {
            return request.RequestUri.AbsoluteUri.Contains("objects/action") || request.RequestUri.AbsoluteUri.Contains("table/action") || request.RequestUri.AbsoluteUri.Contains("table/rule");
        }

        private static string GetActionName()
        {
            object args = GetCacheInCurrentRequest("js" + GetCacheInCurrentRequest(GuidKey));
            if (args == null)
                return string.Empty;

            Dictionary<string, object> argsDic = (Dictionary<string, object>)args;
            if (argsDic.ContainsKey("actionName"))
                return argsDic["actionName"].ToString();

            return string.Empty;
        }

        private static Durados.TriggerDataAction GetDataAction()
        {
            object args = GetCacheInCurrentRequest("js" + GetCacheInCurrentRequest(GuidKey));
            if (args == null)
                return Durados.TriggerDataAction.OnDemand;

            Dictionary<string, object> argsDic = (Dictionary<string, object>)args;
            if (argsDic.ContainsKey("dataAction"))
                return (Durados.TriggerDataAction)argsDic["dataAction"];

            return Durados.TriggerDataAction.OnDemand;
        }

        public virtual void Execute(object controller, Dictionary<string, Parameter> parameters, View view, Dictionary<string, object> values, DataRow prevRow, string pk, string connectionString, int currentUsetId, string currentUserRole, IDbCommand command, IDbCommand sysCommand, string actionName, Durados.TriggerDataAction dataAction)
        {
            if (pk != null && (prevRow == null || dataAction == TriggerDataAction.AfterCreate || dataAction == TriggerDataAction.AfterEdit || dataAction == TriggerDataAction.AfterCreateBeforeCommit || dataAction == TriggerDataAction.AfterEditBeforeCommit) && controller is Durados.Data.IData)
            {
                try
                {
                    if (((Durados.Data.IData)controller).DataHandler != null)
                        prevRow = ((Durados.Data.IData)controller).DataHandler.GetDataRow(view, pk, command);
                }
                catch { }
            }

            var guid = Guid.NewGuid();
            //if (System.Web.HttpContext.Current != null && System.Web.HttpContext.Current.Request.QueryString[GuidKey] != null)
            //{
            //    guid = new Guid(System.Web.HttpContext.Current.Request.QueryString[GuidKey]);
            //}
            SetCacheInCurrentRequest("js" + guid, new Dictionary<string, object>() { { "controller", controller }, { "connectionString", connectionString }, { "currentUsetId", currentUsetId }, { "currentUserRole", currentUserRole }, { "command", command }, { "sysCommand", sysCommand }, { "actionName", actionName }, { "dataAction", dataAction } });
            

            SetCacheInCurrentRequest(ConnectionStringKey, view.Database.SysDbConnectionString);
            SetCacheInCurrentRequest(GuidKey, guid);
            
            if (!parameters.ContainsKey("code"))
                throw new DuradosException("code was not supplied");

            string code = parameters["code"].Value.Replace(Engine.AsToken(values), ((Durados.Workflow.INotifier)controller).GetTableViewer(), view);

            string currentUsername = view.Database.GetCurrentUsername();

            try
            {
                code = code.Replace(Durados.Database.UserPlaceHolder, currentUsetId.ToString(), false).Replace(Durados.Database.SysUserPlaceHolder.AsToken(), currentUsetId.ToString(), false)
                      .Replace(Durados.Database.UsernamePlaceHolder, currentUsername, false).Replace(Durados.Database.SysUsernamePlaceHolder.AsToken(), currentUsername)
                      .Replace(Durados.Database.RolePlaceHolder, currentUserRole, false).Replace(Durados.Database.SysRolePlaceHolder.AsToken(), currentUserRole)
                      .ReplaceConfig(view);
            }
            catch { }

            Dictionary<string, object> clientParameters = new Dictionary<string, object>();
            Dictionary<string, object> newRow = new Dictionary<string, object>();
            Dictionary<string, object> oldRow = new Dictionary<string, object>();
            Dictionary<string, object> userProfile = new Dictionary<string, object>();

            bool debug = false;
            if (IsDebug())
            {
                debug = true;
            }
                        
            if (values != null)
            {
                foreach (string key in values.Keys)
                {
                    if (key == "$$debug$$" || key == "$$debug$$".AsToken())
                    {
                        debug = true;
                    }
                    else
                    {
                        string keyWithoutToken = key.TrimStart("{{".ToCharArray()).TrimEnd("}}".ToCharArray());

                        if (key.StartsWith("{{"))
                        {
                            if (!clientParameters.ContainsKey(keyWithoutToken))
                                clientParameters.Add(keyWithoutToken, values[key]);
                        }

                        if (view.GetFieldsByJsonName(keyWithoutToken) == null || view.GetFieldsByJsonName(keyWithoutToken).Length == 0)
                        {
                            if (view.Fields.ContainsKey(keyWithoutToken))
                            {
                                string jsonName = view.Fields[keyWithoutToken].JsonName;

                                if (!newRow.ContainsKey(jsonName))
                                    newRow.Add(jsonName, values[key]);
                            }
                            else
                            {
                                if (!clientParameters.ContainsKey(keyWithoutToken))
                                    clientParameters.Add(keyWithoutToken, values[key]);
                            }
                            
                        }
                        else
                        {
                            if (!newRow.ContainsKey(keyWithoutToken))
                                newRow.Add(keyWithoutToken, values[key]);
                        }
                    }
                }
            }
            SetCacheInCurrentRequest(Debug, debug);

            if (prevRow != null)
            {
                foreach (Field field in view.Fields.Values)
                {
                    if (!oldRow.ContainsKey(field.JsonName))
                    {
                        if (field.FieldType == FieldType.Column && field.IsDate)
                        {
                            oldRow.Add(field.JsonName, prevRow[((ColumnField)field).DataColumn.ColumnName]);
                        }
                        else
                        {
                            oldRow.Add(field.JsonName, field.GetValue(prevRow));
                        }
                    }
                }
                //var shallowRow = view.RowToShallowDictionary(prevRow, pk);
                //foreach (Field field in view.Fields.Values)
                //{
                //    if (oldRow.ContainsKey(field.JsonName) && shallowRow.ContainsKey(field.JsonName))
                //    {
                //        if (field.FieldType == FieldType.Column && (field.IsBoolean || field.IsPoint || field.IsNumeric))
                //        {
                //            oldRow[field.JsonName] = shallowRow[field.JsonName];
                //        }
                //    }
                //}
            }

            userProfile.Add("username", view.Database.GetCurrentUsername());
            userProfile.Add("role", currentUserRole);
            userProfile.Add("app", view.Database.GetCurrentAppName());
            userProfile.Add("userId", view.Database.GetCurrentUserId());
            userProfile.Add("token", GetUserProfileAuthToken(view));
            userProfile.Add("anonymousToken", view.Database.GetAnonymousToken().ToString());
            userProfile.Add("info", GetUserProfileInfo(view));


            if (!clientParameters.ContainsKey(FILEDATA))
            {
                int parametersLimit = (int)view.Database.GetLimit(Limits.ActionParametersKbSize);

                HandleParametersSizeLimit(parametersLimit, clientParameters);
                userProfile.Add("request", GetRequest());
            }



            var CONSTS = new Dictionary<string, object>() { { "apiUrl", System.Web.HttpContext.Current.Request.Url.Scheme + "://" + System.Web.HttpContext.Current.Request.Url.Host + ":" + System.Web.HttpContext.Current.Request.Url.Port + System.Web.HttpContext.Current.Request.ApplicationPath }, { "actionGuid", System.Web.HttpContext.Current.Request.QueryString[GuidKey] ?? (System.Web.HttpContext.Current.Items[GuidKey] ?? guid.ToString()) } };

            //Newtonsoft.Json.JsonConvert.SerializeObject
            
            var theJavaScriptSerializer = new System.Web.Script.Serialization.JavaScriptSerializer();
            theJavaScriptSerializer.MaxJsonLength = int.MaxValue;

            var parser = new Jint.Native.Json.JsonParser(call2);
            //var userInput = parser.Parse(theJavaScriptSerializer.Serialize(newRow));
            //Object clientParametersToSend = null;
            //if (!clientParameters.ContainsKey("filedata"))
            //{
            //    clientParametersToSend = parser.Parse(theJavaScriptSerializer.Serialize(clientParameters));
            //}
            //else
            //{
            //    System.Web.HttpContext.Current.Items["file_stream"] = clientParameters["filedata"];
            //    clientParameters["filedata"] = "file_stream";
            //    clientParametersToSend = clientParameters;
            //}
            //var dbRow = parser.Parse(theJavaScriptSerializer.Serialize(oldRow));
            //var userProfile2 = parser.Parse(theJavaScriptSerializer.Serialize(userProfile));
            //var CONSTS2 = parser.Parse(theJavaScriptSerializer.Serialize(CONSTS));
            
            
            //var Config = view.Database.GetConfigDictionary();
            //var Config2 = parser.Parse(theJavaScriptSerializer.Serialize(Config));
            var userInput = parser.Parse(Newtonsoft.Json.JsonConvert.SerializeObject(newRow));
            Object clientParametersToSend = null;

            bool upload = false;

            if (!clientParameters.ContainsKey(FILEDATA))
            {
                clientParametersToSend = parser.Parse(Newtonsoft.Json.JsonConvert.SerializeObject(clientParameters));
            }
            else
            {
                upload = true;
                System.Web.HttpContext.Current.Items["file_stream"] = clientParameters[FILEDATA];
                clientParameters[FILEDATA] = "file_stream";
                clientParametersToSend = clientParameters;
            }

            if (clientParameters.ContainsKey(FILEDATA) || clientParameters.ContainsKey(FILENAME))
            {
                System.Web.HttpContext.Current.Items[StorageAccountsKey] = view.Database.CloudStorages;
            }

            var dbRow = parser.Parse(Newtonsoft.Json.JsonConvert.SerializeObject(oldRow));
            var userProfile2 = parser.Parse(Newtonsoft.Json.JsonConvert.SerializeObject(userProfile));
            var CONSTS2 = parser.Parse(Newtonsoft.Json.JsonConvert.SerializeObject(CONSTS));


            var Config = view.Database.GetConfigDictionary();
            var Config2 = parser.Parse(Newtonsoft.Json.JsonConvert.SerializeObject(Config));

            var Environment = view.Database.GetEnvironmentDictionary();
            var Environment2 = parser.Parse(Newtonsoft.Json.JsonConvert.SerializeObject(Environment));


            Limits limit = Limits.ActionTimeMSec;
            if (upload)
            {
                limit = Limits.UploadTimeMSec;
            }

            object actionTimeMSecObject = view.Database.GetLimit(limit);
            int actionTimeMSec = -1;

            if (!Int32.TryParse(actionTimeMSecObject.ToString(), out actionTimeMSec))
            {
                throw new DuradosException("actionTimeMSecLimit in web.config is not numeric or too long");
            }

            TimeSpan? timeoutInterval = new TimeSpan(0, 0, 0, 0, actionTimeMSec);
            if (actionTimeMSec == 0)
                timeoutInterval = null;

            string actionId = Guid.NewGuid().ToString();

            string startMessage = theJavaScriptSerializer.Serialize(new { objectName = view.JsonName, actionName = actionName, id = actionId, @event = "started", time = GetSequence(view), data = new { userInput = newRow, dbRow = oldRow, parameters = clientParameters, userProfile = userProfile } });

            Backand.Logger.Log(startMessage, 502);


            var call = new Jint.Engine(cfg => cfg.AllowClr(typeof(Backand.XMLHttpRequest).Assembly), timeoutInterval, 184, new ActionPath() { @object = view.JsonName, action = actionName });

            try
            {
                call.SetValue("userInput", userInput)
                .SetValue("btoa", new btoaHandler(Backand.Convert.btoa))
                .SetValue("dbRow", dbRow)
                .SetValue("parameters", clientParametersToSend)
                .SetValue("userProfile", userProfile2)
                .SetValue("CONSTS", CONSTS2)
                .SetValue("Config", Config2)
                .SetValue("Environment", Environment2)
                .Execute(GetXhrWrapper() + code + "; function call(){return backandCallback(userInput, dbRow, parameters, userProfile);}");
            }
            catch (TimeoutException exception)
            {
                Handle503(theJavaScriptSerializer, view.JsonName, actionName, actionId, exception.Message, view);
                string errorMessage = "Timeout: The operation took longer than " + actionTimeMSec + " milliseconds limit. at (" + view.JsonName + "/" + actionName + ")";
                if (!IsSubAction())
                {
                    Backand.Logger.Log(exception.Message, 501);
                    if (IsDebug())
                        throw new MainActionInDebugJavaScriptException(errorMessage, exception);
                    else
                        throw new MainActionJavaScriptException(errorMessage, exception);
                }
                else
                {
                    throw new SubActionJavaScriptException(errorMessage, exception);
                }
            }
            catch (Exception exception)
            {
                Handle503(theJavaScriptSerializer, view.JsonName, actionName, actionId, exception.Message, view);
                string errorMessage = "Syntax error: " + HandleLineCodes(exception.Message, view.JsonName, actionName, view, IsDebug());
                if (!IsSubAction())
                {
                    Backand.Logger.Log(exception.Message, 501);
                    if (IsDebug())
                        throw new MainActionInDebugJavaScriptException(errorMessage, exception);
                    else
                        throw new MainActionJavaScriptException(errorMessage, exception);
                }
                else
                {
                    throw new SubActionJavaScriptException(errorMessage, exception);
                }

            }
            object r = null;
            try
            {
                var r2 = call.GetValue("call").Invoke();
                if (!r2.IsNull())
                    r = r2.ToObject();

                string endMessage = null;
                try
                {
                    endMessage = theJavaScriptSerializer.Serialize(new { objectName = view.JsonName, actionName = actionName, id = actionId, @event = "ended", time = GetSequence(view), data = r });
                }
                catch (Exception exception)
                {
                    if (exception.Message.StartsWith("A circular reference was detected while serializing an object"))
                    {
                        object oNull = null;
                        endMessage = theJavaScriptSerializer.Serialize(new { objectName = view.JsonName, actionName = actionName, id = actionId, @event = "ended", time = GetSequence(view), data = oNull });

                    }
                    else
                    {
                        endMessage = "Failed to serialize response";
                        if (!IsSubAction())
                        {
                            Backand.Logger.Log(endMessage, 501);
                            //if (IsDebug())
                            //{
                            //    values[ReturnedValueKey] = message;
                            //    return;
                            //}
                            //else
                            if (IsDebug())
                                throw new MainActionInDebugJavaScriptException(endMessage, exception);
                            else
                                throw new MainActionJavaScriptException(endMessage, exception);
                        }
                        else
                        {
                            throw new SubActionJavaScriptException(endMessage, exception);
                        }
                    }
                }

                Backand.Logger.Log(endMessage, 503);

            }
            catch (TimeoutException exception)
            {
                string message = "Timeout: The operation took longer than " + actionTimeMSec + " milliseconds limit. at (" + view.JsonName + "/" + actionName + ")";
                Handle503(theJavaScriptSerializer, view.JsonName, actionName, actionId, message, view);
                if (!IsSubAction())
                {
                    Backand.Logger.Log(message, 501);
                    if (IsDebug())
                        throw new MainActionInDebugJavaScriptException(message, exception);
                    else
                        throw new MainActionJavaScriptException(message, exception);
                }
                else
                {
                    throw new SubActionJavaScriptException(message, exception);
                }
            }
            catch (Exception exception)
            {
                string message = (exception.InnerException == null) ? exception.Message : exception.InnerException.Message;
                string trace = message;
                if (exception is Jint.Runtime.JavaScriptException)
                {
                    trace = ((Jint.Runtime.JavaScriptException)exception).GetTrace();
                }
                trace = HandleLineCodes(trace, view.JsonName, actionName, view, IsDebug());
                Handle503(theJavaScriptSerializer, view.JsonName, actionName, actionId, trace, view);
                if (!IsSubAction())
                {
                    IMainActionJavaScriptException mainActionJavaScriptException;
                    Backand.Logger.Log(trace, 501);
                    if (IsDebug())
                        mainActionJavaScriptException = new MainActionInDebugJavaScriptException(message, exception);
                    else
                        mainActionJavaScriptException = new MainActionJavaScriptException(message, exception);

                    mainActionJavaScriptException.JintTrace = trace;
                    throw (Exception)mainActionJavaScriptException;
                }
                else
                {
                    throw new SubActionJavaScriptException(message, exception);
                }
            }

            var v = call.GetValue("userInput").ToObject();

            if (v != null && v is System.Dynamic.ExpandoObject)
            {
                IDictionary<string, object> newValues = v as IDictionary<string, object>;
                foreach (string key in newValues.Keys)
                {
                    if (values.ContainsKey(key))
                    {
                        object val = newValues[key];
                        Field[] fields = view.GetFieldsByJsonName(key);
                        val = DateConversion(view, val, fields);
                        values[key] = val;
                    }
                    else
                    {
                        Field[] fields = view.GetFieldsByJsonName(key);
                        if (fields.Length > 0)
                        {
                            string fieldName = fields[0].Name;
                            object val = newValues[key];
                            val = DateConversion(view, val, fields);
                            if (values.ContainsKey(fieldName))
                            {
                                values[fieldName] = val;
                            }
                            else
                            {
                                values.Add(fieldName, val);
                            }
                        }
                        else
                        {
                            values.Add(key, newValues[key]);
                        }
                    }
                }
            }

            if (r != null && values != null && dataAction == TriggerDataAction.OnDemand)
            {
                if (!values.ContainsKey(ReturnedValueKey))
                    values.Add(ReturnedValueKey, r);
                else
                    values[ReturnedValueKey] = r;
            }
        }

        private object GetUserProfileInfo(View view)
        {
            if (System.Web.HttpContext.Current.Items.Contains(Durados.Database.TokenInfo))
            {
                return System.Web.HttpContext.Current.Items[Durados.Database.TokenInfo];
            }
            return null;
        }

        private bool IsSubAction()
        {
            return System.Web.HttpContext.Current != null && System.Web.HttpContext.Current.Request.Headers[Durados.Workflow.JavaScript.ActionHeaderKey] != null;
        }

        private void Handle503(System.Web.Script.Serialization.JavaScriptSerializer theJavaScriptSerializer, string objectName, string actionName, string actionId, string message, View view)
        {
            string endMessage = theJavaScriptSerializer.Serialize(new { objectName = objectName, actionName = actionName, id = actionId, @event = "ended", time = GetSequence(view), data = new { error = message } });

            Backand.Logger.Log(endMessage, 503);
        }

        private void HandleParametersSizeLimit(object limit, Dictionary<string, object> clientParameters)
        {
            int sizeLimit = Convert.ToInt32(limit);
            int parametersSize = GetSize();
            if (parametersSize > sizeLimit)
            {
                throw new OverTheActionParametersSizeLimitException(sizeLimit, parametersSize);
            }
        }

        private int GetSize()
        {

            string s = (string)System.Web.HttpContext.Current.Items["body"];
            if (s == null)
                return 0;
            int size = System.Text.ASCIIEncoding.Unicode.GetByteCount(s);

            return size / 1000;
        }

        private int GetSize(Dictionary<string, object> clientParameters)
        {
            int size = 0;
            foreach (string key in clientParameters.Keys)
            {
                if (key != FILEDATA)
                {
                    object parameter = clientParameters[key];
                    if (parameter is string)
                    {
                        size += System.Text.ASCIIEncoding.Unicode.GetByteCount((string)parameter);
                    }
                }
            }

            return size / 1000;
        }

        private static string GetUserProfileAuthToken(View view)
        {
            if (System.Web.HttpContext.Current.Request.Url.PathAndQuery.ToLower().Contains("1/user/signup") || System.Web.HttpContext.Current.Request.Url.PathAndQuery.ToLower().Contains("1/user/requestResetPassword".ToLower()))
                return view.Database.GetAuthorization();
            else if (IsBackandRequest && IsBasic && HasAppId)
                return view.Database.GetAuthorization();
            else
                return System.Web.HttpContext.Current.Request.Headers["Authorization"] ?? view.Database.GetAuthorization();
        }

        private static Dictionary<string, object> GetRequest()
        {
            Dictionary<string, object> req =  new Dictionary<string, object>() { { "userIP", GetUserIP() }, { "method", System.Web.HttpContext.Current.Request.HttpMethod }, { "headers", GetHeaders(System.Web.HttpContext.Current.Request.Headers) } };

            req.Add("body", GetRequestBody());
            
            return req;
        }

        static Jint.Engine call2 = new Jint.Engine(cfg => cfg.AllowClr(typeof(Backand.XMLHttpRequest).Assembly));
        static Jint.Native.Json.JsonParser parser2 = new Jint.Native.Json.JsonParser(call2);
            
        public static object GetRequestBody()
        {
            object body = null;
            if (System.Web.HttpContext.Current.Items.Contains("body"))
            {
                string s = (string)System.Web.HttpContext.Current.Items["body"];
                //string json = System.Web.HttpUtility.UrlDecode(s);

                try
                {
                    body = parser2.Parse(s);
                    
                }
                catch { }
            }
            return body;
        }

        public static object GetJintObject(object o)
        {
            return parser2.Parse(Newtonsoft.Json.JsonConvert.SerializeObject(o));
        }

        private static object DateConversion(View view, object val, Field[] fields)
        {
            if (fields[0].DataType == DataType.DateTime)
            {
                if (val != null && !(val is DateTime))
                {
                    if (val.ToString().StartsWith("/Date("))
                    {
                        try
                        {
                            long l = Convert.ToInt64(val.ToString().Replace("/Date(", "").Replace(")/", ""));
                            view.Database.Logger.Log("!date", "", "", null, -14, l.ToString());
                            val = new DateTime(1970, 1, 1).AddTicks(l * 10000);
                        }
                        catch { }
                    }
                }
            }
            return val;
        }

        public static Dictionary<string, object> GetHeaders(System.Collections.Specialized.NameValueCollection headers)
        {
            Dictionary<string, object> dic = new Dictionary<string, object>();

            HashSet<string> hash = new HashSet<string>() { "Cache-Control", "Connection", "Content-Length", "Content-Type", "Accept", "Accept-Encoding", "Accept-Language", "Host", "User-Agent", "Origin", "Postman-Token" };

            foreach (string name in headers.AllKeys)
            {
                if (!hash.Contains(name))
                {
                    dic.Add(name, headers[name]);
                }
            }

            return dic;
        }

        public static string GetUserIP()
        {
            var ip = (System.Web.HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"] != null
                      && System.Web.HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"] != "")
                     ? System.Web.HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"]
                     : System.Web.HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
            if (ip.Contains(","))
                ip = ip.Split(',').First().Trim();
            return ip;
        }
        
        private static int GetSequence(View view)
        {
            if (!IsDebug())
                return 0;
            const string Sequense = "Sequense";
            const int TenMinutes = 1000 * 60 * 10;

            Guid requestGuid = (Guid)Durados.Workflow.JavaScript.GetCacheInCurrentRequest(Durados.Workflow.JavaScript.GuidKey);
            string value = view.Database.GetCacheValue(Sequense + requestGuid);
            int seq = 0;
            if (value != null)
            {
                seq = System.Convert.ToInt32(value);
            }
            seq = seq + 1;

            view.Database.SetCacheValue(Sequense + requestGuid, seq.ToString(), TenMinutes);

            return seq;
        }

        
    }

    public class JavaScriptException :DuradosException
    {
        public JavaScriptException(string message)
            : base(message)
        {

        }

        public JavaScriptException(string message, Exception innerException)
            : base(message, innerException)
        {

        }
    }

    public class ActionPath : Jint.IPath
    {
        public string @object;
        public string action;

        public override string ToString()
        {
            return "\"object\"" + ":" + "\"" + @object + "\"" + "," + "\"action\"" + ":" + "\"" + action + "\"";
        }
    }

}
