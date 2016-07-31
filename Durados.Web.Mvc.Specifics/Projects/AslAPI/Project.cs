using System;
using System.Data;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Durados.Web.Mvc.Specifics.Projects.AslAPI
{
    public class ASLAPIProject : Project
    {
        public override DataSet GetDataSet()
        {
            return new ASLAPIDataSet();
        }


        public override string ConnectionStringKey
        {
            get
            {
                Durados.Web.Mvc.Specifics.DataAccess.User.connectionKey = "ASLAPIConnectionString";
                return "ASLAPIConnectionString";
            }
        }

        public override string ConfigFileNameKey
        {
            get
            {
                return "ASLAPIConfig";
            }
        }

    }
}
