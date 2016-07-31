using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Durados
{
    public class Parameter
    {
        [Durados.Config.Attributes.ColumnProperty()]
        public string Name { get; set; }
        [Durados.Config.Attributes.ColumnProperty()]
        public string Value { get; set; }
        [Durados.Config.Attributes.ColumnProperty()]
        public bool UseSqlParser { get; set; }
        //[Durados.Config.Attributes.ColumnProperty()]
        //public string Message { get; set; }

    }
}
