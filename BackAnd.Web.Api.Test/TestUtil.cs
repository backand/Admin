using Microsoft.VisualStudio.TestTools.UnitTesting;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BackAnd.Web.Api.Test
{
    public class TestUtil
    {
        
        public TestUtil()
        {

        }

        private string GetServerAddress()
        {
            return Backand.Config.ConfigStore.GetConfig().serverAddress;
        }
        private RestClient GetRestClient(string url = null)
        {
            RestClient client = new RestClient(url ?? GetServerAddress());

            return client;
        }
        
        public LoginResult SignIn(string username, string password, string appName)
        {
            var request = new RestRequest("token", Method.POST);
            request.AddParameter("username", username);
            request.AddParameter("password", password);

            request.AddParameter("appname", appName);
            request.AddParameter("grant_type", "password");
            request.AddHeader("Content-Type", "application/x-www-form-urlencoded");
            request.AddHeader("Accept", "application/json");

            var response = GetRestClient().Execute<LoginResult>(request).Data;
            return response;
        }

        public RestClient GetAuthentificatedClient()
        {
            return GetAuthentificatedClient(Backand.Config.ConfigStore.GetConfig().appname, Backand.Config.ConfigStore.GetConfig().username, Backand.Config.ConfigStore.GetConfig().pwd);
        }

        public RestClient GetAuthentificatedClient(string appName)
        {
            return GetAuthentificatedClient(appName, Backand.Config.ConfigStore.GetConfig().username, Backand.Config.ConfigStore.GetConfig().pwd);
        }

        public RestClient GetAuthentificatedClient(string appName, string username, string password, string url = null)
        {
           // Trace.WriteLine("trace");
            var rest = GetRestClient(url);
            
            var res = SignIn(username, password, appName);
            Assert.IsTrue(res != null && res.token_type != null && res.access_token != null, "Fail to sign in");
            rest.AddDefaultHeader("Authorization", res.token_type + " " + res.access_token);
            Console.WriteLine(res.access_token);
            return rest;
        }
    }

    public class LoginResult
    {
        public string access_token { get; set; }
        public string token_type { get; set; }
        public int expires_in { get; set; }
        public string appName { get; set; }
        public string username { get; set; }
        public string role { get; set; }
        public string fullName { get; set; }
        public string userId { get; set; }
    }

}
