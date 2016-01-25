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
    public class BlobTransferAsyncState
    {
        public CloudBlob Blob;
        public Stream Stream;
        public DateTime Started;
        public bool Cancelled;
        public CloudBlobContainer Container;
        public string BlobName;
        public Map Map;

        public BlobTransferAsyncState(CloudBlob blob, Stream stream, CloudBlobContainer container, string blobName, Map map)
            : this(blob, stream, DateTime.Now, container, blobName, map)
        { }

        public BlobTransferAsyncState(CloudBlob blob, Stream stream, DateTime started, CloudBlobContainer container, string blobName, Map map)
        {
            Blob = blob;
            Stream = stream;
            Started = started;
            Cancelled = false;
            Container = container;
            BlobName = blobName;
            Map = map;
        }
    }
}
