using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Durados
{
    public class UrlLink
    {
        public UrlLink()
        {
        }

        [Durados.Config.Attributes.ColumnProperty()]
        public string Title { get; set; }

        [Durados.Config.Attributes.ColumnProperty()]
        public string Url { get; set; }

        [Durados.Config.Attributes.ColumnProperty()]
        public int Order { get; set; }

        [Durados.Config.Attributes.ColumnProperty(Description="The target of the page. Empty by default. If empty the page will be open in the current window.")]
        public string Target { get; set; }

        //[Durados.Config.Attributes.ParentProperty(TableName = "Menu", DoNotCopy = true)]
        //public Menu Menu { get; set; }
    }
}
