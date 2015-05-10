using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Durados
{
    public class Chart : Durados.Services.ISecurable
    {
        [Durados.Config.Attributes.ColumnProperty()]
        public int ID { get; set; }
        
        [Durados.Config.Attributes.ColumnProperty()]
        public string Name { get; set; }

        [Durados.Config.Attributes.ColumnProperty()]
        public string SubTitle { get; set; }

        [Durados.Config.Attributes.ColumnProperty()]
        public ChartType ChartType { get; set; }

        [Durados.Config.Attributes.ColumnProperty()]
        public string SQL { get; set; }
        
        [Durados.Config.Attributes.ColumnProperty()]
        public string XField { get; set; }

        [Durados.Config.Attributes.ColumnProperty()]
        public string YField { get; set; }

        [Durados.Config.Attributes.ColumnProperty()]
        public string XTitle { get; set; }

        [Durados.Config.Attributes.ColumnProperty()]
        public string YTitle { get; set; }

        [Durados.Config.Attributes.ColumnProperty()]
        public int Height { get; set; }

        [Durados.Config.Attributes.ColumnProperty()]
        public ChartAlignment Align { get; set; }

        [Durados.Config.Attributes.ColumnProperty()]
        public int Ordinal { get; set; }

        private int column;
        [Durados.Config.Attributes.ColumnProperty()]
        public int Column { get { return (column!=0 ?column:(Align==ChartAlignment.Left?1:2)); } set { column = value; } }

        public bool IsVisible(string role)
        {
            return role == "Admin" || role == "Developer" || !string.IsNullOrEmpty(SQL) && IsAllow();
        }
        private int workspaceId = 0;
        [Durados.Config.Attributes.ColumnProperty(DoNotCopy = true, Description = "A group of views that have a common security configuration")]
        public int WorkspaceID { get { return workspaceId; } set { workspaceId = value; } }
       
        public Database Database { get;  set; }
        public Workspace Workspace
        {
            get
            {
                if (Database == null || !Database.Workspaces.ContainsKey(WorkspaceID))
                    return null;
                return Database.Workspaces[WorkspaceID];
            }
        }
        [Durados.Config.Attributes.ColumnProperty(DoNotCopy = true, Description = "If true page takes the page roles, otherwise the page takes the workspace roles")]
        public bool Precedent { get; set; }

        private string allowSelectRoles;

        [Durados.Config.Attributes.ColumnProperty(DoNotCopy = true, Description = "Hide/Show page to users with these roles.")]
        public string AllowSelectRoles
        {
            get
            {

                if (Precedent)
                    return allowSelectRoles;
                else if (Workspace != null && Workspace.Precedent)
                    return Workspace.AllowSelectRoles;
                else
                    return Database.DefaultAllowSelectRoles;
            }
            set
            {
                allowSelectRoles = value;
            }
        }

        

        public bool IsAllow()
        {
            return Database.IsAllow(this);

        }

        [Durados.Config.Attributes.ColumnProperty()]
        public string GaugeGreen { get; set; }

        [Durados.Config.Attributes.ColumnProperty()]
        public string GaugeYellow { get; set; }

        [Durados.Config.Attributes.ColumnProperty()]
        public string GaugeRed { get; set; }

        [Durados.Config.Attributes.ColumnProperty()]
        public int RefreshInterval { get; set; }

        [Durados.Config.Attributes.ColumnProperty()]
        public int GaugeMinValue { get; set; }

        [Durados.Config.Attributes.ColumnProperty()]
        public int GaugeMaxValue { get; set; }

        [Durados.Config.Attributes.ColumnProperty()]
        public bool ShowTable { get; set; }

       
       
        public int GetColumn()
        {
            int column = Column;
            int? dashboardId = Database.GetDashboardPK(ID);
            if (dashboardId.HasValue)
            {
                int columns = Database.Dashboards[dashboardId.Value].Columns;
                column = (Column % (columns + 1));
                if (column == 0)
                    column = columns;
            }
            return column;
        }
    }

    public enum ChartAlignment
    {
        Left,
        Right,
    }
}
