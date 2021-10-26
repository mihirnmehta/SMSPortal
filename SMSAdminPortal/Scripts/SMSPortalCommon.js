//$(document).bind("ajaxError", function (e, jqXHR, ajaxSettings, thrownError) {

//    //alert('you got a problem');

//    if (jqXHR.status == 401) {
//        // perform a redirect to the login page since we're no longer authorized
//        window.location.href = "~/Account/Login";
//    }
//    else {
//        alert(thrownError);
//        window.location.href = "~/Account/Error";
//    }
//});



function ShowLoadingBox() {
    $('body').addClass("loading");
}

function HideLoadingBox() {
    $('body').removeClass("loading");
}

//Access Level Methods
//Start

//function HasAdminAccess() {
//    var isAdmin = new Boolean();
//    $.ajax({
//        url: '~/Account/HasAdminAccess',
//        type: "POST",
//        cache: false,
//        timeout: 6000000,
//        async: false,
//        success: function (result) {
//            isAdmin = result;
//        }
//    });
//    return isAdmin;
//}

//function HasAccountManagementAccess() {
//    var isAccountMngmt = new Boolean();
//    $.ajax({
//        url: '~/Account/HasAccountManagementAccess',
//        type: "POST",
//        cache: false,
//        timeout: 6000000,
//        async: false,
//        success: function (result) {
//            isAccountMngmt = result;
//        }
//    });
//    return isAccountMngmt;
//}

//function HasAddPostPayAccess() {
//    var isAddPostPay = new Boolean();
//    $.ajax({
//        url: '/Account/HasAddPostPayAccess',
//        type: "POST",
//        cache: false,
//        timeout: 6000000,
//        async: false,
//        success: function (result) {
//            isAddPostPay = result;
//        }
//    });
//    return isAddPostPay;
//}

//function HasAddTopupAccess() {
//    var isAddTopup = new Boolean();
//    $.ajax({
//        url: '/Account/HasAddTopupAccess',
//        type: "POST",
//        cache: false,
//        timeout: 6000000,
//        async: false,
//        success: function (result) {
//            isAddTopup = result;
//        }
//    });
//    return isAddTopup;
//}

//function HasReadOnlyAccess() {
//    var isReadOnly = new Boolean();
//    $.ajax({
//        url: '/Account/HasReadOnlyAccess',
//        type: "POST",
//        cache: false,
//        timeout: 6000000,
//        async: false,
//        success: function (result) {
//            isReadOnly = result;
//        }
//    });
//    return isReadOnly;
//}