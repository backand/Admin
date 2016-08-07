using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Durados.Web.Mvc.Billing
{
    public enum PaymentStatus
    {
        Active = 0,
        Locked = 1, // admin locked
        Suspended = 2,
    }
}
