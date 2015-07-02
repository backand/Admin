using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using System.Web.Mvc.Ajax;

using Durados.Web.Mvc.Controllers;
using Durados.Web.Mvc;
using Durados.DataAccess;
using System.Web;
using System.Data;

namespace Durados.Website.Controllers
{
    [HandleError]
    //[Durados.Web.Mvc.Controllers.Attributes.DynamicAuthorizeAttribute(false, false)]
    public class WebsiteAccountController : DuradosAccountController//: Durados.Web.Mvc.Controllers.DuradosController
    {
        [AcceptVerbs(HttpVerbs.Post)]
        public JsonResult IsLoggedIn()
        {
            bool isLoggedIn = (System.Web.HttpContext.Current.User != null && System.Web.HttpContext.Current.User.Identity != null && !string.IsNullOrEmpty(System.Web.HttpContext.Current.User.Identity.Name));

            string userId = Map.Database.GetUserID();

            if (string.IsNullOrEmpty(userId))
                return Json(new { isLoggedIn = false });

            int identity = Convert.ToInt32(userId);

            return Json(new { isLoggedIn = isLoggedIn, username = Map.Database.GetUsernameById(userId),DemoDefaults = GetDefaultDemo(identity) });
        }
        [Durados.Web.Mvc.Controllers.Attributes.DynamicAuthorizeAttribute()]
        public ActionResult IsLoggedInClosed(string gid)
        {
            return Json(new { success = "OK" });
        }
        [AcceptVerbs(HttpVerbs.Post)]
        public JsonResult SignUp(string username, string password, string send, string phone, string fullname, string dbtype, string dbother)
        {
            int identity = -1;

            try
            {
                Durados.DataAccess.SqlAccess sql = new Durados.DataAccess.SqlAccess();
                Dictionary<string, object> parameters = new Dictionary<string, object>();

                string email = username.Trim();

                parameters.Add("@Email", email);
                parameters.Add("@Username", username);

                if (sql.ExecuteScalar(Map.Database.ConnectionString, "SELECT TOP 1 [Username] FROM [durados_User] WHERE [Username]=@Username", parameters) != string.Empty)
                {
                    return Json(new { Success = false, Message =string.Format("{0} is already signed up." ,username)});
                }

                string email1 = email;
                try
                {
                    email1 = email.Split('@')[0];
                }
                catch { }

                email1 = email1.ReplaceNonAlphaNumeric();
                if (string.IsNullOrEmpty(email1))
                    email1 = email;

                string[] email1arr = email1.Split('_');
                string firstName = string.Empty;
                if (email1arr.Length > 0)
                    firstName = email1arr[0];
                else
                    firstName = email;

                parameters.Add("@FirstName", firstName);
                string lastName = string.Empty;
                if (email1arr.Length > 0)
                    lastName = email1arr[email1arr.Length - 1];
                else
                    lastName = email;
                parameters.Add("@LastName", lastName);

                //Create random Password
                if (string.IsNullOrEmpty(password))
                {
                    password = new AccountMembershipService().GetRandomPassword(10);
                }
                parameters.Add("@Password", password);
                string role = "User";
                parameters.Add("@Role", role);

                Guid guid = Guid.NewGuid();
                parameters.Add("@Guid", guid);

                sql.ExecuteNonQuery(Map.Database.ConnectionString, "INSERT INTO [durados_User] ([Username],[FirstName],[LastName],[Email],[Role],[Guid]) VALUES (@Username,@FirstName,@LastName,@Email,@Role,@Guid)", parameters, CreateMembershipCallback);

                System.Web.Security.MembershipUser user = System.Web.Security.Membership.Provider.GetUser(username, true);
                if (user != null)
                {
                    if (!user.IsApproved && Maps.MultiTenancy)
                    {
                        user.IsApproved = true;
                        System.Web.Security.Membership.UpdateUser(user);

                    }
                }

                FormsAuth.SignIn(username, true);

                identity = Convert.ToInt32(Map.Database.GetUserRow(username)["Id"]);
                //CreatePendingDatabase(identity);

                bool sendEmail = false;
                sendEmail = send != null && send == "true";

                if (sendEmail)
                    Durados.Web.Mvc.UI.Helpers.Account.SendRegistrationRequest(fullname, lastName, email, guid.ToString(), username, password, Map, DontSend);

                try
                {
                    Durados.Web.Mvc.UI.Helpers.Account.UpdateWebsiteUsers(username, identity);
                }
                catch (Exception ex)
                {
                    Maps.Instance.DuradosMap.Logger.Log(RouteData.Values["Controller"].ToString(), RouteData.Values["Action"].ToString(), "SignUp", ex, 1, "failed to update websiteusercookie with userid");

                }

                //Insert into website users
                try
                {
                    Durados.Web.Mvc.UI.Helpers.Account.InsertContactUsUsers(username, fullname, null, phone, 10, int.Parse(dbtype), dbother); //10=welcome email
                }
                catch (Exception ex)
                {
                    Maps.Instance.DuradosMap.Logger.Log(RouteData.Values["Controller"].ToString(), RouteData.Values["Action"].ToString(), "SignUp", ex, 1, "failed to update websiteuser in ContactUs");

                }

            }
            catch (DuradosException exception)
            {
                Map.Logger.Log(this.ControllerContext.RouteData.Values["controller"].ToString(), this.ControllerContext.RouteData.Values["action"].ToString(), exception.Source, exception, 3, null);
                return Json(new { Success = false, Message = "The server is busy, please try again later." });
            }
            catch (Exception exception)
            {
                Map.Logger.Log(this.ControllerContext.RouteData.Values["controller"].ToString(), this.ControllerContext.RouteData.Values["action"].ToString(), exception.Source, exception, 1, null);
                //ViewData["confirmed"] = false;
                return Json(new { Success = false, Message = "The server is busy, please try again later." });
            }

            return Json(new { Success = true, Message = "Success",identity=identity, DemoDefaults = GetDefaultDemo(identity) });
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public JsonResult UpdateUserTracking(string username, string phone, string fullname, string dbtype, string dbother)
        {
            try
            {
                object cookieGuid = Durados.Web.Mvc.UI.Helpers.Account.GetTrackingCookieGuid();
                if (cookieGuid == null)
                    return Json(false);

                SqlAccess sqlAccess = new SqlAccess();
                string sql = "SELECT TOP 1 [Email],[FullName],[DBType],[Phone]  FROM [website_TrackingCookie] WHERE [Guid]=@CookieGuid ";
                Dictionary<string, object> parameters = new Dictionary<string, object>();
                parameters.Add("@CookieGuid", cookieGuid);

                DataTable trackingTable = sqlAccess.ExecuteTable(Maps.Instance.DuradosMap.connectionString, sql, parameters, CommandType.Text);

                if (trackingTable.Rows.Count <= 0)
                {
                    Durados.website.Helpers.UserTrackingHelper tracker = new website.Helpers.UserTrackingHelper();
                    tracker.Track();
                    trackingTable.Rows.Add(trackingTable.NewRow());
                }

                
                DataRow dr = trackingTable.Rows[0];

                string setSql= string.Empty;

                string prevEmail =(dr["Email"]==null)?string.Empty:dr["Email"].ToString();
                if (!string.IsNullOrEmpty(username) && string.Compare(username, prevEmail, false) != 0)
                    setSql += "[Email]='" + username + "',";

                string prevFullName = (dr["FullName"] == null) ? string.Empty : dr["FullName"].ToString();
                if (!string.IsNullOrEmpty(fullname) && string.Compare(fullname, prevFullName, false) != 0)
                    setSql += "[FullName]='" + fullname + "',";

                string prevPhone = (dr["Phone"] == null) ? string.Empty : dr["Phone"].ToString();
                if (!string.IsNullOrEmpty(phone) && string.Compare(phone, prevPhone, false) != 0)
                    setSql += "[Phone]='" + phone + "',";

                string prevDBType = (dr["DBType"] == null) ? string.Empty : dr["DBType"].ToString();
                if (!string.IsNullOrEmpty(dbtype) && string.Compare(dbtype, prevDBType, false) != 0)
                    setSql += "[DBType]='" + dbtype + "',";

                if (string.IsNullOrEmpty(setSql))
                    return Json(false);

                setSql = setSql.TrimEnd(',');

                string updateSql = string.Format("UPDATE website_TrackingCookie SET {0} WHERE  [Guid]=@CookieGuid", setSql);
                object scalar = sqlAccess.ExecuteScalar(Maps.Instance.DuradosMap.connectionString, updateSql, parameters);

                if(scalar == null)
                    return Json(false);

                return Json(true);


            }
            catch (Exception ex)
            {
                Maps.Instance.DuradosMap.Logger.Log(RouteData.Values["Controller"].ToString(), RouteData.Values["Action"].ToString(), "Sign Up -Update User Tracking", ex, 1, "failed to update user tracking table when element lost focus");
                return Json(false);
            }
        }

        

        
        //private void CreatePendingDatabase(int identity)
        //{
        //    if (Maps.DemoCreatePending)
        //    {
        //        int next = Maps.DemoPendingNext;

        //        SqlAccess sqlAccess = new SqlAccess();

        //        string server = Maps.DemoAzureServer;
        //        string catalog = Maps.DemoDatabaseName + (identity + next) + Map.PandingDatabaseSuffix;
        //        string username = Maps.DemoUsername;
        //        string password = Maps.DemoPassword;
        //        string source = Maps.DemoDatabaseName + Map.SourceSuffix;

        //        try
        //        {
        //            sqlAccess.CopyAzureDatabase(server, catalog, username, password, source);
        //        }
        //        catch (DuradosException exception)
        //        {
        //            Map.Logger.Log(this.ControllerContext.RouteData.Values["controller"].ToString(), this.ControllerContext.RouteData.Values["action"].ToString(), exception.Source, exception, 1, "Fail to copy azure database to " + catalog);
        //        }
        //    }
        //}
        public JsonResult GetConnectDefault(int identity)
        {
            return Json(new { connectDefaults = GetDefaultDemo(identity) });
        }
        private object GetDefaultDemo(int identity)
        {
            List<object> list = new List<object>();

            list.Add(GetDefaultOnPremiseDemo(identity));
            list.Add(GetDefaultAzureDemo(identity));

            return list;
        }

        private object GetDefaultAzureDemo(int identity)
        {
            string server = Maps.DemoAzureServer;
            string catalog = Maps.DemoDatabaseName + identity;
            string username = (System.Configuration.ConfigurationManager.AppSettings["newDemoUser"] ?? "user") + identity;
            string name = Maps.DemoDatabaseName + identity + "azure";
            string title = (System.Configuration.ConfigurationManager.AppSettings["demoTitle"] ?? "northwind");

            return new { server = server, catalog = catalog, username = username, name = name, title = title };
        }

        private object GetDefaultOnPremiseDemo(int identity)
        {
            string server = Maps.DemoOnPremiseServer;
            string catalog = Maps.DemoDatabaseName + identity;
            string username = (System.Configuration.ConfigurationManager.AppSettings["newDemoUser"] ?? "user") + identity;
            string name = Maps.DemoDatabaseName + identity + "sql";
            string title = (System.Configuration.ConfigurationManager.AppSettings["demoTitle"] ?? "northwind");

            return new { server = server, catalog = catalog, username = username, name = name, title = title };
        }

        [ValidateInput(false)]
        [AcceptVerbs(HttpVerbs.Post)]
        public virtual JsonResult ContactUs(string from, string to, string cc, string message, string subject, string name, string comments, string phone, string requestSubjectId, string requestType )
        {
            try
            {
                string host = Convert.ToString(System.Configuration.ConfigurationManager.AppSettings["host"]);
                int port = Convert.ToInt32(System.Configuration.ConfigurationManager.AppSettings["port"]);
                string username = Convert.ToString(System.Configuration.ConfigurationManager.AppSettings["username"]);
                string password = Convert.ToString(System.Configuration.ConfigurationManager.AppSettings["password"]);

                Durados.Cms.DataAccess.Email.Send(host, Map.Database.UseSmtpDefaultCredentials, port, username, password, false, new string[1] { to }, new string[1] { cc }, new string[0], subject, message, from, null, null, DontSend, new string[0], Map.Database.Logger, true);

                if (!string.IsNullOrEmpty(requestType))
                {//requestType = "contact";
                    string confirmationSubject = CmsHelper.GetHtml(requestType + "ConfirmationSubject");
                    string confirmationMessage = CmsHelper.GetHtml(requestType + "ConfirmationMessage");
                    string siteWithoutQueryString = System.Web.HttpContext.Current.Request.Url.Scheme + "://" + System.Web.HttpContext.Current.Request.Url.Authority;
                    confirmationMessage = confirmationMessage.Replace("{0}", "[Name]").Replace("[Name]", name).Replace("[Url]", siteWithoutQueryString);


                    if (string.IsNullOrEmpty(confirmationSubject))
                        confirmationSubject = "Thanks for contacting " + Durados.Database.LongProductName;

                    if (string.IsNullOrEmpty(confirmationMessage))
                        confirmationMessage = string.Format("Hi {0}!<br><br>Thanks for getting in touch.  We will contact you as soon as possible.<br><br>Sincerely,<br>" + Durados.Database.LongProductName + " Team", name);
                    
                        //confirmationMessage = string.Format(confirmationMessage, name);

                    Durados.Cms.DataAccess.Email.Send(host, Map.Database.UseSmtpDefaultCredentials, port, username, password, false, new string[1] { from }, new string[0], new string[0], confirmationSubject, confirmationMessage, to, null, null, DontSend, new string[0], Map.Database.Logger, true);
                }

                try
                {
                    Durados.Web.Mvc.UI.Helpers.Account.InsertContactUsUsers(from, name, comments, phone, 100 + int.Parse(requestSubjectId), null, null);
                }
                catch (Exception ex)
                {
                    Maps.Instance.DuradosMap.Logger.Log(RouteData.Values["Controller"].ToString(), RouteData.Values["Action"].ToString(), "ContactUs", ex, 1, "failed to update websiteuser in ContactUs");

                }
                return Json(true);
            }
            catch
            {
                return Json(false);
            }
        }
      
        [ValidateInput(false)]
        [AcceptVerbs(HttpVerbs.Post)]
        public virtual JsonResult InsertUserAction(string email,  string comments,  string actionId)
        {
           
                try
                {
                    Durados.Web.Mvc.UI.Helpers.Account.InsertContactUsUsers(email, null, comments, null, int.Parse(actionId), null, null);
                }
                catch (Exception ex)
                {
                    Maps.Instance.DuradosMap.Logger.Log(RouteData.Values["Controller"].ToString(), RouteData.Values["Action"].ToString(), "ContactUs", ex, 1, "failed to update websiteuser in ContactUs");

                }
                return Json(true);
           
        }
        [ValidateInput(false)]
        [AcceptVerbs(HttpVerbs.Post)]
        public JsonResult ClickToCall(string caller)
        {
           
                try
                {
                    string uri = System.Configuration.ConfigurationManager.AppSettings["ClickToCallUrl"];

                    uri = string.Format(uri, caller);
                    System.Net.WebRequest req = null;
                    System.Net.WebResponse rsp = null;
                    req = System.Net.WebRequest.Create(uri);
                    req.Method = "GET";
                    req.ContentType = "text/json";

                    rsp = req.GetResponse();
                    string json;
                    using (var reader = new System.IO.StreamReader(rsp.GetResponseStream()))
                    {
                        json = reader.ReadToEnd();
                    }

                    Click2CallJson click2Callres = new System.Web.Script.Serialization.JavaScriptSerializer().Deserialize<Click2CallJson>(json);
                    if (click2Callres.ERRORCODE == 0)
                        return Json(new { Success = true, Message = string.Empty });
                    else
                        return Json(new { Success = false, Message =  click2Callres.ERRORMESSAGE });
                }
                catch (Exception ex)
                {
                    Maps.Instance.DuradosMap.Logger.Log(RouteData.Values["Controller"].ToString(), RouteData.Values["Action"].ToString(), "ClickToCall", ex, 1, "failed to start a call.");
                    return Json(new { Success = false, Message = ex.Message });
                }
            

            
        }
    
    }
    public class Click2CallJson
    {
        public int ERRORCODE { get; set; }
        public string ERRORMESSAGE { get; set; }
        public string CALLID { get; set; }
    }

}
