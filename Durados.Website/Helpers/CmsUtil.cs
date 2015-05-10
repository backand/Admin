using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Durados.Web.Mvc.Logging;


  namespace Durados.website.Helpers
{
    public class CmsUtil
    {
        public void SendError(int logType, Exception exception, string controller, string action, Durados.Web.Mvc.Logging.Logger logger, string moreInfo)
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
                    string applicationName = System.Web.HttpContext.Current.Request.Url.Host;
                    string from = Convert.ToString(System.Configuration.ConfigurationManager.AppSettings["fromError"]);
                    string to = Convert.ToString(System.Configuration.ConfigurationManager.AppSettings["toError"]);

                    string message = "The following error occurred:\n\r" + exception.ToString();
                    if (!string.IsNullOrEmpty(moreInfo))
                    {
                        message += "\n\r\n\r\n\rMore info:\n\r" + moreInfo;
                    }

                    
                    
                    Durados.Cms.DataAccess.Email.Send(host, false, port, username, password, false, new string[1] { to }, null, null, "Application Error", message, from, null, null, false, logger);

                }
            }
            catch (Exception ex)
            {
                logger.Log(controller, action, exception.Source, ex, 1, "Error sending email when logging an exception");
            }
        }
    }
}
