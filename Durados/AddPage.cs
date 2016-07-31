using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;

namespace Durados
{
    public class AddPage
    {
        Dictionary<LinkType, Dictionary<LinkSubType, string>> types = null;


        Dictionary<string, string> linkTypeDisplayNames = null;
        Dictionary<string, string> linkSubTypeDisplayNames = null;
            
        public AddPage()
        {
            
            Type linkType = typeof(LinkType);

            object[] attributes = linkType.GetCustomAttributes(typeof(Durados.Config.Attributes.EnumDisplayAttribute), true);
            if (attributes.Length == 1)
            {
                linkTypeDisplayNames = ((Durados.Config.Attributes.EnumDisplayAttribute)attributes[0]).GetEnumDisplayNames();
            }

            Type linkSubType = typeof(LinkSubType);

            attributes = linkSubType.GetCustomAttributes(typeof(Durados.Config.Attributes.EnumDisplayAttribute), true);
            if (attributes.Length == 1)
            {
                linkSubTypeDisplayNames = ((Durados.Config.Attributes.EnumDisplayAttribute)attributes[0]).GetEnumDisplayNames();
            }

            InitTypes();

        }

        private void InitTypes()
        {
            types = new Dictionary<LinkType, Dictionary<LinkSubType, string>>();
            types.Add(LinkType.View, new Dictionary<LinkSubType, string>());
            types[LinkType.View].Add(LinkSubType.Grid1, GetLinkSubTypeDisplayName(LinkSubType.Grid1));
            types[LinkType.View].Add(LinkSubType.Grid2, GetLinkSubTypeDisplayName(LinkSubType.Grid2));
            types[LinkType.View].Add(LinkSubType.Cards, GetLinkSubTypeDisplayName(LinkSubType.Cards));
            types[LinkType.View].Add(LinkSubType.Preview, GetLinkSubTypeDisplayName(LinkSubType.Preview));
            
            types.Add(LinkType.MyCharts, new Dictionary<LinkSubType, string>());
            types[LinkType.MyCharts].Add(LinkSubType.Charts, GetLinkSubTypeDisplayName(LinkSubType.Charts));
            
            types.Add(LinkType.Page, new Dictionary<LinkSubType, string>());
            types[LinkType.Page].Add(LinkSubType.HtmlCustom, GetLinkSubTypeDisplayName(LinkSubType.HtmlCustom));
            types[LinkType.Page].Add(LinkSubType.EmbeddedIFrame, GetLinkSubTypeDisplayName(LinkSubType.EmbeddedIFrame));
            types[LinkType.Page].Add(LinkSubType.EmbeddedLink, GetLinkSubTypeDisplayName(LinkSubType.EmbeddedLink));
            
            types.Add(LinkType.Report, new Dictionary<LinkSubType, string>());
            types[LinkType.Report].Add(LinkSubType.ReportingServices, GetLinkSubTypeDisplayName(LinkSubType.ReportingServices));
            types[LinkType.Report].Add(LinkSubType.MailChimp, GetLinkSubTypeDisplayName(LinkSubType.MailChimp));
            types[LinkType.Report].Add(LinkSubType.Ongage, GetLinkSubTypeDisplayName(LinkSubType.Ongage));
            types[LinkType.Report].Add(LinkSubType.AppNotifications, GetLinkSubTypeDisplayName(LinkSubType.AppNotifications));
            
        }

        public Dictionary<LinkType, Dictionary<LinkSubType, string>> Types
        {
            get
            {
                return types;
            }
        }

        public string GetLinkTypeDisplayName(LinkType linkType)
        {
            return linkTypeDisplayNames[linkType.ToString()];
        }

        public string GetLinkSubTypeDisplayName(LinkSubType linkSubType)
        {
            return linkSubTypeDisplayNames[linkSubType.ToString()];
        }

        

        public DataView EntityTable { get; set; }

        public List<string> Tables { get; set; }

        public Dictionary<int,string> Dashboards { get; set; }


        public string Content { get; set; }

        public string GetValue(DataRow row)
        {
            return row["Name"].ToString();
        }

        public string GetText(DataRow row)
        {
            return row["Name"].ToString() + " (" + row["EntityType"].ToString() + ")" + (row.IsNull("DisplayName") ? "" : " \"" + row["DisplayName"].ToString() + "\"" );
        }

        public string GetEditableText(DataRow row)
        {
            return row["Name"].ToString() + (row.IsNull("DisplayName") ? "" : " \"" + row["DisplayName"].ToString() + "\"");
        }

        public string GetDisplayName(DataRow row)
        {
            return row.IsNull("DisplayName") ? "" : row["DisplayName"].ToString();
        }

        public bool IsView(DataRow row)
        {
            return row["EntityType"].ToString().ToLower().Equals("view");
        }

        public bool IsNew(DataRow row)
        {
            return row.IsNull("DisplayName");
        }

        public bool IsNewView(DataRow row)
        {
            return IsNew(row) && IsView(row);
        }
    }

    [Durados.Config.Attributes.EnumDisplay(EnumNames = new string[4] { "View", "Page", "MyCharts", "Report" }, EnumDisplayNames = new string[4] { "Tables/Views", "Content", "Dashboard", "Integrations" })]
    public enum LinkType
    {
        View,
        Page,
        MyCharts,
        Report
    }

    [Durados.Config.Attributes.EnumDisplay(EnumNames = new string[12] { "Grid1", "Grid2", "Cards", "Preview", "Charts", "HtmlCustom", "EmbeddedIFrame", "EmbeddedLink", "ReportingServices", "MailChimp", "Ongage", "AppNotifications" },
        EnumDisplayNames = new string[12] { "Grid1", "Grid2", "Cards", "Preview", "Dashboard Charts", "HTML Custom", "Embedded iFrame", "Link", "Reporting Services", "MailChimp", "Ongage", "App Notifications" })]
    public enum LinkSubType
    {
        Grid1,
        Grid2,
        Cards,
        Preview,

        Charts,

        HtmlCustom, 
        EmbeddedIFrame, 
        EmbeddedLink,

        ReportingServices,
        MailChimp,
        Ongage,
        AppNotifications

    }
}
