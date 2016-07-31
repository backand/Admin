using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Durados
{
    public class Query : Durados.Services.ISecurable
    {
        public Query()
        {

        }
        [Durados.Config.Attributes.ColumnProperty()]
        public int ID { get; set; }

        [Durados.Config.Attributes.ColumnProperty()]
        public string Name { get; set; }

        [Durados.Config.Attributes.ColumnProperty()]
        public string SQL { get; set; }
        
        [Durados.Config.Attributes.ColumnProperty()]
        public string NoSQL { get; set; }

        [Durados.Config.Attributes.ColumnProperty()]
        public string Parameters { get; set; }
        
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

        
    }

    
}
