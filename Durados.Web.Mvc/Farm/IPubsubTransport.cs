using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Durados.Web.Mvc.Farm
{
    public interface IFarmPubsubTransport
    {
        string SubscriberID { get; }
       
        event EventHandler<FarmEventArgs> OnMessage;

        void PublishSync(FarmMessage message);
    }
}
