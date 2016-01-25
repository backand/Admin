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
    public class JsonConfigCache
    {
        public Dictionary<string, Dictionary<string, object>> jsonConfigCache = new Dictionary<string, Dictionary<string, object>>();

        public Dictionary<string, object> Get(string name)
        {
            if (jsonConfigCache.ContainsKey(name))
                return jsonConfigCache[name];

            return null;
        }

        public bool ContainsKey(string name)
        {
            return jsonConfigCache.ContainsKey(name);
        }

        public void Set(string name, Dictionary<string, object> value)
        {
            if (jsonConfigCache.ContainsKey(name))
                jsonConfigCache[name] = value;
            else
                jsonConfigCache.Add(name, value);
        }
        public void Clear()
        {
            jsonConfigCache.Clear();
        }
    }
}
