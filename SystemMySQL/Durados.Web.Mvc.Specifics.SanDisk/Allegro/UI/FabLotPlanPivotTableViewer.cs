using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Globalization;
using System.Data;
using System.Data.SqlClient;

using Durados.Web.Mvc.UI.Helpers;

namespace Durados.Web.Mvc.Specifics.SanDisk.Allegro.UI
{
    public class FabLotPlanPivotTableViewer : FabLotTableViewer
    {

        

        protected override string GetLastUplaodFileViewName()
        {
            return "v_LastFabLotPlanUpload";
        }

        
    }
}
