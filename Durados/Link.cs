using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Durados
{
    public class Link
    {
        public Link()
        {
            WorkspaceID = -1;
        }

        [Durados.Config.Attributes.ColumnProperty()]
        public string Title { get; set; }

        [Durados.Config.Attributes.ColumnProperty()]
        public string Url { get; set; }

        [Durados.Config.Attributes.ColumnProperty()]
        public string ReportServicePath { get; set; }

        [Durados.Config.Attributes.ColumnProperty()]
        public string ViewName { get; set; }

        [Durados.Config.Attributes.ColumnProperty()]
        public int Order { get; set; }

        [Durados.Config.Attributes.ColumnProperty(Description="The target of the page. Empty by default. If empty the page will be open in the current window.")]
        public string Target { get; set; }

        [Durados.Config.Attributes.ColumnProperty(Description = "The workspace security, Relevant only when Url or Report Service Path is supplied.")]
        public int WorkspaceID { get; set; }

        public Workspace GetWorkspace(Database database)
        {
            if (!database.Workspaces.ContainsKey(WorkspaceID))
                return null;
            return database.Workspaces[WorkspaceID];
        }

        [Durados.Config.Attributes.ColumnProperty()]
        public bool Dashboard { get; set; }

        [Durados.Config.Attributes.ColumnProperty()]
        public string Description { get; set; }

        [Durados.Config.Attributes.ColumnProperty()]
        public string Icon { get; set; }

        [Durados.Config.Attributes.ColumnProperty()]
        public LinkType LinkType { get; set; }

    }
}
