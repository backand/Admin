using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Durados.Web.Mvc.App.Views.Shared.Controls
{
    public class AdminMenuModel
    {
        public string MenuName { get; set; }
        public IEnumerable<Durados.Web.Mvc.View> Views { get; set; }
        public bool Expand { get; set; }
    }
}
