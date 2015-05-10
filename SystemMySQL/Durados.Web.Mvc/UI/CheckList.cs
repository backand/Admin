using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;

namespace Durados.Web.Mvc.UI
{
    public class CheckList
    {
        public IEnumerable<SelectListItem> SelectList { get; set; }
        public string Guid { get; set; }
        public ChildrenField ChildrenField { get; set; }
        public Durados.DataAction DataAction { get; set; }
        public string Name
        {
            get
            {
                return ID;
            }
        }
        public string ID
        {
            get
            {
                return Guid + Prefix + ChildrenField.Name;
            }
        }
        public string Prefix
        {
            get
            {
                return FieldViewer.GetDataActionPrefix(DataAction);
            }
        }
    }
}
