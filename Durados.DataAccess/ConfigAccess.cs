using System;
using System.Configuration;
using System.Data;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Durados.Data;
using Durados.DataAccess.Storage;

namespace Durados.DataAccess
{
    public class ConfigTransaction
    {
        private Guid tranGuid = Guid.NewGuid();
        private List<int> pendingHistoryForCommit = new List<int>();
        private History history = null;
        private Func<IStorage> storageGetter;
       
        private AppLockerGetter lockGetter {
            get
            {
                if (storageGetter() != null)
                {
                    if (_lockGetter == null)
                    {
                        _lockGetter = new AppLockerGetter(storageGetter().LockerCache);
                    }

                    return _lockGetter;
                }

                throw new NotSupportedException("You have to initalize storage in MAP before call ConfigTransaction");
            }
        }

        private AppLockerGetter _lockGetter;

        /**/
        public ConfigTransaction(Func<IStorage> storage)
        {
            this.storageGetter = storage;
            
        }

        private void InitTransaction()
        {
            tranGuid = Guid.NewGuid();
            pendingHistoryForCommit = new List<int>();
        }

        public void AddPendingHistoryId(int id)
        {
            pendingHistoryForCommit.Add(id);
        }

        public void Commit(string connectionString, DataSet dataSet, string filename, Durados.Diagnostics.ILogger logger)
        {
            history = DataAccess.History.GetHistory(DataAccess.History.GetProduct(connectionString));
            lock (GetLocker(filename))
            {
                dataSet.AcceptChanges();

                string tempFileName = filename + ".temp_" + Guid.NewGuid().ToString() + ".xml";
                try
                {
                    if (ConfigAccess.cloud && ConfigAccess.storage != null && !ConfigAccess.storage.IsMainApp(filename))
                    {
                        ConfigAccess.storage.WriteConfigToCloud(dataSet, filename);
                    }
                    else
                    {
                        dataSet.WriteXml(tempFileName, XmlWriteMode.WriteSchema);
                    }
                }
                catch (Exception exception)
                {
                    if (logger != null)
                        logger.Log("ConfigAccess", "Commit", "WriteXml", exception, 1, "filename=" + filename);
                    throw new DuradosException("Failed to WriteXml config file. " + filename, exception);
                }

                //dataSet.WriteXml(filename, XmlWriteMode.WriteSchema);

                if (!string.IsNullOrEmpty(connectionString))
                    history.Commit(tranGuid, pendingHistoryForCommit.ToArray(), connectionString);

                InitTransaction();

                if (ConfigAccess.cloud && ConfigAccess.storage != null && !ConfigAccess.storage.IsMainApp(filename))
                    return;

                try
                {
                    if (!FileHelper.ValidateConfig(tempFileName, filename))
                        return;
                }
                catch (Exception exception)
                {
                    if (logger != null)
                        logger.Log("ConfigAccess", "Commit", "ValidateConfig", exception, 2, "filename=" + filename);
                }

                try
                {
                    File.Copy(tempFileName, filename, true);
                }
                catch (Exception exception)
                {
                    if (logger != null)
                        logger.Log("ConfigAccess", "Commit", "Copy", exception, 1, "filename=" + filename);
                    if (!System.IO.File.Exists(filename))
                    {
                        throw new DuradosException("Missing config file. " + filename, exception);
                    }
                }
                finally
                {
                    try
                    {
                        File.Delete(tempFileName);
                    }
                    catch (Exception exception)
                    {
                        if (logger != null)
                            logger.Log("ConfigAccess", "Commit", "delete temp", exception, 2, "filename=" + filename);
                    }
                }
            }
        }


        private object GetLocker(string filename)
        {
            if (filename == null)
            {
                throw new ArgumentNullException("filename can't be null");
            }

            return lockGetter.GetLock(filename);
            
        }
    }

    public class ConfigAccess : SqlAccess, IDataTableAccess
    {
        private static DataSet dataSet2 = null;
        public static bool multiTenancy = false;
        public static bool cloud = false;
        
        public static Durados.DataAccess.Storage.IStorage storage { get; set; }

        public static Func<IStorage> storageGetter = () => storage; 

        //private static Dictionary<string, DataSet> dataSets = new Dictionary<string, DataSet>();
        private static Durados.Data.ICache<DataSet> dataSets
        {
            get
            {
                if (storage != null)
                {
                    return storage.ConfigCache;
                }
                return null;
            }
        }

        private static ConfigTransaction configTransaction = new ConfigTransaction(storageGetter);


        //private static DataSet dataSet
        //{
        //    get
        //    {

        //        return dataSet2;
        //    }
        //    set
        //    {
        //        dataSet2 = value;
        //    }
        //}



        private static DataSet GetDataSet(string fileName)
        {
            if (multiTenancy)
            {
               
                DataSet myDataSet = dataSets.Get(fileName);
                
                // no double lock avoid optimistic concurrency
                if (myDataSet == null)
                {
                    // create dataset
                    try
                    {
                        var dataset = new DataSet();

                        if (cloud && storage != null && !storage.IsMainApp(fileName))
                        {
                            storage.ReadConfigFromCloud(dataset, fileName);
                        }
                        else
                        {
                            dataset.ReadXml(fileName, XmlReadMode.ReadSchema);
                        }

                        // add to cache
                        dataSets.Add(fileName, dataset);
                        myDataSet = dataset;
                    }
                    catch (Exception e)
                    {
                        throw new DuradosException("could not read configuration from cloud into datasdet properly for " + fileName, e);
                    }
                      
                }
               
                HandleOrder(myDataSet, fileName);
                return myDataSet;
            }
            else  // non multitenancy, shouldn't arrive  here
            {
                if (dataSet2 == null)
                {
                    dataSet2 = new DataSet();

                    dataSet2.ReadXml(fileName, XmlReadMode.ReadSchema);

                }

                return dataSet2;
            }
        }

        private static void HandleOrder(DataSet dataSet, string fileName)
        {
            const string Order = "Order";
            const string View = "View";
            if (!dataSet.Tables.Contains(View))
                return;

            DataTable table = dataSet.Tables[View];
            if (table.Rows.Count <= 0)
                return;

            DataRow[] rows = table.Select(string.Empty, Order + " desc");

            if (rows.Length == 0)
                return;

            int lastOrder = rows[0].IsNull(Order) ? 0 : (int)rows[0][Order];

            bool reorder = false;

            foreach (DataRow row in table.Rows)
            {
                if (row.IsNull(Order) || row[Order].Equals(0))
                {
                    reorder = true;
                    lastOrder += 10;
                    row[Order] = lastOrder;
                }
            }

            if (reorder)
            {
                dataSet.AcceptChanges();
                if (ConfigAccess.cloud && ConfigAccess.storage != null && !ConfigAccess.storage.IsMainApp(fileName))
                {
                    ConfigAccess.storage.WriteConfigToCloud(dataSet, fileName);
                }
                else
                {
                    dataSet.WriteXml(fileName, XmlWriteMode.WriteSchema);
                }
            }
        }

        public static void Restart(string fileName)
        {
            if (multiTenancy)
            {
                if (dataSets.ContainsKey(fileName))
                {
                    dataSets.Remove(fileName);
                }
            }
        }

        public static void Refresh(string fileName)
        {
            if (multiTenancy)
            {
                if (!dataSets.ContainsKey(fileName))
                {
                    dataSets.Add(fileName, null);
                }
                dataSets[fileName] = new DataSet();
                if (cloud && storage != null && !storage.IsMainApp(fileName))
                {
                    storage.ReadConfigFromCloud(dataSets[fileName], fileName);
                }
                else
                {
                    dataSets[fileName].ReadXml(fileName, XmlReadMode.ReadSchema);
                } 
                HandleOrder(dataSets[fileName], fileName);
            }
            else
            {
                dataSet2 = new DataSet();

                dataSet2.ReadXml(fileName, XmlReadMode.ReadSchema);
            }
        }

        private static string connectionString;
        
        

        const char comma = ',';
        const string PageIndexerColumnName = "PageIndexer";

        public override DataView FillPage(View view, int page, int pageSize, Dictionary<string, object> values, bool? search, bool? useLike, Dictionary<string, SortDirection> sortColumns, out int rowCount, BeforeSelectEventHandler beforeSelectCallback, AfterSelectEventHandler afterSelectCallback)
        {
            if (beforeSelectCallback != null)
                beforeSelectCallback(this, new SelectEventArgs(view, null));

            Filter filter;
            if (search.HasValue && search.Value)
            {
                filter = GetFilter(view, values, LogicCondition.Or, true, useLike);
            }
            else
            {
                filter = GetFilter(view, values, LogicCondition.And, false, useLike);
            }

            string sortColumn = null;
            SortDirection direction = SortDirection.Asc;

            if (sortColumns != null)
            {
                if (sortColumns.Count > 0)
                {
                    sortColumn = sortColumns.Keys.ToList()[0];
                    direction = sortColumns[sortColumn];

                }
            }

            DataTable dataTable = FillDataTable(view, page, pageSize, filter, sortColumn, direction, out rowCount, null, null, beforeSelectCallback, afterSelectCallback);

            DataView dataView = new DataView(dataTable);

            string whereStatement = filter.GetWhereStatementWithoutParameters();
            if (!string.IsNullOrEmpty(whereStatement))
            {
                try
                {
                    dataView.RowFilter = (whereStatement + (string.IsNullOrEmpty(view.PermanentFilter) ? "" : " and " + view.PermanentFilter)).Replace(" = N'", " = '").Replace("like N'", "Like '").Replace("<> N'", "<> '");
                }
                catch { }
            }

            if (!string.IsNullOrEmpty(sortColumn) && view.Fields.ContainsKey(sortColumn))
            {
                if (view.Fields[sortColumn] is ColumnField)
                    try
                    {
                        dataView.Sort = ((ColumnField)view.Fields[sortColumn]).DataColumn.ColumnName + " " + direction.ToString();
                    }
                    catch { }
                else if (view.Fields[sortColumn] is ParentField)
                {
                    try
                    {
                        dataView.Sort = ((ParentField)view.Fields[sortColumn]).GetColumnsNames()[0] + " " + direction.ToString();
                    }
                    catch { }
                }
            }
            else if (!string.IsNullOrEmpty(view.DefaultSort))
            {
                dataView.Sort = view.DefaultSort;
            }
            else if (!string.IsNullOrEmpty(view.OrdinalColumnName) && dataView.Table.Columns.Contains(view.OrdinalColumnName))
            {
                dataView.Sort = view.OrdinalColumnName + " " + direction.ToString();
            }
            if (afterSelectCallback != null)
                afterSelectCallback(this, new SelectEventArgs(view, null));

            rowCount = dataView.Count;

            if (rowCount <= 100)
                FilterPage(view, dataView, page, pageSize);

            return dataView;
        }

        public static void UpdateVersion(string version, string filename)
        {
            DataSet ds = GetDataSet(filename);
            if (ds.Tables.Contains("Database") && ds.Tables["Database"].Rows.Count == 1 && ds.Tables["Database"].Columns.Contains("ConfigVersion"))
                ds.Tables["Database"].Rows[0]["ConfigVersion"] = version;
        }

        public override DataTable FillDataTable(View view, int page, int pageSize, Filter filter, string sortColumn, SortDirection direction, out int totalRowCount, ParentField parentField, string join, BeforeSelectEventHandler beforeSelectCallback, AfterSelectEventHandler afterSelectCallback)
        {
            string filename = view.Database.ConnectionString;

            //DataSet ds = new DataSet();

            //ds.ReadXml(filename, XmlReadMode.ReadSchema);


            DataSet ds = GetDataSet(filename).Copy();

            DataTable dataTable = ds.Tables[view.DataTable.TableName];
            totalRowCount = dataTable.Rows.Count;

            return dataTable;
        }

        protected virtual void FilterPage(View view, DataView dataView, int page, int pageSize)
        {
            //DataTable table = dataView.Table.Clone();
            //for (int i = (page - 1) * pageSize; i < page * pageSize - 1; i++)
            //{
            //    if (i >= dataView.Count)
            //    {
            //        break;
            //    }
            //    table.ImportRow(dataView[i].Row);
            //}
            //return new DataView(table);



            //for (int i = 0; i < dataView.Count; i++)
            //{
            //    if (i >= dataView.Count)
            //    {
            //        break;
            //    }
            //    if (!(i >= (page - 1) * pageSize && i <= page * pageSize - 1))
            //    {
            //        dataView[i].Row.Delete();
            //    }
            //}
            //return new DataView(dataView.Table); 

            string filter = string.Empty;

            for (int i = (page - 1) * pageSize; i < page * pageSize; i++)
            {
                if (i >= dataView.Count)
                {
                    break;
                }

                filter += view.GetPkValue(dataView[i].Row, true) + ",";
            }

            filter = filter.TrimEnd(',');

            if (!string.IsNullOrEmpty(filter))
            {
                filter = " ID in (" + filter + ")";
                if (!string.IsNullOrEmpty(dataView.RowFilter))
                {
                    filter = dataView.RowFilter + " and " + filter;
                }

                dataView.RowFilter = filter;
            }
        }

        //public DataRow GetViewRow(string viewName)
        //{
        //    return GetViewRow(viewName, null);
        //}

        public DataRow GetRow(string viewName, string filterFieldName, string filterValue, string filename)
        {
            DataSet ds;
            //if (string.IsNullOrEmpty(filename))
            //{
            //    if (dataSet == null)
            //        return null;
            //    ds = dataSet;
            //}
            //else
            //{
            ds = GetDataSet(filename);
            //}

            DataView dataView = new DataView(ds.Tables[viewName]);
            dataView.RowFilter = filterFieldName + " = '" + filterValue + "'";
            if (dataView.Count != 1)
                return null;

            return dataView[0].Row;
        }

        public DataView GetRows(string viewName, string filterFieldName, string filterValue, string filename)
        {
            return GetRows(viewName, filterFieldName + " = '" + filterValue + "'", filename);
        }

        public DataView GetRows(string viewName, string whereStatement, string filename)
        {
            DataSet ds;
            ds = GetDataSet(filename);
            
            DataView dataView = new DataView(ds.Tables[viewName]);
            dataView.RowFilter = whereStatement;
            
            return dataView;
        }

        public DataRow GetViewRow(string viewName, string filename)
        {
            DataSet ds;
            //if (string.IsNullOrEmpty(filename))
            //{
            //    if (dataSet == null)
            //        return null;
            //    ds = dataSet;
            //}
            //else
            //{
            ds = GetDataSet(filename);
            //}

            DataView dataView = new DataView(ds.Tables["View"]);
            dataView.RowFilter = "Name = '" + viewName + "'";
            if (dataView.Count != 1)
                return null;

            return dataView[0].Row;

        }

        public string GetFieldPK(string fieldName, string filename)
        {
            DataSet ds = GetDataSet(filename);

            foreach (DataRow row in ds.Tables["Field"].Rows)
            {
                if (row["Name"].Equals(fieldName))
                    return row["ID"].ToString();
            }

            return null;
        }

        public DataRow GetFieldRow(string viewName, string fieldName, string filename)
        {
            DataRow viewRow = GetViewRow(viewName, filename);
            if (viewRow == null)
                return null;

            foreach (DataRow row in viewRow.GetChildRows("Fields"))
            {
                if (row["Name"].Equals(fieldName))
                    return row;
            }

            return null;
        }

        public DataRow[] GetFieldsRows(string viewName, string filename)
        {
            DataRow viewRow = GetViewRow(viewName, filename);
            if (viewRow == null)
                return null;

            return viewRow.GetChildRows("Fields");
        }

        public string GetFieldPK(string viewName, string fieldName, string filename)
        {
            DataRow row = GetFieldRow(viewName, fieldName, filename);
            if (row == null)
                return null;
            return row["ID"].ToString();
        }

        public string GetMenuPK(string menuName, string filename)
        {
            DataRow row = GetRow("Menu", "Name", menuName, filename);
            if (row == null)
                return null;
            return row["ID"].ToString();
        }

        public string GetViewPK(string viewName, string filename)
        {
            DataRow row = GetViewRow(viewName, filename);
            if (row == null)
                return null;
            return row["ID"].ToString();
        }

        //public bool HideField(string viewName, string fieldName, string filename)
        //{
        //    DataRow row = GetFieldRow(viewName, fieldName, filename);
        //    if (row == null)
        //        return false;
        //    row["HideInTable"] = true;
        //    return true;
        //}

        public override DataRow GetDataRow(View view, string pk, DataSet dataset, BeforeSelectEventHandler beforeSelectCallback, AfterSelectEventHandler afterSelectCallback, bool usePermanentFilter = false)
        {
            return GetDataRow(view, pk);
        }

        public override DataRow GetDataRow(View view, string pk, DataSet dataset)
        {
            return GetDataRow(view, pk);
        }

        public override DataRow GetDataRow(View view, string pk)
        {
            string filename = view.Database.ConnectionString;

            //DataSet ds = new DataSet();
            //ds.ReadXml(filename, XmlReadMode.ReadSchema);
            string viewName = view.Name;

            return GetDataRow(viewName, filename, pk);
        }

        private string GetCloneViewName(string viewName)
        {
            if (viewName == "MenuOrganizerView")
                return "View";
            else
                return viewName;
        }

        public DataRow GetDataRow(string viewName, string filename, string pk)
        {
            //DataSet ds = new DataSet();
            //ds.ReadXml(filename, XmlReadMode.ReadSchema);
            DataSet ds = GetDataSet(filename);

            viewName = GetCloneViewName(viewName);

            DataTable dataTable = ds.Tables[viewName];
            int id = 0;
            bool isInt = Int32.TryParse(pk, out id);
            if (!isInt)
                return null;

            DataRow dataRow = dataTable.Rows.Find(id);

            return dataRow;
        }

        public string GetViewNameByPK(string pk, string filename)
        {
            DataRow row = GetDataRow("View", filename, pk);

            if (row == null)
                return null;

            if (row.Table.Columns.Contains("Name"))
            {
                string name = row["Name"].ToString();
                return name;
            }

            return null;
        }

        public string GetFieldNameByPK(string pk, string filename)
        {
            DataRow row = GetDataRow("Field", filename, pk);

            if (row.Table.Columns.Contains("Name"))
            {
                string name = row["Name"].ToString();
                return name;
            }

            return null;
        }
        public string  GetViewPKByFieldPK(string fieldPK, string filename)
        {
            DataRow row = GetDataRow("Field", filename, fieldPK);

            if (row.Table.Columns.Contains("Fields"))
            {
                string  viewPk = row["Fields"].ToString();
                return viewPk;
            }

            return null;
        }

        protected override System.Data.IDataParameter GetNewSqlParameter(View view, string name, object value)
        {
            if (value is string && value != null && value != string.Empty && view.Fields.ContainsKey(name) && view.Fields[name].FieldType == FieldType.Column && ((ColumnField)view.Fields[name]).Encrypted)
            {
                string encryptedValue = Durados.Security.CipherUtility.Encrypt<System.Security.Cryptography.AesManaged>(value.ToString(), view.Database.DefaultMasterKeyPassword, view.Database.Salt);
                return base.GetNewSqlParameter(view, name, encryptedValue);
            }
            else
                return base.GetNewSqlParameter(view, name, value);
        }

        protected override long GetOrder(View view, string insertAbovePK, string fk, ParentField integralParentField, System.Data.IDbCommand command)
        {
            Int64 inc = 10;
            string filename = view.Database.ConnectionString;

            DataSet ds = GetDataSet(filename);

            DataTable table = ds.Tables[view.Name];

            if (table.Rows.Count == 0 || !table.Columns.Contains(view.OrdinalColumnName))
                return inc;

            DataView dataView = new DataView(table);

            dataView.Sort = view.OrdinalColumnName;

            if (!string.IsNullOrEmpty(insertAbovePK))
            {
                Int64 ordinal = inc;
                for (int i = dataView.Count - 1; i >= 0; i--)
                {
                    string pk = view.GetPkValue(dataView[i].Row);

                    if (pk == insertAbovePK)
                    {
                        break;
                    }
                    if (dataView[dataView.Count - 1].Row.IsNull(view.OrdinalColumnName))
                    {
                        break;
                    }

                    ordinal = Convert.ToInt64(dataView[i].Row[view.OrdinalColumnName]);

                    dataView[i].Row[view.OrdinalColumnName] = ordinal + inc;
                }

                return ordinal;
            }
            else
            {
                if (dataView[dataView.Count - 1].Row.IsNull(view.OrdinalColumnName))
                    return inc;
                Int64 ordinal = Convert.ToInt64(dataView[dataView.Count - 1].Row[view.OrdinalColumnName]);
                return ordinal + inc;
            }
        }

        public override DataRow GetNewRow(View view, Dictionary<string, object> values, string insertAbovePK, BeforeCreateEventHandler beforeCreateCallback, BeforeCreateInDatabaseEventHandler beforeCreateInDatabaseEventHandler, AfterCreateEventHandler afterCreateBeforeCommitCallback, AfterCreateEventHandler afterCreateAfterCommitCallback)
        {
            CreateEventArgs createEventArgs = new CreateEventArgs(view, values, null, null, null);
            if (beforeCreateCallback != null)
                beforeCreateCallback(this, createEventArgs);

            if (createEventArgs.Cancel)
                return null;

            string filename = view.Database.ConnectionString;

            //DataSet ds = new DataSet();
            //ds.ReadXml(filename, XmlReadMode.ReadSchema);
            DataSet ds = GetDataSet(filename);

            DataTable dataTable = ds.Tables[view.Name];
            DataRow dataRow = dataTable.NewRow();

            HandleOrder(view, values, insertAbovePK, null);

            System.Data.IDataParameter[] parameters = GetParemeters(view, values);

            foreach (System.Data.IDataParameter parameter in parameters)
            {
                dataRow[parameter.ParameterName] = parameter.Value;
            }

            dataTable.Rows.Add(dataRow);

            ds.AcceptChanges();

            History history = null;
            int? userId = null;
            if (createEventArgs != null && createEventArgs.History != null)
            {
                history = (History)createEventArgs.History;
                userId = createEventArgs.UserId;
                //History history = (History)editEventArgs.History;

            }

            string pk = view.GetPkValue(dataRow);

            if (history != null)
            {
                if (view.Database.SysDbConnectionString != null)
                    connectionString = view.Database.SysDbConnectionString;
                else
                    connectionString = view.Database.DbConnectionString;

                System.Data.IDbCommand command;
                if (createEventArgs.SysCommand != null)
                {
                    command = createEventArgs.SysCommand;
                }
                else
                {
                    //command = GetNewCommand(view);
                    command = GetNewCommand(string.Empty, GetNewConnection(view.Database.SystemSqlProduct, connectionString), view.Database.SystemSqlProduct);
                   
                }
                command.Connection.Open();
                try
                {
                    string objectName = pk;
                    int id = DataAccess.History.GetHistory(view.Database.SystemSqlProduct).SaveCreate(command, view, objectName, userId.Value, view.Database.SwVersion, view.GetWorkspaceName());
                    configTransaction.AddPendingHistoryId(id);
                }
                finally
                {
                    command.Connection.Close();
                }
            }
            //ds.WriteXml(filename, XmlWriteMode.WriteSchema);
            //SaveConfigDataset(filename);

            createEventArgs = new CreateEventArgs(view, values, null, null, null);
            createEventArgs.PrimaryKey = pk;
            if (afterCreateAfterCommitCallback != null)
                afterCreateAfterCommitCallback(this, createEventArgs);
            try
            {
                view.SendRealTimeEvent(pk, Crud.create);
            }
            catch { }

            return dataRow;
        }

        public void Clone(View view, string baseName, string name, IEnumerable<string> excludeRelations)
        {
            string filename = view.Database.ConnectionString;

            DataSet ds = GetDataSet(filename);

            DataTable dataTable = ds.Tables[view.Name];

            DataRow[] rows = dataTable.Select("Name = '" + baseName + "'");

            if (rows.Length == 0)
            {
                throw new ViewNotFoundExeption(baseName);
            }
            else if (rows.Length > 1)
            {
                throw new MoreThanOneViewExeption(baseName);
            }

            DataRow baseRow = rows[0];

            DataRow row = CopyRow(baseRow, excludeRelations);

            row["Name"] = name;
            row["BaseName"] = baseName;

            SaveConfigDataset(filename, view.Database.Logger);

        }

        private DataRow CopyRow(DataRow baseRow, IEnumerable<string> excludeRelations)
        {
            DataRow row = baseRow.Table.NewRow();

            foreach (DataColumn dataColumn in baseRow.Table.Columns)
            {
                if (!dataColumn.AutoIncrement)
                {
                    row[dataColumn] = baseRow[dataColumn];
                }
            }

            baseRow.Table.Rows.Add(row);

            foreach (DataRelation dataRelation in baseRow.Table.ChildRelations)
            {
                if (!excludeRelations.Contains(dataRelation.RelationName))
                {
                    DataRow[] baseChildRows = baseRow.GetChildRows(dataRelation);

                    foreach (DataRow baseChildRow in baseChildRows)
                    {
                        DataRow childRow = CopyRow(baseChildRow, excludeRelations);
                        childRow.SetParentRow(row, dataRelation);
                    }
                }
            }

            return row;
        }

        public static void SaveConfigDataset(string filename, Durados.Diagnostics.ILogger logger)
        {
            if (multiTenancy)
            {
                UpdateHistory(GetDataSet(filename), filename, logger);
            }
            else
            {
                if (dataSet2 != null)
                {

                    UpdateHistory(dataSet2, filename, logger);
                }
            }
        }

        public static bool ContainsColumnValue(string viewName, string columnName, string value, string fileName)
        {
            DataSet dataSet = GetDataSet(fileName);

            if (dataSet != null)
            {
                // validate input is valid
                if (!dataSet.Tables.Contains(viewName))
                {
                    StringBuilder builder = new StringBuilder();
                    builder.Append("Count: ");
                    builder.Append(dataSet.Tables.Count);

                  
                    foreach (DataTable table in dataSet.Tables)
	                {
                        builder.Append(table.TableName);
                        builder.Append(",");
	                }        

                    throw new DuradosException("Can't find view name " + viewName + "  in dataset " + fileName + " " + builder.ToString());
                }

                return dataSet.Tables[viewName].Select(columnName + " = '" + value + "'").Count() > 0;
            }

            return false;
        }

        public static bool ContainsName(string viewName, string name, string fileName)
        {
            return ContainsColumnValue(viewName, "Name", name, fileName);
        }

        public static bool ContainsWorkspaceName(string name, string fileName)
        {
            return ContainsName("Workspace", name, fileName);
        }

        public static bool ContainsCategoryName(string name, string fileName)
        {
            return ContainsName("Category", name, fileName);
        }

        public static int? GetWorkspaceId(string name, string fileName)
        {
            return GetFirstId("Workspace", "Name", name, fileName);
        }

        public static int? GetCategoryId(string name, string fileName)
        {
            int? id = GetFirstId("Category", "Name", name, fileName);
            if (id.HasValue)
                return id;

            DataSet dataSet = GetDataSet(fileName);
            DataRow row = dataSet.Tables["Category"].NewRow();
            row["Name"] = "General";
            row["Ordinal"] = 0;
            dataSet.Tables["Category"].Rows.Add(row);
            dataSet.AcceptChanges();

            return Convert.ToInt32(row["ID"]);
        }

        public static int? GetFirstId(string viewName, string columnName, string value, string fileName)
        {
            DataSet dataSet = GetDataSet(fileName);

            if (dataSet != null)
            {
                if (dataSet.Tables.Contains(viewName))
                {
                    DataRow[] rows = dataSet.Tables[viewName].Select(columnName + " = '" + value + "'");
                    if (rows.Count() > 0)
                        return Convert.ToInt32(rows[0]["ID"]);
                    else
                        return null;
                }
            }

            return null;
        }

        public static int? AddCategory(string fileName)
        {
           // return AddCategory(fileName, "General", 0);
            return null;
        }

        public static int AddCategory(string fileName, string name, int ordinal)
        {
            DataSet dataSet = GetDataSet(fileName);
            DataTable categoryTable = dataSet.Tables["Category"];

            DataRow categoryRow = categoryTable.NewRow();

            categoryRow["Name"] = name;
            categoryRow["Ordinal"] = ordinal;

            categoryTable.Rows.Add(categoryRow);

            return (int)categoryRow["ID"];
        }

        public static int? GetFirstCategoryId(string configViewPk, string fileName)
        {
            DataSet dataSet = GetDataSet(fileName);

            if (dataSet != null)
            {
                DataTable viewTable = dataSet.Tables["View"];
                DataRow viewRow = viewTable.Rows.Find(Convert.ToInt32(configViewPk));
                if (viewRow == null)
                    return null;

                HashSet<int> categories = new HashSet<int>();
                foreach (DataRow row in viewRow.GetChildRows("Fields"))
                {
                    if (!row.IsNull("Category"))
                    {
                        int categoryId = (int)row["Category"];
                        if (!categories.Contains(categoryId))
                        {
                            categories.Add(categoryId);
                        }
                    }
                }

                if (categories.Count == 0)
                    return null;

                if (categories.Count == 1)
                    return categories.FirstOrDefault();

                DataTable categoryTable = dataSet.Tables["Category"];

                int minOrd = int.MaxValue;
                int? minCategoryId = null;

                foreach (int category in categories)
                {
                    DataRow categoryRow = categoryTable.Rows.Find(category);
                    if (categoryRow != null)
                    {
                        if (!categoryRow.IsNull("Ordinal"))
                        {
                            int min = (int)categoryRow["Ordinal"];
                            if (min <= minOrd)
                            {
                                minOrd = min;
                                minCategoryId = category;
                            }
                        }
                    }
                }

                if (minCategoryId.HasValue)
                    return minCategoryId.Value;

            }

            return null;
        }

        public static string GetDatabasePK(string fileName)
        {
            DataSet dataSet = GetDataSet(fileName);

            if (dataSet != null && dataSet.Tables["Database"].Rows.Count == 1)
            {

                return dataSet.Tables["Database"].Rows[0][dataSet.Tables["Database"].PrimaryKey[0].ColumnName].ToString();
            }

            return null;
        }

        private static void UpdateHistory(DataSet dataSet, string filename, Durados.Diagnostics.ILogger logger)
        {
            configTransaction.Commit(connectionString, dataSet, filename, logger);
        }

        private DataRow CopyDataRow(DataRow row)
        {
            DataRow copyRow = row.Table.NewRow();
            foreach (DataColumn column in row.Table.Columns)
            {
                copyRow[column] = row[column];
            }

            return copyRow;
        }

        public override void Edit(View view, Dictionary<string, object> values, string pk, BeforeEditEventHandler beforeEditCallback, BeforeEditInDatabaseEventHandler beforeEditInDatabaseEventHandler, AfterEditEventHandler afterEditBeforeCommitCallback, AfterEditEventHandler afterEditAfterCommitCallback)
        {


            string filename = view.Database.ConnectionString;

            //DataSet ds = new DataSet();
            //ds.ReadXml(filename, XmlReadMode.ReadSchema);
            DataSet ds = GetDataSet(filename);

            DataTable dataTable = ds.Tables[view.DataTable.TableName];
            DataRow dataRow = dataTable.Rows.Find(Convert.ToInt32(pk));
            if (dataRow == null)
                return;

            EditEventArgs editEventArgs = null;
            if (beforeEditCallback != null)
            {
                editEventArgs = new EditEventArgs(view, values, pk, null, null);
                editEventArgs.PrevRow = dataRow;

                beforeEditCallback(this, editEventArgs);
            }

            if (editEventArgs != null && editEventArgs.Cancel)
                return;

            DataRow prevRow = CopyDataRow(dataRow);


            System.Data.IDataParameter[] parameters = GetParemeters(view, values);

            dataRow.BeginEdit();
            foreach (System.Data.IDataParameter parameter in parameters)
            {
                dataRow[parameter.ParameterName] = parameter.Value;
            }
            dataRow.EndEdit();
            ds.AcceptChanges();

            History history = null;
            int? userId = null;
            if (editEventArgs != null && editEventArgs.History != null)
            {
                history = (History)editEventArgs.History;
                userId = editEventArgs.UserId;
                //History history = (History)editEventArgs.History;

            }

            if (history != null)
            {
                //connectionString = view.Database.DbConnectionString;
                if (view.Database.SysDbConnectionString != null)
                    connectionString = view.Database.SysDbConnectionString;
                else
                    connectionString = view.Database.DbConnectionString;

                System.Data.IDbCommand command;
                if (editEventArgs != null && editEventArgs.SysCommand != null && editEventArgs.SysCommand.Connection != null)
                {
                    command = editEventArgs.SysCommand;
                }
                else
                {
                    command = GetNewCommand(string.Empty, GetNewConnection(view.Database.SystemSqlProduct, connectionString), view.Database.SystemSqlProduct);
                   
                }
                try
                {
                    command.Connection.Open();
                    OldNewValue[] oldNewValues = null;
                    string objectName = pk;
                    
                    int? id = DataAccess.History.GetHistory(view.Database.SystemSqlProduct).SaveEdit(command, view, prevRow, values, objectName, userId.Value, out oldNewValues, view.Database.SwVersion, view.GetWorkspaceName()); //((History)history).SaveEdit(command, view, prevRow, values, pk, userId.Value, out oldNewValues, view.Database.SwVersion, view.GetWorkspaceName());
                    if (id.HasValue)
                        configTransaction.AddPendingHistoryId(id.Value);
                }
                finally
                {
                    try
                    {
                        command.Connection.Close();
                    }
                    catch { }
                }
            }

            //ds.WriteXml(filename, XmlWriteMode.WriteSchema);
            //SaveConfigDataset(filename);

            if (afterEditAfterCommitCallback != null)
                afterEditAfterCommitCallback(this, new EditEventArgs(view, values, pk, null, null));

            try
            {
                view.SendRealTimeEvent(pk, Crud.update);
            }
            catch { }

        }

        public override void Delete(View view, string pk, BeforeDeleteEventHandler beforeDeleteCallback, AfterDeleteEventHandler afterDeleteBeforeCommitCallback, AfterDeleteEventHandler afterDeleteAfterCommitCallback, Dictionary<string, object> values = null)
        {
            DeleteEventArgs deleteEventArgs = new DeleteEventArgs(view, pk, null, null);
            if (beforeDeleteCallback != null)
                beforeDeleteCallback(this, deleteEventArgs);

            string filename = view.Database.ConnectionString;

            //DataSet ds = new DataSet();
            //ds.ReadXml(filename, XmlReadMode.ReadSchema);
            DataSet ds = GetDataSet(filename);

            DataTable dataTable = ds.Tables[view.Name];
            DataRow dataRow = dataTable.Rows.Find(Convert.ToInt32(pk));

            if (dataRow == null)
                return;

            DataRow prevRow = CopyDataRow(dataRow);

            dataRow.Delete();

            ds.AcceptChanges();

            History history = null;
            int? userId = null;
            if (deleteEventArgs != null && deleteEventArgs.History != null)
            {
                history = (History)deleteEventArgs.History;
                userId = deleteEventArgs.UserId;
                //History history = (History)editEventArgs.History;

            }

            if (history != null)
            {
                if (view.Database.SysDbConnectionString != null)
                    connectionString = view.Database.SysDbConnectionString;
                else
                    connectionString = view.Database.DbConnectionString;

               IDbCommand command;
                if (deleteEventArgs.Command != null)
                {
                    command = deleteEventArgs.SysCommand;
                }
                else
                {
                    command = GetNewCommand(string.Empty, GetNewConnection(view.Database.SystemSqlProduct, connectionString), view.Database.SystemSqlProduct);
                  
                }
                command.Connection.Open();
                try
                {
                    string objectName = pk;
                    
                    int id = DataAccess.History.GetHistory(view.Database.SystemSqlProduct).SaveDelete(command, view, objectName, userId.Value, view.Database.SwVersion, view.GetWorkspaceName(), prevRow);
                    configTransaction.AddPendingHistoryId(id);
                }
                finally
                {
                    command.Connection.Close();
                }
            }
            //ds.WriteXml(filename, XmlWriteMode.WriteSchema);
            //SaveConfigDataset(filename);

            if (afterDeleteAfterCommitCallback != null)
                afterDeleteAfterCommitCallback(this, new DeleteEventArgs(view, null, null, null));

            try
            {
                view.SendRealTimeEvent(pk, Crud.delete);
            }
            catch { }
        }

        public override string GetDisplayValue(ParentField parentField, string pk)
        {
            if (string.IsNullOrEmpty(pk))
                return string.Empty;

            View view = parentField.ParentView;

            string filename = view.Database.ConnectionString;

            //DataSet ds = new DataSet();

            //ds.ReadXml(filename, XmlReadMode.ReadSchema);
            DataSet ds = GetDataSet(filename);

            DataTable dataTable = ds.Tables[view.Name];



            string displayColumnName = string.Empty;

            if (view.DisplayField.FieldType == FieldType.Column)
            {
                displayColumnName = view.DisplayField.Name;
            }
            else
            {
                Field displayField = view.DisplayField;

                while (displayField.FieldType == FieldType.Parent)
                {
                    displayField = ((ParentField)displayField).ParentView.DisplayField;
                }
                displayColumnName = displayField.Name;
            }

            BeforeDropDownOptionsEventArgs eventArgs = new BeforeDropDownOptionsEventArgs(parentField, string.Empty);

            parentField.OnBeforeDropDownOptions(eventArgs);


            Dictionary<string, string> selectOptions = new Dictionary<string, string>();
            object[] keys = new object[view.DataTable.PrimaryKey.Count()];

            if (view.DataTable.PrimaryKey[0].DataType == typeof(int))
            {
                keys[0] = Convert.ToInt32(pk);
            }
            DataRow dataRow = dataTable.Rows.Find(keys);

            string display = "";

            if (view.DisplayField.FieldType == FieldType.Column)
            {
                display = dataRow[displayColumnName].ToString();
            }
            else
            {
                Field displayField = view.DisplayField;
                DataRow parentRow = null;
                while (displayField.FieldType == FieldType.Parent)
                {
                    displayField = ((ParentField)displayField).ParentView.DisplayField;
                    parentRow = dataRow.GetParentRow(displayField.Name);
                }
                display = parentRow[displayColumnName].ToString();
            }


            return display;
        }

        public int GetMaxFieldOrder(string viewName, string orderColumnName, string fileName)
        {
            int max = 0;

            DataRow viewRow = GetViewRow(viewName, fileName);
            if (viewRow == null)
                return 0;

            foreach (DataRow fieldRow in viewRow.GetChildRows("Fields"))
            {
                int order = 0;
                if (fieldRow.Table.Columns.Contains(orderColumnName))
                {
                    if (!fieldRow.IsNull(orderColumnName))
                    {
                        order = Convert.ToInt32(fieldRow[orderColumnName]);
                    }
                }

                max = Math.Max(max, order);
            }

            return max;
        }

        protected override string GetMatchingFilter(string columnName, string tableName, string match, bool startWith, ISqlTextBuilder sqlTextBuilder)
        {
            string like = "like ";
            if (startWith)
                like += "'";
            else
                like += "'%";
            like += match.Replace("'", "''") + "%' ";

            return columnName + " " + like;
        }

        public override Dictionary<string, string> GetSelectOptions(ColumnField columnField, string filter, int? top)
        {
            if (!string.IsNullOrEmpty(columnField.AutocompleteSql))
                return base.GetSelectOptions(columnField, filter, top);

            Dictionary<string, string> selectOptions = new Dictionary<string, string>();

            if (!string.IsNullOrEmpty(columnField.MultiValueAdditionals))
            {
                string[] additionals = columnField.MultiValueAdditionals.Split(',');
                for (int i = 0; i < additionals.Length; i++)
                {
                    string additional = additionals[i];
                    if (!selectOptions.ContainsKey(additional))
                        selectOptions.Add(additional, additional);
                }
            }


            if (!string.IsNullOrEmpty(columnField.AutocompleteTable))
            {
                if (columnField.View.Database.Views.ContainsKey(columnField.AutocompleteTable))
                {
                    int totalRowCount = 0;

                    View view = columnField.View.Database.Views[columnField.AutocompleteTable];
                    DataTable table = FillDataTable(view, 1, 1000, null, null, SortDirection.Asc, out totalRowCount, null, null, null, null);
                    DataView dataView = new DataView(table);
                    try
                    {
                        dataView.RowFilter = filter;
                    }
                    catch { }

                    foreach (System.Data.DataRowView row in dataView)
                    {
                        string key = string.Empty;
                        if (!row.Row.IsNull(columnField.AutocompleteColumn))
                            key = row[columnField.AutocompleteColumn].ToString();

                        string display = key;
                        if (!string.IsNullOrEmpty(columnField.AutocompleteDisplayColumn) && !row.Row.IsNull(columnField.AutocompleteDisplayColumn))
                            display = row[columnField.AutocompleteDisplayColumn].ToString();

                        selectOptions.Add(key, display);
                    }
                }
            }

            return selectOptions;
        }

        //changed by br 2
        public override Dictionary<string, string> GetSelectOptions(ParentField parentField, bool forFilter, bool useUniqueName, int? top, Filter filter)
        {

            View view = parentField.ParentView;

            string filename = view.Database.ConnectionString;

            //DataSet ds = new DataSet();

            //ds.ReadXml(filename, XmlReadMode.ReadSchema);
            DataSet ds = GetDataSet(filename);

            DataTable dataTable = ds.Tables[view.Name];



            string displayColumnName = string.Empty;

            if (view.DisplayField.FieldType == FieldType.Column)
            {
                displayColumnName = view.DisplayField.Name;
            }
            else
            {
                Field displayField = view.DisplayField;

                while (displayField.FieldType == FieldType.Parent)
                {
                    displayField = ((ParentField)displayField).ParentView.DisplayField;
                }
                displayColumnName = displayField.Name;
            }

            BeforeDropDownOptionsEventArgs eventArgs = new BeforeDropDownOptionsEventArgs(parentField, string.Empty);

            parentField.OnBeforeDropDownOptions(eventArgs);


            Dictionary<string, string> selectOptions = new Dictionary<string, string>();

            foreach (DataRow dataRow in dataTable.Rows)
            {
                string pk = string.Empty;

                foreach (DataColumn column in view.DataTable.PrimaryKey)
                {
                    pk += dataRow[column.ColumnName].ToString() + ",";
                }

                pk = pk.TrimEnd(',');

                string display = "";

                if (view.DisplayField.FieldType == FieldType.Column)
                {
                    display = dataRow[displayColumnName].ToString();
                }
                else
                {
                    Field displayField = view.DisplayField;
                    DataRow parentRow = null;
                    while (displayField.FieldType == FieldType.Parent)
                    {
                        displayField = ((ParentField)displayField).ParentView.DisplayField;
                        parentRow = dataRow.GetParentRow(displayField.Name);
                    }
                    display = parentRow[displayColumnName].ToString();
                }

                selectOptions.Add(pk, display);

            }

            List<KeyValuePair<string, string>> sorted1 = selectOptions.OrderBy(k => k.Value).ToList();
            Dictionary<string, string> sorted2 = new Dictionary<string, string>();

            foreach (var pair in sorted1)
            {
                sorted2.Add(pair.Key, pair.Value);
            }
            return sorted2;
        }

        public Dictionary<string, string> GetMenuSelectOptions(string workspaceID, string filename)
        {
            Dictionary<string, string> selectOptions = new Dictionary<string, string>();

            foreach (DataRow row in GetDataSet(filename).Tables["Menu"].Rows)
            {
                if (row["WorkspaceID"].ToString().Equals(workspaceID) || string.IsNullOrEmpty(workspaceID))
                {
                    selectOptions.Add(row["ID"].ToString(), row["Name"].ToString());
                }
            }

            return selectOptions;
        }

        internal class RowOrder
        {
            internal int Order { get; set; }
            internal string ID { get; set; }
        }

        public IList<string> GetSortedPKs(DataSet ds, View view, string sortBy, string filename)
        {
            IList<string> sortedPKs = new List<string>();

            return sortedPKs;
        }    
    
        /// <summary>
        /// Get pks of a specific view sorted by ordinalColumnName.
        /// </summary>
        /// <param name="view"></param>
        /// <param name="ordinalColumnName"></param>
        /// <param name="filename"></param>
        /// <param name="byDataRow"></param>
        /// <returns></returns>
        public string[] GetSortedPKs(View view, string ordinalColumnName, string filename, string byPk)
        {
            const string FIELDS_VIEW_NAME = "Field";
            const string VIEW_VIEW_NAME = "View";

            string[] sortedPks = null;

            switch (view.Name)
            {
                case FIELDS_VIEW_NAME:
                    string fieldsViewPk = GetViewPKByFieldPK(byPk, filename);
                    string fieldsViewName = GetViewNameByPK(fieldsViewPk, filename);
                    sortedPks = GetSortedFieldsPKs(fieldsViewName, ordinalColumnName, filename);
                    break;
                case VIEW_VIEW_NAME:
                    //br TODO
                    sortedPks = GetSortedPKs(view, ordinalColumnName, filename);
                    break;
                default:
                    sortedPks = GetSortedPKs(view, ordinalColumnName, filename);
                    break;
            }

            return sortedPks;
        }

        /// <summary>
        /// Get pks of a specific view sorted by ordinalColumnName
        /// </summary>
        /// <param name="view"></param>
        /// <param name="ordinalColumnName"></param>
        /// <param name="filename"></param>
        /// <returns></returns>
        public string[] GetSortedPKs(View view, string ordinalColumnName, string filename)
        {
            List<RowOrder> sortedPKs = new List<RowOrder>();

            if (view.PrimaryKeyFileds != null && view.PrimaryKeyFileds.Count() > 0)
            {
                DataSet ds = GetDataSet(filename);
                DataTable dataTable = ds.Tables[view.DataTable.TableName];
                Field PKFiled = null;
                string PKName = string.Empty;

                PKFiled = view.PrimaryKeyFileds[0];
                PKName = PKFiled.Name;

                foreach (DataRow row in dataTable.Rows)
                {
                    sortedPKs.Add(new RowOrder() { Order = Convert.ToInt32(row[ordinalColumnName]), ID = row[PKName].ToString() });
                }
            }

            return sortedPKs.OrderBy(row => row.Order).Select(row => row.ID).ToArray();
        }

        /// <summary>
        /// Get pks of a Fields view sorted by ordinalColumnName.
        /// </summary>
        /// <param name="viewName"></param>
        /// <param name="sortBy"></param>
        /// <param name="filename"></param>
        /// <returns></returns>
        public string[] GetSortedFieldsPKs(string viewName, string sortBy, string filename)
        {
            DataRow viewRow = GetViewRow(viewName, filename);

            List<RowOrder> sortedPKs = new List<RowOrder>();

            foreach (DataRow row in viewRow.GetChildRows("Fields"))
            {
                sortedPKs.Add(new RowOrder() { Order = Convert.ToInt32(row[sortBy]), ID = row["ID"].ToString() });
            }

            return sortedPKs.OrderBy(row => row.Order).Select(row => row.ID).ToArray();
        }

        /// <summary>
        /// Move field to new positioin- replace ordinal values for fields between o_pk, d_pk
        /// </summary>
        /// <param name="view"></param>
        /// <param name="viewName"></param>
        /// <param name="o_pk"></param>
        /// <param name="d_pk"></param>
        /// <param name="userID"></param>
        /// <param name="before"></param>
        public void MoveField(View view, string viewName, string o_pk, string d_pk, int userID, bool before)
        {
            ChangeOrdinal(view, o_pk, d_pk, userID, before);
        }

        /// <summary>
        /// Get fixed d_pk by 2 conditions:0
        /// *direction of movement- up or down. *New position- before pk or after it
        /// </summary>
        /// <param name="sortedPks"></param>
        /// <param name="o_pkOrdinal"></param>
        /// <param name="d_pkOrdinal"></param>
        /// <param name="d_pk"></param>
        /// <param name="before"></param>
        /// <returns></returns>
        private string getFixedPkForChangeOrdinal(IEnumerable<string> sortedPks, int o_pkOrdinal, int d_pkOrdinal, string d_pk, bool before)
        {
            string fixedPk = d_pk;

            //If we move o_pk down, and after d_pk- we want o_pk get ordinal of element after d_pk
            if (o_pkOrdinal > d_pkOrdinal)
            {
                if (!before)
                {
                    int d_pkIndex = sortedPks.ToList().IndexOf(d_pk);

                    fixedPk = sortedPks.ElementAtOrDefault(d_pkIndex + 1);
                }
            }
            //If we move o_pk up, and before d_pk- we want o_pk get ordinal of element before d_pk
            else
            {
                if (before)
                {
                    int d_pkIndex = sortedPks.ToList().IndexOf(d_pk);

                    fixedPk = sortedPks.ElementAtOrDefault(d_pkIndex - 1);
                }
            }

            return fixedPk;
        }

        /// <summary>
        /// Change ordinal of records between o_pk and d_pk- in order to move o_pk to d_pk place
        /// </summary>
        /// <param name="view"></param>
        /// <param name="o_pk"></param>
        /// <param name="d_pk"></param>
        /// <param name="userID"></param>
        /// <param name="before"></param>
        private void ChangeOrdinal(View view, string o_pk, string d_pk, int userID, bool? before)
        {
            string filename = view.Database.ConnectionString;
            string ordinalColumnName = view.OrdinalColumnName ?? "Order";
            IList<string> sortedPks = null;

            //Get dataTable
            DataSet ds = GetDataSet(filename);
            DataTable dataTable = ds.Tables[view.DataTable.TableName];

            //Validate if o_pk exist in view
            DataRow oRow = dataTable.Rows.Find(Convert.ToInt32(o_pk));
            if (oRow == null)
                return;

            //Validate if d_pk exist in view
            DataRow dRow = dataTable.Rows.Find(Convert.ToInt32(d_pk));
            if (dRow == null)
                return;

            //Get sortedPks (If its FIELDS_VIEW_NAME- Get fields by oRow view)
            sortedPks = GetSortedPKs(view, ordinalColumnName, filename, o_pk).ToList();

            //Get ordinal values for o_pk, d_pk
            int o_pkOrdinal = Convert.ToInt32(oRow[ordinalColumnName]);
            int d_pkOrdinal = Convert.ToInt32(dRow[ordinalColumnName]);

            //If ordinal equals- do nothing
            if (o_pkOrdinal == d_pkOrdinal)
                return;

            //Get fixed d_pk by movement direction and by before value
            if (before.HasValue)
            {
                d_pk = getFixedPkForChangeOrdinal(sortedPks, o_pkOrdinal, d_pkOrdinal, d_pk, before.Value);
                if (string.IsNullOrEmpty(d_pk))
                {
                    return;
                }
            }

            //Reverse items if movement direction is down
            if (o_pkOrdinal > d_pkOrdinal)
            {
                sortedPks = sortedPks.Reverse().ToList();
            }

            //Get sortedPks between o_pk and d_pk
            int o_pkIndex = sortedPks.IndexOf(o_pk);
            int d_pkIndex = sortedPks.IndexOf(d_pk);

            sortedPks = sortedPks.Skip(o_pkIndex + 1).Take(d_pkIndex - o_pkIndex).ToList();

            connectionString = view.Database.SysDbConnectionString;
            IDbCommand command;
            SqlProduct sysSqlProduct = view.Database.SystemSqlProduct;
            command = GetNewCommand(string.Empty, GetNewConnection(sysSqlProduct, connectionString), sysSqlProduct);
        
            command.Connection.Open();

            try
            {
                sortedPks.ToList().ForEach(pk => Switch(view, o_pk, pk, userID, command));
            }
            finally
            {
                command.Connection.Close();
            }
        }

        /// <summary>
        /// Change ordinal of records between o_pk and d_pk- in order to move o_pk to d_pk place
        /// </summary>
        /// <param name="view"></param>
        /// <param name="o_pk"></param>
        /// <param name="d_pk"></param>
        /// <param name="userID"></param>
        public override void ChangeOrdinal(View view, string o_pk, string d_pk, int userID)
        {
            ChangeOrdinal(view, o_pk, d_pk, userID, null);
        }

        public override void Switch(View view, string o_pk, string d_pk, int userID)
        {
            connectionString = view.Database.SysDbConnectionString;
            IDbCommand command;

            command = GetNewCommand(view);
          
            command.Connection.Open();

            try
            {
                Switch(view, o_pk, d_pk, userID, command);
            }
            finally
            {
                command.Connection.Close();
            }
        }

        public void ChangeViewsAndPagesWorkspace(View menuView, string menuId, string workspaceId, int userID, IDbCommand command)
        {
            string filename = menuView.Database.ConnectionString;
            DataSet ds = GetDataSet(filename);
            string workspaceIdColumnName = "WorkspaceId";

            DataTable menuTable = ds.Tables[menuView.Name];
            DataRow menuRow = menuTable.Rows.Find(Convert.ToInt32(menuId));

            DataRow[] viewRows = menuRow.GetChildRows("Menu");

            View viewView = menuView.Database.Views["View"];

            ChangeWorkspace(viewView, viewRows, workspaceId, workspaceIdColumnName, userID, command);

            DataRow[] pageRows = menuRow.GetChildRows("PageMenu");

            View pageView = menuView.Database.Views["Page"];

            ChangeWorkspace(pageView, pageRows, workspaceId, workspaceIdColumnName, userID, command);
            //foreach (DataRow viewRow in viewRows)
            //{
            //    DataRow prevRow = viewRow.Table.NewRow();
            //    foreach (DataColumn column in viewRow.Table.Columns)
            //    {
            //        prevRow[column] = viewRow[column];
            //    }

            //    viewRow.BeginEdit();
            //    viewRow[workspaceIdColumnName] = Convert.ToInt32(workspaceId);
            //    viewRow.EndEdit();

            //    Dictionary<string, object> values = new Dictionary<string, object>();
            //    values.Add(workspaceIdColumnName, workspaceId);
            //    SaveHistory(menuView, prevRow, values, viewRow["ID"].ToString(), userID, command);
            //}

            //DataRow[] pageRows = menuRow.GetChildRows("");
        }

        private void ChangeWorkspace(View view, DataRow[] rows, string workspaceId, string workspaceIdColumnName, int userID, IDbCommand command)
        {
            foreach (DataRow row in rows)
            {
                DataRow prevRow = row.Table.NewRow();
                foreach (DataColumn column in row.Table.Columns)
                {
                    prevRow[column] = row[column];
                }

                row.BeginEdit();
                row[workspaceIdColumnName] = Convert.ToInt32(workspaceId);
                row.EndEdit();

                Dictionary<string, object> values = new Dictionary<string, object>();
                values.Add(workspaceIdColumnName, workspaceId);
                SaveHistory(view, prevRow, values, row["ID"].ToString(), userID, command);
            }
        }

        public void ApplyToAll(System.Collections.IEnumerable rows, Dictionary<string, object> values)
        {
            foreach (DataRow row in rows)
            {
                row.BeginEdit();
                foreach (string columnName in values.Keys)
                {
                    row[columnName] = values[columnName];
                }
                row.EndEdit();
            }
        }

        public void ApplyToAll(string viewName, string fileName, Dictionary<string, object> values)
        {
            DataSet ds = GetDataSet(fileName);

            DataTable dataTable = ds.Tables[viewName];

            ApplyToAll(dataTable.Rows, values);
        }

        public void Switch(View view, string o_pk, string d_pk, int userID, IDbCommand command)
        {
            string filename = view.Database.ConnectionString;
            string ordinalColumnName = view.OrdinalColumnName ?? "Order";

            //DataSet ds = new DataSet();
            //ds.ReadXml(filename, XmlReadMode.ReadSchema);
            DataSet ds = GetDataSet(filename);

            DataTable dataTable = ds.Tables[view.DataTable.TableName];
            DataRow oRow = dataTable.Rows.Find(Convert.ToInt32(o_pk));

            if (oRow == null)
                return;

            DataRow prevoRow = dataTable.NewRow();
            foreach (DataColumn column in dataTable.Columns)
            {
                prevoRow[column] = oRow[column];
            }


            DataRow dRow = dataTable.Rows.Find(Convert.ToInt32(d_pk));

            if (dRow == null)
                return;

            DataRow prevdRow = dataTable.NewRow();
            foreach (DataColumn column in dataTable.Columns)
            {
                prevdRow[column] = dRow[column];
            }

            object oOrdinal = oRow[ordinalColumnName];
            object dOrdinal = dRow[ordinalColumnName];

            oRow.BeginEdit();
            oRow[ordinalColumnName] = dOrdinal;
            oRow.EndEdit();

            Dictionary<string, object> ovalues = new Dictionary<string, object>();
            ovalues.Add(ordinalColumnName, dOrdinal);
            SaveHistory(view, prevoRow, ovalues, o_pk, userID, command);


            dRow.BeginEdit();
            dRow[ordinalColumnName] = oOrdinal;
            dRow.EndEdit();

            Dictionary<string, object> dvalues = new Dictionary<string, object>();
            dvalues.Add(ordinalColumnName, oOrdinal);
            SaveHistory(view, prevdRow, dvalues, d_pk, userID, command);

            //SaveConfigDataset(filename);


            //ds.AcceptChanges();

            //ds.WriteXml(filename, XmlWriteMode.WriteSchema);

        }

        private void SaveHistory(View view, DataRow prevRow, Dictionary<string, object> values, string pk, int userID, IDbCommand command)
        {
            History history = DataAccess.History.GetHistory(view.Database.SystemSqlProduct); 

            OldNewValue[] oldNewValues = null;
            int? id = history.SaveEdit(command, view, prevRow, values, pk, userID, out oldNewValues, view.Database.SwVersion, view.GetWorkspaceName());
            if (id.HasValue)
                configTransaction.AddPendingHistoryId(id.Value);
        }

        public void Reorder(string viewPK, string fileName)
        {
            //if (dataSet == null)
            //    return;

            DataSet dataSet = GetDataSet(fileName);

            DataView dataView = new DataView(dataSet.Tables["Field"]);

            dataView.RowFilter = "Fields = " + viewPK;
            dataView.Sort = "Order asc";

            int order = 0;
            foreach (DataRowView row in dataView)
            {
                order += 10;
                row["Order"] = order;
            }

            dataView.Sort = "OrderForCreate asc";

            order = 0;
            foreach (DataRowView row in dataView)
            {
                order += 10;
                row["OrderForCreate"] = order;
            }

            dataView.Sort = "OrderForEdit asc";

            order = 0;
            foreach (DataRowView row in dataView)
            {
                order += 10;
                row["OrderForEdit"] = order;
            }
        }

        public void Reorder(string fieldPK, string filename, Dictionary<string, object> values, View view)
        {

            //DataSet ds = GetDataSet(filename);

            //DataTable dataTable = ds.Tables["Field"];
            //DataRow fRow = dataTable.Rows.Find(fieldPK);

            //DataRow viewRow = fRow.GetParentRow("Fields");

            //if (viewRow != null)
            //{
            //    DataRow[] fieldRows = viewRow.GetChildRows("Fields");

            //    foreach (DataRow fieldRow in fieldRows)
            //    {
            //        fieldRow["OrderForCreate"] = fieldRow["Order"];
            //        fieldRow["OrderForEdit"] = fieldRow["Order"];
            //    }
            //}


            Filter filter = GetFilter(view, values, LogicCondition.And, false, false);
            int rowCount = 0;
            DataTable dataTable = FillDataTable(view, 1, 1000000, filter, null, SortDirection.Asc, out rowCount, null, null, null, null);

            DataView dataView = new DataView(dataTable);

            string whereStatement = filter.GetWhereStatementWithoutParameters();
            if (!string.IsNullOrEmpty(whereStatement))
            {
                try
                {
                    dataView.RowFilter = whereStatement.Replace(" = N'", " = '").Replace("like N'", "Like '");
                }
                catch (Exception exception)
                {
                    throw new DuradosException("Filter on fields failed. Please try different filter.", exception);
                }
            }

            foreach (DataRowView fieldRow in dataView)
            {
                fieldRow.Row["OrderForCreate"] = fieldRow.Row["Order"];
                fieldRow.Row["OrderForEdit"] = fieldRow.Row["Order"];
            }

            if (multiTenancy)
            {
                if (!dataSets.ContainsKey(filename))
                {
                    dataSets.Add(filename, null);
                }
                dataSets[filename] = dataView.Table.DataSet;
            }
            else
            {
                dataSet2 = dataView.Table.DataSet;
            }

        }
    }

    public class ViewNotFoundExeption : DuradosException
    {
        public ViewNotFoundExeption(string name)
            : base("The view '" + name + "' was not found.")
        {
        }
    }

    public class MoreThanOneViewExeption : DuradosException
    {
        public MoreThanOneViewExeption(string name)
            : base("The view '" + name + "' was found more than once.")
        {
        }
    }
}
