<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<dynamic>" %>

<asp:PlaceHolder ID="javaScripts" runat="server">
    <script type="text/javascript">

        var bHasAddPostPayAccess = HasAddPostPayAccess();
        var bHasAddTopupAccess = HasAddTopupAccess();

        $(document).ready(function () {

            if (bHasAddPostPayAccess || bHasAddTopupAccess) {
                $("#chkBoxEnableMPAccount").attr("disabled", "disabled");
            }

            if (bHasAddTopupAccess) {
                $("#chkBoxInvoice").attr("disabled", "disabled");
            }

            $("#chkLowBal").bind("click", CheckLowBalChangeEvent);

            $("#chkUsageRestriction").bind("click", CheckUsageRestrictionChangeEvent);

            $("#txtWorkingDayStart").attr("disabled", "disabled");
            $("#txtWorkingDayFinish").attr("disabled", "disabled");

            $("#txtNonWorkingDayStart").attr("disabled", "disabled");
            $("#txtNonWorkingDayFinish").attr("disabled", "disabled");

            $("#txtLowBalWrnLvl").attr("disabled", "disabled");
            $("#txtLowBalContactEmail").attr("disabled", "disabled");

            var iAccountID = '<%= ViewData["AccountID"] %>';
            $('#hdnAccountID').val(iAccountID);

            if (iAccountID != 0) {

                $('#btnAddMPAccount').hide();
                $('#btnUpdateMPAccount').show();
                LoadMPAccountDetails(iAccountID);
            }
            else {
                $('#chkBoxEnableMPAccount').attr("checked", "checked");
                $('#trBal_EstDt').hide();
                $('#btnAddMPAccount').show();
                $('#btnUpdateMPAccount').hide();
            }

            DisablePaymentTypeBasedOnOrg();
        });

        $('#chkBoxEnableMPAccount').click(function () {
            if ($(this).is(':checked') == false) {
                return confirm("Are you sure want to disable the Micropayment Account?");
            }
        });

        function DisablePaymentTypeBasedOnOrg() {
            ShowLoadingBox();
            $.ajax({
                url: '<%= ResolveUrl("~/Organisation/GetOrgBillingMethods") %>',
                type: "POST",
                cache: false,
                timeout: 6000000,
                async: true,
                success: function (result) {
                    HideLoadingBox();
                    if (result != null) {
                        if (!result.Paypal) {
                            $("#chkBoxPaypal").attr("disabled", "disabled");
                        }

                        if (!result.Invoice) {
                            $('#chkBoxInvoice').attr("disabled", "disabled");
                        }
                    }
                    else {
                        $("#chkBoxPaypal").attr("disabled", "disabled");
                        $('#chkBoxInvoice').attr("disabled", "disabled");
                    }
                },  // ends success        
                error: function (httpRequest, msg) {
                    HideLoadingBox();
                    $('#errorMsg').show().text('Error');
                }

            });//ends ajax   
        }

        function CheckLowBalChangeEvent() {
            if ($('#chkLowBal').is(':checked')) {
                $("#txtLowBalWrnLvl").prop('disabled', false);
                $("#txtLowBalContactEmail").prop('disabled', false);
            }
            else {
                $("#txtLowBalWrnLvl").attr("disabled", "disabled");
                $("#txtLowBalContactEmail").attr("disabled", "disabled");

            }
        };

        function CheckUsageRestrictionChangeEvent() {

            if ($('#chkUsageRestriction').is(':checked')) {
                $("#txtWorkingDayStart").prop('disabled', false);
                $("#txtWorkingDayFinish").prop('disabled', false);

                $("#txtNonWorkingDayStart").prop('disabled', false);
                $("#txtNonWorkingDayFinish").prop('disabled', false);
            }
            else {
                $("#txtWorkingDayStart").attr("disabled", "disabled");
                $("#txtWorkingDayFinish").attr("disabled", "disabled");

                $("#txtNonWorkingDayStart").attr("disabled", "disabled");
                $("#txtNonWorkingDayFinish").attr("disabled", "disabled");

            }
        };

        //Checks wheater mandatory fields are not empty
        function Validate() {

            if ($.trim($("#txtName").val()).length == 0) {
                alert('Please enter name.');
                document.getelementbyid("txtname").focus();
                return false;
            }

            if ($.trim($("#txtUsername").val()).length == 0) {
                alert('Please enter User Name.');
                document.getElementById("txtUsername").focus();
                return false;
            }

            //------Validation of selecting atleast one Payment Method------

            //var fields = $("input[name='paymentMethod']").serializeArray();
            //if (fields.length == 0) {
            //    alert('Please select atleast one Payment Method');
            //    // cancel submit
            //    return false;
            //}

            var minPswdLength = '<%= SMSAdminPortal.Commons.PortalConstants.MINPASSWORDLENGTH %>';

            if ($.trim($("#txtPassword").val()).length < minPswdLength) {
                alert('The password should be atleast ' + minPswdLength + ' characters long.');
                document.getElementById("txtPassword").focus();
                return false;
            }

            if (!checkPasswordComplexity($.trim($("#txtPassword").val()))) {
                alert('Password must be alpha numeric with aleast one special character.')
                return false;
            }

            if ($.trim($("#txtPassword").val()) != $.trim($("#txtRepeatPassword").val())) {
                alert('Password & Repeat Password must match.');
                document.getElementById("txtRepeatPassword").focus();
                return false;
            }

            if ($('#chkLowBal').is(':checked')) {
                if ($.trim($("#txtLowBalWrnLvl").val()).length == 0) {
                    alert('Please enter Low balance warning level.');
                    document.getElementById("txtLowBalWrnLvl").focus();
                    return false;
                }

                if (!ValidateNumericValues($("#txtLowBalWrnLvl").val())) {
                    alert('Invalid warning level. Please enter number.');
                    document.getElementById("txtLowBalWrnLvl").focus();
                    return false;
                }

                if ($.trim($("#txtLowBalContactEmail").val()).length == 0) {
                    alert('Please enter Low Balance Contact Email.');
                    document.getElementById("txtLowBalContactEmail").focus();
                    return false;
                }
                else if (!isEmailAddress($.trim($("#txtLowBalContactEmail").val()))) {
                    alert('Please enter Low Balance Contact Email in proper format.')
                    document.getElementById("txtLowBalContactEmail").focus();
                    return false;
                }
            }

            if ($('#chkUsageRestriction').is(':checked')) {

                if ($.trim($("#txtWorkingDayStart").val()).length == 0) {
                    alert('Please enter Working Day Start Time.');
                    document.getElementById("txtWorkingDayStart").focus();
                    return false;
                }
                else if (!check24hrTime($.trim($("#txtWorkingDayStart").val()))) {
                    alert('Please enter Working Day Start Time in 24 hr format.')
                    document.getElementById("txtWorkingDayStart").focus();
                    return false
                }


                if ($.trim($("#txtWorkingDayFinish").val()).length == 0) {
                    alert('Please enter Working Day Finish Time.');
                    document.getElementById("txtWorkingDayFinish").focus();
                    return false;
                }
                else if (!check24hrTime($.trim($("#txtWorkingDayFinish").val()))) {
                    alert('Please enter Working Day Finish Time in 24 hr format.')
                    document.getElementById("txtWorkingDayFinish").focus();
                    return false
                }

                if (!checkTimeRange($.trim($("#txtWorkingDayStart").val()), $.trim($("#txtWorkingDayFinish").val()))) {
                    alert('Invalid Working Day Time Range. Finish Time must be greater than Start Time.')
                    document.getElementById("txtWorkingDayFinish").focus();
                    return false
                }

                if ($.trim($("#txtNonWorkingDayStart").val()).length == 0) {
                    alert('Please enter Non-Working Day Start Time.');
                    document.getElementById("txtNonWorkingDayStart").focus();
                    return false;
                }
                else if (!check24hrTime($.trim($("#txtNonWorkingDayStart").val()))) {
                    alert('Please enter Non-Working Day Start Time in 24 hr format.')
                    document.getElementById("txtNonWorkingDayStart").focus();
                    return false
                }

                if ($.trim($("#txtNonWorkingDayFinish").val()).length == 0) {
                    alert('Please enter Non-Working Day Finish Time.');
                    document.getElementById("txtNonWorkingDayFinish").focus();
                    return false;
                }
                else if (!check24hrTime($.trim($("#txtNonWorkingDayFinish").val()))) {
                    alert('Please enter Non-Working Day Finish Time in 24 hr format.')
                    document.getElementById("txtNonWorkingDayFinish").focus();
                    return false
                }

                if (!checkTimeRange($.trim($("#txtNonWorkingDayStart").val()), $.trim($("#txtNonWorkingDayFinish").val()))) {
                    alert('Invalid Non-Working Day Time Range. Finish Time must be greater than Start Time.')
                    document.getElementById("txtNonWorkingDayFinish").focus();
                    return false
                }
            }
            return true;
        }


        function LoadMPAccountDetails(iAccountID) {
            ShowLoadingBox();
            $.ajax({
                url: '<%= ResolveUrl("~/Organisation/GetMPAccountByID") %>',
                data: { "iAccountID": iAccountID },
                type: "POST",
                cache: false,
                timeout: 6000000,
                async: true,
                success: function (result) {
                    HideLoadingBox();
                    $('#lblCurrentBalance').text(result.Balance);

                    if (result.IsEnabled) {
                        $('#chkBoxEnableMPAccount').attr("checked", "checked");
                    }

                    document.getElementById("txtName").value = result.Description;
                    document.getElementById("txtUsername").value = result.AccountLogin;
                    document.getElementById("txtPassword").value = result.Password;
                    document.getElementById("txtRepeatPassword").value = result.Password;

                    if (result.Paypal) {
                        $('#chkBoxPaypal').attr("checked", "checked");
                    }

                    if (result.Invoice) {
                        $('#chkBoxInvoice').attr("checked", "checked");
                    }

                    if (result.SendLowBalanceWarnings) {
                        $('#chkLowBal').attr("checked", "checked");
                        $("#txtLowBalWrnLvl").prop('disabled', false);
                        $("#txtLowBalContactEmail").prop('disabled', false);

                        document.getElementById("txtLowBalWrnLvl").value = result.BalanceWarningLimit;
                        sBalWarnEmail = document.getElementById("txtLowBalContactEmail").value = result.BalanceWarningEmail;
                    }

                    if (result.UsageRestriction) {
                        $('#chkUsageRestriction').attr("checked", "checked");

                        $("#txtWorkingDayStart").prop('disabled', false);
                        $("#txtWorkingDayFinish").prop('disabled', false);

                        $("#txtNonWorkingDayStart").prop('disabled', false);
                        $("#txtNonWorkingDayFinish").prop('disabled', false);

                        document.getElementById("txtWorkingDayStart").value = result.WorkDayAllowedFrom;
                        document.getElementById("txtWorkingDayFinish").value = result.WorkDayAllowedTo;

                        document.getElementById("txtNonWorkingDayStart").value = result.NonWorkDayAllowedFrom;
                        document.getElementById("txtNonWorkingDayFinish").value = result.NonWorkDayAllowedTo;
                    }

                },  // ends success        
                error: function (httpRequest, msg) {
                    HideLoadingBox();
                    $('#errorMsg').show().text('Error');
                }

            });//ends ajax   
        }

        function UsernameExist(strUsername) {
            var bExist = new Boolean();

            ShowLoadingBox();
            $.ajax({
                url: '<%= ResolveUrl("~/Organisation/IsUsernameUnique") %>',
                data: { "strUsername": strUsername },
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

        function UsernameExistOnUpdate(strUsername, iAccountID) {
            var bExist = new Boolean();

            ShowLoadingBox();
            $.ajax({
                url: '<%= ResolveUrl("~/Organisation/IsUsernameUniqueOnUpdate") %>',
                data: { "strUsername": strUsername, "iAccountID": iAccountID },
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

        function AddMPAccount() {
            if (Validate()) {

                var bIsEnabled = false;

                if ($('#chkBoxEnableMPAccount').is(':checked')) {
                    bIsEnabled = true;
                }

                var sAccLogin = $.trim(document.getElementById("txtUsername").value);

                var bExist = UsernameExist(sAccLogin);

                if (bExist) {
                    alert('Username already exists.');
                    return false;
                }

                var sDesc = $.trim(document.getElementById("txtName").value);
                var sPassword = $.trim(document.getElementById("txtPassword").value);
                var bSendLowBalWarn = false;
                var sBalWarnEmail = "";
                var iBalWarnLmt = 0;
                if ($('#chkLowBal').is(':checked')) {
                    bSendLowBalWarn = true;
                    iBalWarnLmt = $.trim(document.getElementById("txtLowBalWrnLvl").value);
                    sBalWarnEmail = $.trim(document.getElementById("txtLowBalContactEmail").value);
                }

                var bPayPal = false;
                var bInvoice = false;

                if ($('#chkBoxPaypal').is(':checked')) {
                    bPayPal = true;
                }
                if ($('#chkBoxInvoice').is(':checked')) {
                    bInvoice = true;
                }

                var bUsageRestriction = false;
                var sWorkingDayStart = "";
                var sWorkingDayFinish = "";
                var sNonWorkingDayStart = "";
                var sNonWorkingDayFinish = "";

                if ($('#chkUsageRestriction').is(':checked')) {
                    bUsageRestriction = true;

                    sWorkingDayStart = $.trim(document.getElementById("txtWorkingDayStart").value);
                    sWorkingDayFinish = $.trim(document.getElementById("txtWorkingDayFinish").value);
                    sNonWorkingDayStart = $.trim(document.getElementById("txtNonWorkingDayStart").value);
                    sNonWorkingDayFinish = $.trim(document.getElementById("txtNonWorkingDayFinish").value);
                }

                ShowLoadingBox();
                $.ajax({
                    url: '<%= ResolveUrl("~/Organisation/AddMPAccount") %>',
                    data: {
                        "bIsEnabled": bIsEnabled,
                        "sAccLogin": sAccLogin,
                        "sDesc": sDesc,
                        "sPassword": sPassword,
                        "bSendLowBalWarn": bSendLowBalWarn,
                        "iBalWarnLmt": iBalWarnLmt,
                        "sBalWarnEmail": sBalWarnEmail,
                        "bUsageRestriction": bUsageRestriction,
                        "sWorkingDayStart": sWorkingDayStart,
                        "sWorkingDayFinish": sWorkingDayFinish,
                        "sNonWorkingDayStart": sNonWorkingDayStart,
                        "sNonWorkingDayFinish": sNonWorkingDayFinish,
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
                            alert("The Micropayment Account has been added.");
                            $('#dlgMPAccount').dialog('close');
                            $("#gridMPAccountList").trigger("reloadGrid");
                        }
                        else {
                            $('#errorMsg').show().text('Error occurred while adding Micropayment Account. Please try again.');
                        }
                    },  // ends success        
                    error: function (httpRequest, msg) {
                        HideLoadingBox();
                        $('#errorMsg').show().text('Error occurred while adding Micropayment Account. Please try again.');
                    }

                });//ends ajax     
            }
        }


        function UpdateMPAccount() {
            if (Validate()) {
                var iAccountID = $.trim($('#hdnAccountID').val());

                var bIsEnabled = false;

                if ($('#chkBoxEnableMPAccount').is(':checked')) {
                    bIsEnabled = true;
                }

                var sAccLogin = $.trim($('#txtUsername').val());


                var bExist = UsernameExistOnUpdate(sAccLogin, iAccountID);

                if (bExist) {
                    alert('Username already exists.');
                    return false;
                }

                var sDesc = $.trim($('#txtName').val());
                var sPassword = $.trim($('#txtPassword').val());
                var bSendLowBalWarn = false;

                var iBalWarnLmt = 0;
                var sBalWarnEmail = 0;
                if ($('#chkLowBal').is(':checked')) {
                    bSendLowBalWarn = true;

                    iBalWarnLmt = $.trim($('#txtLowBalWrnLvl').val());
                    sBalWarnEmail = $.trim($('#txtLowBalContactEmail').val());
                }

                var bUsageRestriction = false;
                var sWorkingDayStart = "";
                var sWorkingDayFinish = "";
                var sNonWorkingDayStart = "";
                var sNonWorkingDayFinish = "";

                var bPayPal = false;
                var bInvoice = false;

                if ($('#chkBoxPaypal').is(':checked')) {
                    bPayPal = true;
                }
                if ($('#chkBoxInvoice').is(':checked')) {
                    bInvoice = true;
                }


                if ($('#chkUsageRestriction').is(':checked')) {
                    bUsageRestriction = true;

                    sWorkingDayStart = $.trim(document.getElementById("txtWorkingDayStart").value);
                    sWorkingDayFinish = $.trim(document.getElementById("txtWorkingDayFinish").value);
                    sNonWorkingDayStart = $.trim(document.getElementById("txtNonWorkingDayStart").value);
                    sNonWorkingDayFinish = $.trim(document.getElementById("txtNonWorkingDayFinish").value);
                }

                ShowLoadingBox();
                $.ajax({
                    url: '<%= ResolveUrl("~/Organisation/UpdateMPAccount") %>',
                    data: {
                        "iAccountID": iAccountID,
                        "bIsEnabled": bIsEnabled,
                        "sAccLogin": sAccLogin,
                        "sDesc": sDesc,
                        "sPassword": sPassword,
                        "bSendLowBalWarn": bSendLowBalWarn,
                        "iBalWarnLmt": iBalWarnLmt,
                        "sBalWarnEmail": sBalWarnEmail,
                        "bUsageRestriction": bUsageRestriction,
                        "sWorkingDayStart": sWorkingDayStart,
                        "sWorkingDayFinish": sWorkingDayFinish,
                        "sNonWorkingDayStart": sNonWorkingDayStart,
                        "sNonWorkingDayFinish": sNonWorkingDayFinish,
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
                            alert("The Micropayment Account has been updated.");
                            $('#dlgMPAccount').dialog('close');
                            $("#gridMPAccountList").trigger("reloadGrid");
                        }
                        else {
                            $('#errorMsg').show().text('Error occurred while updating Micropayment Account. Please try again.');
                        }
                    },  // ends success        
                    error: function (httpRequest, msg) {
                        HideLoadingBox();
                        $('#errorMsg').show().text('Error occurred while updating Micropayment Account. Please try again.');
                    }
                });
            }
        }

        $("img.imgInfo").hover(function () {
            var el = document.getElementById("idImgInfo");
            el.setAttribute('Title', $('#txtPassword').val());
        });

        $('#frmAddEditMPAccount').submit(function (e) {
            e.preventDefault();
        });

    </script>
</asp:PlaceHolder>
<form id="frmAddEditMPAccount">
<div id="dlgAddEditMPAccount">

    <input type="hidden" id="hdnAccountID" />
    <table id="user_form">
        <tr>
            <td>
                <table>
                    <tr id="trBal_EstDt">
                        <td></td>
                        <td colspan="2" style="text-align: right">
                            <span style="font-weight: 300;">Current Balance: £<label id="lblCurrentBalance">&nbsp;</label></span>
                        </td>
                        <td style="width: 10px;"></td>
                        <td>
                            <span style="font-weight: 300;">Low Balance Estimate: 01/10/2013</span>
                        </td>
                    </tr>
                </table>
            </td>
        </tr>
        <tr>
            <td colspan="5"></td>
        </tr>

        <tr>
            <td colspan="4">
                <table>

                    <tr>
                        <td class="required">Name</td>
                        <td style="width: 154px">
                            <input type="text" id="txtName" maxlength="50" tabindex="1" />
                        </td>

                        <td style="width: 10px;"></td>

                        <td class="required">Username</td>
                        <td>
                            <input type="text" id="txtUsername" maxlength="20" tabindex="2" />
                        </td>

                        <td style="width: 10px;"></td>

                        <td style="width: 206px" rowspan="2">
                            <fieldset>
                                <legend>Payment Types</legend>
                                <table>
                                    <tr>
                                        <td>
                                            <input type="checkbox" id="chkBoxPaypal" name="paymentMethod" tabindex="5" /></td>
                                        <td style="text-align: left">Paypal</td>
                                    </tr>
                                    <%--<tr>
                                            <td>
                                                <input type="checkbox" id="chkBoxDirectDebit" name="paymentMethod" /></td>
                                            <td style="text-align: left">Direct Debit</td>
                                        </tr>--%>
                                    <tr>
                                        <td>
                                            <input type="checkbox" id="chkBoxInvoice" name="paymentMethod" tabindex="6" /></td>
                                        <td style="text-align: left">Invoice</td>
                                    </tr>
                                </table>
                            </fieldset>
                        </td>
                    </tr>
                    <tr>
                        <td class="required">Password</td>
                        <td style="width: 154px">
                            <input type="password" id="txtPassword" maxlength="40" tabindex="3" />
                        </td>

                        <td style="width: 10px;">
                            <img src="<%= ResolveUrl("~/Content/Images/info.png")%>" title="Password" class="imgInfo" id="idImgInfo" /></td>

                        <td id="lblRepeatPassword" class="required">Repeat Password</td>
                        <td>
                            <input type="password" id="txtRepeatPassword" maxlength="40" tabindex="4" />
                        </td>

                        <td style="width: 10px;"></td>

                    </tr>
                    <tr>
                        <td style="text-align: right;">
                            <input type="checkbox" id="chkBoxEnableMPAccount" tabindex="7" /></td>
                        <td style="text-align: left; font-family: 'Segoe UI';">Enabled</td>
                        <td colspan="5"></td>
                    </tr>
                    <tr>
                        <td colspan="8">
                            <table>
                                <tr>
                                    <td colspan="2" style="text-align: left">
                                        <span style="font-weight: 600;">Low Balance</span>
                                    </td>
                                    <td colspan="3"></td>
                                    <td style="width: 10px;"></td>
                                    <td colspan="2"></td>
                                </tr>
                                <tr>
                                    <td style="text-align: right; width: 50px;">
                                        <input type="checkbox" id="chkLowBal" tabindex="8" /></td>
                                    <td style="text-align: left;">Low Balance Warning</td>

                                    <td colspan="2" style="text-align: left;">
                                        <label for="lblLowBalanceWarningLevel">Low Balance Warning Level</label><br />
                                        <input type="text" id="txtLowBalWrnLvl" maxlength="3" tabindex="9" />
                                    </td>

                                    <td style="width: 10px;"></td>

                                    <td colspan="2" style="text-align: left;">
                                        <label for="lblLowBalanceContactEmail">Low Balance Contact Email</label>
                                        <br />
                                        <input type="text" id="txtLowBalContactEmail" maxlength="255" tabindex="10" />
                                    </td>
                                </tr>
                                <tr>
                                    <td colspan="2" style="text-align: left">
                                        <span style="font-weight: 600;">Usage Limit</span>
                                    </td>
                                    <td colspan="2"></td>
                                    <td style="width: 10px;"></td>
                                    <td colspan="2" style="width: 206px"></td>
                                </tr>
                                <tr id="workingDaysVal">
                                    <td style="text-align: right;">
                                        <input type="checkbox" id="chkUsageRestriction" tabindex="11" /></td>
                                    <td style="text-align: left; width: 154px;">Usage Restriction</td>

                                    <td colspan="2" style="text-align: left;">Working Day Start<br />
                                        <input type="text" id="txtWorkingDayStart" placeholder="hh:mm" maxlength="5" tabindex="12" />

                                    </td>

                                    <td style="width: 10px;"></td>

                                    <td colspan="2" style="text-align: left;">Working Day Finish
                                <br />
                                        <input type="text" id="txtWorkingDayFinish" placeholder="hh:mm" maxlength="5" tabindex="13" />

                                    </td>
                                </tr>
                                <tr id="nonWorkingDaysVal">
                                    <td style="text-align: right;">&nbsp;</td>
                                    <td style="text-align: left; width: 154px;">&nbsp;</td>

                                    <td colspan="2" style="text-align: left;">Non-Working Day Start<br />
                                        <input type="text" id="txtNonWorkingDayStart" placeholder="hh:mm" maxlength="5" tabindex="14" />

                                    </td>

                                    <td style="width: 10px;"></td>

                                    <td colspan="2" style="text-align: left;">Non-Working Day Finish
                                <br />
                                        <input type="text" id="txtNonWorkingDayFinish" placeholder="hh:mm" maxlength="5" tabindex="15" />

                                    </td>
                                </tr>
                                <tr>
                                    <td style="text-align: right;">&nbsp;</td>
                                    <td style="text-align: left; width: 154px;">&nbsp;</td>

                                    <td style="width: 10px;"></td>

                                    <td colspan="2" style="text-align: left;">
                                        <br />
                                        &nbsp;</td>

                                    <td colspan="2" style="text-align: right;">

                                        <input type="button" id="btnAddMPAccount" value="Add" onclick="AddMPAccount()" class="buttons" tabindex="16" />
                                        <input type="button" id="btnUpdateMPAccount" value="Update" onclick="UpdateMPAccount()" class="buttons" tabindex="17" />
                                        <input type="button" id="btnCancelDlg" class="buttons" value="Cancel" onclick="javascript: $('#dlgMPAccount').dialog('close')" tabindex="18" />

                                    </td>
                                </tr>

                            </table>
                        </td>
                    </tr>

                </table>
            </td>
        </tr>

    </table>


</div>
</form>
<div class="modal">
    <div id="LoadingText">Loading</div>
</div>

