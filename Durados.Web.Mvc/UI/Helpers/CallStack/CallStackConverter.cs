using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Durados.Web.Mvc.UI.Helpers.CallStack
{
    public class CallStackConverter
    {
        public IList<IAction> ChronologicalListToTree(IList<IActionEvent> events)
        {
            if (events.Count() == 0)
                return null;

            IActionEvent firstEvent = events.First();
            IActionEvent lastEvent = GetEndEvent(firstEvent, events, 1);

            IAction firstAction = GetNewAction(firstEvent, lastEvent);

            IList<IAction> actions = new List<IAction>() { firstAction };

            IList<IAction> parentActions = new List<IAction>() { firstAction };

            for (int i = 1; i < events.Count(); i++ )
            {
                IActionEvent actionEvent = events[i];

                IAction parentAction = parentActions.LastOrDefault();
                
                if (actionEvent == lastEvent)
                {
                    if (parentAction != null)
                        parentActions.Remove(parentAction);
                    continue;
                }

                if (parentAction == null)
                {
                    if (!actionEvent.Event.Equals(Event.Started))
                    {
                        throw new Exception();
                    }

                    firstEvent = actionEvent;
                    lastEvent = GetEndEvent(firstEvent, events, i);

                    IAction nextSibling = GetNewAction(firstEvent, lastEvent);

                    actions.Add(nextSibling);

                    parentActions = new List<IAction>() { nextSibling };

                    continue;
                }

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


            return actions;
        }

        private IActionEvent GetEndEvent(IActionEvent starttEvent, IList<IActionEvent> events, int start)
        {
            for (int i = start; i < events.Count(); i++)
            {
                IActionEvent actionEvent = events[i];
                if (actionEvent.Id.Equals(starttEvent.Id))
                {
                    return actionEvent;
                }
            }

            return null;
        }

        private IAction GetNewAction(IActionEvent started, IActionEvent ended = null)
        {
            return new Action(started.ObjectName, started.ActionName, started.Id, started, ended);
        }

        
       
    }
}
