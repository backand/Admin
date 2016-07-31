using System;
using System.Data;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace Durados.Web.Mvc.Specifics.Projects.Stam
{
    public class StamProject : Project
    {
        public override DataSet GetDataSet()
        {
            return new StamDataSet();
        }


        public override string ConnectionStringKey 
        {
            get
            {
                return "StamConnectionString";
            }
        }

        public override string ConfigFileNameKey
        {
            get
            {
                return "StamConfig";
            }
        }
    }
}
