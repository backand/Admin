using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Durados.Web.Mvc.UI.Helpers
{
    public class ActionNotFoundException : DuradosException
    {
        public ActionNotFoundException(int actionId, int? viewId = null) : base("The action was not found actionId: " + actionId + (viewId.HasValue ? " viewId: " + viewId : string.Empty))
        {

        }
    }
}
