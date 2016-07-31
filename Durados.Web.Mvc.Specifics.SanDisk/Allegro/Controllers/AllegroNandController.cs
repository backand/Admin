using System;
using System.Data;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Ajax;

using Durados;
using Durados.DataAccess;
using Durados.Web.Mvc.UI.Helpers;
using Durados.Web.Membership;


namespace Durados.Web.Mvc.Specifics.SanDisk.Allegro.Controllers
{
    public class AllegroNandController : AllegroHomeController
    {
        protected override void AfterCreate(View view, string pk)
        {
            base.AfterCreate(view, pk);

            Durados.DataAccess.SqlAccess sqlAccess = new SqlAccess();

            string sql = "exec Durados_Allegro_NANDCreateMDE {0}";
            sqlAccess.ExecuteNonQuery(view.Database.ConnectionString, string.Format(sql, pk));

            string sql1 = "insert into NAND_CS (NANDId, CSTypeId)	values (" + pk + ", 1)";

            sqlAccess.ExecuteNonQuery(view.Database.ConnectionString, sql1);

        }

        
    }
}
