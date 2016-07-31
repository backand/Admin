using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Durados.Config.Attributes
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public abstract class PropertyAttribute : Attribute
    {
        public string DisplayName { get; set; }

        public abstract PropertyType PropertyType { get; }

        public string Description { get; set; }

        public bool DoNotCopy { get; set; }

        public object Default { get; set; }

        public string Groups { get; set; }

    }

    public enum PropertyType
    {
        Column,
        Parent,
        Children
    }

    public enum PropertyGroup
    {
        ColorsDesign,
        Skin
    }
}
