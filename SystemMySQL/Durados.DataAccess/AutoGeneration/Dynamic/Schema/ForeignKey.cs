using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Durados.DataAccess.AutoGeneration.Dynamic.Schema
{
    public class ForeignKey
    {
        public string Name { get; set; }
        public string TableName { get; set; }
        public string ReferenceTableName { get; set; }
        public List<string> ColumnsNames { get; private set; }
        public List<string> ReferenceColumnsNames { get; private set; }

        public ForeignKey()
        {
            ColumnsNames = new List<string>();
            ReferenceColumnsNames = new List<string>();
        }
    }
}
