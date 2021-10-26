<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/MainMaster.Master" Inherits="System.Web.Mvc.ViewPage<dynamic>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
    Unauthorized Access
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">


    <div style="font-family: Segoe UI Regular,Segoe UI; font-size: 20px; font-weight: 400; margin-left: 400px; margin-top: 200px; width: 800px;">
        Authorization Failed!!!
        <br />
        You do not have access to this resource.
    </div>
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="SubContent" runat="server">
</asp:Content>
