using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading;

namespace BackAnd.Web.Api.Test
{
    [TestClass]
    public class SanetyTest
    {
        string mainApp = "www";
        string appName = "app205";
        string username = "relly@backand.com";
        string password = "123456";
        Envirement env = Envirement.Dev;
        
        [TestMethod]
        public void SignupToBackand()
        {

        }

            
        [TestMethod]
        public void CreateApp()
        {
            //ManualResetEvent mre = new ManualResetEvent(false);

            //new SocketHelper().On("applicationReady", (data) =>
            //{
            //    // todo: serialize    
            //    // Assert appName is true

            //    mre.Set();
            //});

            AdminUtility admin = new AdminUtility(mainApp, username, password, env);
            admin.CreateApp(appName, appName);
            admin.ConnectApp(appName);
            admin.WaitUntilAppIsReady(appName);


            //var res = mre.WaitOne(TimeSpan.FromMinutes(10));

            //Assert.IsTrue(res);
        }


        [TestMethod]
        public void ChangeSchema()
        {

        }

        [TestMethod]
        public void Crud()
        {
            CrudUtility crud = new CrudUtility(appName, username, password, env);
            crud.RunSimpleCrud();
        }

        [TestMethod]
        public void CrudInTransactionAndCreateDeleteAndEditActions()
        {

        }

        [TestMethod]
        public void AddAppUser()
        {

        }

        [TestMethod]
        public void CrudWithUser()
        {

        }

        [TestMethod]
        public void CrudDeepWithUser()
        {

        }

        [TestMethod]
        public void CreateAndRunQueries()
        {

        }

        [TestMethod]
        public void CreateAllTemplateActions()
        {

        }

        [TestMethod]
        public void RunAllTemplateActionsAsUser()
        {

        }

        [TestMethod]
        public void EditAndDeleteAppUser()
        {

        }


        [TestMethod]
        public void DeleteApp()
        {

        }

        [TestMethod]
        public void DeleteBackandUser()
        {

        }

        
    }
}
