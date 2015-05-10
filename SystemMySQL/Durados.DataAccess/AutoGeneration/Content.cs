using System;
using System.IO;
using System.Data;
using System.Data.SqlClient;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Durados.DataAccess.AutoGeneration
{
    public class Content : Generator
    {
        
        public Content(string connectionString, string schemaGeneratorFileName):
            base(connectionString, schemaGeneratorFileName)
        {
        }

        protected override string RootObjectName
        {
            get { return "durados_Html"; }
        }
    }
}
