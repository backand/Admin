using System;
using System.Data;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace Durados.Web.Mvc.Specifics.Projects.CRM
{
    public class CRMBaseProject : Project
    {
        public override DataSet GetDataSet()
        {
            return new CRMbase.CRMBaseDataSet();
        }


        public override string ConnectionStringKey
        {
            get
            {
                Durados.Web.Mvc.Specifics.Projects.User.connectionKey = HttpContext.Current.Request.ApplicationPath + "_CRMBaseCS";
                return Durados.Web.Mvc.Specifics.Projects.User.connectionKey;
            }
        }

        public override string ConfigFileNameKey
        {
            get
            {
                return HttpContext.Current.Request.ApplicationPath + "_CRMBaseConfig";
            }
        }

    }
}
