<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Organisation/OrganisationDetails.master" Inherits="System.Web.Mvc.ViewPage<dynamic>" %>

<asp:Content ID="OrganisationUserTitle" ContentPlaceHolderID="OrgTitleContent" runat="server">
    Organisation Users
</asp:Content>

<asp:Content ID="OrganisationUserMainContent" ContentPlaceHolderID="OrgMainContent" runat="server">

    <asp:PlaceHolder ID="phScripts" runat="server">
        <script type="text/javascript">

            var bHasReadOnlyAccess = HasReadOnlyAccess();

            $(document).ready(function () {

                LoadOrganisationUsers();

                if (bHasReadOnlyAccess) {
                    $('#btnAdd').addClass('ui-state-disabled');
                    $('#btnDelete').addClass('ui-state-disabled');
                    jQuery("#gridOrganisationUsers").jqGrid().hideCol("Password").trigger("reloadGrid");
                }
            });

            function LoadOrganisationUsers() {
                $("#gridOrganisationUsers").jqGrid({
                    url: '<%= ResolveUrl("~/Organisation/GetOrganisationUsers/")%>',
                    datatype: 'json',
                    mtype: 'GET',
                    colNames: ['', 'Name', 'Email', 'Access Level', 'Send New Password'],
                    colModel:
                    [
                        { name: 'OrganisationUserID', key: true, hidden: true, index: 'OrganisationUserID', align: 'center', search: false, editable: false },
                        { name: 'Forename', index: 'Forename', align: 'left', formatter: getEditLink, searchoptions: { sopt: ['cn', 'eq'] }, searchrules: { required: true } },
                        { name: 'Email', index: 'Email', align: 'left', sortable: true, searchoptions: { sopt: ['cn', 'eq'] }, searchrules: { required: true } },
                        { name: 'AccessLevel', index: 'AccessLevel', align: 'left', sortable: true, search:false},
                        { name: 'Password', index: 'Password', align: 'center', search: false, sortable: false, formatter: GetGeneratePasswordLink }
                    ],
                    pager: '#OrganisationPager',
                    rowNum: 10,
                    rowList: [10, 20, 50],
                    viewrecords: true,
                    //caption: 'Manage Organisation Users',
                    hidegrid: false,
                    height: '100%',
                    width: '900',
                    align: 'center',
                    rownumbers: false,
                    gridComplete: function () {

                        $('a.UpdateClass').click(function () {

                            var iOrganisationUserID = $(this).attr('iOrganisationUserID');
                            ShowAddEditOrganisationUserDialog(iOrganisationUserID);

                        });
                        $('img.SendPassword').click(function () {
                            var strEmail = $(this).attr('strEmail');
                            ResetPassword(strEmail);

                        });
                    },

                    pgbuttons: true,
                    onPaging: function (pgButton) {

                        if ("user" == pgButton) {
                            var requestedPage = $("#gridOrganisationUsers").getGridParam("page");
                            var lastPage = $("#gridOrganisationUsers").getGridParam("lastpage");
                            if (eval(requestedPage) > eval(lastPage)) {
                                $("#gridOrganisationUsers").setGridParam({ page: lastPage });
                            }

                        } else if ("records" == pgButton) {
                            var requestedPage = $("#gridOrganisationUsers").getGridParam("page");
                            var totalRows = $("#gridOrganisationUsers").getGridParam("records");
                            var rowsPerPage = $("#gridOrganisationUsers").getGridParam("rowNum");
                            var lastPage = Math.ceil(eval(totalRows) / eval(rowsPerPage));
                            if (eval(requestedPage) > lastPage) {
                                $("#gridOrganisationUsers").setGridParam({ page: lastPage });
                            }
                        }
                    }, //end onPaging
                    pginput: true
                }).navGrid('#OrganisationPager', {
                    edit: false, add: false, del: false, search: true, searchtext: "Search",
                    searchtitle: "Click to search."
                },
					{}, // settings for edit
					{}, // add
					{}, // delete
					{
					    caption: "Search Organisation Users",
					    Find: "Search",
					    searchOnEnter: true,
					    modal: true,
					    closeAfterSearch: true,
					    closeOnEscape: true,
					    //closeAfterReset: true,
					    width: "350",
					    onReset: function () {
					        var jqModal = true;
					        $.jgrid.hideModal("#searchmodfbox_" + this.id,
                                { gb: "#gbox_" + this.id, jqm: jqModal, onClose: null });
					    }
					}, // search options
					{}
					).navButtonAdd('#OrganisationPager', {
					    id: "btnDelete",
					    caption: "Delete",
					    title: "Click to delete.",
					    buttonicon: "ui-icon ui-icon-trash",
					    width: "350",
					    onClickButton: function () {
					        DeleteOrganisationUser();
					    }, //OnClick
					    position: "first"
					}).navButtonAdd('#OrganisationPager', {
					    id: "btnAdd",
					    caption: "Add",
					    title: "Click to add.",
					    buttonicon: "ui-icon-circle-plus",
					    width: "350",
					    onClickButton: function () {

					        ShowAddEditOrganisationUserDialog(0);
					    }, //OnClick
					    position: "first"
					});;
            }

            function ShowAddEditOrganisationUserDialog(iOrgUserID) {

                var dlgtitle = "";
                if (iOrgUserID == 0)
                    dlgtitle = "Add Organisation User";
                else
                    dlgtitle = "Edit Organisation User";

                $.ajax({
                    url: '<%= ResolveUrl("~/Organisation/ShowAddEditOrganisationUsersDialog") %>',
                    data: { "iOrganisationUserID": iOrgUserID },
                    type: "POST",
                    dataType: "html",
                    cache: false,
                    success: function (data) {
                        $('#dlgOrganisationUser').html(data); //write the dialog content into the dialog container
                        $("#dlgOrganisationUser").dialog({
                            autoOpen: false, modal: true, title: dlgtitle, resizable: false, height: 230, width: 500});
                        $("#dlgOrganisationUser").dialog("open"); //open it!
                    },
                    error: function (xhr, ajaxOptions, thrownError)
                    {
                        alert(thrownError);
                    }
                });//ends ajax
             }

                function ResetPassword(strEmail) {
                    ShowLoadingBox();
                    $.ajax({
                        url: '<%= ResolveUrl("~/Organisation/ResetPassword") %>',
                        data: { "strEmail": strEmail },
                        type: "POST",
                        cache: false,
                        success: function (result) {
                            HideLoadingBox();
                            if (result) {
                                alert("Password has been reset.");
                            }
                            else {
                                alert('Some error occurred while reseting the password.');
                            }
                        },
                        error: function (xhr, ajaxOptions, thrownError) {
                            HideLoadingBox();
                            alert(thrownError);
                        }
                    });
                }


                function GetGeneratePasswordLink(el, cellval, opts) {
                    return '<img src="<%= ResolveUrl("~/Content/Images/lock.png")%>"  height="20px" width="20px" alt="my image" id="SendPassword' + opts[0] + '"class="SendPassword" href="#" strEmail="' + opts[2] + '" />';
                }

                function getEditLink(el, cellval, opts) {
                    if (bHasReadOnlyAccess) {
                        return opts[1];
                    }
                    return '<a id="UpdateOrganisationUser' + opts[0] + '" class="UpdateClass" href="#" iOrganisationUserID="' + opts[0] + '" > ' + opts[1] + '</a>';
                }

                function DeleteOrganisationUser() {

                    var selectedRow = jQuery('#gridOrganisationUsers').getGridParam('selrow');

                    if (selectedRow == null)
                        alert('Please select a record to delete.');
                    else {

                        if (confirm("Are you sure want to delete the User? ")) {
                            ShowLoadingBox();
                            $.ajax({
                                url: '<%= ResolveUrl("~/Organisation/DeleteOrganisationUser") %>',
                            data: { "iOrganisationUserID": selectedRow },
                            type: "POST",
                            cache: false,
                            timeout: 6000000,
                            async: true,
                            success: function (result) {
                                HideLoadingBox();
                                if (result == "true") {
                                    alert("The User has been deleted.");
                                    $("#gridOrganisationUsers").trigger("reloadGrid");
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
            <td style="font-family: Calibri; font-size: large" id="Heading">Manage Organisation Users</td>
        </tr>
    </table>    
    <table>
        <tr>
            <td style="vertical-align: top">
                <table id="gridOrganisationUsers" class="scroll" style="width: 700px;"></table>
                <div id="OrganisationPager" class="scroll" style="text-align: center;"></div>
            </td>
        </tr>
    </table>

    <div id='dlgOrganisationUser' style="display: none;">
    </div>

    <div class="modal">
        <div id="LoadingText">Loading</div>
    </div>
</asp:Content>
