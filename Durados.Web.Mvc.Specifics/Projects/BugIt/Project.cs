using System;
using System.Data;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Durados.Web.Mvc.Specifics.Projects.BugIt
{
    public class BugItDemoProject : Project
    {
        public override DataSet GetDataSet()
        {
            return new BugsDataSet();
        }


        public override string ConnectionStringKey
        {
            get
            {
                Durados.Web.Mvc.Specifics.DataAccess.User.connectionKey = "BugDBDemoConnectionString";
                return "BugDBDemoConnectionString";
            }
        }

        public override string ConfigFileNameKey
        {
            get
            {
                return "BugItDemoConfig";
            }
        }

    }

    public class BugItLiveProject : Project
    {
        public override DataSet GetDataSet()
        {
            return new BugsDataSet();
        }


        public override string ConnectionStringKey
        {
            get
            {
                Durados.Web.Mvc.Specifics.DataAccess.User.connectionKey = "BugDBLiveConnectionString";
                return "BugDBLiveConnectionString";
            }
        }

        public override string ConfigFileNameKey
        {
            get
            {
                return "BugItLiveConfig";
            }
        }

    }

}
