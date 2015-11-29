using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BackObject = System.Collections.Generic.Dictionary<string, object>;

namespace BackAnd.Web.Api.Test
{
    public class CrudContext
    {
        public CrudUtility crud = null;

        public string ObjectName { get; set; }

        public string Id { get; set; }

        public BackObject DataForCreate { get; set; }

        public bool DeepOnUpdate = false;

        public List<FilterItem> filter { get; set; }

        public CrudContext()
            : this(Backand.Config.ConfigStore.GetConfig().appname, Backand.Config.ConfigStore.GetConfig().username, Backand.Config.ConfigStore.GetConfig().pwd)
        {

        }
        public CrudContext(string appName, string username, string password)
        {
            crud = new CrudUtility(appName, username, password);
            filter = new List<FilterItem>();
        }
    }



    public static class DeepHelper
    {
        public static CrudContext AddDeepOnUpdate(this CrudContext runner)
        {
            runner.DeepOnUpdate = true;
            return runner;
        }
    }

    public static class FilterHelper
    {
        public static CrudContext AddFilter(this CrudContext runner, FilterItem item)
        {
            runner.filter.Add(item);
            return runner;
        }


    }

    public static class CrudRunnerExtension
    {
        public static CrudContext CreateItem(this CrudContext runner, BackObject dataForCreate)
        {
            var response = runner.crud.Post(runner.ObjectName, dataForCreate, runner.DeepOnUpdate);
            Assert.IsTrue(response.StatusCode == System.Net.HttpStatusCode.OK);
            var result = JsonConvert.DeserializeObject<BackObject>(response.Content);
            var id = ((Newtonsoft.Json.Linq.JObject)result["__metadata"])["id"].ToString();

            runner.DataForCreate = dataForCreate;
            runner.Id = id;
            return runner;
        }

        public static CrudContext ReadOneItem(this CrudContext runner)
        {
            runner.ReadOneItem(runner.DataForCreate, runner.Id);
            return runner;
        }

        public static CrudContext ReadOneItem(this CrudContext runner, BackObject dataToCompare, string id)
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

        public static CrudContext UpdateItem(this CrudContext runner, BackObject dataToUpdate)
        {
            var response = runner.crud.Put(runner.ObjectName, runner.Id, dataToUpdate, null, true);
            Assert.IsTrue(response.StatusCode == System.Net.HttpStatusCode.OK);
            var result = JsonConvert.DeserializeObject<BackObject>(response.Content);

            AssertDeepEqual(dataToUpdate, result);
            return runner;
        }


        public static CrudContext DeleteItem(this CrudContext runner)
        {
            var response = runner.crud.Delete(runner.ObjectName, runner.Id);
            Assert.IsTrue(response.StatusCode == System.Net.HttpStatusCode.OK);
            response = runner.crud.GetOne(runner.ObjectName, runner.Id);

            Assert.IsTrue(response.StatusCode == System.Net.HttpStatusCode.NotFound);
            return runner;
        }
    }
}
