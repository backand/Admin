using System;
using System.Collections.Generic;

namespace Backand
{
    public class request
    {
        public string method 
        {
            get
            {
                return System.Web.HttpContext.Current.Request.HttpMethod;
            }
        }

        public object body
        {
            get
            {
                return Durados.Workflow.JavaScript.GetRequestBody();
            }
        }


        public Dictionary<string, object> headers
        {
            get
            {
                return Durados.Workflow.JavaScript.GetHeaders(System.Web.HttpContext.Current.Request.Headers);
            }
        }
    }
}
