using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RestSharp;

namespace BackAnd.Web.Api.Test
{
    [TestClass]
    public class LoginTests
    {
       
        [TestMethod]
        public void TestValidLogin()
        {
            string username = "relly@backand.com";
            string password = "123456";
            string appName = "app185";

            var response = TestUtil.SignIn(username, password, appName);

            Assert.IsFalse(string.IsNullOrEmpty(response.access_token));
            Assert.AreEqual(response.token_type, "bearer");
            Assert.AreEqual(response.appName, appName);
            Assert.AreEqual(response.username, username);


        }

        [TestMethod]
        public void TestAnonymousLoginReturnValidRoleFromProfile()
        {
            // Arrane
            var client = new RestClient(TestUtil.ServiceAddressDev);
            var request = new RestRequest("/api/Account/profile", Method.GET);
            request.AddHeader("AnonymousToken", "1212ad72-1625-4e69-aef5-438f2b6f3c2b");
            request.AddHeader("Content-Type", "application/x-www-form-urlencoded");
            request.AddHeader("Accept", "application/json");

            // Act
            var response = client.Execute(request);

            // Assert
            Assert.IsTrue(response.Content.ToLower().Contains("role"), response.Content);
            Assert.IsTrue(response.Content.ToLower().Contains("user"), response.Content);
        }


        [TestMethod]
        public void TestAnonymousLoginTestValidRoleFromProfile()
        {
            // Arrane
            var client = new RestClient(TestUtil.ServiceAddressDev);
            var request = new RestRequest("/api/Account/profile", Method.GET);
            request.AddHeader("AppName", "app185");
            request.AddHeader("AnonymousToken", "1212ad72-1625-4e69-aef5-438f2b6f3c2b");
            request.AddHeader("Content-Type", "application/x-www-form-urlencoded");
            request.AddHeader("Accept", "application/json");

            // Act
            var response = client.Execute(request);

            // Assert
            Assert.IsTrue(response.Content.ToLower().Contains("role"), response.Content);
        }

    }
}
