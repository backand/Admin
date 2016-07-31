using System;
using System.Data;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Durados.Web.Mvc.Specifics.Projects.LMS
{
    public class LMSProject : Project
    {
        public override DataSet GetDataSet()
        {
            return new LMSDataSet();
        }


        public override string ConnectionStringKey
        {
            get
            {
                //Durados.Web.Mvc.Specifics.DataAccess.User.connectionKey = "LMSConnectionString";
                return "LMSConnectionString";
            }
        }

        public override string ConfigFileNameKey
        {
            get
            {
                return "LMSConfig";
            }
        }

    }
}
