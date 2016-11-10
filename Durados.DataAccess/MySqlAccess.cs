using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using MySql.Data.MySqlClient;

namespace Durados.DataAccess
{
    public class MySqlAccess : SqlAccess
    {
        public static bool IsMySqlConnectionString(string connectionString)
        {
            return connectionString.StartsWith("server=");
        }
        public static string GetConnectionStringSchema(bool useSSH)
        {
            return (useSSH ? "server=localhost;database={1};User Id={2};password={3};CharSet=utf8;UseProcedureBodies=true;" : "server={0};database={1};User Id={2};password={3}") + ";port={4};convert zero datetime=True;default command timeout=90;Connection Timeout=60;CharSet=utf8;UseProcedureBodies=true;";
        }
        protected override System.Data.IDbConnection GetNewConnection(string connectionString)
        {
            if (IsMySqlConnectionString(connectionString))
            {
                MySqlConnection connection = new MySqlConnection(connectionString);// + ";port=1234");

                return connection;
            }
            else
                return base.GetNewConnection(connectionString);
        }

        protected override System.Data.IDbCommand GetNewCommand(string cmdText, System.Data.IDbConnection connection)
        {
            if (connection is MySqlConnection)
                return new MySqlCommand(cmdText, (MySqlConnection)connection);
            else
                return base.GetNewCommand(cmdText, connection);
        }

        protected override System.Data.IDataParameter GetNewParameter(System.Data.IDbCommand command, string parameterName, object value)
        {
            if (command is MySqlCommand)
                return new MySqlParameter(parameterName, value);
            else
                return base.GetNewParameter(command, parameterName, value);
        }

        protected override string GetPointParameterValue(Field field, Dictionary<string, object> values)
        {
            if (!(values[field.Name] is object[]))
                return values[field.Name].ToString();

            object[] point = (object[])values[field.Name];
            if (point.Length != 2)
                return values[field.Name].ToString();

            return string.Format("POINT({0} {1})", point[0], point[1]);
        }

        const char comma = ',';

        protected override string GetPointUpdateSetColumn(Field field, string columnName, ISqlTextBuilder sqlTextBuilder)
        {
            return sqlTextBuilder.EscapeDbObject(columnName) + sqlTextBuilder.DbEquals + "GeomFromText(" + sqlTextBuilder.DbParameterPrefix + columnName.ReplaceNonAlphaNumeric() + ")" + comma;
        }

        protected override string GetPointForInsert(Field field, string columnName, ISqlTextBuilder sqlTextBuilder)
        {
            return "GeomFromText(" + sqlTextBuilder.DbParameterPrefix + columnName.ReplaceNonAlphaNumeric() + ")" + comma;
        }

        protected override System.Data.IDataParameter GetNewParameter(View view, string parameterName, object value)
        {
            if (IsMySqlConnectionString(view.ConnectionString))
                return new MySqlParameter(parameterName, value);
            else
                return base.GetNewParameter(view, parameterName, value);
        }
        //protected override object ChangeType(DataColumn dataColumn, string value)
        //{
        //    if ((value.ToLower() == bool.TrueString.ToLower() || value.ToLower() == bool.FalseString.ToLower()) && dataColumn.DataType.Equals(typeof(int)))
        //    {
        //        value = (Convert.ToBoolean(value) ? 1 : 0).ToString();
        //    }
            

        //    return Convert.ChangeType(value, dataColumn.DataType);
        //}

        protected override string GetPointFieldStatement(Field field)
        {
            return new MySqlTextBuilder().GetPointFieldStatement(field.View.DataTable.TableName, field.GetColumnsNames()[0]);
            //return string.Format("AsText(`{0}`.`{1}`) as `{1}`", field.View.DataTable.TableName, field.GetColumnsNames()[0]);
        }
       
        protected override System.Data.IDataAdapter GetNewAdapter(System.Data.IDbCommand command)
        {
            if (command is MySqlCommand)
                return new MySqlDataAdapter((MySqlCommand)command);
            else
                return base.GetNewAdapter(command);
        }

        protected override ISqlTextBuilder GetSqlTextBuilder(View view)
        {
            if (view == null || string.IsNullOrEmpty(view.ConnectionString) || IsMySqlConnectionString(view.ConnectionString))
                return new MySqlTextBuilder();
            else
                return base.GetSqlTextBuilder(view);
        }

        protected override int Fill(IDataAdapter adapter, DataTable table)
        {
            if (adapter is MySqlDataAdapter)
                return ((MySqlDataAdapter)adapter).Fill(table);
            else
                return base.Fill(adapter, table);
        }

        //public override int RowCount(View view)
        //{
        //    return RowFilterCount(view, null);
        //}

        protected override string GetParameterType(IDataParameter parameter)
        {
            if (parameter is MySqlParameter)
                return ((MySqlParameter)parameter).DbType.ToString();
            else
                return base.GetParameterType(parameter);
        }

        public override string GetDbTypeofDataType(DataType dataType)
        {
            switch (dataType)
            {
                case DataType.Boolean:
                    return "BINARY(1)";

                case DataType.DateTime:
                    return "datetime";

                case DataType.LongText:
                    return "CHAR(2000)";

                case DataType.Numeric:
                    return "DECIMAL";

                case DataType.Image:
                case DataType.Url:
                case DataType.Email:
                case DataType.Html:
                case DataType.ShortText:
                    return "CHAR(500)";

                default:
                    throw new DuradosException("Unsupported data type " + dataType.ToString());

            }
        }

        protected override string GetEscapeFieldFormula(string formula)
        {
            return "(" + formula.Replace('[', '`').Replace(']', '`') + ")";
        }

        public override SqlSchema GetNewSqlSchema()
        {
            return new MySqlSchema();
        }

        public override int RowCount(View view)
        {
            try
            {
                //string sql = "SELECT rows FROM sys.sysindexes  AS s1 WHERE (rows > 0) AND (indid IN (SELECT MIN(indid) AS Expr1 FROM sys.sysindexes AS s2 WHERE (s1.id = id))) AND (id = OBJECT_ID('" + view.DataTable.TableName + "'))";

                using (IDbConnection connection = GetNewConnection(view.ConnectionString))
                {
                    connection.Open();

                    SqlSchema schema = GetNewSqlSchema();

                    string sql = schema.GetPrimaryIndexName(view.DataTable.TableName);


                    IDbCommand command = GetNewCommand(sql, connection);


                    object scalar = command.ExecuteScalar();

                    if (scalar == null || scalar == DBNull.Value)
                    {
                        return RowFilterCount(view, GetFilter(view, new Dictionary<string, object>(), LogicCondition.And, false, null));
                    }

                    string indexMame = scalar.ToString();

                    sql = schema.GetTableRowsCount(view.DataTable.TableName, indexMame);

                    command.CommandText = sql;

                    scalar = command.ExecuteScalar();

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

        public override bool IsLoginFailureException(Exception exception)
        {
            if (!(exception is MySqlException))
                return false;

            switch (((MySqlException)exception).Number)
            {
                case 1041: // Server error 
                case 1042: // IP address of the client
                case 1044: // db Failed 
                case 1045: // Login Failed 
                case 1049: // Check the DB Name 
                    return true;
                default:
                    return false;
            }
        }

        
        public override void DeleteDatabase(string connectionString, string catalog)
        {

            string sql = "DROP DATABASE `" + catalog + "`";
            sql = string.Format(sql, catalog);

            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                connection.Open();
                //connection.ChangeDatabase("master");
                using (MySqlCommand command = new MySqlCommand(sql, connection))
                {
                    command.ExecuteNonQuery();
                }
            }
        }

        public override void CopyDatabase(string adminConnectionString, string sourceConnectionString, string targetConnectionString)
        {
            MySqlConnection conn = new MySqlConnection(sourceConnectionString);
            string sourceDatabaseName = conn.Database;
            conn = new MySqlConnection(targetConnectionString);
            string targetDatabaseName = conn.Database;

            MySqlConnectionStringBuilder connBuilder = new MySqlConnectionStringBuilder(adminConnectionString);
            string username = connBuilder.UserID;
            string password = connBuilder.Password;

            CopyDatabase(adminConnectionString, sourceDatabaseName, username, password, targetDatabaseName);
        }


        public void CopyDatabase(string adminConnectionString, string sourceDatabaseName, string username, string password, string targetDatabaseName)
        {
            MySqlConnection conn = new MySqlConnection(adminConnectionString);
            MySqlCommand cmd;
            string s0;

            try
            {
                conn.Open();
                s0 = "mysqldump <sourcedb> -u <USERNAME> -p<PASS> | mysql <targetdb> -u <USERNAME> -p<PASS>".Replace("<sourcedb>", sourceDatabaseName).Replace("<targetdb>", targetDatabaseName).Replace("<USERNAME>", username).Replace("<PASS>", password);
                //s0 = "mysqldump -h localhost -u <USERNAME> -p<PASS> <sourcedb> | mysql -h localhost -u <USERNAME> -p<PASS> <targetdb>".Replace("<sourcedb>", sourceDatabaseName).Replace("<targetdb>", targetDatabaseName).Replace("<USERNAME>", username).Replace("<PASS>", password);
                cmd = new MySqlCommand(s0, conn);
                cmd.ExecuteNonQuery();
                conn.Close();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
                throw new DuradosException("Failed to copy MySQL database " + sourceDatabaseName + " to " + targetDatabaseName);
            } 
        }
    }


    public class MySqlTextBuilder : SqlTextBuilder
    {
        public override string FromDual()
        {
            return " FROM DUAL ";
        }

        public override string GetAlterColumn(string columnName)
        {
            return " CHANGE COLUMN " + columnName + " ";
        }

        public override string EscapeDbObjectStart
        {
            get { return "`"; }
        }

        public override string EscapeDbObjectEnd
        {
            get { return "`"; }
        }

        public override string GetRowNumber(string orderByColumn)
        {
            return string.Empty; 
        }

        public override string GetRowNumber(string orderByTable, string orderByColumn, string sortOrder)
        {
            return string.Empty;
        }

        public override string GetPageOrder(string orderByColumn)
        {
            if (string.IsNullOrEmpty(orderByColumn) || orderByColumn.Trim().Equals(string.Empty))
                return string.Empty;
            return " order by " + orderByColumn + " ";
        }

        public override string GetPage(int page, int rowsCount)
        {
            return " LIMIT " + ((page - 1) * rowsCount) + ", " + rowsCount + " ";
        }

        public override string WithNolock
        {
            get { return string.Empty; }
        }

        public override string GetLastInsertedRow(string tableName)
        {
            return "SELECT LAST_INSERT_ID() AS ID";
        }

        public override string GetLastInsertedRow(View view)
        {
            return "SELECT LAST_INSERT_ID() AS ID";
        }

        public override string GetDateDiffDays(string start, string end)
        {
            return "DATEDIFF(" + end + "," + start + ")";
        }

        public override string GetDateAddDays(string days, string date)
        {
            return "DATE_ADD(" + date + ", INTERVAL " + days + " DAY)";
        }

        public override string GetDateOnly(string date)
        {
            return "date(" + date + ")";
        }

        public override string Top(string sql, int limit)
        {
            return sql + " LIMIT " + limit;
        }

        public override string NLS
        {
            get
            {
                return string.Empty;
            }
        }

        public override string GetConvertDateToVarcharStatement(string escapedColumnName, string dateFormat)
        {
            return "DATE_FORMAT(" + escapedColumnName + ", '" + dateFormat + "')";
        }

        public override string mmddyyyy
        {
            get { return "%m/%d/%Y"; }
        }

        public override string monddyyyy
        {
            get { return "%m/%d/%Y"; }
        }

        public override string InsertWithoutColumns()
        {
            return " () values () ";
        }

        public override string GetPointFieldStatement(string tableName, string fieldName)
        {
            return string.Format("CONCAT(X(`{0}`.`{1}`), \", \", Y(`{0}`.`{1}`))  as `{1}`", tableName, fieldName);
           
        }
    }

    public class MySqlSchema : SqlSchema
    {
        public override string GetDataType(DataType dataType)
        {
            switch (dataType)
            {
                case DataType.Boolean:
                    return "bit  NULL";

                case DataType.DateTime:
                    return "DateTime  NULL";

                case DataType.LongText:
                    return "varchar(4000)  NULL";

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
                    return "varchar(250)  NULL";

                default:
                    throw new DuradosException("Ilegal data type " + dataType.ToString());

            }
        }

        public bool IsMySqlConnectionString(string connectionString)
        {
            return connectionString.StartsWith("server=");
        }

        public override string GetTableRowsCount(string tableName)
        {
            return "SELECT TABLE_ROWS as rows FROM information_schema.tables WHERE table_schema = DATABASE() and TABLE_NAME = '" + tableName + "'";
        }

        public override string GetTotalRowsCount()
        {
            return "SELECT SUM(TABLE_ROWS) as rows FROM information_schema.tables WHERE table_schema = DATABASE()";
        }

        public override string GetMaxTableRowsCount()
        {
            return "SELECT MAX(TABLE_ROWS) as rows FROM information_schema.tables WHERE table_schema = DATABASE()";
        }

        public override string GetPrimaryIndexName(string tableName)
        {
            return "select index_name from information_schema.statistics where table_schema = DATABASE() and table_name='" + tableName + "' and non_unique = 0";
        }

        public override string GetTableRowsCount(string tableName, string indexName)
        {
            return "select count(*) from `" + tableName + "` use index(" + indexName + ")";
        }

        public override string GetEntitiesSelectStatement()
        {
            return "select table_name as Name, table_schema as `Schema`, table_type as EntityType from information_schema.tables where table_schema = DATABASE()";
        }

        public override string GetTableNamesSelectStatement()
        {
            return "select table_name as Name, table_schema as `Schema`, table_type as EntityType from information_schema.tables where table_schema = DATABASE() and table_type = 'BASE TABLE'";
        }

        public override string CountTablesSelectStatement()
        {
            return "select Count(*) from information_schema.tables where table_schema = DATABASE() and table_type = 'BASE TABLE'";
        }

        public override string GetTableNamesSelectStatementWithFilter()
        {
            return "select table_name as Name, table_schema as `Schema`, table_type as EntityType from information_schema.tables where table_schema = DATABASE() and table_type = 'BASE TABLE' AND table_name  like N'%[filter]%'" ;
        }
        public override System.Data.IDbCommand GetCommand()
        {
            return new DuradosCommand(SqlProduct.MySql);
        }

        public override IDbConnection GetConnection(string connectionString)
        {
            if (IsMySqlConnectionString(connectionString))
            {
                MySqlConnection connection = new MySqlConnection(connectionString);
                return connection;
            }
            else
                return base.GetConnection(connectionString);
        }

        public override IDbCommand GetCommand(string commandText, IDbConnection connection)
        {
            if (connection is MySqlConnection)
                return new MySqlCommand(commandText, (MySqlConnection)connection);
            else
                return base.GetCommand(commandText, connection);
        }
        public override IDbCommand GetNewCommand()
        {
            return new MySqlCommand();
        }
        protected override string GetDefaultSchema(IDbCommand command)
        {
            string defaulSchema = "";
            try
            {
                command.CommandText = GetDefaultSchemaSelectStatement();
                defaulSchema = command.ExecuteScalar().ToString();
            }
            catch { }

            return defaulSchema;
        }

        public override string GetDefaultSchemaSelectStatement()
        {
            return "select DATABASE() as default_schema";
        }

        protected override bool IsValidSchema(string schema, string defaultSchema)
        {
            return true;
        }

        public override string IsTableOrViewExistsSelectStatement(string tableName)
        {
            return "select table_name as Name, table_schema as `Schema`, table_type as EntityType from information_schema.tables where table_schema = DATABASE() and table_name = '" + tableName + "'";
 
        }

        public override string IsViewExistsSelectStatement(string viewName)
        {
            return "select table_name as Name, table_schema as `Schema`, table_type as EntityType from information_schema.tables where table_schema = DATABASE() and table_name = '" + viewName + "' and table_type='VIEW'";
        }

        public override string GetColumnsSelectStatement(string tableName)
        {
            return "select * from information_schema.columns where table_schema = DATABASE() and table_name = '" + tableName + "'";

        }

        public override string GetAutoIdentityColumns(string tableName)
        {
            return "select COLUMN_NAME, TABLE_NAME from information_schema.columns where table_schema = DATABASE() and extra = 'auto_increment' and table_name = '" + tableName + "'";
        }

        public override string GetPrimaryKeyColumns(string tableName)
        {
            return "select COLUMN_NAME from information_schema.columns where table_schema = DATABASE() and column_key = 'PRI' and table_name = '" + tableName + "'";
        }

        protected override string GetMyForeignKeyConstraintsSql(string tableName)
        {
            return "select table_name as TableName, column_name as ColumnName, referenced_table_name as ReferenceTableName, referenced_column_name as ReferenceColumnName from information_schema.key_column_usage where referenced_table_name is not null and table_schema = DATABASE() and table_name = '" + tableName + "'";
        }

        public override string GetForeignKeyConstraints()
        {
            return "select CONSTRAINT_NAME name, TABLE_NAME as TableName, COLUMN_NAME as ColumnName, REFERENCED_TABLE_NAME ReferenceTableName,  REFERENCED_COLUMN_NAME ReferenceColumnName from information_schema.key_column_usage where referenced_table_name is not null and table_schema = DATABASE() ";
        }

        public override string GetForeignKeyConstraint(string tableName, string referenceTableName, string columnName)
        {
            return "select constraint_name as TableName, column_name as ColumnName, referenced_table_name as ReferenceTableName, referenced_column_name as ReferenceColumnName from information_schema.key_column_usage where referenced_table_name is not null and table_schema = DATABASE() and table_name = '" + tableName + "' and referenced_table_name = '" + referenceTableName + "' and column_name ='" + columnName + "'";
        }

        public override string GetForeignKeyConstraintsToMe(string tableName)
        {
            return "select table_name as TableName, column_name as ColumnName, referenced_table_name as ReferenceTableName, referenced_column_name as ReferenceColumnName from information_schema.key_column_usage where referenced_table_name is not null and table_schema = DATABASE() and referenced_table_name = '" + tableName + "'";
        }

        public override string GetColumnDataTypeSelectStatement(string tableName, string columnName)
        {
            return "select IF(`CHARACTER_MAXIMUM_LENGTH` IS NULL, `data_type`, concat(data_type, concat('(',`CHARACTER_MAXIMUM_LENGTH`, ')'))) as dt from INFORMATION_SCHEMA.COLUMNS IC where TABLE_NAME = '" + tableName + "' and COLUMN_NAME = '" + columnName + "'";

        }


        protected override string GetCreateTableStatement(DataTable table)
        {
            System.Text.StringBuilder sb = new StringBuilder();

            sb.AppendFormat("CREATE TABLE " + sqlTextBuilder.EscapeDbObject("{0}") + "(", table.TableName);
            foreach (DataColumn column in table.Columns)
            {
                sb.Append(GetCreateColumnScript(column));
                sb.Append(",");
            }
            sb.AppendFormat(" PRIMARY KEY (" + sqlTextBuilder.EscapeDbObject("{0}") + "), UNIQUE KEY " + sqlTextBuilder.EscapeDbObject("{0}" + "_UNIQUE") + " (" + sqlTextBuilder.EscapeDbObject("{0}") + ") ) ENGINE=InnoDB ", table.PrimaryKey[0].ColumnName);

            return sb.ToString();


        }

        protected override string GetCreateColumnScript(DataColumn column)
        {
            string script = sqlTextBuilder.EscapeDbObject("{0}") + " {1} {2} {3}";

            string name = column.ColumnName;
            string type = GetColumnTypeScript(column);
            string nullable = column.AutoIncrement ? "NOT NULL" : "NULL";
            string identity = column.AutoIncrement ? "AUTO_INCREMENT" : string.Empty;
            
            return string.Format(script, name, type, identity, nullable);
        }

        protected override string GetColumnTypeScript(DataColumn column)
        {
            if (column.DataType == typeof(int))
                return "int(11)";
            else if (column.DataType == typeof(DateTime))
                return "datetime";
            else if (column.DataType == typeof(double))
                return "float";
            else if (column.DataType == typeof(bool))
                return "binary(1)";
            else
            {
                return string.Format("nvarchar({0})", column.MaxLength);
            }
        }

        protected override string GetCreateFkConstraintStatement(string fkTableName, string fkColumnName, string pkTableName, string pkColumnName)
        {
            string name = "FK_" + fkTableName + "_" + pkTableName + "_" + fkColumnName;
            if (name.Length >= 60)
            {
                name = name.Substring(0, 20) + "_" + Guid.NewGuid().ToString();
            }
            return "ALTER TABLE " + sqlTextBuilder.EscapeDbObject(fkTableName) + " ADD CONSTRAINT " + sqlTextBuilder.EscapeDbObject(name) + " FOREIGN KEY (" + sqlTextBuilder.EscapeDbObject(fkColumnName) + ") REFERENCES " + sqlTextBuilder.EscapeDbObject(pkTableName) + " (" + sqlTextBuilder.EscapeDbObject(pkColumnName) + ") ON DELETE NO ACTION ON UPDATE NO ACTION, ADD INDEX " + sqlTextBuilder.EscapeDbObject(name + "_idx") + " (" + sqlTextBuilder.EscapeDbObject(fkColumnName) + " ASC)";
            
        }

        protected void ChangeTableEngine(string table, string engine, IDbCommand command)
        {
            string sql = "ALTER TABLE " + sqlTextBuilder.EscapeDbObject(table) + " ENGINE = " + engine;

            command.CommandText = sql;
            try
            {
                command.ExecuteNonQuery();
            }
            catch (Exception exception)
            {
                throw new DuradosException("Could not change the table: " + table + " engine to: " + engine + ".\r\nAdditional information:\n " + exception.Message, exception);
            }
        }

        protected string GetTableEngine(string table, IDbCommand command)
        {
            string sql = "SELECT ENGINE FROM information_schema.TABLES WHERE TABLE_SCHEMA = DATABASE() AND TABLE_NAME = '" + table + "'";

            command.CommandText = sql;
            try
            {
                object scalar = command.ExecuteScalar();
                if (scalar == null || scalar == DBNull.Value)
                {
                    throw new DuradosException("Could not get the table storage engine: " + table);
                }

                return scalar.ToString();
            }
            catch (Exception exception)
            {
                throw new DuradosException("Could not get the table storage engine: " + table + ".\r\nAdditional information:\n " + exception.Message, exception);
            }
        }

        protected HashSet<string> FkEngines = new HashSet<string>() {"innodb" };
        public override void CreateFkConstraint(string fkTableName, string fkColumnName, string pkTableName, string pkColumnName, IDbCommand command)
        {
            return;
            string fkEngine = GetTableEngine(fkTableName, command).ToLower();
            string pkEngine = GetTableEngine(fkTableName, command).ToLower();

            //ChangeTableEngine(pkTableName, engine, command);

            if (!fkEngine.Equals(pkEngine))
                return;

            if (!FkEngines.Contains(fkEngine))
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

        public override void DropFkConstraint(string tableName, string referenceTableName, string columnName, IDbCommand command)
        {
            string sql = GetForeignKeyConstraint(tableName, referenceTableName, columnName);
            command.CommandText = sql;
            try
            {
                object scalar = command.ExecuteScalar();

                if (scalar == null || scalar == DBNull.Value)
                    return;

                string fkConstraintName = scalar.ToString();
                
                sql = "ALTER TABLE " +  sqlTextBuilder.EscapeDbObject(tableName) + " DROP FOREIGN KEY " + sqlTextBuilder.EscapeDbObject(fkConstraintName);

                command.CommandText = sql;

                command.ExecuteNonQuery();
            }
            catch (Exception exception)
            {
                throw new DuradosException("Could not drop the relation between the parent view " + referenceTableName + " and the child view " + tableName + ".\r\nAdditional information:\n " + exception.Message, exception);
            }
        }

        public override string GetUpdateNewNotNestedParentStatement(string tableName, string columnName, string newTableName, string fieldName, string relatedViewPkName, string relatedViewDisplayName)
        {
            return string.Format("SET SQL_SAFE_UPDATES=0;" + "update " + sqlTextBuilder.EscapeDbObject("{0}") + " join " + sqlTextBuilder.EscapeDbObject("{2}") + " on " + sqlTextBuilder.EscapeDbObject("{0}") + sqlTextBuilder.DbTableColumnSeperator + sqlTextBuilder.EscapeDbObject("{3}") + " = " + sqlTextBuilder.EscapeDbObject("{2}") + sqlTextBuilder.DbTableColumnSeperator + sqlTextBuilder.EscapeDbObject("{5}") + " set " + sqlTextBuilder.EscapeDbObject("{0}") + sqlTextBuilder.DbTableColumnSeperator + sqlTextBuilder.EscapeDbObject("{1}") + " = " + sqlTextBuilder.EscapeDbObject("{2}") + sqlTextBuilder.DbTableColumnSeperator + sqlTextBuilder.EscapeDbObject("{4}"), tableName, columnName, newTableName, fieldName, relatedViewPkName, relatedViewDisplayName);
            
        }

        protected override SqlSchema GetNewSqlSchema()
        {
            return new MySqlSchema();
        }

        protected override ISqlTextBuilder GetSqlTextBuilder()
        {
            return new MySqlTextBuilder();
        }

        public override void StopIdentityInsert(string tableName, IDbCommand command)
        {
        }

        public override void ContinueIdentityInsert(string tableName, IDbCommand command)
        { 
        }

        public override string GetFormula(string calculatedField, string tableName)
        {
            return "SELECT " + calculatedField + " FROM " + sqlTextBuilder.EscapeDbObject(tableName) + " LIMIT 0,1";
        }

        public override string GetTableEntity()
        {
            return "BASE TABLE";
        }

        public override string GetServerName(string connectionString)
        {
            MySqlConnectionStringBuilder builder = new MySqlConnectionStringBuilder(connectionString);
            return builder.Server;
        }
        public override string GetPort(string connectionString)
        {
            MySqlConnectionStringBuilder builder = new MySqlConnectionStringBuilder(connectionString);
            return builder.Port.ToString();
        }
        public override string GetSelectForCustomeViewTable()
        {
            return "SELECT  `CustomView` FROM `durados_CustomViews` WHERE `UserId` = @UserId AND `ViewName` = @ViewName LIMIT 0,1";
        }
        public override IDataParameter GetNewParameter()
        {
            return new MySqlParameter();
        }
        public override IDataParameter GetNewParameter(string name, object value)
        {
            return new MySqlParameter(name, value);
        }
      
    }

    public class MySqlCopyPaste : CopyPaste
    {
        public MySqlCopyPaste(View view)
            : base(view)
        {
        }

        protected override IDbConnection GetNewConnection(string connectionString)
        {
            return new MySqlConnection(connectionString);
        }

        protected override IDbCommand GetNewCommand(string cmdText, IDbConnection connection)
        {
            return new MySqlCommand(cmdText, (MySqlConnection)connection);
        }

        protected override SqlAccess GetSqlAccess()
        {
            return new MySqlAccess();
        }
    }

    public class MySqlImporter : Importer
    {
        public MySqlImporter(string ConnectionString, string sysConnectionString, bool doRollbackOnError)
            : base(ConnectionString, sysConnectionString, doRollbackOnError)
        {
        }

        protected override IDbConnection GetNewConnection(string connectionString)
        {
            return new MySqlConnection(connectionString);
        }

        protected override IDbCommand GetNewCommand(string cmdText, IDbConnection connection)
        {
            return new MySqlCommand(cmdText, (MySqlConnection)connection);
        }

        protected override SqlAccess GetNewSqlAccess()
        {
            return new MySqlAccess();
        }
    }

    public class MySqlHistory : History
    {
        protected override System.Data.IDataParameter GetNewParameter(IDbCommand command, string parameterName, object value)
        {
            return new MySqlParameter(parameterName, value);
        }

        protected override ISqlTextBuilder GetSqlTextBuilder()
        {
            return new MySqlTextBuilder();
        }

        protected override IDbConnection GetConnection(string connectionString)
        {
            return new MySqlConnection(connectionString);
        }

        protected override IDbCommand GetCommand()
        {
            return new MySqlCommand();
        }
        protected override string GetHistoryOldValueSelect()
        {
            return "SELECT  OldValueKey " +
                                        "FROM      durados_ChangeHistory INNER JOIN " +
                                        "durados_ChangeHistoryField ON durados_ChangeHistory.id = durados_ChangeHistoryField.ChangeHistoryId " +
                                        "WHERE  (ViewName = @ViewName) AND (PK = @PK) AND (FieldName = @FieldName) AND (NewValueKey = @NewValue) LIMIT 0,1 " +
                                        "ORDER BY UpdateDate DESC";
        }
    }
}
