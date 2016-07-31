using System;
using System.Data;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Durados.Web.Mvc.Specifics.Mobix.SiteAdmin
{
    public class MobixSiteAdminProject : Durados.Web.Mvc.Specifics.Projects.Project
    {
        public override DataSet GetDataSet()
        {
            return new MobixSiteAdminDataSet();
        }


        public override string ConnectionStringKey
        {
            get
            {
                Durados.Web.Mvc.Specifics.Projects.User.connectionKey = "MobixSiteAdminConnectionString";
                return "MobixSiteAdminConnectionString";
            }
        }

        public override string ConfigFileNameKey
        {
            get
            {
                return "MobixSiteAdminConfig";
            }
        }

    }
}
