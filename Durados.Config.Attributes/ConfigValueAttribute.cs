using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Durados.Config.Attributes
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = true)]
    public class ConfigValueAttribute : Attribute
    {
        public object Value { get; set; }
        public Type Type { get; set; }
    }
}
