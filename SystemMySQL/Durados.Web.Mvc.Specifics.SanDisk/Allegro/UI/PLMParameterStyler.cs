using System;
using System.Data;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Durados.Web.Mvc.Specifics.SanDisk.Allegro.BusinessLogic;
using Durados.Web.Mvc.Specifics.SanDisk.Allegro.Controllers;
using System.Data.SqlClient;


namespace Durados.Web.Mvc.Specifics.SanDisk.Allegro.UI
{
    public class PLMParameterStyler : BETechnicalStyler
    {

        string beSysReqCloneViewName = "v_PLMparameterMode_BESysReq";
        CapabilitiesConfigComparer capComparer;
        public PLMParameterStyler(View view, DataView dataView)
            : base(view, dataView)
        {
            capComparer = new CapabilitiesParamModeComparer(view);
        }

        protected override Durados.Web.Mvc.Specifics.SanDisk.Allegro.BusinessLogic.BETchnicalChanges GetPlmChanges(View view, DataView dataView)
        {
            return new Durados.Web.Mvc.Specifics.SanDisk.Allegro.BusinessLogic.PlmParameterChanges();
        }

        protected override string GetPlmIDColumnName()
        {
            return "Id";
        }

        public override string GetCellCss(Field field, DataRow row)
        {
            //Durados.ParentField parentField = field.View.GetParentField("PLMId");
            //if (!string.IsNullOrEmpty(parentField.CloneParentViewName))
            //{

            if (field.View.Name == beSysReqCloneViewName)//parentField.CloneParentViewName ==beSysReqCloneViewName;
            {

                string yellowAlert = " cellYellowAlert";
                string paramVal = capComparer.GetCapabilitiesMismatch(field, row);
                if (!string.IsNullOrEmpty(paramVal))
                {
                    return yellowAlert;//
                }
            }

            return base.GetCellCss(field, row);
        }

 
        public override string GetAlt(Field field, DataRow row, string guid)
        {
            if (field.View.Name == beSysReqCloneViewName)//;
            {
                string altResult = capComparer.GetCapabilitiesMismatch(field, row);
                if (!string.IsNullOrEmpty(altResult))
                {
                    return "BE SYS Capability value is: " + altResult;
                }
            }
            return base.GetAlt(field, row, guid);
        }
    }
}