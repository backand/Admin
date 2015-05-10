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

        protected override Generator GetNewGenerator()
        {
            return new MySqlGenerator();
        }

        protected override SqlAccess GetNewSqlAccess()
        {
            return new MySqlAccess();
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
