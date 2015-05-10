<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<System.Data.DataView>" %>
<%@ Import Namespace="System.Data"%>
 <%  Durados.DictionaryType dictionaryType = ViewData["DictionaryType"] != null ? (Durados.DictionaryType)ViewData["DictionaryType"]:Durados.DictionaryType.DisplayNames; %>
<% if( ViewData["PlaceHolders"] != null) {%>
<table cellspacing="0" cellpadding="0"  class="dictionary-table">
  <tr name="rowtitle">
    
            <td style="font-weight:bold"><u>Select sytem token:</u></td>
           
        </tr>
        <!-- DATA -->
    <%DataView placeHolders = (DataView)ViewData["PlaceHolders"]; %>
        <% bool toPadsys = (dictionaryType != Durados.DictionaryType.DisplayNames); %>
        <% foreach (DataRowView row in placeHolders)
           { %>
            <tr>
                <td>
                <%
                    Durados.DataType type = (Durados.DataType)row.Row["DataType"];
                    string token = toPadsys?row.Row["Token"].ToString().AsToken().Pad(type):row.Row["Token"].ToString().AsToken();
                    
                                            %>
                    <span class="dic-curr" value="<%=token%>"> <%=row.Row["Tag"].ToString()%> </span></td><td>
                   
                </td>
              
            </tr>
            <%} %>
    </table>
<%} %>
    <table cellspacing="0" cellpadding="0"  class="dictionary-table">
   
        <!-- COLUMNS HEADER -->
        <tr id="rowtitle">
        <%string title = dictionaryType == Durados.DictionaryType.PlaceHolders ? "Select sytem token" : "Select columns";%>
            <td style="font-weight:bold"><u><%=title%>:</u></td>
           
        </tr>
        <!-- DATA -->
        <% bool toPad = (dictionaryType != Durados.DictionaryType.DisplayNames); %>
        <% foreach (DataRowView row in Model)
           { %>
            <tr>
                <td>
                <%
                    Durados.DataType type = (Durados.DataType)row.Row["DataType"];
                    string token = toPad?row.Row["Token"].ToString().AsToken().Pad(type):row.Row["Token"].ToString().AsToken();
                    string prevToken = toPad ? row.Row["Token"].ToString().AsToken(false).Pad(type) : row.Row["Token"].ToString().AsToken(false);
                                            %>
                    <span class="dic-curr" value="<%=token%>"> <%=row.Row["Tag"].ToString()%> </span></td><td>
                    <span class="dic-prev" style="display:none" value="<%= prevToken%>">Value before action</span>
                </td>
              
            </tr>
            <%} %>
    </table>
  
