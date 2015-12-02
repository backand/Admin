using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading;
using System.Diagnostics;
using Backand.Config;
using Newtonsoft.Json;
using System.Collections;

namespace BackAnd.Web.Api.Test
{
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
            string appName = GetAppName();
            AdminContext admin = new AdminContext();
                admin.CreateConnectAndWaitUntilAppIsReady(appName)
                .CreateJsInTransaction(appName);
            
            CrudUtility crud = new CrudUtility(appName, Backand.Config.ConfigStore.GetConfig().username, Backand.Config.ConfigStore.GetConfig().pwd);
                crud.RunSimpleCrud();
        }

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
