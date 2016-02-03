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
    public class PluginCache
    {
        private Dictionary<int, int> counters = null;
        private int maxCount;
        private int batch;
        private int remains;

        public PluginCache()
        {
            counters = new Dictionary<int, int>();
            maxCount = Convert.ToInt32(System.Configuration.ConfigurationManager.AppSettings["plugInMaxCount"] ?? "2");
            batch = Convert.ToInt32(System.Configuration.ConfigurationManager.AppSettings["plugInBatch"] ?? "4");
            remains = Convert.ToInt32(System.Configuration.ConfigurationManager.AppSettings["plugInRemains"] ?? "1");

        }

        private int GetCount(int sampleId)
        {
            if (!counters.ContainsKey(sampleId))
            {
                counters.Add(sampleId, 0);
            }

            counters[sampleId] += 1;

            return counters[sampleId];
        }

        public void Initiate(int sampleId)
        {
            if (!counters.ContainsKey(sampleId))
            {
                counters.Add(sampleId, 0);
            }

            counters[sampleId] = 0;

        }

        public bool IsPassMaxCount(int sampleId)
        {
            return GetCount(sampleId) >= maxCount;
        }

        public int Batch
        {
            get
            {
                return batch;
            }
        }

        public int Remains
        {
            get
            {
                return remains;
            }
        }
    }
}
