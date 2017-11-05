using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Http;
using System.Net;
using System.Net.Http;
using System.Collections.Specialized;
using System.Web.Script.Serialization;
using Durados.Web.Mvc.UI.Helpers;

using Durados.Web.Mvc;
using Durados.DataAccess;
using Durados.Web.Mvc.Controllers.Api;
/*
 HTTP Verb	|Entire Collection (e.g. /customers)	                                                        |Specific Item (e.g. /customers/{id})
-----------------------------------------------------------------------------------------------------------------------------------------------
GET	        |200 (OK), list data items. Use pagination, sorting and filtering to navigate big lists.	    |200 (OK), single data item. 404 (Not Found), if ID not found or invalid.
PUT	        |404 (Not Found), unless you want to update/replace every resource in the entire collection.	|200 (OK) or 204 (No Content). 404 (Not Found), if ID not found or invalid.
POST	    |201 (Created), 'Location' header with link to /customers/{id} containing new ID.	            |404 (Not Found).
DELETE	    |404 (Not Found), unless you want to delete the whole collection—not often desirable.	        |200 (OK). 404 (Not Found), if ID not found or invalid.
 
 */

namespace BackAnd.Web.Api.Controllers
{
     public class wfController : apiController, Durados.Workflow.INotifier, Durados.Workflow.IExecuter
    {
        #region workflow

        #region notifier

        public bool DontSend
        {
            get
            {
                return Convert.ToBoolean(System.Configuration.ConfigurationManager.AppSettings["DontSend"] ?? "false");
            }
        }

        public virtual string GetSiteWithoutQueryString()
        {
            return System.Web.HttpContext.Current.Request.Url.Scheme + "://" + System.Web.HttpContext.Current.Request.Url.Authority;
            //return Request.Url.Scheme + "://" + Request.Url.Authority;

        }

        public virtual string GetMainSiteWithoutQueryString()
        {
            //return System.Web.HttpContext.Current.Request.Url.Scheme + "://" + System.Web.HttpContext.Current.Request.Url.Authority;
            return Maps.Instance.DuradosMap.Url;

        }

        public string GetUrlAction(Durados.View view, string pk)
        {
            //return Url.Action(((View)view).IndexAction, ((View)view).Controller, new { viewName = view.Name, pk = pk });
            return string.Empty;

        }
         //TODO : Main Mysql
        //public virtual string SaveInMessageBoard(Dictionary<string, Durados.Parameter> parameters, Durados.View view, Dictionary<string, object> values, System.Data.DataRow prevRow, string pk, string siteWithoutQueryString, string urlAction, string subject, string message, int currentUserId, string currentUserRole, Dictionary<int, bool> recipients)
        //{
        //    return SaveInMessageBoard(parameters, (View)view, values, prevRow, pk, siteWithoutQueryString, urlAction, subject, message, currentUserId, recipients);
        //}

        public virtual void SaveMessageAction(View view, string pk, Durados.Web.Mvc.UI.Json.Field jsonField, Durados.Web.Mvc.Controllers.MessageBoardAction messageBoardAction)
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

        
        public virtual string SaveInMessageBoard(Dictionary<string, Durados.Parameter> parameters, View view, Dictionary<string, object> values, System.Data.DataRow prevRow, string pk, string siteWithoutQueryString, string urlAction, string subject, string message, int currentUserId, Dictionary<int, bool> recipients)
        {
            if (view.Database.IsApi())
                return null;

            string id = null;
            try
            {
                View messageBoardView = (View)Map.Database.Views["durados_v_MessageBoard"];

                
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
                    SaveMessageAction(messageStatusView, id, recipients[recipient] ? "True" : "False", 4, recipient);
                }
            }
            catch (Exception ex)
            {
                Map.Logger.Log(GetControllerNameForLog(this.ControllerContext), "SaveInMessageBoard", ex.Source, ex, 1, "Save Message Board View name: " + view.Name + ", pk: " + pk);
            }

            return id;
        }

        public virtual string GetFieldValue(Durados.Field field, string pk)
        {
            return field.GetValue(pk);
        }

        protected virtual Durados.Web.Mvc.UI.TableViewer GetNewTableViewer()
        {
            return new Durados.Web.Mvc.UI.TableViewer();
        }

        Durados.Web.Mvc.UI.TableViewer tableViewer = null;

        public ITableConverter GetTableViewer()
        {
            if (tableViewer == null)
                tableViewer = GetNewTableViewer();

            return tableViewer;
        }

        public void LoadValues(Dictionary<string, object> values, System.Data.DataRow dataRow, Durados.View view, Durados.ParentField parentField, Durados.View rootView, string dynastyPath, string prefix, string postfix, Dictionary<string, Durados.Workflow.DictionaryField> dicFields, string internalDynastyPath)
        {
            if (view.Equals(rootView))
            {
                dynastyPath = GetViewDisplayName((View)view) + ".";
                internalDynastyPath = view.Name + ".";
            }
            foreach (Durados.Field field in view.Fields.Values.Where(f => f.FieldType == Durados.FieldType.Column))
            {
                LoadValue(values, dataRow, view, field, dynastyPath, prefix, postfix, dicFields, internalDynastyPath);
            }

            var childrenFields = view.Fields.Values.Where(f => f.FieldType == Durados.FieldType.Children && ((ChildrenField)f).LoadForBlockTemplate);
            foreach (ChildrenField field in childrenFields)
            {
                string name = prefix + dynastyPath + field.DisplayName + postfix;
                string internalName = prefix + internalDynastyPath + field.Name + postfix;
                System.Data.DataView value = GetDataView(field, dataRow);
                if (!values.ContainsKey(name))
                {
                    values.Add(name, value);
                    dicFields.Add(internalDynastyPath, new Durados.Workflow.DictionaryField { DisplayName = field.DisplayName, Type = field.DataType, Value = value });
                }

                foreach (ColumnField columnField in field.ChildrenView.Fields.Values.Where(f => f.FieldType == Durados.FieldType.Column))
                {
                    if (columnField.Upload != null)
                    {
                        value.Table.Columns[columnField.Name].ExtendedProperties["ImagePath"] = columnField.GetUploadPath();
                    }
                }
            }

            foreach (ParentField field in view.Fields.Values.Where(f => f.FieldType == Durados.FieldType.Parent))
            {
                if (view.Equals(rootView))
                {
                    dynastyPath = view.DisplayName + ".";
                    internalDynastyPath = view.Name + ".";
                }
                LoadValue(values, dataRow, view, field, dynastyPath, prefix, postfix, dicFields, internalDynastyPath);

                System.Data.DataRow parentRow = null;
                View parentView = null;
                if (dataRow != null)
                {
                    parentRow = dataRow.GetParentRow(field.DataRelation.RelationName);
                    parentView = (View)field.ParentView;
                    if (parentRow == null)
                    {
                        string key = field.GetValue(dataRow);
                        if (!string.IsNullOrEmpty(key))
                            parentRow = parentView.GetDataRow(key, dataRow.Table.DataSet);
                    }
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

        public void LoadValue(Dictionary<string, object> values, System.Data.DataRow dataRow, Durados.View view, Durados.Field field, string dynastyPath, string prefix, string postfix, Dictionary<string, Durados.Workflow.DictionaryField> dicFields, string internalDynastyPath)
        {
            string name = prefix + dynastyPath + field.DisplayName + postfix;
            string InternalName = prefix + internalDynastyPath + field.Name + postfix;
            string value = view.GetDisplayValue(field.Name, dataRow);
            if (!values.ContainsKey(name))
            {
                values.Add(name, value);
                dicFields.Add(InternalName, new Durados.Workflow.DictionaryField { DisplayName = name, Type = field.DataType, Value = value });

            }
            if (field.FieldType == Durados.FieldType.Column && ((ColumnField)field).Upload != null)
            {
                if (dataRow.Table.Columns.Contains(field.Name))
                {

                    dataRow.Table.Columns[field.Name].ExtendedProperties["ImagePath"] = ((ColumnField)field).GetUploadPath();
                }
            }
        }


        public Durados.View GetNonConfigView(string viewName)
        {
            return GetView(viewName);
        }

        public string HtmlDecode(string text)
        {
            return HttpUtility.HtmlDecode(text);
        }

        #endregion notifier

        #region executer
        public string[] GetFilterFieldValue(Durados.View view)
        {
            return null;
        }
        #endregion executer

        #endregion workflow
    }

}
