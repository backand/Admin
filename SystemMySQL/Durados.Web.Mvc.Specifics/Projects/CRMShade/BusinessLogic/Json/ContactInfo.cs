using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Durados.Web.Mvc.Specifics.Projects.CRMShade.BusinessLogic.Json
{
    [DataContract]
    public class ContactInfo
    {
        [DataMember]
        public string ClientName { get; set; }

        [DataMember]
        public string ClientPhone { get; set; }

        [DataMember]
        public string ClientCellular { get; set; }

        [DataMember]
        public string ClientEmail { get; set; }

        [DataMember]
        public string AddressID { get; set; }

        [DataMember]
        public string AddressText { get; set; }


    }
}
