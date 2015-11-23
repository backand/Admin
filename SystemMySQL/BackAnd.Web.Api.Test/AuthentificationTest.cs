
//using System;
//using Microsoft.VisualStudio.TestTools.UnitTesting;
//using RestSharp;
//using RestSharp.Deserializers;
//using Newtonsoft.Json;

//namespace BackAnd.Web.Api.Test
//{
//    [TestClass]
//    public class AuthentificationTest
//    {
//        public const string ServiceAddress = "http://localhost:4004/";

//        public RestClient client = new RestClient(ServiceAddress);

//        public string LoginTemplate = "/token?Accept=application/json&Content-Type=application/x-www-form-urlencoded";

//        public string dataAddress;

//        [TestMethod]
//        public void TestValidAuthRetunToken()
//        {
//            // client.Authenticator = new HttpBasicAuthenticator(username, password);

//            var request = new RestRequest(LoginTemplate, Method.POST);
//            CreateLoginRequest(request, new UserLogin());

//            // execute the request
//            var response = client.Execute(request);
//            var logResult = JsonConvert.DeserializeObject<LoginResult>(response.Content);
//            Assert.IsTrue(logResult.access_token.Length > 10);

//        }

//        [TestMethod]
//        public void TestNotValidAuthGetError()
//        {
//            var request = new RestRequest(LoginTemplate, Method.POST);
//            CreateLoginRequest(request, new UserLogin() { Password = "1234"});

//            // execute the request
//            var response = client.Execute(request);

//            Assert.IsTrue(response.Content.Contains("error"));

//        }

//        [TestMethod]
//        public void TestCanGetAnotherTokenWithRefreshToken()
//        {
//            var request = new RestRequest(LoginTemplate, Method.POST);
//            CreateLoginRequest(request, new UserLogin());

//            // execute the request
//            var response = client.Execute(request);
//            var logResult = JsonConvert.DeserializeObject<LoginResult>(response.Content);


//            var request2 = new RestRequest(LoginTemplate, Method.POST);
//            CreateLoginRequest(request2, new UserLogin() { Username = null, RefreshToken = logResult.refresh_token, GrantType= "refresh_token" });

//            // execute the request
//            var response2 = client.Execute(request2);
//            var logResult2 = JsonConvert.DeserializeObject<LoginResult>(response.Content);

//            Assert.AreNotEqual(logResult.access_token, logResult2.access_token);
//        }



//        [TestMethod]
//        public void TestAfterUseRefresTokenCantUseOldToken()
//        {
//            var request = new RestRequest(LoginTemplate, Method.POST);
//            CreateLoginRequest(request, new UserLogin());

//            // execute the request
//            var response = client.Execute(request);
//            var logResult = JsonConvert.DeserializeObject<LoginResult>(response.Content);


//            var request2 = new RestRequest(LoginTemplate, Method.POST);
//            CreateLoginRequest(request2, new UserLogin() { Username = null, RefreshToken = logResult.refresh_token, GrantType = "refresh_token" });

//            // execute the request
//            var response2 = client.Execute(request2);
//            var logResult2 = JsonConvert.DeserializeObject<LoginResult>(response.Content);

//            Assert.AreNotEqual(logResult.access_token, logResult2.access_token);
//        }
//        public void CreateLoginRequest(RestRequest request, UserLogin user)
//        {
//            request.AddParameter("grant_type", user.GrantType); // adds to POST or URL querystring based on Method
//             request.AddParameter("username", user.Username);

//            if (user.Username != null)
//            {
//                request.AddParameter("password", user.Password);
                
//            }

//            request.AddParameter("appname", user.ApplicationName);

//            if(user.RefreshToken != null)
//            {
//                request.AddParameter("refresh_token", user.RefreshToken);               
//            }
//        }


//        public class UserLogin
//        {
//            public string GrantType = "password";
//            public string Username = "dev@devitout.com";
//            public string Password = "dura2008";
//            public string ApplicationName = "consolename";
//            public string RefreshToken = null;
//        }

//        public class LoginResult
//        {
//            public string access_token;
//            public string token_type;
//            public string expires_in;
//            public string refresh_token;
//        }
//    }
//}
