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
using Durados.Web.Mvc.Specifics.Projects.CRM;
using Durados.Web.Membership;

namespace Durados.Web.Mvc.App.Controllers
{

    public class CRMProposalController : CRMBaseController
    {
        protected override void BeforeCreate(CreateEventArgs e)
        {
            AddOrganization((View)e.View, e.Values);
            AddOwner(e.Values);
            base.BeforeCreate(e);
        }

        protected void AddOrganization(View view, Dictionary<string, object> values)
        {
            string contactKey;
            if (view.Name == CRMViews.Proposal.ToString())
            {
                contactKey = values[Proposal.FK_Proposal_Contact_Parent.ToString()].ToString();
            }
            else
            {
                contactKey = values[v_ProposalLast2Months.V_Contact_v_ProposalLast2Months_Parent.ToString()].ToString();
            }

            string orgName;

            if (view.Name == CRMViews.Proposal.ToString())
            {
                orgName = Proposal.FK_Proposal_Organization_Parent.ToString();
            }
            else
            {
                orgName = v_ProposalLast2Months.FK_Proposal_Organization1_Parent.ToString();
            }


            if (!string.IsNullOrEmpty(contactKey))
            {
                string organizationKey = GetContactOrganizationKey(contactKey);
                values.Add(orgName, organizationKey);
            }
        }

        protected override string GetJsonViewSerialized(View view, DataAction dataAction, Durados.Web.Mvc.UI.Json.View jsonView)
        {
            if (dataAction == DataAction.Create)
            {
                string ownerFieldName;
                if (view.Name == CRMViews.v_ProposalLast2Months.ToString())
                    ownerFieldName = v_ProposalLast2Months.FK_Proposal_User_Parent.ToString();
                else
                    ownerFieldName = Proposal.User_Proposal_Parent.ToString();

                if (jsonView.Fields.ContainsKey(ownerFieldName))
                {
                    string userPK = GetUserRow()[Durados.Web.Mvc.Specifics.Projects.CRM.User.ID.ToString()].ToString();
                    jsonView.Fields[ownerFieldName].Default = userPK;
                }
            }
            return base.GetJsonViewSerialized(view, dataAction, jsonView);
        }

        protected void AddOwner(Dictionary<string, object> values)
        {
            if (User.IsInRole("User"))
            {
                string userPK = GetUserRow()[Durados.Web.Mvc.Specifics.Projects.CRM.User.ID.ToString()].ToString();
                string ownerFieldName = Proposal.User_Proposal_Parent.ToString(); 
                if (values.ContainsKey(ownerFieldName))
                {
                    values[ownerFieldName] = userPK;
                }
                else
                {
                    values.Add(ownerFieldName, userPK);
                }
            }
        }

        private string GetContactOrganizationKey(string contactKey)
        {
            Durados.Web.Mvc.View contactView = GetView(CRMViews.V_Contact.ToString());
            DataRow dataRow = contactView.GetDataRow(contactKey);
            ParentField organizationField = (Durados.Web.Mvc.ParentField)contactView.Fields[V_Contact.FK_Contact_Organization_Parent.ToString()];
            return organizationField.GetValue(dataRow);
        }

        protected override void BeforeEdit(EditEventArgs e)
        {
            AddOrganization((View)e.View, e.Values);

            if (e.Values[Proposal.WordDocument.ToString()].ToString() == string.Empty)
            {
                DataRow row = e.View.GetDataRow(e.PrimaryKey);
                string wordDocument = row[Proposal.WordDocument.ToString()].ToString();
                e.Values[Proposal.WordDocument.ToString()] = wordDocument;
            }

            base.BeforeEdit(e);
        }

        protected override void AfterCreateAfterCommit(CreateEventArgs e)
        {
            base.AfterCreateAfterCommit(e);

            CreateDocument((View)e.View, e.Values, false);

        }

        protected virtual string GetTemplateForNew(View view, Dictionary<string, object> values)
        {
            

            string templateFieldName;
            if (view.Name == CRMViews.Proposal.ToString())
                templateFieldName = Proposal.FK_Proposal_Template_Parent.ToString();
            else
                templateFieldName = v_ProposalLast2Months.FK_Proposal_Template1_Parent.ToString();

            string templatePK = values[templateFieldName].ToString();
            Durados.Web.Mvc.View templateView = GetView(CRMViews.Template.ToString());
            string documentLocationFiledName = Template.DocumentLocation.ToString();
            ColumnField documentLocationField = (ColumnField)templateView.Fields[documentLocationFiledName];
            string virtualPath = documentLocationField.Upload.UploadVirtualPath;

            if (values[Proposal.WordDocument.ToString()].ToString() != string.Empty)
            {
                documentLocationFiledName = Proposal.WordDocument.ToString();
                documentLocationField = (ColumnField)view.Fields[documentLocationFiledName];
                virtualPath = documentLocationField.Upload.UploadVirtualPath;
                return HttpContext.Server.MapPath(virtualPath + values[Proposal.WordDocument.ToString()].ToString());
            }
            return GetTemplate(templatePK, templateView, documentLocationFiledName, virtualPath);
        }

        protected virtual string GetTemplateForEdit(View view, Dictionary<string, object> values)
        {
            string templatePK = values[Proposal.Id.ToString()].ToString();
            Durados.Web.Mvc.View templateView = GetView(CRMViews.Template.ToString());
            string documentLocationFiledName = Proposal.WordDocument.ToString();
            ColumnField documentLocationField = (ColumnField)view.Fields[documentLocationFiledName];
            string virtualPath = documentLocationField.Upload.UploadVirtualPath;

            return GetTemplate(templatePK, view, Proposal.WordDocument.ToString(), virtualPath);
        }

       
        protected virtual string GetNewFile(View view, Dictionary<string, object> values)
        {
            string id = values[Proposal.Id.ToString()].ToString();

            string subject = values[Proposal.Subject.ToString()].ToString();

            Durados.Web.Mvc.ColumnField wordDocumentField = (Durados.Web.Mvc.ColumnField)view.Fields[Proposal.WordDocument.ToString()];

            return GetNewFile(id, subject, wordDocumentField);
        }

        

        protected Dictionary<string, object> GetBlocksValues(View view, Dictionary<string, object> values)
        {
            string id = values[Proposal.Id.ToString()].ToString();
            return GetBlocksValues(id, view);
        }


        protected override void CreateDocument(string newFile, string template, Dictionary<string, object> blocksValues)
        {
            Durados.Xml.OpenXml openXml = new Durados.Xml.OpenXml();
            openXml.CreateDocument(newFile, template, blocksValues);

        }

        protected virtual void CreateDocument(View view, Dictionary<string, object> values, bool edit)
        {
            string id = values[Proposal.Id.ToString()].ToString();

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

            if (template.EndsWith(".docx"))
            {
                Dictionary<string, object> blocksValues = GetBlocksValues(view, values);
                CreateDocument(newFile, template, blocksValues);
            }
            else
            {
                System.IO.File.Copy(template, newFile, true);
            }
            string newFileName = (new System.IO.FileInfo(newFile)).Name;


            string pdf = string.Empty;

            try
            {

                pdf = CreatePdf(newFile);

                pdf = (new System.IO.FileInfo(pdf)).Name;
            }
            catch(Exception e) 
            {
                Durados.Web.Mvc.Logging.Logger.Log(this.ControllerContext.RouteData.Values["controller"].ToString(), this.ControllerContext.RouteData.Values["action"].ToString(), e.Source, e, 1, null);

            }

            UpdateRow(view, id, newFileName, pdf);
        }

        private void UpdateRow(View view, string id, string docx, string pdf)
        {
            Dictionary<string, object> fieldValues = new Dictionary<string, object>();
            fieldValues.Add(Proposal.WordDocument.ToString(), docx);
            fieldValues.Add(Proposal.PDFDocument.ToString(), pdf);

            view.Edit(fieldValues, id, null, null, null);
        }

        

        
        protected override void AfterEditAfterCommit(EditEventArgs e)
        {
            base.AfterEditAfterCommit(e);

            string word = e.Values[Proposal.WordDocument.ToString()].ToString();
            if (word.EndsWith(".docx") || word.EndsWith(".doc"))
                CreateDocument((View)e.View, e.Values, true);
        }

        protected override string GetTemplateViewName()
        {
            return CRMViews.durados_Html.ToString();
        }

        protected override string GetDocumentTemplatePK()
        {
            return "Proposel Letter";
        }

        

        protected override string GetViewDisplayName(View view)
        {
            return "Proposals";
        }

        protected override ParentField GetOwnerField(View view, string pk)
        {
            return (ParentField)view.Fields[v_ProposalLast2Months.FK_Proposal_User_Parent.ToString()];
        }

        protected override string GetUserEmailFieldName()
        {
            return Durados.Web.Mvc.Specifics.Projects.CRM.User.Email.ToString();
        }

        //protected override string GetDefaultEmailSubject()
        //{
        //    return "EPL Proposal";
        //}

        protected override string GetEmailSubject(View view, DataRow row)
        {
            ColumnField subjectField = (ColumnField)view.Fields[Proposal.Subject.ToString()];
            string subject = subjectField.GetValue(row);
            return subject;
        }


        protected override string GetEmailView()
        {
            return "~/Views/Specifics/Controls/Email.ascx";
        }

        
        
        protected override string GetTo(string viewName, string pk)
        {
            View view = GetView(viewName);
            DataRow dataRow = view.GetDataRow(pk);

            Durados.Web.Mvc.ParentField contactField;
            if (viewName == CRMViews.Proposal.ToString())
            {
                contactField = (Durados.Web.Mvc.ParentField)view.Fields[Proposal.FK_Proposal_Contact_Parent.ToString()];
            }
            else
            {
                contactField = (Durados.Web.Mvc.ParentField)view.Fields[v_ProposalLast2Months.V_Contact_v_ProposalLast2Months_Parent.ToString()];
            }

            string contactFk = contactField.GetValue(dataRow);

            Durados.Web.Mvc.View contactView = GetView(CRMViews.V_Contact.ToString());
            dataRow = contactView.GetDataRow(contactFk);
            Field emailField = (Durados.Web.Mvc.ColumnField)contactView.Fields[V_Contact.Email.ToString()];
            string email = emailField.GetValue(dataRow);

            return email; 
        }

        protected override string GetCC(string viewName, string pk)
        {
            return GetUserRow()[GetUserEmailFieldName()].ToString();
        }

        

        protected override ColumnField GetAttachmentField(View view, string pk)
        {
            DataRow dataRow = view.GetDataRow(pk);
            if (dataRow[Proposal.PDFDocument.ToString()] != null && dataRow[Proposal.PDFDocument.ToString()].ToString() != string.Empty)
                return (Durados.Web.Mvc.ColumnField)view.Fields[Proposal.PDFDocument.ToString()];
            else
                return (Durados.Web.Mvc.ColumnField)view.Fields[Proposal.WordDocument.ToString()];
        }

        protected override string GetUserFirstNameFieldName()
        {
            return Durados.Web.Mvc.Specifics.Projects.CRM.User.FirstName.ToString();
        }

        protected override string GetUserLastNameFieldName()
        {
            return Durados.Web.Mvc.Specifics.Projects.CRM.User.LastName.ToString();
        }
    }
}


