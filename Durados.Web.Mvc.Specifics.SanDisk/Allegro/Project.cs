using System;
using System.Data;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Durados.Web.Mvc.Specifics.SanDisk.Allegro
{
    public class AllegroProject : Durados.Web.Mvc.Specifics.Projects.Project
    {
        public override DataSet GetDataSet()
        {
            return new AllegroDataSet1();
        }


        public override string ConnectionStringKey
        {
            get
            {
                Durados.Web.Mvc.Specifics.Projects.User.connectionKey = "SanDiskAllegroConnectionString";
                return "SanDiskAllegroConnectionString";
            }
        }

        public override string ConfigFileNameKey
        {
            get
            {
                return "SanDiskAllegroConfig";
            }
        }

    }
}
