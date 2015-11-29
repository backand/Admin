using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RestSharp;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Threading;

namespace BackAnd.Web.Api.Test
{


    public class AdminUtility
    {
        public Actions Actions { get; private set; }
        public Objects Objects { get; private set; }

        internal RestClient client = null;

        private string GetMainAppName()
        {
            return Backand.Config.ConfigStore.GetConfig().mainAppName;
        }
        public AdminUtility(string username, string password)
        {
            client = new TestUtil().GetAuthentificatedClient(GetMainAppName(), username, password);
            Actions = new Actions(this);
            Objects = new Objects(this);
        }

        public void SetCurrentAppName(string appName)
        {
            client.AddDefaultHeader("AppName", appName);
           
        }

        public IRestResponse CreateApp(string name, string title)
        {
            // Arrange
            var data = new { Name = name, Title = title };
            var request = new RestRequest("/admin/myApps", Method.POST);
            request.RequestFormat = DataFormat.Json;
            request.AddBody(data);
            
            // Act
            var response = client.Execute(request);
            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                var result = JsonConvert.DeserializeObject<Dictionary<string, object>>(response.Content);

                // Assert
                Assert.IsTrue(result.ContainsKey("__metadata"));
                Assert.IsTrue(result["__metadata"] is Newtonsoft.Json.Linq.JObject);
                
            }
            return response;
        }

        public List<Dictionary<string, object>> GetDefaultSchema()
        {
            var schema = new List<Dictionary<string, object>>() 
            { 
                new Dictionary<string, object>() 
                { 
                    { 
                        "name", "items" 
                    }, 
                    { 
                        "fields", new Dictionary<string, object>() 
                        { 
                            { 
                                "name", new Dictionary<string, object>() 
                                { 
                                    {
                                        "type", "string"
                                    }
                                }
                            },
                            { 
                                "description", new Dictionary<string, object>() 
                                { 
                                    {
                                        "type", "text"
                                    }
                                }
                            },
                            { 
                                "user", new Dictionary<string, object>() 
                                { 
                                    {
                                        "object", "users"
                                    }
                                }
                            }
                        } 
                    } 
                },
                new Dictionary<string, object>() 
                { 
                    { 
                        "name", "users" 
                    }, 
                    { 
                        "fields", new Dictionary<string, object>() 
                        { 
                            { 
                                "email", new Dictionary<string, object>() 
                                { 
                                    {
                                        "type", "string"
                                    }
                                }
                            },
                            { 
                                "firstName", new Dictionary<string, object>() 
                                { 
                                    {
                                        "type", "string"
                                    }
                                }
                            },
                            { 
                                "lastName", new Dictionary<string, object>() 
                                { 
                                    {
                                        "type", "string"
                                    }
                                }
                            },
                            { 
                                "items", new Dictionary<string, object>() 
                                { 
                                    {
                                        "collection", "items"
                                    },
                                    {
                                        "via", "user"
                                    }
                                }
                            }
                        } 
                    } 
                } 
            };
            return schema;
        }

        public IRestResponse ConnectApp(string name)
        {
            //string schema = "[" +
            //  "{" +
            //    "\"name\": \"items\"," +
            //    "\"fields\": {" +
            //      "\"name\": {" +
            //        "\"type\": \"string\"" +
            //      "}," +
            //      "\"description\": {" +
            //        "\"type\": \"text\"" +
            //      "}," +
            //      "\"user\": {" +
            //        "\"object\": \"users\"" +
            //      "}" +
            //    "}" +
            //  "}," +
            //  "{" +
            //    "\"name\": \"users\"," +
            //    "'\"fields\": {" +
            //      "\"email\": {" +
            //        "\"type\": \"string\"" +
            //      "}," +
            //      "\"firstName\": {" +
            //        "\"type\": \"string\"" +
            //      "}," +
            //      "\"lastName\": {" +
            //        "\"type\": \"string\"" +
            //      "}," +
            //      "\"items\": {" +
            //        "\"collection\": \"items\"," +
            //        "\"via\": \"user\"" +
            //      "}" +
            //    "}" +
            //  "}" +
            //"]";

            
            

//            {"product":"10","sampleApp":"","schema":[{
   
//    name:"yyy",
//    fields:[{name:"c1", type:"ShortText"},{name:"c2",type:"SingleSelect"}]
//}]}

            return ConnectApp(name, GetDefaultSchema());
        }

        public IRestResponse ConnectApp(string name, List<Dictionary<string, object>> schema)
        {
            // Arrange
            var data = new {name = name, product= "10", schema = schema };
            var request = new RestRequest("/admin/myAppConnection/{name}", Method.POST);
            request.AddUrlSegment("name", name);
            request.RequestFormat = DataFormat.Json;
            request.AddBody(data);
            request.Timeout = 1000 * 60 * 10;
            // Act
            var response = client.Execute(request);
            // Assert
            Assert.IsTrue(response.StatusCode == System.Net.HttpStatusCode.OK, response.Content);
            
            return response;
        }

        public IRestResponse ChangeModel(List<Dictionary<string, object>> schema)
        {
            // Arrange
            var request = new RestRequest("/1/model", Method.POST);
            request.RequestFormat = DataFormat.Json;
            request.AddBody(new Dictionary<string, object>() { { "newSchema", schema }, { "severity", 0 } });
            request.Timeout = 1000 * 60 * 10;
            // Act
            var response = client.Execute(request);
            // Assert
            Assert.IsTrue(response.StatusCode == System.Net.HttpStatusCode.OK, response.Content);

            return response;
        }


        public IRestResponse GetAppStatus(string name)
        {
            // Arrange
            var request = new RestRequest("/admin/myAppConnection/status/{name}", Method.GET);
            request.AddUrlSegment("name", name);
            request.RequestFormat = DataFormat.Json;
            // Act
            var response = client.Execute(request);
            // Assert
            Assert.IsTrue(response.StatusCode == System.Net.HttpStatusCode.OK, response.Content);

            return response;
        }

        public IRestResponse WaitUntilAppIsReady(string name, int firstWait = 60, int interval = 10, int totalWait = 600)
        {
            Thread.Sleep(interval * 1000);
            IRestResponse response = GetAppStatus(name);
            string status = GetStatus(response);
            if (status != OnBoardingStatus.Processing.ToString())
            {
                return response;
            }
            Thread.Sleep(firstWait * 1000);
            DateTime start = DateTime.Now;

            bool tooLong = false;
            
            while (!tooLong)
            {
                response = GetAppStatus(name);
                status = GetStatus(response);
                if (status != OnBoardingStatus.Processing.ToString())
                {
                    return response;
                }
                Thread.Sleep(interval * 1000);
                tooLong = DateTime.Now.Subtract(start).TotalMinutes > totalWait;
            }
            Exception exception = new Exception("Too much time has passed");
            Assert.Fail(exception.Message);
            throw exception;
        }

        private string GetStatus(IRestResponse response)
        {
            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                Dictionary<string, object> data = null;
                try
                {
                    data = JsonConvert.DeserializeObject<Dictionary<string, object>>(response.Content);
                }
                catch(Exception exception)
                {
                    Assert.Fail(exception.Message);
                }
                Assert.IsTrue(data.ContainsKey("status"), "response does not contain status");
                return data["status"].ToString();
            }
            else
            {
                return response.StatusCode.ToString();
            }
        }
    }

    public class Objects : Config
    {
        public Objects(AdminUtility admin)
            : base(admin)
        {
        }

        public override string Route
        {
            get { return "/1/view/config"; }
        }


    }

    public class Actions : Config
    {
        public Actions(AdminUtility admin)
            : base(admin)
        {
        }

        public override string Route
        {
            get { return "/1/action/config"; }
        }

        public IRestResponse Post(ActionData data)
        {
            return base.Post(data);
        }

        public IRestResponse Post(string name, string insideCode, string objectId, TriggerDataAction triggerDataAction, WorkflowAction workflowAction)
        {
            string code = "/* globals\n" +
            "$http - Service for AJAX calls \n" +
            "CONSTS - CONSTS.apiUrl for Backands API URL\n" +
            "Config - Global Configuration\n" +
            "*/\n" +
            "'use strict';\n" +
            "function backandCallback(userInput, dbRow, parameters, userProfile) {\n" +
            "   \n" +
            insideCode +
            "}";

            code = code.Replace("+", "%2B");

            return base.Post(new ActionData() { name = name, code = code, useSqlParser = false, viewTable = objectId, workflowAction = workflowAction.ToString(), dataAction = triggerDataAction.ToString(), additionalView = "", databaseViewName = "", inputParameters = "", whereCondition = "true" });
        }


        public IRestResponse Put(string id, ActionData data)
        {
            return base.Put(id, data);
        }

        public IRestResponse[] DeleteAll(string objectId = null)
        {
            List<IRestResponse> responses = new List<IRestResponse>();
            FilterItem[] filter = null;
            if (objectId != null)
            {
                filter = new FilterItem[1] { new FilterItem() { fieldName = "Rules_Parent", @operator = FilterOperandType.@in.ToString(), value = objectId } };
            }
            var response = GetAll(null, null, null, filter);
           
            var result = JsonConvert.DeserializeObject<Dictionary<string, object>>(response.Content);
            Assert.IsTrue(result.ContainsKey("data"));
            Newtonsoft.Json.Linq.JArray data = (Newtonsoft.Json.Linq.JArray)result["data"];
            if (data.Count > 0)
            {
                foreach (Newtonsoft.Json.Linq.JObject jobject in data)
                {
                    Newtonsoft.Json.Linq.JToken jtoken = jobject["__metadata"];
                    string id = jtoken["id"].ToString();
                    var responseDelete = base.Delete(id);
                    responses.Add(responseDelete);
                }

            }
            return responses.ToArray();
        }

        public string GetPostActionTemplate(string objectName, string varName, Dictionary<string, object> data, Dictionary<string, object> parameters = null)
        {
            string code = "   var " + varName + " = $http({\n" +
        "      method: \"POST\",\n" +
        "      url:CONSTS.apiUrl + \"/1/objects/" + objectName + "\",\n" +
        "      parameters: " + JsonConvert.SerializeObject(parameters ?? new Dictionary<string, object>()) + ",\n" +
        "      data: " + JsonConvert.SerializeObject(data ?? new Dictionary<string, object>()) + ",\n" +
        "      headers: {\"Authorization\": userProfile.token}\n" +
        "   });";
            return code;
        }

        public string GetPutActionTemplate(string objectName, string id, string varName, Dictionary<string, object> data, Dictionary<string, object> parameters = null)
        {
            string code = "   var " + varName + " = $http({\n" +
        "      method: \"PUT\",\n" +
        "      url:CONSTS.apiUrl + \"/1/objects/" + objectName + "/" + id + "\",\n" +
        "      parameters: " + JsonConvert.SerializeObject(parameters ?? new Dictionary<string, object>()) + ",\n" +
        "      data: " + JsonConvert.SerializeObject(data ?? new Dictionary<string, object>()) + ",\n" +
        "      headers: {\"Authorization\": userProfile.token}\n" +
        "   });";
            return code;
        }

        public string GetDeleteActionTemplate(string objectName, string id, string varName, Dictionary<string, object> parameters = null)
        {
            string code = "   var " + varName + " = $http({\n" +
        "      method: \"DELETE\",\n" +
        "      url:CONSTS.apiUrl + \"/1/objects/" + objectName + "/" + id + "\",\n" +
        "      parameters: " + JsonConvert.SerializeObject(parameters ?? new Dictionary<string, object>()) + ",\n" +
        "      headers: {\"Authorization\": userProfile.token}\n" +
        "   });";
            return code;
        }

        public string GetOneActionTemplate(string objectName, string id, string varName, Dictionary<string, object> parameters = null)
        {
            string code = "   var " + varName + " = $http({\n" +
        "      method: \"GET\",\n" +
        "      url:CONSTS.apiUrl + \"/1/objects/" + objectName + "/" + id + "\",\n" +
        "      parameters: " + JsonConvert.SerializeObject(parameters ?? new Dictionary<string, object>()) + ",\n" +
        "      headers: {\"Authorization\": userProfile.token}\n" +
        "   });";
            return code;
        }

        public string GetAllActionTemplate(string objectName, string varName, Dictionary<string, object> parameters = null)
        {
            string code = "   var " + varName + " = $http({\n" +
        "      method: \"GET\",\n" +
        "      url:CONSTS.apiUrl + \"/1/objects/" + objectName + "\",\n" +
        "      parameters: " + JsonConvert.SerializeObject(parameters ?? new Dictionary<string, object>()) + ",\n" +
        "      headers: {\"Authorization\": userProfile.token}\n" +
        "   });";
            return code;
        }


    }

    public abstract class Config
    {
        protected AdminUtility admin = null;
        public Config(AdminUtility admin)
        {
            this.admin = admin;
        }
        public abstract string Route
        {
            get;
        }
        public IRestResponse GetAll(bool? withSelectOptions = null, int? pageNumber = null, int? pageSize = null, FilterItem[] filter = null, SortItem[] sort = null, string search = null)
        {
            // Arrange
            var client = new TestUtil().GetAuthentificatedClient(Backand.Config.ConfigStore.GetConfig().appname, Backand.Config.ConfigStore.GetConfig().username, Backand.Config.ConfigStore.GetConfig().pwd);
            var request = new RestRequest(Route, Method.GET);
            if (pageNumber.HasValue)
                request.AddParameter("pageNumber", pageNumber.Value, ParameterType.QueryString);
            if (pageSize.HasValue)
                request.AddParameter("pageSize", pageSize.Value, ParameterType.QueryString);
            if (filter != null)
                request.AddParameter("filter", JsonConvert.SerializeObject(filter), ParameterType.QueryString);
            if (sort != null)
                request.AddParameter("sort", JsonConvert.SerializeObject(sort), ParameterType.QueryString);
            if (search != null)
                request.AddParameter("search", search, ParameterType.QueryString);

            // Act
            var response = client.Execute(request);
            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                var result = JsonConvert.DeserializeObject<Dictionary<string, object>>(response.Content);

                // Assert
                Assert.IsTrue(result.ContainsKey("data"));
                Assert.IsTrue(result.ContainsKey("totalRows"));
            }
            return response;
        }


        public IRestResponse GetOne(string id)
        {
            // Arrange
            var client = new TestUtil().GetAuthentificatedClient(Backand.Config.ConfigStore.GetConfig().appname, Backand.Config.ConfigStore.GetConfig().username, Backand.Config.ConfigStore.GetConfig().pwd);
            var request = new RestRequest(Route + "/{id}", Method.GET);
            request.AddUrlSegment("id", id);

            // Act
            var response = client.Execute(request);
            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                var result = JsonConvert.DeserializeObject<Dictionary<string, object>>(response.Content);

                // Assert
                Assert.IsTrue(result.ContainsKey("__metadata"));
                Assert.IsTrue(result["__metadata"] is Newtonsoft.Json.Linq.JObject);
                Assert.IsTrue(((Newtonsoft.Json.Linq.JObject)result["__metadata"])["id"].ToString().Equals(id));
            }

            return response;
        }


        public IRestResponse Post(object data)
        {
            // Arrange
            var client = new TestUtil().GetAuthentificatedClient(Backand.Config.ConfigStore.GetConfig().appname, Backand.Config.ConfigStore.GetConfig().username, Backand.Config.ConfigStore.GetConfig().pwd);
            var request = new RestRequest(Route, Method.POST);
            request.RequestFormat = DataFormat.Json;
            request.AddBody(data);

            // Act
            var response = client.Execute(request);
            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                var result = JsonConvert.DeserializeObject<Dictionary<string, object>>(response.Content);

                // Assert
                Assert.IsTrue(result.ContainsKey("__metadata"));
                Assert.IsTrue(result["__metadata"] is Newtonsoft.Json.Linq.JObject);
            }
            return response;
        }

        public IRestResponse Put(string id, object data)
        {
            // Arrange
            var client = new TestUtil().GetAuthentificatedClient(Backand.Config.ConfigStore.GetConfig().appname, Backand.Config.ConfigStore.GetConfig().username, Backand.Config.ConfigStore.GetConfig().pwd);
            var request = new RestRequest(Route + "/{id}", Method.PUT);
            request.AddUrlSegment("id", id);
            request.RequestFormat = DataFormat.Json;
            request.AddBody(data);

            // Act
            var response = client.Execute(request);

            // Assert
            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                Assert.IsTrue(string.IsNullOrEmpty(response.Content));

            }

            return response;

        }


        public IRestResponse Delete(string id)
        {
            // Arrange
            var client = new TestUtil().GetAuthentificatedClient(Backand.Config.ConfigStore.GetConfig().appname, Backand.Config.ConfigStore.GetConfig().username, Backand.Config.ConfigStore.GetConfig().pwd);
            var request = new RestRequest(Route + "/{id}", Method.DELETE);
            request.AddUrlSegment("id", id);

            // Act
            var response = client.Execute(request);

            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                var result = JsonConvert.DeserializeObject<Dictionary<string, object>>(response.Content);

                Assert.IsTrue(result.ContainsKey("__metadata"));
                Assert.IsTrue(result["__metadata"] is Newtonsoft.Json.Linq.JObject);
                Assert.IsTrue(((Newtonsoft.Json.Linq.JObject)result["__metadata"])["id"].ToString().Equals(id));
            }

            return response;
        }

        public virtual IRestResponse GetId(string name, out string id)
        {
            var filter = new FilterItem[1] { new FilterItem() { fieldName = "name", @operator = FilterOperandType.equals.ToString(), value = name } };
            var response = GetAll(null, null, null, filter);
            id = null;
            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                var result = JsonConvert.DeserializeObject<Dictionary<string, object>>(response.Content);
                Newtonsoft.Json.Linq.JArray data = (Newtonsoft.Json.Linq.JArray)result["data"];
                if (data.Count > 0)
                {
                    Newtonsoft.Json.Linq.JObject jobject = (Newtonsoft.Json.Linq.JObject)data[0];
                    Newtonsoft.Json.Linq.JToken jtoken = jobject["__metadata"];
                    id = jtoken["id"].ToString();
                }
            }

            return response;
        }

    }

    public class ActionData
    {
        public string viewTable { get; set; }
        public string additionalView { get; set; }
        public string databaseViewName { get; set; }
        public bool useSqlParser { get; set; }
        public string whereCondition { get; set; }
        public string code { get; set; }
        public string dataAction { get; set; }
        public string workflowAction { get; set; }
        public string name { get; set; }
        public string inputParameters { get; set; }
    }

    
    public enum TriggerDataAction
    {
        BeforeCreate = 0,
        BeforeEdit = 1,
        BeforeDelete = 2,
        AfterCreate = 4,
        AfterEdit = 8,
        AfterDelete = 32,
        AfterCreateBeforeCommit = 64,
        AfterEditBeforeCommit = 128,
        AfterDeleteBeforeCommit = 256,
        OnDemand = 16384
    }

    public enum WorkflowAction
    {
        Notify,
        // Task,
        Validate,
        Execute,
        WebService,
        CompleteStep,
        Approval,
        Document,
        Xml,
        Custom,
        JavaScript
    }

    public enum OnBoardingStatus
    {
        NotStarted = 0,
        Ready = 1,
        Processing = 2,
        Error = 3
    }
}
