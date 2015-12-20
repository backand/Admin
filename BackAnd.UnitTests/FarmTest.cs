using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Durados.Web.Mvc.UI.Helpers;
using Durados.Web.Mvc;

namespace BackAnd.UnitTests
{
    [TestClass]
    public class FarmTest : BackandBaseTest
    {
        [TestInitialize]
        public void Init()
        {
            this.InitInner();
        }

        [TestMethod]
        public void TestAfterResetAppDontExistInCache()
        {
            // Arrange
            var appName = this.ValidAppName;
            var res = Maps.Instance.GetMap(appName);

            // Act
            FarmCachingSingeltone.Instance.ClearInternalCache(appName);

            // Assert
            Assert.IsFalse(Maps.Instance.AppInCach(appName));
        }

        [TestMethod]
        public void TestSendResentToNonInCacheAppDontThrowException()
        {
            var appName = this.ValidAppName;

            // Assert
            Assert.IsFalse(Maps.Instance.AppInCach(appName));

            // Act
            FarmCachingSingeltone.Instance.ClearInternalCache(appName);

            // Assert
            Assert.IsFalse(Maps.Instance.AppInCach(appName));
        }

        [TestMethod]
        public void TestSendResentToNonExistableDontThrowException()
        {
            var appName = Guid.NewGuid().ToString();

            // Assert
            Assert.IsFalse(Maps.Instance.AppInCach(appName));

            // Act
            FarmCachingSingeltone.Instance.ClearInternalCache(appName);

            // Assert
            Assert.IsFalse(Maps.Instance.AppInCach(appName));
        }

    }
}
