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
        public static void SetCacheInCurrentRequest(string key, object value)
        {
            if (key == Durados.Workflow.JavaScript.GuidKey && System.Web.HttpContext.Current.Request.QueryString[Durados.Workflow.JavaScript.GuidKey] != null)
            {
                value = Guid.Parse(System.Web.HttpContext.Current.Request.QueryString[Durados.Workflow.JavaScript.GuidKey]);
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

        private string HandleLineCodes(string message, string objectName, string actionName)
        {
            message += "\n\t at " + objectName + " - " + actionName;
            
            string[] segments = message.Split(new string[] { ":" }, StringSplitOptions.None);
            
            foreach(string segment in segments)
            {
                if (segment.Contains("Line"))
                {
                    string[] subsegments = segment.Split(new string[] { " " }, StringSplitOptions.None);
                    foreach (string subsegment in subsegments)
                    {
                        int number = -1;
                        if (int.TryParse(subsegment, out number))
                        {
                            int adjustedNumber = number - 160;
                            return message.Replace(number.ToString(), adjustedNumber.ToString());
                            
                        }
                    }
                }
            }
                
            return message;

            // "The follwoing action: "aaa" failed to perform: Failed to load the javascript code: Line 166: Unexpected token }"
        }

        public static bool IsCrud(System.Net.WebRequest request)
        {
            if (System.Web.HttpContext.Current == null || System.Web.HttpContext.Current.Request.Headers["Authorization"] == null || (System.Web.HttpContext.Current.Request.Url.PathAndQuery.ToLower().Contains("1/user/signup")))
                return false;
            HashSet<string> methods = new HashSet<string>() { "POST", "PUT", "DELETE" };
            string route = "/objects/";

            return IsBackand(request) && request.RequestUri.AbsoluteUri.Contains(route) & methods.Contains(request.Method.ToUpper());
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
            return PerformCrud(request, json, executeArgs, (Durados.Data.IData)executeArgs["controller"], (string)executeArgs["actionName"]);
        }

        private static string PerformCrud(System.Net.WebRequest request, string json, Dictionary<string, object> executeArgs, Durados.Data.IData controller, string actionName)
        {
            if (IsEndlessLoop(request, json))
            {
                throw new JavaScriptException(string.Format("The request '{0}' is repeating calling itself. Please change the action {1}", request.RequestUri.AbsoluteUri, actionName), new StackOverflowException());
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
                return true;
            }
            requests.Add(request.RequestUri.AbsoluteUri + request.Method + json);
            return false;

            
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

        public virtual void Execute(object controller, Dictionary<string, Parameter> parameters, View view, Dictionary<string, object> values, DataRow prevRow, string pk, string connectionString, int currentUsetId, string currentUserRole, IDbCommand command, IDbCommand sysCommand, string actionName)
        {
            if (pk != null && prevRow == null && controller is Durados.Data.IData)
            {
                try
                {
                    if (((Durados.Data.IData)controller).DataHandler != null)
                        prevRow = ((Durados.Data.IData)controller).DataHandler.GetDataRow(view, pk, command);
                }
                catch { }
            }

            var guid = Guid.NewGuid();
            SetCacheInCurrentRequest("js" + guid, new Dictionary<string, object>() { { "controller", controller }, { "connectionString", connectionString }, { "currentUsetId", currentUsetId }, { "currentUserRole", currentUserRole }, { "command", command }, { "sysCommand", sysCommand }, { "actionName", actionName } });
            

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
            }

            userProfile.Add("username", view.Database.GetCurrentUsername());
            userProfile.Add("role", currentUserRole);
            userProfile.Add("app", view.Database.GetCurrentAppName());
            userProfile.Add("userId", view.Database.GetCurrentUserId());
            userProfile.Add("token", GetUserProfileAuthToken(view));

            
            if (!clientParameters.ContainsKey(FILEDATA))
            {
            HandleParametersSizeLimit(view.Database.GetLimit(Limits.ActionParametersKbSize), clientParameters);
    userProfile.Add("request", GetRequest());
            }

            var call = new Jint.Engine(cfg => cfg.AllowClr(typeof(Backand.XMLHttpRequest).Assembly));

            
            var CONSTS = new Dictionary<string, object>() { { "apiUrl", System.Web.HttpContext.Current.Request.Url.Scheme + "://" + System.Web.HttpContext.Current.Request.Url.Host + ":" + System.Web.HttpContext.Current.Request.Url.Port + System.Web.HttpContext.Current.Request.ApplicationPath } };

            //Newtonsoft.Json.JsonConvert.SerializeObject
            
            var theJavaScriptSerializer = new System.Web.Script.Serialization.JavaScriptSerializer();
            var parser = new Jint.Native.Json.JsonParser(call);
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

            

            if (!clientParameters.ContainsKey(FILEDATA))
            {
                clientParametersToSend = parser.Parse(Newtonsoft.Json.JsonConvert.SerializeObject(clientParameters));
            }
            else
            {
                System.Web.HttpContext.Current.Items["file_stream"] = clientParameters[FILEDATA];
                clientParameters[FILEDATA] = "file_stream";
                clientParametersToSend = clientParameters;
            }
            var dbRow = parser.Parse(Newtonsoft.Json.JsonConvert.SerializeObject(oldRow));
            var userProfile2 = parser.Parse(Newtonsoft.Json.JsonConvert.SerializeObject(userProfile));
            var CONSTS2 = parser.Parse(Newtonsoft.Json.JsonConvert.SerializeObject(CONSTS));


            var Config = view.Database.GetConfigDictionary();
            var Config2 = parser.Parse(Newtonsoft.Json.JsonConvert.SerializeObject(Config));

            try
            {
                call.SetValue("userInput", userInput)
                .SetValue("btoa", new btoaHandler(Backand.Convert.btoa))
                .SetValue("dbRow", dbRow)
                .SetValue("parameters", clientParametersToSend)
                .SetValue("userProfile", userProfile2)
                .SetValue("CONSTS", CONSTS2)
                .SetValue("Config", Config2)
                .Execute(GetXhrWrapper() + code + "; function call(){return backandCallback(userInput, dbRow, parameters, userProfile);}");
            }
            catch (Exception exception)
            {
                Backand.Logger.Log(exception.Message, 501);
                throw new DuradosException("Syntax error: " + HandleLineCodes(exception.Message, view.Name, actionName), exception); 
            }
            object r = null;
            try
            {
                var r2 = call.GetValue("call").Invoke();
                if (!r2.IsNull())
                    r = r2.ToObject();
            }
            catch (Exception exception)
            {
                string message = (exception.InnerException == null) ? exception.Message : exception.InnerException.Message;
                message = HandleLineCodes(message, view.Name, actionName);
                Exception e = new DuradosException(message, exception);
                Backand.Logger.Log(e.Message, 501);
                if (IsDebug())
                {
                    values[ReturnedValueKey] = message;
                    return;
                }
                else
                    throw e;
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

            if (r != null && values != null)
            {
                if (!values.ContainsKey(ReturnedValueKey))
                    values.Add(ReturnedValueKey, r);
                else
                    values[ReturnedValueKey] = r;
            }
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
            if (System.Web.HttpContext.Current.Request.Url.PathAndQuery.ToLower().Contains("1/user/signup"))
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

}