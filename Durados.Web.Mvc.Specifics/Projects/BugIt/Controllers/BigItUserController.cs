using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.Mvc;
using System.Web.Mvc.Ajax;

using Durados;

namespace Durados.Web.Mvc.App.Controllers
{
    [Authorize(Roles = "Developer, Admin, User")]
    public class BigItUserController : BigItBaseController
    {
        private MembershipCreateStatus CreateUser(string userName, string password, string email)
        {
            MembershipCreateStatus status;
            System.Web.Security.Membership.Provider.CreateUser(userName, password, email, null, null, true, null, out status);
            return status;
        }

        protected override void AfterCreateBeforeCommit(CreateEventArgs e)
        {
            string username = e.Values["Username"].ToString();
            string email = e.Values["Email"].ToString();
            string tempPassword = "123456";
            System.Web.Security.MembershipCreateStatus status = CreateUser(username, tempPassword, email);
            System.Web.Security.Roles.AddUserToRole(username, "User");
                    
            if (status != System.Web.Security.MembershipCreateStatus.Success)
                e.Cancel = true;
            base.AfterCreateBeforeCommit(e);

            if (e.Cancel)
                throw new Exception(status.ToString());
        }

        protected override void AfterEditBeforeCommit(EditEventArgs e)
        {
            string username = e.Values["Username"].ToString();
            System.Web.Security.MembershipUser currentUser = System.Web.Security.Membership.Provider.GetUser(username, false /* userIsOnline */);
            string email = e.Values["Email"].ToString();
            
            currentUser.Email = email;
            System.Web.Security.Membership.Provider.UpdateUser(currentUser);
            base.AfterEditBeforeCommit(e);
        }

        protected override void BeforeDelete(DeleteEventArgs e)
        {

            string username = Durados.Web.Mvc.Specifics.DataAccess.User.GetUsername(Convert.ToInt32(e.PrimaryKey));
            e.Cancel = !System.Web.Security.Membership.Provider.DeleteUser(username, true);
            base.BeforeDelete(e);

            if (e.Cancel)
                throw new Exception("Failed to delete user");
        }
    }
}
