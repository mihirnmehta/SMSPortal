<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Organisation/OrganisationDetails.master" Inherits="System.Web.Mvc.ViewPage<dynamic>" %>

<asp:Content ID="Content2" ContentPlaceHolderID="OrgTitleContent" runat="server">
    Organisation Report
</asp:Content>

<asp:Content ID="Content1" ContentPlaceHolderID="OrgMainContent" runat="server">
    <asp:PlaceHolder ID="phScripts" runat="server">

        <script type="text/javascript">

            $(document).ready(function () {

                $('.datepicker').datepicker({
                    showOptions: { speed: 'fast' },
                    //changeDay: true,
                    changeMonth: true,
                    changeYear: true,
                    dateFormat: 'dd/mm/yy',
                    //gotoCurrent: false,
                    showOn: 'button',
                    buttonImage: '<%= ResolveUrl("~/Content/Images/Calender1.png")%>',
                    buttonImageOnly: true
                });

                $(".StatusCheckbox").bind("click", CheckBoxChangeEvent);

                HideUsageReportControls();

                $('.TopupRptFooter').hide();

                $('.UsageRptFooter').hide();

                $('#btnExportReport').hide();

                document.getElementById("chkTopupReport").checked = true;
            });

            function CheckBoxChangeEvent() {
                var stbx;
                $.each($(".StatusCheckbox:checked"), function (index, elem) {
                    stbx = $(elem).val();

                });

                if (stbx == 1) {
                    $('#gridUsageReport').jqGrid('GridUnload');
                    $('#btnExportReport').hide();

                    HideUsageReportControls();
                    ShowTopupReportControls();
                }
                else if (stbx == 2) {
                    $('#gridTopupReport').jqGrid('GridUnload');
                    $('#btnExportReport').hide();

                    HideTopupReportControls();
                    ShowUsageReportControls();
                }
            }

            function ShowTopupReportControls() {
                $('#lblTopupType').show();
                $('#ddlBillingMethod').show();

                $('#btnRunTopupReport').show();
            }

            function HideTopupReportControls() {
                $("#txtStartDate").val('');
                $("#txtEndDate").val('');

                $('.TopupRptFooter').hide();
                $('#TopupRptTbl').hide();

                $('#lblTopupType').hide();
                $('#ddlBillingMethod').hide();

                $('#btnRunTopupReport').hide();
            }

            function ShowUsageReportControls() {
                $('#lblMPAccount').show();
                $('#ddlMPAccount').show();

                $('#btnRunUsageReport').show();
                PopulateMPAccounts();
            }

            function HideUsageReportControls() {
                $("#txtStartDate").val('');
                $("#txtEndDate").val('');

                $('.UsageRptFooter').hide();
                $('#UsageRptTbl').hide();

                $('#lblMPAccount').hide();
                $('#ddlMPAccount').hide();

                $('#btnRunUsageReport').hide();
            }

            function PopulateMPAccounts() {
                $("#ddlMPAccount option").remove();
                $('#ddlMPAccount').append($("<option></option>").attr("value", "loading").text("Loading.."));

                $.ajax({
                    url: '<%= ResolveUrl("~/Organisation/GetMPAcctListByOrg")%>',
                    cache: false,
                    timeout: 6000000,
                    success: function (result) {
                        if (result.length > 0) {
                            $("#ddlMPAccount option[value='loading']").remove();
                            $('#ddlMPAccount').append($("<option></option>").attr("value", "0").text("All"));

                            for (var i = 0; i < result.length; i++) {
                                $('#ddlMPAccount').append($("<option></option>").attr("value", result[i].Value).text(result[i].Text));
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

            function LoadTopupDetails() {
                $('#TopupRptTbl').show();

                var iBillingMethodID = $("#ddlBillingMethod").val();

                $('#gridTopupReport').jqGrid('GridUnload');
                $('.TopupRptFooter').hide();
                $('#btnExportReport').hide();

                var sStartDate = $.trim($("#txtStartDate").val());
                var sEndDate = $.trim($("#txtEndDate").val());

                if (sStartDate.length == 0) {
                    alert('Please enter the Start date.');
                    return false;
                }
                if (sEndDate.length == 0)
                {
                    alert('Please enter the End date.');
                    return false;
                }

                if (!checkStartEndDate(sStartDate, sEndDate)) {
                    alert('Start date must be smaller than or equal to End date.');
                    return false;
                }

                $('#hdnStartDate').val(sStartDate);
                $('#hdnEndDate').val(sEndDate);

                //alert("StartDate: " + sStartDate + " EndDate: " + sEndDate+" BillingID: "+iBillingMethodID);

                $("#gridTopupReport").jqGrid({
                    url: '<%= ResolveUrl("~/Organisation/GetTopupDetailsByOrganisationID/")%>',
                    datatype: 'json',
                    mtype: 'GET',
                    postData: { "sStartDate": sStartDate, "sEndDate": sEndDate, "iBillingMethodID": iBillingMethodID },
                    colNames: ['SMS Account', 'Date Time', 'Topup Amount', 'Method'],
                    colModel:
                    [
                        { name: 'MPAccountName', index: 'MPAccountName', width: '30%', align: 'left', sortable: true, search: false },
                        { name: 'TransactionDate', index: 'TransactionDate', width: '30%', align: 'center', sortable: true, search: false },
                        { name: 'Amount', index: 'Amount', align: 'right', width: '20%', sortable: true, search: false },
                        { name: 'BillingMethod', index: 'BillingMethod', width: '20%', align: 'center', sortable: true, search: false }
                    ],
                    pager: '#TopupPager',
                    toppager: true,
                    rowNum: 100,
                    rowList: [100, 250, 500],
                    height: '100%',
                    width: '720',
                    viewrecords: true,
                    pgbuttons: true,
                    onPaging: function (pgButton) {

                        if ("user" == pgButton) {
                            var requestedPage = $("#gridTopupReport").getGridParam("page");
                            var lastPage = $("#gridTopupReport").getGridParam("lastpage");
                            if (eval(requestedPage) > eval(lastPage)) {
                                $("#gridTopupReport").setGridParam({ page: lastPage });
                            }

                        } else if ("records" == pgButton) {
                            var requestedPage = $("#gridTopupReport").getGridParam("page");
                            var totalRows = $("#gridTopupReport").getGridParam("records");
                            var rowsPerPage = $("#gridTopupReport").getGridParam("rowNum");
                            var lastPage = Math.ceil(eval(totalRows) / eval(rowsPerPage));
                            if (eval(requestedPage) > lastPage) {
                                $("#gridTopupReport").setGridParam({ page: lastPage });
                            }
                        }
                    }, //end onPaging
                    pginput: true,
                    gridComplete: function () {

                        var strSum = jQuery("#gridTopupReport").getGridParam('userData');

                        if (strSum.length > 0) {
                            $('#lblTotal_TopupAmount').text(strSum);

                            $('.TopupRptFooter').show();
                            $('#btnExportReport').show();
                        }
                    }

                }).navGrid('#TopupPager', { edit: false, add: false, del: false, search: false },
                {}, // settings for edit
                {}, // add
                {}, // delete
                {}, // search options
                {}
                )
                    iBillingMethodID = 0;
                }

                function LoadUsageDetails() {
                    $('#UsageRptTbl').show();

                    var iMPAccountCode = $("#ddlMPAccount").val();

                    $('#gridUsageReport').jqGrid('GridUnload');
                    $('.UsageRptFooter').hide();
                    $('#btnExportReport').hide();

                    var sStartDate = $.trim($("#txtStartDate").val());
                    var sEndDate = $.trim($("#txtEndDate").val());


                    if (sStartDate.length == 0) {
                        alert('Please enter the Start date.');
                        return false;
                    }

                    if (sEndDate.length == 0) {
                        alert('Please enter the End date.');
                        return false;
                    }

                    if (!checkStartEndDate(sStartDate, sEndDate)) {
                        alert('Start date must be smaller than or equal to End date.');
                        return false;
                    }

                    $('#hdnStartDate').val(sStartDate);
                    $('#hdnEndDate').val(sEndDate);

                    //alert("StartDate: " + sStartDate + " EndDate: " + sEndDate + " MPAccountCode: " + iMPAccountCode);

                    $("#gridUsageReport").jqGrid({
                        url: '<%= ResolveUrl("~/Organisation/GetUsageDetailsByOrganisationID/")%>',
                        datatype: 'json',
                        mtype: 'GET',
                        postData: { "sStartDate": sStartDate, "sEndDate": sEndDate, "iMPAccountCode": iMPAccountCode },
                        colNames: ['SMS Account', 'Date & Time', 'Description', 'Cost'],
                        colModel:
                        [
                            { name: 'MPAccountName', index: 'MPAccountName', width: '20%', sortable: true, search: false },
                            { name: 'Date', index: 'Date', width: '20%', sortable: true, align: 'center', search: false },
                            { name: 'StatementDescription', index: 'StatementDescription', width: '50%', sortable: true, search: false },
                            { name: 'NetAmount', index: 'NetAmount', width: '10%', align: 'right', sortable: true, search: false }
                        ],
                        pager: '#UsagePager',
                        toppager: true,
                        rowNum: 10,
                        rowList: [10, 20, 50],
                        viewrecords: true,
                        hidegrid: false,
                        height: '100%',
                        width: '900',
                        align: 'center',
                        rownumbers: false,
                        pgbuttons: true,
                        onPaging: function (pgButton) {

                            if ("user" == pgButton) {
                                var requestedPage = $("#gridUsageReport").getGridParam("page");
                                var lastPage = $("#gridUsageReport").getGridParam("lastpage");
                                if (eval(requestedPage) > eval(lastPage)) {
                                    $("#gridUsageReport").setGridParam({ page: lastPage });
                                }

                            } else if ("records" == pgButton) {
                                var requestedPage = $("#gridUsageReport").getGridParam("page");
                                var totalRows = $("#gridUsageReport").getGridParam("records");
                                var rowsPerPage = $("#gridUsageReport").getGridParam("rowNum");
                                var lastPage = Math.ceil(eval(totalRows) / eval(rowsPerPage));
                                if (eval(requestedPage) > lastPage) {
                                    $("#gridUsageReport").setGridParam({ page: lastPage });
                                }
                            }
                        }, //end onPaging
                        pginput: true,
                        gridComplete: function () {

                            if ((jQuery("#gridUsageReport").getGridParam('userData')).length > 0) {
                                var footerValue = (jQuery("#gridUsageReport").getGridParam('userData')).split(",");

                                $('#lblTotal_MessageSent').text(footerValue[0]);
                                $('#lblTotal_Cost').text(footerValue[1]);

                                $('.UsageRptFooter').show();
                                $('#btnExportReport').show();
                            }
                        },
                        error: function (httpRequest, msg) {
                            alert("Error occured: " + msg);
                        }

                    }).navGrid('#UsagePager', { edit: false, add: false, del: false, search: false },
                {}, // settings for edit
                {}, // add
                {}, // delete
                {}, // search options
                {}
                )
                iMPAccountCode = 0;
            }


            //---------------------Export CSV Function-----------------

            function ExportTopupReportCSV() {

                var iBillingMethodID = $("#ddlBillingMethod").val();

                var sStartDate = $.trim($("#txtStartDate").val());
                var sEndDate = $.trim($("#txtEndDate").val());

                //2. Construct URL
                var DownloadURL = "<%= ResolveUrl("/Organisation/GetTopupDetailsByOrgIDForExport?sStartDate=")%>" + sStartDate + "&sEndDate=" + sEndDate + "&iBillingMethodID=" + iBillingMethodID;

                //3. Alert URL
                //alert(DownloadURL);

                //Example
                //window.location.href = "/Report/GetFinanceReportForExport?sStartDate=01/12/2013&sEndDate=12/12/2013&iBillingMethodID=0";

                window.location.href = DownloadURL;
            }

            function ExportUsageReportCSV() {

                var iMPAccountCode = $("#ddlMPAccount").val();

                var sStartDate = $.trim($("#txtStartDate").val());
                var sEndDate = $.trim($("#txtEndDate").val());

                //2. Construct URL
                var DownloadURL = "<%= ResolveUrl("/Organisation/GetUsageReportByOrgIDForExport?sStartDate=")%>)" + sStartDate + "&sEndDate=" + sEndDate + "&iMPAccountCode=" + iMPAccountCode;

                //3. Alert URL
                //alert(DownloadURL);

                //Example
                //window.location.href = "/Report/GetFinanceReportForExport?sStartDate=01/12/2013&sEndDate=12/12/2013&iBillingMethodID=0";

                window.location.href = DownloadURL;
            }

        </script>
    </asp:PlaceHolder>

    <table style="width: 100%;">
        <tr>
            <td style="font-family: Calibri; font-size: large" id="Heading">Reports</td>
        </tr>
    </table>
    <form action="<%= ResolveUrl("~/Organisation/GetReportForExport")%>" method="post">
        <table align="center">
            <tr>
                <td nowrap>
                    <input type="radio" id="chkTopupReport" value="1" name="OrgBasedReport" checked="checked" class="StatusCheckbox" />Topup Report
                </td>
                <td style="width: 30%"></td>
                <td nowrap>
                    <input type="radio" id="chkUsageReport" value="2" name="OrgBasedReport" class="StatusCheckbox" />Usage Report
                </td>
            </tr>
        </table>


        <table style="width: 100%;">
            <tr>
                <td style="vertical-align: top">
                    <table>
                        <tr>
                            <td>Period&nbsp;
                            </td>
                            <td>
                                <input type="text" class="datepicker" id="txtStartDate" disabled="disabled" />&nbsp;
                            <input type="hidden" id="hdnStartDate" name="sStartDate" />
                            </td>
                            <td>to&nbsp;
                            </td>
                            <td>
                                <input type="text" class="datepicker" id="txtEndDate" disabled="disabled" />&nbsp;&nbsp;
                            <input type="hidden" id="hdnEndDate" name="sEndDate" />
                            </td>
                            <td style="width: 100px; text-align: right;">
                                <label id="lblTopupType">Topup Type&nbsp;</label>
                                <label id="lblMPAccount">Account&nbsp;</label>
                                &nbsp;
                            </td>
                            <td>
                                <select id="ddlBillingMethod" name="BillingMethod" style="width: 250px;">
                                    <option value="0">All</option>
                                    <option value="1">Paypal</option>
                                    <option value="2">Invoice</option>
                                </select>
                                <select id="ddlMPAccount" name="MPAccount" style="width: 250px;" />&nbsp;&nbsp;
                            </td>
                            <td>
                                <input type="button" id="btnRunTopupReport" value="Run Report" onclick="LoadTopupDetails()" />
                                <input type="button" id="btnRunUsageReport" value="Run Report" onclick="LoadUsageDetails()" />
                            </td>
                        </tr>
                    </table>
                    <br />

                    <table style="width: 100%" id="TopupRptTbl">
                        <tr>
                            <td style="width: 100%">
                                <table id="gridTopupReport"></table>
                                <div id="TopupPager" style="text-align: center;"></div>
                            </td>
                        </tr>
                        <tr class="TopupRptFooter">
                            <td>
                                <span style="font-family: Calibri; font-size: large;">Total:&nbsp;&nbsp;</span><label id="lblTotal_TopupAmount"></label>
                            </td>
                        </tr>
                        <%-- <tr class="TopupRptFooter">
                        <td>
                            <input type="button" id="btnExportTopupReportCSV" value="Export Report" onclick="ExportTopupReportCSV()"/>
                        </td>
                    </tr>--%>
                    </table>


                    <table style="width: 100%" id="UsageRptTbl">
                        <tr>
                            <td style="width: 100%">
                                <table id="gridUsageReport"></table>
                                <div id="UsagePager" style="text-align: center;"></div>
                            </td>
                        </tr>
                        <tr class="UsageRptFooter" style="font-size: 16px;">
                            <td>
                                <span style="font-family: Calibri;">Total Messages sent:&nbsp;&nbsp;</span><label id="lblTotal_MessageSent"></label>
                            </td>
                        </tr>
                        <tr class="UsageRptFooter" style="font-size: 16px;">
                            <td>
                                <span style="font-family: Calibri;">Total Cost:&nbsp;&nbsp;</span><label id="lblTotal_Cost"></label>
                            </td>
                        </tr>
                        <%-- <tr class="UsageRptFooter">
                        <td colspan="2">
                            <input type="button" id="btnExportUsageReportCSV" value="Export Report" onclick="ExportUsageReportCSV()"/>
                        </td>
                    </tr>--%>
                    </table>

                </td>
            </tr>
            <tr>
                <td>
                    <input type="submit" value="Export Report" id="btnExportReport" />
                </td>
            </tr>
        </table>
    </form>
</asp:Content>
