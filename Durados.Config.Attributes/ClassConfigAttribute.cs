using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Durados.Config.Attributes
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class ClassConfigAttribute : Attribute
    {
        public string TableName { get; set; }

        public string DerivedTypesColumnName { get; set; }


    }
}
