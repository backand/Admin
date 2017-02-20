using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Durados.Web.Mvc.UI.Helpers;
using Durados.Web.Mvc;
using BackAnd.Web.Api.Controllers;
using System.Reflection;
using Durados.Web.Mvc.Farm;
using Durados;
namespace BackAnd.UnitTests
{
    [TestClass]
    public class FarmControllerTest : apiController
    {
        [TestInitialize]
        public void Init()
        {
            TestHelper.Init();
        }

        [TestMethod]
        public void TestChangeInCacheCallRedisPublishMethod()
        {
            var appName = new FarmTest().ValidAppName;
            var res = Maps.Instance.GetMap(appName);

            if (!System.Web.HttpContext.Current.Items.Contains(Durados.Database.AppName))
                System.Web.HttpContext.Current.Items.Add(Durados.Database.AppName, appName);
            this.RefreshConfigCache(res);

            var transport = typeof(FarmCachingNormal).GetField("transport", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(FarmCachingSingeltone.Instance);

            var pCount = (transport as RedisFarmTransport).PublishCount;
            
            Assert.AreEqual(1, pCount);
        }

    }
}
