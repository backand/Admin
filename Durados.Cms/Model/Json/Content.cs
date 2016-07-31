using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Durados.Cms.Model.Json
{
    public class Content
    {
        public bool Random { get; set; }
        public int Delay { get; set; }
        public Image[] Images { get; set; }
    }
}
