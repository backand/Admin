<%@ Page Title="" Language="C#" MasterPageFile="~/Website/Main.master" AutoEventWireup="true"  %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
<script type="text/javascript">
    function getParameterByName(name) {
        name = name.replace(/[\[]/, "\\\[").replace(/[\]]/, "\\\]");
        var regex = new RegExp("[\\?&]" + name + "=([^&#]*)"),
        results = regex.exec(location.search);
        return results == null ? "" : decodeURIComponent(results[1].replace(/\+/g, " "));
    }
 $(document).ready(function () {

            var productId = getParameterByName("plan");
            var iframElm = $("#paymentFram");
            var payUrl = "https://sandbox.plimus.com/jsp/buynow.jsp?contractId=" + productId + "&templateId=652024";
            iframElm.attr('src', payUrl);
        });
</script>
<style type="text/css">
#paymentFram
{
   width: 980px;
    height: 850px;
    border: none;
   overflow-y: hidden;
   z-index:4;
    
}
.payment-container
{
    clear: both;
    text-align: center;
    padding: 30px;
}
div.seq-container.main-loader
{
    display:none;
}
</style>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="Main" runat="server">
<div class="payment-container">
 <iframe src="" id="paymentFram" scrolling="no"></iframe>
 </div>
</asp:Content>
