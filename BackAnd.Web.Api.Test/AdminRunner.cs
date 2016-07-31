using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BackObject = System.Collections.Generic.Dictionary<string, object>;

namespace BackAnd.Web.Api.Test
{
    public class AdminContext
    {
        public AdminUtility admin = null;

        public string AppName { get; set; }

        public AdminContext()
            : this(Backand.Config.ConfigStore.GetConfig().username, Backand.Config.ConfigStore.GetConfig().pwd)
        {

        }
        public AdminContext(string username, string password)
        {
            admin = new AdminUtility(username, password);
        }

        public List<BackObject> Schema { get; set; }

        
    }

    public static class JsInTransactionHelper
    {
        public static readonly string ObjectNameUpdated = "items2";
        public static readonly string ObjectName = "items";
        public static Dictionary<string, object> data = new Dictionary<string, object>() { { "name", "name1" }, { "description", "description1" } };

        public static List<Dictionary<string, object>> GetSchema(List<Dictionary<string, object>> defaultSchema)
        {
            defaultSchema.Add(GetSchemaObject());
            return defaultSchema;
        }
        private static Dictionary<string, object> GetSchemaObject()
        {
            return new Dictionary<string, object>() 
                { 
                    { 
                        "name", ObjectNameUpdated
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
                            }
                        } 
                    } 
                };
        }
    }

    
    public static class AdminRunnerExtension
    {
        public static AdminContext CreateApp(this AdminContext runner, string appName)
        {
            var response = runner.admin.CreateApp(appName, appName);
            Assert.IsTrue(response.StatusCode == System.Net.HttpStatusCode.OK);
            runner.AppName = appName;

            return runner;
        }

        public static AdminContext ConnectApp(this AdminContext runner, List<Dictionary<string, object>> schema = null)
        {
            if (schema == null)
                schema = runner.admin.GetDefaultSchema();
            runner.Schema = schema;
            runner.admin.ConnectApp(runner.AppName, schema); 
            return runner;
        }

        public static AdminContext WaitUntilAppIsReady(this AdminContext runner)
        {
            runner.admin.WaitUntilAppIsReady(runner.AppName);
            return runner;
        }

        public static AdminContext CreateConnectAndWaitUntilAppIsReady(this AdminContext runner, string appName)
        {
            return runner.CreateApp(appName)
                .ConnectApp()
                .WaitUntilAppIsReady();
        }

        public static AdminContext Signup(string username, string password)
        {
            throw new NotImplementedException();
        }

        public static AdminContext SetCurrentAppName(this AdminContext runner, string appName)
        {
            runner.admin.SetCurrentAppName(appName);
            return runner;
        }

        public static AdminContext ChangeModel(this AdminContext runner, List<Dictionary<string, object>> schema)
        {
            runner.admin.ChangeModel(schema);
            return runner;
        }

        

        public static AdminContext CreateJsInTransaction(this AdminContext runner, string appName)
        {
            runner.SetCurrentAppName(appName)
            .ChangeModel(JsInTransactionHelper.GetSchema(runner.admin.GetDefaultSchema()));

            string objectNameUpdated = JsInTransactionHelper.ObjectNameUpdated;
            string objectName = JsInTransactionHelper.ObjectName;
            string objectId = null;
            Dictionary<string, object> data = JsInTransactionHelper.data;

            var getObjectIdResponse = runner.admin.Objects.GetId(objectName, out objectId);
            Assert.IsTrue(getObjectIdResponse.StatusCode == System.Net.HttpStatusCode.OK);


            string code = "\n" +
                runner.admin.Actions.GetPostActionTemplate(objectNameUpdated, "post1", data) + "\n" +
                runner.admin.Actions.GetPutActionTemplate(objectNameUpdated, "\" + post1.__metadata.id + \"", "put1", data) + "\n" +
                runner.admin.Actions.GetDeleteActionTemplate(objectNameUpdated, "\" + post1.__metadata.id + \"", "delete1") + "\n";

            foreach (TriggerDataAction trigger in Enum.GetValues(typeof(TriggerDataAction)))
            {
                string name = objectName + trigger.ToString();
                string actionId = null;
                var getActionIdResponse = runner.admin.Actions.GetId(name, out actionId);
                Assert.IsTrue(getActionIdResponse.StatusCode == System.Net.HttpStatusCode.OK);
                if (actionId != null)
                {
                    var getDeleteActionResponse = runner.admin.Actions.Delete(actionId);
                    Assert.IsTrue(getActionIdResponse.StatusCode == System.Net.HttpStatusCode.OK);
                }
                var createActionResponse = runner.admin.Actions.Post(name, code, objectId, trigger, WorkflowAction.JavaScript);
                Assert.IsTrue(createActionResponse.StatusCode == System.Net.HttpStatusCode.OK);
            }
            return runner;
        }
    }
}
