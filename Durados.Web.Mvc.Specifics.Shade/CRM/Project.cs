using System;
using System.Data;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Durados.Web.Mvc.Specifics.Projects;

namespace Durados.Web.Mvc.Specifics.Shade.CRM
{
    public class CRMShadeProject : Project
    {
        public override DataSet GetDataSet()
        {
            return new ShadeDataSet();
        }


        public override string ConnectionStringKey
        {
            get
            {
                Durados.Web.Mvc.Specifics.Projects.User.connectionKey = "CRMShadeConnectionString";
                return "CRMShadeConnectionString";
            }
        }

        public override string ConfigFileNameKey
        {
            get
            {
                return "CRMShadeConfig";
            }
        }

    }
}
