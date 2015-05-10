using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Ajax;
using System.Data;
using System.IO;
using System.Xml;

using Durados;
using Durados.DataAccess;
using Durados.Web.Mvc.UI.Helpers;

namespace Durados.Web.Mvc.Controllers
{
    public class UtilsController : BaseController
    {
        public virtual string DuplicateViews(string sourceXML, string destXML)
        {
            //return "Need to debug!";
            //http://localhost:3694/Utils/DuplicateViews?sourceXML=/Config/crmshade.xml&destXML=/config/crmbase.xml

            if(string.IsNullOrEmpty(sourceXML) || string.IsNullOrEmpty(destXML))
                return "Missing XML values";

            sourceXML = this.Server.MapPath(sourceXML);
            destXML = this.Server.MapPath(destXML);

            XmlDocument sXML = new XmlDocument();
            sXML.Load(sourceXML);

            XmlDocument dXML = new XmlDocument();
            dXML.Load(destXML);

            //copy all Menu
            AppendNodes("Menu", sXML, dXML);

            //copy all the Category
            AppendNodes("Category", sXML, dXML);

            //copy all Upload
            AppendNodes("Upload", sXML, dXML);

            //loop on all the source views
            XmlNodeList sViews = sXML.SelectNodes("//View");

            foreach (XmlNode view in sViews)
            {
                //get view name
                string svName = view.SelectSingleNode("Name").InnerText;

                //get destination view
                XmlNode dView = dXML.SelectSingleNode("//View[Name='" + svName + "']");

                if (dView != null)
                {
                    //clone the source view and replace the ID with the destination
                    XmlNode newView = view.CloneNode(true);
                    newView.SelectSingleNode("ID").InnerXml = dView.SelectSingleNode("ID").InnerXml;

                    //replace the source clone node in the destination
                    dView.InnerXml = newView.InnerXml;

                    //update all fields
                    UpdateFields(view, dView);
                }
            }

            dXML.Save(destXML);

            return "Done!";
        }

        private void AppendNodes(string name, XmlDocument sXML, XmlDocument dXML)
        {
            XmlNodeList nodes = sXML.SelectNodes("//" + name + "[ID]");
            foreach (XmlNode node in nodes)
            {
                XmlNode newNode = dXML.CreateNode(XmlNodeType.Element, name, null);
                newNode.InnerXml = node.InnerXml;
                dXML.DocumentElement.AppendChild(newNode);
            }
        }

        private void UpdateFields(XmlNode sView, XmlNode dView)
        {
            //get all the fields of the source view
            string sViewId = sView.SelectSingleNode("ID").InnerXml;
            XmlNodeList sFields = sView.SelectNodes("//Field[Fields='" + sViewId + "']");
            
            //get destination view id
            string dViewId = dView.SelectSingleNode("ID").InnerXml;

            foreach (XmlNode field in sFields)
            {

                //get field name
                string sfName = field.SelectSingleNode("Name").InnerText;

                //get destination field
                XmlNode dField = dView.SelectSingleNode("//Field[Name='" + sfName + "' and Fields='" + dViewId + "']");

                if (dField != null)
                {
                    //clone source field
                    XmlNode newField = field.CloneNode(true);

                    //update the original values
                    newField.SelectSingleNode("ID").InnerXml = dField.SelectSingleNode("ID").InnerXml;
                    newField.SelectSingleNode("Fields").InnerXml = dField.SelectSingleNode("Fields").InnerXml;

                    //replace the source clone node in the destination
                    dField.InnerXml = newField.InnerXml;
                }


            }

        }

    }

    
}
