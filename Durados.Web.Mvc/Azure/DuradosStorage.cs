using System;
using System.Data;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.WindowsAzure;
using Microsoft.WindowsAzure.ServiceRuntime;
using Microsoft.WindowsAzure.StorageClient;
using Microsoft.WindowsAzure.StorageClient.Protocol;

namespace Durados.Web.Mvc.Azure
{
    public class DuradosStorage
    {
        CloudStorageAccount account = null;
        public DuradosStorage()
        {
            if (Maps.Cloud)
            {
                account = StorageHelper.GetAccount();
            }
        }

        public CloudBlobContainer GetContainer(string filename)
        {
            return StorageHelper.GetContainer(account, filename);
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

        //public void Load(DataSet data, string filename)
        //{
        //    var container = GetContainer();
        //    System.IO.FileInfo fileInfo = new FileInfo(filename);
        //    string filenameOnly = fileInfo.Name;

        //    using (var mem = new MemoryStream())
        //    {
        //        if (!container.DoesBlobExist("Data.xml"))
        //            return;

        //        var fileBlob = new BlobContents(mem);
        //        container.GetBlobReference(filenameOnly);
        //        var stream = fileBlob.AsStream;

        //        // Required - stream pointer must be at position 0
        //        stream.Seek(0, SeekOrigin.Begin);

        //        data.ReadXml(stream);
        //    }
        //}

        //public void Save(DataSet data)
        //{
        //    var container = GetContainer();
        //    if (container.DoesBlobExist("Data.xml"))
        //        container.DeleteBlob("Data.xml");

        //    var metadata = new NameValueCollection();
        //    metadata["FileName"] = "Data.xml";

        //    var properties = new BlobProperties("Data.xml")
        //    {
        //        Metadata = metadata,
        //        ContentType = "text/xml"
        //    };

        //    using (var mem = new MemoryStream())
        //    {
        //        data.WriteXml(mem);
        //        var fileBlob = new BlobContents(mem.ToArray());
        //        container.CreateBlob(properties, fileBlob, true);
        //    }
        //}      

    }
}
