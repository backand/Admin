using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Durados.Web.Mvc.Specifics.Projects.CRMShade.BusinessLogic.Json
{
    [DataContract]
    public class VendorInfo
    {
        [DataMember]
        public double Multiply { get; set; }
    }
}
