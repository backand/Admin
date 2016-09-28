﻿using System;
using System.Data;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Web;
using System.IO;
using System.Reflection;
using System.Data.SqlClient;

using Microsoft.WindowsAzure;
using Microsoft.WindowsAzure.ServiceRuntime;
using Microsoft.WindowsAzure.StorageClient;
using Microsoft.WindowsAzure.StorageClient.Protocol;

using Durados.DataAccess;
using Durados.Web.Mvc.UI.Helpers;
using Durados.Web.Mvc.Infrastructure;
using Durados.Web.Mvc.Azure;
using Newtonsoft.Json;
using System.Diagnostics;
using Durados.Data;
using Durados.SmartRun;
using Durados.Web.Mvc.Farm;

namespace Durados.Web.Mvc
{
    public class DuradosMap : Map
    {
        public override bool IsMainMap
        {
            get
            {
                return true;
            }
        }

        protected override Durados.Web.Mvc.Config.IProject GetProject()
        {
            return new DuradosProject();
        }

        public override string GetLogOnPath()
        {
            string logon = System.Configuration.ConfigurationManager.AppSettings["MainLogOnPath"] ?? "~/Views/Account/LogOn.aspx";

            return logon;
        }

        public override bool GetLogMvc()
        {
            return Convert.ToBoolean(System.Configuration.ConfigurationManager.AppSettings["MainLogOnMvc"] ?? "true");

        }

        public Dictionary<string, string> GetUserApps()
        {
            int rowCount = 0;
            Dictionary<string, object> values = new Dictionary<string, object>();

            Durados.View appsView = Database.Views["durados_App"];

            DataView dataView = null;
            try
            {
                dataView = appsView.FillPage(1, 10000, values, null, null, out rowCount, null, null);
            }
            catch (Exception)
            {
                AddSslAndAahKeyColumn();
                dataView = appsView.FillPage(1, 10000, values, null, null, out rowCount, null, null);
            }

            AddSslAndAahKeyColumnConfig();

            Dictionary<string, string> apps = new Dictionary<string, string>();
            foreach (System.Data.DataRowView row in dataView)
            {
                apps.Add(row["Id"].ToString(), row["Title"].ToString());
            }

            return apps;
        }

        public void AddSslAndAahKeyColumn()
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                using (SqlCommand command = new SqlCommand())
                {
                    command.Connection = connection;
                    new SqlSchema().AddNewColumnToTable("durados_SqlConnection", "SslUses", DataType.Boolean, command);
                    new SqlSchema().AddNewColumnToTable("durados_SqlConnection", "SshPrivateKey", DataType.LongText, command);
                }
            }
        }

        public void AddSslAndAahKeyColumnConfig()
        {
            View connectionsView = (View)Database.Views["durados_SqlConnection"];

            if (!connectionsView.Fields.ContainsKey("SslUses"))
            {
                connectionsView.Fields.Add("SslUses", new ColumnField(connectionsView, connectionsView.DataTable.Columns["SslUses"]));
            }
            if (!connectionsView.Fields.ContainsKey("SshPrivateKey"))
            {
                connectionsView.Fields.Add("SshPrivateKey", new ColumnField(connectionsView, connectionsView.DataTable.Columns["SshPrivateKey"]));
            }
        }

        public Dictionary<string, string> GetUserApps(int userId)
        {
            Dictionary<string, object> values = new Dictionary<string, object>();
            SqlAccess sqlAccess = new SqlAccess();

            DataTable table = sqlAccess.ExecuteTable(Maps.Instance.DuradosMap.Database.ConnectionString, "select * from durados_App where durados_app.[ToDelete]=0 AND  Creator = " + userId + " or id in (select durados_UserApp.AppId from durados_UserApp where durados_UserApp.UserId = " + userId + ") ", null, CommandType.Text);

            Dictionary<string, string> apps = new Dictionary<string, string>();
            foreach (System.Data.DataRow row in table.Rows)
            {
                apps.Add(row["Id"].ToString(), row["Name"].ToString());
            }

            return apps;
        }

        public virtual string GetPlanContent()
        {
            if (string.IsNullOrEmpty(Database.PlanContent) || Database.PlanContent == "<a title=\"Change plan\"><img class=\"plan\" onclick=\"setPlan(this)\" src=\"/Content/Images/Plan.png\"></a>")
            {
                return "<span class='plan' onclick='setPlan(this)'>Upgrade to Premium</span>";//"<img class='plan' onclick='setPlan(this)' src='/Content/Images/Plan.png'/>";
            }

            return Database.PlanContent;
        }

        public override string GetLogoSrc()
        {
            return string.Format("/Content/Images/{0}", this.Database.SiteInfo.Logo);
            //return string.Format("/Home/{0}/{1}?fieldName={2}&amp;fileName={3}&amp;pk='\'", DownloadActionName, "durados_App", "Image", this.Database.SiteInfo.Logo);
        }

        public override SqlProduct SqlProduct
        {
            get
            {
                return SqlProduct.SqlServer;
            }
            set
            {
            }
        }

        public override Guid Guid
        {
            get
            {
                return Guid.Parse(System.Configuration.ConfigurationManager.AppSettings["masterGuid"] ?? Guid.Empty.ToString());
            }
            set
            {
                base.Guid = value;
            }
        }
    }
}
