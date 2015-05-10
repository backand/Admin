using System;
using System.Data;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Durados.Web.Mvc.Specifics.Projects.ERP
{
    public class ERPProject : Project
    {
        public override DataSet GetDataSet()
        {
            return new ERPDataSet();
        }


        public override string ConnectionStringKey
        {
            get
            {
                Durados.Web.Mvc.Specifics.Projects.User.connectionKey = "ERPConnectionString";
                return "ERPConnectionString";
            }
        }

        public override string ConfigFileNameKey
        {
            get
            {
                return "ERPConfig";
            }
        }

    }
}
