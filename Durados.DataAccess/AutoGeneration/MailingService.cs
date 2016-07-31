using System;
using System.IO;
using System.Data;
using System.Data.SqlClient;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Durados.DataAccess.AutoGeneration
{
    public class MailingService : Generator
    {

        public MailingService(string connectionString, string schemaGeneratorFileName) :
            base(connectionString, schemaGeneratorFileName)
        {
        }

        protected override string RootObjectName
        {
            get { return "durados_MailingServiceSubscribers"; }
        }
    }
}
