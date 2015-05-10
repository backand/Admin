using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Durados.Config.Attributes
{
    public class ColumnPropertyAttribute : PropertyAttribute
    {
        public override PropertyType PropertyType { get { return PropertyType.Column; } }
        
    }

    public class EnumDisplayAttribute : Attribute
    {
        public string[] EnumNames { get; set; }
        public string[] EnumDisplayNames { get; set; }

        public Dictionary<string, string> GetEnumDisplayNames()
        {
            if (EnumDisplayNames == null)
                return null;

            if (EnumNames == null)
                return null;

            if (EnumNames.Length != EnumDisplayNames.Length)
                throw new Exception("The display name array length must be equal to the enum names");

            Dictionary<string, string> d = new Dictionary<string, string>();

            for (int i = 0; i < EnumNames.Length; i++)
            {
                d.Add(EnumNames[i], EnumDisplayNames[i]);
            }

            return d;
        }
    }


    public class ParameterAttribute : Attribute
    {
        public string ParameterName { get; set; }
        public ParameterRole Role { get; set; }
        public string Pair{ get; set; }
        public string  WFAction { get; set; }

    }

    public enum ParameterRole{None =0
        ,Name
        ,Value
    }
}
