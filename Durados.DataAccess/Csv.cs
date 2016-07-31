using System;
using System.Data;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using Durados;
using System.Xml;
using System.IO;
using System.Xml.Xsl;

namespace Durados.DataAccess
{
    public class Csv
    {
        private bool IsDenied(string denyRoles, string[] userRoles)
        {
            if (string.IsNullOrEmpty(denyRoles))
            {
                return false;
            }
            else
            {
                foreach (string role in userRoles)
                {
                    if (denyRoles.Contains(role))
                    {
                        return true;
                    }
                }

                return false;
            }

        }

        public string Export(DataView dataView, View view, string[] roles, ITableViewer tableViewer, string guid)
        {

            //Load SubGrids Data once
            Dictionary<string, DataView> SubGrids = new Dictionary<string, DataView>();

            Dictionary<string, Field> excluded = new Dictionary<string, Field>();

            foreach (Field field in view.Fields.Values.OrderBy(f => f.Order))
            {
                if (field.FieldType == FieldType.Children && field.GetFieldControlType() != "CheckList" && !field.IsHiddenInTable() && !IsDenied(field.DenySelectRoles, roles) && tableViewer.IsVisible(field, excluded, guid))
                {
                    ChildrenField c_field = (ChildrenField)field;

                    if (c_field.SubGridExport)
                    {
                        View c_view = (View)c_field.ChildrenView;

                        SubGrids.Add(c_view.Name, GetSubGridData(c_view));
                    }
                }
            }

            //Headers
            string header = GetColumnsHeaders(view, roles, tableViewer, guid, true, SubGrids, false) + charNewLine;

            //Columns Data - CSV
            return header + GetColumnsData(dataView, view, roles, tableViewer, guid, SubGrids) + charNewLine;
        }

        private string GetColumnsData(DataView dataView, View view, string[] roles, ITableViewer tableViewer, string guid, Dictionary<string, DataView> SubGrids)
        {
            StringBuilder content = new StringBuilder();

            Dictionary<string, Field> excluded = new Dictionary<string, Field>();

            //Main Grid Data
            foreach (DataRowView row in dataView)
            {
                string line = string.Empty;

                foreach (Durados.Field field in view.Fields.Values.OrderBy(f => f.Order))
                {
                    if (!field.IsHiddenInTable() && !IsDenied(field.DenySelectRoles, roles) && tableViewer.IsVisible(field, excluded, guid))
                    {

                        //SubGrids Data
                        if (field.FieldType == FieldType.Children && field.GetFieldControlType() != "CheckList")
                        {
                            ChildrenField c_field = (ChildrenField)field;

                            if (c_field.SubGridExport)
                            {

                                View c_view = (View)c_field.ChildrenView;

                                string pk = view.GetPkValue(row.Row);

                                DataView c_grid = SubGrids[c_view.Name];

                                tableViewer.DataView = c_grid;

                                DataTable dt = c_grid.Table.DataSet.Tables[view.DataTable.TableName];

                                DataRow[] ChildsRows = null;

                                if (pk != null && dt != null)
                                {
                                    DataRow dr = dt.Rows.Find(view.GetPkValue(pk));
                                    if (dr != null)
                                    {
                                        ChildsRows = dr.GetChildRows(c_field.DataRelation.RelationName);
                                    }
                                }

                                if (ChildsRows != null)
                                {
                                    foreach (Field cc_field in c_view.Fields.Values.OrderBy(f => f.Order))
                                    {
                                        if (!cc_field.IsHiddenInTable() && !IsDenied(cc_field.DenySelectRoles, roles) && tableViewer.IsVisible(cc_field, excluded, ""))
                                        {
                                            string val = string.Empty;
                                            string dispalyValue = null;

                                            foreach (DataRow childrensDataRow in ChildsRows)
                                            {
                                                dispalyValue = tableViewer.GetFieldDisplayValue(cc_field, childrensDataRow, true);
                                                if (!string.IsNullOrEmpty(dispalyValue))
                                                {
                                                    val += dispalyValue.Replace(charNewLine, ' ');
                                                }

                                                val += (char)10;

                                            }

                                            line += ValidCsvContent(val, true) + charDelimeter;
                                        }
                                    }
                                }
                                else
                                {
                                    foreach (Field cc_field in c_view.Fields.Values.OrderBy(f => f.Order))
                                    {
                                        if (!cc_field.IsHiddenInTable() && !IsDenied(cc_field.DenySelectRoles, roles) && tableViewer.IsVisible(cc_field, excluded, ""))
                                        {
                                            line += charDelimeter;//" - " + 
                                        }
                                    }
                                }
                            }
                        }
                        else //Main Grid Data
                        {
                            tableViewer.DataView = dataView;

                            line += ValidCsvContent(tableViewer.GetFieldDisplayValue(field, row.Row, true), false) + charDelimeter;
                        }

                    }

                }

                line += charNewLine;

                content.Append(line);

            }

            return content.ToString();
        }

        private DataView GetSubGridData(View view)
        {
            int rowsCounter;
            return view.FillPage(1, 1000000000, null, null, null, out rowsCounter, null, null);
        }

        private string GetColumnsHeaders(View view, string[] roles, ITableViewer tableViewer, string guid, bool withSubgrids, Dictionary<string, DataView> SubGrids, bool isSubGrid)
        {
            string header = string.Empty;

            string columnHeader = string.Empty;

            Dictionary<string, Field> excluded = new Dictionary<string, Field>();

            foreach (Field field in view.Fields.Values.OrderBy(f => f.Order))
            {

                if (!field.IsHiddenInTable() && !IsDenied(field.DenySelectRoles, roles) && tableViewer.IsVisible(field, excluded, guid))
                {
                    if (field.FieldType == FieldType.Children && field.GetFieldControlType() != "CheckList")
                    {
                        ChildrenField c_field = (ChildrenField)field;

                        if (withSubgrids && (c_field.SubGridExport))
                        {
                            View c_view = (View)c_field.ChildrenView;

                            DataView c_grid = SubGrids[c_view.Name];

                            tableViewer.DataView = c_grid;

                            header += GetColumnsHeaders(c_view, roles, tableViewer, "", false, null, true);
                        }
                    }
                    else
                    {
                        columnHeader = tableViewer.GetDisplayName(field, null, guid);

                        if (field.Equals(view.Fields.First()) && columnHeader.ToLower() == "id")
                        {
                            columnHeader = " " + columnHeader;
                        }

                        if (isSubGrid)
                        {
                            header += view.DisplayName + " ";
                        }

                        header += ValidCsvContent(columnHeader, false) + charDelimeter;

                    }
                }
            }
            return header;
        }

        public DataTable ExportToDataTable(DataView dataView, View view, string[] roles, ITableViewer tableViewer, string guid)
        {

            DataTable csvDataTable = new DataTable();

            string columnHeader = string.Empty;

            foreach (Field field in view.Fields.Values.OrderBy(f => f.Order))
            {
                if (!field.IsHiddenInTable() && !IsDenied(field.DenySelectRoles, roles))//&& field.FieldType != FieldType.Children
                {
                    if (tableViewer == null)
                    {
                        columnHeader = field.DisplayName;
                    }
                    else
                    {
                        columnHeader = tableViewer.GetDisplayName(field, null, guid);
                    }
                    if (field.Equals(view.Fields.First()) && columnHeader.ToLower() == "id")
                    {
                        columnHeader = "_" + columnHeader;
                    }

                    csvDataTable.Columns.Add(columnHeader, typeof(string));

                }
            }


            foreach (DataRowView row in dataView)
            {

                DataRow newRow = csvDataTable.NewRow();

                int i = 0;

                foreach (Durados.Field field in view.Fields.Values.OrderBy(f => f.Order))
                {
                    if (!field.IsHiddenInTable() && !IsDenied(field.DenySelectRoles, roles)) //&& field.FieldType != FieldType.Children
                    {
                        newRow[i] = tableViewer.GetFieldDisplayValue(field, row.Row, true);
                        //newRow[i] = field.ConvertToString(row.Row);
                        i++;
                    }

                }

                csvDataTable.Rows.Add(newRow);
            }


            return csvDataTable;
        }

        private static char charDelimeter = ',';

        private static char charEscape = '"';

        private static char charQuote = '"';

        private static char charNewLine = '\n';


        private string ValidCsvContent(string content, bool isMultiLine)
        {
            string validContent = "";

            bool needsQuotes = false;

            if (content != null)
            {
                if (!isMultiLine)
                {
                    validContent = content.Replace(charNewLine, ' ');
                }
                else
                {
                    validContent = content;

                    needsQuotes = true;
                }

                // Check if the value contains the quote char...

                if (validContent.Contains(charQuote.ToString()))
                {

                    // if so, add the escape char in front of the quote char.

                    validContent.Replace(charQuote.ToString(), charEscape.ToString() + charQuote.ToString());

                    needsQuotes = true;

                }

                // Check if the value contains the delimeter, if so, surround the whole row in quotes;

                validContent = validContent.Replace((char)13, (char)10);

                if (needsQuotes || validContent.Contains(charDelimeter.ToString()) || validContent.Contains(charNewLine.ToString()) || validContent.Contains(((char)13).ToString()) || validContent.Contains(((char)10).ToString()))

                    validContent = charQuote.ToString() + validContent + charQuote.ToString();

                //validContent = validContent.Replace((char)13, ' ');

            }

            else

                validContent = string.Empty;

            return validContent;
        }
    }

    public class WorkbookEngine
    {
        // you could have other overloads if you want to get creative... 
        public static string CreateWorkbook(DataSet ds, string xsl)
        {
            XmlDataDocument xmlDataDoc = new XmlDataDocument(ds);
            XslTransform xt = new XslTransform();
            StreamReader reader = new StreamReader(xsl);
            XmlTextReader xRdr = new XmlTextReader(reader);
            xt.Load(xRdr, null, null);
            StringWriter sw = new StringWriter();
            xt.Transform(xmlDataDoc, null, sw, null);
            return sw.ToString();
        }


    }
}
