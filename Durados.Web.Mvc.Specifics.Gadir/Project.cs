using System;
using System.Data;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Durados.Web.Mvc.Specifics.Gadir
{
    public class GadirProject : Durados.Web.Mvc.Specifics.Projects.Project
    {
        public override DataSet GetDataSet()
        {
            return new Durados.Web.Mvc.Specifics.Gadir.GadirDataSet();
        }


        public override string ConnectionStringKey
        {
            get
            {
                Durados.Web.Mvc.Specifics.Projects.User.connectionKey = "GadirConnectionString";
                return "GadirConnectionString";
            }
        }

        public override string ConfigFileNameKey
        {
            get
            {
                return "GadirConfig";
            }
        }

    }
}
