using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Durados.Web.Mvc.UI.Json
{
    [DataContract]
    public class Derivation
    {
        public Derivation()
        {
            DisabledFields = new Dictionary<string, object>();
            Deriveds = new List<Derived>();
        }

        [DataMember]
        public string DerivationField { get; set; }

        [DataMember]
        public List<Derived> Deriveds { get; set; }

        //[DataMember]
        //public List<string> CreateDisabledFields { get; set; }

        //[DataMember]
        //public List<string> EditDisabledFields { get; set; }

        //[DataMember]
        //public List<string> InlineAddingDisabledFields { get; set; }

        //[DataMember]
        //public List<string> InlineEditDisabledFields { get; set; }

        [DataMember]
        public Dictionary<string, object> DisabledFields { get; set; }

    }
}
