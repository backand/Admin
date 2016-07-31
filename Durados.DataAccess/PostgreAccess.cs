using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using Npgsql;

namespace Durados.DataAccess
{
    public class PostgreAccess : MySqlAccess
    {
        public static bool IsPostgreConnectionString(string connectionString)
        {
            return connectionString.Contains("Encoding=");
        }
        public static string GetConnectionStringSchema(bool useSSL)
        {
            return (useSSL ? "server={0};database={1};User Id={2};password={3};SSL=true;SslMode=Require;" : "server={0};database={1};User Id={2};password={3}") + ";port={4};Encoding=UNICODE;CommandTimeout=90;Timeout=60;";
        }
        protected override System.Data.IDbConnection GetNewConnection(string connectionString)
        {
            if (IsPostgreConnectionString(connectionString))
            {
                NpgsqlConnection connection = new NpgsqlConnection(connectionString);// + ";port=1234");
                connection.ValidateRemoteCertificateCallback += new ValidateRemoteCertificateCallback(connection_ValidateRemoteCertificateCallback);
                return connection;
            }
            else
                return base.GetNewConnection(connectionString);
        }

        bool connection_ValidateRemoteCertificateCallback(System.Security.Cryptography.X509Certificates.X509Certificate cert, System.Security.Cryptography.X509Certificates.X509Chain chain, System.Net.Security.SslPolicyErrors errors)
        {
            return true;
        }

        protected override System.Data.IDbCommand GetNewCommand(string cmdText, System.Data.IDbConnection connection)
        {
            if (connection is NpgsqlConnection)
                return new NpgsqlCommand(cmdText, (NpgsqlConnection)connection);
            else
                return base.GetNewCommand(cmdText, connection);
        }

        protected override System.Data.IDataParameter GetNewParameter(System.Data.IDbCommand command, string parameterName, object value)
        {
            if (command is NpgsqlCommand)
                return new NpgsqlParameter(parameterName, value);
            else
                return base.GetNewParameter(command, parameterName, value);
        }

        protected override System.Data.IDataParameter GetNewParameter(View view, string parameterName, object value)
        {
            if (IsPostgreConnectionString(view.ConnectionString))
                return new NpgsqlParameter(parameterName, value);
            else
                return base.GetNewParameter(view, parameterName, value);
        }

        protected override System.Data.IDataAdapter GetNewAdapter(System.Data.IDbCommand command)
        {
            if (command is NpgsqlCommand)
                return new NpgsqlDataAdapter((NpgsqlCommand)command);
            else
                return base.GetNewAdapter(command);
        }

        protected override ISqlTextBuilder GetSqlTextBuilder(View view)
        {
            if (view == null || string.IsNullOrEmpty(view.ConnectionString) || IsPostgreConnectionString(view.ConnectionString))
                return new PostgreTextBuilder();
            else
                return base.GetSqlTextBuilder(view);
        }

        //protected override int Fill(IDataAdapter adapter, DataTable table)
        //{
        //    if (adapter is NpgsqlDataAdapter)
        //        return ((NpgsqlDataAdapter)adapter).Fill(table);
        //    else
        //        return base.Fill(adapter, table);
        //}


        protected override int Fill(IDataAdapter adapter, DataTable table)
        {
            //if (adapter is NpgsqlDataAdapter)
            //{
            //    NpgsqlDataAdapter npgsqlDataAdapter = (NpgsqlDataAdapter)adapter;

            //    using (IDbConnection connection = npgsqlDataAdapter.SelectCommand.Connection)
            //    {
            //        if (connection.State != ConnectionState.Open)
            //            connection.Open();

            //        using (IDataReader reader = npgsqlDataAdapter.SelectCommand.ExecuteReader())
            //        {
            //            table.Load(reader, LoadOption.OverwriteChanges);
            //            reader.Close();
            //        }

            //        connection.Close();
            //    }


            //    int r = table.Rows.Count;

            //    return r;
            //}
            if (adapter is NpgsqlDataAdapter)
                return ((NpgsqlDataAdapter)adapter).Fill(table);
            else
                return base.Fill(adapter, table);
        }

        //public override int RowCount(View view)
        //{
        //    return RowFilterCount(view, null);
        //}

        protected override string GetParameterType(IDataParameter parameter)
        {
            if (parameter is NpgsqlParameter)
                return ((NpgsqlParameter)parameter).DbType.ToString();
            else
                return base.GetParameterType(parameter);
        }

        public override  string GetDbTypeofDataType(DataType dataType)
        {
            switch (dataType)
            {
                case DataType.Boolean:
                    return "BOOLEAN";

                case DataType.DateTime:
                    return "TIMESTAMP";

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
            return formula.Replace('[', '\"').Replace(']', '\"');
        }

        public override SqlSchema GetNewSqlSchema()
        {
            return new PostgreSchema();
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
            if (!(exception is NpgsqlException))
                return false;

            return true;
        }
    }


    public class PostgreTextBuilder : MySqlTextBuilder
    {
       
        
        public override string EscapeDbObjectStart
        {
            get { return "\""; }
        }

        public override string EscapeDbObjectEnd
        {
            get { return "\""; }
        }

        public override string GetPage(int page, int rowsCount)
        {
            return " LIMIT " + rowsCount + " offset " + ((page - 1) * rowsCount);
        }

        public override string GetLastInsertedRow(View view)
        {
            string tableName = string.IsNullOrEmpty(view.EditableTableName) ? view.DataTable.TableName : view.EditableTableName;
            string pkFieldName = GetPrimaryKeyColumn(view);
            
            return GetLastInsertedRow(tableName, pkFieldName);
        }
        private  string GetLastInsertedRow(string tableName,string pkFieldName)
        {
            
            return string.Format(" SELECT CURRVAL(pg_get_serial_sequence('{0}','{1}'))", tableName, pkFieldName);
          // return string.Format("SELECT CURRVAL(pg_get_serial_sequence({0}','{1}'));",tableName,);
        }

        private string GetPrimaryKeyColumn(View view)
        {
            return view.DataTable.PrimaryKey[0].ColumnName;
            
        }
    }

    public class PostgreSchema : MySqlSchema
    {
        public override string GetDataType(DataType dataType)
        {
            switch (dataType)
            {
                case DataType.Boolean:
                    return "BOOLEAN  NULL";

                case DataType.DateTime:
                    return "TIMESTAMP   NULL";

                case DataType.LongText:
                    return "VARCHAR(4000)  NULL";

                case DataType.Numeric:
                    return "FLOAT  NULL";

                case DataType.ImageList:
                case DataType.SingleSelect:
                    return "INT  NULL";

                case DataType.Image:
                case DataType.Email:
                case DataType.Url:
                case DataType.Html:

                case DataType.ShortText:
                    return "VARCHAR(250)  NULL";

                default:
                    throw new DuradosException("Ilegal data type " + dataType.ToString());

            }
        }

        private bool IsPostgreConnectionString(string connectionString)
        {
            return connectionString.Contains("Encoding=");
        }

        public override IDbConnection GetConnection(string connectionString)
        {
            if (IsPostgreConnectionString(connectionString))
            {
                NpgsqlConnection connection = new NpgsqlConnection(connectionString);
                connection.ValidateRemoteCertificateCallback += new ValidateRemoteCertificateCallback(connection_ValidateRemoteCertificateCallback);
                return connection;
            }
            else
                return base.GetConnection(connectionString);
        }

        bool connection_ValidateRemoteCertificateCallback(System.Security.Cryptography.X509Certificates.X509Certificate cert, System.Security.Cryptography.X509Certificates.X509Chain chain, System.Net.Security.SslPolicyErrors errors)
        {
            return true;
        }

        public override IDbCommand GetCommand(string commandText, IDbConnection connection)
        {
            if (connection is NpgsqlConnection)
                return new NpgsqlCommand(commandText, (NpgsqlConnection)connection);
            else
                return base.GetCommand(commandText, connection);
        }


        protected override SqlSchema GetNewSqlSchema()
        {
            return new PostgreSchema();
        }

        protected override ISqlTextBuilder GetSqlTextBuilder()
        {
            return new PostgreTextBuilder();
        }

        public override System.Data.IDbCommand GetCommand()
        {
            return new DuradosCommand(SqlProduct.Postgre);
        }

        public override string GetDefaultSchemaSelectStatement()
        {
            return "select current_database() as default_schema";
        }

        public override string IsTableOrViewExistsSelectStatement(string tableName)
        {
            return "select table_name as \"Name\", table_schema as \"Schema\", table_type as EntityType from information_schema.tables where table_catalog = current_DATABASE() and table_schema = 'public' and table_name = '" + tableName + "'";

        }

        public override string GetPrimaryKeyColumns(string tableName)
        {
            return "SELECT kcu.column_name FROM    INFORMATION_SCHEMA.TABLES t LEFT JOIN INFORMATION_SCHEMA.TABLE_CONSTRAINTS tc ON tc.table_catalog = t.table_catalog AND tc.table_schema = t.table_schema AND tc.table_name = t.table_name AND tc.constraint_type = 'PRIMARY KEY' LEFT JOIN INFORMATION_SCHEMA.KEY_COLUMN_USAGE kcu ON kcu.table_catalog = tc.table_catalog AND kcu.table_schema = tc.table_schema AND kcu.table_name = tc.table_name AND kcu.constraint_name = tc.constraint_name  where t.table_catalog = current_DATABASE() and t.table_schema = 'public' and t.table_name = '" + tableName + "'";
        }

        public override string GetColumnsSelectStatement(string tableName)
        {
            return "select * from information_schema.columns where table_catalog = current_DATABASE() and table_schema = 'public' and table_name = '" + tableName + "'";

        }

        public override string GetAutoIdentityColumns(string tableName)
        {
            return "select COLUMN_NAME, TABLE_NAME from information_schema.columns where table_catalog = current_DATABASE() and table_schema = 'public' and position('nextval' in column_default) > 0 and table_name = '" + tableName + "'";
        }

        public override string GetEntitiesSelectStatement()
        {
            return "select table_name as Name, table_schema as \"Schema\", table_type as EntityType from information_schema.tables where table_catalog = current_DATABASE() and table_schema = 'public' ";
        }

        public override string GetTableNamesSelectStatement()
        {
            return "select table_name as Name, table_schema as \"Schema\", table_type as EntityType from information_schema.tables where table_catalog = current_DATABASE() and table_schema = 'public' and table_type = 'BASE TABLE'";
        }

        protected override string GetMyForeignKeyConstraintsSql(string tableName)
        {
            return "SELECT tc.table_name as TableName, kcu.column_name as ColumnName, ccu.table_name AS ReferenceTableName, ccu.column_name AS ReferenceColumnName FROM information_schema.table_constraints AS tc JOIN information_schema.key_column_usage AS kcu ON tc.constraint_name = kcu.constraint_name JOIN information_schema.constraint_column_usage AS ccu ON ccu.constraint_name = tc.constraint_name WHERE constraint_type = 'FOREIGN KEY' AND tc.table_name='" + tableName + "'";
        }

        public override string GetForeignKeyConstraints()
        {
            return "SELECT tc.constraint_name as name, tc.table_name as TableName, kcu.column_name as ColumnName, ccu.table_name AS ReferenceTableName, ccu.column_name AS ReferenceColumnName FROM information_schema.table_constraints AS tc JOIN information_schema.key_column_usage AS kcu ON tc.constraint_name = kcu.constraint_name JOIN information_schema.constraint_column_usage AS ccu ON ccu.constraint_name = tc.constraint_name WHERE constraint_type = 'FOREIGN KEY' ";
        }

        public override string GetForeignKeyConstraintsToMe(string tableName)
        {
            return "SELECT tc.table_name as TableName, kcu.column_name as ColumnName, ccu.table_name AS ReferenceTableName, ccu.column_name AS ReferenceColumnName FROM information_schema.table_constraints AS tc JOIN information_schema.key_column_usage AS kcu ON tc.constraint_name = kcu.constraint_name JOIN information_schema.constraint_column_usage AS ccu ON ccu.constraint_name = tc.constraint_name WHERE constraint_type = 'FOREIGN KEY' AND ccu.table_name='" + tableName + "'";
        }

        public virtual string GetTableRowsCount(string tableName)
        {
            return "SELECT n_live_tup   FROM pg_stat_user_tables  where schemaname = current_schema and relname='" + tableName + "' ";
            //return "SELECT rows FROM sys.sysindexes  AS s1 WHERE (rows > 0) AND (indid IN (SELECT MIN(indid) AS Expr1 FROM sys.sysindexes AS s2 WHERE (s1.id = id))) AND (id = OBJECT_ID('" + tableName + "'))";
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
            sb.AppendFormat(" CONSTRAINT "+sqlTextBuilder.EscapeDbObject("PK_{0}")+"  PRIMARY KEY (" + sqlTextBuilder.EscapeDbObject("{1}") + "),CONSTRAINT " +sqlTextBuilder.EscapeDbObject("{0}_{1}_UNIQUE")+ " UNIQUE " + " (" + sqlTextBuilder.EscapeDbObject("{1}") + ") )  ", table.TableName, table.PrimaryKey[0].ColumnName);
            //WITHOUT OIDS;ALTER TABLE g2_{table_name} OWNER TO gallery2;

            return sb.ToString();
        }

        protected override string GetCreateColumnScript(DataColumn column)
        {
            string script = sqlTextBuilder.EscapeDbObject("{0}") + " {1} {2} ";

            string name = column.ColumnName;
            string type = GetColumnTypeScript(column);
            string nullable = column.AutoIncrement ? "NOT NULL" : "NULL";
        
            return string.Format(script, name, type, nullable);
        }

        protected override string GetColumnTypeScript(DataColumn column)
        {
            if (column.AutoIncrement)
                return "SERIAL ";
            else if (column.DataType == typeof(int) || column.DataType == typeof(Int64))
                return "int4";
            else if (column.DataType == typeof(DateTime))
                return "TIMESTAMP";
            else if (column.DataType == typeof(double))
                return "REAL";
            else if (column.DataType == typeof(bool))
                return "BOOLEAN";
            else
            {
                return string.Format("varchar({0})", column.MaxLength);
            }
        }

        public override string GetColumnDataTypeSelectStatement(string tableName, string columnName)
        {
            return "select concat(DATA_TYPE , case when CHARACTER_MAXIMUM_LENGTH is not null then concat('(' , CHARACTER_MAXIMUM_LENGTH::text , ')') else '' end) as dt from INFORMATION_SCHEMA.COLUMNS IC where TABLE_NAME ='"+tableName+"' and COLUMN_NAME = '"+columnName+"'";
            // return "select CASE WHEN CHARACTER_MAXIMUM_LENGTH IS NULL then  data_type else concat('(',CHARACTER_MAXIMUM_LENGTH, ')') end as dt from INFORMATION_SCHEMA.COLUMNS IC where TABLE_NAME = 'qaz2' and COLUMN_NAME = 'Id'
           // return "select IF(`CHARACTER_MAXIMUM_LENGTH` IS NULL, `data_type`, concat(data_type, concat('(',`CHARACTER_MAXIMUM_LENGTH`, ')'))) as dt from INFORMATION_SCHEMA.COLUMNS IC where TABLE_NAME = '" + tableName + "' and COLUMN_NAME = '" + columnName + "'";

        }

        protected override  string GetCreateFkConstraintStatement(string fkTableName, string fkColumnName, string pkTableName, string pkColumnName)
        {
            return "ALTER TABLE " + sqlTextBuilder.EscapeDbObject(fkTableName) + "  ADD  CONSTRAINT " + sqlTextBuilder.EscapeDbObject("FK_" + fkTableName + "_" + pkTableName + "_" + fkColumnName) + " FOREIGN KEY(" + sqlTextBuilder.EscapeDbObject(fkColumnName) + ") REFERENCES " + sqlTextBuilder.EscapeDbObject(pkTableName) + " (" + sqlTextBuilder.EscapeDbObject(pkColumnName) + ")  MATCH FULL;";
        }

        public override void CreateFkConstraint(string fkTableName, string fkColumnName, string pkTableName, string pkColumnName, IDbCommand command)
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

        public override string GetUpdateNewParentStatement(string newTableName, string relatedViewPkName, string relatedViewOrdinal)
        {
            return string.Format("update " + sqlTextBuilder.EscapeDbObject("{0}") + " set " + sqlTextBuilder.EscapeDbObject("{2}") + " = " + sqlTextBuilder.EscapeDbObject("{1}") + " where " + sqlTextBuilder.EscapeDbObject("{2}") + " is null", newTableName, relatedViewPkName, relatedViewOrdinal);

        }

        public override  string GetUpdateNewNotNestedParentStatement(string tableName, string columnName, string newTableName, string fieldName, string relatedViewPkName, string relatedViewDisplayName)
        {
            return string.Format("update " + sqlTextBuilder.EscapeDbObject("{0}") + " set " +sqlTextBuilder.EscapeDbObject("{1}") + " = " + sqlTextBuilder.EscapeDbObject("{2}") + sqlTextBuilder.DbTableColumnSeperator + sqlTextBuilder.EscapeDbObject("{4}") + " from " + sqlTextBuilder.EscapeDbObject("{2}") + " WHERE " + sqlTextBuilder.EscapeDbObject("{0}") + sqlTextBuilder.DbTableColumnSeperator + sqlTextBuilder.EscapeDbObject("{3}") + " = " + sqlTextBuilder.EscapeDbObject("{2}") + sqlTextBuilder.DbTableColumnSeperator + sqlTextBuilder.EscapeDbObject("{5}"), tableName, columnName, newTableName, fieldName, relatedViewPkName, relatedViewDisplayName);

        }

        public override string GetFormula(string calculatedField, string tableName)
        {
            return "SELECT " + calculatedField + " FROM " + sqlTextBuilder.EscapeDbObject(tableName) + " LIMIT 1 OFFSET 0";
        }
       
    }

    public class PostgreCopyPaste : MySqlCopyPaste
    {
        public PostgreCopyPaste(View view)
            : base(view)
        {
        }

        protected override IDbConnection GetNewConnection(string connectionString)
        {
            
            NpgsqlConnection connection = new NpgsqlConnection(connectionString);// + ";port=1234");
            connection.ValidateRemoteCertificateCallback += new ValidateRemoteCertificateCallback(connection_ValidateRemoteCertificateCallback);
            return connection;
            
        }

        bool connection_ValidateRemoteCertificateCallback(System.Security.Cryptography.X509Certificates.X509Certificate cert, System.Security.Cryptography.X509Certificates.X509Chain chain, System.Net.Security.SslPolicyErrors errors)
        {
            return true;
        }

        protected override IDbCommand GetNewCommand(string cmdText, IDbConnection connection)
        {
            return new NpgsqlCommand(cmdText, (NpgsqlConnection)connection);
        }

        protected override SqlAccess GetSqlAccess()
        {
            return new PostgreAccess();
        }
    }

    public class PostrgreImporter : MySqlImporter
    {
        public PostrgreImporter(string ConnectionString, string sysConnectionString, bool doRollbackOnError)
            : base(ConnectionString, sysConnectionString, doRollbackOnError)
        {
        }

        protected override IDbConnection GetNewConnection(string connectionString)
        {
            NpgsqlConnection connection = new NpgsqlConnection(connectionString);
            connection.ValidateRemoteCertificateCallback += new ValidateRemoteCertificateCallback(connection_ValidateRemoteCertificateCallback);
            return connection;
        }

        bool connection_ValidateRemoteCertificateCallback(System.Security.Cryptography.X509Certificates.X509Certificate cert, System.Security.Cryptography.X509Certificates.X509Chain chain, System.Net.Security.SslPolicyErrors errors)
        {
            return true;
        }

        protected override IDbCommand GetNewCommand(string cmdText, IDbConnection connection)
        {
            return new NpgsqlCommand(cmdText, (NpgsqlConnection)connection);
        }

        protected override SqlAccess GetNewSqlAccess()
        {
            return new PostgreAccess();
        }
    }
}
