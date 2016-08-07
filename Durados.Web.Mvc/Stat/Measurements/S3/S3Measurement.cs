using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Durados.DataAccess;
using System.Data;
using System.Data.SqlClient;
using Amazon.S3;
using Amazon;
using Amazon.S3.Model;


namespace Durados.Web.Mvc.Stat.Measurements.S3
{
    public abstract class S3Measurement : Measurement
    {
        public S3Measurement(App app, MeasurementType measurementType)
            : base(app, measurementType)
        {

        }

        public override object Get(DateTime date)
        {
            string folderPath = App.AppName;
            long folderSize = GetFolderSize(folderPath, GetBucketName());
            return folderSize;
        }


        protected abstract string GetBucketName();

        protected virtual long GetFolderSize(string folderPath, string bucketName)
        {
            AmazonS3 s3Client = GetS3Client(Maps.AwsCredentials.AccessKeyID, Maps.AwsCredentials.SecretAccessKey);
            return GetFolderSize(s3Client, folderPath, bucketName);
        }

        protected virtual long GetFolderSize(AmazonS3 client, string folderPath, string bucketName)
        {
            ListObjectsRequest request = new ListObjectsRequest();
            request.WithBucketName(bucketName);
            request.WithPrefix(folderPath);
            long total = 0;
            do
            {
                ListObjectsResponse response = client.ListObjects(request);

                if (response != null && response.S3Objects != null)
                    total += response.S3Objects.Sum(s => s.Size);

                if (response.IsTruncated)
                {
                    request.Marker = response.NextMarker;
                }
                else
                {
                    request = null;
                }
            } while (request != null);

            return total;
        }

        protected Amazon.S3.AmazonS3 GetS3Client(string accessKeyID, string secretAccessKey)
        {
            AmazonS3 s3Client = AWSClientFactory.CreateAmazonS3Client(
                    accessKeyID,
                    secretAccessKey
                    );
            return s3Client;
        }
    }
}
