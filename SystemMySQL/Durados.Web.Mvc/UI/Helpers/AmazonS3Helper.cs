using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Amazon.S3.Transfer;
using Amazon.S3;
using Amazon.S3.Model;


namespace Durados.Web.Mvc.UI.Helpers
{
    public static class AmazonS3Helper
    {
        public static string SaveUploadedFileToAws(string accessKeyID, string secretAccessKey, string existingBucketName, string strFileName)
        {
            return SaveUploadedFileToAws(accessKeyID, secretAccessKey, existingBucketName, strFileName, System.Web.HttpContext.Current.Request.Files[0].ContentType, System.Web.HttpContext.Current.Request.Files[0].InputStream);
        }

        public static string SaveUploadedFileToAws(string accessKeyID, string secretAccessKey, string existingBucketName, string strFileName, string contentType, System.IO.Stream stream)
        {
            //try
            //{
                TransferUtility fileTransferUtility = new
                    TransferUtility(accessKeyID, secretAccessKey);

                //// 1. Upload a file, file name is used as the object key name.
                //fileTransferUtility.Upload(filePath, existingBucketName);
                //Console.WriteLine("Upload 1 completed");

                //// 2. Specify object key name explicitly.
                //fileTransferUtility.Upload(filePath,
                //                          existingBucketName, keyName);
                //Console.WriteLine("Upload 2 completed");

                // 3. Upload data from a type of System.IO.Stream.
                //string keyName =System.IO.Path.GetFileNameWithoutExtension(strFileName) + "_" + Guid.NewGuid() + System.IO.Path.GetExtension(strFileName);
                string keyName = strFileName;
                fileTransferUtility.Upload(stream, existingBucketName, keyName);

                Console.WriteLine("Upload 3 completed");

                //// 4.// Specify advanced settings/options.
                //TransferUtilityUploadRequest fileTransferUtilityRequest =
                //    new TransferUtilityUploadRequest()
                //    .WithBucketName(existingBucketName)
                //    .WithFilePath(filePath)
                //    .WithStorageClass(S3StorageClass.ReducedRedundancy)
                //    .WithMetadata("param1", "Value1")
                //    .WithMetadata("param2", "Value2")
                //    .WithPartSize(6291456) // This is 6 MB.
                //    .WithKey(keyName)
                //    .WithCannedACL(S3CannedACL.PublicRead);
                //fileTransferUtility.Upload(fileTransferUtilityRequest);
                //Console.WriteLine("Upload 4 completed");

                SetACLRequest request = new Amazon.S3.Model.SetACLRequest();
                request.BucketName = existingBucketName;
                request.Key = keyName;
                request.CannedACL = S3CannedACL.PublicRead;
                fileTransferUtility.S3Client.SetACL(request);

                //string preSignedURL = fileTransferUtility.S3Client.GetPreSignedURL(new GetPreSignedUrlRequest()
                //{
                //    BucketName = existingBucketName,
                //    Key = keyName,
                //    Expires = System.DateTime.Now.AddDays(1000)
                //});

                //return preSignedURL;

                string seperator = "/";
                existingBucketName = existingBucketName.Trim(seperator.ToCharArray()) + seperator;
                keyName = keyName.Trim(seperator.ToCharArray());
                return string.Format("http://s3.amazonaws.com/{0}{1}", existingBucketName, keyName);

            //}
            //catch (AmazonS3Exception s3Exception)
            //{
            //    Console.WriteLine(s3Exception.Message,
            //                      s3Exception.InnerException);
            //}
            //return string.Empty;
        }
    }
}
