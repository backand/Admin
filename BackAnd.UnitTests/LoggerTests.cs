using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Durados.Web.Mvc.Logging;
using Durados.Web.Mvc;
using Durados.DataAccess;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Data.SqlClient;

namespace BackAnd.UnitTests
{
    [TestClass]
    public class LoggerTests : BackandBaseTest
    {
        [TestInitialize]
        public void Init()
        {
            base.InitInner();
        }


        [TestCleanup]
        public void CleanUp()
        {
            base.CleanupInner();
        }

        [TestMethod]
        public void TestCanWriteToSpecificAppLogger()
        {
            var controller = "contrller";
            var action = "action";
            var method = "method";
            var message = "message";
            var trace = "trace";
            var logType = 3;
            var freeText = "freeText";
            var time = DateTime.Now;
            Guid? guid = Guid.NewGuid();
            Log log = new Log();
            var applicationName = "applicationName";
            var username = "username";

            this.ValidMap.Logger.WriteToSpecificAppLog(controller, action, method, message, trace, logType, freeText, time, guid, log, applicationName, username);


            View view = (View)this.ValidMap.Database.Views["Durados_Log"];
            Assert.IsFalse(view == null, "Durados_Log is mission in map");

            DataView dataView = FetchLogEntry(guid, view);

            Assert.AreEqual(dataView[0].Row[1], applicationName);
            Assert.AreEqual(dataView[0].Row[2], username);
            //Assert.AreEqual(dataView[0].Row[3], machineNam);
            //Assert.AreEqual(dataView[0].Row[4], time);
            Assert.AreEqual(dataView[0].Row[5], controller);
            Assert.AreEqual(dataView[0].Row[6], action);
            Assert.AreEqual(dataView[0].Row[7], method);
            Assert.AreEqual(dataView[0].Row[8], logType);
            Assert.AreEqual(dataView[0].Row[9], message);
            Assert.AreEqual(dataView[0].Row[10], trace);
            Assert.AreEqual(dataView[0].Row[11], freeText);

            Assert.AreEqual(dataView.Count, 1);
        }




        [TestMethod]
        public void TestCanWriteToReportLogger()
        {

            var controller = "contrller";
            var action = "action";
            var method = "method";
            var message = "message";
            var trace = "trace";
            var logType = 3;
            var freeText = "freeText";
            var time = DateTime.Now;
            Guid? guid = Guid.NewGuid();
            Log log = new Log();
            var applicationName = "applicationName";
            var username = "username";

            var appName = "appName";
            var clientIp = "clientIP";
            var clientInfo = "clientInfo";

            this.ValidMap.Logger.WriteToReportLogger(controller, action, method, message, trace, logType, freeText, time, guid, log, applicationName, username,
                appName, clientIp, clientInfo);

            var reportConnectionString = System.Web.Configuration.WebConfigurationManager.ConnectionStrings["reportConnectionString"].ConnectionString;

            using (SqlConnection sqlConnection = new SqlConnection(reportConnectionString))
            {
                using (SqlCommand command = new SqlCommand(string.Format(commandText, guid.ToString()), sqlConnection))
                {
                    sqlConnection.Open();

                    using (SqlDataReader reader = command.ExecuteReader())
                    {

                        if (!reader.Read())
                        {
                            Assert.IsTrue(true, "Did not find the guid in the log");
                        }
                    }
                }
            }
        }


        [TestMethod]
        public void TestCanWriteToKibana()
        {
            var controller = "contrller";
            var action = "action";
            var method = "method";
            var message = "message";
            var trace = "trace";
            var logType = 3;
            var freeText = "freeText";
            var time = DateTime.Now;
            Guid? guid = Guid.NewGuid();
            Log log = new Log();
            var applicationName = "applicationName";
            var username = "username";

            var appName = "appName";
            var clientIp = "clientIP";
            var clientInfo = "clientInfo";

            this.ValidMap.Logger.WriteToLogstash(controller, action, method, message, trace, logType, freeText, time, guid, log, applicationName, username,
                appName, clientIp, clientInfo);
        }

        private static DataView FetchLogEntry(Guid? guid, View view)
        {
            view.PermanentFilter = "Guid = '" + guid.ToString() + "'";
            int rowCount = 0;
            Dictionary<string, object> filter = new Dictionary<string, object>();
            DataView dataView = view.FillPage(1, 2, filter, false, null, out rowCount, null, null);
            return dataView;
        }



        public string commandText
        {
            get
            {
                return "select * from [Durados_Log] where guid = '{0}'";
            }
        }
    }
}
