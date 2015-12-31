using Durados.DataAccess;
using Durados.Web.Mvc.Azure;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace BackAnd.UnitTests
{
    [TestClass]
    public class AppLockerGetterTests
    {
        [TestMethod]
        public void TestSameAppGetSameLocker()
        {
            // arrange
            var runner = new LockRunner();

            // act

            var task = Task.Factory.StartNew(() =>
            {
                runner.Run("test1", "1");
            });

            var task2 = Task.Factory.StartNew(() =>
            {
                runner.Run("test1", "2");
            });

            var task3 = Task.Factory.StartNew(() => {
                runner.eventerFor3.WaitOne(10000);
                runner.Run("test2", "3");
                
                // free 'test1'
                runner.eventer.Set();
            });

            Task[] arr = new Task[3];
            arr[0] = task;
            arr[1] = task2;
            arr[2] = task3;

            Task.WaitAll(arr);
           
            // assert  
            Assert.AreEqual( string.Join(",", runner.Order), "1,3,2");
            
        }

        [TestMethod]
        public void TestKeyWithSameNameGateSameLocker()
        {
            AppLockerGetter lockGetter = new AppLockerGetter(new LocalCache<object>("test"));
            var a1 = lockGetter.GetLock("aaa");
            var a2 = lockGetter.GetLock("aaa");
            var a3 = lockGetter.GetLock("other");

            Assert.AreEqual(a1, a2);
            Assert.AreNotEqual(a1, a3);
        }
    }



    public class LockRunner
    {
        AppLockerGetter lockGetter = new AppLockerGetter(new LocalCache<object>("aaa"));

        public List<string> Order = new List<string>();

        public ManualResetEvent eventer = new ManualResetEvent(false);

        public ManualResetEvent eventerFor3 = new ManualResetEvent(false);
        public void Run(string appName, string message)
        {
            Debug.WriteLine("Start Lock " + appName + " " + message);
            lock (lockGetter.GetLock(appName))
            {
                Debug.WriteLine("Got Lock " + appName + " " + message);

                Order.Add(message);

                if (appName == "test1")
                {
                    eventerFor3.Set();
                    eventer.WaitOne(100000);
                }
            }

            Debug.WriteLine("Finish" + appName + " " + message);
        }
    }
}
