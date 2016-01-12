using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading;
using System.Diagnostics;
using Backand.Config;
using Newtonsoft.Json;
using System.Collections;
using RestSharp;
using System.Collections.Generic;
using System.Net;
using System.Security.Cryptography.X509Certificates;
using System.Net.Security;

namespace BackAnd.Web.Api.Test
{

    [TestClass]
    public class FarmCache
    {
        
        //[TestMethod]
        //public void TestConfigUpdate()
        //{
        //    string ins1url = GetIns1url();
        //    string ins2url = GetIns2url();

        //    ClearCache(GetClient1(ins1url));
        //    ClearCache(GetClient2(ins2url));

        //    string ins1config = GetInsConfig(GetClient1(ins1url));
        //    string ins2config = GetInsConfig(GetClient2(ins2url));

        //    Assert.AreEqual(ins1config, ins2config, "In the beginning both instances config should be equal and they are not.");

        //    string ins1configChanged = ChangeInsConfig(ins1config);
        //    UpdateConfig(GetClient1(ins1url), ins1configChanged);

        //    //Thread.Sleep(1000);
        //    ins2config = GetInsConfig(GetClient2(ins2url));

        //    Assert.AreNotEqual(ins1config, ins2config, "ins2 config was not changed.");
        //    Assert.AreEqual(ins1configChanged, ins2config, "In the end both instances config should be equal and they are not.");
        //}

        [TestMethod]
        public void TestModelUpdate()
        {
            string ins1url = GetIns1url();
            string ins2url = GetIns2url();

            ClearCache(GetClient1(ins1url));
            ClearCache(GetClient2(ins2url));

            string ins1model = GetInsModel(GetClient1(ins1url));
            string ins2model = GetInsModel(GetClient2(ins2url));

            Assert.AreEqual(ins1model, ins2model, "In the beginning both instances config should be equal and they are not.");

            string ins1ModelChanged = ChangeInsModel(ins1model);
            UpdateModel(GetClient1(ins1url), ins1ModelChanged);

            //Thread.Sleep(1000);
            ins2model = GetInsModel(GetClient2(ins2url));
            
            Assert.AreNotEqual(ins1model, ins2model, "ins2 config was not changed.");
            
        }

        private void UpdateModel(RestClient client, string model)
        {
            var request = new RestRequest("/1/model", Method.POST);
            //var json = JsonConvert.DeserializeObject(config);
            // request.JsonSerializer = RestSharp.Serialize new JsonSerializer();
            request.RequestFormat = DataFormat.Json;
            request.AddParameter("application/json; charset=utf-8", model, ParameterType.RequestBody);

            // Act
            var response = client.Execute(request);

            // Assert
            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
            }
            else
            {
                Assert.Fail("Fail to update model");
            }

        }

        private string ChangeInsModel(string ins1config)
        {
            string changed = ins1config;
            if (ins1config.Contains("lastName"))
                changed = ins1config.Replace("lastName", "last1Name");
            else if (ins1config.Contains("last1Name"))
                changed = ins1config.Replace("last1Name", "lastName");
            else
                Assert.Fail("Fail to change model");

            return "{newSchema: " + changed + ", severity: 0}";
        }

        private string GetInsModel(RestClient client)
        {
            var request = new RestRequest("/1/model", Method.GET);

            // Act
            var response = client.Execute<Dictionary<string, object>>(request);
            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                return response.Content;
            }
            else
            {
                Assert.Fail("Fail to get model");
            }

            return null;
        }

        private void ClearCache(RestClient client)
        {
            var request = new RestRequest("/1/app/reload", Method.GET);

            // Act
            var response = client.Execute<Dictionary<string, object>>(request);


            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
            }
            else
            {
                Console.WriteLine("status: " + response.StatusCode + "; ErrorMessage:" + response.ErrorMessage);
                Assert.Fail("Fail to clear cache for " + client.BaseUrl + ";\n error: " + response.Content);
            }

        }

        private void UpdateConfig(RestClient client, string config)
        {
            var request = new RestRequest("/1/table/config/items", Method.PUT);
            //var json = JsonConvert.DeserializeObject(config);
           // request.JsonSerializer = RestSharp.Serialize new JsonSerializer();
            request.RequestFormat = DataFormat.Json;
            request.AddParameter("application/json; charset=utf-8", config, ParameterType.RequestBody);
            
            // Act
            var response = client.Execute(request);

            // Assert
            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
            }
            else
            {
                Assert.Fail("Fail to update config");
            }
            
        }

        private string ChangeInsConfig(string ins1config)
        {
            return ins1config.Replace("\"name\":\"description", "\"name\":\"description1");
        }

        private RestClient _client1 = null;
        private RestClient _client2 = null;

        private RestClient GetClient1(string ins1url)
        {
            if (_client1 == null)
            {
                ServerConfig config = Backand.Config.ConfigStore.GetConfig();
                _client1 = new TestUtil().GetAuthentificatedClient(config.appname, config.username, config.pwd, ins1url);
            }
            return _client1;
        }

        private RestClient GetClient2(string ins2url)
        {
            if (_client2 == null)
            {
                ServerConfig config = Backand.Config.ConfigStore.GetConfig();
                _client2 = new TestUtil().GetAuthentificatedClient(config.appname, config.username, config.pwd, ins2url);
            }
            return _client2;
        }

        private string GetInsConfig(RestClient client)
        {
            var request = new RestRequest("/1/table/config/items", Method.GET);
            
            // Act
            var response = client.Execute<Dictionary<string,object>>(request);
            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                return response.Content;
            }
            else
            {
                Assert.Fail("Fail to get config");
            }

            return null;
        }

        private string GetIns2url()
        {
            return Backand.Config.ConfigStore.GetConfig().ins2url;
        }

        private string GetIns1url()
        {
            return Backand.Config.ConfigStore.GetConfig().ins1url;
        }

    }

    [TestClass]
    public class SanityTest
    {

        private TestContext m_testContext;
        public TestContext TestContext
        {
            get { return m_testContext; }
            set { m_testContext = value; }
        }


        private string GetAppName(bool random = false)
        {
            string appName = Backand.Config.ConfigStore.GetConfig().appname;
            string key = Backand.Config.ConfigStore.GetCurrentKey();
                
            if (key == "QA" || key == "PROD" || random)
                return appName + DateTime.Now.Year + DateTime.Now.Month + DateTime.Now.Day + DateTime.Now.Hour + DateTime.Now.Minute + DateTime.Now.Second;

            return appName;
        }

        [TestMethod]
        public void RunAll()
        {
            string appName = GetAppName(true);
            AdminContext admin = new AdminContext();
                admin.CreateConnectAndWaitUntilAppIsReady(appName)
                .CreateJsInTransaction(appName);
            
            CrudUtility crud = new CrudUtility(appName, Backand.Config.ConfigStore.GetConfig().username, Backand.Config.ConfigStore.GetConfig().pwd);
                crud.RunSimpleCrud();
        }

        

        //[TestMethod]
        //public void SignupCreateAppDeleteAppDeleteUser()
        //{
        //    string appName = GetAppName();
        //    string username = GetUsername();
        //    string password = Backand.Config.ConfigStore.GetConfig().pwd;

        //    AdminContext admin = new AdminContext();
        //    admin.Signup(username, password)
        //        .CreateConnectAndWaitUntilAppIsReady(appName)
        //        .RunSimpleCrud(appName, username, password)
        //        .DeleteApp(appName)
        //        .DeleteUser(username);
        //}

        [TestCleanup]
        public void Cleanup()
        {
            if (TestFailed())
            {
                OutputStatus();
            }
        }



        private void OutputStatus()
        {
            Trace.WriteLine("CurrentKey " + ConfigStore.GetCurrentKey());
            Trace.WriteLine("CurrentConfig " + JsonConvert.SerializeObject(ConfigStore.GetConfig()));

            Trace.WriteLine("GetEnvironmentVariables: ");
            foreach (DictionaryEntry de in Environment.GetEnvironmentVariables())
                Trace.WriteLine(string.Format("  {0} = {1}", de.Key, de.Value));
        }

        private bool TestFailed()
        {
           // return TestContext.CurrentTestOutcome == UnitTestOutcome.Failed;
            return true;
        }



        //[TestMethod]
        //public void SignupToBackand()
        //{

        //}

            
        //[TestMethod]
        //public void CreateApp()
        //{
        //    //ManualResetEvent mre = new ManualResetEvent(false);

        //    //new SocketHelper().On("applicationReady", (data) =>
        //    //{
        //    //    // todo: serialize    
        //    //    // Assert appName is true

        //    //    mre.Set();
        //    //});

        //    //AdminUtility admin = new AdminUtility(username, password, env);
        //    //admin.CreateApp(appName, appName);
        //    //admin.ConnectApp(appName);
        //    //admin.WaitUntilAppIsReady(appName);


        //    //var res = mre.WaitOne(TimeSpan.FromMinutes(10));

        //    //Assert.IsTrue(res);

        //    string appName = "app206";
        //    AdminContext admin = new AdminContext(username, password, env);
        //    admin.CreateConnectAndWaitUntilAppIsReady(appName);
        //}


        //[TestMethod]
        //public void ChangeSchema()
        //{

        //}

        //[TestMethod]
        //public void Crud()
        //{
        //    string appName = "app204";
        //    CrudUtility crud = new CrudUtility(appName, username, password, env);
        //    crud.RunSimpleCrud();
        //}

        //[TestMethod]
        //public void CrudInTransactionAndCreateDeleteAndEditActions()
        //{

        //}

        //[TestMethod]
        //public void AddAppUser()
        //{

        //}

        //[TestMethod]
        //public void CrudWithUser()
        //{

        //}

        //[TestMethod]
        //public void CrudDeepWithUser()
        //{

        //}

        //[TestMethod]
        //public void CreateAndRunQueries()
        //{

        //}

        //[TestMethod]
        //public void CreateAllTemplateActions()
        //{

        //}

        //[TestMethod]
        //public void RunAllTemplateActionsAsUser()
        //{

        //}

        //[TestMethod]
        //public void EditAndDeleteAppUser()
        //{

        //}


        //[TestMethod]
        //public void DeleteApp()
        //{

        //}

        //[TestMethod]
        //public void DeleteBackandUser()
        //{

        //}

        
    }

}
