using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Durados.DataAccess.AutoGeneration.Dynamic
{
    public class PostgreMapper : MySqlMapper
    {
        protected override SqlSchema GetNewSqlSchema()
        {
            return new PostgreSchema();
        }

        public override Generator GetNewGenerator()
        {
            return new PostgreGenerator();
        }

        protected override SqlAccess GetNewSqlAccess()
        {
            return new PostgreAccess();
        }
    }

    public class PostgreGenerator : MySqlGenerator
    {
        protected override SqlSchema GetNewSqlSchema()
        {
            return new PostgreSchema();
        }

        protected override SqlAccess GetNewSqlAccess()
        {
            return new PostgreAccess();
        }
    }
}
