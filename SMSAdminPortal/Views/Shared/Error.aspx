<%@ Page Title="" Language="C#" MasterPageFile="Login.Master" Inherits="System.Web.Mvc.ViewPage<dynamic>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
    Error
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    
    
        <div class="text" id="u13" style="font-family:Segoe UI Regular,Segoe UI; font-size: 20px; font-weight: 400; top:300px; left:300px; width:600px;">
            Sorry, an error occurred while processing your request.
            <br /><br />
           <center><a href="/Account/Login">Login</a></center> 
        </div>
    
        
</asp:Content>
