using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Durados.Web.Mvc.Farm
{
    // json.net is very bad.
    // it call your ctor on every serialize
    // so we got different Id when message pass over redis.
    // this attribute tell json.net ot serialize only fields
    [JsonObject(MemberSerialization.Fields)]
    public class FarmMessage
    {
        public FarmMessage()
        {
            Id = Guid.NewGuid();
        }

        public string AppName { get; set; }

        public DateTime Time { get; set; }

        public Guid Id { get; private set; }

    }


    public class FarmMessageWrapper
    {
        public FarmMessage Message;

        public string SenderId;

    }
}
