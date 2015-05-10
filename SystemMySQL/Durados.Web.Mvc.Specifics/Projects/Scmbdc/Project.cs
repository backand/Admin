using System;
using System.Data;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace Durados.Web.Mvc.Specifics.Projects.Scmbdc
{
    public class ScmbdcProject : Durados.Web.Mvc.Specifics.Projects.Project
    {
        public override DataSet GetDataSet()
        {
            return new ScmbdcDataSet();
        }


        public override string ConnectionStringKey 
        {
            get
            {
                return "ScmbdcConnectionString";
            }
        }

        public override string ConfigFileNameKey
        {
            get
            {
                return "ScmbdcConfig";
            }
        }
    }
}
