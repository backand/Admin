using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Durados.Web.Mvc.UI.Helpers.CallStack
{
    public class ActionTree : IActionTree
    {
        internal ActionTree(IAction root)
        {
            ActionRoot = root;
        }
        public IAction ActionRoot { get; private set; }
    }
}
