using Microsoft.WindowsAzure;
using Microsoft.WindowsAzure.ServiceRuntime;
using Microsoft.WindowsAzure.StorageClient;
using Microsoft.WindowsAzure.StorageClient.Protocol;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Durados.Windows.Utilities.AzureUploader
{
    public class Storage
    {
        CloudStorageAccount account = null;
        CloudBlobClient client = null;

        public void Connect(string accountName, string accountKey)
        {
            account = CloudStorageAccount.Parse(string.Format("DefaultEndpointsProtocol=http;AccountName={0};AccountKey={1}", accountName, accountKey));
            client = account.CreateCloudBlobClient();
        }

        public void Upload(string filename)
        {
            if (account == null)
                throw new Exception("Please connect to an Azure account weith an Account Name and an Account Key");

            System.IO.FileInfo fileInfo = new System.IO.FileInfo(filename);
            string filenameOnly = fileInfo.Name.Remove(fileInfo.Name.Length - fileInfo.Extension.Length, fileInfo.Extension.Length);

            string containerName = filenameOnly.Replace("_", "").Replace(".", "").ToLower();

            CloudBlobContainer container = GetContainer(containerName);

            CloudBlob blob = container.GetBlobReference(containerName);

            blob.UploadFile(filename);
        }

        public void Upload(string containerName, string blobName, string filename)
        {
            if (account == null)
                throw new Exception("Please connect to an Azure account with an Account Name and an Account Key");

            CloudBlobContainer container = GetContainer(containerName);
            BlobContainerPermissions containerPermissions = new BlobContainerPermissions();
            containerPermissions.PublicAccess = BlobContainerPublicAccessType.Blob;
            container.SetPermissions(containerPermissions);

            CloudBlob blob = container.GetBlobReference(blobName);

            blob.UploadFile(filename);
        }

        public CloudBlobContainer GetContainer(string filename)
        {
            // Get a handle on account, create a blob service client and get container proxy
            //CloudBlobContainer  cloudBlobContainer  = client.GetContainerReference(RoleEnvironment.GetConfigurationSettingValue(serviceConfigKey));
            CloudBlobContainer cloudBlobContainer = client.GetContainerReference(filename);
            
            cloudBlobContainer.CreateIfNotExist();

            return cloudBlobContainer;
        }

        public IEnumerable<CloudBlobContainer> ListContainers(string prefix)
        {
            return client.ListContainers(prefix);
        }

        public bool Exists(CloudBlob blob)
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

        public void Copy(CloudBlob source, CloudBlob target)
        {
            target.CopyFromBlob(source);
        }
    }
}
