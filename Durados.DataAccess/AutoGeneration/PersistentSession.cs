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
                return "durados_session";
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
            using (IDbConnection connection = GetConnection(connectionString))
            {
                connection.Open();

                string sql = GetSessionSelectStatement();

                IDbCommand command = GetCommand(sql, connection);

                command.CommandText = sql;

                IDataParameter name_parameter = GetNewParameter();

                name_parameter.DbType = System.Data.DbType.String;
                name_parameter.Value = name;
                name_parameter.ParameterName = "@Name";
                command.Parameters.Add(name_parameter);

                IDataParameter sessionID_parameter = GetParameter("@SessionID", sessionID);
                command.Parameters.Add(sessionID_parameter);

                //command.Parameters.Add("@SessionID", sessionID);


                IDataReader reader = command.ExecuteReader();

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
                using (IDbConnection connection = GetConnection(connectionString))
                {

                    string sql = "durados_SetSession";
                    IDbCommand command = GetCommand(sql, connection);
                    
                    command.CommandText = sql;
                    command.CommandType = CommandType.StoredProcedure;

                    IDataParameter name_parameter = GetNewParameter();

                    name_parameter.DbType = System.Data.DbType.String;
                    name_parameter.Value = name;
                    name_parameter.ParameterName = "@Name";

                    command.Parameters.Add(GetParameter("@SessionID", sessionID));
                    command.Parameters.Add(GetParameter("@Scalar", value.ToString()));
                    command.Parameters.Add(GetParameter("@TypeCode", typeCode.ToString()));

                     command.Parameters.Add(name_parameter);

                    connection.Open();

                    IDbTransaction transaction = connection.BeginTransaction(IsolationLevel.ReadUncommitted);
                    command.Transaction = transaction;

                    command.ExecuteNonQuery();

                    transaction.Commit();
                }
            }
        }
        protected virtual string GetSessionSelectStatement()
        {
            return "select Scalar, TypeCode, SerializedObject, ObjectType from Durados_Session with(nolock) where SessionID=@SessionID and Name=@Name";
        }

        protected virtual IDataParameter GetNewParameter()
        {
            return GetNewSqlSchema().GetNewParameter();
            //return new SqlParameter();
        }
        protected virtual IDataParameter GetParameter(string name, object value)
        {
            return GetNewSqlSchema().GetNewParameter(name, value);
            //return new SqlParameter();
        }
        protected virtual IDbCommand GetCommand(string sql, IDbConnection connection)
        {
            return GetNewSqlSchema().GetCommand(sql, connection);
        }

        protected virtual IDbConnection GetConnection(string connectionString)
        {
            return GetNewSqlSchema().GetConnection(connectionString);
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
    public class MySqPersistentSession : PersistentSession
    {
        public MySqPersistentSession(string connectionString, string schemaGeneratorFileName) :
            base(connectionString, schemaGeneratorFileName)
        {
        }
        protected override SqlSchema GetNewSqlSchema()
        {
            return new MySqlSchema();
        }

        
    }
}
