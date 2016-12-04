using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Durados.Web.Mvc.UI.Helpers.CallStack
{
    public interface IAction
    {
        string ObjectName { get; }
        string ActionName { get; }

        IActionEvent Started { get; }

        IActionEvent Ended { get; }

        void SetEnded(IActionEvent ended);

        void AddActionChild(IAction action);

        IAction[] InnerActions { get; }
    }
}
