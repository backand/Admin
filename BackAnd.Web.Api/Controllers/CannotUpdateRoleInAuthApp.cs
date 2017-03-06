using Durados;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BackAnd.Web.Api.Controllers
{
    public class CannotUpdateRoleInAuthApp : DuradosException
    {
        public CannotUpdateRoleInAuthApp()
            : base("Cannot update role in Auth App")
        {

        }
    }
}
