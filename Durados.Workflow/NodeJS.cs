using Backand;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net;
using System.Text;

namespace Durados.Workflow
{
    public class NodeJS
    {
        private string BaseUrl = System.Configuration.ConfigurationManager.AppSettings["nodeHost"] ?? "http://127.0.0.1:9000";
        private string NodeJSBucket = System.Configuration.ConfigurationManager.AppSettings["NodeJSBucket"] ?? "nodejs.backand.net";
        private const string ARN = "arn:aws:iam::328923390206:role/lambda_control";
        private const int MemorySize = 128;
        private const int Timeout = 3;
        private const string DebugKey = "{{$$debug$$}}";
        private const string Logs = "logs";
        private const string Handled = "Handled";
        private const string FunctionError = "FunctionError";
        private const string Payload = "Payload";
        private const string ErrorMessage = "errorMessage";

        private Dictionary<string, object> GetCallLambdaPayload(object controller, Dictionary<string, Parameter> parameters, View view, Dictionary<string, object> values, DataRow prevRow, string pk, string connectionString, int currentUsetId, string currentUserRole, IDbCommand command)
        {
            JsActionArguments arguments = new JsActionArguments(controller, parameters, view, values, prevRow, pk, connectionString, currentUsetId, currentUserRole, command);

            Dictionary<string, object> payload = new Dictionary<string, object>();

            payload.Add("userInput", arguments.UserInput);
            payload.Add("dbRow", arguments.DbRow);
            payload.Add("parameters", arguments.Parameters);
            payload.Add("userProfile", arguments.UserProfile);
            return payload;
 
        }

        private bool IsDebug(Dictionary<string, object> values)
        {
            return values.ContainsKey("{{$$debug$$}}") || System.Web.HttpContext.Current.Request.QueryString["$$debug$$"] == "true"; ;
        }

        public virtual void ExecuteOld(object controller, Dictionary<string, Parameter> parameters, View view, Dictionary<string, object> values, DataRow prevRow, string pk, string connectionString, int currentUsetId, string currentUserRole, IDbCommand command, IDbCommand sysCommand, string actionName, string arn, Durados.Security.Aws.IAwsCredentials awsCredentials)
        {
            const string Payload = "Payload";
            const string ErrorMessage = "errorMessage";
            
            bool isDebug = IsDebug(values);

            string url = BaseUrl + "/callLambda";
            XMLHttpRequest request = new XMLHttpRequest();
            request.open("POST", url, false);
            Dictionary<string, object> data = new Dictionary<string, object>();
            
            Dictionary<string, object> payload = GetCallLambdaPayload(controller, parameters, view, values, prevRow, pk, connectionString, currentUsetId, currentUserRole, command);

            string functionName = view.Name + "_" + actionName;
            string folder = view.Database.GetCurrentAppName();

            data.Add("folder", folder);
            data.Add("functionName", functionName);
            data.Add("payload", payload);
            data.Add("Role", arn);
            if (isDebug)
            {
                if (!System.Web.HttpContext.Current.Items.Contains(JavaScript.GuidKey))
                    System.Web.HttpContext.Current.Items.Add(JavaScript.GuidKey, Guid.NewGuid());
                data.Add("getLog", true);
            }

            request.setRequestHeader("content-type", "application/json");

            System.Web.Script.Serialization.JavaScriptSerializer jss = new System.Web.Script.Serialization.JavaScriptSerializer();
            request.send(jss.Serialize(data));

            if (request.status != 200)
            {
                throw new NodeJsException(request.responseText);
            }

            Dictionary<string, object> response = null;
            try
            {
                response = jss.Deserialize<Dictionary<string, object>>(request.responseText);
            }
            catch (Exception exception)
            {
                throw new Durados.DuradosException("Could not parse NodeJS response", exception);
            }

            if (response.ContainsKey(FunctionError))
            {
                if (response.ContainsKey(Payload))
                {
                    var responsePayload = response[Payload];
                    if (responsePayload is string)
                    {
                        IDictionary<string, object> responsePayloadError = null;
                        try
                        {
                            responsePayloadError = jss.Deserialize<Dictionary<string,object>>((string)responsePayload);
                        }
                        catch
                        {
                            throw new NodeJsException((string)responsePayload);
                        }
                        if (responsePayloadError.ContainsKey(ErrorMessage))
                        {
                            throw new NodeJsException(responsePayloadError[ErrorMessage].ToString());
                        }
                        else
                        {
                            throw new NodeJsException((string)responsePayload);
                        }
                    }
                    else
                    {
                        throw new NodeJsException(request.responseText);
                    }
                }
                else
                {
                    throw new NodeJsException(request.responseText);
                }
            }

            if (response != null && values != null)
            {
                if (isDebug)
                {
                    //HandleLog(response);
                }

                if (!values.ContainsKey(JavaScript.ReturnedValueKey))
                    values.Add(JavaScript.ReturnedValueKey, response);
                else
                    values[JavaScript.ReturnedValueKey] = response;
            }
        }

        public virtual Dictionary<string, object>[] GetLambdaList(Durados.Security.Aws.IAwsCredentials awsCredentials)
        {
            string url = BaseUrl + "/getLambdaList";
            XMLHttpRequest request = new XMLHttpRequest();
            request.open("POST", url, false);
            Dictionary<string, object> data = new Dictionary<string, object>();

            data.Add("awsRegion", awsCredentials.Region);
            data.Add("accessKeyId", awsCredentials.AccessKeyID);
            data.Add("secretAccessKey", awsCredentials.SecretAccessKey);
            
            request.setRequestHeader("content-type", "application/json");

            System.Web.Script.Serialization.JavaScriptSerializer jss = new System.Web.Script.Serialization.JavaScriptSerializer();
            request.send(jss.Serialize(data));

            if (request.status != 200)
            {
                throw new NodeJsException(request.responseText);
            }

            Dictionary<string, object>[] response = null;
            try
            {
                response = jss.Deserialize<Dictionary<string, object>[]>(request.responseText);
            }
            catch (Exception exception)
            {
                throw new Durados.DuradosException("Could not parse NodeJS response", exception);
            }

            return response;
        }

        public virtual object Download(Durados.Security.Aws.IAwsCredentials awsCredentials, string lambdaFunctionName)
        {
            string url = BaseUrl + "/downloadLambda";
            XMLHttpRequest request = new XMLHttpRequest();
            request.open("POST", url, false);
            Dictionary<string, object> data = new Dictionary<string, object>();

            data.Add("awsRegion", awsCredentials.Region);
            data.Add("accessKeyId", awsCredentials.AccessKeyID);
            data.Add("secretAccessKey", awsCredentials.SecretAccessKey);
            data.Add("functionName", lambdaFunctionName);

            request.setRequestHeader("content-type", "application/json");

            System.Web.Script.Serialization.JavaScriptSerializer jss = new System.Web.Script.Serialization.JavaScriptSerializer();
            request.send(jss.Serialize(data));

            if (request.status != 200)
            {
                throw new NodeJsException(request.responseText);
            }

            object response = null;
            try
            {
                response = jss.Deserialize<object>(request.responseText);
            }
            catch (Exception exception)
            {
                throw new Durados.DuradosException("Could not parse NodeJS response", exception);
            }

            return response;
        }

        
        public virtual void Execute(object controller, Dictionary<string, Parameter> parameters, View view, Dictionary<string, object> values, DataRow prevRow, string pk, string connectionString, int currentUserId, string currentUserRole, IDbCommand command, IDbCommand sysCommand, string actionName, string arn, Durados.Security.Aws.IAwsCredentials awsCredentials, bool isLambda)
        {
            
            bool isDebug = IsDebug(values);

            string url = BaseUrl + "/invokeLambda";
            XMLHttpRequest request = new XMLHttpRequest();
            request.open("POST", url, false);
            Dictionary<string, object> data = new Dictionary<string, object>();

            Dictionary<string, object> payload = GetCallLambdaPayload(controller, parameters, view, values, prevRow, pk, connectionString, currentUserId, currentUserRole, command);

            string folder = view.Database.GetCurrentAppName();
            string functionArn = arn + folder + "_" + view.Name + "_" + actionName;
            if (isLambda)
            {
                functionArn = arn;
                payload = new Dictionary<string, object>();
                foreach (string key in values.Keys)
                {
                    if (key != DebugKey)
                    {
                        payload.Add(key.ReplaceToken("{{", "").ReplaceToken("}}", ""), values[key]);
                    }
                }
            }

            data.Add("awsRegion", awsCredentials.Region);
            data.Add("accessKeyId", awsCredentials.AccessKeyID);
            data.Add("secretAccessKey", awsCredentials.SecretAccessKey);
            data.Add("functionArn", functionArn);
            data.Add("payload", payload);
            Guid requestId = Guid.NewGuid();
            if (isDebug)
            {
                if (!System.Web.HttpContext.Current.Items.Contains(JavaScript.GuidKey))
                    System.Web.HttpContext.Current.Items.Add(JavaScript.GuidKey, requestId);
                else
                    requestId = (Guid)System.Web.HttpContext.Current.Items[JavaScript.GuidKey];

                string appName = (string)System.Web.HttpContext.Current.Items["appName"];
                string username = (string)System.Web.HttpContext.Current.Items["username"];
                string token = (string)System.Web.HttpContext.Current.Request.Headers["authorization"];


                data.Add("backandRequest", new { id = requestId.ToString(), appName = appName, username = username, accessToken = token });
            
                data.Add("isProduction", false);
            }
            else
            {
                data.Add("isProduction", true);
            }

            request.setRequestHeader("content-type", "application/json");

            System.Web.Script.Serialization.JavaScriptSerializer jss = new System.Web.Script.Serialization.JavaScriptSerializer();
            request.send(jss.Serialize(data));

            if (IsFunction(view))
            {
                FunctionResponse(request, jss, values, isDebug, requestId);
                return;
            }

            if (request.status != 200)
            {
                throw new NodeJsException(request.responseText);
            }

            Dictionary<string, object> response = null;
            try
            {
                response = jss.Deserialize<Dictionary<string, object>>(request.responseText);
            }
            catch (Exception exception)
            {
                throw new Durados.DuradosException("Could not parse NodeJS response", exception);
            }

            if (response.ContainsKey(FunctionError))
            {
                if (response.ContainsKey(Payload))
                {
                    var responsePayload = response[Payload];
                    if (responsePayload is string)
                    {
                        IDictionary<string, object> responsePayloadError = null;
                        try
                        {
                            responsePayloadError = jss.Deserialize<Dictionary<string, object>>((string)responsePayload);
                        }
                        catch
                        {
                            throw new NodeJsException((string)responsePayload);
                        }
                        if (responsePayloadError.ContainsKey(ErrorMessage))
                        {
                            throw new NodeJsException(responsePayloadError[ErrorMessage].ToString());
                        }
                        else
                        {
                            throw new NodeJsException((string)responsePayload);
                        }
                    }
                    else
                    {
                        throw new NodeJsException(request.responseText);
                    }
                }
                else
                {
                    throw new NodeJsException(request.responseText);
                }
            }

            if (response != null && values != null)
            {
                if (isDebug)
                {
                    HandleLog(response, requestId);
                }

                CleanResponse(response);

                
                if (!values.ContainsKey(JavaScript.ReturnedValueKey))
                    values.Add(JavaScript.ReturnedValueKey, response);
                else
                    values[JavaScript.ReturnedValueKey] = response;
            }
        }

        private bool IsFunction(View view)
        {
            return view.Name == "_root";
        }

        private void FunctionResponse(XMLHttpRequest request, System.Web.Script.Serialization.JavaScriptSerializer jss, Dictionary<string, object> values, bool isDebug, Guid requestId)
        {
            if (request.status != 200)
            {
                throw new NodeJsException(request.responseText);
            }

            Dictionary<string, object> response = null;
            try
            {
                response = jss.Deserialize<Dictionary<string, object>>(request.responseText);
            }
            catch (Exception exception)
            {
                throw new Durados.DuradosException("Could not parse NodeJS response", exception);
            }

            object responsePayload = null;
            if (response.ContainsKey(Payload))
            {
                responsePayload = response[Payload];
                if (((string)responsePayload).EndsWith("Z\""))
                {
                    try
                    {
                        responsePayload = jss.Deserialize<DateTime>((string)responsePayload);
                    }
                    catch
                    {
                        try
                        {
                            responsePayload = jss.Deserialize<object>((string)responsePayload);
                        }
                        catch
                        {
                            //throw new NodeJsException((string)responsePayload);
                        }
                    }
                }
                else
                {
                    try
                    {
                        responsePayload = jss.Deserialize<object>((string)responsePayload);
                    }
                    catch
                    {
                        //throw new NodeJsException((string)responsePayload);
                    }
                }
            }
            else
            {
                throw new NodeJsException(request.responseText);
            }

            //if (response.ContainsKey(FunctionError))
            //{

            //    if (responsePayload != null && responsePayload is string)
            //    {
            //        IDictionary<string, object> responsePayloadError = null;
            //        try
            //        {
            //            responsePayloadError = jss.Deserialize<Dictionary<string, object>>((string)responsePayload);
            //        }
            //        catch
            //        {
            //            throw new NodeJsException((string)responsePayload);
            //        }
            //        if (responsePayloadError.ContainsKey(ErrorMessage))
            //        {
            //            throw new NodeJsException(responsePayloadError[ErrorMessage].ToString());
            //        }
            //        else
            //        {
            //            throw new NodeJsException((string)responsePayload);
            //        }
            //    }
            //    else
            //    {
            //        throw new NodeJsException(request.responseText);
            //    }
            //}

            if (response.ContainsKey(FunctionError))
            {
                object logs = null;
                if (response.ContainsKey(Logs))
                {
                    logs = response[Logs];
                }
                HandleError(response, responsePayload, jss, logs);
            }

            if (response != null && values != null)
            {
                if (isDebug)
                {
                    HandleLog(response, requestId);
                }

                CleanResponse(response);

                if (!values.ContainsKey(JavaScript.ReturnedValueKey))
                    values.Add(JavaScript.ReturnedValueKey, responsePayload);
                else
                    values[JavaScript.ReturnedValueKey] = responsePayload;
            }
        }

        const string StatusCode = "StatusCode";
        const string StackTrace = "stackTrace";
        private void HandleError(Dictionary<string, object> response, object responsePayload, System.Web.Script.Serialization.JavaScriptSerializer jss, object logs)
        {
            string responsePayloadString;
            if (responsePayload is IDictionary<string, object>)
            {
                if (!((IDictionary<string, object>)responsePayload).ContainsKey(StackTrace) && logs != null)
                {
                    ((IDictionary<string, object>)responsePayload).Add(StackTrace, logs);
                }
                responsePayloadString = jss.Serialize(responsePayload);
            }
            else
            {
                responsePayloadString = responsePayload.ToString();
            }
            if (response.ContainsKey(StatusCode) && !response[StatusCode].Equals(200))
            {
                HttpStatusCode httpStatusCode = (HttpStatusCode)Enum.ToObject(typeof(HttpStatusCode), System.Convert.ToInt32(response[StatusCode]));
                throw new NodeJsException(responsePayloadString, true, httpStatusCode);
            }
            throw new NodeJsException(responsePayloadString, true);
        }

        private bool IsHandled(Dictionary<string, object> response)
        {
            
            return response.ContainsKey(FunctionError) && response[FunctionError].Equals(Handled);
        }

        private void HandleLog(Dictionary<string, object> response, Guid requestId)
        {
            if (response.ContainsKey(Logs))
            {
                LambdaLogConverter lambdaLog = new LambdaLogConverter();

                lambdaLog.Convert((System.Collections.ArrayList)response[Logs], requestId);
                
            }
            
        }

        public void LambdaLog(System.Collections.ArrayList logs, Guid requestId)
        {
            LambdaLogConverter lambdaLog = new LambdaLogConverter();

            lambdaLog.Convert(logs, requestId);
        }

        
        
        private void CleanResponse(Dictionary<string, object> response)
        {
            if (response.ContainsKey("LogResult"))
            {
                response.Remove("LogResult");
            }
            if (response.ContainsKey("logs"))
            {
                //var log = response["logs"];
                response.Remove("logs");
                //response.Add("log", log);
            }
            if (response.ContainsKey("startTime"))
            {
                response.Remove("startTime");
            }
            if (response.ContainsKey("endTime"))
            {
                response.Remove("endTime");
            }
            if (response.ContainsKey("requestId"))
            {
                response.Remove("requestId");
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="bucket">bucket is S3</param>
        /// <param name="folder">app name</param>
        /// <param name="fileName">the zip file created by the CLI</param>
        /// <param name="functionName">the lambda function name</param>
        /// <param name="handlerName">the js file with the root function</param>
        /// <param name="callFunctionName">the root function name</param>
        public virtual void Create(string bucket, string folder, string fileName, string functionName, string handlerName, string callFunctionName, string arn = ARN, int memorySize = MemorySize, int timeout = Timeout)
        {
            string url = BaseUrl + "/createLambda";
            XMLHttpRequest request = new XMLHttpRequest();
            request.open("POST", url, false);
            Dictionary<string, object> data = new Dictionary<string, object>();

            data.Add("bucket", bucket);
            data.Add("folder", folder);
            data.Add("fileName", fileName);
            data.Add("functionName", functionName);
            data.Add("handlerName", handlerName);
            data.Add("callFunctionName", callFunctionName);
            data.Add("Role", arn);
            data.Add("memorySize", memorySize);
            data.Add("timeout", timeout);
            

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
                throw new Durados.DuradosException("Could not parse upload response", exception);
            }

        }

        public virtual void Update(string bucket, string folder, string fileName, string functionName, string arn = ARN, int memorySize = MemorySize, int timeout = Timeout)
        {
            string url = BaseUrl + "/updateLambda";
            XMLHttpRequest request = new XMLHttpRequest();
            request.open("POST", url, false);
            Dictionary<string, object> data = new Dictionary<string, object>();

            data.Add("bucket", bucket);
            data.Add("folder", folder);
            data.Add("fileName", fileName);
            data.Add("functionName", functionName);
            data.Add("Role", arn);
            data.Add("memorySize", memorySize);
            data.Add("timeout", timeout);
            

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
                throw new Durados.DuradosException("Could not parse upload response", exception);
            }
        }

        public virtual void Delete(string folder, string functionName, string arn = ARN)
        {
            string url = BaseUrl + "/deleteLambda";
            XMLHttpRequest request = new XMLHttpRequest();
            request.open("POST", url, false);
            Dictionary<string, object> data = new Dictionary<string, object>();

            data.Add("folder", folder);
            data.Add("functionName", functionName);
            data.Add("Role", arn);

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
                throw new Durados.DuradosException("Could not parse upload response", exception);
            }
        }
    }
}
