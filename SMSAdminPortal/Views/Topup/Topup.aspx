<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/MainMaster.Master" Inherits="System.Web.Mvc.ViewPage<dynamic>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
    Topup
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

    <asp:PlaceHolder ID="phScripts" runat="server">
        <script type="text/javascript">

            $(document).ready(function () {

                $('#trBalance').hide();

                var msg = '<%= ViewData["ErrorMessage"] %>';
                if (msg != null)
                    $('#errorMsg').show().text(msg);

                var successMsg = '<%= ViewData["SuccessMessage"] %>';

                if (successMsg.length > 0) {
                    alert(successMsg);
                }

                PopulateCompanies();
                
                $("#ddlCompanies").change(function () {

                    e = document.getElementById("ddlCompanies");
                    var CompanyID = e.options[e.selectedIndex].value;

                    var index = $("option:selected", this).index();

                    if (index != 0) {
                        //Populate MPAccount List
                        PopulateOrganisations(CompanyID);
                    }
                    else {
                        $("#ddlOrganisation option").remove();
                        $("#ddlMPAccount option").remove();
                    }

                    //Hide 'Balance' label
                    $('#trBalance').hide();

                });

                $("#ddlOrganisation").change(function () {

                    e = document.getElementById("ddlOrganisation");
                    var iOrganisationID = e.options[e.selectedIndex].value;

                    var index = $("option:selected", this).index();

                    if (index != 0) {
                        //Populate MPAccount List
                        PopulateMPAccounts(iOrganisationID);
                    }
                    else
                        $("#ddlMPAccount option").remove();

                    //Hide 'Balance' label
                    $('#trBalance').hide();

                });

                $("#ddlMPAccount").change(function () {
                    var e = document.getElementById("ddlMPAccount");
                    var iMPActID = e.options[e.selectedIndex].value;

                    var index = $("option:selected", this).index();

                    if (index == 0) {
                        //Hide 'Balance' label
                        $('#trBalance').hide();
                    }
                    $('#trBalance').show();
                    var Balance = $('option:selected', this).attr('balance');
                    $("#lblBalance").text(Balance);
                });

            });
            
            function PopulateCompanies()
            {
                $("#ddlCompanies option").remove();

                $('#ddlCompanies').append($("<option></option>").attr("value", "loading").text("Loading.."));

                $.ajax({
                    url: '<%= ResolveUrl("~/Topup/GetListOfCompanies") %>',
                    cache: false,
                    timeout: 6000000,
                    success: function (result) {

                        if (result.length > 0) {
                            $("#ddlCompanies option[value='loading']").remove();
                            $('#ddlCompanies').append($("<option></option>").attr('selected', 'selected').attr("value", 0).text("Select Company"));
                            for (var i = 0; i < result.length; i++) {
                                $('#ddlCompanies').append($("<option></option>").attr("value", result[i].Value).attr("address", result[i].Attribute).text(result[i].Text));

                            }
                        }
                        else {
                            $("#ddlCompanies option[value='loading']").remove();
                            $('#ddlCompanies').append($("<option></option>").attr("value", 0).text("Select Company"));
                        }
                    },
                    error: function (httpRequest, msg) {
                        $('#errorMsg').show().text('An error occurred while retrieving Company list. Please try again.');
                    }
                }); //ends ajax
            }

            function PopulateOrganisations(CompanyID) {
                $("#ddlOrganisation option").remove();
                $("#ddlMPAccount option").remove();

                $('#ddlOrganisation').append($("<option></option>").attr("value", "loading").text("Loading.."));

                $.ajax({
                    url: '<%= ResolveUrl("~/Topup/GetOrganisationList") %>',
                    data: { "CompanyID": CompanyID },
                    cache: false,
                    timeout: 6000000,
                    success: function (result) {

                        if (result.length > 0) {
                            $("#ddlOrganisation option[value='loading']").remove();
                            $('#ddlOrganisation').append($("<option></option>").attr("value", "0").text("Select Organisation"));

                            for (var i = 0; i < result.length; i++) {
                                $('#ddlOrganisation').append($("<option></option>").attr("value", result[i].Value).text(result[i].Text));
                            }
                        }
                        else {
                            $("#ddlOrganisation option[value='loading']").remove();
                            $('#ddlOrganisation').append($("<option></option>").attr("value", "0").text("Select Organisation"));
                        }
                    },
                    error: function (httpRequest, msg) {
                        $('#errorMsg').show().text(msg);
                    }
                }); //ends ajax
            }

            function PopulateMPAccounts(iOrganisationID) {
                $("#ddlMPAccount option").remove();
                $('#ddlMPAccount').append($("<option></option>").attr("value", "loading").text("Loading.."));

                $.ajax({
                    url: '<%= ResolveUrl("~/Topup/GetMPAcctListByOrg") %>',
                    data: { "iOrganisationID": iOrganisationID },
                    cache: false,
                    timeout: 6000000,
                    success: function (result) {

                        if (result.length > 0) {
                            $("#ddlMPAccount option[value='loading']").remove();
                            $('#ddlMPAccount').append($("<option></option>").attr("value", "0").text("Select Micropayment Account"));

                            for (var i = 0; i < result.length; i++) {
                                $('#ddlMPAccount').append($("<option></option>").attr("value", result[i].Value).attr("balance", result[i].Attribute).text(result[i].Text));
                            }
                        }
                        else {
                            $("#ddlMPAccount option[value='loading']").remove();
                            $('#ddlMPAccount').append($("<option></option>").attr("value", "0").text("Select Micropayment Account"));
                        }
                    },
                    error: function (httpRequest, msg) {
                        $('#errorMsg').show().text(msg);
                    }
                }); //ends ajax
            }

            function Validate() {

                e = document.getElementById("ddlCompanies");
                var iCompanyID = e.options[e.selectedIndex].value;

                if (iCompanyID == 0) {
                    alert('Please selected the Company');
                    document.getElementById("ddlCompanies").focus();
                    return false;
                }

                e = document.getElementById("ddlOrganisation");
                var iOrganisationID = e.options[e.selectedIndex].value;

                if (iOrganisationID == 0) {
                    alert('Please selected the Organisation');
                    document.getElementById("ddlOrganisation").focus();
                    return false;
                }

                e = document.getElementById("ddlMPAccount");
                var iMPAccountCode = e.options[e.selectedIndex].value;

                if (iMPAccountCode == 0) {
                    alert('Please selected the Micropayment Account');
                    document.getElementById("ddlMPAccount").focus();
                    return false;
                }

                var iAmt = $.trim($("#txtAmount").val());
                if (iAmt.length == 0) {
                    alert('Please enter Topup Amount');
                    document.getElementById("txtAmount").focus();
                    return false;
                }

                if (!ValidateDecimalValues($.trim($("#txtAmount").val()))) {
                    alert('Invalid Topup Amount. Please enter valid Topup Amount upto two decimal points');
                    document.getElementById("txtAmount").focus();
                    return false;
                }

                return true;
            }

        </script>
    </asp:PlaceHolder>
    <form action="<%= ResolveUrl("~/Topup/Topup")%>" method="post">
        <div id="user_form">
            <br />
            <fieldset>
                <legend>
                    <font style="font-family: Calibri; font-size: large">Topup</font>
                </legend>
                <br />
                <table style="margin-right: 5%">
                    <tr>
                        <td colspan="3" style="color: Red; display: none;" id="errorMsg"></td>
                    </tr>
                    <tr>
                        <td class="required">Company</td>
                        <td>&nbsp;</td>
                        <td>
                            <select id="ddlCompanies" name="Company" style="width:300px;"/>
                        </td>
                    </tr>
                    <tr>
                        <td class="required">Organisation</td>
                        <td>&nbsp;</td>
                        <td>
                            <select id="ddlOrganisation" name="Organisation" style="width:300px;"/>
                        </td>
                    </tr>
                    <tr>
                        <td class="required">Micropayment Account</td>
                        <td>&nbsp;</td>
                        <td>
                            <select id="ddlMPAccount" name="MPAccount" style="width:300px;"/>
                        </td>
                    </tr>
                    <tr id="trBalance">
                        <td colspan="2"></td>
                        <td style="padding-bottom: 10px;">Balance: £<label id="lblBalance"></label>
                        </td>
                    </tr>
                    <tr>
                        <td class="required">Amount</td>
                        <td>&nbsp;</td>
                        <td>
                            <input type="text" id="txtAmount" onblur="roundOffDecimals(this);" name="Amount" maxlength="14"  style="width:300px;"/>
                        </td>
                    </tr>
                    <tr>
                        <td colspan="3" align="right">
                            <input id="btnTopup" type="submit" value="Topup!" class="buttons" onclick="return Validate()" />
                        </td>
                    </tr>
                </table>
            </fieldset>
        </div>
    </form>
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="SubContent" runat="server">
</asp:Content>
