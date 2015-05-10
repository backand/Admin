using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.WindowsAzure.StorageClient;
using Microsoft.WindowsAzure;

namespace Durados.Web.Mvc.UI.Helpers
{
    public static class AzureHelper
    {
        public static CloudBlobContainer Duplicate(this CloudBlobContainer container, string newContainerName)
        {
            CloudBlobContainer newContainer = CreateContainer(Maps.AzureStorageAccountKey, Maps.AzureStorageAccountName, newContainerName);
            foreach (CloudBlob blob in container.ListBlobs())
            {
                CloudBlob newBlob = newContainer.GetBlobReference(blob.Name);
                newBlob.CopyFromBlob(blob);
            }

            return newContainer;
        }

        public static CloudBlobContainer CreateContainer(string accountKey, string accountName, string containerName)
        {
            BlobContainerPermissions containerPermissions;

            CloudBlobContainer blobContainer = GetContainerReference(accountKey, accountName, containerName);
            blobContainer.CreateIfNotExist();
            containerPermissions = new BlobContainerPermissions();
            containerPermissions.PublicAccess = BlobContainerPublicAccessType.Blob;
            blobContainer.SetPermissions(containerPermissions);

            return blobContainer;    
        }

        public static bool DoesDefaultContainerExist(string containerName)
        {
            return DoesContainerExist(GetDefaultBlobClient(), containerName);
        }

        /// <summary>
        /// Checks if a container exist.
        /// </summary>
        /// <param name="containerName">Name of the container.</param>
        /// <returns>True if conainer exists</returns>
        public static bool DoesContainerExist(CloudBlobClient blobClient, string containerName)
        {
            bool returnValue = false;
            ExecuteWithExceptionHandling(
                    () =>
                    {
                        IEnumerable<CloudBlobContainer> containers = blobClient.ListContainers();
                        returnValue = containers.Any(one => one.Name == containerName);
                    });
            return returnValue;
        }

        private static void ExecuteWithExceptionHandling(Action action)
        {
            try
            {
                action();
            }
            catch (StorageClientException ex)
            {
                if ((int)ex.StatusCode != 409)
                {
                    throw;
                }
            }
        }


        public static CloudBlobContainer GetDefaultContainerReference(int appId)
        {
            
            string containerName = Maps.AzureAppPrefix + appId;

            if (!DoesContainerExist(GetDefaultBlobClient(), containerName))
                return null;
            
            CloudBlobContainer blobContainer = GetContainerReference(Maps.AzureStorageAccountKey, Maps.AzureStorageAccountName, containerName);
            
            return blobContainer;
        }


        public static CloudBlobClient GetDefaultBlobClient()
        {
            return GetBlobClient(Maps.AzureStorageAccountKey, Maps.AzureStorageAccountName);
        }

        public static CloudBlobClient GetBlobClient(string accountKey, string accountName)
        {
            // Variables for the cloud storage objects.
            CloudStorageAccount cloudStorageAccount;

            //cloudStorageAccount = CloudStorageAccount.Parse("DefaultEndpointsProtocol=http;AccountName=qacontent;AccountKey=6/OpnuGeLRm8REKhQ/5Led/WoJcHjmFJII4GM5HYJ120o7OxtgC1zCstw0kqyn05N2m1jGadY3aYFypOzD4L5A==");
            cloudStorageAccount = CloudStorageAccount.Parse(string.Format("DefaultEndpointsProtocol=http;AccountName={0};AccountKey={1}", accountName, accountKey));

            // Create the blob client, which provides
            // authenticated access to the Blob service.
            return cloudStorageAccount.CreateCloudBlobClient();

        }

        public static CloudBlobContainer GetContainerReference(string accountKey, string accountName, string containerName)
        {
            CloudBlobClient blobClient;
            CloudBlobContainer blobContainer;
            
            blobClient = GetBlobClient(accountKey, accountName);

            // Get the container reference.
            //blobContainer = blobClient.GetContainerReference("general");
            blobContainer = blobClient.GetContainerReference(containerName);

            return blobContainer;
        }

        public static CloudBlobContainer GetContainerReference(CloudBlobClient blobClient, string containerName)
        {
            CloudBlobContainer blobContainer;

            // Get the container reference.
            //blobContainer = blobClient.GetContainerReference("general");
            blobContainer = blobClient.GetContainerReference(containerName);

            return blobContainer;
        }

        public static string SaveUploadedFileToAzure(string accountName, string accountKey, string folder, string strFileName)
        {
            return SaveUploadedFileToAzure(accountName, accountKey, folder, strFileName, System.Web.HttpContext.Current.Request.Files[0].ContentType, System.Web.HttpContext.Current.Request.Files[0].InputStream);
        }

        public static string SaveUploadedFileToAzure(string accountName, string accountKey, string folder, string strFileName, string contentType, System.IO.Stream stream)
        {
            string fileUri = string.Empty;

            if (Maps.AzureStorageAccountKey == accountKey)
                folder = Maps.AzureAppPrefix + Maps.Instance.GetCurrentAppId(); //Azure must have at least 3 chars

            // Variables for the cloud storage objects.
            CloudStorageAccount cloudStorageAccount;
            CloudBlobClient blobClient;
            CloudBlobContainer blobContainer;
            BlobContainerPermissions containerPermissions;
            CloudBlob blob;

            //cloudStorageAccount = CloudStorageAccount.Parse("DefaultEndpointsProtocol=http;AccountName=qacontent;AccountKey=6/OpnuGeLRm8REKhQ/5Led/WoJcHjmFJII4GM5HYJ120o7OxtgC1zCstw0kqyn05N2m1jGadY3aYFypOzD4L5A==");
            cloudStorageAccount = CloudStorageAccount.Parse(string.Format("DefaultEndpointsProtocol=http;AccountName={0};AccountKey={1}", accountName, accountKey));

            // Create the blob client, which provides
            // authenticated access to the Blob service.
            blobClient = cloudStorageAccount.CreateCloudBlobClient();

            // Get the container reference.
            //blobContainer = blobClient.GetContainerReference("general");
            blobContainer = blobClient.GetContainerReference(folder);

            // Create the container if it does not exist.
            if (Maps.AzureStorageAccountKey == accountKey)
            {
                // Set permissions on the container.
                blobContainer.CreateIfNotExist();
                containerPermissions = new BlobContainerPermissions();
                containerPermissions.PublicAccess = BlobContainerPublicAccessType.Blob;
                blobContainer.SetPermissions(containerPermissions);
            }

            // Get a reference to the blob.
            blob = blobContainer.GetBlobReference(System.IO.Path.GetFileNameWithoutExtension(strFileName) + "_" + Guid.NewGuid() + System.IO.Path.GetExtension(strFileName));

            blob.Properties.ContentType = contentType;

            // Upload a file from the local system to the blob.
            //blob.UploadFile(Request.Files[0].FileName);  // File from emulated storage.
            blob.UploadFromStream(stream);
            fileUri = blob.Uri.ToString();



            return fileUri;

        }
    }
}
