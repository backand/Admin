using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Durados.Services
{
    public interface ISecurable
    {
        bool IsAllow();
        string AllowSelectRoles { get; set; }
    }
}
