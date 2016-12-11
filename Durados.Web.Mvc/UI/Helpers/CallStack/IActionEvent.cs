using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Durados.Web.Mvc.UI.Helpers.CallStack
{
    public interface IActionEvent
    {
        int Time { get; }
        Event Event { get; }

        string ObjectName { get; }
        string ActionName { get; }
        string Id { get; }
        
    }
}
