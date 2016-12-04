using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Durados.Web.Mvc.UI.Helpers.CallStack
{
    public class Action : IAction
    {
        private List<IAction> innerActions;

        public Action(string objectName, string actionName, IActionEvent started, IActionEvent ended = null)
        {
            ObjectName = objectName;
            ActionName = actionName;
            Started = started;
            Ended = ended;
            innerActions = new List<IAction>();

        }
        public string ObjectName { get; private set; }
        public string ActionName { get; private set; }
        public IActionEvent Started { get; private set; }
        public IActionEvent Ended { get; private set; }

        public void SetEnded(IActionEvent ended)
        {
            Ended = ended;
        }

        public void AddActionChild(IAction action)
        {
            innerActions.Add(action);
        }
        public IAction[] InnerActions
        {
            get
            {
                return innerActions.ToArray();
            }
        }
    }
}
