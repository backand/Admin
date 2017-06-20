using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Durados.Web.Mvc.Aws
{
    public class AppNameHandler
    {
        private string S3StorageBucketName = System.Configuration.ConfigurationManager.AppSettings["S3FilesBucket"] ?? "files.backand.net";
        private string S3HostingBucketName = System.Configuration.ConfigurationManager.AppSettings["S3Bucket"] ?? "hosting.backand.net";
        private string S3NodeJsBucketName = System.Configuration.ConfigurationManager.AppSettings["NodeJSBucket"] ?? "nodejs.backand.net";

        private S3 s3;

        public AppNameHandler()
        {
            s3 = new S3(string.Empty);
        }

        public void Rename(string oldName, string newName)
        {
            HandleS3(oldName, newName);
            HandleLambda(oldName, newName);
        }

        private void HandleLambda(string oldName, string newName)
        {
            throw new NotImplementedException();
        }

        private void HandleS3(string oldName, string newName)
        {
            HandleStorage(oldName, newName);
            HandleHosting(oldName, newName);
            HandleNodeJs(oldName, newName);
        }

        private void HandleStorage(string oldName, string newName)
        {
            ChangeFolderName(S3StorageBucketName, oldName, newName);
        }

        private void ChangeFolderName(string S3StorageBucketName, string oldName, string newName)
        {
            s3.ChangeFolderName(S3StorageBucketName, oldName, newName);
        }

        private void HandleHosting(string oldName, string newName)
        {
            ChangeFolderName(S3HostingBucketName, oldName, newName);
        }

        private void HandleNodeJs(string oldName, string newName)
        {
            ChangeFolderName(S3NodeJsBucketName, oldName, newName);
        }
    }
}
