using Microsoft.VisualStudio.TestTools.UnitTesting;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BackAnd.Web.Api.Test
{
    public class TestUtil
    {
        public const string ServiceAddressDev = "http://localhost:4110/";
        public const string ServiceAddressQa = "http://api.backand.co:8099/";
        public const string ServiceAddressProd = "https://api.backand.com/";

        Envirement env = Envirement.Dev;

        public TestUtil()
        {

        }
        public TestUtil(Envirement env)
        {
            this.env = env;
        }
        public RestClient GetRestClient()
        {
            return GetRestClient(env);
        }

        private RestClient GetRestClient(Envirement env)
        {
            RestClient client = null;
            if (env == Test.Envirement.Dev)
                client = new RestClient(ServiceAddressDev);
            else if (env == Test.Envirement.QA)
                client = new RestClient(ServiceAddressQa);
            else if (env == Test.Envirement.Prod)
                client = new RestClient(ServiceAddressProd);

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


        public RestClient GetAuthentificatedClient(string appName = "app185", string username = "relly@backand.com", string password = "123456")
        {
            var rest = GetRestClient();
            
            var res = SignIn(username, password, appName);
            Assert.IsTrue(res.token_type != null && res.access_token != null, "Fail to sign in");
            rest.AddDefaultHeader("Authorization", res.token_type + " " + res.access_token);
            return rest;
        }
    }

    public enum Envirement
    {
        Dev,
        QA,
        Prod
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
