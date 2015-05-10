using System;
using System.Data;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Schema;

using Durados.DataAccess;


namespace Durados.Xml.Schema
{
    public class Converter
    {

        public virtual DataSet Convert(View view, DataRow row, string schemaPath)
        {
            DataSet xmlSchemaDataset = new DataSet();
            xmlSchemaDataset.ReadXmlSchema(schemaPath);

            LoadElement(xmlSchemaDataset, view, row, null, null);

            return xmlSchemaDataset;
        }

        protected virtual void LoadElement(DataSet xmlSchemaDataset, View view, DataRow row, DataRelation xmlRelation, DataRow xmlParentRow)
        {
            #region Handle Atrributes
            string elementName = view.GetXmlElement();
            if (!xmlSchemaDataset.Tables.Contains(elementName))
                throw new DuradosXmlConverterException("The view " + view.DisplayName + " has no equivalent element " + elementName + " in the schema. Please check the Xml Element configuration of the view. Or that it has the appropriate schema.");

            DataTable table = xmlSchemaDataset.Tables[elementName];

            DataRow xmlRow = table.NewRow();

            if (xmlParentRow != null && xmlRelation != null)
            {
                xmlRow.SetParentRow(xmlParentRow, xmlRelation);
            }

            foreach (Field field in view.Fields.Values.Where(f => f.ExportToXml))
            {
                switch (field.FieldType)
                {
                    case FieldType.Column:
                        LoadColumn((ColumnField)field, row, xmlRow, table);
                        break;

                    case FieldType.Parent:
                        LoadParent((ParentField)field, row, xmlRow, table);
                        break;


                    default:
                        break;
                }
            }

            #endregion Handle Atrributes

            table.Rows.Add(xmlRow);


            #region Handle Elements
            foreach (DataRelation relation in table.ChildRelations)
            {
                DataTable childTable = relation.ChildTable;

               // var v = view.Fields.Values.Where(f => f.FieldType == FieldType.Children && f.ExportToXml && ((ChildrenField)f).GetXmlElement() == childTable.TableName).GroupBy(g => ((ChildrenField)g).GetXmlElement());
               foreach (ChildrenField childrenField in view.Fields.Values.Where(f => f.FieldType == FieldType.Children && f.ExportToXml && ((ChildrenField)f).GetXmlElement() == childTable.TableName))
                {
                    if (childrenField != null )
                    {
                        DataView dataView = childrenField.GetDataView(row);

            
                        DataRow groupRow = xmlRow;
                        DataRelation xmlChildRelation = relation;

                        #region Handle Group Element
                        if (IsChildrenGroup(childrenField) && dataView.Count > 0)
                        {
                            DataRow childGroupRow = childTable.NewRow();
                            childGroupRow.SetParentRow(xmlRow, relation);
                            childTable.Rows.Add(childGroupRow);
                            groupRow = childGroupRow;

                            foreach (DataRelation r in childTable.ChildRelations)
                            {

                                if (r.ChildTable.TableName.Equals(childrenField.ChildrenView.GetXmlElement()))
                                {
                                    xmlChildRelation = childTable.ChildRelations[r.RelationName];
                                }
                            }

                        }
                        #endregion Handle Group Element

                        LoadChildren(childrenField, row, groupRow, xmlChildRelation, dataView);
                       
                    }
                }

            }
            #endregion Handle Elements


        }

        

        protected virtual bool IsChildrenGroup(ChildrenField childrenField)
        {
            return childrenField.GetXmlElement() != childrenField.ChildrenView.GetXmlElement();
        }


        protected virtual void LoadChildren(ChildrenField childrenField, DataRow row, DataRow xmlRow, DataRelation xmlRelation, DataView dataView)
        {
            foreach (DataRowView childRow in dataView)
            {
                LoadElement(xmlRow.Table.DataSet, childrenField.ChildrenView, childRow.Row, xmlRelation, xmlRow);
            }
        }

        protected virtual void LoadParent(ParentField parentField, DataRow row, DataRow xmlRow, DataTable table)
        {
            string[] attributesNames = GetAttributesNames(parentField);
            Field[] fields = GetFields(parentField);

            if (attributesNames.Length != fields.Length)
            {
                throw new DuradosXmlException();
            }

             DataRow parentRow = GetParentViewRow(parentField, row);
             if (parentRow == null)
                 return;


            for (int i = 0; i < attributesNames.Length; i++)    
            {
                string attributeName = attributesNames[i];
                Field field = fields[i];

                if (table.Columns.Contains(attributeName))
                {
                    //if (parentRow == null)
                    //    xmlRow[attributeName] = string.Empty;
                    //else
                    //{
                        Type attributeType = table.Columns[attributeName].DataType;
                        xmlRow[attributeName] = GetValue(field, parentRow, attributeType) ?? DBNull.Value;
                    //}
                }
                else
                {
                    // handle error
                }
            }
        }

        protected virtual DataRow GetParentViewRow(ParentField parentField, DataRow row)
        {
            DataRow parentRow = null;

            parentRow = row.GetParentRow(parentField.DataRelation.RelationName);
            if (parentRow == null)
            {
                View parentView = parentField.ParentView;
                if (row.Table.Columns.Contains(parentField.DatabaseNames))
                {

                    if (!string.IsNullOrEmpty(row[parentField.DatabaseNames].ToString()))
                    {
                        parentRow = parentView.GetDataRow(row[parentField.DatabaseNames].ToString());
                    }
                }
            }
            return parentRow;
        }

        protected virtual void LoadColumn(ColumnField columnField, DataRow row, DataRow xmlRow, DataTable table)
        {
            string attributeName = GetAttributeName(columnField);

            if (table.Columns.Contains(attributeName))
            {
                Type attributeType = table.Columns[attributeName].DataType;
                xmlRow[attributeName] = GetValue(columnField, row, attributeType) ?? DBNull.Value;
            }
            else
            {
                // handle error
            }
        }

        protected virtual object GetValue(Field field, DataRow row, Type attributeType)
        {
            string value = string.Empty;
            value = field.GetValue(row);
            
            if (string.IsNullOrEmpty(value))
                return null;

            if (attributeType.Equals(typeof(bool)))
            {
                return value == field.View.Database.True;
            }
            try
            {
                return System.Convert.ChangeType(value, attributeType);
            }
            catch (Exception)
            {
                throw new DuradosXmlConverterException("Failed to convert the value " + value.ToString() + " from view " + field.View.DisplayName + " and field " + field.DisplayName + " to " + attributeType.Name);
            }
        }

        protected virtual string GetAttributeName(ColumnField field)
        {
            return field.GetAttributeName();
        }

        protected virtual string[] GetAttributesNames(ParentField field)
        {
            return field.GetAttributesNames();
        }

        protected virtual Field[] GetFields(ParentField field)
        {
            return field.GetXmlFields();
        }

    }

    public class DuradosXmlConverterException : DuradosException
    {
        public DuradosXmlConverterException(string message) : base(message) { }
    }
}
