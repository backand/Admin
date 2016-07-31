using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Durados.Web.Mvc.UI.Json
{
    [DataContract]
    public class AdvancedFilter<T>
    {
        protected EquallityOperator equallityOperator;
        protected string equallityOperatorString;

        [DataMember]
        public T First { get; set; }
        [DataMember]
        public T Second { get; set; }

        public EquallityOperator EquallityOperator
        {
            get
            {
                return equallityOperator;
            }
            set
            {
                equallityOperator = value;
                equallityOperatorString = value.ToString();
            }
        }

        [DataMember]
        public string EquallityOperatorString
        {
            get
            {
                return equallityOperatorString;
            }
            set
            {
                equallityOperatorString = value;
                equallityOperator = (EquallityOperator) Enum.Parse(typeof(EquallityOperator), value);
            }
        }

        public AdvancedFilterType AdvancedFilterType
        {
            get
            {
                if (typeof(T).Equals(typeof(double)))
                    return AdvancedFilterType.NumericFilter;
                else if (typeof(T).Equals(typeof(DateTime)))
                    return AdvancedFilterType.DateFilter;
                else
                    throw new NotSupportedException();
            }
        }
    }

    public enum EquallityOperator
    {
        Equal,
        Greater,
        Lesser,
        GreaterAndEqual,
        LesserAndEqual,
        Between
    }

    public enum AdvancedFilterType
    {
        NumericFilter,
        DateFilter,
        TextFilter,
        Multi
    }
}
