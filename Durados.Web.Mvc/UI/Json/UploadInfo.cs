using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Durados.Web.Mvc.UI.Json
{
    [DataContract]
    public class UploadInfo
    {
        [DataMember]
        public string FileName { get; set; }

        [DataMember]
        public string Path { get; set; }
        
    }
}
