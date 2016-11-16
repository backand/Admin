using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Durados.Web.Mvc.UI.Helpers.CallStack
{
    public class CallStackConverter
    {
        public IActionTree ChronologicalListToTree(IEnumerable<IActionEvent> events)
        {
            if (events.Count() == 0)
                return null;

            IActionEvent firstEvent = events.First();
            IActionEvent lastEvent = events.Last();

            IAction root = GetNewAction(firstEvent, lastEvent);

            IActionTree tree = GetNewTree(root);

            IList<IAction> parentActions = new List<IAction>() { root };

            foreach (IActionEvent actionEvent in events)
            {
                if (actionEvent == firstEvent)
                {
                    continue;
                }

                if (actionEvent == lastEvent)
                {
                    break;
                }

                IAction parentAction = parentActions.Last();

                if (actionEvent.Event.Equals(Event.Started))
                {
                    IAction action = GetNewAction(actionEvent);
                    parentAction.AddActionChild(action);
                    parentActions.Add(action);
                }
                else if (actionEvent.Event.Equals(Event.Ended))
                {
                    parentAction.SetEnded(actionEvent);
                    parentActions.Remove(parentAction);
                }
                

            }


            return tree;
        }

        private IAction GetNewAction(IActionEvent started, IActionEvent ended = null)
        {
            return new Action(started.ObjectName, started.ActionName, started, ended);
        }

        
        protected virtual IActionTree GetNewTree(IAction root)
        {
            return new ActionTree(root);
        }
    }
}
