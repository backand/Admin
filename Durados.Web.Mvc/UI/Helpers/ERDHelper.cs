using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using Durados.DataAccess;
using System.Web.Mvc;
using Durados.Web.Mvc.Controllers;

using System.Web.Script.Serialization;
using System.Text.RegularExpressions;

namespace Durados.Web.Mvc.UI.Helpers
{
    public class ERDHelper
    {
        Database database;
        string ERDGraphName = "ERD";
        string ERDViewName = "Views";
        string DataBaseType = "mssql";
        string RootElement = "sql";
        string RootPath = "~/";


        string ViewNodeName = "table";
        string TblNameAttr = "name";
        string ViewNameAttr = "viewName";
        string ViewXAttr = "x";
        string ViewYAttr = "y";
        string ColumnNodeName = "row";
        string ColumnNullAttr = "null";
        string ColumnAutoIncAttr = "autoincrement";
        string ColumnNameAttr = "name";
        string KeyNodeName = "key";
        private string DataTypeNodeName = "datatype";

        private string RelationNodeName = "relation";
        
        private string KeyPartNodeName = "part";
        private string RelationRowNodeAttrib = "row";
        private string KeyTypeNodeName="type";
        private string PrimaryAttribName = "PRIMARY";

        string FieldIdSpan = "<span class='erdField'>";
        string ViewIdSpan ="<span class='erdView'>";
        string EndFieldIdSpan = "</span>";

        string ViewNamePRE = "%%%ID_";
        string ViewNamePOS = "_ID%%%";
        string DuradosDataTypesFileName = "Content/erd/db/datatypes.xml";
        int newTableCounter = 1;
        int xWidth = 10;
        int yWidth = 10;
        

        

       
        public ERDHelper()
        {
            // TODO: Complete member initialization
        }



         public void SaveERDState(string xml, Database database)
        {
            this.database = database;
            string cnn=database.SystemConnectionString;
            string sp = "durados_UpdateWorkflowGraphState";
            string state = GetERDTableStateFromXml(xml);

            Dictionary<string, object> parameters = new Dictionary<string, object>();

            parameters.Add("ViewName", ERDGraphName);
            parameters.Add("State", state);

            SqlAccess da = new SqlAccess();
            try
            {
                da.ExecuteNoneQueryStoredProcedure(cnn, sp, parameters);
                //return "success";
            }
            catch (Exception exception)
            {
                database.Logger.Log(ERDViewName, "GetERDState", exception.Source, exception, 1, null);
              //  throw new DuradosException("Failed to save tables position.", exception);
                //string message = "Failed to save tables position." + exception.Message;
                //return message;
            }
        }

         private string GetERDTableStateFromXml(string xml)
         {
             
             XmlDocument doc = new XmlDocument();
             xml = ReplaceSpans(xml);
             try
             {
                 doc.LoadXml(xml);
             }
             catch (Exception ex)
             {

                 throw new DuradosException("The saved ERD data is not formated.", ex);
             }
             
             XmlNodeList tableNodes = doc.GetElementsByTagName(ViewNodeName);
             ERDState erdState = new ERDState();
             erdState.tableState = new Dictionary<string, ERDCoordinates>();
             foreach (XmlNode node in tableNodes)
             {
                 if (node.Attributes[ViewNameAttr] != null)
                 {

                     string viewName = node.Attributes[ViewNameAttr].Value;
                     int xCoor, yCoor;
                     if (int.TryParse(node.Attributes[ViewXAttr].Value, out xCoor) && int.TryParse(node.Attributes[ViewYAttr].Value, out yCoor))
                         erdState.tableState.Add(viewName, new ERDCoordinates { X = node.Attributes[ViewXAttr].Value, Y = node.Attributes[ViewYAttr].Value });
                 }
             }

             
             string erdJasonState = new System.Web.Script.Serialization.JavaScriptSerializer().Serialize(erdState);//.Deserialize<ERDState>(json);
             
             return erdJasonState;


         }

         private string GetViewName(string viewNameId)
         {
             int sIndex=viewNameId.IndexOf(ViewNamePRE)+ViewNamePRE.Length;
             int eIndex=viewNameId.IndexOf(ViewNamePOS);
             string viewName = viewNameId.Substring(sIndex, eIndex - sIndex);
             return viewName;
         }

         private string ReplaceSpans(string xml)
         {
             XmlDocument factoDoc = new XmlDocument();
             int sIndex = xml.IndexOf(ViewIdSpan);
             int count;
             while (sIndex > 0)
             {
                 count = xml.IndexOf(EndFieldIdSpan, sIndex) + EndFieldIdSpan.Length - sIndex;
                 string span = xml.Substring(sIndex, count);
                 XmlElement sElm = factoDoc.CreateElement("span");
                 sElm.InnerXml = span;
                 string viewName = sElm.InnerText;
                 span=span+"\"";
                 xml = xml.Replace(span, "\" " + ViewNameAttr + "='" + viewName + "'");
                 sIndex = xml.IndexOf(ViewIdSpan);
             }

             //bool FoundMatch = false;

             try
             {
                 Regex regex = new Regex(@"<span class='erdField'.*?</span>");
                 while (regex.IsMatch(xml))
                 {
                     xml = regex.Replace(xml, "");
                 }
             }
             catch (ArgumentException)
             {
                 // Syntax error in the regular expression
             }
             //Regex r = new Regex(FieldIdSpan + @"\w" + EndFieldIdSpan);
             //xml = r.Replace(xml, "");
             return xml;
         }

        
        public virtual XmlDocument GetERDXml(Durados.Web.Mvc.Database database)
        {
            
            
            if (database == null)
            {
                return null;
            }
            Dictionary<string, ERDCoordinates> tables= GetERDState(database);;
            
            string databaseType = DataBaseType;
            XmlDocument xmlDocumet = new XmlDocument();
            XmlElement root = xmlDocumet.CreateElement(RootElement);
            xmlDocumet.AppendChild(root);

            string dataTypesFileName = RootPath + DuradosDataTypesFileName;
            dataTypesFileName = System.Web.HttpContext.Current.Server.MapPath(dataTypesFileName);

            XmlDocumentFragment dataTypesXmlFrag = xmlDocumet.CreateDocumentFragment();
            XmlDocument dataTypesXml = new XmlDocument();
            dataTypesXml.Load(dataTypesFileName);
            dataTypesXmlFrag.InnerXml = dataTypesXml.DocumentElement.OuterXml;

            root.AppendChild(dataTypesXmlFrag);

            foreach (Durados.Web.Mvc.View view in database.Views.Values.Where(r=>!r.SystemView))
            {
                string viewName=GetViewName(view);
                XmlElement tableElm = xmlDocumet.CreateElement(ViewNodeName);
                XmlAttribute tableNameAttrib = xmlDocumet.CreateAttribute(TblNameAttr);
                XmlAttribute xAttrib = xmlDocumet.CreateAttribute(ViewXAttr);
                XmlAttribute yAttrib = xmlDocumet.CreateAttribute(ViewYAttr);
                tableNameAttrib.Value = GetViewNameWithSpan(view);
                xAttrib.Value = tables.ContainsKey(view.Name) ? tables[view.Name].X : (++newTableCounter * xWidth).ToString();
                yAttrib.Value = tables.ContainsKey(view.Name) ? tables[view.Name].Y : (newTableCounter * yWidth).ToString();
                tableElm.Attributes.Append(tableNameAttrib);
                tableElm.Attributes.Append(xAttrib);
                tableElm.Attributes.Append(yAttrib);

                foreach (Durados.Field field in view.Fields.Values)
                {
                    XmlElement rowElm = xmlDocumet.CreateElement(ColumnNodeName);
                    XmlAttribute rowName = xmlDocumet.CreateAttribute(ColumnNameAttr);
                    XmlAttribute rowNull = xmlDocumet.CreateAttribute(ColumnNullAttr);
                    XmlAttribute rowAutoInc = xmlDocumet.CreateAttribute(ColumnAutoIncAttr);
                    rowNull.Value = field.Required ? "0" : "1";
                    rowAutoInc.Value = view.PrimaryKeyFileds.Contains(field) || view.IsAutoIdentity ? "1" : "0";
                    rowName.Value = GetFieldNameWithSpan(field);
                    rowElm.Attributes.Append(rowName);
                    rowElm.Attributes.Append(rowNull);
                    rowElm.Attributes.Append(rowAutoInc);
                    /* Data Type */
                    XmlElement dataTypeElm = xmlDocumet.CreateElement(DataTypeNodeName);
                    dataTypeElm.InnerText = field.DataType.ToString();
                    rowElm.AppendChild(dataTypeElm);
                    
                    /* Foriegn Key */
                    if (field.FieldType == FieldType.Parent && !string.IsNullOrEmpty(field.RelatedViewName))
                    {
                        XmlElement relationElm = xmlDocumet.CreateElement(RelationNodeName);
                        XmlAttribute relationTable = xmlDocumet.CreateAttribute(ViewNodeName);
                        XmlAttribute relationField = xmlDocumet.CreateAttribute(RelationRowNodeAttrib);
                        relationTable.Value = GetViewNameWithSpan((Durados.Web.Mvc.View)database.Views[field.RelatedViewName]);
                        //string columnName = null;
                        if (HasPrimary(view, field))
                            relationField.Value = GetFieldNameWithSpan(view.Database.Views[field.RelatedViewName].PrimaryKeyFileds[0]);
                        else
                            throw new DuradosException("Table [" + view.Name + "]  must have a primary key.");
                        relationElm.Attributes.Append(relationField);
                        relationElm.Attributes.Append(relationTable);
                        rowElm.AppendChild(relationElm);
                    }
                    /* Default Value */
                    if (field.DefaultValue !=null && !string.IsNullOrEmpty(field.DefaultValue.ToString()))
                    {
                        XmlElement defaultRowElm = xmlDocumet.CreateElement("Default");
                        defaultRowElm.InnerText = field.DefaultValue.ToString();
                        rowElm.AppendChild(defaultRowElm);
                    }
                    tableElm.AppendChild(rowElm);
                    /* Keys*/
                    XmlElement keyElm = xmlDocumet.CreateElement(KeyNodeName);
                    XmlAttribute keyAttrib = xmlDocumet.CreateAttribute(KeyTypeNodeName);
                    keyAttrib.Value=PrimaryAttribName;
                    foreach (Field keyField in view.PrimaryKeyFileds)
                    {
                        XmlElement keyPartElm = xmlDocumet.CreateElement(KeyPartNodeName);
                        keyPartElm.InnerText = field.DisplayName;
                        keyElm.AppendChild(keyPartElm);
                    }
                    tableElm.AppendChild(keyElm);
                    

                }
                xmlDocumet.DocumentElement.AppendChild(tableElm);
              
            }
            return xmlDocumet;
        }

        private Dictionary<string, ERDCoordinates> GetERDState(Database database)
        {
            try{
               string sql = "SELECT TOP 1 GraphState FROM [dbo].[durados_WF_Info] WHERE [ViewName] = @ViewName";

                Dictionary<string, object> parameters = new Dictionary<string, object>();

                parameters.Add("ViewName", ERDGraphName);

                SqlAccess da = new SqlAccess();
                Dictionary<string, ERDCoordinates> tables = new Dictionary<string, ERDCoordinates>();
                string json = da.ExecuteScalar(database.SystemConnectionString, sql, parameters);
                if (!string.IsNullOrEmpty(json))
                {
                    try
                    {
                        ERDState erdState = new System.Web.Script.Serialization.JavaScriptSerializer().Deserialize<ERDState>(json);
                        tables = erdState.tableState;
                    }
                    catch (Exception ex)
                    {
                        database.Logger.Log("ERDHelper", "", "GetERDState", ex, 1, "Deserialize Failed!");
                        
                    }
                }
                return tables;
                
                //TO DO Load View Dictionary with ERD state X,Y
                //return View(new Durados.Workflow.Graph(view, this, Durados.WorkflowAction.CompleteStep));
            }
            catch (Exception exception)
            {

               database.Logger.Log(ERDViewName, "GetERDState", exception.Source, exception, 1, null);
               throw new DuradosException("Failed to load table state",exception);
            }
        }

        private static bool HasPrimary(Durados.Web.Mvc.View view, Durados.Field field)
        {
            if (view.Database.Views.ContainsKey(field.RelatedViewName))
                return view.Database.Views[field.RelatedViewName].PrimaryKeyFileds.Length > 0;
            else
                return false;
        }

        private  string GetFieldName(Durados.Field field)
        {
            return field.DisplayName;
        }
        private string GetFieldNameWithSpan(Durados.Field field)
        {
            return GetFieldName(field) + FieldIdSpan + field.View.Name + "," + field.Name + EndFieldIdSpan; 
        }

        private  string GetViewName(Durados.Web.Mvc.View view)
        {

            return view.DisplayName  ;
            //return view.Name;
        }
        private string GetViewNameWithSpan(Durados.Web.Mvc.View view)
        {

            return GetViewName(view)  + ViewIdSpan + view.Name + EndFieldIdSpan; ;
            //return view.Name;
        }

        public void CreateERDFile(Mvc.Database database, bool save, out string retXml)
        {
            retXml = null;
            string erdFileName = Maps.GetConfigPath(database.Map.AppName + ".ERD.xml");
            //erdFileName = System.Web.HttpContext.Current.Server.MapPath(erdFileName);
            //string erdFileName = RootPath + "Config\\" + database.Map.AppName + ".ERD.xml";
            if (save)
                GetERDXml(database).Save(erdFileName);
            else
                retXml = GetERDXml(database).OuterXml;
        }

        
    }

    internal class ERDState
    {
        public Dictionary<string,ERDCoordinates> tableState { get; set; }
    }
    internal class ERDCoordinates
    {
        public ERDCoordinates()
        {

        }
        public string X { get; set; }
        public string Y { get; set; }

      

    }
   
}
