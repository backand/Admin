using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;
using System.Data;

namespace Durados.DataAccess
{
    public static class SqlGeneralAccess
    {
		#region Fields (1) 

        private const char COMMA = ',';

		#endregion Fields 

		#region Methods (15) 

		// Public Methods (11) 

        public static object Create(SqlRequest sqlRequest, SqlConnection connection, SqlTransaction transaction)
        {
            if (sqlRequest == null)
            {
                throw new DuradosException("sqlRequest is empty");
            }

            object returnedValue = null;
            SqlCommand command = new SqlCommand(sqlRequest.Sql, connection);

            if (transaction != null)
            {
                command.Transaction = transaction;
            }

            if (sqlRequest.Parameters != null)
            {
                command.Parameters.AddRange(sqlRequest.Parameters.ToArray());
            }

            returnedValue = command.ExecuteScalar();
            command.Parameters.Clear();

            return returnedValue;
        }

        public static object Create(Dictionary<string, object> values, string tableName, bool autoIdentity, SqlConnection connection, SqlTransaction transaction)
        {
            return Create(values, tableName, null, autoIdentity, connection, transaction);
        }

        public static object Create(Dictionary<string, object> values, string tableName, IList<string> encryptedcolumnNamesList, bool autoIdentity, SqlConnection connection, SqlTransaction transaction)
        {
            string sql = GetSqlInsertStatement(values.Keys.ToList(), tableName, autoIdentity, encryptedcolumnNamesList);
            IList<SqlParameter> parameters = GetParemeters(values);
            SqlRequest sqlRequest = GetInsertSqlRequest(values, tableName, encryptedcolumnNamesList, autoIdentity);

            return Create(sqlRequest, connection, transaction);
        }

        public static object Create(Dictionary<string, object> values, string tableName, bool autoIdentity, string connectionString)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                object returnValue = Create(values, tableName, null, autoIdentity, connection, null);
                return returnValue;
            }
        }

        public static SqlRequest GetInsertSqlRequest(Dictionary<string, object> values, string tableName, IList<string> encryptedcolumnNamesList, bool autoIdentity)
        {
            string sql = GetSqlInsertStatement(values.Keys.ToList(), tableName, autoIdentity, encryptedcolumnNamesList);
            IList<SqlParameter> parameters = GetParemeters(values);

            return new SqlRequest(sql, parameters);
        }

        public static IList<SqlParameter> GetParemeters(Dictionary<string, object> values)
        {
            List<System.Data.SqlClient.SqlParameter> parameters = new List<System.Data.SqlClient.SqlParameter>();

            foreach (KeyValuePair<string, object> item in values)
            {
                SqlParameter parameter = new SqlParameter(item.Key, item.Value);

                parameters.Add(parameter);
            }

            return parameters.ToArray();
        }

        public static string GetSqlInsertStatement(IList<string> columnNames, bool autoIdentity, string tableName)
        {
            return GetSqlInsertStatement(columnNames, tableName, autoIdentity, null);
        }

        public static string GetSqlInsertStatement(IList<string> columnNames, string tableName, bool autoIdentity, IList<string> encryptedcolumnNamesList)
        {
            string sql = "insert into [{0}] ({1}) values ({2});";
            string delimitedColumns = GetDelimitedColumns(columnNames);

            if (string.IsNullOrEmpty(delimitedColumns))
            {
                sql = string.Format("insert into [{0}] default values;", tableName);
            }
            else
            {
                string delimitedColumnsParameters = GetDelimitedColumnsParameters(columnNames, encryptedcolumnNamesList);

                sql = string.Format(sql, tableName, delimitedColumns, delimitedColumnsParameters);
            }

            if (autoIdentity)
            {
                sql += "SELECT IDENT_CURRENT(N'[" + tableName + "]') AS ID";
            }


            return sql;
        }

        public static string GetSqlUpdateStatement(IList<string> columnNames, string tableName, string whereStatement)
        {
            string sql = "update [{0}] set {2} where {1}";
            string updateSetColumns = GetUpdateSetColumns(columnNames);

            sql = string.Format(sql, tableName, whereStatement, updateSetColumns);

            return sql;
        }

        public static void ExecuteNonQuery(SqlRequest sqlRequest, SqlConnection connection, SqlTransaction transaction)
        {
            SqlCommand command = new SqlCommand(sqlRequest.Sql, connection);

            if (transaction != null)
            {
                command.Transaction = transaction;
            }

            if (sqlRequest.Parameters != null)
            {
                command.Parameters.AddRange(sqlRequest.Parameters.ToArray());
            }

            command.ExecuteNonQuery();
            command.Parameters.Clear();
        }

        public static void Update(SqlRequest sqlRequest, SqlConnection connection, SqlTransaction transaction)
        {
            ExecuteNonQuery(sqlRequest, connection, transaction);
        }
       
        public static void Update(Dictionary<string, object> values, string tableName, string whereStatement, SqlConnection connection, SqlTransaction transaction)
        {
            string sql = SqlGeneralAccess.GetSqlUpdateStatement(values.Keys.ToList(), tableName, whereStatement);
            IList<SqlParameter> parameters = SqlGeneralAccess.GetParemeters(values);

            SqlRequest sqlRequest = new SqlRequest(sql, parameters);
            Update(sqlRequest, connection, transaction);
        }

        public static void Update(Dictionary<string, object> values, string tableName, string whereStatement, string connectionString)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                Update(values, tableName, whereStatement, connection, null);
            }
        }

        public static DataTable Select(Dictionary<string, object> values, string tableName, string whereStatement, string connectionString)
        {
            string sql = SqlGeneralAccess.GetSqlSelectStatement(values.Keys.ToList(), tableName, whereStatement);
            SqlConnection connection = new SqlConnection(connectionString);
            SqlCommand command = new SqlCommand(sql,connection);
            SqlDataAdapter adapter = new SqlDataAdapter(command);
            DataTable table = new DataTable();
            adapter.Fill(table);
            return table;
        }

        public static void Delete(string tableName, string whereStatement, string connectionString)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string sql = "delete from [{0}] where {1}";
                sql = string.Format(sql, tableName, whereStatement);
                SqlRequest requst = new SqlRequest(sql, null);
                connection.Open();
                ExecuteNonQuery(requst, connection, null);
            }
        }
        // Private Methods (4) 

        private static string GetSqlSelectStatement(IList<string> columnNames, string tableName, string whereStatement)
        {
            string sql = "select {0} from [{1}] where {2};";
            string delimitedColumns = GetDelimitedColumns(columnNames);

            if (string.IsNullOrEmpty(delimitedColumns))
            {
                sql = string.Format("select * from [{0}] where {1};", tableName, whereStatement);
            }
            else
            {
                sql = string.Format(sql,delimitedColumns ,tableName ,whereStatement );
            }
            return sql;
        }

        private static string GetDelimitedColumns(IList<string> columnNamesList)
        {
            string delimitedColumns = string.Empty;

            foreach (string columnName in columnNamesList)
            {
                delimitedColumns += "[" + columnName + "],";
            }

            return delimitedColumns.TrimEnd(COMMA);
        }

        private static string GetDelimitedColumnsParameters(IList<string> columnNamesList, IList<string> encryptedcolumnNamesList)
        {
            string delimitedColumns = string.Empty;

            foreach (string columnName in columnNamesList)
            {
                if (encryptedcolumnNamesList != null && encryptedcolumnNamesList.Contains(columnName))
                {
                    delimitedColumns += "ENCRYPTBYKEY(KEY_GUID('DuradosSymmetricKey'),@" + GetVarFromName(columnName) + ")" + COMMA;
                }
                else
                {
                    delimitedColumns += "@" + GetVarFromName(columnName) + COMMA;
                }
            }

            return delimitedColumns.TrimEnd(COMMA);
        }

        private static string GetUpdateSetColumns(IList<string> columnNames)
        {
            string updateSetColumns = string.Empty;

            foreach (string columnName in columnNames)
            {
                updateSetColumns += "[" + columnName + "] = " + "@" + GetVarFromName(columnName) + COMMA;
            }

            return updateSetColumns.TrimEnd(COMMA);
        }

        private static string GetVarFromName(string name)
        {
            return name.ReplaceNonAlphaNumeric();
        }

		#endregion Methods 
    
        
    
       
    }
}
