using System;
using System.IO;
using System.Data;
using System.Data.SqlClient;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Durados.DataAccess.AutoGeneration
{
    public class CustomViews : Generator
    {

        public CustomViews(string connectionString, string schemaGeneratorFileName) :
            base(connectionString, schemaGeneratorFileName)
        {
        }

        protected override string RootObjectName
        {
            get { return "durados_CustomViews"; }
        }
    }
    public class MySqlCustomViews : CustomViews
    {
        public MySqlCustomViews(string connectionString, string schemaGeneratorFileName) :
            base(connectionString, schemaGeneratorFileName)
        {
        }
        protected override SqlSchema GetNewSqlSchema()
        {
            return new MySqlSchema();
        }
    }
}
