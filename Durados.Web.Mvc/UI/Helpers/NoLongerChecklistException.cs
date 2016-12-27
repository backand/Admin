using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Durados.Web.Mvc.UI.Helpers
{
    public class NoLongerChecklistException : DuradosException
    {
        public ChildrenField ChildrenField { get; private set; }
        public NoLongerChecklistException(ChildrenField childrenField) : base("The field " + childrenField.JsonName + " is no longer checklist. Please try again or call the administrator.") 
        {
            ChildrenField = childrenField;
        }
    }
}
