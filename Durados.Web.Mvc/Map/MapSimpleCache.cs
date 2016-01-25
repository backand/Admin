using System;
using System.Data;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Web;
using System.IO;
using System.Reflection;
using System.Data.SqlClient;

using Microsoft.WindowsAzure;
using Microsoft.WindowsAzure.ServiceRuntime;
using Microsoft.WindowsAzure.StorageClient;
using Microsoft.WindowsAzure.StorageClient.Protocol;

using Durados.DataAccess;
using Durados.Web.Mvc.UI.Helpers;
using Durados.Web.Mvc.Infrastructure;
using Durados.Web.Mvc.Azure;
using Newtonsoft.Json;
using System.Diagnostics;
using Durados.Data;
using Durados.SmartRun;
using Durados.Web.Mvc.Farm;

namespace Durados.Web.Mvc
{
    public class MapSimpleCache
    {
        public Dictionary<string, object> cache = new Dictionary<string, object>();

        public object Get(string name)
        {
            if (cache.ContainsKey(name))
                return cache[name];

            return null;
        }

        public bool ContainsKey(string name)
        {
            return cache.ContainsKey(name);
        }

        public void Set(string name, object value)
        {
            if (cache.ContainsKey(name))
                cache[name] = value;
            else
                cache.Add(name, value);
        }
    }
}
