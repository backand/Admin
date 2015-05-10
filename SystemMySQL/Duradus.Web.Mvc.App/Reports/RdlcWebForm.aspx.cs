using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Durados.Web.Mvc.App.Reports
{
    public partial class RdlcWebForm : System.Web.UI.Page
    {
        private Map map = null;
        public Map Map
        {
            get
            {
                if (map == null)
                    map = Maps.Instance.GetMap();
                return map;
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            //    If Not IsPostBack Then
            //    Me.ReportViewer1.ServerReport.ReportServerCredentials = New ReportServerCredentials("user", "pass", "domain")
            //    Me.ReportViewer1.ServerReport.Refresh()
            //End If
            if (!IsPostBack)
            {
                string username = !string.IsNullOrEmpty(Map.Database.SsrsUsername) ? Map.Database.SsrsUsername : Convert.ToString(System.Configuration.ConfigurationManager.AppSettings["ssrsUsername"]);
                string password = !string.IsNullOrEmpty(Map.Database.SsrsPassword) ? Map.Database.SsrsPassword.Decrypt(Map.Database) : Convert.ToString(System.Configuration.ConfigurationManager.AppSettings["ssrsPassword"]);
                string domain = !string.IsNullOrEmpty(Map.Database.SsrsDomain) ? Map.Database.SsrsDomain : Convert.ToString(System.Configuration.ConfigurationManager.AppSettings["ssrsDomain"]);
                string reportServerUrl = !string.IsNullOrEmpty(Map.Database.SsrsReportServerUrl) && Map.Database.SsrsReportServerUrl.Split('|').Length == 3 ? Map.Database.SsrsReportServerUrl.Split('|')[2] : Convert.ToString(System.Configuration.ConfigurationManager.AppSettings["reportServerUrl"]);
                string reportPath = !string.IsNullOrEmpty(Map.Database.SsrsPath) ? Map.Database.SsrsPath : Convert.ToString(System.Configuration.ConfigurationManager.AppSettings["reportPath"]);

                string reportName = Request.QueryString["ReportName"];
                string reportDisplayName = Request.QueryString["ReportDisplayName"];
                //ReportViewer1.ServerReport.ReportServerCredentials = new ReportServerCredentials("itay", "dio2008", "devitoutsrv1");
                ReportViewer1.ServerReport.ReportServerUrl = new Uri(reportServerUrl);
                ReportViewer1.ServerReport.ReportPath = reportPath + reportName;
                ReportViewer1.ServerReport.DisplayName = reportDisplayName;

                bool newWindow = !string.IsNullOrEmpty(this.Request.QueryString["NewWindow"]);
                if (newWindow)
                {
                    ReportViewer1.SizeToReportContent = false;
                    ReportViewer1.Height = Unit.Percentage(100);
                    ReportViewer1.ZoomMode = Microsoft.Reporting.WebForms.ZoomMode.FullPage;
                }

                ReportServerCredentials reportServerCredentials = null;
                if (string.IsNullOrEmpty(username))
                {
                    reportServerCredentials = new ReportServerCredentials();
                }
                else
                {
                    reportServerCredentials = new ReportServerCredentials(username, password, domain);
                }

                ReportViewer1.ServerReport.ReportServerCredentials = reportServerCredentials;

                //Add user id and user role parameters
                //Add by miri h
                try
                {
                    Microsoft.Reporting.WebForms.ReportParameter[] Param = new Microsoft.Reporting.WebForms.ReportParameter[2];
                    Param[0] = new Microsoft.Reporting.WebForms.ReportParameter("UserId", Map.Database.GetUserID());
                    Param[1] = new Microsoft.Reporting.WebForms.ReportParameter("UserRole", Map.Database.GetUserRole());
                    ReportViewer1.ServerReport.SetParameters(Param);
                }
                catch (Exception ex)
                {
                    Map.Logger.Log("Report", "SetParameters", "Page_Load", ex, 1, "");
                }
                ReportViewer1.ServerReport.Refresh();
            }
        }
    }
}
