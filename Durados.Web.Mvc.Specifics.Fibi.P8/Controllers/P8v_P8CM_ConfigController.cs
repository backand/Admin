using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Xml.XPath;
using System.Xml.Xsl;
using System.Xml;
using System.IO;

namespace Durados.Web.Mvc.Specifics.Fibi.P8.Controllers
{
    public class P8v_P8CM_ConfigController : P8CMBaseController
    {


        protected override void CreateXmlDocument(string newFile, Durados.Workflow.XmlTemplate template, object data, Durados.View view)
        {
            base.CreateXmlDocument(newFile, template, data, view);
        }
                 //DataSet xml = CreateXmlDataset(template, data, view);


            //xml.Namespace = null;

            //xml.WriteXml(newFile);

            //TransformXML(newFile);


        
        public override string GetFileName(Durados.View view, string documentFieldName, string fileName)
        {
            return base.GetFileName(view, documentFieldName, fileName);
        }

        //protected virtual void TransformXML(string newFile)
        //{
        //    UsingXLST(newFile);
        //}

    
        //protected virtual void UsingXLST(string newFile)
        //{

        //    string path=GetPath(newFile);

        //    XPathDocument myXPathDocument = new XPathDocument(newFile);//loading the original xml
          
        //    System.Xml.Xsl.XslCompiledTransform myXslTransform = new System.Xml.Xsl.XslCompiledTransform();

        //    XmlTextWriter writer = new XmlTextWriter(path + "\\" + GetFileName(newFile) + ".xml", null);//loading the output xml
            
        //    string xsltFileName =path + "\\"+GetFileName(newFile)+".xslt";

        //    if (IsXsltFileExists(xsltFileName))
        //    {

        //        myXslTransform.Load(xsltFileName);//loading xslt file to transformer

        //        myXslTransform.Transform(myXPathDocument, null, writer);
        //    }
        //    else
        //    {
        //        throw new DuradosException("Xslt file could be found!");
        //    }
        //    writer.Close();
            
        //}

        private bool IsXsltFileExists(string xsltFileName)
        {
            return (string.IsNullOrEmpty(xsltFileName) && !System.IO.File.Exists(xsltFileName));
            
        }

        //private static string GetPath(string newFile)
        //{
        //    FileInfo fileInfo = new FileInfo(newFile);
        //    return fileInfo.DirectoryName;
        //}
        //private static string GetFileName(string newFile)
        //{
        //    FileInfo fileInfo = new FileInfo(newFile);
        //    return fileInfo.Name.Substring(0, fileInfo.Name.IndexOf("_1."));
        //}

        //private static string GetXMLFileName(string newFile)
        //{
        //    return newFile + ".xml";
        //}
        /*    private void ModifyConfigToP8(string newFile ,System.Xml.XmlDocument xmlDoc,bool old)
         {
             //UsingXLST(newFile);
           

             System.Xml.XmlNodeList  xmlNodes = xmlDoc.DocumentElement.GetElementsByTagName("Result");
             for (int i=xmlNodes.Count-1 ;i>=0;i--)
             {

                 string nodeName = xmlNodes[i].Attributes["ResultType"].Value;
                
                 XmlElement newXmlElement = xmlDoc.CreateElement(nodeName, xmlNodes[i].NamespaceURI);

                 newXmlElement.InnerXml = xmlNodes[i].InnerXml;

                 foreach (XmlNode node in xmlNodes[i].Attributes)
                 {
                     newXmlElement.SetAttribute(node.Name, node.Value);
                 }
                 newXmlElement.RemoveAttribute("ResultType");
                
                 XmlNode parentNode = xmlNodes[i].ParentNode;
                 parentNode.ReplaceChild(newXmlElement, xmlNodes[i]);
             }

           // System.Xml.Xsl.XslTransform 
           
                
            
         }*/
    }
}
