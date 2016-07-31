using System;
using System.Data;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Durados.Web.Mvc.Specifics.EPL.CRM
{
    public class CRMProject : Durados.Web.Mvc.Specifics.Projects.Project
    {
        public override DataSet GetDataSet()
        {
            return new CRMDataSet();
        }


        public override string ConnectionStringKey
        {
            get
            {
                Durados.Web.Mvc.Specifics.Projects.User.connectionKey = "CRMConnectionString";
                return "CRMConnectionString";
            }
        }

        public override string ConfigFileNameKey
        {
            get
            {
                return "CRMConfig";
            }
        }

    }
}
