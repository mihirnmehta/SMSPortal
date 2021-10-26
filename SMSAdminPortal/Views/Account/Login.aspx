<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Login.Master" Inherits="System.Web.Mvc.ViewPage<dynamic>" %>
<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
    Login
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

    <asp:PlaceHolder ID="phScripts" runat="server">

        <script type="text/javascript">
            $(document).ready(function () {
                document.getElementById("txtUserName").focus();

                $('#hdnReturnURL').val('<%= ViewData["ReturnURL"]%>');

                var msg = '<%= ViewData["ErrorMessage"] %>';
                if (msg != null)
                    $('#errorMsg').show().text(msg);

            });

            function Validate() {
                var strUserName = $.trim(document.getElementById("txtUserName").value);
                var strPassword = $.trim(document.getElementById("txtPassword").value);
                
                if (strUserName.length == 0) {
                    alert('Please enter the EmailID.');
                    document.getElementById("txtUserName").focus();
                    return false;
                }

                if (strPassword.length == 0) {
                    alert('Please enter the Password.');
                    document.getElementById("txtPassword").focus();
                    return false;
                }
                
                if (!isEmailAddress(strUserName)) {
                    alert('Please enter a valid EmailID.');
                    document.getElementById("txtUserName").focus();
                    return false;
                }
                return true;
            }

            function ShowForgotPasswordDialog() {
                $('#txtEmailId').val('');
                $("#dlgForgotPassword").dialog({ modal: true, title: "Forgot Password", resizable: false, height: 150, width: 350 });
                $("#dlgForgotPassword").dialog("open");  //open it!
            }

            function VerifyEmailAndResetPassword() {
                var strEmail = $.trim(document.getElementById("txtEmailId").value);

                if (strEmail.length == 0) {
                    alert('Please enter the EmailID.');
                    document.getElementById("txtUserName").focus();
                    return false;
                }

                if (!isEmailAddress(strEmail)) {
                    alert('Invalid EmailID. Please enter a valid EmailID.');
                    document.getElementById("txtUserName").focus();
                    return false;
                }
                ShowLoadingBox();
                $.ajax({
                    url: '<%= ResolveUrl("~/Account/CheckEmailExists") %>',
                    data: { "strEmail": strEmail },
                    type: "POST",
                    cache: false,
                    success: function (result) {
                        HideLoadingBox();
                        if (result) {
                            ResetPassword(strEmail);
                        }
                        else {
                            alert('EmailID does not exist.');
                        }
                    },
                    error: function (xhr, ajaxOptions, thrownError) {
                        HideLoadingBox();
                        $('#errorMsg').show().text("An error occured while resetting password.");
                        //alert(xhr.responseText);
                    }
                });
            }

            function ResetPassword(strEmail) {
                ShowLoadingBox();
                $.ajax({
                    url: '<%= ResolveUrl("~/Account/ResetPassword") %>',
                    data: { "strEmail": strEmail },
                    type: "POST",
                    datatype: 'json',
                    cache: false,
                    success: function (result) {
                        HideLoadingBox();
                        if (result) {
                            alert("Your password has been reset.");
                            $('#dlgForgotPassword').dialog('close')
                        }
                        else {
                            alert('An error occured while resetting password.');
                        }
                    },
                    error: function (xhr, ajaxOptions, thrownError) {
                        HideLoadingBox();
                        $('#errorMsg').show().text("An error occured while resetting password.");
                    }
                });
            }

        </script>

    </asp:PlaceHolder>
    <br />
    
    <form action= '<%= ResolveUrl("~/Account/Login") %>' method="post">
    <div class="ax_shape" id="u12">
        <img class="img " id="u12_img" src="<%= ResolveUrl("~/Content/Images/u12.png")%>" />
        <div class="text" id="u13">
            Login
        </div>
    </div>
    <div class="ax_shape" id="u14">
        <img class="img " id="u14_img" src="<%= ResolveUrl("~/Content/Images/u14.png")%>" />
        <div class="text" id="u15"></div>
    </div>
   
     <div class="loginErrMsg" id="errorMsg">
           Error msg header
    </div>

    <div class="ax_h1" id="u16">
        <div class="text" id="u17">
           UserName
        </div>
    </div>

    <div class="ax_text_field" id="u22">
        <div id="u22_input">
            <input type="text" id="txtUserName" style="width: 348px; height: 26px;" name="UserName"/>
        </div>
    </div>

    <div class="ax_h1" id="u18">
        <div class="text" id="u19">
            Password
        </div>
    </div>
    <div class="ax_text_field" id="u23">
        <div id="u23_input">
            <input type="password" id="txtPassword" style="width: 348px; height: 26px;" name="Password"/>
        </div>
    </div>
    <div class="ax_shape" id="u20" style="cursor: pointer;">
            <input type="submit" id="btnLogin" value="Login" onclick="return Validate()";/>
    </div>

    
    <div id="forgotPswdDiv">
        <a href="#" onclick="ShowForgotPasswordDialog()">Forgot Password</a>
    </div>

     
    <div id='dlgForgotPassword' style="display: none;">
        <table id="user_form">
            <tr>
                <td nowrap>Email ID
                </td>
                <td>
                    <input type="text" id="txtEmailId" style="width: 250px;" />
                </td>
            </tr>
            <tr>
                <td colspan="2" align="right">
                    <input type="button" value="OK" onclick="VerifyEmailAndResetPassword()" />&nbsp;&nbsp;
                    <input type="button" value="Cancel"  onclick="javascript: $('#dlgForgotPassword').dialog('close')"/></td>
            </tr>
        </table>
    </div>

    <input type="hidden" name="returnURL" id="hdnReturnURL" />
    </form>


</asp:Content>
