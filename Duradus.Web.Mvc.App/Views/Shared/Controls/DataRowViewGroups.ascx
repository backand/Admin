<%@ Control Language="C#" Inherits="System.Web.Mvc.UI.Views.BaseViewUserControl<Durados.Web.Mvc.UI.DataActionFields>" %>
<%@ Import Namespace="System.Data"%>
<%@ Import Namespace="Durados.Web.Mvc.UI.Helpers"%>

<% 
    string guid = Model.Guid;
    Durados.Web.Mvc.View view = (Durados.Web.Mvc.View)Model.View;
    IEnumerable<Durados.Field> fieldsWithNoCategory = new List<Durados.Field>();
    if (view.FieldsWithNoCategory != null)
        fieldsWithNoCategory = view.GetVisibleFieldsForRow(Model.DataAction, view.FieldsWithNoCategory.Fields);

    List<object> items = new List<object>();
    
    foreach(Durados.Field field in fieldsWithNoCategory.OrderBy(f=>f.Order))
    {
        items.Add(field);
    }
    
    foreach(Durados.Category category in view.Categories.Values.OrderBy(c=>c.Ordinal))
    {
        if (category.Fields.Count>0)
        {
            items.Add(category);
            foreach(Durados.Field field in view.GetVisibleFieldsForRow(Model.DataAction, category.Fields).OrderBy(f=>f.Order))
            {
                items.Add(field);
            }
        }
    }

    int itemsCount = items.Count;
    int columnsInDialog = view.ColumnsInDialog;
    int itemsInColumn = itemsCount / columnsInDialog;
    if (itemsCount % itemsInColumn != 0)
    {
        itemsInColumn++;
    }
    
    
%>

<form id='<%=guid + Model.DataAction.ToString() + view.Name.ReplaceNonAlphaNumeric() %>DataRowForm' enctype="multipart/form-data" action="" onsubmit="return false;">
    <table cellpadding="0" cellspacing="5" >
        <tr>
            <% for (int i = 0; i < columnsInDialog; i++)
            { %>
                <td valign="top" class="groupsColumn">
                    <table>
                        <%for (int j = i * itemsInColumn; j < (i + 1) * itemsInColumn; j++)
                          {
                            if (j >= itemsCount)
                            {
                                break;
                            }
                            object item = items[j];
                            %>
                            <% if (item is Durados.Category){ %>
                            <tr>
                                <td colspan="2" align="center" class="groupsCategory" style="font-weight:bold; color:#718abe">
                                    <% Durados.Category category = (Durados.Category)item; %>
                                    <%=Map.Database.Localizer.Translate(category.Name)%>
                                </td>
                            </tr>
                            <%} else{ %>
                            <tr>
                                <% Durados.Field field = (Durados.Field)item; %>
                                <td valign="top" style="white-space:nowrap; vertical-align:middle; padding-left:20px">
                                    <% if (!(field.FieldType == Durados.FieldType.Parent && ((Durados.Web.Mvc.ParentField)field).ParentHtmlControlType == Durados.Web.Mvc.ParentHtmlControlType.Hidden)){ %>
                                        <%=field.GetLocalizedDisplayName()%>:
                                    <%} %>
                                </td>
                                <td id='the<%=guid + field.GetDataActionPrefix(Model.DataAction) + view.Name + "_" + field.Name %>' colspan='<%=(field.ColSpanInDialog * 2 - 1).ToString() %>' style="vertical-align:middle;">
                                    <%=field.GetElementForRowView(Model.DataAction, guid)%>
                                    <%=field.GetValidationElements(Model.DataAction, guid)%>
                                </td>
                            </tr>
                            <%} %>
                            
                        <%}%>
                    </table>
                </td>
            <%} %>
            </tr>
        </table>
    
</form>
