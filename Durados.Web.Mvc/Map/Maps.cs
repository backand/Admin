using System;
using System.Data;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Web;
using System.IO;
using System.Reflection;
using System.Data.SqlClient;

using Microsoft.WindowsAzure;
using Microsoft.WindowsAzure.ServiceRuntime;
using Microsoft.WindowsAzure.StorageClient;
using Microsoft.WindowsAzure.StorageClient.Protocol;

using Durados.DataAccess;
using Durados.Web.Mvc.UI.Helpers;
using Durados.Web.Mvc.Infrastructure;
using Durados.Web.Mvc.Azure;
using Newtonsoft.Json;
using System.Diagnostics;
using Durados.Data;
using Durados.SmartRun;
using Durados.Web.Mvc.Farm;
using Durados.Web.Mvc.Analytics;
using Durados.Web.Mvc.Stat.Measurements.S3;
using Durados.Web.Mvc.Webhook;
using System.Collections.Specialized;
using System.Configuration;
using System.Web.Script.Serialization;

namespace Durados.Web.Mvc
{
    public class Maps
    {
        private static Maps instance;

        public static string GetInMemoryKey()
        {
            try
            {
                if (System.Web.HttpContext.Current != null && System.Web.HttpContext.Current.Request != null)
                    return System.Web.HttpContext.Current.Request.Headers["TestKey"];
                return null;
            }
            catch
            {
                return null;
            }
        }

        public static bool IsInMemoryMode()
        {
            return GetInMemoryKey() != null;
        }

        public string[] GetVersions(string name)
        {
            int? id = AppExists(name);
            if (!id.HasValue)
                return null;

            string containerName = Maps.DuradosAppPrefix.Replace("_", "").Replace(".", "").ToLower() + id.ToString();
            CloudBlobContainer container = GetContainer(containerName);

            List<string> versions = (new Durados.Web.Mvc.Azure.BlobBackup()).GetVersions(container);

            return versions.ToArray();
        }

        public void Restore(string name, string version = null)
        {
            int? id = AppExists(name);
            if (!id.HasValue)
                return;

            string containerName = Maps.DuradosAppPrefix.Replace("_", "").Replace(".", "").ToLower() + id.ToString();
            CloudBlobContainer container = GetContainer(containerName);

            if (version == null)
            {
                (new Durados.Web.Mvc.Azure.BlobBackup()).RestoreSync(container, containerName);
            }
            else
            {
                (new Durados.Web.Mvc.Azure.BlobBackup()).CopyBack(container, version, containerName);
            }

            containerName += "xml";
            container = GetContainer(containerName);
            if (version == null)
            {
                (new Durados.Web.Mvc.Azure.BlobBackup()).RestoreSync(container, containerName);
            }
            else
            {
                (new Durados.Web.Mvc.Azure.BlobBackup()).CopyBack(container, version, containerName);
            }
        }
        public Dictionary<string, Stream> GetAllConfigs(string id, string version)
        {

            string containerName = Maps.DuradosAppPrefix.Replace("_", "").Replace(".", "").ToLower() + id;
            Dictionary<string, Stream> configs = new Dictionary<string, Stream>();
            configs.Add(containerName, GetConfig(containerName, version));
            configs.Add(containerName + "xml", GetSchemaConfig(id, version));
            return configs;
        }
        public Stream GetSchemaConfig(string id, string version)
        {
            string containerName = Maps.DuradosAppPrefix.Replace("_", "").Replace(".", "").ToLower() + id + "xml";
            return GetConfig(containerName, version);
        }

        public Stream GetConfig(string filename, string version)
        {
            if (version == null)
                version = string.Empty;
            else
                version = (new Durados.Web.Mvc.Azure.BlobBackup()).VersionPrefix + version;

            CloudBlobContainer container = GetContainer(filename);
            var source = container.GetBlobReference(filename + version);
            if (!source.Exists())
            {
                throw new DuradosException("Could not find configuration version " + version);
            }
            MemoryStream stream = new MemoryStream();
            source.DownloadToStream(stream);
            return stream;
        }

        Azure.DuradosStorage storage = new Azure.DuradosStorage();

        private CloudBlobContainer GetContainer(string filename)
        {
            // Get a handle on account, create a blob service client and get container proxy
            //var account = CloudStorageAccount.Parse(RoleEnvironment.GetConfigurationSettingValue("ConfigAzureStorage"));
            //var client = account.CreateCloudBlobClient();
            //return client.GetContainerReference(RoleEnvironment.GetConfigurationSettingValue("configContainer"));
            return storage.GetContainer(filename);
        }

        private Maps()
        {
            InitPersistency();

            if (multiTenancy)
            {
                duradosMap = new DuradosMap();
                duradosMap.connectionString = persistency.ConnectionString;
                duradosMap.systemConnectionString = persistency.ConnectionString;
                duradosMap.ConfigFileName = Maps.GetConfigPath(Maps.GetmainAppConfigName() + ".xml");
                duradosMap.Url = GetAppUrl(duradosAppName);
                duradosMap.Initiate(false);

                View appView = (View)duradosMap.Database.Views["durados_App"];
                appView.PermanentFilter = "(durados_App.toDelete =0 AND (durados_App.Creator = [m_User] or durados_App.id in (select durados_UserApp.AppId from durados_UserApp where durados_UserApp.UserId = [m_User] and (durados_UserApp.[Role] = 'Admin' or durados_UserApp.[Role] = 'Developer'))))";
                appView.Controller = "MultiTenancy";

                View connectionView = (View)duradosMap.Database.Views["durados_SqlConnection"];
                connectionView.PermanentFilter = "";// "DuradosUser = [Durados_User] ";//OR durados_SqlConnection.id  in 
                //((select SqlConnectionId from durados_app inner join durados_userApp on durados_app.id =durados_userApp.appId where durados_UserApp.UserId = [Durados_User])
                //union
                //(select SystemSqlConnectionId from durados_app inner join durados_userApp on durados_app.id =durados_userApp.appId where durados_UserApp.UserId = [Durados_User]))";

                //maps = new Dictionary<string, Map>();
                mapsCache = CacheFactory.CreateCache<Map>("maps");

                LoadDnsAliases();

                PluginsCache = new Dictionary<PlugInType, PluginCache>();

                foreach (PlugInType plugInType in Enum.GetValues(typeof(PlugInType)))
                {
                    PluginsCache.Add(plugInType, new PluginCache());
                }
            }
        }



        public static Maps Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new Maps();
                }
                return instance;
            }
        }

        public void WakeupCalltoApps()
        {
            using (SqlConnection connection = new SqlConnection(duradosMap.connectionString))
            {
                connection.Open();

                string sql = "select [Id],[Url] from dbo.durados_App with (NOLOCK) where [Creator] is null";

                using (SqlCommand command = new SqlCommand(sql, connection))
                {
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        int urlOrdinal = reader.GetOrdinal("Url");

                        while (reader.Read())
                        {
                            string url = reader.GetString(urlOrdinal).ToLower();
                            //Infrastructure.ISendAsyncErrorHandler SendAsyncErrorHandler = null;
                            //ignore errors
                            Infrastructure.Http.CallWebRequest(url.Split('|')[2]);
                        }
                    }
                }
            }

        }

        private void LoadDnsAliases()
        {
            DnsAliases = new Dictionary<string, string>();

            using (SqlConnection connection = new SqlConnection(duradosMap.connectionString))
            {
                connection.Open();

                string sql = "SELECT dbo.durados_DnsAlias.Alias, dbo.durados_App.Name FROM dbo.durados_App with (NOLOCK) INNER JOIN dbo.durados_DnsAlias with (NOLOCK) ON dbo.durados_App.Id = dbo.durados_DnsAlias.AppId";

                using (SqlCommand command = new SqlCommand(sql, connection))
                {
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        int aliasOrdinal = reader.GetOrdinal("Alias");
                        int nameOrdinal = reader.GetOrdinal("Name");
                        while (reader.Read())
                        {
                            DnsAliases.Add(reader.GetString(aliasOrdinal).ToLower(), reader.GetString(nameOrdinal).ToLower());
                        }
                    }
                }
            }

        }

        static Maps()
        {
            host = System.Configuration.ConfigurationManager.AppSettings["durados_host"] ?? "durados.com";
            poolCreator = Convert.ToInt32(System.Configuration.ConfigurationManager.AppSettings["poolCreator"] ?? "55555");
            poolShouldBeUsed = Convert.ToBoolean(System.Configuration.ConfigurationManager.AppSettings["poolShouldBeUsed"] ?? "false");

            redisConnectionString = System.Configuration.ConfigurationManager.AppSettings["redisConnectionString"] ?? "pub-redis-10938.us-east-1-4.3.ec2.garantiadata.com:10938,password=bell1234";


            mainAppConfigName = System.Configuration.ConfigurationManager.AppSettings["mainAppConfigName"] ?? "backand";
            hostByUs = Convert.ToBoolean(System.Configuration.ConfigurationManager.AppSettings["hostByUs"] ?? "false");
            duradosAppName = System.Configuration.ConfigurationManager.AppSettings["durados_appName"] ?? "www";
            demoAzureUsername = System.Configuration.ConfigurationManager.AppSettings["demoAzureUsername"] ?? "itayher";
            demoAzurePassword = System.Configuration.ConfigurationManager.AppSettings["demoAzurePassword"] ?? "Durados2012";
            demoSqlUsername = System.Configuration.ConfigurationManager.AppSettings["demoSqlUsername"] ?? "durados";
            demoSqlPassword = System.Configuration.ConfigurationManager.AppSettings["demoSqlPassword"] ?? "durados";
            demoDatabaseName = System.Configuration.ConfigurationManager.AppSettings["demoDatabaseName"] ?? "Northwind";
            demoConfigFilename = System.Configuration.ConfigurationManager.AppSettings["demoConfigFilename"] ?? "Northwind";
            demoAzureServer = System.Configuration.ConfigurationManager.AppSettings["demoAzureServer"] ?? "tcp:d9gwdrhh5n.database.windows.net,1433";
            demoOnPremiseServer = System.Configuration.ConfigurationManager.AppSettings["demoOnPremiseServer"] ?? @"durados.info\sqlexpress";
            demoCreatePending = Convert.ToBoolean(System.Configuration.ConfigurationManager.AppSettings["demoCreatePending"] ?? "true");
            demoPendingNext = Convert.ToInt32(System.Configuration.ConfigurationManager.AppSettings["demoPendingNext"] ?? "5");
            demoFtpTempHost = System.Configuration.ConfigurationManager.AppSettings["demoFtpTempHost"] ?? "temp";
            demoFtpHost = System.Configuration.ConfigurationManager.AppSettings["demoFtpHost"] ?? "durados.info";
            demoFtpPort = System.Configuration.ConfigurationManager.AppSettings["demoFtpPort"] ?? "21";
            demoFtpFileSizeLimitKb = Convert.ToInt64(System.Configuration.ConfigurationManager.AppSettings["demoFtpFileSizeLimitKb"] ?? "1024");
            demoFtpFolderSizeLimitKb = Convert.ToInt64(System.Configuration.ConfigurationManager.AppSettings["demoFtpFolderSizeLimitKb"] ?? "1024");
            demoFtpUser = System.Configuration.ConfigurationManager.AppSettings["demoFtpUser"] ?? "itay";
            demoFtpPassword = System.Configuration.ConfigurationManager.AppSettings["demoFtpPassword"] ?? "dio2008";
            demoFtpPhysicalPath = System.Configuration.ConfigurationManager.AppSettings["demoFtpPhysicalPath"] ?? @"C:\FTP\";
            demoUploadSourcePath = System.Configuration.ConfigurationManager.AppSettings["demoUploadSourcePath"] ?? "/Uploads/220/";
            demoOnPremiseSourcePath = System.Configuration.ConfigurationManager.AppSettings["demoOnPremiseSourcePath"] ?? @"C:\Dev\Databases\";
            allowLocalConnection = Convert.ToBoolean(System.Configuration.ConfigurationManager.AppSettings["allowLocalConnection"] ?? "false");
            cloud = Convert.ToBoolean(System.Configuration.ConfigurationManager.AppSettings["cloud"] ?? "false"); // false;// RoleEnvironment.IsAvailable;//
            multiTenancy = Convert.ToBoolean(System.Configuration.ConfigurationManager.AppSettings["multiTenancy"] ?? "false");
            useSecureConnection = Convert.ToBoolean(System.Configuration.ConfigurationManager.AppSettings["UseSecureConnection"] ?? "false");
            skin = Convert.ToBoolean(System.Configuration.ConfigurationManager.AppSettings["skin"] ?? "false");
            duradosAppPrefix = Convert.ToString(System.Configuration.ConfigurationManager.AppSettings["duradosAppSysPrefix"] ?? "durados_AppSys_");
            duradosAppSysPrefix = Convert.ToString(System.Configuration.ConfigurationManager.AppSettings["duradosAppPrefix"] ?? "durados_App_");
            debug = Convert.ToBoolean(System.Configuration.ConfigurationManager.AppSettings["debug"] ?? "false");
            appNameMax = Convert.ToInt32(System.Configuration.ConfigurationManager.AppSettings["appNameMax"] ?? "32");
            dropAppDatabase = Convert.ToBoolean(System.Configuration.ConfigurationManager.AppSettings["dropAppDatabase"] ?? "true");
            privateCloud = Convert.ToBoolean(System.Configuration.ConfigurationManager.AppSettings["privateCloud"] ?? "false");
            ConfigPath = Convert.ToString(System.Configuration.ConfigurationManager.AppSettings["configPath"] ?? "~/Config/");
            plugInSampleGenerationCount = Convert.ToInt32(System.Configuration.ConfigurationManager.AppSettings["plugInSampleGenerationCount"] ?? "5");

            superDeveloper = Convert.ToString(System.Configuration.ConfigurationManager.AppSettings["superDeveloper"] ?? "dev@devitout.com").ToLower();

            DownloadDenyPolicy = Convert.ToBoolean(System.Configuration.ConfigurationManager.AppSettings["DownloadDenyPolicy"] ?? "true");
            OldAdminHttp = Convert.ToBoolean(System.Configuration.ConfigurationManager.AppSettings["OldAdminHttp"] ?? "false");
            AllowedDownloadFileTypes = Convert.ToString(System.Configuration.ConfigurationManager.AppSettings["AllowedDownloadFileTypes"] ?? allowedDownloadFileTypesDefault).Split(',').ToArray();
            DenyDownloadFileTypes = Convert.ToString(System.Configuration.ConfigurationManager.AppSettings["DenyDownloadFileTypes"] ?? denyDownloadFileTypesDefault).Split(',').ToArray();

            ReservedAppNames = Convert.ToString(System.Configuration.ConfigurationManager.AppSettings["ReservedAppNames"] ?? reservedAppNames).Split(',').Select(k => k).ToHashSet<string>();

            azureDatabasePendingPool = new PendingPool(demoPendingNext);
            onPremiseDatabasePendingPool = new PendingPool(demoPendingNext);

            AzureStorageAccountName = System.Configuration.ConfigurationManager.AppSettings["AzureStorageAccountName"];
            if (AzureStorageAccountName == null)
            {
                throw new DuradosException("Please add the AzureStorageAccountName to the web.config.");
            }

            ConfigAzureStorageAccountName = System.Configuration.ConfigurationManager.AppSettings["ConfigAzureStorageAccountName"];
            if (ConfigAzureStorageAccountName == null)
            {
                throw new DuradosException("Please add the ConfigAzureStorageAccountName to the web.config.");
            }

            AzureStorageAccountKey = System.Configuration.ConfigurationManager.AppSettings["AzureStorageAccountKey"];
            if (AzureStorageAccountKey == null)
            {
                throw new DuradosException("Please add the AzureStorageAccountKey to the web.config.");
            }

            ConfigAzureStorageAccountKey = System.Configuration.ConfigurationManager.AppSettings["ConfigAzureStorageAccountKey"];
            if (ConfigAzureStorageAccountKey == null)
            {
                throw new DuradosException("Please add the ConfigAzureStorageAccountKey to the web.config.");
            }


            AzureStorageUrl = System.Configuration.ConfigurationManager.AppSettings["AzureStorageUrl"] ?? "http://{0}.blob.core.windows.net/{1}";


            AzureCacheAccountKey = System.Configuration.ConfigurationManager.AppSettings["AzureCacheAccountKey"];
            if (AzureCacheAccountKey == null)
            {
                throw new DuradosException("Please add the AzureCacheAccountKey to the web.config.");
            }

            AzureCacheAccountName = System.Configuration.ConfigurationManager.AppSettings["AzureCacheAccountName"];
            if (AzureCacheAccountName == null)
            {
                throw new DuradosException("Please add the AzureCacheAccountName to the web.config.");
            }

            AzureCacheUrl = System.Configuration.ConfigurationManager.AppSettings["AzureCacheUrl"];
            if (AzureCacheUrl == null)
            {
                throw new DuradosException("Please add the AzureCacheUrl to the web.config.");
            }



            AzureCachePort = Convert.ToInt32(System.Configuration.ConfigurationManager.AppSettings["AzureCachePort"] ?? "22233");

            AzureCacheUpdateInterval = new TimeSpan(0, 0, Convert.ToInt32(System.Configuration.ConfigurationManager.AppSettings["AzureCacheUpdateInterval"] ?? "60"));

            DefaultUploadName = System.Configuration.ConfigurationManager.AppSettings["DefaultUploadName"] ?? "DefaultUpload";

            DefaultImageHeight = Convert.ToInt32(System.Configuration.ConfigurationManager.AppSettings["DefaultImageHeight"] ?? "80");

            SplitProducts = Convert.ToBoolean(System.Configuration.ConfigurationManager.AppSettings["SplitProducts"] ?? "true");

            ProductsPort = new Dictionary<SqlProduct, int>();

            foreach (SqlProduct sqlProduct in Enum.GetValues(typeof(SqlProduct)))
            {
                ProductsPort.Add(sqlProduct, Convert.ToInt32(System.Configuration.ConfigurationManager.AppSettings[sqlProduct.ToString() + "Port"] ?? "80"));
            }

            ApiUrls = (System.Configuration.ConfigurationManager.AppSettings["apiUrls"] ?? string.Empty).Split(';');

            UserPreviewUrl = System.Configuration.ConfigurationManager.AppSettings["UserPreviewUrl"] ?? ".backand.loc:4012/";

            S3Bucket = System.Configuration.ConfigurationManager.AppSettings["S3Bucket"];
            if (string.IsNullOrEmpty(S3Bucket))
            {
                throw new DuradosException("Missing S3Bucket key in web config");
            }

            S3FilesBucket = System.Configuration.ConfigurationManager.AppSettings["S3FilesBucket"];

            if (string.IsNullOrEmpty(S3FilesBucket))
            {
                throw new DuradosException("Missing S3FilesBucket key in web config");
            }

            ParseConverterMasterKey = System.Configuration.ConfigurationManager.AppSettings["ParseConverterMasterKey"];
            if (string.IsNullOrEmpty(ParseConverterMasterKey))
            {
                throw new DuradosException("Missing ParseConverterMasterKey key in web config");
            }

            ParseConverterAdminKey = System.Configuration.ConfigurationManager.AppSettings["ParseConverterAdminKey"];
            if (string.IsNullOrEmpty(ParseConverterAdminKey))
            {
                throw new DuradosException("Missing ParseConverterAdminKey key in web config");
            }

            ParseConverterObjectName = System.Configuration.ConfigurationManager.AppSettings["ParseConverterObjectName"];
            if (string.IsNullOrEmpty(ParseConverterObjectName))
            {
                throw new DuradosException("Missing ParseConverterObjectName key in web config");
            }

            AppLockedMessage = System.Configuration.ConfigurationManager.AppSettings["AppLockedMessage"];

            if (string.IsNullOrEmpty(AppLockedMessage))
            {
                throw new DuradosException("Missing AppLockedMessage key in web config");
            }

            NodeJSBucket = System.Configuration.ConfigurationManager.AppSettings["NodeJSBucket"];

            if (string.IsNullOrEmpty(NodeJSBucket))
            {
                throw new DuradosException("Missing NodeJSBucket key in web config");
            }


            ExcludedEmailDomains = System.Configuration.ConfigurationManager.AppSettings["ExcludedEmailDomains"];

            if (string.IsNullOrEmpty(ExcludedEmailDomains))
            {
                throw new DuradosException("Missing ExcludedEmailDomains key in web config");
            }

            ReportConnectionString = System.Web.Configuration.WebConfigurationManager.ConnectionStrings["reportConnectionString"].ConnectionString;

            if (string.IsNullOrEmpty(ReportConnectionString))
            {
                throw new DuradosException("Missing reportConnectionString key in web config");
            }

            SendWelcomeEmail = System.Configuration.ConfigurationManager.AppSettings["SendWelcomeEmail"] ?? "true";

            System.Net.ServicePointManager.SecurityProtocol = System.Net.SecurityProtocolType.Ssl3 | System.Net.SecurityProtocolType.Tls | System.Net.SecurityProtocolType.Tls11 | System.Net.SecurityProtocolType.Tls12;

            DevUsers = System.Configuration.ConfigurationManager.AppSettings["DevUsers"].Split(',');

            string embeddedReportsApiToken = System.Configuration.ConfigurationManager.AppSettings["embeddedReportsApiToken"];
            if (string.IsNullOrEmpty(embeddedReportsApiToken))
            {
                throw new DuradosException("Missing embeddedReportsApiToken key in web config");
            }
            string embeddedReportsUrlStep1 = System.Configuration.ConfigurationManager.AppSettings["embeddedReportsUrlStep1"];
            if (string.IsNullOrEmpty(embeddedReportsUrlStep1))
            {
                throw new DuradosException("Missing embeddedReportsUrlStep1 key in web config");
            }
            string embeddedReportsUrlStep2 = System.Configuration.ConfigurationManager.AppSettings["embeddedReportsUrlStep2"];
            if (string.IsNullOrEmpty(embeddedReportsUrlStep2))
            {
                throw new DuradosException("Missing embeddedReportsUrlStep2 key in web config");
            }
            string embeddedReportAppPropertyName = System.Configuration.ConfigurationManager.AppSettings["embeddedReportAppPropertyName"];
            if (string.IsNullOrEmpty(embeddedReportAppPropertyName))
            {
                throw new DuradosException("Missing embeddedReportAppPropertyName key in web config");
            }
            embeddedReportsConfig = new EmbeddedReportsConfig(embeddedReportsApiToken, embeddedReportsUrlStep1, embeddedReportsUrlStep2, embeddedReportAppPropertyName);

            string awsAccessKeyId = System.Configuration.ConfigurationManager.AppSettings["awsAccessKeyId"];
            if (string.IsNullOrEmpty(awsAccessKeyId))
            {
                throw new DuradosException("Missing awsAccessKeyId key in web config");
            }
            string awsSecretAccessKey = System.Configuration.ConfigurationManager.AppSettings["awsSecretAccessKey"];
            if (string.IsNullOrEmpty(awsSecretAccessKey))
            {
                throw new DuradosException("Missing awsSecretAccessKey key in web config");
            }
            awsCredentials = new AwsCredentials() { AccessKeyID = awsAccessKeyId, SecretAccessKey = awsSecretAccessKey };

            WebhooksParametersFileName = System.Configuration.ConfigurationManager.AppSettings["webhooksParametersFileName"];

            cqlConfig = new CqlConfig();
            cqlConfig.ApiUrl = System.Configuration.ConfigurationManager.AppSettings["CqlApiUrl"];
            if (string.IsNullOrEmpty(cqlConfig.ApiUrl))
            {
                throw new DuradosException("Missing CqlApiUrl key in web config");
            }

            cqlConfig.AuthorizationHeader = System.Configuration.ConfigurationManager.AppSettings["CqlAuthorizationHeader"];
            if (string.IsNullOrEmpty(cqlConfig.AuthorizationHeader))
            {
                throw new DuradosException("Missing CqlAuthorizationHeader key in web config");
            }

            string cqlsFileName = System.Configuration.ConfigurationManager.AppSettings["CqlsFileName"];
            if (string.IsNullOrEmpty(cqlsFileName))
            {
                throw new DuradosException("Missing CqlsFileName key in web config");
            }
            cqlConfig.Cqls = GetCqls(cqlsFileName);
            //GetWebhookParameters("AppCreated");


            LambdaArn = System.Configuration.ConfigurationManager.AppSettings["lambdaArn"];
            if (string.IsNullOrEmpty(LambdaArn))
            {
                throw new DuradosException("Missing lambdaArn key in web config");
            }

            CronPrefix = System.Configuration.ConfigurationManager.AppSettings["cronPrefix"];
            if (string.IsNullOrEmpty(CronPrefix))
            {
                throw new DuradosException("Missing CronPrefix key in web config");
            }

            CronAuthorizationHeader = System.Configuration.ConfigurationManager.AppSettings["cronAuthorizationHeader"];
            if (string.IsNullOrEmpty(CronAuthorizationHeader))
            {
                throw new DuradosException("Missing CronAuthorizationHeader key in web config");
            }

            limits.Add(Limits.Cron, Convert.ToInt32(System.Configuration.ConfigurationManager.AppSettings["cronLimit"]));

            LocalAddress = System.Configuration.ConfigurationManager.AppSettings["localAddress"] ?? "http://localhost:8080";

            SocialRedirectHost = System.Configuration.ConfigurationManager.AppSettings["socialRedirectHost"];
        }

        private static Dictionary<string, string> GetCqls(string cqlsFileName)
        {
            Dictionary<string, string> dic =  new Dictionary<string, string>();
            string jsonString = GetStringFromFile(cqlsFileName, ref cqlsJsonString).Replace("\\", "\\\\");


            Dictionary<string, object> cqls = Durados.Web.Mvc.Controllers.Api.JsonConverter.Deserialize(jsonString);

            foreach (string name in cqls.Keys)
            {
                dic.Add(name, cqls[name].ToString());
            }

            return dic;
        }



        private static AwsCredentials awsCredentials;
        public static AwsCredentials AwsCredentials
        {
            get
            {
                return awsCredentials;
            }
        }

        public static string GetConfigPath(string filename)
        {
            //if (ConfigPath.StartsWith("~"))
            //    return System.Web.HttpContext.Current.Server.MapPath(ConfigPath + filename);
            //else 
            //    return ConfigPath + filename.Replace('/','\\');

            return GetConfigPath(filename, ConfigPath);
        }

        public static string GetConfigPath(string filename, string configPath)
        {
            if (configPath.StartsWith("~"))
                return System.Web.HttpContext.Current.Server.MapPath(configPath + filename);
            else
                return configPath + filename.Replace('/', '\\');
        }

        public static string GetConfigPath(string filename, SqlProduct sqlProduct)
        {
            string key = sqlProduct.ToString() + "ConfigPath";
            string configPath = System.Configuration.ConfigurationManager.AppSettings[key] ?? ConfigPath;

            return GetConfigPath(filename, configPath);
        }

        public static string GetUploadPath(SqlProduct sqlProduct)
        {
            string key = sqlProduct.ToString() + "UploadPath";
            string uploadPath = System.Configuration.ConfigurationManager.AppSettings[key];

            if (string.IsNullOrEmpty(uploadPath))
                return System.Web.HttpContext.Current.Server.MapPath("/Uploads/");
            else if (uploadPath.StartsWith("~"))
                return System.Web.HttpContext.Current.Server.MapPath(uploadPath);
            else
                return uploadPath;
        }

        public static string GetDeploymentPath(string filename)
        {
            return System.Web.HttpContext.Current.Server.MapPath("~/Deployment/") + filename;

        }

        public static string Version = null;

        public static Dictionary<SqlProduct, int> ProductsPort { get; private set; }
        public static bool SplitProducts { get; private set; }
        public static string AzureStorageUrl { get; private set; }
        public static string AzureStorageAccountName { get; private set; }
        public static string AzureStorageAccountKey { get; private set; }
        public static string DefaultUploadName { get; private set; }
        public static int DefaultImageHeight { get; private set; }
        public static string ConfigAzureStorageAccountName { get; private set; }
        public static string ConfigAzureStorageAccountKey { get; private set; }
        public static string[] ApiUrls { get; private set; }

        public static string AzureCacheAccountKey { get; private set; }
        public static string AzureCacheAccountName { get; private set; }
        public static string AzureCacheUrl { get; private set; }
        public static int AzureCachePort { get; private set; }
        public static string UserPreviewUrl { get; private set; }
        public static string S3Bucket { get; private set; }
        public static string S3FilesBucket { get; private set; }
        public static string SendWelcomeEmail { get; private set; }
        public static string[] DevUsers { get; private set; }

        public static string ParseConverterMasterKey { get; private set; }
        public static string LambdaArn { get; private set; }
        public static string CronPrefix { get; private set; }
        public static string CronAuthorizationHeader { get; private set; }
        
        
        public static string ParseConverterAdminKey { get; private set; }
        public static string ParseConverterObjectName { get; private set; }
        public static string NodeJSBucket { get; private set; }
        public static string AppLockedMessage { get; private set; }
        public static string ExcludedEmailDomains { get; private set; }
        public static string ReportConnectionString { get; private set; }

        public static TimeSpan AzureCacheUpdateInterval { get; private set; }

        public static string ConfigPath { get; private set; }

        public static bool DownloadDenyPolicy { get; private set; }
        public static string[] AllowedDownloadFileTypes { get; private set; }
        public static string[] DenyDownloadFileTypes { get; private set; }

        private Durados.Data.ICache<Map> mapsCache = null;
        public static Dictionary<string, string> DnsAliases = null;
        private IPersistency persistency = null;
        private static bool multiTenancy = false;
        private static string duradosAppPrefix;
        private static string duradosAppSysPrefix;
        private static bool cloud = false;
        private static bool skin = false;
        private static bool useSecureConnection = false;
        private static bool debug = false;
        private static bool dropAppDatabase = true;
        private static int appNameMax = 32;
        private static string host = "durados.com";
        private static int poolCreator = 5555;
        private static bool poolShouldBeUsed = false;

        private static string redisConnectionString = "";

        private static string mainAppConfigName = "backand";
        private static bool hostByUs = false;
        private static string duradosAppName = "www";
        private static string demoAzureUsername = "itayher";
        private static string demoAzurePassword = "Durados2012";
        private static string demoSqlUsername = "durados";
        private static string demoSqlPassword = "durados";
        private static string demoDatabaseName = "Northwind";
        private static string demoConfigFilename = "Northwind";
        private static string demoAzureServer = "tcp:d9gwdrhh5n.database.windows.net,1433";
        private static string demoOnPremiseServer = @"durados.info\sqlexpress";
        private static bool demoCreatePending = true;
        private static int demoPendingNext = 5;
        private static string demoFtpTempHost = "temp";
        private static string demoFtpHost = "durados.info";
        private static string demoFtpPort = "21";
        private static long demoFtpFileSizeLimitKb = 1024;
        private static long demoFtpFolderSizeLimitKb = 1024;
        private static string demoFtpPhysicalPath = @"C:\FTP\";
        private static string demoUploadSourcePath = @"C:\Dev\Demo\";
        private static string demoOnPremiseSourcePath = @"C:\Dev\Databases\";
        private static string demoFtpUser = "itay";
        private static string demoFtpPassword = "dio2008";
        private static bool allowLocalConnection = false;
        private static PendingPool azureDatabasePendingPool;
        private static PendingPool onPremiseDatabasePendingPool;
        private static string PandingDatabaseSuffix = "Pending";
        private static bool privateCloud = false;

        private static bool downloadDenyPolicy = true;
        private static string allowedDownloadFileTypesDefault = "jpg,png,gif,pdf,docx,doc,xls,xlsx,pptx,ppt";
        private static string denyDownloadFileTypesDefault = "ade,adp,app,bas,bat,chm,cmd,cpl,crt,csh,exe,fxp,hlp,hta,inf,ins,isp,ksh,Lnk,mda,mdb,mde,mdt,mdt,mdw,mdz,msc,msi,msp,mst,ops,pcd,pif,prf,prg,pst,reg,scf,scr,sct,shb,shs,url,vb,vbe,vbs,wsc,wsf,wsh,config,dll";
        private static string downloadActionName = "Download";
        private static string azureAppPrefix = "app";

        private static int plugInSampleGenerationCount = 5;
        private static string superDeveloper = "dev@devitout.com";
        private static string adminButtonText = "Admin";
        private static string publicButtonText = "Public";
        public static bool OldAdminHttp = false;

        private static string reservedAppNames = "api";

        private Map duradosMap = null;
        System.Data.SqlClient.SqlConnectionStringBuilder builder = new System.Data.SqlClient.SqlConnectionStringBuilder();

        private static CqlConfig cqlConfig;
        public static string GetAdminButtonText()
        {
            return adminButtonText;
        }

        public static string GetPublicButtonText()
        {
            return publicButtonText;
        }

        public static string GetAdminButtonUrl(Map map)
        {
            return "/Home/Default?workspaceId=" + map.Database.GetAdminWorkspaceId() + "&menuId=10001";
        }

        public static string GetPublicButtonUrl(Map map)
        {
            return "/Home/Default?workspaceId=" + map.Database.GetPublicWorkspaceId();
        }

        public static bool IsSuperDeveloper(string userName)
        {
            if (string.IsNullOrEmpty(userName) && HttpContext.Current.User != null && HttpContext.Current.User.Identity != null && HttpContext.Current.User.Identity.Name != null)
                userName = HttpContext.Current.User.Identity.Name;
            return !string.IsNullOrEmpty(userName) && userName.Equals(SuperDeveloper);
        }

        protected virtual void InitPersistency()
        {
            if (multiTenancy)
            {


                IPersistency sqlPersistency = GetNewPersistency();
                sqlPersistency.ConnectionString = System.Configuration.ConfigurationManager.ConnectionStrings["MapsConnectionString"].ConnectionString;
                if (System.Configuration.ConfigurationManager.ConnectionStrings["SystemMapsConnectionString"] == null)
                    throw new DuradosException("Please add SystemMapsConnectionString to the web.config connection strings");
                sqlPersistency.SystemConnectionString = System.Configuration.ConfigurationManager.ConnectionStrings["SystemMapsConnectionString"].ConnectionString;

                persistency = sqlPersistency;
                builder.ConnectionString = sqlPersistency.ConnectionString;

                Durados.DataAccess.ConfigAccess.storage = new Map();
                Durados.DataAccess.ConfigAccess.multiTenancy = multiTenancy;
                Durados.DataAccess.ConfigAccess.cloud = cloud;


            }
        }



        public static string GetPendingDatabase(string template)
        {
            string next;
            if (template == "1")
                next = azureDatabasePendingPool.Next().ToString();
            else
                next = onPremiseDatabasePendingPool.Next().ToString();
            return demoDatabaseName + PandingDatabaseSuffix + next;
        }

        public static CqlConfig CqlConfig
        {
            get
            {
                return cqlConfig;
            }
        }

        public virtual IPersistency GetNewPersistency()
        {
            return new SqlPersistency();
        }

        public static bool PrivateCloud
        {
            get
            {
                return privateCloud;
            }
        }

        public static bool DropAppDatabase
        {
            get
            {
                return dropAppDatabase;
            }
        }

        public static bool AllowLocalConnection
        {
            get
            {
                return allowLocalConnection;
            }
        }

        public static string DemoFtpPhysicalPath
        {
            get
            {
                return demoFtpPhysicalPath;
            }
        }

        public static string DemoUploadSourcePath
        {
            get
            {
                return demoUploadSourcePath;
            }
        }


        public static string DemoOnPremiseSourcePath
        {
            get
            {
                return demoOnPremiseSourcePath;
            }
        }

        public static string DemoFtpPassword
        {
            get
            {
                return demoFtpPassword;
            }
        }

        public static string DemoFtpUser
        {
            get
            {
                return demoFtpUser;
            }
        }

        public static string DemoFtpHost
        {
            get
            {
                return demoFtpHost;
            }
        }
        public static int PoolCreator
        {
            get
            {
                return poolCreator;
            }
        }

        public static string RedisConnectionString
        {
            get
            {
                return redisConnectionString;
            }
        }


        public static bool PoolShouldBeUsed
        {
            get
            {
                return poolShouldBeUsed;
            }
        }




        public static string DemoFtpPort
        {
            get
            {
                return demoFtpPort;
            }
        }

        public static long DemoFtpFileSizeLimitKb
        {
            get
            {
                return demoFtpFileSizeLimitKb;
            }
        }

        public static long DemoFtpFolderSizeLimitKb
        {
            get
            {
                return demoFtpFolderSizeLimitKb;
            }
        }

        public static string DemoFtpTempHost
        {
            get
            {
                return demoFtpTempHost;
            }
        }

        public static int DemoPendingNext
        {
            get
            {
                return demoPendingNext;
            }
        }

        public static bool DemoCreatePending
        {
            get
            {
                return demoCreatePending;
            }
        }

        public static string DemoConfigFilename
        {
            get
            {
                return demoConfigFilename;
            }
        }

        public static string DemoDatabaseName
        {
            get
            {
                return demoDatabaseName;
            }
        }

        public static string DemoAzureUsername
        {
            get
            {
                return demoAzureUsername;
            }
        }

        public static string DemoAzurePassword
        {
            get
            {
                return demoAzurePassword;
            }
        }

        public static string DemoSqlUsername
        {
            get
            {
                return demoSqlUsername;
            }
        }

        public static string DemoSqlPassword
        {
            get
            {
                return demoSqlPassword;
            }
        }

        public static string DemoAzureServer
        {
            get
            {
                return demoAzureServer;
            }
        }

        public static string DemoOnPremiseServer
        {
            get
            {
                return demoOnPremiseServer;
            }
        }

        public static string Host
        {
            get
            {
                return host;
            }
        }

        public static bool HostByUs
        {
            get
            {
                return hostByUs;
            }
        }

        public static string DuradosAppName
        {
            get
            {
                return duradosAppName;
            }
        }

        public static bool Debug
        {
            get
            {
                return debug;
            }
        }

        public static bool UseSecureConnection
        {
            get
            {
                return useSecureConnection;
            }
        }

        public static int AppNameMax
        {
            get
            {
                return appNameMax;
            }
        }

        public static string SuperDeveloper
        {
            get
            {
                return superDeveloper;
            }
        }


        public static int PlugInSampleGenerationCount
        {
            get
            {
                return plugInSampleGenerationCount;
            }
        }

        public string ConnectionString
        {
            get
            {
                return persistency.ConnectionString;
            }
        }
        public string SystemConnectionString
        {
            get
            {
                return persistency.SystemConnectionString;
            }
        }

        public static bool MultiTenancy
        {
            get
            {
                return multiTenancy;
            }
        }

        public static bool Skin
        {
            get
            {
                return skin;
            }
        }

        public static string DuradosAppSysPrefix
        {
            get
            {
                return duradosAppSysPrefix;
            }
        }

        public static string DuradosAppPrefix
        {
            get
            {
                return duradosAppPrefix;
            }
        }

        public static bool Cloud
        {
            get
            {
                return cloud;
            }
        }

        public static string DownloadActionName
        {
            get
            {
                return downloadActionName;
            }
        }
        public static string AzureAppPrefix
        {
            get
            {
                return azureAppPrefix;
            }
        }

        public static string GetCurrentAppName()
        {
            if (System.Web.HttpContext.Current == null)
            {
                return null;
            }
            if (System.Web.HttpContext.Current.Items.Contains(Database.AppName))
            {
                return System.Web.HttpContext.Current.Items[Database.AppName].ToString();
            }

            if (System.Web.HttpContext.Current == null)
                throw new DuradosException("System.Web.HttpContext.Current is null");
            if (System.Web.HttpContext.Current.Request == null)
                throw new DuradosException("System.Web.HttpContext.Current.Request is null");
            if (System.Web.HttpContext.Current.Request.Headers == null)
                throw new DuradosException("System.Web.HttpContext.Current.Request.Headers is null");
            if (System.Web.HttpContext.Current.Request.Headers["Host"] == null)
                throw new DuradosException("System.Web.HttpContext.Current.Request.Headers[\"Host\"] is null");

            string headersHost = System.Web.HttpContext.Current.Request.Headers["Host"];
            string port = System.Web.HttpContext.Current.Request.Url.Port.ToString();

            if (headersHost.ToLower().Contains(host.ToLower()))
            {

                return headersHost.Replace("." + host, string.Empty).Replace(":" + port, string.Empty);
            }
            else if (DnsAliases.ContainsKey(headersHost.ToLower().Replace(":" + port, string.Empty)))
            {
                return DnsAliases[headersHost.ToLower().Replace(":" + port, string.Empty)];
            }
            else
                return null;
        }

        public Map DuradosMap
        {
            get
            {
                return duradosMap;
            }
        }

        public Map GetMap()
        {
            if (!multiTenancy)
            {
                if (this.map == null)
                {
                    this.map = new Map();
                    this.map.Initiate(false);
                }

                return this.map;
            }

            Map m = GetMap(GetAppName());

            if (System.Web.HttpContext.Current != null)
            {
                System.Web.HttpContext.Current.Items[Database.AppId] = m.Id;
            }

            return m;
        }

        string prevAppName = null;

        public string GetAppName()
        {
            try
            {
                if (System.Web.HttpContext.Current == null)
                {
                    return null;
                }

                //int l = System.Web.HttpContext.Current.Request.Url.Segments.Length;

                //if (l > 3)
                //{

                //    if (System.Web.HttpContext.Current.Items.Contains("xxxzzzzzzzzz") && System.Web.HttpContext.Current.Request.Url.Segments[l - 2] == "myAppConnection/" && System.Web.HttpContext.Current.Request.HttpMethod == "POST")
                //    {
                //        return System.Web.HttpContext.Current.Request.Url.Segments[l - 1];
                //    }
                //}

                if (System.Web.HttpContext.Current.Items.Contains(Database.AppName))
                {
                    return System.Web.HttpContext.Current.Items[Database.AppName].ToString();
                }

                string logText = "OriginalString: " + System.Web.HttpContext.Current.Request.Url.OriginalString + "; host:" + System.Web.HttpContext.Current.Request.Headers["Host"] + "; Referer: " + System.Web.HttpContext.Current.Request.Headers["Referer"];
                string appName = GetCurrentAppName();
                if (appName == null || prevAppName == null || !appName.Equals(prevAppName))
                    DuradosMap.Logger.Log("Maps", "GetAppName", appName ?? string.Empty, null, 170, logText);
                prevAppName = appName;
                return appName;
            }
            catch (Exception exception)
            {
                DuradosMap.Logger.Log("Maps", "GetMap", "GetAppName", exception, 5, "");
                return null;
            }
        }

        private Map map = null;

        private View GetAppView()
        {
            return (View)duradosMap.Database.Views["durados_App"];
        }

        /***Return - Plugin Type (Id) or 0 if value wasn't set or exist*/
        private int GetPluginType(int appId)
        {
            SqlAccess sql = new SqlAccess();

            string sSqlCommand = "SELECT     dbo.durados_PlugInSite.PlugInId ";
            sSqlCommand += "from  dbo.durados_PlugInSite with(nolock), dbo.durados_PlugInSiteApp with(nolock) ";
            sSqlCommand += "where dbo.durados_PlugInSite.Id = dbo.durados_PlugInSiteApp.PlugInSiteId ";
            sSqlCommand += " and  dbo.durados_PlugInSiteApp.AppId = " + appId + " ";

            object scalar = sql.ExecuteScalar(duradosMap.connectionString, sSqlCommand);

            if (scalar.Equals(string.Empty) || scalar == null || scalar == DBNull.Value)
                return 0;
            else
                return Convert.ToInt32(scalar);
        }

        public int GetPluginSiteId(int appId)
        {
            SqlAccess sql = new SqlAccess();

            string sSqlCommand = "SELECT     dbo.durados_PlugInSiteApp.PlugInSiteId ";
            sSqlCommand += "from  dbo.durados_PlugInSiteApp with(nolock) ";
            sSqlCommand += " where  dbo.durados_PlugInSiteApp.AppId = " + appId + " ";

            object scalar = sql.ExecuteScalar(duradosMap.connectionString, sSqlCommand);

            if (scalar.Equals(string.Empty) || scalar == null || scalar == DBNull.Value)
                return 0;
            else
                return Convert.ToInt32(scalar);
        }
        private View GetDnsAliasView()
        {
            return (View)duradosMap.Database.Views["durados_DnsAlias"];
        }


        public void Rename(string oldAppName, string newAppName)
        {
            Map map = null;
            if (mapsCache.ContainsKey(oldAppName))
            {
                map = mapsCache[oldAppName];
                RemoveMap(oldAppName);
            }
            else if (mapsCache.ContainsKey(oldAppName.ToLower()))
            {
                map = mapsCache[oldAppName.ToLower()];
                RemoveMap(oldAppName.ToLower());
            }
            if (map != null)
            {
                map.AppName = newAppName;
                AddMap(newAppName, map);
            }
        }

        public void Restart(string pk)
        {
            //try
            //{

            //    //string filename = GetMap().ConfigFileName;
            //    string key = "duradosappsys";//GetStorageBlobName(filename);
            //    int? id = AppExists(pk);
            //    if (id.HasValue)
            //        key += id.Value;
            //    else
            //    {
            //        id = AppExists(pk.ToLower());
            //        if (id.HasValue)
            //            key += id.Value;
            //    }
            //    if (id.HasValue)
            //    {
            //        StorageCache.Remove(key);
            //        key = key + "xml";
            //        StorageCache.Remove(key);
            //    }
            //}
            //catch { }

            mapsCache.Remove(pk);
            mapsCache.Remove(pk.ToLower());

            try
            {
                RemoveSqlProduct(pk);
            }
            catch { }
            //GetMap(pk);
        }

        public void Delete(string pk)
        {
            FarmCachingSingeltone.Instance.ClearMachinesCache(pk);

            if (mapsCache.ContainsKey(pk))
            {
                RemoveMap(pk);
            }
        }

        private static AppLockerGetter lockerPerApp = new AppLockerGetter("mapCreation");

        public Map GetMap(string appName)
        {
            if (appName == null || ReservedAppNames.Contains(appName))
            {
                return duradosMap;
            }

            if (appName == DuradosAppName)
            {
                return duradosMap;
            }

            Map map = null;

            if (IsInMemoryMode())
            {
                if (mapsCache.ContainsKey(appName + GetInMemoryKey()))
                {
                    map = mapsCache[appName + GetInMemoryKey()];
                }
            }
            else if (mapsCache.ContainsKey(appName))
            {
                map = mapsCache[appName];
            }


            //else if (maps.ContainsKey(appName.ToLower()))
            //{
            //    map = maps[appName.ToLower()];
            //}

            // map not in cache create one
            if (map == null)
            {
                bool newStructure = false;
                try
                {
                    lock (lockerPerApp.GetLock(appName))
                    {
                        map = CreateMap(appName, out newStructure);
                    }
                }
                catch (Exception exception)
                {
                    
                    Maps.Instance.duradosMap.Logger.Log("Map", "GetMap", "", exception, 1, (exception.InnerException == null?"App name:" + Maps.GetCurrentAppName(): exception.InnerException.Message));
                    throw new AppNotReadyException(appName);
                }

                // app not exist
                if (map == null)
                {
                    return null;
                }

                //todo: check null return

                if (!newStructure)
                {
                    if (IsInMemoryMode())
                    {
                        AddMap(appName + GetInMemoryKey(), map);
                    }
                    else
                    {
                        AddMap(appName, map);
                    }
                }
            }

            return map;
        }

        public static bool IsApi2()
        {
            string s = System.Configuration.ConfigurationManager.AppSettings["GoogleClientId"];
            return !string.IsNullOrEmpty(s);
        }

        public bool IsApi()
        {
            string s = System.Configuration.ConfigurationManager.AppSettings["GoogleClientId"];
            return !string.IsNullOrEmpty(s);
        }

        private bool BlobExists(string appName)
        {
            int? appId = AppExists(appName);
            if (!appId.HasValue)
                return false;

            if (Maps.Instance.AppInCach(appName))
                return true;

            bool blobExists = (new Durados.Web.Mvc.Azure.DuradosStorage()).Exists(Maps.GetConfigPath(Maps.DuradosAppPrefix + appId.ToString() + ".xml"));

            if (!blobExists)
                return false;

            return true;
        }

        private Map GetMapFromSession(Map map)
        {

            //if (IsPreviewModeOff)
            //{
            //    SetPreviewModeOff();
            //}
            /*
            else
            {
                if (IsPreviewModeOn && HttpContext.Current.Session["AdminPreviewMap"] == null)
                {
                    Map mapCopy = null;

                    DateTime started = DateTime.Now;
                    mapCopy = Maps.Instance.CreateMap(map.AppName);
                    TimeSpan span = DateTime.Now.Subtract(started);
                    double ms = span.TotalMilliseconds;

                    //started = DateTime.Now;
                    //mapCopy = GenericCopier<Map>.DeepCopy(map);
                    //span = DateTime.Now.Subtract(started);
                    //ms = span.TotalMilliseconds;

                    HttpContext.Current.Session["AdminPreviewMap"] = mapCopy;
                }
                if (HttpContext.Current.Session["AdminPreviewMap"] != null)
                {
                    map = (Map)HttpContext.Current.Session["AdminPreviewMap"];

                }
            }
            */
            return map;
        }

        //private void SetPreviewModeOff()
        //{
        //    //if (HttpContext.Current.Session == null) return;

        //    //if (HttpContext.Current.Session["AdminPreviewMap"] != null)// out of preview mode - remove Map from session
        //    //{
        //    //    HttpContext.Current.Session["AdminPreviewMap"] = null;
        //    //}
        //}

        //public bool IsPreviewModeOff
        //{
        //    get
        //    {
        //        string actionName = GetActionName();
        //        return string.IsNullOrEmpty(actionName) || actionName == "IndexPage" || actionName == "InlineEditingEdit" || actionName == "PreviewModeOff" || actionName == "Edit";

        //    }
        //}

        //public bool IsPreviewModeOn
        //{
        //    get
        //    {
        //        string actionName = GetActionName();
        //        return !string.IsNullOrEmpty(actionName) && actionName == "PreviewEdit";

        //    }
        //}


        public HttpContextBase httpContext
        {
            get
            {
                HttpContextWrapper context =
                    new HttpContextWrapper(System.Web.HttpContext.Current);
                return (HttpContextBase)context;
            }
        }

        public string GetActionName()
        {
            string url = HttpContext.Current.Request.RawUrl;
            System.Web.Routing.RouteData route = System.Web.Routing.RouteTable.Routes.GetRouteData(httpContext);
            if (route == null)
                return string.Empty;
            System.Web.Mvc.UrlHelper urlHelper = new System.Web.Mvc.UrlHelper(new System.Web.Routing.RequestContext(httpContext, route));

            var routeValueDictionary = urlHelper.RequestContext.RouteData.Values;

            if (!routeValueDictionary.ContainsKey("action"))
                return string.Empty;

            string actionName = routeValueDictionary["action"].ToString();
            return actionName;
        }

        PortManager portManager = new PortManager(Convert.ToInt32(System.Configuration.ConfigurationManager.AppSettings["startSshTunnelPort"] ?? "10000"), Convert.ToInt32(System.Configuration.ConfigurationManager.AppSettings["endSshTunnelPort"] ?? "63000"));

        private Map CreateMap(string appName, out bool newStructure)
        {
            //Durados.Diagnostics.EventViewer.WriteEvent("Start CreateMap for: " + appName);

            newStructure = false;
            View appView = GetAppView();
            Field idField = appView.Fields["Id"];

            int? id = AppExists(appName, null);

            if (!id.HasValue)
            {
                Durados.Diagnostics.EventViewer.WriteEvent("CreateMap: could not find is for: " + appName);
                return null;
            }

            // if you are here, your application exist but don't have already created map
            MapDataSet.durados_AppRow appRow = null;
            try
            {
                appRow = (MapDataSet.durados_AppRow)appView.GetDataRow(idField, id.Value.ToString(), false);
            }
            catch (Exception exception)
            {
                Durados.Diagnostics.EventViewer.WriteEvent("failed to GetDataRow for id: " + id.Value, exception);
                try
                {
                    ((DuradosMap)Maps.Instance.DuradosMap).AddSslAndAahKeyColumn();
                    appRow = (MapDataSet.durados_AppRow)appView.GetDataRow(idField, id.Value.ToString(), false);
                }
                catch (Exception exception2)
                {
                    //Durados.Diagnostics.EventViewer.WriteEvent(exception2);
                }
            }

            if (appRow == null)
            {
                return null;
            }

            //Durados.Diagnostics.EventViewer.WriteEvent("appRow found for: " + appName + " id: " + id, System.Diagnostics.EventLogEntryType.SuccessAudit, 500);

            Map map = new Map();

            map.PlugInId = GetPluginType((int)id);/***Return - Plugin Type (Id) or 0 if value wasn't set or exist*/

            //map.connectionString = persistency.GetConnection(appRow, builder).ToString();

            int sqlProduct = 1;
            int systemSqlProduct = 1;
            if (appRow.durados_SqlConnectionRowByFK_durados_App_durados_SqlConnection == null)
                throw new DuradosException("The app " + appName + " is not connected. Please connect your app.");

            if (!appRow.durados_SqlConnectionRowByFK_durados_App_durados_SqlConnection.IsSqlProductIdNull())
                sqlProduct = appRow.durados_SqlConnectionRowByFK_durados_App_durados_SqlConnection.SqlProductId;

            if (!appRow.durados_SqlConnectionRowByFK_durados_App_durados_SqlConnection_System.IsSqlProductIdNull())
                systemSqlProduct = appRow.durados_SqlConnectionRowByFK_durados_App_durados_SqlConnection_System.SqlProductId;

            map.SqlProduct = (SqlProduct)sqlProduct;
            map.SystemSqlProduct = (SqlProduct)systemSqlProduct;
            if (appsSqlProducts.ContainsKey(appName))
            {
                appsSqlProducts.Remove(appName);
            }
            try
            {
                appsSqlProducts.Add(appName, (SqlProduct)sqlProduct);
            }
            catch { }

            int localPort = 0;
            if (sqlProduct == (int)SqlProduct.MySql)
            {
                if (appRow.durados_SqlConnectionRowByFK_durados_App_durados_SqlConnection.IsSshUsesNull() || !appRow.durados_SqlConnectionRowByFK_durados_App_durados_SqlConnection.SshUses)
                    localPort = 3306;
                else
                    localPort = AssignLocalPort();
            }
            else if (sqlProduct == (int)SqlProduct.Postgre)
            {
                localPort = Convert.ToInt32(appRow.durados_SqlConnectionRowByFK_durados_App_durados_SqlConnection.ProductPort ?? "5432");
            }
            else if (sqlProduct == (int)SqlProduct.Oracle)
            {
                localPort = Convert.ToInt32(appRow.durados_SqlConnectionRowByFK_durados_App_durados_SqlConnection.ProductPort ?? "1521");
            }

            map.LocalPort = localPort;

            if (sqlProduct == 3)
            {
                map.connectionString = persistency.GetMySqlConnection(appRow, builder, localPort).ToString();
            }
            else if (sqlProduct == 4)
            {
                map.connectionString = persistency.GetPostgreConnection(appRow, builder, localPort).ToString();
            }
            else if (sqlProduct == 5)
            {
                map.connectionString = persistency.GetOracleConnection(appRow, builder, localPort).ToString();
            }
            else
            {
                map.connectionString = persistency.GetSqlServerConnection(appRow, builder).ToString();
            }
            map.systemConnectionString = persistency.GetSystemConnection(appRow, builder).ToString();
            map.securityConnectionString = Convert.ToString(persistency.GetSecurityConnection(appRow, builder));
            map.Logger.RedisProvider = SharedMemorySingeltone.Instance;

            map.Logger.ConnectionString = persistency.GetLogConnection(appRow, builder).ToString();
            string pk = appRow.Id.ToString();
            map.Id = pk;
            map.DatabaseName = appRow != null && appRow.durados_SqlConnectionRowByFK_durados_App_durados_SqlConnection != null && !appRow.durados_SqlConnectionRowByFK_durados_App_durados_SqlConnection.IsCatalogNull() ? appRow.durados_SqlConnectionRowByFK_durados_App_durados_SqlConnection.Catalog : "Yours";

            map.ConfigFileName = Maps.GetConfigPath(Maps.DuradosAppPrefix + pk + ".xml");
            if (!appRow.IsUsesSpecificBinaryNull() && appRow.UsesSpecificBinary)
                // map.selectedProject = string.Format("Durados.Web.Mvc.Specifics.{0}.{0}Project, Durados.Web.Mvc.Specifics.{0}, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null", Maps.DuradosAppPrefix + pk);
                map.selectedProject = HttpContext.Current.Server.MapPath("/" + appRow.SpecificDOTNET);

            //Durados.Diagnostics.EventViewer.WriteEvent("connections set for: " + appName + " id: " + id);

            map.SiteInfo = new SiteInfo();
            if (appRow.IsTitleNull())
                map.SiteInfo.Product = string.Empty;
            else
                map.SiteInfo.Product = appRow.Title;

            try
            {
                map.SiteInfo.Logo = appRow.Image;
            }
            catch { }


            map.UsingSsh = !appRow.durados_SqlConnectionRowByFK_durados_App_durados_SqlConnection.IsSshUsesNull() && appRow.durados_SqlConnectionRowByFK_durados_App_durados_SqlConnection.SshUses;
            if (sqlProduct == (int)SqlProduct.MySql && map.UsingSsh)
            {
                Durados.Security.Ssh.ITunnel tunnel = new Durados.DataAccess.Ssh.Tunnel();
                tunnel.RemoteHost = appRow.durados_SqlConnectionRowByFK_durados_App_durados_SqlConnection.SshRemoteHost;
                tunnel.User = appRow.durados_SqlConnectionRowByFK_durados_App_durados_SqlConnection.SshUser;
                tunnel.Password = appRow.durados_SqlConnectionRowByFK_durados_App_durados_SqlConnection.IsSshPasswordNull() ? null : appRow.durados_SqlConnectionRowByFK_durados_App_durados_SqlConnection.SshPassword;
                tunnel.Port = appRow.durados_SqlConnectionRowByFK_durados_App_durados_SqlConnection.SshPort;
                tunnel.PrivateKey = appRow.durados_SqlConnectionRowByFK_durados_App_durados_SqlConnection.IsSshPrivateKeyNull() ? null : appRow.durados_SqlConnectionRowByFK_durados_App_durados_SqlConnection.SshPrivateKey;

                map.SshSession = new Durados.DataAccess.Ssh.ChilkatSession(tunnel, Convert.ToInt32(appRow.durados_SqlConnectionRowByFK_durados_App_durados_SqlConnection.ProductPort), localPort);
                ////map.SshSession = new Durados.DataAccess.Ssh.TamirSession(tunnel, 3306);
                map.OpenSshSession();
            }

            bool firstTime = map.Initiate(false);
            ConfigAccess.Refresh(map.GetConfigDatabase().ConnectionString);

            map.AppName = appName;
            map.Url = GetAppUrl(appName);
            map.SiteInfo.LogoHref = map.Url;
            map.Guid = appRow.Guid;
            map.CreatorId = appRow.Creator;
            map.PaymentLocked = appRow.PaymentLocked;
            map.PaymentStatus = (Billing.PaymentStatus)Enum.ToObject(typeof(Billing.PaymentStatus), appRow.PaymentStatus);
            map.AnonymousToken = appRow.AnonymousToken;
            map.SignUpToken = appRow.SignUpToken;
            map.LoadLimits();

            int themeId = 0;
            string themeName = "";
            string themePath = "";

            MapDataSet.durados_ThemeRow themeRow = appRow.durados_ThemeRow ?? GetDefaultTheme();

            themeId = themeRow.Id;
            themeName = themeRow.Name;
            if (themeRow.Id == CustomTheme)
            {
                themePath = appRow.IsCustomThemePathNull() ? string.Empty : appRow.CustomThemePath;
            }
            else
            {
                themePath = themeRow.RelativePath;
            }

            map.Theme = new Theme() { Id = themeId, Name = themeName, Path = themePath };
            if (firstTime && Convert.ToBoolean(System.Configuration.ConfigurationManager.AppSettings["NotifyUserOnConsoleReady"] ?? "true"))
                map.NotifyUser("consoleFirstTimeSubject", "consoleFirstTimeMessage");

            RefreshMapDnsAlias(map);
            UpdatePlan(appRow.Id, map);

            newStructure = map.SaveChangesInConfigStructure();

            try
            {
                map.MenusBackwardCompatebility();
            }
            catch (Exception exception)
            {
                DuradosMap.Logger.Log("Map", "MenusBackwardCompatebility", appName ?? string.Empty, exception, 1, "Backward Compatebility Failed");
            }

            try
            {
                map.ChartsBackwardCompatebility();
            }
            catch (Exception exception)
            {
                DuradosMap.Logger.Log("Map", "ChartsBackwardCompatebility", appName ?? string.Empty, exception, 1, "Charts Backward Compatebility Failed");
            }

            return map;
        }

        private MapDataSet.durados_ThemeRow defaultThemeRow = null;

        public const int DefaultThemeId = 2;
        public const int CustomTheme = 1;

        private MapDataSet.durados_ThemeRow GetDefaultTheme()
        {

            if (defaultThemeRow == null)
                defaultThemeRow = GetTheme(DefaultThemeId);

            return defaultThemeRow;
        }

        public string GetAppThemePath(string appName)
        {
            if (string.IsNullOrEmpty(appName))
                return GetTheme().RelativePath;

            if (AppInCach(appName))
            {
                return this.GetMap(appName).Theme.Path;
            }

            int? themeId = null;
            string sql = "select ThemeId from durados_App where [Name]=@appName";
            try
            {

                string scalar = (new SqlAccess()).ExecuteScalar(duradosMap.connectionString, sql, new Dictionary<string, object>() { { "appName", appName } });
                if (!string.IsNullOrEmpty(scalar))
                    themeId = Convert.ToInt32(scalar);
            }
            catch
            {

            }
            if (themeId.Equals(CustomTheme))
            {
                sql = "select CustomThemePath from durados_App where [Name]=@appName";
                try
                {

                    string scalar = (new SqlAccess()).ExecuteScalar(duradosMap.connectionString, sql, new Dictionary<string, object>() { { "appName", appName } });
                    if (!string.IsNullOrEmpty(scalar))
                        return scalar;
                    else
                        return GetTheme(themeId).RelativePath;
                }
                catch
                {
                    return GetTheme(themeId).RelativePath;
                }
            }
            else
            {
                return GetTheme(themeId).RelativePath;
            }
        }

        public MapDataSet.durados_ThemeRow GetTheme(int? themeId = DefaultThemeId)
        {
            if (!themeId.HasValue)
                themeId = DefaultThemeId;
            return (MapDataSet.durados_ThemeRow)duradosMap.Database.Views["durados_Theme"].GetDataRow(themeId.ToString());
        }

        public int AssignLocalPort()
        {
            return portManager.Assign();
        }

        public void UpdatePlan(int appId, Map map)
        {
            string sql = "select top(1) PlanId from durados_AppPlan where AppId=" + appId + " order by PurchaseDate desc";
            try
            {
                string scalar = (new SqlAccess()).ExecuteScalar(duradosMap.connectionString, sql);
                if (string.IsNullOrEmpty(scalar))
                    map.Plan = 0;
                else
                    map.Plan = Convert.ToInt32(scalar);
            }
            catch
            {
                map.Plan = 0;
            }

        }

        private void RefreshMapDnsAlias(Map map)
        {
            int rowCount = 0;
            Dictionary<string, object> values = new Dictionary<string, object>();

            View dnsAliasView = GetDnsAliasView();

            DataView dnsAliasDataView = dnsAliasView.FillPage(1, 10000, values, null, null, out rowCount, null, null);
            map.Aliases.Clear();
            foreach (System.Data.DataRowView dnsAliasRow in dnsAliasDataView)
            {
                map.Aliases.Add(dnsAliasRow["Alias"].ToString());
            }
        }

        public bool AppInCach(string appName)
        {
            return mapsCache.ContainsKey(appName);
        }

        public string GetAppNameByGuid(string guid)
        {
            SqlAccess sql = new SqlAccess();
            string sSqlCommand = "";

            Guid parsedGuid;
            if (!Guid.TryParse(guid, out parsedGuid))
            {
                throw new ArgumentException("Illegal token");
            }

            sSqlCommand = "select [name] from durados_App with(nolock) where [Guid] = '" + guid + "'";

            object scalar = sql.ExecuteScalar(duradosMap.connectionString, sSqlCommand);

            if (scalar.Equals(string.Empty) || scalar == null || scalar == DBNull.Value)
                return null;
            else
                return scalar.ToString();
        }

        public bool AppLocked(string appName)
        {
            return PaymentStatus(appName) == Billing.PaymentStatus.Suspended;
        }

        public Billing.PaymentStatus PaymentStatus(string appName)
        {
            if (AppInCach(appName))
            {
                Billing.PaymentStatus paymentStatus = GetMap(appName).PaymentStatus;
                if (paymentStatus == Billing.PaymentStatus.Active)
                {
                    return Billing.PaymentStatus.Active;
                }
            }
            try
            {
                SqlAccess sql = new SqlAccess();
                string sSqlCommand = "select PaymentStatus from durados_App with(nolock) where Name = N'" + appName + "'";

                object scalar = sql.ExecuteScalar(duradosMap.connectionString, sSqlCommand);

                if (scalar.Equals(string.Empty) || scalar == null || scalar == DBNull.Value)
                    return Billing.PaymentStatus.Active;
                else
                    return (Billing.PaymentStatus)Enum.ToObject(typeof(Billing.PaymentStatus), Convert.ToInt32(scalar));
            }
            catch
            {
                return Billing.PaymentStatus.Active;
            }
        }


        public int? AppExists(string appName, int? userId = null, bool ignoreDevUser = false)
        {
            SqlAccess sql = new SqlAccess();
            string sSqlCommand = "";

            if (!userId.HasValue || (IsDevUser() && !ignoreDevUser))
            {
                sSqlCommand = "select Id from durados_App with(nolock) where Name = N'" + appName + "'";
            }
            else
            {
                sSqlCommand = "SELECT dbo.durados_App.Id FROM dbo.durados_App with(nolock), dbo.durados_UserApp with(nolock) where (dbo.durados_App.Name = N'" + appName + "' and ((dbo.durados_UserApp.UserId=" + userId + " and dbo.durados_UserApp.AppId = dbo.durados_App.Id) or dbo.durados_App.Creator=" + userId + ") ) group by(dbo.durados_App.Id)";
                /*"SELECT dbo.durados_App.Id FROM dbo.durados_App with(nolock) INNER JOIN dbo.durados_UserApp with(nolock) ON dbo.durados_App.Id = dbo.durados_UserApp.AppId WHERE (dbo.durados_App.Name = N'" + appName + "' and dbo.durados_UserApp.UserId = "+userId+")";*/
            }

            object scalar = sql.ExecuteScalar(duradosMap.connectionString, sSqlCommand);

            if (scalar.Equals(string.Empty) || scalar == null || scalar == DBNull.Value)
                return null;
            else
                return Convert.ToInt32(scalar);
        }

        public MapDataSet.durados_AppRow GetAppRow(int id)
        {
            View appView = GetAppView();
            Field idField = appView.Fields["Id"];
            return (MapDataSet.durados_AppRow)appView.GetDataRow(idField, id.ToString(), false);

        }

        public static string GetAppUrl(string appName, bool? fullUrl = null)
        {
            string port = System.Web.HttpContext.Current.Request.Url.Port.ToString();
            string appHost = host;
            if (System.Web.HttpContext.Current.Request.Url.ToString().Contains(port))
                appHost += ":" + port;

            //Prepare url format
            string urlFormat = System.Web.HttpContext.Current.Request.Url.Scheme + "://{0}.{1}";
            if (fullUrl == true)
            {
                urlFormat = "{0}.{1}|_blank|" + urlFormat;
            }

            string url = string.Format(urlFormat, appName, appHost);
            return url;
        }

        public static string GetMainAppUrl()
        {
            return GetAppUrl(duradosAppName);
        }

        public static string GetmainAppConfigName()
        {
            return mainAppConfigName;
        }

        public MapDataSet.v_durados_UserRow GetCreatorUserRow()
        {
            return GetCreatorUserRow(GetAppName());
        }

        public MapDataSet.durados_AppRow GetAppRow()
        {
            return GetAppRow(GetAppName());

        }

        public MapDataSet.durados_AppRow GetAppRow(string appName)
        {
            View appView = GetAppView();
            Field nameField = appView.Fields["Name"];

            return (MapDataSet.durados_AppRow)appView.GetDataRow(nameField, appName, false);
        }

        public MapDataSet.v_durados_UserRow GetCreatorUserRow(string appName)
        {
            MapDataSet.durados_AppRow appRow = GetAppRow(appName);
            return appRow.v_durados_UserRow;
        }

        public void AddMap(string appName, Map map)
        {
            if (!mapsCache.ContainsKey(appName))
                mapsCache.Add(appName, map);

        }

        public void UpdateCache(string appName, Map map)
        {
            mapsCache.Add(appName, map);

        }

        public void RemoveMap(string appName)
        {
            mapsCache.Remove(appName);
        }
        private void RemoveConfig(string filename)//DataSet ds,
        {
            if (Maps.Cloud)
            {
                try
                {
                    DeleteConfig(filename);
                }
                catch (Exception exception)
                {
                    Maps.Instance.duradosMap.Logger.Log("Map", "RemoveApp", "RemoveConfig", exception, 1, "App name:" + Maps.GetCurrentAppName() + ", File name: " + filename);
                }

                return;
            }
            //if (!File.Exists(filename))
            //{
            //    try
            //    {
            //        DeleteConfig( filename);
            //    }
            //    catch (Exception exception)
            //    {
            //        Maps.Instance.duradosMap.Logger.Log("Map", "RemoveApp", "RemoveConfig", exception, 1, "App name:" + Maps.GetCurrentAppName() + ", File name: " + filename);
            //    }
            //}
        }

        private void DeleteConfig(string filename)
        {
            if (Maps.Cloud)
            {
                RemoveConfigFromCloud(filename);

            }
            else
            {
                if (File.Exists(filename))
                    File.Delete(filename);
            }
        }
        public void RemoveConfigFromCloud(string filename)
        {
            string containerName = Maps.GetStorageBlobName(filename);

            CloudBlobContainer container = new Azure.DuradosStorage().GetContainer(containerName);
            if (Convert.ToBoolean(System.Configuration.ConfigurationManager.AppSettings["deleteFullContainerOndelete"] ?? "true"))
                container.Delete();
            else
            {
                //if (container.GetBlockBlobReference(filename).Exists())
                //    container.GetBlobReference(filename).DeleteIfExists();
                if (container.GetBlockBlobReference(containerName).Exists())
                    container.GetBlobReference(containerName).DeleteIfExists();
            }
            if (Maps.Instance.StorageCache.ContainsKey(containerName))
            {
                Maps.Instance.StorageCache.Remove(containerName);
            }
        }


        internal void ChangeName(string oldName, string newName)
        {
            if (mapsCache.ContainsKey(oldName))
            {
                Map map = mapsCache[oldName];
                if (mapsCache.ContainsKey(newName))
                    throw new DuradosException("The " + newName + " already exists in the dictionary.");
                AddMap(newName, map);
                RemoveMap(oldName);
            }

        }

        public int GetAppAcount()
        {
            int appCount = 4010;
            if (HttpContext.Current.Session["AppAcount"] != null)
                return Convert.ToInt16(HttpContext.Current.Session["AppAcount"]);
            try
            {
                using (SqlConnection connection = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["MapsConnectionString"].ConnectionString))
                {
                    connection.Open();
                    string sql = "SELECT count(*) FROM dbo.durados_App a with(nolock) INNER JOIN dbo.durados_PlugInInstance p with(nolock) ON a.id = p.Appid WHERE Deleted =0 and p.selected=1";
                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        object scalar = command.ExecuteScalar();
                        appCount = Convert.ToInt16(scalar);
                        HttpContext.Current.Session["AppAcount"] = appCount;
                    }

                }
            }
            catch { }

            return appCount;

        }
        public string GetCurrentAppId()
        {
            return AppExists(GetCurrentAppName()).ToString();
        }

        public int? GetConnection(string server, string catalog, string username, string userId)
        {
            SqlAccess sql = new SqlAccess();

            object scalar = sql.ExecuteScalar(duradosMap.connectionString, string.Format("select Id from durados_SqlConnection where ServerName=N'{0}' and Catalog=N'{1}' and Username=N'{2}' and DuradosUser={3}", server, catalog, username, userId));

            if (string.Empty.Equals(scalar) || scalar == null || scalar == DBNull.Value)
                return null;
            else
                return Convert.ToInt32(scalar);
        }
        public static bool IsAlloweDownload(string virtualPath)
        {
            string extension = VirtualPathUtility.GetExtension(virtualPath).TrimStart('.');
            if (DownloadDenyPolicy)
            {
                return !DenyDownloadFileTypes.Contains(extension, StringComparer.OrdinalIgnoreCase);
            }
            else
            {
                return AllowedDownloadFileTypes.Contains(extension, StringComparer.OrdinalIgnoreCase);
            }

        }
        public Dictionary<PlugInType, PluginCache> PluginsCache { get; private set; }

        public static SqlProduct GetCurrentSqlProduct()
        {
            return GetSqlProduct(GetCurrentAppName());
        }

        //private static Dictionary<string, SqlProduct> appsSqlProducts = new Dictionary<string, SqlProduct>();
        private static Durados.Data.ICache<SqlProduct> appsSqlProducts = CacheFactory.CreateCache<SqlProduct>("SqlProduct");

        public static void RemoveSqlProduct(string appName)
        {
            if (appsSqlProducts.ContainsKey(appName))
                appsSqlProducts.Remove(appName);
        }

        public static void UpdateSqlProduct(string appName, SqlProduct sqlProduct)
        {
            RemoveSqlProduct(appName);
            appsSqlProducts.Add(appName, sqlProduct);
        }

        public static SqlProduct GetSqlProduct(string appName)
        {
            if (string.IsNullOrEmpty(appName) || appName == duradosAppName)
                return SqlProduct.SqlServer;

            if (!appsSqlProducts.ContainsKey(appName))
            {
                using (SqlConnection connection = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["MapsConnectionString"].ConnectionString))
                {
                    connection.Open();
                    string sql = "SELECT dbo.durados_SqlConnection.SqlProductId FROM dbo.durados_App with(nolock) INNER JOIN dbo.durados_SqlConnection with(nolock) ON dbo.durados_App.SqlConnectionId = dbo.durados_SqlConnection.Id WHERE (dbo.durados_App.Name = @AppName)";
                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@AppName", appName);
                        object scalar = command.ExecuteScalar();
                        if (scalar == null || scalar == DBNull.Value)
                        {
                            return SqlProduct.SqlServer;
                        }

                        appsSqlProducts.Add(appName, (SqlProduct)scalar);
                    }
                }
            }

            return appsSqlProducts[appName];
        }

        public static string GetStorageBlobName(string filename)
        {
            System.IO.FileInfo fileInfo = new FileInfo(filename);
            string filenameOnly = fileInfo.Name.Remove(fileInfo.Name.Length - fileInfo.Extension.Length, fileInfo.Extension.Length);

            return filenameOnly.Replace("_", "").Replace(".", "").ToLower();

        }

        Durados.Data.ICache<DataSet> storageCache = new CacheWithStatus(CacheFactory.CreateCache<DataSet>("storageCache"));

        public Durados.Data.ICache<DataSet> StorageCache
        {
            get
            {
                return storageCache;
            }
        }

        BlobBackup backup = new BlobBackup();
        public BlobBackup Backup
        {
            get
            {
                return backup;
            }
        }

        FieldProperty fieldProperty = new FieldProperty();
        public FieldProperty FieldProperty
        {
            get
            {
                return fieldProperty;
            }
        }

        public static HashSet<string> ReservedAppNames { get; set; }

        public void UpdateOnBoardingStatus(OnBoardingStatus onBoardingStatus, string appId)
        {
            string sql = "Update durados_App set DatabaseStatus = " + (int)onBoardingStatus + " where id = " + appId;
            Durados.DataAccess.SqlAccess sqlAccess = new Durados.DataAccess.SqlAccess();

            sqlAccess.ExecuteNonQuery(DuradosMap.connectionString, sql);
        }

        public OnBoardingStatus GetOnBoardingStatus(string appId)
        {
            string sql = "select DatabaseStatus from dbo.durados_App with (NOLOCK) where id = " + appId;
            Durados.DataAccess.SqlAccess sqlAccess = new Durados.DataAccess.SqlAccess();

            object scalar = sqlAccess.ExecuteScalar(DuradosMap.connectionString, sql);

            if (scalar.Equals(string.Empty) || scalar == null || scalar == DBNull.Value)
                return OnBoardingStatus.NotStarted;
            else
                return (OnBoardingStatus)Convert.ToInt32(scalar);
        }

        public string GetAppNameById(int appId)
        {
            string sql = "select Name from dbo.durados_App with (NOLOCK) where id = " + appId;
            Durados.DataAccess.SqlAccess sqlAccess = new Durados.DataAccess.SqlAccess();

            object scalar = sqlAccess.ExecuteScalar(DuradosMap.connectionString, sql);

            if (scalar.Equals(string.Empty) || scalar == null || scalar == DBNull.Value)
                return null;
            else
                return Convert.ToString(scalar);
        }

        public static bool IsDevUser(string username = null)
        {
            if (username == null)
                username = Instance.DuradosMap.Database.GetCurrentUsername();
            return DevUsers.Contains(username);
        }

        private static EmbeddedReportsConfig embeddedReportsConfig;
        public static Analytics.EmbeddedReportsConfig GetEmbeddedReportsConfig()
        {
            return embeddedReportsConfig;
        }

        private static string GetTextFileContent(string fileName)
        {
            string fileContent = null;
            if (File.Exists(fileName))
            {
                fileContent = File.ReadAllText(fileName);
            }
            else
            {
                throw new System.IO.FileNotFoundException("The file was not found", fileName);
            }
            return fileContent;
        }

        private static string webhookJsonString = null;
        private static string cqlsJsonString = null;
        //private static Dictionary<string, object> webhookJson = null;
        //private static Dictionary<string, object> GetJsonFromFile(string fileName)
        //{
        //    fileName = Path.GetDirectoryName(AppDomain.CurrentDomain.BaseDirectory) +  fileName;
        //    if (webhookJson == null)
        //    {
        //        string json = GetTextFileContent(fileName);
        //        JavaScriptSerializer jss = new JavaScriptSerializer();
        //        webhookJson = (Dictionary<string, object>)jss.Deserialize<dynamic>(json);
        //    }
        //    return webhookJson;
        //}

        private static Dictionary<string, object> GetJsonFromString(string json)
        {
            JavaScriptSerializer jss = new JavaScriptSerializer();
            var  webhookJson = (Dictionary<string, object>)jss.Deserialize<dynamic>(json);
            return webhookJson;
        }

        private static string GetStringFromFile(string fileName, ref string jsonString)
        {
            fileName = Path.GetDirectoryName(AppDomain.CurrentDomain.BaseDirectory) + @"\" + fileName;
            if (jsonString == null)
            {
                jsonString = GetTextFileContent(fileName);
            }
            return jsonString;
        }

        private static string WebhooksParametersFileName = null;
        internal static WebhookParameters GetWebhookParameters(string webhookType)
        {
            if (string.IsNullOrEmpty(WebhooksParametersFileName))
                return null;

            string jsonString = GetStringFromFile(WebhooksParametersFileName, ref webhookJsonString);

            jsonString = ReplaceParameters(jsonString);

            Dictionary<string, object> json = GetJsonFromString(jsonString);
            if (!json.ContainsKey(webhookType))
            {
                return null;
            }

            json = (Dictionary<string, object>)json[webhookType.ToString()];
            WebhookParameters webhookParameters = new WebhookParameters();
            if (json.ContainsKey("Method"))
            {
                webhookParameters.Method = json["Method"].ToString();
            }
            if (json.ContainsKey("Url"))
            {
                webhookParameters.Url = json["Url"].ToString();
            }
            if (json.ContainsKey("Body"))
            {
                webhookParameters.Body = json["Body"];
            }
            if (json.ContainsKey("ErrorHandling"))
            {
                Dictionary<string, object> errorHandlingDic = (Dictionary<string, object>)json["ErrorHandling"];
                WebhookErrorHandling errorHandling = new WebhookErrorHandling();
                if (errorHandlingDic.ContainsKey("Cancel"))
                {
                    errorHandling.Cancel = (bool)errorHandlingDic["Cancel"];
                }
                if (errorHandlingDic.ContainsKey("Message"))
                {
                    errorHandling.Message = (string)errorHandlingDic["Message"];
                }
                webhookParameters.ErrorHandling = errorHandling;
            }
            if (json.ContainsKey("QueryStringParameters"))
            {
                webhookParameters.QueryStringParameters = (Dictionary<string, object>)json["QueryStringParameters"];
            }
            if (json.ContainsKey("Headers"))
            {
                webhookParameters.Headers = (Dictionary<string, object>)json["Headers"];
            }
            if (json.ContainsKey("Async"))
            {
                webhookParameters.Async = (bool)json["Async"];
            }
            if (json.ContainsKey("LimitApps"))
            {
                webhookParameters.LimitApps = (string)json["LimitApps"];
            }

            return webhookParameters;
        }

        private static string AppNamePlaceHolder = "$AppName$";
        private static string AppIdPlaceHolder = "$AppId$";
        private static string UsernamePlaceHolder = "$Username$";
        private static string CreatorPlaceHolder = "$Creator$";

        private static string ReplaceParameters(string jsonString)
        {
            return ReplaceQueryString(ReplaceCreator(ReplaceUsername(ReplaceAppName(ReplaceAppId(jsonString)))));
        }

        private static string ReplaceQueryString(string jsonString)
        {
            foreach (string key in System.Web.HttpContext.Current.Request.QueryString.AllKeys)
            {
                jsonString = jsonString.Replace("$" + key + "$", System.Web.HttpContext.Current.Request.QueryString[key], false);
            }

            return jsonString;
        }

        private static string ReplaceCreator(string jsonString)
        {
            if (!jsonString.Contains(CreatorPlaceHolder))
            {
                return jsonString;
            }

            Map map = instance.GetMap();
            string creator = Instance.DuradosMap.Database.GetCreatorUsername(Convert.ToInt32(map.Id));
            return jsonString.Replace(CreatorPlaceHolder, creator, false);
        }

        private static string ReplaceUsername(string jsonString)
        {
            if (!jsonString.Contains(UsernamePlaceHolder))
            {
                return jsonString;
            }
            string username = instance.GetMap().Database.GetCurrentUsername();
            return jsonString.Replace(UsernamePlaceHolder, username, false);
        }
        
        private static string ReplaceAppName(string jsonString)
        {
            if (!jsonString.Contains(AppNamePlaceHolder))
            {
                return jsonString;
            }
            string appName = instance.GetAppName();
            return jsonString.Replace(AppNamePlaceHolder, appName, false);
        }

        private static string ReplaceAppId(string jsonString)
        {
            if (!jsonString.Contains(AppIdPlaceHolder))
            {
                return jsonString;
            }
            Map map = instance.GetMap();
            string appId = map.Id;
            if (string.IsNullOrEmpty(appId))
                return jsonString;
            return jsonString.Replace(AppIdPlaceHolder, appId, false);
        }


        private static Dictionary<Limits, int> limits = new Dictionary<Limits, int>();
        
        internal static int GetLimit(Limits limit)
        {
            return limits[limit];
        }

        public static string LocalAddress { get; set; }
        public static string SocialRedirectHost { get; set; }

        
    }
}
