using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Durados.Xml.Schema;
using Durados.Workflow;
using System.IO;
using System.Xml;
using System.Data;

namespace Durados.Web.Mvc.Specifics.Fibi.P8.BusinessLogic
{
    public class P8XMLConverter : Durados.Xml.Schema.Converter
    {
        //public override System.Data.DataSet Convert(Durados.View view, System.Data.DataRow row, string schemaPath)
        //{
        //    string xmlStr = "<P8CM_Config>" + base.Convert(view, row, schemaPath).GetXml() + "</P8CM_Config>";
        //    System.Xml.XmlDocument xmlDoc = new System.Xml.XmlDocument();
        //    xmlDoc.LoadXml(xmlStr);


        //    System.Data.DataSet dataSet = new System.Data.DataSet();
        //    System.IO.StringReader xmlSR = new System.IO.StringReader(xmlDoc.InnerXml);
        //    //XmlWriter xmlWriter = new XmlTextWriter(xmlSR, Encoding.Default);
        //    //xmlDoc.WriteContentTo(xmlWriter);
        //    dataSet.ReadXml(xmlSR);
        //    return dataSet;
        //}
    }
}
