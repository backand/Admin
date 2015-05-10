using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Durados.Config.Attributes
{
    public class ChildrenPropertyAttribute : RelationPropertyAttribute
    {
        public Type Type { get; set; }

        public override PropertyType PropertyType { get { return PropertyType.Children; } }

        public string DictionaryKeyColumnName { get; set; }

    }
}
