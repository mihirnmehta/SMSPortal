<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<dynamic>" %>

<script type="text/javascript">

    $(document).ready(function () {
        var CompanyID = '<%= ViewData["CompanyID"] %>';

        $("#ddlOrganisations option").remove();
        $('#ddlOrganisations').append($("<option></option>").attr("value", "loading").text("Loading.."));

        $.ajax({
            url: '<%= ResolveUrl("~/Organisation/GetOrganisationsNotSetup") %>',
            data: { "CompanyID": CompanyID },
            cache: false,
            timeout: 6000000,
            success: function (result) {

                if (result.length > 0) {
                    $("#ddlOrganisations option[value='loading']").remove();
                    $('#ddlOrganisations').append($("<option></option>").attr('selected', 'selected').attr("value", "").text("Select Organisation"));
                    for (var i = 0; i < result.length; i++) {
                        $('#ddlOrganisations').append($("<option></option>").attr("value", result[i].Value).attr("address", result[i].Attribute).text(result[i].Text));

                    }
                }
                else {
                    $("#ddlOrganisations option[value='loading']").remove();
                    $('#ddlOrganisations').append($("<option></option>").attr("value", "").text("Select Organisation"));
                }
            },
            error: function (httpRequest, msg) {
                $('#errorMsg').show().text('An error occurred while retrieving Organisation list. Please try again.');
            }
        }); //ends ajax


        $("#ddlOrganisations").change(function() {

            //var index = $("select[name='Organisation'] option:selected").index();
            var index = $("option:selected", this).index();

            if (index == 0) {
                $("#lblAddress").text("");
                return;
            }

            var OrganisationID = $("#ddlOrganisations").val();

            var Address = $('option:selected', this).attr('address');
            $("#lblAddress").text(Address);
        });
    
        var bHasAddTopupAccess = HasAddTopupAccess();
        if (bHasAddTopupAccess)
            $("#chkInvoice").attr("disabled", "disabled");

    });
       
    function SetupOrganisation() {

        var OrganisationID = $("#ddlOrganisations").val();
        var strName        = $.trim(document.getElementById("txtName").value);
        var strEmail       = $.trim(document.getElementById("txtEmail").value);
        var strPhoneNumber = $.trim(document.getElementById("txtPhone").value);

        if (OrganisationID=="") {
            alert('Please Select the Organisation.');
            document.getElementById("ddlOrganisations").focus();
            return false;
        }

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
            alert('Invalid EmailAddress. Please enter valid Contact Email.');
            document.getElementById("txtEmail").focus();
            return false;
        }

        if (strPhoneNumber.length > 0 && !validatePhoneNumber(strPhoneNumber)) {
            alert('The Phone number must be a 9/10 digit numeric value with an optional \' + \' sign prefixed.');
            document.getElementById("txtPhone").focus();
            return false;
        }

        var bPayPal = $("#chkPayPal").is(':checked') ? true : false;
        var bInvoice = $("#chkInvoice").is(':checked') ? true : false;

        /*******************************************************************/
        ShowLoadingBox();
        $.ajax({
            url: '<%= ResolveUrl("~/Organisation/AddOrganisation") %>',
            data: {
                "iOrganisationID": OrganisationID,
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
                    $('#popupOrganisation').dialog('close');
                    $("#gridOrganisations").trigger("reloadGrid");
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

  
</script>

<div id="dlgAddEditOrg">

    <input type="hidden" id="hdnOrganisationID" />

    <table id="user_form">
            <tr>
                <td colspan="2" style="color:Red; display:none;" id="errorMsg"></td>
            </tr>
            <tr>
                <td style="text-align:center" colspan="2">Select Organisation <select id="ddlOrganisations" name="Organisation" style="width:300px;"/></td>
            </tr>
            <tr>
                <td colspan="2" >
                    <label id="lblAddress">&nbsp;</label>
                </td>
            </tr>
            <tr>
                <td>
                    <table>
                        <tr>
                            <td class="required">Contact Name</td>
                            <td>
                                <input id="txtName" type="text" maxlength="255"/>
                            </td>
                        </tr>
                        <tr>
                            <td class="required">Contact Email Address</td>
                            <td>
                                <input id="txtEmail" type="text" maxlength="255"/>
                            </td>
                        </tr>
                        <tr>
                            <td>Contact Phone Number</td>
                            <td>
                                <input id="txtPhone" type="text" maxlength="15"/>
                            </td>
                        </tr>
                    </table>
                </td>
                <td style="vertical-align:top;">
                    <fieldset>
                        <legend>Payment Types</legend>
                        <table>
                            <tr>
                                <td><input type="checkbox" id="chkPayPal" name="paymentMethod" /></td>
                                <td style="text-align: left">Paypal</td>
                            </tr>                            
                            <tr>
                                <td><input type="checkbox" id="chkInvoice" name="paymentMethod" /></td>
                                <td style="text-align: left">Invoice</td>
                            </tr>
                        </table>
                    </fieldset>
                </td>
            </tr>
            <tr>
                <td colspan="2"></td>
            </tr>            
            <tr>
                <td colspan="2" style="text-align:right">
                    <input type="button" id="btnSetupOrg" value="Setup" onclick="SetupOrganisation()" class="buttons" />&nbsp;                    
                    <input type="button" id="btnCancelDlg" class="buttons" value="Cancel" onclick="javascript: $('#popupOrganisation').dialog('close')" />
                </td>
            </tr>
      </table>
      
</div>
