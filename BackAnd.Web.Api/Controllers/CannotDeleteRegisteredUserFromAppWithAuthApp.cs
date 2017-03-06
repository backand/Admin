using Durados;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BackAnd.Web.Api.Controllers
{
    public class CannotDeleteRegisteredUserFromAppWithAuthApp : DuradosException
    {
        public CannotDeleteRegisteredUserFromAppWithAuthApp()
            : base("Cannot delete registered user from app with Auth App")
        {
                
        }
    }
}
