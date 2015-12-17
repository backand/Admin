using System;
using System.Data;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Durados.Web.Mvc;
using System.Web;
using Backand.Config;
using Durados.Web.Mvc.Azure;
using System.Diagnostics;

namespace BackAnd.UnitTests
{
    [Microsoft.VisualStudio.TestTools.UnitTesting.TestClass]
    public class MemoryCacheTest
    {
        [TestMethod]
        public void TestCanAddDataSetObjects()
        {
            var cache = CacheFactory.CreateCache<DataSet>("testCache");
            DataSet ds = new DataSet();

            DataTable table = new DataTable();
            table.Columns.Add("a", typeof(int));
            table.Columns.Add("b", typeof(string));
            table.Columns.Add("c", typeof(bool));
            table.Rows.Add(1, "s", true);
            ds.Tables.Add(table);

            // Act
            cache.Add("first", ds);

            var fetchedDs = cache.Get("first");
            var row = ds.Tables[0].Rows[0];

            Assert.AreEqual(row[0], 1);
            Assert.AreEqual(row[1], "s");
            Assert.AreEqual(row[2], true);
        }

        [TestMethod]
        public void TestTwoInstancesDontHaveCommonObjects()
        {
            var cache1 = CacheFactory.CreateCache<string>("testCache1");
            var cache2 = CacheFactory.CreateCache<string>("testCache2");

            cache1.Add("first", "stam1");
            cache2.Add("first", "stam2");

            Assert.AreNotEqual(cache1.Get("first"), cache2.Get("first"));

        }

        [TestMethod]
        public void TestAddAndReadInDifferentThreadsWorkQuickly()
        {
            var cache1 = CacheFactory.CreateCache<string>("testCache1");

            var t1 = Task.Factory.StartNew(() =>
            {
                int i = 0;
                while (i < 1000)
                {
                    var key = "key" + (i % 10);
                   // Debug.WriteLine("1  | " + key);
                    cache1.Add(key, "A");
                    i++;
                }
            });

            var t2 = Task.Factory.StartNew(() =>
            {
                int i = 0;
                while (i < 1000)
                {
                    var key = "key" + (i % 10);
                    //Debug.WriteLine("2  | " + key);
                    cache1.Add(key, "A");
                    i++;
                }
            });

            Task.WaitAll(t1, t2);
        }

        [TestMethod]
        public void TestAddTwiceSameKeyGetTheLastOne()
        {
            // Arrange
            var cache = CacheFactory.CreateCache<string>("testCache");
            cache.Add("first", "stam");

            // Act
            cache.Add("first", "stam2");

            // Assert
            Assert.AreEqual(cache.Get("first"),"stam2");

        }

        [TestMethod]
        public void TestGetNonExistingKeyReturnNull()
        {
            var cache = CacheFactory.CreateCache<string>("testCache");
            Assert.IsNull(cache.Get("unexsiting"));

        }

    }
    
}
