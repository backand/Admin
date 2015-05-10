using System;
using System.Data;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Durados.Web.Mvc.Specifics.SanDisk.Samples
{
    public class SamplesProject : Durados.Web.Mvc.Specifics.Projects.Project
    {
        public override DataSet GetDataSet()
        {
            return new SamplesDataSet1();
        }


        public override string ConnectionStringKey
        {
            get
            {
                Durados.Web.Mvc.Specifics.Projects.User.connectionKey = "SanDiskSamplesConnectionString";
                return "SanDiskSamplesConnectionString";
            }
        }

        public override string ConfigFileNameKey
        {
            get
            {
                return "SanDiskSamplesConfig";
            }
        }

    }
}
