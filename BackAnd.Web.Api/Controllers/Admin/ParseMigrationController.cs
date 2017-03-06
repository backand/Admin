using Durados.Data;
using Durados.Web.Mvc;
using Durados.Web.Mvc.Controllers.Api;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Script.Serialization;

using Durados.DataAccess;
using System.Data;

namespace BackAnd.Web.Api.Controllers.Admin
{
    [RoutePrefix("1")]
    [BackAnd.Web.Api.Controllers.Filters.BackAndAuthorize("Admin,Developer")]
    public class ParseMigrationController : apiController
    {
        const string Status = "status";
        const string AppName = "appName";
        const string AppToken = "appToken";
        const string CreationDate = "CreationDate";
        const string ParseSchema = "parseSchema";
        const string ParseUrl = "parseUrl";
        
        const string Authorization = "Authorization";

        [System.Web.Http.HttpPut]
        [Route("~/1/parse/updateCurrentUser")]
        public IHttpActionResult UpdateCurrentUser()
        {
            if (Map.IsMainMap)
            {
                return ResponseMessage(Request.CreateResponse(HttpStatusCode.Forbidden, Messages.NotSignInToApp));
            }

            try
            {
                DataRow row = Map.Database.GetUserRow();

                string firstName = row.IsNull("FirstName") ? string.Empty : row["FirstName"].ToString();
                string lastName = row.IsNull("LastName") ? string.Empty : row["LastName"].ToString();
                string email = row.IsNull("Email") ? string.Empty : row["Email"].ToString();

                if (!Map.Database.Views.ContainsKey("users"))
                {
                    return ResponseMessage(Request.CreateResponse(HttpStatusCode.NotFound, "users was not found"));
                }

                View usersView = (View)Map.Database.Views["users"];

                Dictionary<string, object> values = new Dictionary<string, object>() { { "email", email }, { "username", email }, { "firstName", firstName }, { "lastName", lastName } };
                usersView.Create(values, null, null, null, null, null);

            }
            catch (Exception exception)
            {
                return ResponseMessage(Request.CreateResponse(HttpStatusCode.InternalServerError, "appName:" + GetCurrentAppName() + ", baseUrl: " + GetCurrentBaseUrl() + "; error" + exception.Message + "; trace: " + exception.StackTrace));

            }
            return Ok();
        }
            
        [Route("~/1/parse")]
        public IHttpActionResult Get()
        {
            
            try
            {
                ArrayList list = GetParseConverterStatus();

                if (Map.IsMainMap)
                {
                    return ResponseMessage(Request.CreateResponse(HttpStatusCode.Forbidden, Messages.NotSignInToApp));
                }
                if (list == null)
                {
                    return Ok(new Dictionary<string, object> { { Status, null } });
                }

                if (list == null || list.Count == 0)
                {
                    return Ok(new Dictionary<string, object> { { Status, null } });
                }
                else if (list.Count > 1)
                {
                    throw new BackAndApiUnexpectedResponseException(new Exception(string.Format(Messages.MoreThanOneParseConversions, GetCurrentAppName())), this);
                }
                return Ok(list[0]);
            }
            catch (Exception exception)
            {
                //throw new BackAndApiUnexpectedResponseException(exception, this);
                return ResponseMessage(Request.CreateResponse(HttpStatusCode.InternalServerError, "appName:" + GetCurrentAppName() + ", baseUrl: " + GetCurrentBaseUrl() + "; error" + exception.Message + "; trace: " + exception.StackTrace));
            }
        }

        private ArrayList GetParseConverterStatus()
        {
            const string QueryPath = "{0}/1/query/data/GetStatus?parameters={{AppName:'{1}'}}";
            const string GET = "GET";
           
            WebRequest request = WebRequest.Create(string.Format(QueryPath, GetCurrentBaseUrl(), GetCurrentAppName()));
            request.Method = GET;
            request.Headers[Authorization] = GetBasicAuth();

            string data = null;
            try
            {
                using (var response = request.GetResponse())
                {
                    using (Stream datastream = response.GetResponseStream())
                    {
                        using (StreamReader sr = new StreamReader(datastream, System.Text.Encoding.UTF8))
                        {
                            data = sr.ReadToEnd();
                        }
                    }
                }
            }
            catch
            {
                return null;
            }
            if (string.IsNullOrEmpty(data))
            {
                return null;
            }

            return new JavaScriptSerializer().Deserialize<ArrayList>(data);
                
        }

        [Route("~/1/parse")]
        public IHttpActionResult Post()
        {
            const string ObjectsPath = "{0}/1/objects/{1}";
            const string POST = "POST";
           
            try
            {
                if (Map.IsMainMap)
                {
                    return ResponseMessage(Request.CreateResponse(HttpStatusCode.Forbidden, Messages.NotSignInToApp));
                }
                
                var invalidStatusResponse = GetInvalidStatusResponse();
                if (invalidStatusResponse != null)
                    return invalidStatusResponse;

                var bytes = GetRequestBody();

                WebRequest request = WebRequest.Create(string.Format(ObjectsPath, GetCurrentBaseUrl(), Maps.ParseConverterObjectName));

                request.Method = POST;
                request.Headers[Authorization] = GetBasicAuth();

                using (Stream requestStream = request.GetRequestStream())
                {
                    //Writes a sequence of bytes to the current stream 
                    requestStream.Write(bytes, 0, bytes.Length);
                    requestStream.Close();//Close stream
                }

                using (var response = request.GetResponse())
                {
                    
                }

                UpdatePkType();

                return Ok();
            }
            catch (Exception exception)
            {
                //throw new BackAndApiUnexpectedResponseException(exception, this);
                return ResponseMessage(Request.CreateResponse(HttpStatusCode.InternalServerError, "appName:" + GetCurrentAppName() + ", baseUrl: " + GetCurrentBaseUrl() + "; error" + exception.Message + "; trace: " + exception.StackTrace));
            }
        }

        private void UpdatePkType()
        {
            View view = (View)Map.GetConfigDatabase().Views["Database"];
            view.Edit(new Dictionary<string, object>() { { "PkType", Durados.Database.AutoGuidPkType } }, "0", view_BeforeEdit, view_BeforeEditInDatabase, view_AfterEditBeforeCommit, view_AfterEditAfterCommit);
            RefreshConfigCache(Map);
        }

        private byte[] GetRequestBody()
        {
            Dictionary<string, object> parseConversionData = GetParseConversionData();
            Dictionary<string, object> dataToPost = new Dictionary<string, object>() { 
                    { AppName, GetAppName(parseConversionData) }, 
                    { ParseUrl, GetParseUrl(parseConversionData) }, 
                    { AppToken, GetAppToken(parseConversionData) }, 
                    { CreationDate, DateTime.Now }, 
                    { Status, 0 }, 
                    { ParseSchema, GetParseSchema(parseConversionData) } 
                };

            string jsonToPost = new JavaScriptSerializer().Serialize(dataToPost);
            var bytes = System.Text.Encoding.ASCII.GetBytes(jsonToPost);
            string requestBody = System.Text.Encoding.ASCII.GetString(bytes);
            return bytes;
        }

        private IHttpActionResult GetInvalidStatusResponse()
        {
            ArrayList status = GetParseConverterStatus();
            if (status != null && status.Count > 0)
            {
                int statusValue = -1;

                string appName = GetCurrentAppName();
                try
                {
                    statusValue = Convert.ToInt32(((Dictionary<string, object>)status[0])[Status]);
                }
                catch
                {
                    return ResponseMessage(Request.CreateResponse(HttpStatusCode.Forbidden, string.Format(Messages.MigrationAlreadyStartedWithoutGettingItsStatus, appName)));
                }

                if (statusValue == (int)ParseMigrationStatus.Idle)
                {
                    return ResponseMessage(Request.CreateResponse(HttpStatusCode.Forbidden, string.Format(Messages.MigrationAlreadyStartedWithStatusIdle, appName)));
                }
                else if (statusValue == (int)ParseMigrationStatus.Started)
                {
                    return ResponseMessage(Request.CreateResponse(HttpStatusCode.Forbidden, string.Format(Messages.MigrationAlreadyStartedWithStatusStarted, appName)));
                }
                else if (statusValue == (int)ParseMigrationStatus.Finished)
                {
                    return ResponseMessage(Request.CreateResponse(HttpStatusCode.Forbidden, string.Format(Messages.MigrationAlreadyStartedWithStatusFinished, appName)));
                }
                else
                {
                    return ResponseMessage(Request.CreateResponse(HttpStatusCode.Forbidden, string.Format(Messages.MigrationAlreadyStartedWithoutGettingItsStatus, appName)));
                }

            }

            return null;
        }

        private object GetParseSchema(Dictionary<string, object> parseConversionData)
        {
            return GetValue(parseConversionData, ParseSchema);
        }

        private object GetAppToken(Dictionary<string, object> parseConversionData)
        {
            return GetCurrentAppMasterToken() + ":" + GetCurrentUserToken();
        }

        private string GetCurrentUserToken()
        {
            return Map.Database.GetUserGuid();
        }

        private string GetCurrentAppMasterToken()
        {
            return Map.Guid.ToString();
        }

        private object GetParseUrl(Dictionary<string, object> parseConversionData)
        {
            return GetValue(parseConversionData, ParseUrl);
        }

        private object GetValue(Dictionary<string, object> parseConversionData, string fieldName)
        {
            if (parseConversionData.ContainsKey(fieldName))
                return parseConversionData[fieldName];
            return null;
        }

        private object GetAppName(Dictionary<string, object> parseConversionData)
        {
            object value = GetValue(parseConversionData, AppName);
            if (value == null)
                return GetCurrentAppName();
            else
                return value;
        }

        private Dictionary<string, object> GetParseConversionData()
        {
            string json = System.Web.HttpContext.Current.Server.UrlDecode(Request.Content.ReadAsStringAsync().Result.Replace("%22", "%2522").Replace("%2B", "%252B").Replace("+", "%2B"));

            return JsonConverter.Deserialize(json);
                
        }

        private static string GetBasicAuth()
        {
            const string Basic = "Basic";
            const char Space = ' ';
            const char Colon = ':';
            return Basic + Space + Convert.ToBase64String(System.Text.Encoding.Default.GetBytes(Maps.ParseConverterMasterKey + Colon + Maps.ParseConverterAdminKey));
        }

        private string GetCurrentAppName()
        {
            return (string)System.Web.HttpContext.Current.Items[Durados.Web.Mvc.Database.AppName];
        }

        
    }

    public enum ParseMigrationStatus
    {
        Idle = 0,
        Started = 1,
        Finished = 2
    }
}
