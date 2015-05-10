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

        public void Copy(object t, object o)
        {
            Copy(t, o, false);

        }

        public void Copy(object t, object o, bool skipDoNotCopy)
        {
            Type tType = t.GetType();
            Type oType = o.GetType();
            //PropertyInfo[] tProperties = tType.GetProperties(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            PropertyInfo[] tProperties = GetProperties(tType);
            
            foreach (PropertyInfo tPropertyInfo in tProperties)
            {
                //object[] propertyAttributes = tPropertyInfo.GetCustomAttributes(typeof(PropertyAttribute), true);
                PropertyAttribute propertyAttribute = GetPropertyAttribute(tPropertyInfo);
                if (propertyAttribute != null)
                {
                    //PropertyAttribute propertyAttribute = (PropertyAttribute)propertyAttributes[0];

                    if (!(skipDoNotCopy && propertyAttribute.DoNotCopy))
                    {
                        PropertyInfo oPropertyInfo = oType.GetProperty(tPropertyInfo.Name);
                        object value = tPropertyInfo.GetValue(t, null);

                        MethodInfo setter = oPropertyInfo.GetSetMethod();
                        bool hasSetter = setter != null && setter.IsPublic;

                        switch (propertyAttribute.PropertyType)
                        {
                            case PropertyType.Column:


                                if (hasSetter)
                                {
                                    oPropertyInfo.SetValue(o, value, null);
                                }

                                break;

                            case PropertyType.Parent:

                                if (hasSetter)
                                {
                                    oPropertyInfo.SetValue(o, value, null);
                                }
                                break;


                            case PropertyType.Children:

                                if (hasSetter)
                                {
                                    oPropertyInfo.SetValue(o, value, null);
                                }
                                break;



                            default:
                                throw new NotSupportedException();
                        }

                    }
                }
            }
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

            AddRow(o, dataSet, tableName, new Dictionary<object,DataRow>());

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
                    Load(dataTable.Rows[0], root, new Dictionary<DataRow,object>());
                }

            }
        }

        protected Dictionary<Type, PropertyInfo[]> typeProperties = new Dictionary<Type, PropertyInfo[]>();
        protected Dictionary<PropertyInfo, PropertyAttribute> propertyInfoAttribute = new Dictionary<PropertyInfo, PropertyAttribute>();

        protected PropertyAttribute GetPropertyAttribute(PropertyInfo propertyInfo)
        {
            if (propertyInfoAttribute.ContainsKey(propertyInfo))
            {
                return propertyInfoAttribute[propertyInfo];
            }
            else
            {
                object[] propertyAttributes = propertyInfo.GetCustomAttributes(typeof(PropertyAttribute), true);
                if (propertyAttributes.Length == 1)
                {
                    PropertyAttribute propertyAttribute = null;
                    propertyAttribute = (PropertyAttribute)propertyAttributes[0];
                    propertyInfoAttribute.Add(propertyInfo, propertyAttribute);
                    return propertyAttribute;

                }
                else
                {
                    propertyInfoAttribute.Add(propertyInfo, null);
                    return null;
                }
            }
            
        }

        protected PropertyInfo[] GetProperties(Type type)
        {
            PropertyInfo[] properties = null; 
            if (typeProperties.ContainsKey(type))
            {
                properties = typeProperties[type];
            }
            else
            {
                properties = type.GetProperties(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
                typeProperties.Add(type, properties);
            }
            return properties;
        }

        public void Load(DataRow dataRow, object o, Dictionary<DataRow, object> loadedObjects)
        {
            Type type = o.GetType();

            PropertyInfo[] properties = GetProperties(type);
            

            foreach (PropertyInfo propertyInfo in properties)
            {
                string relationName;
                //object[] propertyAttributes = propertyInfo.GetCustomAttributes(typeof(PropertyAttribute), true);
                PropertyAttribute propertyAttribute = GetPropertyAttribute(propertyInfo);
                if (propertyAttribute != null)
                {
                    //PropertyAttribute propertyAttribute = (PropertyAttribute)propertyAttributes[0];

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
                                        if (propertyAttribute.Default != null)
                                        {
                                            value = propertyAttribute.Default;
                                        }
                                    }
                                    else
                                    {
                                        if (propertyInfo.PropertyType.BaseType != null && propertyInfo.PropertyType.BaseType.FullName == "System.Enum")
                                        {
                                            
                                            
                                            if (Enum.IsDefined(propertyInfo.PropertyType, value.ToString()))
                                            {
                                                value = Enum.Parse(propertyInfo.PropertyType, value.ToString());
                                            }
                                            else
                                            {
                                                value = null;
                                            }
                                            
                                        }

                                    }
                                    try
                                    {
                                        propertyInfo.SetValue(o, value, null);
                                    }
                                    catch { }
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

                                        if (loadedObjects.ContainsKey(parentRow))
                                        {
                                            parent = loadedObjects[parentRow];
                                            propertyInfo.SetValue(o, parent, null);
                                        }
                                        else
                                        {
                                            
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
                                                                        loadedObjects.Add(parentRow, parent);
                                                                        propertyInfo.SetValue(o, parent, null);
                                                                        break;
                                                                    }
                                                                }
                                                            }
                                                        }
                                                    }
                                                }
                                            }
                                        }

                                        if (parent == null)
                                        {
                                            parent = System.Activator.CreateInstance(propertyType);
                                            loadedObjects.Add(parentRow, parent);
                                            propertyInfo.SetValue(o, parent, null);
                                        }
                                    }
                                    Load(parentRow, parent, loadedObjects);
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
                                    Type childrenType = childrenPropertyAttribute.Type;
                                    
                                    DataRelation dataRelation = dataRow.Table.DataSet.Relations[relationName];

                                    if (dataRelation == null)
                                        dataRelation = CompleteDataRelationStructure(dataRow.Table.DataSet, dataRow.Table, childrenPropertyAttribute.TableName ?? childrenType.Name, childrenType, propertyInfo, childrenPropertyAttribute);

                                    DataTable childrenTable = dataRelation.ChildTable;

                                    if (childrenTable.Columns.Contains(dictionaryKeyColumnName))
                                    {
                                        dictionaryKeyColumn = childrenTable.Columns[dictionaryKeyColumnName];
                                    }
                                   

                                }

                                if (children is IList)
                                {
                                    ((IList)children).Clear();
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
                                        else
                                        {
                                            child = null;
                                            try
                                            {
                                                if (!childrenPropertyAttribute.Type.IsAbstract)
                                                {
                                                    child = Activator.CreateInstance(childrenPropertyAttribute.Type);
                                                    dictionary.Add(key, child);
                                                }
                                            }
                                            catch { }
                                        }
                                    }
                                    else if (children is IList)
                                    {
                                        if (!loadedObjects.ContainsKey(childRow))
                                        {
                                            child = Activator.CreateInstance(childrenPropertyAttribute.Type);
                                            ((IList)children).Add(child);
                                        }
                                    }
                                    else
                                    {
                                        throw new NotSupportedException();
                                    }

                                    if (child != null)
                                    {
                                        if (!loadedObjects.ContainsKey(childRow))
                                        {
                                            loadedObjects.Add(childRow, child);
                                            Load(childRow, child, loadedObjects);
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

            //OnObjectLoaded(new ObjectLoadedEventArgs(dataRow, o, loadedObjects, this));
        }

        public delegate void ObjectLoadedEventHandler(object sender, ObjectLoadedEventArgs e);
        public event ObjectLoadedEventHandler ObjectLoaded;

        public virtual void OnObjectLoaded(ObjectLoadedEventArgs e)
        {
            if (ObjectLoaded != null)
            {
                ObjectLoaded(this, e);
            }
        }
        
        private DataRelation CompleteDataRelationStructure(DataSet dataSet, DataTable parentTable, string childrenTableName, Type childrenType, PropertyInfo property, RelationPropertyAttribute relationPropertyAttribute)
        {
            DataTable childrenTable = GetDataTable(dataSet, childrenTableName);
            
            CompleteDataTableStructure(null, childrenType, childrenTable);



            DataColumn parentDataColumn = CompleteParentDataColumn(childrenTable, property, relationPropertyAttribute, parentTable.PrimaryKey[0].DataType);
            return CreateRelation(childrenTable, parentTable, parentDataColumn, relationPropertyAttribute);
        }

        private string GetChildrenPropertyName(DataRelation childrenRelation)
        {
            string relationName = childrenRelation.RelationName;
            string childrenPropertyName = relationName.TrimEnd(childrenRelation.ParentTable.TableName.ToCharArray()).TrimEnd(childrenRelation.ChildTable.TableName.ToCharArray());
            return childrenPropertyName;
            
        }

        private DataRow AddRow(object o, DataSet dataSet, string tableName, Dictionary<object, DataRow> loadedRows)
        {
            DataTable dataTable = GetDataTable(dataSet, tableName);
            DataRow dataRow = null;

            

            Type type = o.GetType();
            PropertyInfo[] properties = type.GetProperties(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);

            CompleteDataTableStructure(o, type, dataTable);

            dataRow = dataTable.NewRow();
            FillDataRow(o, dataRow, properties, dataTable, loadedRows);
            dataTable.Rows.Add(dataRow);
            return dataRow;
        }

        private int? GetID(object o)
        {
            PropertyInfo idProperty = o.GetType().GetProperty("ID",BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            if (idProperty == null)
                return null;

            object id = idProperty.GetValue(o, null);

            if (id == null)
                return null;

            if (id is int)
                return (int)id;

            return null;
        }

        private void FillDataRow(object o, DataRow dataRow, PropertyInfo[] properties, DataTable dataTable, Dictionary<object, DataRow> loadedRows)
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
                                DataRow parentRow;
                                if (loadedRows.ContainsKey(parent))
                                {
                                    parentRow = loadedRows[parent];
                                }
                                else
                                {
                                    parentRow = AddRow(parent, dataTable.DataSet, parentTableName, loadedRows);
                                    loadedRows.Add(parent, parentRow);
                                }
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
                                    bool hasChildren = false;
                                    foreach (object child in list)
                                    {
                                        if (child != null)
                                        {
                                            hasChildren = true;
                                            DataRow childRow = AddRow(child, dataTable.DataSet, childrenTableName, loadedRows);
                                            childRow.SetParentRow(dataRow);
                                        }
                                    }
                                    if (!hasChildren)
                                        CompleteDataRelationStructure(dataTable.DataSet, dataTable, childrenPropertyAttribute.TableName ?? childrenPropertyAttribute.Type.Name, childrenPropertyAttribute.Type, propertyInfo, childrenPropertyAttribute);
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
            if (dataTable.TableName == "Workspace" && columnName == "ID")
            {
                int x = 1;
                x++;
            }
            DataColumn dataColumn = dataTable.Columns[columnName];
            object value = propertyInfo.GetValue(o, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance, null, null, null);
            if (propertyInfo.PropertyType.BaseType != null && propertyInfo.PropertyType.BaseType.FullName == "System.Enum")
            {
                value = value.ToString();
            }
            if (columnName == "ID")
            {
                int? id = GetID(o);
                if (id.HasValue && id != -1)
                {
                    if (dataTable.Rows.Find(id.Value) == null)
                    {
                        dataRow[dataColumn] = value;
                    }
                }
            }
            else
            {
                dataRow[dataColumn] = value;
            }
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
                            if (parentPropertyType != type)
                            {
                                CompleteDataTableStructure(parent, parentPropertyType, parentDataTable);
                            }
                            DataColumn parentDataColumn = CompleteParentDataColumn(dataTable, propertyInfo, propertyAttribute, parentDataTable.PrimaryKey[0].DataType);
                            CreateRelation(dataTable, parentDataTable, parentDataColumn, parentPropertyAttribute);
                            break;

                        case PropertyType.Children:
                            ChildrenPropertyAttribute childrenPropertyAttribute = (ChildrenPropertyAttribute)propertyAttribute;

                            Type childrenPropertyType = null;
                            if (o == null)
                            {
                                childrenPropertyType = childrenPropertyAttribute.Type;
                                if (childrenPropertyType != type)
                                    CompleteChildrenStructure(null, childrenPropertyType, dataTable, propertyInfo, childrenPropertyAttribute);
                            }               
                            else
                            {
                                object children = propertyInfo.GetValue(o, null);
                                if (children != null)
                                {
                                    IEnumerable list = null;
                                    bool emptyList = true;
                                    if (children is IList)
                                    {
                                        list = (IEnumerable)children;
                                        emptyList = ((IList)children).Count == 0;
                                    }
                                    else if (children is IDictionary)
                                    {
                                        IDictionary dictionary = (IDictionary)children;
                                        list = (IEnumerable)dictionary.Values;
                                        emptyList = dictionary.Values.Count == 0;
                                    }

                                    if (emptyList)
                                    {
                                        childrenPropertyType = childrenPropertyAttribute.Type;
                                        //CompleteChildrenStructure(null, childrenPropertyType, dataTable, propertyInfo, childrenPropertyAttribute);
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
                //if (childTable.TableName.Equals(parentTable.TableName))
                //    dataRelation.Nested = true;

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
                DataColumn dataColumn = new DataColumn();
                dataColumn.ColumnName = columnName;
                dataColumn.DataType = dataType;
                dataColumn.Caption = caption;
                dataTable.Columns.Add(dataColumn);
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

    public class ObjectLoadedEventArgs : EventArgs
    {
        public DataRow DataRow { get; private set; }
        public object Obj { get; private set; }
        public Dictionary<DataRow, object> LoadedObjects { get; private set; }
        public Configurator Configurator { get; private set; }
        public ObjectLoadedEventArgs(DataRow dataRow, object o, Dictionary<DataRow, object> loadedObjects, Configurator configurator)
            : base()
        {
            DataRow = dataRow;
            Obj = o;
            LoadedObjects = loadedObjects;
            Configurator = configurator;
        }
    }
}
