<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/MainMaster.Master" Inherits="System.Web.Mvc.ViewPage<dynamic>" %>


<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
    Change Password
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <asp:PlaceHolder ID="phScripts" runat="server">

        <script type="text/javascript">

            $(document).ready(function () {
                var errMsg = '<%=ViewData["ErrorMessage"]%>';
                if(errMsg.length > 0)
                    $('#errorMsg').show().text(errMsg);
                                
                var successMsg = '<%=ViewData["SuccessMessage"]%>';
                if (successMsg.length > 0) {
                    alert(successMsg);
                    Reset();
                    window.location.href =  "<%= ResolveUrl("~/Organisation")%>";
                }

                document.getElementById("txtCurrPassword").focus();
            });

            function Validate() {
                var strCurrPassword    = $.trim(document.getElementById("txtCurrPassword").value);
                var strNewPassword     = $.trim(document.getElementById("txtNewPassword").value);
                var strConfirmPassword = $.trim(document.getElementById("txtConfirmPassword").value);

                if (strCurrPassword.length == 0) {
                    alert('Please enter the Current Password.');
                    document.getElementById("txtCurrPassword").focus();
                    return false;
                }

                if (strNewPassword.length == 0) {
                    alert('Please enter the New Password.');
                    document.getElementById("txtNewPassword").focus();
                    return false;
                }

                var minPswdLength = '<%= SMSAdminPortal.Commons.PortalConstants.MINPASSWORDLENGTH %>';

                if (strNewPassword.length < minPswdLength) {
                    alert('The password should be atleast ' + minPswdLength + ' characters long.');
                    document.getElementById("txtNewPassword").focus();
                    return false;
                }

                if (!checkPasswordComplexity(strNewPassword)) {
                    alert('Password must be alphanumeric with aleast one special character.')
                    return false;
                }

                if (strConfirmPassword.length == 0) {
                    alert('Please enter the Confirm Password.');
                    document.getElementById("txtConfirmPassword").focus();
                    return false;
                }

                if (strNewPassword != strConfirmPassword) {
                    alert("New Password & Confirm Password must match.");
                    document.getElementById("txtNewPassword").focus();
                    return false;
                }

                return true;
            }

            function Reset() {
                document.getElementById("txtCurrPassword").value = "";
                document.getElementById("txtNewPassword").value = "";
                document.getElementById("txtConfirmPassword").value = "";
            }

        </script>

    </asp:PlaceHolder>

    <form action= "<%= ResolveUrl("~/Account/ChangePassword")%>" method="post" autocomplete="off">
        <div>
            <fieldset>
                <legend>
                    <font style="font-family: Calibri; font-size: large">Change Password</font>
                </legend>
                <table id="user_form" style="margin-right: 5%">
                    <tr>
                        <td colspan="2" style="color: Red; display: none;" id="errorMsg"></td>
                        <td colspan="2" style="display: none;" id="successMsg"></td>
                    </tr>
                    <tr>
                        <td class="required">Current Password</td>

                        <td>
                            <input id="txtCurrPassword" type="password" maxlength="40" name="CurrentPassword" />

                        </td>
                    </tr>

                    <tr>
                        <td class="required">New Password</td>

                        <td>
                            <input id="txtNewPassword" type="password" maxlength="40" name="NewPassword" />
                           <br /> <label for="NewPassword"><font style="font-size: smaller; color: gray; vertical-align: top;">special characters from (!,%,@,#,$,*,_)</font></label>
                        </td>
                    </tr>

                    <tr>
                        <td class="required">Confirm Password</td>

                        <td>
                            <input id="txtConfirmPassword" type="password" maxlength="40" />
                        </td>
                    </tr>

                    <tr>
                        <td style="text-align: right" colspan="2">
                            <input type="submit" id="btnSave" value="Save" class="button" onclick="return Validate();" />

                            <input type="button" id="btnReset" value="Reset" class="button" onclick="Reset()" />
                        </td>
                    </tr>
                </table>
            </fieldset>
        </div>

        <div class="modal">
            <div id="LoadingText">Loading</div>
        </div>
    </form>
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="SubContent" runat="server">
</asp:Content>
