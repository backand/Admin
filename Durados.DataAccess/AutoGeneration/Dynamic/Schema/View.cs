using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Durados.DataAccess.AutoGeneration.Dynamic.Schema
{
    public class View : Table
    {
        public View() : base() 
        {
            Dependencies = new List<Dependency>();
        }

        public string EditableTableName { get; set; }
        public override ObjectType ObjectType
        {
            get
            {
                return ObjectType.View;
            }
        }
        public List<Dependency> Dependencies { get; private set; }

    }
}
