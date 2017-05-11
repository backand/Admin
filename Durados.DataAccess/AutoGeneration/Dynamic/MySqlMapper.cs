using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Durados.DataAccess.AutoGeneration.Dynamic
{
    public class MySqlMapper : Mapper
    {
        protected override SqlSchema GetNewSqlSchema()
        {
            return new MySqlSchema();
        }

        public override Generator GetNewGenerator()
        {
            return new MySqlGenerator();
        }

        protected override SqlAccess GetNewSqlAccess()
        {
            return new MySqlAccess();
        }
        public override string GetGenerateScriptFileName(string fileName)
        {
            return fileName.Replace(".sql", "-MySql.sql");
        }
        public override History GetHistoryGenerator(string systemConnectionString, string historySchemaGeneratorFileName)
        {
            return new  MySqlHistory(systemConnectionString, historySchemaGeneratorFileName);
        }

        public override Cloud GetCloudGenerator(string systemConnectionString, string cloudSchemaGeneratorFileName)
        {
            return new MySqlCloud(systemConnectionString, cloudSchemaGeneratorFileName);
        }
        public override Durados.DataAccess.AutoGeneration.PersistentSession GetPersistentSessionGenerator(string systemConnectionString, string sessionSchemaGeneratorFileName)
        {
            return new Durados.DataAccess.AutoGeneration.MySqPersistentSession(systemConnectionString, sessionSchemaGeneratorFileName);
        }
        public override Content GetPersistentContentGenerator(string systemConnectionString, string contentSchemaGeneratorFileName)
        {
            return new Durados.DataAccess.AutoGeneration.MySqlContent(systemConnectionString, contentSchemaGeneratorFileName);
        }
        public override Link GetLinkGenerator(string systemConnectionString, string contentSchemaGeneratorFileName)
        {
            return new Durados.DataAccess.AutoGeneration.MySqlLink(systemConnectionString, contentSchemaGeneratorFileName);
        }

        public override CustomViews GetCustomViewsGenerator(string systemConnectionString, string contentSchemaGeneratorFileName)
        {
            return new Durados.DataAccess.AutoGeneration.MySqlCustomViews(systemConnectionString, contentSchemaGeneratorFileName);
        }
        public override Durados.DataAccess.AutoGeneration.User GetUserGenerator(string systemConnectionString, string contentSchemaGeneratorFileName)
        {
            return new Durados.DataAccess.AutoGeneration.MySqlUser(systemConnectionString, contentSchemaGeneratorFileName);
        }

       
    }

    public class MySqlGenerator : Generator
    {
        protected override SqlSchema GetNewSqlSchema()
        {
            return new MySqlSchema();
        }

        protected override SqlAccess GetNewSqlAccess()
        {
            return new MySqlAccess();
        }
    }
}
