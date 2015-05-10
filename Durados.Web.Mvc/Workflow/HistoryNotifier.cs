using System;
using System.Data;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Durados.DataAccess;
using Durados.Web.Mvc.UI.Helpers;

namespace Durados.Web.Mvc.Workflow
{
    public class HistoryNotifier : Notifier
    {
        
        public HistoryNotifier()
            : base()
        {
        }

        //public void Notify(View view, int action, string by, OldNewValue[] oldNewValues, string pk, DataRow prevRow)
        //{
        //    try
        //    {
        //        if (string.IsNullOrEmpty(view.HistoryNotifyList))
        //            return;

        //        string host = Convert.ToString(System.Configuration.ConfigurationSettings.AppSettings["host"]);
        //        int port = Convert.ToInt32(System.Configuration.ConfigurationSettings.AppSettings["port"]);
        //        string username = Convert.ToString(System.Configuration.ConfigurationSettings.AppSettings["username"]);
        //        string password = Convert.ToString(System.Configuration.ConfigurationSettings.AppSettings["password"]);

        //        string[][] distributionLists = GetDistributionLists(view.HistoryNotifyList);
        //        string[] to = distributionLists[0];
        //        string[] cc = distributionLists[1];

        //        string message = string.Empty;
        //        string userFullName = Map.Database.GetUserFullName(by);
        //        string usernameAndFullName = userFullName + " (" + by + ") ";
        //        message += "The following changes were made:<br>View: {0}<br>User: {1}<br>Time: {2}<br>Action {3}<br>Primary Key Value: {4}<br>{5}: {6}<br>";
        //        bool displayValueChanged = false;
        //        string oldNewValuesMessage = GetOldNewValues(view, oldNewValues, out displayValueChanged);

        //        if (!string.IsNullOrEmpty(oldNewValuesMessage))
        //        {
        //            message += oldNewValuesMessage;

        //            string displayValue = null;
        //            if (prevRow == null || displayValueChanged)
        //            {
        //                if (string.IsNullOrEmpty(pk))
        //                {
        //                    displayValue = string.Empty;
        //                }
        //                else
        //                {
        //                    DataRow row = view.GetDataRow(pk);
        //                    if (row == null)
        //                    {
        //                        displayValue = string.Empty;
        //                    }
        //                    else
        //                    {
        //                        displayValue = view.GetDisplayValue(row);
        //                        if (displayValue == null)
        //                            displayValue = string.Empty;
        //                    }
        //                }
        //            }
        //            else
        //            {
        //                displayValue = view.GetDisplayValue(prevRow);
        //            }
        //            message = string.Format(message, view.DisplayName, usernameAndFullName, DateTime.Now.ToShortDateString() + " " + DateTime.Now.ToShortTimeString(), GetAction(action), pk, view.DisplayField.DisplayName, displayValue);
        //            Durados.Cms.DataAccess.Email.Send(host, false, port, username, password, false, to, cc, null, GetSubject(view), message, GetFrom(view), null, null);
        //        }
        //    }
        //    catch( Exception ex)
        //    {
        //        Map.Logger.Log("Workflow.HistoryNotifier", "Notify", null, ex, 1,"");
        //    }
        //}

        public void Notify(View view, int action, string by, OldNewValue[] oldNewValues, string pk, DataRow prevRow, object controller, Dictionary<string, object> values, string siteWithoutQueryString, string mainSiteWithoutQueryString)
        {
            try
            {
                if (string.IsNullOrEmpty(view.HistoryNotifyList))
                    return;

                string host = Convert.ToString(System.Configuration.ConfigurationManager.AppSettings["host"]);
                int port = Convert.ToInt32(System.Configuration.ConfigurationManager.AppSettings["port"]);
                string username = Convert.ToString(System.Configuration.ConfigurationManager.AppSettings["username"]);
                string password = Convert.ToString(System.Configuration.ConfigurationManager.AppSettings["password"]);

                string[][] distributionLists = GetDistributionLists(view.HistoryNotifyList, (Database)view.Database);
                string[] to = distributionLists[0];
                string[] cc = distributionLists[1];

                bool displayValueChanged = false;
                string oldNewValuesMessage = GetOldNewValues(view, oldNewValues, out displayValueChanged);

                Durados.Web.Mvc.Controllers.CrmController crmController = (Durados.Web.Mvc.Controllers.CrmController)controller;
                if (!string.IsNullOrEmpty(oldNewValuesMessage))
                {
                    string message = string.Empty;
                    string subject = string.Empty;

                    message += "<html dir=\"ltr\"><head><style><!--";

                    string css = ".tableCss {margin-top:5px; border:solid 1px #9ca3ad;} .tdCss, .tdCssalt, .oldVal, .oldValalt {padding: 0px 5px 0px 5px; vertical-align: top; color: #000000; border-bottom:solid 1px #e8eaec; padding-top:2px; padding-bottom:5px; background: #f8f8f9;} .tdCssalt{ background: #CEE3F6;} .oldVal{ text-decoration: line-through;} .oldValalt{ background: #CEE3F6; text-decoration: line-through;}";
                    message += css;
                    message += "--></style></head><body>";

                    if (!string.IsNullOrEmpty(view.NotifySubjectKey) && !string.IsNullOrEmpty(view.NotifySubjectKey))
                    {
                        string[] s = base.GetSubjectAndMessage(crmController, view.NotifySubjectKey, view.NotifyMessageKey, view, values, pk, siteWithoutQueryString, mainSiteWithoutQueryString);
                        subject = s[0];
                        message += s[1];
                    }
                    else
                    {
                        message += GetMessage(view, action, by, pk, prevRow, crmController, values, siteWithoutQueryString, mainSiteWithoutQueryString);
                        subject = GetSubject(view, action, by, pk, prevRow, crmController, values, siteWithoutQueryString, mainSiteWithoutQueryString);
                    }
                    message += oldNewValuesMessage;
                    message += "</body></html>";
                    Durados.Cms.DataAccess.Email.Send(host, view.Database.UseSmtpDefaultCredentials, port, username, password, false, to, cc, null, subject, message, GetFrom(view), null, null, ((Durados.Web.Mvc.Controllers.CrmController)controller).DontSend, view.Database.Logger);
                }
            }
            catch (Exception ex)
            {
                ((Database)view.Database).Map.Logger.Log("Workflow.HistoryNotifier", "Notify", null, ex, 1, "");
            }
        }

        private string GetMessage(View view, int action, string by, string pk, DataRow prevRow, Durados.Web.Mvc.Controllers.CrmController controller, Dictionary<string, object> values, string siteWithoutQueryString, string mainSiteWithoutQueryString)
        {
            if (string.IsNullOrEmpty(view.NotifyMessageKey))
            {
                string message = string.Empty;
                string userFullName = controller.Map.Database.GetUserFullName(by);
                string usernameAndFullName = userFullName + " (" + by + ") ";
                message += "The following changes were made:<br>View: {0}<br>User: {1}<br>Time: {2}<br>Action {3}<br>Primary Key Value: {4}<br>{5}: {6}<br>";
                bool displayValueChanged = false;


                string displayValue = null;
                if (prevRow == null || displayValueChanged)
                {
                    if (string.IsNullOrEmpty(pk))
                    {
                        displayValue = string.Empty;
                    }
                    else
                    {
                        DataRow row = view.GetDataRow(pk);
                        if (row == null)
                        {
                            displayValue = string.Empty;
                        }
                        else
                        {
                            displayValue = view.GetDisplayValue(row);
                            if (displayValue == null)
                                displayValue = string.Empty;
                        }
                    }
                }
                else
                {
                    displayValue = view.GetDisplayValue(prevRow);
                }
                message = string.Format(message, view.DisplayName, usernameAndFullName, DateTime.Now.ToShortDateString() + " " + DateTime.Now.ToShortTimeString(), GetAction(action), pk, view.DisplayField.DisplayName, displayValue);

                return message;
            }
            else
            {
                return base.GetMessage(controller, view.NotifyMessageKey, view, values, pk, siteWithoutQueryString, mainSiteWithoutQueryString);
            }
        }

        private string GetAction(int action)
        {
            switch (action)
            {
                case 1:
                    return "Add";
                case 2:
                    return "Change";
                case 3:
                    return "Delete";
                default:
                    return string.Empty;
            }
        }

        private string GetOldNewValues(View view, OldNewValue[] oldNewValues, out bool displayValueChanged)
        {
            displayValueChanged = false;

            if (oldNewValues == null || oldNewValues.Length == 0)
                return string.Empty;

            string s = string.Empty;

            s = "<br><br>The following values were changed:<br>";

            bool anyChanged = false;

            StringBuilder sb = new StringBuilder();

            //string css = "<style type='text/css'>body {font-family: Calibri; font-size:14px;} table {color:#333333;	border-width: 1px;	border-color: #666666;	border-collapse: collapse;}table th {	border-width: 1px;	padding: 4px;	border-style: solid;	border-color: #666666;	background-color: #dedede;}table td {	border-width: 1px;	padding: 4px;	border-style: solid;	border-color: #666666;	background-color: #ffffff;}table td.alt{ background: #CEE3F6;}table td.oldVal{ text-decoration: line-through;}table td.newVal{}</style>";

            //sb.Append(css);

            sb.Append(s);
            sb.Append("<table class=\"tableCss\" cellspacing=\"0\" cellpadding=\"0\">");
            //sb.Append("<th>Name</th><th>Old Value</th><th>New Value</th>");
            //sb.Append("</tr>");

            int alt = 0;
            foreach (OldNewValue oldNewValue in oldNewValues)
            {
                string fieldName = view.Fields.ContainsKey(oldNewValue.FieldName) ? view.Fields[oldNewValue.FieldName].DisplayName : oldNewValue.FieldName;
                //s += "<br><br>Column: " + fieldName + "<br>";
                //s += "Old Value: " + oldNewValue.OldValue + "<br>";
                //s += "New Value: " + oldNewValue.NewValue + "<br>";

                string altCss = alt % 2 != 0 ? "alt" : string.Empty;

                sb.Append("<tr>");
                sb.Append(string.Format("<td class='tdCss{3}'>{0}:</td><td class='oldVal{3}'>{1}</td><td class='tdCss{3}'>{2}</td>", fieldName, string.IsNullOrEmpty(oldNewValue.OldValue) ? "&nbsp;&nbsp;&nbsp;&nbsp;" : oldNewValue.OldValue, oldNewValue.NewValue, altCss));
                sb.Append("</tr>");

                alt++;

                if (oldNewValue.OldValue != oldNewValue.NewValue)
                    anyChanged = true;

                if (fieldName == view.DisplayName)
                    displayValueChanged = true;
            }

            sb.Append("</table>");

            if (!anyChanged)
                return string.Empty;

            return sb.ToString();
        }

        protected virtual string GetFrom(View view)
        {
            string from = null;
            if (System.Configuration.ConfigurationManager.AppSettings["fromAlert"] != null)
                from = Convert.ToString(System.Configuration.ConfigurationManager.AppSettings["fromAlert"]);
            if (string.IsNullOrEmpty(from))
            {
                from = view.Database.SiteInfo.GetTitle()+ "@durados.com";
            }

            return from;
        }

        protected virtual string GetSubject(View view, int action, string by, string pk, DataRow prevRow, Durados.Web.Mvc.Controllers.CrmController controller, Dictionary<string, object> values, string siteWithoutQueryString, string mainSiteWithoutQueryString)
        {
            if (string.IsNullOrEmpty(view.NotifySubjectKey))
            {
                return "Changes at " + view.Database.SiteInfo.GetTitle()+ " " + view.DisplayName;
            }
            else
            {
                return base.GetMessage(controller, view.NotifySubjectKey, view, values, pk, siteWithoutQueryString, mainSiteWithoutQueryString);
            }
        }

    }
}
