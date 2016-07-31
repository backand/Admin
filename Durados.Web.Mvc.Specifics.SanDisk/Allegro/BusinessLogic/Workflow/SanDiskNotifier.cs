using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;

namespace Durados.Web.Mvc.Specifics.SanDisk.Allegro.BusinessLogic.Workflow
{
    public class SanDiskNotifier : Durados.Web.Mvc.Workflow.HistoryNotifier
    {
        public SanDiskNotifier()
            : base()
        {
        }

        protected override Durados.Workflow.EmailParameters GetEmailParameters(object controller, Dictionary<string, Durados.Parameter> parameters, Durados.View view, Dictionary<string, object> values, DataRow prevRow, string pk, string siteWithoutQueryString, string connectionString)
        {
            Durados.Workflow.EmailParameters emailParameters = base.GetEmailParameters(controller, parameters, view, values, prevRow, pk, siteWithoutQueryString, connectionString);

            if (view.Name == "Issue")
            {
                emailParameters.Cc = GetIssueCc(view, values, pk, emailParameters);
            }

            return emailParameters;
        }

        private string[] GetIssueCc(Durados.View view, Dictionary<string, object> values, string pk, Durados.Workflow.EmailParameters emailParameters)
        {
            string reportedUserEmail = GetUserEmail(view, pk);

            string[] cc = emailParameters.Cc;

            List<string> ccList = new List<string>(cc);
            ccList.Add(reportedUserEmail);

            return ccList.ToArray();
        }

        private string GetUserEmail(Durados.View view, string pk)
        {
            return ((Durados.Web.Mvc.Database)view.Database).GetUserRow()["Email"].ToString();
        }
    }
}
