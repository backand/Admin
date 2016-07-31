
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Web.Http;


namespace BackAnd.Web.Api.Controllers
{
    
    public class themeController : apiController
    {
        public HttpResponseMessage Get(string id = null)
        {
            try
            {
                if (id == null)
                {
                    id = GetAppName();
                }
                string path = Durados.Web.Mvc.Maps.Instance.GetAppThemePath(id);
                //if (!path.StartsWith("/"))
                //    path = "/" + path;
                string responseBody = string.Format(@" location.replace('{0}') ", path);

                HttpResponseMessage response = Request.CreateResponse(System.Net.HttpStatusCode.OK, responseBody, new TextPlainFormatter());
                response.Content.Headers.ContentType = new MediaTypeHeaderValue("application/javascript");
                response.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment");
                response.Content.Headers.ContentDisposition.Name = "theme";
                response.Content.Headers.ContentDisposition.FileName = "theme.js";

                return response;
            }
            catch (Exception exception)
            {
                throw new BackAndApiUnexpectedResponseException(exception, this);
            }
        }

        private string GetAppName()
        {
            try
            {
                return System.Web.HttpContext.Current.Request.UrlReferrer.Authority.Split('.')[0];
            }
            catch
            {
                try
                {
                    return Durados.Web.Mvc.Maps.GetCurrentAppName();
                }
                catch
                {
                    return null;
                }
            }
        }
    }

    
}
