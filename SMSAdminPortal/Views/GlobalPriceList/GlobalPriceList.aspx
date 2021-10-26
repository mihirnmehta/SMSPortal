<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/MainMaster.Master" Inherits="System.Web.Mvc.ViewPage<dynamic>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
    Global Price List
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <asp:PlaceHolder ID="phScripts" runat="server">

        <script type="text/javascript">

            var bHasPostPayAccess  = HasAddPostPayAccess();
            var bHasAddTopupAccess = HasAddTopupAccess();
            var bHasReadOnlyAccess = HasReadOnlyAccess();

            $(document).ready(function () {
                LoadGlobalPriceList();

                if (bHasPostPayAccess || bHasAddTopupAccess || bHasReadOnlyAccess) {
                    $('#btnAdd').addClass('ui-state-disabled');
                    $("#btnDelete").addClass('ui-state-disabled');
                }
            });

            function LoadGlobalPriceList() {

                var myFloatTemplate = { width: 80, align: "right", sorttype: "float" };

                $("#gridPriceList").jqGrid({
                    url: '<%= ResolveUrl("~/GlobalPriceList/GetGlobalPriceList/")%>',
                    datatype: 'json',
                    mtype: 'GET',
                    colNames: ['', 'Price Per SMS', 'Banding'],
                    colModel: [
                        { name: 'TierID', key: true, hidden: true, index: 'TierID', search: false, editable: false },
                        { name: 'PricePerSMS', index: 'PricePerSMS', width: '50%', formatter: getEditLink, search: false, editable: false },
                        { name: 'Banding', index: 'Band', width: '50%', search: false }
                    ],
                    pager: '#pager',
                    rownumbers: true,
                    viewrecords: true,
                    hidegrid: false,
                    height: '100%',
                    width: 500,
                    gridComplete: function () {

                        $('a.UpdateClass').click(function () {

                            var iTierID = $(this).attr('iTierID');
                            ShowAddPriceListDialog(iTierID);

                        });
                    },
                    pgbuttons: false,
                    pginput: false
                }).navGrid('#pager', { edit: false, add: false, del: false, search: false },
                {}, // settings for edit
                {}, // add
                {}, // delete
                {}, // search options
                {}
                ).navButtonAdd('#pager', {
                    id: "btnDelete",
                    caption: "Delete",
                    title: "Click to delete.",
                    buttonicon: "ui-icon ui-icon-trash",
                    onClickButton: function () {
                        DeletePriceList();
                    }, //OnClick
                    position: "first"
                }).navButtonAdd('#pager', {
                    id: "btnAdd",
                    caption: "Add",
                    title: "Click to add.",
                    buttonicon: "ui-icon-circle-plus",
                    onClickButton: function () {
                        ShowAddPriceListDialog(0);
                    }, //OnClick
                    position: "first"
                });; //ends navGrid

            } //Ends function

            function ShowAddPriceListDialog(iTierID) {

                var dlgtitle = "";
                if (iTierID == 0)
                    dlgtitle = "Add Price List";
                else
                    dlgtitle = "Edit Price List";

                $.ajax({
                    url: '<%= ResolveUrl("~/GlobalPriceList/ShowAddEditPLDialog") %>',
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

    if (bHasPostPayAccess || bHasAddTopupAccess || bHasReadOnlyAccess) {
        return opts[1];
    }
    return '<a id="UpdatePL' + opts[0] + '" class="UpdateClass" href="#" iTierID="' + opts[0] + '" > ' + opts[1] + '</a>';
}

function DeletePriceList() {
    var selectedRow = jQuery('#gridPriceList').getGridParam('selrow');

    if (selectedRow == null)
        alert('Please select a record to delete.');
    else {

        if (confirm("Are you sure want to delete the Global Price List Tier? ")) {
            ShowLoadingBox();
            $.ajax({
                url: '<%= ResolveUrl("~/GlobalPriceList/DeletePriceList") %>',
                data: { "iPriceListID": selectedRow },
                type: "POST",
                cache: false,
                timeout: 6000000,
                async: true,
                success: function (result) {
                    HideLoadingBox();
                    if (result == "true") {
                        alert("The Price List Tier has been deleted.");
                        $("#gridPriceList").trigger("reloadGrid");
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
        } //ends Confirm            
    }// If row is selected
}

</script>

    </asp:PlaceHolder>
    <table>
        <tr>
            <td style="font-family: Calibri; font-size:large" >Global Price List</td>
        </tr> 
    </table>
     <br />
    <table id="gridPriceList" class="scroll">
        
    </table>
    <div id="pager" class="scroll" style="text-align: center;"></div>
    <br />

    <div id='dlgPriceList' style="display: none;">
        <input type="hidden" value="1" id="hdnUpdatePLID" />
    </div>

    <div class="modal">
        <div id="LoadingText">Loading</div>
    </div>
</asp:Content>
