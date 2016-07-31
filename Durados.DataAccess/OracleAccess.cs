using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using Oracle.ManagedDataAccess.Client;


namespace Durados.DataAccess
{
    public class OracleAccess : MySqlAccess
    {
        public static bool IsOracleConnectionString(string connectionString)
        {
            return connectionString.StartsWith("Data Source=(");
        }

        protected override System.Data.IDbConnection GetNewConnection(string connectionString)
        {
            if (IsOracleConnectionString(connectionString))
            {
                OracleConnection connection = new OracleConnection(connectionString);// + ";port=1234");
                
                return connection;
            }
            else
                return base.GetNewConnection(connectionString);
        }

        protected override System.Data.IDbCommand GetNewCommand(string cmdText, System.Data.IDbConnection connection)
        {
            if (connection is OracleConnection)
                return new OracleCommand(cmdText, (OracleConnection)connection);
            else
                return base.GetNewCommand(cmdText, connection);
        }

        protected override System.Data.IDataParameter GetNewParameter(System.Data.IDbCommand command, string parameterName, object value)
        {
            if (command is OracleCommand)
                return new OracleParameter(parameterName, value);
            else
                return base.GetNewParameter(command, parameterName, value);
        }

        protected override System.Data.IDataParameter GetNewParameter(View view, string parameterName, object value)
        {
            if (IsOracleConnectionString(view.ConnectionString))
                return new OracleParameter(new OracleTextBuilder().DbParameterPrefix+ parameterName, value);
            else
                return base.GetNewParameter(view, parameterName, value);
        }

        protected override System.Data.IDataAdapter GetNewAdapter(System.Data.IDbCommand command)
        {
            if (command is OracleCommand)
                return new OracleDataAdapter((OracleCommand)command);
            else
                return base.GetNewAdapter(command);
        }

        protected override ISqlTextBuilder GetSqlTextBuilder(View view)
        {
            if (view == null || string.IsNullOrEmpty(view.ConnectionString) || IsOracleConnectionString(view.ConnectionString))
                return new OracleTextBuilder();
            else
                return base.GetSqlTextBuilder(view);
        }

        protected override int Fill(IDataAdapter adapter, DataTable table)
        {
            if (adapter is OracleDataAdapter)
                return ((OracleDataAdapter)adapter).Fill(table);
            else
                return base.Fill(adapter, table);
        }

        //public override int RowCount(View view)
        //{
        //    return RowFilterCount(view, null);
        //}

        protected override string GetParameterType(IDataParameter parameter)
        {
            if (parameter is OracleParameter)
                return ((OracleParameter)parameter).DbType.ToString();
            else
                return base.GetParameterType(parameter);
        }

        public override string GetDbTypeofDataType(DataType dataType)
        {
            switch (dataType)
            {
                case DataType.Boolean:
                    return "RAW(1)";

                case DataType.DateTime:
                    return "DATE";

                case DataType.LongText:
                    return "CHAR(2000)";

                case DataType.Numeric:
                    return "FLOAT (24)";

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
            return formula.Replace('[', '"').Replace(']', '"');
        }

        public override SqlSchema GetNewSqlSchema()
        {
            return new OracleSchema();
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

        public static string GetConnectionStringSchema()
        {
            return "Data Source=(DESCRIPTION=(ADDRESS=(PROTOCOL=TCP)(HOST={0})(PORT={4}))(CONNECT_DATA=(SERVICE_NAME={1})));User ID={2};Password={3};";
        }

        public override bool IsLoginFailureException(Exception exception)
        {
            if (!(exception is OracleException))
                return false;

            return true;
        }
    }


    public class OracleTextBuilder : MySqlTextBuilder
    {
        //public override string GetAlterColumn(string columnName)
        //{
        //    return " CHANGE COLUMN " + columnName + " ";
        //}

        public override string EscapeDbObjectStart
        {
            get { return "\""; }
        }

        public override string EscapeDbObjectEnd
        {
            get { return "\""; }
        }

        //public override string GetRowNumber(string orderByColumn)
        //{
        //    return string.Empty; 
        //}

        //public override string GetRowNumber(string orderByTable, string orderByColumn, string sortOrder)
        //{
        //    return string.Empty;
        //}

        public override string GetPageOrder(string orderByColumn)
        {
            //if (string.IsNullOrEmpty(orderByColumn) || orderByColumn.Trim().Equals(string.Empty))
                return string.Empty;
           // return " order by " + orderByColumn + " ";
        }

        public override string GetPage(int page, int rowsCount)
        {
            return " AND ROWNUM>= " + ((page - 1) * rowsCount) + " AND ROWNUM<= " + rowsCount + " ";
        }

        //public override string WithNolock
        //{
        //    get { return string.Empty; }
        //}

        public override string GetLastInsertedRow(string tableName)
        {
            return string.Empty;// "SELECT SEQUENCE_NAME.CURRVAL  ID";
        }
        //public override string GetLastInsertedRow2(string tableName)
        //{
        //    return string.Empty;// "SELECT SEQUENCE_NAME.CURRVAL  ID";
        //}
        public override string GetLastInsertedRow(View view)
        {
            return string.Empty;
        }
        public override string GetLastInsertedRow2(View view)
        {
            if (view.PrimaryKeyFileds.Count() > 0 && !string.IsNullOrEmpty(view.PrimaryKeyFileds[0].AutoIncrementSequanceName))
                return string.Format("SELECT {0}.CURRVAL AS ID FROM DUAL", view.PrimaryKeyFileds[0].AutoIncrementSequanceName);
            else return null;
        }
        

        //public override string GetDateDiffDays(string start, string end)
        //{
        //    return "DATEDIFF(" + end + "," + start + ")";
        //}

        //public override string GetDateAddDays(string days, string date)
        //{
        //    return "DATE_ADD(" + date + ", INTERVAL " + days + " DAY)";
        //}

        public override string GetDateOnly(string date)
        {
            return "trunc(" + date + ")";
        }

        public override string Top(string sql, int limit)
        {
            
            return string.Format("select * from ({0}) where ROWNUM <={1}", sql, limit);
        }

        public override string DbAs { get { return " "; } }

        public override string DbParameterPrefix { get { return ":"; } }

        public override string DbEndStatement
        {
            get
            {
                return string.Empty;// return " END";
            }
        }

         public override string DbBeginStatement
        {
            get
            {
                return string.Empty;// return "BEGIN ";
            }
        }
         public override string DbEndOfStatement
         {
             get { return string.Empty; }

         }
    }

    public class OracleSchema : MySqlSchema
    {
        public override string GetDataType(DataType dataType)
        {
            switch (dataType)
            {
                case DataType.Boolean:
                    return "RAW(1)  NULL";

                case DataType.DateTime:
                    return "DATE  NULL";

                case DataType.LongText:
                    return "VARCHAR2(4000)  NULL";

                case DataType.Numeric:
                    return "FLOAT  NULL";

                case DataType.ImageList:
                case DataType.SingleSelect:
                    return "NUMBER(10, 0)  NULL";

                case DataType.Image:
                case DataType.Email:
                case DataType.Url:
                case DataType.Html:

                case DataType.ShortText:
                    return "VARCHAR2(250)  NULL";

                default:
                    throw new DuradosException("Ilegal data type " + dataType.ToString());

            }
        }

        private bool IsOracleConnectionString(string connectionString)
        {
            return connectionString.StartsWith("Data Source=(");
        }

        //public override string GetTableRowsCount(string tableName)
        //{
        //    return "SELECT TABLE_ROWS as rows FROM information_schema.tables WHERE TABLE_NAME = '" + tableName + "'";
        //}

        //public override string GetPrimaryIndexName(string tableName)
        //{
        //    return "select index_name from information_schema.statistics where table_schema = DATABASE() and table_name='" + tableName + "' and non_unique = 0";
        //}

        //public override string GetTableRowsCount(string tableName, string indexName)
        //{
        //    return "select count(*) from " + tableName + " use index(" + indexName + ")";
        //}

        public override string GetEntitiesSelectStatement()
        {
            return @"select OBJECT_Name as Name,Owner as Schema, OBJECT_TYPE as EntityType from all_objects where object_type in ('TABLE','VIEW') and Owner = (select  sys_context( 'userenv', 'current_schema' ) from dual)";
        }

        public override string GetTableNamesSelectStatement()
        {
            //return "select table_name as Name, table_schema as `Schema`, table_type as EntityType from information_schema.tables where table_schema = DATABASE() and table_type = 'BASE TABLE'";
            return "select TABLE_NAME Name, OWNER as Schema, 'TABLE' as EntityType  from SYS.ALL_TABLES where OWNER = (select sys_context( 'userenv', 'current_schema' ) from dual)";
        }

        public override System.Data.IDbCommand GetCommand()
        {
            return new DuradosCommand(SqlProduct.Oracle);
        }

        public override IDbConnection GetConnection(string connectionString)
        {
            if (IsOracleConnectionString(connectionString))
            {
                OracleConnection connection = new OracleConnection();
                connection.ConnectionString = connectionString;
                return connection;
            }
            else
                return base.GetConnection(connectionString);
        }

        public override IDbCommand GetCommand(string commandText, IDbConnection connection)
        {
            if (connection is OracleConnection)
            {
                OracleCommandBuilder builder = new OracleCommandBuilder();

                OracleCommand oracleCommand = new OracleCommand(commandText, (OracleConnection)connection);
                oracleCommand.BindByName = true;

                return oracleCommand;
            }
            else
                return base.GetCommand(commandText, connection);
        }

        public override string GetDefaultSchemaSelectStatement()
        {
            return "select sys_context( 'userenv', 'current_schema' )as default_schema from dual";
        }

        //protected override string GetCreatedNewId(string tableName)
        //{
        //}
        public override string IsTableOrViewExistsSelectStatement(string tableName)
        {

            return "select OBJECT_NAME as Name,OWNER as Schema, OBJECT_TYPE as EntityType from all_objects where object_type in ('TABLE','VIEW') and Owner = (select  sys_context( 'userenv', 'current_schema' ) from dual) and OBJECT_NAME = '" + tableName + "'";

        }

        public override string IsViewExistsSelectStatement(string viewName)
        {
            
            return "select OBJECT_NAME as Name,OWNER as Schema, OBJECT_TYPE as EntityType from all_objects where object_type in ('VIEW') and Owner = (select  sys_context( 'userenv', 'current_schema' ) from dual) and OBJECT_NAME = '" + viewName + "'";
        }

        public override string GetColumnsSelectStatement(string tableName)
        {
            return "SELECT   column_name, data_type, (case nullable when  'Y' then 'YES' else 'NO' end) as is_nullable, CHAR_COL_DECL_LENGTH as character_maximum_length FROM USER_TAB_COLUMNS WHERE TABLE_NAME = '" + tableName + "'";

        }

        public override string GetAutoIdentityColumns(string tableName)
        {
            return string.Empty;// "select COLUMN_NAME, TABLE_NAME from all_tab_columns where  OWNER = (select  sys_context( 'userenv', 'current_schema' ) from dual) and  extra = 'auto_increment' and table_name = '" + tableName + "'";
        }

        public override string GetPrimaryKeyColumns(string tableName)
        {

            return "SELECT cols.table_name, cols.column_name, cols.position, cons.status, cons.owner  FROM all_constraints cons, all_cons_columns cols  WHERE cols.table_name = '" + tableName.ToUpper() + "'  AND cons.constraint_type = 'P'  AND cons.constraint_name = cols.constraint_name  AND cons.owner = cols.owner  ORDER BY cols.table_name, cols.position";
        }

        protected override string GetMyForeignKeyConstraintsSql(string tableName)
        {
            return @" SELECT a.table_name, a.column_name ColumnName, a.constraint_name, c.owner, c.r_owner, c_pk.table_name  ReferenceTableName, c_pk.constraint_name r_pk
                      FROM user_cons_columns a  JOIN user_constraints c ON a.owner = c.owner       AND a.constraint_name = c.constraint_name  JOIN user_constraints c_pk ON c.r_owner = c_pk.owner
                                               AND c.r_constraint_name = c_pk.constraint_name
                     WHERE c.constraint_type = 'R'   AND a.table_name ='" +tableName.ToUpper() +"'";
//          
        }

        public override string GetForeignKeyConstraint(string tableName, string referenceTableName, string columnName)
        {
            return "select constraint_name as TableName, column_name as ColumnName, referenced_table_name as ReferenceTableName, referenced_column_name as ReferencedColumnName from information_schema.key_column_usage where referenced_table_name is not null and table_schema = DATABASE() and table_name = '" + tableName + "' and referenced_table_name = '" + referenceTableName + "' and column_name ='" + columnName + "'";
        }

        public override string GetForeignKeyConstraintsToMe(string tableName)
        {
            return "select table_name as TableName, column_name as ColumnName, referenced_table_name as ReferenceTableName, referenced_column_name as ReferencedColumnName from information_schema.key_column_usage where referenced_table_name is not null and table_schema = DATABASE() and referenced_table_name = '" + tableName + "'";
        }

        public override string GetColumnDataTypeSelectStatement(string tableName, string columnName)
        {
            return "select  CASE WHEN \"CHAR_COL_DECL_LENGTH\" IS NULL THEN \"DATA_TYPE\" ELSE  concat( \"DATA_TYPE\" , concat(concat('(',\"CHAR_COL_DECL_LENGTH\"),')')) END as dt from user_tab_columns  ac where TABLE_NAME = \"" + tableName + "\" and COLUMN_NAME = \"" + columnName + "\"";
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
                return "NUMBER";
            else if (column.DataType == typeof(DateTime))
                return "DATE";
            else if (column.DataType == typeof(double))
                return "FLOAT";
            else if (column.DataType == typeof(bool))
                return "RAW";
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

        protected HashSet<string> FkEngines = new HashSet<string>() { "innodb" };
        public override void CreateFkConstraint(string fkTableName, string fkColumnName, string pkTableName, string pkColumnName, IDbCommand command)
        {
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

                sql = "ALTER TABLE " + sqlTextBuilder.EscapeDbObject(tableName) + " DROP FOREIGN KEY " + sqlTextBuilder.EscapeDbObject(fkConstraintName);

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
            return new OracleSchema();
        }

        protected override ISqlTextBuilder GetSqlTextBuilder()
        {
            return new OracleTextBuilder();
        }

        public override void StopIdentityInsert(string tableName, IDbCommand command)
        {
        }

        public override void ContinueIdentityInsert(string tableName, IDbCommand command)
        {
        }

        public override string GetFormula(string calculatedField, string tableName)
        {
            return "SELECT " + calculatedField + " FROM " + sqlTextBuilder.EscapeDbObject(tableName) + " WHERE ROWNUM=1";
        }

        public override string GetTableEntity()
        {
            return "BASE TABLE";
        }

        public override string GetIsColumnExistsSelectStatement(string tableName, string columnName)
        {
            return "SELECT column_name  FROM user_tab_columns WHERE table_name = N'" + tableName + "' AND column_name= N'" + columnName + "'";

        }
    }

    public class OracleCopyPaste : MySqlCopyPaste
    {
        public OracleCopyPaste(View view)
            : base(view)
        {
        }

        protected override IDbConnection GetNewConnection(string connectionString)
        {
            return new OracleConnection(connectionString);
        }

        protected override IDbCommand GetNewCommand(string cmdText, IDbConnection connection)
        {
            return new OracleCommand(cmdText, (OracleConnection)connection);
        }

        protected override SqlAccess GetSqlAccess()
        {
            return new OracleAccess();
        }
    }

    public class OracleImporter : MySqlImporter
    {
        public OracleImporter(string ConnectionString, string sysConnectionString, bool doRollbackOnError)
            : base(ConnectionString, sysConnectionString, doRollbackOnError)
        {
        }

        protected override IDbConnection GetNewConnection(string connectionString)
        {
            return new OracleConnection(connectionString);
        }

        protected override IDbCommand GetNewCommand(string cmdText, IDbConnection connection)
        {
            return new OracleCommand(cmdText, (OracleConnection)connection);
        }

        protected override SqlAccess GetNewSqlAccess()
        {
            return new OracleAccess();
        }
    }
}
