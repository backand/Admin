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

    public class CRMShadeJobVendorController : CRMShadeBaseController
    {
        
        protected override string GetUserPK(View view, string pk, DataRow dataRow)
        {
            ParentField ownerField = GetOwnerField(view, pk);

            ParentField jobField = (Durados.Web.Mvc.ParentField)view.Fields[v_JobVendor.FK_JobVendor_Job_Parent.ToString()];
            string jobFk = jobField.GetValue(dataRow);
            Durados.Web.Mvc.View jobView = GetView(ShadeViews.Job.ToString());

            DataRow jobRow = jobView.GetDataRow(jobFk);

            string userPk = ownerField.GetValue(jobRow);
            return userPk;
        }
        protected override ParentField GetOwnerField(View view, string pk)
        {
            return (ParentField)GetView(ShadeViews.Job.ToString()).Fields[Job.FK_User_Job_Parent.ToString()];
        }

        protected override string GetUserEmailFieldName()
        {
            return Durados.Web.Mvc.Specifics.Shade.CRM.User.Email.ToString();
        }

        protected override string GetDefaultEmailSubject()
        {
            return "Order Email Subject";
        }

        

        protected override string GetEmailView()
        {
            return "~/Views/Specifics/Controls/Email.ascx";
        }

        
        
        protected override string GetTo(string viewName, string pk)
        {
            return string.Empty;
        }

        protected override string GetBCC(string viewName, string pk)
        {
            return GetUserRow()[GetUserEmailFieldName()].ToString();
        }

        protected override string GetDocumentTemplatePK()
        {
            return "Order Letter";
        }


        protected override void AfterEmailSent(string Subject, string To, string body, string file, string viewName, string pk)
        {
            View view = GetView(viewName);
            DataRow dataRow = view.GetDataRow(pk);

            Durados.Web.Mvc.ParentField jobField;
            jobField = (Durados.Web.Mvc.ParentField)view.Fields[v_JobVendor.FK_JobVendor_Job_Parent.ToString()];
            string jobFk = jobField.GetValue(dataRow);
            System.IO.FileInfo fileInfo = new System.IO.FileInfo(file);
            string fileName = fileInfo.Name;

            List<d_Field> fields = new List<d_Field>();
            fields.Add(new d_Field() { Name = "Note", Value = "Email: " + fileName + ", sent to vendor: " + To });
            fields.Add(new d_Field() { Name = "Auto", Value = "true" });
            fields.Add(new d_Field() { Name = "JobID", Value = jobFk });
            ViewHelper.AddRow("JobNotes", fields);

        }

        protected override ColumnField GetAttachmentField(View view, string pk)
        {
            DataRow dataRow = view.GetDataRow(pk);
            return (Durados.Web.Mvc.ColumnField)view.Fields[v_JobVendor.OrderDocument.ToString()];
            
        }

        protected override string GetUserFirstNameFieldName()
        {
            return Durados.Web.Mvc.Specifics.Shade.CRM.User.FirstName.ToString();
        }

        protected override string GetUserLastNameFieldName()
        {
            return Durados.Web.Mvc.Specifics.Shade.CRM.User.LastName.ToString();
        }
    }
}


