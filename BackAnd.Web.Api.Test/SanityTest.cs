using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading;
using System.Diagnostics;
using Backand.Config;
using Newtonsoft.Json;
using System.Collections;
using RestSharp;
using System.Collections.Generic;

namespace BackAnd.Web.Api.Test
{

    [TestClass]
    public class FarmCache
    {
        [TestMethod]
        public void Test()
        {
            string ins1url = GetIns1url();
            string ins2url = GetIns2url();

            string ins1config = GetInsConfig(GetClient1(ins1url));
            string ins2config = GetInsConfig(GetClient2(ins2url));

            Assert.AreEqual(ins1config, ins2config, "In the beginning both instances config should be equal and they are not.");

            string ins1configChanged = ChangeInsConfig(ins1config);
            UpdateConfig(GetClient1(ins1url), ins1configChanged);
            ins2config = GetInsConfig(GetClient2(ins2url));

            Assert.AreNotEqual(ins1config, ins2config, "ins2 config was not changed.");
            Assert.AreEqual(ins1configChanged, ins2config, "In the end both instances config should be equal and they are not.");
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


        private string GetAppName()
        {
            string appName = Backand.Config.ConfigStore.GetConfig().appname;
            string key = Backand.Config.ConfigStore.GetCurrentKey();
                
            if (key == "QA" || key == "PROD")
                return appName + DateTime.Now.Year + DateTime.Now.Month + DateTime.Now.Day + DateTime.Now.Hour + DateTime.Now.Minute + DateTime.Now.Second;

            return appName;
        }

        [TestMethod]
        public void RunAll()
        {
            string appName = "test" + DateTime.Now.Ticks;
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
