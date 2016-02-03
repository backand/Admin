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
    public class PendingPool
    {
        int size;
        int prev;
        int first;

        public PendingPool(int size) :
            this(size, 1)
        {
        }

        public PendingPool(int size, int first)
        {
            this.size = size;
            this.first = first;
            this.prev = first - 1;
        }

        int last
        {
            get
            {
                return size + first - 1;
            }
        }


        public int Next()
        {
            if (prev >= last)
            {
                return first;
            }
            else
            {
                prev++;
                return prev;
            }
        }
    }
}
