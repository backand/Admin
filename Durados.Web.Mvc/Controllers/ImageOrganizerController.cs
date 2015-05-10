using System;
using System.Xml;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Ajax;
using System.Data;
using System.IO;

using Durados;
using Durados.DataAccess;
using Durados.Web.Mvc.UI.Helpers;
using Durados.Web.Mvc.Controllers.Filters;

namespace Durados.Web.Mvc.Controllers
{
    

    [NoCache]
    public class ImageOrganizerController : BaseController
    {

        public ContentResult GetUploadFolders()
        {
            XmlDocument xml = new XmlDocument();
            xml.InnerXml = "<xml></xml>";
            //string path = HttpContext.Server.MapPath("~/" + Map.Database.UploadFolder);
            string path = Map.Database.UploadFolder;
            DirectoryInfo dir = new DirectoryInfo(path);
            
            XmlElement element = xml.CreateElement("Folder");
            XmlAttribute name = xml.CreateAttribute("name");
            name.Value = dir.Name;
            element.Attributes.Append(name);
                
            xml.DocumentElement.AppendChild(element);

            LoadFolders(xml, element, dir);

            return this.Content(xml.InnerXml, "text/xml");
        }

        private void LoadFolders(XmlDocument xml, XmlElement parentElement, DirectoryInfo parentFolder)
        {
            foreach (DirectoryInfo dir in parentFolder.GetDirectories())
            {
                if (dir.Name != ".svn")
                {
                    XmlElement element = xml.CreateElement("Folder");
                    XmlAttribute name = xml.CreateAttribute("name");
                    name.Value = dir.Name;
                    element.Attributes.Append(name);
             
                    parentElement.AppendChild(element);
                    LoadFolders(xml, element, dir);
                }
            }
        }

        public ContentResult GetFolderImages(string folder)
        {
            XmlDocument xml = new XmlDocument();
            xml.InnerXml = "<xml></xml>";

            //string path = HttpContext.Server.MapPath("~/" + folder);

            string seperator = "\\";
            string root = Map.Database.UploadFolder;
            if (!Map.Database.UploadFolder.EndsWith("\\"))
            {
                root = root.TrimEnd('\\');
            }

            root = root.Remove(root.LastIndexOf('\\'));
            string path = root + seperator + folder.Replace('/', '\\');
            
            DirectoryInfo dir = new DirectoryInfo(path);

            foreach (FileInfo file in dir.GetFiles())
            {
                XmlElement element = xml.CreateElement("File");
                XmlAttribute name = xml.CreateAttribute("name");
                //name.Value = MapURL(file.FullName);
                name.Value = file.FullName.Replace(root, string.Empty).Replace('\\', '/');
                element.Attributes.Append(name); 
                xml.DocumentElement.AppendChild(element);
            }
            return this.Content(xml.InnerXml, "text/xml");
        }
        
        public ContentResult GetRowImages(string rowViewName, string key, string imageViewName)
        {
            XmlDocument xml = new XmlDocument();
            xml.InnerXml = "<xml></xml>";

            Durados.Web.Mvc.View imageView = (Durados.Web.Mvc.View)Map.Database.Views[imageViewName];

            Dictionary<string,object> values= new Dictionary<string,object>();
            Field field = imageView.Fields.Values.Where(f => f.FieldType.Equals(FieldType.Parent) && ((ParentField)f).ParentView.Name.Equals(rowViewName)).FirstOrDefault();
            values.Add(field.Name, key);

            Dictionary<string, SortDirection> sortColumns = new Dictionary<string, SortDirection>();
            if (imageView.DataTable.Columns.Contains(imageView.OrdinalColumnName))
            {
                sortColumns.Add(imageView.OrdinalColumnName, SortDirection.Asc);
            }

            int rowCount = 0;
            DataView dataView = imageView.FillPage(1, 1000, values, false, sortColumns, out rowCount, null, null);


            foreach (DataRow row in dataView.Table.Rows)
            {
                XmlElement element = xml.CreateElement("File");
                XmlAttribute name = xml.CreateAttribute("name");
                name.Value = row[imageView.ImageSrcColumnName].ToString();
                element.Attributes.Append(name); 
                xml.DocumentElement.AppendChild(element);
            }
            return this.Content(xml.InnerXml, "text/xml");
        }

        public virtual ContentResult GetRows(string rowsViewName)
        {
            XmlDocument xml = new XmlDocument();
            xml.InnerXml = "<xml></xml>";

            Durados.Web.Mvc.View view = (Durados.Web.Mvc.View)Map.Database.Views[rowsViewName];

            Dictionary<string, object> values = new Dictionary<string, object>();

            Dictionary<string, SortDirection> sortColumns = new Dictionary<string, SortDirection>();
            sortColumns.Add(view.DisplayColumn, SortDirection.Asc);
            int rowCount = 0;
            DataView dataView = view.FillPage(1, 1000, values, false, sortColumns, out rowCount, null, null);


            foreach (DataRow row in dataView.Table.Rows)
            {
                XmlElement element = xml.CreateElement("Row");
                XmlAttribute key = xml.CreateAttribute("key");
                key.Value = view.GetPkValue(row);
                element.Attributes.Append(key);
                XmlAttribute name = xml.CreateAttribute("name");
                name.Value = GetDisplayName(view, row);
                element.Attributes.Append(name);

                AddAdditionals(view, xml, element, row);
                xml.DocumentElement.AppendChild(element);
            }
            return this.Content(xml.InnerXml, "text/xml");
        }

        protected virtual void AddAdditionals(Durados.Web.Mvc.View view, XmlDocument xml, XmlElement element, DataRow row)
        {
        }

        protected virtual string GetDisplayName(Durados.Web.Mvc.View view, DataRow row)
        {
            return view.GetDisplayValue(row);
        }


        [ValidateInput(false)]
        [AcceptVerbs(HttpVerbs.Post)]
        public virtual ContentResult SetRowImages(string rowsViewName, string key, string imageViewName)
        {
            XmlDocument xml = new XmlDocument();

            using (System.IO.StreamReader sr = new System.IO.StreamReader(HttpContext.Request.InputStream))
            {
                string t = sr.ReadToEnd();
                xml.LoadXml(t);
            }

            Durados.Web.Mvc.View imageView = (Durados.Web.Mvc.View)Map.Database.Views[imageViewName];
            string fkField = imageView.Fields.Values.Where(f => f.FieldType == FieldType.Parent && ((ParentField)f).ParentView.Name.Equals(rowsViewName)).FirstOrDefault().Name;

            try
            {
                imageView.Delete(key, fkField, null, null, null);

                foreach (XmlElement element in xml.DocumentElement.ChildNodes)
                {
                    Dictionary<string, object> values = GetImageValues(imageView, key, fkField, element);

                    imageView.Create(values, null, null, null, null, null);
                }


                return this.Content("<xml>success</xml>", "text/xml");
            }
            catch (Exception exception)
            {
                return this.Content(string.Format("<xml><failure>{0}</failure></xml>", exception.Message), "text/xml");
            }
        }

        protected virtual Dictionary<string, object> GetImageValues(Durados.Web.Mvc.View imageView, string key, string fkField, XmlElement element)
        {
            Dictionary<string, object> values = new Dictionary<string, object>();

            values.Add(fkField, key);
            values.Add(imageView.OrdinalColumnName, element.Attributes["order"].Value);
            values.Add(imageView.ImageSrcColumnName, element.Attributes["name"].Value);

            AddImageValues(imageView, element, values);

            return values;
        }

        protected virtual void AddImageValues(Durados.Web.Mvc.View imageView, XmlElement element, Dictionary<string, object> values)
        {
        }

        public string MapURL(string path)
        {
            string appPath = HttpContext.Server.MapPath("~");
            string url = String.Format("~{0}", path.Replace(appPath, "").Replace('\\', '/'));
            return url;
        }
    }
}
