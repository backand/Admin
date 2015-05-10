using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using System.Web.Mvc.Ajax;

using Durados.Web.Mvc;
using Durados.Web.Mvc.Controllers;
using Durados;
using Durados.DataAccess;
using System.Data;
using Durados.Web.Mvc.UI.Helpers;
using System.Data.SqlClient;
using System.IO;

namespace Durados.HerokuWebsiteController.Controllers
{
    public class HerokuWebsiteController : MultiTenancyController
    {
        protected override void view_AfterCreateBeforeCommit(object sender, CreateEventArgs e)
        {
            base.view_AfterCreateBeforeCommit(sender, e);
            if (e.View.Name == "durados_App")
            {
                /*SqlParameter SiteId = new SqlParameter("@SiteId", e.Values["pluginAppName"].ToString());
                SqlParameter PlanId = new SqlParameter("@PlanId", 3);
                SqlParameter PlugInId = new SqlParameter("@PlugInId", PlugInType.Heroku);

                e.Command.Parameters.Clear();
                e.Command.CommandText = "INSERT INTO [durados_PlugInSite] ([SiteId],[PlanId],[PlugInId]) VALUES (@SiteId,@PlanId,@PlugInId)";

                e.Command.Parameters.Add(SiteId);
                e.Command.Parameters.Add(PlanId);
                e.Command.Parameters.Add(PlugInId);*/
                string pluginName = e.Values["pluginAppName"].ToString() ;

                e.Command.CommandText = "INSERT INTO dbo.durados_PlugInSite (SiteId,PlanId,PlugInId) VALUES ('"+pluginName+"',3,3)";
                object scalar = e.Command.ExecuteScalar();

                 e.Command.CommandText = "select Id from dbo.durados_PlugInSite where SiteId = N'" + pluginName + "'";
                 object scalarId = e.Command.ExecuteScalar();

                 Int32 siteId = 0;
                 if (!scalarId.Equals(string.Empty) && scalarId != null && scalarId != DBNull.Value)
                 {
                     siteId = Convert.ToInt32(scalarId);
                 }

                 string backAndAppName = e.Values["Name"].ToString();
                 int? appId = Maps.Instance.AppExists(backAndAppName);

                 e.Command.CommandText = "INSERT INTO dbo.durados_PlugInSiteApp (PlugInSiteId,AppId) VALUES (" + siteId + "," + appId.Value + ")";
                 e.Command.ExecuteScalar();
            }
        }

        /// </summary>
        /// <param name="template"></param>
        /// <param name="name"></param>
        /// <param name="title"></param>
        /// <param name="server"></param>
        /// <param name="catalog"></param>
        /// <param name="username"></param>
        /// <param name="password"></param>
        /// <param name="usingSsh"></param>
        /// <param name="usingSsl"></param>
        /// <param name="sshRemoteHost"></param>
        /// <param name="sshUser"></param>
        /// <param name="sshPassword"></param>
        /// <param name="sshPrivateKey"></param>
        /// <param name="sshPort"></param>
        /// <param name="productPort"></param>
        /// <returns></returns>
        /// 

        public JsonResult CreateApp(int connectionId, string appName, string pluginAppName)
        {
            /**TODO get required parameters by connection id*/
           // Map.Logger.Log(GetControllerNameForLog(this.ControllerContext), this.ControllerContext.RouteData.Values["action"].ToString(), "CreateApp", "started", "", 87, "server: " + server + ", database: " + catalog, DateTime.Now);

            if (connectionId == 0)
            {
                return Json(new { Success = false, Message = "Invalid Connection Id", Code = Durados.Web.Mvc.Controllers.CreateAppParameter.CODES.INVALID_ARGS });
                /***TODO CreateApp Failure Scenario*/
            }

            View view = GetView("durados_App");

            string name = "";
            try
            {
                name = GetCleanName(appName);
            }
            catch (Exception exception)
            {
                Map.Logger.Log(GetControllerNameForLog(this.ControllerContext), this.ControllerContext.RouteData.Values["action"].ToString(), exception.Source, exception, 1, "app name exception");

                return Json(new { Success = false, Message = exception.Message, Code = Durados.Web.Mvc.Controllers.CreateAppParameter.CODES.INVALID_ARGS });
            }
        
            Dictionary<string, object> values = new Dictionary<string, object>();
            values.Add("Name", name);
            values.Add("Title", name);
            
            values.Add("FK_durados_App_durados_DataSourceType_Parent", "2");
            values.Add("FK_durados_App_durados_SqlConnection_Parent", Convert.ToString(connectionId));

            values.Add("FK_durados_App_durados_SqlConnection_System_Parent", string.Empty);
            values.Add("FK_durados_App_durados_Template_Parent", string.Empty);
            values.Add("SpecificDOTNET", string.Empty);
            values.Add("SpecificJS", string.Empty);
            values.Add("SpecificCss", string.Empty);
            values.Add("Description", string.Empty);
            values.Add("TemplateFile", string.Empty);
            values.Add("Basic", true);
            values.Add("FK_durados_App_durados_SqlConnection_Security_Parent", string.Empty);

            values.Add("pluginAppName", pluginAppName);
            //values.Add("Basic", false);

            DataRow row = null;
            try
            {
                row = view.Create(values, null, view_BeforeCreate, view_BeforeCreateInDatabase, view_AfterCreateBeforeCommit, view_AfterCreateAfterCommit);

                /*Dictionary<string, object> vPluginSite = new Dictionary<string, object>();
                vPluginSite.Add("appname", pluginAppName);
                vPluginSite.Add("PlugInId", PlugInType.Heroku);
                vPluginSite.Add("planid", 3);*/
                //Create(vPluginSite, null, view_BeforeCreate, view_BeforeCreateInDatabase, view_AfterCreateBeforeCommit, view_AfterCreateAfterCommit);
                /*if ((int)Map.PlugInId == (int)PlugInType.Heroku)
                {
                    if (pluginAppName.HasValue == true)
                    {
                        //UpdatePluginSite()
                    }
                }*/
            }
            catch (SqlException exception)
            {
                if (exception.Number == 2601)
                {
                    Map.Logger.Log(GetControllerNameForLog(this.ControllerContext), this.ControllerContext.RouteData.Values["action"].ToString(), exception.Source, exception, 6, "App name already exists");
                    return Json(new { Success = false, Message = "Application name already exists, please enter a different name.", Code = Durados.Web.Mvc.Controllers.CreateAppParameter.CODES.INVALID_ARGS });
                }
                else
                {
                    Map.Logger.Log(GetControllerNameForLog(this.ControllerContext), this.ControllerContext.RouteData.Values["action"].ToString(), exception.Source, exception, 1, "failed to create app row");
                    SendError(1, exception, GetControllerNameForLog(this.ControllerContext), this.ControllerContext.RouteData.Values["action"].ToString(), Map.Logger);
                    return Json(new { Success = false, Message = "Server is busy, please try again later", Code = Durados.Web.Mvc.Controllers.CreateAppParameter.CODES.INVALID_ARGS });
                }
            }
            catch (PlugInUserException exception)
            {
                Map.Logger.Log(GetControllerNameForLog(this.ControllerContext), this.ControllerContext.RouteData.Values["action"].ToString(), exception.Source, exception, 2, "failed to create app row");
                //SendError(1, exception, GetControllerNameForLog(this.ControllerContext), this.ControllerContext.RouteData.Values["action"].ToString(), Map.Logger);
                return Json(new { Success = false, Message = exception.Message, Code = Durados.Web.Mvc.Controllers.CreateAppParameter.CODES.INVALID_ARGS });
            }
            catch (Exception exception)
            {
                Map.Logger.Log(GetControllerNameForLog(this.ControllerContext), this.ControllerContext.RouteData.Values["action"].ToString(), exception.Source, exception, 1, "failed to create app row");
                SendError(1, exception, GetControllerNameForLog(this.ControllerContext), this.ControllerContext.RouteData.Values["action"].ToString(), Map.Logger);
                return Json(new { Success = false, Message = "Server is busy, please try again later", Code = Durados.Web.Mvc.Controllers.CreateAppParameter.CODES.INVALID_ARGS });
            }

            var builder = new UriBuilder(Request.Url);

            string url = builder.Scheme + "://" + name + "." + Maps.Host + ":" + builder.Port + "/";
            /*Maps.GetAppUrl(name)*/

            return Json(new { Success = true, Url = url, Url1 = Maps.GetAppUrl(name) });
        }

        
    }
}
