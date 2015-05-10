using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Durados.Web.Mvc.Specifics.Shade.CRM.BusinessLogic.Json
{
    [DataContract]
    public class ProductInfo
    {
        [DataMember]
        public string Description { get; set; }

        // should be order by width, height
        [DataMember]
        public Price[] Prices { get; set; }
    }
}
