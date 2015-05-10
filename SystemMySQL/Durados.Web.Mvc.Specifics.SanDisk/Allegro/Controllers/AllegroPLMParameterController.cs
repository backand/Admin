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
using Durados.Web.Mvc.Specifics.SanDisk.Allegro.BusinessLogic;


namespace Durados.Web.Mvc.Specifics.SanDisk.Allegro.Controllers
{
    public class AllegroPLMParameterController : AllegroHomeController
    {
        protected override void BeforeEdit(EditEventArgs e)
        {
            base.BeforeEdit(e);
            PlmParameterChangesUtil plmParamUtil = new PlmParameterChangesUtil();
            if (plmParamUtil.IsInChangeRequest(e) && plmParamUtil.HasParametersChanged(e.View, e.PrevRow, e.Values))
            {
                plmParamUtil.UpdateBEResponseStatus( e);

            }
        }
       

        protected override Durados.Web.Mvc.UI.Styler GetNewStyler(View view, DataView dataView)
        {
            return new Durados.Web.Mvc.Specifics.SanDisk.Allegro.UI.PLMParameterStyler(view, dataView);
        }
        
    }
}
//   protected override void BeforeEdit(EditEventArgs e)