using System;
using System.Collections.Generic;
using System.Collections.Specialized;

namespace Backand
{
    public class request
    {
        public string id
        {
            get
            {
                return ((Guid)(Durados.Workflow.JavaScript.GetCacheInCurrentRequest(Durados.Workflow.JavaScript.GuidKey) ?? Guid.NewGuid())).ToString(); 
            }
        }
        public string method 
        {
            get
            {
                return System.Web.HttpContext.Current.Request.HttpMethod;
            }
        }

        public object query
        {
            get
            {
                Dictionary<string, object> dictData = new Dictionary<string, object>();
                foreach (string key in System.Web.HttpContext.Current.Request.QueryString)
                {
                    dictData.Add(key, System.Web.HttpContext.Current.Request.QueryString.Get(key));
                }
                return dictData;
            }
        }

        public object body
        {
            get
            {
                return Durados.Workflow.JavaScript.GetRequestBody();
            }
        }


        public object headers
        {
            get
            {
                return Durados.Workflow.JavaScript.GetHeaders(System.Web.HttpContext.Current.Request.Headers);
            }
        }
    }
}
