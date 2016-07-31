using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Durados.Config.Attributes
{
    public abstract class RelationPropertyAttribute : PropertyAttribute
    {
        public string TableName { get; set; }

        public string RelationName { get; set; }

        public bool NonRecursive { get; set; }
    }
}
