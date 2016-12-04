using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Durados.DataAccess
{
    internal class AfterCommitDispatcher
    {
        AfterEditEventHandler afterEditAfterCommitCallback;
        AfterCreateEventHandler afterCreateAfterCommitCallback;
        AfterDeleteEventHandler afterDeleteAfterCommitCallback;

        List<DataActionEventArgs> dataActionEventArgsList;

        internal AfterCommitDispatcher(AfterEditEventHandler afterEditAfterCommitCallback, AfterCreateEventHandler afterCreateAfterCommitCallback, AfterDeleteEventHandler afterDeleteAfterCommitCallback)
        {
            this.afterCreateAfterCommitCallback = afterCreateAfterCommitCallback;
            this.afterEditAfterCommitCallback = afterEditAfterCommitCallback;
            this.afterDeleteAfterCommitCallback = afterDeleteAfterCommitCallback;
            this.dataActionEventArgsList = new List<DataActionEventArgs>();
        }

        protected internal virtual void AfterEditAfterCommitCallback(object sender, EditEventArgs e)
        {
            dataActionEventArgsList.Add(e);
        }

        protected internal virtual void AfterCreateAfterCommitCallback(object sender, CreateEventArgs e)
        {
            dataActionEventArgsList.Add(e);
        }

        protected internal virtual void AfterDeleteAfterCommitCallback(object sender, DeleteEventArgs e)
        {
            dataActionEventArgsList.Add(e);
        }

        internal void Dispatch()
        {
            foreach (DataActionEventArgs e in dataActionEventArgsList)
            {
                if (e is CreateEventArgs)
                {
                    if (afterCreateAfterCommitCallback != null)
                    {
                        afterCreateAfterCommitCallback(this, (CreateEventArgs)e);
                    }
                }
                else if (e is EditEventArgs)
                {
                    if (afterEditAfterCommitCallback != null)
                    {
                        afterEditAfterCommitCallback(this, (EditEventArgs)e);
                    }
                }
                else if (e is DeleteEventArgs)
                {
                    if (afterDeleteAfterCommitCallback != null)
                    {
                        afterDeleteAfterCommitCallback(this, (DeleteEventArgs)e);
                    }
                }
            }
        }
    }
}
