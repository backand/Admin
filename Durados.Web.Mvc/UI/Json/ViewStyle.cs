using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Durados.Web.Mvc.UI.Json
{

    [DataContract]
    public class ViewStyle
    {
        [DataMember]
        public Option[] views { get; set; }
        [DataMember]
        public Option[] styles { get; set; }
    }
}
