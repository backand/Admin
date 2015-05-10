using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Durados.Web.Mvc.Specifics.Projects.CRMShade;

namespace Durados.Web.Mvc.Specifics.Projects.SanDisk.UI
{
    public class TtpStyler : Durados.Web.Mvc.UI.Styler
    {
        public TtpStyler()
            : base()
        {
        }

        public override string GetCellCss(Field field, System.Data.DataRow row)
        {
            return GetCellCss(field, (Durados.Web.Mvc.Specifics.Projects.SanDisk.Allegro1DataSet.v_TTPRow)row);
        }

        public string GetCellCss(Field field, Durados.Web.Mvc.Specifics.Projects.SanDisk.Allegro1DataSet.v_TTPRow row)
        {
            if (field.FieldType == FieldType.Parent && ((Durados.Web.Mvc.ParentField)field).GetDataColumns()[0].ColumnName == "StateId")
            {
                if (row.StateId == 2)
                {
                    return "alertTtpState";
                }
            }
            return base.GetCellCss(field, row);
        }

        public override string GetRowCss(View view, System.Data.DataRow row)
        {
            return GetRowCss(view, (Durados.Web.Mvc.Specifics.Projects.SanDisk.Allegro1DataSet.v_TTPRow)row);
        }

        public string GetRowCss(View view, Durados.Web.Mvc.Specifics.Projects.SanDisk.Allegro1DataSet.v_TTPRow row)
        {
            if (row.StateId == 4)
            {
                return "cancelTtpState";
            }
            return base.GetRowCss(view, row);
        } 
    }
}
