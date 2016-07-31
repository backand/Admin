using System;
using System.Data;
using System.Collections.Generic;
using System.Linq;
using System.Text;

  
namespace Durados.Web.Mvc.Specifics.Projects.Gear
{
    public class GearProject : Project
    {
        public override DataSet GetDataSet()
        {
            return new GearDataSet();
        }


        public override string ConnectionStringKey 
        {
            get
            {
                return "GearConnectionString";
            }
        }

        public override string ConfigFileNameKey
        {
            get
            {
                return "GearConfig";
            }
        }
    }
}
