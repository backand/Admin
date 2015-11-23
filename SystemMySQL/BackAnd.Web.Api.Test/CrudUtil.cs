using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RestSharp;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace BackAnd.Web.Api.Test
{
    public class CrudUtility
    {
        RestClient client = null;
        public CrudUtility()
        {
            client = TestUtil.GetAuthentificatedClient();
        }

        public CrudUtility(string appName)
        {
            client = TestUtil.GetAuthentificatedClient(appName);
        }
        public CrudUtility(string appName, string username, string password)
        {
            client = TestUtil.GetAuthentificatedClient(appName, username, password);
        }
        public IRestResponse GelAll(string name, bool? withSelectOptions = null, bool? withFilterOptions = null, int? pageNumber = null, int? pageSize = null, object filter = null, object sort = null, string search = null, bool? deep = null, bool? descriptive = true, bool? relatedObjects = false)
        {
            // Arrange
            var request = new RestRequest("/1/objects/{name}", Method.GET);
            request.AddUrlSegment("name", name);
            if (withFilterOptions.HasValue)
                request.AddParameter("withSelectOptions", withSelectOptions.Value, ParameterType.QueryString);
            if (withFilterOptions.HasValue)
                request.AddParameter("withFilterOptions", withFilterOptions.Value, ParameterType.QueryString);
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
            if (deep.HasValue)
                request.AddParameter("deep", deep.Value, ParameterType.QueryString);
            if (descriptive.HasValue)
                request.AddParameter("descriptive", descriptive.Value, ParameterType.QueryString);
            if (relatedObjects.HasValue)
                request.AddParameter("relatedObjects", relatedObjects.Value, ParameterType.QueryString);

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

        public IRestResponse GetOne(string name, string id, bool? deep = null, int? level = null)
        {
            // Arrange
            var request = new RestRequest("/1/objects/{name}/{id}", Method.GET);
            request.AddUrlSegment("name", name);
            request.AddUrlSegment("id", id);
            if (deep.HasValue)
                request.AddParameter("deep", deep.Value, ParameterType.QueryString);
            if (level.HasValue)
                request.AddParameter("level", level.Value, ParameterType.QueryString);

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


        public IRestResponse Post(string name, object data, bool? deep = null, bool? returnObject = null, string parameters = null)
        {
            // Arrange
            var request = new RestRequest("/1/objects/{name}", Method.POST);
            request.AddUrlSegment("name", name);
            request.RequestFormat = DataFormat.Json;
            request.AddBody(data);
            if (deep.HasValue)
                request.AddParameter("deep", deep.Value, ParameterType.QueryString);
            if (returnObject.HasValue)
                request.AddParameter("returnObject", returnObject.Value, ParameterType.QueryString);
            if (parameters != null)
                request.AddParameter("parameters", parameters, ParameterType.QueryString);

            // Act
            var response = client.Execute(request);
            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                var result = JsonConvert.DeserializeObject<Dictionary<string, object>>(response.Content);

                Assert.IsTrue(result.ContainsKey("__metadata"));
                Assert.IsTrue(result["__metadata"] is Newtonsoft.Json.Linq.JObject);
                // Assert
                if (returnObject.HasValue && returnObject.Value)
                {
                    Assert.IsTrue(result.Count > 1);
                }
                else
                {
                    Assert.IsTrue(result.Count == 1);
                }
            }
            return response;
        }

        public IRestResponse Put(string name, string id, object data, bool? deep = null, bool? returnObject = null, string parameters = null, bool? overwrite = null)
        {
            // Arrange
            var request = new RestRequest("/1/objects/{name}/{id}", Method.PUT);
            request.AddUrlSegment("name", name);
            request.AddUrlSegment("id", id);
            request.RequestFormat = DataFormat.Json;
            request.AddBody(data);
            if (deep.HasValue)
                request.AddParameter("deep", deep.Value, ParameterType.QueryString);
            if (returnObject.HasValue)
                request.AddParameter("returnObject", returnObject.Value, ParameterType.QueryString);
            if (parameters != null)
                request.AddParameter("parameters", parameters, ParameterType.QueryString);
            if (overwrite.HasValue)
                request.AddParameter("overwrite", overwrite.Value, ParameterType.QueryString);
            
            // Act
            var response = client.Execute(request);
            
            // Assert
            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                if (returnObject.HasValue && returnObject.Value)
                {
                    var result = JsonConvert.DeserializeObject<Dictionary<string, object>>(response.Content);
                    Assert.IsTrue(result.Count > 1);
                    Assert.IsTrue(result.ContainsKey("__metadata"));
                    Assert.IsTrue(result["__metadata"] is Newtonsoft.Json.Linq.JObject);
                    Assert.IsTrue(((Newtonsoft.Json.Linq.JObject)result["__metadata"])["id"].ToString().Equals(id));

                }
                else
                {
                    Assert.IsTrue(string.IsNullOrEmpty(response.Content));
                }
            }

            return response;

        }


        public IRestResponse Delete(string name, string id, bool? deep = null, string parameters = null)
        {
            // Arrange
            var request = new RestRequest("/1/objects/{name}/{id}", Method.DELETE);
            request.AddUrlSegment("name", name);
            request.AddUrlSegment("id", id);
            if (deep.HasValue)
                request.AddParameter("deep", deep.Value, ParameterType.QueryString);
            if (parameters != null)
                request.AddParameter("parameters", parameters, ParameterType.QueryString);
            
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
    }

    public class FilterItem
    {
        public string fieldName { get; set; }
        public string @operator { get; set; }
        public string value { get; set; }
    }

    public class SortItem
    {
        public string fieldName { get; set; }
        public Order order { get; set; }
    }

    public enum Order
    {
        asc,
        desc
    }

    public enum FilterOperandType
    {
        equals,
        notEquals,
        lessThan,
        lessThanOrEqualsTo,
        greaterThan,
        greaterThanOrEqualsTo,
        empty,
        notEmpty,

        startsWith,
        endsWith,
        contains,
        notContains,

        @in,
    }
}
