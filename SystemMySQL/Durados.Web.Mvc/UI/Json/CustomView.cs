using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Durados.Web.Mvc.UI.Json
{
    [DataContract]
    public class CustomView
    {
        public CustomView() 
        {

        }

        [DataMember]
        public Dictionary<string, CustomField> Fields { get; set; }

        //[DataMember]
        //public string ViewName { get; set; }

    }

    [DataContract]
    public class CustomField
    {
        [DataMember]
        public string width { get; set; }
    }

}
