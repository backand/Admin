using System;
using System.Data;
using System.Data.SqlClient;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Text;

using Durados;
using Durados.DataAccess;

namespace Durados.DataAccess
{

    public abstract class DataAccessObject
    {
        public static IDbConnection GetNewConnection(SqlProduct sqlProduct)
        {
            if (sqlProduct == SqlProduct.Oracle)
                return new Oracle.ManagedDataAccess.Client.OracleConnection();
            else if (sqlProduct == SqlProduct.Postgre)
            {
                Npgsql.NpgsqlConnection connection = new Npgsql.NpgsqlConnection();
                connection.ValidateRemoteCertificateCallback += new Npgsql.ValidateRemoteCertificateCallback(connection_ValidateRemoteCertificateCallback);
                return connection;
            }
            else if (sqlProduct == SqlProduct.MySql)
                return new MySql.Data.MySqlClient.MySqlConnection();
            else
                return new SqlConnection();
        }

        public static IDbConnection GetNewConnection(SqlProduct sqlProduct, string connectionString)
        {
            if (sqlProduct == SqlProduct.Oracle)
            {
                return new Oracle.ManagedDataAccess.Client.OracleConnection(connectionString);
            }
            else if (sqlProduct == SqlProduct.Postgre)
            {
                Npgsql.NpgsqlConnection connection = new Npgsql.NpgsqlConnection(connectionString);
                connection.ValidateRemoteCertificateCallback += new Npgsql.ValidateRemoteCertificateCallback(connection_ValidateRemoteCertificateCallback);
                return connection;
            }
            else if (sqlProduct == SqlProduct.MySql)
                return new MySql.Data.MySqlClient.MySqlConnection(connectionString);
            else
                return new SqlConnection(connectionString);
        }

        

        static bool connection_ValidateRemoteCertificateCallback(System.Security.Cryptography.X509Certificates.X509Certificate cert, System.Security.Cryptography.X509Certificates.X509Chain chain, System.Net.Security.SslPolicyErrors errors)
        {
            return true;
        }

        protected virtual IDbConnection GetNewConnection(string connectionString)
        {
            return new SqlConnection(connectionString);
        }

        //protected virtual IDbConnection GetNewConnection()
        //{
        //    return new SqlConnection();
        //}

        //protected virtual IDbCommand GetNewCommand()
        //{
        //    return new SqlCommand();
        //}

        protected virtual IDbCommand GetNewCommand(string cmdText, IDbConnection connection)
        {
            return new SqlCommand(cmdText, (SqlConnection)connection);
        }

        protected virtual IDbCommand GetNewCommand(string cmdText, IDbConnection connection, SqlProduct sqlProduct)
        {
            if (sqlProduct == SqlProduct.Oracle)
            {
                return new Oracle.ManagedDataAccess.Client.OracleCommand(cmdText, (Oracle.ManagedDataAccess.Client.OracleConnection)connection);
            }
            else if (sqlProduct == SqlProduct.Postgre)
            {
                return new Npgsql.NpgsqlCommand(cmdText, (Npgsql.NpgsqlConnection)connection);
            }
            else if (sqlProduct == SqlProduct.MySql)
                return new MySql.Data.MySqlClient.MySqlCommand(cmdText, (MySql.Data.MySqlClient.MySqlConnection)connection);
            else
                return new SqlCommand(cmdText, (SqlConnection)connection);

        }

        protected virtual System.Data.IDataParameter GetNewParameter(IDbCommand command, string parameterName, object value)
        {
            return new SqlParameter(parameterName, value);
        }

        protected virtual System.Data.IDataParameter GetNewParameter(View view, string parameterName, object value)
        {
            return new SqlParameter(parameterName, value);
        }

        protected virtual IDataAdapter GetNewAdapter(IDbCommand command)
        {
            return new SqlDataAdapter((SqlCommand)command);
        }

        protected virtual int Fill(IDataAdapter adapter, DataTable table)
        {
            return ((SqlDataAdapter)adapter).Fill(table);
        }

        protected virtual string GetParameterType(IDataParameter parameter)
        {
            return ((SqlParameter)parameter).TypeName;
        }
    }

    public delegate string ExecuteNonQueryRollbackCallback(Dictionary<string, object> parameters);

    //public delegate bool ExecuteNonQueryRollbackCallback(Dictionary<string, object> parameters);

    public delegate bool ExecuteProcedureRollbackCallback(Dictionary<string, object> parameters);

    public delegate void DictionaryArgsCallback(Dictionary<string, object> args);

    public delegate void NoArgsCallbak();

    public delegate bool BoolNoArgsCallbak();

    public class SqlAccess : DataAccessObject, IDataTableAccess
    {

        public static void OpenConnection(IDbConnection connection)
        {

            try
            {
                connection.Open();
            }
            catch (Exception exception)
            {
                if (exception.Message.ToLower().Contains("timeout"))
                {
                }
                else
                {
                    throw new DuradosException("Connection failue. Please check your connection.", exception);
                }
            }
        }

        protected virtual ISqlTextBuilder GetSqlTextBuilder(View view)
        {
            return new SqlTextBuilder();
        }

        public SqlAccess()
        {

        }

        const char comma = ',';

        public virtual void CopyDatabase(string adminConnectionString, string sourceConnectionString, string targetConnectionString)
        {

        }

        public string CopyAzureDatabase(string server, string catalog, string username, string password, string source)
        {
            SqlConnectionStringBuilder csb = new SqlConnectionStringBuilder();
            csb.DataSource = server;
            csb.UserID = username;
            csb.Password = password;
            csb.IntegratedSecurity = false;
            string connectionString = csb.ConnectionString;

            return CopyAzureDatabase(connectionString, source, catalog);
        }

        public string CopySqlServerDatabase(string server, string catalog, string username, string password, string source, string path)
        {
            SqlConnectionStringBuilder csb = new SqlConnectionStringBuilder();
            csb.DataSource = server;
            csb.UserID = username;
            csb.Password = password;
            csb.IntegratedSecurity = false;
            string connectionString = csb.ConnectionString;
            path = path.TrimEnd(@"\".ToCharArray()) + @"\";
            string mdf = path + catalog + ".mdf";
            string ldf = path + catalog + ".ldf";
            string sourceMdf = path + source + ".mdf";
            string sourceLdf = path + source + ".ldf";
            //System.IO.File.Copy(sourceMdf, mdf, true);
            //System.IO.File.Copy(sourceLdf, ldf, true);
            CopySqlServerCmdShell(connectionString, sourceMdf, mdf);
            CopySqlServerCmdShell(connectionString, sourceLdf, ldf);

            return CopySqlServerDatabase(connectionString, mdf, ldf, catalog);
        }

        public void CopySqlServerCmdShell(string connectionString, string source, string destination)
        {
            string cmd = "exec master..xp_cmdShell 'Copy " + "\"" + source + "\"" + " " + "\"" + destination + "\"'";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                using (SqlCommand command = new SqlCommand(cmd, connection))
                {
                    try
                    {
                        command.ExecuteNonQuery();
                    }
                    catch (Exception exception)
                    {
                        throw exception;
                    }
                }
            }
        }

        public string RenameAzureDatabase(string server, string catalog, string username, string password, string source)
        {
            SqlConnectionStringBuilder csb = new SqlConnectionStringBuilder();
            csb.DataSource = server;
            csb.UserID = username;
            csb.Password = password;
            csb.IntegratedSecurity = false;
            string connectionString = csb.ConnectionString;

            return RenameAzureDatabase(connectionString, source, catalog);
        }

        public string CopyAzureDatabase(string connectionString, string source, string destination)
        {
            SqlConnectionStringBuilder csb = new SqlConnectionStringBuilder(connectionString);

            string sql = "CREATE DATABASE " + destination + " AS COPY OF " + source;
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                using (SqlCommand command = new SqlCommand(sql, connection))
                {
                    command.ExecuteNonQuery();
                }
            }



            csb.InitialCatalog = destination;

            return csb.ConnectionString;
        }

        public string CopySqlServerDatabase(string connectionString, string mdf, string ldf, string destination)
        {
            SqlConnectionStringBuilder csb = new SqlConnectionStringBuilder(connectionString);

            string sql = "CREATE DATABASE " + destination + " ON PRIMARY (FILENAME = '" + mdf + "'), ( FILENAME = N'" + ldf + "' ) FOR ATTACH_REBUILD_LOG "; ;

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                using (SqlCommand command = new SqlCommand(sql, connection))
                {
                    command.ExecuteNonQuery();
                }
            }



            csb.InitialCatalog = destination;

            return csb.ConnectionString;
        }

        public string RenameAzureDatabase(string connectionString, string source, string destination)
        {
            SqlConnectionStringBuilder csb = new SqlConnectionStringBuilder(connectionString);

            string sql = "ALTER DATABASE " + source + " modify name=" + destination;
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                using (SqlCommand command = new SqlCommand(sql, connection))
                {
                    command.ExecuteNonQuery();
                }
            }



            csb.InitialCatalog = destination;

            return csb.ConnectionString;
        }

        public void CreateDatabaseOwnerUser(string server, string catalog, string username, string password, bool integrated, string newUser, string newPassword)
        {
            string db_owner = "db_owner";
            CreateDatabaseUser(server, catalog, username, password, integrated, newUser, newPassword, db_owner);
        }

        public void CreateDatabaseUser(string server, string catalog, string username, string password, bool integrated, string newUser, string newPassword, string role)
        {
            SqlConnectionStringBuilder csb = new SqlConnectionStringBuilder();
            csb.DataSource = server;
            if (!integrated)
            {
                csb.UserID = username;
                csb.Password = password;
            }
            csb.IntegratedSecurity = integrated;
            string connectionString = csb.ConnectionString;

            CreateDatabaseUser(connectionString, catalog, newUser, newPassword, role);
        }

        public void CreateDatabaseUser(string connectionString, string catalog, string newUser, string newPassword, string role)
        {
            string sql = "CREATE LOGIN " + newUser + " WITH password='" + newPassword + "'";
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                using (SqlCommand command = new SqlCommand(sql, connection))
                {
                    command.ExecuteNonQuery();
                }
            }


            SqlConnectionStringBuilder csb = new SqlConnectionStringBuilder(connectionString);
            csb.InitialCatalog = catalog;

            sql = "CREATE USER " + newUser + " FROM LOGIN " + newUser;
            using (SqlConnection connection = new SqlConnection(csb.ConnectionString))
            {
                connection.Open();
                using (SqlCommand command = new SqlCommand(sql, connection))
                {
                    command.ExecuteNonQuery();

                    sql = "EXEC sp_addrolemember '" + role + "', '" + newUser + "'";
                    command.CommandText = sql;
                    command.ExecuteNonQuery();
                }
            }




        }

        public void CreateDatabaseUserWithoutLogin(string connectionString, string catalog, string newUser, string newPassword, string role)
        {
            SqlConnectionStringBuilder csb = new SqlConnectionStringBuilder(connectionString);
            csb.InitialCatalog = catalog;

            string sql = "CREATE USER " + newUser + " FROM LOGIN " + newUser;
            using (SqlConnection connection = new SqlConnection(csb.ConnectionString))
            {
                connection.Open();
                using (SqlCommand command = new SqlCommand(sql, connection))
                {
                    command.ExecuteNonQuery();

                    sql = "EXEC sp_addrolemember '" + role + "', '" + newUser + "'";
                    command.CommandText = sql;
                    command.ExecuteNonQuery();
                }
            }




        }

        public void CreateDatabase(string connectionString, string catalog)
        {
            string sql = "CREATE database " + catalog;
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                using (SqlCommand command = new SqlCommand(sql, connection))
                {
                    command.ExecuteNonQuery();
                }
            }
        }
        public virtual void DeleteDatabase(string connectionString, string catalog)
        {

            string rollbackCommand = @"ALTER DATABASE [" + catalog + "] SET  SINGLE_USER WITH ROLLBACK IMMEDIATE";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                connection.ChangeDatabase("master");
                using (SqlCommand deletecommand = new SqlCommand(rollbackCommand, connection))
                {
                    deletecommand.ExecuteNonQuery();
                }
            }
            
            string sql = "DROP DATABASE ["+catalog+"]";
            sql = string.Format(sql, catalog);

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                connection.ChangeDatabase("master");
                using (SqlCommand command = new SqlCommand(sql, connection))
                {
                    command.ExecuteNonQuery();
                }
            }
        }
        public int GetIdentity(string name, string filename, string connectionString)
        {
            try
            {
                return GetIdentity2(name, connectionString);
            }
            catch (Exception exception)
            {
                CreateIdentityTable(filename, connectionString);
                return GetIdentity2(name, connectionString);
            }
        }

        private void CreateIdentityTable(string filename, string connectionString)
        {
            RunScriptFile(filename, connectionString);
        }

        private int GetIdentity2(string name, string connectionString)
        {
            string sql = "insert into durados_Identity(Name) values ('" + name + "') ";
            sql += "SELECT IDENT_CURRENT('durados_Identity') AS ID";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                using (SqlCommand command = new SqlCommand(sql, connection))
                {
                    connection.Open();
                    object scalar = command.ExecuteScalar();

                    if (scalar == null || scalar == DBNull.Value)
                    {
                        throw new DuradosException();
                    }

                    return Convert.ToInt32(scalar);
                }
            }
        }

        public void RunScriptFile(string filename, string connectionString)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                using (SqlCommand command = new SqlCommand())
                {
                    connection.Open();
                    command.Connection = connection;
                    RunScriptFile(filename, command);
                }
            }
        }

        public void RunScriptText(string script, SqlCommand command)
        {
            string[] scriptArray = null;

            script = script.Replace("__DB_NAME__", command.Connection.Database);
            scriptArray = script.Split(new string[1] { "\nGO\r" }, StringSplitOptions.RemoveEmptyEntries);

            foreach (string scriptSection in scriptArray)
            {
                command.CommandText = scriptSection;
                command.ExecuteNonQuery();
            }
        }

        public void RunScriptFile(string filename, SqlCommand command)
        {
            //System.IO.FileInfo file = new System.IO.FileInfo(filename);
            //string scripts = file.OpenText().ReadToEnd();
            if (!System.IO.File.Exists(filename))
                return;
            System.IO.StreamReader reader = new System.IO.StreamReader(filename, Encoding.Default);
            string scripts = reader.ReadToEnd();

            RunScriptText(scripts, command);
        }

        //public void RunScriptFileSmo(string filename, string connectionString)
        //{
        //    Package package = app.LoadPackage("c:\\ExamplePackage.dtsx", null);
        //    package.ImportConfigurationFile("c:\\ExamplePackage.dtsConfig");
        //    Variables vars = package.Variables;
        //    vars["MyVariable"].Value = "value from c#";

        //    DTSExecResult result = package.Execute();

        //}


        //public void RunScriptFileSmo(string filename, string connectionString)
        //{
        //    SqlConnection sqlConnection = new SqlConnection(connectionString);

        //    Microsoft.SqlServer.Management.Common.ServerConnection svrConnection = new Microsoft.SqlServer.Management.Common.ServerConnection();

        //    Microsoft.SqlServer.Management.Smo.Server server = new Microsoft.SqlServer.Management.Smo.Server(svrConnection);

        //    System.IO.FileInfo file = new System.IO.FileInfo(filename);
        //    string scripts = file.OpenText().ReadToEnd();
        //    server.ConnectionContext.ExecuteNonQuery(scripts);

        //}



        public DataRow LoadNewRow(View view, string[] pk)
        {
            int rowCount = 0;
            DataTable table = FillDataTable(view, 1, 1, GetFilter(view, pk), null, SortDirection.Asc, out rowCount, null, null, null, null);

            if (table.Rows.Count == 1)
            {
                return table.Rows[0];
            }
            else
            {
                return table.Rows.Find(view.GetPkValue(pk));
            }
        }

        public virtual string CreateNewRow(View view, Dictionary<string, object> values, string insertAbovePK, BeforeCreateEventHandler beforeCreateCallback, BeforeCreateInDatabaseEventHandler beforeCreateInDatabaseEventHandler, AfterCreateEventHandler afterCreateBeforeCommitCallback, AfterCreateEventHandler afterCreateAfterCommitCallback)
        {
            int? autoIdentity = Create(view, values, insertAbovePK, beforeCreateCallback, beforeCreateInDatabaseEventHandler, afterCreateBeforeCommitCallback, afterCreateAfterCommitCallback);

            string pk = string.Empty;

            if (view.IsAutoIdentity)
            {
                if (autoIdentity == null)
                    throw new ApplicationException("Auto Identity is Null");

                pk = autoIdentity.Value.ToString();
            }
            else
            {
                //for (int i = 0; i < view.DataTable.PrimaryKey.Length; i++)
                //{
                //    DataColumn column = view.DataTable.PrimaryKey[i];
                //    pk += values[column.ColumnName].ToString() + comma;
                //}
                //pk = pk.TrimEnd(comma);
                foreach (Field field in view.PrimaryKeyFileds)
                {
                    pk += values[field.Name] + (view.PrimaryKeyFileds.Last() == field ? "" : ",");
                }
            }

            return pk;
        }

        public virtual DataRow GetNewRow(View view, Dictionary<string, object> values, string insertAbovePK, BeforeCreateEventHandler beforeCreateCallback, BeforeCreateInDatabaseEventHandler beforeCreateInDatabaseEventHandler, AfterCreateEventHandler afterCreateBeforeCommitCallback, AfterCreateEventHandler afterCreateAfterCommitCallback)
        {
            string pk = CreateNewRow(view, values, insertAbovePK, beforeCreateCallback, beforeCreateInDatabaseEventHandler, afterCreateBeforeCommitCallback, afterCreateAfterCommitCallback);

            DataRow row = LoadNewRow(view, pk.Split(comma));
            return row;
        }

        public virtual DataView FillPage(View view, int page, int pageSize, Dictionary<string, object> values, bool? search, bool? useLike, Dictionary<string, SortDirection> sortColumns, out int rowCount, BeforeSelectEventHandler beforeSelectCallback, AfterSelectEventHandler afterSelectCallback)
        {
            Filter filter;
            if (search.HasValue && search.Value)
            {
                filter = GetFilter(view, values, LogicCondition.Or, true, useLike);
            }
            else
            {
                filter = GetFilter(view, values, LogicCondition.And, view.Database.InsideTextSearch, useLike);
            }
            // return FillPage(view, page, pageSize, filter, search, useLike, sortColumns, out rowCount, beforeSelectCallback, afterSelectCallback);
            return FillPage(view, page, pageSize, filter, search, useLike, sortColumns, out rowCount, beforeSelectCallback, afterSelectCallback);

        }

        public virtual DataView FillPage(View view, int page, int pageSize, Filter filter, bool? search, bool? useLike, Dictionary<string, SortDirection> sortColumns, out int rowCount, BeforeSelectEventHandler beforeSelectCallback, AfterSelectEventHandler afterSelectCallback)
        {
            DataTable table = null;
            ISqlTextBuilder sqlTextBuilder = GetSqlTextBuilder(view);

            //Filter filter;
            //if (search.HasValue && search.Value)
            //{
            //    filter = GetFilter(view, values, LogicCondition.Or, true, useLike);
            //}
            //else
            //{
            //    filter = GetFilter(view, values, LogicCondition.And, view.Database.InsideTextSearch, useLike);
            //}

            string sortColumn = null;
            SortDirection direction = SortDirection.Asc;

            if (sortColumns != null && sortColumns.Count == 1 && !view.Fields.ContainsKey(sortColumns.Keys.ToList()[0]))
            {
                view.Database.Logger.Log(view.Name, "DataAccess", "FillPage", "The Sort column: " + sortColumns.Keys.ToList()[0] + " doesn't exist in the view", "", 3, "The column name probably changed or typo", DateTime.Now);

            }

            if (sortColumns != null && sortColumns.Count == 1 && view.Fields.ContainsKey(sortColumns.Keys.ToList()[0]))
            {



                //throw new ApplicationException("The Sort column: " + sortColumn + " doesn't exist in the view");
                sortColumn = sortColumns.Keys.ToList()[0];
                direction = sortColumns[sortColumn];

                Field field = view.Fields[sortColumn];

                if (field is ColumnField)
                {
                    table = FillDataTable(view, page, pageSize, filter, sortColumn, direction, out rowCount, null, null, beforeSelectCallback, afterSelectCallback);
                }
                else if (field is ParentField)
                {
                    ParentField parentField = (ParentField)field;
                    sortColumn = parentField.GetSortByParent();
                    string join = GetJoin(parentField);
                    table = FillDataTable(view, page, pageSize, filter, sortColumn, direction, out rowCount, parentField, join, beforeSelectCallback, afterSelectCallback);
                }
                else
                {
                    throw new DuradosException("Cannot sort according to collection field");
                }
            }
            else if (sortColumns != null && sortColumns.Count > 1)
            {
                string defaultSortColumn = GetOrderBy(view, sortColumns);

                //string selectStatement = "SELECT * FROM (" + GetSelect(view, pageSize) + " ROW_NUMBER() OVER(ORDER BY {4}) as RowNum FROM [{0}] with(nolock) {5} " + filter.WhereStatement + " )  as [{0}] WHERE RowNum BETWEEN ({2} - 1) * {3} + 1 AND ({2} * {3}) ";
                string selectStatement = "SELECT * FROM (" + GetSelect(view, pageSize) + sqlTextBuilder.GetRowNumber("{4}") + " FROM " + sqlTextBuilder.EscapeDbObject("{0}") + sqlTextBuilder.WithNolock + " {5} " + filter.WhereStatement + sqlTextBuilder.GetPageOrder(defaultSortColumn) + " ) " + sqlTextBuilder.DbAs + sqlTextBuilder.EscapeDbObject("{0}") + " WHERE 1=1 " + sqlTextBuilder.GetPage(page, pageSize);//RowNum BETWEEN ({2} - 1) * {3} + 1 AND ({2} * {3}) ";

                selectStatement = string.Format(selectStatement, new object[6] { view.DataTable.TableName, sortColumn, page, pageSize, defaultSortColumn, GetOrderByJoin(view, sortColumns) });

                table = FillDataTable(view, page, pageSize, filter, sortColumn, direction, out rowCount, null, null, selectStatement, beforeSelectCallback, afterSelectCallback);

            }
            else
            {

                Dictionary<string, SortDirection> defaultSortColumns = view.GetDefaultSortColumns();

                if (defaultSortColumns == null || defaultSortColumns.Count == 0)
                {
                    table = FillDataTable(view, page, pageSize, filter, null, direction, out rowCount, null, null, beforeSelectCallback, afterSelectCallback);

                }
                else
                {
                    string defaultSortColumn = GetOrderBy(view, defaultSortColumns);

                    //string selectStatement = "SELECT * FROM (" + GetSelect(view, pageSize) + " ROW_NUMBER() OVER(ORDER BY {4}) as RowNum FROM [{0}] with(nolock) {5} " + filter.WhereStatement + " )  as [{0}] WHERE RowNum BETWEEN ({2} - 1) * {3} + 1 AND ({2} * {3}) ";
                    string selectStatement = "SELECT * FROM (" + GetSelect(view, pageSize) + sqlTextBuilder.GetRowNumber("{4}") + " FROM " + sqlTextBuilder.EscapeDbObject("{0}") + sqlTextBuilder.WithNolock + " {5} " + filter.WhereStatement + sqlTextBuilder.GetPageOrder(defaultSortColumn) + " ) " +sqlTextBuilder.DbAs   + sqlTextBuilder.EscapeDbObject("{0}") + " WHERE 1=1 " + sqlTextBuilder.GetPage(page, pageSize);//RowNum BETWEEN ({2} - 1) * {3} + 1 AND ({2} * {3}) ";

                    selectStatement = string.Format(selectStatement, new object[6] { view.DataTable.TableName, sortColumn, page, pageSize, defaultSortColumn, GetOrderByJoin(view, defaultSortColumns) });

                    table = FillDataTable(view, page, pageSize, filter, sortColumn, direction, out rowCount, null, null, selectStatement, beforeSelectCallback, afterSelectCallback);
                }
            }


            //if (string.IsNullOrEmpty(sortColumn))
            //{
            //    table = FillDataTable(view, page, pageSize, filter, null, direction, out rowCount, null, null, beforeSelectCallback, afterSelectCallback);
            //}
            //else
            //{
            //    if (!view.Fields.ContainsKey(sortColumn))
            //        throw new ApplicationException("The Sort column: " + sortColumn + " doesn't exist in the view");

            //    Field field = view.Fields[sortColumn];

            //    if (field is ColumnField)
            //    {
            //        table = FillDataTable(view, page, pageSize, filter, sortColumn, direction, out rowCount, null, null, beforeSelectCallback, afterSelectCallback);
            //    }
            //    else
            //    {
            //        ParentField parentField = (ParentField)field;
            //        string parentTable = parentField.ParentView.Name;
            //        sortColumn = parentField.GetSortByParent();
            //        string join = GetJoin(parentField);
            //        table = FillDataTable(view, page, pageSize, filter, sortColumn, direction, out rowCount, parentTable, join, beforeSelectCallback, afterSelectCallback);
            //    }
            //}

            DataView dataView = new DataView(table);

            string whereStatement = filter.GetWhereStatementWithoutParameters();
            if (view.HasHiarchy())
            {

                if (string.IsNullOrEmpty(whereStatement) || whereStatement == "( 1=1)")
                    whereStatement = string.Empty;
                //else
                //    whereStatement = whereStatement.Replace(" = N'", " = '").Replace("like N'", "Like '") + " and ";
                if (!string.IsNullOrEmpty(whereStatement))
                {
                    //string parentFilter = GetParentFilter(view, page, pageSize, filter, sortColumn, direction);

                    try
                    {
                        dataView.RowFilter = whereStatement.Replace(" = N'", " = '").Replace("like N'", "Like '");
                    }
                    catch { }

                    //Distinct(dataView, view);

                    if (!string.IsNullOrEmpty(sortColumn))
                    {
                        dataView.Sort = sortColumn + " " + direction.ToString().ToUpper();
                    }
                }
            }
            else
            {
                bool isSearch = search.HasValue && search.Value;
                if (!string.IsNullOrEmpty(whereStatement) && !whereStatement.Contains(sqlTextBuilder.DbParameterPrefix) && !isSearch && whereStatement.Length < 1000 && !whereStatement.Contains(" in "))
                {
                    try
                    {
                        //Changed by br
                        dataView.RowFilter = whereStatement.Replace(" = N'", " = '").Replace("=N'", " = '").Replace("<>N'", " <> '").Replace("<> N'", " <> '").Replace("like N'", "Like '");
                        //dataView.RowFilter = whereStatement.Replace(" = N'", " = '").Replace("like N'", "Like '");
                    }
                    catch { }
                }
            }
            return dataView;
        }

        private string GetOrderBy(View view, Dictionary<string, SortDirection> sortColumns)
        {
            ISqlTextBuilder sqlTextBuilder = GetSqlTextBuilder(view);
            string sql = string.Empty;
            string template = "  " + sqlTextBuilder.EscapeDbObject("{0}") + sqlTextBuilder.DbTableColumnSeperator + sqlTextBuilder.EscapeDbObject("{1}") + " {2},";

            foreach (string sortColumn in sortColumns.Keys)
            {
                SortDirection direction = sortColumns[sortColumn];

                if (!view.Fields.ContainsKey(sortColumn))
                    throw new ApplicationException("The Sort column: " + sortColumn + " doesn't exist in the view");

                Field field = view.Fields[sortColumn];

                if (field is ColumnField)
                {
                    string _sortColumn = sortColumn;

                    if (field.IsCalculated)
                    {
                        _sortColumn = GetCalculatedFieldStatement(field, null);
                    }
                    sql += string.Format(template, view.DataTable.TableName, _sortColumn, direction.ToString());
                }
                else if (field is ParentField)
                {
                    ParentField parentField = (ParentField)field;
                    string parentTable = parentField.ParentView.DataTable.TableName;
                    sql += string.Format(template, parentTable, parentField.GetSortByParent(), direction.ToString());
                }
            }
            return sql.TrimEnd(',');
        }

        private string GetOrderByJoin(View view, Dictionary<string, SortDirection> sortColumns)
        {
            ISqlTextBuilder sqlTextBuilder = GetSqlTextBuilder(view);
            string sql = string.Empty;
            string template = " LEFT OUTER JOIN " + sqlTextBuilder.EscapeDbObject("{0}") + sqlTextBuilder.WithNolock + " ON {1} ";

            foreach (string sortColumn in sortColumns.Keys)
            {
                if (!view.Fields.ContainsKey(sortColumn))
                    throw new ApplicationException("The Sort column: " + sortColumn + " doesn't exist in the view");

                Field field = view.Fields[sortColumn];

                if (field is ParentField)
                {
                    ParentField parentField = (ParentField)field;
                    string parentTable = parentField.ParentView.DataTable.TableName;
                    string join = GetJoin(parentField);
                    sql += string.Format(template, parentTable, join);
                }
            }
            return sql;
        }


        private void Distinct(DataView dataView, View view)
        {
            List<int> dupIndices = new List<int>();
            Dictionary<string, DataRow> distinct = new Dictionary<string, DataRow>();
            for (int i = 0; i < dataView.Count; i++)
            {
                DataRow row = dataView[i].Row;
                string key = view.GetPkValue(row);
                if (distinct.ContainsKey(key))
                {
                    dupIndices.Add(i);
                }
                else
                {
                    distinct.Add(key, row);
                }
            }

            foreach (int index in dupIndices)
            {
                dataView[index].Delete();
            }
        }

        private string GetParentFilter(View view, int page, int pageSize, Filter filter, string sortColumn, SortDirection direction)
        {
            ISqlTextBuilder sqlTextBuilder = GetSqlTextBuilder(view);

            if (!string.IsNullOrEmpty(sortColumn)) sortColumn = sqlTextBuilder.EscapeDbObject(sortColumn);

            string selectStatement = GetSelectStatement(view, page, pageSize, filter, sortColumn, direction);

            string parentFilter = string.Empty;

            using (IDbConnection connection = GetNewConnection(view.ConnectionString))
            {
                using (IDbCommand command = GetNewCommand(selectStatement, connection))
                {
                    foreach (IDataParameter parameter in filter.Parameters)
                    {

                        command.Parameters.Add(GetNewParameter(command, parameter.ParameterName, parameter.Value));
                    }

                    connection.Open();
                    IDataReader reader = command.ExecuteReader();

                    while (reader.Read())
                    {
                        parentFilter += "(";
                        foreach (DataColumn dataColumn in view.DataTable.PrimaryKey)
                        {
                            parentFilter += dataColumn.ColumnName + "=" + reader.GetValue(reader.GetOrdinal(dataColumn.ColumnName)).ToString() + " and ";

                        }
                        parentFilter = parentFilter.Remove(parentFilter.Length - 5);
                        parentFilter += ")" + " or ";
                    }

                    if (parentFilter != string.Empty)
                    {
                        parentFilter = parentFilter.Remove(parentFilter.Length - 4);
                        parentFilter = " (" + parentFilter + ")";
                    }
                }
            }

            return parentFilter;
        }

        internal string GetJoin(ParentField parentField)
        {
            return GetJoin(parentField, null, null);
        }

        internal string GetJoin(ParentField parentField, string childSynonym, string parentSynonym)
        {
            ISqlTextBuilder sqlTextBuilder = GetSqlTextBuilder(parentField.View);
            string join = "";

            for (int i = 0; i < parentField.DataRelation.ParentColumns.Count(); i++)
            {
                DataTable childTable = parentField.DataRelation.ChildTable;
                DataColumn childColumn = parentField.DataRelation.ChildColumns[i];
                DataTable parentTable = parentField.DataRelation.ParentTable;
                DataColumn parentColumn = parentField.DataRelation.ParentColumns[i];

                string childTableName = childSynonym ?? childTable.TableName;
                string parentTableName = parentSynonym ?? parentTable.TableName;

                join += sqlTextBuilder.EscapeDbObject(parentTableName) + sqlTextBuilder.DbTableColumnSeperator + sqlTextBuilder.EscapeDbObject(parentColumn.ColumnName) + sqlTextBuilder.DbEquals + sqlTextBuilder.EscapeDbObject(childTableName) + sqlTextBuilder.DbTableColumnSeperator + sqlTextBuilder.EscapeDbObject(childColumn.ColumnName) + sqlTextBuilder.DbAnd;
            }
            join = join.TrimEnd((sqlTextBuilder.DbAnd).ToCharArray()) + " ";

            return join;
        }

        internal string GetJoin(ChildrenField childrenField)
        {
            ISqlTextBuilder sqlTextBuilder = GetSqlTextBuilder(childrenField.View);
            string join = "";

            for (int i = 0; i < childrenField.DataRelation.ParentColumns.Count(); i++)
            {
                DataTable childTable = childrenField.DataRelation.ChildTable;
                DataColumn childColumn = childrenField.DataRelation.ChildColumns[i];
                DataTable parentTable = childrenField.DataRelation.ParentTable;
                DataColumn parentColumn = childrenField.DataRelation.ParentColumns[i];
                join += "p." + sqlTextBuilder.EscapeDbObject(parentColumn.ColumnName) + sqlTextBuilder.DbEquals + "c." + sqlTextBuilder.EscapeDbObject(childColumn.ColumnName) + sqlTextBuilder.DbAnd;
            }
            join = join.TrimEnd((sqlTextBuilder.DbAnd).ToCharArray()) + " ";

            return join;
        }

        public virtual DataTable FillDataTable(View view, int page, int pageSize, Filter filter, string sortColumn, SortDirection direction, out int totalRowCount, ParentField parentField, string join, BeforeSelectEventHandler beforeSelectCallback, AfterSelectEventHandler afterSelectCallback)
        {
            return FillDataTable(view, page, pageSize, filter, sortColumn, null, direction, out totalRowCount, parentField, join, null, beforeSelectCallback, afterSelectCallback);
        }

        public virtual DataTable FillDataTable(View view, int page, int pageSize, Filter filter, string sortColumn, SortDirection direction, out int totalRowCount, ParentField parentField, string join, string selectStatement, BeforeSelectEventHandler beforeSelectCallback, AfterSelectEventHandler afterSelectCallback)
        {
            return FillDataTable(view, page, pageSize, filter, sortColumn, null, direction, out totalRowCount, parentField, join, selectStatement, beforeSelectCallback, afterSelectCallback);
        }

        public virtual DataTable FillDataTable(View view, int page, int pageSize, Filter filter, string sortColumn, DataSet dataSet, SortDirection direction, out int totalRowCount, ParentField parentField, string join, string selectStatement, BeforeSelectEventHandler beforeSelectCallback, AfterSelectEventHandler afterSelectCallback)
        {
            if (view.Fields.Count == 0)
                throw new DuradosException("The view " + view.Name + " has no fields.");

            ISqlTextBuilder sqlTextBuilder = GetSqlTextBuilder(view);
            SelectEventArgs selectEventArgs = new SelectEventArgs(view, filter);
            if (beforeSelectCallback != null)
                beforeSelectCallback(this, selectEventArgs);

            DataSet dataset = null;
            if (dataSet == null)
                dataset = view.DataTable.DataSet.Copy();
            else
                dataset = dataSet;

            dataset.EnforceConstraints = false;

            bool isCalculated = false;
            if (sortColumn != null && view.Fields.ContainsKey(sortColumn))
            {
                Field field = view.Fields[sortColumn];
                if (field.IsCalculated)
                {
                    sortColumn = GetCalculatedFieldStatement(field, string.Empty);
                    isCalculated = true;
                }
            }

            if (string.IsNullOrEmpty(selectStatement))
            {

                if (parentField == null)
                {
                    if (!string.IsNullOrEmpty(sortColumn) && !isCalculated) sortColumn = sqlTextBuilder.EscapeDbObject(sortColumn);

                    selectStatement = GetSelectStatement(view, page, pageSize, filter, sortColumn, direction);
                }
                else
                {
                    selectStatement = GetSortParentSelectStatement(view, page, pageSize, filter, sortColumn, direction, parentField, join);
                }
            }

            selectEventArgs.Sql = selectStatement;
            if (beforeSelectCallback != null)
                beforeSelectCallback(this, selectEventArgs);
            selectStatement = selectEventArgs.Sql;

            ////FillParentsDataTable(view, page, pageSize, filter, selectStatement);
            //FillParentsDataTable(view, page, pageSize, filter, selectStatement, dataset);

            IDbConnection connection = GetNewConnection(view.ConnectionString);
            IDbCommand command = GetNewCommand(selectStatement, connection);

            foreach (IDataParameter parameter in filter.Parameters)
            {
                command.Parameters.Add(GetNewParameter(command, parameter.ParameterName, parameter.Value));
            }

            IDataAdapter adapter = GetNewAdapter(command);


            DataTable table = dataset.Tables[view.DataTable.TableName];
            //adapter.Fill(view.DataTable);
            if (table.Rows.Count > 0)
            {
                try
                {
                    Fill(adapter, table);
                }
                catch
                {
                    table.Rows.Clear();
                    Fill(adapter, table);
                }
            }
            else
            {
                try
                {
                    view.Database.Logger.Log(view.Name, DateTime.Now.Ticks.ToString(), "before Fill", null, 5, selectStatement);
 
                    Fill(adapter, table);

                    view.Database.Logger.Log(view.Name, DateTime.Now.Ticks.ToString(), "after Fill", null, 5, table.Rows.Count.ToString() + ", " + selectStatement);
                }
                catch (System.Data.ConstraintException exception)
                {
                    string constraintName = GetViolatedConstraint(table);
                    throw new DuradosException(constraintName, exception);
                }
                catch (InvalidCastException exception)
                {
                    string message = "Database: " + connection.Database + ", select statement: " + selectStatement;
                    throw new DuradosException(message, exception);
                }
                catch(SqlException exception)
                {
                   
                    throw new DuradosException(exception.Message, exception);
                }
            }

            HashSet<string> keys = new HashSet<string>();

            foreach (DataRow row in table.Rows)
            {
                keys.Add(view.GetPkValue(row));
            }

            FillParentsDataTable(view, page, pageSize, filter, selectStatement, dataset);

            MarkRows(table, view, keys);

            if (filter.IsNosqlFilter || filter.Parameters.Count() > 0 || filter.WhereStatement.Contains("like N") || !string.IsNullOrEmpty(view.PermanentFilter) || filter.WhereStatement.ToLower().Contains(" exists ") || filter.WhereStatement.ToLower().Contains(" is null ") || filter.WhereStatement.ToLower().Contains(" is not null "))
            {
                totalRowCount = RowFilterCount(view, filter);
            }
            else
            {
                if (!view.Database.NoSysIndex)
                {

                    totalRowCount = RowCount(view);
                    if (totalRowCount == -1)
                        totalRowCount = RowFilterCount(view, filter);
                    //view.Database.NoSysIndex = true;

                }
                else
                {
                    totalRowCount = RowFilterCount(view, filter);
                }
            }

            if (afterSelectCallback != null)
                afterSelectCallback(this, new SelectEventArgs(view, filter, table));

            // Performance Diagnostics
            if (view.Database.DiagnosticsReportInProgress)
            {
                int count = table.Rows.Count;
                if (count > view.Database.DiagnosticsReport.OverLoadLimit && view.Database.Logger != null)
                {
                    view.Database.Logger.Log(view.Name, "", view.Database.DiagnosticsReport.Name, string.Empty, view.Database.DiagnosticsReport.GetStackTrace(), -count, view.Database.Logger.NowWithMilliseconds(), view.Database.DiagnosticsReport.DateTime);
                }
            }

            return table;
        }

        private void MarkRows(DataTable table, View view, HashSet<string> keys)
        {
            foreach (DataRow row in table.Rows)
            {
                if (keys.Contains(view.GetPkValue(row)) && row.RowState != DataRowState.Modified)
                    row.SetModified();
            }
        }

        //public static bool NoSysIndex = false;

        private string GetViolatedConstraint(DataTable dataTable)
        {
            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            if (dataTable.HasErrors)
            {
                foreach (DataRow row in dataTable.GetErrors())
                {
                    sb.AppendLine(row.RowError);
                }
            }

            return "In table " + dataTable.TableName + " " + sb.ToString();
        }

        public string GetScalar(ColumnField columnField, string pk)
        {
            object scalar = null;

            View view = columnField.View;
            ISqlTextBuilder sqlBuilder = GetSqlTextBuilder(columnField.View);
            string sql = "SELECT " + sqlBuilder.EscapeDbObjectStart + columnField.DataColumn.ColumnName + sqlBuilder.EscapeDbObjectEnd + " FROM " + sqlBuilder.EscapeDbObjectStart + view.DataTable.TableName + sqlBuilder.EscapeDbObjectEnd + " WHERE " + GetWhereStatement(view);

            using (IDbConnection connection = GetNewConnection(view.ConnectionString))
            {
                connection.Open();
                using (IDbCommand command = GetNewCommand(sql, connection))
                {
                    foreach (IDataParameter parameter in GetWhereParemeters(view, pk))
                    {
                        command.Parameters.Add(GetNewParameter(command, parameter.ParameterName, parameter.Value));
                    }
                    scalar = command.ExecuteScalar();

                }
            }

            if (scalar == null || scalar == DBNull.Value)
                return null;
            else
                return scalar.ToString();

        }

        public string GetFirstPK(View view)
        {
            string pkColumn = GetPrimaryKeyColumnsDelimited(view);

            ISqlTextBuilder sqlTextBuilder = GetSqlTextBuilder(view);

            string sql = sqlTextBuilder.Top("select " + pkColumn + " from " + sqlTextBuilder.EscapeDbObject(view.DataTable.TableName), 1);

            string pk = null;

            using (IDbConnection connection = GetNewConnection(view.ConnectionString))
            {
                connection.Open();

                using (IDbCommand command = GetNewCommand(sql, connection))
                {
                    using (IDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            pk = string.Empty;
                            foreach (DataColumn column in view.DataTable.PrimaryKey)
                            {
                                if (!reader.IsDBNull(reader.GetOrdinal(column.ColumnName)))
                                    pk += reader[column.ColumnName].ToString();
                                pk += ",";
                            }
                            pk = pk.TrimEnd(',');
                        }
                    }
                }

            }

            return pk;
        }

        public string GetScalar(ParentField columnField, string pk)
        {
            object scalar = null;

            View view = columnField.View;

            string[] columnsNames = columnField.GetColumnsNames();
            string sql = "SELECT " + columnsNames.DelimitedColumns() + " FROM [" + view.DataTable.TableName + "] WHERE " + GetWhereStatement(view);

            using (IDbConnection connection = GetNewConnection(view.ConnectionString))
            {
                connection.Open();
                using (IDbCommand command = GetNewCommand(sql, connection))
                {
                    foreach (IDataParameter parameter in GetWhereParemeters(view, pk))
                    {
                        command.Parameters.Add(GetNewParameter(command, parameter.ParameterName, parameter.Value));
                    }

                    if (columnsNames.Length == 1)
                        scalar = command.ExecuteScalar();
                    else
                    {
                        string val = string.Empty;

                        using (IDataReader reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                foreach (string columnName in columnsNames)
                                {
                                    if (!reader.IsDBNull(reader.GetOrdinal(columnName)))
                                    {
                                        val += reader[reader.GetOrdinal(columnName)].ToString() + ",";

                                    }
                                    else
                                    {
                                        val += ",";
                                    }
                                }
                            }

                            val = pk.TrimEnd(',');
                        }
                        scalar = val;
                    }
                }
            }

            if (scalar == null || scalar == DBNull.Value)
                return null;
            else
                return scalar.ToString();

        }

        public virtual bool IsRowAlreadyExistsByDisplayName(View view, Dictionary<string, object> values)
        {
            string displayFieldName = view.DisplayField.Name;
            if (values.ContainsKey(displayFieldName))
            {
                object value = values[displayFieldName] ?? string.Empty;
                return IsRowAlreadyExistsByDisplayName(view, value.ToString());
            }

            throw new DuradosException("Values do not contain the display field");
        }

        public virtual bool IsRowAlreadyExistsByDisplayName(View view, string value)
        {
            GetPKValueByDisplayValueStatus status = GetPKValueByDisplayValueStatus.NotFound;
            GetPKValueByDisplayValue(view, value, out status);
            return status != GetPKValueByDisplayValueStatus.NotFound;
        }

        public virtual bool? IsRowAlreadyExistsByScalars(View view, Dictionary<string, object> values)
        {
            ISqlTextBuilder sqlTextBuilder = GetSqlTextBuilder(view);
            using (IDbConnection connection = GetNewConnection(view.ConnectionString))
            {
                connection.Open();

                string sql = "SELECT TOP 1 * FROM [" + view.DataTable.TableName + "] WITH (NOLOCK) WHERE ";
                bool hasScalarColumns = false;
                IDataParameter param;

                IDbCommand command = GetNewCommand(sql, connection);

                foreach (string key in values.Keys)
                {
                    if (view.Fields.ContainsKey(key))
                    {
                        Field field = view.Fields[key];

                        string parameterName = sqlTextBuilder.DbParameterPrefix  + field.Name;

                        if (field.FieldType == FieldType.Column)
                        {

                            ColumnField col = (ColumnField)field;

                            DataColumn dataColumn = col.DataColumn;

                            //SET ANSI_NULLS OFF ??
                            //sql += field.Name + " = ISNULL (" + parameterName + ", " + field.Name + ") AND ";                           

                            sql += "(" + field.Name + " = " + parameterName + " OR (" + field.Name + " IS NULL AND " + parameterName + " IS NULL)) AND ";

                            try
                            {
                                if (dataColumn.AllowDBNull && (values[key] == null || values[key].ToString() == string.Empty))
                                {
                                    param = GetNewParameter(command, dataColumn.ColumnName, DBNull.Value);
                                }
                                else if (values[key] == null)
                                {
                                    throw new DuradosException("The value can not be NULL in column name [" + dataColumn.ColumnName + "]");
                                }
                                else
                                {
                                    param = GetNewParameter(command, dataColumn.ColumnName, Convert.ChangeType(field.ConvertFromString(values[key].ToString()), dataColumn.DataType));
                                }

                            }
                            catch (Exception exception)
                            {
                                if (values[key] == null) { values[key] = "NULL"; }
                                throw new DuradosException("The value [" + values[key].ToString() + "] is not in the correct format [" + dataColumn.DataType + "] in column name [" + dataColumn.ColumnName + "]", exception);
                            }

                            hasScalarColumns = true;
                            command.Parameters.Add(param);
                        }
                    }
                }

                if (!hasScalarColumns)
                    return null;

                sql = sql.Remove(sql.Length - 5, 5);

                command.CommandText = sql;

                command.Connection = connection;

                object scalar = command.ExecuteScalar();

                if (scalar != null && scalar != DBNull.Value)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        public virtual string GetPKValueByDisplayValue(View view, string displayValue, out GetPKValueByDisplayValueStatus status)
        {
            return GetPKValueByDisplayValue(view, displayValue, null, null, out status);
        }

        public virtual string GetPKValueByDisplayValue(View view, string displayValue, string where, IDataParameter[] parameters, out GetPKValueByDisplayValueStatus status)
        {
            ISqlTextBuilder sqlTextBuilder = GetSqlTextBuilder(view);
            using (IDbConnection connection = GetNewConnection(view.ConnectionString))
            {
                string sql = "select {0}, {1} from {2} where {3}" + (string.IsNullOrEmpty(where) ? string.Empty : " and " + where);

                string displayColumnName = string.Empty;
                string fromSratement = sqlTextBuilder.EscapeDbObject(view.DataTable.TableName) + sqlTextBuilder.WithNolock;

                if (view.DisplayField.FieldType == FieldType.Column)
                {
                    displayColumnName = sqlTextBuilder.EscapeDbObject(view.DataTable.TableName) + sqlTextBuilder.DbTableColumnSeperator + sqlTextBuilder.EscapeDbObject(view.DisplayField.Name);
                }
                else
                {
                    Field displayField = view.DisplayField;

                    string tableName = string.Empty;
                    while (displayField.FieldType == FieldType.Parent)
                    {
                        tableName = ((ParentField)displayField).ParentView.DataTable.TableName;
                        fromSratement += " INNER JOIN " + sqlTextBuilder.EscapeDbObject(tableName) + sqlTextBuilder.WithNolock + " on " + GetJoin((ParentField)displayField);
                        displayField = ((ParentField)displayField).ParentView.DisplayField;
                    }
                    displayColumnName = sqlTextBuilder.EscapeDbObject(tableName) + sqlTextBuilder.DbTableColumnSeperator + sqlTextBuilder.EscapeDbObject(displayField.Name);
                }

                sql = string.Format(sql, GetPrimaryKeyColumnsDelimited(view), displayColumnName, fromSratement, displayColumnName + " = N'" + displayValue + "'");
                //sql = string.Format(sql, GetPrimaryKeyColumnsDelimited(view), sqlTextBuilder.EscapeDbObject(displayColumnName + "]", fromSratement, GetWhereStatement(view));



                IDbCommand command = GetNewCommand(sql, connection);

                if (parameters != null)
                {
                    foreach (IDataParameter parameter in parameters)
                        command.Parameters.Add(parameter);
                }
                //foreach (SqlParameter parameter in GetWhereParemeters(view, pk))
                //{
                //    command.Parameters.AddWithValue(parameter.ParameterName, parameter.Value);
                //}

                connection.Open();

                string pk = string.Empty;
                IDataReader reader = command.ExecuteReader();

                status = GetPKValueByDisplayValueStatus.NotFound;

                if (reader.Read())
                {
                    //if (view.DisplayField is ColumnField)
                    //{
                    status = GetPKValueByDisplayValueStatus.FoundUnique;
                    foreach (DataColumn column in view.DataTable.PrimaryKey)
                    {
                        if (!reader.IsDBNull(reader.GetOrdinal(column.ColumnName)))
                        {
                            pk += reader[reader.GetOrdinal(column.ColumnName)].ToString() + ",";

                        }
                        else
                        {
                            status = GetPKValueByDisplayValueStatus.NotFound;
                        }
                    }
                }

                pk = pk.TrimEnd(',');

                if (reader.Read())
                {
                    status = GetPKValueByDisplayValueStatus.FoundMoreThanOne;
                }

                connection.Close();

                return pk;
            }
        }

        public virtual string GetPKValueByDisplayValue(ParentField field, string displayValue, string fkValue, out GetPKValueByDisplayValueStatus status)
        {
            string where = GetDependencyWhereStatementFromDependencyField(field);
            IDataParameter[] parameters = GetDependencyWhereParemetersFromDependencyField(field, fkValue).ToArray();

            return GetPKValueByDisplayValue(field.View, displayValue, where, parameters, out status);
        }

        public virtual string GetPKValueByDisplayValue(ParentField field, string displayValue, Dictionary<ParentField, string> dependencyDisplayValues, out GetPKValueByDisplayValueStatus status)
        {
            if (field.InsideTriggerField == null)
            {
                return GetPKValueByDisplayValue(field.ParentView, displayValue, out status);
            }
            else
            {
                var dependencyTriggerFieldReverse = dependencyDisplayValues.Keys.Reverse();
                string dependencyValue = null;
                ParentField dependencyTriggerFieldLast = null;

                foreach (ParentField dependencyTriggerField in dependencyTriggerFieldReverse)
                {
                    dependencyTriggerFieldLast = dependencyTriggerField;
                    if (dependencyTriggerField == dependencyTriggerFieldReverse.First())
                    {
                        dependencyValue = GetPKValueByDisplayValue(dependencyTriggerField.ParentView, dependencyDisplayValues[dependencyTriggerField], out status);
                        if (status != GetPKValueByDisplayValueStatus.FoundUnique)
                        {
                            return "The field [" + field.DisplayName + "] has a dependency field [" + dependencyTriggerFieldLast.DisplayName + "] with a value [" + dependencyDisplayValues[dependencyTriggerFieldLast] + "] that its primary key was not found.";
                        }
                    }
                    else
                    {
                        dependencyValue = GetPKValueByDisplayValue(dependencyTriggerField.DependencyField, dependencyDisplayValues[dependencyTriggerField], dependencyValue, out status);
                        if (status != GetPKValueByDisplayValueStatus.FoundUnique)
                        {
                            return "The field [" + field.DisplayName + "] has a dependency field [" + dependencyTriggerFieldLast.DisplayName + "] with a value [" + dependencyDisplayValues[dependencyTriggerFieldLast] + "] that its primary key was not found.";
                        }
                    }
                }

                if (dependencyValue == null)
                {
                    status = GetPKValueByDisplayValueStatus.NotFound;
                    return "The field [" + field.DisplayName + "] has a dependency field [" + dependencyTriggerFieldLast.DisplayName + "] with a value [" + dependencyDisplayValues[dependencyTriggerFieldLast] + "] that its primary key was not found.";
                }
                //throw new DuradosException("The field " + field.DisplayName + " has a dependency field " + dependencyTriggerFieldLast.DisplayName + " with a value " + dependencyDisplayValues[dependencyTriggerFieldLast] + " that its primary key was not found.");

                return GetPKValueByDisplayValue(field.DependencyField, displayValue, dependencyValue, out status);

            }
        }

        public virtual string GetDisplayValue(ChildrenField childrenField, string pk)
        {
            Durados.ParentField parentField = null;
            Durados.ParentField fkField = null;
            View displayView = childrenField.GetOtherParentView(out parentField, out fkField);
            View childrenView = childrenField.ChildrenView;
            ISqlTextBuilder sqlTextBuilder = GetSqlTextBuilder(parentField.ParentView);
            if (string.IsNullOrEmpty(pk))
                return null;
            if (pk == Database.EmptyString)
            {
                return displayView.Database.EmptyDisplay;
            }
            Field displayField = null;
            using (IDbConnection connection = GetNewConnection(displayView.ConnectionString))
            {
                string sql = "select {0}, {1} from {2} where {3} order by {0}";

                string displayColumnName = string.Empty;
                string fromSratement = sqlTextBuilder.EscapeDbObject(displayView.DataTable.TableName) + sqlTextBuilder.WithNolock + " INNER JOIN " + sqlTextBuilder.EscapeDbObject(childrenView.DataTable.TableName) + sqlTextBuilder.WithNolock + " on " + GetJoin(parentField);

                if (displayView.DisplayField.FieldType == FieldType.Column)
                {
                    //displayColumnName = view.DisplayField.Name;
                    if (displayView.DisplayField.IsCalculated)
                    {
                        displayColumnName = GetCalculatedFieldStatement(displayView.DisplayField, displayView.DisplayField.Name);
                    }
                    else
                    {
                        displayColumnName = sqlTextBuilder.EscapeDbObject(displayView.DataTable.TableName) + sqlTextBuilder.DbTableColumnSeperator + sqlTextBuilder.EscapeDbObject(displayView.DisplayField.Name);
                        //displayColumnName = "[" + view.DataTable.TableName + "].[" + view.DisplayField.Name + "]";
                    }
                }
                else
                {
                    displayField = displayView.DisplayField;

                    string tableName = string.Empty;
                    while (displayField.FieldType == FieldType.Parent)
                    {
                        tableName = ((ParentField)displayField).ParentView.DataTable.TableName;

                        fromSratement += " INNER JOIN " + sqlTextBuilder.EscapeDbObject(tableName) + sqlTextBuilder.WithNolock + " on " + GetJoin((ParentField)displayField);
                        displayField = ((ParentField)displayField).ParentView.DisplayField;
                    }
                    //displayColumnName = ((ParentField)view.DisplayField).DataRelation.ChildColumns[0].ColumnName;
                    //displayColumnName = displayField.Name;
                    if (displayField.IsCalculated)
                    {
                        displayColumnName = GetCalculatedFieldStatement(displayField, displayField.Name);
                    }
                    else
                    {
                        //displayColumnName = "[" + tableName + "].[" + displayField.Name + "]";
                        displayColumnName = sqlTextBuilder.EscapeDbObject(tableName) + sqlTextBuilder.DbTableColumnSeperator + sqlTextBuilder.EscapeDbObject(displayField.Name);
                    }
                }

                sql = string.Format(sql, GetPrimaryKeyColumnsDelimited(displayView), displayColumnName, fromSratement, GetWhereStatement(fkField));



                IDbCommand command = GetNewCommand(sql, connection);

                foreach (IDataParameter parameter in GetWhereParemeters(fkField, pk))
                {
                    command.Parameters.Add(GetNewParameter(command, parameter.ParameterName, parameter.Value));
                }

                connection.Open();

                string display = "";
                IDataReader reader = command.ExecuteReader();

                if (displayField == null)
                {
                    displayField = displayView.DisplayField;
                }

                while (reader.Read())
                {
                    //if (view.DisplayField is ColumnField)
                    //{
                    if (!reader.IsDBNull(reader.GetOrdinal(displayField.Name)))
                    {
                        display += ((ColumnField)displayField).ConvertValueToString(reader[reader.GetOrdinal(displayField.Name)].ToString()) + ", ";
                    }
                }
                display = display.TrimEnd().TrimEnd(',');

                connection.Close();


                return display;
            }
        }

        public virtual string GetDisplayValue(ParentField parentField, string pk)
        {
            ISqlTextBuilder sqlTextBuilder = GetSqlTextBuilder(parentField.ParentView);
            if (string.IsNullOrEmpty(pk))
                return null;
            View view = parentField.ParentView;
            if (pk == Database.EmptyString)
            {
                return view.Database.EmptyDisplay;
            }
            Field displayField = null;
            using (IDbConnection connection = GetNewConnection(view.ConnectionString))
            {
                string sql = "select {0}, {1} from {2} where {3}";

                string displayColumnName = string.Empty;
                string fromSratement = sqlTextBuilder.EscapeDbObject(view.DataTable.TableName) + sqlTextBuilder.WithNolock;

                if (view.DisplayField.FieldType == FieldType.Column)
                {
                    //displayColumnName = view.DisplayField.Name;
                    if (view.DisplayField.IsCalculated)
                    {
                        displayColumnName = GetCalculatedFieldStatement(view.DisplayField, view.DisplayField.Name);
                    }
                    else
                    {
                        displayColumnName = sqlTextBuilder.EscapeDbObject(view.DataTable.TableName) + sqlTextBuilder.DbTableColumnSeperator + sqlTextBuilder.EscapeDbObject(view.DisplayField.Name);
                        //displayColumnName = "[" + view.DataTable.TableName + "].[" + view.DisplayField.Name + "]";
                    }
                }
                else
                {
                    displayField = view.DisplayField;

                    string tableName = string.Empty;
                    while (displayField.FieldType == FieldType.Parent)
                    {
                        tableName = ((ParentField)displayField).ParentView.DataTable.TableName;

                        fromSratement += " INNER JOIN " + sqlTextBuilder.EscapeDbObject(tableName) + sqlTextBuilder.WithNolock + " on " + GetJoin((ParentField)displayField);
                        displayField = ((ParentField)displayField).ParentView.DisplayField;
                    }
                    //displayColumnName = ((ParentField)view.DisplayField).DataRelation.ChildColumns[0].ColumnName;
                    //displayColumnName = displayField.Name;
                    if (displayField.IsCalculated)
                    {
                        displayColumnName = GetCalculatedFieldStatement(displayField, displayField.Name);
                    }
                    else
                    {
                        //displayColumnName = "[" + tableName + "].[" + displayField.Name + "]";
                        displayColumnName = sqlTextBuilder.EscapeDbObject(tableName) + sqlTextBuilder.DbTableColumnSeperator + sqlTextBuilder.EscapeDbObject(displayField.Name);
                    }
                }

                sql = string.Format(sql, GetPrimaryKeyColumnsDelimited(view), displayColumnName, fromSratement, GetWhereStatement(view));



                IDbCommand command = GetNewCommand(sql, connection);

                foreach (IDataParameter parameter in GetWhereParemeters(view, pk))
                {
                    command.Parameters.Add(GetNewParameter(command, parameter.ParameterName, parameter.Value));
                }

                connection.Open();

                string display = "";
                IDataReader reader = command.ExecuteReader();

                if (displayField == null)
                {
                    displayField = view.DisplayField;
                }

                if (reader.Read())
                {
                    //if (view.DisplayField is ColumnField)
                    //{
                    if (!reader.IsDBNull(reader.GetOrdinal(displayField.Name)))
                    {
                        display = reader[reader.GetOrdinal(displayField.Name)].ToString();
                    }
                }

                connection.Close();


                display = ((ColumnField)displayField).ConvertValueToString(display);
                return display;
            }
        }

        public virtual string GetUniqFeildsDisplayNames(View view)
        {
            ISqlTextBuilder sqlTextBuilder = GetSqlTextBuilder(view);
            string names = "";
            foreach (Field field in view.Fields.Values.Where(f => f.IsUnique == true))
            {
                names += sqlTextBuilder.EscapeDbObject(field.DisplayName);
            }

            return names;

        }

        public virtual string GetPKsByUniqFieldsDisplayNames(View view, DataRow row, out GetPKValueByDisplayValueStatus status)
        {
            ISqlTextBuilder sqlTextBuilder = GetSqlTextBuilder(view);
            using (IDbConnection connection = GetNewConnection(view.ConnectionString))
            {
                string sql = "select {0} from {1} where 1=1 {2}";

                string fromSratement = sqlTextBuilder.EscapeDbObject(view.DataTable.TableName) + sqlTextBuilder.WithNolock;

                //Field displayField = view.DisplayField;
                //while (displayField.FieldType == FieldType.Parent)
                //{
                //    fromSratement += " INNER JOIN " + ((ParentField)displayField).ParentView.DataTable.TableName + " WITH (NOLOCK) on " + GetJoin((ParentField)displayField);
                //    displayField = ((ParentField)displayField).ParentView.DisplayField;
                //}

                string whereStatement = string.Empty;
                foreach (Field field in view.Fields.Values.Where(f => f.IsUnique == true))
                {
                    string colName = ((ColumnField)field).DataColumn.ColumnName;

                    object val = row[((ColumnField)field).DisplayName];

                    if (field is ColumnField && val != null)
                    {
                        whereStatement += sqlTextBuilder.DbAnd + colName + "=N'" + val.ToString() + "' ";
                    }
                    //else if (field is ParentField)
                    //{
                    //    foreach (DataColumn dataColumn in ((ParentField)field).DataRelation.ChildColumns)
                    //    {
                    //        delimitedColumns += "@" + dataColumn.ColumnName + comma;
                    //    }
                    //}
                }

                whereStatement = whereStatement.TrimEnd(comma);

                status = GetPKValueByDisplayValueStatus.NotFound;

                if (whereStatement == string.Empty)
                {
                    return string.Empty;
                }

                sql = string.Format(sql, GetPrimaryKeyColumnsDelimited(view), fromSratement, whereStatement);

                IDbCommand command = GetNewCommand(sql, connection);

                connection.Open();

                string pk = string.Empty;
                IDataReader reader = command.ExecuteReader();

                if (reader.Read())
                {
                    //if (view.DisplayField is ColumnField)
                    //{
                    status = GetPKValueByDisplayValueStatus.FoundUnique;
                    foreach (DataColumn column in view.DataTable.PrimaryKey)
                    {
                        if (!reader.IsDBNull(reader.GetOrdinal(column.ColumnName)))
                        {
                            pk += reader[reader.GetOrdinal(column.ColumnName)].ToString() + ",";

                        }
                        else
                        {
                            status = GetPKValueByDisplayValueStatus.NotFound;
                        }
                    }
                }

                pk = pk.TrimEnd(',');

                if (reader.Read())
                {
                    status = GetPKValueByDisplayValueStatus.FoundMoreThanOne;
                }

                connection.Close();

                return pk;
            }
        }

        public virtual Dictionary<string, string> GetSelectOptions(Field field, string match, bool startWith, int? top, Filter filter)
        {
            if (field.FieldType == FieldType.Parent)
            {
                return GetSelectOptions((ParentField)field, match, startWith, top, filter);
            }
            else if (field.FieldType == FieldType.Column)
            {
                return GetSelectOptions((ColumnField)field, match, startWith, top);
            }
            else
            {
                throw new NotImplementedException();
            }
        }

        public virtual Dictionary<string, string> GetSelectOptions(ParentField parentField, string match, bool startWith, int? top, Filter filter)
        {
            return GetSelectOptions(parentField, false, null, GetMatchingFilter(parentField, match, startWith), top, null);
        }

        public virtual Dictionary<string, string> GetSelectOptions(ColumnField columnField, string match, bool startWith, int? top)
        {
            return GetSelectOptions(columnField, GetMatchingFilter(columnField, match, startWith), top);
        }

        protected virtual string GetMatchingFilter(ParentField parentField, string match, bool startWith)
        {
            DataColumn dataColumn = parentField.GetDisplayDataColumn(true);
            if (parentField.ParentView.DisplayField.IsCalculated)
            {
                return GetCalculatedMatchingFilter(parentField.ParentView.DisplayField, match, startWith);
            }
            else
            {
                return GetMatchingFilter(dataColumn.ColumnName, dataColumn.Table.TableName, match, startWith, GetSqlTextBuilder(parentField.View));
            }
        }

        protected virtual string GetMatchingFilter(ColumnField columnField, string match, bool startWith)
        {
            if (string.IsNullOrEmpty(columnField.AutocompleteSql))
            {
                string columnName = string.IsNullOrEmpty(columnField.AutocompleteColumn) ? columnField.DataColumn.ColumnName : columnField.AutocompleteColumn;
                string tableName = string.IsNullOrEmpty(columnField.AutocompleteTable) ? columnField.DataColumn.Table.TableName : columnField.AutocompleteTable;
                return GetMatchingFilter(columnName, tableName, match, startWith, GetSqlTextBuilder(columnField.View));
            }
            else
            {
                return match;
            }
        }

        protected virtual string GetCalculatedMatchingFilter(Field field, string match, bool startWith)
        {
            string like = " like ";
            if (startWith)
                like += "'";
            else
                like += "'%";
            like += match.Replace("'", "''") + "%' ";

            return " " + GetCalculatedFieldStatement(field) + like;
        }

        protected virtual string GetMatchingFilter(string columnName, string tableName, string match, bool startWith, ISqlTextBuilder sqlTextBuilder)
        {
            string like = " like " +sqlTextBuilder.NLS;
            if (startWith)
                like += "'";
            else
                like += "'%";
            like += match.Replace("'", "''") + "%' ";

            return sqlTextBuilder.EscapeDbObject(tableName) + sqlTextBuilder.DbTableColumnSeperator + sqlTextBuilder.EscapeDbObject(columnName) + like;
        }

        //public virtual Dictionary<string, string> GetSelectOptions(ParentField parentField)
        //{
        //    return GetSelectOptions(parentField, null, null, null);
        //}

        //changed by br 2
        public virtual Dictionary<string, string> GetSelectOptions(ParentField parentField, bool forFilter, bool useUniqueName, int? top, Filter filter)
        {
            return GetSelectOptions(parentField, forFilter, null, null, top, useUniqueName, filter);
        }

        //changed by br 2
        public virtual Dictionary<string, string> GetSelectOptions(ParentField parentField, string fk, int? top, Filter filter, bool forFilter)
        {
            return GetSelectOptions(parentField, false, fk, null, top, filter);
        }

        public virtual Dictionary<string, string> GetSelectOptions(ColumnField columnField, string filter, int? top)
        {
            View view = columnField.View;
            string connectionString = string.IsNullOrEmpty(columnField.AutocompleteConnectionString) ? view.ConnectionString : columnField.AutocompleteConnectionString;
            ISqlTextBuilder sqlTextBuilder = GetSqlTextBuilder(view);
            using (IDbConnection connection = GetNewConnection(connectionString))
            {
                string sql;

                string columnName = string.IsNullOrEmpty(columnField.AutocompleteColumn) ? columnField.DataColumn.ColumnName : columnField.AutocompleteColumn;
                string tableName = string.IsNullOrEmpty(columnField.AutocompleteTable) ? columnField.DataColumn.Table.TableName : columnField.AutocompleteTable;

                if (string.IsNullOrEmpty(columnField.AutocompleteSql))
                {
                    sql = "select distinct " + "  {0} from {1} ";

                    sql = string.Format(sql, columnName, tableName);
                    sql += " where " + filter;
                    sql += " order by " + columnName;

                    if (top.HasValue)
                    {
                        sql = sqlTextBuilder.Top(sql, top.Value);
                    }
                }
                else
                {
                    sql = columnField.AutocompleteSql.Replace("[top]", top.ToString()).Replace("[filter]", filter);
                }

                Dictionary<string, string> selectOptions = new Dictionary<string, string>();
                try
                {
                    IDbCommand command = GetNewCommand(sql, connection);
                    connection.Open();

                    IDataReader reader = command.ExecuteReader();

                    while (reader.Read())
                    {
                        string display = string.Empty;
                        if (!reader.IsDBNull(reader.GetOrdinal(columnName)))
                        {
                            display = reader[reader.GetOrdinal(columnName)].ToString();
                        }
                        selectOptions.Add(display, display);
                    }
                }
                catch
                {
                    selectOptions.Add("no adsl", "no adsl");
                }
                finally
                {
                    connection.Close();
                }

                if (columnField.View.Database.DiagnosticsReportInProgress)
                {
                    if (selectOptions.Count > columnField.View.Database.DiagnosticsReport.OverLoadLimit && columnField.View.Database.Logger != null)
                    {
                        columnField.View.Database.Logger.Log(columnField.View.Name, columnField.Name, columnField.View.Database.DiagnosticsReport.Name, string.Empty, columnField.View.Database.DiagnosticsReport.GetStackTrace(), -selectOptions.Count, columnField.View.Database.Logger.NowWithMilliseconds(), columnField.View.Database.DiagnosticsReport.DateTime);
                    }
                }

                return selectOptions;
            }
        }

        public virtual Dictionary<string, string> GetSelectOptions(ParentField parentField, bool forFilter, string fk, string filter, int? top, Filter filterData)
        {
            return GetSelectOptions(parentField, forFilter, fk, filter, top, false, filterData);
        }
        private string HandleSqlConnectionFilter(ParentField parentField, string sql)
        {
            string sqlCnnViewName = "durados_SqlConnection";
            string sqlCnnFieldName = "FK_durados_App_durados_SqlConnection_Parent";
            string sqlCnnSystemFieldName = "FK_durados_App_durados_SqlConnection_System_Parent";
            string sqlCnnSecurityFieldName = "FK_durados_App_durados_SqlConnection_Security_Parent";
            string userId = parentField.View.Database.GetUserID();
            if (string.IsNullOrEmpty(userId))
                return sql + " AND 1=2 ";
            Durados.View view = parentField.ParentView;

            if (view.Name == sqlCnnViewName)
            {
                string whereStatement = " AND durados_SqlConnection.id  IN (SELECT {0} FROM durados_app WITH(NOLOCK) LEFT JOIN durados_userApp WITH(NOLOCK) ON durados_app.id =durados_userApp.appId WHERE durados_app.[ToDelete]=0 AND  durados_app.creator= " + userId + " OR durados_UserApp.UserId = " + userId + ")"; ;
                if (parentField.Name == sqlCnnFieldName)
                {
                    sql += string.Format(whereStatement, "SqlConnectionId");
                }
                if (parentField.Name == sqlCnnSystemFieldName)
                {
                    sql += string.Format(whereStatement, "SystemSqlConnectionId");
                }
                if (parentField.Name == sqlCnnSecurityFieldName)
                {
                    sql += string.Format(whereStatement, "SecuritySqlConnectionId");
                }
            }
            return sql;
        }
        public virtual Dictionary<string, string> GetSelectOptions(ParentField parentField, bool forFilter, string fk, string filter, int? top, bool useUniqueName, Filter filterData)
        {
            View view = parentField.View;
            View parentView = parentField.ParentView;
            ISqlTextBuilder sqlTextBuilder = GetSqlTextBuilder(parentView);
            if (!top.HasValue)
                top = parentField.View.Database.SelectionLimit;
            using (IDbConnection connection = GetNewConnection(parentView.ConnectionString))
            {
                string sql = "select " + (forFilter ? "distinct " : "") + "  {0}, {1}{3} from {2} ";
               
                string displayColumnName = string.Empty;
                string fromSratement = sqlTextBuilder.EscapeDbObject(parentView.DataTable.TableName) + sqlTextBuilder.WithNolock;

                Field textualDisplayField;
                Field displayField = parentField.GetDisplayField();

                if (forFilter)
                {
                    if (!view.SystemView && parentView.SystemView)
                    {
                    }
                    else
                    {
                        if (parentView.DataTable.TableName.Equals(parentField.View.DataTable.TableName))
                        {
                            string synonym = parentField.View.DataTable.TableName + "_1";

                            //by br 3
                            fromSratement += " RIGHT JOIN " + sqlTextBuilder.EscapeDbObject(parentField.View.DataTable.TableName) + " as " + synonym + sqlTextBuilder.WithNolock + " on " + GetJoin(parentField, synonym, parentField.View.DataTable.TableName);
                        }
                        else
                        {
                            //by br 3
                            fromSratement += " RIGHT JOIN " + sqlTextBuilder.EscapeDbObject(parentField.View.DataTable.TableName) + sqlTextBuilder.WithNolock + " on " + GetJoin(parentField);
                        }
                    }
                }

                if (displayField.FieldType == FieldType.Column)
                {
                    //textualDisplayField = parentView.DisplayField;
                    textualDisplayField = displayField;
                    if (textualDisplayField.IsCalculated)
                    {
                        displayColumnName = textualDisplayField.GetFormula() + " as " + sqlTextBuilder.EscapeDbObject(textualDisplayField.Name);
                    }
                    else
                    {
                        displayColumnName = sqlTextBuilder.EscapeDbObject(parentView.DataTable.TableName) + sqlTextBuilder.DbTableColumnSeperator + sqlTextBuilder.EscapeDbObject(textualDisplayField.Name);
                    }
                }
                else
                {
                    textualDisplayField = displayField;

                    while (textualDisplayField.FieldType == FieldType.Parent)
                    {
                        View displayFieldParentView = ((ParentField)textualDisplayField).ParentView;
                        fromSratement += " INNER JOIN " + sqlTextBuilder.EscapeDbObject(displayFieldParentView.DataTable.TableName) + sqlTextBuilder.WithNolock + " on " + GetJoin((ParentField)textualDisplayField);
                        textualDisplayField = displayFieldParentView.DisplayField;
                    }
                    //displayColumnName = ((ParentField)parentView.DisplayField).DataRelation.ChildColumns[0].ColumnName;
                    if (textualDisplayField.IsCalculated)
                    {
                        displayColumnName = textualDisplayField.GetFormula() + " as " + sqlTextBuilder.EscapeDbObject(textualDisplayField.Name);
                    }
                    else
                    {
                        displayColumnName = sqlTextBuilder.EscapeDbObject(textualDisplayField.View.DataTable.TableName) + sqlTextBuilder.DbTableColumnSeperator + sqlTextBuilder.EscapeDbObject(textualDisplayField.Name);
                    }
                }

                sql += " where 1=1";
              
                if (forFilter && !view.SystemView && parentView.SystemView && parentField.GetDataColumns().Length == 1)
                {
                    string[] parentValues = GetDistinctParentValues(parentField);
                    if (parentValues.Length > 0)
                    {
                        ChildrenField childrenField = parentField.GetEquivalentChildrenField();
                        sql += " and ";
                        sql += sqlTextBuilder.EscapeDbObject(childrenField.View.DataTable.TableName) + sqlTextBuilder.DbTableColumnSeperator + sqlTextBuilder.EscapeDbObject(childrenField.GetColumnsNames()[0]) + " in (" + parentValues.Delimited() + ") ";

                    }
                    else
                    {
                        sql += " and 1=2 ";

                    }
                }

                if (!string.IsNullOrEmpty(fk))
                {
                    string dep = GetDependencyWhereStatement(parentField);
                    if (!string.IsNullOrEmpty(dep))
                    {
                        sql += sqlTextBuilder.DbAnd + dep;
                    }
                }

                if (!string.IsNullOrEmpty(filter))
                {
                    sql += sqlTextBuilder.DbAnd + filter;
                }

                if (!string.IsNullOrEmpty(parentView.PermanentFilter))
                {
                    sql += sqlTextBuilder.DbAnd + parentView.GetPermanentFilter() + " ";
                }

                //by br 3
                if (forFilter && filterData != null)
                {
                    sql += " and " + filterData.WhereStatement.Remove(0, 6) + " ";
                }
               
                if (parentField.ParentView.Name == "durados_SqlConnection")
                    sql = HandleSqlConnectionFilter(parentField, sql);
              
                string orderByColumn;
                string orderBy;


                if (!string.IsNullOrEmpty(parentField.SelectionSortColumn))
                {
                    //sql += " order by " + parentField.SelectionSortColumn;
                    orderByColumn = parentField.SelectionSortColumn;
                    orderBy = orderByColumn;
                }
                else if (!string.IsNullOrEmpty(parentField.ParentView.DefaultSort))
                {
                    try
                    {
                        //sql += " order by " + GetDefaultOrderBy(parentView);
                        orderByColumn = GetDefaultOrderByColumns(parentView);
                        orderBy = GetDefaultOrderBy(parentView);
                    }
                    catch
                    {
                        orderByColumn = displayColumnName;
                        orderBy = orderByColumn;
                    }
                }
                else if (parentField.ParentView.DataTable.Columns.Contains(parentField.ParentView.DisplayColumn))
                {
                    if (parentField.ParentView.Fields.ContainsKey(parentField.ParentView.DisplayColumn) && parentField.ParentView.Fields[parentField.ParentView.DisplayColumn].IsCalculated)
                    {
                        orderByColumn = parentField.ParentView.Fields[parentField.ParentView.DisplayColumn].GetFormula();
                    }
                    else
                    {
                        //sql += " order by [" + parentField.ParentView.DataTable.TableName + "].[" + parentField.ParentView.DisplayColumn + "]";
                        orderByColumn = sqlTextBuilder.EscapeDbObject(parentField.ParentView.DataTable.TableName) + sqlTextBuilder.DbTableColumnSeperator + sqlTextBuilder.EscapeDbObject(parentField.ParentView.DisplayColumn);
                    }
                    orderBy = orderByColumn;
                }
                else
                {
                    //sql += " order by " + displayColumnName;
                    orderByColumn = displayColumnName;
                    orderBy = orderByColumn;
                }

                if (orderByColumn == displayColumnName)
                {
                    orderByColumn = string.Empty;
                }
                else
                {
                    orderByColumn = ", " + orderByColumn;
                }

                sql = string.Format(sql, GetPrimaryKeyColumnsDelimited(parentView), displayColumnName, fromSratement, orderByColumn);

                BeforeDropDownOptionsEventArgs eventArgs = new BeforeDropDownOptionsEventArgs(parentField, sql);

                parentField.OnBeforeDropDownOptions(eventArgs);


                sql = eventArgs.Sql;

                sql += " order by " + orderBy;

                if (top.HasValue)
                {
                    sql = sqlTextBuilder.Top(sql, top.Value);
                }


                IDbCommand command = GetNewCommand(sql, connection);
                connection.Open();

                if (!string.IsNullOrEmpty(fk) && fk != "null" && parentField.DependencyField != null)
                {
                    foreach (IDataParameter parameter in GetDependencyWhereParemeters(parentField, fk).ToArray())
                    {
                        command.Parameters.Add(parameter);
                    }
                }

                //by br 3
                if (forFilter && filterData != null)
                {
                    foreach (IDataParameter parameter in filterData.Parameters)
                    {
                        command.Parameters.Add(parameter);
                    }
                }

                Dictionary<string, string> selectOptions = new Dictionary<string, string>();

                bool hasEmptyValue = false;

                IDataReader reader = command.ExecuteReader();

                while (reader.Read())
                {
                    string pk = string.Empty;

                    foreach (DataColumn column in parentView.DataTable.PrimaryKey)
                    {
                        pk += reader[reader.GetOrdinal(column.ColumnName)].ToString() + ",";
                    }

                    pk = pk.TrimEnd(',');

                    string display = "";
                    //if (parentView.DisplayField is ColumnField)
                    //{
                    if (!reader.IsDBNull(reader.GetOrdinal(textualDisplayField.Name)))
                    {
                        object value = reader[reader.GetOrdinal(textualDisplayField.Name)];
                        if (textualDisplayField.FieldType == FieldType.Column)
                        {
                            display = ((ColumnField)textualDisplayField).ConvertValueToString(value);
                        }
                        else
                        {
                            display = value.ToString();
                        }

                        //}
                        //else
                        //{
                        //    throw new NotImplementedException();
                        //}

                        if (forFilter && useUniqueName)
                        {
                            string old_pk = string.Empty;

                            foreach (KeyValuePair<String, String> kvp in selectOptions)
                            {
                                if (kvp.Value == display)
                                {
                                    old_pk = kvp.Key;
                                }
                            }

                            if (old_pk != string.Empty)
                            {
                                selectOptions.Remove(old_pk);
                                pk = old_pk + "," + pk;
                            }
                        }

                        selectOptions.Add(pk, display);
                    }
                    //by br 3
                    else
                    {
                        hasEmptyValue = true;
                    }
                }

                if (forFilter && hasEmptyValue)
                {
                    selectOptions.Add(Database.EmptyString, parentField.View.Database.EmptyDisplay);
                }

                connection.Close();

                if (parentField.View.Database.DiagnosticsReportInProgress)
                {
                    if (selectOptions.Count > parentField.View.Database.DiagnosticsReport.OverLoadLimit && parentField.View.Database.Logger != null)
                    {
                        parentField.View.Database.Logger.Log(parentField.View.Name, parentField.Name, parentField.View.Database.DiagnosticsReport.Name, string.Empty, parentField.View.Database.DiagnosticsReport.GetStackTrace(), -selectOptions.Count, parentField.View.Database.Logger.NowWithMilliseconds(), parentField.View.Database.DiagnosticsReport.DateTime);
                    }
                }

                return selectOptions;
            }
        }

        private string[] GetDistinctParentValues(ParentField parentField)
        {
            ISqlTextBuilder sqlTextBuilder = GetSqlTextBuilder(parentField.View);
            string columnName = parentField.GetDataColumns()[0].ColumnName;
            string tableName = parentField.View.DataTable.TableName;
            string selectStatement = "select distinct " + sqlTextBuilder.EscapeDbObject(tableName) + sqlTextBuilder.DbTableColumnSeperator + sqlTextBuilder.EscapeDbObject(columnName) + " from " + sqlTextBuilder.EscapeDbObject(tableName) + sqlTextBuilder.WithNolock;

            int total = 0;
            Filter filter = new Filter();
            filter.Parameters = new SqlParameter[0];
            filter.WhereStatement = string.Empty;
            filter.WhereStatementWithoutTablePrefix = string.Empty;

            DataTable table = FillDataTable(parentField.View, 1, 1000000, filter, null, SortDirection.Asc, out total, null, null, selectStatement, null, null);

            List<string> values = new List<string>();
            foreach (DataRow row in table.Rows)
            {
                if (!row.IsNull(columnName))
                {
                    values.Add(row[columnName].ToString());
                }
            }

            return values.ToArray();
        }

        public DataTable GetDataRows(View view, string[] pks)
        {
            ISqlTextBuilder sqlTextBuilder = GetSqlTextBuilder(view);
            string selectStatement = "select * from " + sqlTextBuilder.EscapeDbObject(view.DataTable.TableName) + sqlTextBuilder.WithNolock + " where " + GetWhereStatement(view, pks);

            int total = 0;
            Filter filter = new Filter();
            filter.Parameters = new IDataParameter[0];
            filter.WhereStatement = string.Empty;
            filter.WhereStatementWithoutTablePrefix = string.Empty;

            return FillDataTable(view, 1, 1000000, filter, null, SortDirection.Asc, out total, null, null, selectStatement, null, null);


        }

        public List<string> GetCheckListKeys(ChildrenField childrenField, string fk)
        {
            if (!childrenField.IsCheckList())
                return null;

            childrenField = (ChildrenField)childrenField.Base;
            View childrenView = childrenField.ChildrenView;
            View view = childrenField.View;
            View parentView = null;
            ParentField parentField = null;
            ParentField fkField = null;

            foreach (ParentField field in childrenView.Fields.Values.Where(f => f.FieldType == FieldType.Parent))
            {
                if (!field.ParentView.Equals(view))
                {
                    parentField = field;
                    parentView = field.ParentView;
                }
                else
                {
                    fkField = field;
                }
            }


            int rowCount = 0;

            List<string> keys = new List<string>();
            if (!string.IsNullOrEmpty(fk))
            {
                Dictionary<string, object> filter = new Dictionary<string, object>();
                filter.Add(fkField.Name, fk);

                DataView childrenDataView = childrenView.FillPage(1, 1000000, filter, false, null, out rowCount, null, null);
                foreach (System.Data.DataRowView row in childrenDataView)
                {
                    string key = parentField.GetValue(row.Row);
                    keys.Add(key);
                }
            }


            return keys;
        }

        private string GetSelectStatement(View view, bool excludeLongText)
        {
            ISqlTextBuilder sqlTextBuilder = GetSqlTextBuilder(view);

            DataTable table = view.DataTable;

            StringBuilder sb = new StringBuilder();

            foreach (DataColumn column in table.Columns)
            {
                //bool ntext = column.DataType.Equals(typeof(string)) && (column.MaxLength < 0 || column.MaxLength > 700);
                bool ntext = false;
                if (view.Fields.ContainsKey(column.ColumnName) && view.Fields[column.ColumnName].IsCalculated)
                {
                    sb.Append(GetCalculatedFieldStatement(view.Fields[column.ColumnName], null));
                    sb.Append(",");
                }
                else if(view.Fields.ContainsKey(column.ColumnName) && view.Fields[column.ColumnName].IsPoint)
                {
                    sb.Append(sqlTextBuilder.GetPointFieldStatement(table.TableName,column.ColumnName));
                    sb.Append(",");
                }
                else
                {
                    if (excludeLongText && ntext)
                    {
                        sb.Append(" '' as " + sqlTextBuilder.EscapeDbObject(column.ColumnName));
                        sb.Append(",");
                    }
                    else
                    {
                        sb.Append(sqlTextBuilder.EscapeDbObject(table.TableName));
                        sb.Append(sqlTextBuilder.DbTableColumnSeperator);
                        sb.Append(sqlTextBuilder.EscapeDbObject(column.ColumnName));
                        sb.Append(",");
                    }
                }
            }

            return sb.ToString().TrimEnd(',');
        }

        public Dictionary<string, string> GetChildren(ChildrenField field, string fk)
        {
            Dictionary<string, string> keys = new Dictionary<string, string>();

            View childrenView = field.ChildrenView;
            ISqlTextBuilder sqlTextBuilder = GetSqlTextBuilder(childrenView);
            ParentField parentField = field.GetEquivalentParentField();
            string select = "select " + GetDelimitedColumns(childrenView.GetPkColumnNames().ToList(), childrenView) + " from " + sqlTextBuilder.EscapeDbObject(childrenView.Name) + sqlTextBuilder.WithNolock + " where " + GetWhereStatement(parentField);

            using (IDbConnection connection = GetNewConnection(childrenView.ConnectionString))
            {
                connection.Open();
                using (IDbCommand command = GetNewCommand(select, connection))
                {
                    foreach (IDataParameter parameter in GetWhereParemeters(parentField, fk))
                    {
                        command.Parameters.Add(GetNewParameter(command, parameter.ParameterName, parameter.Value));
                    }
                    using (IDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            string pk = string.Empty;
                            foreach (DataColumn column in childrenView.DataTable.PrimaryKey)
                            {
                                if (!reader.IsDBNull(reader.GetOrdinal(column.ColumnName)))
                                    pk += reader[column.ColumnName].ToString();
                                pk += ",";
                            }
                            pk = pk.TrimEnd(',');
                            keys.Add(pk, pk);
                        }
                    }
                }
            }
            return keys;

        }
        

        public Dictionary<string, Dictionary<string, string>> GetChildren(DataView dataView, ChildrenField field)
        {
            ISqlTextBuilder sqlTextBuilder = GetSqlTextBuilder(field.View);
            View childrenView = field.ChildrenView;

            ParentField parentField = null;
            ParentField fkField = null;
            View parentView = field.GetOtherParentView(out parentField, out fkField);
            if (parentView == null)
                return new Dictionary<string, Dictionary<string, string>>(); ;

            string whereStatement = GetWhereStatement(field, dataView);

            string selectStatement = "select distinct " + GetSelectStatement(parentView, true) + " from " + sqlTextBuilder.EscapeDbObject(parentView.DataTable.TableName) + sqlTextBuilder.WithNolock + " INNER JOIN " + sqlTextBuilder.EscapeDbObject(childrenView.DataTable.TableName) + sqlTextBuilder.WithNolock + " on " + GetJoin(parentField) + " where " + GetWhereStatement(field, dataView);


            //SqlDataAdapter adapter = new SqlDataAdapter(selectStatement, childrenView.ConnectionString);

            //DataTable table = dataset.Tables[parentView.DataTable.TableName];

            //adapter.Fill(table);
            int total = 0;
            Filter filter = new Filter();
            filter.Parameters = new IDataParameter[0];
            filter.WhereStatement = string.Empty;
            filter.WhereStatementWithoutTablePrefix = string.Empty;

            DataTable parentTable = FillDataTable(parentView, 1, 1000000, filter, null, SortDirection.Asc, out total, null, null, selectStatement, null, null);

            Dictionary<string, Dictionary<string, string>> children = new Dictionary<string, Dictionary<string, string>>();

            selectStatement = string.Format("select " + sqlTextBuilder.EscapeDbObject("{0}") + sqlTextBuilder.DbTableColumnSeperator + "* from " + sqlTextBuilder.EscapeDbObject("{0}") + " " + sqlTextBuilder.WithNolock + " where " + whereStatement, childrenView.DataTable.TableName);


            //DataTable table = FillDataTable(childrenView, 1, 1000000, filter, null, SortDirection.Asc, out total, null, null, selectStatement, null, null);

            IDbConnection connection = GetNewConnection(childrenView.ConnectionString);
            IDbCommand command = GetNewCommand(selectStatement, connection);

            IDataAdapter adapter = GetNewAdapter(command);

            DataTable table = parentTable.DataSet.Tables[childrenView.DataTable.TableName];

            try
            {
                Fill(adapter, table);
            }
            catch (System.Data.ConstraintException exception)
            {
                string constraintName = GetViolatedConstraint(table);
                throw new DuradosException(constraintName, exception);
            }
            catch (InvalidCastException exception)
            {
                string message = "Database: " + connection.Database + ", select statement: " + selectStatement;
                throw new DuradosException(message, exception);
            }

            ParentField equivalent = field.GetEquivalentParentField();
            ParentField other = field.GetFirstNonEquivalentParentField();
            //ParentField other = (ParentField) childrenView.Fields.Values.Where(f=>f.FieldType== FieldType.Parent && f.Name != equivalent.Name).First();

            foreach (DataRow row in table.Rows)
            {
                string key = equivalent.GetValue(row);
                string key2 = other.GetValue(row);
                string val = other.ConvertToString(row);
                if (!children.ContainsKey(key))
                {
                    children.Add(key, new Dictionary<string, string>());
                }
                if (!children[key].ContainsKey(key2))
                {
                    children[key].Add(key2, val);
                }
            }

            if (childrenView.Database.DiagnosticsReportInProgress)
            {
                int count = table.Rows.Count;
                if (count > childrenView.Database.DiagnosticsReport.OverLoadLimit && childrenView.Database.Logger != null)
                {
                    childrenView.Database.Logger.Log(childrenView.Name, field.Name, childrenView.Database.DiagnosticsReport.Name, string.Empty, childrenView.Database.DiagnosticsReport.GetStackTrace(), -count, childrenView.Database.Logger.NowWithMilliseconds(), childrenView.Database.DiagnosticsReport.DateTime);
                }
            }
            return children;
        }


        private string GetDefaultOrderBy(View view)
        {
            ISqlTextBuilder sqlTextBuilder = GetSqlTextBuilder(view);
            string[] columnsAndOrder = view.GetDefaultSortColumnsAndOrder();

            string orderBy = string.Empty;
            foreach (string columnAndOrder in columnsAndOrder)
            {
                string fieldName = view.GetDefaultSortColumn(columnAndOrder);
                string order = view.GetDefaultSortColumnOrder(columnAndOrder);

                string[] columns = view.Fields[fieldName].GetColumnsNames();

                foreach (string column in columns)
                {
                    orderBy += sqlTextBuilder.EscapeDbObject(view.DataTable.TableName) + sqlTextBuilder.DbTableColumnSeperator + sqlTextBuilder.EscapeDbObject(column) + " " + order + ",";
                }


            }

            orderBy = orderBy.TrimEnd(',');

            return orderBy;
        }

        private string GetDefaultOrderByColumns(View view)
        {
            ISqlTextBuilder sqlTextBuilder = GetSqlTextBuilder(view);
            string[] columnsAndOrder = view.GetDefaultSortColumnsAndOrder();

            string orderBy = string.Empty;
            foreach (string columnAndOrder in columnsAndOrder)
            {
                string fieldName = view.GetDefaultSortColumn(columnAndOrder);

                string[] columns = view.Fields[fieldName].GetColumnsNames();

                foreach (string column in columns)
                {
                    orderBy += sqlTextBuilder.EscapeDbObject(view.DataTable.TableName) + sqlTextBuilder.DbTableColumnSeperator + sqlTextBuilder.EscapeDbObject(column) + ",";
                }


            }

            orderBy = orderBy.TrimEnd(',');

            return orderBy;
        }

        //private string GetDefaultOrderBy(View view)
        //{
        //    string[] columnsAndOrder = view.GetDefaultSortColumnsAndOrder();

        //    string orderBy = string.Empty;
        //    foreach (string columnAndOrder in columnsAndOrder)
        //    {
        //        string fieldName = view.GetDefaultSortColumn(columnAndOrder);
        //        string order = view.GetDefaultSortColumnOrder(columnAndOrder);

        //        if (view.Fields.ContainsKey(fieldName))
        //        {
        //            Field field = view.Fields[fieldName];
        //            orderBy += GetFieldOrderBy(field) + " " + order + ",";

        //        }
        //    }

        //    orderBy = orderBy.TrimEnd(',');

        //    return orderBy;
        //}

        //private string GetDefaultOrderByColumns(View view)
        //{
        //    string[] columnsAndOrder = view.GetDefaultSortColumnsAndOrder();

        //    string orderBy = string.Empty;



        //    foreach (string columnAndOrder in columnsAndOrder)
        //    {
        //        string fieldName = view.GetDefaultSortColumn(columnAndOrder);

        //        if (view.Fields.ContainsKey(fieldName))
        //        {
        //            Field field = view.Fields[fieldName];
        //            orderBy += GetFieldOrderBy(field) + ",";

        //        }

        //    }

        //    orderBy = orderBy.TrimEnd(',');

        //    return orderBy;
        //}

        private string GetFieldOrderBy(Field field)
        {
            if (field.FieldType == FieldType.Column)
                return GetColumnFieldOrderBy((ColumnField)field);
            else if (field.FieldType == FieldType.Parent)
                return GetParentFieldOrderBy((ParentField)field);
            else
                return string.Empty;
        }

        private string GetColumnFieldOrderBy(ColumnField field)
        {
            ISqlTextBuilder sqlTextBuilder = GetSqlTextBuilder(field.View);
            return sqlTextBuilder.EscapeDbObject(field.View.DataTable.TableName) + sqlTextBuilder.DbTableColumnSeperator + sqlTextBuilder.EscapeDbObject(field.GetColumnsNames()[0]);
        }

        private string GetParentFieldOrderBy(ParentField field)
        {
            ISqlTextBuilder sqlTextBuilder = GetSqlTextBuilder(field.View);
            Field displayField = field;

            while (displayField.FieldType == FieldType.Parent)
            {
                View displayFieldparentView = ((ParentField)displayField).ParentView;
                displayField = displayFieldparentView.DisplayField;
            }
            //displayColumnName = ((ParentField)parentView.DisplayField).DataRelation.ChildColumns[0].ColumnName;

            return sqlTextBuilder.EscapeDbObject(displayField.View.DataTable.TableName) + sqlTextBuilder.DbTableColumnSeperator + sqlTextBuilder.EscapeDbObject(displayField.Name);
        }

        public virtual Dictionary<string, Dictionary<string, string>> GetSelectOptionsWithGroups(ParentField parentField)
        {
            return GetSelectOptionsWithGroups(parentField, null);
        }

        public virtual Dictionary<string, Dictionary<string, string>> GetSelectOptionsWithGroups(ParentField parentField, string fk)
        {
            View view = parentField.View;
            ISqlTextBuilder sqlTextBuilder = GetSqlTextBuilder(view);
            View parentView = parentField.ParentView;
            using (IDbConnection connection = GetNewConnection(parentView.ConnectionString))
            {
                string sql = "select {0}, {1}, {3}, {4} from {2} " + sqlTextBuilder.WithNolock + " inner join {5} on ";

                string join = GetJoin(parentField.DependencyField);

                sql += join;

                string displayColumnName = string.Empty;
                string fromSratement = sqlTextBuilder.EscapeDbObject(parentView.DataTable.TableName);

                Field displayField;

                if (parentView.DisplayField.FieldType == FieldType.Column)
                {
                    displayField = parentView.DisplayField;
                    displayColumnName = sqlTextBuilder.EscapeDbObject(parentView.DataTable.TableName) + sqlTextBuilder.DbTableColumnSeperator + sqlTextBuilder.EscapeDbObject(displayField.Name);
                }
                else
                {
                    displayField = parentView.DisplayField;

                    while (displayField.FieldType == FieldType.Parent)
                    {
                        fromSratement += " INNER JOIN " + sqlTextBuilder.EscapeDbObject(((ParentField)displayField).ParentView.DataTable.TableName) + " on " + GetJoin((ParentField)displayField);
                        displayField = ((ParentField)displayField).ParentView.DisplayField;
                    }
                    //displayColumnName = ((ParentField)parentView.DisplayField).DataRelation.ChildColumns[0].ColumnName;

                    displayColumnName = sqlTextBuilder.EscapeDbObject(displayField.View.DataTable.TableName) + sqlTextBuilder.DbTableColumnSeperator + sqlTextBuilder.EscapeDbObject(displayField.Name);
                }

                string groupAsSuffix = "__group";
                sql = string.Format(sql, GetPrimaryKeyColumnsDelimited(parentView), displayColumnName, fromSratement, GetPrimaryKeyColumnsDelimited(parentField.DependencyField.ParentView, groupAsSuffix), sqlTextBuilder.EscapeDbObject(parentField.DependencyField.ParentView.DataTable.TableName) + sqlTextBuilder.DbTableColumnSeperator + sqlTextBuilder.EscapeDbObject(parentField.DependencyField.ParentView.DisplayColumn) + " as " + sqlTextBuilder.EscapeDbObject(parentField.DependencyField.ParentView.DisplayColumn) + groupAsSuffix, parentField.DependencyField.ParentView.DataTable.TableName);

                if (!string.IsNullOrEmpty(fk))
                {
                    string where = " where " + GetDependencyWhereStatement(parentField);
                    sql += where;
                }

                BeforeDropDownOptionsEventArgs eventArgs = new BeforeDropDownOptionsEventArgs(parentField, sql);

                parentField.OnBeforeDropDownOptions(eventArgs);


                IDbCommand command = GetNewCommand(eventArgs.Sql, connection);
                connection.Open();

                if (!string.IsNullOrEmpty(fk) && parentField.DependencyField != null)
                {
                    foreach (IDataParameter parameter in GetDependencyWhereParemeters(parentField, fk).ToArray())
                    {
                        command.Parameters.Add(parameter);
                    }
                }

                Dictionary<string, Dictionary<string, string>> selectOptionsWithGroups = new Dictionary<string, Dictionary<string, string>>();
                IDataReader reader = command.ExecuteReader();

                while (reader.Read())
                {
                    string pk = string.Empty;

                    foreach (DataColumn column in parentView.DataTable.PrimaryKey)
                    {
                        pk += reader[reader.GetOrdinal(column.ColumnName)].ToString() + ",";
                    }

                    pk = pk.TrimEnd(',');

                    string groupPk = string.Empty;

                    foreach (DataColumn column in parentField.DependencyField.ParentView.DataTable.PrimaryKey)
                    {
                        groupPk += reader[reader.GetOrdinal(column.ColumnName + groupAsSuffix)].ToString() + ",";
                    }

                    groupPk = groupPk.TrimEnd(',');

                    string display = "";
                    if (!reader.IsDBNull(reader.GetOrdinal(displayField.Name)))
                    {
                        display = reader[reader.GetOrdinal(displayField.Name)].ToString();
                    }

                    string groupDisplay = "";
                    if (!reader.IsDBNull(reader.GetOrdinal(parentField.DependencyField.ParentView.DisplayColumn + groupAsSuffix)))
                    {
                        groupDisplay = reader[reader.GetOrdinal(parentField.DependencyField.ParentView.DisplayColumn + groupAsSuffix)].ToString();
                    }

                    if (!selectOptionsWithGroups.ContainsKey(groupDisplay))
                        selectOptionsWithGroups.Add(groupDisplay, new Dictionary<string, string>());

                    Dictionary<string, string> selectOptions = selectOptionsWithGroups[groupDisplay];

                    selectOptions.Add(pk, display);
                }

                connection.Close();

                if (parentField.View.Database.DiagnosticsReportInProgress)
                {
                    if (selectOptionsWithGroups.Count > parentField.View.Database.DiagnosticsReport.OverLoadLimit && parentField.View.Database.Logger != null)
                    {
                        parentField.View.Database.Logger.Log(parentField.View.Name, parentField.Name, parentField.View.Database.DiagnosticsReport.Name, string.Empty, parentField.View.Database.DiagnosticsReport.GetStackTrace(), -selectOptionsWithGroups.Count, parentField.View.Database.Logger.NowWithMilliseconds(), parentField.View.Database.DiagnosticsReport.DateTime);
                    }
                }

                return selectOptionsWithGroups;
            }
        }

        private string GetSelect(Durados.View view, int pageSize)
        {
            //if (pageSize == 1)
            //{
            //    return "select [" + view.DataTable.TableName + "].*, " + GetEncryptedColumnsStatement(view);
            //}
            ISqlTextBuilder sqlTextBuilder = GetSqlTextBuilder(view);

            bool dontHide = pageSize == 1;

            string selectStatement = "SELECT ";

            HashSet<string> colorFields = view.GetColorFields();

            foreach (DataColumn column in view.DataTable.Columns)
            {
                bool hidden = false;
                bool encrypted = false;
                bool calculated = false;
                string selectCalculated = string.Empty;

                if (view.Fields.ContainsKey(column.ColumnName))
                {
                    Field field = view.Fields[column.ColumnName];

                    if (field.Excluded && !colorFields.Contains(column.ColumnName))
                        hidden = true;
                    else
                        hidden = field.FieldType == FieldType.Column && field.HideInTable && column.DataType == typeof(string) && column.MaxLength > 500 && column.AllowDBNull && !view.Name.ToLower().Contains("durados");

                    if (dontHide && !column.DataType.Equals(typeof(byte[])))
                        hidden = false;

                    encrypted = field.FieldType == FieldType.Column && ((ColumnField)field).Encrypted;

                    calculated = field.IsCalculated;
                    if (calculated)
                        selectCalculated = GetCalculatedFieldStatement(field, null);

                    if (field.IsPoint)
                    {
                        calculated = true;
                        selectCalculated = GetPointFieldStatement(field);
                    }
                }

                if (!hidden)
                {
                    if (encrypted)
                        selectStatement += string.Empty;
                    else if (calculated)
                        selectStatement += selectCalculated + ", ";
                    else
                        selectStatement += sqlTextBuilder.EscapeDbObject(view.DataTable.TableName) + sqlTextBuilder.DbTableColumnSeperator + sqlTextBuilder.EscapeDbObject(column.ColumnName) + ", ";
                }

            }

            selectStatement += GetEncryptedColumnsStatement(view);
            if (view.Name == "items")
            {
                int x = 0;
                x++;
            }

            return selectStatement.Trim().TrimEnd(',');
        }

        protected virtual string GetPointFieldStatement(Field field)
        {
            return field.Name;
        }

        public string GetCalculatedFieldStatement(Field field)
        {
            return GetCalculatedFieldStatement(field, string.Empty, null);
        }

        public string GetCalculatedFieldStatement(Field field, string fieldName)
        {
            return GetCalculatedFieldStatement(field, fieldName, null);
        }
        public string GetCalculatedFieldStatement(Field field, string fieldName, View view)
        {
            ISqlTextBuilder sqlTextBuilder = GetSqlTextBuilder(field.View);
            if (fieldName == null) fieldName = field.Name;

            if (!string.IsNullOrEmpty(field.Formula))
            {
                string select = "CAST(" + GetEscapeFieldFormula(field.Formula.Parse(field, view)) + " AS " + GetDbTypeofDataType(field.DataType) + ")";
                //string select = "CAST(" + field.Formula.Parse(field,view) + " AS " + GetDbTypeofDataType(field.DataType) + ")";

                if (fieldName.Equals(string.Empty))
                    return select;
                return select + " AS " + sqlTextBuilder.EscapeDbObject(fieldName);
            }
            return "NULL AS " + sqlTextBuilder.EscapeDbObject(fieldName) + ", ";
        }

        protected virtual string GetEscapeFieldFormula(string formula)
        {
            return formula.Replace("{", "{{").Replace("}","}}");
        }

        public virtual string GetDbTypeofDataType(DataType dataType)
        {
            switch (dataType)
            {
                case DataType.Boolean:
                    return "bit";

                case DataType.DateTime:
                    return "datetime";

                case DataType.LongText:
                    return "nvarchar(max)";

                case DataType.Numeric:
                    return "float";

                case DataType.Image:
                case DataType.Url:
                case DataType.Email:
                case DataType.Html:
                case DataType.ShortText:
                    return "nvarchar(500)";

                default:
                    throw new DuradosException("Unsupported data type " + dataType.ToString());

            }
        }

        private string GetSelectStatement(Durados.View view, int page, int pageSize, Filter filter, string sortColumn, SortDirection direction)
        {
            ISqlTextBuilder sqlTextBuilder = GetSqlTextBuilder(view);
            string selectStatement = "SELECT " + sqlTextBuilder.EscapeDbObject("{0}") + sqlTextBuilder.DbTableColumnSeperator + sqlTextBuilder.AllColumns + " FROM (" + GetSelect(view, pageSize) + sqlTextBuilder.GetRowNumber("{1}") + " FROM " + sqlTextBuilder.EscapeDbObject("{0}") + sqlTextBuilder.WithNolock + filter.WhereStatement + sqlTextBuilder.GetPageOrder("{1}") + " ) " +sqlTextBuilder.DbAs + sqlTextBuilder.EscapeDbObject("{0}") + " WHERE 1=1 " + sqlTextBuilder.GetPage(page, pageSize);
            selectStatement = GetOpenCertificatesStatement(view) + selectStatement + GetCloseCertificatesStatement(view);
            if ((!string.IsNullOrEmpty(sortColumn)) && view.Fields.ContainsKey(sortColumn.TrimStart(sqlTextBuilder.EscapeDbObjectStart.ToCharArray()).TrimEnd(sqlTextBuilder.EscapeDbObjectEnd.ToCharArray())) && view.Fields[sortColumn.TrimStart(sqlTextBuilder.EscapeDbObjectStart.ToCharArray()).TrimEnd(sqlTextBuilder.EscapeDbObjectEnd.ToCharArray())].FieldType == FieldType.Column)
            {
                ColumnField columnField = (ColumnField)view.Fields[sortColumn.TrimStart(sqlTextBuilder.EscapeDbObjectStart.ToCharArray()).TrimEnd(sqlTextBuilder.EscapeDbObjectEnd.ToCharArray())];
                if (columnField.Encrypted)
                {
                    sortColumn = " CONVERT(NVARCHAR(250), DECRYPTBYKEY(" + columnField.EncryptedName + ")) ";
                }
            }
            return string.Format(selectStatement, new object[2] { view.DataTable.TableName, string.IsNullOrEmpty(sortColumn) ? GetPrimaryKeyColumnsDelimited(view) : sortColumn + " " + direction.ToString() });
        }

        private string GetSortParentSelectStatement(Durados.View view, int page, int pageSize, Filter filter, string sortColumn, SortDirection direction, ParentField parentField, string join)
        {
            ISqlTextBuilder sqlTextBuilder = GetSqlTextBuilder(view);
            if (parentField.ParentView.DisplayField.FieldType == FieldType.Parent)
            {
                string selectStatement = "SELECT " + sqlTextBuilder.EscapeDbObject("{0}") + sqlTextBuilder.DbTableColumnSeperator + sqlTextBuilder.AllColumns + " FROM (" + GetSelect(view, pageSize) + sqlTextBuilder.GetRowNumber("{4}", "{1}", "{6}") + " FROM " + sqlTextBuilder.EscapeDbObject("{0}") + sqlTextBuilder.WithNolock + " {5} " + filter.WhereStatement + " )  as " + sqlTextBuilder.EscapeDbObject("{0}") + " WHERE 1=1 " + sqlTextBuilder.GetPage(page, pageSize);
                selectStatement = GetOpenCertificatesStatement(view) + selectStatement + GetCloseCertificatesStatement(view);
                return string.Format(selectStatement, new object[7] { view.DataTable.TableName, parentField.GetSortByParent(), page, pageSize, parentField.GetSortByParentTableName(), GetSortParentJoin(parentField), direction.ToString() });
            }
            else
            {
                bool calculated = false;
                if (parentField.ParentView.Fields.ContainsKey(sortColumn.TrimStart('[').TrimEnd(']')))
                {
                    Field field = parentField.ParentView.Fields[sortColumn.TrimStart('[').TrimEnd(']')];
                    if (field.FieldType == FieldType.Column)
                    {
                        ColumnField columnField = (ColumnField)field;
                        if (columnField.IsCalculated)
                        {
                            sortColumn = GetCalculatedFieldStatement(columnField);
                            calculated = true;
                        }
                    }
                }


                //string selectStatement = "SELECT * FROM (" + GetSelect(view, pageSize) + " ROW_NUMBER() OVER(ORDER BY " + (calculated? "{1}" : "[{4}].[{1}]") + " {6}) as RowNum FROM [{0}] with(nolock) LEFT OUTER JOIN [{4}] with(nolock) ON {5} " + filter.WhereStatement + " )  as [{0}] WHERE RowNum BETWEEN ({2} - 1) * {3} + 1 AND ({2} * {3}) ";
                string selectStatement = "SELECT " + sqlTextBuilder.AllColumns + " FROM (" + GetSelect(view, pageSize) + (calculated ? sqlTextBuilder.GetRowNumberNotEscaped("{1}", "{6}") : sqlTextBuilder.GetRowNumber("{4}", "{1}", "{6}")) + " FROM " + sqlTextBuilder.EscapeDbObject("{0}") + sqlTextBuilder.WithNolock + " LEFT OUTER JOIN " + sqlTextBuilder.EscapeDbObject("{4}") + sqlTextBuilder.WithNolock + " ON {5} " + filter.WhereStatement + " )  as " + sqlTextBuilder.EscapeDbObject("{0}") + " WHERE 1=1 " + sqlTextBuilder.GetPage(page, pageSize);
                string parentTable = parentField.ParentView.DataTable.TableName;
                selectStatement = GetOpenCertificatesStatement(view) + selectStatement + GetCloseCertificatesStatement(view);
                if ((!string.IsNullOrEmpty(sortColumn)) && view.Fields.ContainsKey(sortColumn.TrimStart(sqlTextBuilder.EscapeDbObjectStart.ToCharArray()).TrimEnd(sqlTextBuilder.EscapeDbObjectEnd.ToCharArray())) && view.Fields[sortColumn.TrimStart(sqlTextBuilder.EscapeDbObjectStart.ToCharArray()).TrimEnd(sqlTextBuilder.EscapeDbObjectEnd.ToCharArray())].FieldType == FieldType.Column)
                {
                    ColumnField columnField = (ColumnField)view.Fields[sortColumn.TrimStart(sqlTextBuilder.EscapeDbObjectStart.ToCharArray()).TrimEnd(sqlTextBuilder.EscapeDbObjectEnd.ToCharArray())];
                    if (columnField.Encrypted)
                    {
                        sortColumn = " CONVERT(NVARCHAR(250), DECRYPTBYKEY(" + columnField.EncryptedName + ")) ";
                    }
                }
                return string.Format(selectStatement, new object[7] { view.DataTable.TableName, sortColumn, page, pageSize, parentTable, join, direction.ToString() });
            }
        }

        private string GetOpenCertificatesStatement(Durados.View view)
        {
            string s = string.Empty;
            if (!view.Database.EnableDecryption)
                return s;
            ISqlTextBuilder sqlBuilder = GetSqlTextBuilder(view);
            string sql = sqlBuilder.GetOpenCertificateStatement(); ;
            
            HashSet<string> keys = new HashSet<string>();

            foreach (ColumnField columnField in view.GetEncryptedColumns())
            {
                if (!keys.Contains(columnField.GetSymmetricKeyName()))
                {
                    s += string.Format(sql, columnField.GetSymmetricKeyName(), columnField.GetCertificateName());
                    keys.Add(columnField.GetSymmetricKeyName());
                }
            }

            return s;
        }

        private string GetCloseCertificatesStatement(Durados.View view)
        {
            string s = string.Empty;
            ISqlTextBuilder sqlBuilder = GetSqlTextBuilder(view);
            string sql = sqlBuilder.GetCloseCertificateStatement();
            HashSet<string> keys = new HashSet<string>();
            foreach (ColumnField columnField in view.GetEncryptedColumns())
            {
                if (!keys.Contains(columnField.GetSymmetricKeyName()))
                {
                    s += string.Format(sql, columnField.GetSymmetricKeyName());
                    keys.Add(columnField.GetSymmetricKeyName());
                }
            }

            return s;
        }

        private string GetEncryptedColumnsStatement(Durados.View view)
        {
            string s = string.Empty;
            ISqlTextBuilder sqlBuilder = GetSqlTextBuilder(view);
            foreach (ColumnField columnField in view.GetEncryptedColumns())
            {
                s += sqlBuilder.GetDecryptColumnStatement( columnField.EncryptedName, columnField.DatabaseNames);

            }

            return s;
        }

        private string GetSortParentJoin(ParentField parentField)
        {
            string template = " LEFT OUTER JOIN [{0}] with(nolock) ON {1} ";
            string joinFields = GetJoin(parentField);

            string join = string.Format(template, parentField.ParentView.DataTable.TableName, joinFields);
            if (parentField.ParentView.DisplayField.FieldType == FieldType.Parent)
            {
                return join + GetSortParentJoin((ParentField)parentField.ParentView.DisplayField);
            }
            else
            {
                return join;
            }
        }

        public static Cache Cache = new Cache();

        public void FillParentsDataTable(View view, int page, int pageSize, Filter filter, string childSelectStatement, DataSet dataset)
        {
            foreach (ParentField parentField in view.Fields.Values.Where(f => f.FieldType == FieldType.Parent))
            {
                DataRelation parentRelation = parentField.DataRelation;
                View parentView = view.GetParentView(parentRelation);

                if (!view.SystemView && parentView.SystemView && !view.Database.IsMain())
                {
                    FillDataTable(parentView, 500, dataset);
                    continue;
                }

                FillParentDataTable(view, parentView, parentField, page, pageSize, filter, childSelectStatement, dataset);

                if (!string.IsNullOrEmpty(parentField.DisplayField) && parentField.GetDisplayField().FieldType == FieldType.Parent)
                {
                    parentView = ((ParentField)parentField.GetDisplayField()).ParentView;
                    FillParentDataTable(view, parentView, parentField, page, pageSize, filter, childSelectStatement, dataset);

                }
                //if (parentView.Cached)
                //{
                //    DataTable parentTable = dataset.Tables[parentView.DataTable.TableName];

                //    if (Cache.HasCache(parentView.Name))
                //    {
                //        Cache.Get(parentView.Name, parentTable);
                //        if (parentView.DisplayField.FieldType == FieldType.Parent)
                //        {
                //            FillAncesterDataTable(((ParentField)parentView.DisplayField).ParentView, dataset);
                //        }
                //    }
                //    else
                //    {
                //        FillAncesterDataTable(parentView, dataset);
                //    }
                //}
                //else
                //{
                //    FillParentDataTable(view, parentField, page, pageSize, filter, childSelectStatement, dataset);
                //}
            }
        }

        public void FillParentDataTable(View view, View parentView, ParentField parentField, int page, int pageSize, Filter filter, string childSelectStatement, DataSet dataset)
        {
            if (parentView.Cached)
            {
                DataTable parentTable = dataset.Tables[parentView.DataTable.TableName];

                if (Cache.HasCache(parentView.Name))
                {
                    Cache.Get(parentView.Name, parentTable);
                    if (parentView.DisplayField.FieldType == FieldType.Parent)
                    {
                        FillAncesterDataTable(((ParentField)parentView.DisplayField).ParentView, dataset);
                    }
                }
                else
                {
                    FillAncesterDataTable(parentView, dataset);
                }
            }
            else
            {
                FillParentDataTable(view, parentField, page, pageSize, filter, childSelectStatement, dataset);
            }
        }

        public void FillDataTable(View view, int pageSize, DataSet dataset)
        {
            ISqlTextBuilder sqlTextBuilder = GetSqlTextBuilder(view);

            string selectStatement = GetSelect(view, pageSize).TrimEnd().TrimEnd(',') + string.Format(" from " + sqlTextBuilder.EscapeDbObject("{0}") + " " + sqlTextBuilder.WithNolock, view.DataTable.TableName);

            SqlDataAdapter adapter = new SqlDataAdapter(selectStatement, view.ConnectionString);
            //adapter.Fill(parentView.DataTable);
            try
            {
                adapter.Fill(dataset.Tables[view.DataTable.TableName]);
            }
            catch (System.Data.ConstraintException exception)
            {
                string constraintName = GetViolatedConstraint(dataset.Tables[view.DataTable.TableName]);
                DuradosException duradosException = new DuradosException(constraintName, exception);
                if (view.Database.DiagnosticsReportInProgress && view.Database.Logger != null)
                {
                    view.Database.Logger.Log(view.Name, "", view.Database.DiagnosticsReport.Name, duradosException, 1, view.Database.Logger.NowWithMilliseconds(), view.Database.DiagnosticsReport.DateTime);
                }

                throw duradosException;
            }

        }

        public void FillParentDataTable(View view, ParentField parentField, int page, int pageSize, Filter filter, string childSelectStatement, DataSet dataset)
        {
            View parentView = parentField.ParentView;
            DataRelation parentRelation = parentField.DataRelation;

            string selectStatement = GetParentSelect(view, parentField, childSelectStatement, dataset.Tables[view.DataTable.TableName]);

            IDbConnection connection = GetNewConnection(view.ConnectionString);
            IDbCommand command = GetNewCommand(selectStatement, connection);

            foreach (IDataParameter parameter in filter.Parameters)
            {
                command.Parameters.Add(GetNewParameter(command, parameter.ParameterName, parameter.Value));
            }
            IDataAdapter adapter = GetNewAdapter(command);

            //adapter.Fill(parentView.DataTable);
            try
            {
                view.Database.Logger.Log(parentField.ParentView.Name, DateTime.Now.Ticks.ToString(), "before Fill parent", null, 5, selectStatement);
 
                Fill(adapter, dataset.Tables[parentView.DataTable.TableName]);

                view.Database.Logger.Log(parentField.ParentView.Name, DateTime.Now.Ticks.ToString(), "after Fill parent", null, 5, dataset.Tables[parentView.DataTable.TableName].Rows.Count.ToString() + ", " + selectStatement);

            }
            catch (System.Data.ConstraintException exception)
            {
                string constraintName = GetViolatedConstraint(dataset.Tables[parentView.DataTable.TableName]);
                DuradosException duradosException = new DuradosException(constraintName, exception);
                if (parentView.Database.DiagnosticsReportInProgress && parentView.Database.Logger != null)
                {
                    parentView.Database.Logger.Log(parentView.Name, parentRelation.RelationName, parentView.Database.DiagnosticsReport.Name, duradosException, 1, parentView.Database.Logger.NowWithMilliseconds(), parentView.Database.DiagnosticsReport.DateTime);
                }

                throw duradosException;
            }
            if (parentView.DisplayField.FieldType == FieldType.Parent)
            {
                FillAncesterDataTable(((ParentField)parentView.DisplayField).ParentView, dataset);
            }
            if (!string.IsNullOrEmpty(parentField.DisplayField) && parentField.GetDisplayField().FieldType == FieldType.Parent)
            {
                FillAncesterDataTable(((ParentField)parentField.GetDisplayField()).ParentView, dataset);
            }
        }

        private bool IsNumeric(Type type)
        {
            if (type == null)
            {
                return false;
            }

            switch (Type.GetTypeCode(type))
            {
                case TypeCode.Byte:
                case TypeCode.Decimal:
                case TypeCode.Double:
                case TypeCode.Int16:
                case TypeCode.Int32:
                case TypeCode.Int64:
                case TypeCode.SByte:
                case TypeCode.Single:
                case TypeCode.UInt16:
                case TypeCode.UInt32:
                case TypeCode.UInt64:
                    return true;
                case TypeCode.Object:
                    if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>))
                    {
                        return IsNumeric(Nullable.GetUnderlyingType(type));
                    }
                    return false;
            }
            return false;

        }

        private bool IsNumeric(DataColumn column)
        {
            return IsNumeric(column.DataType);
        }


        public string GetParentSelect(View view, ParentField parentField, string childSelectStatement, DataTable table)
        {
            ISqlTextBuilder sqlTextBuilder = GetSqlTextBuilder(view);
            View parentView = parentField.ParentView;
            DataRelation parentRelation = parentField.DataRelation;

            int countIn = 0;
            string inDelimitedValues = GetDelimitedValues(parentField, table, out countIn);
            if (parentRelation.ChildColumns.Length == 1 && countIn < 50 && (IsNumeric(parentRelation.ChildColumns[0].DataType) || parentRelation.ChildColumns[0].DataType.Equals(typeof(string))))
            {
                string selectStatement = GetSelect(parentView, 1) + " from " + sqlTextBuilder.EscapeDbObject("{0}") + sqlTextBuilder.WithNolock + " where {1} in ({2})";
                selectStatement = GetOpenCertificatesStatement(parentView) + selectStatement + GetCloseCertificatesStatement(parentView);
                return string.Format(selectStatement, new object[3] { parentView.DataTable.TableName, GetColumnName(parentView, parentRelation.ParentColumns[0]), inDelimitedValues });

            }
            else
            {
                string selectStatement = GetSelect(parentView, 1) + " from " + sqlTextBuilder.EscapeDbObject("{0}") + sqlTextBuilder.WithNolock + " inner join (select distinct {4} from " + sqlTextBuilder.EscapeDbObject("{0}") + sqlTextBuilder.WithNolock + " inner join ({1}) as {2} on {3}) as pk on {5}";
                selectStatement = GetOpenCertificatesStatement(parentView) + selectStatement + GetCloseCertificatesStatement(parentView);

                return string.Format(selectStatement, new object[6] { parentView.DataTable.TableName, childSelectStatement, "fk", GetForiegnKeyColumnsWhereStatement(parentView, parentRelation, "fk"), GetPrimaryKeyColumnsDelimited(parentView), GetPrimaryKeyJoin(parentView, "pk") });

            }
        }

        public void FillAncesterDataTable(View parentView, DataSet dataset)
        {
            ISqlTextBuilder sqlTextBuilder = GetSqlTextBuilder(parentView);
            if (parentView.Cached && Cache.HasCache(parentView.Name))
            {
                DataTable parentTable = dataset.Tables[parentView.DataTable.TableName];

                Cache.Get(parentView.Name, parentTable);

            }
            else
            {
                string selectStatement = "select * from " + sqlTextBuilder.EscapeDbObject("{0}") + sqlTextBuilder.WithNolock;
                if (parentView.HasEncryptedColumns())
                    selectStatement = GetSelect(parentView, 1) + " from " + sqlTextBuilder.EscapeDbObject("{0}") + sqlTextBuilder.WithNolock + " where {1} in ({2})";
                selectStatement = GetOpenCertificatesStatement(parentView) + selectStatement + GetCloseCertificatesStatement(parentView);

                selectStatement = string.Format(selectStatement, new object[1] { parentView.DataTable.TableName });
                IDbConnection connection = GetNewConnection(parentView.ConnectionString);
                IDbCommand command = GetNewCommand(selectStatement, connection);

                IDataAdapter adapter = GetNewAdapter(command);

                DataTable parentTable = dataset.Tables[parentView.DataTable.TableName];
                Fill(adapter, parentTable);

                if (parentView.Cached)
                {
                    Cache.Set(parentView.Name, parentTable);
                }

            }

            if (parentView.DisplayField.FieldType == FieldType.Parent)
            {
                FillAncesterDataTable(((ParentField)parentView.DisplayField).ParentView, dataset);
            }
        }



        //public void FillParentsDataTable(View view, int page, int pageSize, Filter filter, string childSelectStatement, DataSet dataset)
        //{
        //    foreach (DataRelation parentRelation in view.DataTable.ParentRelations)
        //    {
        //        View parentView = view.GetParentView(parentRelation);
        //        if (parentView.Cached)
        //        {
        //            DataTable parentTable = dataset.Tables[parentView.DataTable.TableName];

        //            if (Cache.HasCache(parentView.Name))
        //            {
        //                Cache.Get(parentView.Name, parentTable);
        //                if (parentView.DisplayField.FieldType == FieldType.Parent)
        //                {
        //                    FillAncesterDataTable(parentView, (ParentField)parentView.DisplayField, dataset, childSelectStatement, filter.Parameters, 1);
        //                }
        //            }
        //            else
        //            {
        //                FillParentDataTable(view, parentView, parentRelation, page, pageSize, filter, childSelectStatement, dataset);

        //                //if (parentView.DisplayField.FieldType == FieldType.Parent)
        //                //{
        //                //    //FillAncesterDataTable(parentView, (ParentField)parentView.DisplayField, dataset, childSelectStatement, filter.Parameters, 1);
        //                //    FillParentDataTable(view, parentView, parentRelation, page, pageSize, filter, childSelectStatement, dataset);

        //                //}
        //                //FillAncesterDataTable(parentView, dataset, childSelectStatement);
        //            }
        //        }
        //        else
        //        {
        //            FillParentDataTable(view, parentView, parentRelation, page, pageSize, filter, childSelectStatement, dataset);
        //        }
        //    }
        //}


        public virtual SqlSchema GetNewSqlSchema()
        {
            return new SqlSchema();
        }
        
        private string GetDistinctSelect(DataTable table)
        {
            StringBuilder s = new StringBuilder();
            foreach (DataColumn column in table.Columns)
            {
                if (column.DataType == typeof(string) && column.MaxLength > 10000)
                {
                    if (!column.AllowDBNull)
                    {
                        s.Append("'' as ");


                        s.Append("[");
                        s.Append(column.ColumnName);
                        s.Append("]");

                        s.Append(",");
                    }
                }
                else
                {
                    s.Append("[");
                    s.Append(table.TableName);
                    s.Append("]");

                    s.Append(".");

                    s.Append("[");
                    s.Append(column.ColumnName);
                    s.Append("]");

                    s.Append(",");
                }

            }

            if (s.Length > 0)
                s.Remove(s.Length - 1, 1);

            return s.ToString();
        }

        //public void FillParentDataTable(View view, View parentView, DataRelation parentRelation, int page, int pageSize, Filter filter, string childSelectStatement, DataSet dataset)
        //{
        //    string selectStatement = string.Empty;

        //    if (parentView.Cached)
        //    {
        //        selectStatement = "select * from [{0}] with(nolock) ";
        //        selectStatement = string.Format(selectStatement, new object[1] { parentView.DataTable.TableName });
        //    }
        //    else
        //    {
        //        selectStatement = "select {6} from [{0}] with(nolock) inner join (select distinct {4} from [{0}] with(nolock) inner join ({1}) as {2} on {3}) as pk on {5}";
        //        selectStatement = string.Format(selectStatement, new object[7] { parentView.DataTable.TableName, childSelectStatement, "fk", GetForiegnKeyColumnsWhereStatement(parentRelation, "fk"), GetPrimaryKeyColumnsDelimited(parentView), GetPrimaryKeyJoin(parentView, "pk"), GetDistinctSelect(parentView.DataTable) });
        //    }

        //    if (parentView.Cached && Cache.HasCache(parentView.Name))
        //    {
        //        DataTable parentTable = dataset.Tables[parentView.DataTable.TableName];

        //        Cache.Get(parentView.Name, parentTable);

        //    }
        //    else
        //    {


        //        SqlDataAdapter adapter = new SqlDataAdapter(selectStatement, view.ConnectionString);

        //        foreach (SqlParameter parameter in filter.Parameters)
        //        {
        //            adapter.SelectCommand.Parameters.AddWithValue(parameter.ParameterName, parameter.Value);
        //        }
        //        //adapter.Fill(parentView.DataTable);
        //        try
        //        {
        //            DataTable table = dataset.Tables[parentView.DataTable.TableName];
        //            adapter.Fill(table);

        //            if (parentView.Cached)
        //            {
        //                Cache.Set(parentView.Name, table);
        //            }

        //            if (parentView.Database.DiagnosticsReportInProgress)
        //            {
        //                int count = table.Rows.Count;
        //                if (count > parentView.Database.DiagnosticsReport.OverLoadLimit && parentView.Database.Logger != null)
        //                {
        //                    parentView.Database.Logger.Log(parentView.Name, parentRelation.RelationName, parentView.Database.DiagnosticsReport.Name, parentView.Database.Logger.NowWithMilliseconds(), parentView.Database.DiagnosticsReport.GetStackTrace(), -count, parentView.Database.Logger.NowWithMilliseconds(), parentView.Database.DiagnosticsReport.DateTime);
        //                }
        //            }
        //        }
        //        catch (System.Data.ConstraintException exception)
        //        {
        //            string constraintName = GetViolatedConstraint(dataset.Tables[parentView.DataTable.TableName]);
        //            DuradosException duradosException = new DuradosException(constraintName, exception);
        //            if (parentView.Database.DiagnosticsReportInProgress && parentView.Database.Logger != null)
        //            {
        //                parentView.Database.Logger.Log(parentView.Name, parentRelation.RelationName, parentView.Database.DiagnosticsReport.Name, duradosException, 1, parentView.Database.Logger.NowWithMilliseconds(), parentView.Database.DiagnosticsReport.DateTime);
        //            }

        //            throw duradosException;
        //        }
        //    }

        //    if (!parentView.Cached)
        //    {
        //        selectStatement = "select {6} from [{0}] with(nolock) inner join (select distinct {4} from [{0}] with(nolock) inner join ({1}) as {2} on {3}) as pk on {5}";
        //        selectStatement = string.Format(selectStatement, new object[7] { parentView.DataTable.TableName, childSelectStatement, "fk", GetForiegnKeyColumnsWhereStatement(parentRelation, "fk"), GetPrimaryKeyColumnsDelimited(parentView), GetPrimaryKeyJoin(parentView, "pk"), GetDistinctSelect(parentView.DataTable) });
        //    }
        //    if (parentView.DisplayField.FieldType == FieldType.Parent)
        //    {
        //        FillAncesterDataTable(parentView, (ParentField)parentView.DisplayField, dataset, selectStatement, filter.Parameters, 1);
        //    }
        //}

        ////public void FillParentDataTable(View view, View parentView, DataRelation parentRelation, int page, int pageSize, Filter filter, string childSelectStatement, DataSet dataset)
        ////{
        ////    string selectStatement = "select {6} from [{0}] with(nolock) inner join (select distinct {4} from [{0}] with(nolock) inner join ({1}) as {2} on {3}) as pk on {5}";

        ////    selectStatement = string.Format(selectStatement, new object[7] { parentView.DataTable.TableName, childSelectStatement, "fk", GetForiegnKeyColumnsWhereStatement(parentRelation, "fk"), GetPrimaryKeyColumnsDelimited(parentView), GetPrimaryKeyJoin(parentView, "pk"), GetDistinctSelect(parentView.DataTable) });

        ////    SqlDataAdapter adapter = new SqlDataAdapter(selectStatement, view.ConnectionString);

        ////    foreach (SqlParameter parameter in filter.Parameters)
        ////    {
        ////        adapter.SelectCommand.Parameters.AddWithValue(parameter.ParameterName, parameter.Value);
        ////    }
        ////    //adapter.Fill(parentView.DataTable);
        ////    try
        ////    {
        ////        DataTable table = dataset.Tables[parentView.DataTable.TableName];
        ////        adapter.Fill(table);

        ////        if (parentView.Database.DiagnosticsReportInProgress)
        ////        {
        ////            int count = table.Rows.Count;
        ////            if (count > parentView.Database.DiagnosticsReport.OverLoadLimit && parentView.Database.Logger != null)
        ////            {
        ////                parentView.Database.Logger.Log(parentView.Name, parentRelation.RelationName, parentView.Database.DiagnosticsReport.Name, parentView.Database.Logger.NowWithMilliseconds(), parentView.Database.DiagnosticsReport.GetStackTrace(), -count, parentView.Database.Logger.NowWithMilliseconds(), parentView.Database.DiagnosticsReport.DateTime);
        ////            }
        ////        }
        ////    }
        ////    catch (System.Data.ConstraintException exception)
        ////    {
        ////        string constraintName = GetViolatedConstraint(dataset.Tables[parentView.DataTable.TableName]);
        ////        DuradosException duradosException = new DuradosException(constraintName, exception);
        ////        if (parentView.Database.DiagnosticsReportInProgress && parentView.Database.Logger != null)
        ////        {
        ////            parentView.Database.Logger.Log(parentView.Name, parentRelation.RelationName, parentView.Database.DiagnosticsReport.Name, duradosException, 1, parentView.Database.Logger.NowWithMilliseconds(), parentView.Database.DiagnosticsReport.DateTime);
        ////        }

        ////        throw duradosException;
        ////    }
        ////    if (parentView.DisplayField.FieldType == FieldType.Parent)
        ////    {
        ////        FillAncesterDataTable(parentView, (ParentField)parentView.DisplayField, dataset, selectStatement, filter.Parameters, 1);
        ////    }
        ////}


        //public void FillAncesterDataTable(View view, ParentField parentField, DataSet dataset, string selectStatementOfParent, SqlParameter[] parameters, int calls)
        //{
        //    View parentView = parentField.ParentView;

        //    string fk = "fk" + calls.ToString();

        //    string selectStatement = string.Empty;
        //    if (parentView.Cached)
        //    {
        //        selectStatement = "select * from [{0}] with(nolock) ";
        //        selectStatement = string.Format(selectStatement, new object[1] { parentView.DataTable.TableName });
        //    }
        //    else
        //    {
        //        selectStatement = "select distinct {4} from [{0}] with(nolock) inner join ({1}) as {2} on {3}";
        //        selectStatement = string.Format(selectStatement, new object[5] { parentView.DataTable.TableName, selectStatementOfParent, fk, GetForiegnKeyColumnsWhereStatement(parentField.DataRelation, fk), GetDistinctSelect(parentView.DataTable) });
        //    }

        //    //selectStatement += " inner join (" + selectStatementOfParent + ") on "

        //    if (parentView.Cached && Cache.HasCache(parentView.Name))
        //    {
        //        DataTable parentTable = dataset.Tables[parentView.DataTable.TableName];

        //        Cache.Get(parentView.Name, parentTable);

        //    }
        //    else
        //    {


        //        SqlDataAdapter adapter = new SqlDataAdapter(selectStatement, parentView.ConnectionString);
        //        //adapter.Fill(parentView.DataTable);
        //        foreach (SqlParameter parameter in parameters)
        //        {
        //            adapter.SelectCommand.Parameters.AddWithValue(parameter.ParameterName, parameter.Value);
        //        }
        //        DataTable parentTable = dataset.Tables[parentView.DataTable.TableName];
        //        adapter.Fill(parentTable);

        //        if (parentView.Cached)
        //        {
        //            Cache.Set(parentView.Name, parentTable);
        //        }

        //        if (parentView.Database.DiagnosticsReportInProgress)
        //        {
        //            int count = parentTable.Rows.Count;
        //            if (count > parentView.Database.DiagnosticsReport.OverLoadLimit && parentView.Database.Logger != null)
        //            {
        //                parentView.Database.Logger.Log(parentView.Name, "", parentView.Database.DiagnosticsReport.Name, string.Empty, parentView.Database.DiagnosticsReport.GetStackTrace(), -count, parentView.Database.Logger.NowWithMilliseconds(), parentView.Database.DiagnosticsReport.DateTime);
        //            }
        //        }
        //    }

        //    if (parentView.DisplayField.FieldType == FieldType.Parent)
        //    {
        //        FillAncesterDataTable(parentView, (ParentField)parentView.DisplayField, dataset, selectStatement, parameters, calls++);
        //    }
        //}

        public virtual int RowCount(View view)
        {
            try
            {
                //string sql = "SELECT rows FROM sys.sysindexes  AS s1 WHERE (rows > 0) AND (indid IN (SELECT MIN(indid) AS Expr1 FROM sys.sysindexes AS s2 WHERE (s1.id = id))) AND (id = OBJECT_ID('" + view.DataTable.TableName + "'))";
                string sql = GetNewSqlSchema().GetTableRowsCount(view.DataTable.TableName);

                using (IDbConnection connection = GetNewConnection(view.ConnectionString))
                {
                    connection.Open();
                    IDbCommand command = GetNewCommand(sql, connection);

                    object scalar = command.ExecuteScalar();

                    if (scalar == null || scalar == DBNull.Value)
                    {
                        return RowFilterCount(view, GetFilter(view, new Dictionary<string, object>(), LogicCondition.And, false, null));
                    }

                    return Convert.ToInt32(scalar);
                }
            }
            catch
            {
                view.Database.NoSysIndex = true;
                return -1;
            }
        }

        public string ExecuteScalar(string connectionString, string sql)
        {
            using (IDbConnection connection = GetNewConnection(connectionString))
            {
                connection.Open();
                IDbCommand command = GetNewCommand(sql, connection);

                object scalar = command.ExecuteScalar();

                if (scalar == null || scalar == DBNull.Value)
                {
                    return string.Empty;
                }

                return scalar.ToString();
            }
        }

        public string ExecuteScalar(string connectionString, string sql, Dictionary<string, object> parameters)
        {
            using (IDbConnection connection = GetNewConnection(connectionString))
            {
                connection.Open();

                IDbCommand command = GetNewCommand(sql, connection);

                if (parameters != null)
                {
                    foreach (string key in parameters.Keys)
                    {
                        command.Parameters.Add(GetNewParameter(command, key, parameters[key]));
                    }
                }
                object scalar = command.ExecuteScalar();

                if (scalar == null || scalar == DBNull.Value)
                {
                    return string.Empty;
                }

                return scalar.ToString();

            }
        }

        public void ExecuteNoneQueryStoredProcedure(string connectionString, string sp, Dictionary<string, object> parameters)
        {
            using (IDbConnection connection = GetNewConnection(connectionString))
            {
                connection.Open();

                IDbCommand command = GetNewCommand("", connection);

                command.CommandType = CommandType.StoredProcedure;

                command.Connection = connection;

                command.CommandText = sp;

                if (parameters != null)
                {
                    foreach (string key in parameters.Keys)
                    {
                        command.Parameters.Add(GetNewParameter(command, key, parameters[key]));
                    }
                }

                command.ExecuteNonQuery();

            }
        }

        public void ExecuteNonQuery(string connectionString, string sql)
        {
            ExecuteNonQuery(connectionString, sql, GetSqlProduct(), null, null);
        }

        public void ExecuteNonQuery(string connectionString, string sql, SqlProduct sqlProduct)
        {
            ExecuteNonQuery(connectionString, sql, sqlProduct, null, null);
        }

        public void ExecuteNonQuery(string connectionString, string sql, Dictionary<string, object> parameters, ExecuteNonQueryRollbackCallback executeNonQueryRollbackCallback)
        {
            ExecuteNonQuery(connectionString, sql, GetSqlProduct(), parameters, executeNonQueryRollbackCallback);
        }


        public void ExecuteNonQueryBulk(string connectionString, string sql, Dictionary<string, object> parameters, DataTable typeTable, ExecuteNonQueryRollbackCallback executeNonQueryRollbackCallback)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                SqlTransaction transaction = connection.BeginTransaction(IsolationLevel.ReadCommitted);

                SqlCommand command = new SqlCommand(sql, connection);
                command.Transaction = transaction;

                if (parameters != null)
                {
                    foreach (string key in parameters.Keys)
                    {
                        command.Parameters.AddWithValue(key, parameters[key]);
                    }
                }
                if (typeTable != null)
                {
                    SqlParameter parameter = new SqlParameter();
                    //The parameter for the SP must be of SqlDbType.Structured 
                    string parameterName = "@table";
                    if (!string.IsNullOrEmpty(typeTable.TableName))
                        parameterName = "@" + typeTable.TableName;
                    parameter.ParameterName = parameterName;
                    parameter.TypeName = typeTable.TableName;
                    parameter.SqlDbType = System.Data.SqlDbType.Structured;
                    parameter.Value = typeTable;
                    command.Parameters.Add(parameter);
                    command.CommandType = CommandType.StoredProcedure;
                }

                command.ExecuteNonQuery();

                if (executeNonQueryRollbackCallback == null)
                    transaction.Commit();
                else
                {
                    string result = executeNonQueryRollbackCallback(parameters);
                    if (result != "success")
                    {
                        transaction.Rollback();
                        throw new DuradosException(result);
                    }
                    else
                        transaction.Commit();
                }
            }
        }

        public void ExecuteNonQuery(string connectionString, string sql, SqlProduct sqlProduct, Dictionary<string, object> parameters, ExecuteNonQueryRollbackCallback executeNonQueryRollbackCallback)
        {
            using (IDbConnection connection = GetNewConnection(sqlProduct, connectionString))
            {
                connection.Open();

                IDbTransaction transaction = connection.BeginTransaction(IsolationLevel.ReadCommitted);

                IDbCommand command = GetNewCommand(sql, connection);
                command.Transaction = transaction;

                if (parameters != null)
                {
                    foreach (string key in parameters.Keys)
                    {
                        command.Parameters.Add(GetNewParameter(command, key, parameters[key]));
                    }
                }
                try
                {
                    command.ExecuteNonQuery();

                    if (executeNonQueryRollbackCallback == null)
                        transaction.Commit();
                    else
                    {
                        string result = executeNonQueryRollbackCallback(parameters);
                        if (result != "success")
                        {
                            //transaction.Rollback();
                            throw new DuradosException(result);
                        }
                        else
                            transaction.Commit();
                    }
                }
                catch (Exception exception)
                {
                    transaction.Rollback();
                    throw exception;
                }
            }
        }

        public void ExecuteNonQuery(View view, IDbCommand command, string sql, Dictionary<string, object> parameters)
        {
            command.CommandText = sql;
            command.CommandType = CommandType.Text;
            command.Parameters.Clear();
            if (parameters != null)
            {
                foreach (string key in parameters.Keys)
                {
                    command.Parameters.Add(GetNewSqlParameter(view, key, parameters[key]));
                }
            }
            command.ExecuteNonQuery();


        }

        public SqlParameter[] ExecuteProcedure(string connectionString, string procedureName, Dictionary<string, object> parameters, ExecuteProcedureRollbackCallback executeProcedureRollbackCallback)
        {
            using (IDbConnection connection = GetNewConnection(connectionString))
            {
                connection.Open();

                IDbTransaction transaction = connection.BeginTransaction(IsolationLevel.ReadCommitted);

                using (IDbCommand command = GetNewCommand(procedureName, connection))
                {
                    command.Transaction = transaction;
                    command.CommandType = CommandType.StoredProcedure;

                    if (parameters != null)
                    {
                        foreach (string key in parameters.Keys)
                        {
                            command.Parameters.Add(GetNewParameter(command, key, parameters[key]));
                        }
                    }
                    command.ExecuteNonQuery();

                    if (executeProcedureRollbackCallback == null)
                        transaction.Commit();
                    else
                    {
                        bool rollback = executeProcedureRollbackCallback(parameters);
                        if (rollback)
                            transaction.Rollback();
                        else
                            transaction.Commit();
                    }

                    List<SqlParameter> sqlParameters = new List<SqlParameter>();
                    foreach (SqlParameter sqlParameter in command.Parameters)
                    {
                        sqlParameters.Add(sqlParameter);
                    }
                    return sqlParameters.ToArray();
                }
            }
        }

        public System.Data.IDataParameter[] ExecuteProcedure(string connectionString, string procedureName, Dictionary<string, object> parameters, List<string> outParameters,ExecuteProcedureRollbackCallback executeProcedureRollbackCallback)
        {
            using (IDbConnection connection = GetNewConnection(connectionString))
            {
                connection.Open();

                IDbTransaction transaction = connection.BeginTransaction(IsolationLevel.ReadCommitted);

                using (IDbCommand command = GetNewCommand(procedureName, connection))
                {
                    command.Transaction = transaction;
                    command.CommandType = CommandType.StoredProcedure;

                    if (parameters != null)
                    {
                        foreach (string key in parameters.Keys)
                        {

                            System.Data.IDataParameter dataParameter = GetNewParameter(command, key, parameters[key]);
                            dataParameter.Direction= outParameters.Contains(key) ? ParameterDirection.Output : ParameterDirection.Input;
                            command.Parameters.Add(dataParameter);
                        }
                    }
                    command.ExecuteNonQuery();

                    if (executeProcedureRollbackCallback == null)
                        transaction.Commit();
                    else
                    {
                        bool rollback = executeProcedureRollbackCallback(parameters);
                        if (rollback)
                            transaction.Rollback();
                        else
                            transaction.Commit();
                    }

                    List<IDataParameter> sqlParameters = new List<IDataParameter>();
                    foreach (IDataParameter sqlParameter in command.Parameters)
                    {
                        sqlParameters.Add(sqlParameter);
                    }
                    return sqlParameters.ToArray();
                }
            }
        }

        public DataTable ExecuteTable(IDbCommand command, string sql, Dictionary<string, object> parameters)
        {
            command.CommandText = sql;

            if (parameters != null)
            {
                foreach (string key in parameters.Keys)
                {
                    command.Parameters.Add(GetNewParameter(command, key, parameters[key]));
                }
            }

            IDataAdapter adapter = GetNewAdapter(command);

            DataTable table = new DataTable();

            Fill(adapter, table);

            return table;

        }

        public DataTable ExecuteTable(string connectionString, string sql, Dictionary<string, object> parameters, CommandType commandType)
        {
            using (IDbConnection connection = GetNewConnection(connectionString))
            {
                connection.Open();

                IDbCommand command = GetNewCommand(sql, connection);

                command.CommandType = commandType;

                if (parameters != null)
                {
                    foreach (string key in parameters.Keys)
                    {
                        command.Parameters.Add(GetNewParameter(command, key, parameters[key]));
                    }
                }

                IDataAdapter adapter = GetNewAdapter(command);

                DataTable table = new DataTable();

                Fill(adapter, table);

                return table;
            }
        }

        public DataTable GetTableFromCommand(IDbCommand command, string sql, Dictionary<string, object> parameters, CommandType commandType)
        {

            command.CommandType = commandType;
            command.CommandText = sql;

            if (parameters != null)
            {
                foreach (string key in parameters.Keys)
                {
                    command.Parameters.Add(GetNewParameter(command, key, parameters[key]));
                }
            }

            IDataAdapter adapter = GetNewAdapter(command);

            DataTable table = new DataTable();

            Fill(adapter, table);

            return table;

        }



        public int RowFilterCount(View view, Filter filter)
        {
            ISqlTextBuilder sqlTextBuilder = GetSqlTextBuilder(view);

            string sql = "SELECT count(*) from " + sqlTextBuilder.EscapeDbObject(view.DataTable.TableName) + sqlTextBuilder.WithNolock + ((filter == null) ? string.Empty : filter.WhereStatement);

            using (IDbConnection connection = GetNewConnection(view.ConnectionString))
            {
                connection.Open();
                IDbCommand command = GetNewCommand(sql, connection);
                if (filter != null)
                {
                    foreach (IDataParameter parameter in filter.Parameters)
                    {
                        command.Parameters.Add(GetNewParameter(command, parameter.ParameterName, parameter.Value));
                    }
                }

                object scalar = command.ExecuteScalar();

                if (scalar == null || scalar == DBNull.Value)
                {
                    return 0;
                }

                return Convert.ToInt32(scalar);
            }
        }

        public int? Create(View view, Dictionary<string, object> values, BeforeCreateEventHandler beforeCreateCallback, BeforeCreateInDatabaseEventHandler beforeCreateInDatabaseEventHandler, AfterCreateEventHandler afterCreateBeforeCommitCallback, AfterCreateEventHandler afterCreateAfterCommitCallback)
        {
            return Create(view, values, null, beforeCreateCallback, beforeCreateInDatabaseEventHandler, afterCreateBeforeCommitCallback, afterCreateAfterCommitCallback);
        }

        protected virtual string GetTableName(View view)
        {
            return string.IsNullOrEmpty(view.EditableTableName) ? view.DataTable.TableName : view.EditableTableName;

        }
        /*
        protected int? Create(View view, Dictionary<string, object> values, string insertAbovePK, BeforeCreateEventHandler beforeCreateCallback, AfterCreateEventHandler afterCreateBeforeCommitCallback, AfterCreateEventHandler afterCreateAfterCommitCallback)
        {
            string pk;
            int? id = null;


            CreateEventArgs createEventArgs = new CreateEventArgs(view, values, null, null);
            if (beforeCreateCallback != null)
                beforeCreateCallback(this, createEventArgs);

            if (createEventArgs.Cancel)
                return null;


            using (SqlConnection connection = GetNewConnection(view.ConnectionString))
            {
                connection.Open();
                SqlTransaction transaction = connection.BeginTransaction(IsolationLevel.ReadCommitted);
                SqlCommand command = new SqlCommand();
                command.Connection = connection;
                command.Transaction = transaction;

                string tableName = GetTableName(view);

                HandleOrder(view, values, insertAbovePK, command);

                bool autoIdentity = view.IsAutoIdentity;
                string sql = "insert into [{0}] ({1}) values ({2});";
                sql = string.Format(sql, tableName, GetDelimitedColumns(view, DataAction.Create), GetDelimitedColumnsParameters(view, DataAction.Create));

                if (autoIdentity)
                {
                    sql += "SELECT IDENT_CURRENT('[" + tableName + "]') AS ID";
                }
                command.CommandText = sql;
                command.Parameters.Clear();
                foreach (SqlParameter parameter in GetParemeters(view, values))
                {
                    command.Parameters.AddWithValue(parameter.ParameterName, parameter.Value);
                }
                object scalar = command.ExecuteScalar();



                if (scalar == null || scalar == DBNull.Value)
                {
                    id = null;
                }
                else
                {
                    id = Convert.ToInt32(scalar);
                }

                if (autoIdentity)
                {
                    string name = view.DataTable.PrimaryKey[0].ColumnName;
                    if (values.ContainsKey(name))
                        values[name] = id;
                    else
                        values.Add(name, id);
                }


                if (id.HasValue)
                    pk = id.ToString();
                else
                {
                    pk = string.Empty;
                    //foreach (DataColumn column in view.DataTable.PrimaryKey)
                    //{
                    //    pk += values[column.ColumnName] + (view.DataTable.PrimaryKey.Last() == column ? "" : ",");
                    //}

                    foreach (Field field in view.PrimaryKeyFileds)
                    {
                        pk += values[field.Name] + (view.PrimaryKeyFileds.Last() == field ? "" : ",");
                    }
                }


                if (createEventArgs.History != null)
                {

                    History history = (History)createEventArgs.History;

                    history.SaveCreate(command, view, pk, createEventArgs.UserId);
                }

                if (afterCreateBeforeCommitCallback != null)
                {
                    createEventArgs = new CreateEventArgs(view, values, pk, command);
                    afterCreateBeforeCommitCallback(this, createEventArgs);
                }
                //view.OnAfterCreateBeforeCommit(createEventArgs);
                if (createEventArgs.Cancel)
                {
                    if (autoIdentity)
                    {
                        string name = view.DataTable.PrimaryKey[0].ColumnName;
                        values.Remove(name);
                    }

                    transaction.Rollback();
                    id = null;
                }
                else
                {
                    transaction.Commit();
                }
            }

            createEventArgs = new CreateEventArgs(view, values, pk, null);
            if (afterCreateAfterCommitCallback != null)
                afterCreateAfterCommitCallback(this, createEventArgs);

            var checkLists = view.Fields.Values.Where(f => f.FieldType == FieldType.Children && ((ChildrenField)f).Persist);
            if (checkLists.Count() > 0)
            {
                UpdateCheckLists(view, pk, values);
            }

            return id;
        }
        */

        public int? Create(View view, Dictionary<string, object> values, string insertAbovePK, BeforeCreateEventHandler beforeCreateCallback, BeforeCreateInDatabaseEventHandler beforeCreateInDatabaseEventHandler, AfterCreateEventHandler afterCreateBeforeCommitCallback, AfterCreateEventHandler afterCreateAfterCommitCallback)
        {
            string pk;
            int? id = null;

            CreateEventArgs createEventArgs = null;
            IDbCommand sysCommand = null;
            bool identicalSystemConnection = view.Database.IdenticalSystemConnection;

            try
            {
                using (IDbConnection connection = GetNewConnection(view.ConnectionString))
                {
                    connection.Open();
                    IDbTransaction transaction = connection.BeginTransaction(IsolationLevel.ReadCommitted);
                    IDbCommand command = GetNewCommand("", connection);
                    command.Connection = connection;
                    command.Transaction = transaction;


                    IDbTransaction sysTransaction = null;
                    if (!identicalSystemConnection)
                    {
                        IDbConnection sysConnection = GetNewConnection(view.Database.SystemConnectionString);
                        sysCommand = GetNewCommand(string.Empty, sysConnection);
                        sysConnection.Open();
                        sysTransaction = sysConnection.BeginTransaction(IsolationLevel.ReadCommitted);
                        sysCommand.Connection = sysConnection;
                        sysCommand.Transaction = sysTransaction;

                    }
                    else
                    {
                        sysCommand = command;

                    }
                    createEventArgs = new CreateEventArgs(view, values, null, command, sysCommand);

                    if (beforeCreateCallback != null)
                        beforeCreateCallback(this, createEventArgs);

                    if (createEventArgs.Cancel)
                    {
                        transaction.Rollback();
                        return null;
                    }

                    List<string> columnNames = GetColumnNamesList(view, DataAction.Create, values);
                    createEventArgs.ColumnNames = columnNames;

                    if (beforeCreateInDatabaseEventHandler != null)
                        beforeCreateInDatabaseEventHandler(this, createEventArgs);

                    if (createEventArgs.Cancel)
                    {
                        transaction.Rollback();
                        if (!identicalSystemConnection)
                        {
                            sysTransaction.Rollback();
                        }
                        return null;
                    }

                    //Change by Mirih
                    if (!view.CreateOverride)
                    {
                        pk = Create(view, values, columnNames, insertAbovePK, command, sysCommand, out id, createEventArgs.History, createEventArgs.UserId);
                    }
                    else
                    {
                        pk = GetPKOnCreateOverride(view, command, values, out id);
                    }

                    if (afterCreateBeforeCommitCallback != null)
                    {
                        createEventArgs = new CreateEventArgs(view, values, pk, command, sysCommand);
                        afterCreateBeforeCommitCallback(this, createEventArgs);
                    }
                    //view.OnAfterCreateBeforeCommit(createEventArgs);
                    if (createEventArgs.Cancel)
                    {
                        if (id.HasValue)
                        {
                            string name = view.DataTable.PrimaryKey[0].ColumnName;
                            values.Remove(name);
                        }

                        transaction.Rollback();
                        if (!identicalSystemConnection)
                        {
                            sysTransaction.Rollback();
                        }
                        id = null;
                    }
                    else
                    {
                        try
                        {
                            transaction.Commit();
                            if (!identicalSystemConnection)
                            {
                                if (sysTransaction != null && sysCommand.Connection.State == ConnectionState.Open)
                                    sysTransaction.Commit();
                            }
                        }
                        catch
                        {

                        }
                        Cache.Clear(view.Name);
                    }
                }
            }
            finally
            {
                if (!identicalSystemConnection)
                {
                    sysCommand.Connection.Close();
                }
            }

            createEventArgs = new CreateEventArgs(view, values, pk, null, null);
            if (afterCreateAfterCommitCallback != null)
                afterCreateAfterCommitCallback(this, createEventArgs);

            return id;
        }

        /// <summary>
        /// Add by MiriH, get PK when create overide by workflow rule(insert by SP)
        /// </summary>
        /// <param name="view"></param>
        /// <param name="command"></param>
        /// <returns></returns>
        protected virtual string GetPKOnCreateOverride(View view, IDbCommand command, Dictionary<string, object> values, out int? id)
        {
            string pk = string.Empty;
            string tableName = GetTableName(view);
            if (view.IsAutoIdentity)
            {
                string sql = "SELECT IDENT_CURRENT(N'[" + tableName + "]') AS ID"; //GetLastInsertedRow2(tableName);
                command.CommandType = CommandType.Text;
                command.CommandText = sql;
                pk = command.ExecuteScalar().ToString();
                id = Convert.ToInt32(pk);
            }
            else
            {
                //TODO have problem with id not AutoIdentity.
                foreach (Field field in view.PrimaryKeyFileds)
                {
                    if (values.ContainsKey(field.Name))
                        pk += values[field.Name] + (view.PrimaryKeyFileds.Last() == field ? "" : ",");
                    else
                        throw new DuradosException("The primary key field '" + field.Name + "' was not supplied by the user. Make sure that this field should be configured as an auto-increment.");
                }
                id = null;
            }

            string name = view.DataTable.PrimaryKey[0].ColumnName;
            if (values.ContainsKey(name))
                values[name] = pk;
            else
                values.Add(name, pk);
            return pk;
        }

        //protected virtual string 2(string tableName)
        //{
        //    return "SELECT IDENT_CURRENT(N'[" + tableName + "]') AS ID";
            
        //}


        public string Create(View view, Dictionary<string, object> values, List<string> columnNames, string insertAbovePK, IDbCommand command, IDbCommand sysCommand, out int? id, object history, int? userId)
        {
            ISqlTextBuilder sqlTextBuilder = GetSqlTextBuilder(view);
            string pk;
            id = null;
            if (userId == null)
            {
                userId = 1;
            }

            string tableName = GetTableName(view);
            bool autoIdentity = view.IsAutoIdentity;
            bool guidIdentity = view.IsGuidIdentity;

            if (view.DataTable.PrimaryKey.Length == 1)
            {
                DataColumn pkColumn = view.DataTable.PrimaryKey[0];
                if (pkColumn.DataType == typeof(string) && !values.ContainsKey(pkColumn.ColumnName) && pkColumn.MaxLength > 35)
                {
                    guidIdentity = true;
                }
            }

            if (guidIdentity)
            {
                string guidName = view.DataTable.PrimaryKey[0].ColumnName;
                if (!columnNames.Contains(guidName))
                {
                    columnNames.Add(guidName);
                }
                if (!values.ContainsKey(guidName))
                {
                    values.Add(guidName, Durados.Security.GuidGenerator.GenerateTimeBasedGuid());
                }
            }

            HandleOrder(view, values, insertAbovePK, command);

            string sql ="insert into " + sqlTextBuilder.EscapeDbObject("{0}") + " ({1}) values ({2})" +sqlTextBuilder.DbEndOfStatement;
            string delimitedColumns = GetDelimitedColumns(columnNames, view);
            if (string.IsNullOrEmpty(delimitedColumns))
            {
                sql = string.Format("insert into " + sqlTextBuilder.EscapeDbObject("{0}") + sqlTextBuilder.InsertWithoutColumns() + sqlTextBuilder.DbEndOfStatement, tableName);
            }
            else
            {
                sql = string.Format(sql, tableName, delimitedColumns, GetDelimitedColumnsParameters(columnNames, view));
                //sql = string.Format(sql, tableName, GetDelimitedColumns(view, DataAction.Create, values), GetDelimitedColumnsParameters(view, DataAction.Create, values));
            }

            if (autoIdentity)
            {
                sql += sqlTextBuilder.GetLastInsertedRow(view); //"SELECT IDENT_CURRENT(N'[" + tableName + "]') AS ID";
            }
            sql = GetOpenCertificatesStatement(view) + sql + GetCloseCertificatesStatement(view);

            command.CommandType = CommandType.Text;
            command.CommandText = sql;
            command.Parameters.Clear();
            foreach (IDataParameter parameter in GetParemeters(view, values))
            {
                try
                {
                    command.Parameters.Add(GetNewParameter(command, parameter.ParameterName, parameter.Value));

                }
                catch (Exception exception)
                {
                    throw new DuradosException("The value [" + parameter.Value + "] is not in the correct format [" + GetParameterType(parameter) + "] in column name [" + parameter.ParameterName + "]", exception);
                }
            }
            object scalar = command.ExecuteScalar();
            Cache.Clear(view.Name);

            if (scalar == null || scalar == DBNull.Value)
            {
                id = null;
                string sql2 = sqlTextBuilder.GetLastInsertedRow2(view);
                if (!string.IsNullOrEmpty(sql2))
                {
                    command.CommandText = sql2;

                    scalar = command.ExecuteScalar();
                    
                }
            }
            if(!(scalar == null || scalar == DBNull.Value))
            {
                if (autoIdentity)
                {
                    id = Convert.ToInt32(scalar);

                    string name = view.DataTable.PrimaryKey[0].ColumnName;
                    if (values.ContainsKey(name))
                        values[name] = id;
                    else
                        values.Add(name, id);
                }

            }

            if (id.HasValue)
                pk = id.ToString();
            else
            {
                pk = string.Empty;
                //foreach (DataColumn column in view.DataTable.PrimaryKey)
                //{
                //    pk += values[column.ColumnName] + (view.DataTable.PrimaryKey.Last() == column ? "" : ",");
                //}

                foreach (Field field in view.PrimaryKeyFileds)
                {
                    if (values.ContainsKey(field.Name))
                        pk += values[field.Name] + (view.PrimaryKeyFileds.Last() == field ? "" : ",");
                    else
                        throw new DuradosException("The primary key field '" + field.Name + "' was not supplied by the user. Make sure that this field should be configured as an auto-increment.");
                }
            }

            if (!string.IsNullOrEmpty(view.PermanentFilter))
            {
                command.Parameters.Clear();
                command.CommandText = "select 1 from " + sqlTextBuilder.EscapeDbObject(tableName) + " where " + GetWhereStatement(view, tableName, true);
                foreach (IDataParameter parameter in GetWhereParemeters(view, pk))
                {
                    command.Parameters.Add(GetNewParameter(command, parameter.ParameterName, parameter.Value));
                }
                object scalar2 = command.ExecuteScalar();
                
                if (scalar2 == null || scalar2 == DBNull.Value)
                {
                    string pk2 = pk;
                    pk = null;
                    command.Transaction.Rollback();
                    try
                    {
                        sysCommand.Transaction.Rollback();
                    }
                    catch { }
                    throw new RowNotFoundException(view, pk2);
                }
            }


            if (history != null && view.Database.SwVersion != null)
            {
                DataAccess.History.GetHistory(view.Database.SystemSqlProduct).SaveCreate(sysCommand, view, pk, userId.Value, view.Database.SwVersion, view.GetWorkspaceName());
            }

            var checkLists = view.Fields.Values.Where(f => f.FieldType == FieldType.Children && ((ChildrenField)f).Persist);
            if (checkLists.Count() > 0)
            {
                UpdateCheckLists(view, pk, values, command, sysCommand);
            }

            return pk;
        }


        protected virtual void HandleOrder(View view, Dictionary<string, object> values, string insertAbovePK, IDbCommand command)
        {
            ParentField field = view.GetIntegralParent();
            if (!(view.IsOrdered && view.Fields[view.OrdinalColumnName].FieldType == FieldType.Column && ((ColumnField)view.Fields[view.OrdinalColumnName]).DataColumn.DataType.Equals(typeof(int))))
                return;

            Int64 order;
            if (field == null)
            {
                order = GetOrder(view, insertAbovePK, null, null, command);
            }
            else
            {
                order = GetOrder(view, insertAbovePK, values[field.Name].ToString(), field, command);
            }


            if (values.ContainsKey(view.OrdinalColumnName))
            {
                values[view.OrdinalColumnName] = order.ToString();
            }
            else
            {
                values.Add(view.OrdinalColumnName, order.ToString());
            }
        }

        protected virtual Int64 GetOrder(View view, string insertAbovePK, string fk, ParentField integralParentField, IDbCommand command)
        {
            ISqlTextBuilder sqlTextBuilder = GetSqlTextBuilder(view);

            if (!string.IsNullOrEmpty(insertAbovePK))
            {
                string sql = "select " + sqlTextBuilder.EscapeDbObject("{0}") + " from " + sqlTextBuilder.EscapeDbObject("{1}") + sqlTextBuilder.WithNolock + " where {2}";
                sql = string.Format(sql, view.OrdinalColumnName, view.DataTable.TableName, GetWhereStatement(view));
                command.CommandText = sql;
                command.Parameters.Clear();
                foreach (IDataParameter parameter in GetWhereParemeters(view, insertAbovePK))
                {
                    command.Parameters.Add(GetNewParameter(command, parameter.ParameterName, parameter.Value));
                }
                object oOrder = command.ExecuteScalar();


                sql = "UPDATE " + sqlTextBuilder.EscapeDbObject("{0}") + " SET " + sqlTextBuilder.EscapeDbObject("{1}") + " = " + sqlTextBuilder.EscapeDbObject("{1}") + " + 1 where {2} and " + sqlTextBuilder.EscapeDbObject("{1}") + " >= {3}";

                sql = string.Format(sql, view.DataTable.TableName, view.OrdinalColumnName, GetWhereStatement(integralParentField), oOrder);
                command.CommandText = sql;
                command.Parameters.Clear();
                foreach (IDataParameter parameter in GetWhereParemeters(integralParentField, fk))
                {
                    command.Parameters.Add(GetNewParameter(command, parameter.ParameterName, parameter.Value));
                }
                command.ExecuteNonQuery();

                return Convert.ToInt64(oOrder);
            }
            else
            {
                string sql = "select Max(" + sqlTextBuilder.EscapeDbObject("{0}") + ") from " + sqlTextBuilder.EscapeDbObject("{1}") + sqlTextBuilder.WithNolock + " where {2}";
                sql = string.Format(sql, view.OrdinalColumnName, view.DataTable.TableName, GetWhereStatement(integralParentField));
                command.CommandText = sql;
                command.Parameters.Clear();
                foreach (IDataParameter parameter in GetWhereParemeters(integralParentField, fk))
                {
                    command.Parameters.Add(GetNewParameter(command, parameter.ParameterName, parameter.Value));
                }
                object oOrder = command.ExecuteScalar();

                if (oOrder == null || oOrder is DBNull || oOrder == DBNull.Value)
                    oOrder = 0;
                return Convert.ToInt64(oOrder) + 10;
            }
        }

        public int? Create(View view, Dictionary<string, object> values, bool handleOrder, string insertAbovePK, BeforeCreateEventHandler beforeCreateCallback, BeforeCreateInDatabaseEventHandler beforeCreateInDatabaseEventHandler, AfterCreateEventHandler afterCreateBeforeCommitCallback, AfterCreateEventHandler afterCreateAfterCommitCallback, IDbCommand command, out bool rollback)
        {
            ISqlTextBuilder sqlTextBuilder = GetSqlTextBuilder(view);
            string tableName = GetTableName(view);

            CreateEventArgs createEventArgs = new CreateEventArgs(view, values, null, null, null);
            if (beforeCreateCallback != null)
                beforeCreateCallback(this, createEventArgs);

            rollback = false;
            if (createEventArgs.Cancel)
            {
                command.Transaction.Rollback();
                rollback = true;
                return null;
            }

            List<string> columnNames = GetColumnNamesList(view, DataAction.Create, values);
            createEventArgs.ColumnNames = columnNames;
            if (beforeCreateInDatabaseEventHandler != null)
                beforeCreateInDatabaseEventHandler(this, createEventArgs);


            if (handleOrder)
                HandleOrder(view, values, insertAbovePK, command);

            bool autoIdentity = view.IsAutoIdentity;
            string sql = "insert into " + sqlTextBuilder.EscapeDbObject("{0}") + " ({1}) values ({2})" + sqlTextBuilder.DbEndOfStatement;
            //sql = string.Format(sql, tableName, GetDelimitedColumns(view, DataAction.Create, values), GetDelimitedColumnsParameters(view, DataAction.Create, values));
            sql = string.Format(sql, tableName, GetDelimitedColumns(columnNames, view), GetDelimitedColumnsParameters(columnNames, view));

            int? id = null;

            if (autoIdentity)
            {
                sql += sqlTextBuilder.GetLastInsertedRow(view); //sql += "SELECT IDENT_CURRENT('[" + tableName + "]') AS ID";
            }

            command.Parameters.Clear();

            sql = GetOpenCertificatesStatement(view) + sql + GetCloseCertificatesStatement(view);

            command.CommandText = sql;
            foreach (IDataParameter parameter in GetParemeters(view, values))
            {
                command.Parameters.Add(GetNewSqlParameter(view, parameter.ParameterName, parameter.Value));
            }
            object scalar = null;

            try
            {
                scalar = command.ExecuteScalar();
            }
            catch (Exception exception)
            {
                throw new DuradosCreateException(view, values, exception);
            }


            if (scalar == null || scalar == DBNull.Value)
            {
                id = null;
            }
            else
            {
                id = Convert.ToInt32(scalar);
            }

            if (autoIdentity)
            {
                string name = view.DataTable.PrimaryKey[0].ColumnName;
                if (values.ContainsKey(name))
                    values[name] = id;
                else
                    values.Add(name, id);
            }




            if (afterCreateBeforeCommitCallback != null)
            {
                createEventArgs = new CreateEventArgs(view, values, id.HasValue ? id.ToString() : null, command, null);
                afterCreateBeforeCommitCallback(this, createEventArgs);
            }
            //view.OnAfterCreateBeforeCommit(createEventArgs);
            if (createEventArgs.Cancel)
            {
                if (autoIdentity)
                {
                    string name = view.DataTable.PrimaryKey[0].ColumnName;
                    values.Remove(name);
                }

                rollback = true;
                command.Transaction.Rollback();
                id = null;
            }



            createEventArgs = new CreateEventArgs(view, values, id.HasValue ? id.ToString() : null, null, null);
            if (afterCreateAfterCommitCallback != null)
                afterCreateAfterCommitCallback(this, createEventArgs);

            //var checkLists = view.Fields.Values.Where(f => f.FieldType == FieldType.Children && ((ChildrenField)f).Persist);
            //if (checkLists.Count() > 0)
            //{
            //    string pk;

            //    if (id.HasValue)
            //        pk = id.ToString();
            //    else
            //    {
            //        pk = string.Empty;
            //        foreach (DataColumn column in view.DataTable.PrimaryKey)
            //        {
            //            pk += values[column.ColumnName] + (view.DataTable.PrimaryKey.Last() == column ? "" : ",");
            //        }
            //    }

            //    UpdateCheckLists(view, pk, values);
            //}

            return id;
        }

        public virtual void Delete(View view, string fk, string fkField, BeforeDeleteEventHandler beforeDeleteCallback, AfterDeleteEventHandler afterDeleteBeforeCommitCallback, AfterDeleteEventHandler afterDeleteAfterCommitCallback)
        {
            string tempFk = fk;
            DeleteEventArgs deleteEventArgs = new DeleteEventArgs(view, tempFk, null, null);

            if (beforeDeleteCallback != null)
                beforeDeleteCallback(this, deleteEventArgs);

            if (deleteEventArgs.Cancel)
                return;
            fk = deleteEventArgs.PrimaryKey;


            using (IDbConnection connection = GetNewConnection(view.ConnectionString))
            {
                connection.Open();
                IDbTransaction transaction = connection.BeginTransaction(IsolationLevel.ReadCommitted);
                IDbCommand command = GetNewCommand("", connection);
                command.Connection = connection;
                command.Transaction = transaction;

                Delete(view, fk, fkField, command);

                deleteEventArgs = new DeleteEventArgs(view, tempFk, command, null);
                if (afterDeleteBeforeCommitCallback != null)
                    afterDeleteBeforeCommitCallback(this, deleteEventArgs);

                if (deleteEventArgs.Cancel)
                    transaction.Rollback();
                else
                {
                    transaction.Commit();
                    Cache.Clear(view.Name);
                }
            }

            if (afterDeleteAfterCommitCallback != null)
                afterDeleteAfterCommitCallback(this, deleteEventArgs);

        }

        public virtual void Delete(View view, string fk, string fkField, IDbCommand command)
        {
            ISqlTextBuilder sqlTextBuilder = GetSqlTextBuilder(view);
            //string tempFk = fk;
            //DeleteEventArgs deleteEventArgs = new DeleteEventArgs(view, tempFk, null);

            //if (beforeDeleteCallback != null)
            //    beforeDeleteCallback(this, deleteEventArgs);

            //if (deleteEventArgs.Cancel)
            //    return;
            //fk = deleteEventArgs.PrimaryKey;

            string sql = "delete from " + sqlTextBuilder.EscapeDbObject("{0}") + " where {1}";

            ParentField parentField = (ParentField)view.Fields[fkField];
            sql = string.Format(sql, GetTableName(view), GetWhereStatement(parentField));

            command.CommandText = sql;
            command.Parameters.Clear();
            //using (SqlConnection connection = GetNewConnection(view.ConnectionString))
            //{
            //    connection.Open();
            //    SqlTransaction transaction = connection.BeginTransaction(IsolationLevel.ReadCommitted);
            //    SqlCommand command = new SqlCommand(sql, connection);
            //    command.Transaction = transaction;

            foreach (IDataParameter parameter in GetWhereParemeters(parentField, fk))
            {
                command.Parameters.Add(GetNewParameter(command, parameter.ParameterName, parameter.Value));
            }
            try
            {
                command.ExecuteNonQuery();
            }
            catch (SqlException sqlException)
            {
                if (sqlException.Number == 4405) // non updatable view
                {
                    throw new DuradosException("The View " + view.DisplayName + " in not updatable. Please update the 'Editable Table Name' field in the Advanced tab to the appropriate table in the database.", sqlException);
                }
            }
            catch (MySql.Data.MySqlClient.MySqlException sqlException)
            {
                if (sqlException.Number == 4405) // non updatable view
                {
                    throw new DuradosException("The View " + view.DisplayName + " in not updatable. Please update the 'Editable Table Name' field in the Advanced tab to the appropriate table in the database.", sqlException);
                }
            }
            Cache.Clear(view.Name);

            //    deleteEventArgs = new DeleteEventArgs(view, tempFk, command);
            //    if (afterDeleteBeforeCommitCallback != null)
            //        afterDeleteBeforeCommitCallback(this, deleteEventArgs);

            //    if (deleteEventArgs.Cancel)
            //        transaction.Rollback();
            //    else
            //        transaction.Commit();
            //}

            //if (afterDeleteAfterCommitCallback != null)
            //    afterDeleteAfterCommitCallback(this, deleteEventArgs);

        }

        //public virtual void Delete(View view, string fk, string fkField, BeforeDeleteEventHandler beforeDeleteCallback, AfterDeleteEventHandler afterDeleteBeforeCommitCallback, AfterDeleteEventHandler afterDeleteAfterCommitCallback)
        //{
        //    string tempFk = fk;
        //    DeleteEventArgs deleteEventArgs = new DeleteEventArgs(view, tempFk, null);

        //    if (beforeDeleteCallback != null)
        //        beforeDeleteCallback(this, deleteEventArgs);

        //    if (deleteEventArgs.Cancel)
        //        return;
        //    fk = deleteEventArgs.PrimaryKey;

        //    string sql = "delete from [{0}] where {1}";

        //    ParentField parentField = (ParentField)view.Fields[fkField];
        //    sql = string.Format(sql, GetTableName(view), GetWhereStatement(parentField));

        //    using (SqlConnection connection = GetNewConnection(view.ConnectionString))
        //    {
        //        connection.Open();
        //        SqlTransaction transaction = connection.BeginTransaction(IsolationLevel.ReadCommitted);
        //        SqlCommand command = new SqlCommand(sql, connection);
        //        command.Transaction = transaction;

        //        foreach (SqlParameter parameter in GetWhereParemeters(parentField, fk))
        //        {
        //            command.Parameters.AddWithValue(parameter.ParameterName, parameter.Value);
        //        }
        //        command.ExecuteNonQuery();

        //        deleteEventArgs = new DeleteEventArgs(view, tempFk, command);
        //        if (afterDeleteBeforeCommitCallback != null)
        //            afterDeleteBeforeCommitCallback(this, deleteEventArgs);

        //        if (deleteEventArgs.Cancel)
        //            transaction.Rollback();
        //        else
        //            transaction.Commit();
        //    }

        //    if (afterDeleteAfterCommitCallback != null)
        //        afterDeleteAfterCommitCallback(this, deleteEventArgs);

        //}

        public virtual void Delete(View view, string pk, BeforeDeleteEventHandler beforeDeleteCallback, AfterDeleteEventHandler afterDeleteBeforeCommitCallback, AfterDeleteEventHandler afterDeleteAfterCommitCallback, Dictionary<string, object> values = null)
        {
            ISqlTextBuilder sqlTextBuilder = GetSqlTextBuilder(view);
            string tableName = GetTableName(view);

            DeleteEventArgs deleteEventArgs = null;

            bool identicalSystemConnection = view.Database.IdenticalSystemConnection;


            using (IDbConnection connection = GetNewConnection(view.ConnectionString))
            {
                connection.Open();
                IDbTransaction transaction = connection.BeginTransaction(IsolationLevel.ReadCommitted);
                IDbCommand command = GetNewCommand("", connection);
                command.Connection = connection;
                command.Transaction = transaction;

                string tempPk = pk;
                deleteEventArgs = new DeleteEventArgs(view, tempPk, command, null, values);
                DataRow deletedRow = GetDataRow(view, pk);
                if (deletedRow == null)
                {
                    try
                    {
                        deletedRow = GetDataRow2(view, pk, command);
                    }
                    catch { }
                }

                if (deletedRow == null)
                {
                    throw new RowNotFoundException(view, pk);
                }
                deleteEventArgs.PrevRow = deletedRow;

                if (beforeDeleteCallback != null)
                    beforeDeleteCallback(this, deleteEventArgs);

                if (deleteEventArgs.Cancel)
                {
                    transaction.Rollback();
                    return;
                }

                if (!view.DeleteOverride)
                {
                    pk = deleteEventArgs.PrimaryKey;

                    string sql = "delete from " + sqlTextBuilder.EscapeDbObject("{0}") + " where {1}";
                    sql = string.Format(sql, tableName, GetWhereStatement(view, tableName));

                    command.CommandText = sql;
                    command.Parameters.Clear();

                    foreach (IDataParameter parameter in GetWhereParemeters(view, pk))
                    {
                        command.Parameters.Add(GetNewParameter(command, parameter.ParameterName, parameter.Value));
                    }




                    command.ExecuteNonQuery();
                }
                History history = null;

                if (deleteEventArgs.History != null)
                {

                    history = (History)deleteEventArgs.History;

                    if (identicalSystemConnection)
                    {
                        history.SaveDelete(command, view, pk, deleteEventArgs.UserId, view.Database.SwVersion, view.GetWorkspaceName(), deletedRow);
                    }
                }

                deleteEventArgs = new DeleteEventArgs(view, pk, command, null, values);
                deleteEventArgs.PrevRow = deletedRow;

                if (afterDeleteBeforeCommitCallback != null)
                    afterDeleteBeforeCommitCallback(this, deleteEventArgs);

                if (deleteEventArgs.Cancel)
                    transaction.Rollback();
                else
                {
                    transaction.Commit();
                    Cache.Clear(view.Name);
                }

                if (!identicalSystemConnection && history != null)
                {
                    IDbConnection sysConnection = GetNewConnection(view.Database.SystemConnectionString);
                    IDbCommand sysCommand = GetNewCommand("", sysConnection);
                    sysCommand.Connection = sysConnection;
                    sysConnection.Open();

                    history.SaveDelete(sysCommand, view, pk, deleteEventArgs.UserId, view.Database.SwVersion, view.GetWorkspaceName(), deletedRow);

                    sysConnection.Close();
                }
            }



            if (afterDeleteAfterCommitCallback != null)
                afterDeleteAfterCommitCallback(this, deleteEventArgs);


        }

        public virtual int Delete(View view, string pk, BeforeDeleteEventHandler beforeDeleteCallback, AfterDeleteEventHandler afterDeleteBeforeCommitCallback, AfterDeleteEventHandler afterDeleteAfterCommitCallback, IDbCommand useCommand, IDbCommand useSysCommand, Dictionary<string, object> values = null)
        {
            ISqlTextBuilder sqlTextBuilder = GetSqlTextBuilder(view);
            string tableName = GetTableName(view);

            DeleteEventArgs deleteEventArgs = null;

            bool identicalSystemConnection = view.Database.IdenticalSystemConnection;

            IDbCommand command = null;
            IDbTransaction transaction = null;
            IDbConnection connection = null;
            int affected = 0;

            if (useCommand == null)
            {
                connection = GetNewConnection(view.ConnectionString);

                connection.Open();
                transaction = connection.BeginTransaction(IsolationLevel.ReadCommitted);
                command = GetNewCommand("", connection);
                command.Connection = connection;
                command.Transaction = transaction;
            }
            else
            {
                command = useCommand;
                transaction = command.Transaction;
                connection = command.Connection;
            }

                string tempPk = pk;
                deleteEventArgs = new DeleteEventArgs(view, tempPk, command, null, values);
                DataRow deletedRow = GetDataRow(view, pk);
                if (deletedRow == null)
                {
                    try
                    {
                        deletedRow = GetDataRow2(view, pk, command);
                    }
                    catch { }
                }

                if (deletedRow == null)
                {
                    throw new RowNotFoundException(view, pk);
                }
                deleteEventArgs.PrevRow = deletedRow;

                if (beforeDeleteCallback != null)
                    beforeDeleteCallback(this, deleteEventArgs);

                if (deleteEventArgs.Cancel)
                {
                    transaction.Rollback();
                    return affected;
                }

                if (!view.DeleteOverride)
                {
                    pk = deleteEventArgs.PrimaryKey;

                    string sql = "delete from " + sqlTextBuilder.EscapeDbObject("{0}") + " where {1}";
                    sql = string.Format(sql, tableName, GetWhereStatement(view, tableName, true));

                    command.CommandText = sql;
                    command.Parameters.Clear();

                    foreach (IDataParameter parameter in GetWhereParemeters(view, pk))
                    {
                        command.Parameters.Add(GetNewParameter(command, parameter.ParameterName, parameter.Value));
                    }




                    affected = command.ExecuteNonQuery();
                }
                History history = null;

                if (deleteEventArgs.History != null)
                {

                    history = (History)deleteEventArgs.History;

                    if (identicalSystemConnection)
                    {
                        history.SaveDelete(command, view, pk, deleteEventArgs.UserId, view.Database.SwVersion, view.GetWorkspaceName(), deletedRow);
                    }
                }

                deleteEventArgs = new DeleteEventArgs(view, pk, command, null, values);
                deleteEventArgs.PrevRow = deletedRow;

                if (afterDeleteBeforeCommitCallback != null)
                    afterDeleteBeforeCommitCallback(this, deleteEventArgs);

                if (deleteEventArgs.Cancel)
                    transaction.Rollback();
                else
                {
                    if (useCommand == null)
                    {
                        transaction.Commit();
                    }
                    Cache.Clear(view.Name);
                }

                if (!identicalSystemConnection && history != null)
                {
                    if (useSysCommand == null)
                    {
                        IDbConnection sysConnection = GetNewConnection(view.Database.SystemConnectionString);
                        IDbCommand sysCommand = GetNewCommand("", sysConnection);
                        sysCommand.Connection = sysConnection;
                        sysConnection.Open();

                        history.SaveDelete(sysCommand, view, pk, deleteEventArgs.UserId, view.Database.SwVersion, view.GetWorkspaceName(), deletedRow);

                        sysConnection.Close();
                    }
                    else
                    {
                        history.SaveDelete(useSysCommand, view, pk, deleteEventArgs.UserId, view.Database.SwVersion, view.GetWorkspaceName(), deletedRow);

                    }
                }

                if (useCommand == null)
                {
                    connection.Close();
                }


            if (afterDeleteAfterCommitCallback != null)
                afterDeleteAfterCommitCallback(this, deleteEventArgs);

            view.SendRealTimeEvent(pk, Crud.delete);

            return affected;
        }

        public virtual int Edit(View view, Dictionary<string, object> values, string pk, BeforeEditEventHandler beforeEditCallback, BeforeEditInDatabaseEventHandler beforeEditInDatabaseEventHandler, AfterEditEventHandler afterEditBeforeCommitCallback, AfterEditEventHandler afterEditAfterCommitCallback, IDbCommand useCommand, IDbCommand useSysCommand, object history, int? userId)
        {
            ISqlTextBuilder sqlTextBuilder = GetSqlTextBuilder(view);
            IDbCommand sysCommand = null;
            bool identicalSystemConnection = view.Database.IdenticalSystemConnection;
            int affected = 0;

            try
            {
                if (view.Database.Logger != null)
                {
                    view.Database.Logger.Log(view.Name, "Start", "Edit", "DataAccess", "", 10, view.Database.Logger.NowWithMilliseconds(), DateTime.Now);
                }

                if (view.IsGuidIdentity)
                {
                    if (values.ContainsKey(view.DataTable.PrimaryKey[0].ColumnName))
                        values.Remove(view.DataTable.PrimaryKey[0].ColumnName);
                }

                string tableName = GetTableName(view);

                DataRow prevRow = null;

                //if (editEventArgs.LoadPrevRow || view.Derivation != null)
                //{
                if (view.Database.Logger != null)
                {
                    view.Database.Logger.Log(view.Name, "Start", "GetDataRow", "DataAccess", "", 10, view.Database.Logger.NowWithMilliseconds(), DateTime.Now);
                }

                prevRow = view.GetDataRow(pk);

                if (view.Database.Logger != null)
                {
                    view.Database.Logger.Log(view.Name, "End", "GetDataRow", "DataAccess", "", 10, view.Database.Logger.NowWithMilliseconds(), DateTime.Now);
                }
                //}

                if (view.Derivation != null)
                {
                    //prevRow = view.GetDataRow(pk);
                    ValidateDerivation(view, prevRow, values);
                }



                OldNewValue[] oldNewValues = null;

                IDbCommand command;

                if (useCommand != null)
                {
                    command = useCommand;
                    command.CommandType = CommandType.Text;

                    if (identicalSystemConnection)
                    {
                        sysCommand = command;
                    }
                    else if (sysCommand == null)
                    {
                        if (useSysCommand == null)
                        {
                            throw new Exception("Please provide system connection");
                        }
                        else
                        {
                            sysCommand = useSysCommand;
                        }
                    }
                }
                else
                {
                    IDbConnection connection = GetNewConnection(view.ConnectionString);
                    connection.Open();
                    IDbTransaction transaction = connection.BeginTransaction(IsolationLevel.ReadCommitted);
                    command = GetNewCommand("", connection);
                    command.Connection = connection;
                    command.CommandType = CommandType.Text;

                    command.Transaction = transaction;

                    if (identicalSystemConnection)
                    {
                        sysCommand = command;
                    }
                    else
                    {
                        IDbConnection sysConnection = GetNewConnection(view.Database.SystemConnectionString);
                        sysConnection.Open();
                        IDbTransaction sysTransaction = sysConnection.BeginTransaction(IsolationLevel.ReadCommitted);
                        sysCommand = GetNewCommand("", sysConnection);
                        sysCommand.Connection = sysConnection;
                        sysCommand.CommandType = CommandType.Text;

                        sysCommand.Transaction = sysTransaction;
                    }
                }

                EditEventArgs editEventArgs = new EditEventArgs(view, values, pk, command, sysCommand);
                editEventArgs.PrevRow = prevRow;

                HandleCounterFields(values);

                if (beforeEditCallback != null)
                    beforeEditCallback(this, editEventArgs);



                List<string> columnNames = GetUpdateSetColumns(view, values);
                editEventArgs.ColumnNames = columnNames;

                if (beforeEditInDatabaseEventHandler != null)
                    beforeEditInDatabaseEventHandler(this, editEventArgs);


                //string updateSetColumns = GetUpdateSetColumns(view, values);
                string updateSetColumns = GetUpdateSetColumns(columnNames, view);


                string sql = "update " + sqlTextBuilder.EscapeDbObject("{0}") + " set {2} where {1}";

                bool usePermanentFilter = true;
                if (editEventArgs.IgnorePermanentFilter)
                    usePermanentFilter = false;
                sql = string.Format(sql, tableName, GetWhereStatement(view, tableName, usePermanentFilter), updateSetColumns);

                if (editEventArgs.Cancel)
                {
                    if (useCommand == null)
                    {
                        command.Transaction.Rollback();
                        if (!identicalSystemConnection)
                            sysCommand.Transaction.Rollback();
                    }
                    return affected;
                }



                sql = GetOpenCertificatesStatement(view) + sql + GetCloseCertificatesStatement(view);


                command.Parameters.Clear();
                command.CommandType = CommandType.Text;
                command.CommandText = sql;

                
                if (!string.IsNullOrEmpty(updateSetColumns))
                {

                    foreach (IDataParameter parameter in GetParemeters(view, values))
                    {
                        command.Parameters.Add(GetNewParameter(command, parameter.ParameterName, parameter.Value));
                    }

                    foreach (IDataParameter parameter in GetWhereParemeters(view, pk))
                    {
                        command.Parameters.Add(GetNewParameter(command, parameter.ParameterName, parameter.Value));
                    }
                    affected = command.ExecuteNonQuery();

                }


                bool autoIdentity = view.IsAutoIdentity;

                if (autoIdentity && !values.ContainsKey(view.DataTable.PrimaryKey[0].ColumnName))
                {
                    values.Add(view.DataTable.PrimaryKey[0].ColumnName, Convert.ToInt32(pk));
                }


                if (editEventArgs.History != null)
                {
                    history = editEventArgs.History;
                    //History history = (History)editEventArgs.History;

                }

                if (!userId.HasValue)
                {
                    userId = editEventArgs.UserId;
                    //History history = (History)editEventArgs.History;

                }

                if (history != null && view.Database.SwVersion != null)
                {
                    if (view.Database.Logger != null)
                    {
                        view.Database.Logger.Log(view.Name, "Start", "history", "DataAccess", "", 10, view.Database.Logger.NowWithMilliseconds(), DateTime.Now);
                    }
                    DataAccess.History.GetHistory(view.Database.SystemSqlProduct).SaveEdit(sysCommand, view, prevRow, values, pk, userId.Value, out oldNewValues, view.Database.SwVersion, view.GetWorkspaceName());
                    if (view.Database.Logger != null)
                    {
                        view.Database.Logger.Log(view.Name, "End", "history", "DataAccess", "", 10, view.Database.Logger.NowWithMilliseconds(), DateTime.Now);
                    }
                }


                command.Parameters.Clear();

                UpdateCheckLists(view, pk, values, command, sysCommand);


                editEventArgs = new EditEventArgs(view, values, pk, command, sysCommand);
                editEventArgs.PrevRow = prevRow;
                editEventArgs.OldNewValues = oldNewValues;


                if (afterEditBeforeCommitCallback != null)
                    afterEditBeforeCommitCallback(this, editEventArgs);


                if (useCommand == null)
                {
                    if (editEventArgs.Cancel)
                    {
                        command.Transaction.Rollback();
                        if (!identicalSystemConnection)
                            sysCommand.Transaction.Rollback();

                    }
                    else
                    {
                        command.Transaction.Commit();
                        if (!identicalSystemConnection)
                            sysCommand.Transaction.Commit();
                        Cache.Clear(view.Name);
                    }

                }
                if (view.Database.Logger != null)
                {
                    view.Database.Logger.Log(view.Name, "wf after commit start", "Edit", "DataAccess", "", 10, view.Database.Logger.NowWithMilliseconds(), DateTime.Now);
                }

                editEventArgs = new EditEventArgs(view, values, pk, command, sysCommand);
                editEventArgs.PrevRow = prevRow;
                editEventArgs.OldNewValues = oldNewValues;

                if (afterEditAfterCommitCallback != null)
                    afterEditAfterCommitCallback(this, editEventArgs);

                view.SendRealTimeEvent(pk, Crud.update);

                if (view.Database.Logger != null)
                {
                    view.Database.Logger.Log(view.Name, "wf after commit end", "Edit", "DataAccess", "", 10, view.Database.Logger.NowWithMilliseconds(), DateTime.Now);

                    view.Database.Logger.Log(view.Name, "End", "Edit", "DataAccess", "", 10, view.Database.Logger.NowWithMilliseconds(), DateTime.Now);
                }

                return affected;
            }
            finally
            {
                if (!identicalSystemConnection && useSysCommand == null)
                {
                    sysCommand.Connection.Close();
                }
            }
        }

        private void ValidateDerivation(View view, DataRow prevRow, Dictionary<string, object> values)
        {
            if (!values.ContainsKey(view.Derivation.DerivationField))
            {

                ParentField derivationField = view.GetDerivationField();
                if (derivationField != null)
                {
                    string derivationValue = derivationField.GetValue(prevRow);


                    foreach (string fieldName in values.Keys)
                    {
                        if (view.Fields.ContainsKey(fieldName))
                        {
                            Field field = view.Fields[fieldName];

                            if (!view.IsDerivationEditable(field, prevRow))
                            {
                                throw new DerivationViolationException(field, prevRow);
                            }
                        }
                    }
                }
            }
        }

        public virtual void Edit(View view, Dictionary<string, object> values, string pk, BeforeEditEventHandler beforeEditCallback, BeforeEditInDatabaseEventHandler beforeEditInDatabaseEventHandler, AfterEditEventHandler afterEditBeforeCommitCallback, AfterEditEventHandler afterEditAfterCommitCallback)
        {
            Edit(view, values, pk, beforeEditCallback, beforeEditInDatabaseEventHandler, afterEditBeforeCommitCallback, afterEditAfterCommitCallback, null, null, null, null);
        }

        public virtual void Edit(View view, Dictionary<string, object> values, string pk, BeforeEditEventHandler beforeEditCallback, BeforeEditInDatabaseEventHandler beforeEditInDatabaseEventHandler, AfterEditEventHandler afterEditBeforeCommitCallback, AfterEditEventHandler afterEditAfterCommitCallback, object history, int? userID)
        {

            Edit(view, values, pk, beforeEditCallback, beforeEditInDatabaseEventHandler, afterEditBeforeCommitCallback, afterEditAfterCommitCallback, null, null, history, userID);

        }

        private void HandleCounterFields(Dictionary<string, object> values)
        {
            List<string> counterFieldsKeys = values.Keys.Where(key => key.StartsWith("FkCounter_")).ToList();

            foreach (string key in counterFieldsKeys)
            {
                values.Remove(key);
            }
        }

        protected virtual void UpdateCheckLists(View view, string pk, Dictionary<string, object> values, IDbCommand command, IDbCommand sysCommand)
        {
            foreach (ChildrenField field in view.Fields.Values.Where(f => f.FieldType == FieldType.Children && ((ChildrenField)f).Persist))
            {
                if (values.ContainsKey(field.Name) && values[field.Name] != null)
                {
                    UpdateCheckList(field, pk, values[field.Name].ToString(), command, sysCommand);
                }
            }

        }

        protected virtual void UpdateCheckList(ChildrenField childrenField, string pk, string value, IDbCommand command, IDbCommand sysCommand)
        {
            childrenField = (ChildrenField)childrenField.Base;
            View childrenView = childrenField.ChildrenView;
            View view = childrenField.View;
            View parentView = null;
            ParentField parentField = null;
            ParentField fkField = null;

            foreach (ParentField field in childrenView.Fields.Values.Where(f => f.FieldType == FieldType.Parent))
            {
                if (!field.ParentView.Equals(view))
                {
                    parentField = field;
                    parentView = field.ParentView;
                }
                else
                {
                    fkField = field;
                }
            }

            Delete(childrenView, pk, fkField.Name, command);

            string[] values = value.Split(',');

            foreach (string val in values)
            {
                if (!string.IsNullOrEmpty(val))
                {
                    Dictionary<string, object> vals = new Dictionary<string, object>();
                    vals.Add(parentField.Name, val);
                    vals.Add(fkField.Name, pk);

                    int? id = null;
                    Create(childrenView, vals, GetColumnNamesList(childrenView, DataAction.Create, vals), null, command, sysCommand, out id, null, null);
                }
            }

        }

        public virtual DataTable GetDataTable(View view, string fieldName, string value)
        {
            int count = 0;
            Dictionary<string, object> fk = new Dictionary<string, object>();
            fk.Add(fieldName, value);

            Filter filter = GetFilter(view, fk, LogicCondition.And, false, null);

            return FillDataTable(view, 0, 0, filter, null, SortDirection.Asc, out count, null, null, null, null);
        }

        public virtual DataRow GetDataRow2(View view, string pk, IDbCommand command)
        {
            ISqlTextBuilder sqlTextBuilder = GetSqlTextBuilder(view);

            string tableName = view.DataTable.TableName;

            //string sql = "select * from " + sqlTextBuilder.EscapeDbObject("{0}") + " where {1}";
            //sql = string.Format(sql, tableName, GetWhereStatement(view, tableName));
            string sql = GetSelectStatement(view, 1, 1, GetFilter(view, pk.Split(','), false), null, SortDirection.Asc).Replace("@ID ","@pk_id ");

            command.CommandText = sql;

            foreach (IDataParameter parameter in GetWhereParemeters(view, pk))
            {
                if (!command.Parameters.Contains(parameter.ParameterName))
                {
                    command.Parameters.Add(GetNewParameter(command, parameter.ParameterName, parameter.Value));
                }
                else
                {
                    ((IDataParameter)command.Parameters[parameter.ParameterName]).Value = parameter.Value;
                }
            }

            DataTable table = view.DataTable.Copy();

            IDataAdapter adapter = GetNewAdapter(command);

            Fill(adapter, table);

            if (table.Rows.Count == 1)
            {
                return table.Rows[0];
            }
            else
                return null;
        }

        public virtual DataRow GetDataRow(View view, string pk)
        {
            return GetDataRow(view, pk, null);
        }

        public virtual DataRow GetDataRow(View view, string pk, DataSet dataset)
        {
            return GetDataRow(view, pk, dataset, null, null);
        }

        public virtual DataRow GetDataRow(View view, string pk, DataSet dataset, BeforeSelectEventHandler beforeSelectCallback, AfterSelectEventHandler afterSelectCallback, bool usePermanentFilter = false)
        {
            if (string.IsNullOrEmpty(pk))
                return null;
            int totalRowCount = 0;
            Filter filter = GetFilter(view, pk.Split(','), usePermanentFilter);

            DataTable table = FillDataTable(view, 1, 1, filter, null, dataset, SortDirection.Asc, out totalRowCount, null, null, null, beforeSelectCallback, afterSelectCallback);

            if (table.Rows.Count >= 1)
            {
                return table.Rows.Find(view.GetPkValue(pk));
            }
            else
            {
                return null;
            }
        }

        public virtual DataRow GetDataRow(View view, Field field, string key)
        {
            return GetDataRow(view, field, key, true);
        }

        public virtual DataRow GetDataRow(View view, Field field, string key, bool usePermanentFilter)
        {
            int totalRowCount = 0;

            Dictionary<string, object> values = new Dictionary<string, object>();
            values.Add(field.Name, key);
            Filter filter = GetFilter(view, values, false, LogicCondition.And, false, usePermanentFilter, null);

            DataTable table = FillDataTable(view, 1, 1, filter, null, SortDirection.Asc, out totalRowCount, null, null, null, null);

            if (totalRowCount == 1)
            {
                if (table.Rows.Count > 1)
                {

                    DataView dataView = new DataView(table);
                    try
                    {
                        dataView.RowFilter = filter.GetWhereStatementWithoutParameters().Replace(" = N'", " = '").Replace("like N'", "Like '");
                    }
                    catch
                    {
                        return table.Rows[table.Rows.Count - 1];
                    }
                    if (dataView.Count == 1)
                        return dataView[0].Row;
                    else
                        return table.Rows[table.Rows.Count - 1];
                }
                else if (table.Rows.Count == 0)
                    return null;
                else
                    return table.Rows[0];

            }
            else
            {
                return null;
            }
        }

        /***********************/

        private string GetPrimaryKeyColumnsDelimited(View view)
        {
            return GetPrimaryKeyColumnsDelimited(view, null);
        }

        private string GetPrimaryKeyColumnsDelimited(View view, string asSuffix)
        {
            ISqlTextBuilder sqlTextBuilder = GetSqlTextBuilder(view);
            string primaryKeyColumnsDelimited = string.Empty;

            foreach (DataColumn dataColumn in view.DataTable.PrimaryKey)
            {
                primaryKeyColumnsDelimited += sqlTextBuilder.EscapeDbObject(dataColumn.Table.TableName) + sqlTextBuilder.DbTableColumnSeperator + sqlTextBuilder.EscapeDbObject(dataColumn.ColumnName) + (asSuffix == null ? string.Empty : " as " + dataColumn.ColumnName + asSuffix + " ").ToString() + (dataColumn.Equals(view.DataTable.PrimaryKey.Last()) ? ' ' : comma).ToString();
            }

            return primaryKeyColumnsDelimited.TrimEnd(comma);

        }

        private string GetForiegnKeyColumnsWhereStatement(View view, DataRelation dataRelation)
        {
            ISqlTextBuilder sqlTextBuilder = GetSqlTextBuilder(view);
            return GetForiegnKeyColumnsWhereStatement(view, dataRelation, sqlTextBuilder.EscapeDbObject(dataRelation.ChildTable.TableName));
        }

        private string GetForiegnKeyColumnsWhereStatement(View view, DataRelation dataRelation, string fk)
        {
            ISqlTextBuilder sqlTextBuilder = GetSqlTextBuilder(view);
            string foriegnKeyColumnsWhereStatement = string.Empty;

            DataTable childTable = dataRelation.ChildTable;
            DataTable parentTable = dataRelation.ParentTable;

            for (int i = 0; i < dataRelation.ParentColumns.Count(); i++)
            {
                DataColumn childColumn = dataRelation.ChildColumns[i];
                DataColumn parentColumn = dataRelation.ParentColumns[i];
                foriegnKeyColumnsWhereStatement += fk + sqlTextBuilder.DbTableColumnSeperator + sqlTextBuilder.EscapeDbObject(childColumn.ColumnName) + sqlTextBuilder.DbEquals + sqlTextBuilder.EscapeDbObject(parentTable.TableName) + sqlTextBuilder.DbTableColumnSeperator + sqlTextBuilder.EscapeDbObject(parentColumn.ColumnName) + sqlTextBuilder.DbAnd;
            }

            return foriegnKeyColumnsWhereStatement.TrimEnd((sqlTextBuilder.DbAnd).ToCharArray()) + " ";

        }

        public Filter GetFilter(View view, Dictionary<string, object> values)
        {
            return GetFilter(view, values, false, LogicCondition.And, false);
        }

        public string[] GetKeys(View view, ParentField parentField, string fieldName, string fk)
        {
            ISqlTextBuilder sqlTextBuilder = GetSqlTextBuilder(view);

            List<string> keys = new List<string>();

            string select = "select {0} from {1} {2} order by {0}";

            Dictionary<string, object> values = new Dictionary<string, object>();
            values.Add(fieldName, fk);

            Filter filter = GetFilter(view, values);

            select = string.Format(select, GetDelimitedColumnsForSelect(parentField), sqlTextBuilder.EscapeDbObject(view.Name), filter.WhereStatement);

            using (IDbConnection connection = GetNewConnection(view.ConnectionString))
            {
                connection.Open();

                using (IDbCommand command = GetNewCommand(select, connection))
                {
                    foreach (IDataParameter parameter in filter.Parameters)
                    {
                        command.Parameters.Add(GetNewSqlParameter(view, parameter.ParameterName, parameter.Value));
                    }
                    using (IDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            string pk = string.Empty;
                            foreach (DataColumn column in parentField.DataRelation.ChildColumns)
                            {
                                if (!reader.IsDBNull(reader.GetOrdinal(column.ColumnName)))
                                    pk += reader[column.ColumnName].ToString();
                                pk += ",";
                            }
                            pk = pk.TrimEnd(',');

                            keys.Add(pk);
                        }
                    }
                }

            }

            return keys.ToArray();

        }

        public Filter GetFilter(View view, Dictionary<string, object> values, LogicCondition logicCondition, bool insideTextSearch, bool? useLike, Field currentField)
        {
            return GetFilter(view, values, useLike.HasValue ? useLike.Value : view.UseLikeInFilter, logicCondition, insideTextSearch, true, currentField);
        }

        public Filter GetFilter(View view, Dictionary<string, object> values, LogicCondition logicCondition, bool insideTextSearch, bool? useLike)
        {
            return GetFilter(view, values, useLike.HasValue ? useLike.Value : view.UseLikeInFilter, logicCondition, insideTextSearch);
        }

        private string GetColumnName(View view, DataColumn column)
        {
            ISqlTextBuilder sqlTextBuilder = GetSqlTextBuilder(view);
            return sqlTextBuilder.EscapeDbObject(column.Table.TableName) + sqlTextBuilder.DbTableColumnSeperator + sqlTextBuilder.EscapeDbObject(column.ColumnName);
        }

        protected Filter GetFilter(View view, Dictionary<string, object> values, bool useLike, LogicCondition logicCondition, bool insideTextSearch)
        {
            return GetFilter(view, values, useLike, logicCondition, insideTextSearch, true, null);
        }

        private string getComparerString(string str, string prefix, string suffix)
        {
            int beginPos = str.IndexOf(prefix, 0);
            string value = "=";

            if (beginPos > -1)
            {
                int start = beginPos + prefix.Length;
                int end = str.IndexOf(suffix, start);
                if (end > -1)
                {
                    value = str.Substring(start, end - start);
                }
            }

            return value;
        }

        private string WrapToken(string str, string prefix, string suffix)
        {
            return prefix + str + suffix;
        }

        private string removeWrapper(string str, string prefix, string suffix)
        {
            if (str.StartsWith("\""))
            {
                str = str.Substring(1);
            }
            if (str.EndsWith("\""))
            {
                str = str.Substring(0, str.Length - 1);
            }

            return str;
        }

        public Filter GetInFilter(View view, Dictionary<string, object> values, string childrenFieldName)
        {
            Filter filter = GetFilter(view, values);
            ISqlTextBuilder sqlTextBuilder = GetSqlTextBuilder(view);

            string where = "(select " + GetDelimitedColumns(view.GetPkColumnNames().ToList(), view) + " from " + sqlTextBuilder.EscapeDbObject(view.Name) + sqlTextBuilder.WithNolock + " " + filter.WhereStatement + ")";
            for (int i = 0; i < filter.Parameters.Length; i++)
            {
                IDataParameter parameter = filter.Parameters[i];
                string oldName = parameter.ParameterName;
                string newName = "@" + childrenFieldName + oldName.TrimStart('@');
                where = where.Replace(oldName, newName);
                filter.WhereStatement = where;
                parameter.ParameterName = newName;
            }
            return filter;
        }

        protected Filter GetFilter(View view, Dictionary<string, object> values, bool useLike, LogicCondition logicCondition, bool insideTextSearch, bool usePermanentFilter, Field currentField)
        {
            ISqlTextBuilder sqlTextBuilder = GetSqlTextBuilder(view);
            Filter filter = new Filter();

            string permanentFilter = (string.IsNullOrEmpty(view.GetPermanentFilter()) || !usePermanentFilter) ? string.Empty : (sqlTextBuilder.DbAnd + view.GetPermanentFilter());
            filter.WhereStatement = " where 1=1 " + permanentFilter;
            filter.WhereStatementWithoutTablePrefix = string.Empty;

            Dictionary<string, IDataParameter> parameters = new Dictionary<string, IDataParameter>();

            string[] where = GetFkFilter(view, values, parameters, currentField);

            if (where[0].Length > 0 && where[0] != "()")
            {
                filter.WhereStatement += sqlTextBuilder.DbAnd + "(" + where[0] + ") ";
            }
            if (where[1].Length > 0 && where[0] != "()")
            {
                filter.WhereStatementWithoutTablePrefix += " (" + where[1] + ")" + " and ";
            }

            filter.WhereStatement += sqlTextBuilder.DbAnd + "(" + (logicCondition == LogicCondition.And ? " 1=1 " : " 1=0 ") + logicCondition.ToString() + " ";
            filter.WhereStatementWithoutTablePrefix += " (" + (logicCondition == LogicCondition.And ? " 1=1 " : " 1=0 ") + logicCondition.ToString() + " ";

            string likePrefix = insideTextSearch ? "%" : string.Empty;
            if (values != null)
            {
                foreach (string key in values.Keys)
                {
                    bool fieldExists = key != null && view.Fields.ContainsKey(key) && view.Fields[key] != currentField && !(view.Fields[key] is ColumnField && ((ColumnField)view.Fields[key]).IsMilestonesField);

                    //if (!fieldExists)
                    //{
                    //    if (view.DataTable.Columns.Contains(key))
                    //    {
                    //        string columnName = key;
                    //        string parameterName = "@" + key;
                    //        object value = values[key];

                    //        try
                    //        {
                    //            object parameterValue = Convert.ChangeType(value, view.DataTable.Columns[columnName].DataType);
                    //        }
                    //        catch
                    //        {
                    //            continue;
                    //        }

                    //        filter.WhereStatement += sqlTextBuilder.EscapeDbObject(view.DataTable.TableName + "].[" + columnName + "]" + " like N'" + likePrefix + value + "%' " + logicCondition.ToString() + " ";
                    //        filter.WhereStatementWithoutTablePrefix += columnName + " like N'" + likePrefix + value + "%' " + logicCondition.ToString() + " ";
                    //    }

                    //}
                    //else 
                    if (fieldExists && !view.Fields[key].Excluded)
                    {
                        object value = values[key];

                        //Field filterField = view.Fields[key];
                        string fieldFormat = view.Fields[key].Format;

                        if (view.DataTable.Columns.Contains(key))
                        {
                            string columnName = key;
                            string parameterName = sqlTextBuilder.DbParameterPrefix + key.ReplaceNonAlphaNumeric();
                            bool isAdvanced = value != null && value.ToString().Contains(Filter.TOKEN);
                            bool isStringCollumnType = view.Fields[key] is ColumnField && ((ColumnField)view.Fields[key]).DataColumn.DataType.Equals(typeof(string));
                            if (fieldExists && isStringCollumnType && useLike && !isAdvanced)
                            {
                                try
                                {
                                    object parameterValue = Convert.ChangeType(value, view.DataTable.Columns[columnName].DataType);
                                }
                                catch
                                {
                                    continue;
                                }
                                ColumnField columnField = (ColumnField)view.Fields[key];
                                if (columnField.Encrypted)
                                {
                                    //filter.WhereStatement += "CONVERT(NVARCHAR(250), DECRYPTBYKEY(" + columnField.EncryptedName + "))" + " like N'" + likePrefix + value + "%' " + logicCondition.ToString() + " ";
                                    filter.WhereStatement += "CONVERT(NVARCHAR(250), DECRYPTBYKEY(" + columnField.EncryptedName + "))" + " like " + parameterName + " " + logicCondition.ToString() + " ";
                                }
                                else if (columnField.IsCalculated)
                                {
                                    filter.WhereStatement += GetCalculatedFieldStatement(columnField) + " like " + parameterName + " " + logicCondition.ToString() + " ";
                                    filter.WhereStatementWithoutTablePrefix += "[" + columnName + "]" + " like N'" + likePrefix + value + "%' " + logicCondition.ToString() + " ";
                                }
                                else
                                {
                                    //filter.WhereStatement += sqlTextBuilder.EscapeDbObject(view.DataTable.TableName) + sqlTextBuilder.DbTableColumnSeperator + sqlTextBuilder.EscapeDbObject(columnName) + " like N'" + likePrefix + value + "%' " + logicCondition.ToString() + " ";
                                    filter.WhereStatement += sqlTextBuilder.EscapeDbObject(view.DataTable.TableName) + sqlTextBuilder.DbTableColumnSeperator + sqlTextBuilder.EscapeDbObject(columnName) + " like " + parameterName + " " + logicCondition.ToString() + " ";
                                    filter.WhereStatementWithoutTablePrefix += "[" + columnName + "]" + " like N'" + likePrefix + value + "%' " + logicCondition.ToString() + " ";
                                }

                                if (!parameters.ContainsKey(parameterName))
                                {
                                    object parameterValue = Convert.ChangeType(likePrefix + value + "%", view.DataTable.Columns[columnName].DataType);
                                    IDataParameter sqlParameter = GetNewSqlParameter(view, parameterName, parameterValue);
                                    parameters.Add(parameterName, sqlParameter);
                                }
                            }
                            else
                            {
                                string s = value.ToString();
                                bool advanced = s.Contains(Filter.TOKEN);

                                if (fieldExists && advanced)
                                {

                                    //between Feb 01, 2011 to Feb 02, 2011
                                    //> Feb 01, 2011   >= Feb 10, 2011
                                    //TODO add try catch
                                    const string NULL_COMPARER = "[null]";
                                    string first = string.Empty;
                                    string second = string.Empty;
                                    string eq = "=";
                                    string toSeperator = WrapToken("To", Filter.TOKEN, Filter.TOKEN);
                                    string nullComparer = string.Empty;

                                    if (s.Contains(toSeperator))
                                    {
                                        string[] words = Regex.Split(s.Trim(), toSeperator);
                                        eq = getComparerString(words[0], Filter.TOKEN, Filter.TOKEN);
                                        first = words[0].Replace(WrapToken(eq, Filter.TOKEN, Filter.TOKEN), "").Trim();

                                        if (words.Length > 1)
                                        {
                                            second = words[1].Trim();
                                        }
                                    }
                                    else
                                    {
                                        eq = getComparerString(s, Filter.TOKEN, Filter.TOKEN);
                                        first = s.Replace(WrapToken(eq, Filter.TOKEN, Filter.TOKEN), "").Trim();
                                    }

                                    if (eq.Contains(NULL_COMPARER))
                                    {
                                        nullComparer = " or {0} is null or {0} = ''";
                                        eq = eq.Replace(NULL_COMPARER, "");
                                    }

                                    if (eq.Contains("like"))
                                    {
                                        first = removeWrapper(first, "\"", "\"");

                                        //eg. "like%" replaced with "Hello%"
                                        first = eq.Replace("not like", first).Replace("like", first);
                                        eq = eq.Replace("%", "");
                                    }

                                    if (second != string.Empty)
                                    {
                                        if (view.Fields[key].IsNumeric)
                                        {
                                            string parameterName1 = sqlTextBuilder.DbParameterPrefix + key + "_between1";
                                            string parameterName2 = sqlTextBuilder.DbParameterPrefix + key + "_between2";

                                            object parameterValue1 = null;
                                            object parameterValue2 = null;
                                            try
                                            {
                                                parameterValue1 = Field.ChangeType(first, view.DataTable.Columns[columnName].DataType, fieldFormat);
                                            }
                                            catch
                                            {
                                                continue;
                                            }
                                            try
                                            {
                                                parameterValue2 = Field.ChangeType(second, view.DataTable.Columns[columnName].DataType, fieldFormat);
                                            }
                                            catch
                                            {
                                                continue;
                                            }
                                            if (view.Fields.ContainsKey(columnName) && view.Fields[columnName].IsCalculated)
                                            {
                                                filter.WhereStatement += GetCalculatedFieldStatement(view.Fields[columnName]) + " >= " + parameterName1 + " and ";
                                                filter.WhereStatement += GetCalculatedFieldStatement(view.Fields[columnName]) + " <= " + parameterName2 + ") " + logicCondition.ToString() + " ";
                                            }
                                            else
                                            {
                                                filter.WhereStatement += "(" + sqlTextBuilder.EscapeDbObject(view.DataTable.TableName) + sqlTextBuilder.DbTableColumnSeperator + sqlTextBuilder.EscapeDbObject(columnName) + " >= " + parameterName1 + " and ";
                                                filter.WhereStatement += sqlTextBuilder.EscapeDbObject(view.DataTable.TableName) + sqlTextBuilder.DbTableColumnSeperator + sqlTextBuilder.EscapeDbObject(columnName) + " <= " + parameterName2 + ") " + logicCondition.ToString() + " ";
                                            }

                                            if (view.Fields.ContainsKey(columnName) && view.Fields[columnName].IsCalculated)
                                            {
                                                filter.WhereStatement += GetCalculatedFieldStatement(view.Fields[columnName]) + " >= " + parameterName1 + sqlTextBuilder.DbAnd;
                                                filter.WhereStatement += GetCalculatedFieldStatement(view.Fields[columnName]) + " <= " + parameterName2 + ") " + logicCondition.ToString() + " ";
                                            }
                                            else
                                            {
                                                filter.WhereStatement += "(" + sqlTextBuilder.EscapeDbObject(view.DataTable.TableName) + sqlTextBuilder.DbTableColumnSeperator + sqlTextBuilder.EscapeDbObject(columnName) + " >= " + parameterName1 + sqlTextBuilder.DbAnd;
                                                filter.WhereStatement += sqlTextBuilder.EscapeDbObject(view.DataTable.TableName) + sqlTextBuilder.DbTableColumnSeperator + sqlTextBuilder.EscapeDbObject(columnName) + " <= " + parameterName2 + ") " + logicCondition.ToString() + " ";
                                            }

                                            filter.WhereStatementWithoutTablePrefix += "([" + columnName + "] >= " + parameterName1 + sqlTextBuilder.DbAnd;
                                            filter.WhereStatementWithoutTablePrefix += "[" + columnName + "]" + " <= " + parameterName2 + ") " + logicCondition.ToString() + " ";
                                            if (!parameters.ContainsKey(parameterName))
                                            {
                                                parameters.Add(parameterName1, GetNewSqlParameter(view, parameterName1, parameterValue1));
                                                parameters.Add(parameterName2, GetNewSqlParameter(view, parameterName2, parameterValue2));
                                            }
                                        }
                                        else if (view.Fields[key].GetColumnFieldType() == ColumnFieldType.DateTime)
                                        {
                                            string parameterName1 = sqlTextBuilder.DbParameterPrefix + key + "_between1";
                                            string parameterName2 = sqlTextBuilder.DbParameterPrefix + key + "_between2";
                                            DateType dateType = DateFormatsMapper.GetDateType(view.Fields[key].Format);
                                            //string dateComparison = dateType == DateType.Date
                                            //    ? "dateadd(dd,datediff(dd,0,{0}),0)" : "{0}";
                                            string dateComparison = dateType == DateType.Date
                                               ? sqlTextBuilder.GetDateOnly("{0}") : "{0}";
                                            object parameterValue1 = null;

                                            try
                                            {
                                                parameterValue1 = Field.ChangeType(first, view.DataTable.Columns[columnName].DataType, fieldFormat);
                                            }
                                            catch
                                            {
                                                continue;
                                            }
                                            object parameterValue2 = null;
                                            try
                                            {
                                                parameterValue2 = Field.ChangeType(second, view.DataTable.Columns[columnName].DataType, fieldFormat);
                                            }
                                            catch
                                            {
                                                continue;
                                            }
                                            string fieldReference = string.Empty;
                                            if (view.Fields.ContainsKey(columnName) && view.Fields[columnName].IsCalculated)
                                            {
                                                fieldReference = GetCalculatedFieldStatement(view.Fields[columnName]);
                                            }
                                            else
                                            {
                                                fieldReference = sqlTextBuilder.EscapeDbObject(view.DataTable.TableName) + sqlTextBuilder.DbTableColumnSeperator + sqlTextBuilder.EscapeDbObject(columnName);
                                            }
                                            filter.WhereStatement += " (" + string.Format(dateComparison, fieldReference) + " >= " + parameterName1 + " and ";
                                            filter.WhereStatement += string.Format(dateComparison, fieldReference) + " <= " + parameterName2 + ") " + logicCondition.ToString() + " ";

                                            filter.WhereStatementWithoutTablePrefix += "(" + string.Format(dateComparison, columnName) + " >= " + parameterName1 + " and ";
                                            filter.WhereStatementWithoutTablePrefix += string.Format(dateComparison, columnName) + " <= " + parameterName2 + ") " + logicCondition.ToString() + " ";
                                            filter.WhereStatementWithoutTablePrefix += " dateadd(dd,datediff(dd,0,[" + columnName + "]),0)" + " <= " + parameterName2 + ") " + logicCondition.ToString() + " ";
                                            if (!parameters.ContainsKey(parameterName))
                                            {
                                                parameters.Add(parameterName1, GetNewSqlParameter(view, parameterName1, parameterValue1));
                                                parameters.Add(parameterName2, GetNewSqlParameter(view, parameterName2, parameterValue2));
                                            }
                                        }
                                    }
                                    else if (view.Fields[key].GetColumnFieldType() == ColumnFieldType.DateTime)
                                    {
                                        string whereStatementFormat = string.Empty;
                                        string fieldReference = string.Empty;
                                        DateType dateType = DateFormatsMapper.GetDateType(view.Fields[key].Format);
                                        //string dateComparison = dateType == DateType.Date
                                        //    ? "dateadd(dd,datediff(dd,0,{0}),0)" : "{0}";
                                        string dateComparison = dateType == DateType.Date
                                            ? sqlTextBuilder.GetDateOnly("{0}") : "{0}";

                                        if (!eq.Contains("empty"))
                                        {
                                            DateTime parameterValue = DateTime.Now;
                                            try
                                            {
                                                parameterValue = ((DateTime)Field.ChangeType(first, view.DataTable.Columns[columnName].DataType, fieldFormat));
                                            }
                                            catch
                                            {
                                                continue;
                                            }

                                            if (!parameters.ContainsKey(parameterName))
                                            {
                                                parameters.Add(parameterName, GetNewSqlParameter(view, parameterName, parameterValue));
                                            }

                                            whereStatementFormat = " " + dateComparison + eq + parameterName;
                                        }
                                        else
                                        {
                                            whereStatementFormat = eq == "empty" ? Filter.EMPTY_WHERE_STATEMENT : isStringCollumnType ?
                                                Filter.STRING_NOT_EMPTY_WHERE_STATEMENT : Filter.NOT_EMPTY_WHERE_STATEMENT;
                                        }
                                        whereStatementFormat += " " + logicCondition.ToString() + " ";

                                        if (view.Fields.ContainsKey(columnName) && view.Fields[columnName].IsCalculated)
                                        {
                                            fieldReference = GetCalculatedFieldStatement(view.Fields[columnName]);
                                        }
                                        else
                                        {
                                            fieldReference = sqlTextBuilder.EscapeDbObject(view.DataTable.TableName) + sqlTextBuilder.DbTableColumnSeperator + sqlTextBuilder.EscapeDbObject(columnName);// "[" + view.DataTable.TableName + "].[" + columnName + "]";
                                        }
                                        filter.WhereStatement += string.Format(whereStatementFormat, fieldReference);

                                        fieldReference = "[" + columnName + "]";
                                        filter.WhereStatementWithoutTablePrefix += string.Format(whereStatementFormat, fieldReference);
                                    }
                                    else
                                    {
                                        string whereStatementFormat = string.Empty;
                                        string fieldReference = string.Empty;

                                        if (!eq.Contains("empty"))
                                        {
                                            object parameterValue = null;
                                            try
                                            {
                                                first = removeWrapper(first, "\"", "\"");
                                                if (eq == "not like")
                                                    first = first.TrimStart("not ".ToCharArray());

                                                parameterValue = Field.ChangeType(first, view.DataTable.Columns[columnName].DataType, fieldFormat);
                                            }
                                            catch
                                            {
                                                continue;
                                            }
                                            if (!parameters.ContainsKey(parameterName))
                                            {
                                                parameters.Add(parameterName, GetNewSqlParameter(view, parameterName, parameterValue));
                                            }
                                            whereStatementFormat = "({0} " + eq + " " + parameterName + "{1})";

                                        }
                                        else
                                        {
                                            whereStatementFormat = eq == "empty" ? Filter.EMPTY_WHERE_STATEMENT : Filter.NOT_EMPTY_WHERE_STATEMENT;
                                        }
                                        whereStatementFormat += " " + logicCondition.ToString() + " ";

                                        if (view.Fields.ContainsKey(columnName) && view.Fields[columnName].IsCalculated)
                                        {
                                            fieldReference = GetCalculatedFieldStatement(view.Fields[columnName]);
                                        }
                                        else
                                        {
                                            fieldReference = sqlTextBuilder.EscapeDbObject(view.DataTable.TableName) + sqlTextBuilder.DbTableColumnSeperator + sqlTextBuilder.EscapeDbObject(columnName);// "[" + view.DataTable.TableName + "].[" + columnName + "]";
                                        }
                                        filter.WhereStatement += string.Format(whereStatementFormat, fieldReference, string.Format(nullComparer, fieldReference));

                                        fieldReference = "[" + columnName + "]";
                                        filter.WhereStatementWithoutTablePrefix += string.Format(whereStatementFormat, fieldReference, string.Format(nullComparer, fieldReference));
                                    }
                                }
                                else if (fieldExists && view.Fields[key].GetColumnFieldType() == ColumnFieldType.DateTime)
                                {
                                    //object parameterValue = s;
                                    if (view.Fields.ContainsKey(columnName) && view.Fields[columnName].IsCalculated)
                                    {
                                        filter.WhereStatement += "(" + sqlTextBuilder.GetConvertDateToVarcharStatement(GetCalculatedFieldStatement(view.Fields[columnName]), sqlTextBuilder.mmddyyyy) + " like " + parameterName + " or " + sqlTextBuilder.GetConvertDateToVarcharStatement(GetCalculatedFieldStatement(view.Fields[columnName]), sqlTextBuilder.monddyyyy) + " like " + parameterName + ") " + logicCondition.ToString() + " ";
                                    }
                                    else
                                    {
                                        filter.WhereStatement += "(" + sqlTextBuilder.GetConvertDateToVarcharStatement(sqlTextBuilder.EscapeDbObject(view.DataTable.TableName) + sqlTextBuilder.DbTableColumnSeperator + sqlTextBuilder.EscapeDbObject(columnName), sqlTextBuilder.mmddyyyy) + " like " + parameterName + " or " + sqlTextBuilder.GetConvertDateToVarcharStatement(sqlTextBuilder.EscapeDbObject(view.DataTable.TableName) + sqlTextBuilder.DbTableColumnSeperator + sqlTextBuilder.EscapeDbObject(columnName), sqlTextBuilder.monddyyyy) + " like " + parameterName + ") " + logicCondition.ToString() + " ";
                                    }

                                    if (!parameters.ContainsKey(parameterName))
                                    {
                                        object parameterValue = s;
                                            
                                        parameters.Add(parameterName, GetNewSqlParameter(view, parameterName, parameterValue));
                                    }

                                    //filter.WhereStatementWithoutTablePrefix += columnName + " = " + parameterName + " " + logicCondition.ToString() + " ";
                                    //if (!parameters.ContainsKey(parameterName))
                                    //{
                                    //    parameters.Add(parameterName, new System.Data.SqlClient.SqlParameter(parameterName, parameterValue));
                                    //}
                                }
                                else
                                {
                                    object parameterValue = null;
                                    try
                                    {
                                        parameterValue = Field.ChangeType(s, view.DataTable.Columns[columnName].DataType, fieldFormat);
                                    }
                                    catch
                                    {
                                        continue;
                                    }
                                    if (view.Fields.ContainsKey(columnName) && view.Fields[columnName].IsCalculated)
                                    {
                                        filter.WhereStatement += GetCalculatedFieldStatement(view.Fields[columnName]) + " = " + parameterName + " " + logicCondition.ToString() + " ";
                                    }
                                    else
                                    {
                                        filter.WhereStatement += sqlTextBuilder.EscapeDbObject(view.DataTable.TableName) + sqlTextBuilder.DbTableColumnSeperator + sqlTextBuilder.EscapeDbObject(columnName) + sqlTextBuilder.DbEquals + parameterName + " " + logicCondition.ToString() + " ";
                                        //filter.WhereStatement += "[" + view.DataTable.TableName + "].[" + columnName + "]" + " = " + parameterName + " " + logicCondition.ToString() + " ";
                                    }
                                    filter.WhereStatementWithoutTablePrefix += "[" + columnName + "]" + " = " + parameterName + " " + logicCondition.ToString() + " ";
                                    if (!parameters.ContainsKey(parameterName))
                                    {
										parameters.Add(parameterName, GetNewSqlParameter(view, parameterName, parameterValue));
                                    }
                                }
                            }
                        }
                        else if (fieldExists)
                        {
                            if (view.Fields[key].FieldType == FieldType.Parent)
                            {
                                Field field = view.Fields[key];
                                int i = 0;
                                string[] fk = value.ToString().Split(comma);
                                int fkLength = fk.Length;
                                DataColumn[] fkColumns = ((ParentField)field).DataRelation.ChildColumns;
                                int fkColumnsLength = fkColumns.Length;
                                if (value.ToString() != Durados.Database.EmptyString)
                                {
                                    foreach (DataColumn column in fkColumns)
                                    {
                                        string columnName = column.ColumnName;
                                        string parameterName =  sqlTextBuilder.DbParameterPrefix +"filter_" + GetVarFromName(columnName);
                                        if (!parameters.ContainsKey(parameterName))
                                        {
                                            object parameterValue = null;
                                            try
                                            {
                                                parameterValue = Field.ChangeType(fk[i], view.DataTable.Columns[columnName].DataType, fieldFormat);
                                            }
                                            catch
                                            {
                                                continue;
                                            }
                                            if (view.Fields.ContainsKey(columnName) && view.Fields[columnName].IsCalculated)
                                            {
                                                filter.WhereStatement += GetCalculatedFieldStatement(view.Fields[columnName]) + " = " + parameterName + " " + logicCondition.ToString() + " ";
                                            }
                                            else
                                            {
                                                filter.WhereStatement += sqlTextBuilder.EscapeDbObject(view.DataTable.TableName) + sqlTextBuilder.DbTableColumnSeperator + sqlTextBuilder.EscapeDbObject(columnName) + sqlTextBuilder.DbEquals + parameterName + " " + logicCondition.ToString() + " ";
                                                //filter.WhereStatement += "[" + view.DataTable.TableName + "].[" + columnName + "]" + " = " + parameterName + " " + logicCondition.ToString() + " ";
                                            }
                                            filter.WhereStatementWithoutTablePrefix += "[" + columnName + "]" + " = " + parameterName + " " + logicCondition.ToString() + " ";
                                            parameters.Add(parameterName, GetNewSqlParameter(view, parameterName, parameterValue));
                                        }
                                        i++;
                                    }
                                }
                                else
                                {
                                    foreach (DataColumn column in fkColumns)
                                    {
                                        string columnName = column.ColumnName;
                                        string parameterName =  sqlTextBuilder.DbParameterPrefix + "filter_" + GetVarFromName(columnName);
                                        if (!parameters.ContainsKey(parameterName))
                                        {
                                            object parameterValue = DBNull.Value;
                                            if (view.Fields.ContainsKey(columnName) && view.Fields[columnName].IsCalculated)
                                            {
                                                filter.WhereStatement += GetCalculatedFieldStatement(view.Fields[columnName]) + " is null " + logicCondition.ToString() + " ";
                                            }
                                            else
                                            {
                                                filter.WhereStatement += sqlTextBuilder.EscapeDbObject(view.DataTable.TableName) + sqlTextBuilder.DbTableColumnSeperator + sqlTextBuilder.EscapeDbObject(columnName) + " is null " + logicCondition.ToString() + " ";
                                                //filter.WhereStatement += "[" + view.DataTable.TableName + "].[" + columnName + "]" + " is null " + logicCondition.ToString() + " ";
                                            }
                                            filter.WhereStatementWithoutTablePrefix += "[" + columnName + "]" + " = is null " + logicCondition.ToString() + " ";
                                        }
                                        i++;
                                    }
                                }
                            }
                            else if (view.Fields[key].FieldType == FieldType.Children)
                            {
                                string[] children = value.ToString().Split(comma);
                                if (children.Length > 0)
                                {
                                    //EXISTS (select id from child join view where view.pk=id and (child.id = value1 or child.id = value 2...)
                                    ChildrenField field = (ChildrenField)view.Fields[key];
                                    View childrenView = field.ChildrenView;

                                    //string exists = " EXISTS (select * from [" + childrenView.DataTable.TableName + "] with (nolock) where ";
                                    ParentField parentField = field.GetEquivalentParentField();
                                    //DataColumn[] fkColumns = parentField.DataRelation.ChildColumns;
                                    //DataColumn[] pkColumns = parentField.DataRelation.ParentColumns;
                                    //int fkColumnsLength = fkColumns.Length;
                                    //for (int i = 0; i < fkColumns.Length; i++)
                                    //{
                                    //    DataColumn fkColumn = fkColumns[i];
                                    //    string fkColumnName = fkColumn.ColumnName;
                                    //    DataColumn pkColumn = pkColumns[i];
                                    //    string pkColumnName = pkColumn.ColumnName;

                                    //    exists += sqlTextBuilder.EscapeDbObject(view.DataTable.TableName + "].[" + pkColumnName + "]" + " = [" + fkColumnName + "] and ";

                                    //}
                                    string exists = string.Empty;

                                    if (children.Contains(Database.EmptyString))
                                    {
                                        exists = GetChildrenExistStatement(field, parentField);
                                        exists = exists.TrimEnd("and ".ToCharArray());
                                        exists = " not " + exists;
                                        exists += ")";
                                        //filter.WhereStatement += exists + " " + logicCondition.ToString() + " ";
                                        //filter.WhereStatementWithoutTablePrefix += exists + " " + logicCondition.ToString() + " ";
                                        filter.WhereStatement += exists;
                                        filter.WhereStatementWithoutTablePrefix += exists;

                                        if (children.Length > 1)
                                        {
                                            filter.WhereStatement += " or ";
                                            filter.WhereStatementWithoutTablePrefix += " or ";
                                        }
                                        else
                                        {
                                            filter.WhereStatement += " " + logicCondition.ToString() + " ";
                                            filter.WhereStatementWithoutTablePrefix += " " + logicCondition.ToString() + " ";
                                        }
                                    }
                                    //else
                                    //{
                                    if (children.Length > 1 || children[0] != Database.EmptyString)
                                    {
                                        exists = GetChildrenExistStatement(field, parentField);

                                        ParentField other = (ParentField)childrenView.Fields.Values.Where(f => f.FieldType == FieldType.Parent && f.Name != parentField.Name).First();

                                        exists += "(";


                                        int k = 0;
                                        string columnName = other.DataRelation.ChildColumns[0].ColumnName;

                                        foreach (string child in children)
                                        {
                                            if (child != Database.EmptyString && !string.IsNullOrEmpty(child))
                                            {
                                                string parameterName =  sqlTextBuilder.DbParameterPrefix +"filter_child" + columnName + k;
                                                exists += sqlTextBuilder.EscapeDbObject(columnName) + sqlTextBuilder.DbEquals + parameterName + sqlTextBuilder.DbOr;
                                                parameters.Add(parameterName, GetNewSqlParameter(childrenView, parameterName, Int32.Parse(child)));

                                                k++;
                                            }
                                        }
                                        exists = exists.TrimEnd(" or ".ToCharArray());

                                        exists += "))";

                                        filter.WhereStatement += exists + " " + logicCondition.ToString() + " ";
                                        filter.WhereStatementWithoutTablePrefix += exists + " " + logicCondition.ToString() + " ";
                                    }
                                    //}
                                }
                            }
                        }
                    }
                }
            }

            int logicConditionLength = (2 + logicCondition.ToString().Length);
            filter.WhereStatement = filter.WhereStatement.Remove(filter.WhereStatement.Length - logicConditionLength);
            filter.WhereStatement += ")";
            if (filter.WhereStatementWithoutTablePrefix.Length > logicConditionLength)
                filter.WhereStatementWithoutTablePrefix = filter.WhereStatementWithoutTablePrefix.Remove(filter.WhereStatementWithoutTablePrefix.Length - logicConditionLength);
            filter.WhereStatementWithoutTablePrefix += ")";
            filter.Parameters = parameters.Values.ToArray();
            return filter;
        }

        private string GetVarFromName(string name)
        {
            return name.ReplaceNonAlphaNumeric();
        }

        public string GetChildrenExistStatement(ChildrenField field, ParentField parentField)
        {
            ISqlTextBuilder sqlTextBuilder = GetSqlTextBuilder(field.View);

            View childrenView = field.ChildrenView;

            string exists = " EXISTS (select * from " + sqlTextBuilder.EscapeDbObject(childrenView.DataTable.TableName) + sqlTextBuilder.WithNolock + " where ";
            //ParentField parentField = field.GetEquivalentParentField();
            DataColumn[] fkColumns = parentField.DataRelation.ChildColumns;
            DataColumn[] pkColumns = parentField.DataRelation.ParentColumns;
            int fkColumnsLength = fkColumns.Length;
            for (int i = 0; i < fkColumns.Length; i++)
            {
                DataColumn fkColumn = fkColumns[i];
                string fkColumnName = fkColumn.ColumnName;
                DataColumn pkColumn = pkColumns[i];
                string pkColumnName = pkColumn.ColumnName;

                exists += sqlTextBuilder.EscapeDbObject(field.View.DataTable.TableName) + sqlTextBuilder.DbTableColumnSeperator + sqlTextBuilder.EscapeDbObject(pkColumnName) + sqlTextBuilder.DbEquals + sqlTextBuilder.EscapeDbObject(fkColumnName) + sqlTextBuilder.DbAnd;

            }

            return exists;
        }

        protected string[] GetFkFilter(View view, Dictionary<string, object> values, Dictionary<string, IDataParameter> parameters, Field currentField)
        {
            ISqlTextBuilder sqlTextBuilder = GetSqlTextBuilder(view);

            string whereStatement = string.Empty;
            string whereStatementWithoutTablePrefix = string.Empty;

            LogicCondition logicCondition = LogicCondition.And;

            List<string> keys = new List<string>();

            if (values != null)
            {
                foreach (string key in values.Keys)
                {
                    //br todo
                    if (key != null && (currentField == null || !view.Fields.ContainsKey(key) || view.Fields[key] != currentField))
                    {
                        object pkValue = values[key];
                        if (view.DataTable.Columns.Contains(key) && !view.Fields.ContainsKey(key))
                        {
                            string columnName = key;
                            string parameterName = sqlTextBuilder.DbParameterPrefix + GetVarFromName(key);
                            object parameterValue;
                            if (pkValue != null && pkValue.ToString().EndsWith("#"))
                            {
                                if (!view.DataTable.Columns[columnName].DataType.Equals(typeof(string)))
                                {
                                    pkValue = pkValue.ToString().TrimEnd('#');
                                }
                            }
                            if (view.DataTable.Columns[columnName].DataType.Equals(typeof(Guid)))
                            {
                                parameterValue = new Guid(pkValue.ToString());
                            }
                            else
                            {
                                parameterValue = Convert.ChangeType(pkValue, view.DataTable.Columns[columnName].DataType);
                            }
                            //object parameterValue = Convert.ChangeType(values[key], view.DataTable.Columns[columnName].DataType);

                            whereStatement += sqlTextBuilder.EscapeDbObject(view.DataTable.TableName) + sqlTextBuilder.DbTableColumnSeperator + sqlTextBuilder.EscapeDbObject(columnName) + sqlTextBuilder.DbEquals + parameterName + " " + logicCondition.ToString() + " ";
                            whereStatementWithoutTablePrefix += "[" + columnName + "] = " + parameterName + " " + logicCondition.ToString() + " ";
                            if (!parameters.ContainsKey(parameterName))
                            {
                                parameters.Add(parameterName, GetNewSqlParameter(view, parameterName, parameterValue));
                            }

                            keys.Add(key);
                        }
                        else if (view.Fields.ContainsKey(key) && view.Fields[key].FieldType == FieldType.Parent)
                        {
                            Field field = view.Fields[key];
                            int i = 0;
                            string[] fk = pkValue.ToString().Split(comma);

                            if (fk.Count() > 0)
                            {
                                if (fk[0] == string.Empty)
                                {
                                    List<string> list = new List<string>(fk);
                                    list.RemoveAt(0);
                                    fk = list.ToArray();
                                }
                            }

                            int fkLength = fk.Length;
                            DataColumn[] fkColumns = ((ParentField)field).DataRelation.ChildColumns;
                            int fkColumnsLength = fkColumns.Length;
                            //if (fkLength == fkColumnsLength)
                            //{
                            //    foreach (DataColumn column in ((ParentField)field).DataRelation.ChildColumns)
                            //    {
                            //        string columnName = column.ColumnName;
                            //        string parameterName = "@filter_" + columnName;
                            //        if (!parameters.ContainsKey(parameterName) && fk[i] != string.Empty)
                            //        {
                            //            object parameterValue = Convert.ChangeType(fk[i], view.DataTable.Columns[columnName].DataType);
                            //            whereStatement += sqlTextBuilder.EscapeDbObject(view.DataTable.TableName + "].[" + columnName + "]" + " = " + parameterName + " " + logicCondition.ToString() + " ";
                            //            whereStatementWithoutTablePrefix += columnName + " = " + parameterName + " " + logicCondition.ToString() + " ";
                            //            parameters.Add(parameterName, new System.Data.SqlClient.SqlParameter(parameterName, parameterValue));
                            //        }
                            //        i++;
                            //    }
                            //}
                            //else
                            //{
                            List<List<string>> multiValues = new List<List<string>>();

                            List<string> l = new List<string>();
                            for (int j = 0; j < fkLength; j++)
                            {
                                if (j % fkColumnsLength == 0)
                                {

                                    l = new List<string>();
                                    multiValues.Add(l);
                                }
                                l.Add(fk[j]);
                            }

                            whereStatement += "(";
                            whereStatementWithoutTablePrefix += "(";

                            int k = 1;
                            foreach (List<string> vs in multiValues)
                            {
                                //whereStatement += "(";
                                //whereStatementWithoutTablePrefix += "(";

                                foreach (DataColumn column in fkColumns)
                                {
                                    string columnName = column.ColumnName;
                                    string parameterName =  sqlTextBuilder.DbParameterPrefix +"filter_" + GetVarFromName(columnName) + k.ToString();
                                    k++;
                                    if (!parameters.ContainsKey(parameterName) && fk[i] != string.Empty)
                                    {
                                        if (fk[i] == Database.EmptyString)
                                        {
                                            whereStatement += sqlTextBuilder.EscapeDbObject(view.DataTable.TableName) + sqlTextBuilder.DbTableColumnSeperator + sqlTextBuilder.EscapeDbObject(columnName) + " is null " + LogicCondition.And.ToString() + " ";
                                            whereStatementWithoutTablePrefix += "[" + columnName + "]" + " is null " + LogicCondition.And.ToString() + " ";
                                        }
                                        else if (values[field.Name] is Filter)
                                        {
                                            Filter filter = (Filter)values[field.Name];
                                            whereStatement += sqlTextBuilder.EscapeDbObject(view.DataTable.TableName) + sqlTextBuilder.DbTableColumnSeperator + sqlTextBuilder.EscapeDbObject(columnName) + " in " + filter.WhereStatement + " " + LogicCondition.And.ToString() + " ";
                                            whereStatementWithoutTablePrefix += "[" + columnName + "]" + " in " + filter.WhereStatement + " " + LogicCondition.And.ToString() + " ";
                                            foreach (IDataParameter parameter in filter.Parameters)
                                            {
                                                parameters.Add(parameter.ParameterName, GetNewSqlParameter(view, parameter.ParameterName, parameter.Value));
                                            }
                                        }
                                        else if (!fk[i].Contains(Filter.TOKEN))
                                        {
                                            object parameterValue;
                                            string fkValue = fk[i];
                                            if (fkValue != null && fkValue.ToString().EndsWith("#"))
                                            {
                                                if (!view.DataTable.Columns[columnName].DataType.Equals(typeof(string)))
                                                {
                                                    fkValue = fkValue.ToString().TrimEnd('#');
                                                }
                                            }
                                            if (view.DataTable.Columns[columnName].DataType.Equals(typeof(Guid)))
                                            {
                                                parameterValue = new Guid(fkValue);
                                            }
                                            else
                                            {
                                                parameterValue = Convert.ChangeType(fkValue.Trim(), view.DataTable.Columns[columnName].DataType);
                                            }

                                            whereStatement += sqlTextBuilder.EscapeDbObject(view.DataTable.TableName) + sqlTextBuilder.DbTableColumnSeperator + sqlTextBuilder.EscapeDbObject(columnName) + sqlTextBuilder.DbEquals + parameterName + " " + LogicCondition.And.ToString() + " ";
                                            whereStatementWithoutTablePrefix += "[" + columnName + "]" + " = " + parameterName + " " + LogicCondition.And.ToString() + " ";
                                            parameters.Add(parameterName, GetNewSqlParameter(view, parameterName, parameterValue));
                                        }
                                        else // just fill statement to remove it in two lines
                                        {
                                            string currentStatement = " " + LogicCondition.And.ToString() + " ";
                                            whereStatement += currentStatement;
                                            whereStatementWithoutTablePrefix += currentStatement;
                                        }
                                    }
                                    i++;
                                }

                                int andLength = (2 + LogicCondition.And.ToString().Length);
                                whereStatement = whereStatement.Remove(whereStatement.Length - andLength);
                                whereStatementWithoutTablePrefix = whereStatementWithoutTablePrefix.Remove(whereStatementWithoutTablePrefix.Length - andLength);

                                //whereStatement += ")";
                                //whereStatementWithoutTablePrefix += ")";

                                whereStatement += " " + LogicCondition.Or + " ";
                                whereStatementWithoutTablePrefix += " " + LogicCondition.Or + " ";

                            }

                            int orLength = (2 + LogicCondition.Or.ToString().Length);
                            whereStatement = whereStatement.Remove(whereStatement.Length - orLength);
                            whereStatementWithoutTablePrefix = whereStatementWithoutTablePrefix.Remove(whereStatementWithoutTablePrefix.Length - orLength);

                            whereStatement += ")";
                            whereStatementWithoutTablePrefix += ")";

                            whereStatement += " " + LogicCondition.And + " ";
                            whereStatementWithoutTablePrefix += " " + LogicCondition.And + " ";



                            //}
                            keys.Add(key);
                        }
                    }
                }
            }

            foreach (string key in keys)
            {
                values.Remove(key);
            }

            int logicConditionLength = (2 + logicCondition.ToString().Length);
            if (whereStatement.Length > logicConditionLength)
                whereStatement = whereStatement.Remove(whereStatement.Length - logicConditionLength);
            if (whereStatementWithoutTablePrefix.Length > logicConditionLength)
                whereStatementWithoutTablePrefix = whereStatementWithoutTablePrefix.Remove(whereStatementWithoutTablePrefix.Length - logicConditionLength);


            string[] where = new string[2] { whereStatement, whereStatementWithoutTablePrefix };

            return where;
        }


        private string GetPrimaryKeyJoin(View view, string pk)
        {
            ISqlTextBuilder sqlTextBuilder = GetSqlTextBuilder(view);
            string primaryKeyColumnsDelimited = string.Empty;

            if (view.DataTable.PrimaryKey == null || view.DataTable.PrimaryKey.Length == 0)
                throw new DuradosException("The View " + view.DisplayName + " has no primary key.");

            foreach (DataColumn dataColumn in view.DataTable.PrimaryKey)
            {
                primaryKeyColumnsDelimited += sqlTextBuilder.EscapeDbObject(view.DataTable.TableName) + sqlTextBuilder.DbTableColumnSeperator + sqlTextBuilder.EscapeDbObject(dataColumn.ColumnName) + sqlTextBuilder.DbEquals + pk + sqlTextBuilder.DbTableColumnSeperator + sqlTextBuilder.EscapeDbObject(dataColumn.ColumnName) + sqlTextBuilder.DbAnd;
            }

            return primaryKeyColumnsDelimited.TrimEnd((sqlTextBuilder.DbAnd).ToCharArray()) + " ";
        }









        private object[] GetKeys(View view, string pk)
        {
            List<object> keys = new List<object>();
            string[] values = pk.Split(comma);
            for (int i = 0; i < values.Length; i++)
            {
                DataColumn column = view.DataTable.PrimaryKey[i];
                object key = Convert.ChangeType(values[i], column.DataType);
                keys.Add(key);
            }

            return keys.ToArray();
        }

        public List<string> GetColumnNamesList(View view, DataAction dataAction, Dictionary<string, object> values)
        {
            List<string> columnNamesList = new List<string>();
            //foreach (Field field in view.Fields.Values.Where(f=>f.IsExcluded(dataAction)==false))
            HashSet<string> columnNames = new HashSet<string>();

            foreach (Field field in view.Fields.Values.Where(f => f.IsExcluded(dataAction) == false && f.IsAllow(dataAction) && f.IsDerivationEditable(values)))
            {
                string columnName = null;
                if (values.ContainsKey(field.Name))
                {
                    if (field is ColumnField)
                    {
                        ColumnField columnField = (ColumnField)field;
                        columnName = columnField.DataColumn.ColumnName;

                        if (!(view.IsAutoIdentity && view.DataTable.PrimaryKey[0].Equals(columnField.DataColumn)) && !columnNames.Contains(columnName))
                        {
                            columnNamesList.Add(columnName);
                            columnNames.Add(columnName);
                        }
                    }
                    else if (field is ParentField)
                    {
                        ParentField parentField = (ParentField)field;
                        foreach (DataColumn dataColumn in parentField.DataRelation.ChildColumns)
                        {
                            columnName = dataColumn.ColumnName;
                            if (!(dataColumn.ReadOnly && parentField.DataRelation.ChildColumns.Length > 1) && !columnNames.Contains(columnName))
                            {
                                columnNamesList.Add(columnName);
                                columnNames.Add(columnName);
                            }
                        }
                    }
                }
                else
                {
                    HandleRequired(field, dataAction);
                }
            }

            return columnNamesList;
        }

        private void HandleRequired(Field field, DataAction dataAction)
        {
            if (!field.IsAutoIdentity && !field.IsGuidIdentity && !field.IsAutoGuid && dataAction == DataAction.Create && field.FieldType == FieldType.Column && field.Required && (field.DefaultValue == null || field.DefaultValue == DBNull.Value))
            {
                throw new MissingRequiredFieldWithoutDefaultValue(field);
            }
        }

        private string GetDelimitedColumns(List<string> columnNamesList, View view)
        {
            ISqlTextBuilder sqlTextBuilder = GetSqlTextBuilder(view);
            string delimitedColumns = string.Empty;

            foreach (string columnName in columnNamesList)
            {
                if (view.Fields.ContainsKey(columnName) && view.Fields[columnName].FieldType == FieldType.Column && ((ColumnField)view.Fields[columnName]).Encrypted)
                {
                    delimitedColumns += sqlTextBuilder.EscapeDbObject(((ColumnField)view.Fields[columnName]).EncryptedName) + ",";
                }
                else
                    delimitedColumns += sqlTextBuilder.EscapeDbObject(columnName) + ",";
            }

            return delimitedColumns.TrimEnd(comma);
        }

        private object GetDelimitedColumnsParameters(List<string> columnNamesList, View view)
        {
            ISqlTextBuilder sqlTextBuilder = GetSqlTextBuilder(view); 
            string delimitedColumns = string.Empty;

            foreach (string columnName in columnNamesList)
            {
                if (view.Fields.ContainsKey(columnName) && view.Fields[columnName].FieldType == FieldType.Column && ((ColumnField)view.Fields[columnName]).Encrypted)
                    delimitedColumns += sqlTextBuilder.GetDbEncryptedColumnParameterNameSql(((ColumnField)view.Fields[columnName]).GetSymmetricKeyName(), GetVarFromName(columnName)) + comma;
                else if (view.Fields.ContainsKey(columnName) && view.Fields[columnName].IsPoint)
                {
                    delimitedColumns += GetPointForInsert(view.Fields[columnName], columnName, sqlTextBuilder);
                }
                else
                    delimitedColumns += sqlTextBuilder.DbParameterPrefix + GetVarFromName(columnName) + comma;

            }

            return delimitedColumns.TrimEnd(comma);
        }

        protected virtual string GetPointForInsert(Field field, string columnName, ISqlTextBuilder sqlTextBuilder)
        {
            return sqlTextBuilder.DbParameterPrefix + GetVarFromName(columnName) + comma;
        }

        private string GetDelimitedColumns(View view, DataAction dataAction, Dictionary<string, object> values)
        {
            ISqlTextBuilder sqlTextBuilder = GetSqlTextBuilder(view);
            string delimitedColumns = string.Empty;
            //foreach (Field field in view.Fields.Values.Where(f=>f.IsExcluded(dataAction)==false))
            HashSet<string> columnNames = new HashSet<string>();

            foreach (Field field in view.Fields.Values.Where(f => f.IsExcluded(dataAction) == false && f.IsDerivationEditable(values)))
            {
                string columnName = null;
                if (field is ColumnField)
                {
                    ColumnField columnField = (ColumnField)field;
                    columnName = columnField.DataColumn.ColumnName;

                    if (!(view.IsAutoIdentity && view.DataTable.PrimaryKey[0].Equals(columnField.DataColumn)) && !columnNames.Contains(columnName))
                    {
                        delimitedColumns += sqlTextBuilder.EscapeDbObject(columnName) + ",";
                        columnNames.Add(columnName);
                    }
                }
                else if (field is ParentField)
                {
                    ParentField parentField = (ParentField)field;
                    foreach (DataColumn dataColumn in parentField.DataRelation.ChildColumns)
                    {
                        columnName = dataColumn.ColumnName;
                        if (!(dataColumn.ReadOnly && parentField.DataRelation.ChildColumns.Length > 1) && !columnNames.Contains(columnName))
                        {
                            delimitedColumns += sqlTextBuilder.EscapeDbObject(dataColumn.ColumnName) + ",";
                            columnNames.Add(columnName);
                        }
                    }
                }
            }

            return delimitedColumns.TrimEnd(comma);
        }

        private byte[] StrToByteArray(string s)
        {
            List<byte> value = new List<byte>();
            foreach (char c in s.ToCharArray())
                value.Add(Convert.ToByte(c));
            return value.ToArray();
        }

        protected IDataParameter[] GetParemeters(View view, Dictionary<string, object> values)
        {
            List<IDataParameter> parameters = new List<IDataParameter>();

            HashSet<string> parameterNames = new HashSet<string>();

            foreach (Field field in view.Fields.Values.Where(f => f.FieldType != FieldType.Children && !f.IsFromOtherView()))
            {
                if (values.ContainsKey(field.Name))
                {
                    if (field is ColumnField)
                    {
                        if (!(view.IsAutoIdentity && view.DataTable.PrimaryKey[0].Equals(((ColumnField)field).DataColumn)))
                        {
                            ColumnField columnField = (ColumnField)field;
                            DataColumn dataColumn = ((ColumnField)field).DataColumn;

                            string value = null;
                            byte[] byteArray = null;
                            if (values[field.Name] != null)
                            {
                                if (columnField.DataColumn.DataType.Equals(typeof(byte[])))
                                {
                                    try
                                    {
                                        byteArray = (byte[])values[field.Name];
                                    }
                                    catch
                                    {
                                        byteArray = StrToByteArray(values[field.Name].ToString());
                                    }
                                }
                                else if (field.IsPoint)
                                {
                                    value = GetPointParameterValue(field, values);
                                }
                                else
                                {
                                    value = values[field.Name].ToString();
                                }
                            }
                            if (columnField.TrimSpaces && columnField.GetColumnFieldType() == ColumnFieldType.String)
                            {
                                if (!string.IsNullOrEmpty(value))
                                    value = value.Trim();
                            }

                            IDataParameter parameter;
                            try
                            {
                                if (dataColumn.DataType.Equals(typeof(byte[])))
                                {
                                    parameter = GetNewSqlParameter(view, GetVarFromName(dataColumn.ColumnName), byteArray);
                                }
                                else if (dataColumn.AllowDBNull && string.IsNullOrEmpty(value))
                                {
                                    parameter = GetNewSqlParameter(view, GetVarFromName(dataColumn.ColumnName), DBNull.Value);
                                }
                                else if (value == null)
                                {
                                    throw new DuradosException("The value can not be NULL in column name [" + dataColumn.ColumnName + "]");
                                }
                                else
                                {
                                    if (string.IsNullOrEmpty(value) && dataColumn.DataType.Equals(typeof(bool)))
                                    {
                                        parameter = GetNewSqlParameter(view, GetVarFromName(dataColumn.ColumnName), false);
                                    }
                                    if (value == "checked" && dataColumn.DataType.Equals(typeof(bool)))
                                    {
                                        parameter = GetNewSqlParameter(view, GetVarFromName(dataColumn.ColumnName), true);
                                    }
                                    else if (dataColumn.DataType.Equals(typeof(Guid)))
                                    {
                                        parameter = GetNewSqlParameter(view, GetVarFromName(dataColumn.ColumnName), new Guid(value));
                                    }
                                    else if (dataColumn.DataType.Equals(typeof(DateTimeOffset)) || dataColumn.DataType.Equals(typeof(DateTime)))
                                    {
                                        object date = value;
                                        if (values[field.Name] is DateTime)
                                        {
                                            date = values[field.Name];
                                        }
                                        else
                                        {
                                            date = Convert.ChangeType(date, typeof(DateTime));
                                        }
                                        parameter = GetNewSqlParameter(view, GetVarFromName(dataColumn.ColumnName), date);
                                    }
                                    else
                                    {
                                        parameter = GetNewSqlParameter(view, GetVarFromName(dataColumn.ColumnName), Convert.ChangeType(value, dataColumn.DataType));
                                    }

                                }
                                if (!parameterNames.Contains(parameter.ParameterName))
                                {
                                    parameters.Add(parameter);
                                    parameterNames.Add(parameter.ParameterName);
                                }
                            }
                            catch (Exception exception)
                            {
                                if (value == null) { value = "NULL"; }
                                throw new DuradosException("The value [" + value.ToString() + "] is not in the correct format [" + dataColumn.DataType + "] in column name [" + dataColumn.ColumnName + "]", exception);
                            }
                        }
                    }
                    else if (field is ParentField)
                    {
                        if (values[field.Name] == null)
                        {
                            values[field.Name] = string.Empty;
                        }
                        if (values[field.Name] != null)
                        {
                            string delimitedValue = values[field.Name].ToString();
                            string[] delimitedValues = delimitedValue.Split(comma);
                            ParentField parentField = field as ParentField;
                            for (int i = 0; i < delimitedValues.Count(); i++)
                            {
                                DataColumn dataColumn = parentField.DataRelation.ChildColumns[i];
                                string value = delimitedValues[i];
                                IDataParameter parameter;
                                if (dataColumn.AllowDBNull && !field.Required && string.IsNullOrEmpty(value))
                                {
                                    parameter = GetNewSqlParameter(view, GetVarFromName(dataColumn.ColumnName), DBNull.Value);
                                }
                                else
                                {
                                    if (string.IsNullOrEmpty(value))
                                    {
                                        throw new DuradosException("The field " + field.DisplayName + " is required.");
                                    }
                                    try
                                    {
                                        if (dataColumn.DataType.Equals(typeof(Guid)))
                                        {
                                            parameter = GetNewSqlParameter(view, GetVarFromName(dataColumn.ColumnName), new Guid(value));
                                        }
                                        else
                                        {
                                            parameter = GetNewSqlParameter(view, GetVarFromName(dataColumn.ColumnName), Convert.ChangeType(value, dataColumn.DataType));
                                        }
                                    }
                                    catch (FormatException exception)
                                    {
                                        throw new DuradosFormatException(field, value, exception);
                                    }
                                }
                                if (!parameterNames.Contains(parameter.ParameterName))
                                {
                                    parameters.Add(parameter);
                                    parameterNames.Add(parameter.ParameterName);
                                }
                            }
                        }
                    }
                }

            }

            return parameters.ToArray();
        }

        protected virtual string GetPointParameterValue(Field field, Dictionary<string, object> values)
        {
            return values[field.Name].ToString(); 
        }

        protected virtual IDataParameter GetNewSqlParameter(View view, string name, object value)
        {
            return GetNewParameter(view, name, value);
        }

        private object GetDelimitedColumnsParameters(View view, DataAction dataAction, Dictionary<string, object> values)
        {
            ISqlTextBuilder sqlTextBuilder = GetSqlTextBuilder(view);
            string delimitedColumns = string.Empty;
            //foreach (Field field in view.Fields.Values.Where(f => f.IsExcluded(dataAction) == false))
            HashSet<string> columnNames = new HashSet<string>();
            foreach (Field field in view.Fields.Values.Where(f => f.IsExcluded(dataAction) == false && f.IsDerivationEditable(values)))
            {
                string columnName = null;
                if (field is ColumnField)
                {
                    ColumnField columnField = (ColumnField)field;
                    columnName = columnField.DataColumn.ColumnName;
                    if (!(view.IsAutoIdentity && view.DataTable.PrimaryKey[0].Equals(columnField.DataColumn)) && !columnNames.Contains(columnName))
                    {
                        delimitedColumns += sqlTextBuilder.DbParameterPrefix + columnField.DataColumn.ColumnName + comma;
                        columnNames.Add(columnName);
                    }
                }
                else if (field is ParentField)
                {
                    ParentField parentField = (ParentField)field;
                    foreach (DataColumn dataColumn in parentField.DataRelation.ChildColumns)
                    {
                        columnName = dataColumn.ColumnName;
                        if (!(dataColumn.ReadOnly && parentField.DataRelation.ChildColumns.Length > 1) && !columnNames.Contains(columnName))
                        {
                            delimitedColumns += sqlTextBuilder.DbParameterPrefix + dataColumn.ColumnName + comma;
                            columnNames.Add(columnName);
                        }
                    }
                }
            }

            return delimitedColumns.TrimEnd(comma);
        }

        private string GetWhereStatement(View view)
        {
            return GetWhereStatement(view, view.DataTable.TableName);
        }

        private string GetWhereStatement(View view, string tableName, bool usePermanentFilter = false)
        {
            ISqlTextBuilder sqlTextBuilder = GetSqlTextBuilder(view);
            string whereStatement = string.Empty;

            foreach (DataColumn dataColumn in view.DataTable.PrimaryKey)
            {
                whereStatement += sqlTextBuilder.EscapeDbObject(tableName) + sqlTextBuilder.DbTableColumnSeperator + sqlTextBuilder.EscapeDbObject(dataColumn.ColumnName) + sqlTextBuilder.DbEquals + sqlTextBuilder.DbParameterPrefix +"pk_" + GetVarFromName(dataColumn.ColumnName) + sqlTextBuilder.DbAnd;
            }
            if (usePermanentFilter && !string.IsNullOrEmpty(view.PermanentFilter))
            {
                whereStatement +=  " (" + view.GetPermanentFilter() + ")";
            }

            return RemoveSuffix(whereStatement, sqlTextBuilder.DbAnd) + " ";
        }

        private string GetWhereStatement(View view, string[] pks)
        {
            ISqlTextBuilder sqlTextBuilder = GetSqlTextBuilder(view);
            string tableName = view.DataTable.TableName;
            string whereStatement = string.Empty;

            foreach (DataColumn dataColumn in view.DataTable.PrimaryKey)
            {
                whereStatement += sqlTextBuilder.EscapeDbObject(tableName) + sqlTextBuilder.DbTableColumnSeperator + sqlTextBuilder.EscapeDbObject(dataColumn.ColumnName) + " in (" + pks.Delimited() + ")";
            }

            return whereStatement;
        }

        private string GetWhereStatement(ChildrenField field, DataView dataView)
        {
            ISqlTextBuilder sqlTextBuilder = GetSqlTextBuilder(field.View);
            string whereStatement = string.Empty;
            string[] columnsNames = field.GetEquivalentParentField().GetColumnsNames();
            if (columnsNames.Length == 1)
            {
                string columnName = columnsNames[0];

                whereStatement += sqlTextBuilder.EscapeDbObject(field.ChildrenView.DataTable.TableName) + sqlTextBuilder.DbTableColumnSeperator + sqlTextBuilder.EscapeDbObject(columnName) + " in (" + GetDelimitedValues(field, dataView) + ")";

            }
            else
            {
                foreach (DataRowView row in dataView)
                {
                    string pk = field.View.GetPkValue(row.Row);

                    string[] delimitedValues = pk.Split(',');

                    whereStatement += "(";

                    for (int i = 0; i < columnsNames.Length; i++)
                    {
                        string columnName = columnsNames[i];
                        string value = delimitedValues[i];

                        whereStatement += sqlTextBuilder.EscapeDbObject(field.ChildrenView.DataTable.TableName) + sqlTextBuilder.DbTableColumnSeperator + sqlTextBuilder.EscapeDbObject(columnName) + sqlTextBuilder.DbEquals + value + sqlTextBuilder.DbAnd;

                    }

                    whereStatement = whereStatement.TrimEnd(sqlTextBuilder.DbAnd.ToCharArray());

                    whereStatement += ")";

                    whereStatement += sqlTextBuilder.DbOr;
                }

                whereStatement = whereStatement.TrimEnd(sqlTextBuilder.DbOr.ToCharArray());
            }

            return whereStatement;

        }

        private string GetDelimitedValues(ChildrenField field, DataView dataView)
        {
            string delimitedValues = string.Empty;

            foreach (DataRowView row in dataView)
            {
                delimitedValues += field.View.GetPkValue(row.Row) + ",";
            }

            return delimitedValues.TrimEnd(',');
        }

        private string GetDelimitedValues(ParentField field, DataTable table, out int count)
        {
            //string delimitedValues = string.Empty;
            HashSet<string> delimitedValues = new HashSet<string>();
            foreach (DataRow row in table.Rows)
            {
                string value = field.GetValue(row);
                if (!string.IsNullOrEmpty(value) && !delimitedValues.Contains(value))
                {
                    if (field.DataRelation.ChildColumns[0].DataType.Equals(typeof(string)))
                    {
                        value = "'" + value + "'";
                    }
                    delimitedValues.Add(value);
                }
            }

            count = delimitedValues.Count;

            if (count == 0)
            {
                if (IsNumeric(field.DataRelation.ChildColumns[0]))
                {
                    return "0";
                }
                else
                {
                    return "'0'";
                }
            }

            return delimitedValues.ToArray().Delimited();
        }

        private string GetDependencyWhereStatement(ParentField field)
        {
            //string whereStatement = string.Empty;

            //foreach (DataColumn dataColumn in field.DependencyField.DataRelation.ChildColumns)
            //{
            //    whereStatement += sqlTextBuilder.EscapeDbObject(dataColumn.Table.TableName + "].[" + dataColumn.ColumnName + "] = @" + dataColumn.ColumnName + " and ";
            //}

            //return RemoveSuffix(whereStatement, "and ");
            if (field.HasDependencyFilter())
            {
                return GetDependencyWhereStatementFromDependencyField(field.DependencyField);
            }
            else
            {
                return string.Empty;
            }
        }

        private string GetDependencyWhereStatementFromDependencyField(ParentField field)
        {
            ISqlTextBuilder sqlTextBuilder = GetSqlTextBuilder(field.View);
            string whereStatement = string.Empty;

            foreach (DataColumn dataColumn in field.DataRelation.ChildColumns)
            {
                whereStatement += sqlTextBuilder.EscapeDbObject(dataColumn.Table.TableName) + sqlTextBuilder.DbTableColumnSeperator + dataColumn.ColumnName + sqlTextBuilder.DbEquals + sqlTextBuilder.DbParameterPrefix + dataColumn.ColumnName + sqlTextBuilder.DbAnd;
            }

            return RemoveSuffix(whereStatement, sqlTextBuilder.DbAnd) + " ";
        }

        private string GetWhereStatement(ParentField field)
        {
            if (field == null)
                return " 1=1 ";
            ISqlTextBuilder sqlTextBuilder = GetSqlTextBuilder(field.View);

            string whereStatement = string.Empty;

            foreach (DataColumn dataColumn in field.DataRelation.ChildColumns)
            {
                whereStatement += sqlTextBuilder.EscapeDbObject(GetTableName(field.View)) + sqlTextBuilder.DbTableColumnSeperator + sqlTextBuilder.EscapeDbObject(dataColumn.ColumnName) + sqlTextBuilder.DbEquals + sqlTextBuilder.DbParameterPrefix + GetVarFromName(dataColumn.ColumnName) + sqlTextBuilder.DbAnd;
            }

            return RemoveSuffix(whereStatement, sqlTextBuilder.DbAnd) + " ";
        }

        private IEnumerable<IDataParameter> GetWhereParemeters(View view, string pk)
        {
            List<IDataParameter> parameters = new List<IDataParameter>();
            string[] delimitedValues = pk.Split(comma);
            for (int i = 0; i < view.DataTable.PrimaryKey.Length; i++)
            {
                object o = null;
                string pkValue = delimitedValues[i];
                if (pkValue.EndsWith("#"))
                {
                    if (!view.DataTable.PrimaryKey[i].DataType.Equals(typeof(string)))
                    {
                        pkValue = pkValue.TrimEnd('#');
                    }
                }
                if (view.DataTable.PrimaryKey[i].DataType.Equals(typeof(Guid)))
                {
                    o = new Guid(pkValue);
                }
                else
                {
                    o = Convert.ChangeType(pkValue, view.DataTable.PrimaryKey[i].DataType);
                }
                IDataParameter parameter = GetNewSqlParameter(view, "pk_" + GetVarFromName(view.DataTable.PrimaryKey[i].ColumnName), o);
                parameters.Add(parameter);
            }

            return parameters;
        }

        private string GetDelimitedColumns(ChildrenField field)
        {
            string s = string.Empty;
            foreach (DataColumn dataColumn in field.DataRelation.ChildColumns)
            {
                s += dataColumn.ColumnName + (dataColumn.Equals(field.DataRelation.ChildColumns.Last()) ? ' ' : comma).ToString();
            }

            return s;
        }

        private string GetDelimitedColumnsForSelect(ParentField field)
        {
            ISqlTextBuilder sqlTextBuilder = GetSqlTextBuilder(field.View);
            string s = string.Empty;
            foreach (DataColumn dataColumn in field.DataRelation.ChildColumns)
            {
                s += sqlTextBuilder.EscapeDbObject(dataColumn.ColumnName) + (dataColumn.Equals(field.DataRelation.ChildColumns.Last()) ? ' ' : comma).ToString();
            }

            return s;
        }

        private string GetDelimitedColumns(ParentField field)
        {
            ISqlTextBuilder sqlTextBuilder = GetSqlTextBuilder(field.View);
            string s = string.Empty;
            foreach (DataColumn dataColumn in field.DataRelation.ParentColumns)
            {
                s += sqlTextBuilder.EscapeDbObject(dataColumn.ColumnName) + (dataColumn.Equals(field.DataRelation.ParentColumns.Last()) ? ' ' : comma).ToString();
            }

            return s;
        }

        private IEnumerable<IDataParameter> GetWhereParemeters(ParentField field, string fk)
        {
            List<IDataParameter> parameters = new List<IDataParameter>();
            if (fk == null || field == null)
                return parameters;

            string[] delimitedValues = fk.Split(comma);
            int i = 0;
            foreach (DataColumn dataColumn in field.DataRelation.ChildColumns)
            {
                IDataParameter parameter = GetNewSqlParameter(field.View, GetVarFromName(dataColumn.ColumnName), Convert.ChangeType(delimitedValues[i], dataColumn.DataType));
                parameters.Add(parameter);
                i++;
            }

            return parameters;
        }

        private IEnumerable<IDataParameter> GetDependencyWhereParemetersFromDependencyField(ParentField field, string fk)
        {
            List<IDataParameter> parameters = new List<IDataParameter>();
            string[] delimitedValues = fk.Split(comma);
            int i = 0;
            foreach (DataColumn dataColumn in field.DataRelation.ChildColumns)
            {
                IDataParameter parameter;
                if (dataColumn.DataType.Equals(typeof(Guid)))
                {
                    parameter = GetNewSqlParameter(field.View, dataColumn.ColumnName, new Guid(delimitedValues[i]));
                }
                else
                {
                    parameter = GetNewSqlParameter(field.View, dataColumn.ColumnName, Convert.ChangeType(delimitedValues[i], dataColumn.DataType));
                }
                parameters.Add(parameter);
                i++;
            }

            return parameters;
        }

        private IEnumerable<System.Data.IDataParameter> GetDependencyWhereParemeters(ParentField field, string fk)
        {
            //List<System.Data.SqlClient.SqlParameter> parameters = new List<System.Data.SqlClient.SqlParameter>();
            //string[] delimitedValues = fk.Split(comma);
            //int i = 0;
            //foreach (DataColumn dataColumn in field.DependencyField.DataRelation.ChildColumns)
            //{
            //    System.Data.SqlClient.SqlParameter parameter = new System.Data.SqlClient.SqlParameter(dataColumn.ColumnName, Convert.ChangeType(delimitedValues[i], dataColumn.DataType));
            //    parameters.Add(parameter);
            //    i++;
            //}

            //return parameters;

            return GetDependencyWhereParemetersFromDependencyField(field.DependencyField, fk);
        }

        private List<string> GetUpdateSetColumns(View view, Dictionary<string, object> values)
        {
            List<string> updateSetColumns = new List<string>();
            //foreach (Field field in view.Fields.Values.Where(f => f.IsExcluded(DataAction.Edit) == false))
            foreach (Field field in view.Fields.Values.Where(f => f.IsExcluded(DataAction.Edit) == false && f.IsAllow(DataAction.Edit) && f.IsDerivationEditable(values) && !f.IsFromOtherView()))
            {
                if (values.ContainsKey(field.Name))
                {
                    if (field is ColumnField)
                    {
                        ColumnField columnField = field as ColumnField;
                        if (!((view.IsAutoIdentity || view.IsGuidIdentity) && view.DataTable.PrimaryKey[0].Equals(columnField.DataColumn)))
                        {
                            updateSetColumns.Add(columnField.DataColumn.ColumnName);

                        }
                    }
                    else if (field is ParentField)
                    {
                        ParentField parentField = field as ParentField;
                        foreach (DataColumn dataColumn in parentField.DataRelation.ChildColumns)
                        {
                            updateSetColumns.Add(dataColumn.ColumnName);
                        }
                    }
                }
            }

            return updateSetColumns;
        }
        private string GetUpdateSetColumns(List<string> columnNames, View view)
        {
            ISqlTextBuilder sqlTextBuilder = GetSqlTextBuilder(view);
            string updateSetColumns = string.Empty;

            foreach (string columnName in columnNames)
            {
                if (view.Fields.ContainsKey(columnName) && view.Fields[columnName].FieldType == FieldType.Column && ((ColumnField)view.Fields[columnName]).Encrypted)
                    updateSetColumns += sqlTextBuilder.EscapeDbObject(((ColumnField)view.Fields[columnName]).EncryptedName) + sqlTextBuilder.DbEquals +  sqlTextBuilder.GetDbEncryptedColumnParameterNameSql(((ColumnField)view.Fields[columnName]).GetSymmetricKeyName()  ,  columnName  ) + comma;
                else if (view.Fields.ContainsKey(columnName) && view.Fields[columnName].IsPoint)
                {
                    updateSetColumns += GetPointUpdateSetColumn(view.Fields[columnName], columnName, sqlTextBuilder);
                }
                else
                    updateSetColumns += sqlTextBuilder.EscapeDbObject(columnName) + sqlTextBuilder.DbEquals + sqlTextBuilder.DbParameterPrefix + GetVarFromName(columnName) + comma;
            }

            return updateSetColumns.TrimEnd(comma);
        }

        protected virtual string GetPointUpdateSetColumn(Field field, string columnName, ISqlTextBuilder sqlTextBuilder)
        {
            return sqlTextBuilder.EscapeDbObject(columnName) + sqlTextBuilder.DbEquals + sqlTextBuilder.DbParameterPrefix + GetVarFromName(columnName) + comma;
        }

        //private string GetUpdateSetColumns(View view, Dictionary<string, object> values)
        //{
        //    string updateSetColumns = string.Empty;
        //    //foreach (Field field in view.Fields.Values.Where(f => f.IsExcluded(DataAction.Edit) == false))
        //    foreach (Field field in view.Fields.Values.Where(f => f.IsExcluded(DataAction.Edit) == false && f.IsDerivationEditable(values)))
        //    {
        //        if (values.ContainsKey(field.Name))
        //        {
        //            if (field is ColumnField)
        //            {
        //                ColumnField columnField = field as ColumnField;
        //                if (!(view.IsAutoIdentity && view.DataTable.PrimaryKey[0].Equals(columnField.DataColumn)))
        //                {
        //                    updateSetColumns += sqlTextBuilder.EscapeDbObject(columnField.DataColumn.ColumnName + "] = " + "@" + columnField.DataColumn.ColumnName + comma;
        //                }
        //            }
        //            else if (field is ParentField)
        //            {
        //                ParentField parentField = field as ParentField;
        //                foreach (DataColumn dataColumn in parentField.DataRelation.ChildColumns)
        //                {
        //                    updateSetColumns += sqlTextBuilder.EscapeDbObject(dataColumn.ColumnName + "] = " + "@" + dataColumn.ColumnName + comma;
        //                }
        //            }
        //        }
        //    }

        //    return updateSetColumns.TrimEnd(comma);
        //}

        public string[] GetIndexInfo(string indexName, string connectionString, out string tableName)
        {
            string sql = "SELECT sys.tables.object_id, sys.tables.name AS table_name, sys.columns.name AS column_name, sys.indexes.name AS index_name, sys.indexes.is_unique, " +
                  "sys.indexes.is_primary_key " +
                "FROM     sys.tables INNER JOIN " +
                  "sys.indexes ON sys.tables.object_id = sys.indexes.object_id INNER JOIN " +
                  "sys.index_columns ON sys.tables.object_id = sys.index_columns.object_id AND sys.indexes.index_id = sys.index_columns.index_id INNER JOIN " +
                    "sys.columns ON sys.tables.object_id = sys.columns.object_id AND sys.index_columns.column_id = sys.columns.column_id " +
                    "where sys.indexes.name ='" + indexName + "'";

            DataTable table = ExecuteTable(connectionString, sql, null, CommandType.Text);

            tableName = null;
            if (table.Rows.Count == 0)
                return null;

            List<string> columns = new List<string>();
            tableName = table.Rows[0]["table_name"].ToString();

            foreach (DataRow row in table.Rows)
            {
                columns.Add(row["column_name"].ToString());
            }

            return columns.ToArray();

        }

        /// <summary>
        /// Change ordinal of records between o_pk and d_pk- in order to move o_pk to d_pk place
        /// </summary>
        /// <param name="view"></param>
        /// <param name="o_pk"></param>
        /// <param name="d_pk"></param>
        public virtual void ChangeOrdinal(View view, string o_pk, string d_pk, int userID)
        {
            string tableName = string.Empty;

            if (o_pk == d_pk)
            {
                return;
            }

            using (SqlConnection connection = new SqlConnection(view.ConnectionString))
            {
                try
                {
                    tableName = GetTableName(view);

                    connection.Open();
                    SqlCommand command = new SqlCommand();
                    command.Connection = connection;

                    StringBuilder sqlBuilder = new StringBuilder();


                    //sql = "update [{0}] set [{1}]={2} where {3}";
                    //sql = string.Format(sql, tableName, view.OrdinalColumnName, -Convert.ToInt64(dOrder), GetWhereStatement(view, tableName));
                    if (view.DataTable.PrimaryKey.Length != 1)
                        throw new DuradosException("Not supported for multi columns primary key " + tableName);

                    string pkColumnName = view.DataTable.PrimaryKey[0].ColumnName;

                    //Variables and validations from here
                    sqlBuilder.Append("DECLARE @returnValue int select @returnValue = 0 ");
                    sqlBuilder.Append("DECLARE @PKFromOrdinal INT DECLARE @PKToOrdinal INT DECLARE @Direction nvarchar(4) ");
                    //If o_pk not exist or d_pk not exist- return error
                    sqlBuilder.Append("if exists (select [{2}] from [{0}] where [{2}]=@PKFrom) and exists (select [{2}] from [{0}] where [{2}]=@PKTo) begin ");
                    sqlBuilder.Append("select @PKFromOrdinal=[{1}] from [{0}] where [{2}]=@PKFrom ");
                    sqlBuilder.Append("select @PKToOrdinal=[{1}] from [{0}] where [{2}]=@PKTo ");
                    //If o_pk ordinal is null or d_pk ordinal is null- do nothing but return success
                    sqlBuilder.Append("if @PKFromOrdinal is null or @PKToOrdinal is null or @PKFromOrdinal=@PKToOrdinal begin select @returnValue =1 end else begin ");
                    sqlBuilder.Append("select @Direction=case when @PKFromOrdinal<@PKToOrdinal then 'down' else 'up' end ");

                    //Update all records which in range
                    sqlBuilder.Append("BEGIN TRAN T1 ");
                    sqlBuilder.Append(";with RecordsToChange([{2}], [{1}], RowNumber) ");
                    sqlBuilder.Append("as (select [{2}], [{1}], ROW_NUMBER() OVER (order by [{1}]) as RowNumber from [{0}] ");
                    sqlBuilder.Append("where @Direction='down' and [{1}]>=@PKFromOrdinal and [{1}]<=@PKToOrdinal ");
                    sqlBuilder.Append("or @Direction='up' and [{1}]>=@PKToOrdinal and [{1}]<=@PKFromOrdinal) ");
                    sqlBuilder.Append("Update [{0}] set [{0}].[{1}]=RecordsToChange2.[{1}] from [{0}] ");
                    sqlBuilder.Append("inner join RecordsToChange RecordsToChange1 on RecordsToChange1.[{2}]=[{0}].[{2}] ");
                    sqlBuilder.Append("inner join RecordsToChange RecordsToChange2 on  ");
                    sqlBuilder.Append("@Direction='down' and RecordsToChange1.RowNumber=RecordsToChange2.RowNumber+1 ");
                    sqlBuilder.Append("or @Direction='up' and RecordsToChange1.RowNumber=RecordsToChange2.RowNumber-1 ");

                    //Update moved record
                    sqlBuilder.Append("update [{0}] set [{0}].[{1}]=@PKToOrdinal where [{2}]=@PKFrom ");
                    sqlBuilder.Append("COMMIT TRAN T1 select @returnValue =1 ");
                    sqlBuilder.Append("end end select @returnValue");

                    string sql = string.Format(sqlBuilder.ToString(), tableName, view.OrdinalColumnName, pkColumnName);
                    command.CommandText = sql;

                    //Add command parameters
                    command.Parameters.AddWithValue("@PKFrom", Convert.ToInt32(o_pk));
                    command.Parameters.AddWithValue("@PKTo", Convert.ToInt32(d_pk));

                    object returnedValue = command.ExecuteScalar();

                    if (Convert.ToInt32(returnedValue) != 1)
                    {
                        throw new DuradosException("Failed to change order in table " + tableName);
                    }
                }
                catch (Exception ex)
                {
                    throw new DuradosException("Failed to change order in table " + tableName, ex);
                }
            }
        }

        public virtual void Switch(View view, string o_pk, string d_pk, int userID)
        {

            ISqlTextBuilder sqlTextBuilder = GetSqlTextBuilder(view);
            using (System.Data.IDbConnection connection = GetNewConnection(view.ConnectionString))
            {
                string tableName = GetTableName(view);

                connection.Open();
                IDbTransaction transaction = connection.BeginTransaction(IsolationLevel.ReadCommitted);
                IDbCommand command = GetNewCommand(string.Empty, connection);
                command.Connection = connection;
                command.Transaction = transaction;

                string sql = "select " + sqlTextBuilder.EscapeDbObject("{0}") + " from " + sqlTextBuilder.EscapeDbObject("{1}") + sqlTextBuilder.WithNolock + " where {2}";
                sql = string.Format(sql, view.OrdinalColumnName, view.DataTable.TableName, GetWhereStatement(view));
                command.CommandText = sql;
                command.Parameters.Clear();
                foreach (IDataParameter parameter in GetWhereParemeters(view, o_pk))
                {
                    command.Parameters.Add(GetNewParameter(command, parameter.ParameterName, parameter.Value));
                }
                object oOrder = command.ExecuteScalar();
                command.Parameters.Clear();
                foreach (IDataParameter parameter in GetWhereParemeters(view, d_pk))
                {
                    command.Parameters.Add(GetNewParameter(command, parameter.ParameterName, parameter.Value));
                }
                object dOrder = command.ExecuteScalar();

                if (dOrder == null || dOrder == DBNull.Value)
                {
                    throw new DuradosException("The view " + view.DisplayName + " is set to be an ordered view. Some of its rows has no values in the " + view.OrdinalColumnName + " field. Please make sure that all the rows have a unique value in this field.");
                }

                sql = "update " + sqlTextBuilder.EscapeDbObject("{0}") + " set " + sqlTextBuilder.EscapeDbObject("{1}") + "={2} where {3}";
                sql = string.Format(sql, tableName, view.OrdinalColumnName, -Convert.ToInt64(dOrder), GetWhereStatement(view, tableName));
                command.CommandText = sql;
                command.Parameters.Clear();
                foreach (IDataParameter parameter in GetWhereParemeters(view, o_pk))
                {
                    command.Parameters.Add(GetNewParameter(command, parameter.ParameterName, parameter.Value));
                }
                command.ExecuteNonQuery();

                sql = "update " + sqlTextBuilder.EscapeDbObject("{0}") + " set " + sqlTextBuilder.EscapeDbObject("{1}") + "={2} where {3}";
                sql = string.Format(sql, tableName, view.OrdinalColumnName, oOrder, GetWhereStatement(view, tableName));
                command.CommandText = sql;
                command.Parameters.Clear();
                foreach (IDataParameter parameter in GetWhereParemeters(view, d_pk))
                {
                    command.Parameters.Add(GetNewParameter(command, parameter.ParameterName, parameter.Value));
                }
                command.ExecuteNonQuery();

                sql = "update " + sqlTextBuilder.EscapeDbObject("{0}") + " set " + sqlTextBuilder.EscapeDbObject("{1}") + "={2} where {3}";
                sql = string.Format(sql, tableName, view.OrdinalColumnName, dOrder, GetWhereStatement(view, tableName));
                command.CommandText = sql;
                command.Parameters.Clear();
                foreach (IDataParameter parameter in GetWhereParemeters(view, o_pk))
                {
                    command.Parameters.Add(GetNewParameter(command, parameter.ParameterName, parameter.Value));
                }
                command.ExecuteNonQuery();

                transaction.Commit();
                Cache.Clear(view.Name);
            }

        }
        // replace with routh data
        protected Filter GetFilter(View view, string[] pk, bool usePermanentFilter = false)
        {
            ISqlTextBuilder sqlTextBuilder = GetSqlTextBuilder(view);
            Filter filter = new Filter();

            string permanentFilter = string.Empty;
            if (usePermanentFilter)
                permanentFilter = string.IsNullOrEmpty(view.PermanentFilter) ? string.Empty : (view.GetPermanentFilter() + " and ");

            filter.WhereStatement = " where 1=1 and " + permanentFilter;
            filter.WhereStatementWithoutTablePrefix = string.Empty;


            List<IDataParameter> parameters = new List<IDataParameter>();

            for (int i = 0; i < pk.Length; i++)
            {
                DataColumn column = view.DataTable.PrimaryKey[i];
                string columnName = column.ColumnName;
                string parameterName = sqlTextBuilder.DbParameterPrefix + GetVarFromName(columnName);
                object parameterValue = null;
                string pkValue = pk[i];
                if (pkValue.EndsWith("#"))
                {
                    if (!column.DataType.Equals(typeof(string)))
                    {
                        pkValue = pkValue.TrimEnd('#');
                    }
                }
                if (column.DataType.Equals(typeof(Guid)))
                {
                    parameterValue = new Guid(pkValue);
                }
                else if (!pkValue.Contains(Filter.TOKEN))
                {
                    parameterValue = Convert.ChangeType(pkValue, column.DataType);
                }

                string currentExpression;
                if (parameterValue != null)
                {
                    filter.WhereStatement += sqlTextBuilder.EscapeDbObject(column.Table.TableName) + sqlTextBuilder.DbTableColumnSeperator + sqlTextBuilder.EscapeDbObject(columnName) + sqlTextBuilder.DbEquals + parameterName + sqlTextBuilder.DbAnd;
                    filter.WhereStatementWithoutTablePrefix += "[" + columnName + "]" + " = " + parameterName + " and ";
                    parameters.Add(GetNewSqlParameter(view, parameterName, parameterValue));
                }
                else //range
                {
                    var eq = getComparerString(pkValue, Filter.TOKEN, Filter.TOKEN);
                    var first = pkValue.Replace(WrapToken(eq, Filter.TOKEN, Filter.TOKEN), "").Trim();

                    currentExpression = "[" + columnName + "] " + pkValue.Replace(Filter.TOKEN, "") + " and ";
                    filter.WhereStatement += sqlTextBuilder.EscapeDbObject(column.Table.TableName) + sqlTextBuilder.DbTableColumnSeperator + sqlTextBuilder.EscapeDbObject(columnName) + " " + eq + " " + parameterName + sqlTextBuilder.DbAnd;
                    filter.WhereStatementWithoutTablePrefix += "[" + columnName + "]" + " " + eq + " " + parameterName + " and ";
                }
            }

            filter.WhereStatement = RemoveSuffix(filter.WhereStatement, sqlTextBuilder.DbAnd) + " ";
            filter.WhereStatementWithoutTablePrefix = RemoveSuffix(filter.WhereStatementWithoutTablePrefix, "and ");
            //filter.WhereStatementWithoutTablePrefix = filter.WhereStatementWithoutTablePrefix.Trim(("and ").ToCharArray());
            filter.Parameters = parameters.ToArray();
            return filter;
        }

        private string RemoveSuffix(string value, string suffix)
        {
            if (value.EndsWith("and "))
                return value.Substring(0, value.Length - 4);
            else
                return value;
        }

        private string GetServerType(string tableName, string columnName, SqlCommand command)
        {

            string sql = "SELECT usrt.name AS [DataType] FROM sys.tables AS tbl INNER JOIN sys.all_columns AS clmns ON clmns.object_id=tbl.object_id LEFT OUTER JOIN sys.types AS usrt ON usrt.user_type_id = clmns.user_type_id WHERE (tbl.name=N'" + tableName + "' and SCHEMA_NAME(tbl.schema_id)=N'dbo' and clmns.name=N'" + columnName + "')";
            command.CommandText = sql;

            object scalar = command.ExecuteScalar();

            return scalar.ToString();
        }

        private string GetDeclarations(ChildrenField field, string childTableName, SqlCommand command)
        {
            string s = string.Empty;

            foreach (DataColumn dataColumn in field.DataRelation.ChildColumns)
            {
                s += "declare @" + GetVarFromName(dataColumn.ColumnName) + " " + GetServerType(childTableName, dataColumn.ColumnName, command) + " ";
            }

            return s;
        }

        private string GetDeclarationsAssignments(ChildrenField field)
        {
            string s = string.Empty;

            foreach (DataColumn dataColumn in field.DataRelation.ChildColumns)
            {
                s += " @" + GetVarFromName(dataColumn.ColumnName) + "=[" + dataColumn.ColumnName + "] ";
            }

            return s;
        }

        private string GetCounterTriggerWhere(ChildrenField field, string tableName)
        {
            string s = string.Empty;

            int l = field.DataRelation.ChildColumns.Count();
            for (int i = 0; i < l; i++)
            {
                DataColumn childColumn = field.DataRelation.ChildColumns[i];
                DataColumn parentColumn = field.DataRelation.ParentColumns[i];
                s += " [" + tableName + "].[" + parentColumn.ColumnName + "] = @" + childColumn.ColumnName + (i < l - 1 ? " and " : "").ToString();
            }


            return s;
        }

        private bool IsView(string viewName, SqlCommand command)
        {
            string sql = "select TABLE_NAME from INFORMATION_SCHEMA.VIEWS where TABLE_NAME = N'" + viewName + "'";

            command.CommandText = sql;

            object scalar = command.ExecuteScalar();

            if (scalar == null || scalar == DBNull.Value)
                return false;
            else
                return true;

        }

        //private string GetTableName(string viewName, SqlCommand command)
        //{
        //string viewDefinition = GetViewDefinition(viewName, command);

        //string[] words = viewDefinition.Split(' ');

        //List<string> words2 = new List<string>();
        //foreach (string word in words)
        //{
        //    string[] words3 = word.Split(@"\n\r".ToCharArray());
        //    foreach (string word in words)
        //    {

        //    }
        //}

        //string prevWord = string.Empty;
        //foreach (string word in words)
        //{
        //    if (prevWord.ToLower() == "from")
        //    {
        //        return word.Split('.').Last();
        //    }

        //    prevWord = word;
        //}

        //return null;
        //}

        private string GetViewDefinition(string viewName, SqlCommand command)
        {
            string sql = "select VIEW_DEFINITION from INFORMATION_SCHEMA.VIEWS where TABLE_NAME = N'" + viewName + "'";

            command.CommandText = sql;

            object scalar = command.ExecuteScalar();

            if (scalar == null || scalar == DBNull.Value)
                return null;
            else
                return scalar.ToString();
        }

        private string GetCreateView(string viewName, string counterFieldName, SqlCommand command)
        {
            string viewDefinition = GetViewDefinition(viewName, command).ToLower();

            int fromIndex = viewDefinition.IndexOf(" from ");
            return viewDefinition.Insert(fromIndex, comma + counterFieldName);
        }

        public void CreateCounter(ChildrenField field)
        {
            SqlTransaction transaction = null;

            using (SqlConnection connection = new SqlConnection(field.View.ConnectionString))
            {
                try
                {
                    connection.Open();
                    transaction = connection.BeginTransaction(IsolationLevel.ReadCommitted);
                    SqlCommand command = new SqlCommand();
                    command.Connection = connection;
                    command.Transaction = transaction;

                    string parentTableName = field.View.DataTable.TableName;
                    string childrenTableName = field.ChildrenView.DataTable.TableName;
                    bool isParentView = IsView(parentTableName, command);
                    bool isChildrenView = IsView(childrenTableName, command);
                    string createView = string.Empty;
                    string dropView = "drop view " + parentTableName;
                    string counterFieldName = ChildrenField.FkCounterPrefix + field.Name;

                    if (isParentView)
                    {
                        //createView = GetCreateView(parentTableName, counterFieldName, command);
                        parentTableName = field.View.BaseTableName; //GetTableName(parentTableName, command);
                    }

                    if (isChildrenView)
                        childrenTableName = field.ChildrenView.BaseTableName;//GetTableName(childrenTableName, command);


                    string createCounterField = "if ((SELECT COUNT(*) FROM SYSCOLUMNS WHERE ID = OBJECT_ID('" + parentTableName + "') AND Name = '" + counterFieldName + "') < 1 ) Begin ALTER TABLE [" + parentTableName + "] ADD  " + counterFieldName + " int NULL end ";
                    string delimitedFk = GetDelimitedColumns(field);
                    string delimitedPk = GetPrimaryKeyColumnsDelimited(field.ChildrenView);
                    string updateCounterField = "update [" + parentTableName + "] set [" + parentTableName + "]." + counterFieldName + " = c.Counts from [" + parentTableName + "] p inner join (select " + delimitedFk + ", Count(*) as Counts from [" + childrenTableName + "] group by " + delimitedFk + ") c on " + GetJoin(field);
                    string deleteInsertTrigger = "if exists (select * from sys.triggers where name = '" + field.Name + "InsertCounter') begin DROP TRIGGER " + field.Name + "InsertCounter end";
                    string createInsertTrigger = "create TRIGGER " + field.Name + "InsertCounter ON [" + childrenTableName + "] FOR  insert AS " + GetDeclarations(field, childrenTableName, command) + " select " + GetDeclarationsAssignments(field) + " from inserted update [" + parentTableName + "] set " + counterFieldName + "=isnull(" + counterFieldName + ",0)+1 where " + GetCounterTriggerWhere(field, parentTableName);
                    string deleteDeleteTrigger = "if exists (select * from sys.triggers where name = '" + field.Name + "DeleteCounter') begin DROP TRIGGER " + field.Name + "DeleteCounter end";
                    string createDeleteTrigger = "create TRIGGER " + field.Name + "DeleteCounter ON [" + childrenTableName + "] FOR  delete AS " + GetDeclarations(field, childrenTableName, command) + " select " + GetDeclarationsAssignments(field) + " from deleted update [" + parentTableName + "] set " + counterFieldName + "=isnull(" + counterFieldName + ",1)-1 where " + GetCounterTriggerWhere(field, parentTableName);


                    command.CommandText = createCounterField;
                    command.ExecuteNonQuery();

                    command.CommandText = updateCounterField;
                    command.ExecuteNonQuery();

                    command.CommandText = deleteInsertTrigger;
                    command.ExecuteNonQuery();

                    command.CommandText = createInsertTrigger;
                    command.ExecuteNonQuery();

                    command.CommandText = deleteDeleteTrigger;
                    command.ExecuteNonQuery();

                    command.CommandText = createDeleteTrigger;
                    command.ExecuteNonQuery();

                    //if (isParentView)
                    //{
                    //    command.CommandText = dropView;
                    //    command.ExecuteNonQuery();

                    //    command.CommandText = createView;
                    //    command.ExecuteNonQuery();

                    //}

                    transaction.Commit();

                }
                catch (Exception exception)
                {
                    transaction.Rollback();
                    throw exception;
                }
            }
        }

        //private bool IsFieldExists(string tableName, string fieldName)
        //{
        //    throw new NotImplementedException();
        //}
        public object HasData(Field field)
        {
            return HasData(field, 1000);
        }

        public object HasData(Field field, int maxRows)
        {
            if (RowCount(field.View) > maxRows)
                return true;

            string tableName = "[" + field.View.DataTable.TableName + "]";
            string columnName = tableName + "." + "[" + field.DatabaseNames.Split(',')[0].Trim() + "]";
            string sql = string.Format("select top(1) {0} from {1} where {0} is not null", columnName, tableName);

            return !string.IsNullOrEmpty(ExecuteScalar(field.View.ConnectionString, sql));
        }

        public void Truncate(IDbCommand command, View view)
        {
            ISqlTextBuilder sqlTextBuilder = GetSqlTextBuilder(view);

            string sql = string.Format("TRUNCATE TABLE " + sqlTextBuilder.EscapeDbObject("{0}"), GetTableName(view)); ;

            using (command)
            {
                try
                {
                    command.CommandText = sql;
                    command.ExecuteNonQuery();
                }
                catch (Exception exception)
                {
                    throw exception;
                }

            }
        }


        public void InsertSelectBulk(View view, ParentField parentField, Filter filter, Dictionary<string, object> defaultValues, int bulkAmount, int totalRows, int timeout, string email, object map, AsyncCallback HandleAddItemsAsyncCallback_Success, DictionaryArgsCallback handleAddItemsAsyncCallback_failure, NoArgsCallbak asyncOperationRunning, BoolNoArgsCallbak isAsyncRunning)
        {
            View parentView = parentField.ParentView;
            string pkKey = "A.ID";
            string sql = string.Empty;
            List<SqlParameter> parameters = new List<SqlParameter>();
            foreach (IDataParameter param in filter.Parameters)
            {
                parameters.Add((SqlParameter)param);
            }
            //parameters.AddRange(filter.Parameters);
            if (defaultValues.ContainsKey(parentField.DatabaseNames))
            {
                pkKey = "@" + parentField.DatabaseNames.ReplaceNonAlphaNumeric();
                parameters.Add(new SqlParameter(pkKey, defaultValues[parentField.DatabaseNames]));
                defaultValues.Remove(parentField.DatabaseNames);
            }
            foreach (KeyValuePair<string, object> parameter in defaultValues)
            {
                parameters.Add(new SqlParameter("@" + parameter.Key.ReplaceNonAlphaNumeric(), parameter.Value));
            }
            //SqlParameter errorParameter=new SqlParameter("@ErrorMsg", SqlDbType.VarChar, 20);
            //errorParameter.Direction=ParameterDirection.InputOutput;
            //parameters.Add(errorParameter);

            if (totalRows <= bulkAmount)
            {
                sql = GetAddItemSyncSql();
                sql = string.Format(sql, view.DataTable.TableName, parentField.DatabaseNames, GetDefaultFieldsForAddItems(defaultValues), parentView.GetPkColumnNames()[0], GetDefaultValuessForAddItems(defaultValues), parentView.DataTable.TableName, filter.WhereStatement, pkKey);


                try
                {
                    ExecuteNonQuery(view.ConnectionString, sql, parameters.ToDictionary(x => x.ParameterName, y => y.Value), null);
                }
                catch (Exception exception)
                {
                    throw new DuradosException("Failed to add Items", exception);

                }
            }
            else
            {
                int loopCount = totalRows / bulkAmount + (totalRows % bulkAmount > 0 ? 1 : 0);
                sql = GetAddItemsAsyncSql();
                sql = string.Format(sql, bulkAmount, loopCount, view.DataTable.TableName, parentField.DatabaseNames, GetDefaultFieldsForAddItems(defaultValues), parentView.GetPkColumnNames()[0], GetDefaultValuessForAddItems(defaultValues), parentView.DataTable.TableName, filter.WhereStatement, pkKey);


                SqlConnectionStringBuilder builder = new SqlConnectionStringBuilder(view.ConnectionString);
                builder.AsynchronousProcessing = true;

                SqlConnection connection = new SqlConnection(builder.ConnectionString);
                SqlCommand command = new SqlCommand(sql, connection);
                Dictionary<string, object> args = new Dictionary<string, object>();
                args.Add("view", view);
                args.Add("command", command);
                args.Add("time", DateTime.Now.ToString("MMM dd yyyy hh:mm"));
                args.Add("email", email);
                args.Add("map", map);

                command.Parameters.AddRange(parameters.ToArray());
                if (isAsyncRunning())
                    throw new DuradosAsyncRunningException();
                asyncOperationRunning();
                try
                {
                    command.Connection.Open();
                    command.CommandTimeout = timeout;
                    IAsyncResult result = command.BeginExecuteNonQuery(HandleAddItemsAsyncCallback_Success, args);
                }
                catch (Exception exception)
                {
                    args.Add("exception", exception);
                    handleAddItemsAsyncCallback_failure(args);
                    if (connection != null)
                    {
                        connection.Close();
                    }
                }


            }

        }

        private object GetDefaultValuessForAddItems(Dictionary<string, object> defaultValues)
        {

            string sql = string.Empty;
            if (defaultValues.Count > 0)
                sql = string.Join("", defaultValues.Select(f => ",@" + f.Key.ReplaceNonAlphaNumeric()).ToArray<string>());
            return sql;
        }

        private object GetDefaultFieldsForAddItems(Dictionary<string, object> defaultValues)
        {
            string sql = string.Empty;
            if (defaultValues.Count > 0)
                sql = string.Join("", defaultValues.Select(f => ",[" + f.Key + "]").ToArray<string>());
            return sql;
        }

        private static string GetAddItemSyncSql()
        {
            return @"SET NOCOUNT ON 
                        BEGIN TRANSACTION T1;
                        INSERT INTO [{0}]([{1}] {2})
                        SELECT {7} {4} FROM (SELECT [{3}] as ID FROM [{5}] WITH(NOLOCK) {6}) as A;
                        COMMIT TRANSACTION T1;";

        }

        private string GetAddItemsAsyncSql()
        {
            return @"SET NOCOUNT ON 
                    DECLARE @__INDEX  INT =1
                    DECLARE @__PAGESIZE int ={0}
                    DECLARE @__loopCount int ={1} 
                  
                    WHILE @__INDEX<=@__loopCount
                    BEGIN
                        BEGIN TRANSACTION T1
	                       INSERT INTO [{2}]([{3}] {4})
		                   SELECT {9} {6} FROM (SELECT [{5}] as ID ,ROW_NUMBER ( ) OVER ( ORDER by [{5}] ASC ) as RowNum FROM [{7}]  WITH(NOLOCK) {8}) as A
		                   WHERE RowNum BETWEEN (@__INDEX - 1) * @__PAGESIZE + 1 AND (@__INDEX * @__PAGESIZE)
		                  
		                   SET @__INDEX=@__INDEX+1 
	                    COMMIT TRANSACTION T1
                    END";


        }


        public IDbCommand GetNewCommand(View view)
        {
            string connectionString = view.SystemView ? view.ConnectionString : view.Database.ConnectionString;
            return GetNewCommand(string.Empty, GetNewConnection(connectionString));
        }
        public virtual IDbCommand GetNewCommand()
        {
            return GetNewSqlSchema().GetNewCommand();
        }

        public string GetUserTrackingSql()
        {
            return @"DECLARE @PKTable TABLE (PK UNIQUEIDENTIFIER);
                    INSERT INTO website_TrackingCookie([Referer],[AllHeader],[QueryString],[LinkedGuid],[showDemo])
                    OUTPUT INSERTED.[GUID] INTO @PKTable
                    Values(@Referer,@AllHeader,@QueryString,@LinkedGuid,@showDemo) ;
                    SELECT PK FROM @PKTable";
        }

        public virtual bool IsLoginFailureException(Exception exception)
        {
            if (!(exception is SqlException))
                return false;

            foreach (SqlError error in ((SqlException)exception).Errors)
            {
                SqlErrorCode code = GetSqlErrorCodeFromInt(error.Number);
                if (code == SqlErrorCode.LoginFailed)
                    return true;
            }

            return false;
        }

        private SqlErrorCode GetSqlErrorCodeFromInt(int p)
        {
            switch (p)
            {
                case 40014:
                case 40054:
                case 40133:
                case 40506:
                case 40507:
                case 40508:
                case 40512:
                case 40516:
                case 40520:
                case 40521:
                case 40522:
                case 40523:
                case 40524:
                case 40525:
                case 40526:
                case 40527:
                case 40528:
                case 40606:
                case 40607:
                case 40636:
                    return SqlErrorCode.FeatureNotSupported;
            }

            try
            {
                return (SqlErrorCode)p;
            }
            catch
            {
                return SqlErrorCode.Unknown;
            }
        }

        public bool TestConnection(string connectionString, SqlProduct sqlProduct )
        {
            try
            {
                using (IDbConnection connection = GetNewConnection(sqlProduct, connectionString))
                {
                    connection.Open();
                    connection.Close();
                }

                return true;
            }
            catch
            {
                return false;
            }
        }

        public void LoadForeignKeys(string connectionString, SqlProduct sqlProduct, Dictionary<string, Dictionary<string, string>> cache)
        {
            SqlSchema schema = GetNewSqlSchema();

            using (IDbConnection connection = GetNewConnection(sqlProduct, connectionString))
            {
                connection.Open();
                using (IDbCommand command = GetNewCommand(schema.GetForeignKeyConstraints(), connection))
                {
                    using (IDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            string tableName = reader.GetString(reader.GetOrdinal("TableName"));
                            string columnName = reader.GetString(reader.GetOrdinal("ColumnName"));
                            string name = reader.GetString(reader.GetOrdinal("name"));

                            if (!cache.ContainsKey(tableName))
                            {
                                cache.Add(tableName, new Dictionary<string, string>());
    }
                            if (!cache[tableName].ContainsKey(columnName))
                            {
                                cache[tableName].Add(columnName, name);
                            }
                            else
                            {
                                cache[tableName][columnName] = name;
                            }
                        }
                    }
                }
            }
        }
        public virtual SqlProduct GetSqlProduct() {
            
                return SqlProduct.SqlServer;
        }

        
    }

    /// <summary>
    /// Documents some of the ErrorCodes from SQL/SQL Azure. 
    /// I have not included all possible errors, only the ones I thought useful for modifying runtime behaviors
    /// </summary>
    /// <remarks>
    /// Comments come from: http://social.technet.microsoft.com/wiki/contents/articles/sql-azure-connection-management-in-sql-azure.aspx
    /// </remarks>
    public enum SqlErrorCode : int
    {
        /// <summary>
        /// We don't recognize the error code returned
        /// </summary>
        Unknown = 0,

        /// <summary>
        /// A SQL feature/function used in the query is not supported. You must fix the query before it will work.
        /// This is a rollup of many more-specific SQL errors
        /// </summary>
        FeatureNotSupported = 1,

        /// <summary>
        /// Probable cause is server maintenance/upgrade. Retry connection immediately.
        /// </summary>
        TransientServerError = 40197,

        /// <summary>
        /// The server is throttling one or more resources. Reasons may be available from other properties
        /// </summary>
        ServerBusy = 40501,

        /// <summary>
        /// You have reached the per-database cap on worker threads. Investigate long running transactions and reduce server load. 
        /// http://social.technet.microsoft.com/wiki/contents/articles/1541.windows-azure-sql-database-connection-management.aspx#Throttling_Limits
        /// </summary>
        DatabaseWorkerThreadThrottling = 10928,

        /// <summary>
        /// The per-server worker thread cap has been reached. This may be partially due to load from other databases in a shared hosting environment (eg, SQL Azure).
        /// You may be able to alleviate the problem by reducing long running transactions.
        /// http://social.technet.microsoft.com/wiki/contents/articles/1541.windows-azure-sql-database-connection-management.aspx#Throttling_Limits
        /// </summary>
        ServerWorkerThreadThrottling = 10929,

        ExcessiveMemoryUsage = 40553,

        BlockedByFirewall = 40615,

        /// <summary>
        /// The database has reached the maximum size configured in SQL Azure
        /// </summary>
        ExceededDatabaseSizeQuota = 40544,

        /// <summary>
        /// A transaction ran for too long. This timeout seems to be 24 hours.
        /// </summary>
        /// <remarks>
        /// 24 hour limit taken from http://social.technet.microsoft.com/wiki/contents/articles/sql-azure-connection-management-in-sql-azure.aspx
        /// </remarks>
        TransactionRanTooLong = 40549,

        TooManyLocks = 40550,

        ExcessiveTempDBUsage = 40551,

        ExcessiveTransactionLogUsage = 40552,

        DatabaseUnavailable = 40613,

        CannotOpenServer = 40532,

        /// <summary>
        /// SQL Azure databases can have at most 128 firewall rules defined
        /// </summary>
        TooManyFirewallRules = 40611,

        /// <summary>
        /// Theoretically means the DB doesn't support encryption. However, this can be indicated incorrectly due to an error in the client library. 
        /// Therefore, even though this seems like an error that won't fix itself, it's actually a retryable error.
        /// </summary>
        /// <remarks>
        /// http://social.msdn.microsoft.com/Forums/en/ssdsgetstarted/thread/e7cbe094-5b55-4b4a-8975-162d899f1d52
        /// </remarks>
        EncryptionNotSupported = 20,

        /// <summary>
        /// User failed to connect to the database. This is probably not recoverable.
        /// </summary>
        /// <remarks>
        /// Some good info on more-specific debugging: http://blogs.msdn.com/b/sql_protocols/archive/2006/02/21/536201.aspx
        /// </remarks>
        LoginFailed = 18456,

        /// <summary>
        /// Failed to connect to the database. Could be due to configuration issues, network issues, bad login... hard to tell
        /// </summary>
        ConnectionFailed = 4060,

        /// <summary>
        /// Client tried to call a stored procedure that doesn't exist
        /// </summary>
        StoredProcedureNotFound = 2812,

        /// <summary>
        /// The data supplied is too large for the column
        /// </summary>
        StringOrBinaryDataWouldBeTruncated = 8152
    }

    public class Filter
    {
        public const string TOKEN = "&&%&";
        public const string EMPTY_WHERE_STATEMENT = " ({0} is null or {0} = '') ";
        public const string STRING_NOT_EMPTY_WHERE_STATEMENT = " ({0} is not null and {0} <> '') ";
        public const string NOT_EMPTY_WHERE_STATEMENT = " {0} is not null ";

        public static IDictionary<string, string> StringFilterComparisons;
        public static IDictionary<string, string> MathematicsFilterComparisons;

        static Filter()
        {
            StringFilterComparisons = new Dictionary<string, string>();
            StringFilterComparisons.Add("=", "Equals");
            StringFilterComparisons.Add("<>[null]", "Does Not Equal");
            StringFilterComparisons.Add("like%", "Begins With");
            StringFilterComparisons.Add("%like", "Ends With");
            StringFilterComparisons.Add("%like%", "Contains");
            StringFilterComparisons.Add("%not like%[null]", "Does Not Contain");
            StringFilterComparisons.Add("empty", "Is Blank");
            StringFilterComparisons.Add("!empty", "Is Not Blank");

            MathematicsFilterComparisons = new Dictionary<string, string>();
            MathematicsFilterComparisons.Add("=", "Equals");
            MathematicsFilterComparisons.Add("<>", "Does Not Equal");
            MathematicsFilterComparisons.Add(">", "Greater Than");
            MathematicsFilterComparisons.Add(">=", "Greater Than Or Equal To");
            MathematicsFilterComparisons.Add("<", "Less Than");
            MathematicsFilterComparisons.Add("<=", "Less Than Or Equal To");
            MathematicsFilterComparisons.Add("between", "Between");
            MathematicsFilterComparisons.Add("empty", "Is Blank");
            MathematicsFilterComparisons.Add("!empty", "Is Not Blank");
        }

        #region Properties (3)
        public string WhereStatement { get; set; }
        public string WhereStatementWithoutTablePrefix { get; set; }
        public IDataParameter[] Parameters { get; set; }

        #endregion Properties

        #region Methods (2)

        // Public Methods (1) 

        public string GetWhereStatementWithoutParameters()
        {
            string whereStatement = WhereStatementWithoutTablePrefix;

            foreach (IDataParameter parameter in Parameters)
            {
                string value = GetParameterValue(parameter);
                whereStatement = whereStatement.Replace(parameter.ParameterName + " ", value + " ").Replace(parameter.ParameterName + ")", value + ")");
            }

            return whereStatement.TrimStart(" and ".ToCharArray());
        }
        // Private Methods (1) 

        private string GetParameterValue(IDataParameter parameter)
        {
            string value = string.Empty;
            switch (parameter.DbType)
            {
                case DbType.Date:
                case DbType.DateTime:
                case DbType.DateTime2:
                case DbType.DateTimeOffset:
                    value = "CONVERT(DATETIME, '" + parameter.Value.ToString() + "', 102)";
                    break;
                case DbType.Boolean:
                case DbType.Binary:
                    value = ((bool)parameter.Value) ? "1" : "0";
                    break;
                case DbType.AnsiString:
                case DbType.AnsiStringFixedLength:
                case DbType.String:
                case DbType.Guid:
                case DbType.StringFixedLength:
                    value = "N'" + parameter.Value.ToString() + "'";
                    break;
                default:
                    value = parameter.Value.ToString();
                    break;


            }

            return value;
        }
        #endregion Methods

        public bool IsNosqlFilter { get; set; }
    }

    public class Duplicator : DataAccess.DataAccessObject
    {
        SqlAccess dataAccess = new SqlAccess();



        #region Methods (4)

        // Public Methods (1) 

        //public string Duplicate(View view, string pk)
        //{
        //    using (SqlConnection connection = new SqlConnection(view.ConnectionString))
        //    {
        //        connection.Open();
        //        SqlTransaction transaction = connection.BeginTransaction(IsolationLevel.ReadCommitted);
        //        SqlCommand command = new SqlCommand();
        //        command.Connection = connection;
        //        command.Transaction = transaction;

        //        string duplicatePk = Duplicate(view, pk, null, null, command, true);

        //        command.Transaction.Commit();

        //        return duplicatePk;
        //    }
        //}

        //protected string Duplicate(View view, string pk, Field fkField, string fkValue, SqlCommand command, bool root)
        //{
        //    DataRow row = view.GetDataRow(pk);

        //    bool rollback = false;

        //    string duplicatedPk = DuplicateRow(view, row, command, fkField, fkValue, out rollback, root);

        //    if (rollback)
        //        return null;

        //    foreach (ChildrenField field in view.Fields.Values.Where(f => f.FieldType == FieldType.Children))
        //    {
        //        if (field.IsDuplicable())
        //        {
        //            DataView childrenDataView = field.GetDataView(pk);
        //            foreach (DataRowView dataRowView in childrenDataView)
        //            {
        //                string childPK = field.ChildrenView.GetPkValue(dataRowView.Row);
        //                Field childFkField = field.GetEquivalentParentField();
        //                Duplicate(field.ChildrenView, childPK, childFkField, duplicatedPk, command, false);
        //            }
        //        }
        //    }

        //    return duplicatedPk;
        //}

        //public void Duplicate(View view, string fromPK, string toPK)
        //{
        //    Duplicate(view, fromPK, toPK, null);
        //}

        //public void Duplicate(View view, string fromPK, string toPK, SqlCommand command)
        //{
        //    Dictionary<string, View> views = new Dictionary<string, View>();
        //    if (command == null)
        //    {
        //        using (SqlConnection connection = new SqlConnection(view.ConnectionString))
        //        {
        //            connection.Open();
        //            SqlTransaction transaction = connection.BeginTransaction(IsolationLevel.ReadCommitted);
        //            command = new SqlCommand();
        //            command.Connection = connection;
        //            command.Transaction = transaction;

        //            Duplicate(view, fromPK, toPK, null, null, command, true, views);

        //            command.Transaction.Commit();

        //        }
        //    }
        //    else
        //    {
        //        Duplicate(view, fromPK, toPK, null, null, command, true, views);
        //    }
        //    foreach (string viewName in views.Keys)
        //    {
        //        SqlAccess.Cache.Clear(viewName);
        //    }
        //}


        public void Duplicate(View view, string fromPK, string toPK)
        {

            using (IDbConnection connection = GetNewConnection(view.Database.SqlProduct, view.ConnectionString))
            {
                connection.Open();
                IDbTransaction transaction = connection.BeginTransaction(IsolationLevel.ReadCommitted);
                using (IDbCommand command = GetNewCommand(string.Empty, connection))
                {
                    command.Connection = connection;
                    command.Transaction = transaction;

                    Dictionary<string, View> views = new Dictionary<string, View>();
                    Duplicate(view, fromPK, toPK, null, null, command, true, views);

                    command.Transaction.Commit();
                    foreach (string viewName in views.Keys)
                    {
                        SqlAccess.Cache.Clear(viewName);
                    }
                }
            }


        }


        protected string Duplicate(View view, string fromPK, string toPK, Field fkField, string fkValue, IDbCommand command, bool root, Dictionary<string, View> views)
        {
            DataRow row = view.GetDataRow(fromPK);

            bool rollback = false;

            string duplicatedPk = null;
            if (root)
            {
                duplicatedPk = toPK;
            }
            else
            {
                duplicatedPk = DuplicateRow(view, row, command, fkField, fkValue, out rollback, root);
            }

            if (!views.ContainsKey(view.Name))
            {
                views.Add(view.Name, view);
            }
            if (rollback)
                return null;

            foreach (ChildrenField field in view.Fields.Values.Where(f => f.FieldType == FieldType.Children))
            {
                if (field.IsDuplicable())
                {
                    DataView childrenDataView = field.GetDataView(fromPK);
                    foreach (DataRowView dataRowView in childrenDataView)
                    {
                        string childPK = field.ChildrenView.GetPkValue(dataRowView.Row);
                        Field childFkField = field.GetEquivalentParentField();
                        Duplicate(field.ChildrenView, childPK, childPK, childFkField, duplicatedPk, command, false, views);
                    }
                }
            }

            return duplicatedPk;
        }

        private string DuplicateRow(View view, DataRow row, IDbCommand command, Field fkField, string fkValue, out bool rollback, bool root)
        {
            int? pk = dataAccess.Create(view, GetDictionaryFromRow(view, row, fkField, fkValue), root, root ? view.GetPkValue(row) : null, null, null, null, null, command, out rollback);

            return pk.ToString();
        }

        private Dictionary<string, object> GetDictionaryFromRow(View view, DataRow row, Field fkField, string fkValue)
        {
            Dictionary<string, object> dictionary = new Dictionary<string, object>();

            //foreach (Field field in view.Fields.Values.Where(f => f.FieldType != FieldType.Children && !(f.HideInCreate && !f.Required) || f.Name == view.OrdinalColumnName))
            foreach (Field field in view.Fields.Values.Where(f => f.FieldType != FieldType.Children))
            {
                if (field.FieldType == FieldType.Parent)
                    if (field.Equals(fkField))
                        dictionary.Add(field.Name, fkValue);
                    else
                        dictionary.Add(field.Name, field.GetValue(row));
                else
                    dictionary.Add(field.Name, row[((ColumnField)field).DataColumn.ColumnName]);
            }

            return dictionary;
        }
        #endregion Methods
    }

    public enum LogicCondition
    {
        And,
        Or
    }

    public enum GetPKValueByDisplayValueStatus
    {
        NotFound,
        FoundUnique,
        FoundMoreThanOne
    }

    public interface ICopyPaste
    {
        #region Operations (3)
        IDbCommand GetCommand();

        void Edit(View view, Dictionary<string, object> values, string pk, BeforeEditEventHandler beforeEditCallback, BeforeEditInDatabaseEventHandler beforeEditInDatabaseEventHandler, AfterEditEventHandler afterEditBeforeCommitCallback, AfterEditEventHandler afterEditAfterCommitCallback);

        string PasteCommit(bool cancelCommit);

        void CloseConnections();

        #endregion Operations
    }

    public class CopyPasteConfig : ICopyPaste
    {
        #region Fields (2)
        private View view;
        private ConfigAccess Sql;

        #endregion Fields
        #region Constructors (1)
        public CopyPasteConfig(View view)
        {
            this.view = view;
            this.Sql = new ConfigAccess();
        }

        #endregion Constructors
        public IDbCommand GetCommand()
        {
            return null;
        }

        #region Methods (3)
        // Public Methods (3) 
        public void Edit(View view, Dictionary<string, object> values, string pk, BeforeEditEventHandler beforeEditCallback, BeforeEditInDatabaseEventHandler beforeEditInDatabaseEventHandler, AfterEditEventHandler afterEditBeforeCommitCallback, AfterEditEventHandler afterEditAfterCommitCallback)
        {
            Sql.Edit(view, values, pk, beforeEditCallback, beforeEditInDatabaseEventHandler, afterEditBeforeCommitCallback, afterEditAfterCommitCallback);
        }

        public string PasteCommit(bool cancelCommit)
        {
            string msg = "ok";


            return msg;
        }
        #endregion Methods

        public void CloseConnections()
        {
        }
    }

    public class CopyPaste : DataAccessObject, ICopyPaste
    {
        private IDbConnection cpConnection;
        private IDbCommand command;
        private SqlCommand sysCommand;
        private IDbTransaction transaction;
        private SqlTransaction sysTransaction;
        private SqlAccess Sql;
        private History history;
        private View view;
        private bool identicalSystemConnection;

        public void CloseConnections()
        {
            if (cpConnection != null && cpConnection.State != ConnectionState.Closed)
            {
                try
                {
                    cpConnection.Close();
                }
                catch { }
            }
            if (sysCommand != null && sysCommand.Connection != null && sysCommand.Connection.State != ConnectionState.Closed)
            {
                try
                {
                    sysCommand.Connection.Close();
                }
                catch { }
            }
        }

        public CopyPaste(View view)
        {
            this.view = view;

            string ConnectionString = view.ConnectionString;

            cpConnection = GetNewConnection(ConnectionString);

            cpConnection.Open();

            command = GetNewCommand(string.Empty, cpConnection);

            command.Connection = cpConnection;

            transaction = cpConnection.BeginTransaction(IsolationLevel.Serializable);



            command.Transaction = transaction;

            identicalSystemConnection = view.Database.IdenticalSystemConnection;
            if (identicalSystemConnection)
            {
                sysCommand = (SqlCommand)command;
            }
            else
            {
                string sysConnectionString = view.Database.SystemConnectionString;

                SqlConnection sysConnection = new SqlConnection(sysConnectionString);

                sysConnection.Open();

                sysCommand = new SqlCommand();

                sysCommand.Connection = sysConnection;

                sysTransaction = sysConnection.BeginTransaction(IsolationLevel.Serializable);

                sysCommand.Transaction = sysTransaction;

            }


            Sql = GetSqlAccess();

            history = new History();


        }

        protected virtual SqlAccess GetSqlAccess()
        {
            return new SqlAccess();
        }

        public IDbCommand GetCommand()
        {
            return this.command;
        }

        public void Edit(View view, Dictionary<string, object> values, string pk, BeforeEditEventHandler beforeEditCallback, BeforeEditInDatabaseEventHandler beforeEditInDatabaseEventHandler, AfterEditEventHandler afterEditBeforeCommitCallback, AfterEditEventHandler afterEditAfterCommitCallback)
        {

            Sql.Edit(view, values, pk, beforeEditCallback, beforeEditInDatabaseEventHandler, afterEditBeforeCommitCallback, afterEditAfterCommitCallback, command, sysCommand, null, null);


        }

        public string PasteCommit(bool cancelCommit)
        {
            string msg = "ok";

            if (command.Transaction != null)
            {
                if (cancelCommit)
                {
                    transaction.Rollback();
                    if (!identicalSystemConnection)
                        sysCommand.Transaction.Rollback();
                }
                else
                {
                    try
                    {
                        transaction.Commit();
                        if (!identicalSystemConnection)
                            sysTransaction.Commit();
                        SqlAccess.Cache.Clear(view.Name);
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        if (!identicalSystemConnection)
                            sysCommand.Transaction.Rollback();
                        msg = "Data didn't saved! <br /><br />Error:" + ex.Message;
                    }
                }
            }

            if (cpConnection != null)
            {
                cpConnection.Close();
                if (!identicalSystemConnection)
                    sysCommand.Connection.Close();

            }

            return msg;
        }


    }


    public class Importer : DataAccessObject
    {
        private IDbConnection importConnection;
        private IDbCommand command;
        private IDbTransaction transaction;
        private SqlAccess Sql;
        private History history;
        private int? userId = null;


        private IDbConnection sysConnection;
        private IDbCommand sysCommand;
        private IDbTransaction sysTransaction;

        private bool identicalConnectionString;

        //private bool doRollbackOnError;

        protected ISqlTextBuilder sqlTextBuilder;

        public Importer(string ConnectionString, string sysConnectionString, bool doRollbackOnError)
        {
            sqlTextBuilder = GetSqlTextBuilder();
            identicalConnectionString = string.IsNullOrEmpty(sysConnectionString) || ConnectionString.Equals(sysConnectionString);

            importConnection = GetNewConnection(ConnectionString);

            importConnection.Open();

            command = GetNewCommand("", importConnection);

            //command.Connection = importConnection;


            if (!identicalConnectionString)
            {
                sysConnection = new SqlConnection(sysConnectionString);

                sysConnection.Open();

                sysCommand = new SqlCommand();

                sysCommand.Connection = sysConnection;

            }
            else
            {
                sysCommand = command;
            }

            if (doRollbackOnError)
            {
                transaction = importConnection.BeginTransaction(IsolationLevel.Serializable);

                command.Transaction = transaction;

                if (!identicalConnectionString)
                {
                    sysTransaction = sysConnection.BeginTransaction(IsolationLevel.Serializable);

                    sysCommand.Transaction = sysTransaction;

                }

            }

            Sql = GetNewSqlAccess();

            history = GetNewHistory();

            //command.CommandType = CommandType.StoredProcedure;         

        }

        protected virtual SqlAccess GetNewSqlAccess()
        {
            return new SqlAccess();
        }

        protected virtual History GetNewHistory()
        {
            return new History();
        }

        protected virtual ISqlTextBuilder GetSqlTextBuilder()
        {
            return new SqlTextBuilder();
        }

        public string GetDisplayNameValue(View view, DataRow row)
        {
            string displayName = view.DisplayField.DisplayName;
            if (row.Table.Columns.Contains(displayName))
            {
                if (row.IsNull(displayName))
                {
                    throw new DuradosException("Cannot import row. The column '" + displayName + "' must have value.");
                }
                return row[displayName].ToString();
            }
            else
                throw new DuradosException("Cannot import row. The table is missing the column '" + displayName + "'.");

        }

        public void setUserId(int id)
        {
            userId = id;
        }

        public IDbCommand getCommand()
        {
            return command;
        }

        public IDbCommand getSysCommand()
        {
            return sysCommand;
        }


        public string Create(View view, Dictionary<string, object> values, DataRow row, ImportModes ImportMode)
        {
            int? id;

            if (ImportModes.InsertIgnoreUnique == ImportMode)
            {
                HandleImportDefaults(view, values);

                return Sql.Create(view, values, Sql.GetColumnNamesList(view, DataAction.Create, values), String.Empty, command, sysCommand, out id, history, userId);

            }


            GetPKValueByDisplayValueStatus status = GetPKValueByDisplayValueStatus.NotFound;

            bool IsViewHasUniqFields = true;

            string pk = string.Empty;

            if (row != null)
            {

                pk = Sql.GetPKsByUniqFieldsDisplayNames(view, row, out status);

                if (pk == string.Empty)
                {
                    string displayValue = GetDisplayNameValue(view, row);

                    pk = Sql.GetPKValueByDisplayValue(view, displayValue, out status);

                    IsViewHasUniqFields = false;
                }
            }

            if (status == GetPKValueByDisplayValueStatus.FoundMoreThanOne)
            {
                throw new DuradosException("Could not import row. The value in '" + GetViewDisplayColumnName(view, IsViewHasUniqFields) + "' exists more then once. Consider changing the unique identification.");
            }

            bool rowExist = pk != string.Empty; //view.IsRowAlreadyExistsByDisplayName(displayValue);

            if (rowExist)
            {
                if (ImportMode == ImportModes.Insert)
                {
                    throw new DuradosException("Could not import row. The value in '" + GetViewDisplayColumnName(view, IsViewHasUniqFields) + "' already exists.");
                }
                else
                {
                    HandleModifiedDateAndUser(view, values, userId);
                    Sql.Edit(view, values, pk, null, null, null, null, command, sysCommand, history, userId);
                }
            }
            else
            {
                if (ImportMode == ImportModes.Insert || ImportMode == ImportModes.UpdateOrInsert)
                {
                    HandleImportDefaults(view, values);

                    pk = Sql.Create(view, values, Sql.GetColumnNamesList(view, DataAction.Create, values), String.Empty, command, sysCommand, out id, history, userId);
                }
                else if (ImportMode == ImportModes.Update)
                {
                    throw new DuradosException("Could not update the row. The row doesn't exist " + GetViewDisplayColumnName(view, IsViewHasUniqFields) + ".");
                }
            }

            return pk;
        }

        private string GetViewDisplayColumnName(View view, bool ByUniq)
        {
            return ByUniq ? Sql.GetUniqFeildsDisplayNames(view) : sqlTextBuilder.EscapeDbObject(view.DisplayField.DisplayName);
        }

        private void HandleImportDefaults(View view, Dictionary<string, object> values)
        {
            foreach (Field field in view.Fields.Values.Where(f => f.IsExcluded(DataAction.Create) == false))
            {
                if (!values.ContainsKey(field.Name))
                {
                    values.Add(field.Name, field.DefaultValue ?? string.Empty);
                }
            }
        }

        private void HandleModifiedDateAndUser(View view, Dictionary<string, object> values, int? userId)
        {
            if (userId.HasValue && view.ModifiedBy != null)
            {
                view.ModifiedBy.ExcludeInUpdate = false;
                if (values.ContainsKey(view.ModifiedBy.Name))
                {
                    values[view.ModifiedBy.Name] = userId;
                }
                else
                {
                    values.Add(view.ModifiedBy.Name, userId.Value.ToString());

                }
            }

            Field modifiedDate = view.ModifiedDate;

            if (modifiedDate != null)
            {
                if (!values.ContainsKey(modifiedDate.Name))
                {
                    values.Add(modifiedDate.Name, DateTime.Now);
                }
                else
                {
                    values[modifiedDate.Name] = DateTime.Now;
                }
            }
        }


        public string CreateParentRecord(ParentField field, string displayValue)
        {

            int? id;
            string pk = string.Empty;

            Dictionary<string, object> values = new Dictionary<string, object>();

            values.Add(field.ParentView.DisplayField.Name, displayValue);

            try
            {
                pk = Sql.Create(field.ParentView, values, Sql.GetColumnNamesList(field.ParentView, DataAction.Create, values), String.Empty, command, sysCommand, out id, history, userId);

            }
            catch
            {
                pk = string.Empty;
            }

            return pk;
        }

        public string ImportCommit(bool cancelCommit, string viewName)
        {
            string msg = "ok";

            if (command.Transaction != null)
            {
                if (cancelCommit)
                {
                    transaction.Rollback();
                    if (!identicalConnectionString)
                        sysTransaction.Rollback();
                }
                else
                {
                    try
                    {
                        transaction.Commit();
                        if (!identicalConnectionString)
                            sysTransaction.Commit();
                        SqlAccess.Cache.Clear(viewName);
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        if (!identicalConnectionString)
                            sysTransaction.Rollback();
                        msg = "Data didn't imported! <br /><br />Error:" + ex.Message;
                    }
                }
            }

            return msg;
        }

        public void ImportEnd()
        {
            importConnection.Close();
            if (!identicalConnectionString)
                sysConnection.Close();

        }


        public void CloseConnections()
        {
            if (importConnection != null && importConnection.State != ConnectionState.Closed)
            {
                try
                {
                    importConnection.Close();
                }
                catch { }
            }
            if (sysConnection != null && sysConnection.State != ConnectionState.Closed)
            {
                try
                {
                    sysConnection.Close();
                }
                catch { }
            }
        }


        public void CleanUpDbTable(View view, string controllerName)
        {
            SqlAccess sqlAccess = new SqlAccess();
            view.Database.Logger.Log(controllerName, "Import", "DeleteTableData", null, 3, "Import - Replace:Start Delete all tabel data and reset Identity");
            sqlAccess.Truncate(command, view);
            view.Database.Logger.Log(controllerName, "Import", "DeleteTableData", null, 3, "Import - Replace:End  Delete all tabel data and reset Identity");

        }
    }

    public class History : DataAccessObject
    {
        public History()
        {
        }

        public virtual void SaveModel(string connectionString, View view, string oldValue, string newValue, int userId, string version)
        {
            using (IDbConnection connection = GetConnection(connectionString))
            {
                connection.Open();
                using (IDbCommand command = GetCommand())
                {
                    command.Connection = connection;
                    SaveModel(command, view, oldValue, newValue, userId, version);
                }
                connection.Close();
            }
            
        }
        public virtual void SaveModel(IDbCommand command, View view, string oldValue, string newValue, int userId, string version)
        {
            int id = SaveAction(command, view, "0", userId, 2, version, "Admin", null);

            string sql = "insert into durados_ChangeHistoryField(ChangeHistoryId, FieldName, ColumnNames, OldValue, NewValue, OldValueKey, NewValueKey) values (@ChangeHistoryId, @FieldName, @ColumnNames, @OldValue, @NewValue, @OldValueKey, @NewValueKey) ";

            command.CommandText = sql;

            command.Parameters.Clear();
            command.Parameters.Add(GetNewParameter(command, "@ChangeHistoryId", id));
            command.Parameters.Add(GetNewParameter(command, "@FieldName", "Model"));
            command.Parameters.Add(GetNewParameter(command, "@ColumnNames", "Model"));
            command.Parameters.Add(GetNewParameter(command, "@OldValue", oldValue));
            command.Parameters.Add(GetNewParameter(command, "@NewValue", newValue));
            command.Parameters.Add(GetNewParameter(command, "@OldValueKey", string.Empty));
            command.Parameters.Add(GetNewParameter(command, "@NewValueKey", string.Empty));

            command.ExecuteNonQuery();
            
        }

        public virtual int? SaveEdit(IDbCommand command, View view, DataRow prevRow, Dictionary<string, object> values, string pk, int userId, out OldNewValue[] oldNewValues, string version, string workspace)
        {
            //string sql = "insert into durados_ChangeHistory(ViewName, PK, ActionId, UpdateUserId) values (@ViewName, @PK, @ActionId, @UpdateUserId) ";
            //sql += "SELECT IDENT_CURRENT('durados_ChangeHistory') AS ID";

            //command.CommandText = sql;

            //command.Parameters.Clear();
            //command.Parameters.AddWithValue("@ViewName", view.Name);
            //command.Parameters.AddWithValue("@PK", pk);
            //command.Parameters.AddWithValue("@ActionId", 2);
            //command.Parameters.AddWithValue("@UpdateUserId", userId);

            //object scalar = command.ExecuteScalar();

            //int id = Convert.ToInt32(scalar);

            oldNewValues = GetChanges(view, prevRow, values, pk);

            if (oldNewValues.Length == 0)
            {
                return null;
            }

            int id = SaveAction(command, view, pk, userId, 2, version, workspace, null);

            string sql = "insert into durados_ChangeHistoryField(ChangeHistoryId, FieldName, ColumnNames, OldValue, NewValue, OldValueKey, NewValueKey) values (@ChangeHistoryId, @FieldName, @ColumnNames, @OldValue, @NewValue, @OldValueKey, @NewValueKey) ";

            foreach (OldNewValue oldNewValue in oldNewValues)
            {
                Field field = view.Fields[oldNewValue.FieldName];
                command.CommandText = sql;

                command.Parameters.Clear();
                command.Parameters.Add(GetNewParameter(command, "@ChangeHistoryId", id));
                command.Parameters.Add(GetNewParameter(command, "@FieldName", oldNewValue.FieldName));
                if (view.Database.IsConfig && view.Name == "Field")
                {
                    string fieldName2 = string.Empty;
                    if (view.DataTable.Columns.Contains("Name") && !prevRow.IsNull("Name"))
                    {
                        DataRow viewRow = prevRow.GetParentRow("Fields");
                        if (viewRow != null)
                            fieldName2 = viewRow["DisplayName"].ToString() + ": " + prevRow["DisplayName"].ToString();
                    }
                    command.Parameters.Add(GetNewParameter(command, "@ColumnNames", fieldName2));
                }
                else
                {
                    if (field.FieldType == FieldType.Children)
                    {
                        command.Parameters.Add(GetNewParameter(command, "@ColumnNames", ((ChildrenField)field).ChildrenView.Name));
                    }
                    else
                    {
                        command.Parameters.Add(GetNewParameter(command, "@ColumnNames", field.GetColumnsNames().Delimited()));
                    }
                }
                command.Parameters.Add(GetNewParameter(command, "@OldValue", oldNewValue.OldValue));
                command.Parameters.Add(GetNewParameter(command, "@NewValue", oldNewValue.NewValue));
                command.Parameters.Add(GetNewParameter(command, "@OldValueKey", oldNewValue.OldKey));
                command.Parameters.Add(GetNewParameter(command, "@NewValueKey", oldNewValue.NewKey));

                command.ExecuteNonQuery();
            }

            return id;
        }

        public virtual OldNewValue[] GetChanges(View view, DataRow prevRow, Dictionary<string, object> values, string pk)
        {
            List<OldNewValue> oldNewValues = new List<OldNewValue>();

            foreach (string fieldName in values.Keys)
            {
                if (view.IsSignatureField(fieldName))
                {
                    continue;
                }

                if (view.Fields.ContainsKey(fieldName))
                {
                    Field field = view.Fields[fieldName];
                    if (field.SaveHistory && field.FieldType != FieldType.Children && !field.ReadOnly)
                    {
                        string newValue;
                        string oldValue;
                        bool isDate = false;


                        if (field.FieldType == FieldType.Column && ((ColumnField)field).DataColumn.DataType == typeof(bool))
                        {
                            newValue = values[fieldName] == null ? string.Empty : values[fieldName].ToString();
                            if (prevRow.IsNull(((ColumnField)field).DataColumn.ColumnName))
                            {
                                oldValue = string.Empty;
                            }
                            else
                            {
                                oldValue = prevRow[((ColumnField)field).DataColumn.ColumnName].ToString();
                            }
                        }
                        else if (field.FieldType == FieldType.Column && ((ColumnField)field).DataColumn.DataType == typeof(DateTime))
                        {
                            isDate = true;
                            newValue = (values[fieldName] == null || values[fieldName].ToString() == string.Empty) ? string.Empty : ((ColumnField)field).ConvertDateToString(values[fieldName]);
                            object tempValue = prevRow[((ColumnField)field).DataColumn.ColumnName];

                            if (tempValue == DBNull.Value)
                            {
                                oldValue = string.Empty;
                            }
                            else
                            {
                                oldValue = ((ColumnField)field).ConvertDateToString(Convert.ChangeType(tempValue, ((ColumnField)field).DataColumn.DataType));
                            }

                        }
                        else
                        {
                            newValue = values[fieldName] == null ? string.Empty : values[fieldName].ToString();
                            oldValue = field.GetValue(prevRow);
                            oldValue = oldValue == null ? string.Empty : oldValue;
                        }

                        string oldKey = oldValue;
                        string newKey = newValue;

                        if (isDate)
                        {
                            if (oldValue != newValue)
                            {
                                oldNewValues.Add(new OldNewValue() { FieldName = fieldName, NewValue = newValue, OldValue = oldValue, NewKey = newKey, OldKey = oldKey });
                            }
                        }
                        else if (!CompareValues(field, newValue, oldValue))
                        {
                            if (field.FieldType == FieldType.Parent)
                            {
                                newValue = ((ParentField)field).GetDisplayValue(newValue);
                                newValue = newValue == null ? (field.NullString == null ? string.Empty : field.NullString) : newValue;
                            }
                            if (!(field.FieldType == FieldType.Column && ((ColumnField)field).DataColumn.DataType == typeof(bool)))
                            {
                                oldValue = field.ConvertToString(prevRow);
                            }
                            oldValue = oldValue == null ? (field.NullString == null ? string.Empty : field.NullString) : oldValue;
                            oldNewValues.Add(new OldNewValue() { FieldName = fieldName, NewValue = newValue, OldValue = oldValue, NewKey = newKey, OldKey = oldKey });
                        }
                    }
                }
            }

            oldNewValues.AddRange(GetChildrenChanges(view, values, pk));

            return oldNewValues.ToArray();
        }

        protected virtual bool CompareValues(Field field, string newValue, string oldValue)
        {
            if (field.FieldType == FieldType.Column)
            {
                return field.ConvertFromString(newValue).Equals(field.ConvertFromString(oldValue));
            }
            else
            {
                return newValue.Equals(oldValue);
            }
        }

        protected virtual OldNewValue[] GetChildrenChanges(View view, Dictionary<string, object> values, string pk)
        {
            List<OldNewValue> oldNewValues = new List<OldNewValue>();

            foreach (string fieldName in values.Keys)
            {
                if (view.Fields.ContainsKey(fieldName))
                {
                    Field field = view.Fields[fieldName];
                    if (field.FieldType == FieldType.Children && field.SaveHistory && ((ChildrenField)field).Persist)
                    {
                        List<string> newKeys = values[fieldName].ToString().Split(',').ToList();
                        List<string> oldKeys = GetChildrenKeys((ChildrenField)field, pk);

                        if (!CompareLists(newKeys, oldKeys))
                        {
                            string newValues = GetChildrenValues((ChildrenField)field, newKeys).ToArray().Delimited();
                            string oldValues = GetChildrenValues((ChildrenField)field, oldKeys).ToArray().Delimited();

                            oldNewValues.Add(new OldNewValue() { FieldName = fieldName, NewValue = newValues, OldValue = oldValues, OldKey = oldKeys.ToArray().Delimited(), NewKey = newKeys.ToArray().Delimited() });
                        }
                    }
                }
            }

            return oldNewValues.ToArray();
        }

        private void CleanList(List<string> keys)
        {
            for (int i = keys.Count - 1; i >= 0; i--)
            {
                if (string.IsNullOrEmpty(keys[i]))
                    keys.Remove(keys[i]);
            }
        }

        private bool CompareLists(List<string> newKeys, List<string> oldKeys)
        {
            CleanList(newKeys);
            CleanList(oldKeys);

            if (newKeys.Count != oldKeys.Count)
                return false;

            newKeys.Sort();
            oldKeys.Sort();

            for (int i = 0; i < newKeys.Count; i++)
            {
                if (newKeys[i] != oldKeys[i])
                    return false;
            }

            return true;
        }

        private List<string> GetChildrenKeys(ChildrenField childrenField, string fk)
        {
            childrenField = (ChildrenField)childrenField.Base;
            View childrenView = childrenField.ChildrenView;
            View view = childrenField.View;
            View parentView = null;
            ParentField parentField = null;
            ParentField fkField = null;

            foreach (ParentField field in childrenView.Fields.Values.Where(f => f.FieldType == FieldType.Parent))
            {
                if (!field.ParentView.Equals(view))
                {
                    parentField = field;
                    parentView = field.ParentView;
                }
                else
                {
                    fkField = field;
                }
            }


            int rowCount = 0;

            List<string> keys = new List<string>();
            if (!string.IsNullOrEmpty(fk))
            {
                Dictionary<string, object> filter = new Dictionary<string, object>();
                filter.Add(fkField.Name, fk);

                DataView childrenDataView = childrenView.FillPage(1, 1000000, filter, false, null, out rowCount, null, null);
                foreach (System.Data.DataRowView row in childrenDataView)
                {
                    string key = parentField.GetValue(row.Row);
                    keys.Add(key);
                }
            }


            return keys;
        }

        private List<string> GetChildrenValues(ChildrenField childrenField, List<string> keys)
        {
            View childrenView = childrenField.ChildrenView;
            View view = childrenField.View;
            View parentView = null;
            ParentField parentField = null;
            ParentField fkField = null;

            foreach (ParentField field in childrenView.Fields.Values.Where(f => f.FieldType == FieldType.Parent))
            {
                if (!field.ParentView.Equals(view))
                {
                    parentField = field;
                    parentView = field.ParentView;
                }
                else
                {
                    fkField = field;
                }
            }


            int rowCount = 0;
            DataView parentDataView = parentView.FillPage(1, 1000000, null, false, null, out rowCount, null, null);

            //Dictionary<object, object> keys = new Dictionary<object, object>();
            //if (!string.IsNullOrEmpty(fk))
            //{
            //    Dictionary<string, object> filter = new Dictionary<string, object>();
            //    filter.Add(fkField.Name, fk);

            //    DataView childrenDataView = childrenView.FillPage(1, 1000000, filter, false, null, out rowCount, null, null);
            //    foreach (System.Data.DataRowView row in childrenDataView)
            //    {
            //        string key = parentField.GetValue(row.Row);
            //        keys.Add(key, key);
            //    }
            //}

            Dictionary<string, string> dictionary = new Dictionary<string, string>();
            foreach (string key in keys)
            {
                if (!dictionary.ContainsKey(key))
                {
                    dictionary.Add(key, key);
                }
            }
            List<string> selectList = new List<string>();

            foreach (System.Data.DataRowView row in parentDataView)
            {
                string value = parentView.GetPkValue(row.Row);
                bool selected = dictionary.ContainsKey(value);

                if (selected)
                {
                    string text = parentView.DisplayField.GetValue(row.Row);
                    selectList.Add(text);
                }
            }


            return selectList;
        }


        public virtual int SaveCreate(IDbCommand command, View view, string pk, int userId, string version, string workspace)
        {
            return SaveAction(command, view, pk, userId, 1, version, workspace, null);
        }


        public virtual int SaveDelete(IDbCommand command, View view, string pk, int userId, string version, string workspace, DataRow deletedRow)
        {
            string comments = GetComments(deletedRow);
            int id = SaveAction(command, view, pk, userId, 3, version, workspace, comments);

            string sql = "insert into durados_ChangeHistoryField(ChangeHistoryId, FieldName, ColumnNames, OldValue, NewValue, OldValueKey, NewValueKey) values (@ChangeHistoryId, @FieldName, @ColumnNames, @OldValue, @NewValue, @OldValueKey, @NewValueKey) ";

            command.CommandText = sql;

            command.Parameters.Clear();
            command.Parameters.Add(GetNewParameter(command, "@ChangeHistoryId", id));
            command.Parameters.Add(GetNewParameter(command, "@FieldName", view.DisplayField.Name));
            command.Parameters.Add(GetNewParameter(command, "@ColumnNames", view.DisplayColumn));
            command.Parameters.Add(GetNewParameter(command, "@OldValue", view.DisplayField.ConvertToString(deletedRow) ?? string.Empty));
            command.Parameters.Add(GetNewParameter(command, "@OldValueKey", view.DisplayField.GetValue(deletedRow) ?? string.Empty));
            command.Parameters.Add(GetNewParameter(command, "@NewValue", string.Empty));
            command.Parameters.Add(GetNewParameter(command, "@NewValueKey", string.Empty));

            command.ExecuteNonQuery();

            return id;
        }

        private string GetComments(DataRow row)
        {
            if (row == null)
                return string.Empty;

            string comments = string.Empty;

            foreach (DataColumn column in row.Table.Columns)
            {
                comments += column.ColumnName + "=" + row[column.ColumnName].ToString() + ";";
            }

            int maxLength = 2000;
            if (comments.Length > maxLength)
                comments = comments.Remove(maxLength);

            return comments;
        }

        protected virtual int SaveAction(IDbCommand command, View view, string pk, int userId, int action, string version, string workspace, string comments)
        {
            return SaveAction(command, view.Base.Name, pk, userId, action, version, workspace, comments);
        }

        protected virtual int SaveAction(IDbCommand command, string viewName, string pk, int userId, int action, string version, string workspace, string comments)
        {
            string sql = "insert into durados_ChangeHistory(ViewName, PK, ActionId, UpdateUserId, Version, Workspace, Comments) values (@ViewName, @PK, @ActionId, @UpdateUserId, @Version, @Workspace, @Comments); ";
            sql +=GetSqlTextBuilder().GetLastInsertedRow("durados_ChangeHistory");


            if (comments == null)
                comments = string.Empty;

            command.CommandText = sql;

            command.Parameters.Clear();
            command.Parameters.Add(GetNewParameter(command, "@ViewName", viewName));
            command.Parameters.Add(GetNewParameter(command, "@PK", pk));
            command.Parameters.Add(GetNewParameter(command, "@ActionId", action));
            command.Parameters.Add(GetNewParameter(command, "@UpdateUserId", userId));
            command.Parameters.Add(GetNewParameter(command, "@Version", version));
            command.Parameters.Add(GetNewParameter(command, "@Workspace", workspace));
            command.Parameters.Add(GetNewParameter(command, "@Comments", comments));

            object scalar = command.ExecuteScalar();

            return Convert.ToInt32(scalar);

        }

        protected virtual ISqlTextBuilder GetSqlTextBuilder()
        {
            return new SqlTextBuilder();
        }

        public static int Save(string viewName, string pk, int userId, int action, string connectionString, string version, string workspace)
        {
            return Save(viewName, pk, userId, action, connectionString, version, workspace, null);
        }

        public static int Save(string viewName, string pk, int userId, int action, string connectionString, string version, string workspace, string comments)
        {
            SqlProduct sqlProduct = GetProduct(connectionString);
            
            History history = GetHistory(sqlProduct);

            IDbCommand command =GetCommand(sqlProduct) ;
            command.Connection =GetConnection(sqlProduct,connectionString);
            try
            {
                command.Connection.Open();
                return history.SaveAction(command, viewName, pk, userId, action, version, workspace, comments);
            }
            finally
            {
                command.Connection.Close();
            }
        }

        private static IDbConnection GetConnection(SqlProduct sqlProduct, string connectionString)
        {
            if (sqlProduct== SqlProduct.MySql)
                return new MySql.Data.MySqlClient.MySqlConnection();
            return new SqlConnection(connectionString);
          
        }

        private static IDbCommand GetCommand(SqlProduct sqlProduct)
        {
            if (sqlProduct== SqlProduct.MySql)
                return new MySql.Data.MySqlClient.MySqlCommand();
            return new SqlCommand();
        }

        public static History GetHistory(SqlProduct sqlProduct)
        {
            if (sqlProduct == SqlProduct.MySql)
                return new MySqlHistory();
            return new History();
        }

        public static SqlProduct GetProduct(string connectionString)
        {
            if (!string.IsNullOrEmpty(connectionString))
            {
                if (MySqlAccess.IsMySqlConnectionString(connectionString))
                    return SqlProduct.MySql;
            }
            return SqlProduct.SqlServer;

        }

        public virtual void Commit(Guid guid, int[] ids, string connectionString)
        {
            if (ids == null || ids.Count() == 0)
                return;

            IDbCommand command = GetCommand();
            command.Connection = GetConnection(connectionString);
            try
            {
                command.Connection.Open();
                string sql = "update durados_ChangeHistory set TransactionName = '" + guid.ToString() + "' where id in (" + ids.Delimited() + ")";
                command.CommandText = sql;
                command.ExecuteNonQuery();
            }
            finally
            {
                command.Connection.Close();
            }
        }

        protected virtual IDbConnection GetConnection(string connectionString)
        {
            return new SqlConnection(connectionString);
        }

        protected virtual IDbCommand GetCommand()
        {
            return new SqlCommand();
        }


        public string GetPreviousValue(string viewName, string fieldName, string pk, string currentValue, string connectionString)
        {
            IDbCommand command = GetCommand();
            command.Connection = GetConnection(connectionString); ;
            try
            {
                command.Connection.Open();
                string sql = GetHistoryOldValueSelect();
                command.CommandText = sql;
                command.Parameters.Add(GetNewParameter(command, "@ViewName", viewName));
                command.Parameters.Add(GetNewParameter(command, "@FieldName", fieldName));
                command.Parameters.Add(GetNewParameter(command, "@PK", pk));
                command.Parameters.Add(GetNewParameter(command, "@NewValue", currentValue));

                object scalar = command.ExecuteScalar();
                if (scalar != null && scalar != DBNull.Value)
                    return scalar.ToString();

                return null;
            }
            finally
            {
                command.Connection.Close();
            }
        }

      

        protected virtual string GetHistoryOldValueSelect()
        {
            return "SELECT TOP (1) OldValueKey " +
                                        "FROM      durados_ChangeHistory INNER JOIN " +
                                        "durados_ChangeHistoryField ON durados_ChangeHistory.id = durados_ChangeHistoryField.ChangeHistoryId " +
                                        "WHERE  (ViewName = @ViewName) AND (PK = @PK) AND (FieldName = @FieldName) AND (NewValueKey = @NewValue) " +
                                        "ORDER BY UpdateDate DESC";
        }
    }

    public class Cache
    {
        private Dictionary<string, CacheView> cacheViews;

        public Cache()
        {
            Refresh();
        }

        public void Refresh()
        {
            cacheViews = new Dictionary<string, CacheView>();
        }

        public void Clear(string viewName)
        {
            if (HasCache(viewName))
            {
                lock (this)
                {
                    cacheViews.Remove(viewName);
                }
            }
        }

        public bool HasCache(string viewName)
        {
            return cacheViews.ContainsKey(viewName);
        }

        internal void Get(string viewName, DataTable parentTable)
        {
            if (!HasCache(viewName))
                return;

            cacheViews[viewName].Get(parentTable);

        }

        internal void Set(string viewName, DataTable parentTable)
        {
            cacheViews.Add(viewName, new CacheView(viewName));

            cacheViews[viewName].Set(parentTable);

        }

        internal class CacheView
        {
            string viewName;
            DataTable cacheTable;

            internal CacheView(string viewName)
            {
                this.viewName = viewName;
            }

            internal void Get(DataTable parentTable)
            {
                if (parentTable.Rows.Count > 0)
                    parentTable.Rows.Clear();

                foreach (DataRow cacheRow in cacheTable.Rows)
                {
                    DataRow row = parentTable.NewRow();

                    foreach (DataColumn cacheColumn in cacheTable.Columns)
                    {
                        if (!parentTable.Columns.Contains(cacheColumn.ColumnName))
                        {
                            parentTable.Columns.Add(cacheColumn.ColumnName, cacheColumn.DataType);
                        }
                        row[cacheColumn.ColumnName] = cacheRow[cacheColumn.ColumnName];
                    }

                    parentTable.Rows.Add(row);
                }

            }

            internal void Set(DataTable parentTable)
            {
                lock (this)
                {
                    cacheTable = parentTable.Copy();
                }
            }


        }
    }

    public class Rest : IRest
    {
        SqlAccess sqlAccess = null;
        private SqlProduct GetProduct(View view)
        {
            return view.SystemView ? view.Database.SystemSqlProduct : view.Database.SqlProduct;
        }
        private SqlAccess GetSqlAccess(View view)
        {
            if (view.Database.IsConfig)
            {
                return new ConfigAccess();
            }
            if (sqlAccess == null)
            {

                switch (GetProduct(view))
                {
                    case SqlProduct.MySql:
                        sqlAccess = new MySqlAccess();
                        break;
                    case SqlProduct.Postgre:
                        sqlAccess = new PostgreAccess();
                        break;
                    case SqlProduct.Oracle:
                        sqlAccess = new OracleAccess();
                        break;

                    default:
                        sqlAccess = new SqlAccess();
                        break;
                }
            }

            return sqlAccess;
        }
        public static SqlAccess GetSqlAccess(SqlProduct sqlProduct)
        {
                       
                switch (sqlProduct)
                {
                    case SqlProduct.MySql:
                        return new MySqlAccess();
                    case SqlProduct.Postgre:
                        return new PostgreAccess();
                    case SqlProduct.Oracle:
                        return new OracleAccess();
      
                    default:
                        return new SqlAccess();
                }
            
           
        }
        protected string Create(View view, Dictionary<string, object> values, string insertAbovePK, IDbCommand command, IDbCommand sysCommand, out int? id, BeforeCreateEventHandler beforeCreateCallback, BeforeCreateInDatabaseEventHandler beforeCreateInDatabaseEventHandler, AfterCreateEventHandler afterCreateBeforeCommitCallback, AfterCreateEventHandler afterCreateAfterCommitCallback)
        {
            bool identicalSystemConnection = view.Database.IdenticalSystemConnection;
            id = null;
            CreateEventArgs createEventArgs = null;
            createEventArgs = new CreateEventArgs(view, values, null, command, sysCommand);

            if (beforeCreateCallback != null)
                beforeCreateCallback(this, createEventArgs);

            if (createEventArgs.Cancel)
            {
                command.Transaction.Rollback();
                return null;
            }

            List<string> columnNames = GetSqlAccess(view).GetColumnNamesList(view, DataAction.Create, values);
            createEventArgs.ColumnNames = columnNames;

            if (beforeCreateInDatabaseEventHandler != null)
                beforeCreateInDatabaseEventHandler(this, createEventArgs);

            if (createEventArgs.Cancel)
            {
                command.Transaction.Rollback();
                if (!identicalSystemConnection)
                {
                    sysCommand.Transaction.Rollback();
                }
                return null;
            }

            string pk = GetSqlAccess(view).Create(view, values, columnNames, insertAbovePK, command, sysCommand, out id, createEventArgs.History, createEventArgs.UserId);


            if (afterCreateBeforeCommitCallback != null)
            {
                createEventArgs = new CreateEventArgs(view, values, pk, command, sysCommand);
                afterCreateBeforeCommitCallback(this, createEventArgs);
            }
            //view.OnAfterCreateBeforeCommit(createEventArgs);
            if (createEventArgs.Cancel)
            {
                if (id.HasValue)
                {
                    string name = view.DataTable.PrimaryKey[0].ColumnName;
                    values.Remove(name);
                }

                command.Transaction.Rollback();
                if (!identicalSystemConnection)
                {
                    sysCommand.Transaction.Rollback();
                }
                id = null;
            }
            else
            {
                if (afterCreateAfterCommitCallback != null)
                {
                    afterCreateAfterCommitCallback(this, createEventArgs);
                }
            
                SqlAccess.Cache.Clear(view.Name);
            }

            
            return pk;
        }

        public string Create(View view, Dictionary<string, object> deepObject, bool deep, BeforeCreateEventHandler beforeCreateCallback, BeforeCreateInDatabaseEventHandler beforeCreateInDatabaseCallback, AfterCreateEventHandler afterCreateBeforeCommitCallback, AfterCreateEventHandler afterCreateAfterCommitCallback)
        {
            return Create(view, new Dictionary<string, object>[1] { deepObject }, deep, beforeCreateCallback, beforeCreateInDatabaseCallback, afterCreateBeforeCommitCallback, afterCreateAfterCommitCallback);
        }

        /*
        public string Create(View view, Dictionary<string, object>[] deepObjects, bool deep, BeforeCreateEventHandler beforeCreateCallback, BeforeCreateInDatabaseEventHandler beforeCreateInDatabaseCallback, AfterCreateEventHandler afterCreateBeforeCommitCallback, AfterCreateEventHandler afterCreateAfterCommitCallback)
        {
            string pk = string.Empty;

            IDbCommand sysCommand = null;
            bool identicalSystemConnection = view.Database.IdenticalSystemConnection;

            try
            {
                using (IDbConnection connection = SqlAccess.GetNewConnection(GetProduct(view)))
                {
                    connection.ConnectionString = view.ConnectionString;
                    connection.Open();
                    IDbTransaction transaction = connection.BeginTransaction(IsolationLevel.ReadCommitted);
                    IDbCommand command = GetSqlAccess(view).GetNewCommand(view);
                    command.Connection = connection;
                    command.Transaction = transaction;


                    IDbTransaction sysTransaction = null;
                    if (!identicalSystemConnection)
                    {
                        sysCommand = GetSqlAccess(view.Database.SystemSqlProduct).GetNewCommand();
                        IDbConnection sysConnection = SqlAccess.GetNewConnection(view.Database.SystemSqlProduct,view.Database.SystemConnectionString);
                        sysConnection.Open();
                        sysTransaction = sysConnection.BeginTransaction(IsolationLevel.ReadCommitted);
                        sysCommand.Connection = sysConnection;
                        sysCommand.Transaction = sysTransaction;

                    }
                    else
                    {
                        sysCommand = command;
                    }

                    int i = 0;
                    foreach (var deepObject in deepObjects)
                    {
                        try
                        {
                            pk += Create(view, deepObject, deep, command, sysCommand, beforeCreateCallback, beforeCreateInDatabaseCallback, afterCreateBeforeCommitCallback, afterCreateAfterCommitCallback) + ";";
                        }
                        catch (Exception exception)
                        {
                            if (deepObjects.Length > 1)
                            {
                                throw new DuradosException(string.Format("Bulk creation failed at item {0} (zero based). Activity rolled back. Message: {1}", i, exception.Message), exception);
                            }
                            else
                                throw exception;
                        }
                        i++;
                    }

                    pk = pk.TrimEnd(';');
                    //Change by Mirih
                    transaction.Commit();
                    if (!identicalSystemConnection)
                    {
                        sysTransaction.Commit();
                    }

                    foreach (var deepObject in deepObjects)
                    {
                        CreateEventArgs createEventArgs = new CreateEventArgs(view, GetValues(view, deepObject), pk, null, null);
                        if (afterCreateAfterCommitCallback != null)
                            afterCreateAfterCommitCallback(this, createEventArgs);
                        view.SendRealTimeEvent(pk, Crud.create);
                    }

                    SqlAccess.Cache.Clear(view.Name);
                }
            }
            finally
            {
                if (!identicalSystemConnection)
                {
                    sysCommand.Connection.Close();
                }
            }

            return pk;
        }
        */

        private bool IsSameResource(IDbConnection connection, string connectionString)
        {
            return connectionString.Contains(connection.Database);
        }

        public string Create(View view, Dictionary<string, object>[] deepObjects, bool deep, BeforeCreateEventHandler beforeCreateCallback, BeforeCreateInDatabaseEventHandler beforeCreateInDatabaseCallback, AfterCreateEventHandler afterCreateBeforeCommitCallback, AfterCreateEventHandler afterCreateAfterCommitCallback, IDbCommand command = null, IDbCommand sysCommand = null)
        {
            string pk = string.Empty;

            IDbConnection connection = null;
            bool identicalSystemConnection = view.Database.IdenticalSystemConnection;
            IDbTransaction transaction = null;
            IDbConnection sysConnection = null;
            IDbTransaction sysTransaction = null;

            try
            {
                if (command == null || command.Connection.State == ConnectionState.Closed || !IsSameResource(command.Connection, view.ConnectionString))
                {
                    connection = SqlAccess.GetNewConnection(GetProduct(view));

                    connection.ConnectionString = view.ConnectionString;
                    connection.Open();
                    transaction = connection.BeginTransaction(IsolationLevel.ReadCommitted);
                    command = GetSqlAccess(view).GetNewCommand(view);
                    command.Connection = connection;
                    command.Transaction = transaction;
                }

                if (sysCommand == null || sysCommand.Connection.State == ConnectionState.Closed || !IsSameResource(command.Connection, view.ConnectionString))
                {
                    if (!identicalSystemConnection)
                    {
                        sysCommand = GetSqlAccess(view.Database.SystemSqlProduct).GetNewCommand();
                        sysConnection = SqlAccess.GetNewConnection(view.Database.SystemSqlProduct, view.Database.SystemConnectionString);
                        sysConnection.Open();
                        sysTransaction = sysConnection.BeginTransaction(IsolationLevel.ReadCommitted);
                        sysCommand.Connection = sysConnection;
                        sysCommand.Transaction = sysTransaction;

                    }
                    else
                    {
                        sysCommand = command;
                    }
                }

                List<AfterCommitDispatcher> afterCommitDispatcherList = new List<AfterCommitDispatcher>();
                int i = 0;
                foreach (var deepObject in deepObjects)
                {
                    AfterCommitDispatcher afterCommitDispatcher = new AfterCommitDispatcher(null, afterCreateAfterCommitCallback, null);
                    try
                    {
                        pk += Create(view, deepObject, deep, command, sysCommand, beforeCreateCallback, beforeCreateInDatabaseCallback, afterCreateBeforeCommitCallback, afterCommitDispatcher.AfterCreateAfterCommitCallback) + ";";
                        afterCommitDispatcherList.Add(afterCommitDispatcher);
                    }
                    catch (Exception exception)
                    {
                        if (deepObjects.Length > 1)
                        {
                            throw new DuradosException(string.Format("Bulk creation failed at item {0} (zero based). Activity rolled back. Message: {1}", i, exception.Message), exception);
                        }
                        else
                            throw exception;
                    }
                    i++;
                }

                pk = pk.TrimEnd(';');
                //Change by Mirih
                if (connection != null)
                {
                    transaction.Commit();
                    if (!identicalSystemConnection)
                    {
                        try
                        {
                            if (sysTransaction != null && sysConnection.State != ConnectionState.Closed)
                                sysTransaction.Commit();
                        }
                        catch { }
                    }

                    foreach (var afterCommitDispatcher in afterCommitDispatcherList)
                    {
                        afterCommitDispatcher.Dispatch();
                    }
                }

                foreach (var deepObject in deepObjects)
                {
                    //CreateEventArgs createEventArgs = new CreateEventArgs(view, GetValues(view, deepObject), pk, null, null);
                    //if (afterCreateAfterCommitCallback != null)
                    //    afterCreateAfterCommitCallback(this, createEventArgs);
                    view.SendRealTimeEvent(pk, Crud.create);
                }

                SqlAccess.Cache.Clear(view.Name);

            }
            finally
            {
                if (connection != null)
                {
                    try
                    {
                        connection.Close();
                    }
                    catch { }
                    if (!identicalSystemConnection)
                    {
                        if (sysCommand != null)
                        {
                            try
                            {
                                sysCommand.Connection.Close();
                            }
                            catch { }
                        }
                    }
                }
            }

            return pk;
        }

        protected virtual Field GetField(View view, string name)
        {
            Field field = null;
            
            Field[] fields = view.GetFieldsByJsonName(name);
            if (fields != null && fields.Length == 1)
            {
                field = fields[0];
            }
            else
            {
                if (view.Fields.ContainsKey(name))
                {
                    field = view.Fields[name];
                }
                else
                {
                    fields = view.GetFieldsByDisplayName(name);

                    if (fields != null && fields.Length == 1)
                    {
                        field = fields[0];
                    }
                    else
                    {
                        field = view.GetFieldByColumnNames(name);
                    }
                }
            }

            return field;
        }

        protected virtual Dictionary<string, object> GetValues(View view, Dictionary<string, object> deepObject, bool createParent = false, IDbCommand command = null, IDbCommand sysCommand = null, BeforeCreateEventHandler beforeCreateCallback = null, BeforeCreateInDatabaseEventHandler beforeCreateInDatabaseCallback = null, AfterCreateEventHandler afterCreateBeforeCommitCallback = null, AfterCreateEventHandler afterCreateAfterCommitCallback = null)
        {
            Dictionary<string, object> values = new Dictionary<string, object>();

            foreach (string key in deepObject.Keys)
            {
                Field field = GetField(view, key);

                if (field != null)
                {
                    if (field.FieldType != FieldType.Children)
                    {
                        // handle parents?
                        if (field.FieldType == FieldType.Parent && deepObject[key] is Dictionary<string, object>)
                        {
                            var parentValues = (Dictionary<string, object>)deepObject[key];
                            string pk = GetPkFromMetadata(parentValues);
                            if (pk == null)
                                pk = GetPkFromObject(view, parentValues);
                            if (pk == null && createParent)
                            {
                                pk = CreateParentObject((ParentField)field, parentValues, command, sysCommand, beforeCreateCallback, beforeCreateInDatabaseCallback, afterCreateBeforeCommitCallback, afterCreateAfterCommitCallback);
                            }
                            values.Add(field.Name, pk);
                        }
                        else
                        {
                            values.Add(field.Name, deepObject[key]);
                        }
                    }
                }
                else
                {
                    values.Add(key, deepObject[key]);
                }
            }

            return values;
        }

        private string CreateParentObject(ParentField parentField, Dictionary<string, object> parentValues, IDbCommand command, IDbCommand sysCommand, BeforeCreateEventHandler beforeCreateCallback, BeforeCreateInDatabaseEventHandler beforeCreateInDatabaseCallback, AfterCreateEventHandler afterCreateBeforeCommitCallback, AfterCreateEventHandler afterCreateAfterCommitCallback)
        {
            return Create(parentField.ParentView, parentValues, true, beforeCreateCallback, beforeCreateInDatabaseCallback, afterCreateBeforeCommitCallback, afterCreateAfterCommitCallback);
        }

        protected virtual string Create(View view, Dictionary<string, object> deepObject, bool deep, IDbCommand command, IDbCommand sysCommand, BeforeCreateEventHandler beforeCreateCallback, BeforeCreateInDatabaseEventHandler beforeCreateInDatabaseCallback, AfterCreateEventHandler afterCreateBeforeCommitCallback, AfterCreateEventHandler afterCreateAfterCommitCallback)
        {
            Dictionary<string, object> values = GetValues(view, deepObject, true, command, sysCommand, beforeCreateCallback, beforeCreateInDatabaseCallback, afterCreateBeforeCommitCallback, afterCreateAfterCommitCallback);

            int? id = null;

            string pk = Create(view, values, null, command, sysCommand, out id, beforeCreateCallback, beforeCreateInDatabaseCallback, afterCreateBeforeCommitCallback, afterCreateAfterCommitCallback);

            if (deep)
            {
                foreach (string key in deepObject.Keys)
                {
                    Field field = GetField(view, key);

                    if (field != null && field.FieldType == FieldType.Children)
                    {
                        ChildrenField childrenField = (ChildrenField)field;
                        View childrenView = childrenField.ChildrenView;

                        var children = (object[])deepObject[key];
                        foreach (Dictionary<string, object> child in children)
                        {
                            SetParent(child, pk, childrenField);
                            Create(childrenView, child, deep, command, sysCommand, beforeCreateCallback, beforeCreateInDatabaseCallback, afterCreateBeforeCommitCallback, afterCreateAfterCommitCallback);

                        }
                    }
                }
            }

            return pk;
        }

        private void SetParent(Dictionary<string, object> child, string pk, ChildrenField childrenField)
        {
            string fieldName = childrenField.GetEquivalentParentField().JsonName;
            if (child.ContainsKey(fieldName))
            {
                child[fieldName] = pk;
            }
            else
            {
                child.Add(fieldName, pk);
            }
        }


        public int Update(View view, Dictionary<string, object> deepObject, string pk, bool deep, BeforeEditEventHandler beforeEditCallback, BeforeEditInDatabaseEventHandler beforeEditInDatabaseCallback, AfterEditEventHandler afterEditBeforeCommitCallback, AfterEditEventHandler afterEditAfterCommitCallback, BeforeCreateEventHandler beforeCreateCallback = null, BeforeCreateInDatabaseEventHandler beforeCreateInDatabaseCallback = null, AfterCreateEventHandler afterCreateBeforeCommitCallback = null, AfterCreateEventHandler afterCreateAfterCommitCallback = null, bool overwrite = false, BeforeDeleteEventHandler beforeDeleteCallback = null, AfterDeleteEventHandler afterDeleteBeforeCommitCallback = null, AfterDeleteEventHandler afterDeleteAfterCommitCallback = null, IDbCommand command = null, IDbCommand sysCommand = null)
        {
            bool identicalSystemConnection = view.Database.IdenticalSystemConnection;
            IDbConnection connection = null;
            IDbTransaction transaction = null;
            IDbTransaction sysTransaction = null;
            int affected = 0;

            try
            {
                if (command == null || command.Connection.State == ConnectionState.Closed || !IsSameResource(command.Connection, view.ConnectionString))
                {
                    connection = SqlAccess.GetNewConnection(GetProduct(view));

                    connection.ConnectionString = view.ConnectionString;
                    connection.Open();
                    transaction = connection.BeginTransaction(IsolationLevel.ReadCommitted);
                    command = GetSqlAccess(view).GetNewCommand(view);
                    command.Connection = connection;
                    command.Transaction = transaction;
                }

                if (sysCommand == null || sysCommand.Connection.State == ConnectionState.Closed || !IsSameResource(command.Connection, view.ConnectionString))
                {
                    if (!identicalSystemConnection)
                    {
                        sysCommand = GetSqlAccess(view.Database.SystemSqlProduct).GetNewCommand();
                        IDbConnection sysConnection = SqlAccess.GetNewConnection(view.Database.SystemSqlProduct, view.Database.SystemConnectionString);
                        sysConnection.Open();
                        sysTransaction = sysConnection.BeginTransaction(IsolationLevel.ReadCommitted);
                        sysCommand.Connection = sysConnection;
                        sysCommand.Transaction = sysTransaction;

                    }
                    else
                    {
                        sysCommand = command;
                    }
                }

                AfterCommitDispatcher dispatcher = new AfterCommitDispatcher(afterEditAfterCommitCallback, afterCreateAfterCommitCallback, afterDeleteAfterCommitCallback);

                affected = Update(view, deepObject, pk, deep, command, sysCommand, beforeEditCallback, beforeEditInDatabaseCallback, afterEditBeforeCommitCallback, dispatcher.AfterEditAfterCommitCallback, beforeCreateCallback, beforeCreateInDatabaseCallback, afterCreateBeforeCommitCallback, dispatcher.AfterCreateAfterCommitCallback, overwrite, beforeDeleteCallback, afterDeleteBeforeCommitCallback, dispatcher.AfterDeleteAfterCommitCallback);

                if (connection != null)
                {
                    if (affected == 0)
                    {
                        transaction.Rollback();
                        if (!identicalSystemConnection)
                        {
                            if (sysTransaction != null)
                                try
                                {
                                    sysTransaction.Rollback();
                                }
                                catch { }
                        }
                    }
                    else
                    {

                        transaction.Commit();
                        if (!identicalSystemConnection)
                        {
                            if (sysTransaction != null)
                                try
                                {
                                    sysTransaction.Commit();
                                }
                                catch { }
                        }
                        dispatcher.Dispatch();
                    }
                }
                SqlAccess.Cache.Clear(view.Name);

                return affected;
            }
            finally
            {
                if (connection != null)
                {
                    try
                    {
                        connection.Close();
                    }
                    catch { }
                    if (!identicalSystemConnection && sysCommand != null)
                    {
                        sysCommand.Connection.Close();
                    }
                }
            }

        }

        protected virtual int Update(View view, Dictionary<string, object> deepObject, string pk, bool deep, IDbCommand command, IDbCommand sysCommand, BeforeEditEventHandler beforeEditCallback, BeforeEditInDatabaseEventHandler beforeEditInDatabaseCallback, AfterEditEventHandler afterEditBeforeCommitCallback, AfterEditEventHandler afterEditAfterCommitCallback, BeforeCreateEventHandler beforeCreateCallback = null, BeforeCreateInDatabaseEventHandler beforeCreateInDatabaseCallback = null, AfterCreateEventHandler afterCreateBeforeCommitCallback = null, AfterCreateEventHandler afterCreateAfterCommitCallback = null, bool overwrite = false, BeforeDeleteEventHandler beforeDeleteCallback = null, AfterDeleteEventHandler afterDeleteBeforeCommitCallback = null, AfterDeleteEventHandler afterDeleteAfterCommitCallback = null)
        {
            if (deep && GetPkFromMetadata(deepObject) == null)
            {
                throw new NotOriginalObjectException();
            }

            Dictionary<string, object> values = new Dictionary<string, object>();

            foreach (string key in deepObject.Keys)
            {
                Field field = GetField(view, key);


                if (field != null)
                {
                    if (field.FieldType == FieldType.Children && !field.IsCheckList())
                    {
                        if (deep)
                        {
                            ChildrenField childrenField =(ChildrenField)field;
                            View childrenView = childrenField.ChildrenView;
                            Dictionary<string, string> keys = GetPrevChildren(childrenField, pk);

                            if (deepObject.ContainsKey(key) && deepObject[key] != null)
                            {
                                var children = (object[])deepObject[key];

                                if (overwrite)
                                {
                                    GetChildrenToDelete(keys, children);

                                    foreach (string keyToDelete in keys.Keys)
                                    {
                                        Delete(childrenView, keyToDelete, true, beforeDeleteCallback, afterDeleteBeforeCommitCallback, afterDeleteAfterCommitCallback, command, sysCommand, null);
                                    }
                                }

                                foreach (Dictionary<string, object> child in children)
                                {
                                    string childPk = GetPkFromMetadata(child);
                                    if (childPk == null)
                                    {
                                        ParentField parentField = childrenField.GetEquivalentParentField();
                                        if (!child.ContainsKey(parentField.JsonName))
                                        {
                                            child.Add(parentField.JsonName, pk);
                                        }
                                        Create(childrenView, child, deep, command, sysCommand, beforeCreateCallback, beforeCreateInDatabaseCallback, afterCreateBeforeCommitCallback, afterCreateAfterCommitCallback);
                                    }
                                    else
                                    {
                                        Update(childrenView, child, childPk, deep, command, sysCommand, beforeEditCallback, beforeEditInDatabaseCallback, afterEditBeforeCommitCallback, afterEditAfterCommitCallback);
                                    }
                                }
                            }
                        }
                    }
                    else
                    {
                        if (field.IsCheckList())
                        {
                            try
                            {
                                ParentField parentField = ((ChildrenField)field).GetFirstNonEquivalentParentField();
                                if (parentField != null)
                                {
                                    if (deepObject.ContainsKey(key) && deepObject[key] != null && deepObject[key] is System.Collections.IEnumerable)
                                    {
                                        List<string> clItems = new List<string>();
                                        foreach (Dictionary<string, object> child in (System.Collections.IEnumerable)deepObject[key])
                                        {
                                            if (child.ContainsKey(parentField.JsonName))
                                            {
                                                string clItem = null;
                                                if (child[parentField.JsonName] is Dictionary<string, object>)
                                                {
                                                    clItem = ((Dictionary<string, object>)((Dictionary<string, object>)child[parentField.JsonName])["__metadata"])["id"].ToString();
                                                }
                                                else if (child[parentField.JsonName] is string || child[parentField.JsonName] is int)
                                                {
                                                    clItem = child[parentField.JsonName].ToString();
                                                }
                                                else
                                                {
                                                    continue;
                                                }
                                                clItems.Add(clItem);
                                            }
                                        }
                                        values.Add(field.Name, clItems.ToArray().Delimited());
                                    }
                                    else
                                    {
                                        values.Add(field.Name, null);
                                    }
                                }
                            }
                            catch { }
                        }
                        // handle parents?
                        else if (!(field.FieldType == FieldType.Parent && deepObject[key] is Dictionary<string, object>))
                            values.Add(field.Name, deepObject[key]);
                    }
                }
                else
                {
                    values.Add(key, deepObject[key]);
                }
            }

            return GetSqlAccess(view).Edit(view, values, pk, beforeEditCallback, beforeEditInDatabaseCallback, afterEditBeforeCommitCallback, afterEditAfterCommitCallback, command, sysCommand, view.SaveHistory ? new History() : null, null);

        }

        private Dictionary<string, string> GetPrevChildren(ChildrenField childrenField, string pk)
        {
            return GetSqlAccess(childrenField.ChildrenView).GetChildren(childrenField, pk);
        }

        private void GetChildrenToDelete(Dictionary<string, string> keys, object[] children)
        {
            foreach (var child in children)
            {
                string childPk = GetPkFromMetadata((Dictionary<string, object>)child);
                if (childPk != null)
                {
                    if (keys.ContainsKey(childPk))
                    {
                        keys.Remove(childPk);
                    }
                }
            }
        }

        private string GetPkFromMetadata(Dictionary<string, object> o)
        {
            if (!o.ContainsKey(Database.__metadata))
            {
                return null;
            }
            try
            {
                Dictionary<string, object> metadata = (Dictionary<string, object>)o[Database.__metadata];
                if (!metadata.ContainsKey("id"))
                {
                    return null;
                }
                string id = metadata["id"].ToString();
                if (string.IsNullOrEmpty(id))
                    return null;
                return metadata["id"].ToString();
            }
            catch (Exception exception)
            {
                throw new DuradosException("Failed to get object id", exception);
            }
        }

        private string GetPkFromObject(View view, Dictionary<string, object> o)
        {
            List<object> pk = new List<object>();
            foreach (DataColumn column in view.DataTable.PrimaryKey)
            {
                Field field = view.GetFieldByColumnNames(column.ColumnName);
                if (field == null)
                    return null;
                if (!o.ContainsKey(field.Name))
                    return null;

                pk.Add(o[field.Name]);
            }

            return string.Join(",", pk.ToArray());
        }

        public int Delete(View view, string pk, bool deep, BeforeDeleteEventHandler beforeDeleteCallback, AfterDeleteEventHandler afterDeleteBeforeCommitCallback, AfterDeleteEventHandler afterDeleteAfterCommitCallback, IDbCommand command = null, IDbCommand sysCommand = null, Dictionary<string, object> values = null)
        {
            if ((command != null && command.Connection.State == ConnectionState.Closed) || (command != null && !IsSameResource(command.Connection, view.ConnectionString)))
                command = null;
            if ((sysCommand != null && sysCommand.Connection.State == ConnectionState.Closed) || (command != null && !IsSameResource(command.Connection, view.ConnectionString)))
                sysCommand = null;
            return GetSqlAccess(view).Delete(view, pk, beforeDeleteCallback, afterDeleteBeforeCommitCallback, afterDeleteAfterCommitCallback, command, sysCommand, values);
        }
    }

    public class NotOriginalObjectException : DuradosException
    {
        public NotOriginalObjectException() : base("Object is missing a metadata property. Please use original object in deep update.") { }
    }

    public interface IRest
    {
        string Create(View view, Dictionary<string, object>[] deepObject, bool deep, BeforeCreateEventHandler beforeCreateCallback, BeforeCreateInDatabaseEventHandler beforeCreateInDatabaseEventHandler, AfterCreateEventHandler afterCreateBeforeCommitCallback, AfterCreateEventHandler afterCreateAfterCommitCallback, IDbCommand command = null, IDbCommand sysCommand = null);
        string Create(View view, Dictionary<string, object> deepObject, bool deep, BeforeCreateEventHandler beforeCreateCallback, BeforeCreateInDatabaseEventHandler beforeCreateInDatabaseEventHandler, AfterCreateEventHandler afterCreateBeforeCommitCallback, AfterCreateEventHandler afterCreateAfterCommitCallback);
        int Update(View view, Dictionary<string, object> deepObject, string pk, bool deep, BeforeEditEventHandler beforeEditCallback, BeforeEditInDatabaseEventHandler beforeEditInDatabaseCallback, AfterEditEventHandler afteEditBeforeCommitCallback, AfterEditEventHandler afterEditAfterCommitCallback, BeforeCreateEventHandler beforeCreateCallback = null, BeforeCreateInDatabaseEventHandler beforeCreateInDatabaseCallback = null, AfterCreateEventHandler afterCreateBeforeCommitCallback = null, AfterCreateEventHandler afterCreateAfterCommitCallback = null, bool overwrite = false, BeforeDeleteEventHandler beforeDeleteCallback = null, AfterDeleteEventHandler afterDeleteBeforeCommitCallback = null, AfterDeleteEventHandler afterDeleteAfterCommitCallback = null, IDbCommand command = null, IDbCommand sysCommand = null);
        int Delete(View view, string pk, bool deep, BeforeDeleteEventHandler beforeDeleteCallback, AfterDeleteEventHandler afteDeleterBeforeCommitCallback, AfterDeleteEventHandler afterDeleteAfterCommitCallback, IDbCommand command = null, IDbCommand sysCommand = null, Dictionary<string, object> values = null);
        
    }
}

public class SqlSchema : ISchema
{

    protected ISqlTextBuilder _sqlTextBuilder = null;

    public ISqlTextBuilder sqlTextBuilder
    {
        get
        {
            if (_sqlTextBuilder == null)
                _sqlTextBuilder = GetSqlTextBuilder();

            return _sqlTextBuilder;
        }
    }

    protected virtual ISqlTextBuilder GetSqlTextBuilder()
    {
        return new SqlTextBuilder();
    }

    public string IsDatabaseExistsStatement(string databaseName)
    {
        return "SELECT name FROM master.sys.databases where name = '" + databaseName + "'";
    }

    public bool IsDatabaseExists(string databaseName, string connectionString)
    {
        using (SqlConnection connection = new SqlConnection(connectionString))
        {
            connection.Open();
            using (SqlCommand command = new SqlCommand())
            {
                command.Connection = connection;
                command.CommandText = IsDatabaseExistsStatement(databaseName);

                object scalar = command.ExecuteScalar();

                return scalar != null && scalar != DBNull.Value;
            }
        }
    }

    public List<string> GetTableNames(string connectionString)
    {
        List<string> tableNames = new List<string>();
        using (IDbConnection connection = GetConnection(connectionString))
        {
            connection.Open();
            using (IDbCommand command = GetCommand())
            {
                command.Connection = connection;
                command.CommandText = GetTableNamesSelectStatement();

                using (IDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        tableNames.Add(reader[0].ToString());
                    }
                }
            }
        }
        return tableNames;
    }

    public virtual string GetPrimaryIndexName(string tableName)
    {
        return null;
    }

    public virtual string GetTableRowsCount(string tableName, string indexName)
    {
        return null;
    }

    public virtual string GetTableRowsCount(string tableName)
    {
        return "SELECT rows FROM sys.sysindexes  AS s1 WHERE (rows > 0) AND (indid IN (SELECT MIN(indid) AS Expr1 FROM sys.sysindexes AS s2 WHERE (s1.id = id))) AND (id = OBJECT_ID('" + tableName + "'))";
    }

    public virtual string GetTotalRowsCount()
    {
        return "SELECT sum(rows) FROM sys.sysindexes  AS s1 WHERE (rows > 0) AND (indid IN (SELECT MIN(indid) AS Expr1 FROM sys.sysindexes AS s2 WHERE (s1.id = id)))";
    }

    public virtual string GetMaxTableRowsCount()
    {
        return "SELECT max(rows) FROM sys.sysindexes  AS s1 WHERE (rows > 0) AND (indid IN (SELECT MIN(indid) AS Expr1 FROM sys.sysindexes AS s2 WHERE (s1.id = id)))";

    }

    public virtual string GetPrimaryKeyColumns(string tableName)
    {
        return "SELECT Col.COLUMN_NAME FROM INFORMATION_SCHEMA.TABLE_CONSTRAINTS Tab, INFORMATION_SCHEMA.CONSTRAINT_COLUMN_USAGE Col WHERE Col.Constraint_Name = Tab.Constraint_Name AND Col.Table_Name = Tab.Table_Name AND Constraint_Type = 'PRIMARY KEY ' AND Col.Table_Name = N'" + tableName + "'";
    }

    public virtual string GetAutoIdentityColumns(string tableName)
    {
        return "select COLUMN_NAME, TABLE_NAME from INFORMATION_SCHEMA.COLUMNS where TABLE_SCHEMA = 'dbo' and COLUMNPROPERTY(object_id(TABLE_NAME), COLUMN_NAME, 'IsIdentity') = 1 and TABLE_NAME = N'" + tableName + "'";
    }

    public virtual string GetTableAndViewsNamesSelectStatement()
    {
        return "select Name from sys.Tables union select Name from sys.Views";
    }

    public virtual string GetDefaultSchemaSelectStatement()
    {
        return "SELECT SCHEMA_NAME()";
    }

    public virtual string GetEntitiesSelectStatement()
    {
        return "select Name, [Schema], EntityType from (select Name, SCHEMA_NAME(schema_id) as [Schema], 'Table' as EntityType from sys.Tables union select Name, SCHEMA_NAME(schema_id) as [Schema], 'View' as EntityType from sys.Views) as a order by EntityType, Name";
    }

    public virtual string GetTableNamesSelectStatement()
    {
        return "select Name from sys.Tables";
    }

    public virtual string CountTablesSelectStatement()
    {
        return "select Count(*) from sys.Tables";
    }

    public virtual string GetTableNamesSelectStatementWithFilter()
    {
        return "select Name from sys.Tables WHERE  Name like N'%[filter]%'";
    }
    public virtual string IsTableOrViewExistsSelectStatement(string tableName)
    {
        return "select Name from sys.Tables where name = N'" + tableName + "'" + " union select Name from sys.Views where name = N'" + tableName + "'";


    }

    public virtual string IsMasterKeyExistsStatement()
    {
        return "SELECT top(1) * FROM sys.symmetric_keys WHERE symmetric_key_id = 101";
    }

    public virtual string IsCertificateExistsStatement(string name)
    {
        return "select top(1) * from sys.certificates where name = '" + name + "'";
    }

    public virtual string IsSymmetricKeyExistsStatement(string name)
    {
        return "select top(1) * from sys.symmetric_keys where name = '" + name + "'";
    }



    public virtual string IsTableExistsSelectStatement(string tableName)
    {
        return "select Name from sys.Tables where name = N'" + tableName + "'";
    }

    public virtual string IsViewExistsSelectStatement(string viewName)
    {
        return "select Name from sys.Views where name = N'" + viewName + "'";
    }

    public virtual string GetColumnsSelectStatement(string tableName)
    {
        return "select column_name, data_type, character_maximum_length, is_nullable, column_default from information_schema.columns where table_name = N'" + tableName + "'";

    }

    public virtual string GetIsColumnExistsSelectStatement(string tableName, string columnName)
    {
        return "select column_name from information_schema.columns where table_name = N'" + tableName + "' and column_name = N'" + columnName + "'";
        
    }

    public string GetColumnsSelectStatement(string[] tablesNames)
    {
        return "select TABLE_NAME, column_name, data_type, character_maximum_length from information_schema.columns where table_name = N'" + tablesNames.Delimited() + "'";

    }

    public virtual string GetColumnDataTypeSelectStatement(string tableName, string columnName)
    {
        return "select DATA_TYPE + case when CHARACTER_MAXIMUM_LENGTH is not null then '(' + Convert(varchar(50), [CHARACTER_MAXIMUM_LENGTH]) + ')' else '' end as dt from INFORMATION_SCHEMA.COLUMNS IC where TABLE_NAME = '" + tableName + "' and COLUMN_NAME = '" + columnName + "'";

    }


    public virtual string GetForeignKeyConstraints(string tablesNames)
    {
        return "SELECT f.name AS ForeignKey, OBJECT_NAME(f.parent_object_id) AS TableName, COL_NAME(fc.parent_object_id, fc.parent_column_id) AS ColumnName, " +
                  "OBJECT_NAME(f.referenced_object_id) AS ReferenceTableName, COL_NAME(fc.referenced_object_id, fc.referenced_column_id) AS ReferenceColumnName " +
                "FROM     sys.foreign_keys AS f INNER JOIN " +
                  "sys.foreign_key_columns AS fc ON f.object_id = fc.constraint_object_id " +
                "WHERE  (OBJECT_NAME(f.parent_object_id) in (" + tablesNames + ")";
    }

    public virtual string GetForeignKeyConstraints()
    {
        return "SELECT f.name AS name, OBJECT_NAME(f.parent_object_id) AS TableName, COL_NAME(fc.parent_object_id, fc.parent_column_id) AS ColumnName, " +
                  "OBJECT_NAME(f.referenced_object_id) AS ReferenceTableName, COL_NAME(fc.referenced_object_id, fc.referenced_column_id) AS ReferenceColumnName " +
                "FROM     sys.foreign_keys AS f INNER JOIN " +
                  "sys.foreign_key_columns AS fc ON f.object_id = fc.constraint_object_id ";
    }

    public virtual string GetForeignKeyConstraint(string tableName, string referenceTableName, string columnName)
    {
        return "SELECT f.name AS ForeignKey " +
                "FROM     sys.foreign_keys AS f INNER JOIN " +
                  "sys.foreign_key_columns AS fc ON f.object_id = fc.constraint_object_id " +
                "WHERE  (OBJECT_NAME(f.parent_object_id) = N'" + tableName + "' and  COL_NAME(fc.parent_object_id, fc.parent_column_id) = N'" + columnName + "' and OBJECT_NAME(f.referenced_object_id) = N'" + referenceTableName + "')";
    }

    public virtual string GetIndexColumnsStatement(string indexName, string tableName)
    {
        string sql = "SELECT S.NAME SCHEMA_NAME,T.NAME TABLE_NAME,I.NAME INDEX_NAME,C.NAME COLUMN_NAME,I.is_unique,I.is_primary_key " +
  "FROM SYS.TABLES T " +
       "INNER JOIN SYS.SCHEMAS S " +
    "ON T.SCHEMA_ID = S.SCHEMA_ID " +
       "INNER JOIN SYS.INDEXES I " +
    "ON I.OBJECT_ID = T.OBJECT_ID " +
       "INNER JOIN SYS.INDEX_COLUMNS IC " +
    "ON IC.OBJECT_ID = T.OBJECT_ID " +
       "INNER JOIN SYS.COLUMNS C " +
    "ON C.OBJECT_ID  = T.OBJECT_ID " +
   "AND IC.INDEX_ID    = I.INDEX_ID " +
   "AND IC.COLUMN_ID = C.COLUMN_ID " +
 "WHERE 1=1 and T.NAME = N'{0}' and I.NAME = N'{1}' " +
"ORDER BY I.NAME,I.INDEX_ID,IC.KEY_ORDINAL ";
        return string.Format(sql, tableName, indexName);
    }
    public virtual Dictionary<string, object> GetIndexColumns(string indexName, string tableName, IDbCommand command)
    {
        string sql = GetIndexColumnsStatement(indexName, tableName);

        command.CommandText = sql;




        Dictionary<string, object> dictionary = new Dictionary<string, object>();

        using (IDataReader reader = command.ExecuteReader())
        {
            while (reader.Read())
            {
                string columnName = reader.GetString(reader.GetOrdinal("COLUMN_NAME"));
                bool unique = reader.GetBoolean(reader.GetOrdinal("is_unique"));
                bool pk = reader.GetBoolean(reader.GetOrdinal("is_primary_key"));
                var index = new { Unique = unique, PK = pk };
                dictionary.Add(columnName, index);
            }
        }

        return dictionary;
    }

    public virtual string GetColumnDataType(string tableName, string columnName, IDbCommand command)
    {
        string sql = GetColumnDataTypeSelectStatement(tableName, columnName);

        command.CommandText = sql;


        object scalar = command.ExecuteScalar();
        if (scalar == null || scalar == DBNull.Value)
            return null;
        else
            return scalar.ToString();
    }

    public virtual string GetForeignKeyConstraintsToMe(string tableName)
    {
        return "SELECT f.name AS ForeignKey, OBJECT_NAME(f.parent_object_id) AS TableName, COL_NAME(fc.parent_object_id, fc.parent_column_id) AS ColumnName, " +
                  "OBJECT_NAME(f.referenced_object_id) AS ReferenceTableName, COL_NAME(fc.referenced_object_id, fc.referenced_column_id) AS ReferenceColumnName " +
                "FROM     sys.foreign_keys AS f INNER JOIN " +
                  "sys.foreign_key_columns AS fc ON f.object_id = fc.constraint_object_id " +
                "WHERE  (OBJECT_NAME(f.referenced_object_id) = N'" + tableName + "')";
    }

    protected virtual string GetMyForeignKeyConstraintsSql(string tableName)
    {
        return "SELECT f.name AS ForeignKey, OBJECT_NAME(f.parent_object_id) AS TableName, COL_NAME(fc.parent_object_id, fc.parent_column_id) AS ColumnName, " +
                  "OBJECT_NAME(f.referenced_object_id) AS ReferenceTableName, COL_NAME(fc.referenced_object_id, fc.referenced_column_id) AS ReferenceColumnName " +
                "FROM     sys.foreign_keys AS f INNER JOIN " +
                  "sys.foreign_key_columns AS fc ON f.object_id = fc.constraint_object_id " +
                "WHERE  (OBJECT_NAME(f.parent_object_id) in (N'" + tableName + "'))";
    }

    protected string GetReferenceTableName(string tableName, string columnName)
    {
        return "SELECT OBJECT_NAME(f.referenced_object_id) AS ReferenceTableName " +
                "FROM     sys.foreign_keys AS f INNER JOIN " +
                  "sys.foreign_key_columns AS fc ON f.object_id = fc.constraint_object_id " +
                "WHERE  (OBJECT_NAME(f.parent_object_id) = N'" + tableName + "' and  COL_NAME(fc.parent_object_id, fc.parent_column_id) = N'" + columnName + "')";
    }

    public virtual Dictionary<string, string> GetMyForeignKeyConstraints(string tableName, string connectionString)
    {
        using (IDbConnection connection = GetConnection(connectionString))
        {
            connection.Open();
            using (IDbCommand command = GetCommand())
            {
                command.Connection = connection;

                return GetMyForeignKeyConstraints(tableName, command);
            }
        }
    }

    public virtual Dictionary<string, string> GetMyForeignKeyConstraints(string tableName, IDbCommand command)
    {
        string sql = GetMyForeignKeyConstraintsSql(tableName);

        command.CommandText = sql;

        Dictionary<string, string> fk = new Dictionary<string, string>();

        using (IDataReader reader = command.ExecuteReader())
        {
            while (reader.Read())
            {
                string columnName = reader.GetString(reader.GetOrdinal("ColumnName"));
                string referenceTableName = reader.GetString(reader.GetOrdinal("ReferenceTableName"));
                if (!fk.ContainsKey(columnName))
                    fk.Add(columnName, referenceTableName);
            }
        }

        return fk;

    }

    public HashSet<string> GetMyAncestors(string tableName, string connectionString)
    {
        using (IDbConnection connection = GetConnection(connectionString))
        {
            connection.Open();
            using (IDbCommand command = GetCommand())
            {
                command.Connection = connection;
                return GetMyAncestors(tableName, command);
            }
        }
    }

    public HashSet<string> GetMyAncestors(string tableName, IDbCommand command)
    {
        HashSet<string> ancestors = new HashSet<string>();

        ancestors.Add(tableName);

        GetMyAncestors(tableName, command, ancestors);

        return ancestors;
    }

    public void GetMyAncestors(string tableName, IDbCommand command, HashSet<string> ancestors)
    {
        Dictionary<string, string> parents = GetMyForeignKeyConstraints(tableName, command);

        foreach (string parent in parents.Values)
        {
            if (!ancestors.Contains(parent))
            {
                ancestors.Add(parent);
                GetMyAncestors(parent, command, ancestors);
            }
        }
    }


    public virtual Dictionary<string, List<string>> GetForeignKeyConstraintsToMe(string refrenceTableName, IDbCommand command)
    {
        string sql = GetForeignKeyConstraintsToMe(refrenceTableName);

        command.CommandText = sql;

        Dictionary<string, List<string>> fk = new Dictionary<string, List<string>>();

        using (IDataReader reader = command.ExecuteReader())
        {
            while (reader.Read())
            {
                string columnName = reader.GetString(reader.GetOrdinal("ColumnName"));
                string tableName = reader.GetString(reader.GetOrdinal("TableName"));
                if (!fk.ContainsKey(tableName))
                {
                    fk.Add(tableName, new List<string>());
                }
                fk[tableName].Add(columnName);
            }
        }

        return fk;

    }

    public virtual Dictionary<string, Entity> GetEntities(IDbCommand command)
    {
        Dictionary<string, Entity> entities = new Dictionary<string, Entity>();

        string defaultSchema = GetDefaultSchema(command);

        string sql = GetEntitiesSelectStatement();
        command.CommandText = sql;


        using (IDataReader reader = command.ExecuteReader())
        {
            while (reader.Read())
            {
                string name = reader.GetString(reader.GetOrdinal("Name"));
                string schema = reader.GetString(reader.GetOrdinal("Schema")).ToLower();
                string type = reader.GetString(reader.GetOrdinal("EntityType"));
                if (!entities.ContainsKey(name) && IsValidSchema(schema, defaultSchema))
                {
                    entities.Add(name, new Entity() { Name = name, EntityType = type, Schema = schema });
                }
            }
        }

        return entities;
    }
    //public Dictionary<string, Entity> GetEntities(IDbCommand command, string defaultSchema)
    //{
    //    Dictionary<string, Entity> entities = new Dictionary<string, Entity>();

    //    string sql = GetEntitiesSelectStatement();
    //    command.CommandText = sql;

    //    using (IDataReader reader = command.ExecuteReader())
    //    {
    //        while (reader.Read())
    //        {
    //            string name = reader.GetString(reader.GetOrdinal("Name"));
    //            string schema = reader.GetString(reader.GetOrdinal("Schema")).ToLower();
    //            string type = reader.GetString(reader.GetOrdinal("EntityType"));
    //            if (!entities.ContainsKey(name) && (schema == "dbo" || schema == defaultSchema.ToLower()))
    //            {
    //                entities.Add(name, new Entity() { Name = name, EntityType = type, Schema = schema });
    //            }
    //        }
    //    }

    //    return entities;
    //}

    protected virtual string GetDefaultSchema(IDbCommand command)
    {
        string defaulSchema = "dbo";
        try
        {
            command.CommandText = GetDefaultSchemaSelectStatement();
            defaulSchema = command.ExecuteScalar().ToString();
        }
        catch { }

        return defaulSchema;
    }

    protected virtual bool IsValidSchema(string schema, string defaultSchema)
    {
        return (schema == "dbo" || schema == defaultSchema.ToLower()) && schema == "dbo";
    }

    public class Entity
    {
        public string Name { get; set; }
        public string Schema { get; set; }
        public string EntityType { get; set; }
    }


    public virtual string GetReferenceTableName(string tableName, string columnName, IDbCommand command)
    {
        string sql = GetReferenceTableName(tableName, columnName);

        command.CommandText = sql;

        object scalar = command.ExecuteScalar();

        if (scalar == null || scalar == DBNull.Value)
            return null;
        else
            return scalar.ToString();
    }

    public virtual string GetViewDefinition(string viewName, IDbCommand command)
    {
        string sql = "select VIEW_DEFINITION from INFORMATION_SCHEMA.VIEWS where TABLE_NAME = N'" + viewName + "'";

        command.CommandText = sql;

        object scalar = command.ExecuteScalar();

        if (scalar == null || scalar == DBNull.Value)
            return null;
        else
            return scalar.ToString();
    }

    protected virtual SqlSchema GetNewSqlSchema()
    {
        return new SqlSchema();
    }

    public virtual string[] GetColumns(string tableName, IDbCommand command)
    {
        List<string> columns = new List<string>();
        string sql = (GetNewSqlSchema()).GetColumnsSelectStatement(tableName);
        command.CommandText = sql;
        using (IDataReader reader = command.ExecuteReader())
        {
            while (reader.Read())
            {
                string name = reader.GetString(reader.GetOrdinal("column_name"));
                columns.Add(name);
            }
        }

        return columns.ToArray();
    }

    public virtual bool IsTableOrViewExists(string name, IDbCommand command)
    {
        string sql = (GetNewSqlSchema()).IsTableOrViewExistsSelectStatement(name);
        command.CommandText = sql;
        return command.ExecuteScalar() != null && command.ExecuteScalar() != DBNull.Value;
    }

    public virtual bool IsTableExists(string name, IDbCommand command)
    {
        string sql = (GetNewSqlSchema()).IsTableExistsSelectStatement(name);
        command.CommandText = sql;
        return command.ExecuteScalar() != null && command.ExecuteScalar() != DBNull.Value;
    }

    public virtual bool IsViewExists(string name, IDbCommand command)
    {
        string sql = (GetNewSqlSchema()).IsViewExistsSelectStatement(name);
        command.CommandText = sql;
        return command.ExecuteScalar() != null && command.ExecuteScalar() != DBNull.Value;
    }

    public virtual bool IsColumnExists(string tableName, string columnName, IDbCommand command)
    {
        string sql = (GetNewSqlSchema()).GetIsColumnExistsSelectStatement(tableName, columnName);
        command.CommandText = sql;
        return command.ExecuteScalar() != null && command.ExecuteScalar() != DBNull.Value;
    }

    public virtual bool IsTableOrViewExists(string name, string connectionString)
    {
        string sql = (GetNewSqlSchema()).IsTableOrViewExistsSelectStatement(name);
        IDbConnection conn = GetConnection(connectionString);
        conn.Open();
        try
        {
            IDbCommand command = GetCommand();

            command.Connection = conn;
            //command.CommandType = System.Data.CommandType.StoredProcedure;
            command.CommandText = sql;
            return command.ExecuteScalar() != null && command.ExecuteScalar() != DBNull.Value;
        }
        finally
        {
            conn.Close();
        }
    }

    public virtual bool IsMasterKeyExists(string connectionString)
    {
        IDbConnection conn = GetConnection(connectionString);
        conn.Open();
        try
        {
            IDbCommand command = GetCommand();

            command.Connection = conn;
            return IsMasterKeyExists(command);
        }
        finally
        {
            conn.Close();
        }
    }

    public virtual bool IsCertificateExists(string name, string connectionString)
    {
        IDbConnection conn = GetConnection(connectionString);
        conn.Open();
        try
        {
            IDbCommand command = GetCommand();

            command.Connection = conn;
            return IsCertificateExists(name, command);
        }
        finally
        {
            conn.Close();
        }
    }

    public virtual bool IsSymmetricKeyExists(string name, string connectionString)
    {
        IDbConnection conn = GetConnection(connectionString);
        conn.Open();
        try
        {
            IDbCommand command = GetCommand();

            command.Connection = conn;
            return IsSymmetricKeyExists(name, command);
        }
        finally
        {
            conn.Close();
        }
    }

    public string CreateMasterKeyStatement(string password)
    {
        return "CREATE MASTER KEY ENCRYPTION BY PASSWORD = '" + password + "'";
    }

    public string CreateCertificateStatement(string name, string password)
    {
        return "CREATE CERTIFICATE " + name + " WITH SUBJECT = '" + password + "'";
    }

    public string CreateSymmetricKeyStatement(string name, string certificateName, string algorithm)
    {
        return "CREATE SYMMETRIC KEY " + name + " WITH ALGORITHM = " + algorithm + " ENCRYPTION BY CERTIFICATE " + certificateName;
    }

    public void CreateMasterKey(string password, IDbCommand command)
    {
        command.CommandText = CreateMasterKeyStatement(password);
        command.ExecuteNonQuery();

    }

    public void CreateCertificate(string name, string password, IDbCommand command)
    {
        command.CommandText = CreateCertificateStatement(name, password);
        command.ExecuteNonQuery();

    }

    public void CreateSymmetricKey(string name, string certificateName, string algorithm, IDbCommand command)
    {
        command.CommandText = CreateSymmetricKeyStatement(name, certificateName, algorithm);
        command.ExecuteNonQuery();

    }

    public bool IsMasterKeyExists(IDbCommand command)
    {
        string sql = IsMasterKeyExistsStatement();
        command.CommandText = sql;
        object scalar = command.ExecuteScalar();
        return scalar != null && scalar != DBNull.Value;
    }

    public bool IsCertificateExists(string name, IDbCommand command)
    {
        string sql = IsCertificateExistsStatement(name);
        command.CommandText = sql;
        object scalar = command.ExecuteScalar();
        return scalar != null && scalar != DBNull.Value;
    }

    public bool IsSymmetricKeyExists(string name, IDbCommand command)
    {
        string sql = IsSymmetricKeyExistsStatement(name);
        command.CommandText = sql;
        object scalar = command.ExecuteScalar();
        return scalar != null && scalar != DBNull.Value;
    }

    public void CreateMasterCertificateAndSymmetric(string password, string certificateName, string symmetricKeyName, string algorithm, IDbCommand command)
    {
        if (!IsMasterKeyExists(command))
        {
            CreateMasterKey(password, command);
        }

        if (!IsCertificateExists(certificateName, command))
        {
            CreateCertificate(certificateName, password, command);
        }

        if (!IsSymmetricKeyExists(symmetricKeyName, command))
        {
            CreateSymmetricKey(symmetricKeyName, certificateName, algorithm, command);
        }
    }


    public virtual string GetInsertIntoNewParentStatement(string newTableName, string tableName, string fieldName, string relatedViewDisplayName)
    {
        return string.Format("insert into " + sqlTextBuilder.EscapeDbObject("{0}") + " (" + sqlTextBuilder.EscapeDbObject("{3}") + ") select distinct " + sqlTextBuilder.EscapeDbObject("{1}") + sqlTextBuilder.DbTableColumnSeperator + sqlTextBuilder.EscapeDbObject("{2}") + " from " + sqlTextBuilder.EscapeDbObject("{1}") + " where " + sqlTextBuilder.EscapeDbObject("{1}") + sqlTextBuilder.DbTableColumnSeperator + sqlTextBuilder.EscapeDbObject("{2}") + " is not null and " + sqlTextBuilder.EscapeDbObject("{1}") + sqlTextBuilder.DbTableColumnSeperator + sqlTextBuilder.EscapeDbObject("{2}") + " not in (select " + sqlTextBuilder.EscapeDbObject("{3}") + " from " + sqlTextBuilder.EscapeDbObject("{0}") + ") order by " + sqlTextBuilder.EscapeDbObject("{1}") + sqlTextBuilder.DbTableColumnSeperator + sqlTextBuilder.EscapeDbObject("{2}"), newTableName, tableName, fieldName, relatedViewDisplayName);

    }

    public virtual string GetUpdateNewParentStatement(string newTableName, string relatedViewPkName, string relatedViewOrdinal)
    {
        return string.Format("update " + sqlTextBuilder.EscapeDbObject("{0}") + " set " + sqlTextBuilder.EscapeDbObject("{2}") + " = " + sqlTextBuilder.EscapeDbObject("{1}") + " where " + sqlTextBuilder.EscapeDbObject("{2}") + " is null", newTableName, relatedViewPkName, relatedViewOrdinal);

    }

    public virtual string GetUpdateNewNotNestedParentStatement(string tableName, string columnName, string newTableName, string fieldName, string relatedViewPkName, string relatedViewDisplayName)
    {
        return string.Format("update " + sqlTextBuilder.EscapeDbObject("{0}") + " set " + sqlTextBuilder.EscapeDbObject("{0}") + sqlTextBuilder.DbTableColumnSeperator + sqlTextBuilder.EscapeDbObject("{1}") + " = " + sqlTextBuilder.EscapeDbObject("{2}") + sqlTextBuilder.DbTableColumnSeperator + sqlTextBuilder.EscapeDbObject("{4}") + " from " + sqlTextBuilder.EscapeDbObject("{0}") + " inner join " + sqlTextBuilder.EscapeDbObject("{2}") + " on " + sqlTextBuilder.EscapeDbObject("{0}") + sqlTextBuilder.DbTableColumnSeperator + sqlTextBuilder.EscapeDbObject("{3}") + " = " + sqlTextBuilder.EscapeDbObject("{2}") + sqlTextBuilder.DbTableColumnSeperator + sqlTextBuilder.EscapeDbObject("{5}"), tableName, columnName, newTableName, fieldName, relatedViewPkName, relatedViewDisplayName);

    }

    public virtual IDbCommand GetCommand()
    {
        return new DuradosCommand(SqlProduct.SqlServer);
    }

    public virtual IDbConnection GetConnection(string connectionString)
    {
        return new SqlConnection(connectionString);
    }

    public virtual IDbCommand GetCommand(string commandText, IDbConnection connection)
    {
        return new SqlCommand(commandText, (SqlConnection)connection);
    }

    public void CreateTable(DataTable table, IDbCommand command)
    {
        if (IsTableOrViewExists(table.TableName, command))
        {
            throw new DuradosException("The table " + table.TableName + " already exists in the database.");
        }

        if (table.PrimaryKey == null || table.PrimaryKey.Length == 0)
        {
            throw new DuradosException("The table " + table.TableName + " does not have a primary key.");
        }

        if (table.PrimaryKey.Length > 1)
        {
            throw new DuradosException("The table " + table.TableName + " has more than one column as a primary key. Currently, not supported.");
        }


        command.CommandText = GetCreateTableStatement(table);

        try
        {
            command.ExecuteNonQuery();
        }
        catch (Exception exception)
        {
            throw new DuradosException("Could not create the table " + table.TableName + ".\r\nAdditional information:\n " + exception.Message, exception);
        }

    }

    protected virtual string GetCreateTableStatement(DataTable table)
    {
        System.Text.StringBuilder sb = new StringBuilder();

        sb.AppendFormat("CREATE TABLE " + sqlTextBuilder.EscapeDbObject("{0}") + "(", table.TableName);
        foreach (DataColumn column in table.Columns)
        {
            sb.Append(GetCreateColumnScript(column));
            sb.Append(",");
        }
        sb.AppendFormat(" CONSTRAINT [PK_{0}] PRIMARY KEY CLUSTERED (	[{1}] ASC) WITH (IGNORE_DUP_KEY = OFF) )", table.TableName, table.PrimaryKey[0].ColumnName);

        return sb.ToString();


    }

    protected virtual string GetCreateColumnScript(DataColumn column)
    {
        string script = sqlTextBuilder.EscapeDbObject("{0}") + " {1} {2} {3}";

        string name = column.ColumnName;
        string type = GetColumnTypeScript(column);
        string identity = column.AutoIncrement ? "IDENTITY(1,1)" : string.Empty;
        string nullable = column.AutoIncrement || column.Table.PrimaryKey.Contains(column) ? "NOT NULL" : "NULL";

        return string.Format(script, name, type, identity, nullable);
    }

    protected virtual string GetColumnTypeScript(DataColumn column)
    {
        if (column.DataType == typeof(int))
            return "[int]";
        else if (column.DataType == typeof(DateTime))
            return "[datetime]";
        else if (column.DataType == typeof(double))
            return "[float]";
        else if (column.DataType == typeof(bool))
            return "[bit]";
        else
        {
            return string.Format("[nvarchar] ({0})", column.MaxLength);
        }
    }

    public void DropView(string viewName, IDbCommand command)
    {
        if (!IsViewExists(viewName, command))
        {
            throw new DuradosException("The view " + viewName + " does not exist in the database.");
        }

        string sql = string.Format("DROP VIEW " + sqlTextBuilder.EscapeDbObject("{0}") + " ", viewName);

        command.CommandText = sql;
        try
        {
            command.ExecuteNonQuery();
        }
        catch (Exception exception)
        {
            throw new DuradosException("Could not drop the view " + viewName + ".\r\nAdditional information:\n " + exception.Message, exception);
        }
    }

    public void DropTable(string tableName, IDbCommand command)
    {
        if (!IsTableExists(tableName, command))
        {
            throw new DuradosException("The table " + tableName + " does not exist in the database.");
        }

        string sql = string.Format("DROP TABLE [{0}] ", tableName);

        command.CommandText = sql;
        try
        {
            command.ExecuteNonQuery();
        }
        catch (Exception exception)
        {
            throw new DuradosException("Could not drop the table " + tableName + ".\r\nAdditional information:\n " + exception.Message, exception);
        }
    }

    public void CreateView(string viewName, string sql, IDbCommand command)
    {
        if (IsTableOrViewExists(viewName, command))
        {
            throw new DuradosException("The view " + viewName + " already exists in the database.");
        }

        sql = string.Format("CREATE VIEW " + sqlTextBuilder.EscapeDbObject("{0}") + " AS {1}", viewName, sql);

        command.CommandText = sql;
        try
        {
            command.ExecuteNonQuery();
        }
        catch (Exception exception)
        {
            throw new DuradosException("Could not create the view " + viewName + ".\r\nAdditional information:\n " + exception.Message, exception);
        }

    }

    public virtual void RemoveColumnFromTable(string tableName, string columnName, string connectionString)
    {
        IDbConnection conn = GetConnection(connectionString);
        conn.Open();
        try
        {
            IDbCommand command = GetCommand();
            command.Connection = conn;
            RemoveColumnFromTable(tableName, columnName, command);
        }
        finally
        {
            conn.Close();
        }
    }

    public virtual void RemoveColumnFromTable(string tableName, string columnName, IDbCommand command)
    {
        if (!IsColumnExists(tableName, columnName, command))
        {
            throw new DuradosException("The column " + columnName + " does not exist");

        }

        string sql = "ALTER TABLE " + sqlTextBuilder.EscapeDbObject(tableName) + " drop Column " + sqlTextBuilder.EscapeDbObject(columnName);

        command.CommandText = sql;

        try
        {
            command.ExecuteNonQuery();
        }
        catch (Exception exception)
        {
            throw new DuradosException("Could not remove the column " + columnName + ".\r\nAdditional information:\n " + exception.Message, exception);
        }
    }

    public virtual void RemoveColumnFromView(string viewName, string columnName, IDbCommand command)
    {
        if (!IsColumnExists(viewName, columnName, command))
        {
            throw new DuradosException("The column " + columnName + " does not exist");

        }
        char comma = ',';
        char space = ' ';

        int index = 0;


        string sql = GetViewDefinition(viewName, command);
        sql = sql.Replace(",", ", ");
        sql = sql.StripInvisibles();
        sql = sql.Replace(" ,", ",");

        string sqlLower = sql.ToLower();

        index = sqlLower.IndexOf(space + columnName.ToLower() + comma);
        int containsBreaks = 0;

        if (index <= 0)
            index = sqlLower.IndexOf(space + columnName.ToLower() + space);

        if (index <= 0)
        {
            index = sqlLower.IndexOf(sqlTextBuilder.EscapeDbObject(columnName.ToLower()) + comma);
            containsBreaks = 1;
        }
        if (index <= 0)
        {
            index = sqlLower.IndexOf(sqlTextBuilder.EscapeDbObject(columnName.ToLower()) + space);
            containsBreaks = 1;
        }

        if (index <= 0)
            throw new DuradosException("Could not remove the column " + columnName + ".");

        int commaIndex = sqlLower.Substring(0, index).LastIndexOf(comma);
        if (commaIndex <= 0)
        {
            commaIndex = index;
            index += 1;
        }
        sql = sql.Substring(0, commaIndex) + sql.Substring(index + columnName.Length + 1 + containsBreaks);

        DropView(viewName, command);

        command.CommandText = sql;

        try
        {
            command.ExecuteNonQuery();
        }
        catch (Exception exception)
        {
            throw new DuradosException("Could not remove the column " + columnName + ".\r\nAdditional information:\n " + exception.Message, exception);
        }
    }

    public virtual void AddNewColumnToTable(string tableName, string columnName, DataType dataType, IDbCommand command)
    {
        AddNewColumnToTable(tableName, columnName, GetDataType(dataType), command);
    }

    public virtual void AddNewColumnToTable(string tableName, string columnName, string dataType, IDbCommand command)
    {

        if (IsColumnExists(tableName, columnName, command))
        {
            throw new DuradosException("The column " + columnName + " already exists");

        }

        string sql = "ALTER TABLE " + sqlTextBuilder.EscapeDbObject(tableName) + " ADD " + sqlTextBuilder.EscapeDbObject(columnName) + " " + dataType;

        command.CommandText = sql;

        try
        {
            command.ExecuteNonQuery();
        }
        catch (Exception exception)
        {
            throw new DuradosException("Could not create the column " + columnName + ".\r\nAdditional information:\n " + exception.Message, exception);
        }
    }

    public virtual string GetDataType(DataType dataType)
    {
        switch (dataType)
        {
            case DataType.Boolean:
                return "bit  NULL";

            case DataType.DateTime:
                return "DateTime  NULL";

            case DataType.LongText:
                return "nvarchar(4000)  NULL";

            case DataType.Numeric:
                return "float  NULL";

            case DataType.ImageList:
            case DataType.SingleSelect:
                return "int  NULL";

            case DataType.Image:
            case DataType.Email:
            case DataType.Url:
            case DataType.Html:

            case DataType.ShortText:
                return "nvarchar(250)  NULL";

            default:
                throw new DuradosException("Ilegal data type " + dataType.ToString());

        }
    }


    private string AddBreaks(string s)
    {
        return sqlTextBuilder.EscapeDbObject(s);
    }

    public virtual void AddNewColumnToView(string viewName, string columnName, IDbCommand command)
    {

        if (IsColumnExists(viewName, columnName, command))
        {
            throw new DuradosException("The column " + columnName + " already exists");

        }

        string sql = GetViewDefinition(viewName, command);
        string sqlLower = sql.ToLower();


        char comma = ',';
        int distinctIndex = sqlLower.IndexOf(" distict ");
        bool hasDistinct = false;
        int topIndex = sqlLower.IndexOf(" top( ");
        bool hasTop = false;
        hasDistinct = (distinctIndex > 1 && distinctIndex < 20);

        hasTop = (topIndex > 1 && topIndex < 20);

        int index = 0;

        if (!hasDistinct && !hasTop)
        {
            index = sqlLower.IndexOf("select ") + 7;
        }
        else if (topIndex > distinctIndex)
        {
            index = sqlLower.IndexOf(") " + 1);
        }
        else
        {
            index = distinctIndex + 9;
        }

        sql = sql.Insert(index, columnName + comma);

        DropView(viewName, command);

        //CreateView(viewName, sql, command);
        command.CommandText = sql;
        try
        {
            command.ExecuteNonQuery();
        }
        catch (Exception exception)
        {
            throw new DuradosException("Could not create the view " + viewName + ".\r\nAdditional information:\n " + exception.Message, exception);
        }

    }

    protected virtual string GetCreateFkConstraintStatement(string fkTableName, string fkColumnName, string pkTableName, string pkColumnName)
    {
        return "ALTER TABLE " + sqlTextBuilder.EscapeDbObject(fkTableName) + " WITH CHECK ADD  CONSTRAINT " + sqlTextBuilder.EscapeDbObject("FK_" + fkTableName + "_" + pkTableName + "_" + fkColumnName) + " FOREIGN KEY(" + sqlTextBuilder.EscapeDbObject(fkColumnName) + ") REFERENCES " + sqlTextBuilder.EscapeDbObject(pkTableName) + " (" + sqlTextBuilder.EscapeDbObject(pkColumnName) + ")";
    }

    public virtual void CreateFkConstraint(string fkTableName, string fkColumnName, string pkTableName, string pkColumnName, IDbCommand command)
    {
        return;

        string sql = GetCreateFkConstraintStatement(fkTableName, fkColumnName, pkTableName, pkColumnName);

        command.CommandText = sql;
        try
        {
            command.ExecuteNonQuery();
        }
        catch (Exception exception)
        {
            throw new DuradosException("Could not create relation between the parent view " + pkTableName + " and the child view " + fkTableName + ".\r\nAdditional information:\n " + exception.Message, exception);
        }
    }

    public virtual void DropFkConstraint(string tableName, string referenceTableName, string columnName, IDbCommand command)
    {
        string sql = GetForeignKeyConstraint(tableName, referenceTableName, columnName);
        command.CommandText = sql;
        try
        {
            object scalar = command.ExecuteScalar();

            if (scalar == null || scalar == DBNull.Value)
                throw new DuradosFkConstraintNotFoundException();

            string fkConstraintName = scalar.ToString();

            sql = "ALTER TABLE " + tableName + " DROP CONSTRAINT " + fkConstraintName;

            command.CommandText = sql;

            command.ExecuteNonQuery();
        }
        catch (Exception exception)
        {
            throw new DuradosException("Could not drop the relation between the parent view " + referenceTableName + " and the child view " + tableName + ".\r\nAdditional information:\n " + exception.Message, exception);
        }
    }

    public virtual void RenameColumn(string tableName, string oldColumnName, string newColumnName, IDbCommand command)
    {
        string sql = "sp_RENAME '" + tableName + "." + oldColumnName + "', '" + newColumnName + "' , 'COLUMN'";
        command.CommandText = sql;
        try
        {
            command.ExecuteNonQuery();
        }
        catch (Exception exception)
        {
            throw new DuradosException("Could not change table '" + tableName + "' column + '" + oldColumnName + "' to '" + newColumnName + "'.\r\nAdditional information:\n " + exception.Message, exception);
        }
    }

    public virtual void ChangeColumnNameInView(string viewName, string oldColumnName, string newColumnName, IDbCommand command)
    {

        string sql = GetViewDefinition(viewName, command);
        string sqlLower = sql.ToLower();


        char comma = ',';
        char space = ' ';
        char lbreak = '[';
        char rbreak = ']';

        if (sql.Contains(comma + oldColumnName + comma))
            sql = sql.Replace(comma + oldColumnName + comma, comma + newColumnName + comma);
        else if (sql.Contains(comma + oldColumnName + space))
            sql = sql.Replace(comma + oldColumnName + space, comma + newColumnName + space);
        else if (sql.Contains(space + oldColumnName + comma))
            sql = sql.Replace(space + oldColumnName + comma, space + newColumnName + comma);
        else if (sql.Contains(space + oldColumnName + space))
            sql = sql.Replace(space + oldColumnName + space, space + newColumnName + space);
        else if (sql.Contains(lbreak + oldColumnName + rbreak))
            sql = sql.Replace(lbreak + oldColumnName + rbreak, lbreak + newColumnName + rbreak);

        DropView(viewName, command);

        //CreateView(viewName, sql, command);
        command.CommandText = sql;
        try
        {
            command.ExecuteNonQuery();
        }
        catch (Exception exception)
        {
            throw new DuradosException("Could not create the view " + viewName + ".\r\nAdditional information:\n " + exception.Message, exception);
        }
    }

    public virtual void ChangeColumnType(string tableName, string columnName, DataType type, IDbCommand command)
    {
        string sql = string.Format("ALTER TABLE " + sqlTextBuilder.EscapeDbObject("{0}") + sqlTextBuilder.GetAlterColumn(sqlTextBuilder.EscapeDbObject("{1}")) + sqlTextBuilder.EscapeDbObject("{1}") + " {2} ", tableName, columnName, GetDataType(type).Replace(" NULL", " "));

        command.CommandText = sql;

        command.ExecuteNonQuery();
    }

    public virtual string GetMultiListRelatedViewValuesStatement(string singleSelectTableName, string relatedViewDisplayName, string singleSelectTablePkColumnName)
    {
        return string.Format("select distinct " + sqlTextBuilder.EscapeDbObject("{0}") + sqlTextBuilder.DbTableColumnSeperator + sqlTextBuilder.EscapeDbObject("{1}") + ", " + sqlTextBuilder.EscapeDbObject("{0}") + sqlTextBuilder.DbTableColumnSeperator + sqlTextBuilder.EscapeDbObject("{2}") + " from " + sqlTextBuilder.EscapeDbObject("{0}") + " where " + sqlTextBuilder.EscapeDbObject("{0}") + sqlTextBuilder.DbTableColumnSeperator + sqlTextBuilder.EscapeDbObject("{1}") + " is not null", singleSelectTableName, relatedViewDisplayName, singleSelectTablePkColumnName);
    }

    public virtual string GetMultiListViewValuesStatement(string tableName, string fieldName, string viewPkColumnName)
    {
        return string.Format("select distinct " + sqlTextBuilder.EscapeDbObject("{0}") + sqlTextBuilder.DbTableColumnSeperator + sqlTextBuilder.EscapeDbObject("{1}") + ", " + sqlTextBuilder.EscapeDbObject("{0}") + sqlTextBuilder.DbTableColumnSeperator + sqlTextBuilder.EscapeDbObject("{2}") + " from " + sqlTextBuilder.EscapeDbObject("{0}") + " where " + sqlTextBuilder.EscapeDbObject("{0}") + sqlTextBuilder.DbTableColumnSeperator + sqlTextBuilder.EscapeDbObject("{1}") + " is not null", tableName, fieldName, viewPkColumnName);
    }

    public virtual void StopIdentityInsert(string tableName, IDbCommand command)
    {
        command.CommandText = "SET IDENTITY_INSERT [" + tableName + "] ON";
        command.ExecuteNonQuery();
    }

    public virtual void ContinueIdentityInsert(string tableName, IDbCommand command)
    {
        command.CommandText = "SET IDENTITY_INSERT [" + tableName + "] OFF";
        command.ExecuteNonQuery();
    }

    public virtual string GetMultiListInsertIntoViewStatement(string singleSelectTableName, string singleSelectTableColumnName0, string singleSelectTableColumnName1, int value, string key, string singleSelectTableColumnName2)
    {
        return string.Format("insert into " + sqlTextBuilder.EscapeDbObject("{0}") + "(" + sqlTextBuilder.EscapeDbObject("{1}") + ", " + sqlTextBuilder.EscapeDbObject("{2}") + ", " + sqlTextBuilder.EscapeDbObject("{5}") + ") values ({3}, '{4}', {3})", singleSelectTableName, singleSelectTableColumnName0, singleSelectTableColumnName1, value, key, singleSelectTableColumnName2);
    }

    public virtual string GetMultiListInsertIntoManyToManyStatement(string manyToManyTableName, string manyToManyTableColumnName1, string manyToManyTableColumnName2, string pk, int fk)
    {
        return string.Format("insert into " + sqlTextBuilder.EscapeDbObject("{0}") + "(" + sqlTextBuilder.EscapeDbObject("{1}") + ", " + sqlTextBuilder.EscapeDbObject("{2}") + ") values ({3},{4})", manyToManyTableName, manyToManyTableColumnName1, manyToManyTableColumnName2, pk, fk);
    }

    public virtual string GetFormula(string calculatedField, string tableName)
    {
        return "SELECT TOP 1 " + calculatedField + " FROM " + sqlTextBuilder.EscapeDbObject(tableName);
    }

    public virtual string GetTreeFirstLevelSql()
    {
        return "SELECT {0} AS " + sqlTextBuilder.EscapeDbObject("title") + ", " + sqlTextBuilder.EscapeDbObject("{1}") + " AS " + sqlTextBuilder.EscapeDbObject("key") + " FROM " + sqlTextBuilder.EscapeDbObject("{2}") + " WHERE " + sqlTextBuilder.EscapeDbObject("{3}") + " {4}";
    }

    public virtual string GetTableEntity()
    {
        return "Table";
    }

    public virtual string GetCreateView(DataTable table)
    {
        return string.Format("select {1} from " + sqlTextBuilder.EscapeDbObject("{0}") + " " + sqlTextBuilder.WithNolock, table.TableName, GetDelimitedColumns(table));
    }

    private string GetDelimitedColumns(DataTable table)
    {
        StringBuilder delimitedColumns = new StringBuilder();

        foreach (DataColumn column in table.Columns)
        {
            delimitedColumns.Append(sqlTextBuilder.EscapeDbObject(column.ColumnName) + ",");
        }

        delimitedColumns.Remove(delimitedColumns.Length - 1, 1);

        return delimitedColumns.ToString();
    }


    public virtual string GetDistinctColumnFieldValues(string tableName, string fieldName, int top)
    {
        return sqlTextBuilder.Top(string.Format("select distinct " + sqlTextBuilder.EscapeDbObject("{0}") + sqlTextBuilder.DbTableColumnSeperator + sqlTextBuilder.EscapeDbObject("{1}") + " from " + sqlTextBuilder.EscapeDbObject("{0}") + " order by " + sqlTextBuilder.EscapeDbObject("{0}") + sqlTextBuilder.DbTableColumnSeperator + sqlTextBuilder.EscapeDbObject("{1}"), tableName, fieldName), top);
    }

    internal string GetUpdateStatementFromNewRelatedTable(string tableName, string columnName, string fieldName)
    {
        return string.Format("update " + sqlTextBuilder.EscapeDbObject("{0}") + " set " + sqlTextBuilder.EscapeDbObject("{1}") + " = " + sqlTextBuilder.DbParameterPrefix + "relatedViewPkName where " + sqlTextBuilder.EscapeDbObject("{0}") + sqlTextBuilder.DbTableColumnSeperator + sqlTextBuilder.EscapeDbObject("{2}") + " = " + sqlTextBuilder.DbParameterPrefix + "relatedViewDisplayName ", tableName, columnName, fieldName);

    }

    public virtual string GetSelectFirstRow(string tableName)
    {
        return string.Format(sqlTextBuilder.Top("select * from " + sqlTextBuilder.EscapeDbObject("{0}"), 1), tableName);
    }
    public virtual string GetServerName(string connectionString)
    {
        SqlConnectionStringBuilder builder = new SqlConnectionStringBuilder(connectionString);
        return builder.DataSource;
    }
    public virtual string GetPort(string connectionString)
    {
        return "1433";
    }

    public virtual IDbCommand GetNewCommand()
    {
        return new SqlCommand();
    }



    public virtual string GetSelectForCustomeViewTable()
    {
        return "SELECT TOP 1 CustomView FROM [dbo].[durados_CustomViews] WHERE [UserId] = @UserId AND [ViewName] = @ViewName";
    }

    public virtual IDataParameter GetNewParameter()
    {
        return new SqlParameter();
    }
    public virtual IDataParameter GetNewParameter(string name, object value)
    {
        return new SqlParameter(name, value);
    }
}

public class DuradosCommand : System.Data.IDbCommand
{
    IDbCommand command;

    public IDbCommand Command
    {
        get
        {
            return command;
        }
    }

    public DuradosCommand(SqlProduct sqlProduct)
    {
        command = GetNewCommand(sqlProduct);
    }

    protected virtual IDbCommand GetNewCommand(SqlProduct sqlProduct)
    {
        switch (sqlProduct)
        {
            case SqlProduct.Postgre:
                return new Npgsql.NpgsqlCommand();
            case SqlProduct.MySql:
                return new MySql.Data.MySqlClient.MySqlCommand();
            case SqlProduct.Oracle:
                return new Oracle.ManagedDataAccess.Client.OracleCommand();
            default:
                return new SqlCommand();
        }
    }

    // Summary:
    //     Gets or sets the text command to run against the data source.
    //
    // Returns:
    //     The text command to execute. The default value is an empty string ("").
    public string CommandText
    {
        get { return command.CommandText; }
        set { command.CommandText = value; }
    }
    //
    // Summary:
    //     Gets or sets the wait time before terminating the attempt to execute a command
    //     and generating an error.
    //
    // Returns:
    //     The time (in seconds) to wait for the command to execute. The default value
    //     is 30 seconds.
    //
    // Exceptions:
    //   System.ArgumentException:
    //     The property value assigned is less than 0.
    public int CommandTimeout
    {
        get { return command.CommandTimeout; }
        set { command.CommandTimeout = value; }
    }

    //
    // Summary:
    //     Indicates or specifies how the System.Data.IDbCommand.CommandText property
    //     is interpreted.
    //
    // Returns:
    //     One of the System.Data.CommandType values. The default is Text.
    public CommandType CommandType
    {
        get { return command.CommandType; }
        set { command.CommandType = value; }
    }
    //
    // Summary:
    //     Gets or sets the System.Data.IDbConnection used by this instance of the System.Data.IDbCommand.
    //
    // Returns:
    //     The connection to the data source.
    public IDbConnection Connection
    {
        get { return command.Connection; }
        set { command.Connection = value; }
    }
    //
    // Summary:
    //     Gets the System.Data.IDataParameterCollection.
    //
    // Returns:
    //     The parameters of the SQL statement or stored procedure.
    public IDataParameterCollection Parameters
    {
        get { return command.Parameters; }
    }
    //
    // Summary:
    //     Gets or sets the transaction within which the Command object of a .NET Framework
    //     data provider executes.
    //
    // Returns:
    //     the Command object of a .NET Framework data provider executes. The default
    //     value is null.
    public IDbTransaction Transaction
    {
        get { return command.Transaction; }
        set { command.Transaction = value; }
    }
    //
    // Summary:
    //     Gets or sets how command results are applied to the System.Data.DataRow when
    //     used by the System.Data.IDataAdapter.Update(System.Data.DataSet) method of
    //     a System.Data.Common.DbDataAdapter.
    //
    // Returns:
    //     One of the System.Data.UpdateRowSource values. The default is Both unless
    //     the command is automatically generated. Then the default is None.
    //
    // Exceptions:
    //   System.ArgumentException:
    //     The value entered was not one of the System.Data.UpdateRowSource values.
    public UpdateRowSource UpdatedRowSource
    {
        get { return command.UpdatedRowSource; }
        set { command.UpdatedRowSource = value; }
    }

    // Summary:
    //     Attempts to cancels the execution of an System.Data.IDbCommand.
    public void Cancel()
    {
        command.Cancel();
    }
    //
    // Summary:
    //     Creates a new instance of an System.Data.IDbDataParameter object.
    //
    // Returns:
    //     An IDbDataParameter object.
    public IDbDataParameter CreateParameter()
    {
        return command.CreateParameter();
    }
    //
    // Summary:
    //     Executes an SQL statement against the Connection object of a .NET Framework
    //     data provider, and returns the number of rows affected.
    //
    // Returns:
    //     The number of rows affected.
    //
    // Exceptions:
    //   System.InvalidOperationException:
    //     The connection does not exist.  -or- The connection is not open.
    public int ExecuteNonQuery()
    {
        return command.ExecuteNonQuery();
    }
    //
    // Summary:
    //     Executes the System.Data.IDbCommand.CommandText against the System.Data.IDbCommand.Connection
    //     and builds an System.Data.IDataReader.
    //
    // Returns:
    //     An System.Data.IDataReader object.
    public IDataReader ExecuteReader()
    {
        return command.ExecuteReader();
    }
    //
    // Summary:
    //     Executes the System.Data.IDbCommand.CommandText against the System.Data.IDbCommand.Connection,
    //     and builds an System.Data.IDataReader using one of the System.Data.CommandBehavior
    //     values.
    //
    // Parameters:
    //   behavior:
    //     One of the System.Data.CommandBehavior values.
    //
    // Returns:
    //     An System.Data.IDataReader object.
    public IDataReader ExecuteReader(CommandBehavior behavior)
    {
        return command.ExecuteReader(behavior);
    }
    //
    // Summary:
    //     Executes the query, and returns the first column of the first row in the
    //     resultset returned by the query. Extra columns or rows are ignored.
    //
    // Returns:
    //     The first column of the first row in the resultset.
    public object ExecuteScalar()
    {
        return command.ExecuteScalar();
    }
    //
    // Summary:
    //     Creates a prepared (or compiled) version of the command on the data source.
    //
    // Exceptions:
    //   System.InvalidOperationException:
    //     The System.Data.OleDb.OleDbCommand.Connection is not set.  -or- The System.Data.OleDb.OleDbCommand.Connection
    //     is not System.Data.OleDb.OleDbConnection.Open().
    public void Prepare()
    {
        command.Prepare();
    }


    // Summary:
    //     Performs application-defined tasks associated with freeing, releasing, or
    //     resetting unmanaged resources.
    public void Dispose()
    {
        command.Dispose();
    }

}

public interface ISchema
{
    string GetTableAndViewsNamesSelectStatement();
    string IsTableOrViewExistsSelectStatement(string tableName);
    string GetColumnsSelectStatement(string tableName);

}

public class DuradosFormatException : DuradosException
{
    public DuradosFormatException(Field field, string value, Exception innerException)
        : base("You cannot enter the value " + value + " into the field " + field.DisplayName + " because in is not in the right format.", innerException)
    {
    }

}

public class DuradosFkConstraintNotFoundException : DuradosException
{
    public DuradosFkConstraintNotFoundException()
        : base()
    {
    }

}

public class DuradosCreateException : DuradosException
{
    public View View { get; private set; }
    public Dictionary<string, object> Values { get; private set; }

    public DuradosCreateException(View view, Dictionary<string, object> values, Exception innerException)
        : base("Failed to edit row " + view.GetDisplayValue(values), innerException)
    {
        View = view;
        Values = values;
    }

}
public class RowNotFoundException : DuradosException
{
    public RowNotFoundException(View view, string pk)
        : base("The " + view.JsonName + " row with id = " + pk + " was not found.")
    {
    }
}



public class SqlTextBuilder : ISqlTextBuilder
{
    public virtual string GetAlterColumn(string columnName)
    {
        return " ALTER COLUMN ";
    }

    public virtual string GetQueryForLike()
    {
        return "like N";
    }

    public virtual string EscapeStringForQuery(string str)
    {
        return str.Replace("'", "''");
    }

    public virtual string GetQueryTemplateForLike(bool startWith, string match)
    {
        string like = GetQueryForLike();
        if (startWith)
            like += "'";
        else
            like += "'%";

        like += EscapeStringForQuery(match) + "%' ";

        return like;
    }

    public virtual string EscapeDbObject(string Name)
    {
        return EscapeDbObjectStart + Name + EscapeDbObjectEnd;
    }

    public virtual string EscapeDbObjectStart
    {
        get { return "["; }
    }

    public virtual string EscapeDbObjectEnd
    {
        get { return "]"; }
    }


    public virtual string DbTableColumnSeperator
    {
        get { return "."; }
    }

    public virtual string DbEquals
    {
        get { return " = "; }
    }

    public virtual string DbAnd
    {
        get { return " and "; }
    }

    public virtual string DbOr
    {
        get { return " or "; }
    }

    public virtual string AllColumns
    {
        get { return "*"; }
    }

    public virtual string WithNolock
    {
        get { return " with(nolock) "; }
    }

    public virtual string GetRowNumber(string orderByColumn)
    {
        return ", ROW_NUMBER() OVER(ORDER BY " + orderByColumn + ") as RowNum ";
    }

    public virtual string GetRowNumber(string orderByColumn, string sortOrder)
    {
        return ", ROW_NUMBER() OVER(ORDER BY " + EscapeDbObject(orderByColumn) + " " + sortOrder + ") as RowNum ";
    }
    public virtual string GetRowNumberNotEscaped(string orderByColumn, string sortOrder)
    {
        return ", ROW_NUMBER() OVER(ORDER BY " + orderByColumn + " " + sortOrder + ") as RowNum ";
    }
    public virtual string GetRowNumber(string orderByTable, string orderByColumn, string sortOrder)
    {
        return ", ROW_NUMBER() OVER(ORDER BY " + EscapeDbObject(orderByTable) + DbTableColumnSeperator + EscapeDbObject(orderByColumn) + " " + sortOrder + ") as RowNum ";
    }

    public virtual string GetPageOrder(string orderByColumn)
    {
        return string.Empty;
    }

    public virtual string GetPage(int page, int rowsCount)
    {
        return " and RowNum BETWEEN (" + page + " - 1) * " + rowsCount + " + 1 AND (" + page + " * " + rowsCount + ") ";
    }

    public virtual string GetLastInsertedRow(string tableName)
    {
        return "SELECT IDENT_CURRENT(N'[" + tableName + "]') AS ID";
    }

    public virtual string GetLastInsertedRow(View view)
    {
        string tableName = string.IsNullOrEmpty(view.EditableTableName) ? view.DataTable.TableName : view.EditableTableName;
        return GetLastInsertedRow(tableName);
    }
    public virtual string GetDateDiffDays(string start, string end)
    {
        return "datediff(dd," + start + "," + end + ")";
    }

    public virtual string GetDateAddDays(string days, string date)
    {
        return "dateadd(dd," + days + "," + date + ")";
    }

    public virtual string GetDateOnly(string date)
    {
        return GetDateAddDays(GetDateDiffDays("0", date), "0");
    }

    public virtual string Top(string sql, int top)
    {
        const string Select = "select ";
        const string SelectDistinct = "select distinct ";

        if (sql.ToLower().Contains(SelectDistinct))
        {
            return sql.Insert(sql.ToLower().IndexOf(SelectDistinct) + SelectDistinct.Length, "top(" + top + ") ");
        }
        else if (sql.ToLower().Contains(Select))
        {
            return sql.Insert(sql.ToLower().IndexOf(Select) + Select.Length, "top(" + top + ") ");
        }
        else
        {
            return sql;
        }
    }

    public virtual string FromDual()
    {
        return string.Empty;
    }

    public virtual string NLS
    {
        get { return "N"; }

    }
    public virtual string DbAs { get { return " AS "; } }

    public virtual string DbParameterPrefix { get { return "@"; } }




    public virtual string DbEndStatement
    {
        get
        {
            return string.Empty;
        }
    }



    public virtual string DbBeginStatement
    {
        get
        {
            return string.Empty;
        }
    }


    public virtual string DbEndOfStatement
    {
        get { return ";"; }
    }


    public virtual string GetLastInsertedRow2(View view)
    {
        return string.Empty;
    }

    public virtual string GetConvertDateToVarcharStatement(string escapedColumnName, string dateFormat)
    {
        return " convert(varchar, " + escapedColumnName + ", " + dateFormat + ") ";
    }

    public virtual string mmddyyyy
    {
        get { return "101"; }
    }

    public virtual string monddyyyy
    {
        get { return "107"; }
    }

    public virtual string InsertWithoutColumns()
    {
        return " default values ";
    }

    public virtual string GetPointFieldStatement(string tableName, string columnName)
    {
        return string.Empty;
    }



    public virtual string GetDecryptColumnStatement(string encryptedName, string databaseNames)
    {
        return string.Format(" CONVERT(NVARCHAR(250), DECRYPTBYKEY({0})) AS {1}, ", encryptedName, databaseNames);
    }

    public virtual string GetCloseCertificateStatement()
    {
        return " close SYMMETRIC KEY {0} ";
    }

    public virtual string GetOpenCertificateStatement()
    {
        return "OPEN SYMMETRIC KEY {0} DECRYPTION BY CERTIFICATE {1} ";
    }



    public virtual string GetDbEncryptedColumnParameterNameSql(string symetricKeyName, string columnName)
    {
        return "ENCRYPTBYKEY(KEY_GUID('" +symetricKeyName + "')," + DbParameterPrefix + columnName + ")" ;
    }

   
}

public class SqlMainSchema :ISqlMainSchema
{
    public virtual IDbConnection GetNewConnection()
    {
        return new SqlConnection();
    }
    public virtual IDbConnection GetNewConnection(string connectionString)
    {
        return new SqlConnection(connectionString);
    }
    public virtual IDbCommand GetNewCommand()
    {
        return new SqlCommand();
    }
    public virtual IDbCommand GetNewCommand(string sql, IDbConnection connection)
    {
        return new SqlCommand(sql,(SqlConnection)connection);
    }
    public  virtual  string GetEmailBySocialIdSql()
    {
        return "select UserId from durados_UserSocial where Provider = @Provider and SocialId = @SocialId and AppId = @AppId";
    }

    public  virtual  string GetEmailBySocialIdSql2()
    {
        return "select UserId from durados_UserSocial where Provider = @Provider and SocialId = @SocialId and AppId is null";
    }

    public  virtual  string GetSocialIdlByEmail()
    {
        return "select SocialId from durados_UserSocial WITH(NOLOCK) where Provider = @Provider and UserId = @UserId and AppId = @AppId";
    }

    public  virtual  string GetSocialIdlByEmail2()
    {
        return "select SocialId from durados_UserSocial WITH(NOLOCK) where Provider = @Provider and UserId = @UserId and AppId is null";
    }

    public  virtual  string InsertNewUserSql(string tableName,string userTable)
    {
        return "if NOT EXISTS (Select [Username] From  [" + tableName + "] WHERE [Username] = @Username) begin INSERT INTO ["+userTable+"] ([Username],[FirstName],[LastName],[Email],[Role],[Guid]) VALUES (@Username,@FirstName,@LastName,@Email,@Role,@Guid) end";
              
    }

    public  virtual  string GetInsertUserAppSql()
    {
        return "INSERT INTO [durados_UserApp] ([UserId],[AppId],[Role]) VALUES (@UserId,@AppId,@Role)";
    }

    public  virtual  string GetUserIdFromUsernameSql()
    {
        return "SELECT TOP 1 [durados_user].[id] FROM durados_user WITH(NOLOCK)  WHERE [durados_user].[username]=@username";
    }

    public  virtual  string GetUserTempTokenSql()
    {
        return "SELECT TOP 1 Id FROM [durados_ValidGuid] WITH(NOLOCK)  WHERE UserGuid=@UserGuid and Used=0";
    }

    public  virtual  string GetUserNameByGuidSql()
    {
        return "SELECT TOP 1 username FROM durados_user WITH(NOLOCK)  WHERE guid=@guid";
    }

    public  virtual  string GetDeleteUserSql()
    {
        return "delete FROM durados_user WHERE [username]=@username";
    }

    public  virtual  string GetUserBelongToMoreThanOneAppSql()
    {
        return "select id FROM durados_userapp WHERE [userid]=@userid and appid<>@appid";
    }

    public  virtual  string GetHasAppsSql()
    {
        return string.Format("SELECT TOP 1 id FROM durados_app WITH(NOLOCK)  WHERE creator=@id"); 
    }

    public  virtual  string GetInviteAdminBeforeSignUpSql(string username, string appId)
    {
        return string.Format("insert into durados_Invite (username, appId) values ('{0}', {1})", username,appId);
    }

    public  virtual  string GetInviteAdminAfterSignupSql(string username)
    {
        return string.Format("select appId from durados_Invite where username = '{0}'", username);
    }

    public  virtual  string GetInviteAdminAfterSignupSql(int userId, string appId, string role)
    {
        return string.Format("insert into durados_UserApp (UserId, AppId, Role) values ({0},{1},'{2}')", userId, appId, role);
    }

    public  virtual  string GetDeleteInviteUser(string username)
    {
        return string.Format("delete durados_Invite where Username = '{0}'", username);
    }

    public virtual string GetAppsPermanentFilter()
    {
 	    return "(durados_App.toDelete =0 AND (durados_App.Creator = [m_User] or durados_App.id in (select durados_UserApp.AppId from durados_UserApp where durados_UserApp.UserId = [m_User] and (durados_UserApp.Role = 'Admin' or durados_UserApp.Role = 'Developer'))))";
    }


    public virtual string GetWakeupCallToAppSql()
    {
        return "select [Id],[Url] from dbo.durados_App with (NOLOCK) where [Creator] is null";
    }


    public virtual string GetAppsCountsql()
    {
        return "SELECT COUNT(*) FROM dbo.durados_App a with(nolock) INNER JOIN dbo.durados_PlugInInstance p WITH(NOLOCK) ON a.id = p.Appid WHERE Deleted =0 AND p.selected=1";
    }

    public virtual string GetSqlProductSql()
    {
        return  "SELECT dbo.durados_SqlConnection.SqlProductId FROM dbo.durados_App WITH(NOLOCK) INNER JOIN dbo.durados_SqlConnection WITH(NOLOCK) ON dbo.durados_App.SqlConnectionId = dbo.durados_SqlConnection.Id WHERE (dbo.durados_App.Name = @AppName)";
    }

    
    public virtual string GetAppsExistsSql(string appName)
    {
        return "SELECT Id FROM durados_App WITH(NOLOCK) WHERE Name = N'" + appName + "'";
    }

    public virtual  string GetAppsExistsForUserSql(string appName, int? userId)
    {
        return "SELECT dbo.durados_App.Id FROM dbo.durados_App WITH(NOLOCK), dbo.durados_UserApp WITH(NOLOCK) WHERE (dbo.durados_App.Name = N'" + appName + "' AND ((dbo.durados_UserApp.UserId=" + userId + " AND dbo.durados_UserApp.AppId = dbo.durados_App.Id) or dbo.durados_App.Creator=" + userId + ") ) group by(dbo.durados_App.Id)";
    }

    public virtual string GetPaymentStatusSql(string appName)
    {
        return "SELECT PaymentStatus FROM durados_App with(nolock) WHERE Name = N'" + appName + "'";
    }

    public virtual string GetCurrentAppIdSql(string server, string catalog, string username, string userId)
    {
        return string.Format("SELECT Id FROM durados_SqlConnection WHERE ServerName=N'{0}' AND Catalog=N'{1}' AND Username=N'{2}' AND DuradosUser={3}", server, catalog, username, userId);
    }


    public virtual string GetPlanForAppSql(int appId)
    {
        return  "SELECT top(1) PlanId FROM durados_AppPlan WHERE AppId=" + appId + " order by PurchaseDate desc";
    }

    public virtual string GetFindAndUpdateAppInMainSql(int? templateId)
    {
        return 
        "begin tran getFromPool " +
                "declare @appId int " +
                "select top(1) @appId = id from durados_App with(UPDLOCK) where TemplateId " + (templateId.HasValue ? " = " + templateId.Value : " is null ").ToString() + " and creator = @poolCreator and DatabaseStatus = 1 order by id asc; " +
                "delete from durados_App where [Name] = @Name; " +
                "update durados_App " +
                "set creator = @creator, " +
                "[CreatedDate] = @CreatedDate, " +
                "[Name] = @Name, " +
                "[Title] = @Title " +
                "where id = @appId; " +
                "select @appId " +
                "commit tran getFromPool";
    }

    public virtual string GetAppNameByGuidFromDb(string guid)
    {
        return "select [name] from durados_App with(nolock) where [Guid] = '" + guid + "'";
    }
    public virtual string GetAppNamesWithPrefixSql(string appNamePrefix)
    {
        return "select name from durados_app where name like '" + appNamePrefix + "%'";
    }

    public virtual string GetDropDatabaseSql(string name)
    {
        return "ALTER DATABASE " + name + " SET SINGLE_USER WITH ROLLBACK IMMEDIATE; drop database " + name;
    }


    public virtual string GetHasOtherConnectiosSql(string appDatabase)
    {
        return "select count(*) from dbo.durados_SqlConnection where [Catalog] = N'" + appDatabase + "'";
    }

    
    public virtual string GetUpdateLogModelExceptionSql()
    {
        return "update [backand_model] set errorMessage = @errorMessage, errorTrace = @errorTrace where id=@id";
    }

    public virtual string GetSaveChangesIndicationFromDb2(string Id)
    {
        return "select ConfigChangesIndication from durados_App with(nolock) where id = " + Id;
    }

    public virtual string GetSetSaveChangesIndicationFromDbSql(int config, string Id)
    {
        return "UPDATE durados_App SET ConfigChangesIndication = " + config + " WHERE id = " + Id;
    }

    public virtual string GetLogModelSql()
    {
        return "INSERT INTO [backand_model] ([appName], [username], [timestamp], [input], [output], [valid], [action]) values (@appName, @username, @timestamp, @input, @output, @valid, @action); SELECT IDENT_CURRENT(N'backand_model') AS ID";
    }

    public virtual string GetAppLimitSql(string Id)
    {
        return "SELECT  Name, Limit FROM durados_AppLimits WITH(NOLOCK) WHERE AppId = " + Id;
    }

    public virtual string GetDeleteUserSql(int userId, string appId)
    {
        return string.Format("DELETE durados_UserApp WHERE UserId = {0} AND AppId = {1}",userId,appId);
    }


    public virtual string GetUpdateAppSystemConnectionSql(int? sysConnId, string primaryKey)
    {
        return "UPDATE durados_App SET SystemSqlConnectionId = " + sysConnId + " WHERE id = " + primaryKey + ";";

    }

    public virtual string GetUpdateDBStatusSql(int onBoardingStatus, int appId)
    {
        return "UPDATE durados_App SET DatabaseStatus = " + onBoardingStatus + " WHERE id = " + appId + ";";
    }


    public virtual string GetAppIdSql(int templateId)
    {
        return "SELECT AppId FROM durados_Template with(NOLOCK) WHERE id = " + templateId; 
    }

    public virtual string GetDeleteAppById(int id)
    {
        return "DELETE durados_App with (rowlock) WHERE Id = " + id;
    }

    public virtual string GetUpdateAppConnectionsSql(int? appConnId, int? sysConnId, string primaryKey)
    {
        return "UPDATE durados_App SET SqlConnectionId = " + appConnId + ", SystemSqlConnectionId = " + sysConnId + " WHERE id = " + primaryKey;
                
    }

    public virtual string GetExecCreateDB(string sysCatalog)
    {
        return string.Format("EXEC('CREATE DATABASE {0}')", sysCatalog);
    }

    public virtual string GetUpdateAppProduct()
    {
        return "UPDATE durados_App SET productType = @productType WHERE Name = @name"; 
    }


    public virtual string GetDbStatusSql(string appId)
    {
        return "SELECT DatabaseStatus FROM dbo.durados_App WITH (NOLOCK) WHERE id = " + appId; ;
    }


    public virtual string GetAppNameByIdSqlSql(int appId)
    {
            return "SELECT Name FROM dbo.durados_App WITH (NOLOCK) WHERE id = " + appId;
            
    }


    public virtual  string InsertNewConnectionToExternalServerTable()
    {
        return "INSERT INTO durados_ExternaInstance(InstanceName ,DbName ,IsActive,Endpoint,SqlConnectionId) VALUES(@serverName,@catalog,@IsActive,@serverName,@SqlConnectionId);SELECT IDENT_CURRENT(N'durados_ExternaInstance') AS Id";
           
    }


    public virtual string GetValidateSelectFunctionExistsSql()
    {
        return @"IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[f_report_connection_type]') AND type in (N'FN', N'IF', N'TF', N'FS', N'FT'))
                BEGIN
                execute dbo.sp_executesql @statement = N'
                -- =============================================
                -- Author:		<Author,,Name>
                -- Create date: <Create Date, ,>
                -- Description:	<Description, ,>
                -- =============================================
                Create FUNCTION [dbo].[f_report_connection_type] 
                (
	                -- Add the parameters for the function here
	                @id int
                )
                RETURNS int
                AS
                BEGIN
	                 -- Declare the return variable here
	                DECLARE @ResultVar int
					 --1	console   --2	free
	                -- Add the T-SQL statements to compute the return value here
	                select @ResultVar = CASE 
						WHEN ServerName IN(SELECT ServerName 
						FROM durados_ExternaInstance WITH(NOLOCK) 
						INNER JOIN durados_SqlConnection WITH(NOLOCK) ON durados_SqlConnection.Id = durados_ExternaInstance.SqlConnectionId)
						  THEN 2  
	                ELSE 1 END 
	                FROM dbo.durados_SqlConnection c with (NOLOCK) 
	                where id=@id

	                -- Return the result of the function
	                RETURN @ResultVar
                END

                ' 
                END
                ";
    }

    public virtual  string GetCreatorSql(int appId)
    {
        return "SELECT Creator FROM dbo.durados_App WITH (NOLOCK) WHERE dbo.durados_App.Id = " + appId; 
    }

    public virtual string GetCreatorUsername(int appId)
    {
        return "SELECT dbo.durados_User.[Username] FROM dbo.durados_App WITH (NOLOCK) INNER JOIN dbo.durados_User WITH (NOLOCK) ON dbo.durados_App.Creator = dbo.durados_User.ID WHERE dbo.durados_App.Id = " + appId;
    }

    public virtual  string GetNewDatabaseNameSql(int plugInType, int templateAppId)
    {
        return "SELECT DatabaseName, DbCount FROM durados_SampleApp WITH (NOLOCK) WHERE PlugInId = " + plugInType + " AND AppId = " + templateAppId;
    }


    public virtual string GetAppSql()
    {
        return @"SELECT a.Id, a.Name,  dbo.f_report_connection_type(a.SqlConnectionId) AS AppType, 
                             a.Creator,cnn.ServerName, cnn.catalog ,syscnn.ServerName sysServerName,syscnn.catalog sysCatalog
                            FROM dbo.durados_App AS a WITH (NOLOCK) INNER JOIN dbo.durados_SqlConnection AS cnn WITH (NOLOCK) ON a.SqlConnectionId = cnn.Id INNER JOIN dbo.durados_SqlConnection AS syscnn WITH (NOLOCK) ON a.SystemSqlConnectionId = syscnn.Id
                            WHERE   [ToDelete]<>1"; ;
    }


    public virtual string GetUserGuidSql()
    {
        return "SELECT TOP 1 [durados_User].[guid] FROM durados_user WITH(NOLOCK)  WHERE [durados_User].[Username]=@username";
    }




    public virtual string GetAppRowByNameSql(string appName)
    {
        return string.Format("SELECT * FROM [durados_app] WITH(NOLOCK)  WHERE [Name] = '{0}'", appName);
    }



    public virtual string GetAppNameByTokenSql(string HeaderToken)
    {
        return string.Format("SELECT [Name] FROM [durados_app] WITH(NOLOCK)  WHERE [{0}] = @token", HeaderToken);
    }


    public virtual string GetUpdateAppToBeDeleted()
    {
        return "UPDATE durados_App SET [ToDelete]=1,[deleteddate] =getdate() WHERE Id=@Id"; 
    }


    public virtual string GetValidateUserSql(int appID, int userId)
    {
        return string.Format("SELECT Cast( case when exists(SELECT 1 FROM durados_App WITH(NOLOCK)  WHERE durados_app.[ToDelete]=0 AND  Id = {0} AND Creator = {1}) or exists(SELECT 1 FROM dbo.durados_UserApp WITH(NOLOCK)  WHERE  AppId = {0} AND UserId = {1}) then 1 else 0 end as bit)", appID, userId);
    }

    public virtual string GetLoadUserDataByGuidSql()
    {
        return string.Format("SELECT TOP 1 username FROM durados_user WITH(NOLOCK)  WHERE guid=@guid");
    }

    public virtual string GetLoadUserDataByUsernameSql(string userFields, string userViewName, string userFieldName)
    {
        return string.Format("SELECT TOP 1 {0} FROM {1} WITH(NOLOCK)  WHERE {2}=@username", userFields, userViewName, userFieldName);
    }

   

    public virtual string GetUsernameByUsernameSql()
    {
        return "SELECT TOP 1 [Username] FROM [durados_User] WHERE [Username]=@Username";
    }

    public virtual string GetUsernameByUsernameInUseSql()
    {
        return "SELECT TOP 1 [Username] FROM [User] WHERE [Username]=@Username";
    }

    public virtual string InsertIntoPluginRegisterUsersSql()
    {
        return "INSERT INTO durados_PlugInRegisteredUser (PlugInUserId ,PlugInId, RegisteredUserId, SelectionDate) VALUES (@PlugInUserId ,@PlugInId, @RegisteredUserId, @SelectionDate)";
    }

    public virtual string InsertIntoUserSql()
    {
        return "INSERT INTO [User] ([Username],[FirstName],[LastName],[Email],[Password],[Role],[NewUser],[Comments]) VALUES (@Username,@FirstName,@LastName,@Email,@Password,@Role,@NewUser,@Comments)";
    }


    public virtual string GetExternalConnectionIdsSql()
    {
        return "SELECT  SqlConnectionId  FROM durados_ExternaInstance WITH(NOLOCK) INNER JOIN durados_SqlConnection WITH(NOLOCK) on durados_SqlConnection.Id = durados_ExternaInstance.SqlConnectionId";
    }


    public virtual string GetDeleteAppByName(string id)
    {
        return "DELETE durados_App WHERE Name = '" + id + "'";
    }


    public virtual string GetAppGuidByName()
    {
        return "SELECT [Guid] FROM [durados_app] WITH(NOLOCK)  WHERE [Name] =@appName";
    }
    public virtual string GetAppGuidById()
    {
        return "SELECT [Guid] FROM [durados_app] WITH(NOLOCK)  WHERE [Id] =@Id";
    }

    public virtual string GetUserFieldsForSelectSql()
    {
        return "[{0}],[{1}],[{2}],[{3}],[{4}]";
        //return "SELECT TOP 1 [durados_user].[guid] FROM durados_user WITH(NOLOCK)  WHERE [durados_user].[username]=@username";

    }


    public virtual string GetUserAappIdSql()
    {
        return "SELECT TOP 1 [ID] FROM [durados_UserApp] WHERE [UserId]=@UserId AND [AppId]=@AppId";
    }


    public virtual string GetAppsNameSql()
    {
        return "SELECT dbo.durados_App.Name FROM dbo.durados_App WITH(NOLOCK) INNER JOIN dbo.durados_SqlConnection WITH(NOLOCK) ON dbo.durados_App.SqlConnectionId = dbo.durados_SqlConnection.Id WHERE (dbo.durados_SqlConnection.Id = 1)"; 
    }


    public virtual string GetInsertLimitsSql(Limits limits, int limit, int? id)
    {
        return "SET TRANSACTION ISOLATION LEVEL SERIALIZABLE; " +
                                   "BEGIN TRANSACTION; " +
                                   "UPDATE dbo.durados_AppLimits SET Limit = " + limit + " WHERE AppId = " + id + " and Name = '" + limits.ToString() + "';" +
                                   " IF @@ROWCOUNT = 0 " +
                                   "BEGIN " +
                                     "INSERT into dbo.durados_AppLimits (Name, Limit, AppId) values ('" + limits.ToString() + "'," + limit + "," + id.Value + "); " +
                                   "END " +
                                   "COMMIT TRANSACTION;";
    }


    public virtual string GetUpdateAppConnectionAndProductSql(string newConnectionId, string image, string pk)
    {
        return "UPDATE durados_App SET SqlConnectionId = " + newConnectionId + ",Image= '" + image + "', DataSourceTypeId=2 WHERE Id = " + pk;
    }


    public virtual string GetInsertIntoUsersSql(string viewName)
    {
        return "INSERT INTO [" + viewName + "] ([Username],[FirstName],[LastName],[Email],[Role],[Guid]) VALUES (@Username,@FirstName,@LastName,@Email,@Role,@Guid)";
    }
    public virtual string GetInsertIntoUsersSql2(string viewName)
    {
        return "INSERT INTO [" + viewName + "] ([Username],[FirstName],[LastName],[Email],[Role],[Guid],[IsApproved]) VALUES (@Username,@FirstName,@LastName,@Email,@Role,@Guid,@IsApproved)";
    }
}