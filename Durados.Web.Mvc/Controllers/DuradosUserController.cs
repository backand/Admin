using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Durados.Web.Mvc.Controllers
{
    public class DuradosUserController:CrmController
    {
        protected override void BeforeEdit(EditEventArgs e)
        {
            SetEmailInValues(e);
            base.BeforeEdit(e);
        }

       
        protected override void BeforeCreate(CreateEventArgs e)
        {
            SetEmailInValues(e);
            base.BeforeCreate(e);
        }
        private void SetEmailInValues(DataActionEventArgs e)
        {
            if (!e.Values.ContainsKey("Email") && e.Values.ContainsKey("Username"))
            {
                e.Values.Add("Email", e.Values["Username"]);
            }
        }
    }
}
