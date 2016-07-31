using System;
using System.Data;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Durados.Web.Mvc.Specifics.DM
{
    public class DMProject : Durados.Web.Mvc.Specifics.Projects.Project
    {
        public override DataSet GetDataSet()
        {
            return new DMDataSet();
        }


        public override string ConnectionStringKey
        {
            get
            {
                Durados.Web.Mvc.Specifics.Projects.User.connectionKey = "DMConnectionString";
                return "DMConnectionString";
            }
        }

        public override string ConfigFileNameKey
        {
            get
            {
                return "DMConfig";
            }
        }

    }
}
