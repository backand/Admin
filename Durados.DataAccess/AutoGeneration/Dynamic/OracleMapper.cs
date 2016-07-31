using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Durados.DataAccess.AutoGeneration.Dynamic
{
    public class OracleMapper : Mapper
    {
        protected override SqlSchema GetNewSqlSchema()
        {
            return new OracleSchema();
        }

        public override Generator GetNewGenerator()
        {
            return new OracleGenerator();
        }
    }

    public class OracleGenerator : Generator
    {
        protected override SqlSchema GetNewSqlSchema()
        {
            return new OracleSchema();
        }
        protected override SqlAccess GetNewSqlAccess()
        {
            return new OracleAccess();
        }
    }
}
