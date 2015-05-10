using System;
using System.IO;
using System.Data;
using System.Data.SqlClient;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Durados.DataAccess
{
    public class PersistentSession
    {
        private string connectionString;
        private string sessionSchemaGeneratorFileName;

        public PersistentSession(string connectionString, string sessionSchemaGeneratorFileName)
        {
            this.connectionString = connectionString;
            this.sessionSchemaGeneratorFileName = sessionSchemaGeneratorFileName;
            BuildSchema(connectionString);
        }

        private void BuildSchema(string connectionString)
        {
            if (SchemaExists(connectionString))
                return;

            FileInfo file = new FileInfo(sessionSchemaGeneratorFileName);
            string script = file.OpenText().ReadToEnd();
            SqlConnection conn = new SqlConnection(connectionString);
            script = script.Replace("__DB_NAME__", conn.Database);
            conn.Open();
            try
            {
                SqlCommand command = new SqlCommand();

                command.Connection = conn;
                //command.CommandType = System.Data.CommandType.StoredProcedure;
                command.CommandText = script;
                command.ExecuteNonQuery();
            }
            finally
            {
                conn.Close();
            }
        }

        private bool SchemaExists(string connectionString)
        {
            string sql = "SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[Durados_Session]')";
            SqlConnection conn = new SqlConnection(connectionString);
            conn.Open();
            try
            {
                SqlCommand command = new SqlCommand();

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

        protected object GetSession(string name, string sessionID)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                string sql = "select Scalar, TypeCode, SerializedObject, ObjectType from Durados_Session where SessionID='" + sessionID + "' and name=N'" + name + "'";
                SqlCommand command = new SqlCommand(sql, connection);
                SqlDataReader reader = command.ExecuteReader();

                if (reader.Read())
                {
                    if (!reader.IsDBNull(reader.GetOrdinal("Scalar")))
                    {
                        string scalar = reader[reader.GetOrdinal("Scalar")].ToString();

                        if (!reader.IsDBNull(reader.GetOrdinal("TypeCode")))
                        {
                            TypeCode typeCode = (TypeCode)Enum.Parse(typeof(TypeCode), reader[reader.GetOrdinal("TypeCode")].ToString());

                            return Convert.ChangeType(scalar, typeCode);
                        }
                    
                    }
                    else if (!reader.IsDBNull(reader.GetOrdinal("SerializedObject")))
                    {
                        string serializedObject = reader[reader.GetOrdinal("SerializedObject")].ToString();

                        if (!reader.IsDBNull(reader.GetOrdinal("ObjectType")))
                        {
                            string objectType = reader[reader.GetOrdinal("ObjectType")].ToString();

                            Type type = Type.GetType(objectType);
                        }

                    }
                }

                return null;
            }
        }

        protected void SetSession(string name, string sessionID, object value)
        {
            TypeCode typeCode = Convert.GetTypeCode(value);

            if (typeCode == TypeCode.Object)
            {
                
            }
            else
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    string sql = "select Name from Durados_Session where SessionID=N'" + sessionID + "' and name=N'" + name + "'";
                    SqlCommand command = new SqlCommand(sql, connection);
                    SqlTransaction transaction = connection.BeginTransaction();
                    command.Transaction = transaction;

                    object scalar = command.ExecuteScalar();

                    if (scalar == null || scalar == DBNull.Value)
                    {
                        sql = "insert into Durados_Session(SessionID, Name, Scalar, TypeCode) values ('{0}', '{1}', '{2}', '{3}')";

                        sql = string.Format(sql, sessionID, name, value.ToString(), typeCode.ToString());
                        command.CommandText = sql;
                        command.ExecuteNonQuery();
                    }
                    else
                    {
                        sql = "update Durados_Session set Scalar='{0}', TypeCode='{1}', SerializedObject=null, ObjectType=null where SessionID='{2}' and Name='{3}')";

                        sql = string.Format(sql, value.ToString(), typeCode.ToString(), sessionID, name);
                        command.CommandText = sql;
                        command.ExecuteNonQuery();
                    }

                    transaction.Commit();
                }
            }
        }

        public void Clear()
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                SqlCommand command = new SqlCommand("truncate table Durados_Session", connection);
                command.ExecuteNonQuery();
            }
        }
    }
}
