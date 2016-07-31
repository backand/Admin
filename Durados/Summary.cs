using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Durados
{
    public class Summary
    {
        public Summary()
        {
        }

        [Durados.Config.Attributes.ColumnProperty()]
        public string GroupFields { get; set; }

        [Durados.Config.Attributes.ColumnProperty()]
        public string SummaryFields { get; set; }

        [Durados.Config.Attributes.ColumnProperty()]
        public string PermanentFilter { get; set; }

    }

    public enum SummaryFunctions
    {
        Sum,
        Count,
        Avg,
        Std
    }
}
