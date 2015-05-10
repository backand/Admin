using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Ajax;
using System.Data;
using System.IO;

using Durados;
using Durados.DataAccess;
using Durados.Web.Mvc.UI.Helpers;

namespace Durados.Web.Mvc.Controllers
{
    public class CrmController : DuradosController, Durados.Workflow.IDocument, Durados.Workflow.ILoader, Durados.Workflow.INotifier, Durados.Workflow.IApprovalProcess, Durados.Workflow.IExecuter
    {
        protected virtual string GetTemplate(string templatePK, Durados.Web.Mvc.View templateView, string documentLocationFiledName, string virtualPath)
        {
            DataRow dataRow = templateView.GetDataRow(templatePK);

            if (dataRow == null)
                throw new DuradosException("Missing document template!");

            virtualPath += templateView.GetDisplayValue(documentLocationFiledName, dataRow);

            string template = HttpContext.Server.MapPath(virtualPath);

            return template;
        }

        public virtual int GetCurrentUserId()
        {
            return Convert.ToInt32(Map.Database.GetUserID());
        }
        //public string GetSiteWithoutQueryString()
        //{
        //    return Request.Url.Scheme + "://" + Request.Url.Authority;
        //}

        public virtual string GetDocumentTemplateViewName()
        {
            return "DocumentTemplate";
        }

        public virtual string GetDocumentTemplateFileNameFieldName()
        {
            return "DocumentName";
        }

        public virtual string GetDocumentTemplateTransformationFileNameFieldName()
        {
            return "DocumentName";
        }

        public virtual string GetTemplate(Durados.View view, string templateViewName, string templatePK, string documentFieldName, string templateViewFileNameFieldName)
        {
            return GetTemplate((View)view, templateViewName, templatePK, documentFieldName, templateViewFileNameFieldName);
        }

        public virtual string GetTemplate(View view, string templateViewName, string templatePK, string documentFieldName, string templateViewFileNameFieldName)
        {
            Durados.Web.Mvc.View templateView = GetView(templateViewName);
            //ColumnField documentLocationField = (ColumnField)view.Fields[documentFieldName];
            ColumnField documentLocationField = (ColumnField)templateView.Fields[templateViewFileNameFieldName];
            if (documentLocationField.Upload == null)
            {
                throw new DuradosException("The field " + documentLocationField.DisplayName + " in view " + view.DisplayName + " must be an uplaod field.");
            }

            string virtualPath = documentLocationField.Upload.UploadVirtualPath;

            return GetTemplate(templatePK, templateView, templateViewFileNameFieldName, virtualPath);
        }


        protected virtual string GetNewFile(string id, string subject, Durados.Web.Mvc.ColumnField documentField)
        {

            string newFileVirtualPath = documentField.Upload.UploadVirtualPath;
            string path = HttpContext.Server.MapPath(newFileVirtualPath);
            string dot = ".";
            string newFileName = subject + dot + DateTime.Now.Date.ToString("dd.MM.yyyy") + string.Format("-{0}", id) + ".docx";
            string newFile = path + newFileName;

            return newFile;
        }

        public virtual string GetFileName(string pk, string template, Dictionary<string, object> blocksValues, string documentFileNameKey, Durados.View view, Dictionary<string, object> values)
        {
            Workflow.Notifier notifier = new Durados.Web.Mvc.Workflow.Notifier();

            return notifier.GetMessage(this, documentFileNameKey, view, values, pk, GetSiteWithoutQueryString(), GetMainSiteWithoutQueryString());
        }

        public virtual string GetFileName(Durados.View view, string documentFieldName, string fileName)
        {
            if (!view.Fields.ContainsKey(documentFieldName))
            {
                throw new DuradosException("The view " + view.DisplayName + " does not contains the document field name " + documentFieldName);
            }

            ColumnField documentField = (ColumnField)view.Fields[documentFieldName];

            string virtualPath = documentField.Upload.UploadVirtualPath;
            string path = HttpContext.Server.MapPath(virtualPath);

            return path + fileName;


        }

        public virtual void ExportAndSend(string viewName, string fileName, string subjectKey, string messageKey, string distributionListNames)
        {
            Durados.Web.Mvc.View view = GetView(viewName, "ExportAndSend");
            fileName = Export(viewName, fileName);
            string[][] distributionList = wfe.Notifier.GetDistributionLists(distributionListNames, (Database)Map.Database);
            if (distributionList == null || distributionList.Length == 0 || distributionList[0] == null || distributionList[0].Length == 0)
            {
                Map.Logger.Log(GetControllerNameForLog(this.ControllerContext), this.ControllerContext.RouteData.Values["action"].ToString(), "The distribution list " + distributionListNames + " contains no recipients. Export and Send was aborted.", null, 1, null);
                return;
            }
            string subject = GetSubject(view, subjectKey, 0);
            string message = GetSubject(view, subjectKey, 0);
            string from = Convert.ToString(System.Configuration.ConfigurationManager.AppSettings["fromAlert"]);
            subject = subject.Replace(Durados.Database.CurrentDatePlaceHolder, DateTime.Now.ToString(Database.DateFormat));
            message = message.Replace(Durados.Database.CurrentDatePlaceHolder, DateTime.Now.ToString(Database.DateFormat));

            SendEmail(from, distributionList[0], distributionList[1], message, subject, new string[1] { fileName });
        }

        public virtual void ExportAndSend2(int bookmarkId, string fileName, string subjectKey, string messageKey, string to, string from)
        {
            Durados.Bookmark bookmark = BookmarksHelper.GetBookmark(bookmarkId);
            if (bookmark == null)
                return;

            Durados.Web.Mvc.View view = GetView(bookmark.ViewName, "SubscribeBatch");
            if (view.Database.IsConfig)
                return;

            fileName = string.IsNullOrEmpty(fileName) ? bookmark.Name : fileName;

            string subject = string.IsNullOrEmpty(subjectKey) ? "Attached, please find " + fileName + "_" + DateTime.Now.ToString("yyyy-MM-dd") : GetSubject(view, subjectKey, 0);
            string message = string.IsNullOrEmpty(messageKey) ? "Please review " + fileName + "_" + DateTime.Now.ToString("yyyy-MM-dd") : GetSubject(view, messageKey, 0);
            from = string.IsNullOrEmpty(from) ? Convert.ToString(System.Configuration.ConfigurationManager.AppSettings["fromAlert"]) : from;
            subject = subject.Replace(Durados.Database.CurrentDatePlaceHolder, DateTime.Now.ToString(Database.DateFormat));
            message = message.Replace(Durados.Database.CurrentDatePlaceHolder, DateTime.Now.ToString(Database.DateFormat));

            if (string.IsNullOrEmpty(to))
                return;

            fileName = fileName + "_" + Guid.NewGuid();

            fileName = Export(view, GetDataViewByBookmark(bookmarkId), fileName);

            SendEmail(from, to.Split(';'), new string[0], message, subject, new string[1] { fileName });
        }

        public Dictionary<string, object> GetBlocksValues(string id, Durados.View view)
        {
            return GetBlocksValues(id, (View)view);
        }

        public string GetDocumentFieldName(Durados.View view)
        {
            return null;
        }

        public string GetTemplateFieldName(Durados.View view)
        {
            return null;
        }

        public string GetTemplate(Durados.View view, Dictionary<string, object> values, DataRow prevRow, string pk)
        {
            return null;
        }

        public string GetTransformationFile(Durados.View view, Dictionary<string, object> values, DataRow prevRow, string pk)
        {
            return null;
        }

        public Dictionary<string, object> GetBlocksValues(string id, View view)
        {
            Dictionary<string, object> blocksValues = new Dictionary<string, object>();
            DataRow dataRow = view.GetDataRow(id);

            Dictionary<string, Durados.Workflow.DictionaryField> dicFields = new Dictionary<string, Durados.Workflow.DictionaryField>();
            LoadValues(blocksValues, dataRow, view, null, view, view.DisplayName + ".", string.Empty, string.Empty, dicFields, view.Name + ".");


            return blocksValues;
        }

        public void UpdateDocumentRow(Durados.View view, string id, string fieldName, string filename, System.Data.IDbCommand command)
        {
            Map map = Maps.Instance.GetMap();
            SqlSchema sqlSchema = new SqlSchema();

            using (IDbConnection connection = sqlSchema.GetConnection(map.systemConnectionString))
            {
                IDbCommand sysCommand = new System.Data.SqlClient.SqlCommand();
                sysCommand.Connection = connection;
                connection.Open();
                UpdateDocumentRow((View)view, id, fieldName, filename, command, sysCommand);
            }
        }

        public void UpdateDocumentRow(View view, string id, string fieldName, string filename, System.Data.IDbCommand command, System.Data.IDbCommand sysCommand)
        {
            Dictionary<string, object> fieldValues = new Dictionary<string, object>();
            fieldValues.Add(fieldName, filename);

            SqlAccess sqlAccess = new SqlAccess();
            sqlAccess.Edit(view, fieldValues, id, null, null, null, null, (System.Data.SqlClient.SqlCommand)command, (System.Data.SqlClient.SqlCommand)sysCommand, null, null);
            //view.Edit(fieldValues, id, null, null, null);
        }

        protected virtual Durados.Xml.Schema.Converter GetXmlConverter()
        {
            return new Durados.Xml.Schema.Converter();
        }

        protected virtual Durados.Xml.Schema.Transformer GetXmlTransformer()
        {
            return new Durados.Xml.Schema.Transformer();
        }

        public virtual void CreateDocument(string newFile, object template, object data, Durados.Workflow.DocumentType documentType, Durados.View view)
        {
            switch (documentType)
            {
                case Durados.Workflow.DocumentType.Word:
                    CreateWordDocument(newFile, (string)template, data, view);

                    break;
                case Durados.Workflow.DocumentType.Xml:
                    CreateXmlDocument(newFile, (Durados.Workflow.XmlTemplate)template, data, view);

                    break;
                default:
                    break;
            }
        }

        protected virtual void CreateWordDocument(string newFile, string template, object data, Durados.View view)
        {
            Dictionary<string, object> blocksValues = (Dictionary<string, object>)data;
            Durados.Xml.Sdk.Word.Filler filler = new Durados.Xml.Sdk.Word.Filler();
            filler.Fill(newFile, template, blocksValues);
        }

        protected virtual void CreateXmlDocument(string newFile, Durados.Workflow.XmlTemplate template, object data, Durados.View view)
        {
            DataSet ds = CreateXmlDataset(template.Schema, data, view);

            ds.Namespace = null;

            ds.WriteXml(newFile);

            string outputFile = GetXmlDocumentName(newFile);

            TransformXml(newFile, template.Xslt, outputFile);

            template.Ouput = outputFile;
        }

        protected virtual void TransformXml(string xsd, string xsl, string outputFile)
        {

            Durados.Xml.Schema.Transformer transformer = GetXmlTransformer();

            transformer.Tranform(xsd, xsl, outputFile, Durados.Xml.Schema.TransformationType.Xslt);

        }

        protected virtual string GetXmlDocumentName(string xsd)
        {
            return xsd.TrimEnd(".xsd".ToCharArray()) + ".xml";
        }

        protected virtual DataSet CreateXmlDataset(string template, object data, Durados.View view)
        {
            DataRow row = (DataRow)data;
            Durados.Xml.Schema.Converter converter = GetXmlConverter();
            return converter.Convert(view, row, template);
        }


        protected virtual string CreatePdf(string docx)
        {
            string docxExtension = (new System.IO.FileInfo(docx)).Extension;
            string pdf = docx.Replace(docxExtension, ".pdf");

            Durados.Xml.OpenXml openXml = new Durados.Xml.OpenXml();
            openXml.DocToPdfOffice2010(docx, pdf);

            return pdf;
        }

        public override void LoadValues(Dictionary<string, object> values, DataRow dataRow, Durados.View view, Durados.ParentField parentField, Durados.View rootView, string dynastyPath, string prefix, string postfix, Dictionary<string, Durados.Workflow.DictionaryField> dicFields, string internalDynastyPath)
        {
            if (view.Equals(rootView))
            {
                dynastyPath = GetViewDisplayName((View)view) + ".";
                internalDynastyPath = view.Name + ".";
            }
            foreach (Field field in view.Fields.Values.Where(f => f.FieldType == FieldType.Column))
            {
                LoadValue(values, dataRow, view, field, dynastyPath, prefix, postfix, dicFields, internalDynastyPath);
            }

            var childrenFields = view.Fields.Values.Where(f => f.FieldType == FieldType.Children && ((ChildrenField)f).LoadForBlockTemplate);
            foreach (ChildrenField field in childrenFields)
            {
                string name = prefix + dynastyPath + field.DisplayName + postfix;
                string internalName = prefix + internalDynastyPath + field.Name + postfix;
                DataView value = GetDataView(field, dataRow);
                if (!values.ContainsKey(name))
                {
                    values.Add(name, value);
                    dicFields.Add(internalDynastyPath, new Durados.Workflow.DictionaryField { DisplayName = field.DisplayName, Type = field.DataType, Value = value });
                }

                foreach (ColumnField columnField in field.ChildrenView.Fields.Values.Where(f => f.FieldType == FieldType.Column))
                {
                    if (columnField.Upload != null)
                    {
                        value.Table.Columns[columnField.Name].ExtendedProperties["ImagePath"] = columnField.GetUploadPath();
                    }
                }
            }

            foreach (ParentField field in view.Fields.Values.Where(f => f.FieldType == FieldType.Parent))
            {
                if (view.Equals(rootView))
                {
                    dynastyPath = view.DisplayName + ".";
                    internalDynastyPath = view.Name + ".";
                }
                LoadValue(values, dataRow, view, field, dynastyPath, prefix, postfix, dicFields, internalDynastyPath);



                DataRow parentRow = dataRow.GetParentRow(field.DataRelation.RelationName);
                View parentView = (View)field.ParentView;
                if (parentRow == null)
                {
                    string key = field.GetValue(dataRow);
                    if (!string.IsNullOrEmpty(key))
                        parentRow = parentView.GetDataRow(key, dataRow.Table.DataSet);
                }
                if (parentRow != null && parentField != field)
                {
                    if (parentView != rootView)
                    {
                        //dynastyPath += field.DisplayName + ".";
                        dynastyPath = GetDynastyPath(dynastyPath, (ParentField)parentField, field);
                        internalDynastyPath = GetInternalDynastyPath(internalDynastyPath, (ParentField)parentField, field);
                        LoadValues(values, parentRow, parentView, field, rootView, dynastyPath, prefix, postfix, dicFields, internalDynastyPath);
                    }
                }
            }


        }

        public Durados.View GetNonConfigView(string viewName)
        {
            return GetView(viewName);
        }

        public Durados.Workflow.Notifier GetNotifier()
        {
            return wfe.Notifier;
        }

        public string GetPreviousValue(string viewName, string fieldName, string pk, string currentValue)
        {
            History history = new History();

            return history.GetPreviousValue(viewName, fieldName, pk, currentValue, Map.Database.ConnectionString);
        }

        protected virtual DataView GetDataView(ChildrenField childrenField, DataRow dataRow)
        {
            return childrenField.GetDataView(dataRow);
        }

        public virtual DataView GetDataView(Durados.ChildrenField childrenField, string pk)
        {
            return childrenField.GetDataView(pk);
        }

        public void LoadValue(Dictionary<string, object> values, DataRow dataRow, Durados.View view, Durados.Field field, string dynastyPath, string prefix, string postfix, Dictionary<string, Durados.Workflow.DictionaryField> dicFields, string internalDynastyPath)
        {
            string name = prefix + dynastyPath + field.DisplayName + postfix;
            string InternalName = prefix + internalDynastyPath + field.Name + postfix;
            string value = view.GetDisplayValue(field.Name, dataRow);
            if (!values.ContainsKey(name))
            {
                values.Add(name, value);
                dicFields.Add(InternalName, new Durados.Workflow.DictionaryField { DisplayName = name, Type = field.DataType, Value = value });

            }
            if (field.FieldType == FieldType.Column && ((ColumnField)field).Upload != null)
            {
                if (dataRow.Table.Columns.Contains(field.Name))
                {

                    dataRow.Table.Columns[field.Name].ExtendedProperties["ImagePath"] = ((ColumnField)field).GetUploadPath();
                }
            }
        }


        protected virtual string GetDynastyPath(string dynastyPath, ParentField parentField, ParentField field)
        {
            if (parentField == null)
                return dynastyPath + field.DisplayName + ".";

            string[] s = dynastyPath.Split('.');

            for (int i = s.Length - 1; i >= 0; i--)
            {
                if (s[i] == parentField.DisplayName)
                {
                    string r = string.Empty;
                    for (int j = 0; j <= i; j++)
                    {
                        r += s[j] + ".";
                    }
                    return r + field.DisplayName + ".";
                }
            }

            return dynastyPath += field.DisplayName + ".";
        }

         protected virtual string GetInternalDynastyPath(string dynastyPath, ParentField parentField, ParentField field)
        {
            if (parentField == null)
                return dynastyPath + field.Name + ".";

            string[] s = dynastyPath.Split('.');

            for (int i = s.Length - 1; i >= 0; i--)
            {
                if (s[i] == parentField.Name)
                {
                    string r = string.Empty;
                    for (int j = 0; j <= i; j++)
                    {
                        r += s[j] + ".";
                    }
                    return r + field.Name + ".";
                }
            }

            return dynastyPath += field.Name + ".";
        }



        protected virtual string GetDocumentTemplatePK(View view, DataRow row)
        {
            return view.SendTemplate;

        }

        protected virtual string GetTemplate(View view, DataRow row, out bool replace)
        {
            string templatePK = GetDocumentTemplatePK(view, row);

            if (string.IsNullOrEmpty(templatePK))
            {
                replace = false;
                return view.GetLink(row, false);
            }

            replace = true;
            return GetTemplate(GetTemplateViewName(), templatePK);
        }

        protected virtual string GetUserEmailFieldName()
        {
            return "Email";
        }

        protected virtual string GetDefaultEmailSubject(View view, DataRow row)
        {
            return view.DisplayName + " - " + view.GetDisplayValue(row);
        }

        protected virtual string GetEmailSubject(View view, DataRow row)
        {
            string viewName = GetTemplateViewName();
            string pk = GetDefaultEmailSubject(view, row);

            Durados.Web.Mvc.View templateView = GetView(viewName);
            DataRow templateRow = templateView.GetDataRow(pk);

            string subject = templateView.GetDisplayValue(GetEmailTemplateFieldName(), templateRow);

            return subject;
        }

         protected virtual string GetEmailSubject(View view, DataRow row, Dictionary<string, object> values, Dictionary<string, object> values2)
        {
            if (string.IsNullOrEmpty(view.SendSubject))
                return GetDefaultEmailSubject(view, row);

            return view.SendSubject.Replace(values).Replace(values2);
        }

        protected virtual string GetEmailView()
        {
            return "~/Views/Specifics/Controls/Email.ascx";
        }

        protected virtual string GetTo(View view, string pk, Dictionary<string, object> values)
        {
            if (string.IsNullOrEmpty(view.SendTo))
                return string.Empty;
            return view.SendTo.Replace(values, new UI.LetterConverter(), view);
        }

        protected virtual string GetCC(View view, string pk, Dictionary<string, object> values)
        {
            if (string.IsNullOrEmpty(view.SendCc))
                return string.Empty;
            return view.SendCc.Replace(values);
        }
        protected virtual string GetBCC(View view, string pk, Dictionary<string, object> values)
        {
            return string.Empty;
        }

        protected virtual string GetTo(string viewName, DataRow dataRow)
        {
            return string.Empty;
        }


        protected virtual Durados.Cms.Model.Email GetEmail(string viewName, string pks)
        {
            View view = GetView(viewName);

            string[] pkArray = pks.Split(',');

            string body = string.Empty;

            string subject = string.Empty;

            string to = string.Empty;
            string cc = string.Empty;
            string bcc = string.Empty;

            Dictionary<string, string> toDictionary = new Dictionary<string, string>();
            Dictionary<string, string> ccDictionary = new Dictionary<string, string>();
            Dictionary<string, string> bccDictionary = new Dictionary<string, string>();

            string recipient = null;
            DataTable table = view.GetDataRows(pkArray);
            string pkFieldName = ((Durados.Web.Mvc.View)view).PrimaryKeyFileds[0].Name;
            foreach (DataRow row in table.Select().Where(r => pkArray.Contains(r[pkFieldName].ToString())))
            {
                string pk = view.GetPkValue(row);
                bool replace = false;
                string template = GetTemplate(view, row, out replace);
                Dictionary<string, object> nameValueDictionary = null;
                Dictionary<string, object> internalNameValueDictionary = null;

                if (replace)
                {
                    nameValueDictionary = new Dictionary<string, object>();

                    string viewDisplayName = GetViewDisplayName(view);
                     Dictionary<string, Durados.Workflow.DictionaryField> dicFields = new Dictionary<string, Durados.Workflow.DictionaryField>();
                    LoadValues(nameValueDictionary, row, view, null, view, viewDisplayName + ".", "[", "]", dicFields, view.Name + ".");
                    internalNameValueDictionary = dicFields.ToDictionary(k => k.Key, v => ((Durados.Workflow.DictionaryField)v.Value).Value);
                    template = template.Replace(nameValueDictionary);
                    template = template.Replace(internalNameValueDictionary);
                }

                body += template + "\n";

                subject += GetEmailSubject(view, row, nameValueDictionary, internalNameValueDictionary) + ", ";

                recipient = GetTo(view, pk, nameValueDictionary);
                if (!string.IsNullOrEmpty(recipient) && !toDictionary.ContainsKey(recipient))
                {
                    toDictionary.Add(recipient, recipient);
                    to += recipient + ";";
                }
                recipient = GetCC(view, pk, nameValueDictionary);
                if (!string.IsNullOrEmpty(recipient) && !ccDictionary.ContainsKey(recipient))
                {
                    ccDictionary.Add(recipient, recipient);
                    cc = recipient + ";";
                }
                recipient = GetBCC(view, pk, nameValueDictionary);
                if (!string.IsNullOrEmpty(recipient) && !bccDictionary.ContainsKey(recipient))
                {
                    bccDictionary.Add(recipient, recipient);
                    bcc = recipient + ";";
                }
            }

            subject = subject.TrimEnd(", ".ToCharArray());

            if (subject.Length > 255)
                subject = subject.Substring(0, 250) + "...";

            Durados.Cms.Model.Email email = new Durados.Cms.Model.Email() { Body = body, Subject = subject, To = to, Cc = cc, BCc = bcc };

            return email;
        }

        protected virtual string GetViewDisplayName(View view)
        {
            return view.DisplayName;
        }

        public virtual ActionResult GetLetter(string viewName, string pk)
        {
            return PartialView(GetEmailView(), GetEmail(viewName, pk));
        }

        public virtual JsonResult GetJsonLetter(string viewName, string pks)
        {
            return Json(GetEmail(viewName, pks));
        }

        protected virtual ParentField GetOwnerField(View view, string pk)
        {
            //throw new NotImplementedException("Please inherit and override");
            return null;
        }

        protected virtual ColumnField GetAttachmentField(View view, string pk)
        {
            foreach (ColumnField field in view.Fields.Values.Where(f => f.FieldType == FieldType.Column))
            {
                if (field.Upload != null)
                    return field;
            }

            return null;
        }

        //[ValidateInput(false)]
        //[AcceptVerbs(HttpVerbs.Post)]
        public virtual ActionResult SendMultiRows(string viewName, string pks, string body, string subject, string to, string cc, string bcc)
        {
            View view = GetView(viewName);
            string userPk = Map.Database.GetUserID();


            string from;
            string fromNick;
            GetEmailInfo(userPk, out from, out fromNick);
            if (bcc == null)
                bcc = string.Empty;
            if (cc == null)
                cc = string.Empty;

            SendProposalEmail(from, fromNick, to, cc, bcc, body, subject, null);
            //run after email send command to log messages if needed
            AfterEmailSent(subject, to, body, null, viewName, pks);

            return PartialView("~/Views/Shared/Controls/TextArea.ascx", string.Empty);
        }

        [ValidateInput(false)]
        [AcceptVerbs(HttpVerbs.Post)]
        public virtual ActionResult Send(string viewName, string pk, string body, string subject, string to, string cc, string bcc)
        {
            try
            {
                if (IsMultiRows(viewName, pk))
                {
                    return RedirectToAction("SendMultiRows", new { viewName = viewName, pks = pk, body = body, subject = subject, to = to, cc = cc, bcc = bcc });
                }
                //DataRow proposalRow = ProposalView.GetDataRow(pk);
                //Durados.Web.Mvc.ColumnField wordDocumentField = (Durados.Web.Mvc.ColumnField)ProposalView.Fields[Proposal.WordDocument.ToString()];

                View view = GetView(viewName);
                DataRow dataRow = view.GetDataRow(pk);
                ColumnField attachmentField = GetAttachmentField(view, pk);
                string file = "";
                if (attachmentField != null)
                {
                    string fileVirtualPath = attachmentField.Upload.UploadVirtualPath;
                    string path = HttpContext.Server.MapPath(fileVirtualPath);
                    string fileName = attachmentField.GetValue(dataRow);
                    file = path + fileName;
                    PrepareAttachedFile(file, viewName, pk);
                }
                string userPk = GetUserPK(view, pk, dataRow);

                PrepareAttachedFile(file, viewName, pk);

                string from;
                string fromNick;
                GetEmailInfo(userPk, out from, out fromNick);
                if (bcc == null)
                    bcc = string.Empty;

                SendProposalEmail(from, fromNick, to, cc, bcc, body, subject, file);
                //run after email send command to log messages if needed
                AfterEmailSent(subject, to, body, file, viewName, pk);

                return PartialView("~/Views/Shared/Controls/TextArea.ascx", string.Empty);
            }
            catch (Exception exception)
            {
                HandleSendException(viewName, pk, exception);
                return PartialView("~/Views/Shared/Controls/ErrorMessage.ascx", FormatExceptionMessage(viewName, "Send", exception));

            }
        }

        private bool IsMultiRows(string viewName, string pk)
        {
            View view = GetView(viewName);
            return (pk.Contains(',') && view.DataTable.PrimaryKey.Length == 1);
        }

        protected virtual void HandleSendException(string viewName, string pk, Exception exception)
        {
            throw new DuradosException(exception.Message);
        }

        public virtual Durados.ChildrenField GetApprovalProcessUsersChildrenField(Durados.View view)
        {
            return null;
        }

        public virtual string GetApprovalProcessUserViewName()
        {
            return "v_durados_ApprovalProcessUser";
        }

        public virtual string GetApprovalProcessMessageKey(Durados.View view, string pk)
        {
            return "Approval Message";
        }

        public virtual string GetApprovalProcessSubjectKey(Durados.View view, string pk)
        {
            return "Approval Message";
        }


        public virtual string GetMessage(Durados.View view, string messageKey, int id)
        {
            return wfe.Notifier.GetMessage(this, messageKey, view, null, id.ToString(), GetSiteWithoutQueryString(), GetMainSiteWithoutQueryString());
        }

        public virtual string GetSubject(Durados.View view, string subjectKey, int id)
        {
            return wfe.Notifier.GetSubject(this, subjectKey, view, null, id.ToString());
        }


        public virtual string GetApprovalProcessStoredProcedureName(Durados.View view)
        {
            return "durados_CreateApprovalProcess";
        }

        public virtual Durados.View GetApprovalProcessView(Durados.View view)
        {
            if (!Database.Views.ContainsKey("durados_ApprovalProcess"))
            {
                throw new Durados.Workflow.WorkflowEngineException("Colud not found the view durados_ApprovalProcess in the database.");
            }
            return Database.Views["durados_ApprovalProcess"];
        }

        public virtual void GetEmailInfo(string userPk, out string email, out string fullname)
        {
            email = string.Empty;
            fullname = string.Empty;
            if (!string.IsNullOrEmpty(userPk))
            {
                DataRow userRow = GetUserRow(userPk);

                if (userRow != null)
                {
                    email = userRow[GetUserEmailFieldName()].ToString();
                    // if(email.ToLower()=="durados")email= Convert.ToString(System.Configuration.ConfigurationManager.AppSettings["fromAlert"]);
                    fullname = userRow[GetUserFirstNameFieldName()].ToString() + " " + userRow[GetUserLastNameFieldName()].ToString();
                }
            }

        }

        protected virtual void PrepareAttachedFile(string physicalPath, string viewName, string pk)
        {

        }

        protected virtual void AfterEmailSent(string Subject, string To, string body, string file, string viewName, string pk)
        {

        }


        protected virtual string GetUserPK(View view, string pk, DataRow dataRow)
        {
            ParentField ownerField = GetOwnerField(view, pk);
            if (ownerField == null)
                return Map.Database.GetUserID();
            string userPk = ownerField.GetValue(dataRow);
            return userPk;
        }

        protected virtual string GetUserFirstNameFieldName()
        {
            return "FirstName";
        }

        protected virtual string GetUserLastNameFieldName()
        {
            return "LastName";
        }


        protected virtual void SendProposalEmail(string from, string fromNick, string to, string cc, string bcc, string message, string subject, string file)
        {

            string host = Convert.ToString(System.Configuration.ConfigurationManager.AppSettings["host"]);
            int port = Convert.ToInt32(System.Configuration.ConfigurationManager.AppSettings["port"]);
            string username = Convert.ToString(System.Configuration.ConfigurationManager.AppSettings["username"]);
            string password = Convert.ToString(System.Configuration.ConfigurationManager.AppSettings["password"]);

            Durados.Cms.DataAccess.Email.Send(host, Map.Database.UseSmtpDefaultCredentials, port, username, password, false, to.Split(';'), cc.Split(';'), bcc.Split(';'), subject, message, from, fromNick, null, DontSend, new string[1] { file }, Map.Database.Logger);
        }

        protected virtual Dictionary<string, object> GetSendAlertsFilter(string viewName)
        {

            return new Dictionary<string, object>();
        }
        protected virtual string GetSendAlertsSordColumn(string viewName)
        {
            return "ReminderDate";
        }

        public void SendAlerts(string viewName, int? pageSize)
        {
            int i = 0;
            View view = GetView(viewName);
            int size = (pageSize.HasValue) ? pageSize.Value : 50;
            int rowCount = 0;

            Dictionary<string, SortDirection> sortColumns = new Dictionary<string, SortDirection>();
            string sortColumn = GetSendAlertsSordColumn(viewName);
            if (!string.IsNullOrEmpty(sortColumn))
            {
                sortColumns.Add(sortColumn, SortDirection.Asc);
            }

            DataView dataView = view.FillPage(1, size, GetSendAlertsFilter(viewName), false, sortColumns, out rowCount, alertView_BeforeSelect, null);
            try
            {
                foreach (DataRow dataRow in dataView.Table.Rows)
                {
                    SendAlert(viewName, dataRow);
                    i++;
                }

                Map.Logger.Log(GetControllerNameForLog(this.ControllerContext), this.ControllerContext.RouteData.Values["action"].ToString(), null, null, 3, "sent " + i.ToString() + " out of " + dataView.Table.Rows.Count.ToString() + " letters");

            }
            catch (Exception ex)
            {
                Map.Logger.Log(GetControllerNameForLog(this.ControllerContext), this.ControllerContext.RouteData.Values["action"].ToString(), ex.Source, ex, 1, "sent " + i.ToString() + " out of " + dataView.Table.Rows.Count.ToString() + " letters");
            }
        }



        protected virtual string GetAlertTemplatePK()
        {
            return "Reminder Letter";
        }

        protected virtual string GetDefaultAlertSubject()
        {
            return "Reminder";
        }


        protected virtual void SendAlert(string viewName, DataRow dataRow)
        {
            string template = GetTemplate(GetTemplateViewName(), GetAlertTemplatePK());

            Dictionary<string, object> nameValueDictionary = new Dictionary<string, object>();

            View view = GetView(viewName);
             Dictionary<string, Durados.Workflow.DictionaryField> dicFields = new Dictionary<string, Durados.Workflow.DictionaryField>();
            LoadValues(nameValueDictionary, dataRow, view, null, view, view.DisplayName + ".", "[", "]", dicFields, view.Name + ".");

            string body = template.Replace(nameValueDictionary);

            string subject = GetDefaultAlertSubject();
            if (System.Configuration.ConfigurationManager.AppSettings.AllKeys.Contains("AlertSubject"))
                subject = Convert.ToString(System.Configuration.ConfigurationManager.AppSettings["AlertSubject"]);

            string to = GetTo(viewName, dataRow);
            string cc = string.Empty;
            string from = Convert.ToString(System.Configuration.ConfigurationManager.AppSettings["fromAlert"]);

            SendEmail(from, to, cc, body, subject);

            string pk = view.GetPkValue(dataRow);
            Dictionary<string, object> values = new Dictionary<string, object>();

            values.Add(GetReminderSentDate(), DateTime.Now);
            view.Edit(values, pk, null, null, null, null);
        }

        protected virtual string GetReminderSentDate()
        {
            return "ReminderSentDate";
        }



        protected void alertView_BeforeSelect(object sender, SelectEventArgs e)
        {
            SetAlertFilter((Durados.Web.Mvc.View)e.View, (Durados.DataAccess.Filter)e.Filter);
        }

        protected virtual void SetAlertFilter(View view, Durados.DataAccess.Filter filter)
        {
            //if (view.Fields.ContainsKey(GetSendAlertsSordColumn(view.Name)))
            //{
            //    filter.WhereStatement += " and (ReminderSentDate is null or ReminderSentDate < ReminderDate)  AND (SendNotificationEmail = 1)";
            //}
            base.SetPermanentFilter(view, filter);
        }

        public virtual string SaveInMessageBoard(Dictionary<string, Parameter> parameters, Durados.View view, Dictionary<string, object> values, DataRow prevRow, string pk, string siteWithoutQueryString, string urlAction, string subject, string message, int currentUserId, string currentUserRole, Dictionary<int, bool> recipients)
        {
            return SaveInMessageBoard(parameters, (View)view, values, prevRow, pk, siteWithoutQueryString, urlAction, subject, message, currentUserId, recipients);
        }

        public virtual void SaveMessageAction(View view, string pk, Durados.Web.Mvc.UI.Json.Field jsonField, MessageBoardAction messageBoardAction)
        {
            SaveMessageAction(view, pk, ((ColumnField)view.Fields[messageBoardAction.ToString()]).ConvertFromString(jsonField.Value.ToString()), messageBoardAction.GetHashCode(), Convert.ToInt32(GetUserID()));
        }

        public virtual void SaveMessageAction(Durados.View view, string pk, object value, int messageBoardAction, int userId)
        {
            SqlAccess sqlAccess = new SqlAccess();
            Dictionary<string, object> parameters = new Dictionary<string, object>();
            parameters.Add("@UserId", userId);
            parameters.Add("@MessageId", Convert.ToInt32(pk));

            parameters.Add("@ActionId", messageBoardAction);
            parameters.Add("@ActionValue", value);
            sqlAccess.ExecuteProcedure(Map.Database.SysDbConnectionString, "Durados_MessageBoard_Action", parameters, null);

        }

        //public virtual string SaveInMessageBoard(Dictionary<string, Parameter> parameters, View view, Dictionary<string, object> values, DataRow prevRow, string pk, string siteWithoutQueryString, string urlAction, string subject, string message, int currentUserId, string currentUserRole, int[] recipients)
        //{
        //    string id = null;
        //    try
        //    {
        //        View messageBoardView = (View)Map.Database.Views["durados_v_MessageBoard"];

        //        Dictionary<string, object> messageBoardValues = new Dictionary<string, object>();
        //        string rowDisplayValue = view.GetDisplayValue(pk, values, prevRow);
        //        messageBoardValues.Add("Subject", subject);
        //        messageBoardValues.Add("Message", message);
        //        messageBoardValues.Add("fk_user_MessageBoard_OriginatedUser_Parent", currentUserId);
        //        messageBoardValues.Add("ViewName", view.Name);
        //        messageBoardValues.Add("ViewDisplayName", view.DisplayName);
        //        messageBoardValues.Add("PK", pk);
        //        messageBoardValues.Add("RowDisplayName", rowDisplayValue);
        //        //messageBoardValues.Add("RowLink", message);
        //        messageBoardValues.Add("RowLink", FieldHelper.GetUrlData(view.DisplayName + " - " + rowDisplayValue, urlAction));
        //        messageBoardValues.Add("ViewLink", FieldHelper.GetUrlData(view.DisplayName + " - " + rowDisplayValue, urlAction));
        //        //messageBoardValues.Add("ViewLink", FieldHelper.GetUrlData(view.DisplayName, Url.Action(view.IndexAction, view.Controller, new { viewName = view.Name })));

        //        id = messageBoardView.Create(messageBoardValues);

        //        foreach (int recipient in recipients)
        //        {
        //            SaveMessageAction(view, id, false, MessageBoardAction.Delete.GetHashCode(), recipient);
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        Map.Logger.Log(GetControllerNameForLog(this.ControllerContext), this.ControllerContext.RouteData.Values["action"].ToString(), ex.Source, ex, 1, "Save Message Board View name: " + view.Name + ", pk: " + pk);
        //    }

        //    return id;
        //}
        public virtual string SaveInMessageBoard(Dictionary<string, Parameter> parameters, View view, Dictionary<string, object> values, DataRow prevRow, string pk, string siteWithoutQueryString, string urlAction, string subject, string message, int currentUserId, Dictionary<int, bool> recipients)
        {
            string id = null;
            try
            {
                View messageBoardView = (View)Map.Database.Views["durados_v_MessageBoard"];

                //Dictionary<string, object> messageBoardValues = new Dictionary<string, object>();
                //string rowDisplayValue = view.GetDisplayValue(pk, values, prevRow);
                //messageBoardValues.Add("Subject", subject);
                //messageBoardValues.Add("Message", message);
                //messageBoardValues.Add("fk_user_MessageBoard_OriginatedUser_Parent", currentUserId);
                //messageBoardValues.Add("ViewName", view.Name);
                //messageBoardValues.Add("ViewDisplayName", view.DisplayName);
                //messageBoardValues.Add("PK", pk);
                //messageBoardValues.Add("RowDisplayName", rowDisplayValue);
                ////messageBoardValues.Add("RowLink", message);
                //messageBoardValues.Add("RowLink", FieldHelper.GetUrlData(view.DisplayName + " - " + rowDisplayValue, urlAction));
                //messageBoardValues.Add("ViewLink", FieldHelper.GetUrlData(view.DisplayName + " - " + rowDisplayValue, urlAction));
                ////messageBoardValues.Add("ViewLink", FieldHelper.GetUrlData(view.DisplayName, Url.Action(view.IndexAction, view.Controller, new { viewName = view.Name })));

                //id = messageBoardView.Create(messageBoardValues);

                string sql = "INSERT INTO [durados_MessageBoard] ([Subject] ,[Message] ,[OriginatedUserId] ,[ViewName] ,[ViewDisplayName] ,[PK] ,[RowDisplayName] ,[CreatedDate] ,[RowLink] ,[ViewLink]) VALUES (";
                sql += "@Subject, @Message, @OriginatedUserId, @ViewName, @ViewDisplayName, @PK, @RowDisplayName, @CreatedDate, @RowLink, @ViewLink);";
                sql += "SELECT IDENT_CURRENT('[durados_MessageBoard]') AS ID";

                Dictionary<string, object> parameters2 = new Dictionary<string, object>();
                string rowDisplayValue = view.GetDisplayValue(pk, values, prevRow);
                parameters2.Add("Subject", subject);
                parameters2.Add("Message", message);
                parameters2.Add("OriginatedUserId", currentUserId);
                parameters2.Add("ViewName", view.Name);
                parameters2.Add("ViewDisplayName", view.DisplayName);
                parameters2.Add("PK", pk);
                parameters2.Add("RowDisplayName", rowDisplayValue);
                parameters2.Add("CreatedDate", DateTime.Now);
                parameters2.Add("RowLink", FieldHelper.GetUrlData(view.DisplayName + " - " + rowDisplayValue, urlAction));
                parameters2.Add("ViewLink", FieldHelper.GetUrlData(view.DisplayName + " - " + rowDisplayValue, urlAction));


                SqlAccess sqlAccess = new SqlAccess();
                id = sqlAccess.ExecuteScalar(Map.Database.SysDbConnectionString, sql, parameters2);

                View messageStatusView = (View)Map.Database.Views["durados_MessageStatus"];

                foreach (int recipient in recipients.Keys)
                {
                    //Dictionary<string, object> messageStatusValues = new Dictionary<string, object>();
                    //messageStatusValues.Add("fk_MessageBoard_MessageStatus_Parent", id);
                    //messageStatusValues.Add("fk_user_MessageStatus_Parent", recipient.ToString());
                    //messageStatusValues.Add("Deleted", false);
                    //messageStatusValues.Add("Important", false);
                    //messageStatusValues.Add("Reviewed", false);
                    //messageStatusValues.Add("ActionRequired", recipients[recipient]);

                    //id = messageStatusView.Create(messageStatusValues);
                    SaveMessageAction(messageStatusView, id, recipients[recipient] ? "True" : "False", 4, recipient);
                }
            }
            catch (Exception ex)
            {
                Map.Logger.Log(GetControllerNameForLog(this.ControllerContext), this.ControllerContext.RouteData.Values["action"].ToString(), ex.Source, ex, 1, "Save Message Board View name: " + view.Name + ", pk: " + pk);
            }

            return id;
        }

    }
    
}