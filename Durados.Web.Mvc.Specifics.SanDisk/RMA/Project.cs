﻿using System;
using System.Data;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Durados.Web.Mvc.Specifics.SanDisk.RMA
{
    public class RMAProject : Durados.Web.Mvc.Specifics.Projects.Project
    {
        public override DataSet GetDataSet()
        {
            return new RMADataSet1();
        }


        public override string ConnectionStringKey
        {
            get
            {
                Durados.Web.Mvc.Specifics.Projects.User.connectionKey = "SanDiskRMAConnectionString";
                return "SanDiskRMAConnectionString";
            }
        }

        public override string ConfigFileNameKey
        {
            get
            {
                return "SanDiskRMAConfig";
            }
        }

    }
}
