using Amazon.S3.IO;
using Amazon.S3.Model;
using Amazon.S3.Transfer;
using Durados.SmartRun;
using Durados.Web.Mvc.Azure;
using Durados.Web.Mvc.Farm;
using Microsoft.WindowsAzure.StorageClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Durados.Web.Mvc.UI.Helpers.Config.Cloud.AWS
{
    public class AwsStorage : IStorage
    {
        private Map Map;

        public AwsStorage(Map map)
        {
            this.Map = map;
        }

        public void Read(DataSet ds, string filename, string appName, bool isMainMap)
        {
            CheckIfConfigIsLockedAndWait(appName, isMainMap);

            TransferUtility fileTransferUtility = GetTransferUtility();

            
            string containerName = Maps.GetStorageBlobName(filename);
            string key = containerName + "/" + containerName;
            
            try
            {
                //BlobStream stream = blob.OpenRead();
                //string tempFileName = fileInfo.DirectoryName + "\\temp" + filenameOnly + "." + fileInfo.Extension;

                using (MemoryStream stream = fileTransferUtility.OpenStream(Maps.ConfigAwsStorageBucketName, key) as MemoryStream)
                {
                    stream.Seek(0, SeekOrigin.Begin);

                    ds.ReadXml(stream);
                }
                //System.IO.File.Delete(tempFileName);

            }
            catch { }
        }

        DuradosStorage storage = new DuradosStorage();

        private TransferUtility GetTransferUtility()
        {
            return new
                    TransferUtility(Maps.ConfigAwsStorageAccessKeyID, Maps.ConfigAwsStorageSecretAccessKey);
        }

        private void CheckIfConfigIsLockedAndWait(string appName, bool isMainMap)
        {
            try
            {
                RunWithRetry.Run<AsyncCacheIsNotCompletedException>(
                        () =>
                        {
                            // for ELB check if caching was completed
                            if (!isMainMap && !string.IsNullOrEmpty(appName))
                            {
                                if (!FarmCachingSingeltone.Instance.IsAsyncCacheCompleted(appName))
                                {
                                    throw new AsyncCacheIsNotCompletedException();
                                }
                            }

                        }, 10, 500);
            }
            catch (AsyncCacheIsNotCompletedException exception)
            {
                // clear lock from redis
                FarmCachingSingeltone.Instance.AsyncCacheCompleted(appName);

                Maps.Instance.DuradosMap.Logger.Log("ReadConfigFromCloud", "ReadConfigFromCloud", "ReadConfigFromCloud", exception, 1, "");
            }
        }

        public void Write(DataSet ds, string filename, bool async, Map map, string version)
        {
            
            TransferUtility fileTransferUtility = GetTransferUtility();

            string containerName = Maps.GetStorageBlobName(filename);
            string key = containerName + "/" + containerName + version;

            if (string.IsNullOrEmpty(version))
                Maps.Instance.StorageCache.Add(containerName, ds);

            

            if (!Maps.Instance.StorageCache.ContainsKey(containerName) || !async)
            {
                using (MemoryStream stream = new MemoryStream())
                {
                    ds.WriteXml(stream, XmlWriteMode.WriteSchema);
                    stream.Seek(0, SeekOrigin.Begin);
                    fileTransferUtility.Upload(stream, Maps.ConfigAwsStorageBucketName, key);
                    Map.RefreshApis(map);
                    //Maps.Instance.Backup.BackupAsync(container, containerName);
                }
            }
            else
            {
                MemoryStream stream = new MemoryStream();
                ds.WriteXml(stream, XmlWriteMode.WriteSchema);
                stream.Seek(0, SeekOrigin.Begin);

                DateTime started = DateTime.Now;

                fileTransferUtility.BeginUpload(stream, Maps.ConfigAwsStorageBucketName, key, TransferCompletedCallback, new TransferAsyncState(fileTransferUtility, stream, started, containerName, key, map));
                
            }
        }

        private bool IsBigBlob(string blobName)
        {
            return !(blobName.EndsWith("xml"));
        }

        private void TransferCompletedCallback(IAsyncResult result)
        {
            TransferAsyncState state = (TransferAsyncState)result.AsyncState;
            if (state == null || state.Map == null)
                return;

            if (IsBigBlob(state.ContainerName))
            {
                FarmCachingSingeltone.Instance.AsyncCacheCompleted(state.Map.AppName);
            }

            
            Map.RefreshApis(state.Map);

            try
            {
                state.FileTransferUtility.EndUpload(result);
                if (!Maps.IsApi2())
                {
                    //Maps.Instance.Backup.BackupAsync(state.Container, state.BlobName);
                }
            }
            catch (Exception exception)
            {
                Map map = Maps.Instance.GetMap();
                map.Logger.Log("Map", "TransferCompletedCallback", map.AppName ?? string.Empty, exception, 1, string.Empty);
            }
        }

        public bool Exists(string filename)
        {
            if (Maps.Cloud)
            {
                System.IO.FileInfo fileInfo = new FileInfo(filename);
                string filenameOnly = fileInfo.Name.Remove(fileInfo.Name.Length - fileInfo.Extension.Length, fileInfo.Extension.Length);
                string containerName = filenameOnly.Replace("_", "").ToLower();

                using (var client = Amazon.AWSClientFactory.CreateAmazonS3Client(Maps.ConfigAwsStorageAccessKeyID, Maps.ConfigAwsStorageSecretAccessKey))
                {
                    var request = new ListObjectsRequest()
         .WithBucketName(Maps.ConfigAwsStorageBucketName)
         .WithPrefix(containerName);

                    var response = client.ListObjects(request);

                    return response.S3Objects.Count > 0;

                    
                }

                
            }
            else
            {
                return System.IO.File.Exists(filename);
            }
        }
    }
}
