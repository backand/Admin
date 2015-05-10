using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.WindowsAzure;
using Microsoft.WindowsAzure.ServiceRuntime;
using Microsoft.WindowsAzure.StorageClient;
using Microsoft.WindowsAzure.StorageClient.Protocol;
using System.Threading.Tasks;

namespace Durados.Web.Mvc.Azure
{
    public static class StorageHelper
    {
        public static CloudStorageAccount GetAccount()
        {

            //return (RoleEnvironment.IsEmulated) ? CloudStorageAccount.DevelopmentStorageAccount :
            //    CloudStorageAccount.Parse(RoleEnvironment.GetConfigurationSettingValue(serviceConfigKey));

            CloudStorageAccount cloudStorageAccount = CloudStorageAccount.Parse(string.Format("DefaultEndpointsProtocol=http;AccountName={0};AccountKey={1}", Maps.ConfigAzureStorageAccountName, Maps.ConfigAzureStorageAccountKey));
            return cloudStorageAccount;
        }

        public static CloudBlobContainer GetContainer(CloudStorageAccount account, string filename)
        {
            // Get a handle on account, create a blob service client and get container proxy
            var client = account.CreateCloudBlobClient();
            //CloudBlobContainer  cloudBlobContainer  = client.GetContainerReference(RoleEnvironment.GetConfigurationSettingValue(serviceConfigKey));
            CloudBlobContainer cloudBlobContainer = client.GetContainerReference(filename);

            cloudBlobContainer.CreateIfNotExist();

            return cloudBlobContainer;
        }

        public static bool Exists(this CloudBlob blob)
        {
            try
            {
                blob.FetchAttributes();
                return true;
            }
            catch (StorageClientException e)
            {
                if (e.ErrorCode == StorageErrorCode.ResourceNotFound)
                {
                    return false;
                }
                else
                {
                    throw;
                }
            }
        }

        
    }

    public class BlobBackup
    {
        public int NumberOfCopies { get; set; }

        public BlobBackup()
        {
            NumberOfCopies = 5;
        }

        public void BackupAsync(CloudBlobContainer container, string blobName)
        {
            CopyAsync(container, blobName, NumberOfCopies);
        }

        private void CopyAsync(CloudBlobContainer container, string blobName, int copy)
        {
            var source = container.GetBlobReference(GetSource(blobName, copy));
            var target = container.GetBlobReference(GetTarget(blobName, copy));
            CopyAsyncState state = new CopyAsyncState() { Container = container, BlobName = blobName, Target = target, Copy = copy - 1 };
            target.BeginCopyFromBlob(source, CopyAsyncCompletedCallback, state);
        }

        private string GetTarget(string blobName, int copy)
        {
            return blobName + "B" + copy;
        }

        private string GetSource(string blobName, int copy)
        {
            if (copy == 1)
                return blobName;
            return blobName + "B" + (copy - 1);
        }

        private void CopyAsyncCompletedCallback(IAsyncResult result)
        {
            CopyAsyncState state = (CopyAsyncState)result.AsyncState;
            try
            {
                state.Target.EndCopyFromBlob(result);
            }
            catch { }
            if (state.Copy > 0)
            {
                CopyAsync(state.Container, state.BlobName, state.Copy);
            }
        }

        public class CopyAsyncState
        {
            public CloudBlobContainer Container { get; set; }
            public string BlobName { get; set; }
            public int Copy { get; set; }
            public CloudBlob Target { get; set; }
        }
    }
}
