using Backand;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
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

        public virtual void Execute(object controller, Dictionary<string, Parameter> parameters, View view, Dictionary<string, object> values, DataRow prevRow, string pk, string connectionString, int currentUsetId, string currentUserRole, IDbCommand command, IDbCommand sysCommand, string actionName, string arn = ARN)
        {
            const string FunctionError = "FunctionError";
            const string Payload = "Payload";
            const string ErrorMessage = "errorMessage";

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
                if (!values.ContainsKey(JavaScript.ReturnedValueKey))
                    values.Add(JavaScript.ReturnedValueKey, response);
                else
                    values[JavaScript.ReturnedValueKey] = response;
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
