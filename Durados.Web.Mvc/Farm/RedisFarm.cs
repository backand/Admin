using Durados.Web.Mvc.UI.Helpers;
using Newtonsoft.Json;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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

        ISubscriber subscriber;
       
        public int MessageCount = 0;

        public int ValidMessageCount = 0;

        public int PublishCount = 0;

        public string SubscriberID { get; private set; }
        
        public RedisFarmTransport(string connectionString, string roomName="farm1")
        {
            this.roomName = roomName;
            this.SubscriberID = Guid.NewGuid().ToString();

            //todo: log

            subscriber = CreateRedisConnection(connectionString);

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

            }
        }

        private static ISubscriber CreateRedisConnection(string connectionString)
        {
            ConfigurationOptions options = ConfigurationOptions.Parse(connectionString);
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

            ISubscriber sub = redis.GetSubscriber();
            return sub;
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
