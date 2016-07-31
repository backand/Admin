using System;
using System.Data;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Durados.Web.Mvc.Specifics.Projects.Visits
{
    public class VisitsProject : Project
    {
        public override DataSet GetDataSet()
        {
            return new VisitDataSet();
        }


        public override string ConnectionStringKey
        {
            get
            {
                Durados.Web.Mvc.Specifics.DataAccess.User.connectionKey = "VisitConnectionString";
                return "VisitConnectionString";
            }
        }

        public override string ConfigFileNameKey
        {
            get
            {
                return "VisitsConfig";
            }
        }

    }
}
