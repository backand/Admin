using System;
using System.Data;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Durados.Web.Mvc.Specifics.Projects.Cms
{
    public class CmsProject : Project
    {
        public override DataSet GetDataSet()
        {
            return new CmsDataSet();
        }


        public override string ConnectionStringKey
        {
            get
            {
                Durados.Web.Mvc.Specifics.DataAccess.User.connectionKey = "CmsConnectionString";
                return "CmsConnectionString";
            }
        }

        public override string ConfigFileNameKey
        {
            get
            {
                return "CmsConfig";
            }
        }

    }
}
