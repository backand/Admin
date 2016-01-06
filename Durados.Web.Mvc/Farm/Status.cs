﻿using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Durados.Web.Mvc.Farm
{
    public class Status : IStatus
    {
        IDatabase database;
        string subscriberID;

        public Status(IDatabase database, string subscriberID)
        {
            this.database = database;
            this.subscriberID = subscriberID;
        }

        public void Clear(string appName)
        {
            database.KeyDelete(appName);
        }

        public void Set(string appName)
        {
            database.StringSet(appName, subscriberID);
        }

        public bool Contains(string appName)
        {
            return database.KeyExists(appName) && database.StringGet(appName) != subscriberID;
        }
    }
}
