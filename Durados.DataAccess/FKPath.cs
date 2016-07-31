using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Durados.DataAccess
{
    public class FKPath
    {
        public string From { get; set; }

        public string To { get; set; }

        public bool isPk { get; set; }

        public bool isAutoIncrement { get; set; }


        public string TypeFullName { get; set; }
        public FKPath()
        {

        }
    }
}
