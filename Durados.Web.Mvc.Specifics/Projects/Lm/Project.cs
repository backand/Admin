﻿using System;
using System.Data;
using System.Collections.Generic;
using System.Linq;
using System.Text;

  
namespace Durados.Web.Mvc.Specifics.Projects.Lm
{
    public class LmProject : Project
    {
        public override DataSet GetDataSet()
        {
            return new LmDataSet();
        }


        public override string ConnectionStringKey 
        {
            get
            {
                return "LmConnectionString";
            }
        }

        public override string ConfigFileNameKey
        {
            get
            {
                return "LmConfig";
            }
        }
    }
}
