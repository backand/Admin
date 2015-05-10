<%@ Page Title="Back&" Language="C#" MasterPageFile="~/Website/Main.Master" AutoEventWireup="true"%>
<%@ Register TagPrefix="My" TagName="MenuControl" Src="Menu.ascx" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="Main" runat="server">
<My:MenuControl runat="server" ID="MenuControl1" MenuSelected=""/>
        <div class="rowcontent page404">
            <div class="container">
                <div class="row">
                    <div class="col-md-9 text-justify">
<%Exception objError = Server.GetLastError();//.GetBaseException();

  StringBuilder lasterror = new StringBuilder();
  if (objError != null)
  {

      lasterror.AppendLine("Error:");
      lasterror.AppendLine();

      if (objError.Message != null)
      {
          lasterror.AppendLine("Message:");
          lasterror.AppendLine(objError.Message);
          lasterror.AppendLine();
      }

      if (objError.InnerException != null)
      {
          lasterror.AppendLine("InnerException:");
          lasterror.AppendLine(objError.InnerException.ToString());
          lasterror.AppendLine();
      }

      if (objError.Source != null)
      {
          lasterror.AppendLine("Source:");
          lasterror.AppendLine(objError.Source);
          lasterror.AppendLine();
      }

      if (objError.StackTrace != null)
      {
          lasterror.AppendLine("StackTrace:");
          lasterror.AppendLine(objError.StackTrace);
          lasterror.AppendLine();
      }

  }
 %>
 
        <h3>Oops it seems something went wrong...</h3>
        <p><%=lasterror.ToString()%></p>
        <br />
        <br />
        <p>If this error continues please contacts us at: <a href="mailto:support@backand.com">support@backand.com</a></p>
        </div>
        </div>
        </div>
        </div>
</asp:Content>