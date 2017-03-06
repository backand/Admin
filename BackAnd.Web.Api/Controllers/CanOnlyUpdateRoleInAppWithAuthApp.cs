using Durados;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BackAnd.Web.Api.Controllers
{
    public class CanOnlyUpdateRoleInAppWithAuthApp : DuradosException
    {
        public CanOnlyUpdateRoleInAppWithAuthApp()
            : base("Can only update the role in app with Auth App")
        {

        }
    }
}
