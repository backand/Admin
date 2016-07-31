using System;
using System.Data;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Durados.Web.Mvc.Specifics.Projects.Gadir
{
    public class GadirProject : Project
    {
        public override DataSet GetDataSet()
        {
            return new GadirDataSet();
        }


        public override string ConnectionStringKey
        {
            get
            {
                Durados.Web.Mvc.Specifics.DataAccess.User.connectionKey = "TasksConnectionString";
                return "ngadirdoradosConnectionString";
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
