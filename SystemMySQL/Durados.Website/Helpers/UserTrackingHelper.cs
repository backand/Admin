using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using Durados.DataAccess;
using Durados.Web.Mvc;

namespace Durados.website.Helpers
{
    public class UserTrackingHelper
    {
        public UserTrackingHelper()
        {

        }

        public void Init()
        {

            HandleDemo();
            Track();
            SiteContentManager siteContentManager = new SiteContentManager();
            siteContentManager.UpdateDynamicContent();

            
        }

        public void Track()
        {
           
            if (!(HttpContext.Current.User == null || HttpContext.Current.User.Identity == null || string.IsNullOrEmpty(HttpContext.Current.User.Identity.Name)))
                return;

            string sessionTrakingKey = "HasTracking";

            if (HttpContext.Current.Session[sessionTrakingKey] != null && Convert.ToBoolean(HttpContext.Current.Session[sessionTrakingKey]))
                return;

            SqlAccess sqlAccess = new SqlAccess();

            HttpContext.Current.Session.Add(sessionTrakingKey, true);

            bool? showDemo=null;
            if (HttpContext.Current.Session["demoToday"] != null)
                showDemo = Convert.ToBoolean(HttpContext.Current.Session["demoToday"]);

            string cookieTrackingName = GetTrackingCookieName();
            HttpCookie trackingCookie = GetTrackingCookie();
            

            if (trackingCookie == null)
            {
                HttpCookie newTrackingCookie = new HttpCookie(cookieTrackingName);
                newTrackingCookie.Expires = DateTime.Now.AddYears(10);
               
                object scalar = sqlAccess.ExecuteScalar(Maps.Instance.DuradosMap.connectionString, sqlAccess.GetUserTrackingSql(), GetTrackingParameters(null,showDemo));
                if (scalar == null || scalar == DBNull.Value)
                    throw new DuradosException("Tracking User Row was not created: New Guid is Null");
                               
                newTrackingCookie.Values.Add("guid", scalar.ToString());
                HttpContext.Current.Response.AppendCookie(newTrackingCookie);
                 
            }
            else
            {
                Guid? orgGuid = null ;
                if(!string.IsNullOrEmpty(trackingCookie["guid"]))
                 orgGuid = new Guid(trackingCookie["guid"]);
                
                sqlAccess.ExecuteNonQuery(Maps.Instance.DuradosMap.connectionString, sqlAccess.GetUserTrackingSql(), GetTrackingParameters(orgGuid,showDemo), null);
            }



        }

        public HttpCookie GetTrackingCookie()
        {
            string cookieTrackingName = GetTrackingCookieName();
            return HttpContext.Current.Request.Cookies.Get(cookieTrackingName);
        }

        private  string GetTrackingCookieName()
        {
            return "ModuBizTracking";
        }
        private void HandleDemo()
        {
            if (HttpContext.Current.Request.QueryString["d"] != null && HttpContext.Current.Request.QueryString["d"] == "yes")
                HttpContext.Current.Session["demoToday"] = true;

            if (HttpContext.Current.Session["demoToday"] == null)
            {
                //check the refer
                string referalUrl = GetReferalUrl();
                string referalDemoOff = (System.Configuration.ConfigurationManager.AppSettings["referalDemoOff"] ?? "sqlauthority;northwindprod11062sql");
                foreach (string r in referalDemoOff.Split(';'))
                {
                    if (referalUrl.IndexOf(r) > -1)
                    {
                        HttpContext.Current.Session["demoToday"] = false;
                        return;
                    }
                }

                if (HttpContext.Current.Session["demoToday"] == null)
                {
                    Random random = new Random();
                    int randomNumber = random.Next(0, 2);
                    bool demoToday = (randomNumber == 1);
                    HttpContext.Current.Session["demoToday"] = demoToday;
                }
            }
        }

        private Dictionary<string, object> GetTrackingParameters(Guid? linkedGuid,bool? showDemo)
        {
            
            Dictionary<string, object> parameters = new Dictionary<string, object>();
            //parameters.Add("@Guid", guid.ToString());
            parameters.Add("@Referer", GetReferalUrl());
            parameters.Add("@AllHeader", GetFormatedHeader());
            parameters.Add("@UserIP", GetUserIP());
            parameters.Add("@QueryString",  HttpContext.Current.Request.QueryString.ToString());
            if (linkedGuid.HasValue && linkedGuid.Value !=null)
                parameters.Add("@LinkedGuid", linkedGuid.Value);
            else
                parameters.Add("@LinkedGuid", DBNull.Value);
            if (showDemo.HasValue )
                parameters.Add("@showDemo", showDemo.Value);
            else
                parameters.Add("@showDemo", DBNull.Value);

            return parameters;
        }

        public string GetReferalUrl()
        {
            if( HttpContext.Current.Request.UrlReferrer==null)
                return string.Empty;
            else
               return HttpContext.Current.Request.UrlReferrer.Host; ;
        }

        private string GetFormatedHeader()
        {
            
            var headers = String.Empty;
            foreach (var key in HttpContext.Current.Request.Headers.AllKeys)
                headers += key + "=" + HttpContext.Current.Request.Headers[key] + Environment.NewLine;
            return headers;
        }

        public string GetUserIP()
        {
            if (HttpContext.Current.Request.UserHostAddress == null)
                return string.Empty;
            else
                return HttpContext.Current.Request.UserHostAddress;
        }
    }
}
