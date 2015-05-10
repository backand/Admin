using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using Durados.DataAccess;
using Durados.Web.Mvc;


namespace Durados.website.Helpers
{
    public class SiteContentManager
    {
        
        public void UpdateDynamicContent()
        {
            UserTrackingHelper tracker = new UserTrackingHelper();
            //string referr = tracker.GetReferalUrl();//HttpContext.Current.Request.UrlReferrer;
            
            HttpCookie trackingCookie = tracker.GetTrackingCookie();

            string urlMsg = GetUrlMsg()??string.Empty;
            if ( trackingCookie != null)
            {
                HttpContext.Current.Session["SiteMainMsg"] = GetAndSaveSiteMainMsg(trackingCookie, urlMsg);
            }

        }

        private string GetAndSaveSiteMainMsg(HttpCookie trackingCookie, string urlMsg)
        {
            
            Dictionary<string,object> parameters = new Dictionary<string,object>();
            parameters.Add("@cookieGuid", trackingCookie["Guid"].ToString());
            parameters.Add("@eventTypeOption", urlMsg);

            SqlAccess sqlAccess = new SqlAccess();
            object scalar = sqlAccess.ExecuteScalar(Maps.Instance.DuradosMap.connectionString, "website_GetAndSaveUserMainMsg @cookieGuid,@eventTypeOption", parameters);
            if (scalar == null || scalar == DBNull.Value)
                throw new DuradosException("Unable to get and set site main message");
            return scalar.ToString();
                               
        }
       
        private string GetUrlMsg()
        {
            return HttpContext.Current.Request["r1"];
        }
        
    }
}
