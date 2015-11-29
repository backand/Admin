using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BackObject = System.Collections.Generic.Dictionary<string, object>;

namespace BackAnd.Web.Api.Test
{
    public class TestContext
    {
        public CrudUtility crud = new CrudUtility();

        public string ObjectName { get; set; }

        public string Id { get; set; }

        public BackObject DataForCreate { get; set; }

        public bool DeepOnUpdate = false;

        public List<FilterItem> filter { get; set; }


        public TestContext()
        {
            filter = new List<FilterItem>();
        }
    }



    public static class DeepHelper
    {
        public static TestContext AddDeepOnUpdate(this TestContext runner)
        {
            runner.DeepOnUpdate = true;
            return runner;
        }
    }

    public static class FilterHelper
    {
        public static TestContext AddFilter(this TestContext runner, FilterItem item)
        {
            runner.filter.Add(item);
            return runner;
        }


    }

    public static class CrudRunnerExtension
    {
        public static TestContext CreateItem(this TestContext runner, BackObject dataForCreate)
        {
            var response = runner.crud.Post(runner.ObjectName, dataForCreate, runner.DeepOnUpdate);
            Assert.IsTrue(response.StatusCode == System.Net.HttpStatusCode.OK);
            var result = JsonConvert.DeserializeObject<BackObject>(response.Content);
            var id = ((Newtonsoft.Json.Linq.JObject)result["__metadata"])["id"].ToString();

            runner.DataForCreate = dataForCreate;
            runner.Id = id;
            return runner;
        }

        public static TestContext ReadOneItem(this TestContext runner)
        {
            runner.ReadOneItem(runner.DataForCreate, runner.Id);
            return runner;
        }

        public static TestContext ReadOneItem(this TestContext runner, BackObject dataToCompare, string id)
        {
            var response = runner.crud.GetOne(runner.ObjectName, id);
            Assert.IsTrue(response.StatusCode == System.Net.HttpStatusCode.OK);
            var result = JsonConvert.DeserializeObject<BackObject>(response.Content);

            AssertDeepEqual(dataToCompare, result);

            return runner;
        }

        private static void AssertDeepEqual(BackObject dataToCompare, BackObject result)
        {
            foreach (string key in dataToCompare.Keys)
            {
                Assert.IsTrue(result.ContainsKey(key));
                Assert.IsTrue(result[key].Equals(dataToCompare[key]));
            }
        }

        public static TestContext UpdateItem(this TestContext runner, BackObject dataToUpdate)
        {
            var response = runner.crud.Put(runner.ObjectName, runner.Id, dataToUpdate, null, true);
            Assert.IsTrue(response.StatusCode == System.Net.HttpStatusCode.OK);
            var result = JsonConvert.DeserializeObject<BackObject>(response.Content);

            AssertDeepEqual(dataToUpdate, result);
            return runner;
        }


        public static TestContext DeleteItem(this TestContext runner)
        {
            var response = runner.crud.Delete(runner.ObjectName, runner.Id);
            Assert.IsTrue(response.StatusCode == System.Net.HttpStatusCode.OK);
            response = runner.crud.GetOne(runner.ObjectName, runner.Id);

            Assert.IsTrue(response.StatusCode == System.Net.HttpStatusCode.NotFound);
            return runner;
        }
    }
}
