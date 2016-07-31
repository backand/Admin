using System;
using System.Data;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Durados.Web.Mvc.Specifics.Bugit
{
    public class TasksProject : Durados.Web.Mvc.Specifics.Projects.Project
    {
        public override DataSet GetDataSet()
        {
            return new TasksDataSet();
        }


        public override string ConnectionStringKey
        {
            get
            {
                Durados.Web.Mvc.Specifics.Bugit.DataAccess.User.connectionKey = "TasksConnectionString";
                return "TasksConnectionString";
            }
        }

        public override string ConfigFileNameKey
        {
            get
            {
                return "TasksConfig";
            }
        }

    }
}
