using System;
using System.IO;
using System.Data;
using System.Data.SqlClient;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Durados.DataAccess.AutoGeneration
{
    public class Generator
    {
        protected string connectionString;
        protected string schemaGeneratorFileName;

        public Generator()
        {
        }

        public Generator(string connectionString, string schemaGeneratorFileName)
        {
            this.connectionString = connectionString;
            this.schemaGeneratorFileName = schemaGeneratorFileName;
            BuildSchema(connectionString);
        }

        protected virtual void BuildSchema(string connectionString)
        {
            if (SchemaExists(connectionString))
                return;

            FileInfo file = new FileInfo(schemaGeneratorFileName);
            string scripts = file.OpenText().ReadToEnd();
            IDbConnection conn = GetNewSqlSchema().GetConnection(connectionString);
            scripts = scripts.Replace("__DB_NAME__", conn.Database);
            conn.Open();
            try
            {
                IDbCommand command = GetNewSqlSchema().GetCommand();

                command.Connection = conn;
                //command.CommandType = System.Data.CommandType.StoredProcedure;

                foreach (string script in scripts.Split(new string[1] { "\nGO\r" }, StringSplitOptions.RemoveEmptyEntries))
                {

                    command.CommandText = script;
                    command.ExecuteNonQuery();
                }
            }
            finally
            {
                conn.Close();
            }
        }

        protected virtual string RootObjectName
        {
            private set;
            get;
        }

        protected virtual bool SchemaExists(string connectionString)
        {
            string sql = (GetNewSqlSchema()).IsTableOrViewExistsSelectStatement(RootObjectName);
            IDbConnection conn = GetNewSqlSchema().GetConnection(connectionString);
            conn.Open();
            try
            {
                IDbCommand command = GetNewSqlSchema().GetCommand();

                command.Connection = conn;
                //command.CommandType = System.Data.CommandType.StoredProcedure;
                command.CommandText = sql;
                object scalar = command.ExecuteScalar();
                return scalar != null && scalar != DBNull.Value;
            }
            finally
            {
                conn.Close();
            }
        }


        public virtual void Clear()
        {
            using (IDbConnection connection = GetNewSqlSchema().GetConnection(connectionString))
            {
                connection.Open();
                IDbCommand command = GetNewSqlSchema().GetCommand("truncate table " + RootObjectName, connection);
                command.ExecuteNonQuery();
            }
        }

        public DataTable CreateTable(string tableName, string connectionString)
        {
            return CreateTable(tableName, tableName, connectionString);
        }

        public DataTable CreateTable(string viewName, string editableTableName, string connectionString)
        {
            using (IDbConnection connection = GetNewSqlSchema().GetConnection(connectionString))
            {
                using (IDbCommand command = GetNewSqlSchema().GetCommand())
                {
                    connection.Open();
                    //SqlTransaction transaction = connection.BeginTransaction(IsolationLevel.ReadCommitted);
                    command.Connection = connection;
                    //command.Transaction = transaction;

                    DataTable dataTable = CreateTable(viewName, editableTableName, command);
                    //transaction.Commit();

                    return dataTable;
                }
            }
        }

        //public DataTable CreateTable(string viewName, string editableTableName, IDbCommand command)
        //{
        //    if (command is DuradosCommand)
        //        return CreateTable(viewName, editableTableName, (SqlCommand)((DuradosCommand)command).Command);
        //    return CreateTable(viewName, editableTableName, (SqlCommand)command);
        //}

        public DataTable CreateTable(string tableName, IDbCommand command)
        {
            return CreateTable(tableName, tableName, command);
        }

        protected virtual SqlAccess GetNewSqlAccess()
        {
            return new SqlAccess();
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

        protected virtual SqlAccess GetNewSqlAccess(IDbCommand command)
        {
            if (command is System.Data.SqlClient.SqlCommand)
                return new SqlAccess();
            else
                return GetNewSqlAccess();
        }

        protected DataTable GetTableFromCommand(string viewName, IDbCommand command)
        {
            SqlSchema sqlSchema = GetNewSqlSchema(command);
            string sql = sqlSchema.GetSelectFirstRow(viewName);

            SqlAccess sqlAccess = GetNewSqlAccess(command);

            return sqlAccess.GetTableFromCommand(command, sql, null, CommandType.Text);
        }

        public DataTable CreateTable(string viewName, string editableTableName, IDbCommand command)
        {
            //if (command is DuradosCommand)
            //    return CreateTable(viewName, editableTableName, ((DuradosCommand)command).Command);

            DataTable tableFromCommand = null;

            try
            {
                tableFromCommand = GetTableFromCommand(viewName, command is DuradosCommand ? ((DuradosCommand)command).Command : command);
            }
            catch { }

            SqlSchema sqlSchema = GetNewSqlSchema(command);

            string sql = sqlSchema.GetColumnsSelectStatement(viewName);

            DataTable table = new DataTable(viewName);
            command.CommandText = sql;

            IDataReader reader = command.ExecuteReader();

            while (reader.Read())
            {
                string columnName = reader.GetString(reader.GetOrdinal("column_name"));
                if (columnName == "b1")
                {
                    int x = 0;
                    x++;
                }
                string dataType = reader.GetString(reader.GetOrdinal("data_type"));
                bool isNullable = reader.GetString(reader.GetOrdinal("is_nullable")).Equals("YES");
                string defaultValue = null;
                try
                {
                    if (!viewName.ToLower().Contains("durados"))
                    {
                        defaultValue = reader.IsDBNull(reader.GetOrdinal("column_default")) ? null : reader.GetString(reader.GetOrdinal("column_default"));
                    }
                }
                catch { }

                bool unique = false;
                try
                {
                    if (reader is MySql.Data.MySqlClient.MySqlDataReader)
                    {
                        int ord = reader.GetOrdinal("column_key");
                        if (ord >= 0)
                        {
                            unique = reader.GetString(ord).Equals("UNI");
                        }
                    }
                }
                catch { }

                DataColumn column = new DataColumn();
                column.ColumnName = columnName;
                Type type = GetType(dataType);
                column.Unique = unique;

                if (type == null && tableFromCommand != null)
                {
                    if (tableFromCommand.Columns.Contains(columnName))
                    {
                        type = tableFromCommand.Columns[columnName].DataType;
                    }
                }

                if (type != null)
                {
                    column.DataType = type;
                    column.AllowDBNull = isNullable;
                    try
                    {
                        if (defaultValue != null)
                        {
                            if (type == typeof(bool))
                            {
                                if (!string.IsNullOrEmpty(defaultValue))
                                {
                                    column.DefaultValue = !defaultValue.Equals("b'0'");
                                }
                            }
                            else if (type == typeof(DateTime))
                            {
                                column.DefaultValue = Convert.ChangeType(defaultValue, type);
                            }
                            else
                            {
                                column.DefaultValue = Convert.ChangeType(defaultValue, type);
                            }
                        }
                    }
                    catch { }
                    if (column.DataType == typeof(string))
                    {
                        int? maxLength = int.MaxValue;
                        if (!reader.IsDBNull(reader.GetOrdinal("character_maximum_length")))
                        {
                            try
                            {
                                long maxLength64 = Convert.ToInt64(reader.GetValue((reader.GetOrdinal("character_maximum_length"))));
                                if (maxLength64 < maxLength)
                                    maxLength = Convert.ToInt32(maxLength64);
                            }
                            catch
                            {
                                maxLength = 4000;
                            }
                        }

                        if (maxLength.Value == 100001)
                        {
                            maxLength = 100000;
                        }
                        if (dataType.ToLower() == "point")
                        {
                            maxLength = 100001;
                        }
                        column.MaxLength = maxLength.Value;
                    }

                    if (column.ExtendedProperties.ContainsKey("dataType"))
                    {
                        column.ExtendedProperties["dataType"] = dataType;
                    }
                    else
                    {
                        column.ExtendedProperties.Add("dataType", dataType);
                    }

                    table.Columns.Add(column);
                }
            }

            reader.Close();

            //sql = "select COLUMN_NAME, TABLE_NAME from INFORMATION_SCHEMA.COLUMNS where TABLE_SCHEMA = 'dbo' and COLUMNPROPERTY(object_id(TABLE_NAME), COLUMN_NAME, 'IsIdentity') = 1 and TABLE_NAME = N'" + editableTableName + "'";
            sql = sqlSchema.GetAutoIdentityColumns(editableTableName);
            if (!string.IsNullOrEmpty(sql))
            {
                command.CommandText = sql;
                reader = command.ExecuteReader();

                if (reader.Read())
                {
                    string columnName = reader.GetString(reader.GetOrdinal("COLUMN_NAME"));
                    if (table.Columns.Contains(columnName))
                    {
                        if (table.Columns[columnName].DefaultValue != null)
                            table.Columns[columnName].DefaultValue = null;
                        table.Columns[columnName].AutoIncrement = true;
                    }
                }

                reader.Close();
            }
            //sql = "SELECT Col.COLUMN_NAME FROM INFORMATION_SCHEMA.TABLE_CONSTRAINTS Tab, INFORMATION_SCHEMA.CONSTRAINT_COLUMN_USAGE Col WHERE Col.Constraint_Name = Tab.Constraint_Name AND Col.Table_Name = Tab.Table_Name AND Constraint_Type = 'PRIMARY KEY ' AND Col.Table_Name = N'" + editableTableName + "'";
            sql = sqlSchema.GetPrimaryKeyColumns(editableTableName);

            command.CommandText = sql;
            reader = command.ExecuteReader();
            List<DataColumn> pk = new List<DataColumn>();

            int ordinal = reader.GetOrdinal("COLUMN_NAME");
            while (reader.Read())
            {
                if (!reader.IsDBNull(ordinal))
                {
                    string columnName = reader.GetString(ordinal);
                    if (table.Columns.Contains(columnName))
                    {
                        pk.Add(table.Columns[columnName]);
                    }
                }
            }

            table.PrimaryKey = pk.ToArray();

            reader.Close();



            return table;

        }



        private Dictionary<string, string> GetDependentOriginalColumnNames()
        {
            return null;
        }

        public static Type GetType(string sqlDbType)
        {
            try
            {
                return GetType((SqlDbType)Enum.Parse(typeof(SqlDbType), sqlDbType.Replace(' ', '_'), true));
            }
            catch
            {

                return null;
            }
        }

        public static Type GetType(SqlDbType sqlDbType)
        {
            switch (sqlDbType)
            {
                case SqlDbType.Binary:
                    return typeof(byte[]);

                case SqlDbType.VarBinary:
                    return typeof(byte[]);

                case SqlDbType.Image:
                    return typeof(byte[]);
                
                case SqlDbType.Timestamp:
                    return typeof(DateTime);
                
                case SqlDbType.BigInt:
                    return typeof(long);

                case SqlDbType.boolean:
                case SqlDbType.Bit:
                    return typeof(bool);

                case SqlDbType.timestamp_without_time_zone:
                case SqlDbType.Date:
                    return typeof(DateTime);

                case SqlDbType.DateTime:
                    return typeof(DateTime);

                case SqlDbType.DateTime2:
                    return typeof(DateTime);

                //case SqlDbType.timestamp_with_time_zone:
                case SqlDbType.DateTimeOffset:
                    return typeof(DateTimeOffset);

                case SqlDbType.Decimal:
                    return typeof(decimal);

                case SqlDbType.double_precision:
                case SqlDbType.DOUBLE:
                case SqlDbType.Float:
                    return typeof(double);

                case SqlDbType.integer:
                case SqlDbType.MEDIUMINT:
                case SqlDbType.Int:
                    return typeof(int);

                case SqlDbType.Money:
                    return typeof(decimal);

                case SqlDbType.Numeric:
                    return typeof(decimal);

                case SqlDbType.Real:
                    return typeof(double);

                case SqlDbType.SmallDateTime:
                    return typeof(DateTime);

                case SqlDbType.SmallInt:
                    return typeof(Int16);

                case SqlDbType.SmallMoney:
                    return typeof(double);

                //case SqlDbType.Time:
                //    return typeof(DateTime);

                case SqlDbType.TinyInt:
                    return typeof(byte);

                case SqlDbType.UniqueIdentifier:
                    return typeof(Guid);

                //case SqlDbType.abstime:
                //case SqlDbType.time_without_time_zone:
                //case SqlDbType.time_with_time_zone:
                case SqlDbType.character_varying:
                case SqlDbType.Geography:
                case SqlDbType.Point:
                    return typeof(string);

                case SqlDbType.Year:
                    return typeof(Int64);



                case SqlDbType.TINYBLOB:
                case SqlDbType.LONGBLOB:
                case SqlDbType.MEDIUMBLOB:
                case SqlDbType.BLOB:
                    return typeof(byte[]);

                default:
                    return typeof(string);
            }
        }
    }







    public enum SqlDbType
    {
        BigInt = 0,
        Binary = 1,
        Bit = 2,
        Char = 3,
        DateTime = 4,
        Decimal = 5,
        Float = 6,
        Image = 7,
        Int = 8,
        Money = 9,
        NChar = 10,
        NText = 11,
        NVarChar = 12,
        Real = 13,
        UniqueIdentifier = 14,
        SmallDateTime = 15,
        SmallInt = 16,
        SmallMoney = 17,
        Text = 18,
        Timestamp = 19,
        TinyInt = 20,
        VarBinary = 21,
        VarChar = 22,
        Variant = 23,
        Xml = 25,
        Udt = 29,
        Structured = 30,
        Date = 31,
        Time = 32,
        DateTime2 = 33,
        DateTimeOffset = 34,
        Numeric = 35,
        Geography = 36,
        Year = 37,
        
        //MySQL - 1000
        BLOB = 1001,
        LONGBLOB = 1002,
        MEDIUMBLOB = 1003,
        TINYBLOB = 1004,
        BINARY = 1005,

        LONGTEXT = 1011,
        TINYTEXT = 1012,
        MEDIUMTEXT = 1013,

        DOUBLE = 1021,
        MEDIUMINT = 1022,

        // Postgre - 2000
        integer = 2001,
        character_varying = 2003,
        double_precision =2004,
        timestamp_without_time_zone=2005,
        //time_with_time_zone =2007,
        //timestamp_with_time_zone = 2008,
        //time_without_time_zone = 2009,
        //abstime = 2010,
        boolean =2006,
        Point = 2100,
    }
}
