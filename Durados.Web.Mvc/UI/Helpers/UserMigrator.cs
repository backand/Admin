using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Durados.DataAccess;
using System.Web.Script.Serialization;
using System.Net;
using System.IO;
using Durados.Web.Mvc.Controllers.Api;

namespace Durados.Web.Mvc.UI.Helpers
{
    public class UserMigrator
    {
        JavaScriptSerializer javaScriptSerializer = new JavaScriptSerializer();
        const string missingEmails = "missingEmails";
        const string errors = "errors";
        const string POST = "POST";
        const string Authorization = "Authorization";
        const string AppName = "AppName";

        public object Migrate(Map map, string usersTableName, string emailFieldName, string usernameFieldName, string firstNameFieldName, string lastNameFieldName, string passwordFieldName, int pageSize, string baseUrl)
        {
            Dictionary<string, object> report = new Dictionary<string, object>();
            report.Add(missingEmails, new List<string>());
            report.Add(errors, new List<string>());

            
            bool endOfData = false;
            int page = 1;

            string signUpUrl = baseUrl + GetSignUpUrl();
            Dictionary<string, object> headers = new Dictionary<string, object>() { { "SignUpToken", GetSignUpToken(map) } };

            View view = (View)map.Database.Views[usersTableName];
            while (!endOfData)
            {
                int rowCount = -1;
                DataView dataView = view.FillPage(page, pageSize, null, null, null, out rowCount, null, null);
                MigratePage(map, view, dataView, usersTableName, emailFieldName, usernameFieldName, firstNameFieldName, lastNameFieldName, passwordFieldName, signUpUrl, baseUrl, headers, report);
                if (dataView.Count < pageSize)
                {
                    endOfData = true;
                }
                else
                {
                    page += pageSize;
                }
            }
            
            return report;
        }

       
        private void MigratePage(Map map, View view, DataView dataView, string usersTableName, string emailFieldName, string usernameFieldName, string firstNameFieldName, string lastNameFieldName, string passwordFieldName, string signUpUrl, string baseUrl, Dictionary<string, object> headers, Dictionary<string, object> report)
        {
            object json = GetJsonInput(map, view, dataView, usersTableName, emailFieldName, usernameFieldName, firstNameFieldName, lastNameFieldName, passwordFieldName, signUpUrl, headers, report);
            object response = RunBulk(map, json, baseUrl);
            AddErrors(report, response);
            
        }

        private void AddErrors(Dictionary<string, object> report, object response)
        {
            List<string> errorsList = (List<string>)report[errors];
            foreach (string error in GetErrors(response))
            {
                errorsList.Add(error);
            }
            //((List<string>)report[errors]).Add(((Dictionary<string, object>)response)["response"].ToString());
        }

        private string[] GetErrors(object response)
        {
            List<string> resposnses = new List<string>();
            string jsonArray = ((Dictionary<string, object>)response)["response"].ToString();
            Dictionary<string, object>[] valuesArray = JsonConverter.DeserializeArray(jsonArray);
            foreach (Dictionary<string, object> values in valuesArray)
            {
                if (values.ContainsKey("status") && values["status"] is int && (int)values["status"] > 299 && values.ContainsKey("data"))
                {
                    string s = javaScriptSerializer.Serialize(values["data"]);
                    if (!s.Contains("The user is already signed up to this app"))
                    {
                        resposnses.Add(s);
                    }
                }
            }

            return resposnses.ToArray();

        }

        private object GetJsonInput(Map map, View view, DataView dataView, string usersTableName, string emailFieldName, string usernameFieldName, string firstNameFieldName, string lastNameFieldName, string passwordFieldName, string signUpUrl, Dictionary<string, object> headers, Dictionary<string, object> report)
        {
            List<Dictionary<string, object>> list = new List<Dictionary<string, object>>();

            
            foreach (System.Data.DataRowView row in dataView)
            {
                Dictionary<string, object> values = new Dictionary<string, object>();

                var data = GetData(row.Row, emailFieldName, usernameFieldName, firstNameFieldName, lastNameFieldName, passwordFieldName);
                if (data["email"] != null)
                {
                    values.Add("method", "post");
                    values.Add("url", signUpUrl);
                    values.Add("data", data);
                    values.Add("headers", headers);

                    list.Add(values);
                }
                else
                {
                    ((List<string>)report[missingEmails]).Add(view.GetPkValue(row.Row));
                }
            }

            return list.ToArray();
        }

        private Dictionary<string, object> GetData(DataRow dataRow, string emailFieldName, string usernameFieldName, string firstNameFieldName, string lastNameFieldName, string passwordFieldName)
        {
            Dictionary<string, object> values = new Dictionary<string, object>();

            string password = GetFieldValue(dataRow, passwordFieldName, GetRandomPassword()).Replace("+", "%2B");
            string email = GetFieldValue(dataRow, emailFieldName, null).Replace("+", "%2B");
            string firstName = GetFieldValue(dataRow, emailFieldName, email.Split('@').FirstOrDefault()).Replace("+", "%2B");
            string lastName = GetFieldValue(dataRow, emailFieldName, string.Empty).Replace("+", "%2B");

            values.Add("email", email);
            values.Add("firstName", firstName);
            values.Add("lastName", lastName);
            values.Add("password", password);
            values.Add("confirmPassword", password);
            values.Add("parameters", new Dictionary<string, object>() { { "sync", true } });
            return values;
        }

        private string GetRandomPassword()
        {
            return DuradosAuthorizationHelper.GeneratePassword(4, 4, 4);
        }

        private string GetFieldValue(DataRow dataRow, string fieldName, string defaultValue)
        {
            return dataRow.IsNull(fieldName) ? defaultValue : dataRow[fieldName].ToString();
        }

        private string GetSignUpToken(Map map)
        {
            return map.SignUpToken.ToString();
        }

        private string GetSignUpUrl()
        {
            return "/1/user/signup";
        }
        

        private object RunBulk(Map map, object json, string baseUrl)
        {
            
            var bytes = GetRequestBody(json);

            WebRequest request = WebRequest.Create(baseUrl + GetBulkUrl());

            request.Method = POST;
            request.Headers[Authorization] = GetCurrentAuth();
            request.Headers[AppName] = GetCurrentAppName(map);

            using (Stream requestStream = request.GetRequestStream())
            {
                //Writes a sequence of bytes to the current stream 
                requestStream.Write(bytes, 0, bytes.Length);
                requestStream.Close();//Close stream
            }

            string responseText = string.Empty;

            int status = -1;

            try
            {
                using (var response = (HttpWebResponse)request.GetResponse())
                {
                    status = (int)response.StatusCode;
                    if (status >= 200 && status < 300)
                    {
                        //Get response stream into StreamReader
                        using (Stream responseStream = response.GetResponseStream())
                        {
                            string charset = GetCharset(request);
                            Encoding encoding = Encoding.UTF8;
                            if (charset != null)
                            {
                                try
                                {
                                    encoding = Encoding.GetEncoding(charset);
                                }
                                catch { }
                            }

                            using (StreamReader reader = new StreamReader(responseStream, encoding))
                                responseText = reader.ReadToEnd();
                            //if(string.IsNullOrEmpty(responseText)) responseText="{}";
                        }
                    }
                }
            }
            catch (WebException we)
            {   //TODO: Add custom exception handling
                if (we.Status == WebExceptionStatus.Timeout)
                    status = (int)HttpStatusCode.RequestTimeout;
                responseText = we.Message;
                var encoding = ASCIIEncoding.ASCII;
                if (we.Response != null)
                {
                    using (var reader = new System.IO.StreamReader(we.Response.GetResponseStream(), encoding))
                    {
                        responseText += ": " + reader.ReadToEnd();
                    }
                    status = (int)((System.Net.HttpWebResponse)(we.Response)).StatusCode;
                }

            }

            return new Dictionary<string, object>() { { "status", status }, { "response", responseText } };
        }

        private string GetCharset(WebRequest request)
        {
            try
            {
                string contentType = request.ContentType;
                string[] arr1 = contentType.Split(";".ToCharArray());
                if (arr1.Length != 2)
                {
                    return null;
                }
                string[] arr2 = arr1.LastOrDefault().Split("=".ToCharArray());
                if (arr2.Length != 2)
                {
                    return null;
                }
                return arr2.LastOrDefault();
            }
            catch
            {
                return null;
            }
        }

        private string GetCurrentAppName(Map map)
        {
            return map.AppName;
        }

        private string GetBulkUrl()
        {
            return "/1/bulk/";
        }

        private string GetCurrentAuth()
        {
            return System.Web.HttpContext.Current.Request.Headers[Authorization].ToString();
        }

        private byte[] GetRequestBody(object dataToPost)
        {
            string jsonToPost = javaScriptSerializer.Serialize(dataToPost);
            var bytes = System.Text.Encoding.ASCII.GetBytes(jsonToPost);
            string requestBody = System.Text.Encoding.ASCII.GetString(bytes);
            return bytes;
        }
    }
}
