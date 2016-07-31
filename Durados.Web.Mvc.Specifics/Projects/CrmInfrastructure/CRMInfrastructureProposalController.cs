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

namespace Durados.Web.Mvc.App.Controllers
{

    [Authorize(Roles = "Developer, Admin, User")]
    public abstract class CRMInfrastructureProposalController : CRMInfrastructureBaseController
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
            //if (view.Name == CRMViews.Proposal.ToString())
            if (view.Name == ProposalViewName)
            {
                //contactKey = values[Proposal.FK_Proposal_Contact_Parent.ToString()].ToString();
                contactKey = values[Proposal_Contact_Parent].ToString();
            }
            else
            {
                //contactKey = values[v_ProposalLast2Months.V_Contact_v_ProposalLast2Months_Parent.ToString()].ToString();
                contactKey = values[Proposal_Last_Contact_Parent].ToString();
            }

            string orgName;

            //if (view.Name == CRMViews.Proposal.ToString())
            if (view.Name == ProposalViewName)
            {
                //orgName = Proposal.FK_Proposal_Organization_Parent.ToString();
                orgName = Proposal_Organization_Parent;
            }
            else
            {
                //orgName = v_ProposalLast2Months.FK_Proposal_Organization1_Parent.ToString();
                orgName = Proposal_Last_Organization_Parent;
            }


            if (!string.IsNullOrEmpty(contactKey))
            {
                string organizationKey = GetContactOrganizationKey(contactKey);
                values.Add(orgName, organizationKey);
            }
        }

        protected abstract string ProposalViewName
        {
            get;
        }

        protected abstract string ProposalLastViewName
        {
            get;
        }

        protected abstract string Proposal_Contact_Parent
        {
            get;
        }

        protected abstract string Proposal_Last_Contact_Parent
        {
            get;
        }

        protected abstract string Proposal_Organization_Parent
        {
            get;
        }

        protected abstract string Proposal_Last_Organization_Parent
        {
            get;
        }
       

        protected override string GetJsonViewSerialized(View view, DataAction dataAction, Durados.Web.Mvc.UI.Json.View jsonView)
        {
            if (dataAction == DataAction.Create)
            {
                string ownerFieldName;
                //if (view.Name == CRMViews.v_ProposalLast2Months.ToString())
                if (view.Name == ProposalLastViewName)
                    ownerFieldName = User_Proposal_Last_Parent;
                    //ownerFieldName = v_ProposalLast2Months.FK_Proposal_User_Parent.ToString();
                else
                    ownerFieldName = User_Proposal_Parent;
                    //ownerFieldName = Proposal.User_Proposal_Parent.ToString();

                if (jsonView.Fields.ContainsKey(ownerFieldName))
                {
                    //string userPK = GetUserRow()[User.ID.ToString()].ToString();
                    string userPK = GetUserRow()[User_PK].ToString();
                    jsonView.Fields[ownerFieldName].Default = userPK;
                }
            }
            //v_ProposalLast2Months
            return base.GetJsonViewSerialized(view, dataAction, jsonView);
        }

        protected abstract string User_Proposal_Parent
        {
            get;
        }

        protected abstract string User_Proposal_Last_Parent
        {
            get;
        }

        protected abstract string User_PK
        {
            get;
        }

        protected void AddOwner(Dictionary<string, object> values)
        {
            if (User.IsInRole("User"))
            {
                //string userPK = GetUserRow()[User.ID.ToString()].ToString();
                string userPK = GetUserRow()[User_PK].ToString();
                //string ownerFieldName = Proposal.User_Proposal_Parent.ToString();
                string ownerFieldName = User_Proposal_Parent; 
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
            //Durados.Web.Mvc.View contactView = GetView(CRMViews.V_Contact.ToString());
            Durados.Web.Mvc.View contactView = GetView(ContactViewName);
            DataRow dataRow = contactView.GetDataRow(contactKey);
            //ParentField organizationField = (Durados.Web.Mvc.ParentField)contactView.Fields[V_Contact.FK_Contact_Organization_Parent.ToString()];
            ParentField organizationField = (Durados.Web.Mvc.ParentField)contactView.Fields[Contact_Organization_Parent];
            return organizationField.GetValue(dataRow);
        }

        protected abstract string ContactViewName
        {
            get;
        }

        protected abstract string Contact_Organization_Parent
        {
            get;
        }
        
        protected override void BeforeEdit(EditEventArgs e)
        {
            AddOrganization((View)e.View, e.Values);
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
            //if (view.Name == CRMViews.Proposal.ToString())
            if (view.Name == ProposalViewName)
                //templateFieldName = Proposal.FK_Proposal_Template_Parent.ToString();
                templateFieldName = Proposal_Template_Parent;
            else
                templateFieldName = Proposal_Last_Template_Parent;
                //templateFieldName = v_ProposalLast2Months.FK_Proposal_Template1_Parent.ToString();

            string templatePK = values[templateFieldName].ToString();
            Durados.Web.Mvc.View templateView = GetView(TemplateViewName);
            //Durados.Web.Mvc.View templateView = GetView(CRMViews.Template.ToString());
            string documentLocationFiledName = DocumentLocationFieldName;
            //string documentLocationFiledName = Template.DocumentLocation.ToString();
            ColumnField documentLocationField = (ColumnField)templateView.Fields[documentLocationFiledName];
            string virtualPath = documentLocationField.Upload.UploadVirtualPath;

            return GetTemplate(templatePK, templateView, documentLocationFiledName, virtualPath);
        }

        protected abstract string Proposal_Template_Parent
        {
            get;
        }

        protected abstract string Proposal_Last_Template_Parent
        {
            get;
        }

        protected abstract string TemplateViewName
        {
            get;
        }

        protected abstract string DocumentLocationFieldName
        {
            get;
        }

        protected virtual string GetTemplateForEdit(View view, Dictionary<string, object> values)
        {
            //string templatePK = values[Proposal.Id.ToString()].ToString();
            string templatePK = values[Proposal_PK].ToString();
            //Durados.Web.Mvc.View templateView = GetView(CRMViews.Template.ToString());
            Durados.Web.Mvc.View templateView = GetView(TemplateViewName);
            //string documentLocationFiledName = Proposal.WordDocument.ToString();
            string documentLocationFiledName = WordDocumentFieldName;
            ColumnField documentLocationField = (ColumnField)view.Fields[documentLocationFiledName];
            string virtualPath = documentLocationField.Upload.UploadVirtualPath;

            return GetTemplate(templatePK, view, WordDocumentFieldName, virtualPath);
            //return GetTemplate(templatePK, view, Proposal.WordDocument.ToString(), virtualPath);
        }

        protected abstract string Proposal_PK
        {
            get;
        }

        protected abstract string WordDocumentFieldName
        {
            get;
        }

        protected virtual string GetNewFile(View view, Dictionary<string, object> values)
        {
            string id = values[Proposal_PK].ToString();
            //string id = values[Proposal.Id.ToString()].ToString();

            //string subject = values[Proposal.Subject.ToString()].ToString();
            string subject = values[SubjectFieldName].ToString();

            //Durados.Web.Mvc.ColumnField wordDocumentField = (Durados.Web.Mvc.ColumnField)view.Fields[Proposal.WordDocument.ToString()];
            Durados.Web.Mvc.ColumnField wordDocumentField = (Durados.Web.Mvc.ColumnField)view.Fields[WordDocumentFieldName];

            return GetNewFile(id, subject, wordDocumentField);
        }

        protected abstract string SubjectFieldName
        {
            get;
        }

        protected Dictionary<string, object> GetBlocksValues(View view, Dictionary<string, object> values)
        {
            //string id = values[Proposal.Id.ToString()].ToString();
            string id = values[Proposal_PK].ToString();
            return GetBlocksValues(id, view);
        }


        
        protected virtual void CreateDocument(View view, Dictionary<string, object> values, bool edit)
        {
            string id = values[Proposal_PK].ToString();
            //string id = values[Proposal.Id.ToString()].ToString();

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
            Dictionary<string, object> blocksValues = GetBlocksValues(view, values);
            CreateDocument(newFile, template, blocksValues);
            
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
            //fieldValues.Add(Proposal.WordDocument.ToString(), docx);
            //fieldValues.Add(Proposal.PDFDocument.ToString(), pdf);
            fieldValues.Add(WordDocumentFieldName, docx);
            fieldValues.Add(PDFDocumentFieldName, pdf);

            view.Edit(fieldValues, id, null, null, null);
        }


        protected abstract string PDFDocumentFieldName
        {
            get;
        }
        

        protected override void AfterEditAfterCommit(EditEventArgs e)
        {
            base.AfterEditAfterCommit(e);

            //if (e.Values[Proposal.WordDocument.ToString()].ToString().EndsWith(".docx"))
            if (e.Values[WordDocumentFieldName].ToString().EndsWith(".docx"))
                CreateDocument((View)e.View, e.Values, true);
        }

        protected override string GetTemplateViewName()
        {
            //return CRMViews.durados_Html.ToString();
            return HtmlViewName;
        }


        protected abstract string HtmlViewName
        {
            get;
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
            //return (ParentField)view.Fields[v_ProposalLast2Months.FK_Proposal_User_Parent.ToString()];
            return (ParentField)view.Fields[Proposal_Last_User_Parent];
        }

        protected abstract string Proposal_Last_User_Parent
        {
            get;
        }

        protected override string GetUserEmailFieldName()
        {
            return UserEmailFieldName;
            //return User.Email.ToString();
        }

        protected abstract string UserEmailFieldName
        {
            get;
        }

        protected override string GetDefaultEmailSubject()
        {
            return "EPL Proposal";
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
            //if (viewName == CRMViews.Proposal.ToString())
            if (viewName == ProposalViewName)
            {
                contactField = (Durados.Web.Mvc.ParentField)view.Fields[Proposal_Contact_Parent];
                //contactField = (Durados.Web.Mvc.ParentField)view.Fields[Proposal.FK_Proposal_Contact_Parent.ToString()];
            }
            else
            {
                contactField = (Durados.Web.Mvc.ParentField)view.Fields[Proposal_Last_Contact_Parent];
                //contactField = (Durados.Web.Mvc.ParentField)view.Fields[v_ProposalLast2Months.V_Contact_v_ProposalLast2Months_Parent.ToString()];
            }

            string contactFk = contactField.GetValue(dataRow);

            //Durados.Web.Mvc.View contactView = GetView(CRMViews.V_Contact.ToString());
            Durados.Web.Mvc.View contactView = GetView(ContactViewName);
            dataRow = contactView.GetDataRow(contactFk);
            Field emailField = (Durados.Web.Mvc.ColumnField)contactView.Fields[ContactEmailFieldName];
            //Field emailField = (Durados.Web.Mvc.ColumnField)contactView.Fields[V_Contact.Email.ToString()];
            string email = emailField.GetValue(dataRow);

            return email; 
        }

        protected abstract string ContactEmailFieldName
        {
            get;
        }

        protected override ColumnField GetAttachmentField(View view, string pk)
        {
            DataRow dataRow = view.GetDataRow(pk);
            //if (dataRow[Proposal.PDFDocument.ToString()] != null && dataRow[Proposal.PDFDocument.ToString()].ToString() != string.Empty)
            if (dataRow[PDFDocumentFieldName] != null && dataRow[PDFDocumentFieldName].ToString() != string.Empty)
                return (Durados.Web.Mvc.ColumnField)view.Fields[PDFDocumentFieldName];
            else
                return (Durados.Web.Mvc.ColumnField)view.Fields[WordDocumentFieldName];
        }

        protected override string GetUserFirstNameFieldName()
        {
            return FirstNameFieldName;
        }

        protected abstract string FirstNameFieldName
        {
            get;
        }

        protected override string GetUserLastNameFieldName()
        {
            return LastNameFieldName;
        }

        protected abstract string LastNameFieldName
        {
            get;
        }
    }
}


