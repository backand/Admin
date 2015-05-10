using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Durados
{
    public class Tooltip
    {
        [Durados.Config.Attributes.ColumnProperty()]
        public string Name { get; private set; }

        [Durados.Config.Attributes.ColumnProperty()]
        public string Title { get; set; }

        private string description;
        [Durados.Config.Attributes.ColumnProperty()]
        public string Description
        {
            get
            {
                return description;
            }
            set
            {
                description = value == null ? null : value.Replace('"', '\'');
            }
        }

        public Tooltip(string name) { Name = name; }
    }
}
