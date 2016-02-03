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
        public string BackupPrefix { get; set; }
        public string VersionPrefix { get; set; }
        public string UploadPrefix { get; set; }

        public BlobBackup()
        {
            NumberOfCopies = 5;
            BackupPrefix = "B";
            VersionPrefix = "ver";
            UploadPrefix = "up";

            if (Maps.IsApi2())
            {
                NumberOfCopies = 1;
                BackupPrefix = "BU";
           
            }
            
        }

        public void BackupAsync(CloudBlobContainer container, string blobName)
        {
            CopyAsync(container, blobName, NumberOfCopies);
        }

        public void BackupSync(CloudBlobContainer container, string blobName)
        {
            Copy(container, blobName, NumberOfCopies);
        }

        public void BackupSync(CloudBlobContainer container, string blobName, string version)
        {
            Copy(container, blobName, NumberOfCopies);

            var source = container.GetBlobReference(blobName);
            var target = container.GetBlobReference(blobName + VersionPrefix + version);
            DeleteOldAsyncState state = new DeleteOldAsyncState() { Container = container, BlobName = blobName, Target = target, Total = 5 };
            target.BeginCopyFromBlob(source, DeleteOldAsyncCompletedCallback, state);
        }

        public void RestoreSync(CloudBlobContainer container, string blobName)
        {
            CopyBack(container, blobName, NumberOfCopies);
        }

        private void CopyAsync(CloudBlobContainer container, string blobName, int copy)
        {
            var source = container.GetBlobReference(GetSource(blobName, copy));
            var target = container.GetBlobReference(GetTarget(blobName, copy));
            CopyAsyncState state = new CopyAsyncState() { Container = container, BlobName = blobName, Target = target, Copy = copy - 1 };
            target.BeginCopyFromBlob(source, CopyAsyncCompletedCallback, state);
        }

        private void Copy(CloudBlobContainer container, string blobName, int copy)
        {
            var source = container.GetBlobReference(GetSource(blobName, copy));
            var target = container.GetBlobReference(GetTarget(blobName, copy));
            target.CopyFromBlob(source);
        }

        private void CopyBack(CloudBlobContainer container, string blobName, int copy)
        {
            var source = container.GetBlobReference(GetSource(blobName, copy));
            var target = container.GetBlobReference(GetTarget(blobName, copy));
            target.CopyFromBlob(source);
        }

        public void CopyBack(CloudBlobContainer container, string version, string targetName)
        {
            
            var source = container.GetBlobReference(targetName + VersionPrefix + version);
            if (!source.Exists())
            {
                throw new DuradosException("Could not find configuration version " + version);
            }
            var target = container.GetBlobReference(targetName);
            target.CopyFromBlob(source);
        }

        private string GetTarget(string blobName, int copy)
        {
            return blobName + BackupPrefix + copy;
        }

        private string GetSource(string blobName, int copy)
        {
            if (copy == 1)
                return blobName;
            return blobName + BackupPrefix + (copy - 1);
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

        private void DeleteOldAsyncCompletedCallback(IAsyncResult result)
        {
            DeleteOldAsyncState state = (DeleteOldAsyncState)result.AsyncState;
            try
            {
                state.Target.EndCopyFromBlob(result);
            }
            catch { }
            try
            {
                DeleteOld(state.Container, state.BlobName, state.Total);
            }
            catch { }
        }

        private void DeleteOld(CloudBlobContainer cloudBlobContainer, string blobName, int total)
        {
            IEnumerable<IListBlobItem> blobItems = cloudBlobContainer.ListBlobs();

            SortedDictionary<long, CloudBlob> verBlobs = GetVerBlobs(cloudBlobContainer, blobItems);
            if (verBlobs.Count <= total)
                return;

            CloudBlob[] blobs = verBlobs.Values.ToArray();
            for (long i = 0; i < blobs.Length - 1; i++)
            {
                if (blobs.Length - i > total)
                {
                    CloudBlob blob = blobs[i];
                    blob.Delete();
                }
            }
        }

        private SortedDictionary<long, CloudBlob> GetVerBlobs(CloudBlobContainer cloudBlobContainer, IEnumerable<IListBlobItem> blobItems)
        {
            SortedDictionary<long, CloudBlob> verBlobs = new SortedDictionary<long, CloudBlob>();

            foreach (IListBlobItem blobItem in blobItems)
            {
                var blob = cloudBlobContainer.GetBlobReference(blobItem.Uri.ToString());
                string name = blob.Name;

                if (name.Contains(VersionPrefix))
                {
                    string version = name.Split(VersionPrefix.ToCharArray()).LastOrDefault();
                    if (!string.IsNullOrEmpty(version))
                    {
                        long versionNumber = 0;
                        if (long.TryParse(version.Replace(".", ""), out versionNumber))
                        {
                            verBlobs.Add(versionNumber, blob);
                        }
                    }
                }
            }

            return verBlobs;
        }

        public string[] GetVersions(CloudBlobContainer cloudBlobContainer)
        {
            IEnumerable<IListBlobItem> blobItems = cloudBlobContainer.ListBlobs();
            SortedDictionary<long, CloudBlob> verBlobs = GetVerBlobs(cloudBlobContainer, blobItems);
            List<string> versions = new List<string>();
            foreach (var blob in verBlobs.Values)
            {
                if (blob.Name.Contains(VersionPrefix))
                {
                    versions.Add(blob.Name.Split(VersionPrefix.ToCharArray()).LastOrDefault());
                }
            }

            return versions.ToArray();
        }

        public class CopyAsyncState
        {
            public CloudBlobContainer Container { get; set; }
            public string BlobName { get; set; }
            public int Copy { get; set; }
            public CloudBlob Target { get; set; }
        }

        public class DeleteOldAsyncState
        {
            public CloudBlobContainer Container { get; set; }
            public string BlobName { get; set; }
            public int Total { get; set; }
            public CloudBlob Target { get; set; }
        }
    }
}
