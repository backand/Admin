using System;
using System.Data;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Durados.Web.Mvc.Specifics.Projects.Tasks
{
    public class TasksProject : Project
    {
        public override DataSet GetDataSet()
        {
            return new TasksDataSet();
        }


        public override string ConnectionStringKey
        {
            get
            {
                Durados.Web.Mvc.Specifics.DataAccess.User.connectionKey = "TasksConnectionString";
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
