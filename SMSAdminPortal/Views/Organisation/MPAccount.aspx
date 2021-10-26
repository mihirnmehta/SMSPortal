<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Organisation/OrganisationDetails.master" Inherits="System.Web.Mvc.ViewPage<dynamic>" %>

<asp:Content ID="MPAccountTitle" ContentPlaceHolderID="OrgTitleContent" runat="server">
    Micropayment Accounts
</asp:Content>

<asp:Content ID="MPAccountMainContent" ContentPlaceHolderID="OrgMainContent" runat="server">

    <asp:PlaceHolder ID="phScripts" runat="server">

        <script>

            var bHasReadOnlyAccess = HasReadOnlyAccess();

            $(document).ready(function () {

                var iOrgID = $('#hdnOrgID').val();

                LoadMPAccountsList();

                if (bHasReadOnlyAccess) {
                    $('#btnAdd').addClass('ui-state-disabled');
                }
            });

            function LoadMPAccountsList() {

                $("#gridMPAccountList").jqGrid({
                    url: '<%= ResolveUrl("~/Organisation/GetMPAccountsByOrganisationID")%>',
                    datatype: 'json',
                    mtype: 'GET',
                    colNames: ['', 'Account Name', 'Balance', 'Low Balance Warning', 'Low Balance WarningLevel', 'Payment Methods', 'Enabled'],
                    colModel: [
                        { name: 'AccountID', key: true, hidden: true, index: 'AccountID', align: 'center', search: false, editable: false },
                        { name: 'AccountName', index: 'Description', formatter: getEditLink, editable: false, searchoptions: { sopt: ['cn', 'eq'] }, searchrules: { required: true } },
                        { name: 'Balance', index: 'Balance', search: false },
                        { name: 'LowBalanceWarning', index: 'SendLowBalanceWarnings', align: 'center', search: false },
                        { name: 'LowBalanceWarningLevel', index: 'LowBalanceWarningLevel', align: 'center', search: false, sortable: false },
                        { name: 'PaymentMethod', index: 'PaymentMethod', align: 'left', search: false, sortable: false },
                        { name: 'Enabled', index: 'IsEnabled', align: 'center', search: false }
                    ],
                    shrinkToFit:true,
                    pager: '#mpAccountPager',
                    rowNum: 10,
                    rowList: [10, 20, 50],
                    viewrecords: true,
                    hidegrid: false,
                    height: '100%',
                    align: 'center',
                    gridComplete: function () {
                        $('a.UpdateClass').click(function () {
                            var iAccountID = $(this).attr('iAccountID');
                            ShowAddMPAccountDialog(iAccountID);
                        });
                    },
                    pgbuttons: true,
                    onPaging: function (pgButton) {

                        if ("user" == pgButton) {
                            var requestedPage = $("#gridMPAccountList").getGridParam("page");
                            var lastPage = $("#gridMPAccountList").getGridParam("lastpage");
                            if (eval(requestedPage) > eval(lastPage)) {
                                $("#gridMPAccountList").setGridParam({ page: lastPage });
                            }

                        } else if ("records" == pgButton) {

                            var requestedPage = $("#gridMPAccountList").getGridParam("page");
                            var totalRows = $("#gridMPAccountList").getGridParam("records");
                            var rowsPerPage = $("#gridMPAccountList").getGridParam("rowNum");
                            var lastPage = Math.ceil(eval(totalRows) / eval(rowsPerPage));
                            if (eval(requestedPage) > lastPage) {
                                $("#gridMPAccountList").setGridParam({ page: lastPage });
                            }
                        }
                    }, //end onPaging
                    pginput: true
                }).navGrid('#mpAccountPager', {
                    edit: false, add: false, del: false, search: true, searchtext: "Search",
                    searchtitle: "Search MP Account"
                },
            {}, // settings for edit
            {}, // add
            {}, // delete
            {
                caption: "Search MP Account",
                Find: "Search",
                searchOnEnter: true,
                modal: true,
                closeAfterSearch: true,
                closeOnEscape: true,
                width: "350",
                onReset: function () {
                    var jqModal = true;
                    $.jgrid.hideModal("#searchmodfbox_" + this.id,
                        { gb: "#gbox_" + this.id, jqm: jqModal, onClose: null });
                }
            }, // search options
            {}
            ).navButtonAdd('#mpAccountPager', {
                        id: "btnAdd",
                        caption: "Add",
                        title: "Click to add new MPAccount",
                        buttonicon: "ui-icon-circle-plus",
                        onClickButton: function () {
                            ShowAddMPAccountDialog(0);
                        }, //OnClick
                        position: "first"
                    });; //ends navGrid
            }


            function ShowAddMPAccountDialog(iAccountID) {

                var dlgtitle = "";
                if (iAccountID == 0)
                    dlgtitle = "Add Micropayment Account";
                else
                    dlgtitle = "Edit Micropayment Account";

                $.ajax({
                    url: '<%= ResolveUrl("~/Organisation/ShowAddMPAccountDialog") %>',
                    data: { "iAccountID": iAccountID },
                    type: "POST",
                    dataType: "html",
                    cache: false,
                    success: function (data) {
                        $('#dlgMPAccount').html(data); //write the dialog content into the dialog container
                        $("#dlgMPAccount").dialog({
                            autoOpen: false, modal: true, title: dlgtitle, resizable: false, height: 480, width: 800});
                        $("#dlgMPAccount").dialog("open"); //open it!

                    }
                });
        }

        function getEditLink(el, cellval, opts) {
            if (bHasReadOnlyAccess) {
                return opts[1];
            }
            return '<a id="UpdateMPAccount' + opts[0] + '" class="UpdateClass" href="#" iAccountID="' + opts[0] + '" > ' + opts[1] + '</a>';
        }


        //-------------------------------------------Function to Test Import Invoice------------------------------------------------
        function ImportInvoices()
        {            
            window.location.href = "<%= ResolveUrl("~/Organisation/ImportInvoices")%>";
        }
        //-------------------------------------------Function to Test Import Invoice------------------------------------------------

        </script>

    </asp:PlaceHolder>

    
     <table>
        <tr>
            <td style="font-family: Calibri; font-size:large" >Micropayment Accounts</td>
        </tr>        
    </table>
    <table>
        <tr>
            <td style="vertical-align: top">
                <table id="gridMPAccountList" class="scroll"></table>
                <div id="mpAccountPager" class="scroll" style="text-align: center;"></div>

                <input type="button" id="btnImportInvoice" value="Generate Invoices" onclick="ImportInvoices()" />
            </td>
        </tr>
        <tr>
            <td>
                <input type="hidden" id="hdnOrgID" value="2" /></td>
        </tr>
    </table>
  <%--  <div id="confirmdialog" title="Confirmation Required" style="display: none;">
        Are you sure? 
    </div>--%>

    <div id='dlgMPAccount' style="display: none;">
        
    </div>
</asp:Content>
