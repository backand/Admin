using System;
using System.Data;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Text.RegularExpressions;

namespace Durados.DataAccess.AutoGeneration.Dynamic
{
    public class Mapper
    {
        public string FileName { get; set; }
        public string ConnectionString { get; set; }
        private MapDataSet mapDataSet = null;
        private Dictionary<string, MapDataSet.ViewRow> viewRows = null;
        public Durados.Diagnostics.ILogger Logger { get; set; }

        public Durados.DataAccess.Storage.IStorage Storage { get; set; }
        
        private void Log(string controller, string action, string method, Exception exception, int logType, string freeText)
        {
            if (Logger != null)
            {
                Logger.Log(controller, action, method, exception, logType, freeText);
            }
        }

        private MapDataSet MapDataSet
        {
            get
            {
                if (mapDataSet == null)
                {
                    mapDataSet = new MapDataSet();

                    viewRows = new Dictionary<string, MapDataSet.ViewRow>();

                    mapDataSet.View.RowChanged += new DataRowChangeEventHandler(View_RowChanged);
                    //mapDataSet.View.RowDeleted += new DataRowChangeEventHandler(View_RowDeleted);
                    mapDataSet.View.RowDeleting += new DataRowChangeEventHandler(View_RowDeleting);

                    //if (System.IO.File.Exists(FileName))
                    if (Storage.Exists(FileName))
                    {
                        try
                        {
                            //mapDataSet.ReadXml(FileName, XmlReadMode.ReadSchema);
                            Storage.ReadFromStorage(mapDataSet, FileName);
                        }
                        catch (ConstraintException ex)
                        {
                            string errMsg = string.Empty;
                            foreach (DataTable dt in mapDataSet.Tables)
                            {
                                if (dt.HasErrors)
                                {
                                    string rowErr = string.Empty;
                                    foreach (DataRow dr in dt.GetErrors())
                                    {
                                        rowErr += dr.RowError + "\n";
                                    }
                                    errMsg += string.Format("Errors occurred in Table {0} :\n{1}\n", dt.TableName, rowErr.TrimEnd('\n'));

                                }
                            }
                            throw new ConstraintException("Error occurred while loading xml file\n" + errMsg, ex);
                        }

                    }

                    //foreach (MapDataSet.ViewRow viewRow in MapDataSet.View)
                    //{
                    //    viewRows.Add(viewRow.Name, viewRow);
                    //}
                }

                return mapDataSet;
            }
        }

        void View_RowDeleting(object sender, DataRowChangeEventArgs e)
        {
            if (e.Action == DataRowAction.Delete)
            {
                MapDataSet.ViewRow viewRow = (MapDataSet.ViewRow)e.Row;
                viewRows.Remove(viewRow.Name);
            }
        }

        //void View_RowDeleted(object sender, DataRowChangeEventArgs e)
        //{
        //    if (e.Action == DataRowAction.Delete)
        //    {
        //        MapDataSet.ViewRow viewRow = (MapDataSet.ViewRow)e.Row;
        //        viewRows.Remove(viewRow.Name);
        //    }
        //}

        void View_RowChanged(object sender, DataRowChangeEventArgs e)
        {

            if (e.Action == DataRowAction.Add)
            {
                MapDataSet.ViewRow viewRow = (MapDataSet.ViewRow)e.Row;
                viewRows.Add(viewRow.Name, viewRow);
            }
        }

        //public void RemoveView(View view, DataSet dataset)
        //{
        //    DeleteView(view, dataset, false);
        //}

        public string[] GetDeletedViews(Database database)
        {
            List<string> deletedViews = new List<string>();
            SqlSchema sqlSchema = GetNewSqlSchema();
            IDbCommand command = sqlSchema.GetCommand();
            try
            {
                IDbConnection connection = sqlSchema.GetConnection(database.ConnectionString);

                command = sqlSchema.GetCommand();
                command.Connection = connection;
                command.Connection.Open();

                foreach (View view in database.Views.Values.Where(v => !v.SystemView && !v.IsCloned))
                {
                    if (!sqlSchema.IsTableOrViewExists(view.Name, command))
                    {
                        deletedViews.Add(view.Name);
                    }
                }
            }
            finally
            {
                try
                {
                    command.Connection.Close();
                }
                catch { }
            }
            return deletedViews.ToArray();
        }

        public void DeleteView(View view, DataSet dataset)
        {
            DeleteView(view, dataset, false);
        }

        public string GetIndexFieldsDisplayNames(string indexName, View view)
        {
            string tableName = string.IsNullOrEmpty(view.EditableTableName) ? view.DataTable.TableName : view.EditableTableName;
            Dictionary<string, object> indexColumns = GetIndexColumns(indexName, tableName, view.ConnectionString);

            string indexFieldsDisplayNames = string.Empty;

            foreach (string columnName in indexColumns.Keys)
            {
                Field field = view.GetFieldByColumnNames(columnName);
                if (field != null)
                {
                    indexFieldsDisplayNames += field.DisplayName + ", ";
                }
            }

            indexFieldsDisplayNames = indexFieldsDisplayNames.TrimEnd(' ').TrimEnd(',');

            return indexFieldsDisplayNames;
        }

        public Dictionary<string, object> GetIndexColumns(string indexName, string tableName, string connectionString)
        {
            SqlSchema sqlSchema = GetNewSqlSchema();
            IDbCommand command = sqlSchema.GetCommand();
            try
            {
                IDbConnection connection = sqlSchema.GetConnection(connectionString);

                command = sqlSchema.GetCommand();
                command.Connection = connection;
                command.Connection.Open();

                return sqlSchema.GetIndexColumns(indexName, tableName, command);
            }
            finally
            {
                command.Connection.Close();
            }
        }

        public void DeleteView2(View view, DataSet dataset)
        {
            string viewName = view.Name;

            if (!dataset.Tables.Contains(viewName))
            {
                throw new DuradosException("The view " + viewName + " does not exit.");
            }

            if (!viewRows.ContainsKey(viewName))
            {
                throw new DuradosException("Cannot delete a design-time view.");
            }

            List<string> parentTableRelations = new List<string>();

            foreach (DataRelation relation in dataset.Relations)
            {
                if (relation.ParentTable.TableName == viewName)
                {
                    if (!relation.ParentTable.Equals(relation.ChildTable))
                    {
                        parentTableRelations.Add(relation.RelationName);
                    }
                }
            }

            if (parentTableRelations.Count > 0)
            {
                throw new DuradosException("The following table are related as children:" + parentTableRelations.ToArray().Delimited() + ". Automatic delete is not implemented yet.");
            }


            List<DataRelation> childTableRelations = new List<DataRelation>();

            foreach (DataRelation relation in dataset.Relations)
            {
                if (relation.ChildTable.TableName == viewName)
                {
                    childTableRelations.Add(relation);
                }
            }

            foreach (DataRelation relation in childTableRelations)
            {
                dataset.Relations.Remove(relation);
                if (dataset.Tables[viewName].Constraints.Contains(relation.RelationName))
                    dataset.Tables[viewName].Constraints.Remove(relation.RelationName);
            }

            dataset.Tables.Remove(viewName);

            foreach (MapDataSet.FieldRow fieldRow in viewRows[viewName].GetFieldRows())
            {
                if (fieldRow.RelationRow != null)
                {
                    MapDataSet.Relation.RemoveRelationRow(fieldRow.RelationRow);
                }
            }
            foreach (DataRow row in viewRows[viewName].GetFieldRows())
            {
                MapDataSet.Field.Rows.Remove(row);
            }

            MapDataSet.View.Rows.Remove(viewRows[viewName]);


        }

        public void DeleteView(View view, DataSet dataset, bool drop)
        {
            string viewName = view.Name;

            if (!dataset.Tables.Contains(viewName))
            {
                throw new DuradosException("The view " + viewName + " does not exit.");
            }

            if (!viewRows.ContainsKey(viewName))
            {
                throw new DuradosException("Cannot delete a design-time view.");
            }

            List<string> parentTableRelations = new List<string>();

            foreach (DataRelation relation in dataset.Relations)
            {
                if (relation.ParentTable.TableName == viewName)
                {
                    if (!relation.ParentTable.Equals(relation.ChildTable))
                    {
                        parentTableRelations.Add(relation.RelationName);
                    }
                }
            }

            if (parentTableRelations.Count > 0)
            {
                throw new DuradosException("The following table are related as children:" + parentTableRelations.ToArray().Delimited() + ". Automatic delete is not implemented yet.");
            }

            SqlSchema sqlSchema = GetNewSqlSchema();
            IDbCommand command = sqlSchema.GetCommand();
            IDbConnection connection = sqlSchema.GetConnection(ConnectionString);

            command = sqlSchema.GetCommand();
            command.Connection = connection;
            command.Connection.Open();
            command.Transaction = connection.BeginTransaction();

            try
            {
                if (drop)
                {
                    if (sqlSchema.IsViewExists(viewName, command))
                    {
                        sqlSchema.DropView(viewName, command);
                        if (!string.IsNullOrEmpty(view.EditableTableName))
                        {
                            if (sqlSchema.IsTableExists(view.EditableTableName, command))
                            {
                                sqlSchema.DropTable(view.EditableTableName, command);

                            }
                        }
                    }
                    else
                    {
                        if (sqlSchema.IsTableExists(viewName, command))
                        {
                            sqlSchema.DropTable(viewName, command);

                        }
                    }
                }

                List<DataRelation> childTableRelations = new List<DataRelation>();

                foreach (DataRelation relation in dataset.Relations)
                {
                    if (relation.ChildTable.TableName == viewName)
                    {
                        childTableRelations.Add(relation);
                    }
                }

                foreach (DataRelation relation in childTableRelations)
                {
                    dataset.Relations.Remove(relation);
                    if (dataset.Tables[viewName].Constraints.Contains(relation.RelationName))
                        dataset.Tables[viewName].Constraints.Remove(relation.RelationName);
                }

                dataset.Tables.Remove(viewName);

                foreach (MapDataSet.FieldRow fieldRow in viewRows[viewName].GetFieldRows())
                {
                    if (fieldRow.RelationRow != null)
                    {
                        MapDataSet.Relation.RemoveRelationRow(fieldRow.RelationRow);
                    }
                }
                foreach (DataRow row in viewRows[viewName].GetFieldRows())
                {
                    MapDataSet.Field.Rows.Remove(row);
                }

                MapDataSet.View.Rows.Remove(viewRows[viewName]);

                command.Transaction.Commit();
            }
            catch (Exception exception)
            {
                if (command != null && command.Transaction != null)
                    command.Transaction.Rollback();
                throw exception;
            }
            finally
            {
                connection.Close();
            }
        }

        public void CreateField(string viewName, string fieldName, string dbType, string parentViewName, DataSet ds)
        {
            if (!ds.Tables.Contains(viewName))
            {
                MapDataSet.View.AddViewRow(viewName);
            }

            MapDataSet.FieldRow fieldRow = Persist(viewName, fieldName, dbType, parentViewName, ds);

            AddField(ds, ds.Tables[viewName], fieldRow, false);

        }

        public void Sync(View view, string configViewPk, View configFieldView, Field defaultField, DataSet dataset, Database database, ParentField defaultParentField)
        {
            Sync(view, configViewPk, configFieldView, defaultField, dataset, database, defaultParentField, null);
        }

        public string CreateField(View view, string configViewPk, View configFieldView, Field defaultField, ParentField defaultParentField, Dictionary<string, object> values, Database defaultDatabase, Dictionary<string, string> viewDefaults, DataSet dataset, out string manyToManyViewName)
        {
            return CreateField(view, configViewPk, configFieldView, defaultField, defaultParentField, values, defaultDatabase, viewDefaults, dataset, null, out manyToManyViewName);
        }

        public string CreateField(View view, string configViewPk, View configFieldView, Field defaultField, ParentField defaultParentField, Dictionary<string, object> values, Database defaultDatabase, Dictionary<string, string> viewDefaults, DataSet dataset, IDbCommand command, out string manyToManyViewName)
        {
            manyToManyViewName = null;
            SqlSchema sqlSchema = GetNewSqlSchema();
            
            string displayName = values["DisplayName"].ToString();
            string fieldName = displayName.Replace(" ", "_");
            string columnName = fieldName;

            string newDataTypeString = values["DataType"].ToString();

            if (string.IsNullOrEmpty(newDataTypeString))
                throw new DuradosException("Please enter a Data Type.");

            DataType dataType = (DataType)Enum.Parse(typeof(DataType), newDataTypeString);

            DataType newDataType = dataType;
            if (dataType == DataType.SingleSelect || dataType == DataType.MultiSelect || dataType == DataType.ImageList)
                newDataType = DataType.ShortText;

            if (values.ContainsKey("Formula") && values["Formula"] != null && values["Formula"].ToString().Trim() != string.Empty)
            {

                defaultField.IsCalculated = true;

                string formula = values["Formula"].ToString().Trim();

                if (formula != null)
                {
                    //Replace display names with field names + security validation
                    formula = DataAccessHelper.ReplaceFieldDisplayNames(formula, true, view);
                }

                defaultField.Formula = formula;

                defaultField.DataType = newDataType;

                defaultField.DisplayName = displayName;

                IDataTableAccess sa = DataAccessHelper.GetDataTableAccess(view);


                string SqlSelectQuery = sqlSchema.GetFormula(sa.GetCalculatedFieldStatement(defaultField, fieldName, view), view.DataTable.TableName);

                //string SqlSelectQuery = "SELECT TOP 1 " + sa.GetCalculatedFieldStatement(defaultField, fieldName,view) + " FROM [" + view.DataTable.TableName + "]";


                try
                {
                    sa.ExecuteNonQuery(view.ConnectionString, SqlSelectQuery, view.Database.SqlProduct);
                }
                catch (Exception exception)
                {
                    throw new DuradosException("The statement - " + SqlSelectQuery + " is incorrect, " + exception.Message);
                }

                Type fieldType = GetTypeofDataType(newDataType);

                AddCalculatedFieldToConfiguration(view, dataset, displayName, fieldName, fieldType, defaultField, new ConfigAccess(), configViewPk, configFieldView);

                //Don't create calculated field in DB
                return fieldName;
            }

            
            bool byItself = command == null;
            if (byItself)
            {
                IDbConnection connection = sqlSchema.GetConnection(ConnectionString);

                command = sqlSchema.GetCommand();
                command.Connection = connection;
                command.Connection.Open();
                command.Transaction = connection.BeginTransaction();

            }

            try
            {
                string tableName = string.IsNullOrEmpty(view.EditableTableName) ? view.Name : view.EditableTableName;
                sqlSchema.AddNewColumnToTable(tableName, fieldName, newDataType, command);
                if (!string.IsNullOrEmpty(view.EditableTableName) && view.EditableTableName != view.Name)
                    sqlSchema.AddNewColumnToView(view.Name, fieldName, command);

                Sync(view, configViewPk, configFieldView, defaultField, dataset, null, null, command);

                if (dataType == DataType.MultiSelect || dataType == DataType.SingleSelect || dataType == DataType.ImageList)
                {
                    ConfigAccess configAccess = new ConfigAccess();
                    string configFieldPK = configAccess.GetFieldPK(view.Name, fieldName, configFieldView.Database.ConnectionString);
                    columnName = ChangeDataType(view, newDataType.ToString(), dataType.ToString(), configFieldPK, configFieldView.Database.Views["View"], configViewPk, configFieldView, defaultDatabase, defaultField, defaultParentField, values, viewDefaults, dataset, command, out manyToManyViewName);
                    try
                    {
                        if (!string.IsNullOrEmpty(view.EditableTableName) && view.EditableTableName != view.Name)
                        {
                            if (sqlSchema.IsColumnExists(view.Name, fieldName, command))
                            {
                                sqlSchema.RemoveColumnFromView(view.Name, fieldName, command);
                            }
                        }
                        sqlSchema.RemoveColumnFromTable(tableName, fieldName, command);
                        Sync(view, configViewPk, configFieldView, defaultField, dataset, null, null, command);

                    }
                    catch { }
                }

                if (byItself)
                {
                    command.Transaction.Commit();
                }
            }
            catch (Exception exception)
            {
                if (command != null && command.Transaction != null)
                    command.Transaction.Rollback();
                throw exception;
            }
            finally
            {
                if (command != null && command.Transaction != null)
                    command.Connection.Close();
            }
            return columnName;
        }

        private Type GetTypeofDataType(DataType dataType)
        {
            switch (dataType)
            {
                case DataType.Boolean:
                    return typeof(bool);

                case DataType.DateTime:
                    return typeof(DateTime);

                case DataType.LongText:
                    return typeof(string);

                case DataType.Numeric:
                    return typeof(double);

                case DataType.ImageList:

                case DataType.SingleSelect:
                    return typeof(int);

                case DataType.Image:
                case DataType.Url:
                case DataType.Email:
                case DataType.Html:
                case DataType.ShortText:
                    return typeof(string);

                default:
                    throw new DuradosException("Unsupported data type " + dataType.ToString());

            }
        }


        public void AddCalculatedFieldToConfiguration(View view, DataSet dataset, string displayName, string fieldName, Type fieldType, Field defaultField, ConfigAccess configAccess, string configViewPk, View configFieldView)
        {
            string viewName = view.Name;

            if (!dataset.Tables.Contains(viewName))
            {
                throw new DuradosException("The view " + viewName + " does not exist.");
            }

            if (dataset.Tables[viewName].Columns.Contains(fieldName))
            {
                throw new DuradosException("The view " + viewName + " already contains column with same name.");
            }

            List<DataColumn> columns = new List<DataColumn>();

            DataColumn newColumn = new DataColumn(fieldName, fieldType);
            //newColumn.MaxLength = column.MaxLength;
            //newColumn.ReadOnly = column.ReadOnly;
            //newColumn.AllowDBNull = column.AllowDBNull;
            //newColumn.Unique = column.Unique;
            //newColumn.AutoIncrement = column.AutoIncrement;

            newColumn.ExtendedProperties.Add("Calculated", true);
            columns.Add(newColumn);


            //Dictionary<string, string> displayNames = new Dictionary<string, string>();
            //displayNames.Add(fieldName, displayName);

            AddColumnsConfiguration(columns, defaultField, configAccess, configViewPk, configFieldView, 10, null);

            dataset.Tables[viewName].Columns.Add(newColumn);

            if (!viewRows.ContainsKey(viewName))
            {
                MapDataSet.View.AddViewRow(viewName);
            }

            Persist(viewRows[viewName], columns, new List<DataColumn>(), new List<DataColumn>(), null, null, null, null, null, null);

        }

        private void LoadExtendedProperties(DataColumn from, DataColumn to)
        {
            to.ExtendedProperties.Clear();
            foreach (object key in from.ExtendedProperties.Keys)
            {
                to.ExtendedProperties.Add(key, from.ExtendedProperties[key]);
            }
        }

        public void Sync(View view, string configViewPk, View configFieldView, Field defaultField, DataSet dataset, Database database, ParentField defaultParentField, IDbCommand command)
        {
            string viewName = view.Name;

            ConfigAccess configAccess = new ConfigAccess();

            if (!dataset.Tables.Contains(viewName))
            {
                throw new DuradosException("The view " + viewName + " does not exist.");
            }

            if (!viewRows.ContainsKey(viewName))
            {
                MapDataSet.View.AddViewRow(viewName);
            }

            string editableTableName = string.IsNullOrEmpty(view.EditableTableName) ? view.DataTable.TableName : view.EditableTableName;

            DataTable table = null;
            if (command == null)
                table = CreateTable(viewName, editableTableName, ConnectionString);
            else
                table = CreateTable(viewName, editableTableName, command);

            FixEncrytedColumnType(view, table);

            IEnumerable<DataColumn> newColumns = GetNewColumns(dataset.Tables[viewName], table);
            List<DataColumn> delColumns = GetDelColumns(dataset.Tables[viewName], table);
            for (int i = delColumns.Count - 1; i >= 0; i--)
            {
                DataColumn column = delColumns[i];
                if (view.Fields.ContainsKey(column.ColumnName) && view.Fields[column.ColumnName].IsCalculated)
                {
                    delColumns.Remove(column);
                }
            }
            IEnumerable<DataColumn> changedColumns = GetChangedColumns(dataset.Tables[viewName], table);

            foreach (DataColumn changedColumn in changedColumns)
            {
                DataColumn originalColumn = dataset.Tables[viewName].Columns[changedColumn.ColumnName];
                try
                {
                    if (!originalColumn.AutoIncrement)
                        originalColumn.DefaultValue = null;
                }
                catch { }
                originalColumn.DataType = changedColumn.DataType;
                try
                {
                    originalColumn.DefaultValue = changedColumn.DefaultValue;
                }
                catch { }
                originalColumn.AutoIncrement = changedColumn.AutoIncrement;
                if (originalColumn.DataType.Equals(typeof(string)))
                {
                    originalColumn.MaxLength = changedColumn.MaxLength;
                }
                originalColumn.ReadOnly = changedColumn.ReadOnly;
                originalColumn.AllowDBNull = changedColumn.AllowDBNull;
                originalColumn.Unique = changedColumn.Unique;
               
                LoadExtendedProperties(changedColumn, originalColumn);
            }




            int maxOrder = configAccess.GetMaxFieldOrder(viewName, "Order", configFieldView.Database.ConnectionString);
            maxOrder += 10;

            AddColumnsConfiguration(newColumns, defaultField, configAccess, configViewPk, configFieldView, maxOrder, null);

            foreach (DataColumn column in newColumns)
            {
                DataColumn newColumn = new DataColumn(column.ColumnName, column.DataType);
                if (newColumn.DataType.Equals(typeof(string)))
                {
                    newColumn.MaxLength = column.MaxLength;
                }
                newColumn.ReadOnly = column.ReadOnly;
                newColumn.AllowDBNull = column.AllowDBNull;
                newColumn.Unique = column.Unique;
                newColumn.AutoIncrement = column.AutoIncrement;
                LoadExtendedProperties(column, newColumn);
                
                dataset.Tables[viewName].Columns.Add(newColumn);
            }

            Dictionary<string, Dictionary<string, DataRelation>> relationDictionary = new Dictionary<string, Dictionary<string, DataRelation>>();

            if (delColumns.Count > 0)
            {
                foreach (DataRelation relation in dataset.Relations)
                {
                    foreach (DataColumn column in relation.ChildColumns)
                    {
                        if (!relationDictionary.ContainsKey(column.ColumnName))
                        {
                            relationDictionary.Add(column.ColumnName, new Dictionary<string, DataRelation>());
                        }

                        if (!relationDictionary[column.ColumnName].ContainsKey(relation.RelationName))
                        {
                            relationDictionary[column.ColumnName].Add(relation.RelationName, relation);
                        }
                    }
                }

            }

            foreach (DataColumn column in delColumns)
            {
                if (relationDictionary.ContainsKey(column.ColumnName))
                {
                    if (relationDictionary[column.ColumnName] != null)
                    {
                        try
                        {
                            foreach (DataRelation relation in relationDictionary[column.ColumnName].Values)
                            {
                                dataset.Relations.Remove(relation);
                                if (column.Table.Constraints.Contains(relation.RelationName))
                                    column.Table.Constraints.Remove(relation.RelationName);
                            }
                        }
                        catch
                        {
                            
                        }
                    }
                }

                if (dataset.Tables[viewName].PrimaryKey.Length == 1)
                {
                    if (dataset.Tables[viewName].PrimaryKey[0].ColumnName.Equals(column.ColumnName))
                    {
                        dataset.Tables[viewName].PrimaryKey = new DataColumn[0];
                    }
                }
                try
                {
                    dataset.Tables[viewName].Columns.Remove(column.ColumnName);
                }
                catch (System.ArgumentException)
                {
                    List<DataRelation> relations = new List<DataRelation>();
                    foreach (DataRelation relation in column.Table.ParentRelations)
                    {
                        if (relation.ChildColumns.Contains(column))
                        {
                            relations.Add(relation);
                        }
                    }
                    foreach (DataRelation relation in relations)
                    {
                        column.Table.ParentRelations.Remove(relation);
                    }
                    relations = new List<DataRelation>();
                    foreach (DataRelation relation in column.Table.ChildRelations)
                    {
                        if (relation.ParentColumns.Contains(column))
                        {
                            relations.Add(relation);
                        }
                    }
                    foreach (DataRelation relation in relations)
                    {
                        column.Table.ChildRelations.Remove(relation);
                    }
                    try
                    {
                        dataset.Tables[viewName].Columns.Remove(column.ColumnName);
                    }
                    catch { }
                }
            }

            if (table.PrimaryKey.Length == 1)
            {
                if (dataset.Tables[viewName].PrimaryKey.Length == 0 || !dataset.Tables[viewName].PrimaryKey[0].ColumnName.Equals(table.PrimaryKey[0].ColumnName))
                {
                    if (dataset.Tables[viewName].Columns.Contains(table.PrimaryKey[0].ColumnName))
                        dataset.Tables[viewName].PrimaryKey = new DataColumn[1] { dataset.Tables[viewName].Columns[table.PrimaryKey[0].ColumnName] };
                }

            }

            if (viewRows.ContainsKey(viewName))
            {
                IEnumerable<DataColumn> designTimeNonDeletedColumns = Persist(viewRows[viewName], newColumns, delColumns, changedColumns, database, dataset.Tables[viewName], view.EditableTableName, defaultParentField, dataset, command);


                foreach (DataColumn column in designTimeNonDeletedColumns)
                {
                    delColumns.Remove(column);
                }

                DelColumnsConfiguration(view, delColumns, configAccess, configViewPk, configFieldView);

                if (designTimeNonDeletedColumns.Count() > 0)
                {
                    string columnsNames = string.Empty;
                    foreach (DataColumn column in designTimeNonDeletedColumns)
                    {
                        columnsNames += column.ColumnName + ",";
                    }
                    columnsNames = columnsNames.TrimEnd(',');
                    throw new DuradosException("The operation succeeded, but the following design time columns were removed from the server and need to be manually removed from the dataset: " + columnsNames + ".");
                }
            }
        }

        private void FixEncrytedColumnType(View view, DataTable table)
        {
            foreach (DataColumn column in table.Columns)
            {
                if (view.Fields.ContainsKey(column.ColumnName) && column.DataType.Equals(typeof(byte[])))
                {
                    if (view.Fields[column.ColumnName].FieldType == FieldType.Column)
                    {
                        ColumnField columnField = (ColumnField)view.Fields[column.ColumnName];
                        if (columnField.Encrypted)
                        {
                            column.DataType = typeof(string);
                        }
                    }
                }
            }
        }

        private void DelColumnsConfiguration(View view, List<DataColumn> delColumns, ConfigAccess configAccess, string configViewPk, View configFieldView)
        {
            foreach (DataColumn column in delColumns)
            {
                string fieldName = string.Empty;
                Field field = view.GetFieldByColumnNames(column.ColumnName);
                if (field != null)
                    fieldName = field.Name;
                else
                    fieldName = column.ColumnName;

                string pk = configAccess.GetFieldPK(view.Name, fieldName, configFieldView.Database.ConnectionString);
                if (!string.IsNullOrEmpty(pk))
                    configAccess.Delete(configFieldView, pk, null, null, null);
            }
        }

        private IEnumerable<DataColumn> GetNewColumns(DataTable oldTable, DataTable newTable)
        {
            List<DataColumn> newColumns = new List<DataColumn>();

            foreach (DataColumn column in newTable.Columns)
            {
                if (!oldTable.Columns.Contains(column.ColumnName))
                {
                    newColumns.Add(column);
                }
            }

            return newColumns;
        }

        private IEnumerable<DataColumn> GetChangedColumns(DataTable oldTable, DataTable newTable)
        {
            List<DataColumn> changedColumns = new List<DataColumn>();

            foreach (DataColumn newColumn in newTable.Columns)
            {
                if (oldTable.Columns.Contains(newColumn.ColumnName))
                {
                    DataColumn oldColumn = oldTable.Columns[newColumn.ColumnName];
                    if (!oldColumn.AllowDBNull.Equals(newColumn.AllowDBNull) || !oldColumn.Unique.Equals(newColumn.Unique) || !oldColumn.DefaultValue.Equals(newColumn.DefaultValue) || !oldColumn.DataType.Equals(newColumn.DataType) || !oldColumn.AutoIncrement.Equals(newColumn.AutoIncrement) || !IsColumnPartOfPK(oldColumn).Equals(IsColumnPartOfPK(newColumn)))
                    {
                        changedColumns.Add(newColumn);
                    }
                }
            }

            return changedColumns;
        }


        private List<DataColumn> GetDelColumns(DataTable oldTable, DataTable newTable)
        {
            List<DataColumn> delColumns = new List<DataColumn>();

            if (oldTable == null) 
                return delColumns;

            foreach (DataColumn column in oldTable.Columns)
            {
                if (!newTable.Columns.Contains(column.ColumnName))
                {
                    delColumns.Add(column);
                }
            }

            return delColumns;
        }

        public string CreateView(DataTable table, Database database, View configView, View configFieldView, DataSet dataset, Field defaultField, ParentField defaultParentField, Dictionary<string, string> viewDefaults, bool createView)
        {
            IDbCommand command = null;
            try
            {
                ConfigAccess configAccess = new ConfigAccess();

                DataTable typedTable = GetTypedTable(table, 100, database);


                string viewName = CreateInDatabase(typedTable, createView, ref command);

                Dictionary<string, object> values = new Dictionary<string, object>();

                values.Add("Name", viewName);
                values.Add("EditableTableName", typedTable.TableName);
                values.Add("DisplayName", table.TableName);
                values.Add("Views_Parent", 0);

                values.Add("AllowCreate", true);
                values.Add("AllowDelete", true);
                values.Add("AllowEdit", true);
                values.Add("AllowDuplicate", true);
                values.Add("HideInMenu", false);
                values.Add("CreateDateColumnName", "CreateDate");
                values.Add("ModifiedDateColumnName", "ModifiedDate");
                values.Add("CreatedByColumnName", "CreateUserId");
                values.Add("ModifiedByColumnName", "ModifiedUserId");
                values.Add("PageSize", database.DefaultPageSize);
                values.Add("ColumnsInDialog", 2);
                values.Add("ImageSrcColumnName", "Image");
                values.Add("DuplicationMethod", "Shallow");
                values.Add("DuplicateMessage", "");
                values.Add("BaseTableName", string.Empty);
                values.Add("DisplayType", "Table");
                values.Add("SaveHistory", true);
                values.Add("WorkspaceID", -1);
                values.Add("ID", -1);
                values.Add("UseLikeInFilter", true);
                values.Add("Send", true);


                values.Add("ExportToCsv", true);
                values.Add("ImportFromExcel", true);
                values.Add("Print", false);
                values.Add("DataRowView", "Tabs");
                values.Add("Popup", true);
                values.Add("TabCache", true);
                values.Add("RefreshOnClose", false);

                values.Add("NewButtonName", "New");
                values.Add("EditButtonName", "Edit");
                values.Add("DuplicateButtonName", "Duplicate");
                values.Add("InsertButtonName", "Insert");
                values.Add("DeleteButtonName", "Delete");
                values.Add("PromoteButtonName", "Promote");
                values.Add("HideToolbar", false);

                values.Add("GridEditable", true);
                values.Add("GridEditableEnabled", true);

                values.Add("MaxSubGridHeight", 400);
                //values.Add("WorkspaceID", database.GetAdminWorkspaceId().ToString());

                foreach (string key in viewDefaults.Keys)
                {
                    values.Add(key, viewDefaults[key]);
                }

                DataRow viewRow = configAccess.GetNewRow(configView, values, null, null, null, null, null);

                string viewId = "ID";

                if (viewRow == null)
                {
                    throw new DuradosException("Could not add the view " + viewName + " to configuration.");
                }

                if (!viewRow.Table.Columns.Contains(viewId))
                {
                    throw new DuradosException("Could not add the view " + viewName + " to configuration.");
                }

                if (viewRow.IsNull(viewId))
                {
                    throw new DuradosException("Could not add the view " + viewName + " to configuration.");
                }

                string pk = viewRow[viewId].ToString();

                command.Transaction.Commit();

                Dictionary<string, string> displayNames = new Dictionary<string, string>();
                foreach (DataColumn column in typedTable.Columns)
                {
                    displayNames.Add(column.ColumnName, column.Caption);
                }

                CreateView(database, viewName, pk, configFieldView, dataset, defaultField, defaultParentField, typedTable.TableName, configView, displayNames);


                //UpdateColumnsDisplayNames(typedTable, viewRow, configAccess, configFieldView, view);


                return viewName;
            }
            catch (Exception exception)
            {
                if (command != null && command.Transaction != null)
                    command.Transaction.Rollback();
                throw new DuradosException(exception.Message, exception);
            }
            finally
            {
                command.Connection.Close();
                command.Dispose();
            }
        }




        public string CreateView(DataTable table, Database database, View configView, View configFieldView, DataSet dataset, Field defaultField, ParentField defaultParentField, Dictionary<string, string> viewDefaults, bool createView, IDbCommand command)
        {
            //IDbCommand command = null;
            try
            {
                ConfigAccess configAccess = new ConfigAccess();

                //DataTable typedTable = GetTypedTable(table, 100, database);


                //string viewName = CreateInDatabase(typedTable, createView, ref command);
                string viewName = table.TableName;

                Dictionary<string, object> values = new Dictionary<string, object>();

                values.Add("Name", viewName);
                //values.Add("EditableTableName", typedTable.TableName);
                values.Add("EditableTableName", viewName);
                values.Add("DisplayName", viewName.TrimStart(Database.SystemRelatedViewPrefix.ToCharArray()));
                values.Add("Views_Parent", 0);

                values.Add("AllowCreate", true);
                values.Add("AllowDelete", true);
                values.Add("AllowEdit", true);
                values.Add("AllowDuplicate", true);
                values.Add("HideInMenu", true);
                values.Add("CreateDateColumnName", "CreateDate");
                values.Add("ModifiedDateColumnName", "ModifiedDate");
                values.Add("CreatedByColumnName", "CreateUserId");
                values.Add("ModifiedByColumnName", "ModifiedUserId");
                values.Add("PageSize", database.DefaultPageSize);
                values.Add("ColumnsInDialog", 2);
                values.Add("ImageSrcColumnName", "Image");
                values.Add("DuplicationMethod", "Shallow");
                values.Add("DuplicateMessage", "");
                values.Add("BaseTableName", string.Empty);
                values.Add("DisplayType", "Table");
                values.Add("SaveHistory", true);
                if (viewDefaults.ContainsKey("WorkspaceID"))
                {
                    values.Add("WorkspaceID", viewDefaults["WorkspaceID"]);
                }
                else
                {
                    values.Add("WorkspaceID", database.GetDefaultWorkspace().ID);
                }
                values.Add("ID", -1);
                values.Add("UseLikeInFilter", true);
                values.Add("Send", true);


                values.Add("ExportToCsv", true);
                values.Add("ImportFromExcel", true);
                values.Add("Print", false);
                values.Add("DataRowView", "Tabs");
                values.Add("Popup", true);
                values.Add("TabCache", true);
                values.Add("RefreshOnClose", false);


                values.Add("NewButtonName", "New");
                values.Add("EditButtonName", "Edit");
                values.Add("DuplicateButtonName", "Duplicate");
                values.Add("InsertButtonName", "Insert");
                values.Add("DeleteButtonName", "Delete");

                values.Add("HideToolbar", false);

                values.Add("GridEditable", true);
                values.Add("GridEditableEnabled", true);

                values.Add("MaxSubGridHeight", 400);

                foreach (string key in viewDefaults.Keys)
                {
                    if (values.ContainsKey(key))
                    {
                        values[key] = viewDefaults[key];
                    }
                    else
                    {
                        values.Add(key, viewDefaults[key]);
                    }
                }

                DataRow viewRow = configAccess.GetNewRow(configView, values, null, null, null, null, null);

                string viewId = "ID";

                if (viewRow == null)
                {
                    throw new DuradosException("Could not add the view " + viewName + " to configuration.");
                }

                if (!viewRow.Table.Columns.Contains(viewId))
                {
                    throw new DuradosException("Could not add the view " + viewName + " to configuration.");
                }

                if (viewRow.IsNull(viewId))
                {
                    throw new DuradosException("Could not add the view " + viewName + " to configuration.");
                }

                string pk = viewRow[viewId].ToString();

                //command.Transaction.Commit();

                Dictionary<string, string> displayNames = new Dictionary<string, string>();
                foreach (DataColumn column in table.Columns)
                {
                    displayNames.Add(column.ColumnName, column.Caption);
                }

                CreateView(database, viewName, pk, configFieldView, dataset, defaultField, defaultParentField, viewName, configView, displayNames, command);


                //UpdateColumnsDisplayNames(typedTable, viewRow, configAccess, configFieldView, view);


                return viewName;
            }
            catch (Exception exception)
            {
                //command.Transaction.Rollback();
                throw new DuradosException(exception.Message, exception);
            }
            finally
            {
                //command.Connection.Close();
                //command.Dispose();
            }
        }

        private void UpdateColumnsDisplayNames(DataTable table, DataRow viewRow, ConfigAccess configAccess, View configFieldView, View view)
        {
            DataRow[] fieldsRows = viewRow.GetChildRows("Fields");

            foreach (DataRow row in fieldsRows)
            {
                string fieldName = row["Name"].ToString();
                if (table.Columns.Contains(fieldName))
                {
                    DataColumn column = table.Columns[fieldName];
                    //row["DisplayName"] = column.Caption;
                    Dictionary<string, object> values = new Dictionary<string, object>();
                    values.Add("DisplayName", column.Caption);
                    string pk = row["ID"].ToString();
                    configAccess.Edit(configFieldView, values, pk, null, null, null, null);
                    view.Fields[fieldName].DisplayName = column.Caption;
                }
            }
        }

        private string CreateInDatabase(DataTable table, bool createView, ref IDbCommand command)
        {
            string viewName = table.TableName;
            if (createView)
                viewName = "v_" + table.TableName;

            SqlSchema sqlSchema = GetNewSqlSchema();

            IDbConnection connection = sqlSchema.GetConnection(ConnectionString);

            command = sqlSchema.GetCommand();
            command.Connection = connection;
            command.Connection.Open();
            command.Transaction = connection.BeginTransaction();

            sqlSchema.CreateTable(table, command);

            if (createView)
            {
                sqlSchema.CreateView(viewName, sqlSchema.GetCreateView(table), command);
            }

            return viewName;
        }

        

        public string GetValidDbName(string name)
        {
            string invalidChars = "%^&()\\|?<>={}+-/ ]['\"@!#*.,$<> ";
            foreach (char c in invalidChars)
            {
                name = name.Replace(c, '_');
            }
            int i = 0;
            if (Int32.TryParse(name[0].ToString(), out i))
            {
                name = "_" + name;
            }
            return name;
            //return System.Text.RegularExpressions.Regex.Replace(tableName.Replace(" ", "_"), "[^A-Za-z0-9_]", string.Empty);
        }

        public DataTable GetTypedTable(DataTable table, int maxRows, Database database)
        {
            DataTable typedTable = new DataTable(GetValidDbName(table.TableName));

            DataSet dataset = new DataSet();

            dataset.Tables.Add(typedTable);

            if (table.Columns.Contains("Id"))
            {
                table.Columns.Remove("Id");
            }

            DataColumn idColumn = typedTable.Columns.Add("Id", typeof(int));
            idColumn.AutoIncrement = true;

            Type stringType = typeof(string);
            foreach (DataColumn column in table.Columns)
            {
                int maxLength = 250;
                string validDbName = GetValidDbName(column.ColumnName);
                string availableName = GetAvailableName(typedTable, validDbName);
                DataColumn typedColumn = typedTable.Columns.Add(availableName, GetColumnType(table, column, 100, database, out maxLength));
                if (typedColumn.DataType == stringType)
                {
                    typedColumn.MaxLength = maxLength;
                }
                typedColumn.Caption = column.ColumnName;
            }

            typedTable.PrimaryKey = new DataColumn[1] { idColumn };

            return typedTable;
        }

        private string GetAvailableName(DataTable table, string columnName)
        {
            if (!table.Columns.Contains(columnName))
                return columnName;

            string name = "Column";
            int i = 1;
            while (table.Columns.Contains(columnName))
            {
                columnName = name + i;
                i++;
            }

            return columnName;
        }

        protected Type GetColumnType(DataTable table, DataColumn column, int maxRows, Database database, out int maxLength)
        {
            int i = 0;

            maxLength = 250;

            Type stringType = typeof(string);
            if (column.DataType != stringType)
            {
                return column.DataType;
            }

            Type dateType = typeof(DateTime);
            Type numericType = typeof(double);
            Type booleanType = typeof(bool);

            DateTime minDate = new DateTime(1930, 1, 1);
            DateTime maxDate = new DateTime(2030, 1, 1);
            double minD = -657435d;
            double maxD = 2958465d;

            Type type = null;

            foreach (DataRow row in table.Rows)
            {
                if (i >= maxRows)
                    break;

                i++;

                string value = row.IsNull(column) ? string.Empty : row[column].ToString();

                if (!string.IsNullOrEmpty(value))
                {
                    DateTime dateTime = DateTime.Now;
                    double d = 0;

                    if (DateTime.TryParse(value, out dateTime))
                    {
                        if (type == null)
                        {
                            type = dateType;
                        }
                        else
                        {
                            if (type != dateType)
                            {
                                return stringType;
                            }
                        }
                    }
                    else if (value == database.True || value == database.False)
                    {
                        if (type == null)
                        {
                            type = booleanType;
                        }
                        else
                        {
                            if (type != booleanType)
                            {
                                return stringType;
                            }
                        }
                    }
                    else if (double.TryParse(value, out d))
                    {
                        bool isDate = false;
                        if (d > minD && d < maxD)
                        {

                            DateTime date = DateTime.FromOADate(d);

                            if (date > minDate && date < maxDate)
                            {
                                isDate = true;
                            }
                        }

                        if (isDate)
                        {
                            if (type == null)
                            {
                                type = dateType;
                            }
                            else
                            {
                                if (type != dateType)
                                {
                                    return stringType;
                                }
                            }
                        }
                        else
                        {

                            if (type == null)
                            {
                                type = numericType;
                            }
                            else
                            {
                                if (type != numericType)
                                {
                                    return stringType;
                                }
                            }
                        }
                    }
                    else
                    {
                        if (value.Length > 100)
                        {
                            maxLength = 4000;
                        }
                        type = stringType;
                        break;
                    }
                }
            }
            if (type == null)
                type = stringType;

            return type;
        }

        public void CreateView(Database database, string viewName, string configViewPk, View configFieldView, DataSet dataset, Field defaultField, ParentField defaultParentField, string editableTableName, View configView, Dictionary<string, string> displayNames)
        {
            SqlSchema sqlSchema = GetNewSqlSchema();
            
            IDbConnection connection = sqlSchema.GetConnection(ConnectionString);

            IDbCommand command = sqlSchema.GetCommand();
            command.Connection = connection;
            command.Connection.Open();
            command.Transaction = connection.BeginTransaction();

            try
            {
                CreateView(database, viewName, configViewPk, configFieldView, dataset, defaultField, defaultParentField, editableTableName, configView, displayNames, command);
                command.Transaction.Commit();

            }
            catch (Exception exception)
            {
                command.Transaction.Rollback();
                throw exception;
            }
            finally
            {
                connection.Close();
            }
        }

        private DataTable CreateTable(string name, IDbCommand command)
        {
            return CreateTable(name, name, command);
        }

        private DataTable CreateTable(string viewName, string editableTableName, IDbCommand command)
        {
            DataTable table = GetNewGenerator().CreateTable(viewName, editableTableName, command);

            if (table.PrimaryKey.Length > 1)
            {
                throw new DuradosException("Primary key can contain only one column");
            }

            return table;
        }

        private DataTable CreateTable(string name, string connectionString)
        {
            return CreateTable(name, name, connectionString);
        }

        public virtual Generator GetNewGenerator()
        {
            return new Generator();
        }

        private DataTable CreateTable(string viewName, string editableTableName, string connectionString)
        {
            SqlSchema sqlSchema = GetNewSqlSchema();

            using (IDbConnection connection = sqlSchema.GetConnection(ConnectionString))
            {

                using (IDbCommand command = sqlSchema.GetCommand())
                {
                    command.Connection = connection;
                    command.Connection.Open();

                    DataTable table = GetNewGenerator().CreateTable(viewName, editableTableName, command);

                    if (table.PrimaryKey.Length > 1)
                    {
                        throw new DuradosException("Primary key can contain only one column");
                    }

                    return table;
                }
            }
        }


        public void CreateView(Database database, string viewName, string configViewPk, View configFieldView, DataSet dataset, Field defaultField, ParentField defaultParentField, string editableTableName, View configView, Dictionary<string, string> displayNames, IDbCommand command)
        {
            ConfigAccess configAccess = new ConfigAccess();
            if (dataset.Tables.Contains(viewName))
            {
                configAccess.Delete(configView, configViewPk, null, null, null);
                throw new DuradosException("The view " + viewName + " already exits.");
            }

            SqlSchema sqlSchema = GetNewSqlSchema();

            DataTable table = null;
            if (!IsViewExistInServer(viewName, command))
            {
                table = GetNewTable(viewName);
                sqlSchema.CreateTable(table, command);
                //editableTableName = viewName;
                //viewName = "v_" + viewName;
                //sqlSchema.CreateView(viewName, "select Id from " + sqlSchema.sqlTextBuilder.EscapeDbObject(editableTableName), command);

                Dictionary<string, object> values = new Dictionary<string, object>();
                values.Add("Name", viewName);
                values.Add("DisplayName", editableTableName.GetDecamal());
                values.Add("JsonName", viewName.ReplaceNonAlphaNumeric2());
                //values.Add("EditableTableName", editableTableName);
                values.Add("GridEditable", true);
                values.Add("GridEditableEnabled", true);
                configAccess.Edit(configView, values, configViewPk, null, null, null, null);

            }

            //if (command == null)
            //    table = Generator.CreateTable(viewName, ConnectionString);
            //else
            table = CreateTable(viewName, command);

            if (table.Columns.Count == 0)
            {
                configAccess.Delete(configView, configViewPk, null, null, null);
                throw new DuradosException("The table or view " + viewName + " does not exist in the database server.");
            }

            if (table.PrimaryKey.Length == 0)
            {
                if (string.IsNullOrEmpty(editableTableName))
                {
                    configAccess.Delete(configView, configViewPk, null, null, null);
                    throw new DuradosException("The Table " + viewName + " has no primary key.");
                }
                else
                {
                    DataTable editableTable = null;
                    if (command == null)
                        editableTable = CreateTable(editableTableName, ConnectionString);
                    else
                        editableTable = CreateTable(editableTableName, command);

                    if (editableTable.PrimaryKey.Length == 0)
                    {
                        configAccess.Delete(configView, configViewPk, null, null, null);
                        throw new DuradosException("The Table " + editableTableName + " has no primary key.");
                    }
                    List<DataColumn> pk = new List<DataColumn>();
                    foreach (DataColumn pkColumn in editableTable.PrimaryKey)
                    {
                        if (!table.Columns.Contains(pkColumn.ColumnName))
                        {
                            configAccess.Delete(configView, configViewPk, null, null, null);
                            throw new DuradosException("The Editable Table " + editableTableName + " key does not exist in the view " + viewName + ".");
                        }
                        else
                        {
                            pk.Add(table.Columns[pkColumn.ColumnName]);
                        }
                    }

                    table.PrimaryKey = pk.ToArray();

                    if (editableTable.PrimaryKey.Length == 1 && editableTable.PrimaryKey[0].AutoIncrement)
                    {
                        table.PrimaryKey[0].AutoIncrement = true;
                    }

                    foreach (DataColumn column in table.Columns)
                    {
                        if (!editableTable.Columns.Contains(column.ColumnName))
                        {
                            if (column.ExtendedProperties.ContainsKey("NotInEditable"))
                            {
                                column.ExtendedProperties["NotInEditable"] = true;
                            }
                            else
                            {
                                column.ExtendedProperties.Add("NotInEditable", true);
                            }
                        }
                    }
                }
            }

            dataset.Tables.Add(table);

            List<DataColumn> columns = new List<DataColumn>();
            foreach (DataColumn column in table.Columns)
            {
                columns.Add(column);
            }

            AddColumnsConfiguration(columns, defaultField, configAccess, configViewPk, configFieldView, 10, displayNames);

            View newView = database.CreateView(table); // new View(table, database);
            database.Views.Add(viewName, newView);

            Persist(database, table, editableTableName, defaultParentField, dataset, command);
        }

        private void AddColumnsConfiguration(IEnumerable<DataColumn> columns, Field defaultField, ConfigAccess configAccess, string configViewPk, View configFieldView, int order, Dictionary<string, string> displayNames)
        {
            foreach (DataColumn column in columns)
            {
                Dictionary<string, object> values = new Dictionary<string, object>();
                values.Add("Fields_Parent", configViewPk);
                values.Add("Name", column.ColumnName);
                string jsonName= column.ColumnName.ReplaceNonAlphaNumeric2();
                configFieldView.ValidateUniqueFieldJsonName(jsonName);
                  
                values.Add("JsonName",jsonName );
                if (displayNames != null && displayNames.ContainsKey(column.ColumnName))
                {
                    values.Add("DisplayName", displayNames[column.ColumnName]);
                }
                else
                {
                    values.Add("DisplayName", column.ColumnName.GetDecamal());
                }
                values.Add("Required", !column.AllowDBNull);
                values.Add("Unique", column.Unique);
                values.Add("Order", order);
                values.Add("OrderForCreate", order);
                values.Add("OrderForEdit", order);
                //int? categoryId = ConfigAccess.GetCategoryId("General", configFieldView.Database.ConnectionString);
                int? categoryId = ConfigAccess.GetFirstCategoryId(configViewPk, configFieldView.Database.ConnectionString);
                if (!categoryId.HasValue)
                {
                    categoryId = ConfigAccess.AddCategory(configFieldView.Database.ConnectionString);
                }
                if (categoryId.HasValue)
                {
                    values.Add("Category_Parent", categoryId.ToString());
                }
                if (column.DataType.Equals(typeof(string)) && column.MaxLength > 300)
                {
                    values.Add("TextHtmlControlType", "TextArea");
                }
                order += 10;

                bool notInEditable = column.ExtendedProperties.ContainsKey("NotInEditable");

                if (column.AutoIncrement || column.ExtendedProperties.ContainsKey("Calculated") || column.DataType.Equals(typeof(byte[])) || notInEditable || (column.Table.PrimaryKey.Length > 0 && column.MaxLength == 36 && column == column.Table.PrimaryKey[0]))
                {
                    values.Add("HideInCreate", true);
                    values.Add("ExcludeInInsert", true);
                    //values.Add("HideInEdit", true);
                    values.Add("ExcludeInUpdate", true);
                }

                LoadNewFieldDefaults(defaultField, values);

                //Handle date format ny database date format
                if (column.DataType == typeof(DateTime))
                {
                    DisplayFormat displayFormat = DisplayFormat.Date_Custom;
                    string dateFormat = configFieldView.Database.DateFormat;

                    bool notDateOnly = column.ExtendedProperties.ContainsKey("dataType") && !column.ExtendedProperties["dataType"].ToString().ToLower().Equals("date");


                    if (dateFormat == "MM/dd/yyyy")
                    {
                        if (notDateOnly)
                        {
                            displayFormat = DisplayFormat.Date_mm_dd_24;
                        }
                        else
                        {
                            displayFormat = DisplayFormat.Date_mm_dd;
                        }
                    }
                    else if (dateFormat == "dd/MM/yyyy")
                    {
                        if (notDateOnly)
                        {
                            displayFormat = DisplayFormat.Date_dd_mm_24;
                        }
                        else
                        {
                            displayFormat = DisplayFormat.Date_dd_mm;
                        }
                    }

                    if (notDateOnly && !dateFormat.Contains(":"))
                    {
                        dateFormat += " hh:mm:ss";
                    }

                    values["DisplayFormat"] = displayFormat;
                    values["Format"] = dateFormat;
                }
                //if (column.DataType == typeof(string))
                //{
                //    Int64 length = column.MaxLength;


                //    if (length < 8001)
                //    {
                //        if (length > 4000)
                //        {
                //            values.Add("Max", "4000");
                //        }
                //        else
                //        {
                //            values.Add("Max", length.ToString());
                //        }
                //    }


                //}
                //else 
                if (column.DataType.Equals(typeof(Int32)) || column.DataType.Equals(typeof(Int64)) || column.DataType.Equals(typeof(Decimal)) || column.DataType.Equals(typeof(Double)) || column.DataType.Equals(typeof(Single)))
                {
                    if (!values.ContainsKey("Max"))
                        values.Add("Max", Int32.MaxValue.ToString());
                    else if (values["Max"].Equals(0) || values["Max"].Equals("0") || values["Max"].Equals(string.Empty))
                        values["Max"] = Int32.MaxValue.ToString();

                    if (!values.ContainsKey("Min"))
                        values.Add("Min", Int32.MinValue.ToString());
                    else if (values["Min"].Equals(0) || values["Min"].Equals("0") || values["Min"].Equals(string.Empty))
                        values["Min"] = Int32.MinValue.ToString();

                }
                else if (column.DataType.Equals(typeof(Int16)))
                {
                    if (!values.ContainsKey("Max"))
                        values.Add("Max", Int16.MaxValue.ToString());
                    else if (values["Max"].Equals(0) || values["Max"].Equals("0") || values["Max"].Equals(string.Empty))
                        values["Max"] = Int16.MaxValue.ToString();

                    if (!values.ContainsKey("Min"))
                        values.Add("Min", Int16.MinValue.ToString());
                    else if (values["Min"].Equals(0) || values["Min"].Equals("0") || values["Min"].Equals(string.Empty))
                        values["Min"] = Int16.MinValue.ToString();
                }


                configAccess.GetNewRow(configFieldView, values, null, null, null, null, null);


            }
        }


        private void LoadNewFieldDefaults(Field defaultField, Dictionary<string, object> values)
        {

            Type type = defaultField.GetType();

            PropertyInfo[] properties = type.GetProperties(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);


            foreach (PropertyInfo propertyInfo in properties)
            {
                if (!values.ContainsKey(propertyInfo.Name))
                {
                    object[] propertyAttributes = propertyInfo.GetCustomAttributes(typeof(Durados.Config.Attributes.PropertyAttribute), true);
                    Durados.Config.Attributes.PropertyAttribute propertyAttribute = null;
                    if (propertyAttributes.Length == 1)
                    {
                        propertyAttribute = (Durados.Config.Attributes.PropertyAttribute)propertyAttributes[0];

                    }

                    if (propertyAttribute != null)
                    {
                        //PropertyAttribute propertyAttribute = (PropertyAttribute)propertyAttributes[0];

                        switch (propertyAttribute.PropertyType)
                        {
                            case Durados.Config.Attributes.PropertyType.Column:
                                try
                                {
                                    values.Add(propertyInfo.Name, propertyInfo.GetValue(defaultField, null));
                                }
                                catch { }
                                break;

                            default:
                                break;
                        }
                    }
                }


            }
        }

        private MapDataSet.ViewRow Persist(Database database, DataTable table, string editableTableName, ParentField defaultParentField, DataSet dataset, IDbCommand command)
        {
            MapDataSet.ViewRow viewRow = null;

            try
            {
                viewRow = MapDataSet.View.AddViewRow(table.TableName);
            }
            catch (Exception ex)
            {
                throw new DuradosException("The view " + table.TableName + " already exits!", ex);
            }

            SqlSchema sqlSchema = GetNewSqlSchema(command);

            AdFkFromMe(viewRow, database, table, editableTableName, sqlSchema, command);

            AdFkToMe(viewRow, database, table, editableTableName, defaultParentField, dataset, sqlSchema, command);


            return viewRow;
        }

        public string Exceptions { get; set; }

        private void AdFkToMe(MapDataSet.ViewRow viewRow, Database database, DataTable table, string editableTableName, ParentField defaultParentField, DataSet dataset, SqlSchema sqlSchema, IDbCommand command)
        {
            Dictionary<string, List<string>> fkToMe = sqlSchema.GetForeignKeyConstraintsToMe(string.IsNullOrEmpty(editableTableName) ? table.TableName : editableTableName, command);

            foreach (string tableName in fkToMe.Keys)
            {
                View view = database.GetViewByEditableTableName(tableName);
                if (view != null && viewRows.ContainsKey(view.Name))
                {
                    foreach (string columnName in fkToMe[tableName])
                    {
                        if (view.Fields.ContainsKey(columnName))
                        {
                            bool pk = false;
                            bool auto = false;
                            if (view.DataTable.Columns.Contains(columnName))
                            {
                                pk = view.DataTable.Columns[columnName].Unique;
                                auto = view.DataTable.Columns[columnName].AutoIncrement;
                            }
                            try
                            {
                                ChangeColumnFieldToParent(view.Name, table.TableName, columnName, defaultParentField, new Dictionary<string, object>(), dataset, pk, auto);
                            }
                            catch (DuradosException duradosException)
                            {
                                Exceptions += "Failed to create a Foreign Key relation from " + columnName + " to " + table.TableName + ". Additional information: " + duradosException.Message + "\n\r";
                            }
                            catch (Exception exception)
                            {
                                throw new DuradosException("Failed to create a Foreign Key relation from " + columnName + " to " + table.TableName, exception);
                            }
                        }
                    }
                }
            }
        }

        private void AdFkFromMe(MapDataSet.ViewRow viewRow, Database database, DataTable table, string editableTableName, SqlSchema sqlSchema, IDbCommand command)
        {

            Dictionary<string, string> fks = sqlSchema.GetMyForeignKeyConstraints(string.IsNullOrEmpty(editableTableName) ? table.TableName : editableTableName, command);

            foreach (DataColumn column in table.Columns)
            {
                string columnName = column.ColumnName;
                bool pk = table.PrimaryKey.Contains(column);
                bool autoIncrement = pk && table.PrimaryKey.Length == 1 && table.PrimaryKey[0].AutoIncrement;

                DataRow[] fieldRows = MapDataSet.Field.Select(string.Format("ViewId = {0} and Name='{1}'", viewRow.Id, columnName));
                MapDataSet.FieldRow fieldRow = null;
                if (fieldRows.Length == 1)
                    fieldRow = (MapDataSet.FieldRow)fieldRows[0];

                if (fks.ContainsKey(columnName))
                {
                    string parentTableName = fks[columnName];
                    View parentView = database.GetViewByEditableTableName(parentTableName);
                    if (parentView != null && viewRows.ContainsKey(parentView.Name) && parentView.DataTable.PrimaryKey.Length == 1 && parentView.DataTable.PrimaryKey[0].DataType.Equals(column.DataType) && !(pk && parentView.DataTable.Equals(table)))
                    {
                        MapDataSet.RelationRow relationRow = null;
                        if (fieldRow == null || fieldRow.RelationRow == null)
                            relationRow = MapDataSet.Relation.AddRelationRow(parentView.Name);

                        if (fieldRow == null)
                        {
                            fieldRow = MapDataSet.Field.AddFieldRow(viewRow, columnName, relationRow, column.DataType.FullName, pk, false, column.MaxLength, column.AllowDBNull, column.DefaultValue == null ? string.Empty : column.DefaultValue.ToString(), column.Unique);
                        }
                        else if (fieldRow.RelationRow == null && relationRow != null)
                        {
                            fieldRow.RelationRow = relationRow;
                        }
                    }
                    else
                    {
                        if (fieldRow == null)
                        {
                            MapDataSet.Field.AddFieldRow(viewRow, column.ColumnName, null, column.DataType.FullName, pk, autoIncrement, column.MaxLength, column.AllowDBNull, column.DefaultValue == null ? string.Empty : column.DefaultValue.ToString(), column.Unique);
                        }
                    }
                }
                else
                {
                    if (fieldRow == null)
                    {
                        MapDataSet.Field.AddFieldRow(viewRow, column.ColumnName, null, column.DataType.FullName, pk, autoIncrement, column.MaxLength, column.AllowDBNull, column.DefaultValue == null ? string.Empty : column.DefaultValue.ToString(), column.Unique);
                    }
                }
            }
        }

        private bool IsColumnPartOfPK(DataColumn column)
        {
            return column.Table.PrimaryKey.Length > 0 && column.Table.PrimaryKey[0].ColumnName.Equals(column.ColumnName);
        }

        private IEnumerable<DataColumn> Persist(MapDataSet.ViewRow viewRow, IEnumerable<DataColumn> newColumns, IEnumerable<DataColumn> deletedColumns, IEnumerable<DataColumn> changedColumns, Database database, DataTable table, string editableTableName, ParentField defaultParentField, DataSet dataset, IDbCommand command)
        {
            List<DataColumn> designTimeColumns = new List<DataColumn>();

            foreach (DataColumn column in newColumns)
            {
                try
                {
                    DataRow[] rows = MapDataSet.Field.Select("Name = '" + column.ColumnName + "' and ViewId = " + viewRow.Id);
                    if (rows.Length == 1)
                    {
                        rows[0].Delete();
                    }
                    MapDataSet.Field.AddFieldRow(viewRow, column.ColumnName, null, column.DataType.FullName, IsColumnPartOfPK(column), false, column.MaxLength, !column.AllowDBNull, column.DefaultValue == null ? string.Empty : column.DefaultValue.ToString(), column.Unique);
                }
                catch { }
            }

            foreach (DataColumn column in deletedColumns)
            {
                MapDataSet.FieldRow fieldRow = viewRow.GetFieldRows().Where(r => r.Name == column.ColumnName).First();
                if (fieldRow != null)
                {
                    fieldRow.Delete();
                }
                else
                {
                    designTimeColumns.Add(column);
                }
            }

            foreach (DataColumn column in changedColumns)
            {
                MapDataSet.FieldRow fieldRow = MapDataSet.Field.Where(r => r.ViewRow != null && r.ViewRow.Name == viewRow.Name && r.Name == column.ColumnName).FirstOrDefault();
                if (fieldRow != null)
                {
                    fieldRow.MaxLength = column.MaxLength;
                    fieldRow.DefaultValue = column.DefaultValue == null ? null : column.DefaultValue.ToString();
                    fieldRow.Required = !column.AllowDBNull && !column.AutoIncrement;
                    fieldRow.Unique = column.Unique;
                    fieldRow.AutoIncrement = column.AutoIncrement;
                    fieldRow.DbType = column.DataType.FullName;
                    fieldRow.PK = IsColumnPartOfPK(column);
                }
            }

            MapDataSet.AcceptChanges();

            /**/
            if (table != null)
            {
                SqlSchema sqlSchema = GetNewSqlSchema();

                if (string.IsNullOrEmpty(editableTableName))
                    editableTableName = table.TableName;

                bool closeCommand = false;
                if (command == null)
                {
                    IDbConnection connection = sqlSchema.GetConnection(ConnectionString);

                    command = sqlSchema.GetCommand();
                    command.Connection = connection;
                    command.Connection.Open();
                    closeCommand = true;

                }
                try
                {
                    if (database != null && table != null && editableTableName != null && defaultParentField != null && dataset != null && command != null)
                    {
                        AdFkFromMe(viewRow, database, table, editableTableName, sqlSchema, command);

                        //AdFkToMe(viewRow, database, table, editableTableName, defaultParentField, dataset, sqlSchema, command);
                    }
                }
                finally
                {
                    if (closeCommand)
                        command.Connection.Close();
                }
            }
            /**/

            return designTimeColumns;
        }

        private MapDataSet.FieldRow Persist(string viewName, string columnName, string dbType, string parentViewName, DataSet ds)
        {
            return Persist(viewName, columnName, dbType, parentViewName, ds, false, false);
        }

        private MapDataSet.FieldRow Persist(string viewName, string columnName, string dbType, string parentViewName, DataSet ds, bool pk, bool autoIncrement)
        {
            MapDataSet.ViewRow viewRow = null;
            MapDataSet.FieldRow fieldRow = null;

            if (viewRows.ContainsKey(viewName))
            {
                viewRow = viewRows[viewName];
            }
            else if (!ds.Tables.Contains(viewName))
            {
                throw new DuradosException("The view " + viewName + " does not exit!");
            }
            else
            {
                viewRow = MapDataSet.View.AddViewRow(viewName);
            }

            MapDataSet.RelationRow relationRow = null;
            if (!string.IsNullOrEmpty(parentViewName))
            {
                relationRow = MapDataSet.Relation.AddRelationRow(parentViewName);
            }

            fieldRow = MapDataSet.Field.AddFieldRow(viewRow, columnName, relationRow, dbType, pk, autoIncrement, -1, false, null, false);

            //MapDataSet.RelationColumns.AddRelationColumnsRow(relationRow, fieldName, 

            return fieldRow;
        }

        public void Save()
        {
            MapDataSet.AcceptChanges();

            //MapDataSet.WriteXml(FileName, XmlWriteMode.WriteSchema);
            Storage.WriteToStorage(MapDataSet, FileName);

        }

        public void AddSchema(DataSet ds)
        {
            foreach (MapDataSet.ViewRow viewRow in MapDataSet.View)
            {

                AddTable(ds, viewRow);
            }
        }

        protected DataTable AddTable(DataSet ds, MapDataSet.ViewRow viewRow)
        {
            DataTable table = null;
            List<DataColumn> pk = new List<DataColumn>();

            if (ds.Tables.Contains(viewRow.Name))
            {
                table = ds.Tables[viewRow.Name];
            }
            else
            {
                table = ds.Tables.Add(viewRow.Name);

            }

            bool hasPK = table.PrimaryKey != null && table.PrimaryKey.Length > 0;

            bool autoIncrement = false;

            foreach (MapDataSet.FieldRow fieldRow in viewRow.GetFieldRows())
            {
                DataColumn[] columns = null;
                try
                {
                    columns = AddField(ds, table, fieldRow, true);
                    if (fieldRow.PK)
                    {
                        pk.AddRange(columns);
                        autoIncrement = fieldRow.AutoIncrement;

                        if (!hasPK)
                        {
                            table.PrimaryKey = pk.ToArray();
                            if (pk.Count == 1)
                            {
                                if (table.PrimaryKey[0].DefaultValue != null)
                                    table.PrimaryKey[0].DefaultValue = null;
                                table.PrimaryKey[0].AutoIncrement = autoIncrement;
                            }
                        }
                    }
                }
                catch (Exception exception)
                {
                    Log("Mapper", "AddField", "AddField", exception, 1, "field: " + fieldRow.Name + ", view: " + table.TableName);

                }
            }



            return table;

        }

        private string GetRelationName(DataColumn pkColumn, DataColumn fkColumn, DataSet ds)
        {
            return GetRelationName(new DataColumn[1] { pkColumn }, new DataColumn[1] { fkColumn }, ds);

        }

        private string GetRelationName(DataColumn[] pkColumns, DataColumn[] fkColumns, DataSet ds)
        {
            string relationName = "FK_" + pkColumns[0].Table.TableName + "_" + fkColumns[0].Table.TableName;

            if (ds.Relations.Contains(relationName))
            {
                relationName += fkColumns[0].ColumnName;
            }

            return relationName;
        }

        protected DataColumn[] AddField(DataSet ds, DataTable table, MapDataSet.FieldRow fieldRow, bool isBuildingSchema)
        {
            List<DataColumn> columns = new List<DataColumn>();

            string fieldName = fieldRow.Name;
            Type dbType = System.Type.GetType(fieldRow.DbType);
            bool isParent = fieldRow.RelationRow != null;
            int maxLength = fieldRow.IsMaxLengthNull() ? -1 : fieldRow.MaxLength;
            bool required = fieldRow.IsRequiredNull() ? false : fieldRow.Required && !fieldRow.AutoIncrement;
            bool unique = fieldRow.IsUniqueNull() ? false : fieldRow.Unique;
            string defaultValue = fieldRow.IsDefaultValueNull() ? null : fieldRow.DefaultValue;

            if (isParent)
            {
                string parentViewName = fieldRow.RelationRow.RelatedViewName;
                if (!viewRows.ContainsKey(parentViewName))
                {
                    return columns.ToArray();
                }
                DataTable parentTable = null;
                if (ds.Tables.Contains(parentViewName))
                {
                    parentTable = ds.Tables[parentViewName];
                }
                else
                {
                    if (isBuildingSchema)
                    {
                        if (!viewRows.ContainsKey(parentViewName))
                        {
                            throw new DuradosException("The parent " + parentViewName + " of the column " + fieldName + " in " + table.TableName + " does not exists.");
                        }
                        else
                        {
                            MapDataSet.ViewRow parentViewRow = viewRows[parentViewName];
                            parentTable = AddTable(ds, parentViewRow);
                        }
                    }
                    else
                    {
                        throw new DuradosException("The parent view " + parentViewName + " of the column " + fieldName + " in " + table.TableName + " does not exists.");
                    }
                }
                MapDataSet.RelationColumnsRow[] relationColumnsRows = fieldRow.RelationRow.GetRelationColumnsRows();
                if (relationColumnsRows.Length == 0)
                {
                    if (!table.Columns.Contains(fieldName))
                    {
                        DataColumn column = table.Columns.Add(fieldName, dbType);
                        try
                        {
                            if (!string.IsNullOrEmpty(defaultValue))
                            {
                                column.DefaultValue = Convert.ChangeType(defaultValue, column.DataType);
                            }
                        }
                        catch { }
                        column.AllowDBNull = !required;
                        column.Unique = unique;
                        DataColumn pkColumn = null;
                        if (parentTable.PrimaryKey.Length > 1)
                        {
                            throw new DuradosException("The view " + parentViewName + " contains more than one primary key column.");
                        }
                        else if (parentTable.PrimaryKey.Length == 0)
                        {
                            List<MapDataSet.FieldRow> pkFieldRows = viewRows[parentViewName].GetFieldRows().Where(f => f.PK).ToList();
                            if (pkFieldRows.Count > 1)
                                throw new DuradosException("The view " + parentViewName + " contains more than one primary key column.");
                            else if (pkFieldRows.Count == 0)
                            {
                                DuradosException duradosException = new DuradosException("The view " + parentViewName + " does not have a primary key.");
                                Log("Mapper", "AddField", "AddField", duradosException, 5, "Parent field: " + fieldName + ", view: " + table.TableName + ", Parent view: " + parentViewName);
                                //DataColumn column2 = table.Columns.Add(fieldName, dbType);
                                //columns.Add(column2);

                            }
                            else
                            {
                                MapDataSet.FieldRow pkFieldRow = pkFieldRows[0];
                                if (parentTable.Columns.Contains(pkFieldRow.Name))
                                {
                                    bool autoIncrement = pkFieldRow.AutoIncrement;
                                    pkColumn = parentTable.Columns[pkFieldRow.Name];
                                    parentTable.PrimaryKey = new DataColumn[1] { pkColumn };
                                    pkColumn.AutoIncrement = autoIncrement;
                                }
                                else
                                    throw new DuradosException("The parent " + parentViewName + " does not have a primary key.");

                            }

                        }

                        if (parentTable.PrimaryKey.Length == 1)
                        {
                            pkColumn = parentTable.PrimaryKey[0];
                            if (!pkColumn.Equals(column))
                            {
                                string relationName = GetRelationName(pkColumn, column, ds);
                                DataRelation relation = null;
                                try
                                {
                                    relation = ds.Relations.Add(relationName, pkColumn, column, false);
                                }
                                catch (Exception exception)
                                {
                                    throw new DuradosException("Could not relate [" + parentViewName + "] with [" + table.TableName + "] on [" + column.ColumnName + "].\n" + exception.Message);
                                }
                                try
                                {
                                    relation.Nested = relation.ParentTable.Equals(relation.ChildTable);
                                }
                                catch { }
                            }
                        }
                    }
                }
                else
                {
                    try
                    {
                        if (!table.Columns.Contains(relationColumnsRows[0].FkColumnName))
                        {
                            List<DataColumn> fk = new List<DataColumn>();
                            List<DataColumn> pk = new List<DataColumn>();
                            foreach (MapDataSet.RelationColumnsRow relationColumnsRow in relationColumnsRows)
                            {
                                DataColumn pkColumn = parentTable.Columns[relationColumnsRow.PkColumnName];
                                DataColumn fkColumn = table.Columns.Add(relationColumnsRow.FkColumnName, pkColumn.DataType);
                                pk.Add(pkColumn);
                                fk.Add(fkColumn);

                            }

                            DataColumn[] pkColumns = pk.ToArray();
                            DataColumn[] fkColumns = fk.ToArray();

                            string relationName = GetRelationName(pkColumns, fkColumns, ds);
                            DataRelation relation = ds.Relations.Add(relationName, pkColumns, fkColumns, false);

                            relation.Nested = relation.ParentTable.Equals(relation.ChildTable);
                        }
                    }
                    catch { }
                }
            }
            else
            {
                if (!table.Columns.Contains(fieldName))
                {
                    DataColumn column = table.Columns.Add(fieldName, dbType);

                    if (column.DataType.Equals(typeof(string)))
                    {
                        column.MaxLength = maxLength;
                    }
                    try
                    {
                        if (!string.IsNullOrEmpty(defaultValue))
                        {
                            column.DefaultValue = Convert.ChangeType(defaultValue, column.DataType);
                        }
                    }
                    catch
                    {

                    }
                    column.AllowDBNull = !required;
                    column.Unique = unique;
                    columns.Add(column);
                }
            }


            if (isParent && columns.Count == 0 && fieldRow.PK)
            {
                DataColumn column = null;
                if (!table.Columns.Contains(fieldName))
                {
                    table.Columns.Add(fieldName, dbType);
                }
                column = table.Columns[fieldName];
                columns.Add(column);
            }

            return columns.ToArray();
        }

        public bool IsViewAlreadyExists(string name, DataSet dataset)
        {
            return dataset.Tables.Contains(name) || viewRows.ContainsKey(name);
        }

        public bool IsViewExistInServer(string name, IDbCommand command)
        {
            return (GetNewSqlSchema(command)).IsTableOrViewExists(name, command);
        }

        public void ChangeFieldRelation(View view, RelationChange relationChange, string oldRelatedViewName, string newRelatedViewName, string configFieldPk, View configFieldView, Database defaultDatabase, Dictionary<string, object> values, DataSet dataset)
        {
            ParentField defaultParentField = (ParentField)defaultDatabase.Views["Table"].Fields.Values.Where(f => f.FieldType == FieldType.Parent).First();

            switch (relationChange)
            {
                case RelationChange.ColumnToParent:
                    ChangeColumnFieldToParent(view, newRelatedViewName, configFieldPk, configFieldView, defaultParentField, values, dataset);
                    break;

                case RelationChange.ParentToColumn:
                    ChangeParentToColumn(view, oldRelatedViewName, configFieldPk, configFieldView, values, dataset);
                    break;

                case RelationChange.ParentToDifferentParent:
                    ChangeParentToDifferentParent(view, oldRelatedViewName, newRelatedViewName, configFieldPk, configFieldView, defaultParentField, values, dataset);
                    break;

                default:
                    break;
            }
        }

        public void ChangeParentToColumn(View view, string oldRelatedViewName, string configFieldPk, View configFieldView, Dictionary<string, object> values, DataSet dataset)
        {
            View oldRelatedView = view.Database.Views[oldRelatedViewName];

            string tableName = string.IsNullOrEmpty(view.EditableTableName) ? view.Name : view.EditableTableName;
            string relatedTableName = string.IsNullOrEmpty(oldRelatedView.EditableTableName) ? oldRelatedView.Name : oldRelatedView.EditableTableName;

            SqlSchema sqlSchema = GetNewSqlSchema();

            IDbConnection connection = sqlSchema.GetConnection(ConnectionString);

            IDbCommand command = sqlSchema.GetCommand();
            command.Connection = connection;
            command.Connection.Open();
            command.Transaction = connection.BeginTransaction();

            try
            {

                if (!viewRows.ContainsKey(view.Name))
                    throw new DuradosException("Cannot change a design time field.");

                MapDataSet.ViewRow viewRow = viewRows[view.Name];

                ConfigAccess configAccess = new ConfigAccess();

                DataRow configFieldRow = configAccess.GetDataRow(configFieldView, configFieldPk);

                if (configFieldRow == null)
                    throw new DuradosException("Configuration field with id = " + configFieldPk + " is missing");


                string fieldName = configFieldRow["Name"].ToString();

                string columnName = view.Fields[fieldName].DatabaseNames;

                try
                {
                    sqlSchema.DropFkConstraint(tableName, relatedTableName, columnName, command);
                }
                catch { }

                values["Name"] = columnName;

                MapDataSet.FieldRow fieldRow = null;
                foreach (MapDataSet.FieldRow fieldRow2 in viewRow.GetFieldRows())
                {
                    if (fieldRow2.Name == columnName)
                    {
                        fieldRow = fieldRow2;
                        break;
                    }
                }

                MapDataSet.RelationRow oldRelationRow = fieldRow.RelationRow;

                string dbType = fieldRow.DbType;

                MapDataSet.Field.RemoveFieldRow(fieldRow);

                if (oldRelationRow != null)
                {
                    foreach (MapDataSet.RelationColumnsRow relationColumnsRow in oldRelationRow.GetRelationColumnsRows())
                    {
                        MapDataSet.RelationColumns.RemoveRelationColumnsRow(relationColumnsRow);
                    }

                    mapDataSet.Relation.RemoveRelationRow(oldRelationRow);
                }


                Persist(view.Name, columnName, dbType, null, dataset);

                command.Transaction.Commit();
            }
            catch (Exception exception)
            {
                if (command != null && command.Transaction != null)
                    command.Transaction.Rollback();
                throw exception;
            }
            finally
            {
                connection.Close();
            }

        }

        public void ChangeParentToDifferentParent(View view, string oldRelatedViewName, string newRelatedView, string configFieldPk, View configFieldView, ParentField defaultParentField, Dictionary<string, object> values, DataSet dataset)
        {
            if (!dataset.Tables.Contains(newRelatedView))
                throw new DuradosException("Adding new Parent to the server is not implemented yet!");

            if (!viewRows.ContainsKey(view.Name))
                throw new DuradosException("Cannot change a design time field.");

            MapDataSet.ViewRow viewRow = viewRows[view.Name];

            ConfigAccess configAccess = new ConfigAccess();

            DataRow configFieldRow = configAccess.GetDataRow(configFieldView, configFieldPk);

            if (configFieldRow == null)
                throw new DuradosException("Configuration field with id = " + configFieldPk + " is missing");


            string fieldName = configFieldRow["Name"].ToString();

            DataRelation relation = ChangeDatasetParentToDifferentParent(view, fieldName, oldRelatedViewName, newRelatedView, dataset);

            string parentFieldName = relation.RelationName + "_Parent";

            string columnName = view.Fields[fieldName].DatabaseNames;


            SetColumnConfiguration(values, defaultParentField, parentFieldName);

            MapDataSet.FieldRow fieldRow = null;
            foreach (MapDataSet.FieldRow fieldRow2 in viewRow.GetFieldRows())
            {
                if (fieldRow2.Name == columnName)
                {
                    fieldRow = fieldRow2;
                    break;
                }
            }

            MapDataSet.RelationRow oldRelationRow = fieldRow.RelationRow;


            MapDataSet.Field.RemoveFieldRow(fieldRow);

            if (oldRelationRow != null)
            {
                foreach (MapDataSet.RelationColumnsRow relationColumnsRow in oldRelationRow.GetRelationColumnsRows())
                {
                    MapDataSet.RelationColumns.RemoveRelationColumnsRow(relationColumnsRow);
                }

                mapDataSet.Relation.RemoveRelationRow(oldRelationRow);
            }


            Persist(view.Name, columnName, relation.ChildColumns[0].DataType.FullName, newRelatedView, dataset);
            //Persist(viewRow, fieldName, relation);

        }

        public void ChangeColumnFieldToParent(View view, string newRelatedView, string configFieldPk, View configFieldView, ParentField defaultParentField, Dictionary<string, object> values, DataSet dataset)
        {
            ChangeColumnFieldToParent(view.Name, newRelatedView, configFieldPk, configFieldView, defaultParentField, values, dataset);
        }

        public void ChangeColumnFieldToParent(string viewName, string newRelatedView, string configFieldPk, View configFieldView, ParentField defaultParentField, Dictionary<string, object> values, DataSet dataset)
        {
            if (!dataset.Tables.Contains(newRelatedView))
                throw new DuradosException("Adding new Parent to the server is not implemented yet!");

            if (!viewRows.ContainsKey(viewName))
                throw new DuradosException("Cannot change a design time field.");


            ConfigAccess configAccess = new ConfigAccess();

            DataRow configFieldRow = configAccess.GetDataRow(configFieldView, configFieldPk);

            if (configFieldRow == null)
                throw new DuradosException("Configuration field with id = " + configFieldPk + " is missing");


            string fieldName = configFieldRow["Name"].ToString();

            ChangeColumnFieldToParent(viewName, newRelatedView, fieldName, defaultParentField, values, dataset);
            //Persist(viewRow, fieldName, relation);

        }

        public void ChangeColumnFieldToParent(string viewName, string newRelatedView, string fieldName, ParentField defaultParentField, Dictionary<string, object> values, DataSet dataset)
        {
            ChangeColumnFieldToParent(viewName, newRelatedView, fieldName, defaultParentField, values, dataset, false, false);
        }

        public void ChangeColumnFieldToParent(string viewName, string newRelatedView, string fieldName, ParentField defaultParentField, Dictionary<string, object> values, DataSet dataset, bool pk, bool autoIncrement)
        {
            MapDataSet.ViewRow viewRow = viewRows[viewName];

            DataRelation relation = ChangeDatasetColumnFieldToParent(viewName, fieldName, newRelatedView, dataset);

            string parentFieldName = relation.RelationName + "_Parent";


            SetColumnConfiguration(values, defaultParentField, parentFieldName);

            MapDataSet.FieldRow fieldRow = null;
            foreach (MapDataSet.FieldRow fieldRow2 in viewRow.GetFieldRows())
            {
                if (fieldRow2.Name == fieldName)
                {
                    fieldRow = fieldRow2;
                    break;
                }
            }

            //if (MapDataSet.Field.Contains(fieldRow))
            MapDataSet.Field.RemoveFieldRow(fieldRow);

            Persist(viewName, fieldName, relation.ChildColumns[0].DataType.FullName, newRelatedView, dataset, pk, autoIncrement);

        }

        private void SetColumnConfiguration(Dictionary<string, object> values, ParentField defaultParentField, string parentFieldName)
        {
            values["ParentHtmlControlType"] = "DropDown";
            values["AutocompleteMathing"] = "StartsWith";
            values["LimitToStartAutocomplete"] = true;
            values["ShowDependencyInTable"] = true;
            values["MultiFilter"] = true;
            values["GridEditable"] = true;
            values["Name"] = parentFieldName;
        }

        //private void Persist(MapDataSet.ViewRow viewRow, string fieldName, DataRelation relation)
        //{
        //    if (viewRow.
        //}

        private DataRelation ChangeDatasetParentToDifferentParent(View view, string fieldName, string oldRelatedViewName, string newRelatedViewName, DataSet dataset)
        {
            string viewName = view.Name;
            if (!dataset.Tables.Contains(viewName))
            {
                throw new DuradosException("The view " + viewName + " does not exist.");
            }

            if (!dataset.Tables.Contains(newRelatedViewName))
            {
                throw new DuradosException("The view " + newRelatedViewName + " does not exist. Sync a new view is not implemented yet. Please add it manually first.");
            }

            DataTable table = dataset.Tables[viewName];

            string columnName = view.Fields[fieldName].DatabaseNames;

            if (!table.Columns.Contains(columnName))
            {
                throw new DuradosException("The column " + columnName + " does not exist.");
            }



            DataColumn column = table.Columns[columnName];



            DataTable relatedTable = dataset.Tables[newRelatedViewName];

            if (relatedTable.PrimaryKey.Length == 0)
                throw new DuradosException("The parent view " + newRelatedViewName + " has no primary key.");

            if (relatedTable.PrimaryKey.Length > 1)
                throw new DuradosException("The parent view " + newRelatedViewName + " has primary key with more than one column.");

            if (relatedTable.PrimaryKey[0].DataType != column.DataType)
                throw new DuradosException("The parent view " + newRelatedViewName + " has primary key with different data type than the forign key column " + column.ColumnName + " of " + viewName + ".");

            DataRelation oldRelation = GetRelationByColumn(column, dataset);
            if (oldRelation != null)
            {
                dataset.Relations.Remove(oldRelation);
                if (table.Constraints.Contains(oldRelation.RelationName))
                    table.Constraints.Remove(oldRelation.RelationName);
            }

            DataColumn pkColumn = relatedTable.PrimaryKey[0];
            string relationName = GetRelationName(pkColumn, column, dataset);

            return dataset.Relations.Add(relationName, pkColumn, column, false);
        }

        private DataRelation ChangeDatasetColumnFieldToParent(string viewName, string columnName, string newRelatedViewName, DataSet dataset)
        {
            if (!dataset.Tables.Contains(viewName))
            {
                throw new DuradosException("The view " + viewName + " does not exist.");
            }

            if (!dataset.Tables.Contains(newRelatedViewName))
            {
                throw new DuradosException("The view " + newRelatedViewName + " does not exist. Sync a new view is not implemented yet. Please add it manually first.");
            }

            DataTable table = dataset.Tables[viewName];

            if (!table.Columns.Contains(columnName))
            {
                throw new DuradosException("The column " + columnName + " does not exist.");
            }


            DataColumn column = table.Columns[columnName];


            if (GetRelationByColumn(column, dataset) != null)
            {
                throw new DuradosException("The column " + columnName + " is already in a relation.");
            }

            DataTable relatedTable = dataset.Tables[newRelatedViewName];

            if (relatedTable.PrimaryKey.Length == 0)
                throw new DuradosException("The parent view " + newRelatedViewName + " has no primary key.");

            if (relatedTable.PrimaryKey.Length > 1)
                throw new DuradosException("The parent view " + newRelatedViewName + " has primary key with more than one column.");

            if (relatedTable.PrimaryKey[0].DataType != column.DataType)
                throw new DuradosException("The parent view " + newRelatedViewName + " has primary key with different data type than the forign key column " + column.ColumnName + " of " + viewName + ".");

            DataColumn pkColumn = relatedTable.PrimaryKey[0];
            string relationName = GetRelationName(pkColumn, column, dataset);

            return dataset.Relations.Add(relationName, pkColumn, column, false);
        }

        private DataRelation GetRelationByColumn(DataColumn column, DataSet dataset)
        {
            foreach (DataRelation relation in dataset.Relations)
            {
                if (relation.ChildColumns.Contains(column) && relation.ChildColumns.Length == 1)
                {
                    return relation;
                }
            }

            return null;
        }

        public bool IsDynamicView(string viewName)
        {
            return viewRows.ContainsKey(viewName);
        }

        protected virtual bool IsTextual(DataType dataType,DataType dataTypeOld,SqlSchema sqlSchema)
        {
            return sqlSchema.GetDataType(dataType).ToLower().Contains("varchar") && sqlSchema.GetDataType(dataTypeOld).ToLower().Contains("varchar");
        }

        public void ChangeSimpleDataType(View view, ColumnField columnField, string oldDataType, string newDataType, string configFieldPk, View configView, string configViewPk, View configFieldView, Database defaultDatabase, Field defaultField, ParentField defaultParentField, Dictionary<string, object> values, Dictionary<string, string> viewDefaults, DataSet dataset)
        {

            SqlSchema sqlSchema = GetNewSqlSchema();

            IDbConnection connection = sqlSchema.GetConnection(ConnectionString);

            IDbCommand command = sqlSchema.GetCommand();
            command.Connection = connection;
            command.Connection.Open();

            DataType dataType = (DataType)Enum.Parse(typeof(DataType), newDataType);
            DataType dataTypeOld = (DataType)Enum.Parse(typeof(DataType), oldDataType);

            string tableName = string.IsNullOrEmpty(view.EditableTableName) ? view.Name : view.EditableTableName;
            try
            {
                if (!columnField.IsCalculated && !IsTextual(dataType, dataTypeOld, sqlSchema))
                {
                    try
                    {
                        sqlSchema.ChangeColumnType(tableName, columnField.Name, dataType, command);
                    }
                    catch (Exception exception)
                    {
                        if ((dataTypeOld == DataType.LongText || dataTypeOld == DataType.ShortText) && (dataType == DataType.LongText || dataType == DataType.ShortText))
                        {
                        }
                        else
                        {
                            ColumnField dataTypeColumnField = (ColumnField)configFieldView.Fields["DataType"];
                            Dictionary<string, string> selectOptions = dataTypeColumnField.GetSelectOptions();

                            throw new DuradosException("You cannot change the column type since it contains incompatible data. Please add a new column instead or adjust the data according.", exception);
                        }
                    }
                }
                MapDataSet.ViewRow viewRow = viewRows[view.Name];

                MapDataSet.FieldRow fieldRow = MapDataSet.Field.Where(r => r.ViewRow != null && r.ViewRow.Name == viewRow.Name && r.Name == columnField.Name).FirstOrDefault();
                if (fieldRow != null)
                {
                    Type type = GetTypeofDataType(dataType);
                    fieldRow.DbType = type.FullName;
                    MapDataSet.AcceptChanges();
                    dataset.Tables[view.Name].Columns[columnField.Name].DataType = type;

                }
            }
            catch (Exception exception)
            {
                throw new DuradosException(exception.Message);
            }
            finally
            {
                connection.Close();
            }
        }

        public string ChangeDataType(View view, string oldDataType, string newDataType, string configFieldPk, View configView, string configViewPk, View configFieldView, Database defaultDatabase, Field defaultField, ParentField defaultParentField, Dictionary<string, object> values, Dictionary<string, string> viewDefaults, DataSet dataset, out string manyToManyViewName)
        {
            return ChangeDataType(view, oldDataType, newDataType, configFieldPk, configView, configViewPk, configFieldView, defaultDatabase, defaultField, defaultParentField, values, viewDefaults, dataset, null, out manyToManyViewName);
        }

        public string ChangeDataType(View view, string oldDataType, string newDataType, string configFieldPk, View configView, string configViewPk, View configFieldView, Database defaultDatabase, Field defaultField, ParentField defaultParentField, Dictionary<string, object> values, Dictionary<string, string> viewDefaults, DataSet dataset, IDbCommand command, out string manyToManyViewName)
        {
            string columnName = null;
            bool byItself = command == null;
            manyToManyViewName = null;
            try
            {
                if (byItself)
                {
                    SqlSchema sqlSchema = GetNewSqlSchema();

                    IDbConnection connection = sqlSchema.GetConnection(ConnectionString);

                    command = sqlSchema.GetCommand();
                    command.Connection = connection;
                    command.Connection.Open();
                    command.Transaction = connection.BeginTransaction();
                }

                if (oldDataType == newDataType)
                    return columnName;

                if (string.IsNullOrEmpty(newDataType))
                    throw new DuradosException("Please enter a Data Type.");

                DataType dataType;
                try
                {
                    dataType = (DataType)Enum.Parse(typeof(DataType), newDataType);
                }
                catch (Exception exception)
                {
                    throw new DuradosException("The Data Type is not supported.", exception);

                }

                switch (dataType)
                {
                    case DataType.ImageList:

                    case DataType.SingleSelect:
                        columnName = ChangeDataTypeToSingleSelect(view, defaultDatabase, configFieldPk, configView, configViewPk, configFieldView, dataset, defaultField, defaultParentField, viewDefaults, defaultDatabase, values, command);
                        break;

                    case DataType.MultiSelect:
                        columnName = ChangeDataTypeToMultiSelect(view, defaultDatabase, configFieldPk, configView, configViewPk, configFieldView, dataset, defaultField, defaultParentField, viewDefaults, defaultDatabase, values, command, out manyToManyViewName);
                        break;

                    default:
                        throw new DuradosException("Changing the Data Type to " + newDataType + " is not implemented yet!");
                }


                return columnName;
            }
            finally
            {
                if (byItself)
                {
                    if (command != null)
                    {
                        if (command.Transaction != null)
                        {
                            command.Transaction.Commit();
                        }
                        if (command.Connection != null)
                        {
                            command.Connection.Close();
                        }
                    }
                }

            }
        }

        private string ChangeDataTypeToMultiSelect(View view, Database database, string configFieldPk, View configView, string configViewPk, View configFieldView, DataSet dataset, Field defaultField, ParentField defaultParentField, Dictionary<string, string> viewDefaults, Database defaultDatabase, Dictionary<string, object> values, IDbCommand command, out string manyToManyViewName)
        {
            char delimiter = ',';
            return ChangeDataTypeToMultiSelect(view, database, configFieldPk, configView, configViewPk, configFieldView, dataset, defaultField, defaultParentField, viewDefaults, defaultDatabase, values, command, delimiter, out manyToManyViewName);
        }

        private string ChangeDataTypeToMultiSelect(View view, Database database, string configFieldPk, View configView, string configViewPk, View configFieldView, DataSet dataset, Field defaultField, ParentField defaultParentField, Dictionary<string, string> viewDefaults, Database defaultDatabase, Dictionary<string, object> values, IDbCommand command, char delimiter, out string manyToManyViewName)
        {
            string viewName = view.Name;
            string tableName = string.IsNullOrEmpty(view.EditableTableName) ? view.Name : view.EditableTableName;
            MapDataSet.ViewRow viewRow = viewRows[view.Name];

            ConfigAccess configAccess = new ConfigAccess();

            DataRow configFieldRow = configAccess.GetDataRow(configFieldView, configFieldPk);

            if (configFieldRow == null)
                throw new DuradosException("Configuration field with id = " + configFieldPk + " is missing");


            string fieldName = configFieldRow["Name"].ToString();

            bool isParent = false;

            if (view.Fields.ContainsKey(fieldName))
            {
                isParent = view.Fields[fieldName].FieldType == FieldType.Parent;
                if (isParent)
                    fieldName = view.Fields[fieldName].GetColumnsNames()[0];
            }

            string columnName = GetParentFieldName(fieldName);

            Type type = typeof(int);

            string singleSelectTableName = null;

            if (values.ContainsKey("RelatedViewName") && values["RelatedViewName"] != null && !values["RelatedViewName"].Equals(string.Empty))
            {
                singleSelectTableName = values["RelatedViewName"].ToString().Replace(" ", "_");
            }
            else
            {
                singleSelectTableName = tableName + fieldName;
            }

            bool isRelatedViewExists = view.Database.Views.ContainsKey(singleSelectTableName);

            DataTable singleSelectTable = null;
            string referenceTableName = null;
            if (isRelatedViewExists)
            {
                singleSelectTable = view.Database.Views[singleSelectTableName].DataTable;
                referenceTableName = string.IsNullOrEmpty(view.Database.Views[singleSelectTableName].EditableTableName) ? singleSelectTableName : view.Database.Views[singleSelectTableName].EditableTableName;
            }
            else
            {
                singleSelectTable = GetSingleSelectTable(singleSelectTableName);
                referenceTableName = singleSelectTable.TableName;
            }

            //DataTable singleSelectTable = GetSingleSelectTable(tableName + fieldName);

            DataTable manyToManyTable = GetManyToManyTable(view.DataTable, singleSelectTable);

            object order = configFieldRow["Order"];
            object orderForCreate = configFieldRow["OrderForCreate"];
            object orderForEdit = configFieldRow["OrderForEdit"];


            SqlSchema sqlSchema = GetNewSqlSchema();



            try
            {
                if (!isRelatedViewExists)
                {
                    sqlSchema.CreateTable(singleSelectTable, command);
                }
                sqlSchema.CreateTable(manyToManyTable, command);

                sqlSchema.CreateFkConstraint(manyToManyTable.TableName, manyToManyTable.Columns[1].ColumnName, tableName, view.DataTable.PrimaryKey[0].ColumnName, command);
                sqlSchema.CreateFkConstraint(manyToManyTable.TableName, manyToManyTable.Columns[2].ColumnName, referenceTableName, singleSelectTable.PrimaryKey[0].ColumnName, command);

                int maxId = 1;
                Dictionary<string, int> singleSelectDictionary = new Dictionary<string, int>();

                //string sql = "select distinct [{0}].[{1}], [{0}].[{2}] from [{0}] where [{0}].[{1}] is not null";
                string relatedViewDisplayName = null;
                if (isRelatedViewExists)
                {
                    relatedViewDisplayName = view.Database.Views[singleSelectTableName].DisplayField.DatabaseNames;
                }
                else
                {
                    relatedViewDisplayName = "Name";
                }
                command.CommandText = sqlSchema.GetMultiListRelatedViewValuesStatement(singleSelectTableName, relatedViewDisplayName, singleSelectTable.PrimaryKey[0].ColumnName);//string.Format(sql, singleSelectTableName, relatedViewDisplayName, singleSelectTable.PrimaryKey[0].ColumnName);
                using (IDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        int pk = reader.GetInt32(1);
                        //string val = reader.GetString(0);
                        string val = reader.GetValue(0).ToString();
                        singleSelectDictionary.Add(val, pk);
                        if (pk >= maxId)
                        {
                            maxId++;
                        }
                    }
                }

                Dictionary<string, int> singleSelectNotExistDictionary = new Dictionary<string, int>();
                Dictionary<string, List<int>> multiSelectDictionary = new Dictionary<string, List<int>>();
                //sql = "select distinct [{0}].[{1}], [{0}].[{2}] from [{0}] where [{0}].[{1}] is not null";

                command.CommandText = sqlSchema.GetMultiListViewValuesStatement(tableName, fieldName, view.DataTable.PrimaryKey[0].ColumnName); // string.Format(sql, tableName, fieldName, view.DataTable.PrimaryKey[0].ColumnName);
                using (IDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        string pk = reader.GetValue(1).ToString();
                        multiSelectDictionary.Add(pk, new List<int>());

                        //string val = reader.GetString(0);
                        string val = reader.GetValue(0).ToString();
                        if (isParent)
                        {
                            int fk = -1;
                            if (Int32.TryParse(val, out fk))
                            {
                                if (singleSelectDictionary.ContainsValue(fk))
                                {
                                    multiSelectDictionary[pk].Add(fk);
                                }
                            }
                        }
                        else
                        {
                            string[] vals = val.Split(delimiter);
                            foreach (string s in vals)
                            {
                                string value = s.Trim();
                                if (!singleSelectDictionary.ContainsKey(value))
                                {
                                    singleSelectDictionary.Add(value, maxId);
                                    singleSelectNotExistDictionary.Add(value, maxId);
                                    maxId++;
                                }

                                int fk = singleSelectDictionary[value];
                                multiSelectDictionary[pk].Add(fk);
                            }
                        }
                    }
                }

                //command.CommandText = "SET IDENTITY_INSERT [" + referenceTableName + "] ON";
                //command.ExecuteNonQuery();

                sqlSchema.StopIdentityInsert(referenceTableName, command);

                foreach (string value in singleSelectNotExistDictionary.Keys)
                {
                    command.CommandText = sqlSchema.GetMultiListInsertIntoViewStatement(singleSelectTable.TableName, singleSelectTable.Columns[0].ColumnName, singleSelectTable.Columns[1].ColumnName, singleSelectDictionary[value], value, singleSelectTable.Columns[2].ColumnName);//string.Format("insert into [{0}]({1}, {2}, {5}) values ({3}, '{4}', {3})", singleSelectTable.TableName, singleSelectTable.Columns[0].ColumnName, singleSelectTable.Columns[1].ColumnName, singleSelectDictionary[value], value, singleSelectTable.Columns[2].ColumnName);
                    command.ExecuteNonQuery();
                }

                //command.CommandText = "SET IDENTITY_INSERT [" + referenceTableName + "] OFF";
                //command.ExecuteNonQuery();

                sqlSchema.ContinueIdentityInsert(referenceTableName, command);

                foreach (string pk in multiSelectDictionary.Keys)
                {
                    foreach (int fk in multiSelectDictionary[pk])
                    {
                        command.CommandText = sqlSchema.GetMultiListInsertIntoManyToManyStatement(manyToManyTable.TableName, manyToManyTable.Columns[1].ColumnName, manyToManyTable.Columns[2].ColumnName, pk, fk); //string.Format("insert into [{0}]({1}, {2}) values ({3},{4})", manyToManyTable.TableName, manyToManyTable.Columns[1].ColumnName, manyToManyTable.Columns[2].ColumnName, pk, fk);
                        command.ExecuteNonQuery();
                    }
                }

                bool drop = false;

                if (drop)
                {
                    sqlSchema.RemoveColumnFromTable(string.IsNullOrEmpty(view.EditableTableName) ? view.DataTable.TableName : view.EditableTableName, fieldName, command);
                    if (!string.IsNullOrEmpty(view.EditableTableName) && view.EditableTableName != view.Name)
                        sqlSchema.RemoveColumnFromView(viewName, fieldName, command);
                }
                else
                {
                    string displayName = configFieldRow["DisplayName"].ToString();
                    configFieldRow["DisplayName"] = displayName + " old";
                    configFieldRow["Excluded"] = true;
                }


                Sync(view, configViewPk, configFieldView, defaultField, dataset, null, null, command);
                string viewDisplayName = singleSelectTable.TableName.GetDecamal();
                if (viewDefaults.ContainsKey("DisplayName"))
                {
                    viewDefaults["DisplayName"] = viewDisplayName;
                }
                else
                {
                    viewDefaults.Add("DisplayName", viewDisplayName);
                }

                string singleSelectViewName = null;
                if (isRelatedViewExists)
                {
                    singleSelectViewName = singleSelectTableName;
                }
                else
                {
                    singleSelectViewName = CreateView(singleSelectTable, database, configView, configFieldView, dataset, defaultField, defaultParentField, viewDefaults, false, command);
                }

                viewDisplayName = viewDisplayName + "s";
                if (viewDefaults.ContainsKey("DisplayName"))
                {
                    viewDefaults["DisplayName"] = viewDisplayName;
                }
                else
                {
                    viewDefaults.Add("DisplayName", viewDisplayName);
                }
                manyToManyViewName = CreateView(manyToManyTable, database, configView, configFieldView, dataset, defaultField, defaultParentField, viewDefaults, false, command);
                //ParentField defaultParentField = (ParentField)defaultDatabase.Views["Table"].Fields.Values.Where(f => f.FieldType == FieldType.Parent).First();
                string manyToManyConfigViewPK = GetNewConfigViewPk(configAccess, configView, manyToManyViewName);
                ChangeColumnFieldToParent(manyToManyViewName, view.Name, GetNewConfigFieldPk(configAccess, configView, manyToManyConfigViewPK, manyToManyTable.Columns[1].ColumnName), configFieldView, defaultParentField, values, dataset);
                ChangeColumnFieldToParent(manyToManyViewName, singleSelectViewName, GetNewConfigFieldPk(configAccess, configView, manyToManyConfigViewPK, manyToManyTable.Columns[2].ColumnName), configFieldView, defaultParentField, values, dataset);

                values["Order"] = order;
                values["OrderForCreate"] = orderForCreate;
                values["OrderForEdit"] = orderForEdit;


                return columnName;
                //command.Transaction.Commit();
            }
            catch (Exception exception)
            {
                if (command != null && command.Transaction != null)
                    command.Transaction.Rollback();
                throw exception;
            }
        }

        private string GetNewConfigViewPk(ConfigAccess configAccess, View configView, string manyToManyViewName)
        {
            return configAccess.GetViewPK(manyToManyViewName, configView.Database.ConnectionString);

        }

        private DataTable GetManyToManyTable(DataTable dataTable, DataTable singleSelectTable)
        {
            string tableName = dataTable.TableName + "_" + singleSelectTable.TableName;
            DataTable table = new DataTable(tableName);

            DataColumn idColumn = table.Columns.Add("Id", typeof(int));
            idColumn.AutoIncrement = true;

            table.Columns.Add(dataTable.TableName + "Id", dataTable.PrimaryKey[0].DataType);
            table.Columns.Add(singleSelectTable.TableName + "Id", singleSelectTable.PrimaryKey[0].DataType);

            table.PrimaryKey = new DataColumn[1] { idColumn };

            return table;
        }

        private string ChangeDataTypeToSingleSelect(View view, Database database, string configFieldPk, View configView, string configViewPk, View configFieldView, DataSet dataset, Field defaultField, ParentField defaultParentField, Dictionary<string, string> viewDefaults, Database defaultDatabase, Dictionary<string, object> values, IDbCommand command)
        {
            if (IsChangeToReferenced(view, configFieldPk, configFieldView, values))
            {
                return ChangeDataTypeToSingleSelectForRefereneced(view, database, configFieldPk, configView, configViewPk, configFieldView, dataset, defaultField, defaultParentField, viewDefaults, defaultDatabase, values, command);
            }
            else
            {
                return ChangeDataTypeToSingleSelectForNewTableNotRefereneced(view, database, configFieldPk, configView, configViewPk, configFieldView, dataset, defaultField, defaultParentField, viewDefaults, defaultDatabase, values, command);
            }
        }

        private bool IsChangeToReferenced(View view, string configFieldPk, View configFieldView, Dictionary<string, object> values)
        {
            ConfigAccess configAccess = new ConfigAccess();

            DataRow configFieldRow = configAccess.GetDataRow(configFieldView, configFieldPk);
            string tableName = string.IsNullOrEmpty(view.EditableTableName) ? view.Name : view.EditableTableName;
            string fieldName = configFieldRow["Name"].ToString();
            string singleSelectTableName = null;
            if (values.ContainsKey("RelatedViewName") && values["RelatedViewName"] != null && !values["RelatedViewName"].Equals(string.Empty))
            {
                singleSelectTableName = values["RelatedViewName"].ToString().Replace(" ", "_");
            }
            else
            {
                singleSelectTableName = tableName + fieldName;
            }

            bool isRelatedViewExists = view.Database.Views.ContainsKey(singleSelectTableName);

            if (!isRelatedViewExists)
                return false;


            View relatedView = view.Database.Views[singleSelectTableName];

            string[] pkColumnNames = relatedView.GetPkColumnNames();

            
            if (pkColumnNames.Length != 1)
                return false;

            string pkColumnName = pkColumnNames[0];

            if (!relatedView.Fields.ContainsKey(pkColumnName))
                return false;

            Field pkField = relatedView.Fields[pkColumnName];

            if (!view.Fields.ContainsKey(fieldName))
                return false;

            Field fkField = view.Fields[fieldName];

            string[] fkColumnNames = fkField.GetColumnsNames();

            if (fkColumnNames.Length != 1)
                return false;

            string fkColumnName = fkColumnNames[0];

            if (!relatedView.DataTable.Columns.Contains(pkColumnName))
                return false;

            if (!view.DataTable.Columns.Contains(fkColumnName))
                return false;

            DataColumn pkColumn = relatedView.DataTable.Columns[pkColumnName];
            DataColumn fkColumn = view.DataTable.Columns[fkColumnName];


            return pkColumn.DataType.Equals(fkColumn.DataType);
        }

        private string ChangeDataTypeToSingleSelectForRefereneced(View view, Database database, string configFieldPk, View configView, string configViewPk, View configFieldView, DataSet dataset, Field defaultField, ParentField defaultParentField, Dictionary<string, string> viewDefaults, Database defaultDatabase, Dictionary<string, object> values, IDbCommand command)
        {
            string viewName = view.Name;
            string tableName = string.IsNullOrEmpty(view.EditableTableName) ? view.Name : view.EditableTableName;
            MapDataSet.ViewRow viewRow = viewRows[view.Name];

            ConfigAccess configAccess = new ConfigAccess();

            DataRow configFieldRow = configAccess.GetDataRow(configFieldView, configFieldPk);
            // get order
            object order = configFieldRow["Order"];
            object orderForCreate = configFieldRow["OrderForCreate"];
            object orderForEdit = configFieldRow["OrderForEdit"];

            if (configFieldRow == null)
                throw new DuradosException("Configuration field with id = " + configFieldPk + " is missing");


            string fieldName = configFieldRow["Name"].ToString();

            string columnName = fieldName;

            Type type = typeof(int);

            string singleSelectTableName = values["RelatedViewName"].ToString().Replace(" ", "_");

            DataTable singleSelectTable = null;

            singleSelectTable = view.Database.Views[singleSelectTableName].DataTable;


            SqlSchema sqlSchema = GetNewSqlSchema();

            try
            {

                string newTableName = string.IsNullOrEmpty(view.Database.Views[singleSelectTableName].EditableTableName) ? singleSelectTable.TableName : view.Database.Views[singleSelectTableName].EditableTableName;
                try
                {
                    sqlSchema.CreateFkConstraint(tableName, columnName, newTableName, singleSelectTable.PrimaryKey[0].ColumnName, command);
                }
                catch { }

                string relatedViewDisplayName = view.Database.Views[singleSelectTableName].DisplayField.DatabaseNames;

                string relatedViewPkName = null;
                if (view.Database.Views[singleSelectTableName].DataTable.PrimaryKey.Length != 1)
                {
                    throw new DuradosException("Dynamic mapping only supports a related table with a single primary key column. For primary key with multi columns please use design-time mapping.");
                }

                relatedViewPkName = view.Database.Views[singleSelectTableName].DataTable.PrimaryKey[0].ColumnName;

                string relatedViewOrdinal = null;
                relatedViewOrdinal = view.Database.Views[singleSelectTableName].OrdinalColumnName;


                Sync(view, configViewPk, configFieldView, defaultField, dataset, null, null, command);
                string newConfigFieldPk = GetNewConfigFieldPk(configAccess, configView, configViewPk, columnName);

                values["Order"] = order;
                values["OrderForCreate"] = orderForCreate;
                values["OrderForEdit"] = orderForEdit;

                ChangeColumnFieldToParent(view, singleSelectTable.TableName, newConfigFieldPk, configFieldView, defaultParentField, values, dataset);

                return columnName;

            }
            catch (Exception exception)
            {
                if (command != null && command.Transaction != null)
                    command.Transaction.Rollback();
                throw exception;
            }
        }

        private string ChangeDataTypeToSingleSelectForNewTableNotRefereneced(View view, Database database, string configFieldPk, View configView, string configViewPk, View configFieldView, DataSet dataset, Field defaultField, ParentField defaultParentField, Dictionary<string, string> viewDefaults, Database defaultDatabase, Dictionary<string, object> values, IDbCommand command)
        {
            string viewName = view.Name;
            string tableName = string.IsNullOrEmpty(view.EditableTableName) ? view.Name : view.EditableTableName;
            MapDataSet.ViewRow viewRow = viewRows[view.Name];

            ConfigAccess configAccess = new ConfigAccess();

            DataRow configFieldRow = configAccess.GetDataRow(configFieldView, configFieldPk);
            // get order
            object order = configFieldRow["Order"];
            object orderForCreate = configFieldRow["OrderForCreate"];
            object orderForEdit = configFieldRow["OrderForEdit"];

            if (configFieldRow == null)
                throw new DuradosException("Configuration field with id = " + configFieldPk + " is missing");


            string fieldName = configFieldRow["Name"].ToString();

            string columnName = GetParentFieldName(fieldName);

            Type type = typeof(int);

            DataType dataType = GetDataType(values);

            string singleSelectTableName = null;

            if (values.ContainsKey("RelatedViewName") && values["RelatedViewName"] != null && !values["RelatedViewName"].Equals(string.Empty))
            {
                singleSelectTableName = values["RelatedViewName"].ToString().Replace(" ", "_");
            }
            else
            {
                singleSelectTableName = tableName + fieldName;
            }

            bool isRelatedViewExists = view.Database.Views.ContainsKey(singleSelectTableName);
            bool systemView = isRelatedViewExists && view.Database.Views[singleSelectTableName].SystemView;
            if (!systemView)
            {
                if (values.ContainsKey("sysdb"))
                {
                    systemView = true;
                    if (string.IsNullOrEmpty(singleSelectTableName))
                    {
                        singleSelectTableName = tableName + fieldName;
                    }
                }
            }

            DataTable singleSelectTable = null;
            bool auto = true;

            if (isRelatedViewExists)
            {
                singleSelectTable = view.Database.Views[singleSelectTableName].DataTable;
            }
            else
            {
                if (values.ContainsKey("sysdb"))
                {
                    singleSelectTableName = Database.SystemRelatedViewPrefix + singleSelectTableName;
                }
                auto = !(view.Fields.ContainsKey(fieldName) && view.Fields[fieldName].FieldType == FieldType.Column && ((ColumnField)view.Fields[fieldName]).DataColumn.DataType.Equals(typeof(int)));
                singleSelectTable = GetSingleSelectTable(singleSelectTableName, auto);
            }

            if (isRelatedViewExists && view.Fields.ContainsKey(fieldName))
            {
                if (view.Fields[fieldName].GetColumnsNames().Length == 1 && view.DataTable.Columns.Contains(view.Fields[fieldName].GetColumnsNames()[0]) && singleSelectTable.PrimaryKey.Length == 1)
                {
                    Type fkType = view.DataTable.Columns[view.Fields[fieldName].GetColumnsNames()[0]].DataType;
                    Type pkType = singleSelectTable.PrimaryKey[0].DataType;
                    if (!fkType.Equals(pkType))
                    {
                        throw new DuradosException("The foriegn key column [" + view.DataTable + "].[" + view.Fields[fieldName].GetColumnsNames()[0] + "]" + " has a different type then [" + singleSelectTable.TableName + "].[" + singleSelectTable.PrimaryKey[0].ColumnName + "]. Cannot create the relation.");
                    }
                }
                else
                {
                    throw new DuradosException("Cannot create the relation");
                }
            }

            SqlSchema sqlSchema = GetNewSqlSchema();

            //IDbConnection connection = sqlSchema.GetConnection(ConnectionString);

            //IDbCommand command = sqlSchema.GetCommand();
            //command.Connection = connection;
            //command.Connection.Open();
            //command.Transaction = connection.BeginTransaction();

            IDbConnection sysConnection = new System.Data.SqlClient.SqlConnection(database.SysDbConnectionString);
            IDbCommand sysCommand = new System.Data.SqlClient.SqlCommand();

            try
            {

                if (singleSelectTable.PrimaryKey.Length == 1)
                {

                    string sqlDataType = sqlSchema.GetColumnDataType(singleSelectTable.TableName, singleSelectTable.PrimaryKey[0].ColumnName, command);
                    if (sqlDataType == null)
                    {
                        sqlSchema.AddNewColumnToTable(tableName, columnName, dataType, command);
                    }
                    else
                    {
                        sqlDataType += " null";
                        sqlSchema.AddNewColumnToTable(tableName, columnName, sqlDataType, command);
                    }
                }
                else
                {
                    sqlSchema.AddNewColumnToTable(tableName, columnName, dataType, command);
                }

                if (tableName != null && tableName != viewName)
                {
                    sqlSchema.AddNewColumnToView(viewName, columnName, command);
                }

                if (!isRelatedViewExists)
                {
                    if (systemView)
                    {
                        sysConnection.Open();
                        sysCommand.Connection = sysConnection;

                        SqlSchema systemSchema = new SqlSchema();
                        systemSchema.CreateTable(singleSelectTable, sysCommand);
                    }
                    else
                    {
                        sqlSchema.CreateTable(singleSelectTable, command);
                    }
                }

                string newTableName = null;
                if (isRelatedViewExists)
                {
                    newTableName = string.IsNullOrEmpty(view.Database.Views[singleSelectTableName].EditableTableName) ? singleSelectTable.TableName : view.Database.Views[singleSelectTableName].EditableTableName;
                }
                else
                {
                    newTableName = singleSelectTable.TableName;
                }

                if (!systemView)
                {
                    try
                    {
                        sqlSchema.CreateFkConstraint(tableName, columnName, newTableName, singleSelectTable.PrimaryKey[0].ColumnName, command);
                    }
                    catch { }
                }

                string relatedViewDisplayName = null;
                if (isRelatedViewExists)
                {
                    relatedViewDisplayName = view.Database.Views[singleSelectTableName].DisplayField.DatabaseNames;
                }
                else
                {
                    relatedViewDisplayName = "Name";
                }

                string relatedViewPkName = null;
                if (isRelatedViewExists)
                {
                    if (view.Database.Views[singleSelectTableName].DataTable.PrimaryKey.Length != 1)
                    {
                        throw new DuradosException("Dynamic mapping only supports a related table with a single primary key column. For primary key with multi columns please use design-time mapping.");
                    }

                    relatedViewPkName = view.Database.Views[singleSelectTableName].DataTable.PrimaryKey[0].ColumnName;
                }
                else
                {
                    relatedViewPkName = "Id";
                }

                string relatedViewOrdinal = null;
                if (isRelatedViewExists)
                {
                    relatedViewOrdinal = view.Database.Views[singleSelectTableName].OrdinalColumnName;
                }
                else
                {
                    viewDefaults.Add("OrdinalColumnName", "Ordinal");
                    relatedViewOrdinal = "Ordinal";
                }

                if (!systemView)
                {

                //string sql = "insert into [{0}] ([{3}]) select distinct [{1}].[{2}] from [{1}] where [{1}].[{2}] is not null and [{1}].[{2}] not in (select [{3}] from [{0}]) order by [{1}].[{2}]";
                command.CommandText = sqlSchema.GetInsertIntoNewParentStatement(newTableName, tableName, fieldName, relatedViewDisplayName); //string.Format(sql, newTableName, tableName, fieldName, relatedViewDisplayName);
                try
                {
                    command.ExecuteNonQuery();
                }
                catch 
                {
                    //if (!isRelatedViewExists)
                    //    throw exception;
                }

                if (!string.IsNullOrEmpty(relatedViewOrdinal))
                {
                    //sql = "update [{0}] set [{2}] = [{1}] where [{2}] is null ";
                    command.CommandText = sqlSchema.GetUpdateNewParentStatement(newTableName, relatedViewPkName, relatedViewOrdinal);
                    command.ExecuteNonQuery();
                }

                try
                {
                    if (!tableName.Equals(newTableName))
                    {
                        //sql = "update [{0}] set [{0}].[{1}] = [{2}].[{4}] from [{0}] inner join [{2}] on [{0}].[{3}] = [{2}].[{5}]";
                        command.CommandText = sqlSchema.GetUpdateNewNotNestedParentStatement(tableName, columnName, newTableName, fieldName, relatedViewPkName, relatedViewDisplayName);
                        command.ExecuteNonQuery();
                    }
                }
                catch { }
                }
                else
                {
                    if (!isRelatedViewExists)
                    {
                        SqlAccess sqlAccess = GetNewSqlAccess();
                        SqlAccess systemAccess = new SqlAccess();
                        string sql = sqlSchema.GetDistinctColumnFieldValues(tableName, fieldName, 1001); 
                        DataTable table = sqlAccess.ExecuteTable(((DuradosCommand)command).Command, sql, null);

                        if (table.Rows.Count > 1000)
                        {
                            throw new DuradosException("Changing a text column to relation column is limited to 1000 distinct values.");
                        }

                        foreach (DataRow row in table.Rows)
                        {
                            if (!row.IsNull(fieldName))
                            {
                                Dictionary<string, object> parameters = new Dictionary<string, object>();
                                parameters.Add("param", row[fieldName]);
                                if (auto)
                                {
                                    sql = string.Format("insert into [{0}] ([{1}]) select @param WHERE NOT EXISTS (select [{1}] from [{0}] where [{1}] = @param) ", newTableName, relatedViewDisplayName);
                                }
                                else
                                {
                                    sql = string.Format("insert into [{0}] ([{2}], [{1}]) select @param, @param WHERE NOT EXISTS (select [{1}] from [{0}] where [{1}] = @param) ", newTableName, relatedViewDisplayName, relatedViewPkName);
                                }
                                try
                                {
                                    systemAccess.ExecuteNonQuery(database.GetUserView(), sysCommand, sql, parameters);
                                }
                                catch (Exception exception)
                                {
                                    //throw new DuradosException("", exception);
                                }
                            }
                        }

                        sql = string.Format("select distinct top(1001) [{0}].[{1}], [{0}].[{2}] from [{0}]", newTableName, relatedViewPkName, relatedViewDisplayName);
                        table = systemAccess.ExecuteTable(database.SysDbConnectionString, sql, null, CommandType.Text);

                        if (table.Rows.Count > 1000)
                        {
                            throw new DuradosException("Changing a text column to relation column is limited to 1000 distinct values.");
                        }

                        foreach (DataRow row in table.Rows)
                        {
                            Dictionary<string, object> parameters = new Dictionary<string, object>();
                            command.Parameters.Clear();
                            parameters.Add("relatedViewPkName", row[relatedViewPkName]);
                            parameters.Add("relatedViewDisplayName", row[relatedViewDisplayName]);
                            sql = sqlSchema.GetUpdateStatementFromNewRelatedTable(tableName, columnName, fieldName);
                            try
                            {
                                sqlAccess.ExecuteNonQuery(view, command, sql, parameters);
                            }
                            catch (Exception exception)
                            {
                                //throw new DuradosException("", exception);
                            }
                        }
                    }
                }

                bool drop = false;

                if (drop)
                {
                    sqlSchema.RemoveColumnFromTable(string.IsNullOrEmpty(view.EditableTableName) ? view.DataTable.TableName : view.EditableTableName, fieldName, command);
                    if (!string.IsNullOrEmpty(view.EditableTableName) && view.EditableTableName != view.Name)
                        sqlSchema.RemoveColumnFromView(viewName, fieldName, command);
                }
                else
                {
                    string displayName = configFieldRow["DisplayName"].ToString();
                    configFieldRow["DisplayName"] = displayName + " old";
                    configFieldRow["Excluded"] = true;
                }

                Sync(view, configViewPk, configFieldView, defaultField, dataset, null, null, command);
                if (!isRelatedViewExists)
                {
                    if (!viewDefaults.ContainsKey("WorkspaceID"))
                        viewDefaults.Add("WorkspaceID", view.WorkspaceID.ToString());
                    else
                        viewDefaults["WorkspaceID"] = view.WorkspaceID.ToString();

                    if (systemView)
                    {
                        if (!viewDefaults.ContainsKey("SystemView"))
                            viewDefaults.Add("SystemView", true.ToString());
                        else
                            viewDefaults["SystemView"] = true.ToString();
                    }

                    if (systemView)
                    {
                        CreateView(singleSelectTable, database, configView, configFieldView, dataset, defaultField, defaultParentField, viewDefaults, false, sysCommand);
                    }
                    else
                    {
                        CreateView(singleSelectTable, database, configView, configFieldView, dataset, defaultField, defaultParentField, viewDefaults, false, command);

                    }
                    
                }
                //ParentField defaultParentField = (ParentField)defaultDatabase.Views["Table"].Fields.Values.Where(f => f.FieldType == FieldType.Parent).First();
                string newConfigFieldPk = GetNewConfigFieldPk(configAccess, configView, configViewPk, columnName);

                values["Order"] = order;
                values["OrderForCreate"] = orderForCreate;
                values["OrderForEdit"] = orderForEdit;

                ChangeColumnFieldToParent(view, singleSelectTable.TableName, newConfigFieldPk, configFieldView, defaultParentField, values, dataset);

                return columnName;
                //DataRow newConfigFieldRow = configAccess.GetDataRow(configFieldView, newConfigFieldPk);
                //newConfigFieldRow.BeginEdit();
                //newConfigFieldRow["Order"] = order;
                //newConfigFieldRow["OrderForCreate"] = orderForCreate;
                //newConfigFieldRow["OrderForEdit"] = orderForEdit;
                //newConfigFieldRow.EndEdit();
                //newConfigFieldRow.Table.DataSet.AcceptChanges();
                //ConfigAccess.SaveConfigDataset(configFieldView.Database.ConnectionString);

                //command.Transaction.Commit();
            }
            catch (Exception exception)
            {
                if (command != null && command.Transaction != null)
                    command.Transaction.Rollback();
                throw exception;
            }
            finally
            {
                if (sysConnection != null && sysConnection.State == ConnectionState.Open)
                    sysConnection.Close();
            }
        }

        private static DataType GetDataType(Dictionary<string, object> values)
        {
            //if (values.Keys.Contains("DataType") && !string.IsNullOrEmpty(values["DataType"].ToString()))
            //{
            //    try
            //    {
            //        DataType type = (DataType)Enum.Parse(typeof(DataType), values["DataType"].ToString());
            //        return type;
            //    }
            //    catch (Exception ex)
            //    {
            //        return DataType.SingleSelect;
            //        //throw new DuradosException("Field data type not exists.", ex);
            //    }

            //}
            return DataType.SingleSelect;

        }


        public string GetNewConfigFieldPk(ConfigAccess configAccess, View configView, string configViewPk, string newFieldName)
        {
            DataRow configViewRow = configAccess.GetDataRow(configView, configViewPk);

            DataRow[] fieldsRows = configViewRow.GetChildRows("Fields");

            foreach (DataRow row in fieldsRows)
            {
                if (row["Name"].ToString() == newFieldName)
                {
                    return row["ID"].ToString();
                }
            }

            return null;
        }

        private DataTable GetSingleSelectTable(string tableName)
        {
            return GetSingleSelectTable(tableName, true);
        }

        private DataTable GetSingleSelectTable(string tableName, bool auto)
        {
            DataTable table = new DataTable(tableName);

            DataColumn idColumn = table.Columns.Add("Id", typeof(int));
            if (auto)
                idColumn.AutoIncrement = true;
            else
                idColumn.AllowDBNull = false;
            DataColumn nameColumn = table.Columns.Add("Name", typeof(string));
            nameColumn.MaxLength = 250;

            table.Columns.Add("Ordinal", typeof(int));

            table.PrimaryKey = new DataColumn[1] { idColumn };

            return table;
        }

        private DataTable GetNewTable(string tableName)
        {
            DataTable table = new DataTable(tableName);

            DataColumn idColumn = table.Columns.Add("Id", typeof(int));
            idColumn.AutoIncrement = true;

            table.PrimaryKey = new DataColumn[1] { idColumn };

            return table;
        }

        private void ChangeDataTypeToSingleSelect2(View view, Database database, string configFieldPk, View configView, View configFieldView, DataSet dataset, Field defaultField, ParentField defaultParentField, Dictionary<string, string> viewDefaults)
        {
            string fieldName = null;
            try
            {
                MapDataSet.ViewRow viewRow = viewRows[view.Name];

                ConfigAccess configAccess = new ConfigAccess();

                DataRow configFieldRow = configAccess.GetDataRow(configFieldView, configFieldPk);

                if (configFieldRow == null)
                    throw new DuradosException("Configuration field with id = " + configFieldPk + " is missing");


                fieldName = configFieldRow["Name"].ToString();


                string newViewName = AddParentField(view, fieldName, database, configView, configFieldView, dataset, defaultField, defaultParentField, viewDefaults);
                MoveDataToParent(view.DataTable.TableName, newViewName, fieldName, GetParentFieldName(fieldName), view.Database.ConnectionString);

                bool drop = false;

                if (drop)
                {
                    RemoveOldColumn(view, fieldName);
                }
                else
                {
                    string displayName = configFieldRow["DisplayName"].ToString();
                    configFieldRow["DisplayName"] = displayName + " old";
                    configFieldRow["Excluded"] = true;
                }
            }
            catch (Exception exception)
            {
                if (fieldName != null)
                {
                    string columnName = GetParentFieldName(fieldName);
                    if (dataset.Tables[view.Name].Columns.Contains(columnName))
                    {
                        dataset.Tables[view.Name].Columns.Remove(columnName);
                    }
                }
                throw exception;
            }
        }

        private void RemoveOldColumn(View view, string columnName)
        {
            SqlSchema schema = GetNewSqlSchema();

            schema.RemoveColumnFromTable(string.IsNullOrEmpty(view.EditableTableName) ? view.DataTable.TableName : view.EditableTableName, columnName, view.Database.ConnectionString);
        }

        private void MoveDataToParent(string tableName, string newTableName, string oldColumnName, string newColumnName, string connectionString)
        {
            SqlAccess sqlAccess = new SqlAccess();

            string sql = "insert into [{0}] (Name) select distinct [{1}].[{2}] from [{1}] order by [{1}].[{2}]";
            sqlAccess.ExecuteNonQuery(connectionString, string.Format(sql, newTableName, tableName, oldColumnName));

            sql = "update [{0}] set Ordinal = Id ";
            sqlAccess.ExecuteNonQuery(connectionString, string.Format(sql, newTableName));
        }

        private string GetParentFieldName(string columnFieldName)
        {
            return columnFieldName + "Id";
        }

        private string AddParentField(View view, string fieldName, Database database, View configView, View configFieldView, DataSet dataset, Field defaultField, ParentField defaultParentField, Dictionary<string, string> viewDefaults)
        {
            string columnName = GetParentFieldName(fieldName);
            if (dataset.Tables[view.Name].Columns.Contains(columnName))
            {
                throw new DuradosException("The column " + columnName + " already exists");
            }

            AddNewColumn(view.Name, string.IsNullOrEmpty(view.EditableTableName) ? view.Name : view.EditableTableName, columnName, fieldName, typeof(int), defaultField, configFieldView, dataset);

            DataTable table = GetSingleSelectDataTable(fieldName);
            string newViewName = CreateView(table, database, configView, configFieldView, dataset, defaultField, defaultParentField, viewDefaults, false, null);
            ChangeDatasetColumnFieldToParent(view.Name, columnName, fieldName, dataset);

            return newViewName;
        }

        private void AddNewColumn(string viewName, string tableName, string columnName, string afterColumnName, Type type, Field defaultField, View configFieldView, DataSet dataset)
        {
            if (dataset.Tables[viewName].Columns.Contains(columnName))
            {
                throw new DuradosException("The column " + columnName + " already exists");
            }
            SqlSchema sqlSchema = GetNewSqlSchema();

            IDbConnection connection = sqlSchema.GetConnection(ConnectionString);

            IDbCommand command = sqlSchema.GetCommand();
            command.Connection = connection;
            command.Connection.Open();
            command.Transaction = connection.BeginTransaction();

            try
            {
                DataType dataType = DataType.ShortText;
                if (type == typeof(int))
                    dataType = DataType.SingleSelect;

                sqlSchema.AddNewColumnToTable(tableName, columnName, dataType, command);

                if (tableName != null && tableName != viewName)
                {
                    sqlSchema.AddNewColumnToView(viewName, columnName, command);
                }

                DataColumn newColumn = new DataColumn(columnName, type);

                List<DataColumn> newColumns = new List<DataColumn>();
                newColumns.Add(newColumn);

                ConfigAccess configAccess = new ConfigAccess();

                string configViewPk = configAccess.GetViewPK(viewName, configFieldView.Database.ConnectionString);

                int maxOrder = configAccess.GetMaxFieldOrder(viewName, "Order", configFieldView.Database.ConnectionString);
                AddColumnsConfiguration(newColumns, defaultField, configAccess, configViewPk, configFieldView, maxOrder, null);

                dataset.Tables[viewName].Columns.Add(newColumn);

                command.Transaction.Commit();
            }
            catch (Exception exception)
            {
                if (command != null && command.Transaction != null)
                    command.Transaction.Rollback();
                throw exception;
            }
            finally
            {
                connection.Close();
            }
        }

        private DataTable GetSingleSelectDataTable(string tableName)
        {
            DataTable table = new DataTable(tableName);

            table.Columns.Add("Name", typeof(string));
            table.Columns.Add("Ordinal", typeof(int));

            return table;
        }

        protected virtual SqlSchema GetNewSqlSchema()
        {
            return new SqlSchema();
        }

        protected virtual SqlSchema GetNewSqlSchema(IDbCommand command)
        {
            if (command is System.Data.SqlClient.SqlCommand)
                return new SqlSchema();
            else
                return GetNewSqlSchema();
        }

        protected virtual SqlAccess GetNewSqlAccess()
        {
            return new SqlAccess();
        }

        public string GetTableEntity()
        {
            return GetNewSqlSchema().GetTableEntity();
        }

        public DataView GetSchemaEntities(View schemaView)
        {

            SqlSchema sqlSchema = GetNewSqlSchema();

            IDbConnection connection = sqlSchema.GetConnection(ConnectionString);

            IDbCommand command = sqlSchema.GetCommand();
            command.Connection = connection;
            try
            {
                command.Connection.Open();
            }
            catch (Exception exception)
            {
                throw new DuradosException("Connection failue. Please check your connection.", exception);
            }
            try
            {
                Dictionary<string, SqlSchema.Entity> entities = sqlSchema.GetEntities(command);

                DataTable table = schemaView.DataTable.Copy();

                foreach (string name in entities.Keys)
                {
                    DataRow row = table.NewRow();
                    row["Name"] = name;
                    row["EntityType"] = entities[name].EntityType;
                    row["Schema"] = entities[name].Schema;

                    table.Rows.Add(row);
                }

                return new DataView(table);
            }
            finally
            {
                connection.Close();
            }

        }

        public void Encrypt(ColumnField field)
        {
            if (field.DataType != DataType.ShortText)
            {
                throw new DuradosException("Field must be a Short Text");
            }

            SqlSchema sqlSchema = GetNewSqlSchema();
            IDbCommand command = sqlSchema.GetCommand();
            IDbConnection connection = sqlSchema.GetConnection(field.View.ConnectionString);
            try
            {

                command = sqlSchema.GetCommand();
                command.Connection = connection;
                command.Connection.Open();

                command.Transaction = connection.BeginTransaction();

                // Check if master key, certification and symmetric key exists and if not create them
                sqlSchema.CreateMasterCertificateAndSymmetric(field.View.Database.DefaultMasterKeyPassword, field.GetCertificateName(), field.GetSymmetricKeyName(), field.SymmetricKeyAlgorithm.ToString(), command);


                // add encrypted column
                sqlSchema.AddNewColumnToTable(field.View.GetTableName(), field.DatabaseNames + field.EncryptedNameSuffix, "VARBINARY(256) NULL", command);

                // update encrypted column
                string sql = "OPEN SYMMETRIC KEY {0} DECRYPTION " +
                        "BY CERTIFICATE {1} " +
                        "UPDATE {2} " +
                        "SET {3} = ENCRYPTBYKEY(KEY_GUID('{0}'), {4}) " +
                        "close SYMMETRIC KEY {0} ";
                command.CommandText = string.Format(sql, field.GetSymmetricKeyName(), field.GetCertificateName(), field.View.GetTableName(), field.DatabaseNames + field.EncryptedNameSuffix, field.DatabaseNames);
                command.ExecuteNonQuery();

                // remove decrypted column
                sqlSchema.RemoveColumnFromTable(field.View.GetTableName(), field.DatabaseNames, command);


                // change column name 
                //if (!string.IsNullOrEmpty( field.View.EditableTableName) && !field.View.EditableTableName.Equals(field.View.DataTable.TableName))
                //    sqlSchema.ChangeColumnNameInView(field.View.DataTable.TableName, field.DatabaseNames, field.EncryptedName, command);
                sqlSchema.RenameColumn(field.View.GetTableName(), field.DatabaseNames + field.EncryptedNameSuffix, field.DatabaseNames, command);
                command.Transaction.Commit();
            }
            catch (Exception exception)
            {
                command.Transaction.Rollback();
                throw exception;
            }
            finally
            {
                connection.Close();
            }
        }

        public virtual string GetGenerateScriptFileName(string fileName)
        {
            return fileName;
        }

        public virtual History GetHistoryGenerator(string systemConnectionString, string historySchemaGeneratorFileName)
        {
           return new Durados.DataAccess.AutoGeneration.History(systemConnectionString, historySchemaGeneratorFileName);
        }

        public virtual Cloud GetCloudGenerator(string systemConnectionString, string historySchemaGeneratorFileName)
        {
            return new Durados.DataAccess.AutoGeneration.Cloud(systemConnectionString, historySchemaGeneratorFileName);
        }

        public virtual Durados.DataAccess.AutoGeneration.PersistentSession GetPersistentSessionGenerator(string systemConnectionString, string sessionSchemaGeneratorFileName)
        {
            return new Durados.DataAccess.AutoGeneration.PersistentSession(systemConnectionString, sessionSchemaGeneratorFileName);
        }

        public virtual Content GetPersistentContentGenerator(string systemConnectionString, string contentSchemaGeneratorFileName)
        {
            return new Durados.DataAccess.AutoGeneration.Content(systemConnectionString, contentSchemaGeneratorFileName);
        }
        public virtual Link GetLinkGenerator(string systemConnectionString, string contentSchemaGeneratorFileName)
        {
            return new Durados.DataAccess.AutoGeneration.Link(systemConnectionString, contentSchemaGeneratorFileName);
        }

        public virtual CustomViews GetCustomViewsGenerator(string systemConnectionString, string contentSchemaGeneratorFileName)
        {
            return new Durados.DataAccess.AutoGeneration.CustomViews(systemConnectionString, contentSchemaGeneratorFileName);
        }
        public virtual Durados.DataAccess.AutoGeneration.User GetUserGenerator(string systemConnectionString, string contentSchemaGeneratorFileName)
        {
            return new Durados.DataAccess.AutoGeneration.User(systemConnectionString, contentSchemaGeneratorFileName);
        }

        //public virtual object GetBoolean(bool b)
        //{
        //    if (b)
        //        return true;
        //    return false;
        //}
    }

    public enum RelationChange
    {
        None,
        ColumnToParent,
        ParentToColumn,
        ParentToDifferentParent
    }

    public class Statistics
    {
        public int TotalViews { get; set; }
        public int ViewsCount { get; set; }
        public int TotalImplicitRelations { get; set; }
        public int TotalExplicitRelations { get; set; }
        public Category[] Categories { get; set; }
        public View[] Views { get; set; }
        public bool Finished { get; set; }

        public class Category
        {
            public string Name { get; set; }
            public int ViewsCount { get; set; }

        }

        public class View
        {
            public string Name { get; set; }
            public bool Success { get; set; }
            public string Message { get; set; }
        }
    }
}
