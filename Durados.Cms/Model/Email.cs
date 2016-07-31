using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Durados.Cms.Model
{
    public class Email
    {
        public string To { get; set; }
        public string Cc { get; set; }
        public string BCc { get; set; }
        public string Body { get; set; }
        public string Subject { get; set; }
    }
}
