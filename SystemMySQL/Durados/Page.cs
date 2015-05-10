using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Durados
{
    public class Page:Durados.Services.ISecurable
    {
        [Durados.Config.Attributes.ColumnProperty()]
        public int ID { get; set; }

        [Durados.Config.Attributes.ColumnProperty()]
        public int Order { get; set; }

        [Durados.Config.Attributes.ColumnProperty(Description="Appears in the tile and the menu")]
        public string Title { get; set; }

        [Durados.Config.Attributes.ColumnProperty(Description="The html content")]
        public string Content { get; set; }

        [Durados.Config.Attributes.ColumnProperty(Description="Overwrites the content with iframe")]
        public string ExternalPage { get; set; }

        [Durados.Config.Attributes.ColumnProperty(Description = "In Pixels. Relevant if External Page, default 100%")]
        public int Width { get; set; }

        [Durados.Config.Attributes.ColumnProperty(Description = "In Pixels. Relevant if External Page.")]
        public int Height { get; set; }

        [Durados.Config.Attributes.ColumnProperty(Description = "Relevant if External Page.")]
        public bool Scroll { get; set; }


        [Durados.Config.Attributes.ColumnProperty(Description = "Open new page")]
        public string ExternalNewPage { get; set; }

        [Durados.Config.Attributes.ColumnProperty(Description = "Open in new tab.")]
        public bool NewTab { get; set; }

        [Durados.Config.Attributes.ColumnProperty(Description = "Target")]
        public string Target { get; set; }

        [Durados.Config.Attributes.ColumnProperty(Description = "Report Services rdlc and relative path")]
        public string ReportName { get; set; }

        
        [Durados.Config.Attributes.ColumnProperty(Description = "The title of the report")]
        public string ReportDisplayName { get; set; }

        [Durados.Config.Attributes.ColumnProperty(Description = "Report type")]
        public PageType PageType { get; set; }

        

        private Menu menu = null;
        [Durados.Config.Attributes.ParentProperty(TableName = "Menu", DoNotCopy = true, RelationName="PageMenu")]
        public Menu Menu
        {
            get
            {
                return menu;
            }
            set
            {
                menu = value;
            }
        }

        public Database Database { get;  set; }
        public Workspace Workspace
        {
            get
            {
                if (!Database.Workspaces.ContainsKey(WorkspaceID))
                    return null;
                return Database.Workspaces[WorkspaceID];
            }
        }
        //private Workspace workspace = null;
        //[Durados.Config.Attributes.ParentProperty(TableName = "Workspace", DoNotCopy = true, RelationName = "PageWorkspace")]
        //public Workspace Workspace
        //{
        //    get
        //    {
        //        return workspace;
        //    }
        //    set
        //    {
        //        workspace = value;
        //    }
        //}
        [Durados.Config.Attributes.ColumnProperty(DoNotCopy = true, Description = "A group of views that have a common security configuration")]
        public int WorkspaceID { get; set; }
       
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

        internal protected void SetMenu(Menu menu)
        {
            this.menu = menu;
        }

        public bool IsAllow()
        {
            return Database.IsAllow(this);
            
            // return !UI.Helpers.SecurityHelper.IsDenied(DenySelectRoles, AllowSelectRoles) || UI.Helpers.SecurityHelper.IsConfigViewForViewOwner(this);
        }
    }

    [Durados.Config.Attributes.EnumDisplay(EnumDisplayNames = new string[4] { "Content", "External - IFrame", "External - Link", "Reporting Services", }, EnumNames = new string[4] { "Content", "IFrame", "External", "ReportingServices" })]
    public enum PageType
    {
        Content,
        IFrame,
        External,
        ReportingServices
    }
}
