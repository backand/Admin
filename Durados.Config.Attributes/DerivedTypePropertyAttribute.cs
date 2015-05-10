using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Durados.Config.Attributes
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = true)]
    public class DerivedTypePropertyAttribute : Attribute
    {
        public Type Type { get; set; }
        public string Name { get; set; }
    }
}
