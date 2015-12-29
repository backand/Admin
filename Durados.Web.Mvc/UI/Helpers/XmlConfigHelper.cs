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
        public string UploadConfigFromFile(string appname,String filedata)
        {
           
            MemoryStream ms = new MemoryStream(Convert.FromBase64String(filedata));
            Dictionary<string,MemoryStream> configFiles= Infrastructure.General.UnZip(ms);
            string version = Maps.Instance.SaveUploadConfig(appname, configFiles);
            Maps.Instance.Restore(appname,version);
            return null;
        }



        public Stream GetZipConfig(string appname)
        {
            Map map = Maps.Instance.GetMap();
            try
            {
                Dictionary<string, MemoryStream> configStream = new Dictionary<string, MemoryStream>();
                MemoryStream xmlStream = new MemoryStream();
                DataSet ds = new DataSet();
                map.ReadConfigFromCloud(ds, map.ConfigFileName);
                ds.WriteXml(xmlStream, XmlWriteMode.WriteSchema);
                configStream.Add(map.AppName + ".xml", xmlStream);

                MemoryStream xmlxmlStream = new MemoryStream();
                ds = new DataSet();
                map.ReadConfigFromCloud(ds, map.ConfigFileName + ".xml");
                ds.WriteXml(xmlxmlStream, XmlWriteMode.WriteSchema);
                configStream.Add(map.AppName + ".xml.xml", xmlxmlStream);


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




        //public JsonResult UploadConfig(string fileName)
        //{
        //    try
        //    {
        //        View uploadConfigView = (View)Map.Database.Views["durados_UploadConfig"];
        //        ColumnField fileNameField = (ColumnField)uploadConfigView.Fields["FileName"];
        //        string path = fileNameField.GetUploadPath().TrimEnd("\\".ToArray());
        //        string[] configFiles = Infrastructure.General.UnZip(path + "\\" + fileName, path, true);

        //        if (configFiles.Length != 2)
        //            return Json(new { success = false, message = "The zip file must contain 2 xml files." });

        //        string newSchemaFileName = null;
        //        string newConfigFileName = null;

        //        if (configFiles[0].ToLower().EndsWith(".xml.xml"))
        //        {
        //            newConfigFileName = path + "\\" + configFiles[1];
        //            newSchemaFileName = path + "\\" + configFiles[0];
        //            if (configFiles[1].ToLower().EndsWith(".xml.xml"))
        //                return Json(new { success = false, message = "The zip file must contain only one file that ends with .xml.xml." });
        //        }
        //        else if (configFiles[1].ToLower().EndsWith(".xml.xml"))
        //        {
        //            newConfigFileName = path + "\\" + configFiles[0];
        //            newSchemaFileName = path + "\\" + configFiles[1];
        //        }
        //        else
        //            return Json(new { success = false, message = "The zip file must contain a files that ends with .xml.xml." });


        //        string id = Maps.Instance.GetCurrentAppId();
        //        string oldConfigFileName = Maps.GetConfigPath(string.Format("durados_AppSys_{0}.xml", id));
        //        string oldSchemaFileName = oldConfigFileName + ".xml";

        //        System.IO.File.Copy(newConfigFileName, oldConfigFileName, true);
        //        System.IO.File.Copy(newSchemaFileName, oldSchemaFileName, true);

        //        if (Maps.Cloud)
        //        {
        //            DataSet ds = new DataSet();
        //            ds.ReadXml(newConfigFileName, XmlReadMode.ReadSchema);
        //            Map.WriteConfigToCloud(ds, newConfigFileName);
        //            try
        //            {
        //                System.IO.File.Delete(oldConfigFileName);
        //                System.IO.File.Delete(newConfigFileName);
        //            }
        //            catch { }
        //            ds = new DataSet();
        //            ds.ReadXml(newSchemaFileName, XmlReadMode.ReadSchema);
        //            Map.WriteConfigToCloud(ds, newSchemaFileName, false);
        //            try
        //            {
        //                System.IO.File.Delete(oldSchemaFileName);
        //                System.IO.File.Delete(newSchemaFileName);
        //            }
        //            catch { }

        //        }
        //        //Map.Initiate();


        //        return Json(new { success = true, message = "The system was updated" });
        //    }
        //    catch (Exception exception)
        //    {
        //        return Json(new { success = false, message = "Failed to update system. Please upload the backed-up configuration.<br><br>Details:<br>" + exception.Message });
        //    }

        //}

        protected virtual string[] GetConfigFiles(Map map)
        {
            string uiConfig = map.ConfigFileName;
            string dbConfig = uiConfig + ".xml";

            return new string[2] { uiConfig, dbConfig };
        }
    }
}
