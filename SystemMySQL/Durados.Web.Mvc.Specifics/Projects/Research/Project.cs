using System;
using System.Data;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Durados.Web.Mvc.Specifics.Projects.Research
{
    public class ResearchProject : Project
    {
        public override DataSet GetDataSet()
        {
            return new ResearchDataSet();
        }


        public override string ConnectionStringKey
        {
            get
            {
                Durados.Web.Mvc.Specifics.Projects.User.connectionKey = "ResearchConnectionString";
                return "ResearchConnectionString";
            }
        }

        public override string ConfigFileNameKey
        {
            get
            {
                return "ResearchConfig";
            }
        }

    }
}
