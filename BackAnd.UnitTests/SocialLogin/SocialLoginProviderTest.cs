using Durados.Web.Mvc.SocialLogin;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BackAnd.UnitTests
{
    [TestClass]
    public class SocialLoginTest
    {
        [TestMethod]
        public void TestCanGetGoogleProvider()
        {
            var provider = SocialProviderFactory.GetSocialProvider("google");
            Assert.IsNotNull(provider);
        }

        [TestMethod]
        public void TestCanGetFacebookProvider()
        {
            var provider = SocialProviderFactory.GetSocialProvider("facebook");
            Assert.IsNotNull(provider);
        }

        [TestMethod]
        public void TestCanGetGithubProvider()
        {
            var provider = SocialProviderFactory.GetSocialProvider("github");
            Assert.IsNotNull(provider);
        }

        [TestMethod]
        public void TestCanGetTwitterProvider()
        {
            var provider = SocialProviderFactory.GetSocialProvider("twitter");
            Assert.IsNotNull(provider);
        }

        [TestMethod]
        public void TestNonExistingProviderReturnNull()
        {
            var provider = SocialProviderFactory.GetSocialProvider("NonExisting");
            Assert.IsNull(provider);
        }
    }
}
