using System;
using System.IO;
using System.Data;
using System.Data.SqlClient;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Durados.DataAccess.AutoGeneration
{
    public class Link : Generator
    {
        
        public Link(string connectionString, string schemaGeneratorFileName):
            base(connectionString, schemaGeneratorFileName)
        {
        }

        protected override string RootObjectName
        {
            get { return "durados_Link"; }
        }
    }

    public class MySqlLink : Link
    {
        public MySqlLink(string connectionString, string schemaGeneratorFileName) :
            base(connectionString, schemaGeneratorFileName)
        {
        }
        protected override SqlSchema GetNewSqlSchema()
        {
            return new MySqlSchema();
        }
    }
}
