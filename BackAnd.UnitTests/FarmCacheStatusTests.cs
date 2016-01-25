using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Durados.Web.Mvc;
using BackAnd.Web.Api.Controllers.Admin;
using System.Web.Http.Results;
using System.Collections.Generic;
using System.Threading;
using System.Data;

namespace BackAnd.UnitTests
{
    [TestClass]
    public class FarmCacheStatusTests :  BackandBaseTest
    {
        [TestInitialize]
        public void Init()
        {
            this.InitInner();
        }
    
        [TestMethod]
        public void TestAddCacheReturnInController()
        {
            Maps.Instance.StorageCache.Add("test", TestHelper.GetSimpleDataSet());

            Thread.Sleep(300);

            var res = new FarmController().Get();

            var contentResult = res as OkNegotiatedContentResult<Dictionary<string,string>>;
            Assert.IsTrue(!string.IsNullOrEmpty(contentResult.Content["test"]));
        }

        [TestMethod]
        public void TestChangeDataSetChangeStatus()
        {
            DataSet ds1 = TestHelper.GetSimpleDataSet();
            Maps.Instance.StorageCache.Add("test1", ds1);

            DataSet ds2 = TestHelper.GetSimpleDataSet();
            ds2.Tables[0].Rows[0]["a"] = 2;
            Maps.Instance.StorageCache.Add("test2", ds2);

            Thread.Sleep(300);

            var res = new FarmController().Get();

            var contentResult = res as OkNegotiatedContentResult<Dictionary<string,string>>;
            Assert.AreNotEqual(contentResult.Content["test1"], contentResult.Content["test2"]);
            
        }
    }
}
