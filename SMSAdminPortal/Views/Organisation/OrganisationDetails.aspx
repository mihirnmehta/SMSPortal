<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Organisation/OrganisationDetails.master" Inherits="System.Web.Mvc.ViewPage<dynamic>" %>

<asp:Content ID="OrgDetailTitle" ContentPlaceHolderID="OrgTitleContent" runat="server">
    Organisation Details
</asp:Content>

<asp:Content ID="OrgDetailMainContent" ContentPlaceHolderID="OrgMainContent" runat="server">

    <script type="text/javascript">

        var bHasPostPayAccess = HasAddPostPayAccess();
        var bHasAddTopupAccess = HasAddTopupAccess();
        var bHasReadOnlyAccess = HasReadOnlyAccess();

        $(document).ready(function () {

            var iOrganisationID = '<%= ViewData["OrganisationID"] %>'
            $('#hdnOrganisationID').val(iOrganisationID);

            if (iOrganisationID != 0) {

                $('#btnEnableControls').show();
                $('#btnUpdateOrg').hide();
                $('#btnCancel').hide();

                LoadOrganisationDetails();
            }
            if (bHasReadOnlyAccess)
                $("#btnEnableControls").attr("disabled", "disabled");

            DisableControls();

            $('#chkPayPal').click(function () {
                if ($(this).is(':checked') == false) {

                    var bExist = PayPalMPActsExist();
                    if (bExist) {
                        alert('Micropayment accounts with PayPal payment option exist.');
                        return false;
                    }
                    return true;
                }
            });

            $('#chkInvoice').click(function () {
                if ($(this).is(':checked') == false) {

                    var bExist = InvoiceMPActsExist();
                    if (bExist) {
                        alert('Micropayment accounts with Invoice payment option exist.');
                        return false;
                    }
                    return true;
                }
            });


        });

        function DisableControls() {
            $("#txtName").attr("disabled", "disabled");
            $("#txtEmail").attr("disabled", "disabled");
            $("#txtPhone").attr("disabled", "disabled");

            $("#chkPayPal").attr("disabled", "disabled");
            $("#chkInvoice").attr("disabled", "disabled");
        }

        function EnableControls() {
            $("#txtName").attr("disabled", false);
            $("#txtEmail").attr("disabled", false);
            $("#txtPhone").attr("disabled", false);

            if (!bHasReadOnlyAccess)
                $("#chkPayPal").attr("disabled", false);
            if (!bHasReadOnlyAccess && !bHasAddTopupAccess)
                $("#chkInvoice").attr("disabled", false);

            $('#btnEnableControls').hide();

            if (!bHasReadOnlyAccess) {
                $('#btnUpdateOrg').show();
                $('#btnCancel').show();
            }
        }

        function LoadOrganisationDetails() {
            ShowLoadingBox();
            $.ajax({
                url: '<%= ResolveUrl("~/Organisation/GetOrgDetailsByID") %>',
                type: "GET",
                cache: false,
                timeout: 6000000,
                async: true,
                success: function (result) {
                    result = eval(result);
                    HideLoadingBox();

                    $('#txtName').val(result.ContactName);
                    $('#txtEmail').val(result.ContactEmail);
                    $('#txtPhone').val(result.ContactPhone);

                    $('#hdnName').val(result.ContactName);
                    $('#hdnEmail').val(result.ContactEmail);
                    $('#hdnPhNo').val(result.ContactPhone);
                    
                    if (result.PayPal) {
                        $('#chkPayPal').attr("checked", "checked");
                        $('#hdnPaypal').val("true");
                    }
                    else {
                        $('#hdnPaypal').val("false");
                    }

                    if (result.Invoice) {
                        $('#chkInvoice').attr("checked", "checked");
                        $('#hdnInvoice').val("true");
                    }
                    else {
                        $('#hdnInvoice').val("false");
                    }

                },  // ends success        
                error: function (httpRequest, msg) {
                    HideLoadingBox();
                    $('#errorMsg').show().text('Error loading Organisation Details. Please try again.');
                }

            });//ends ajax   
        }


        function UpdateOrgDetails() {

            var bPayPal = $("#chkPayPal").is(':checked') ? true : false;
            var bInvoice = $("#chkInvoice").is(':checked') ? true : false;

            var strName        = $.trim(document.getElementById("txtName").value);
            var strEmail       = $.trim(document.getElementById("txtEmail").value);
            var strPhoneNumber = $.trim(document.getElementById("txtPhone").value);

            if (strName.length == 0) {
                alert('Please enter the Contact Name.');
                document.getElementById("txtName").focus();
                return false;
            }

            if (strEmail.length == 0) {
                alert('Please enter the Contact Email.');
                document.getElementById("txtEmail").focus();
                return false;
            }

            if (!isEmailAddress(strEmail)) {
                alert('Invalid EmailID. Please enter a valid EmailID.');
                document.getElementById("txtEmail").focus();
                return false;
            }

            if (strPhoneNumber.length > 0 && !validatePhoneNumber(strPhoneNumber)) {
                alert('Invalid Phone Number. Please enter valid Phone Number.');
                document.getElementById("txtPhone").focus();
                return false;
            }

            ShowLoadingBox();
            $.ajax({
                url: '<%= ResolveUrl("~/Organisation/UpdateOrganisation") %>',
                data: {
                    "strContactName": strName,
                    "strContactEmail": strEmail,
                    "strContactPhone": strPhoneNumber,
                    "bPayPal": bPayPal,
                    "bInvoice": bInvoice
                },
                type: "POST",
                cache: false,
                timeout: 6000000,
                async: true,
                success: function (result) {
                    HideLoadingBox();
                    if (result == "true") {
                        alert("The Organisation details have been updated.");

                        //Reloading the value in the hidden controls
                        $('#hdnName').val(strName);
                        $('#hdnEmail').val(strEmail);
                        $('#hdnPhNo').val(strPhoneNumber);

                        if (bPayPal) {
                            $('#hdnPaypal').val("true");
                        }
                        else {
                            $('#hdnPaypal').val("false");
                        }

                        if (bInvoice) {
                            $('#hdnInvoice').val("true");
                        }
                        else {
                            $('#hdnInvoice').val("false");
                        }

                        //Displaying the Edit button by hidding Save and Cancel buttons
                        $('#btnEnableControls').show();
                        $('#btnUpdateOrg').hide();
                        $('#btnCancel').hide();

                        DisableControls();
                    }
                    else {
                        $('#errorMsg').show().text('An error occurred while updating Organisation Details. Please try again.');
                    }
                },  // ends success        
                error: function (httpRequest, msg) {
                    HideLoadingBox();
                    $('#errorMsg').show().text('An error occurred while updating Organisation Details. Please try again.');
                }
            });//ends ajax     
        }

        function PayPalMPActsExist() {
            var bExist = new Boolean();
            $.ajax({
                url: '<%= ResolveUrl("~/Organisation/PayPalMPActExist") %>',
                type: "POST",
                cache: false,
                timeout: 6000000,
                async: false,
                success: function (result) {
                    bExist = result;
                }
            });
            return bExist;
        }

        function InvoiceMPActsExist() {
            var bExist = new Boolean();
            $.ajax({
                url: '<%= ResolveUrl("~/Organisation/InvoiceMPActExist") %>',
                type: "POST",
                cache: false,
                timeout: 6000000,
                async: false,
                success: function (result) {
                    bExist = result;
                }
            });
            return bExist;
        }

        function CancelEdit()
        {
            //Initializing the controls with the previous value stored in hidden controls
            $('#txtName').val($('#hdnName').val());
            $('#txtEmail').val($('#hdnEmail').val());
            $('#txtPhone').val($('#hdnPhNo').val());

            if ($('#hdnPaypal').val() == "true") {
                document.getElementById("chkPayPal").checked = true;
            }
            else {
                $('#chkPayPal').removeAttr("checked");
            }

            if ($('#hdnInvoice').val() == "true") {
                document.getElementById("chkInvoice").checked = true;
            }
            else {
                $('#chkInvoice').removeAttr("checked");
            }

            //Show Edit by hidding Save & Cancel
            $('#btnEnableControls').show();
            $('#btnUpdateOrg').hide();
            $('#btnCancel').hide();
            DisableControls();

        }
    </script>


    <div id="EditOrg">

        <input type="hidden" id="hdnOrganisationID" />

        <table id="user_form">
            <tr>
                <td colspan="2" style="color: Red; display: none;" id="errorMsg"></td>
            </tr>
            <tr>
                <td colspan="2">
                    <label id="lblAddress"></label>
                </td>
            </tr>
            <tr>
                <td>

                    <table>
                        <tr>
                            <td class="required">Contact Name</td>
                            <td>
                                <input id="txtName" type="text" maxlength="255" />
                            </td>
                        </tr>
                        <tr>
                            <td class="required">Contact Email Address</td>
                            <td>
                                <input id="txtEmail" type="text" maxlength="255" />
                            </td>
                        </tr>
                        <tr>
                            <td>Contact Phone Number</td>
                            <td>
                                <input id="txtPhone" type="text" maxlength="15" />
                            </td>
                        </tr>
                        <tr>
                            <td>Payment Types</td>
                            <td><input type="checkbox" id="chkPayPal" name="paymentMethod" />Paypal
                                <input type="checkbox" id="chkInvoice" name="paymentMethod" />Invoice
                            </td>
                        </tr>

                    </table>
                </td>
                <td>
                    
                </td>
            </tr>
            <tr>
                <td colspan="2" style="text-align: right">
                    <input type="button" id="btnEnableControls" value="Edit" onclick="EnableControls()" class="button" />&nbsp;
                    <input type="button" id="btnUpdateOrg" value="Save" onclick="UpdateOrgDetails()" class="button" />&nbsp;
                    <input type="button" id="btnCancel" value="Cancel" onclick="CancelEdit()" class="button" />&nbsp;
                </td>
            </tr>
            <tr>
                <td colspan="2"></td>
            </tr>

            <tr>
                <td colspan="2">
                    <table id="gridMPAccounts" class="scroll"></table>
                    <div id="pager" class="scroll" style="text-align: center;"></div>
                </td>
            </tr>
        </table>

        <input type="hidden" id="hdnName" />
        <input type="hidden" id="hdnEmail" />
        <input type="hidden" id="hdnPhNo" />
        <input type="hidden" id="hdnPaypal" />
        <input type="hidden" id="hdnInvoice" />
    </div>

    <div class="modal">
        <div id="LoadingText">Loading</div>
    </div>
</asp:Content>
