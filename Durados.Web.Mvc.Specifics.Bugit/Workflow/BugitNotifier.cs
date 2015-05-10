using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;


namespace Durados.Web.Mvc.Specifics.Bugit.Workflow
{
    public class BugitNotifier : Durados.Web.Mvc.Workflow.HistoryNotifier
    {
        public BugitNotifier()
            : base()
        {
        }

        protected override Dictionary<string, bool> GetEmails(string distributionListsName)
        {
            return null;
        }

        protected override Durados.Workflow.EmailParameters GetEmailParameters(object controller, Dictionary<string, Parameter> parameters, Durados.View view, Dictionary<string, object> values, DataRow prevRow, string pk, string siteWithoutQueryString, string connectionString)
        {
            Durados.Workflow.EmailParameters emailParameters = base.GetEmailParameters(controller, parameters, view, values, prevRow, pk, siteWithoutQueryString, connectionString);

            if (view.Name == "Issue")
            {
                int? uReport = null;
                int id = Int32.Parse(pk);
                if (values.ContainsKey("FK_Issue_User_Report_Parent"))
                {
                    if (values["FK_Issue_User_Report_Parent"] != null && values["FK_Issue_User_Report_Parent"].ToString() != "")
                        uReport = int.Parse(values["FK_Issue_User_Report_Parent"].ToString());
                }
                else
                {
                    uReport = Durados.Web.Mvc.Specifics.Bugit.DataAccess.Issue.GetReportedBy(id);
                }
                if (uReport.HasValue)
                    emailParameters.To = GetEmails(emailParameters.Cc, uReport.Value);

                int? uAssign = null;
                if (values.ContainsKey("FK_Issue_User_Assign_Parent"))
                {
                    if (values["FK_Issue_User_Assign_Parent"] != null && values["FK_Issue_User_Assign_Parent"].ToString() != "")
                    {
                        uAssign = int.Parse(values["FK_Issue_User_Assign_Parent"].ToString());
                    }
                }
                else
                {
                    uAssign = Durados.Web.Mvc.Specifics.Bugit.DataAccess.Issue.GetAssignedTo(id);
                }
                if (uAssign.HasValue)
                    emailParameters.To = GetEmails(emailParameters.To, uAssign.Value);

            }

            return emailParameters;
        }

        private string[] GetEmails(string[] emails, int userId)
        {
            string UserEmail = Durados.Web.Mvc.Specifics.Bugit.DataAccess.User.GetUserEmail(userId);

            List<string> emailList = new List<string>(emails);
            emailList.Add(UserEmail);

            return emailList.ToArray();
        }

        private string GetUserEmail(Durados.View view, string pk)
        {
            return ((Durados.Web.Mvc.Database)view.Database).GetUserRow()["Email"].ToString();
        }
    }
}
