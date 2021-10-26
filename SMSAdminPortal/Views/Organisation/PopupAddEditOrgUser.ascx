<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<dynamic>" %>
<asp:PlaceHolder ID="javaScripts" runat="server">
    <script type="text/javascript">

        $(document).ready(function () {

            var iOrganisationUserID = '<%= ViewData["OrganisationUserID"] %>';
            $('#hdnOrganisationID').val(iOrganisationUserID);

            if (iOrganisationUserID != 0) {

                $('#btnAddOrgUsers').hide();
                $('#btnUpdateOrgUsers').show();

                document.getElementById("txtForename").disabled = true;
                document.getElementById("txtSurname").disabled = true;

                LoadOrganisationUserDetails(iOrganisationUserID);
            }
            else {

                $('#btnAddOrgUsers').show();
                $('#btnUpdateOrgUsers').hide();

                document.getElementById("txtForename").disabled = false;
                document.getElementById("txtSurname").disabled = false;                

            }

            $("#ddlAccessLevel option").remove();
            $('#ddlAccessLevel').append($("<option></option>").attr("value", "loading").text("Loading.."));

            
            $.ajax({
                url: '<%= ResolveUrl("~/Organisation/GetUserPortalAccessLevels") %>',
                cache: false,
                timeout: 6000000,
                success: function (result)
                {                
                 if (result.length > 0) {
                    $("#ddlAccessLevel option[value='loading']").remove();
                    $('#ddlAccessLevel').append($("<option></option>").text("Select Access Level"));
                    for (var i = 0; i < result.length; i++) {
                        $('#ddlAccessLevel').append($("<option></option>").attr("value", result[i].Value).text(result[i].Text));

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

    function LoadOrganisationUserDetails(iOrganisationUserID) {
        ShowLoadingBox();
        $.ajax({
            url: '<%= ResolveUrl("~/Organisation/GetOrganisationUserByID/") %>',
            data: { "iOrganisationUserID": iOrganisationUserID },
            type: "POST",
            cache: false,
            timeout: 6000000,
            async: true,
            success: function (result) {
                HideLoadingBox();

                document.getElementById("txtForename").value = result.Forename;
                document.getElementById("txtSurname").value  = result.Surname;
                document.getElementById("txtEmail").value    = result.Email;

                var ddlAccessLevel = document.getElementById("ddlAccessLevel");
                setTimeout(function () { set_matching_word(ddlAccessLevel, result.AccessLevelID); }, 500);

            },  // ends success        
            error: function (httpRequest, msg) {
                HideLoadingBox();
                $('#errorMsg').show().text('Error');
            }

        });//ends ajax   

    }

    function set_matching_word(selectObj, txt) {
        for (var i = 0; i < selectObj.length; i++) {
            if (selectObj.options[i].value == txt)
                selectObj.selectedIndex = i;
        }
    }

    function EmailIDExist(strEmail) {
        var bExist = new Boolean();

        ShowLoadingBox();
        $.ajax({
            url: '<%= ResolveUrl("~/Organisation/IsEmailIDUnique") %>',
            data: { "strEmail": strEmail },
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

    function EmailIDExistOnUpdate(iOrganisationUserID, strEmail) {
        var bExist = new Boolean();

        ShowLoadingBox();
        $.ajax({
            url: '<%= ResolveUrl("~/Organisation/IsEmailIDUniqueOnUpdate") %>',
                data: { "iOrganisationUserID": iOrganisationUserID, "strEmail": strEmail },
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
            var strSurname  = $.trim(document.getElementById("txtSurname").value);
            var strEmail    = $.trim(document.getElementById("txtEmail").value);

            var e = document.getElementById("ddlAccessLevel");
            var iAccessLevelID = e.options[e.selectedIndex].index;

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

            if (strEmail.length == 0) {
                alert('Please enter Email Address');
                document.getElementById("txtEmail").focus();
                return false;
            }

            if (iAccessLevelID == 0) {
                alert('Please select the Access Level.');
                document.getElementById("ddlAccessLevel").focus();
                return false;
            }

            if (!isEmailAddress(strEmail)) {
                alert('Invalid EmailID. Please enter a valid EmailID.');
                document.getElementById("txtEmail").focus();
                return false;
            }
            return true;
        }

        function AddOrganisationUser() {

            var strForename = $.trim(document.getElementById("txtForename").value);
            var strSurname  = $.trim(document.getElementById("txtSurname").value);
            var strEmail    = $.trim(document.getElementById("txtEmail").value);

            var e = document.getElementById("ddlAccessLevel");
            var iAccessLevelID = e.options[e.selectedIndex].value;

            if (Validate()) {
                var bExist = EmailIDExist(strEmail);
                if (bExist) {
                    alert('EmailID already exists');
                    return false;
                }
                ShowLoadingBox();
                $.ajax({
                    url: '<%= ResolveUrl("~/Organisation/AddOrganisationUser") %>',
                data: { "strForename": strForename, "strSurname": strSurname, "strEmail": strEmail, "iAccessLevelID": iAccessLevelID },
                type: "POST",
                cache: false,
                timeout: 6000000,
                async: true,
                success: function (result) {
                    HideLoadingBox();
                    if (result == "true") {
                        alert("The User has been added.");
                        $('#dlgOrganisationUser').dialog('close');
                        $("#gridOrganisationUsers").trigger("reloadGrid");
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

        function UpdateOrganisationUser() {
            var iOrganisationUserID = document.getElementById("hdnOrganisationID").value;
            var strEmail            = $.trim(document.getElementById("txtEmail").value);

            var iAccessLevelID = $("#ddlAccessLevel").val();

            if (Validate()) {

                var bExist = EmailIDExistOnUpdate(iOrganisationUserID, strEmail);

                if (bExist) {
                    alert('EmailID already exists');
                    return false;
                }

                ShowLoadingBox();
                $.ajax({
                    url: '<%= ResolveUrl("~/Organisation/UpdateOrganisationUser") %>',
                data: { "iOrganisationUserID": iOrganisationUserID, "strEmail": strEmail, "iAccessLevelID": iAccessLevelID },
                type: "POST",
                cache: false,
                timeout: 6000000,
                async: true,
                success: function (result) {
                    HideLoadingBox();
                    if (result == "true") {
                        
                        alert("The User details have been Updated.");
                        $('#dlgOrganisationUser').dialog('close');
                        $("#gridOrganisationUsers").trigger("reloadGrid");
                    }
                    else {
                        HideLoadingBox();
                        $('#errorMsg').show().text('Error occurred while updating User. Please try again.');
                    }
                },  // ends success        
                error: function (httpRequest, msg) {
                    
                    $('#errorMsg').show().text('Error occurred while updating User. Please try again.');
                }
            });//ends ajax        
        }
    }
    </script>
</asp:PlaceHolder>

<div id="dlgAddEditPL">

    <input type="hidden" id="hdnOrganisationID" />

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
                <input id="txtForename" type="text" maxlength="20"/>

            </td>
            <td>
                <input id="txtSurname" type="text" maxlength="20" />

            </td>
        </tr>
        <tr>
            <td class="required">Email Address
            </td>
            <td class="required">Access Level
            </td>

        </tr>
        <tr>
            <td>
                <input id="txtEmail" type="text"  maxlength="50"/>

            </td>

            <td colspan="2">
                <select id="ddlAccessLevel" name="AccessLevel" /></td>

        </tr>

        <tr>
            <td></td>
        </tr>
        <tr>
            <td colspan="3" style="text-align: right">
                <input id="btnAddOrgUsers" type="button" value="Add" style="width: 100px" onclick="AddOrganisationUser()" class="buttons" />
                <input id="btnUpdateOrgUsers" type="button" value="Update" style="width: 100px" onclick="UpdateOrganisationUser()" class="buttons" />
                <input id="btnCancelOrgUsers" type="button" value="Cancel" style="width: 100px" onclick="javascript: $('#dlgOrganisationUser').dialog('close')" />
            </td>
        </tr>
    </table>

</div>
