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
using Durados.Web.Mvc.Specifics.Shade.CRM;
using Durados.Web.Membership;

namespace Durados.Web.Mvc.Specifics.Shade.CRM.Controllers
{

    public class CRMShadeBaseController : Durados.Web.Mvc.Controllers.CrmController
    {
        protected override void SetPermanentFilter(View view, Durados.DataAccess.Filter filter)
        {
            //int? userID = Durados.Web.Mvc.Specifics.Projects.User.GetUserID(User.Identity.Name);

            //if (view.Fields.ContainsKey("V_Contact_User_Parent"))
            //    view.Fields["V_Contact_User_Parent"].DefaultValue = userID;

            //if (view.Fields.ContainsKey("Task_User_Parent"))
            //    view.Fields["Task_User_Parent"].DefaultValue = userID;

            //if (view.Fields.ContainsKey("FK_Task_User_Parent"))
            //    view.Fields["FK_Task_User_Parent"].DefaultValue = userID;

            //if (view.Fields.ContainsKey("FK_Task_User1_Parent"))
            //    view.Fields["FK_Task_User1_Parent"].DefaultValue = userID;

            base.SetPermanentFilter(view, filter);
        }


        protected override void BeforeEdit(EditEventArgs e)
        {
            string[] views = new string[4] { ShadeViews.Job.ToString(), ShadeViews.V_Contact.ToString(), ShadeViews.v_Proposal.ToString(), ShadeViews.Organization.ToString() };
            if (views.Contains(e.View.Name))
            {
                Durados.ParentField parentField = e.View.GetParentField("d_LastUpdatedBy");
                if (parentField != null)
                {
                    string fieldName = parentField.Name;
                    if (!e.Values.ContainsKey(fieldName))
                    {
                        e.Values.Add(fieldName, ((Database)e.View.Database).GetUserID());
                    }
                    else
                    {
                        e.Values[fieldName] = ((Database)e.View.Database).GetUserID();
                    }
                }

                if (!e.Values.ContainsKey("d_LastUpdateDate"))
                {
                    e.Values.Add("d_LastUpdateDate", DateTime.Now.ToString());
                }
                else
                {
                    e.Values["d_LastUpdateDate"] = DateTime.Now.ToString();
                }
            }
            base.BeforeEdit(e);
        }
        
    }
}
