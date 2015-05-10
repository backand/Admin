using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Durados.Web.Mvc.UI.Json
{
    [DataContract]
    public class Field
    {
        [DataMember]
        public string Name { get; set; }
        [DataMember]
        public object Value { get; set; }
        [DataMember]
        public object Default { get; set; }
        [DataMember]
        public string Type { get; set; }
        [DataMember]
        public string ValidationType { get; set; }
        [DataMember]
        public string Format { get; set; } //Used for client to server in filterJson for displayValues
        [DataMember]
        public bool Searchable { get; set; }
        [DataMember]
        public bool Permanent { get; set; }
        [DataMember]
        public bool Required { get; set; }
        [DataMember]
        public bool Disabled { get; set; }
        [DataMember]
        public bool DisDup { get; set; }
        [DataMember]
        public bool Refresh { get; set; }

        [DataMember]
        public int Min { get; set; } //for range validation

        [DataMember]
        public int Max { get; set; }
        
        [DataMember]
        public string[] DependencyChildren { get; set; }

        [DataMember]
        public string DependencyData { get; set; }        
    }
}
