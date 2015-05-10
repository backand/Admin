using System;
using System.Data;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Durados.Web.Mvc.Specifics.Excel
{
    public class ExcelProject : Durados.Web.Mvc.Specifics.Projects.Project
    {
        public override DataSet GetDataSet()
        {
            return new Durados.Web.Mvc.Specifics.Excel.ExcelDataSet();
        }


        public override string ConnectionStringKey
        {
            get
            {
                Durados.Web.Mvc.Specifics.Projects.User.connectionKey = "ExcelConnectionString";
                return "ExcelConnectionString";
            }
        }

        public override string ConfigFileNameKey
        {
            get
            {
                return "ExcelConfig";
            }
        }

    }
}
