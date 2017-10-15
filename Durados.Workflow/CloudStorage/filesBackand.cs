using System;
using System.Collections.Generic;

namespace Backand
{
    public class filesBackand : IFiles
    {
        protected string BaseUrl = System.Configuration.ConfigurationManager.AppSettings["nodeHost"] ?? "http://127.0.0.1:9000";
        protected string S3FilesBucket = System.Configuration.ConfigurationManager.AppSettings["S3FilesBucket"] ?? "files.backand.net";
        protected int MaxJSONSizeMB = System.Convert.ToInt32(System.Configuration.ConfigurationManager.AppSettings["FtpMaxFileSize"] ?? "5");

        public Durados.Cloud Cloud{ get; set; }
        public filesBackand(Durados.Cloud cloud)
        {
            Cloud = cloud;
        }

        public virtual string upload(string fileName, string fileData, string bucket)
        {
            return upload(fileName, fileData, bucket, null);
        }
        public virtual string upload(string fileName, string fileData)
        {
            return upload(fileName, fileData, null,null);
        }
        public string upload(string fileName, string fileData, string providerAccount, string bucket, string path)
        {
            return upload(fileName, fileData, bucket, path);
        }
        public virtual string upload(string fileName, string fileData, string bucket, string path)
        {
            fileData = CheckFileSize(fileData);

            Dictionary<string, object> data = GetFileDetails(fileName,  bucket,  path);
           
            data.Add("file", fileData);
            
            return UploadFile(data);
        }

        protected virtual Dictionary<string, object> GetFileDetails(string fileName, string bucket, string path)
        {
           
            string appName = System.Web.HttpContext.Current.Items[Durados.Database.AppName].ToString();
            Dictionary<string, object> data = new Dictionary<string, object>();
            data.Add("cloudProvider", Durados.CloudVendor.AWS.ToString());
            data.Add("storage", GetStorageDetails(fileName, S3FilesBucket, appName));
            return data;
            
        }

        private Dictionary<string, object> GetStorageDetails(string fileName,  string bucket, string appName)
        {
            Dictionary<string, object> data = new Dictionary<string, object>();
            data.Add("fileName", fileName);
            data.Add("bucket", bucket);
            data.Add("dir", appName);
            //if (!string.IsNullOrEmpty(fileType))
            //{
            //    data.Add("fileType", fileType);
            //}

            return data;
        }

        protected string UploadFile(Dictionary<string, object> data)
        {
            if (!data.ContainsKey(StorageKeys.Storage) || data[StorageKeys.Storage] == null )
                throw new Durados.DuradosException(Messages.MissingStorageObjectInJS);

            if(! (data[StorageKeys.Storage] as Dictionary<string,object>).ContainsKey(StorageKeys.FileName))
                throw new Durados.DuradosException(Messages.MissingFileName);

            string fileName = ((Dictionary<string, object>)data[StorageKeys.Storage])[StorageKeys.FileName].ToString();
            
            string url = BaseUrl + "/uploadFile";
            XMLHttpRequest request = new XMLHttpRequest();
            request.open("POST", url, false);

            request.setRequestHeader("content-type", "application/json");

            System.Web.Script.Serialization.JavaScriptSerializer jss = new System.Web.Script.Serialization.JavaScriptSerializer();
            jss.MaxJsonLength = int.MaxValue;
            request.send(jss.Serialize(data));


            if (request.status != 200)
            {
                if (Logger != null)
                    Logger.Log("files", "upload", fileName, "Server return status " + request.status, request.responseText, 1, "upload " + fileName, DateTime.Now, 0);

                throw new Durados.DuradosException("Server return status " + request.status + ", " + request.responseText);
            }

            if (Logger != null)
                Logger.Log("files", "upload", fileName, null, 3, "upload " + fileName, DateTime.Now, 0);


            Dictionary<string, object> response = null;
            try
            {
                response = jss.Deserialize<Dictionary<string, object>>(request.responseText);
            }
            catch (Exception exception)
            {
                throw new Durados.DuradosException("Could not parse upload response", exception);
            }

            if (!response.ContainsKey("link"))
            {
                throw new Durados.DuradosException("The response does not contain the link");
            }
            return response["link"].ToString();
        }

        protected string CheckFileSize(string fileData)
        {
            int MaxJSONSize = MaxJSONSizeMB * 1024 * 1024;
            if (fileData == "file_stream" && System.Web.HttpContext.Current.Items["file_stream"] != null)
                fileData = System.Web.HttpContext.Current.Items["file_stream"].ToString();
            if (IsSizeExceeded(fileData, MaxJSONSize))
            {
                throw new Durados.DuradosException("File size is limited to " + MaxJSONSizeMB + " MB");
            }

            return fileData;
        }

        protected bool IsSizeExceeded(string fileData, int MaxJSONSize)
        {
            int size = fileData.Length;
            return size > MaxJSONSize * 4 / 3;
        }
  
        public virtual void delete(string fileName, string bucket= null, string path= null)
        {
            string url = BaseUrl + "/deleteFile";
            XMLHttpRequest request = new XMLHttpRequest();
            request.open("POST", url, false);
            
            string appName = System.Web.HttpContext.Current.Items[Durados.Database.AppName].ToString();
            Dictionary<string, object> data = GetFileDetails(fileName, S3FilesBucket, appName);

            request.setRequestHeader("content-type", "application/json");

            request.send(new System.Web.Script.Serialization.JavaScriptSerializer().Serialize(data));

            if (request.status != 200)
            {
                throw new Durados.DuradosException("Server return status " + request.status + ", " + request.responseText);
            }
        }

        protected Durados.Diagnostics.ILogger Logger
        {
            get
            {
                if (System.Web.HttpContext.Current == null)
                    return null;
                return (Durados.Diagnostics.ILogger)System.Web.HttpContext.Current.Items[Durados.Database.MainLogger];
            }
        }

        public virtual void delete(string fileName)
        {
            delete(fileName, null, null);
        }

        public virtual void delete(string fileName, string bucket)
        {
            delete(fileName, bucket, null);
        }


     
    }

   
    
}
