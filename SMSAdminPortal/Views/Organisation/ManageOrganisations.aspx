<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/MainMaster.Master" Inherits="System.Web.Mvc.ViewPage<dynamic>" %>

<asp:Content ID="Content2" ContentPlaceHolderID="TitleContent" runat="server">Manage Organisations</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="SubContent" runat="server">
</asp:Content>
<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">

    <asp:PlaceHolder ID="phScripts" runat="server">

        <script type="text/javascript">

            $(document).ready(function () {

                $('#ddlCompanies').append($("<option></option>").attr("value", "loading").text("Loading.."));

                $.ajax({
                    url: '<%= ResolveUrl("~/Organisation/GetListOfCompanies") %>',
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
                            $('#ddlCompanies').append($("<option></option>").attr("value", "").text("Select Company"));
                        }
                    },
                    error: function (httpRequest, msg) {
                        $('#errorMsg').show().text('An error occurred while retrieving Company list. Please try again.');
                    }
                }); //ends ajax


                $("#ddlCompanies").change(function () {

                    //var index = $("select[name='Organisation'] option:selected").index();
                    var index = $("option:selected", this).index();

                    if (index == 0) {
                        $('#gridOrganisations').jqGrid('GridUnload');
                        $('#gridOrganisations').hide();
                        return;
                    }

                    $('#gridOrganisations').show();

                    var CompanyID = $("#ddlCompanies").val();
                    LoadOrganisations(CompanyID);

                    bHasReadOnlyAccess = HasReadOnlyAccess();

                    if (bHasReadOnlyAccess) {
                        $('#btnAdd').addClass('ui-state-disabled');
                    }
                });

            });

            function LoadOrganisations(CompanyID) {
                $('#gridOrganisations').jqGrid('GridUnload');

                $("#gridOrganisations").jqGrid({
                    url: '<%= ResolveUrl("~/Organisation/GetOrganisations/")%>',
                    postData: { "CompanyID": CompanyID },
                    datatype: 'json',
                    mtype: 'GET',
                    colNames: ['ID', 'Organisation', 'Open Account ID', 'Micropayment Accounts', 'Custom Price List', 'PayPal', 'Invoice'],
                    colModel: [
                        { name: 'OrganisationID', key: true, hidden: true, index: 'OrganisationID', align: 'center', search: false, editable: false },
                        { name: 'Name', index: 'OrganisationName', formatter: getEditLink, width: '25%', align: 'left', editable: false, searchoptions: { sopt: ['cn', 'eq'] }, searchrules: { required: true } },
                        { name: 'OpenAccountID', index: 'OrgOpenAccountID', width: '17%', align: 'center', editable: false, searchoptions: { sopt: ['cn', 'eq'] }, searchrules: { required: true, integer: true } },
                        { name: 'MPAccounts', index: 'MPActCount', width: '20%', align: 'center', search: false, editable: false },
                        { name: 'Custom Price List', index: 'CustomPLExist', width: '18%', align: 'center', search: false, editable: false },
                        { name: 'PayPal', index: 'PayPal', width: '10%', align: 'center', search: false, editable: false },
                        { name: 'Invoice', index: 'Invoice', width: '10%', align: 'center', search: false, editable: false }
                    ],
                    pager: '#pager',
                    //toppager: true,
                    rowNum: 10,
                    rowList: [10, 20, 50],
                    viewrecords: true,
                    //caption: 'Organisations(Already setup)',
                    hidegrid: false,
                    height: '100%',
                    width: 870,
                    gridComplete: function () {
                        $('a.UpdateClass').click(function () {
                            var iOrgID = $(this).attr('iOrganisationID');
                            window.location.href = "<%= ResolveUrl("~/Organisation/OrganisationDetails?iOrganisationID=")%>" + iOrgID;
                            //$.post("/Organisation/OrganisationDetails", { "iOrganisationID": iOrgID });                            
                        });
                    },
                    pgbuttons: true,
                    onPaging: function (pgButton) {

                        if ("user" == pgButton) {
                            var requestedPage = $("#gridOrganisations").getGridParam("page");
                            var lastPage = $("#gridOrganisations").getGridParam("lastpage");
                            if (eval(requestedPage) > eval(lastPage)) {
                                $("#gridOrganisations").setGridParam({ page: lastPage });
                            }
                        } else if ("records" == pgButton) {

                            var requestedPage = $("#gridOrganisations").getGridParam("page");
                            var totalRows = $("#gridOrganisations").getGridParam("records");
                            var rowsPerPage = $("#gridOrganisations").getGridParam("rowNum");
                            var lastPage = Math.ceil(eval(totalRows) / eval(rowsPerPage));
                            if (eval(requestedPage) > lastPage) {
                                $("#gridOrganisations").setGridParam({ page: lastPage });
                            }
                        }
                    }, //end onPaging
                    pginput: true
                }).navGrid('#pager', {
                    edit: false, add: false, del: false, search: true, searchtext: "Search",
                    searchtitle: "Search Organisations"
                },
            {}, // settings for edit
            {}, // add
            {}, // delete
            {
                caption: "Search Organisations",
                Find: "Search",
                searchOnEnter: true,
                modal: true,
                closeAfterSearch: true,
                //closeAfterReset: true,
                closeOnEscape: true,
                width: "350",
                onReset: function () {
                    var jqModal = true;
                    $.jgrid.hideModal("#searchmodfbox_" + this.id,
                        { gb: "#gbox_" + this.id, jqm: jqModal, onClose: null });
                }

            }, // search options
            {}
            ).navButtonAdd('#pager', {
                id: "btnAdd",
                caption: "Add",
                title: "Click to set up a new Organisation",
                buttonicon: "ui-icon-circle-plus",
                onClickButton: function () {
                    ShowAddOrganisationDialog(0);
                }, //OnClick
                position: "first"
            });; //ends navGrid
                }


                function getEditLink(el, cellval, opts) //This function converts the Organisation Name column to link
                {
                    return '<a id="UpdateOrganisation' + opts[0] + '" class="UpdateClass" href="#" iOrganisationID="' + opts[0] + '" > ' + opts[1] + '</a>';
                }

                function ShowAddOrganisationDialog(iOrganisationID) // This function show the Add / Edit Popup
                {
                    var CompanyID = $("#ddlCompanies").val();

                    $.ajax({
                        url: '<%= ResolveUrl("~/Organisation/ShowAddOrganisationDialog") %>',
                    data: { "CompanyID": CompanyID, "iOrganisationID": iOrganisationID },
                    type: "POST",
                    dataType: "html",
                    cache: false,
                    success: function (data) {
                        $('#popupOrganisation').html(data); //write the dialog content into the diaog container
                        $("#popupOrganisation").dialog({ autoOpen: false, modal: true, title: "Setup Organisations", resizable: false, height: 290, width: 550 });
                        $("#popupOrganisation").dialog("open"); //open it!
                    }
                });

            }

        </script>

    </asp:PlaceHolder>


    <style type="text/css">
        .ui-widget-content td
        {
            white-space: normal !important;
        }
    </style>

    <!--  
<style type="text/css">
    .ui-jqgrid .ui-jqgrid-htable th div
    {
        height: auto;
        overflow: hidden;
        padding-right: 4px;
        padding-top: 2px;
        position: relative;
        vertical-align: text-top;
        white-space: normal !important;
    }
</style> -->

    <table>
        <tr>

            <td style="font-family: Calibri; font-size: large">Manage Organisations</td>
        </tr>
    </table>

    <br />

    <table style="width: 800px;">
        <tr>
            <td style="text-align: center;">
                <select id="ddlCompanies" name="Companies" style="width: 300px;" /></td>
        </tr>
        <tr>
            <td style="height: 10px;"></td>
        </tr>
        <tr>
            <td>
                <table id="gridOrganisations" class="scroll"></table>
                <div id="pager" class="scroll" style="text-align: center;"></div>
            </td>
        </tr>
    </table>

    <div id='popupOrganisation' style="display: none;">
    </div>

</asp:Content>
