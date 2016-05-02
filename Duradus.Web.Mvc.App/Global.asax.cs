using System;
using System.IO;
using System.Reflection;
using System.Data;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using System.Web.Configuration;
using System.Web.Security;
//using Microsoft.WindowsAzure;
//using Microsoft.WindowsAzure.ServiceRuntime;

namespace Durados.Web.Mvc.App
{
    // Note: For instructions on enabling IIS6 or IIS7 classic mode, 
    // visit http://go.microsoft.com/?LinkId=9394801

    public class MvcApplication : System.Web.HttpApplication
    {
        public static Map Map
        {
            get
            {
                return Maps.Instance.GetMap();
            }
        }

        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

             
           
            //AuthenticationSection auth =
            //    (AuthenticationSection)System.Web.HttpContext.Current.GetSection("system.web/authentication");

            //auth.Mode = AuthenticationMode.None;

            // Map.Initiate(); //"~/bugit2.xml");
            //Durados.Web.Mvc.View view = Map.Database.FirstView;
            //routes.MapRoute(
            //    "Default",                                              // Route name
            //    "{controller}/{action}/{viewName}",                           // URL with parameters
            //    new { controller = view.Controller, action = view.IndexAction, viewName = view.Name }  // Parameter defaults
            //);

            //routes.MapRoute(
            //    "Default",                                              // Route name
            //    "{controller}/{action}/{viewName}"                           // URL with parameters
            //);

            //Durados.Web.Mvc.View view = Map.Database.FirstView;
            if (Maps.Skin)
            {
                routes.MapRoute(
                    "Default",                                              // Route name
                    "{controller}/{action}/{viewName}",                           // URL with parameters
                    new { controller = "Home", action = "FirstTime", viewName = string.Empty }  // Parameter defaults
                );
            }
            else
            {
                //routes.MapRoute(
                //"dashboard",
                // "dashboard/{action}/{pk}",
                // new { controller = "dashboard", action = "config", pk = UrlParameter.Optional }
                //);

                //routes.MapRoute(
                //"chart",
                // "chart/{action}/{pk}",
                // new { controller = "chart", action = "config", pk = UrlParameter.Optional }
                //);

                //routes.MapRoute(
                //"api",
                // "1/{controller}/{action}/{name}/{pk}",
                // new { controller = "view", action = "data", name = UrlParameter.Optional, pk = UrlParameter.Optional }
                //);

                
                 routes.MapRoute(
                   "LogOn",                                              // Route name
                   "Account/LogOn/{viewName}",                           // URL with parameters
                   new { controller = "Account", action = "LogOn", viewName = string.Empty }  // Parameter defaults
                );

                routes.MapRoute(
                  "Default2",                                              // Route name
                  "",                           // URL with parameters
                  new { controller = "Default", action = "Home", viewName = string.Empty } , // Parameter defaults
                                   new { controller = @"[^\.]*" }                          // Parameter constraints
                );

                routes.MapRoute(
                   "Default",                                              // Route name
                   "{controller}/{action}/{viewName}",                           // URL with parameters
                   new { controller = "Account", action = "LogOn", viewName = string.Empty },// Parameter defaults
                                    new { controller = @"[^\.]*" }                          // Parameter constraints
               );

                
                 
            }

           
        }

        protected void Application_Start()
        {
            //if (Maps.Cloud && !RoleEnvironment.IsEmulated)
            //{
            //    Microsoft.WindowsAzure.CloudStorageAccount.SetConfigurationSettingPublisher((configName, configSetter) =>
            //    {
            //        configSetter(RoleEnvironment.GetConfigurationSettingValue(configName));
            //    });
            //}
            
            RegisterRoutes(RouteTable.Routes);
            Map.SystemVersion = Durados.Web.Mvc.Infrastructure.General.Version(Durados.Web.Mvc.App.Helpers.AssemblyHelper.GetWebEntryAssembly());
            Logging.Logger logger = new Logging.Logger();
            logger.WriteToEventLog("Application started", System.Diagnostics.EventLogEntryType.Information, 2020);

        }

        protected void Application_End()
        {
            if (!Maps.MultiTenancy)
            {
                Map.Session.Clear();
            }
            else
            {
                //Maps.Instance.ClearSession();
            }

            Logging.Logger logger = new Logging.Logger();
            logger.WriteToEventLog("Application endded", System.Diagnostics.EventLogEntryType.Information, 2020);
        }

        protected void Session_Start()
        {
            try
            {

                //Durados.website.Helpers.UserTrackingHelper userTracker = new Durados.website.Helpers.UserTrackingHelper();
                //userTracker.Init();
                
            }
            catch (Exception ex)
            {
                Logging.Logger logger = new Logging.Logger();
                logger.WriteToEventLog("Tracking User has error: "+ex.Message, System.Diagnostics.EventLogEntryType.Error, 1);
       
            }
        }
        protected virtual void SendError(int logType, Exception exception, string controller, string action, Logging.Logger logger, string moreInfo)
        {
            try
            {
                bool sendError = Convert.ToBoolean(System.Configuration.ConfigurationManager.AppSettings["sendError"]) && logType == 1;
                if (sendError)
                {
                    string host = Convert.ToString(System.Configuration.ConfigurationManager.AppSettings["host"]);
                    int port = Convert.ToInt32(System.Configuration.ConfigurationManager.AppSettings["port"]);
                    string username = Convert.ToString(System.Configuration.ConfigurationManager.AppSettings["username"]);
                    string password = Convert.ToString(System.Configuration.ConfigurationManager.AppSettings["password"]);
                    
                    string from = Convert.ToString(System.Configuration.ConfigurationManager.AppSettings["fromError"]);
                    string to = Convert.ToString(System.Configuration.ConfigurationManager.AppSettings["toError"]);
                    string applicationName = string.Empty;
                    try
                    {
                        applicationName = System.Web.HttpContext.Current == null ? "No Current request" : System.Web.HttpContext.Current.Request.Url.Host;
                    }
                    catch { }
                    string requestUrl = string.Empty;
                    try
                    {
                        requestUrl = System.Web.HttpContext.Current.Request.RawUrl;
                    }
                    catch { }

                    string message = string.Format("The following error occurred on: {0}<br>Request Url:{1}<br>Exception:<br>{2}", DateTime.Now.ToString(), requestUrl, exception.ToString());
                    if (!string.IsNullOrEmpty(moreInfo))
                    {
                        
                        message += "\n\r\n\r\n\rMore info:\n\r" + moreInfo;
                    }

                    //System.Diagnostics.Debug.Write(to + "|" + from + "|" + applicationName + "|" + password);
                    //System.Diagnostics.Debug.Write("Application Error");
                    //System.Diagnostics.Debug.Write(message);

                    Durados.Cms.DataAccess.Email.Send(host, false, port, username, password, false, new string[1] { to }, null, null, "Application Error :"+applicationName, message, from, null, null, false, logger);

                }
            }
            catch (Exception ex)
            {
                logger.Log(controller, action, exception.Source, ex, 1, "Error sending email when logging an exception");
            }
        }

        protected void Application_Error(object sender, EventArgs e)
        {
            //System.Diagnostics.Debugger.Break();
            // Transfer the user to the appropriate custom error page
            Logging.Logger logger = new Logging.Logger();
            Exception lastError = null;
            try {
                 lastError = Server.GetLastError();

                if (lastError.Message.StartsWith("The controller for path '/") && lastError.Message.EndsWith("was not found or does not implement IController."))
                    return;
            if (lastError.Message.StartsWith("A public action method") && lastError.Message.EndsWith("was not found on controller 'Durados.Website.Controllers.WebsiteController'."))
                    return;

               
                logger.WriteToEventLog("Application Error: " + lastError.Message, System.Diagnostics.EventLogEntryType.Information, 1);

                SendError(1, lastError, "Global.asax", "Application_Error", logger, string.Empty);

                if (lastError is HttpException && (lastError as HttpException).GetHttpCode() == 404)
                    Server.Transfer("~/Views/Shared/e404.aspx");

                else
                    Server.Transfer("~/Views/Shared/Error.aspx");
            }
            catch(Exception ex)
            {
                string message = string.Empty;
                if (lastError == null)
                    message = "Get last error faild";
                else
                    message = lastError.Message;
                logger.Log("Global.asax", "Application_Error", ex.Source, ex, 1, "Error in Application_Error :" + message);
            }
            
        }
    }
}