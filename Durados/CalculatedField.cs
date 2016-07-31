using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Durados
{
    public class CalculatedField
    {
        [Durados.Config.Attributes.ColumnProperty()]
        public int ID { get; set; }

        [Durados.Config.Attributes.ColumnProperty()]
        public int ViewID { get; set; }

        [Durados.Config.Attributes.ColumnProperty()]
        public int FieldID { get; set; }

    }
}
