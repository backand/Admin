<%@ Control Language="C#" Inherits="System.Web.Mvc.UI.Views.BaseViewUserControl<Durados.Web.Mvc.View>" %>
<%  
    string guid = ViewData["guid"] as string;
    Durados.DataDisplayType? displayType = ViewData["displayType"] as Durados.DataDisplayType?;
    bool needDisplayToolbar = (Convert.ToInt32(Model.EnableTableDisplay) + Convert.ToInt32(Model.EnableDashboardDisplay) + Convert.ToInt32(Model.EnablePreviewDisplay)) > 1;
    string onclick = "Durados.DisplayType.changeDisplayType('" + guid + "', '{0}');return false;";
    string alternateOnclick = "return false;";
    bool? tablecommandDisplay = ViewData["tablecommandDisplay"] as bool?;

    if (needDisplayToolbar)
    {   
%>
<%--<div class="group">--%>
    <a href="#" style="width:75px;"><span class="icon" style="width:110px;">
        <%if (Model.EnableTableDisplay)
          {
              string _onclick = displayType == Durados.DataDisplayType.Table ?
                    alternateOnclick : string.Format(onclick, "Table");
        %>
        <img alt="" src="<%=Durados.Web.Mvc.Infrastructure.General.GetRootPath() + "Content/Images/grid.png" %>"
            class="dashboard_img float" onclick="<% =_onclick %>" title="<%=Map.Database.Localizer.Translate("Grid View")%>" />
        <%
          }

          if (Model.EnableDashboardDisplay)
          {
              string _onclick = displayType == Durados.DataDisplayType.Dashboard ?
                    alternateOnclick : string.Format(onclick, "Dashboard");
        %>
        <img alt="" src="<%=Durados.Web.Mvc.Infrastructure.General.GetRootPath() + "Content/Images/dashboard.png" %>"
            class="dashboard_img float disabled" onclick="<% =_onclick %>" title="<%=Map.Database.Localizer.Translate("Summary")%>" />
        <%
          }

          if (Model.EnablePreviewDisplay)
          {
              string _onclick = displayType == Durados.DataDisplayType.Preview ?
                    alternateOnclick : string.Format(onclick, "Preview");
        %>
        <img alt="" src="<%=Durados.Web.Mvc.Infrastructure.General.GetRootPath() + "Content/Images/preview.gif" %>"
            class="dashboard_img float disabled" onclick="<% =_onclick %>" title="Preview" />
        <%
      }
      
        %>
        </span>
        </a>
<%--</div>--%>
<%
    }
%>
