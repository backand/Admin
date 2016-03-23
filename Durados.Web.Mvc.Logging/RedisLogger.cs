using Durados.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Durados.Web.Mvc.Logging
{
    public class RedisLogger
    {
        const string BackandLogKey = "log_api";

        private ISharedMemory redisProvider;

        public RedisLogger(ISharedMemory redisProvider)
        {
            this.redisProvider = redisProvider;
        }

        public void Log(string message)
        {
            redisProvider.ListRightPush(BackandLogKey, message);
        }
        
    }
}
