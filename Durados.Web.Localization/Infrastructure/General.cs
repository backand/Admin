using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace Durados.Web.Infrastructure
{
    public class General
    {

        public static string GetRootPath()
        {


            if (HttpContext.Current.Request.ApplicationPath == "/")
                return '/'.ToString();
            else
                return HttpContext.Current.Request.ApplicationPath + '/';

        }

        public static string GetRootName()
        {


            if (HttpContext.Current.Request.ApplicationPath == "/")
                return "";
            else
                return HttpContext.Current.Request.ApplicationPath.Replace("/","");

        }

        public static bool IsMobile()
        {
            string name = "ismobile";
            if (System.Web.HttpContext.Current.Session == null)
            {
                if (System.Web.HttpContext.Current.Items[name] == null)
                {
                    System.Web.HttpContext.Current.Items[name] = isMobile(HttpContext.Current.Request);
                }
                return Convert.ToBoolean(System.Web.HttpContext.Current.Items[name]);
            }
            else if (HttpContext.Current.Session[name] == null)
            {
                HttpContext.Current.Session[name] = isMobile(HttpContext.Current.Request);
            }

            return Convert.ToBoolean(HttpContext.Current.Session[name]);
        }

        private static bool isMobile(HttpRequest request)
        {
            try
            {
                //if (System.Configuration.ConfigurationManager.AppSettings["Mobile"] == "true")
                //    return true;
                if (System.Configuration.ConfigurationManager.AppSettings["Mobile"] == "false")
                    return false;

                if (request.Browser == null || request.UserAgent == null)
                    return false;
                string userAgent = request.UserAgent;
                string mobileCookie = string.Empty;
                if (request["mobile"] != null)
                    mobileCookie = request["mobile"];
                else if (request.Cookies != null && request.Cookies.Get("mobile") != null)
                    mobileCookie = request.Cookies.Get("mobile").Value;
                bool val = false;
                if (!bool.TryParse(mobileCookie, out val))
                    val = false;
                return val || request.Browser.IsMobileDevice || (userAgent.IndexOf(@"AvantGo") != -1) || (userAgent.IndexOf(@"Windows CE") != -1) || (userAgent.IndexOf(@"NetFront") != -1) || (userAgent.IndexOf(@"BlackBerry") != -1) || (userAgent.IndexOf(@"iPhone") != -1);
            }
            catch
            {
                return false;
            }
        }


        
    }


}
