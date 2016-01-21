﻿using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Durados.Web.Mvc.Farm
{
    public class SharedMemorySingeltone
    {
        private static object locker1 = new object();

        public static ISharedMemory Instance
        {
            get
            {
                if (instance == null)
                {
                    lock (locker1)
                    {
                        if (instance == null)
                        {
                            instance = CreateInstance();
                        }
                    }
                }

                return instance;
            }
        }

        private static ISharedMemory CreateInstance()
        {
            return new SharedMemory(Maps.RedisConnectionString);
        }

        private static ISharedMemory instance { get; set; }

    }

    public class SharedMemory : ISharedMemory
    {
        ConnectionMultiplexer redis;
        IDatabase database;

        public SharedMemory(string connectionString)
        {
            redis = RedisFarmTransport.CreateRedisConnection(connectionString);
            database = redis.GetDatabase();

        }

        const string Seperator = "//";

        private string GetKey(string appName, SharedMemoryKey key)
        {
            return appName + Seperator + key.ToString();
        }

        public void Delete(string appName, SharedMemoryKey key)
        {
            database.KeyDelete(GetKey(appName, key));
        }

        public void Set(string appName, SharedMemoryKey key, string value, int milliseconds)
        {
            database.StringSet(GetKey(appName, key), value, new TimeSpan(0, 0, 0, 0, milliseconds));
        }

        public string Get(string appName, SharedMemoryKey key)
        {
            return database.StringGet(GetKey(appName, key));
        }

        public bool Contains(string appName, SharedMemoryKey key)
        {
            return database.KeyExists(GetKey(appName, key));
        }

    }

}
