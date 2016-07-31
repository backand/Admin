using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Durados.Web.Mvc.Specifics.Shade.CRM.BusinessLogic.Json
{
    [DataContract]
    public class Price
    {
        [DataMember]
        public double Width { get; set; }
        [DataMember]
        public double Height { get; set; }
        [DataMember]
        public double Cost { get; set; }
        [DataMember]
        public double PPrice { get; set; }
        [DataMember]
        public bool Seamed { get; set; }
    }
}
