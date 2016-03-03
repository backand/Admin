using System;
using System.Collections.Generic;

namespace Backand
{
    public class files : IFiles
    {
        private string BaseUrl = System.Configuration.ConfigurationManager.AppSettings["nodeHost"] ?? "http://127.0.0.1:9000";
        private string S3FilesBucket = System.Configuration.ConfigurationManager.AppSettings["S3FilesBucket"] ?? "files.backand.net";
        private int MaxJSONSize = System.Convert.ToInt32(System.Configuration.ConfigurationManager.AppSettings["FtpMaxFileSize"] ?? "5")*1024*1024;
        
        public string upload(string fileName, string fileData)
        {
            return upload(fileName, fileData, null);
        }

         public string upload(string fileName, string fileData, string fileType)
        {
            if (fileData == "file_stream" && System.Web.HttpContext.Current.Items["file_stream"] != null)
                fileData = System.Web.HttpContext.Current.Items["file_stream"].ToString();
            string url = BaseUrl + "/uploadFile";
            XMLHttpRequest request = new XMLHttpRequest();
            request.open("POST", url, false);
            Dictionary<string, object> data = new Dictionary<string, object>();
            string appName = System.Web.HttpContext.Current.Items[Durados.Database.AppName].ToString();

            data.Add("fileName", fileName);
            data.Add("file", fileData);
            data.Add("bucket", S3FilesBucket);
            data.Add("dir", appName);
            if (!string.IsNullOrEmpty(fileType))
            {
                data.Add("fileType", fileType);
            }

            request.setRequestHeader("content-type", "application/json");

            System.Web.Script.Serialization.JavaScriptSerializer jss = new System.Web.Script.Serialization.JavaScriptSerializer();
            jss.MaxJsonLength = MaxJSONSize;
            request.send(jss.Serialize(data));

            if (request.status != 200)
            {
                throw new Durados.DuradosException("Server return status " + request.status + ", " + request.responseText);
            }

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

        public void delete(string fileName)
        {
            string url = BaseUrl + "/deleteFile";
            XMLHttpRequest request = new XMLHttpRequest();
            request.open("POST", url, false);
            Dictionary<string, object> data = new Dictionary<string, object>();
            string appName = System.Web.HttpContext.Current.Items[Durados.Database.AppName].ToString();

            data.Add("fileName", fileName);
            data.Add("bucket", S3FilesBucket);
            data.Add("dir", appName);

            request.setRequestHeader("content-type", "application/json");

            request.send(new System.Web.Script.Serialization.JavaScriptSerializer().Serialize(data));

            if (request.status != 200)
            {
                throw new Durados.DuradosException("Server return status " + request.status + ", " + request.responseText);
            }
        }
    }

    public interface IFiles
    {
        string upload(string fileName, string fileData, string fileType = null);

        void delete(string fileName);

    }
}
