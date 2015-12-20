using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Durados.Web.Mvc.Farm
{
    public class FarmMessage
    {
        public string AppName { get; set; }

        public DateTime Time { get; set; }
    }


    public class FarmMessageWrapper
    {
        public FarmMessage Message;

        public string SenderId;
    }
}
