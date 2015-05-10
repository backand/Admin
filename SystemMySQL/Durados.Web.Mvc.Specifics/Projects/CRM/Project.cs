using System;
using System.Data;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Durados.Web.Mvc.Specifics.Projects.CRM
{
    public class CRMProject : Project
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
