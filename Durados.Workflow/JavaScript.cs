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
            if (System.Web.HttpContext.Current.Items.Contains(key))
                System.Web.HttpContext.Current.Items[key] = value;
            else
                System.Web.HttpContext.Current.Items.Add(key, value);

        }

        public static object GetCacheInCurrentRequest(string key)
        {
            if (System.Web.HttpContext.Current.Items.Contains(key))
                return System.Web.HttpContext.Current.Items[key];

            return null;
        }

        public static bool IsDebug()
        {
            return System.Convert.ToBoolean(Durados.Workflow.JavaScript.GetCacheInCurrentRequest(Durados.Workflow.JavaScript.Debug) ?? false);
        }

        private string HandleLineCodes(string message)
        {
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
                            int adjustedNumber = number - 157;
                            return message.Replace(number.ToString(), adjustedNumber.ToString());
                            
                        }
                    }
                }
            }

            return message;

            // "The follwoing action: "aaa" failed to perform: Failed to load the javascript code: Line 166: Unexpected token }"
        }

        public virtual void Execute(object controller, Dictionary<string, Parameter> parameters, View view, Dictionary<string, object> values, DataRow prevRow, string pk, string connectionString, int currentUsetId, string currentUserRole, IDbCommand command)
        {
            SetCacheInCurrentRequest(ConnectionStringKey, view.Database.SysDbConnectionString);
            SetCacheInCurrentRequest(GuidKey, Guid.NewGuid());
            
            if (!parameters.ContainsKey("code"))
                throw new DuradosException("code was not supplied");

            string code = parameters["code"].Value.Replace(Engine.AsToken(values), ((Durados.Workflow.INotifier)controller).GetTableViewer(), view);

            string currentUsername = view.Database.GetCurrentUsername();
            
            code = code.Replace(Durados.Database.UserPlaceHolder, currentUsetId.ToString(), false).Replace(Durados.Database.SysUserPlaceHolder.AsToken(), currentUsetId.ToString(), false)
                  .Replace(Durados.Database.UsernamePlaceHolder, currentUsername, false).Replace(Durados.Database.SysUsernamePlaceHolder.AsToken(), currentUsername)
                  .Replace(Durados.Database.RolePlaceHolder, currentUserRole, false).Replace(Durados.Database.SysRolePlaceHolder.AsToken(), currentUserRole);

            Dictionary<string, object> clientParameters = new Dictionary<string, object>();
            Dictionary<string, object> newRow = new Dictionary<string, object>();
            Dictionary<string, object> oldRow = new Dictionary<string, object>();
            Dictionary<string, object> userProfile = new Dictionary<string, object>();

            //if (System.Web.HttpContext.Current.Request.Files.Count > 0){
            //    clientParameters.Add("__bko_file", System.Web.HttpContext.Current.Request.Files[0].InputStream.)
            //System.Web.HttpContext.Current.Request.Files[0].InputStream
            //}
            bool debug = false;
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
                        oldRow.Add(field.JsonName, field.GetValue(prevRow));
                }
            }

            userProfile.Add("username", view.Database.GetCurrentUsername());
            userProfile.Add("role", currentUserRole);
            userProfile.Add("app", view.Database.GetCurrentAppName());
            userProfile.Add("token", System.Web.HttpContext.Current.Request.Headers["Authorization"]);

            var call = new Jint.Engine(cfg => cfg.AllowClr(typeof(Backand.XMLHttpRequest).Assembly));

            
            var CONSTS = new Dictionary<string, object>() { { "apiUrl", System.Web.HttpContext.Current.Request.Url.Scheme + "://" + System.Web.HttpContext.Current.Request.Url.Host + ":" + System.Web.HttpContext.Current.Request.Url.Port + System.Web.HttpContext.Current.Request.ApplicationPath } };


            var theJavaScriptSerializer = new System.Web.Script.Serialization.JavaScriptSerializer();
            var parser = new Jint.Native.Json.JsonParser(call);
            var userInput = parser.Parse(new System.Web.Script.Serialization.JavaScriptSerializer().Serialize(newRow));
            Object clientParametersToSend = null;
            if (!clientParameters.ContainsKey("filedata"))
            {
                clientParametersToSend = parser.Parse(theJavaScriptSerializer.Serialize(clientParameters));
            }
            else
            {
                clientParametersToSend = clientParameters;
            }
            var dbRow = parser.Parse(new System.Web.Script.Serialization.JavaScriptSerializer().Serialize(oldRow));
            var userProfile2 = parser.Parse(new System.Web.Script.Serialization.JavaScriptSerializer().Serialize(userProfile));
            var CONSTS2 = parser.Parse(new System.Web.Script.Serialization.JavaScriptSerializer().Serialize(CONSTS));


            try
            {
                call.SetValue("userInput", userInput)
                .SetValue("btoa", new btoaHandler(Backand.Convert.btoa))
                .SetValue("dbRow", dbRow)
                .SetValue("parameters", clientParametersToSend)
                .SetValue("userProfile", userProfile2)
                .SetValue("CONSTS", CONSTS2)
                .Execute(GetXhrWrapper() + code + "; function call(){return backandCallback(userInput, dbRow, parameters, userProfile);}");
            }
            catch (Exception exception)
            {
                Backand.Logger.Log(exception.Message, 501);
                throw new DuradosException("Failed to load the javascript code: " + HandleLineCodes(exception.Message), exception); 
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
                Exception e = new DuradosException(message, exception);
                Backand.Logger.Log("417 " + e.Message, 501);
                if (IsDebug())
                {
                    values[ReturnedValueKey] = "417 " + message;
                    return;
                }
                else
                    throw e;
            }

            var v = call.GetValue("userInput").ToObject();


            //var serialize = function (data) { return JSON.stringify(data); }; var userInputJson = null; var serializeUserInput = function () { userInputJson = serialize(userInput) };
            call.Execute("serializeUserInput()");

            var vj = call.GetValue("userInputJson").ToObject();
            var vjDic = new System.Web.Script.Serialization.JavaScriptSerializer().Deserialize<dynamic>(vj.ToString()) as Dictionary<string,object>;


            if (v != null && v is System.Dynamic.ExpandoObject)
            {
                IDictionary<string, object> newValues = v as IDictionary<string, object>;
                foreach (string key in newValues.Keys)
                {
                    if (values.ContainsKey(key))
                    {
                        object val = newValues[key];
                        Field[] fields = view.GetFieldsByJsonName(key);
                        if (fields[0].DataType == DataType.DateTime)
                        {
                            if (!(val is DateTime))
                            {
                                if (val.ToString().StartsWith("/Date("))
                                {
                                    try
                                    {
                                        val = Convert.ToDateTime(vjDic[key]);
                                    }
                                    catch { }
                                }
                            }
                        }
                        values[key] = val;
                    }
                    else
                    {
                        Field[] fields = view.GetFieldsByJsonName(key);
                        if (fields.Length > 0)
                        {
                            string fieldName = fields[0].Name;
                            object val = newValues[key];
                            if (fields[0].DataType == DataType.DateTime)
                            {
                                if (!(val is DateTime))
                                {
                                    try
                                    {
                                        val = Convert.ToDateTime(val);
                                    }
                                    catch { }
                                }
                            }

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

        
    }

}