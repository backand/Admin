using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace Durados.Diagnostics
{
    public class Report
    {
        public Report()
        {
            OverLoadLimit = 100;
        }

        public DateTime DateTime { get; set; }
        public string Name
        {
            get
            {
                return "Diagnostics Report";
            }
        }

        public int OverLoadLimit { get; set; }

        public string GetStackTrace()
        {
            StackTrace stackTrace = new StackTrace();
            return stackTrace.ToString();
        }
    }
}
