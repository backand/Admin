using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Durados.Web.Mvc.Farm;
using Durados;
using System.Threading;

namespace BackAnd.UnitTests
{
    [TestClass]
    public class RedisPubSubTest
    {
        [TestMethod]
        public void TestCanConnect()
        {
            var connectionString = ValidConnectionString();
            var pubsubManager = new RedisFarmTransport(connectionString);
            //pubsubManager.PublishSync(new F)
        }

        private static string ValidConnectionString()
        {
            return "pub-redis-10938.us-east-1-4.3.ec2.garantiadata.com:10938,password=bell1234";
        }


        [ExpectedException(typeof(DuradosException))]
        [TestMethod]
        public void TestWrogConnectionThrowException()
        {
             var connectionString = "pub-redis-10938.us-east-1-4.3.ec2.garantiadata.com:10938,password=WRONG";
            var pubsubManager = new RedisFarmTransport(connectionString);
        }

        [TestMethod]
        public void TestSendMessageDontArriveToMyself()
        {
            var connectionString = ValidConnectionString();
            var pubsubManager = new RedisFarmTransport(connectionString);

            ManualResetEvent manual1 = new ManualResetEvent(false);
            
            pubsubManager.OnMessage += (a , b) => {
                    throw new NullReferenceException();
            };

            pubsubManager.PublishSync(new FarmMessage { AppName = "test", Time = DateTime.Now});

            Assert.AreEqual(pubsubManager.MessageCount, 1);
            Assert.AreEqual(pubsubManager.ValidMessageCount, 0);
        }

        [TestMethod]
        public void TestCanSendMessageAndGetItBackOnePublisherAndDifferentSubscriber()
        {
            var connectionString = ValidConnectionString();
            var pubsubManagerPublisher = new RedisFarmTransport(connectionString);
            var pubsubManagerSubscriber = new RedisFarmTransport(connectionString);

            ManualResetEvent manual1 = new ManualResetEvent(false);

            pubsubManagerSubscriber.OnMessage += (a, b) =>
            {
                Assert.AreEqual(b.Message.AppName, "test");
                manual1.Set();
            };

            pubsubManagerPublisher.PublishSync(new FarmMessage { AppName = "test", Time = DateTime.Now });
            var res = manual1.WaitOne(5 * 1000);
            
            Assert.IsTrue(res);
        }

        [TestMethod, Timeout(10000)]
        public void TestSendSyncNotBlockForever()
        {
            var connectionString = ValidConnectionString();
            var pubsubManagerPublisher = new RedisFarmTransport(connectionString);
            pubsubManagerPublisher.PublishSync(new FarmMessage { AppName = "test", Time = DateTime.Now });
        }
    }
}
