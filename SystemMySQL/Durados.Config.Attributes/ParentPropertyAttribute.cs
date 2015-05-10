using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Durados.Config.Attributes
{
    public class ParentPropertyAttribute : RelationPropertyAttribute
    {
        public override PropertyType PropertyType { get { return PropertyType.Parent; } }

    }
}
