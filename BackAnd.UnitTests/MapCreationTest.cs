using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Durados.Web.Mvc;
using System.Web;
using Backand.Config;

namespace BackAnd.UnitTests
{
    [Microsoft.VisualStudio.TestTools.UnitTesting.TestClass]
    public class MapCreationTest : BackandBaseTest
    {
        [TestInitialize]
        public void Init()
        {
            this.InitInner();

        }

        [TestCleanup]
        public void Cleanup()
        {
            this.CleanupInner();
        }

        [TestMethod]
        public void TestCreateMapWithAdminAppReturnDuradosApp()
        {
            var res = Maps.Instance.GetMap("www");
            Assert.IsInstanceOfType(res, typeof(DuradosMap));
            Assert.IsFalse(Maps.Instance.AppInCach("www"));
        }

        [TestMethod]
        public void TestCreateMapWithNonExistingApplicationReturnNull()
        {
            var appName = Guid.NewGuid().ToString();
            var res = Maps.Instance.GetMap(appName);
            Assert.IsNull(res);
        }

        [TestMethod]
        public void TestCreateMapWithNonExistingApplicationDontAddToCache()
        {
            var appName = Guid.NewGuid().ToString();
            var res = Maps.Instance.GetMap(appName);
            Assert.IsFalse(Maps.Instance.AppInCach(appName));
        }

        [TestMethod]
        public void TestCreateMapOnValidAppReturnValidMap()
        {
            var appName = this.ValidAppName;
            var res = Maps.Instance.GetMap(appName);
            Assert.IsNotNull(res);
            Assert.AreEqual(appName, res.AppName);
        }

        [TestMethod]
        public void TestCreateMapOnValidAppInsertToCache()
        {

            // Houston we have problem!
            // Because we test can run in any order, previous test can run first, so app will already be in cache.

            var appName = this.ValidAppName;
            var res = Maps.Instance.GetMap(appName);
            Assert.IsTrue(Maps.Instance.AppInCach(appName));
        }
    }
    
}
