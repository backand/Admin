using Durados.Web.Mvc.SocialLogin;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;

namespace BackAnd.UnitTests
{
    [TestClass]
    public class SocialLoginProfileGlobalTest
    {
        [TestMethod]
        public void TestGetProfileReturnProfile()
        {
            var provider = SocialProviderFactory.GetSocialProvider("google");
            provider.MockReturnFromService = new Dictionary<string, object>
            { { "testKey" , "testValue" },
                {"id" , "idValue" } };

            var result = provider.GetProfile("appName", "accessToken", "email@gmail.com");

            Assert.AreEqual(result.appName, "appName");
            Assert.AreEqual(result.email, "email@gmail.com");
            Assert.AreEqual(result.id, "idValue");
            //Assert.AreEqual(result.returnAddress, "dummy");
        }

    }
}
