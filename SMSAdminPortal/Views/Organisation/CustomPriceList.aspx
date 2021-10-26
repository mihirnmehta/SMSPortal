<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Organisation/OrganisationDetails.master" Inherits="System.Web.Mvc.ViewPage<dynamic>" %>

<asp:Content ID="CustomPriceListTitle" ContentPlaceHolderID="OrgTitleContent" runat="server">
    Custom Price List
</asp:Content>

<asp:Content ID="CustomPriceListMainContent" ContentPlaceHolderID="OrgMainContent" runat="server">

    <asp:PlaceHolder ID="phScripts" runat="server">

        <script type="text/javascript">
            var bHasReadOnlyAccess = HasReadOnlyAccess();

            $(document).ready(function () {

                LoadGlobalPriceList();

                LoadCustomPriceList();
                if (bHasReadOnlyAccess) {
                    $('#btnAdd').addClass('ui-state-disabled');
                    $("#btnDelete").addClass('ui-state-disabled');
                }

            });

            function LoadCustomPriceList() {

                $("#gridCustomPriceList").jqGrid({
                    url: '<%= ResolveUrl("~/Organisation/GetCustomPriceList/")%>',
                    //postData: { iOrganisationID : iOrgID },
                    datatype: 'json',
                    mtype: 'GET',
                    colNames: ['', 'Price per SMS', 'Banding'],
                    colModel: [
                        { name: 'TierID', key: true, hidden: true, index: 'TierID', search: false, editable: false },
                        { name: 'PricePerSMS', index: 'PricePerSMS', formatter: getEditLink, search: false, editable: false },
                        { name: 'Band', index: 'Band', search: false, sortable: true }
                    ],
                    pager: '#customPager',
                    rowNum: 10,
                    viewrecords: true,
                    //caption: 'Custom Price List',
                    hidegrid: false,
                    height: '100%',
                    width: '450',
                    align: 'center',
                    rownumbers: true,
                    gridComplete: function () {

                        $('a.UpdateClass').click(function () {

                            var iTierID = $(this).attr('iTierID');
                            ShowAddPriceListDialog(iTierID);

                        });
                    },
                    pgbuttons: false,                    
                    pginput: false
                }).navGrid('#customPager', { edit: false, add: false, del: false, search: false },
            {}, // settings for edit
            {}, // add
            {}, // delete
            {}, // search options
            {}
            ).navButtonAdd('#customPager', {
                id: "btnDelete",
                caption: "Delete",
                title: "Click to delete a Pricelist",
                buttonicon: "ui-icon ui-icon-trash",
                onClickButton: function () {
                    DeletePriceList();
                }, //OnClick
                position: "first"
            }).navButtonAdd('#customPager', {
                id: "btnAdd",
                caption: "Add",
                title: "Click to add new Pricelist",
                buttonicon: "ui-icon-circle-plus",
                onClickButton: function () {
                    ShowAddPriceListDialog(0);
                }, //OnClick
                position: "first"
            });; //ends navGrid
            }


    function ShowAddPriceListDialog(iTierID)
    {
        var dlgtitle = "";
        if (iTierID == 0)
            dlgtitle = "Add Price List";
        else
            dlgtitle = "Edit Price List";

        $.ajax({
            url: '<%= ResolveUrl("~/Organisation/ShowAddEditPLDialog") %>',
            data: { "iTierID": iTierID },
            type: "POST",
            dataType: "html",
            cache: false,
            success: function (data) {
                $('#dlgPriceList').html(data); //write the dialog content into the dialog container
                $("#dlgPriceList").dialog({
                    autoOpen: false, modal: true, title: dlgtitle, resizable: false, height: 225, width: 310});
                $("#dlgPriceList").dialog("open"); //open it!
            }
        });
    }


            function getEditLink(el, cellval, opts) {
                if (bHasReadOnlyAccess) {
                    return opts[1];
                }
            return '<a id="UpdatePL' + opts[0] + '" class="UpdateClass" href="#" iTierID="' + opts[0] + '" > ' + opts[1] + '</a>';
        }

        function DeletePriceList() {
            var selectedRow = jQuery('#gridCustomPriceList').getGridParam('selrow');

            if (selectedRow == null)
                alert('Please select a record to delete.');
            else {

                if (confirm("Are you sure want to delete the Custom Tier? ")) {

                    ShowLoadingBox();
                    $.ajax({
                        url: '<%= ResolveUrl("~/Organisation/DeleteCustomTier") %>',
                        data: { "iTierID": selectedRow },
                        type: "POST",
                        cache: false,
                        timeout: 6000000,
                        async: true,
                        success: function (result) {

                            HideLoadingBox();
                            if (result == "true") {
                                alert("The Custom Tier has been deleted.");
                                $("#gridCustomPriceList").trigger("reloadGrid");
                            }
                            else {
                                alert('Error occurred while deleting Tier. Please try again.');
                            }
                        },
                        error: function (httpRequest, msg) {
                            HideLoadingBox();
                            alert('Error occurred while deleting Tier. Please try again.');
                        }

                    });     //ends ajax  
                } // ends Confirm

            }// If row is selected
        }

        function LoadGlobalPriceList() {
            $("#gridGlobalPriceList").jqGrid({
                url: '<%= ResolveUrl("~/GlobalPriceList/GetGlobalPriceList/")%>',
                    datatype: 'json',
                    mtype: 'GET',
                    colNames: ['', 'Price per SMS', 'Banding'],
                    colModel: [
                        { name: 'TierID', key: true, hidden: true, index: 'TierID', search: false, editable: false },
                        { name: 'PricePerSMS', index: 'PricePerSMS', search: false, editable: false },
                        { name: 'Banding', index: 'Band', search: false, sortable: true }
                    ],
                    pager: '#globalPager',
                    rowNum: 10,
                    rownumbers: true,
                    viewrecords: true,
                    //caption: 'Global Price List',
                    hidegrid: false,
                    height: '100%',
                    width: '30%',
                    align: 'center',
                    pgbuttons: false,
                    pginput: false
                }).navGrid('#globalPager', { edit: false, add: false, del: false, search: false },
            {}, // settings for edit
            {}, // add
            {}, // delete
            {}, // search options
            {}
            )
            }
        </script>
    </asp:PlaceHolder>
      
     <table>
        <tr>
            <td style="font-family: Calibri; font-size:large" >Organisation Price List Details</td>
        </tr>        
    </table>    
    <table>
        <tr>
            <td style="vertical-align: top">
                <table id="gridGlobalPriceList" class="scroll"></table>
                <div id="globalPager" class="scroll" style="text-align: center;"></div>
            </td>
            <td style="width: 10px"></td>
            <td style="vertical-align: top">
                <table id="gridCustomPriceList" class="scroll"></table>
                <div id="customPager" class="scroll" style="text-align: center;"></div>
            </td>
        </tr>
    </table>
   
    <div id='dlgPriceList' style="display: none;">
        <input type="hidden" value="1" id="hdnUpdatePLID" />
    </div>
    
    <div class="modal">
        <div id="LoadingText">Loading</div>
    </div>
</asp:Content>
