using System;
using System.Data;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Durados.Web.Mvc.Specifics.Fibi.P8
{
    public class FibiP8cmProject : Durados.Web.Mvc.Specifics.Projects.Project
    {
        public override DataSet GetDataSet()
        {
            return new P8cmDataSet();
        }

        public override string ConnectionStringKey
        {
            get
            {
                Durados.Web.Mvc.Specifics.Projects.User.connectionKey = "FibiP8cmConnectionString";
                return "FibiP8cmConnectionString";
            }
        }

        public override string ConfigFileNameKey
        {
            get
            {
                return "FibiP8cmConfig";
            }
        }

    }
}
