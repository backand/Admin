using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Durados.Web.Mvc.UI.Json
{
    [DataContract]
    public class View
    {
        [DataMember]
        public Dictionary<string, Field> Fields { get; private set; }

        [DataMember]
        public string ViewName { get; set; }

        [DataMember]
        public string InlineAddingCreateUrl { get; set; }

        [DataMember]
        public string InlineEditingCreateUrl { get; set; }

        [DataMember]
        public Derivation Derivation { get; set; }

        public View()
        {
            Fields = new Dictionary<string, Field>();
        }
    }
}
