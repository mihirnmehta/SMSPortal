<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<dynamic>" %>


<asp:PlaceHolder ID="javaScripts" runat="server">
    <script type="text/javascript">

        $(document).ready(function () {

            var iManagementUserID = '<%= ViewData["ManagementUserID"] %>';
            
            if (iManagementUserID != 0) {

                $('#hdnManagementUsersID').val(iManagementUserID);
                $('#btnAddUser').hide();
                $('#btnUpdateUser').show();

                document.getElementById("txtForename").disabled = true;
                document.getElementById("txtSurname").disabled = true;

                LoadManagementUsersDetails(iManagementUserID);
            }
            else {

                $('#btnAddUser').show();
                document.getElementById("txtForename").disabled = false;
                document.getElementById("txtSurname").disabled = false;
                $('#btnUpdateUser').hide();
            }


            $("#ddlAccessLevel option").remove();
            $('#ddlAccessLevel').append($("<option></option>").attr("value", "loading").text("Loading.."));

            $.ajax({
                url: '<%= ResolveUrl("~/ManagementUser/GetAdminPortalAccessLevels") %>',
                cache: false,
                timeout: 6000000,
                success: function (result) {
                    if (result.length > 0) {
                        $("#ddlAccessLevel option[value='loading']").remove();
                        $('#ddlAccessLevel').append($("<option></option>").text("Select Access Level"));
                        for (var i = 0; i < result.length; i++) {
                            $('#ddlAccessLevel').append($("<option></option>").attr("value", result[i].Value).attr("AccessLevel", result[i].Attribute).text(result[i].Text));

                        }
                    }
                    else {
                        $("#ddlAccessLevel option[value='loading']").remove();
                        $('#ddlAccessLevel').append($("<option></option>").attr("value", "").text("Select Access Level"));
                    }
                },
                error: function (httpRequest, msg) {

                }
            }); //ends ajax


        });

        function LoadManagementUsersDetails(iManagementUserID) {

            ShowLoadingBox();
            $.ajax({

                url: '<%= ResolveUrl("~/ManagementUser/GetUserByID") %>',
                data: { "iManagementUserID": iManagementUserID },
                type: "POST",
                cache: false,
                timeout: 6000000,
                async: true,
                success: function (result) {
                    HideLoadingBox();

                    result = eval(result);

                    $('#txtForename').val(result.Forename);
                    $('#txtSurname').val(result.Surname);
                    $('#txtContactEmail').val(result.Email);

                    //Set email to compare logged user with edited record to update session
                    $('#hdnContactEmail').val(result.Email);

                    $('#txtPhoneno').val(result.PhoneNumber);

                    var ddlAccessLevel = document.getElementById("ddlAccessLevel");

                    setTimeout(function () { set_matching_word(ddlAccessLevel, result.AccessLevelID); }, 500);

                },  // ends success        
                error: function (httpRequest, msg) {
                    HideLoadingBox();
                    $('#errorMsg').show().text('Error');
                }

            });//ends ajax   

        }

        function EmailIDExist(strContactEmailAddress) {
            var bExist = new Boolean();

            ShowLoadingBox();
            $.ajax({
                url: '<%= ResolveUrl("~/ManagementUser/IsEmailIDUnique") %>',
                data: { "strContactEmailAddress": strContactEmailAddress },
                type: "POST",
                cache: false,
                timeout: 6000000,
                async: false,
                success: function (result) {
                    HideLoadingBox();
                    bExist = !result;
                }
            });

            return bExist;
        }

        function EmailIDExistOnUpdate(iMgmtUserID, strContactEmailAddress) {
            var bExist = new Boolean();

            ShowLoadingBox();
            $.ajax({
                url: '<%= ResolveUrl("~/ManagementUser/IsEmailIDUniqueOnUpdate") %>',
                data: { "iManagementUserID": iMgmtUserID, "strContactEmailAddress": strContactEmailAddress },
                type: "POST",
                cache: false,
                timeout: 6000000,
                async: false,
                success: function (result) {
                    HideLoadingBox();
                    bExist = !result;
                }
            });

            return bExist;
        }

        function Validate() {


            var strForename = $.trim(document.getElementById("txtForename").value);
            var strSurname = $.trim(document.getElementById("txtSurname").value);
            var strContactEmailAddress = $.trim(document.getElementById("txtContactEmail").value);


            var e = document.getElementById("ddlAccessLevel");
            var iAccessLevelID = e.options[e.selectedIndex].index;

            var strContactPhonenumber = $.trim(document.getElementById("txtPhoneno").value);

            if (strForename.length == 0) {
                alert('Please enter the Forename.');
                document.getElementById("txtForename").focus();
                return false;
            }

            if (strSurname.length == 0) {
                alert('Please enter the Surname.');
                document.getElementById("txtSurname").focus();
                return false;
            }

            if (strContactEmailAddress.length == 0) {
                alert('Please enter the Email Address.');
                document.getElementById("txtContactEmail").focus();
                return false;
            }

            if (iAccessLevelID == 0) {
                alert('Please select the Access Level.');
                document.getElementById("ddlAccessLevelID").focus();
                return false;
            }

            if (!isEmailAddress(strContactEmailAddress)) {
                alert('Invalid EmailAddress. Please enter valid Email Address');
                document.getElementById("txtContactEmail").focus();
                return false;
            }

            if (strContactPhonenumber.length > 0 && !validatePhoneNumber(strContactPhonenumber)) {
                alert('The Phone number must be a 9/10 digit numeric value with an optional \' + \' sign prefixed.');
                document.getElementById("txtPhoneno").focus();
                return false;
            }
            return true;
        }

        function AddManagementUser() {

            var strForename = $.trim(document.getElementById("txtForename").value);
            var strSurname = $.trim(document.getElementById("txtSurname").value);
            var strContactEmailAddress = $.trim(document.getElementById("txtContactEmail").value);

            var iAccessLevelID = $("#ddlAccessLevel").val();

            var strContactPhonenumber = $.trim(document.getElementById("txtPhoneno").value);

            if (Validate()) {

                var bExist = EmailIDExist(strContactEmailAddress);
                if (bExist) {
                    alert('Email already exists. Please enter another Email.');
                    return false;
                }

                ShowLoadingBox();
                $.ajax({
                    url: '<%= ResolveUrl("~/ManagementUser/AddManagementUser") %>',
                    data: {
                        "strForename": strForename,
                        "strSurname": strSurname,
                        "strContactEmailAddress": strContactEmailAddress,
                        "iAccessLevelID": iAccessLevelID,
                        "strContactPhonenumber": strContactPhonenumber
                    },
                    type: "POST",
                    cache: false,
                    timeout: 6000000,
                    async: true,
                    success: function (result) {
                        HideLoadingBox();
                        if (result == "true") {
                            alert("The User has been added.");
                            $('#dlgManageUser').dialog('close');
                            $("#gridManagementUsers").trigger("reloadGrid");
                        }
                        else {
                            $('#errorMsg').show().text('Error occurred while adding User. Please try again.');
                        }
                    },  // ends success        
                    error: function (httpRequest, msg) {
                        HideLoadingBox();
                        $('#errorMsg').show().text('Error occurred while adding User. Please try again.');
                    }

                });//ends ajax   

            }
        }


        function set_matching_word(selectObj, txt) {

            for (var i = 0; i < selectObj.length; i++) {
                if (selectObj.options[i].value == txt)
                    selectObj.selectedIndex = i;
            }
        }

        function UpdateManagementUser() {

            var iMgmtUserID            = $.trim(document.getElementById("hdnManagementUsersID").value);
            var strForename            = $.trim(document.getElementById("txtForename").value);
            var strSurname             = $.trim(document.getElementById("txtSurname").value);
            var strContactEmailAddress = $.trim(document.getElementById("txtContactEmail").value);
            var strContactPhonenumber  = $.trim(document.getElementById("txtPhoneno").value);
            var e = document.getElementById("ddlAccessLevel");
            var iAccessLevelID = e.options[e.selectedIndex].value;


            if (Validate()) {
                var bExist = EmailIDExistOnUpdate(iMgmtUserID, strContactEmailAddress);
                if (bExist) {
                    alert('Email already exists. Please enter another Email.');
                    return false;
                }

                var sLoggedInUserEmail = $('#hdnContactEmail').val();

                ShowLoadingBox();
                $.ajax({
                    url: '<%= ResolveUrl("~/ManagementUser/UpdateManagementUser") %>',
                    data: {
                        "iMgmtUserID": iMgmtUserID,
                        "strContactEmailAddress": strContactEmailAddress,
                        "iAccessLevelID": iAccessLevelID,
                        "strContactPhonenumber": strContactPhonenumber,
                        "sLoggedInUserEmail": sLoggedInUserEmail
                    },
                    type: "POST",
                    cache: false,
                    timeout: 6000000,
                    async: true,
                    success: function (result) {
                        HideLoadingBox();
                        if (result == "true") {                            
                            alert("The User details have been Updated.");
                            $('#dlgManageUser').dialog('close');
                            $("#gridManagementUsers").trigger("reloadGrid");
                            location.reload();
                        }
                        else {
                            
                            $('#errorMsg').show().text('Error occurred while Updating User. Please try again.');
                        }
                    },  // ends success        
                    error: function (httpRequest, msg) {
                        HideLoadingBox();
                        $('#errorMsg').show().text('Error occurred while Updating User. Please try again.');
                    }

                });//ends ajax    
            }

        }
    </script>
</asp:PlaceHolder>

<div id="dlgAddEditPL">

    <input type="hidden" id="hdnManagementUsersID" />

    <table id="user_form">
        <tr>
            <td colspan="2" style="color: Red; display: none;" id="errorMsg"></td>
        </tr>
        <tr>
            <td class="required">Forename
            </td>
            <td class="required">Surname
            </td>

        </tr>
        <tr>
            <td>
                <input id="txtForename" type="text" maxlength="20" />

            </td>
            <td>
                <input id="txtSurname" type="text" maxlength="20" />

            </td>
        </tr>
        <tr>
            <td class="required">Contact Email Address
            </td>
            <td class="required">Access Level
            </td>

        </tr>
        <tr>
            <td>
                <input id="txtContactEmail" type="text" maxlength="50" />

            </td>

            <td colspan="2" style="text-align: center">
                <select id="ddlAccessLevel" name="AccessLevel" /></td>

        </tr>
        <tr>
            <td>Contact Phone Number
            </td>
        </tr>
        <tr>
            <td>
                <input id="txtPhoneno" type="text" maxlength="15" />
            </td>
        </tr>
        <tr>
            <td></td>
        </tr>
        <tr>
            <td style="text-align: right" colspan="2">
                <input id="btnAddUser" type="button" value="Add" style="width: 100px" onclick="AddManagementUser(); " class="buttons" />
                <input id="btnUpdateUser" type="button" value="Update" style="width: 100px" onclick="UpdateManagementUser()" class="buttons" />
                <input id="btnCancelUsers" type="button" value="Cancel" style="width: 100px" onclick="javascript: $('#dlgManageUser').dialog('close')" />
            </td>
        </tr>
    </table>
    <input type="hidden" id="hdnContactEmail" />
</div>

<div class="modal">
    <div id="LoadingText">Loading</div>
</div>
