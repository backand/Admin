<%@ Page Title="Back&" Language="C#" MasterPageFile="~/Website/Main.Master" AutoEventWireup="true" %>

<%@ Register TagPrefix="My" TagName="MenuControl" Src="../../Menu.ascx" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="Main" runat="server">
    <My:MenuControl runat="server" ID="MenuControl1" MenuSelected="" />
    <div class="rowcontent page404">
        <div class="container">
            <div class="row">
                <div class="col-md-9 text-justify">
                    <h1>
                        Import From Excel</h1>
                    <p>
                        <span>1. Open "App settings".</span><br />
                        You can do that either&nbsp; by double clicking the list or by right-click the list
                        and in the context menu select "App Settings".
                    </p>
                    <img alt="Import From Excel" src="../Images/ImportFromExcelIcon.png" /><br />
                    <br />
                    <p>
                        <span>2. Wait for the excel icon to turn green and then click it. The Import window
                            will open.</span></p>
                    <img alt="Import Schema And Data From Excel Window" src="../Images/ImportSchemaAndDataFromExcelWindow.PNG" />
                    <br />
                    <br />
                    <p>
                        <span>3. Click the "Upload" button.</span></p>
                    <p>
                        <span>4. Select your excel file </span>
                        <br />
                        In the first sheet the file should containing your data.The first row should hold
                        the columns headres.<br />
                        Please omit any special character (eg. ,':;" ) from the column Headers</p>
                    <img alt="Import From Excel Open Window Server" src="../Images/ImportFromExcelOpenWindowServer.png" /><br />
                    <br />
                    <p>
                        <span>5. Click Open</span></p>
                    <p>
                        <span>6. Choose you import method.</span><br />
                        Choosing "Replace" will delete all the current data in the list and replace it with
                        the new columns and data from your excel.
                        <br />
                        Choosing "Merge" will maintain the current data in the list and merge it with the
                        new columns and data from your excel.
                    </p>
                    <p>
                        <span>7. Now it's time to click "Run" and let
                            <%=Durados.Database.ShortProductName %>
                            List Creator create your own custom list.</span>
                </div>
            </div>
        </div>
        <%--<div class="side-item-container">
      <div class="side-item tutorial">Video Demo</div>
      <div class="side-item demo">Working Demo</div>
      <div class="side-item try">TRY</div>
    </div>--%>
    </div>
    <%--</section>--%>
</asp:Content>
