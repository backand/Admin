using System;
using System.IO;
using System.Data;
using System.Data.SqlClient;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Durados.DataAccess.AutoGeneration
{
        public class PlugIn : Generator
        {

            public PlugIn(string connectionString, string schemaGeneratorFileName) :
            base(connectionString, schemaGeneratorFileName)
        {
        }

        

        protected override string RootObjectName
        {
            get { return "durados_PlugIn"; }
        }
    }

}
