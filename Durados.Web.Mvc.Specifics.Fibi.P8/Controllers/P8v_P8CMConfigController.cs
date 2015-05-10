using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.IO;
using System.Xml;


namespace Durados.Web.Mvc.Specifics.Fibi.P8.Controllers
{
    public class P8v_P8CMConfigController : P8CMBaseController
    {
        protected override Durados.Web.Mvc.Workflow.Engine CreateWorkflowEngine()
        {
            return new BusinessLogic.P8CMEngin();
        }
        protected override string GetXmlDocumentName(string xsd)
        {
            if (string.IsNullOrEmpty(xsd))
            {
                throw new DuradosException("The xml output file name is null!");
            }
            else
            {
                if (!xsd.EndsWith(Durados.Workflow.DocumentType.Xml.ToString(),StringComparison.CurrentCultureIgnoreCase))
                {
                    return base.GetXmlDocumentName(xsd);
                }
                else
                    return xsd;
            }
        }
        protected override void CreateXmlDocument(string newFile, Durados.Workflow.XmlTemplate template, object data, Durados.View view)
        {
        //    if (view != null && !view.Fields.ContainsKey("Active"))
        //    {
        //         throw new DuradosException("Missing Active Config Field.");
        //    }
        //    if(data != null && !(data is  DataRow))
        //    {
        //        throw new DuradosException("Missing data row for xml document file name.");
        //    }

        //    Durados.Field field = view.Fields["Active"];
        //    string activeConfig = field.GetValue((DataRow)data);
           
        //    bool isActiveConfig =false;;
        //    string outputXmlFileName = newFile;
        //    if (!string.IsNullOrEmpty(activeConfig) && field is ColumnField && bool.TryParse(((ColumnField)field).ConvertFromString(activeConfig).ToString(), out isActiveConfig))
        //    {
        //        if (isActiveConfig)
        //        {
        //            outputXmlFileName = GetOutputXmlFileName(newFile, template,data,view);
        //        }
        //    }


            base.CreateXmlDocument(newFile, template, data, view);
            //if (view.Name == "v_P8CMRootProperty")
            //{
            //    FileInfo fileInfo = new FileInfo(template.Ouput);
            //    FileStream fs = fileInfo.Open(FileMode.Open);
            //    string str = "<?xml version='1.0' encoding='UTF-8'?><!DOCTYPE properties SYSTEM 'http://java.sun.com/dtd/properties.dtd'>";
            //    byte[] buffer = new byte[8000];
               
            //    System.Text.UTF8Encoding encoding = new System.Text.UTF8Encoding();
            //    fs.Read(buffer, 0, (int)fs.Length);
            //    str = str + encoding.GetString(buffer);
            //    fs.Write(encoding.GetBytes(str), 0, str.Length);
            //    fs.Flush();
            //    fs.Close();
            //}
            
        }
        //protected override void TransformXml(string xsd, string xsl, string outputFile)
        //{
        //    Durados.Web.Mvc.Specifics.Fibi.P8.BusinessLogic.P8CMTransformer transformer = new Durados.Web.Mvc.Specifics.Fibi.P8.BusinessLogic.P8CMTransformer();
        //    transformer.Tranform(xsd, xsl, outputFile, Durados.Xml.Schema.TransformationType.Xslt);
        //}
       
        private string GetOutputXmlFileName(string newFile, Durados.Workflow.XmlTemplate template, object data, Durados.View view)
        {
            string filename = string.Empty;
            string dot = ".";

            
            System.IO.FileInfo fileInfo = new System.IO.FileInfo(template.Schema);
            string orgExtension = fileInfo.Extension ?? string.Empty;
            string extension = ".xml";

            if (string.IsNullOrEmpty(newFile))
            {
                filename = template.Schema.TrimEnd((orgExtension).ToCharArray());
                filename += (extension.StartsWith(dot) ? string.Empty : dot) + extension;
            }
            else
            {
                filename += fileInfo.DirectoryName + @"\" + newFile;

                if (!newFile.EndsWith(".xml"))
                {
                    filename += (extension.StartsWith(dot) ? string.Empty : dot) + extension;
                }
            }

            return filename;
        }
    }
}
