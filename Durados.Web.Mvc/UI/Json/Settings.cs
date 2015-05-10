using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Durados.Web.Mvc.UI.Json
{
    [DataContract]
    public class Settings
    {
        [DataMember]
        public Option[] apps { get; set; }

        [DataMember]
        public string parameters { get; set; }
        
    }
}
