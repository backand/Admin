using Microsoft.WindowsAzure.StorageClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Durados.Web.Mvc.UI.Helpers
{
    public class XmlConfigHelper
    {
        public string UploadConfigFromFile(string appname, String filedata)
        {

            MemoryStream ms = new MemoryStream(Convert.FromBase64String(filedata));
            Dictionary<string, MemoryStream> configFiles = Infrastructure.General.UnZip(ms);
            Map map = Maps.Instance.GetMap();
            string filename = Maps.DuradosAppPrefix.Replace("_", "").Replace(".", "").ToLower() + map.Id.ToString();
            Durados.Web.Mvc.Azure.BlobBackup bbk = new Durados.Web.Mvc.Azure.BlobBackup();
            string version = bbk.UploadPrefix + DateTime.Now.ToString("yyyyMMddHHmmssfff");

            foreach (var configFile in configFiles)
            {
                DataSet ds = new DataSet();
                MemoryStream configStream = (MemoryStream)ChangeAppIdInConfigFiles(configFile.Value, map.Id);
                ds.ReadXml(configStream);

                filename = filename + (configFile.Key.EndsWith("xml") ? "xml" : string.Empty);
                map.WriteConfigToCloud3(ds, filename, false, map, bbk.VersionPrefix + version);


            }//string version = Maps.Instance.SaveUploadConfig(appname, configFiles);
            Maps.Instance.Restore(appname, version);
            return version;
        }

        public Stream ChangeAppIdInConfigFiles(Stream stream, string newAppId)
        {
            System.Xml.XmlDocument doc = new System.Xml.XmlDocument();
            doc.Load(stream);
            System.Xml.XmlNodeList uploadNodes = doc.SelectNodes("/NewDataSet/Upload/UploadVirtualPath/text()[contains(.,'/Uploads/')]");

            foreach (System.Xml.XmlNode node in uploadNodes)
            {
                node.InnerText = "/Uploads/" + newAppId + "/";
            }

            uploadNodes = doc.SelectNodes("/NewDataSet/Database/UploadFolder/text()[contains(.,'/Uploads/')]");
            foreach (System.Xml.XmlNode node in uploadNodes)
            {
                node.InnerText = "/Uploads/" + newAppId + "/";
            }
            MemoryStream s = new MemoryStream();
            doc.Save(s);
            s.Seek(0, SeekOrigin.Begin);
            return s;
        }

        public Stream GetZipConfig(string appname, string version)
        {
            Map map = Maps.Instance.GetMap();
            try
            {
                Dictionary<string, Stream> configStream = Maps.Instance.GetAllConfigs(map.Id, version);
                Stream compress = Infrastructure.General.Zip(configStream);
                compress.Position = 0;
                return compress;

            }
            catch (Exception exception)
            {
                map.Logger.Log("XMLConfigHelper", "GetConfigFile", exception.Source, exception, 1, null);
                //throw new DuradosException("Failed to download configuration file", exception);
                return null;
            }
        }


        protected virtual string[] GetConfigFiles(Map map)
        {
            string uiConfig = map.ConfigFileName;
            string dbConfig = uiConfig + ".xml";

            return new string[2] { uiConfig, dbConfig };
        }


    }
}