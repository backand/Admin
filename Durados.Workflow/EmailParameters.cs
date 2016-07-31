using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Durados.Workflow
{
    public class EmailParameters
    {
        public string From { get; set; }
        public string[] To { get; set; }
        public string[] Cc { get; set; }
        public string[] Bcc { get; set; }
        public string Subject { get; set; }
        public string Message { get; set; }
        
    }
}
