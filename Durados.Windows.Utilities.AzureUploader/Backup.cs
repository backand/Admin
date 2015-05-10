using Microsoft.WindowsAzure.StorageClient;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Durados.Windows.Utilities.AzureUploader
{
    public delegate void StartBackupEventHandler(object sender, BackupStartedEventArgs e);
    public delegate void EndBackupEventHandler(object sender, BackupEndedEventArgs e);
    public delegate void StartBackupContainerEventHandler(object sender, BackupContainerStartedEventArgs e);
    public delegate void EndBackupContainerEventHandler(object sender, BackupContainerEndedEventArgs e);

    public class Backup
    {
        public event StartBackupEventHandler BackupStarted;
        public event EndBackupEventHandler BackupEnded;
        public event StartBackupContainerEventHandler BackupContainerStarted;
        public event EndBackupContainerEventHandler BackupContainerEnded;

        public StorageCred StorageCred { get; set; }
        public StorageCred BackupStorageCred { get; set; }
        public int Copies { get; set; }
        public string ContainersPrefix { get; set; }

        public IBackupEngine BackupEngine { get; private set; }
        public Backup()
        {
            BackupEngine = new BackupEngine();
        }

        public void LoadSettings()
        {
            StorageCred = new StorageCred() { Name = Convert.ToString(Properties.Settings.Default.BackupAzureAccountName ?? ""), Key = Convert.ToString(Properties.Settings.Default.BackupAzureAccountKey ?? "") };
            BackupStorageCred = new StorageCred() { Name = Convert.ToString(Properties.Settings.Default.BackupBackupAzureAccountName ?? ""), Key = Convert.ToString(Properties.Settings.Default.BackupBackupAzureAccountKey ?? "") };
            Copies = Convert.ToInt32(Properties.Settings.Default["Copies"] ?? 14);
            ContainersPrefix = "duradosappsys";
        }

        public void SaveSettings()
        {
            Properties.Settings.Default["BackupAzureAccountName"] = StorageCred.Name;
            Properties.Settings.Default["BackupAzureAccountKey"] = StorageCred.Key;
            Properties.Settings.Default["BackupBackupAzureAccountName"] = BackupStorageCred.Name;
            Properties.Settings.Default["BackupBackupAzureAccountKey"] = BackupStorageCred.Key;
            Properties.Settings.Default["Copies"] = Copies;
            Properties.Settings.Default.Save();
         }

        public void All()
        {
            BackupEngine.Backup(BackupStarted, BackupEnded, BackupContainerStarted, BackupContainerEnded, StorageCred, BackupStorageCred, Copies, ContainersPrefix);
        }
    }

    public class BackupEngine : IBackupEngine
    {
        public void Backup(StartBackupEventHandler startBackupCallback, EndBackupEventHandler endBackupCallback, StartBackupContainerEventHandler startBackupContainerCallback, EndBackupContainerEventHandler endBackupContainerCallback, StorageCred storageCred = null, StorageCred backupStorageCred = null, int copies = 14, string containersPrefix = "duradosappsys")
        {
            Dictionary<string, Exception> exceptions = new Dictionary<string, Exception>();

            

            try
            {
                BackupStartedEventArgs e = new BackupStartedEventArgs() { Occured = DateTime.Now, StorageName = storageCred.Name };

                Storage storage = GetStorage(storageCred);
                Storage backupStorage = GetStorage(backupStorageCred);
                var containers = storage.ListContainers(containersPrefix);

                if (startBackupCallback != null)
                {
                    e.ContainersCount = containers.Count();
                    startBackupCallback(this, e);
                }

                foreach (var container in containers)
                {
                    if (startBackupContainerCallback != null)
                    {
                        startBackupContainerCallback(this, new BackupContainerStartedEventArgs() { Occured = DateTime.Now, Container = container });
                    }
                    try
                    {
                        DateTime? lastModified = null;
                        bool modified = BackupContainer(container, backupStorage, storage, copies, out lastModified);
                        if (endBackupContainerCallback != null)
                        {
                            endBackupContainerCallback(this, new BackupContainerEndedEventArgs() { Occured = DateTime.Now, Container = container, LastModified=lastModified, Modified = modified });
                        }
                    }
                    catch (Exception exception)
                    {
                        exceptions.Add(container.Name, exception);
                        if (endBackupContainerCallback != null)
                        {
                            endBackupContainerCallback(this, new BackupContainerEndedEventArgs() { Occured = DateTime.Now, Container = container, Exception = exception });
                        }
                    }

                }
            }
            catch (Exception exception)
            {
                exceptions.Add("general", exception);
            }
            finally
            {
                if (endBackupCallback != null)
                {
                    endBackupCallback(this, new BackupEndedEventArgs() { Occured = DateTime.Now, StorageName = storageCred.Name, Exceptions = exceptions });
                }
            }
        }

        public bool BackupContainer(CloudBlobContainer container, Storage backupStorage,Storage storage, int copies, out DateTime? lastModified)
        {
            var blob = GetBlobToBackup(container);
            lastModified = null;
            if (!storage.Exists(blob))
                return false;

            var backupContainer = backupStorage.GetContainer(container.Name);
            bool isModufied = IsModufied(backupContainer, blob, container, backupStorage);
            lastModified = GetLastModified(blob);
            if (isModufied)
            {
                BackupBlob(backupContainer, blob);
                DeleteLastBlobs(backupContainer, copies);
            }

            return isModufied;
        }

        private void DeleteLastBlobs(CloudBlobContainer backupContainer, int copies)
        {
            IEnumerable<IListBlobItem> blobItems = backupContainer.ListBlobs();
            int totalBlobs = blobItems.Count();
            if (totalBlobs <= copies)
                return;
            SortedDictionary<DateTime, CloudBlob> blobs = new SortedDictionary<DateTime, CloudBlob>();

            foreach (IListBlobItem blobItem in blobItems)
            {
                var blob = backupContainer.GetBlobReference(blobItem.Uri.ToString());
                blobs.Add(GetLastModified(blob), blob);
            }
            CloudBlob[] blobsArray = blobs.Values.ToArray();
            for (int i = totalBlobs - 1; i >= copies; i--)
            {
                blobsArray[i].Delete();
            }
        }

        private string GetBackupBlobName(CloudBlob blob)
        {
            return blob.Name + GetLastModified(blob).ToString("yyyyMMddHHmmss");
        }

        private void BackupBlob(CloudBlobContainer backupContainer, CloudBlob blob)
        {
            CloudBlob target = backupContainer.GetBlobReference(GetBackupBlobName(blob));

            using (Stream targetStream = target.OpenWrite())
            {
                blob.DownloadToStream(targetStream);
            }

            //var signature = blob.GetSharedAccessSignature(new Microsoft.WindowsAzure.StorageClient.SharedAccessPolicy()
            //{
            //    Permissions = SharedAccessPermissions.Read,
            //    SharedAccessExpiryTime = DateTime.UtcNow + TimeSpan.FromMinutes(10)
            //});
            //target.CopyFromBlob(new Uri(blob.Uri.AbsoluteUri + signature));
        }

        private bool IsModufied(CloudBlobContainer backupContainer, CloudBlob blob, CloudBlobContainer container, Storage backupStorage)
        {
            var backupBlob = backupContainer.GetBlobReference(GetBackupBlobName(blob));
            return !backupStorage.Exists(backupBlob);
            
        }

        private DateTime GetLastModified(CloudBlob blob)
        {
            return blob.Properties.LastModifiedUtc;
        }

        private CloudBlob GetBlobToBackup(CloudBlobContainer container)
        {
            return container.GetBlobReference(container.Name);
        }

        protected virtual Storage GetStorage(StorageCred storageCred)
        {
            Storage storage = new Storage();
            storage.Connect(storageCred.Name, storageCred.Key);
            return storage;
        }

        public void Restore(string appId, StorageCred storageCred = null, StorageCred backupStorageCred = null)
        {

        }
    }

    public interface IBackupEngine
    {
        void Backup(StartBackupEventHandler startBackupCallback, EndBackupEventHandler endBackupCallback, StartBackupContainerEventHandler startBackupContainerCallback, EndBackupContainerEventHandler endBackupContainerCallback, StorageCred storageCred = null, StorageCred backupStorageCred = null, int copies = 14, string containersPrefix = "duradosappsys");

        void Restore(string appId, StorageCred storageCred = null, StorageCred backupStorageCred = null);

    }

    public class StorageCred
    {
        public string Name { get; set; }
        public string Key { get; set; }

    }

    public class BackupStartedEventArgs : EventArgs
    {
        public DateTime Occured { get; set; }
        public string StorageName { get; set; }
        public int ContainersCount { get; set; }

        public override string ToString()
        {
            return string.Format("Backup started for {0} at {1}, {2} apps", StorageName, Occured, ContainersCount);
        }

    }

    public class BackupEndedEventArgs : BackupStartedEventArgs
    {
        public Dictionary<string, Exception> Exceptions { get; set; }

        public bool Success
        {
            get
            {
                return Exceptions == null || Exceptions.Count == 0;
            }
        }
        public override string ToString()
        {
            return string.Format("Backup ended with {0} for {1} at {2}", Success ? "success" : "failure", StorageName, Occured);
        }


        internal string GetExceptionsReport()
        {
            StringBuilder text = new StringBuilder();
            foreach (string name in Exceptions.Keys)
            {
                text.AppendLine(name);
                text.AppendLine(Exceptions[name].Message);
                text.AppendLine(Exceptions[name].StackTrace);
                text.AppendLine();
                text.AppendLine();
            }

            return text.ToString();
        }
    }

    public class BackupContainerStartedEventArgs : EventArgs
    {
        public DateTime Occured { get; set; }
        public CloudBlobContainer Container { get; set; }
        public override string ToString()
        {
            return string.Format("Container backup started for {0} at {1}", Container.Name, Occured);
        }
    }

    public class BackupContainerEndedEventArgs : BackupContainerStartedEventArgs
    {
        public Exception Exception { get; set; }

        public DateTime? LastModified { get; set; }
        public bool? Modified { get; set; }
        public bool Success
        {
            get
            {
                return Exception == null;
            }
        }
        public override string ToString()
        {
            return string.Format("Container backup ended with {0} for {1} at {2} {3}", Success ? "success" : "failure", Container.Name, Occured, Success? "" : Exception.Message);
        }
    }
}
