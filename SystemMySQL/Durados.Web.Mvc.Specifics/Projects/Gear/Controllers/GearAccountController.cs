using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.Mvc;
using System.Data;
using System.IO;

using Durados;
using Durados.Web.Membership;
using Durados.Web.Mvc.UI.Helpers;

namespace Durados.Web.Mvc.App.Controllers
{
    [HandleError]
    public class GearAccountController : GearBaseController
    {
        public GearAccountController()
            : this(null, null)
        {
        }

        // This constructor is not used by the MVC framework but is instead provided for ease
        // of unit testing this type. See the comments at the end of this file for more
        // information.
        public GearAccountController(IFormsAuthentication formsAuth, IMembershipService service)
        {
            FormsAuth = formsAuth ?? new FormsAuthenticationService();
            MembershipService = service ?? new AccountMembershipService();
        }

        public IFormsAuthentication FormsAuth
        {
            get;
            private set;
        }

        public IMembershipService MembershipService
        {
            get;
            private set;
        }


        private void SendAuthenticationEmail(string email, string firstName, string lastName)
        {
            string htmlName = "AuthenticationEmail";

            string token = MembershipService.GetUserToken(email);

            string link = string.Format("http://{0}" + Request.ApplicationPath + "/GearAccount/EmailAuthentication?token={1}", System.Web.HttpContext.Current.Request.Url.Authority, token);
            string message = (new Durados.Cms.Letter()).GetMessage(htmlName, new object[3] { firstName, lastName, link });

            string from = Convert.ToString(System.Configuration.ConfigurationSettings.AppSettings["from"]);
            string subject = Convert.ToString(System.Configuration.ConfigurationSettings.AppSettings["subject"]);
            string fromNick = Convert.ToString(System.Configuration.ConfigurationSettings.AppSettings["fromNick"]);

            string host = Convert.ToString(System.Configuration.ConfigurationSettings.AppSettings["host"]);
            int port = Convert.ToInt32(System.Configuration.ConfigurationSettings.AppSettings["port"]);
            string username = Convert.ToString(System.Configuration.ConfigurationSettings.AppSettings["username"]);
            string password = Convert.ToString(System.Configuration.ConfigurationSettings.AppSettings["password"]);
            //string from = Gear.Models.Config.Email.Authentication.From;
            //string subject = Gear.Models.Config.Email.Authentication.Subject;
            //string fromNick = Gear.Models.Config.Email.Authentication.FromNick;
            //string host = Gear.Models.Config.Email.Host;
            //int port = Gear.Models.Config.Email.Port;
            //string username = Gear.Models.Config.Email.Username;
            //string password = Gear.Models.Config.Email.Password;

            Durados.Cms.DataAccess.Email.Send(host, false, port, username, password, false, new string[1] { email }, new string[0] { }, new string[1] { from }, subject, message, from, fromNick, null); 
        }

        //private string GetUserToken(string email)
        //{
        //    return Membership.Provider.GetUser(email, false).ProviderUserKey.ToString();
        //}

        //private MembershipCreateStatus CreateUser(string userName, string password, string email)
        //{
        //    MembershipCreateStatus status;
        //    Membership.Provider.CreateUser(userName, password, email, null, null, false, null, out status);
        //    return status;
        //}

        //private string ConvertToString(object[] array)
        //{
        //    string s = string.Empty;

        //    foreach (object o in array)
        //    {
        //        s += o.ToString() + ",";
        //    }

        //    return s;
        //}

        protected override void AfterCreateBeforeCommit(CreateEventArgs e)
        {
            //System.Diagnostics.EventLog.WriteEntry("Gear", e.Values.Values.ToArray().ToString());
            //System.Threading.Thread.Sleep(5000);
            //System.Diagnostics.EventLog.WriteEntry("Gear", ConvertToString(e.Values.Values.ToArray()));
            string username = e.Values["Email"].ToString();
            string email = e.Values["Email"].ToString();
            string password = e.Values["Password"].ToString();
            string firstName = e.Values["FirstName"].ToString();
            string lastName = e.Values["LastName"].ToString();
            System.Web.Security.MembershipCreateStatus status = MembershipService.CreateUser(username, password, email, false);

            if (status == System.Web.Security.MembershipCreateStatus.Success)
                System.Web.Security.Roles.AddUserToRole(username, "User");
            else
                e.Cancel = true;
            base.AfterCreateBeforeCommit(e);

            if (e.Cancel)
                throw new Exception(status.ToString());
            else
                SendAuthenticationEmail(email, firstName, lastName);
        }

        //protected override void AfterEditBeforeCommit(EditEventArgs e)
        //{
        //    string username = e.Values["Email"].ToString();
        //    System.Web.Security.MembershipUser currentUser = System.Web.Security.Membership.Provider.GetUser(username, false /* userIsOnline */);
        //    string email = e.Values["Email"].ToString();

        //    currentUser.Email = email;
        //    System.Web.Security.Membership.Provider.UpdateUser(currentUser);
        //    base.AfterEditBeforeCommit(e);
        //}

        protected override void BeforeDelete(DeleteEventArgs e)
        {

            string username = Durados.Web.Mvc.Specifics.Gear.DataAccess.User.GetUsername(Convert.ToInt32(e.PrimaryKey));
            e.Cancel = !MembershipService.DeleteUser(username);
            base.BeforeDelete(e);

            if (e.Cancel)
                throw new Exception("Failed to delete user");
        }

        protected override string FormatExceptionMessage(string viewName, string action, Exception exception)
        {
            if (exception.Message.Contains("already in role") || exception.Message.Contains("IX_gear_User_Email"))
                return "EmailAlreadyExists";
            else
                return base.FormatExceptionMessage(viewName, action, exception);
        }

        public ActionResult Register()
        {
            return View();
        }

        public ActionResult EmailAuthentication(string token)
        {
            bool isUserExist = MembershipService.IsUserExist(token);
            bool isUserApproved = MembershipService.IsUserApproved(token);

            string pageName;

            if (isUserExist && !isUserApproved)
            {
                pageName = "AuthenticationSuccess";
                MembershipService.ApproveUser(token);
                FormsAuth.SignIn(MembershipService.GetUserName(token), false /* createPersistentCookie */);
            }
            else
                pageName = "AuthenticationFailure";

            return RedirectToAction("Page", "Cms", new { pageName = pageName });
        }

        public ActionResult LogOff()
        {

            FormsAuth.SignOut();
            
            string returnUrl = Request.UrlReferrer.ToString();

            return Redirect(returnUrl);
        }

        public ActionResult LogOn()
        {
            string pageName = "LogOn";

            return RedirectToAction("Page", "Cms", new { pageName = pageName });
        }

        [AcceptVerbs(HttpVerbs.Post)]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1054:UriParametersShouldNotBeStrings",
            Justification = "Needs to take same parameter type as Controller.Redirect()")]
        public ActionResult LogOn(string userName, string password, bool rememberMe, string returnUrl)
        {

            if (!ValidateLogOn(userName, password))
            {
                return RedirectToAction("LogOn");
            }

            FormsAuth.SignIn(userName, rememberMe);

            if (!String.IsNullOrEmpty(returnUrl))
            {
                return Redirect(returnUrl);
            }
            else
            {
                return RedirectToAction("Page", "Catalog");
            }
        }

        private bool ValidateLogOn(string userName, string password)
        {
            if (String.IsNullOrEmpty(userName))
            {
                ModelState.AddModelError("username", "You must specify a username.");
            }
            if (String.IsNullOrEmpty(password))
            {
                ModelState.AddModelError("password", "You must specify a password.");
            }
            if (!MembershipService.ValidateUser(userName, password))
            {
                ModelState.AddModelError("_FORM", "The username or password provided is incorrect.");
            }

            return ModelState.IsValid;
        }

        //public ActionResult EmailAuthentication(string token)
        //{
        //    bool isUserExist = MembershipService.IsUserExist(token);
        //    bool isUserApproved = MembershipService.IsUserApproved(token);

        //    if (!isUserExist || (isUserExist && isUserApproved))
        //    {
        //        return RedirectToAction("Index", "Home");
        //    }

        //    ViewData["token"] = token;
        //    return View();
        //}

        //[AcceptVerbs(HttpVerbs.Post)]
        //public ActionResult EmailAuthentication(string token, bool approve)
        //{
        //    bool isUserExist = MembershipService.IsUserExist(token);
        //    bool isUserApproved = MembershipService.IsUserApproved(token);

        //    if (isUserExist && !isUserApproved && approve)
        //    {
        //        MembershipService.ApproveUser(token);
        //        FormsAuth.SignIn(MembershipService.GetUserName(token), false /* createPersistentCookie */);
        //    }
        //    return RedirectToAction("Index", "Home");
        //}

        //private bool IsUserExist(string token)
        //{
        //    return IsUserExist(new Guid(token));
        //}

        //private bool IsUserExist(Guid providerUserKey)
        //{
        //    return Membership.Provider.GetUser(providerUserKey, false) != null;
        //}

        //private bool IsUserApproved(string token)
        //{
        //    return IsUserApproved(new Guid(token));
        //}

        //private bool IsUserApproved(Guid providerUserKey)
        //{
        //    MembershipUser user = Membership.Provider.GetUser(providerUserKey, false);
        //    if (user == null)
        //    {
        //        return false;
        //    }
        //    return user.IsApproved;
        //}

        //public void ApproveUser(string token)
        //{
        //    ApproveUser(new Guid(token));
        //}

        //public void ApproveUser(Guid providerUserKey)
        //{
        //    MembershipUser user = Membership.Provider.GetUser(providerUserKey, false);
        //    if (user != null)
        //    {
        //        user.IsApproved = true;
        //        Membership.Provider.UpdateUser(user);
        //    }
        //}

        //public string GetEmail(string token)
        //{
        //    return GetEmail(new Guid(token));
        //}

        //public string GetEmail(Guid providerUserKey)
        //{
        //    return Membership.Provider.GetUser(providerUserKey, false).UserName;
        //}
    }

    //public interface IFormsAuthentication
    //{
    //    void SignIn(string userName, bool createPersistentCookie);
    //    void SignOut();
    //}

    //public class FormsAuthenticationService : IFormsAuthentication
    //{
    //    public void SignIn(string userName, bool createPersistentCookie)
    //    {
    //        FormsAuthentication.SetAuthCookie(userName, createPersistentCookie);
    //    }
    //    public void SignOut()
    //    {
    //        FormsAuthentication.SignOut();
    //    }
    //}


}
