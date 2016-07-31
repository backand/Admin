<%@ Control Language="C#" Inherits="System.Web.Mvc.UI.Views.BaseViewUserControl<string>" %>
<%
    bool? insideTextSearch=(bool?)ViewData["insideTextSearch"];
    string insideTextSearchAttr = insideTextSearch.HasValue && Model == Durados.Web.Mvc.UI.Json.AdvancedFilterType.TextFilter.ToString() ?
        " insideTextSearch='" + insideTextSearch.Value + "'" : "";
    %>
<div class="advancedFilterDialog ui-widget" id="<%=Model %>" <%=insideTextSearchAttr %>>
    <div>
        <form>
        <select class="filter-equal">
            <%
                IDictionary<string, string> comparersTranslations = null;

                if (Model == Durados.Web.Mvc.UI.Json.AdvancedFilterType.TextFilter.ToString())
                    comparersTranslations = Durados.DataAccess.Filter.StringFilterComparisons;
                else
                    comparersTranslations = Durados.DataAccess.Filter.MathematicsFilterComparisons;

                foreach (KeyValuePair<string, string> item in comparersTranslations)
                {
            %>
            <option value='<%=item.Key %>'>
                <%= Map.Database.Localizer.Translate(item.Value)%></option>
            <%
                }
            %>
        </select>
        <br />
        <% if (Model == Durados.Web.Mvc.UI.Json.AdvancedFilterType.DateFilter.ToString())
           { %>
        <span id='date1' class="dateContainer">
            <input type="text" name="first" class="date___ filter-date" />
            <span class="textfieldInvalidFormatMsg"></span></span>
        <%}
           else if (Model == Durados.Web.Mvc.UI.Json.AdvancedFilterType.NumericFilter.ToString())
           { %>
        <span id='numeric1'>
            <input class="filter-numeric" type="text" name="first" />
            <span class="textfieldInvalidFormatMsg">invalid number</span> </span>
        <%}
           else
           { %>
        <span id='text1'>
            <input class="filter-text" type="text" name="first" />
            <span class="textfieldInvalidFormatMsg">invalid text</span> </span>
        <%}
        %>
        <div class="between" style="display: none">
            <span><%= Map.Database.Localizer.Translate("To")%></span>
            <br />
            <% if (Model == Durados.Web.Mvc.UI.Json.AdvancedFilterType.DateFilter.ToString())
               { %>
            <span id='date2' class="dateContainer">
                <input type="text" name="second" class="date___ filter-date" />
                <span class="textfieldInvalidFormatMsg"></span></span>
            <%}
               else
               { %>
            <span id='numeric2'>
                <input class="filter-numeric" type="text" name="second" />
                <span class="textfieldInvalidFormatMsg"></span></span>
            <%} %>
        </div>
        <span class="advancedFilterDialog-buttons">
            <br />
            <button type="button" class="ui-button ui-widget ui-state-default ui-corner-all ui-button-text-only"
                role="button" aria-disabled="false" onclick="return okAdvancedFilter();">
                <span class="ui-button-text"><%= Map.Database.Localizer.Translate("OK")%></span></button>
            <button type="button" class="ui-button ui-widget ui-state-default ui-corner-all ui-button-text-only"
                role="button" aria-disabled="false" onclick="return cancelAdvancedFilter();">
                <span class="ui-button-text"><%= Map.Database.Localizer.Translate("Cancel")%></span></button>
        </span>
        </form>
    </div>
</div>
