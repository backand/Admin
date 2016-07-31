using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Durados.Web.Mvc;
using System.Windows.Forms;
using Durados.Web.Mvc.Logging;

namespace Durados.Windows.Utilities.AzureUploader.DeleteApp
{
    class DeleteAppManager 
    {
        Logger logger = new Logger();
        public DeleteAppManager()
        {
            logger.ConnectionString = Properties.Settings.Default.LocalConnection;
        }
        public string DeleteApp(string appId, string appName)
        {
            //if (!(Map is DuradosMap && Maps.IsSuperDeveloper(null)))
            //{
            //    Maps.Instance.DuradosMap.Logger.Log("DeleteAppApplication"+SystemInformation.ComputerName, this.ControllerContext.RouteData.Values["action"].ToString(), null, "Accessd by non-super developer in a spesific app", null, 1, null, DateTime.Now);
            //    return "This action is not allowd";
            //}
            int id;
            int.TryParse(appId, out id);
            if (id <= 0 && string.IsNullOrEmpty(appName))
                return "AppId or App Name could not be parsed.";

            ProductMaintenance productMaintenece = new  ProductMaintenance();
            try
            {
                if (id > 0)
                    productMaintenece.RemoveApp(id);
                else
                    productMaintenece.RemoveApp(appName);

                return "App number " + ((id == 0) ? appName : appId) + " was deleted";
            }
            catch (Exception ex)
            {
               
                logger.Log("DeleteAppApplication on "+System.Windows.Forms.SystemInformation.ComputerName, "DeleteApp", null, ex, 1, null);
                return "An error occuered while deleteing te app ,message: " + ex.Message;
            }
        }

        private string GeLogConnectionString()
        {
            throw new NotImplementedException();
        }
        public string DeleteAppView()
        {
            //if (!(Map is DuradosMap && Maps.IsSuperDeveloper(null)))
            //{
            //    Maps.Instance.DuradosMap.Logger.Log("DeleteAppApplication"+SystemInformation.ComputerName, this.ControllerContext.RouteData.Values["action"].ToString(), null, "Accessd by non-super developer in a spesific app", null, 1, null, DateTime.Now);
            //    return "This action is not allowd";
            //}
            ProductMaintenance productMaintenece = new  ProductMaintenance(resultOutput);
            try
            {
                return productMaintenece.RemoveApps(null, null);

            }
            catch (Exception ex)
            {
                
                logger.Log("DeleteAppApplication" + SystemInformation.ComputerName, "DeleteAppView", null, ex, 1, null);
                return "An Error occured while tring to delete apps:" + ex.Message;
            }
        }

        public RichTextBox resultOutput { get; set; }
    }
   
}
