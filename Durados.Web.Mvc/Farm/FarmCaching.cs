﻿using Durados.Diagnostics;
using Durados.Web.Mvc.Farm;
using Durados.Web.Mvc.Infrastructure;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Durados.Web.Mvc.UI.Helpers
{
    public class FarmCachingSingeltone
    {
        private static object locker1 = new object();

        public static IFarmCaching Instance
        {
            get
            {
                if(instance == null)
                {
                    lock(locker1)
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

        private static IFarmCaching CreateInstance()
        {
            return  new FarmCachingNormal(new RedisFarmTransport(Maps.RedisConnectionString));
        }

        private static IFarmCaching instance { get; set; }

        static FarmCachingSingeltone()
        {
            
        }



        private FarmCachingSingeltone()
        {

        }
    }

    public class FarmCachingNormal : IFarmCaching, IDisposable
    {
        private IFarmPubsubTransport transport;

        private ILogger logger = Maps.Instance.DuradosMap.Logger;

        public FarmCachingNormal(IFarmPubsubTransport transport)
        {
            if(transport == null)
            {
                //todo: throw nullException
            }

            this.transport = transport;
            logger.Log("FarmNormal", null, string.Empty, null, 3, "Start transport with " + this.transport.GetType());
            transport.OnMessage += transport_OnMessage;
        }

        void transport_OnMessage(object sender, FarmEventArgs e)
        {
            if(e.Message == null || string.IsNullOrEmpty(e.Message.AppName))
            {
                logger.Log("FarmNormal", string.Empty, string.Empty, null, 1, "Message Arrive Null");
                // todo: log
                return;
            }

            logger.Log("FarmNormal", null, string.Empty, null, 3, "Arrive Message for app " + e.Message.AppName);
            logger.Log("FarmNormal","Perf", string.Empty, null, 3, (DateTime.Now - e.Message.Time).Milliseconds.ToString());

            this.ClearInternalCache(e.Message.AppName);
        }

        public void ClearInternalCache(string appName)
        {
            if (Maps.Instance.AppInCach(appName))
                RestHelper.Refresh(appName);
        }
       
        public void AppEnded()
        {
            this.Dispose();
            logger.Log("FarmNormal", string.Empty, string.Empty, null, 3 , "Finish dispose");
        }

        public void Dispose()
        {
            if(transport != null)
            {
                transport.OnMessage -= transport_OnMessage;
                if(transport is IDisposable)
                {
                    ((IDisposable)transport).Dispose();
                }
            }
        }

        public void ClearMachinesCache(string appName, bool async = false)
        {
            if(string.IsNullOrEmpty(appName))
            {
                return;
            }

            logger.Log("FarmNormal", null, string.Empty, null, 3, "Send Message for app " + appName);
            this.transport.Publish(new FarmMessage { AppName = appName, Time = DateTime.Now });
        }
    }
}
