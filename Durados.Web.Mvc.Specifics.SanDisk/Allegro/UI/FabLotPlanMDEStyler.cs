using System;
using System.Data;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace Durados.Web.Mvc.Specifics.SanDisk.Allegro.UI
{
    public class FabLotPlanMDEStyler : Durados.Web.Mvc.UI.Styler
    {
        public FabLotPlanMDEStyler(View view, DataView dataView)
            : base(view, dataView)
        {

        }

        public override string GetRowCss(View view, DataRow row, int rowIndex)
        {
            //if (rowIndex == 1)
            if (row.IsNull("EngRevisionId"))
            {
                return "d_disabled_row";
            }
            else
            {
                return base.GetRowCss(view, row, rowIndex);
            }
        }
    }
}
