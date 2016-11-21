using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Ajax;

using System.Data;
using System.IO;
using System.Web.Script.Serialization;

using Microsoft.WindowsAzure;
using Microsoft.WindowsAzure.StorageClient;
using Amazon.S3.Transfer;
using Amazon.S3;
using Amazon.S3.Model;

using Durados;
using Durados.DataAccess;
using Durados.Web.Mvc.UI.Helpers;
using Durados.Web.Mvc.Logging;
using Durados.Diagnostics;

namespace Durados.Web.Mvc.Controllers
{
    public class BaseController : Controller
    {
        private Map map = null;
        public virtual Map Map
        {
            get
            {
                if (map == null)
                    map = Maps.Instance.GetMap();
                map.OpenSshSession();
                return map;
            }
        }

        protected override void OnException(ExceptionContext filterContext)
        {
            string action = filterContext.RouteData.Values["action"].ToString();
            //string controller = filterContext.RouteData.Values["controller"].ToString();
            string controller = GetControllerNameForLog(filterContext);
            string viewName = null;
            if (filterContext.RouteData.Values.ContainsKey("viewName"))
            {
                viewName = filterContext.RouteData.Values["viewName"].ToString();
            }

            ILog log = null;
            Logging.Logger logger = new Logging.Logger();

            int logType = filterContext.Exception is Durados.FileNotFoundException ? 3 : 1;
            try
            {
                logger.Log(controller, action, filterContext.Exception.Source, filterContext.Exception, logType, null);
            }
            catch { }
            logger.WriteToEventLog("Message: " + filterContext.Exception.Message + "; Trace: " + filterContext.Exception.StackTrace, System.Diagnostics.EventLogEntryType.Error, logType);

            if (log == null)
            {
                log = new Log();
                log.Action = action;
                log.Controller = controller;
                if (filterContext.Exception != null)
                {
                    log.ExceptionMessage = filterContext.Exception.Message;
                    log.Trace = filterContext.Exception.StackTrace;
                }
            }
            SendError(logType, filterContext.Exception, controller, action, logger);

            // Output a nice error page
            if (filterContext.HttpContext.IsCustomErrorEnabled)
            {
                filterContext.ExceptionHandled = true;
                this.View("CustomError", GetCustomError(log, viewName)).ExecuteResult(this.ControllerContext);
                Server.ClearError();
            }
        }

        protected virtual void SendError(int logType, Exception exception, string controller, string action, Logging.Logger logger)
        {
            SendError(logType, exception, controller, action, logger, string.Empty);
        }

        protected virtual void SendError(int logType, Exception exception, string controller, string action, Logging.Logger logger, string moreInfo)
        {
            try
            {
                bool sendError = Convert.ToBoolean(System.Configuration.ConfigurationManager.AppSettings["sendError"]) && logType == 1;
                if (sendError)
                {
                    Durados.Cms.Services.EmailProvider provider = new Durados.Cms.Services.EmailProvider();
                    Durados.Cms.Services.SMTPServiceDetails smtp = provider.GetSMTPServiceDetails(provider.GetSMTPProvider());
                   
                    string applicationName = System.Web.HttpContext.Current.Request.Url.Host;

                    string defaultTo = smtp.to;
                    string[] to = !string.IsNullOrEmpty(Map.Database.AdminEmail) ? Map.Database.AdminEmail.Split(';') : null;
                    string[] cc = new string[1]{defaultTo};
                    if (to ==null || to.Length==0)
                    {
                        to = cc;
                        cc = null;
                    }
                  
                    

                    string message = "The following error occurred:\n\r" + exception.ToString();
                    if (!string.IsNullOrEmpty(moreInfo))
                    {
                        message += "\n\r\n\r\n\rMore info:\n\r" + moreInfo;
                    }

                    Durados.Cms.DataAccess.Email.Send(smtp.host, smtp.useDefaultCredentials, smtp.port, smtp.username, smtp.password, false, to, cc, null, applicationName + " error", message, smtp.from, null, null, DontSend, logger);
                }
            }
            catch (Exception ex)
            {
                logger.Log(controller, action, exception.Source, ex, 1, "Error sending email when logging an exception");
            }
        }


        protected virtual Durados.Web.Mvc.View GetView(string viewName, string action)
        {
            return GetView(viewName);
        }

        protected virtual Durados.Web.Mvc.View GetView(string viewName)
        {
            return ViewHelper.GetView(viewName);
        }


        protected int? SaveConnection(string server, string catalog, string username, string password, string userId, SqlProduct sqlProduct, bool usingSsl = false)
        {
            return SaveConnection(server, catalog, username, password, userId, sqlProduct, false, usingSsl, string.Empty, string.Empty, string.Empty, string.Empty, 0, 0);
        }

        protected int? SaveConnection(string server, string catalog, string username, string password, string userId, SqlProduct sqlProduct, bool usingSsh=false, bool usingSsl=false, string sshRemoteHost=null, string sshUser=null, string sshPassword=null, string sshPrivateKey=null, int sshPort=0, int productPort=0)
        {
            View view = GetView("durados_SqlConnection");

            Dictionary<string, object> values = new Dictionary<string, object>();
            values.Add("ServerName", server);
            values.Add("Catalog", catalog);
            values.Add("Username", username);
            values.Add("IntegratedSecurity", false);
            values.Add("Password", password);
            values.Add(view.GetFieldByColumnNames("SqlProductId").Name, ((int)sqlProduct).ToString());
            values.Add(view.GetFieldByColumnNames("DuradosUser").Name, userId);

            values.Add("ProductPort", productPort.ToString());
            values.Add("SshRemoteHost", sshRemoteHost);
            values.Add("SshPort", sshPort);
            values.Add("SshUser", sshUser);
            values.Add("SshPassword", sshPassword);
            values.Add("SshPrivateKey", sshPrivateKey);
            values.Add("SshUses", usingSsh);
            values.Add("SslUses", usingSsl);

            string pk = view.Create(values);

            if (string.IsNullOrEmpty(pk))
                throw new DuradosException("Failed to get connection id");

            int id = -1;

            if (Int32.TryParse(pk, out id))
                return id;
            else
                throw new DuradosException("Failed to get connection id");
        }
        protected virtual Durados.Web.Mvc.Infrastructure.CustomError GetCustomError(ILog log, string viewName)
        {
            return new Durados.Web.Mvc.Infrastructure.CustomError(log, viewName);
        }

        /// <summary>
        /// 404 error - Page not found 
        /// </summary>
        /// <returns></returns>
        public ActionResult e404()
        {
            return View();
        }

        public bool DontSend
        {
            get
            {
                return Convert.ToBoolean(System.Configuration.ConfigurationManager.AppSettings["DontSend"] ?? "false");
            }
        }

        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            string action = filterContext.RouteData.Values["action"].ToString();
            if (Request.Browser.Browser == "IE")
                System.Web.HttpContext.Current.Response.AddHeader("p3p", "CP=\"IDC DSP COR ADM DEVi TAIi PSA PSD IVAi IVDi CONi HIS OUR IND CNT\"");
            Map.Logger.Log(GetViewNameForLog(filterContext), "Start", "OnActionExecuting", "Controller", "", 13, Map.Logger.NowWithMilliseconds(), DateTime.Now);

            try
            {
                bool isAllEventsAction = Map.Logger.IsAllEventsAction(action);
                if (Map.Logger.Events.BeforeAction || isAllEventsAction)
                {
                    Map.Logger.Log(GetControllerNameForLog(filterContext), action, "BeforeAction", null, 3, GetActionParameters(filterContext));
                }

                if (Map.Database.RequiresSSL)
                    (new Attributes.RequiresSSL()).ForceHttps(filterContext);

                base.OnActionExecuting(filterContext);
            }
            catch { }

            Map.Logger.Log(GetViewNameForLog(filterContext), "End", "OnActionExecuting", "Controller", "", 13, Map.Logger.NowWithMilliseconds(), DateTime.Now);

        }

        protected virtual string GetViewNameForLog(ControllerContext controllerContext)
        {
            string viewName = string.Empty;
            if (controllerContext.RouteData.Values.ContainsKey("viewName"))
            {
                viewName = controllerContext.RouteData.Values["viewName"].ToString();
            }
            return viewName;
        }

        protected virtual string GetControllerNameForLog(ControllerContext controllerContext)
        {
            string viewName = GetViewNameForLog(controllerContext);

            string controller = controllerContext.RouteData.Values["controller"].ToString();
            if (!string.IsNullOrEmpty(viewName))
                controller += " - " + viewName;

            return controller;
        }

        virtual protected string GenerateAppName(string app_name, int uid)
        {
            return app_name + uid;
        }

        protected virtual string GetControllerName()
        {
            return this.ControllerContext.RouteData.Values["controller"].ToString();
        }

        protected virtual string GetActionParameters(ActionExecutingContext filterContext)
        {
            string s = string.Empty;
            Dictionary<string, object> parameters = new Dictionary<string, object>(filterContext.ActionParameters);
            parameters.Remove("password");
            foreach (string key in parameters.Keys)
            {
                s += key + "=";
                if (filterContext.ActionParameters[key] == null)
                {
                    s += "NULL,";
                }
                else
                {
                    s += filterContext.ActionParameters[key].ToString() + ",";
                }
            }
            s.Replace("'", "\"");
            return s;
        }

        protected virtual string GetUserID()
        {
            return Map.Database.GetUserID();
        }

        protected override void OnActionExecuted(ActionExecutedContext filterContext)
        {
            Map.Logger.Log(GetViewNameForLog(filterContext), "Start", "OnActionExecuted", "Controller", "", 13, Map.Logger.NowWithMilliseconds(), DateTime.Now);

            try
            {
                string action = filterContext.RouteData.Values["action"].ToString();

                bool isAllEventsAction = Map.Logger.IsAllEventsAction(action);
                if (Map.Logger.Events.AfterAction || isAllEventsAction)
                {
                    Map.Logger.Log(GetControllerNameForLog(filterContext), action, "AfterAction", null, 3, isAllEventsAction ? "MA" : "");
                }

                base.OnActionExecuted(filterContext);
            }
            catch { }

            Map.Logger.Log(GetViewNameForLog(filterContext), "End", "OnActionExecuted", "Controller", "", 13, Map.Logger.NowWithMilliseconds(), DateTime.Now);

        }

        protected override void OnResultExecuting(ResultExecutingContext filterContext)
        {
            Map.Logger.Log(GetViewNameForLog(filterContext), "Start", "OnResultExecuting", "Controller", "", 13, Map.Logger.NowWithMilliseconds(), DateTime.Now);
            if (filterContext.Result is JsonResult)
            {
                ((JsonResult)filterContext.Result).JsonRequestBehavior = JsonRequestBehavior.AllowGet;
            }

            try
            {
                string action = filterContext.RouteData.Values["action"].ToString();

                bool isAllEventsAction = Map.Logger.IsAllEventsAction(action);

                if (Map.Logger.Events.BeforeResult || isAllEventsAction)
                {
                    Map.Logger.Log(GetControllerNameForLog(filterContext), action, "BeforeResult", null, 3, isAllEventsAction ? "MA" : "");
                }
                base.OnResultExecuting(filterContext);
            }
            catch { }

            Map.Logger.Log(GetViewNameForLog(filterContext), "End", "OnResultExecuting", "Controller", "", 13, Map.Logger.NowWithMilliseconds(), DateTime.Now);

        }

        protected override void OnResultExecuted(ResultExecutedContext filterContext)
        {
            Map.Logger.Log(GetViewNameForLog(filterContext), "Start", "OnResultExecuted", "Controller", "", 13, Map.Logger.NowWithMilliseconds(), DateTime.Now);

            try
            {
                string action = filterContext.RouteData.Values["action"].ToString();

                bool isAllEventsAction = Map.Logger.IsAllEventsAction(action);

                if (Map.Logger.Events.AfterResult || isAllEventsAction)
                {
                    Map.Logger.Log(GetControllerNameForLog(filterContext), action, "AfterResult", null, 3, isAllEventsAction ? "MA" : "");
                }
                base.OnResultExecuted(filterContext);
            }
            catch { }

            Map.Logger.Log(GetViewNameForLog(filterContext), "End", "OnResultExecuted", "Controller", "", 13, Map.Logger.NowWithMilliseconds(), DateTime.Now);

        }

        //protected virtual void LogEvent(ControllerContext filterContext, string eventName)
        //{
        //    string action = filterContext.RouteData.Values["action"].ToString();
        //    string controller = filterContext.RouteData.Values["controller"].ToString();

        //    if (Logger.Events.BeforeAction || Logger.IsAllEventsAction(action))
        //    {
        //        Logger.Log(controller, action, eventName, null, 3, "action event");
        //    }
        //}




        ///////////////////////////////////////////////DB ValidateConnection from DurasController


        protected Durados.Security.Ssh.ISession session = null;
        protected int localPort = 0;


        protected virtual IDbConnection GetConnection(SqlProduct sqlProduct, string connectionString)
        {
            if (sqlProduct == SqlProduct.Oracle)
            {
                return  new Oracle.ManagedDataAccess.Client.OracleConnection(connectionString);
            }
            else if (sqlProduct == SqlProduct.Postgre)
            {
                Npgsql.NpgsqlConnection connection = new Npgsql.NpgsqlConnection(connectionString);
                connection.ValidateRemoteCertificateCallback += new Npgsql.ValidateRemoteCertificateCallback(connection_ValidateRemoteCertificateCallback);
                return connection;
            }
            else if (sqlProduct == SqlProduct.MySql)
                return new MySql.Data.MySqlClient.MySqlConnection(connectionString);
            else
                return new System.Data.SqlClient.SqlConnection(connectionString);
        }

        protected virtual bool connection_ValidateRemoteCertificateCallback(System.Security.Cryptography.X509Certificates.X509Certificate cert, System.Security.Cryptography.X509Certificates.X509Chain chain, System.Net.Security.SslPolicyErrors errors)
        {
            return true;
        }


        public virtual string GetConnection(string serverName, string catalog, bool? integratedSecurity, string username, string password, string duradosuserId, SqlProduct sqlProduct, int localPort, bool usesSsh, bool usesSsl)
        {

            string connectionString = null;
            System.Data.SqlClient.SqlConnectionStringBuilder builder = new System.Data.SqlClient.SqlConnectionStringBuilder();

            builder.ConnectionString = Map.connectionString;
            
            bool hasServer = !string.IsNullOrEmpty(serverName);
            bool hasCatalog = !string.IsNullOrEmpty(catalog);


            if (!hasCatalog)
                throw new DuradosException("Catalog Name is missing");


            if (integratedSecurity.HasValue && integratedSecurity.Value)
            {
                if (!hasServer)
                {
                    serverName = builder.DataSource;

                }
                connectionString = "Data Source={0};Initial Catalog={1};Integrated Security=True;";
                return string.Format(connectionString, serverName, catalog);
            }
            else
            {

                connectionString = "Data Source={0};Initial Catalog={1};User ID={2};Password={3};Integrated Security=False;";
                if (sqlProduct == SqlProduct.MySql)
                {
                    if (usesSsh)
                        connectionString = "server=localhost;database={1};User Id={2};password={3};port={4};convert zero datetime=True";
                    else
                        connectionString = "server={0};database={1};User Id={2};password={3};port={4};convert zero datetime=True";
                }
                if (sqlProduct == SqlProduct.Postgre)
                {
                    if (usesSsl)
                        if (usesSsh)
                            connectionString = "server=localhost;database={1};User Id={2};password={3};port={4};SSL=true;SslMode=Require;";
                        else
                            connectionString = "server={0};database={1};User Id={2};password={3};port={4};SSL=true;SslMode=Require;";
                    //connectionString = "HOST={0};DATABASE={1};USER ID={2};PASSWORD={3}";//test1.cb8bfk90dnws.us-west-2.rds.amazonaws.com;DATABASE=demo;USER ID=root;PASSWORD=Modubiz2012
                    else
                        if (usesSsh)
                            connectionString = "server=localhost;database={1};User Id={2};password={3};port={4};Encoding=UNICODE;";
                        else
                            connectionString = "server={0};database={1};User Id={2};password={3};port={4};Encoding=UNICODE;";
                    // connectionString = "HOST={0};DATABASE={1};USER ID={2};PASSWORD={3}";//
                }
                if (sqlProduct == SqlProduct.Oracle)
                {//"Data Source=(DESCRIPTION=(ADDRESS=(PROTOCOL=TCP)(HOST={0})(PORT=1521))(CONNECT_DATA=(SERVICE_NAME={1})));User ID={2};Password={3};"
                    connectionString = OracleAccess.GetConnectionStringSchema();// "server=localhost;database={1};User Id={2};password={3};port={4};convert zero datetime=True";
                }
                bool hasUsername = !string.IsNullOrEmpty(username);
                bool hasPassword = !string.IsNullOrEmpty(password);

                //if (!hasServer)
                //{
                //    if (Maps.AllowLocalConnection) 
                //        serverName = builder.DataSource;
                //    else
                //        throw new DuradosException("Server Name is missing");
                //}

                //if (!hasUsername)
                //{
                //    if (Maps.AllowLocalConnection) 
                //        username = builder.UserID;
                //    else
                //        throw new DuradosException("Username Name is missing");
                //}

                //if (!hasPassword)
                //{
                //    if (Maps.AllowLocalConnection) 
                //        password = builder.Password;
                //    else
                //        throw new DuradosException("Password Name is missing");
                //}

                //return string.Format(connectionString, serverName, catalog, username, password);  
                if (!hasServer)
                {
                    if (Maps.AllowLocalConnection)
                        serverName = builder.DataSource;
                    else
                        throw new DuradosException("Server Name is missing");
                }

                if (!hasUsername)
                {
                    if (Maps.AllowLocalConnection)
                        username = builder.UserID;
                    else
                        throw new DuradosException("Username is missing");
                }

                if (!hasPassword)
                {
                    if (Maps.AllowLocalConnection)
                        password = builder.Password;
                    else
                        throw new DuradosException("Password is missing");
                }

                return string.Format(connectionString, serverName, catalog, username, password, localPort);

            }
        }

        protected virtual IDbConnection GetNewConnection(SqlProduct sqlProduct, string connectionString)
        {
            return Durados.DataAccess.DataAccessObject.GetNewConnection(sqlProduct, connectionString);
        }

        protected string ServernameFieldName = "ServerName";
        protected string CatalogFieldName = "Catalog";
        protected string UsernameFieldName = "Username";
        protected string PasswordFieldName = "Password";
        protected string IntegratedSecurityFieldName = "IntegratedSecurity";
        protected string DuradosUserFieldName = "DuradosUser";
        protected string ProductPortFieldName = "ProductPort";

        protected string SshRemoteHost = "SshRemoteHost";
        protected string SshPort = "SshPort";
        protected string SshUser = "SshUser";
        protected string SshPassword = "SshPassword";
        protected string SshUses = "SshUses";
        protected string ProductPort = "ProductPort";

        protected virtual void ValidateConnectionString(bool integratedSecurity, string serverName, string catalog, string username, string password, bool usesSsh, bool usesSsl, string duradosUserId, SqlProduct sqlProduct, string sshRemoteHost, string sshUser, string sshPassword, string sshPrivateKey, int sshPort, int productPort)
        {
            OpenSshSessionIfNecessary(usesSsh, sshRemoteHost, sshUser, sshPassword, sshPrivateKey, sshPort, productPort);

            int port = productPort;
            if (usesSsh)
                port = localPort;
            string connectionString = GetConnection(serverName, catalog, integratedSecurity, username, password, duradosUserId, sqlProduct, port, usesSsh, usesSsl);
            IDbConnection connection = GetNewConnection(sqlProduct, connectionString);

            try
            {
                connection.Open();

            }
            catch (InvalidOperationException ex)
            {
                throw new DuradosException("Connection to Database Faild. Please check connection fields.", ex);
            }
            catch (System.Data.SqlClient.SqlException ex)
            {
                throw new DuradosException(ex.Message, ex);
            }
            catch (MySql.Data.MySqlClient.MySqlException ex)
            {
                throw new DuradosException(ex.Message, ex);
            }
            catch (ExceedLengthException ex)
            {
                throw new DuradosException(ex.Message, ex);
            }
            finally
            {
                if (connection != null)
                {
                    connection.Close();
                    connection.Dispose();
                }
                CloseSshSessionIfNecessary();
            }
        }

        protected virtual void CloseSshSessionIfNecessary()
        {
            if (session != null)
                session.Close();
        }


        protected virtual void OpenSshSessionIfNecessary(bool usingSsh, string sshRemoteHost, string sshUser, string sshPassword, string privateKey, int sshPort, int productPort)
        {
            if (usingSsh)
            {
                Durados.Security.Ssh.ITunnel tunnel = new Durados.DataAccess.Ssh.Tunnel();

                tunnel.RemoteHost = sshRemoteHost;
                tunnel.User = sshUser;
                tunnel.Password = sshPassword;
                tunnel.PrivateKey = privateKey;
                tunnel.Port = sshPort;
                int remotePort = productPort;
                localPort = Maps.Instance.AssignLocalPort();

                session = new Durados.DataAccess.Ssh.ChilkatSession(tunnel, remotePort, localPort);
                session.Open(15);
            }
        }

        //////////////////////////////////////////////////////////
    }

    public class FtpDownloadResult : FileResult
    {
        public FtpDownloadResult(string contentType)
            : base(contentType)
        {

        }

        public System.IO.Stream responseStream { get; set; }

        public override void ExecuteResult(ControllerContext context)
        {
            context.HttpContext.Response.Buffer = true;
            context.HttpContext.Response.Clear();
            context.HttpContext.Response.AddHeader("content-disposition", "attachment; filename=" + FileDownloadName);
            context.HttpContext.Response.ContentType = "application/octet-stream";

            byte[] buffer = ReadFully(responseStream);

            //byte[] buffer = new byte[16 * 1024];
            //int len = 0;
            //while ((len = responseStream.Read(buffer, 0, buffer.Length)) > 0 && context.HttpContext.Response.IsClientConnected)
            //{

            // Write the data to the current output stream.
            context.HttpContext.Response.OutputStream.Write(buffer, 0, buffer.Length);

            // Flush the data to the HTML output.
            context.HttpContext.Response.Flush();

            //}

        }

        private static byte[] ReadFully(System.IO.Stream input)
        {
            byte[] buffer = new byte[16 * 1024];
            using (System.IO.MemoryStream ms = new System.IO.MemoryStream())
            {
                int read;
                while ((read = input.Read(buffer, 0, buffer.Length)) > 0)
                {
                    ms.Write(buffer, 0, read);
                }
                return ms.ToArray();
            }
        }

        protected override void WriteFile(HttpResponseBase response)
        {
            throw new NotImplementedException();
        }




    }

}