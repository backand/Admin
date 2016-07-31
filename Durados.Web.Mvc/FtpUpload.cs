using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.WindowsAzure;
using Microsoft.WindowsAzure.StorageClient;
using System.Text.RegularExpressions;
using System.Net;
using Amazon.S3;
using Amazon;
using Amazon.S3.Model;

namespace Durados.Web.Mvc
{
    public class FtpUpload : IUpload
    {
        private static string ftpPrefix = "ftp://";

        [Durados.Config.Attributes.ColumnProperty()]
        public string Title { get; set; }

        [Durados.Config.Attributes.ColumnProperty()]
        public bool Override { get; set; }

        private int fileMaxSize;

        [Durados.Config.Attributes.ColumnProperty()]
        public int FileMaxSize
        {
            get
            {
                if (fileMaxSize == 0) return DefaultFileSize;
                return fileMaxSize;
            }
            set
            {
                if (value <= 0)
                    fileMaxSize = DefaultFileSize;
                if (value > MaxFileSize)
                    fileMaxSize = MaxFileSize;
                else
                    fileMaxSize = value;
            }
        }

        [Durados.Config.Attributes.ColumnProperty()]
        public UploadFileType UploadFileType { get; set; }

        [Durados.Config.Attributes.ColumnProperty()]
        public string FileAllowedTypes { get; set; }

        [Durados.Config.Attributes.ColumnProperty()]
        public string DirectoryVirtualPath { get; set; }

        [Durados.Config.Attributes.ColumnProperty()]
        public string DirectoryBasePath { get; set; }

        [Durados.Config.Attributes.ColumnProperty()]
        public string TemplatePath { get; set; }

        [Durados.Config.Attributes.ColumnProperty()]
        public string FtpUserName { get; set; }

        [Durados.Config.Attributes.ColumnProperty()]
        public bool UsePassive { get; set; }

        [Durados.Config.Attributes.ColumnProperty()]
        public int Width { get; set; }

        [Durados.Config.Attributes.ColumnProperty()]
        public int Height { get; set; }

        [Durados.Config.Attributes.ColumnProperty()]
        public string FtpPassword { get; set; }

        [Durados.Config.Attributes.ColumnProperty()]
        public string AzureAccountName { get; set; }

        [Durados.Config.Attributes.ColumnProperty()]
        public string AzureAccountKey { get; set; }

        [Durados.Config.Attributes.ColumnProperty()]
        public string AwsAccessKeyId { get; set; }

        [Durados.Config.Attributes.ColumnProperty()]
        public string AwsSecretAccessKey { get; set; }

        [Durados.Config.Attributes.ColumnProperty()]
        public StorageType StorageType { get; set; }

        public string GetDecryptedPassword(Database database)
        {
            return Durados.Security.CipherUtility.Decrypt<System.Security.Cryptography.AesManaged>(FtpPassword, database.DefaultMasterKeyPassword, database.Salt);
        }
        public string GetDecryptedAzureAccountKey(Database database)
        {
            return Durados.Security.CipherUtility.Decrypt<System.Security.Cryptography.AesManaged>(AzureAccountKey, database.DefaultMasterKeyPassword, database.Salt);
        }

        public string GetDecryptedAwsSecretAccessKey(Database database)
        {
            return Durados.Security.CipherUtility.Decrypt<System.Security.Cryptography.AesManaged>(AwsSecretAccessKey, database.DefaultMasterKeyPassword, database.Salt);
        }

        //private string ftpPassword;

        //[Durados.Config.Attributes.ColumnProperty()]
        //public string FtpPassword { 

        //    get 
        //    {
        //        if (string.IsNullOrEmpty(ftpPassword)) return string.Empty;

        //        return CipherUtility.Decrypt<AesManaged>(ftpPassword, "password", GetFtpSalt()); 
        //    }            

        //    set
        //    {
        //        //Available encription methods: <TripleDESCryptoServiceProvider>/<RijndaelManaged>
        //        if (!string.IsNullOrEmpty(value))
        //            ftpPassword = CipherUtility.Encrypt<AesManaged>(value, "password", GetFtpSalt());
        //        else                
        //            ftpPassword = value; //maybe if empty do not set
        //    } 
        //}




        public virtual string GetFtpBasePath(string strFileName)
        {
            if (!string.IsNullOrEmpty(DirectoryBasePath))
                strFileName = DirectoryBasePath.Trim('/') + "/" + Path.GetFileName(strFileName);
            else
                strFileName = Path.GetFileName(strFileName);
            // return "ftp://" + FtpHost + ":" + FtpPort.ToString() + "/";
            return strFileName;
        }

        public virtual string GetFtpFilepath(string strFileName)
        {
            return "ftp://" + FtpHost + ":" + FtpPort.ToString() + "/" + GetFtpBasePath(strFileName);
        }

        public virtual string GetFileTemplatePath(string fileName)
        {
            return GetFtpFileTemplatePath(fileName);
        }
        public virtual string GetFtpFileTemplatePath(string strFileName)
        {
            string path = "ftp://" + FtpHost + ":" + FtpPort.ToString() + "/" + strFileName.TrimStart('/');
            return path;
        }

        /// <summary>
        /// Check FTP connection to the server
        /// </summary>
        /// <param name="host"></param>
        /// <param name="port"></param>
        /// <param name="username"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        public bool CheckConnection(string host, string port, string username, string password)
        {
            try
            {
                if (!string.IsNullOrEmpty(port)) host += ":" + port;
                System.Net.FtpWebRequest myFTP = (System.Net.FtpWebRequest)System.Net.HttpWebRequest.Create("ftp://" + host);

                myFTP.Credentials = new System.Net.NetworkCredential(username, password);

                myFTP.UsePassive = true;
                myFTP.UseBinary = true;
                myFTP.KeepAlive = false;

                myFTP.Method = System.Net.WebRequestMethods.Ftp.PrintWorkingDirectory;

                myFTP.GetResponse();

                return true;
            }
            catch (Exception ex)
            {
                throw new DuradosException("FTP Connection failed, Error: " + ex.Message);
                //return false;
            }
        }

        /// <summary>
        /// Check Azure connection
        /// </summary>
        /// <param name="azureAccountName"></param>
        /// <param name="azureAccountKey"></param>
        /// <returns></returns>
        public bool CheckConnection(string azureAccountName, string azureAccountKey, string folder)
        {
            try
            {
                CloudStorageAccount cloudStorageAccount;
                CloudBlobClient blobClient;
                CloudBlobContainer blobContainer;

                cloudStorageAccount = CloudStorageAccount.Parse(string.Format("DefaultEndpointsProtocol=http;AccountName={0};AccountKey={1}", azureAccountName, azureAccountKey));

                blobClient = cloudStorageAccount.CreateCloudBlobClient();

                blobContainer = blobClient.GetContainerReference(folder);

                blobContainer.FetchAttributes();

                return true;
            }
            catch (Exception ex)
            {
                throw new DuradosException("Azure Connection failed, Error: " + ex.Message);
                //return false;
            }
        }

        /// <summary>
        /// Check Azure connection
        /// </summary>
        /// <param name="azureAccountName"></param>
        /// <param name="azureAccountKey"></param>
        /// <returns></returns>
        public bool CheckAwsConnection(string accessKeyID, string secretAccessKey, string path, bool create)
        {
            return IsPathExists(accessKeyID, secretAccessKey, path, create);
        }

        public string GetBucketNameFromPath(string path, string seperator)
        {
            if (string.IsNullOrEmpty(path))
                return null;

            string[] objectNames = GetObjectNames(path, seperator);

            if (objectNames.Length < 1)
                return null;

            return objectNames[0];
        }

        public string[] GetObjectNames(string path, string seperator)
        {
            return path.Split(seperator.ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
        }

        public bool IsPathExists(string accessKeyID, string secretAccessKey, string path, bool create)
        {
            string seperator = "/";
            if (string.IsNullOrEmpty(path))
                return false;

            string[] objectNames = GetObjectNames(path, seperator);

            if (objectNames.Length < 1)
                return false;

            return IsBucketNameExists(accessKeyID, secretAccessKey, objectNames[0], create);

            //if (!IsBucketNameExists(accessKeyID, secretAccessKey, objectNames[0], create))
            //    return false;

            //if (objectNames.Length == 1)
            //    return true;

            //string bucketName = objectNames[0];
            //string key = path.TrimStart(seperator.ToCharArray()).TrimStart(bucketName.ToCharArray()).TrimEnd(seperator.ToCharArray()) + seperator;
            //if (!IsFolderExists(accessKeyID, secretAccessKey, bucketName, key, create))
            //        return false;

            //return true;
        }

        //public bool IsFolderExists(string accessKeyID, string secretAccessKey, string bucketName, string key, bool create)
        //{
        //    AmazonS3 client = GetS3Client(accessKeyID, secretAccessKey);
        //    ListObjectsRequest listRequest = new ListObjectsRequest();
        //    listRequest.WithBucketName(bucketName)
        //    .WithPrefix(key);

        //    // get all objects inside the "folder"
        //    ListObjectsResponse objects = client.ListObjects(listRequest);
        //    foreach (S3Object s3o in objects.S3Objects)
        //    {
        //        // get the acl of the object
        //        GetACLRequest aclRequest = new GetACLRequest();
        //        aclRequest.WithBucketName("thebucket")
        //        .WithKey(s3o.Key);
        //        GetACLResponse getAclResponse = client.GetACL(aclRequest);

        //        //// copy the object without acl
        //        //string newKey = s3o.Key.Replace(oldOWnerId.ToString(), newOwnerId.ToString());
        //        //CopyObjectRequest copyRequest = new CopyObjectRequest();
        //        //copyRequest.SourceBucket = "thebucket";
        //        //copyRequest.DestinationBucket = "thebucket";
        //        //copyRequest.WithSourceKey(s3o.Key)
        //        //.WithDestinationKey(newKey);
        //        //S3Response copyResponse = client.CopyObject(copyRequest);
        //        //// set the acl of the newly made object
        //        //SetACLRequest setAclRequest = new SetACLRequest();
        //        //setAclRequest.WithBucketName("ytimusic")
        //        //.WithKey(newKey)
        //        //.WithACL(getAclResponse.AccessControlList);
        //        //SetACLResponse setAclRespone = client.SetACL(setAclRequest);
        //        //DeleteObjectRequest deleteRequest = new DeleteObjectRequest();
        //        //deleteRequest.WithBucketName("thebucket")
        //        //.WithKey(s3o.Key);
        //        //DeleteObjectResponse deleteResponse = client.DeleteObject(deleteRequest);

        //    }

        //    return false;
        //}

        /// <summary>
        /// Check Azure connection
        /// </summary>
        /// <param name="azureAccountName"></param>
        /// <param name="azureAccountKey"></param>
        /// <returns></returns>
        public bool IsBucketNameExists(string accessKeyID, string secretAccessKey, string bucketName, bool create)
        {
            try
            {
                AmazonS3 client = GetS3Client(accessKeyID, secretAccessKey);
                ListBucketsResponse response = client.ListBuckets();
                bool found = false;
                foreach (S3Bucket bucket in response.Buckets)
                {
                    if (bucket.BucketName == bucketName)
                    {
                        found = true;
                        break;
                    }
                }
                if (found == false)
                {
                    if (create)
                        client.PutBucket(new PutBucketRequest().WithBucketName(bucketName));
                    else
                        throw new Exception("The bucket " + bucketName + " does not exists.");
                }
                return true;
            }
            catch (Exception ex)
            {
                throw new DuradosException("AWS Connection failed, Error: " + ex.Message);
                //return false;
            }
        }

        public Amazon.S3.AmazonS3 GetS3Client(string accessKeyID, string secretAccessKey)
        {
            AmazonS3 s3Client = AWSClientFactory.CreateAmazonS3Client(
                    accessKeyID,
                    secretAccessKey
                    );
            return s3Client;
        }

        public int MaxFileSize //in MB
        {
            get
            {
                return Convert.ToInt32(System.Configuration.ConfigurationManager.AppSettings["FtpMaxFileSize"]);
            }
        }
        public int DefaultFileSize //in MB
        {
            get
            {
                return Convert.ToInt32(System.Configuration.ConfigurationManager.AppSettings["FtpDefaultFileSize"]);
            }
        }


        [Durados.Config.Attributes.ColumnProperty()]
        public string FtpHost { get; set; }

        private int ftpPort;
        [Durados.Config.Attributes.ColumnProperty()]
        public int FtpPort
        {
            get
            {
                if (ftpPort == 0) return 21;
                return ftpPort;
            }
            set
            {
                ftpPort = value;
            }
        }


        public FtpUpload()
        {
            FtpPort = 21;
            UsePassive = true;
            StorageType = Mvc.StorageType.Ftp;
        }

        #region IUpload Members

        public virtual string GetBaseUploadPath(string fileName)
        {
            //string fileName = strFileName;
            return "ftp://" + FtpHost + ":" + FtpPort.ToString() + "/" + this.GetFtpBasePath(fileName);
        }
        public virtual string GetUploadPath(string fileName)
        {
            string ftpFilePath = fileName;
            if (string.IsNullOrEmpty(TemplatePath))
                ftpFilePath = GetFtpBasePath(ftpFilePath);
            else
                ftpFilePath = GetFtpFileTemplatePath(ftpFilePath);

            return ftpFilePath;
        }
        public virtual void CreateNewDirectory(string newFilePath)
        {
            string filePath = newFilePath.Replace("\\", "/").Replace("//", "/").TrimStart('/').TrimEnd('/');
            char pathSpliter = '/';
            string pathBuild = string.Empty;
            //FileInfo fileInfo = new FileInfo(newFilePath);
            //string newFilePath = fileInfo.DirectoryName;

            if (!string.IsNullOrEmpty(DirectoryBasePath) && filePath.TrimStart('/').StartsWith(DirectoryBasePath.TrimStart('/')))
            {
                filePath = filePath.TrimStart('/').TrimStart(DirectoryBasePath.TrimStart('/').ToCharArray());
                pathBuild = DirectoryBasePath.TrimEnd('/');
            }

            foreach (string folder in filePath.Split(pathSpliter))
            {
                pathBuild = string.Format("{0}/{1}/", pathBuild, folder);
                string path = GetFtpFileTemplatePath(pathBuild);
                if (IsDirectoryExists(path)) continue;
                WebRequest request = WebRequest.Create(path);//"ftp://host.com/directory"
                request.Method = WebRequestMethods.Ftp.MakeDirectory;
                request.Credentials = GetFtpNetworkCredential();
                using (var resp = (FtpWebResponse)request.GetResponse())
                {
                    //pathBuild = string.Format("{0}/{1}",pathBuild,folder);
                }
            }
        }

        public virtual void CreateNewDirectory2(string newPhisicalPath)
        {
            //CreateNewDirectory(newPhisicalPath);
        }

        public virtual void DeleteOldFile(string newPhisicalPath)
        {
            // The serverUri parameter should use the ftp:// scheme.
            // It contains the name of the server file that is to be deleted.
            // Example: ftp://contoso.com/someFile.txt.
            // 
            string newPath = newPhisicalPath.Replace("\\", "/").Replace("//", "/");
            string newPath2 = GetFtpFileTemplatePath(newPath);
            if (!IsFileExists(newPath2)) return;

            Uri serverUri = new Uri(newPhisicalPath);
            if (serverUri.Scheme != Uri.UriSchemeFtp)
            {
                throw new DuradosException("File was not deleted from remote ftp server");
                //return false;
            }

            // Get the object used to communicate with the server.
            FtpWebRequest request = (FtpWebRequest)WebRequest.Create(serverUri);
            request.Credentials = GetFtpNetworkCredential();
            request.Method = WebRequestMethods.Ftp.DeleteFile;

            FtpWebResponse response = (FtpWebResponse)request.GetResponse();
            //Console.WriteLine("Delete status: {0}", response.StatusDescription);
            response.Close();
            //return true;
        }


        public virtual void MoveUploadedFile(string oldPath, string newPhisicalPath)
        {
            try
            {
                //System.Net.WebRequestMethods.Ftp
                FtpWebRequest ftpReq = GetFtpRequest(oldPath);
                ftpReq.Method = WebRequestMethods.Ftp.Rename;
                string destFile = newPhisicalPath.Replace("\\", "/").Replace("//", "/"); ; // this will move the file in same folder
                ftpReq.RenameTo = destFile;
                FtpWebResponse ftpResp = (FtpWebResponse)ftpReq.GetResponse();
            }
            catch (WebException ex)
            {
                FtpWebResponse response = (FtpWebResponse)ex.Response;
                if (response.StatusCode == FtpStatusCode.ActionNotTakenFileUnavailable)
                {
                }
            }
        }

        private FtpWebRequest GetFtpRequest(string oldPath)
        {
            FtpWebRequest ftpReq = (FtpWebRequest)WebRequest.Create(oldPath);
            ftpReq.Credentials = GetFtpNetworkCredential();

            ftpReq.UsePassive = true;
            ftpReq.UseBinary = true;
            ftpReq.KeepAlive = false;
            return ftpReq;
        }

        public virtual bool IsFileExists(string ftpFilePath)
        {
            bool isFileExists = false;
            var request = (FtpWebRequest)WebRequest.Create(ftpFilePath);//ftp://ftp.domain.com/doesntexist.txt
            request.Credentials = GetFtpNetworkCredential();
            request.Method = WebRequestMethods.Ftp.GetFileSize;

            try
            {
                FtpWebResponse response = (FtpWebResponse)request.GetResponse();
                return true;
            }
            catch (WebException ex)
            {
                FtpWebResponse response = (FtpWebResponse)ex.Response;
                if (response.StatusCode == FtpStatusCode.ActionNotTakenFileUnavailable)
                {
                    //Does not exist
                }
                else
                {

                };
            }

            return isFileExists; ;
        }

        public virtual bool IsDirectoryExists(string directory)
        {
            try
            {
                //create the directory
                FtpWebRequest requestDir = (FtpWebRequest)FtpWebRequest.Create(new Uri(directory));
                requestDir.Method = WebRequestMethods.Ftp.MakeDirectory;
                requestDir.Credentials = GetFtpNetworkCredential();
                requestDir.UsePassive = true;
                requestDir.UseBinary = true;
                requestDir.KeepAlive = false;
                FtpWebResponse response = (FtpWebResponse)requestDir.GetResponse();
                Stream ftpStream = response.GetResponseStream();

                ftpStream.Close();
                response.Close();

                return true;
            }
            catch (WebException ex)
            {
                FtpWebResponse response = (FtpWebResponse)ex.Response;
                if (response.StatusCode == FtpStatusCode.ActionNotTakenFileUnavailable)
                {
                    response.Close();
                    return true;
                }
                else
                {
                    response.Close();
                    return false;
                }
            }
        }
        #endregion

        private System.Net.NetworkCredential GetFtpNetworkCredential()
        {
            return new System.Net.NetworkCredential(FtpUserName, FtpPassword.Decrypt(Maps.Instance.GetMap().Database));
        }
    }


    [Durados.Config.Attributes.EnumDisplay(EnumDisplayNames = new string[3] { "Amazon S3", "Azure", "FTP" }, EnumNames = new string[3] { "Aws", "Azure", "Ftp" })]
    public enum StorageType
    {
        Aws,
        Azure,
        Ftp
    }


}
