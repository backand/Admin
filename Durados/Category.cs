using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Durados
{
    public class Category
    {
        [Durados.Config.Attributes.ColumnProperty()]
        public string Name { get; set; }
        
        [Durados.Config.Attributes.ColumnProperty()]
        public int Ordinal { get; set; }

        
        public List<Field> Fields { get; private set; }


        public Category()
        {
            Fields = new List<Field>();
        }
    }
}
