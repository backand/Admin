using System;
using System.Collections;
using System.Reflection;
using System.Data;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Durados.Config.Attributes;

namespace Durados.Config
{
    public class Configurator
    {
        public Configurator()
        {
        }

        public void LoadMetaConfiguration(object o)
        {
            Type type = o.GetType();
            PropertyInfo[] properties = type.GetProperties(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);

            foreach (PropertyInfo propertyInfo in properties)
            {
                object[] propertyAttributes = propertyInfo.GetCustomAttributes(typeof(PropertyAttribute), true);
                if (propertyAttributes.Length == 1)
                {
                    PropertyAttribute propertyAttribute = (PropertyAttribute)propertyAttributes[0];
                    object[] configValueAttributes = propertyInfo.GetCustomAttributes(typeof(ConfigValueAttribute), true);

                    foreach (ConfigValueAttribute configValueAttribute in configValueAttributes)
                    {
                        if (type.Equals(configValueAttribute.Type))
                        {
                            propertyInfo.SetValue(o, configValueAttribute.Value, null);
                        }
                    }

                    switch (propertyAttribute.PropertyType)
                    {
                        case PropertyType.Column:

                            break;

                        case PropertyType.Parent:
                            object parent = propertyInfo.GetValue(o, null);
                            if (parent != null)
                            {
                                LoadMetaConfiguration(parent);
                            }
                            
                            break;

                        case PropertyType.Children:
                            ChildrenPropertyAttribute childrenPropertyAttribute = (ChildrenPropertyAttribute)propertyAttribute;
                            
                            object children = propertyInfo.GetValue(o, null);
                            if (children != null)
                            {
                                IEnumerable list = null;
                                if (children is IDictionary)
                                {
                                    list = (IEnumerable)((IDictionary)children).Values;
                                }
                                else if (children is IList)
                                {
                                    list = (IEnumerable)children;
                                }
                                else
                                {
                                    throw new NotSupportedException();
                                }
                                if (list != null)
                                {
                                    foreach (object child in list)
                                    {
                                        if (child != null)
                                        {
                                            LoadMetaConfiguration(child);
                                        }
                                    }
                                }
                                
                            }

                            break;

                        default:
                            throw new NotSupportedException();
                    }

                }
            }
        }

        public DataSet ConvertToDataSet(object o)
        {
            DataSet dataSet = new DataSet();

            Type type = o.GetType();

            string tableName = type.Name;

            object[] classConfigAttributes = type.GetCustomAttributes(typeof(ClassConfigAttribute), true);
            if (classConfigAttributes.Length == 1 && classConfigAttributes[0] is ClassConfigAttribute)
            {
                ClassConfigAttribute classConfigAttribute = (ClassConfigAttribute)classConfigAttributes[0];
                if (!string.IsNullOrEmpty(classConfigAttribute.TableName))
                    tableName = classConfigAttribute.TableName;
            }

            AddRow(o, dataSet, tableName);

            return dataSet;
        }

        public void Load(DataSet dataSet, object root)
        {
            Type type = root.GetType();

            string tableName = type.Name;
            object[] classConfigAttributes = type.GetCustomAttributes(typeof(ClassConfigAttribute), true);
            if (classConfigAttributes.Length == 1 && classConfigAttributes[0] is ClassConfigAttribute)
            {
                ClassConfigAttribute classConfigAttribute = (ClassConfigAttribute)classConfigAttributes[0];
                if (!string.IsNullOrEmpty(classConfigAttribute.TableName))
                    tableName = classConfigAttribute.TableName;
            }

            if (dataSet.Tables.Contains(tableName))
            {
                DataTable dataTable = dataSet.Tables[tableName];
                if (dataTable.Rows.Count == 1)
                {
                    Load(dataTable.Rows[0], root);
                }

            }
        }

        private void Load(DataRow dataRow, object o)
        {
            Type type = o.GetType();
            PropertyInfo[] properties = type.GetProperties(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);

            foreach (PropertyInfo propertyInfo in properties)
            {
                string relationName;
                object[] propertyAttributes = propertyInfo.GetCustomAttributes(typeof(PropertyAttribute), true);
                if (propertyAttributes.Length == 1)
                {
                    PropertyAttribute propertyAttribute = (PropertyAttribute)propertyAttributes[0];

                    switch (propertyAttribute.PropertyType)
                    {
                        case PropertyType.Column:

                            if (dataRow.Table.Columns.Contains(propertyInfo.Name))
                            {
                                DataColumn dataColumn = dataRow.Table.Columns[propertyInfo.Name];

                                object value = dataRow[dataColumn];
                                if (propertyInfo.GetSetMethod() != null)
                                {
                                    if (value is DBNull || (dataColumn.DataType is object && value.ToString().ToLower() == "null"))
                                    {
                                        value = null;
                                    }
                                    else
                                    {
                                        if (propertyInfo.PropertyType.BaseType != null && propertyInfo.PropertyType.BaseType.FullName == "System.Enum")
                                        {
                                            value = Enum.Parse(propertyInfo.PropertyType, value.ToString());
                                        }

                                    }
                                    propertyInfo.SetValue(o, value, null);

                                }
                            }
                            break;

                        case PropertyType.Parent:
                            if (dataRow.Table.Columns.Contains(propertyInfo.Name))
                            {
                                DataColumn dataColumn = dataRow.Table.Columns[propertyInfo.Name];
                                ParentPropertyAttribute parentPropertyAttribute = (ParentPropertyAttribute)propertyAttribute;
                                relationName = parentPropertyAttribute.RelationName ?? propertyInfo.Name;
                                DataRow parentRow = dataRow.GetParentRow(relationName);
                                object parent = propertyInfo.GetValue(o, null);
                                if (parentRow == null)
                                {
                                    if (parent != null)
                                    {
                                        if (propertyInfo.GetSetMethod() != null)
                                        {
                                            propertyInfo.SetValue(o, null, null);
                                        }
                                    }
                                }
                                else
                                {
                                    if (parent == null)
                                    {
                                        Type propertyType = propertyInfo.PropertyType;
                                        object[] classAttributes = propertyType.GetCustomAttributes(typeof(ClassConfigAttribute), true);

                                        if (classAttributes.Length == 1 && classAttributes[0] is ClassConfigAttribute)
                                        {
                                            ClassConfigAttribute classAttribute = (ClassConfigAttribute)classAttributes[0];
                                            if (!string.IsNullOrEmpty(classAttribute.DerivedTypesColumnName))
                                            {
                                                PropertyInfo derivedTypesProperty = propertyType.GetProperty(classAttribute.DerivedTypesColumnName);
                                                if (derivedTypesProperty != null)
                                                {
                                                    object[] derivedTypesPropertyAttributes = derivedTypesProperty.GetCustomAttributes(typeof(DerivedTypePropertyAttribute), true);
                                                    if (derivedTypesPropertyAttributes.Length > 0)
                                                    {
                                                        if (dataRow[classAttribute.DerivedTypesColumnName] != null)
                                                        {
                                                            string typeNickName = dataRow[classAttribute.DerivedTypesColumnName].ToString();
                                                            foreach (DerivedTypePropertyAttribute derivedTypePropertyAttribute in derivedTypesPropertyAttributes)
                                                            {
                                                                if (derivedTypePropertyAttribute.Name == typeNickName)
                                                                {
                                                                    parent = System.Activator.CreateInstance(derivedTypePropertyAttribute.Type);
                                                                    propertyInfo.SetValue(o, parent, null);
                                                                    break;
                                                                }
                                                            }
                                                        }
                                                    }

                                                }
                                            }
                                        }

                                        if (parent == null)
                                        {
                                            if (!parentPropertyAttribute.NonRecursive)
                                            {
                                                parent = System.Activator.CreateInstance(propertyType);
                                                propertyInfo.SetValue(o, parent, null);
                                            }
                                        }
                                    }
                                    if (!parentPropertyAttribute.NonRecursive)
                                    {
                                        Load(parentRow, parent);
                                    }
                                }
                            }
                            break;

                        case PropertyType.Children:
                            ChildrenPropertyAttribute childrenPropertyAttribute = (ChildrenPropertyAttribute)propertyAttribute;
                            relationName = childrenPropertyAttribute.RelationName ?? propertyInfo.Name;
                            DataRow[] childrenRows = dataRow.GetChildRows(relationName);
                
                            object children = propertyInfo.GetValue(o, null);
                            object child = null;
                            if (children != null)
                            {
                                bool isDictionary = children is IDictionary;

                                IDictionary dictionary = null;
                                DataColumn dictionaryKeyColumn = null;
                                if (isDictionary)
                                {
                                    dictionary = (IDictionary)children;
                                    string dictionaryKeyColumnName = childrenPropertyAttribute.DictionaryKeyColumnName;
                                    
                                    DataRelation dataRelation = dataRow.Table.DataSet.Relations[relationName];

                                    DataTable childrenTable = dataRelation.ChildTable;

                                    if (childrenTable.Columns.Contains(dictionaryKeyColumnName))
                                    {
                                        dictionaryKeyColumn = childrenTable.Columns[dictionaryKeyColumnName];
                                    }
                                   

                                }

                                foreach (DataRow childRow in childrenRows)
                                {
                                    if (isDictionary && dictionaryKeyColumn != null)
                                    {
                                        object key = childRow[dictionaryKeyColumn];
                                        if (dictionary.Contains(key))
                                        {
                                            child = dictionary[key];
                                        }
                                    }
                                    else if (children is IList)
                                    {
                                        child = Activator.CreateInstance(childrenPropertyAttribute.Type);
                                        ((IList)children).Add(child);
                                    }
                                    else
                                    {
                                        throw new NotSupportedException();
                                    }

                                    if (child!=null)
                                        Load(childRow, child);
                                }

                            }

                            break;

                        default:
                            throw new NotSupportedException();
                    }
                    
                }
            }
        }

        private string GetChildrenPropertyName(DataRelation childrenRelation)
        {
            string relationName = childrenRelation.RelationName;
            string childrenPropertyName = relationName.TrimEnd(childrenRelation.ParentTable.TableName.ToCharArray()).TrimEnd(childrenRelation.ChildTable.TableName.ToCharArray());
            return childrenPropertyName;
            
        }

        private DataRow AddRow(object o, DataSet dataSet, string tableName)
        {
            DataTable dataTable = GetDataTable(dataSet, tableName);

            Type type = o.GetType();
            PropertyInfo[] properties = type.GetProperties(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);

            CompleteDataTableStructure(o, type, dataTable);

            DataRow dataRow = dataTable.NewRow();
            FillDataRow(o, dataRow, properties, dataTable);
            dataTable.Rows.Add(dataRow);
            return dataRow;
        }

        private void FillDataRow(object o, DataRow dataRow, PropertyInfo[] properties, DataTable dataTable)
        {
            foreach (PropertyInfo propertyInfo in properties)
            {
                object[] propertyAttributes = propertyInfo.GetCustomAttributes(typeof(PropertyAttribute), true);
                if (propertyAttributes.Length == 1 && propertyAttributes[0] is PropertyAttribute)
                {
                    PropertyAttribute propertyAttribute = (PropertyAttribute)propertyAttributes[0];

                    switch (propertyAttribute.PropertyType)
                    {
                        case PropertyType.Column:
                            SetColumnPropertyValue(o, propertyInfo, propertyAttribute, dataTable, dataRow);
                            break;

                        case PropertyType.Parent:
                            ParentPropertyAttribute parentPropertyAttribute = (ParentPropertyAttribute)propertyAttribute;
                            string parentTableName = parentPropertyAttribute.TableName;
                            object parent = propertyInfo.GetValue(o, null);
                            if (parent != null)
                            {
                                DataRow parentRow = AddRow(parent, dataTable.DataSet, parentTableName);
                                dataRow.SetParentRow(parentRow);
                            }
                            break;

                        case PropertyType.Children:
                            ChildrenPropertyAttribute childrenPropertyAttribute = (ChildrenPropertyAttribute)propertyAttribute;
                            string childrenTableName = childrenPropertyAttribute.TableName;
                            object children = propertyInfo.GetValue(o, null);
                            if (children != null)
                            {
                                IEnumerable list = null;
                                if (children is IDictionary)
                                {
                                    list = (IEnumerable)((IDictionary)children).Values;
                                }
                                else if (children is IList)
                                {
                                    list = (IEnumerable)children;
                                }
                                if (list != null)
                                {
                                    foreach (object child in list)
                                    {
                                        if (child != null)
                                        {
                                            DataRow childRow = AddRow(child, dataTable.DataSet, childrenTableName);
                                            childRow.SetParentRow(dataRow);
                                        }
                                    }
                                }
                            }
                            break;

                        default:
                            throw new NotSupportedException();
                    }
                }
            }
        }

        private void SetColumnPropertyValue(object o, PropertyInfo propertyInfo, PropertyAttribute propertyAttribute, DataTable dataTable, DataRow dataRow)
        {
            string columnName = propertyInfo.Name;
            DataColumn dataColumn = dataTable.Columns[columnName];
            object value = propertyInfo.GetValue(o, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance, null, null, null);
            if (propertyInfo.PropertyType.BaseType != null && propertyInfo.PropertyType.BaseType.FullName == "System.Enum")
            {
                value = value.ToString();
            }
            
            dataRow[dataColumn] = value;
            
        }


        private void CompleteDataTableStructure(object o, Type type, DataTable dataTable)
        {
            PropertyInfo[] properties = type.GetProperties(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            
            foreach (PropertyInfo propertyInfo in properties)
            {
                object[] propertyAttributes = propertyInfo.GetCustomAttributes(typeof(PropertyAttribute), true);
                if (propertyAttributes.Length == 1 && propertyAttributes[0] is PropertyAttribute)
                {
                    PropertyAttribute propertyAttribute = (PropertyAttribute)propertyAttributes[0];

                    switch (propertyAttribute.PropertyType)
                    {
                        case PropertyType.Column:
                            ColumnPropertyAttribute columnPropertyAttribute = (ColumnPropertyAttribute)propertyAttribute;
                            CompleteDataColumn(dataTable, propertyInfo, propertyAttribute);
                            break;

                        case PropertyType.Parent:
                            ParentPropertyAttribute parentPropertyAttribute = (ParentPropertyAttribute)propertyAttribute;
                            object parent = null;
                            Type parentPropertyType = null;
                            if (o == null)
                            {
                                parentPropertyType = propertyInfo.PropertyType;
                            }
                            else
                            {
                                parent = propertyInfo.GetValue(o, null);
                                if (parent == null)
                                {
                                    parentPropertyType = propertyInfo.PropertyType;
                                }
                                else
                                {
                                    parentPropertyType = parent.GetType();
                                }
                            }
                                
                            PropertyInfo[] parentProperties = parentPropertyType.GetProperties(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
                            string parentTableName = parentPropertyAttribute.TableName;
                            DataTable parentDataTable = GetDataTable(dataTable.DataSet, parentTableName);
                            if (!parentPropertyAttribute.NonRecursive)
                                CompleteDataTableStructure(parent, parentPropertyType, parentDataTable);
                            DataColumn parentDataColumn = CompleteParentDataColumn(dataTable, propertyInfo, propertyAttribute, parentDataTable.PrimaryKey[0].DataType);
                            CreateRelation(dataTable, parentDataTable, parentDataColumn, parentPropertyAttribute);
                            
                            break;

                        case PropertyType.Children:
                            ChildrenPropertyAttribute childrenPropertyAttribute = (ChildrenPropertyAttribute)propertyAttribute;

                            Type childrenPropertyType = null;
                            if (o == null)
                            {
                                childrenPropertyType = childrenPropertyAttribute.Type;
                                CompleteChildrenStructure(null, childrenPropertyType, dataTable, propertyInfo, childrenPropertyAttribute);
                            }               
                            else
                            {
                                object children = propertyInfo.GetValue(o, null);
                                if (children != null)
                                {
                                    IEnumerable list = null;
                                    if (children is IList)
                                    {
                                        list = (IEnumerable)children;
                                        
                                    }
                                    else if (children is IDictionary)
                                    {
                                        IDictionary dictionary = (IDictionary)children;
                                        list = (IEnumerable)dictionary.Values;
                                    }
                                    foreach (object child in list)
                                    {
                                        if (child != null)
                                        {
                                            childrenPropertyType = child.GetType();
                                            //PropertyInfo[] childrenProperties = childrenPropertyType.GetProperties(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
                                            //string childrenTableName = childrenPropertyAttribute.TableName;
                                            //DataTable childrenDataTable = GetDataTable(dataTable.DataSet, childrenTableName);
                                            //CompleteDataTableStructure(child, childrenPropertyType, childrenDataTable);
                                            //DataColumn childrenDataColumn = CompleteParentDataColumn(childrenDataTable, propertyInfo, propertyAttribute, dataTable.PrimaryKey[0].DataType);
                                            //CreateRelation(childrenDataTable, dataTable, childrenDataColumn, childrenPropertyAttribute);

                                            CompleteChildrenStructure(child, childrenPropertyType, dataTable, propertyInfo, childrenPropertyAttribute);
                                        }
                                    }
                                
                                    
                                }
                            }

                            break;

                        default:
                            throw new NotSupportedException();
                    }
                }
            }
        }

        private void CompleteChildrenStructure(object child, Type childrenPropertyType, DataTable dataTable, PropertyInfo propertyInfo, ChildrenPropertyAttribute childrenPropertyAttribute)
        {
            PropertyInfo[] childrenProperties = childrenPropertyType.GetProperties(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            string childrenTableName = childrenPropertyAttribute.TableName;
            DataTable childrenDataTable = GetDataTable(dataTable.DataSet, childrenTableName);
            CompleteDataTableStructure(child, childrenPropertyType, childrenDataTable);
            DataColumn childrenDataColumn = CompleteParentDataColumn(childrenDataTable, propertyInfo, childrenPropertyAttribute, dataTable.PrimaryKey[0].DataType);
            CreateRelation(childrenDataTable, dataTable, childrenDataColumn, childrenPropertyAttribute);
        }

        private DataRelation CreateRelation(DataTable childTable, DataTable parentTable, DataColumn fkColumn, RelationPropertyAttribute relationPropertyAttribute)
        {
            string relationName = fkColumn.ColumnName;
            if (!string.IsNullOrEmpty(relationPropertyAttribute.RelationName))
                relationName = relationPropertyAttribute.RelationName;
            if (childTable.DataSet.Relations.Contains(relationName))
                return childTable.DataSet.Relations[relationName];
            else
            {
                DataRelation dataRelation = new DataRelation(relationName, parentTable.PrimaryKey[0], fkColumn, false);

                childTable.DataSet.Relations.Add(dataRelation);

                return dataRelation;
            }
        }


        private DataColumn CompleteDataColumn(DataTable dataTable, PropertyInfo propertyInfo, PropertyAttribute propertyAttribute)
        {
            string columnName = propertyInfo.Name;
            string caption = propertyAttribute.DisplayName;
            Type dataType = propertyInfo.PropertyType;
            
            if (dataType.BaseType != null && dataType.BaseType.FullName == "System.Enum")
            {
                dataType = typeof(string);
            }
            return CompleteDataColumn(dataTable, columnName, caption, dataType);
        }

        private DataColumn CompleteParentDataColumn(DataTable dataTable, PropertyInfo propertyInfo, PropertyAttribute propertyAttribute, Type dataType)
        {
            string columnName = propertyInfo.Name;
            string caption = propertyAttribute.DisplayName;
            return CompleteDataColumn(dataTable, columnName, caption, dataType);
        }

        private DataColumn CompleteDataColumn(DataTable dataTable, string columnName, string caption, Type dataType)
        {
            if (dataTable.Columns.Contains(columnName))
            {
                DataColumn dataColumn = dataTable.Columns[columnName];
                if (dataColumn.DataType != dataType)
                {
                    dataColumn.DataType = dataType;
                }
                return dataColumn;
            }
            else
            {
                DataColumn dataColumn = dataTable.Columns.Add(columnName);
                dataColumn.DataType = dataType;
                dataColumn.Caption = caption;
                return dataColumn;
            }
        }

        
        private DataTable GetDataTable(DataSet dataSet, string tableName)
        {
            if (dataSet.Tables.Contains(tableName))
                return dataSet.Tables[tableName];
            else
            {
                DataTable dataTable = dataSet.Tables.Add(tableName);
                DataColumn dataColumn = dataTable.Columns.Add("ID");
                dataColumn.DataType = typeof(Int32);
                dataColumn.AutoIncrement = true;
                dataTable.PrimaryKey = new DataColumn[1] { dataColumn };

                return dataTable;
            }
        }

    }
}
