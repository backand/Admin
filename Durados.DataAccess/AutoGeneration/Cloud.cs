using System;
using System.IO;
using System.Data;
using System.Data.SqlClient;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Durados.DataAccess.AutoGeneration
{
    public class Cloud : Generator
    {

        public Cloud(string connectionString, string schemaGeneratorFileName) :
            base(connectionString, schemaGeneratorFileName)
        {
        }
        

        protected override string RootObjectName
        {
            get { return "durados_Cloud"; }
        }


    }

    public class MySqlCloud : Cloud
    {
        public MySqlCloud(string connectionString, string schemaGeneratorFileName) :
            base(connectionString, schemaGeneratorFileName)
        {
        }
        protected override SqlSchema GetNewSqlSchema()
        {
            return   new MySqlSchema();
        }


    }
}
