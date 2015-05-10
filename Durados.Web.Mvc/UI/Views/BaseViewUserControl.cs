using System;
using System.Data;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Xml.Linq;
using System.Web.Mvc;
using System.Threading;
using System.Globalization;

using Durados.Web.Mvc;
using Durados.Web.Mvc.UI.Helpers;

namespace System.Web.Mvc.UI.Views
{
    public class BaseViewUserControl : System.Web.Mvc.ViewUserControl
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

        public Durados.Web.Mvc.Database Database
        {
            get
            {
                if (ViewData["Database"] == null)
                    return Map.Database;
                else
                    return (Durados.Web.Mvc.Database)ViewData["Database"];
            }
        }

        public string ViewName
        {
            get
            {
                return ViewData["ViewName"].ToString();
            }
        }

        public Durados.DataAction DataAction
        {
            get
            {
                if (ViewData["DataAction"] == null)
                    return Durados.DataAction.Create;
                else
                    return (Durados.DataAction)ViewData["DataAction"];
            }
        }
    }

    public class BaseViewUserControl<TModel> : ViewUserControl<TModel> where TModel : class
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

        public Durados.Web.Mvc.Database Database
        {
            get
            {
                if (ViewData["Database"] == null)
                    return Map.Database;
                else
                    return (Durados.Web.Mvc.Database)ViewData["Database"];
            }
        }

        public string ViewName
        {
            get
            {
                return ViewData["ViewName"].ToString();
            }
        }

        public Durados.DataAction DataAction
        {
            get
            {
                if (ViewData["DataAction"] == null)
                    return Durados.DataAction.Create;
                else
                    return (Durados.DataAction)ViewData["DataAction"];
            }
        }
    }

    public class DataActionFieldsViewUserControl<TModel> : BaseViewUserControl<TModel> where TModel : Durados.Web.Mvc.UI.DataActionFields
    {
        public string GetInitiationJavaScript(string guid)
        {
            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            sb.Append("$(document).ready(function() {");
            sb.Append(Environment.NewLine);
            sb.Append("Forms.Add('" + Model.DataAction.ToString() + "_" + View.Name + "_" + "Form', '" + View.Name + "', '" + Model.DataAction.ToString() + "', '" + View.GetDataActionUrl(Model.DataAction) + "', " + View.GetJsonView(Model.DataAction, guid) + ", '" + View.GetAutoCompleteUrl() + "', '" + Request.ApplicationPath + "', '" + View.GetDateFormat() + "');");
            sb.Append(Environment.NewLine);
            sb.Append("Forms.HandleSpecifics('" + Model.DataAction.ToString() + "_" + View.Name + "_" + "Form');");
            sb.Append(Environment.NewLine);
            sb.Append("if ('" + Model.DataAction.ToString() + "' == 'Edit'){");
            sb.Append(Environment.NewLine);
            sb.Append("Forms.GetForm('" + Model.DataAction.ToString() + "_" + View.Name + "_" + "Form').Load('" + View.GetJsonViewUrl() + "', '" + (ViewData["PK"] == null ? string.Empty : ViewData["PK"].ToString()).ToString() + "');");
            sb.Append(Environment.NewLine);
            sb.Append("}");
            sb.Append(Environment.NewLine);
            sb.Append("});");
            sb.Append(Environment.NewLine);


            //$(document).ready(function() {
            //    Forms.Add('<%=Model.DataAction.ToString() + "_" + View.Name + "_" %>Form', '<%=View.Name %>', '<%=Model.DataAction.ToString() %>', '<%=View.GetDataActionUrl(Model.DataAction) %>', <%=View.GetJsonView(Model.DataAction) %>, '<%=View.GetAutoCompleteUrl() %>', '<%= Request.ApplicationPath%>', '<%=View.GetDateFormat() %>');
            //    Forms.HandleSpecifics('<%=Model.DataAction.ToString() + "_" + View.Name + "_" %>Form');
            //    if ('<%=Model.DataAction.ToString()%>' == "Edit"){
            //        Forms.GetForm('<%=Model.DataAction.ToString() + "_" + View.Name + "_" %>Form').Load('<%=View.GetJsonViewUrl() %>', '<%= ViewData["PK"] == null ? string.Empty : ViewData["PK"].ToString() %>');
            //    }    
            //}); 

            return sb.ToString();
        }

        public Durados.Web.Mvc.View View
        {
            get
            {
                return (Durados.Web.Mvc.View)Model.Fields.First().View;

            }
        }
    }

}
