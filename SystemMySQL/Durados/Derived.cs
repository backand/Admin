using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Durados
{
    public class Derived
    {
        [Durados.Config.Attributes.ColumnProperty()]
        public string Value { get; set; }

        [Durados.Config.Attributes.ColumnProperty()]
        public string Fields { get; set; }

        protected Derivation derivation;
        //[Durados.Config.Attributes.ParentProperty(TableName = "Derivation")]
        public Derivation Derivation
        {
            get
            {
                return derivation;
            }
            set
            {
                if (derivation == value)
                    return;

                if (derivation != null)
                    derivation.Deriveds.Remove(this);

                derivation = value;
                if (derivation != null)
                    derivation.Deriveds.Add(this);
            }
        }

    }
}
