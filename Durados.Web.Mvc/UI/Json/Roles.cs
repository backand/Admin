using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Durados.Web.Mvc.UI.Json
{
    [DataContract]
    public class Roles
    {
        
        #region Allow
        [DataMember]
        public string AllowCreateRoles { get; set; }
        [DataMember]
        public string AllowEditRoles { get; set; }
        [DataMember]
        public string AllowSelectRoles { get; set; }
        [DataMember]
        public string AllowDeleteRoles { get; set; }
        #endregion

        #region Deny
        [DataMember]
        public string DenyCreateRoles { get; set; }
        [DataMember]
        public string DenyEditRoles { get; set; }
        [DataMember]
        public string DenyDeleteRoles { get; set; }
        [DataMember]
        public string DenySelectRoles { get; set; }
        #endregion

    }
}
