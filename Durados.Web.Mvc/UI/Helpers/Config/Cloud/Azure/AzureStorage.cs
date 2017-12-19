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

namespace Durados.Web.Mvc.UI.Helpers.Config.Cloud.Azure
{
    public class AzureStorage : IStorage
    {
        private Map Map;

        public AzureStorage(Map map)
        {
            this.Map = map;
        }

        public void Read(DataSet ds, string filename, string appName, bool isMainMap)
        {
            CheckIfConfigIsLockedAndWait(appName, isMainMap);

            string containerName = Maps.GetStorageBlobName(filename);
            CloudBlobContainer container = GetContainer(containerName);

            CloudBlob blob = container.GetBlobReference(containerName);
            try
            {
                //BlobStream stream = blob.OpenRead();
                //string tempFileName = fileInfo.DirectoryName + "\\temp" + filenameOnly + "." + fileInfo.Extension;

                using (MemoryStream stream = new MemoryStream())
                {
                    blob.DownloadToStream(stream);

                    stream.Seek(0, SeekOrigin.Begin);

                    ds.ReadXml(stream);
                }
                //System.IO.File.Delete(tempFileName);

            }
            catch { }
        }

        DuradosStorage storage = new DuradosStorage();

        private CloudBlobContainer GetContainer(string filename)
        {
            // Get a handle on account, create a blob service client and get container proxy
            //var account = CloudStorageAccount.Parse(RoleEnvironment.GetConfigurationSettingValue("ConfigAzureStorage"));
            //var client = account.CreateCloudBlobClient();
            //return client.GetContainerReference(RoleEnvironment.GetConfigurationSettingValue("configContainer"));
            return storage.GetContainer(filename);
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
            string containerName = Maps.GetStorageBlobName(filename);
            if (string.IsNullOrEmpty(version))
                Maps.Instance.StorageCache.Add(containerName, ds);

            CloudBlobContainer container = GetContainer(containerName);

            CloudBlob blob = container.GetBlobReference(containerName + version);
            blob.Properties.ContentType = "application/xml";

            if (!Maps.Instance.StorageCache.ContainsKey(containerName) || !async)
            {
                using (MemoryStream stream = new MemoryStream())
                {
                    ds.WriteXml(stream, XmlWriteMode.WriteSchema);
                    stream.Seek(0, SeekOrigin.Begin);
                    blob.UploadFromStream(stream);
                    Map.RefreshApis(map);
                    Maps.Instance.Backup.BackupAsync(container, containerName);
                }
            }
            else
            {
                MemoryStream stream = new MemoryStream();
                ds.WriteXml(stream, XmlWriteMode.WriteSchema);
                stream.Seek(0, SeekOrigin.Begin);

                DateTime started = DateTime.Now;

                blob.BeginUploadFromStream(stream, BlobTransferCompletedCallback, new BlobTransferAsyncState(blob, stream, started, container, containerName, map));
            }
        }

        private bool IsBigBlob(string blobName)
        {
            return !(blobName.EndsWith("xml"));
        }

        private void BlobTransferCompletedCallback(IAsyncResult result)
        {
            BlobTransferAsyncState state = (BlobTransferAsyncState)result.AsyncState;
            if (state == null || state.Map == null)
                return;

            if (IsBigBlob(state.BlobName))
            {
                FarmCachingSingeltone.Instance.AsyncCacheCompleted(state.Map.AppName);
            }

            //try
            //{
            //    Maps.Instance.DuradosMap.Logger.Log("Map", "WriteConfigToCloud", state.Map.AppName ?? string.Empty, DateTime.Now.Subtract(state.Started).TotalMilliseconds.ToString(), string.Empty, -8, state.BlobName + " ended", DateTime.Now);
            //}
            //catch { }

            Map.RefreshApis(state.Map);

            try
            {
                state.Blob.EndUploadFromStream(result);
                if (!Maps.IsApi2())
                {
                    Maps.Instance.Backup.BackupAsync(state.Container, state.BlobName);
                }
            }
            catch (Exception exception)
            {
                Map map = Maps.Instance.GetMap();
                map.Logger.Log("Map", "BlobTransferCompletedCallback", map.AppName ?? string.Empty, exception, 1, string.Empty);
            }
        }

        public bool Exists(string filename)
        {
            if (Maps.Cloud)
            {
                System.IO.FileInfo fileInfo = new FileInfo(filename);
                string filenameOnly = fileInfo.Name.Remove(fileInfo.Name.Length - fileInfo.Extension.Length, fileInfo.Extension.Length);
                string containerName = filenameOnly.Replace("_", "").ToLower();

                CloudBlobContainer container = GetContainer(containerName);

                CloudBlob blob = container.GetBlobReference(containerName);

                return blob.Exists();
            }
            else
            {
                return System.IO.File.Exists(filename);
            }
        }
    }
}
