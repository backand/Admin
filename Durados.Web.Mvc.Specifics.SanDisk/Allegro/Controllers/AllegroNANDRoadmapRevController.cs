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
    public class AllegroNANDRoadmapRevController : AllegroHomeController
    {
        protected override void Duplicate(View view, string pk, string duplicatePK)
        {
            Durados.DataAccess.SqlAccess sqlAccess = new SqlAccess();

            string sql = "exec CopyNandToNandRev {0}, {1}";

            sqlAccess.ExecuteNonQuery(view.Database.ConnectionString, string.Format(sql, duplicatePK, pk));

        }


    }
}
