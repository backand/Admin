using System;
using System.IO;
using System.Data;
using System.Data.SqlClient;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Durados.DataAccess.AutoGeneration
{
        public class User : Generator
        {

        public User(string connectionString, string schemaGeneratorFileName) :
            base(connectionString, schemaGeneratorFileName)
        {
        }

        protected override void BuildSchema(string connectionString)
        {
            return;
        }

        public void Buid()
        {
            base.BuildSchema(connectionString);
        }

        public bool Exists()
        {
            return base.SchemaExists(connectionString);
        }
        

        protected override string RootObjectName
        {
            get { return "durados_User"; }
        }
    }
        public class MySqlUser : User
        {
            public MySqlUser(string connectionString, string schemaGeneratorFileName) :
                base(connectionString, schemaGeneratorFileName)
            {
            }
            protected override SqlSchema GetNewSqlSchema()
            {
                return new MySqlSchema();
            }


            protected override void BuildSchema(string connectionString)
            {
                return;
            }
        }

}
