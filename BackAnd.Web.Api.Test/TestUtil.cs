using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BackAnd.Web.Api.Test
{
    public static class TestUtil
    {
        public const string ServiceAddressDev = "http://localhost:4110/";
        public const string ServiceAddressQa = "http://localhost:4110/";
        public const string ServiceAddressProd = "http://localhost:4110/";

        public static Envirement envirement = Envirement.Dev;
        private static RestClient client = null;
       public static RestClient Client
        {
            get
            {
                if (client == null)
                    client = GetRestClient();
                return client;
            }
        }

        
        

        private static RestClient GetRestClient()
        {
            return GetRestClient(envirement);
        }

        private static RestClient GetRestClient(Envirement env)
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
        
        public static LoginResult SignIn(string username, string password, string appName)
        {
            var request = new RestRequest("token", Method.POST);
            request.AddParameter("username", username);
            request.AddParameter("password", password);

            request.AddParameter("appname", appName);
            request.AddParameter("grant_type", "password");
            request.AddHeader("Content-Type", "application/x-www-form-urlencoded");
            request.AddHeader("Accept", "application/json");

            var response = Client.Execute<LoginResult>(request).Data;
            return response;
        }


        public static RestClient GetAuthentificatedClient(string appName = "app185", string username = "relly@backand.com", string password = "123456")
        {
            var rest = GetRestClient();
            
            var res = SignIn(username, password, appName);

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
