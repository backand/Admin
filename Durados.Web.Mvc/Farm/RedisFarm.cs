using Durados.Data;
using Durados.Web.Mvc.Azure;
using Durados.Web.Mvc.UI.Helpers;
using Newtonsoft.Json;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Caching;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Durados.Web.Mvc.Farm
{
    public class FarmEventArgs : EventArgs
    {
        public FarmMessage Message;
    }


    public class RedisFarmTransport : IFarmPubsubTransport, IDisposable
    {
        public event EventHandler<FarmEventArgs> OnMessage;

        private string roomName;

        ConnectionMultiplexer redis;

        ISubscriber subscriber;
        
        IDatabase database;

        public int MessageCount = 0;

        public int ValidMessageCount = 0;

        public int PublishCount = 0;

        public const int WAIT_TIME = 3000;
        private ICache<ManualResetEvent> lockers;

        public IStatus Status { get; private set; }

        public string SubscriberID { get; private set; }

        public RedisFarmTransport(string connectionString, string roomName = "BackandElb")
        {
            this.roomName = roomName;
            this.SubscriberID = Guid.NewGuid().ToString();

            this.lockers = CacheFactory.CreateCache<ManualResetEvent>("RedisFarmTransport" + roomName);
            //todo: log

            redis = CreateRedisConnection(connectionString);

            subscriber = redis.GetSubscriber();

            database = redis.GetDatabase();

            Status = new Status(database, SubscriberID);

            subscriber.Subscribe(roomName, (channel, message) =>
            {
                MessageCount++;

                // todo: log info
                var messageAsObject = JsonConvert.DeserializeObject<FarmMessageWrapper>((string)message);

                if (messageAsObject.SenderId != SubscriberID)
                {
                    ValidMessageCount++;
                    OnMessage(this, new FarmEventArgs { Message = messageAsObject.Message });
                }
                else // message sent by myself, check another thread wait for ack
                {
                    var key = messageAsObject.Message.Id.ToString();

                    if (lockers.ContainsKey(key))
                    {
                        lockers[key].Set();
                    }
                }
            });
        }

        public void Publish(FarmMessage message)
        {
            PublishCount++;
            var wrappedMessage = new FarmMessageWrapper { Message = message, SenderId = this.SubscriberID };
            var str = JsonConvert.SerializeObject(wrappedMessage);
            try
            {
                subscriber.Publish(this.roomName, str);
            }
            catch (Exception exception)
            {
                Maps.Instance.DuradosMap.Logger.Log("RedisFarmTransport", "Publish", "", exception, 1, "");
            }
        }

        public void PublishSync(FarmMessage message)
        {
            CreateLockerForApp(message);
            Publish(message);
            var res = WaitMessageArriveBackFromRedis(message);

            // todo: if res false
            if (!res)
            {
                throw new PublishSyncTimeoutException();
            }

        }

        private void CreateLockerForApp(FarmMessage message)
        {
            var currentAppLocker = new ManualResetEvent(false);
            lockers[message.Id.ToString()] = currentAppLocker;
        }

        private bool WaitMessageArriveBackFromRedis(FarmMessage message)
        {
            var locker = lockers[message.Id.ToString()];
            var res = locker.WaitOne(WAIT_TIME);
            lockers.Remove(message.Id.ToString());
            return res;
        }

        internal static ConnectionMultiplexer CreateRedisConnection(string connectionString)
        {
            ConfigurationOptions options = ConfigurationOptions.Parse(connectionString);
            options.AbortOnConnectFail = false;

            ConnectionMultiplexer redis = null;
            try
            {
                redis = ConnectionMultiplexer.Connect(options);
            }
            catch (RedisConnectionException exception)
            {
                // todo: when can't connect
                throw new DuradosException("Can't connect to REDIS connectionString: " + connectionString, exception);
            }

            return redis;
        }

        private void HandleMessage(FarmMessage message)
        {
            throw new NotImplementedException();
        }

        public void Dispose()
        {
            subscriber.UnsubscribeAll();
        }
    }

    
}
