using Backand;
using Durados.DataAccess;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Durados.Web.Mvc.UI.Helpers.Cloning
{
    public class Cloner
    {
        public void Clone(int sourceAppId, int targetAppId, CopyOptions copyOptions)
        {
            if (copyOptions.Schema && copyOptions.Data)
            {
                CopyDatabase(sourceAppId, targetAppId);
            }
            else if (copyOptions.Schema)
            {
                CopySchema(sourceAppId, targetAppId);
            }

            if (copyOptions.Configuration)
            {
                CopyConfiguration(sourceAppId, targetAppId);
            }
            else
            {
                Sync(targetAppId);
            }

            if (copyOptions.Cron)
            {
                CopyCron(sourceAppId, targetAppId);
            }

            if (copyOptions.Files)
            {
                CopyFiles(sourceAppId, targetAppId);
            }

            if (copyOptions.Host)
            {
                CopyHost(sourceAppId, targetAppId);
            }

            if (copyOptions.NodeJs)
            {
                CopyNodeJs(sourceAppId, targetAppId);
            }
        }

        private void CopyNodeJs(int sourceAppId, int targetAppId)
        {
            string bucketName = Maps.NodeJSBucket;
            CopyS3(bucketName, sourceAppId, targetAppId);
            CopyLambda(sourceAppId, targetAppId);
        }

        private void CopyLambda(int sourceAppId, int targetAppId)
        {
            throw new NotImplementedException();
        }

        protected virtual string GetNodeUrl()
        {
            return System.Configuration.ConfigurationManager.AppSettings["nodeHost"] ?? "http://127.0.0.1:9000";

        }

        private void CopyS3(string bucketName, int sourceAppId, int targetAppId)
        {
            string sourceAppName = Maps.Instance.GetAppNameById(sourceAppId);
            string destAppName = Maps.Instance.GetAppNameById(targetAppId);


            string url = GetNodeUrl() + "/copyFolder";
            XMLHttpRequest request = new XMLHttpRequest();
            request.open("POST", url, false);
            Dictionary<string, object> data = new Dictionary<string, object>();

            data.Add("bucket", bucketName);
            data.Add("sourceFolder", sourceAppName);
            data.Add("destFolder", destAppName);


            request.setRequestHeader("content-type", "application/json");

            System.Web.Script.Serialization.JavaScriptSerializer jss = new System.Web.Script.Serialization.JavaScriptSerializer();
            request.send(jss.Serialize(data));

            if (request.status != 200)
            {
                Maps.Instance.DuradosMap.Logger.Log("Cloner", "CopyS3", request.responseText, null, 1, "status: " + request.status);
                throw new DuradosException(request.responseText);
            }

            Dictionary<string, object>[] response = null;
            try
            {
                response = jss.Deserialize<Dictionary<string, object>[]>(request.responseText);
            }
            catch (Exception exception)
            {
                Maps.Instance.DuradosMap.Logger.Log("Cloner", "CopyS3", exception.Source, exception, 1, request.responseText);
                throw new DuradosException("Could not copy folder, response: " + request.responseText + ". With the following error: " + exception.Message);
            }
        }

        private void CopyHost(int sourceAppId, int targetAppId)
        {
            string bucketName = Maps.S3Bucket;
            CopyS3(bucketName, sourceAppId, targetAppId);
            
        }

        private void CopyFiles(int sourceAppId, int targetAppId)
        {
            string bucketName = Maps.S3FilesBucket;
            CopyS3(bucketName, sourceAppId, targetAppId);
            
        }

        private void CopyCron(int sourceAppId, int targetAppId)
        {
            throw new NotImplementedException();
        }

        private void Sync(int targetAppId)
        {
            throw new NotImplementedException();
        }

        private void CopyConfiguration(int sourceAppId, int targetAppId)
        {
            XmlConfigHelper configHelper = new XmlConfigHelper();

            Stream config = configHelper.GetZipConfig(Maps.Instance.GetAppNameById(sourceAppId), null);

            configHelper.UploadConfigFromFile(Maps.Instance.GetAppNameById(targetAppId), StreamToString(config));

        }

        public static string StreamToString(Stream stream)
        {
            stream.Position = 0;
            using (StreamReader reader = new StreamReader(stream, Encoding.UTF8))
            {
                return reader.ReadToEnd();
            }
        }

        private void CopyDatabase(int sourceAppId, int targetAppId)
        {
            MySqlAccess sa = new MySqlAccess();
            string adminConnectionString = GetAdminConnectionString();
            MySqlConnectionStringBuilder connBuilder = new MySqlConnectionStringBuilder(adminConnectionString);
            string username = connBuilder.UserID;
            string password = connBuilder.Password;

            sa.CopyDatabase(adminConnectionString, GetDatabaseName(sourceAppId), username, password, GetDatabaseName(targetAppId));
        }

        private string GetDatabaseName(int appId)
        {
            string sql =
                "SELECT        dbo.durados_SqlConnection.Catalog " +
                "FROM            dbo.durados_SqlConnection with(nolock) INNER JOIN " +
                         "dbo.durados_App with(nolock) ON dbo.durados_SqlConnection.Id = dbo.durados_App.SqlConnectionId " +
                "WHERE        dbo.durados_App.Id = " + appId;
            string scalar = new SqlAccess().ExecuteScalar(Maps.Instance.DuradosMap.connectionString, sql);
            if (!string.IsNullOrEmpty(scalar))
            {
                return scalar;
            }
            throw new Durados.DuradosException("Could not find app: " + appId);
        }

        private string GetAdminConnectionString()
        {
            AppFactory appFactory = new AppFactory();

            string server;
            int port;
            return appFactory.GetExternalAvailableInstanceConnectionString(SqlProduct.MySql, out server, out port);
        }

        private void CopySchema(int sourceAppId, int targetAppId)
        {
            throw new NotImplementedException();
        }
    }
}
