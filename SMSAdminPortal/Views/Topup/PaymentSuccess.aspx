<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/MainMaster.Master" Inherits="System.Web.Mvc.ViewPage<dynamic>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
    PayPal Payment Successfull
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

    <script type="text/javascript">

        $(document).ready(function () {
            $('#lblTransactionID').text('<%=ViewData["TransactionID"]%>');
            $('#lblUSerName').text('<%=ViewData["UserName"]%>');
            $('#lblEmail').text('<%=ViewData["PayerEmail"]%>');
            $('#lblItemName').text('<%=ViewData["ItemName"]%>');
            $('#lblAmount').text('<%=ViewData["Amount"]%>');
            $('#lblCurrency').text('<%=ViewData["Currency"]%>');
        });

    </script>

    <div style="font-family:Segoe UI Regular,Segoe UI; font-size: 20px; font-weight: 400; margin-left: 100px; margin-top: 20px; width: 800px;">
        PayPal Topup Successful!!!
        <br />
        <table border="1" style="border:solid; border-width:thin; border-collapse:collapse; margin-top:20px;font-size:13px;">
            <tr>
                <td style="padding-right:60px;">Transaction ID</td>
                <td>&nbsp;<label id="lblTransactionID" /></td>
            </tr>
            <tr>
                <td>User Name</td>
                <td>&nbsp;<label id="lblUSerName" /></td>
            </tr>
            <tr>
                <td>Email</td>
                <td>&nbsp;<label id="lblEmail" /></td>
            </tr>
            <tr>
                <td>Item Name</td>
                <td>&nbsp;<label id="lblItemName" /></td>
            </tr>
            <tr>
                <td>Amount</td>
                <td>&nbsp;<label id="lblAmount" /></td>
            </tr>
            <tr>
                <td>Currency</td>
                <td>&nbsp;<label id="lblCurrency" /></td>
            </tr>
            
        </table>
    </div>
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="SubContent" runat="server">
</asp:Content>
