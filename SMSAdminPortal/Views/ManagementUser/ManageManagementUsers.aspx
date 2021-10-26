<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/MainMaster.Master" Inherits="System.Web.Mvc.ViewPage<dynamic>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
    Management User
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

    <asp:PlaceHolder ID="phScripts" runat="server">

        <script type="text/javascript">

            var bHasReadOnlyAccess   = false;
            var bHasAddTopupAccess   = false;
            var bHasAddPostPayAccess = false;
            var LoginID;
            $(document).ready(function () {

                bHasReadOnlyAccess   = HasReadOnlyAccess();
                bHasAddTopupAccess   = HasAddTopupAccess();
                bHasAddPostPayAccess = HasAddPostPayAccess();

                LoginID = '<%= ViewData["LoginID"]%>';

                LoadManagementUser();

                if (bHasReadOnlyAccess || bHasAddTopupAccess || bHasAddPostPayAccess) {
                    $('#btnAdd').addClass('ui-state-disabled');
                    $('#btnDelete').addClass('ui-state-disabled');
                    jQuery("#gridManagementUsers").jqGrid().hideCol("Password").trigger("reloadGrid");
                }
            });


            function LoadManagementUser() {
                $("#gridManagementUsers").jqGrid({
                    url: '<%= ResolveUrl("~/ManagementUser/GetManagementUser/")%>',
                    datatype: 'json',
                    mtype: 'GET',

                    colNames: ['', 'Name', 'Email', 'Phone Number', 'Access Level', 'Reset Password'],
                    colModel: [
                        { name: 'ManagementUserID', key: true, hidden: true, index: 'ManagementUserID', align: 'center', search: false, editable: false }, 
                        { name: 'Forename', index: 'Forename', width: '22%', align: 'left', editable: false, formatter: getEditLink, searchoptions: { sopt: ['cn', 'eq'] }, searchrules: { required: true } },
                        { name: 'Email', index: 'Email', width: '35%', align: 'left', sortable: true, searchoptions: { sopt: ['cn', 'eq'] }, searchrules: { required: true } },
                        { name: 'PhoneNumber', index: 'PhoneNumber', width: '13%', align: 'center', sortable: false, searchoptions: { sopt: ['cn', 'eq'] }, searchrules: { required: true } },
                        { name: 'AccessLevelID', index: 'AccessLevel', width: '17%', align: 'left', search: false, sortable: true },
                        { name: 'Password', index: '', width: '13%', align: 'center', search: false, formatter: GetGeneratePasswordLink, sortable: false }
                    ],
                    pager: '#managePager',
                    rowNum: 10,
                    rowList: [10, 20, 50],
                    viewrecords: true,
                    //caption: 'Management Users',
                    hidegrid: false,
                    height: '100%',
                    width: 870,
                    align: 'center',
                    rownumbers: false,

                    gridComplete: function () {

                        $('a.UpdateClass').click(function () {

                            var iManagementUserID = $(this).attr('iManagementUserID');
                            ShowAddManagementUsersDialog(iManagementUserID);

                        });
                        $('img.SendPassword').click(function () {
                            var strEmail = $(this).attr('strEmail');
                            ResetPassword(strEmail);

                        });
                    },

                    pgbuttons: true,
                    onPaging: function (pgButton) {

                        if ("user" == pgButton) {
                            var requestedPage = $("#gridManagementUsers").getGridParam("page");
                            var lastPage = $("#gridManagementUsers").getGridParam("lastpage");
                            if (eval(requestedPage) > eval(lastPage)) {
                                $("#gridManagementUsers").setGridParam({ page: lastPage });
                            }

                        } else if ("records" == pgButton) {

                            var requestedPage = $("#gridManagementUsers").getGridParam("page");
                            var totalRows = $("#gridManagementUsers").getGridParam("records");
                            var rowsPerPage = $("#gridManagementUsers").getGridParam("rowNum");
                            var lastPage = Math.ceil(eval(totalRows) / eval(rowsPerPage));
                            if (eval(requestedPage) > lastPage) {
                                $("#gridManagementUsers").setGridParam({ page: lastPage });
                            }
                        }
                    }, //end onPaging
                    pginput: true
                }).navGrid('#managePager', {
                    edit: false, add: false, del: false, search: true, searchtext: "Search",

                    searchtitle: "Click to search."

                },
            {}, // settings for edit
            {}, // add
            {}, // delete
             {
                 caption: "Search Management Users",
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
            ).navButtonAdd('#managePager', {
                id: "btnDelete",
                caption: "Delete",
                title: "Click to delete.",
                buttonicon: "ui-icon ui-icon-trash",
                onClickButton: function () {
                    DeleteManagementUser();

                }, //OnClick
                position: "first"
            }).navButtonAdd('#managePager', {
                id:"btnAdd",
                caption: "Add",
                title: "Click to add.",
                buttonicon: "ui-icon-circle-plus",
                onClickButton: function () {
                    ShowAddManagementUsersDialog(0);

                }, //OnClick
                position: "first"
            });;
            }
                

    function ResetPassword(strEmail) {
        ShowLoadingBox();
        $.ajax({
            url: '<%= ResolveUrl("~/ManagementUser/ResetPassword") %>',
            data: { "strEmail": strEmail },
            type: "POST",
            datatype:'json',           
            cache: false,
            success: function (result) {
                HideLoadingBox();
                
                if (result) {
                    alert("The password has been reset.");
                }
                else {
                    alert('An error occurred while resetting the password. Please try again.');
                }
            },
            error: function (xhr, ajaxOptions, thrownError) {
                HideLoadingBox();
                alert(thrownError);
            }
        });
}



    function ShowAddManagementUsersDialog(iManagementUserID) {
        var dlgtitle = "";
        if (iManagementUserID == 0)
            dlgtitle = "Add Management User";
        else
            dlgtitle = "Edit Management User";

    $.ajax({
        url: '<%= ResolveUrl("~/ManagementUser/ShowAddEditManagementUsersDialog") %>',
            data: { "iManagementUserID": iManagementUserID },
            type: "POST",
            dataType: "html",
            cache: false,
            success: function (data) {
                $('#dlgManageUser').html(data); //write the dialog content into the dialog container
                $("#dlgManageUser").dialog({autoOpen: false, modal: true, title: dlgtitle, resizable: false, height: 300, width: 470});
                $("#dlgManageUser").dialog("open"); //open it!

            },
            error: function (xhr, ajaxOptions, thrownError) {
                alert(thrownError);
            }
        });
    }


    function GetGeneratePasswordLink(el, cellval, opts) {
        return '<img src="<%= ResolveUrl("~/Content/Images/lock.png")%>"  height="20px" width="20px" alt="my image" id="SendPassword' + opts[0] + '"class="SendPassword" href="#" strEmail="' + opts[2] + '" />';
    }

    function getEditLink(el, cellval, opts) {
        if (bHasReadOnlyAccess || bHasAddTopupAccess || bHasAddPostPayAccess) {
            return opts[1];
        }
        return '<a id="UpdateMgmtUsers' + opts[0] + '" class="UpdateClass" href="#" iManagementUserID="' + opts[0] + '" > ' + opts[1] + '</a>';
    }

    function DeleteManagementUser() {
        var selectedRow = jQuery('#gridManagementUsers').getGridParam('selrow');

        if (selectedRow == null)
            alert('Please select a record to delete.');
        else {

            if (selectedRow == LoginID) // Can't delete your own self.
                return;            

            if (confirm("Are you sure want to delete the User? ")) {
                ShowLoadingBox();
                        $.ajax({
                            url: '<%= ResolveUrl("~/ManagementUser/DeleteManagementUser") %>',
                        data: { "iManagementUserID": selectedRow },
                        type: "POST",
                        cache: false,
                        timeout: 6000000,
                        async: true,
                        success: function (result) {
                            HideLoadingBox();
                            if (result == "true") {
                                alert("The User has been deleted.");
                                $("#gridManagementUsers").trigger("reloadGrid");
                            }
                            else {
                                alert('Error occurred while deleting User. Please try again.');
                            }
                        },
                        error: function (httpRequest, msg) {
                            HideLoadingBox();
                            alert('Error occurred while deleting User. Please try again.');
                        }
                    });     //ends ajax  

                }//ends Confirm
                           

    }// If row is selected
}
        </script>

    </asp:PlaceHolder>
    <table>
        <tr>
            <td style="font-family: Calibri; font-size: large" id="Heading">Manage Management Users</td>
        </tr>
    </table>      
    <br />
    <table id="gridManagementUsers" class="scroll"></table>
    <div id="managePager" class="scroll" style="text-align: center;"></div>
            
    <br />
    <div id="confirmdialog" title="Confirmation Required" style="display: none;">
        Are You Sure?
    </div>

    <div id='dlgManageUser' style="display: none;">
        <input type="hidden" value="1" id="hdnUpdateManagementUsersID" />
    </div>
    
    <div class="modal">
        <div id="LoadingText">Loading</div>
    </div>
</asp:Content>


