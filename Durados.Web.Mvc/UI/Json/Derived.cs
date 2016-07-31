using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Durados.Web.Mvc.UI.Json
{
    [DataContract]
    public class Derived
    {
        public Derived()
        {
            Fields = new Dictionary<string, bool>();
        }

        [DataMember]
        public string Value { get; set; }

        [DataMember]
        public Dictionary<string, bool> Fields { get; set; }
    }
}
