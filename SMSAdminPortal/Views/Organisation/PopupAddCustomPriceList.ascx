<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<dynamic>" %>

<script type="text/javascript">

    var iMinPrice;
    $(document).ready(function () {
        iMinPrice = '<%= SMSAdminPortal.Commons.PortalConstants.PRICELIST_MINPRICE%>';

        var iTierID = '<%= ViewData["TierID"] %>';
        $('#hdnTierID').val(iTierID);

        if (iTierID != 0) {

            $('#btnAddPL').hide();
            $('#btnUpdatePL').show();

            LoadTierDetails(iTierID);
        }
        else {
            $('#btnAddPL').show();
            $('#btnUpdatePL').hide();
        }

    });


    function LoadTierDetails(iTierID) {
        ShowLoadingBox();
        $.ajax({

            url: '<%= ResolveUrl("~/Organisation/GetCustomTierByID") %>',
            data: { "iTierID": iTierID },
            type: "POST",
            cache: false,
            timeout: 6000000,
            async: true,
            success: function (result) {
                HideLoadingBox();
                document.getElementById("txtPrice").value = result.PricePerSMS;
                document.getElementById("txtBand").value = result.Band;

            },  // ends success        
            error: function (httpRequest, msg) {
                HideLoadingBox();
                $('#errorMsg').show().text('Error');
            }

        });//ends ajax   

    }

    function Validate()
    {
        var fPrice = $.trim(document.getElementById("txtPrice").value);
        var iBand = $.trim(document.getElementById("txtBand").value);

        if (fPrice.length == 0) {
            alert('Please enter the Price per SMS.');
            document.getElementById("txtPrice").focus();
            return false;
        }

        if (iBand.length == 0) {
            alert('Please enter the number of SMSes.');
            document.getElementById("txtBand").focus();
            return false;
        }
                
        if (!ValidateDecimalValues(fPrice)) {
            alert('Please enter valid Price Per SMS upto two decimal points.');
            document.getElementById("txtPrice").focus();
            return false;
        }

        if (!ValidateNumericValues(iBand)) {
            alert('Please enter a number of SMSes without fractions.');
            document.getElementById("txtBand").focus();
            return false;
        }

        if (parseFloat(fPrice) < parseFloat(iMinPrice)) {
            alert('Price Per SMS cannot be less than ' + iMinPrice+'.');
            document.getElementById("txtPrice").focus();
            return false;
        }

        return true;
    }

    function DoesBandExist(iBand) {
        var bExist = new Boolean();
        ShowLoadingBox();
        $.ajax({
            url: '<%= ResolveUrl("~/Organisation/DoesBandExist") %>',
            data: { "iBand": iBand },
            type: "POST",
            cache: false,
            timeout: 6000000,
            async: false,
            success: function (result) {
                HideLoadingBox();
                bExist = result;
            }
        });

        return bExist;
    }

    function DoesBandExistOnUpdate(iTierID, iBand) {
        var bExist = new Boolean();

        ShowLoadingBox();
        $.ajax({
            url: '<%= ResolveUrl("~/Organisation/DoesBandExistOnUpdate") %>',
            data: { "iTierID": iTierID, "iBand": iBand },
            type: "POST",
            cache: false,
            timeout: 6000000,
            async: false,
            success: function (result) {
                HideLoadingBox();
                bExist = result;
            }
        });

        return bExist;
    }

    function AddPriceList() {

        var fPrice = document.getElementById("txtPrice").value;
        var iBand = document.getElementById("txtBand").value;

        if (Validate()) {

            if (DoesBandExist(iBand)) {
                alert('Band already exist.');
                document.getElementById("txtPrice").focus();
                return false;
            }

            ShowLoadingBox();
            $.ajax({
                url: '<%= ResolveUrl("~/Organisation/AddCustomTier") %>',
                data: { "fPricePerPence": fPrice, "iBand": iBand },
                type: "POST",
                cache: false,
                timeout: 6000000,
                async: true,
                success: function (result) {
                    HideLoadingBox();
                    if (result == "true") {
                        alert("The Price List tier has been added.");
                        $('#dlgPriceList').dialog('close');
                        $("#gridCustomPriceList").trigger("reloadGrid");
                    }
                    else {
                        $('#errorMsg').show().text('Error occurred while adding Custom Tier. Please try again.');
                    }
                },  // ends success        
                error: function (httpRequest, msg) {
                    HideLoadingBox();
                    $('#errorMsg').show().text('Error occurred while adding Custom Tier. Please try again.');
                }

            });//ends ajax   
        }
    }



    function UpdatePriceList() {
        var iTierID = $.trim(document.getElementById("hdnTierID").value);
        var fPrice = $.trim(document.getElementById("txtPrice").value);
        var iBand = $.trim(document.getElementById("txtBand").value);

        if (Validate()) {

            if (DoesBandExistOnUpdate(iTierID, iBand)) {
                alert('Band already exist.');
                document.getElementById("txtPrice").focus();
                return false;
            }

            ShowLoadingBox();
            $.ajax({
                url: '<%= ResolveUrl("~/Organisation/UpdateCustomTier") %>',
                data: { "iTierID": iTierID, "fPricePerPence": fPrice, "iBand": iBand },
                type: "POST",
                cache: false,
                timeout: 6000000,
                async: true,
                success: function (result) {
                    HideLoadingBox();
                    if (result == "true") {
                        alert("The Price List tier has been updated.");
                        $('#dlgPriceList').dialog('close');
                        $("#gridCustomPriceList").trigger("reloadGrid");
                    }
                    else {
                        $('#errorMsg').show().text('Error occurred while updating Custom Tier. Please try again.');
                    }
                },  // ends success        
                error: function (httpRequest, msg) {
                    HideLoadingBox();
                    $('#errorMsg').show().text('Error occurred while updating Custom Tier. Please try again.');
                }
            });
        }
    }

</script>


<div id="dlgAddEditPL">

    <input type="hidden" id="hdnTierID" />

    <table  id="user_form">
        <tr>
            <td colspan="2" style="color: Red; display: none;" id="errorMsg"></td>
        </tr>
        <tr>
            <td style="padding-bottom:10px;"><div class="required">Price per SMS</div><div style="font-size:smaller;">(Pence)</div></td>
            <td style="padding-bottom:10px;">
                <input id="txtPrice" type="text" maxlength="5" style="width: 150px;" /></td>
        </tr>

         <tr>
            <td style="padding-bottom:10px;" class="required">Band</td>
            <td style="padding-bottom:10px;">
                <input id="txtBand" type="text" maxlength="6" style="width: 150px;" />
            </td>
        </tr>
        <tr>
            <td colspan="3" style="text-align: right">
                <input type="button" id="btnAddPL" value="Add" onclick="AddPriceList()" class="buttons" />
                <input type="button" id="btnUpdatePL" value="Update" onclick="UpdatePriceList()" class="buttons" />
                <input type="button" id="btnCancelDlg" class="buttons" value="Cancel" onclick="javascript: $('#dlgPriceList').dialog('close')" />
            </td>
        </tr>
    </table>

</div>
