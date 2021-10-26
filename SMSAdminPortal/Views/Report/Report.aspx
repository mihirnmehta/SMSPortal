<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/MainMaster.Master" Inherits="System.Web.Mvc.ViewPage<dynamic>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
    Report
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="SubContent" runat="server">
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

    <asp:PlaceHolder ID="phScripts" runat="server">
   
        <script type="text/javascript">

            var iSum;

            $(document).ready(function () {

                $('.datepicker').datepicker({
                    showOptions: { speed: 'fast' },
                    changeMonth: true,
                    changeYear: true,
                    dateFormat: 'dd/mm/yy',
                    showOn: 'button',
                    buttonImage: '<%= ResolveUrl("~/Content/Images/Calender1.png")%>',
                    buttonImageOnly: true
                });

                $(".StatusCheckbox").bind("click", CheckBoxChangeEvent);

                HideUsageReportControls();

                $('.TopupRptFooter').hide();

                $('.UsageRptFooter').hide();

                $('#btnExportReport').hide();

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
                $('#ddlBillingMethod').show();
                $('#lblTopupType').show();

                $('#btnRunTopupReport').show();
            }

            function HideTopupReportControls() {
                $("#txtStartDate").val('');
                $("#txtEndDate").val('');

                $('#TopupRptTbl').hide();

                $('#lblTopupType').hide();
                $('#ddlBillingMethod').hide();

                $('#btnRunTopupReport').hide();
            }

            function ShowUsageReportControls() {
                $('#btnRunUsageReport').show();
            }

            function HideUsageReportControls() {
                $('#UsageRptTbl').hide();

                $("#txtStartDate").val('');
                $("#txtEndDate").val('');

                $('#btnRunUsageReport').hide();
            }

            function LoadTopupDetails() {
                $('#TopupRptTbl').show();
                
                var iBillingMethodID = $("#ddlBillingMethod").val();

                $('#gridTopupReport').jqGrid('GridUnload');

                $('#btnExportReport').hide();
                $('.TopupRptFooter').hide();

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

                $("#gridTopupReport").jqGrid({
                    url: '<%= ResolveUrl("~/Report/GetFinanceReportForAdminPortal/")%>',
                    datatype: 'json',
                    mtype: 'POST',
                    postData: { "sStartDate": sStartDate, "sEndDate": sEndDate, "iBillingMethodID": iBillingMethodID },
                    colNames: ['Customer', 'SMS Account', 'Date & Time', 'Topup Amount', 'Method', 'OA Export Date'],
                    colModel:
                    [
                        { name: 'CustomerName', index: 'CustomerName', width: '25%', sortable: true, search: false },
                        { name: 'MPAccountName', index: 'MPAccountName', width: '25%', sortable: true, search: false },
                        { name: 'TransactionDate', index: 'TransactionDate', width: '15%', align: 'center', sortable: true, search: false },
                        { name: 'Amount', index: 'Amount', align: 'right', width: '15%', sortable: true, search: false },
                        { name: 'BillingMethod', index: 'BillingMethod', width: '10%', align: 'center', sortable: true, search: false },
                        { name: 'OAExportDate', index: 'OAExportDate', width: '10%', align: 'center', sortable: true, search: false }
                    ],
                    pager: '#TopupPager',
                    toppager: true,
                    rowNum: 10,
                    rowList: [10, 20, 50],
                    height: '100%',
                    width: '870',
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

                $('#gridUsageReport').jqGrid('GridUnload');

                $('#btnExportReport').hide();
                $('.UsageRptFooter').hide();

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

                if (!checkStartEndDate(sStartDate, sEndDate))
                {
                    alert('Start date must be smaller than or equal to End date.');
                    return false;
                }

                $('#hdnStartDate').val(sStartDate);
                $('#hdnEndDate').val(sEndDate);

                $("#gridUsageReport").jqGrid({
                    url: '<%= ResolveUrl("~/Report/GetUsagePerDayReportForAdminPortal/")%>',
                    datatype: 'json',
                    mtype: 'GET',
                    postData: { "sStartDate": sStartDate, "sEndDate": sEndDate },
                    colNames: ['Date', 'Messages Sent', 'Amount'],
                    colModel:
                    [
                        { name: 'Date', index: 'Date', align: 'center', width: '50%', sortable: true, search: false },
                        { name: 'TotalMessagesSentPerDay', index: 'TotalMessagesSentPerDay', width: '20%', sortable: true, search: false },
                        { name: 'TotalNetAmountPerDay', index: 'TotalNetAmountPerDay', width: '30%', align: 'right', sortable: true, search: false }
                    ],
                    pager: '#UsagePager',
                    toppager: true,
                    rowNum: 10,
                    rowList: [10, 20, 50],
                    viewrecords: true,
                    hidegrid: false,
                    height: '100%',
                    width: '870',
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

            function ExportTopupReportCSV() {
                //1. Read variables 
                var iBillingMethodID = $("#ddlBillingMethod").val();
                var sStartDate = $.trim($("#txtStartDate").val());
                var sEndDate = $.trim($("#txtEndDate").val());

                //2. Construct URL
                var DownloadURL = "<%= ResolveUrl("~/Report/GetFinanceReportForExport?sStartDate=")%>" + sStartDate + "&sEndDate=" + sEndDate + "&iBillingMethodID=" + iBillingMethodID

                //3. Alert URL
                //Example
                //window.location.href = "/Report/GetFinanceReportForExport?sStartDate=01/12/2013&sEndDate=12/12/2013&iBillingMethodID=0";

                //window.location.href = DownloadURL;                
                window.open(DownloadURL);
            }

            function ExportUsagePerDayReportCSV() {
                //1. Read variables 
                var sStartDate = $.trim($("#txtStartDate").val());
                var sEndDate = $.trim($("#txtEndDate").val());

                //2. Construct URL
                var DownloadURL = "<%= ResolveUrl("~/Report/GetUsagePerDayReportForExport?sStartDate=")%>" + sStartDate + "&sEndDate=" + sEndDate;

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
    
    
<form action="<%= ResolveUrl("~/Report/GetReportForExport")%>" method="post">
    <table align="center">
        <tr>
            <td nowrap style="vertical-align: top;">
                <input type="radio" id="chkTopupReport" value="1" name="AdminTopLevelReport" checked="checked" class="StatusCheckbox" />Finance report
            </td>
            <td style="width: 30%"></td>
            <td nowrap>
                <input type="radio" id="chkUsageReport" value="2" name="AdminTopLevelReport" class="StatusCheckbox" />Usage per day report
            </td>
        </tr>
    </table>

    <table style="width: 800px;">
        <tr>
            <td style="vertical-align: top">
                <table style="width:810px;">
                    <tr>
                        <td style="width:30px; vertical-align:top; padding-top:8px;">Period&nbsp;
                        </td>
                        <td style="width:200px;">
                            <input type="text" class="datepicker" id="txtStartDate" disabled="disabled" />&nbsp;
                            <input type="hidden" id="hdnStartDate" name="sStartDate" />
                        </td>
                        <td style="width:10px;  vertical-align:top; padding-top:8px;">to&nbsp;
                        </td>
                        <td style="width:200px;">
                            <input type="text" class="datepicker" id="txtEndDate" disabled="disabled" />&nbsp;&nbsp;
                            <input type="hidden" id="hdnEndDate" name="sEndDate" />
                        </td>
                        <td style=" text-align: left; vertical-align:top; padding-top:8px;" nowrap>
                            <label id="lblTopupType">Topup Type&nbsp;</label>

                            <input type="button" id="btnRunUsageReport" value="Run Report" onclick="LoadUsageDetails()" />
                            &nbsp;
                        </td>
                        <td style="width: 150px; vertical-align:top; padding-top:8px;">
                            <select id="ddlBillingMethod" name="BillingMethod" style="width: 150px;">
                                <option value="0">All</option>
                                <option value="1">Paypal</option>
                                <option value="2">Invoice</option>
                            </select>
                        </td>
                        <td style=" vertical-align:top; padding-top:8px; text-align: left;">
                            <input type="button" id="btnRunTopupReport" value="Run Report" onclick="LoadTopupDetails()" />
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
                    <tr class="TopupRptFooter" style="font-size:16px;">
                        <td>
                            <span style="font-family: Calibri;">Total:&nbsp;&nbsp;</span><label id="lblTotal_TopupAmount"></label>
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
                    <tr class="UsageRptFooter" style="font-size:16px;">
                        <td>
                            <span style="font-family: Calibri;">Total Messages sent:&nbsp;&nbsp;</span><label id="lblTotal_MessageSent"></label>
                        </td>
                    </tr>
                    <tr class="UsageRptFooter" style="font-size:16px;">
                        <td>
                            <span style="font-family: Calibri;">Total cost:&nbsp;&nbsp;</span><label id="lblTotal_Cost"></label>
                        </td>
                    </tr>                    
                <%--    <tr class="UsageRptFooter">
                        <td>
                            <input type="button" id="btnExportUsageReportCSV" value="Export Report" onclick="ExportUsagePerDayReportCSV()"/>
                        </td>
                    </tr>--%>
                </table>
            </td>
        </tr>
        <tr>
            <td>
                <input type="submit" value="Export Report"  id="btnExportReport"/>
            </td>
        </tr>
    </table>
    </form>

</asp:Content>
