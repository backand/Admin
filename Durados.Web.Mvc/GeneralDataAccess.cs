using Durados.Data;
using Durados.DataAccess;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace Durados.Web.Mvc
{
    public class DataAccessFactory 
    {
        public static Durados.Data.IDataAccess GetDataAccess(string connectionstring)
        {
            if (string.IsNullOrEmpty(connectionstring))
                return null;
            if (MySqlAccess.IsMySqlConnectionString(connectionstring))
                return new MYSqlDataAccess() { ConnectionString = connectionstring };
            return new MSSqlDataAccess() { ConnectionString = connectionstring };
        }
    }

    public abstract class DuradosDataAcesss : IDataAccess
    {
        protected virtual SqlSchema GetSchema() { return null; }
        public virtual System.Data.IDbConnection GetConnection(string connectionString = null)
        {
            if (string.IsNullOrEmpty(connectionString))
                connectionString = ConnectionString;
            return GetSchema().GetConnection(connectionString);
        }
        public virtual System.Data.IDbCommand GetCommand(string connectionString = null)
        {
            SqlSchema schema = GetSchema();
            if (string.IsNullOrEmpty(connectionString))
                connectionString = ConnectionString;
            return schema.GetCommand(string.Empty, schema.GetConnection(connectionString));
        }
       
         public virtual IDbCommand GetCommand(System.Data.IDbConnection cnn, string cmdText)
        {
            return GetSchema().GetCommand(cmdText, cnn);
        }

      
        public virtual ISqlTextBuilder GetSqlBuilder() { return null; }

        protected string connectionString;
        public string ConnectionString
        {
            get
            {
                return connectionString;
            }
            set
            {
                connectionString = value;
            }
        }
    }
    public class MSSqlDataAccess : DuradosDataAcesss
    {
        protected override SqlSchema GetSchema()
        {
            return new SqlSchema(); 
        }
       
        public override ISqlTextBuilder GetSqlBuilder()
        {
            return new SqlTextBuilder();
        }
    }
    public class MYSqlDataAccess : DuradosDataAcesss
    {
        protected override SqlSchema GetSchema()
        {
            return new MySqlSchema();
        }


        public override ISqlTextBuilder GetSqlBuilder()
        {
            return new MySqlTextBuilder();
        }
    }
}
