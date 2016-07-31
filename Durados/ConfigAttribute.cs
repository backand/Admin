using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Durados
{
    public class ConfigAttribute : Attribute
    {
        public bool Configurable {get;set;}
    }
}
