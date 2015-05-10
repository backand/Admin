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

    public class IlegalProposalName : DuradosException
    {
        public IlegalProposalName(string message)
            : base(message)
        {
        }
    }

    public class CRMShadeProposalController : CRMShadeBaseController
    {
        protected override void BeforeCreate(CreateEventArgs e)
        {
            //AddOrganization((View)e.View, e.Values);
            //AddOwner(e.Values);
            if (e.Values[v_Proposal.Subject.ToString()].ToString().Contains("\""))
                throw new IlegalProposalName("Please do not use quotes (\") in the proposal name.");

            base.BeforeCreate(e);
        }

        protected override string FormatExceptionMessage(string viewName, string action, Exception exception)
        {
            return exception.Message;
        }
        //protected void AddOrganization(View view, Dictionary<string, object> values)
        //{
        //    string contactKey;
        //    if (view.Name == CRMViews.Proposal.ToString())
        //    {
        //        contactKey = values[Proposal.FK_Proposal_Contact_Parent.ToString()].ToString();
        //    }
        //    else
        //    {
        //        contactKey = values[v_ProposalLast2Months.V_Contact_v_ProposalLast2Months_Parent.ToString()].ToString();
        //    }

        //    string orgName;

        //    if (view.Name == CRMViews.Proposal.ToString())
        //    {
        //        orgName = Proposal.FK_Proposal_Organization_Parent.ToString();
        //    }
        //    else
        //    {
        //        orgName = v_ProposalLast2Months.FK_Proposal_Organization1_Parent.ToString();
        //    }


        //    if (!string.IsNullOrEmpty(contactKey))
        //    {
        //        string organizationKey = GetContactOrganizationKey(contactKey);
        //        values.Add(orgName, organizationKey);
        //    }
        //}

        //protected override string GetJsonViewSerialized(View view, DataAction dataAction, Durados.Web.Mvc.UI.Json.View jsonView)
        //{
        //    if (dataAction == DataAction.Create)
        //    {
        //        string ownerFieldName;
        //        if (view.Name == CRMViews.v_ProposalLast2Months.ToString())
        //            ownerFieldName = v_ProposalLast2Months.FK_Proposal_User_Parent.ToString();
        //        else
        //            ownerFieldName = Proposal.User_Proposal_Parent.ToString();

        //        if (jsonView.Fields.ContainsKey(ownerFieldName))
        //        {
        //            string userPK = GetUserRow()[Durados.Web.Mvc.Specifics.Projects.CRM.User.ID.ToString()].ToString();
        //            jsonView.Fields[ownerFieldName].Default = userPK;
        //        }
        //    }
        //    return base.GetJsonViewSerialized(view, dataAction, jsonView);
        //}

        //protected void AddOwner(Dictionary<string, object> values)
        //{
        //    if (User.IsInRole("User"))
        //    {
        //        string userPK = GetUserRow()[Durados.Web.Mvc.Specifics.Projects.CRM.User.ID.ToString()].ToString();
        //        string ownerFieldName = Proposal.User_Proposal_Parent.ToString(); 
        //        if (values.ContainsKey(ownerFieldName))
        //        {
        //            values[ownerFieldName] = userPK;
        //        }
        //        else
        //        {
        //            values.Add(ownerFieldName, userPK);
        //        }
        //    }
        //}

        //private string GetContactOrganizationKey(string contactKey)
        //{
        //    Durados.Web.Mvc.View contactView = GetView(CRMViews.V_Contact.ToString());
        //    DataRow dataRow = contactView.GetDataRow(contactKey);
        //    ParentField organizationField = (Durados.Web.Mvc.ParentField)contactView.Fields[V_Contact.FK_Contact_Organization_Parent.ToString()];
        //    return organizationField.GetValue(dataRow);
        //}

        protected override void BeforeEdit(EditEventArgs e)
        {
            //AddOrganization((View)e.View, e.Values);
            base.BeforeEdit(e);
        }

        protected override void AfterCreateAfterCommit(CreateEventArgs e)
        {
            base.AfterCreateAfterCommit(e);

            CreateDocument((View)e.View, e.Values, false);

        }

        protected override void AfterEditAfterCommit(EditEventArgs e)
        {
            base.AfterEditAfterCommit(e);

            string id = e.Values[v_Proposal.Id.ToString()].ToString();
            DataRow dataRow = ((View)e.View).GetDataRow(id);

            object pdf = dataRow[v_Proposal.PDFDocument.ToString()];
            if (pdf == null || pdf == (object)string.Empty || pdf == DBNull.Value)
                CreateDocument((View)e.View, e.Values, false);

        }

        protected virtual string GetTemplateForNew(View view, Dictionary<string, object> values)
        {
            string templateFieldName;
            //if (view.Name == CRMViews.Proposal.ToString())
            //    templateFieldName = Proposal.FK_Proposal_Template_Parent.ToString();
            //else
            //    templateFieldName = v_ProposalLast2Months.FK_Proposal_Template1_Parent.ToString();
            templateFieldName = v_Proposal.FK_Proposal_Template_Parent.ToString();

            string templatePK = values[templateFieldName].ToString();
            Durados.Web.Mvc.View templateView = GetView(ShadeViews.Template.ToString());
            string documentLocationFiledName = Template.DocumentLocation.ToString();
            ColumnField documentLocationField = (ColumnField)templateView.Fields[documentLocationFiledName];
            string virtualPath = documentLocationField.Upload.UploadVirtualPath;

            return GetTemplate(templatePK, templateView, documentLocationFiledName, virtualPath);
        }

        protected virtual string GetTemplateForNew(View view, string pk)
        {
            DataRow dataRow = view.GetDataRow(pk);

            string templateFieldName;
            //if (view.Name == CRMViews.Proposal.ToString())
            //    templateFieldName = Proposal.FK_Proposal_Template_Parent.ToString();
            //else
            //    templateFieldName = v_ProposalLast2Months.FK_Proposal_Template1_Parent.ToString();
            templateFieldName = v_Proposal.FK_Proposal_Template_Parent.ToString();

            ParentField templateField = (ParentField)view.Fields[templateFieldName];

            string templatePK = templateField.GetValue(dataRow);
            Durados.Web.Mvc.View templateView = GetView(ShadeViews.Template.ToString());
            string documentLocationFiledName = Template.DocumentLocation.ToString();
            ColumnField documentLocationField = (ColumnField)templateView.Fields[documentLocationFiledName];
            string virtualPath = documentLocationField.Upload.UploadVirtualPath;

            return GetTemplate(templatePK, templateView, documentLocationFiledName, virtualPath);
        }

        protected virtual string GetTemplateForEdit(View view, Dictionary<string, object> values)
        {
            string templatePK = values[v_Proposal.Id.ToString()].ToString();
            Durados.Web.Mvc.View templateView = GetView(ShadeViews.Template.ToString());
            string documentLocationFiledName = v_Proposal.WordDocument.ToString();
            ColumnField documentLocationField = (ColumnField)view.Fields[documentLocationFiledName];
            string virtualPath = documentLocationField.Upload.UploadVirtualPath;

            return GetTemplate(templatePK, view, v_Proposal.WordDocument.ToString(), virtualPath);
        }

        protected virtual string GetTemplate(View view, DataRow dataRow)
        {
            string templatePK = dataRow[v_Proposal.Id.ToString()].ToString();
            Durados.Web.Mvc.View templateView = GetView(ShadeViews.Template.ToString());
            string documentLocationFiledName = v_Proposal.WordDocument.ToString();
            ColumnField documentLocationField = (ColumnField)view.Fields[documentLocationFiledName];
            string virtualPath = documentLocationField.Upload.UploadVirtualPath;

            return GetTemplate(templatePK, view, v_Proposal.WordDocument.ToString(), virtualPath);
        }

        protected virtual string GetNewFile(View view, Dictionary<string, object> values)
        {
            string id = values[v_Proposal.Id.ToString()].ToString();

            string subject = values[v_Proposal.Subject.ToString()].ToString();

            Durados.Web.Mvc.ColumnField wordDocumentField = (Durados.Web.Mvc.ColumnField)view.Fields[v_Proposal.WordDocument.ToString()];

            return GetNewFile(id, subject, wordDocumentField);
        }

        protected override void PrepareAttachedFile(string physicalPath, string viewName, string pk)
        {
            PrepareDownloadFile(physicalPath, viewName, pk);
        }

        protected override void AfterEmailSent(string Subject, string To, string body, string file, string viewName, string pk)
        {
            View view = GetView(viewName);
            DataRow dataRow = view.GetDataRow(pk);

            Durados.Web.Mvc.ParentField jobField;
            jobField = (Durados.Web.Mvc.ParentField)view.Fields[v_Proposal.FK_Proposal_Job_Parent.ToString()];
            string jobFk = jobField.GetValue(dataRow);
            System.IO.FileInfo fileInfo = new System.IO.FileInfo(file);
            string fileName = fileInfo.Name;

            List<d_Field> fields = new List<d_Field>();
            fields.Add(new d_Field() { Name = "Note", Value = "Proposal Email: " +  fileName + ", sent to: " + To });
            fields.Add(new d_Field() { Name = "Auto", Value = "True" });
            fields.Add(new d_Field() { Name = "JobID", Value = jobFk });
            ViewHelper.AddRow("JobNotes", fields);

        }

        protected override void PrepareDownloadFile(string physicalPath, string viewName, string pk)
        {
            View view = GetView(viewName);
            Dictionary<string, object> blocksValues = GetBlocksValues(pk, view);

            string template = GetTemplateForNew(view, pk);


            string docx = physicalPath.Remove(physicalPath.Length - 3) + "docx";
            CreateDocument(docx, template, blocksValues);
            
            try
            {
                CreatePdf(docx);
            }
            catch (Exception e)
            {
                Durados.Web.Mvc.Logging.Logger.Log(this.ControllerContext.RouteData.Values["controller"].ToString(), this.ControllerContext.RouteData.Values["action"].ToString(), e.Source, e, 1, null);

            }

        }

        protected override DataView GetDataView(ChildrenField childrenField, DataRow dataRow)
        {
            DataView dataView = base.GetDataView(childrenField, dataRow);

            DataTable dataTable = new DataTable();
            
            string[] fieldsNames = new string[6] 
            {
               ProposalItem.FK_ProposalItem_ProposalItemType_Parent.ToString(), 
               ProposalItem.Description.ToString() ,
               ProposalItem.Color.ToString(),
               ProposalItem.Qty.ToString(),
               ProposalItem.Rate.ToString(),
               ProposalItem.Total.ToString()
            };

            foreach (string name in fieldsNames)
            {
                dataTable.Columns.Add(name);
            }


            foreach (DataRow row in dataView.Table.Rows)
            {
                List<object> values = new List<object>();
                foreach (string name in fieldsNames)
                {
                    string value = childrenField.ChildrenView.Fields[name].ConvertToString(row);
                    values.Add(value);
                }

                dataTable.Rows.Add(values.ToArray());
            }


            return new DataView(dataTable);
        }

        protected Dictionary<string, object> GetBlocksValues(View view, Dictionary<string, object> values)
        {
            string id = values[v_Proposal.Id.ToString()].ToString();
            return GetBlocksValues(id, view);
        }


        
        protected virtual void CreateDocument(View view, Dictionary<string, object> values, bool edit)
        {
            string id = values[v_Proposal.Id.ToString()].ToString();

            string newFile = GetNewFile(view, values);
            string template;
            if (edit)
            {
                template = GetTemplateForEdit(view, values);
            }
            else
            {
                template = GetTemplateForNew(view, values);
            }

            //Dictionary<string, object> blocksValues = GetBlocksValues(view, values);
            //CreateDocument(newFile, template, blocksValues);
            CreateDocument(newFile, template);

            string newFileName = (new System.IO.FileInfo(newFile)).Name;

            string pdf = newFileName.Remove(newFileName.Length - 4) + "pdf";

            
            UpdateRow(view, id, newFileName, pdf);
        }

        private void CreateDocument(string newFile, string template)
        {
            System.IO.File.Copy(template, newFile, true);
        }

        private void UpdateRow(View view, string id, string docx, string pdf)
        {
            Dictionary<string, object> fieldValues = new Dictionary<string, object>();
            fieldValues.Add(v_Proposal.WordDocument.ToString(), docx);
            fieldValues.Add(v_Proposal.PDFDocument.ToString(), pdf);

            view.Edit(fieldValues, id, null, null, null);
        }

        

        
        //protected override void AfterEditAfterCommit(EditEventArgs e)
        //{
        //    base.AfterEditAfterCommit(e);

        //    if (e.Values[v_Proposal.WordDocument.ToString()].ToString().EndsWith(".docx"))
        //        CreateDocument((View)e.View, e.Values, true);
        //}

        protected override string GetTemplateViewName()
        {
            return ShadeViews.durados_Html.ToString();
        }

        protected override string GetDocumentTemplatePK()
        {
            return "Proposal Body";
        }

        

        protected override string GetViewDisplayName(View view)
        {
            return "Proposals";
        }

        protected override string GetUserPK(View view, string pk, DataRow dataRow)
        {
            ParentField ownerField = GetOwnerField(view, pk);

            ParentField jobField = (Durados.Web.Mvc.ParentField)view.Fields[v_Proposal.FK_Proposal_Job_Parent.ToString()];
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
            return "Proposal Email Subject";
        }

        //protected override string GetEmailSubject(View view, DataRow row)
        //{
        //    ColumnField subjectField = (ColumnField)view.Fields[v_Proposal.Subject.ToString()];
        //    string subject = subjectField.GetValue(row);
        //    return subject;
        //}


        protected override string GetEmailView()
        {
            return "~/Views/Specifics/Controls/Email.ascx";
        }

        protected override void GetFrom(string userPk, out string from, out string fromNick)
        {
            from = "sales@shadesco.com";
            fromNick = "Sales Shadesco";

        }
        
        protected override string GetTo(string viewName, string pk)
        {
            View view = GetView(viewName);
            DataRow dataRow = view.GetDataRow(pk);

            Durados.Web.Mvc.ParentField jobField;
            Durados.Web.Mvc.ParentField contactField;
            //if (viewName == CRMViews.Proposal.ToString())
            //{
            //    contactField = (Durados.Web.Mvc.ParentField)view.Fields[Proposal.FK_Proposal_Contact_Parent.ToString()];
            //}
            //else
            //{
            //    contactField = (Durados.Web.Mvc.ParentField)view.Fields[v_ProposalLast2Months.V_Contact_v_ProposalLast2Months_Parent.ToString()];
            //}
            jobField = (Durados.Web.Mvc.ParentField)view.Fields[v_Proposal.FK_Proposal_Job_Parent.ToString()];
            string jobFk = jobField.GetValue(dataRow);
            Durados.Web.Mvc.View jobView = GetView(ShadeViews.Job.ToString());
            
            DataRow jobRow = jobView.GetDataRow(jobFk);


            contactField = (Durados.Web.Mvc.ParentField)jobView.Fields[Job.FK_V_Contact_Job_Parent.ToString()];

            string contactFk = contactField.GetValue(jobRow);

            Durados.Web.Mvc.View contactView = GetView(ShadeViews.V_Contact.ToString());
            DataRow contactRow = contactView.GetDataRow(contactFk);
            Field emailField = (Durados.Web.Mvc.ColumnField)contactView.Fields[V_Contact.Email.ToString()];
            string email = emailField.GetValue(contactRow);

            return email; 
        }

        protected override string GetBCC(string viewName, string pk)
        {
            return GetUserRow()[GetUserEmailFieldName()].ToString();
        }
        

        protected override ColumnField GetAttachmentField(View view, string pk)
        {
            DataRow dataRow = view.GetDataRow(pk);
            if (dataRow[v_Proposal.PDFDocument.ToString()] != null && dataRow[v_Proposal.PDFDocument.ToString()].ToString() != string.Empty)
                return (Durados.Web.Mvc.ColumnField)view.Fields[v_Proposal.PDFDocument.ToString()];
            else
                return (Durados.Web.Mvc.ColumnField)view.Fields[v_Proposal.WordDocument.ToString()];
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


