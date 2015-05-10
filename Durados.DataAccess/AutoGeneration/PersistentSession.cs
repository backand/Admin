using System;
using System.IO;
using System.Data;
using System.Data.SqlClient;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Durados.DataAccess.AutoGeneration
{
    public class PersistentSession : Generator
    {
        //private string connectionString;
        //private string sessionSchemaGeneratorFileName;

        public PersistentSession(string connectionString, string sessionSchemaGeneratorFileName)
            : base(connectionString, sessionSchemaGeneratorFileName)
        {
        }

        protected override string RootObjectName
        {
            get
            {
                return "Durados_Session";
            }
        }
        //private void BuildSchema(string connectionString)
        //{
        //    if (SchemaExists(connectionString))
        //        return;

        //    FileInfo file = new FileInfo(sessionSchemaGeneratorFileName);
        //    string script = file.OpenText().ReadToEnd();
        //    SqlConnection conn = new SqlConnection(connectionString);
        //    script = script.Replace("__DB_NAME__", conn.Database);
        //    conn.Open();
        //    try
        //    {
        //        SqlCommand command = new SqlCommand();

        //        command.Connection = conn;
        //        //command.CommandType = System.Data.CommandType.StoredProcedure;
        //        command.CommandText = script;
        //        command.ExecuteNonQuery();
        //    }
        //    finally
        //    {
        //        conn.Close();
        //    }
        //}

        //private bool SchemaExists(string connectionString)
        //{
        //    string sql = "SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[Durados_Session]')";
        //    SqlConnection conn = new SqlConnection(connectionString);
        //    conn.Open();
        //    try
        //    {
        //        SqlCommand command = new SqlCommand();

        //        command.Connection = conn;
        //        //command.CommandType = System.Data.CommandType.StoredProcedure;
        //        command.CommandText = sql;
        //        return command.ExecuteScalar() != null && command.ExecuteScalar() != DBNull.Value;
        //    }
        //    finally
        //    {
        //        conn.Close();
        //    }
        //}

        protected object GetSession(string name, string sessionID)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                string sql = "select Scalar, TypeCode, SerializedObject, ObjectType from Durados_Session with(nolock) where SessionID=@SessionID and Name=@Name";

                SqlCommand command = new SqlCommand(sql, connection);

                command.CommandText = sql;

                SqlParameter name_parameter = new SqlParameter();

                name_parameter.SqlDbType = System.Data.SqlDbType.NVarChar;
                name_parameter.Value = name;
                name_parameter.ParameterName = "@Name";
                command.Parameters.Add(name_parameter);

                command.Parameters.AddWithValue("@SessionID", sessionID);


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
            if (value == null)
                return;

            TypeCode typeCode = Convert.GetTypeCode(value);

            if (typeCode == TypeCode.Object)
            {

            }
            else
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {

                    string sql = "durados_SetSession";
                    SqlCommand command = new SqlCommand(sql, connection);

                    command.CommandText = sql;
                    command.CommandType = CommandType.StoredProcedure;

                    SqlParameter name_parameter = new SqlParameter();

                    name_parameter.SqlDbType = System.Data.SqlDbType.NVarChar;
                    name_parameter.Value = name;
                    name_parameter.ParameterName = "@Name";
                    command.Parameters.AddWithValue("@SessionID", sessionID);
                    command.Parameters.AddWithValue("@Scalar", value.ToString());
                    command.Parameters.AddWithValue("@TypeCode", typeCode.ToString());
                    command.Parameters.Add(name_parameter);

                    connection.Open();

                    SqlTransaction transaction = connection.BeginTransaction(IsolationLevel.ReadUncommitted);
                    command.Transaction = transaction;

                    command.ExecuteNonQuery();

                    transaction.Commit();
                }
            }
        }

        //protected void SetSession(string name, string sessionID, object value)
        //{
        //    TypeCode typeCode = Convert.GetTypeCode(value);

        //    if (typeCode == TypeCode.Object)
        //    {
                
        //    }
        //    else
        //    {
        //        using (SqlConnection connection = new SqlConnection(connectionString))
        //        {

        //            string sql = "select Name from Durados_Session where SessionID=@SessionID  and Name=@Name";
        //            SqlCommand command = new SqlCommand(sql, connection);

        //            command.CommandText = sql;

        //            SqlParameter name_parameter = new SqlParameter();

        //            name_parameter.SqlDbType = System.Data.SqlDbType.NVarChar;
        //            name_parameter.Value = name;
        //            name_parameter.ParameterName = "@Name";
        //            command.Parameters.Add(name_parameter);

        //            command.Parameters.AddWithValue("@SessionID", sessionID);

        //            connection.Open();

        //            SqlTransaction transaction = connection.BeginTransaction(IsolationLevel.ReadUncommitted);
        //            command.Transaction = transaction;

        //            object scalar = command.ExecuteScalar();

        //            if (scalar == null || scalar == DBNull.Value)
        //            {
        //                sql = "insert into Durados_Session(SessionID, Name, Scalar, TypeCode) values (@SessionID, @Name, @Scalar, @TypeCode)";

        //            }
        //            else
        //            {
        //                sql = "update Durados_Session set Scalar=@Scalar, TypeCode=@TypeCode, SerializedObject=null, ObjectType=null where SessionID=@SessionID and Name=@Name";

        //            }

        //            command.CommandText = sql;

        //            command.Parameters.Clear();
        //            command.Parameters.AddWithValue("@SessionID", sessionID);
        //            command.Parameters.AddWithValue("@Scalar", value.ToString());
        //            command.Parameters.AddWithValue("@TypeCode", typeCode.ToString());
        //            command.Parameters.Add(name_parameter);

        //            command.ExecuteNonQuery();

        //            transaction.Commit();
        //        }
        //    }
        //}

        //public void Clear()
        //{
        //    using (SqlConnection connection = new SqlConnection(connectionString))
        //    {
        //        connection.Open();
        //        SqlCommand command = new SqlCommand("truncate table Durados_Session", connection);
        //        command.ExecuteNonQuery();
        //    }
        //}
    }
}
