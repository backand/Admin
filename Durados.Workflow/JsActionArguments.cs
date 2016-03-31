using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Durados.Workflow
{
    public class JsActionArguments
    {
        public JsActionArguments(object controller, Dictionary<string, Parameter> parameters, View view, Dictionary<string, object> values, DataRow prevRow, string pk, string connectionString, int currentUsetId, string currentUserRole,IDbCommand command)
        {
            Init(controller, parameters, view, values, prevRow, pk, connectionString, currentUsetId, currentUserRole, command);
        }

        Dictionary<string, object> clientParameters = new Dictionary<string, object>();
        Dictionary<string, object> newRow = new Dictionary<string, object>();
        Dictionary<string, object> oldRow = new Dictionary<string, object>();
        Dictionary<string, object> userProfile = new Dictionary<string, object>();
        bool debug = false;

        public Dictionary<string, object> Parameters
        {
            get
            {
                return clientParameters;
            }
        }
        public Dictionary<string, object> UserInput
        {
            get
            {
                return newRow;
            }
        }
        public Dictionary<string, object> DbRow
        {
            get
            {
                return oldRow;
            }
        }
        public Dictionary<string, object> UserProfile
        {
            get
            {
                return userProfile;
            }
        }
        public bool Debug
        {
            get
            {
                return debug;
            }
        }
        
        private void Init(object controller, Dictionary<string, Parameter> parameters, View view, Dictionary<string, object> values, DataRow prevRow, string pk, string connectionString, int currentUsetId, string currentUserRole, IDbCommand command)
        {
            LoadUserInputAndParameters(view, values);
            LoadDbRow(controller, prevRow, view, pk, command);
            LoadUserProfile(view, currentUserRole);
        }

        private bool IsDebug()
        {
            return System.Convert.ToBoolean(Durados.Workflow.JavaScript.GetCacheInCurrentRequest(Durados.Workflow.JavaScript.Debug) ?? false);
        }

        private void LoadUserInputAndParameters(View view, Dictionary<string, object> values)
        {
            if (IsDebug())
            {
                debug = true;
            }

            if (values != null) {
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
        }



        public void LoadDbRow(object controller, DataRow prevRow, View view, string pk, IDbCommand command)
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
        }


        public void LoadUserProfile(View view, string currentUserRole)
        {
            userProfile.Add("username", view.Database.GetCurrentUsername());
            userProfile.Add("role", currentUserRole);
            userProfile.Add("app", view.Database.GetCurrentAppName());
            userProfile.Add("userId", view.Database.GetCurrentUserId());
            userProfile.Add("token", System.Web.HttpContext.Current.Request.Headers["Authorization"] ?? "anonymous-" + (System.Web.HttpContext.Current.Request.Headers["AnonymousToken"] ?? view.Database.GetAnonymousToken().ToString()));
            userProfile.Add("request", new Dictionary<string, object>() { { "userIP", GetUserIP() }, { "method", System.Web.HttpContext.Current.Request.HttpMethod }, { "headers", GetHeaders(System.Web.HttpContext.Current.Request.Headers) } });

        }

        public string GetUserIP()
        {
            var ip = (System.Web.HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"] != null
                      && System.Web.HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"] != "")
                     ? System.Web.HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"]
                     : System.Web.HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
            if (ip.Contains(","))
                ip = ip.Split(',').First().Trim();
            return ip;
        }

        public Dictionary<string, object> GetHeaders(System.Collections.Specialized.NameValueCollection headers)
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
    }
}
