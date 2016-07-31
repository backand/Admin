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

    public class MSUserController : Durados.Web.Mvc.Controllers.DuradosController
    {
        private MembershipCreateStatus CreateUser(string userName, string password, string email)
        {
            MembershipCreateStatus status;
            System.Web.Security.Membership.Provider.CreateUser(userName, password, email, null, null, true, null, out status);
            return status;
        }

        //protected override void BeforeCreate(CreateEventArgs e)
        //{
        //    string tempPassword;
        //    if (e.Values.ContainsKey("Password"))
        //    {
        //        tempPassword = e.Values["Password"].ToString();
        //    }
        //    else
        //    {
        //        tempPassword = GetRandomPassword(8);
        //        e.Values.Add("Password", tempPassword);
        //    }
        //    base.BeforeCreate(e);
        //}
        string Developer = "Developer";
            
        protected override void BeforeCreate(CreateEventArgs e)
        {
            string role = e.Values["Role"].ToString();
            if (role.Equals(Developer))
            {
                string currentUserRole = ((Database)e.View.Database).GetUserRole();
                if (!currentUserRole.Equals(Developer))
                    throw new DuradosException("Only a Developer can create a user with a Developer role.");
            }

            if (e.View.Fields.ContainsKey("IsApproved"))
            {
                if (e.Values.ContainsKey("IsApproved"))
                {
                    bool isApproved = Convert.ToBoolean(e.Values["IsApproved"].ToString());

                    if (!isApproved)
                    {
                        if (e.Values.ContainsKey("NewUser"))
                        {
                            e.Values["NewUser"] = true;
                        }
                        else
                        {
                            e.Values.Add("NewUser", true);
                        }
                    }
                }
                else
                {
                    if (e.Values.ContainsKey("NewUser"))
                    {
                        e.Values["NewUser"] = false;
                    }
                    else
                    {
                        e.Values.Add("NewUser", false);
                    }
                }
            }

            base.BeforeCreate(e);
        }

        protected override void AfterCreateBeforeCommit(CreateEventArgs e)
        {
            string username = e.Values["Username"].ToString();
            string email = e.Values["Email"].ToString();
            
            //string tempPassword = e.Values["Password"].ToString();
            string tempPassword;
            if (e.Values.ContainsKey("Password"))
            {
                tempPassword = e.Values["Password"].ToString();
            }
            else
            {
                tempPassword = GetRandomPassword((View)e.View);
                //e.Values.Add("Password", tempPassword);
            } 
            System.Web.Security.MembershipCreateStatus status = CreateUser(username, tempPassword, email);

            if (status == MembershipCreateStatus.Success)
            {

                string role = GetRole(e.Values);
                if (String.IsNullOrEmpty(role))
                {
                    System.Web.Security.Membership.Provider.DeleteUser(username, true);
                    throw new DuradosException(Map.Database.Localizer.Translate("Failed to create user, Role is missing"));
                }
                System.Web.Security.Roles.AddUserToRole(username, role);

                if (e.Values.ContainsKey("IsApproved"))
                {
                    bool isApproved = Convert.ToBoolean(e.Values["IsApproved"].ToString());

                    if (!isApproved)
                    {
                        System.Web.Security.MembershipUser user = System.Web.Security.Membership.Provider.GetUser(username, true);
                        user.IsApproved = false;
                        System.Web.Security.Membership.UpdateUser(user);
                    }
                }

                base.AfterCreateBeforeCommit(e);
            }
            else
            {
                e.Cancel = true;
            }

            if (e.Cancel)
                throw new DuradosException(ErrorCodeToString(status));
                
        }

        private static string ErrorCodeToString(MembershipCreateStatus createStatus)
        {
            // See http://msdn.microsoft.com/en-us/library/system.web.security.membershipcreatestatus.aspx for
            // a full list of status codes.
            switch (createStatus)
            {
                case MembershipCreateStatus.DuplicateUserName:
                    return "Username already exists. Please enter a different user name.";

                case MembershipCreateStatus.DuplicateEmail:
                    return "A username for that e-mail address already exists. Please enter a different e-mail address.";

                case MembershipCreateStatus.InvalidPassword:
                    return "The password provided is invalid. Please enter a valid password value.";

                case MembershipCreateStatus.InvalidEmail:
                    return "The e-mail address provided is invalid. Please check the value and try again.";

                case MembershipCreateStatus.InvalidAnswer:
                    return "The password retrieval answer provided is invalid. Please check the value and try again.";

                case MembershipCreateStatus.InvalidQuestion:
                    return "The password retrieval question provided is invalid. Please check the value and try again.";

                case MembershipCreateStatus.InvalidUserName:
                    return "The user name provided is invalid. Please check the value and try again.";

                case MembershipCreateStatus.ProviderError:
                    return "The authentication provider returned an error. Please verify your entry and try again. If the problem persists, please contact your system administrator.";

                case MembershipCreateStatus.UserRejected:
                    return "The user creation request has been canceled. Please verify your entry and try again. If the problem persists, please contact your system administrator.";

                default:
                    return "An unknown error occurred. Please verify your entry and try again. If the problem persists, please contact your system administrator.";
            }
        }

        List<string> importedUsers = null;

        protected override void ImportInit()
        {
            importedUsers = new List<string>();
        }

        protected override void ImportTerm(bool cancelCommit)
        {
            if (cancelCommit)
            {
                foreach (string username in importedUsers)
                {
                    System.Web.Security.Membership.Provider.DeleteUser(username, true);
                }
            }
        }

        protected override string ImportInsert(View view, Dictionary<string, object> values, System.Data.IDbCommand command, string Comments, int UserId, Durados.DataAccess.Importer importer, System.Data.DataRow row)
        {
            CreateEventArgs createEventArgs = new CreateEventArgs(view, values, null, command, null);

            AfterCreateBeforeCommit(createEventArgs);

            string username = values["Username"].ToString();
            importedUsers.Add(username);
            string displayNameValue = values["FirstName"] + " " + values["LastName"];
            //return importer.Create(view, values, displayNameValue, ImportModes.Insert);
            return importer.Create(view, values, row, ImportModes.Insert);

        }

        protected override bool IsSpecificImport()
        {
            return true;
        }

        private string GetRandomPassword(View view)
        {
            if (view.Fields.ContainsKey("Password"))
            {
                return view.Fields["Password"].DefaultValue.ToString();
            }
            else
            {
                return GetRandomPassword(8);
            }
        }

        private string GetRandomPassword(int chars)
        {
            string s = string.Empty;

            Random r = new Random(DateTime.Now.Millisecond);

            for (int i = 0; i < chars; i++)
            {
                s += r.Next(0, 9).ToString();
            }

            return s;
        }

        private string GetRole(Dictionary<string,object> values)
        {
            if (values.ContainsKey("Role"))
                return values.ContainsKey("Role").ToString();

            foreach (string key in values.Keys)
            {
                if (key.IndexOf("UserRole") >= 0)
                    return values[key].ToString();
            }

            return string.Empty;
        }


        protected override void AfterEditBeforeCommit(EditEventArgs e)
        {
            string usernameFieldName = GetUsernameFieldName();
            if (e.Values.ContainsKey(usernameFieldName) && e.Values.ContainsKey("Email"))
            {
                string username = e.Values["Username"].ToString();
                System.Web.Security.MembershipUser currentUser = System.Web.Security.Membership.Provider.GetUser(username, false /* userIsOnline */);
                string email = e.Values["Email"].ToString();

                currentUser.Email = email;
                System.Web.Security.Membership.Provider.UpdateUser(currentUser);

                ChangePassword(e);
            }

            if (e.Values.ContainsKey("IsApproved"))
            {
                if (e.PrevRow != null)
                {
                    Field field = e.View.Fields["IsApproved"];
                    bool isApproved = Convert.ToBoolean(e.Values["IsApproved"].ToString());
                    bool prevIsApproved = (bool)e.PrevRow["IsApproved"];

                    if (isApproved != prevIsApproved)
                    {
                        string username = e.PrevRow["Username"].ToString();
                        System.Web.Security.MembershipUser user = System.Web.Security.Membership.Provider.GetUser(username, true);
                        user.IsApproved = isApproved;
                        System.Web.Security.Membership.UpdateUser(user);

                        
                    }
                }
            }
            base.AfterEditBeforeCommit(e);
        }

        protected override void AfterEditAfterCommit(EditEventArgs e)
        {
            if (e.Values.ContainsKey("IsApproved"))
            {
                if (e.PrevRow != null)
                {
                    Field field = e.View.Fields["IsApproved"];
                    bool isApproved = Convert.ToBoolean(e.Values["IsApproved"].ToString());
                    bool prevIsApproved = (bool)e.PrevRow["IsApproved"];

                    if (isApproved != prevIsApproved)
                    {
                        if (isApproved)
                        {
                            if (e.PrevRow["NewUser"].Equals(true))
                            {
                                string firstName =string.Empty;
                                if (e.Values.ContainsKey("FirstName"))
                                    firstName = e.Values["FirstName"].ToString();
                                else
                                    firstName = e.PrevRow["FirstName"].ToString();

                                string lastName =string.Empty;
                                if (e.Values.ContainsKey("LastName"))
                                    lastName = e.Values["LastName"].ToString();
                                else
                                    lastName = e.PrevRow["LastName"].ToString();

                                string email =string.Empty;
                                if (e.Values.ContainsKey("Email"))
                                    email = e.Values["Email"].ToString();
                                else
                                    email = e.PrevRow["Email"].ToString();

                                SendRegistrationApproval(email, firstName, lastName);
                            }
                        }
                    }
                }
            }
            base.AfterEditAfterCommit(e);
        }

        protected override void BeforeDelete(DeleteEventArgs e)
        {
            string username = Durados.Web.Mvc.Specifics.Projects.User.GetUsername(Convert.ToInt32(e.PrimaryKey));
            e.Cancel = !System.Web.Security.Membership.Provider.DeleteUser(username, true);
            base.BeforeDelete(e);

            if (e.Cancel)
                throw new DuradosException(Map.Database.Localizer.Translate("Failed to delete user"));
        }

        string oldPassword = null;
        protected override void BeforeEdit(EditEventArgs e)
        {
            string role = e.Values["Role"].ToString();
            string prevRole = string.Empty;
            if (e.PrevRow != null && !e.PrevRow.IsNull("Role"))
                e.PrevRow["Role"].ToString();
            if (role.Equals(Developer) || prevRole.Equals(Developer))
            {
                string currentUserRole = ((Database)e.View.Database).GetUserRole();
                if (!currentUserRole.Equals(Developer))
                    throw new DuradosException("Only a Developer can update a user with a Developer role.");
            }

            string usernameFieldName = GetUsernameFieldName();
            if (e.Values.ContainsKey(usernameFieldName))
            {
                string username = e.Values[usernameFieldName].ToString();
                System.Data.DataRow dataRow = GetUserRowByUsername(username);
                string passwordFieldName = GetPasswordFieldName();
                if (e.Values.ContainsKey(passwordFieldName))
                {
                    oldPassword = dataRow[passwordFieldName].ToString();
                }
            }

            Field field = e.View.Fields["IsApproved"];
            if (e.Values.ContainsKey("IsApproved"))
            {
                bool isApproved = Convert.ToBoolean(e.Values["IsApproved"].ToString());
                if (isApproved)
                {
                    if (!e.Values.Keys.Contains("NewUser"))
                    {
                        e.Values.Add("NewUser", false);
                    }
                    else
                    {
                        e.Values["NewUser"] = false;
                    }
                }
            }
            base.BeforeEdit(e);
        }


       

        

        protected virtual void SendRegistrationApproval(string email, string firstName, string lastName)
        {
            string host = Convert.ToString(System.Configuration.ConfigurationManager.AppSettings["host"]);
            int port = Convert.ToInt32(System.Configuration.ConfigurationManager.AppSettings["port"]);
            string username = Convert.ToString(System.Configuration.ConfigurationManager.AppSettings["username"]);
            string password = Convert.ToString(System.Configuration.ConfigurationManager.AppSettings["password"]);

            string from = Convert.ToString(System.Configuration.ConfigurationManager.AppSettings["fromAlert"]);
            string subject = "Allegro Registration Approval";
            string siteWithoutQueryString = System.Web.HttpContext.Current.Request.Url.Scheme + "://" + System.Web.HttpContext.Current.Request.Url.Authority;

            string message = CmsHelper.GetHtml("Registration Approval").Replace("[FirstName]", firstName).Replace("[LastName]", lastName).Replace("[AppPath]", siteWithoutQueryString);

            string to = email;

            Durados.Cms.DataAccess.Email.Send(host, false, port, username, password, false, to.Split(';'), null, null, subject, message, from, null, null, DontSend, null);
        }

        protected virtual void SendRegistrationRequest(FormCollection collection)
        {
            string host = Convert.ToString(System.Configuration.ConfigurationManager.AppSettings["host"]);
            int port = Convert.ToInt32(System.Configuration.ConfigurationManager.AppSettings["port"]);
            string username = Convert.ToString(System.Configuration.ConfigurationManager.AppSettings["username"]);
            string password = Convert.ToString(System.Configuration.ConfigurationManager.AppSettings["password"]);

            string from = Convert.ToString(System.Configuration.ConfigurationManager.AppSettings["fromAlert"]);
            string subject = collection["Subject"];
            string message = string.Empty;
            foreach (string key in collection.AllKeys)
            {
                if (!(key == "Subject" || key == "Comments" || key == "to" || key == "cc"))
                    message += key.Replace("_", " ") + ":" + " " + collection[key] + "<br>";
            }
            message += collection["Comments"];
            string to = collection["to"];
            string cc = collection["cc"];

            Durados.Cms.DataAccess.Email.Send(host, false, port, username, password, false, to.Split(';'), cc.Split(';'), null, subject, message, from, null, null, DontSend, null);

        }


        protected virtual void ChangePassword(EditEventArgs e) 
        {
            string usernameFieldName = GetUsernameFieldName();
            if (e.Values.ContainsKey(usernameFieldName))
            {
                string username = e.Values[usernameFieldName].ToString();

                string passwordFieldName = GetPasswordFieldName();
                if (e.Values.ContainsKey(passwordFieldName))
                {
                    string newPassword = e.Values[passwordFieldName].ToString();

                    if (!string.IsNullOrEmpty(oldPassword) && oldPassword != newPassword)
                    {
                        System.Web.Security.Membership.Provider.ChangePassword(username, oldPassword, newPassword);
                    }
                }
            }
        }
    }
}
