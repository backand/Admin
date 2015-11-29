using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RestSharp;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace BackAnd.Web.Api.Test
{
    [TestClass]
    public class CrudItems
    {
        string objectName = "items";
        string id = "1";
        string appName = "app202";
        string username = "relly@backand.com";
        string password = "123456";
        
        CrudUtility crud = new CrudUtility();
        AdminUtility admin = null;
        Dictionary<string, object> data = new Dictionary<string, object>() { { "name", "name1" }, { "description", "description1" } };

        public CrudItems()
        {
            this.admin = new AdminUtility(appName, username, password);
        }


        [TestMethod]
        public void RunJsInTransaction()
        {
            string objectNameUpdated = "items2";
            string objectId = null; 
            var getObjectIdResponse = admin.Objects.GetId(objectName, out objectId);
            Assert.IsTrue(getObjectIdResponse.StatusCode == System.Net.HttpStatusCode.OK);


            string code = "\n" +
                admin.Actions.GetPostActionTemplate(objectNameUpdated, "post1", data) + "\n" +
                admin.Actions.GetPutActionTemplate(objectNameUpdated, "\" + post1.__metadata.id + \"", "put1", data) + "\n" +
                admin.Actions.GetDeleteActionTemplate(objectNameUpdated, "\" + post1.__metadata.id + \"", "delete1") + "\n";

            foreach (TriggerDataAction trigger in Enum.GetValues(typeof(TriggerDataAction)))
            {
                string name = objectName + trigger.ToString();
                string actionId = null;
                var getActionIdResponse = admin.Actions.GetId(name, out actionId);
                Assert.IsTrue(getActionIdResponse.StatusCode == System.Net.HttpStatusCode.OK);
                if (actionId != null)
                {
                    var getDeleteActionResponse = admin.Actions.Delete(actionId);
                    Assert.IsTrue(getActionIdResponse.StatusCode == System.Net.HttpStatusCode.OK);
                }
                var createActionResponse = admin.Actions.Post(name, code, objectId, trigger, WorkflowAction.JavaScript);
                Assert.IsTrue(createActionResponse.StatusCode == System.Net.HttpStatusCode.OK);
            }

           // RunSimpleCrud();
        }

        [TestMethod]
        public void Endless()
        {
            
            string objectNameUpdated = "items2";
            string objectId = null;
            var getObjectIdResponse = admin.Objects.GetId(objectNameUpdated, out objectId);
            Assert.IsTrue(getObjectIdResponse.StatusCode == System.Net.HttpStatusCode.OK);
            admin.Actions.DeleteAll(objectId);


            string code = "\n" +
                admin.Actions.GetPostActionTemplate(objectNameUpdated, "post1", data) + "\n";

            TriggerDataAction trigger = TriggerDataAction.AfterCreateBeforeCommit;
            string name = objectNameUpdated + "Endless" + trigger.ToString();
            string actionId = null;
            var getActionIdResponse = admin.Actions.GetId(name, out actionId);
            Assert.IsTrue(getActionIdResponse.StatusCode == System.Net.HttpStatusCode.OK);
            if (actionId != null)
            {
                var getDeleteActionResponse = admin.Actions.Delete(actionId);
                Assert.IsTrue(getDeleteActionResponse.StatusCode == System.Net.HttpStatusCode.OK);
            }
            var createActionResponse = admin.Actions.Post(name, code, objectId, trigger, WorkflowAction.JavaScript);
            var result = JsonConvert.DeserializeObject<Dictionary<string, object>>(createActionResponse.Content);
            actionId = ((Newtonsoft.Json.Linq.JObject)result["__metadata"])["id"].ToString();
            Assert.IsTrue(createActionResponse.StatusCode == System.Net.HttpStatusCode.OK);
            var response = crud.Post(objectNameUpdated, data);
            Assert.IsTrue(createActionResponse.Content == "The following action: \"items2EndlessAfterCreateBeforeCommit\" failed to perform: The following action: \"items2EndlessAfterCreateBeforeCommit\" failed to perform: The request 'http://localhost:4110//1/objects/items2' is repeating calling itself. Please change the action items2EndlessAfterCreateBeforeCommit");
            var getDeleteActionResponse2 = admin.Actions.Delete(actionId);
            Assert.IsTrue(getDeleteActionResponse2.StatusCode == System.Net.HttpStatusCode.OK);

        }



        [TestMethod]
        public void ReadOneDeep()
        {
            CrudUtility crud2 = new CrudUtility("app189");
        
            var response = crud2.GetOne("items", "1", true);
            Assert.IsTrue(response.StatusCode == System.Net.HttpStatusCode.OK);
            var result = JsonConvert.DeserializeObject<Dictionary<string, object>>(response.Content);

             
        }


        private void ReadAllItems()
        {
            var response = crud.GelAll(objectName);
            Assert.IsTrue(response.StatusCode == System.Net.HttpStatusCode.OK);
                
        }

        private void CreateItem()
        {
            var response = crud.Post(objectName, data);
            Assert.IsTrue(response.StatusCode == System.Net.HttpStatusCode.OK);
            var result = JsonConvert.DeserializeObject<Dictionary<string, object>>(response.Content);
            id = ((Newtonsoft.Json.Linq.JObject)result["__metadata"])["id"].ToString();
        }

        private void ReadOneItem()
        {
            var response = crud.GetOne(objectName, id);
            Assert.IsTrue(response.StatusCode == System.Net.HttpStatusCode.OK);
            var result = JsonConvert.DeserializeObject<Dictionary<string, object>>(response.Content);

            Assert.IsTrue(result.ContainsKey("name"));
            Assert.IsTrue(result["name"].Equals(data["name"]));
            Assert.IsTrue(result.ContainsKey("description"));
            Assert.IsTrue(result["description"].Equals(data["description"]));
            
            
        }

        private void UpdateItem()
        {
            data["name"] = "update" + id;
            var response = crud.Put(objectName, id, data, null, true);
            Assert.IsTrue(response.StatusCode == System.Net.HttpStatusCode.OK);
            var result = JsonConvert.DeserializeObject<Dictionary<string, object>>(response.Content);

            Assert.IsTrue(result.ContainsKey("name"));
            Assert.IsTrue(result["name"].Equals(data["name"]));
            Assert.IsTrue(result.ContainsKey("description"));
            Assert.IsTrue(result["description"].Equals(data["description"]));
            
        }

        private void DeleteItem()
        {
            var response = crud.Delete(objectName, id);
            Assert.IsTrue(response.StatusCode == System.Net.HttpStatusCode.OK);
            response = crud.GetOne(objectName, id);

            Assert.IsTrue(response.StatusCode == System.Net.HttpStatusCode.NotFound);
            
        }
    }
}
