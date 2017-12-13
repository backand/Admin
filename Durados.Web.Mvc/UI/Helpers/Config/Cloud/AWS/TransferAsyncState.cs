using Amazon.S3.Transfer;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Durados.Web.Mvc.UI.Helpers.Config.Cloud.AWS
{
    public class TransferAsyncState
    {
        public TransferUtility FileTransferUtility;
        public Stream Stream;
        public DateTime Started;
        public string ContainerName;
        public string Key;
        public Map Map;

        public TransferAsyncState(TransferUtility fileTransferUtility, Stream stream, DateTime started, string containerName, string key, Map map)
        {
            FileTransferUtility = fileTransferUtility;
            Stream = stream;
            Started = started;
            ContainerName = containerName;
            Key = key;
            Map = map;
        }
    }
}
