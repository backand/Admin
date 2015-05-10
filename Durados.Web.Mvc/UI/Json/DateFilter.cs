using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Durados.Web.Mvc.UI.Json
{
    [DataContract]
    public abstract class DateFilter<T> : AdvancedFilter<T> where T : DateTime
    {
        
    }
}
