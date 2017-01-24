using System;
using System.Data;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Durados;
using Durados.DataAccess;
using Durados.Web.Mvc.UI;
using Durados.Web.Mvc.UI.Helpers;

namespace Durados.Web.Mvc.Workflow
{
    public class Notifier : Durados.Workflow.Notifier
    {
        //protected Map Map
        //{
        //    get
        //    {
        //        return Maps.Instance.GetMap();
        //    }
        //}


        public Notifier()
            : base()
        {
        }



        protected override Durados.Workflow.EmailParameters GetEmailParameters(object controller, Dictionary<string, Parameter> parameters, Durados.View view, Dictionary<string, object> values, DataRow prevRow, string pk, string siteWithoutQueryString, string mainSiteWithoutQueryString, string connectionString, string currentUserId, string currentUsername, string currentUserRole)
        {
            Durados.Workflow.EmailParameters emailParameters = new Durados.Workflow.EmailParameters();

            Dictionary<string, string> to = new Dictionary<string, string>();
            Dictionary<string, string> cc = new Dictionary<string, string>();

            if (parameters.ContainsKey("DistributionLists"))
            {
                string[][] distributionLists = GetDistributionLists(parameters["DistributionLists"].Value, (Database)view.Database);
                if (distributionLists[0] != null)
                {
                    foreach (string email in distributionLists[0])
                        if (!to.ContainsKey(email))
                            to.Add(email, email);
                }

                if (distributionLists[1] != null)
                    foreach (string email in distributionLists[1])
                        if (!cc.ContainsKey(email))
                            cc.Add(email, email);
                //emailParameters.To = distributionLists[0];
                //emailParameters.Cc = distributionLists[1];
            }

            if (parameters.ContainsKey("to"))
                foreach (string email in GetEmailsFromParameter((Durados.Workflow.INotifier)controller, parameters["to"].Value, view, values, prevRow, pk, connectionString, currentUserId, currentUsername, currentUserRole))
                    if (!to.ContainsKey(email))
                        to.Add(email, email);

            //    emailParameters.To = parameters["to"].Split(';');
            if (parameters.ContainsKey("cc"))
                foreach (string email in GetEmailsFromParameter((Durados.Workflow.INotifier)controller, parameters["cc"].Value, view, values, prevRow, pk, connectionString, currentUserId, currentUsername, currentUserRole))
                    if (!cc.ContainsKey(email))
                        cc.Add(email, email);
            //    emailParameters.Cc = parameters["cc"].Split(';');

            if (parameters.ContainsKey("bcc"))
                emailParameters.Bcc = GetEmailsFromParameter((Durados.Workflow.INotifier)controller, parameters["bcc"].Value, view, values, prevRow, pk, connectionString, currentUserId, currentUsername, currentUserRole);
            
            emailParameters.To = to.Values.ToArray();
            emailParameters.Cc = cc.Values.ToArray();

            try
            {
                if (parameters.ContainsKey("from"))
                    emailParameters.From = GetEmailsFromParameter((Durados.Workflow.INotifier)controller, parameters["from"].Value, view, values, prevRow, pk, connectionString, currentUserId, currentUsername, currentUserRole).FirstOrDefault();
            }
            catch { }

            if (parameters.ContainsKey("subject"))
                emailParameters.Subject = GetMessage((Durados.Workflow.INotifier)controller, parameters["subject"].Value, view, values, pk, siteWithoutQueryString, mainSiteWithoutQueryString);
            if (parameters.ContainsKey("message"))
                emailParameters.Message = GetMessage((Durados.Workflow.INotifier)controller, parameters["message"].Value, view, values, pk, siteWithoutQueryString, mainSiteWithoutQueryString);
            //}
            //else
            //{
            //    throw new DuradosException("No distribution list");
            //}

            
            return emailParameters;
        }

        protected string[] GetEmailsFromParameter(Durados.Workflow.INotifier controller, string parameter, Durados.View view, Dictionary<string, object> values, DataRow prevRow, string pk, string connectionString, string currentUserId, string currentUsername, string currentUserRole)
        {
            //// replace the tokens in the parameter 
            if (!string.IsNullOrEmpty(pk) && ((parameter.Contains('[') && parameter.Contains(']')) || (parameter.Contains(Database.DictionaryPrefix) && parameter.Contains(Database.DictionaryPostfix)) || parameter.Contains('$')))
            {
                if (nameValueDictionary.Count == 0 && prevRow != null)
                    controller.LoadValues(nameValueDictionary, prevRow, (Durados.Web.Mvc.View)view, null, (Durados.Web.Mvc.View)view, view.DisplayName + ".", "[", "]", dicFields, view.Name + ".");

                parameter = parameter.ReplaceAllTokens(view, values, pk, currentUserId, currentUsername, currentUserRole, prevRow);
                parameter = parameter.Replace(nameValueDictionary, controller.GetTableViewer(), view);
                parameter = parameter.Replace(dicFields.ToDictionary(k => k.Key, v => ((Durados.Workflow.DictionaryField)v.Value).Value), controller.GetTableViewer(), view);
            }

            parameter = parameter.Replace(values, controller.GetTableViewer(), view);
  
            string[] parameters = parameter.Split(';');
            Dictionary<string, string> emails = new Dictionary<string, string>();
            if (parameters != null)
            {
                for (int i = 0; i < parameters.Length; i++)
                {
                    string param = parameters[i];
                    string email = GetEmail(param, view, values, prevRow, pk, connectionString);
                    if (!string.IsNullOrEmpty(email) && !emails.ContainsKey(email))
                        emails.Add(email, email);
                }
            }

            return emails.Values.ToArray();
        }

        protected virtual string GetEmail(string parameter, Durados.View view, Dictionary<string, object> values, DataRow prevRow, string pk, string connectionString)
        {
            parameter = parameter.Trim();

            if (parameter.StartsWith(Database.DictionaryPrefix) && values.ContainsKey(parameter))
                return values[parameter].ToString();
            
            if (parameter.StartsWith("$"))
            {
                string fieldName = parameter.TrimStart('$');
                if (view.Fields.ContainsKey(fieldName))
                {
                    Field field = view.Fields[fieldName];
                    if (field.FieldType == FieldType.Column)
                    {
                        ColumnField columnField = (ColumnField)field;
                        if (columnField.TextHtmlControlType == TextHtmlControlType.Autocomplete)
                        {
                            return GetEmailFromAutocomplete(parameter, view, values, prevRow, pk, connectionString);
                        }
                        else
                        {
                            if (values.ContainsKey(fieldName))
                                return values[fieldName].ToString();
                            else
                            {
                                return columnField.GetValue(prevRow);
                            }
                        }
                    }
                    else if (field.FieldType == FieldType.Parent)
                    {
                        ParentField parentField = (ParentField)field;
                        if (parentField.ParentView.Base.Name.ToLower() == ((Database)view.Database).UserViewName.ToLower())
                        {
                            string key;
                            if (values.ContainsKey(fieldName))
                                key = values[fieldName].ToString();
                            else
                            {
                                key = parentField.GetValue(prevRow);
                            }
                            return ((Database)view.Database).Map.GetUserEmail(key);
                        }
                        else
                        {
                            return GetEmailFromParent(parameter, view, values, prevRow, pk, connectionString);
                        }
                    }
                    else
                    {
                        return null;
                    }
                }
                else
                {
                    return null;
                }
            }
            else
            {
                return parameter;
            }
        }

        protected virtual string GetEmailFromAutocomplete(string parameter, Durados.View view, Dictionary<string, object> values, DataRow prevRow, string pk, string connectionString)
        {
            return null;
        }

        protected virtual string GetEmailFromParent(string parameter, Durados.View view, Dictionary<string, object> values, DataRow prevRow, string pk, string connectionString)
        {
            return null;
        }

        public string[][] GetDistributionLists(string distributionLists, Database database) 
        {
            string[][] s = new string [2][];

            if (string.IsNullOrEmpty(distributionLists))
                return s;

            string[] distributionListsNames = distributionLists.Split(';');

            Dictionary<string, string> tos = new Dictionary<string, string>();
            Dictionary<string, string> ccs = new Dictionary<string, string>();

            foreach (string distributionListsName in distributionListsNames)
            {
                Dictionary<string, bool> emails = GetEmails(distributionListsName, database);

                if (emails != null)
                {
                    foreach (string email in emails.Keys)
                    {
                        bool to = emails[email];
                        if (to)
                        {
                            if (!tos.ContainsKey(email))
                            {
                                tos.Add(email, email);
                            }
                            if (ccs.ContainsKey(email))
                            {
                                ccs.Remove(email);
                            }
                        }
                        else
                        {
                            if (!tos.ContainsKey(email))
                            {
                                if (!ccs.ContainsKey(email))
                                {
                                    ccs.Add(email, email);
                                }
                            }

                        }
                    }
                }
            }

            s[0] = tos.Values.ToArray();
            s[1] = ccs.Values.ToArray();

            return s;
        }

        protected virtual Dictionary<string, bool> GetEmails(string distributionListsName, Database database)
        {
            Durados.Web.Mvc.View distributionView = (Durados.Web.Mvc.View)database.Views[GetDistributionViewName()];

            Dictionary<string, object> values = new Dictionary<string, object>();
            values.Add("Name", distributionListsName);

            int rowCount = 0;
            DataView dataView = distributionView.FillPage(1, 1000, values, false, null, out rowCount, null, null);


            Dictionary<string, bool> emails = new Dictionary<string, bool>();

            foreach (System.Data.DataRowView row in dataView)
            {
                string email = row.Row["Email"].ToString();
                bool pointOfContact = (bool)row.Row["PointOfContact"];

                if (emails.ContainsKey(email))
                {
                    if (!emails[email] && pointOfContact)
                    {
                        emails[email] = true;
                    }
                }
                else
                {
                    emails.Add(email, pointOfContact);
                }
            }


            return emails;
        }


        protected virtual string GetDistributionViewName()
        {
            return "v_DistributionListEmail";
        }

        public string GetSubject(Durados.Workflow.INotifier controller, string subjectKey, Durados.View view, Dictionary<string, object> values, string pk)
        {
            //return GetContent(controller, subjectKey);
            return GetMessage(controller, subjectKey, view, values, pk, string.Empty, string.Empty);

        }

        Dictionary<string, object> nameValueDictionary = new Dictionary<string, object>();
        Dictionary<string, Durados.Workflow.DictionaryField> dicFields = new Dictionary<string, Durados.Workflow.DictionaryField>();


        public string GetMessage(Durados.Workflow.INotifier controller, string messageKey, Durados.View view, Dictionary<string, object> values, string pk, string siteWithoutQueryString, string mainSiteWithoutQueryString)
        {
            string content = GetContent(controller, messageKey);

            if (content == null)
                return messageKey;

            content = controller.HtmlDecode(content);
            
            
            string viewDisplayName = view.DisplayName;

            if (Maps.MultiTenancy)
            {
                content = content.Replace("[AppPath]", ((Database)view.Database).Map.Url);
                content = content.Replace("[MainAppPath]", mainSiteWithoutQueryString);
                content = content.Replace("{{AppPath}}", ((Database)view.Database).Map.Url);
                content = content.Replace("{{MainAppPath}}", mainSiteWithoutQueryString);

            }
            else
            {
                content = content.Replace("[AppPath]", siteWithoutQueryString);
                content = content.Replace("{{AppPath}}", siteWithoutQueryString);
            }
            content = content.Replace("[Product]", ((Database)view.Database).Map.AppName);
            content = content.Replace("{{AppName}}", ((Database)view.Database).Map.AppName);

            if (!string.IsNullOrEmpty(pk) && (content.Contains('[') && content.Contains(']') || content.Contains(Database.DictionaryPrefix) && content.Contains(Database.DictionaryPostfix)))
            {
                DataRow row = view.GetDataRow(pk);

                if (nameValueDictionary.Count == 0)
                    controller.LoadValues(nameValueDictionary, row, (Durados.Web.Mvc.View)view, null, (Durados.Web.Mvc.View)view, viewDisplayName + ".", "[", "]", dicFields, view.Name + ".");
                
                content = content.Replace(nameValueDictionary, controller.GetTableViewer(), view);
                content = content.Replace(dicFields.ToDictionary(k => k.Key, v => ((Durados.Workflow.DictionaryField)v.Value).Value), controller.GetTableViewer(), view);
                //content = content.Replace(values, controller.GetTableViewer(), view);
                
                if (content.Contains("[User.First Name]"))
                {
                    if (row.Table.Columns.Contains("FirstName"))
                    {
                        if (!row.IsNull("FirstName"))
                        {
                            content = content.Replace("[User.First Name]", row["FirstName"].ToString());
                        }
                    }
                }
            
            }
            else
            {
                content = content.Replace(values, controller.GetTableViewer(), view);
                
            }
            return content;
        }

        public string[] GetSubjectAndMessage(Durados.Workflow.INotifier controller, string subjectKey, string messageKey, Durados.View view, Dictionary<string, object> values, string pk, string siteWithoutQueryString, string mainSiteWithoutQueryString)
        {
            string messageContent = GetContent(controller, messageKey);
            string subjectContent = GetContent(controller, subjectKey);

            string[] s = new string[2] { subjectKey, messageKey };
            if (messageContent == null && subjectContent == null)
                return s;

            Dictionary<string, object> nameValueDictionary = new Dictionary<string, object>();
            Dictionary<string, Durados.Workflow.DictionaryField> dicFields = new Dictionary<string, Durados.Workflow.DictionaryField>();

            string viewDisplayName = view.DisplayName;
            if (!string.IsNullOrEmpty(pk) && ((messageContent.Contains('[') && messageContent.Contains(']')) || (subjectContent.Contains('[') && subjectContent.Contains(']'))))
            {
                DataRow row = view.GetDataRow(pk);

                controller.LoadValues(nameValueDictionary, row, (Durados.Web.Mvc.View)view, null, (Durados.Web.Mvc.View)view, viewDisplayName + ".", "[", "]", dicFields, view.Name + ".");
            }

            else
            {
                return s;
            }

            if (messageContent != null && messageContent.Contains('[') && messageContent.Contains(']'))
            {
                s[1] = GetContent(controller, messageContent, view, nameValueDictionary, pk, siteWithoutQueryString, mainSiteWithoutQueryString);
            }

            if (subjectContent != null && subjectContent.Contains('[') && subjectContent.Contains(']'))
            {
                s[0] = GetContent(controller, subjectContent, view, nameValueDictionary, pk, siteWithoutQueryString, mainSiteWithoutQueryString);
            }

            return s;
        }

        public string GetContent(Durados.Workflow.INotifier controller, string content, Durados.View view, Dictionary<string, object> nameValueDictionary, string pk, string siteWithoutQueryString, string mainSiteWithoutQueryString)
        {
            content = controller.HtmlDecode(content);

            
            string viewDisplayName = view.DisplayName;

            content = content.Replace("[MainAppPath]", mainSiteWithoutQueryString);
            content = content.Replace("[AppPath]", siteWithoutQueryString);
            content = content.Replace("{{MainAppPath}}", mainSiteWithoutQueryString);
            content = content.Replace("{{AppPath}}", siteWithoutQueryString);

            if (nameValueDictionary != null && content.Contains('[') && content.Contains(']'))
            {
                content = content.Replace(nameValueDictionary, controller.GetTableViewer(), view);
            }

            return content;
        }

        protected virtual string GetContentViewName()
        {
            return "durados_Html";
        }

        //protected virtual string GetContent(string pk)
        //{
        //    Durados.Web.Mvc.View contentView = (Durados.Web.Mvc.View)Map.Database.Views[GetContentViewName()];
        //    contentView.Database.Logger.Log(contentView.Name, "Start", "GetContent", "Notifier", "", 14, DateTime.Now.Millisecond.ToString(), DateTime.Now);
        //    DataRow contentRow = contentView.GetDataRow(pk);

        //    if (contentRow == null)
        //        return null;

        //    string content = contentView.GetDisplayValue(GetContentFieldName(), contentRow);

        //    contentView.Database.Logger.Log(contentView.Name, "End", "GetContent", "Notifier", "", 14, DateTime.Now.Millisecond.ToString(), DateTime.Now);
        //    return content;
        //}

        protected virtual string GetContent(Durados.Workflow.INotifier controller, string pk)
        {
            Durados.Web.Mvc.View contentView = (Durados.Web.Mvc.View)controller.GetNonConfigView(GetContentViewName());
            Field field = contentView.Fields[GetContentFieldName()];
            return controller.GetFieldValue((ColumnField)field, pk) ?? pk;
        }

        protected virtual string GetContentFieldName()
        {
            return "Text";
        }
    }
}
