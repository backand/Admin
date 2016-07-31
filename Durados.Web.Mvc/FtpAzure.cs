using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Microsoft.WindowsAzure;
using Microsoft.WindowsAzure.StorageClient;

namespace Durados.Web.Mvc
{
    class FtpAzure:FtpUpload
    {
        #region IUpload Members

        public override  string GetUploadPath(string fileName)
        {
            //throw new NotImplementedException();
            string azureFilePath = fileName;
            if (string.IsNullOrEmpty(TemplatePath))
                azureFilePath = Path.GetFileName(azureFilePath);
            else
                azureFilePath = GetAzureFileTemplatePath(azureFilePath);

            return azureFilePath;
        }

        private string GetAzureFileTemplatePath(string azureFilePath)
        {
            //string path = "ftp://" + FtpHost + ":" + FtpPort.ToString() + "/" + azureFilePath.TrimStart('/');
            //return path;
            return string.Empty;
        }

       // public override string TemplatePath { get; set; }

        public override  void CreateNewDirectory(string newDirPath)
        {
            //throw new NotImplementedException();
        }

        public override void CreateNewDirectory2(string newPhisicalPath)
        {
        //    throw new NotImplementedException();
        }

        public override  void DeleteOldFile(string newPhisicalPath)
        {
            //DeleteOldFileInAzure(newPhisicalPath);
        }

        private void DeleteOldFileInAzure(string newPhisicalPath)
        {
            throw new NotImplementedException();
        }

        public override  void MoveUploadedFile(string oldPath, string newPhisicalPath)
        {

           // SaveFileToAzure(oldPath, newPhisicalPath);
            
        }

        private void SaveFileToAzure(string oldPath, string newPhisicalPath)
        {
            string fileUri = string.Empty;
            try
            {
                // Variables for the cloud storage objects.
                CloudStorageAccount cloudStorageAccount;
                CloudBlobClient blobClient;
                CloudBlobContainer blobContainer;
                //BlobContainerPermissions containerPermissions;
                CloudBlob blob;

                string accountKey = GetDecryptedAzureAccountKey(Maps.Instance.GetMap().GetConfigDatabase());
                //cloudStorageAccount = CloudStorageAccount.Parse("DefaultEndpointsProtocol=http;AccountName=qacontent;AccountKey=6/OpnuGeLRm8REKhQ/5Led/WoJcHjmFJII4GM5HYJ120o7OxtgC1zCstw0kqyn05N2m1jGadY3aYFypOzD4L5A==");
                cloudStorageAccount = CloudStorageAccount.Parse(string.Format("DefaultEndpointsProtocol=http;AccountName={0};AccountKey={1}", AzureAccountKey, accountKey));

                // Create the blob client, which provides
                // authenticated access to the Blob service.
                blobClient = cloudStorageAccount.CreateCloudBlobClient();

                // Get the container reference.
                //blobContainer = blobClient.GetContainerReference("general");
                blobContainer = blobClient.GetContainerReference(DirectoryBasePath);

                // Create the container if it does not exist.
                //blobContainer.CreateIfNotExist();
                // Set permissions on the container.
                //containerPermissions = new BlobContainerPermissions();

                // Get a reference to the blob.
                blob = blobContainer.GetBlobReference(newPhisicalPath);

                ////blob.Properties.ContentType = Request.Files[0].ContentType;

                // Upload a file from the local system to the blob.
                ////blob.UploadFile(Request.Files[0].FileName);  // File from emulated storage.
                ////blob.UploadFromStream(Request.Files[0].InputStream);
                ////fileUri = blob.Uri.ToString();


            }
            catch (Exception exception)
            {
                Maps.Instance.GetMap().Logger.Log("FtpAzure", "MoveUploadFile", exception.Source, exception, 1, null);
                throw new DuradosException(Maps.Instance.GetMap().Database.Localizer.Translate("Operation failed" + ", " + exception.Message));
            }

            //return fileUri;
        }

        public override  bool IsFileExists(string oldPath)
        {
            //return IsExist(oldPath);
            return false;
        }

        private bool IsExist(string oldPath)
        {
            var blob = CloudStorageAccount.DevelopmentStorageAccount
        .CreateCloudBlobClient().GetBlobReference(oldPath);
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

        public override  string GetBaseUploadPath(string fileName)
        {
            return string.Empty;// throw new NotImplementedException();
        }

        #endregion
    }
}
