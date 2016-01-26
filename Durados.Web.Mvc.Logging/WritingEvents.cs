using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Durados.Web.Mvc.Logging
{
    public class WritingEvents
    {
        public bool BeforeAction { get; private set; }
        public bool AfterAction { get; private set; }
        public bool BeforeResult { get; private set; }
        public bool AfterResult { get; private set; }

        public WritingEvents(bool beforeAction, bool afterAction, bool beforeResult, bool afterResult)
        {
            this.BeforeAction = beforeAction;
            this.AfterAction = afterAction;
            this.BeforeResult = beforeResult;
            this.AfterResult = afterResult;
        }
    }
}
