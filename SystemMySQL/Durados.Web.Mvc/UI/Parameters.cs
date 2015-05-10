using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Durados.Web.Mvc.UI
{
    public class Parameters
    {
        public Dictionary<string, List<object>> Views { get; private set; }
        public Durados.DataAction DataAction { get; set; }
        
        public Parameters()
        {
            Views = new Dictionary<string, List<object>>();
            DataAction = DataAction.Create;
        }
    }
}
