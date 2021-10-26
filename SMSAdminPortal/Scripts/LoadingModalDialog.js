var spinnerVisible = false;

//Show progress image
function ShowProgress(control) {
    if (!spinnerVisible) {
        $("div#" + control).fadeIn("fast");
        //$("div#spinner").fadeIn("fast");
        spinnerVisible = true;
    }
};

//Hide progress image
function HideProgress(control) {
    if (spinnerVisible) {
        //var spinner = $("div#spinner");
        var spinner = $("div#" + control);
        spinner.stop();
        spinner.fadeOut("fast");
        spinnerVisible = false;
    }
};


function ShowLoading() {
    $('body').addClass("loading");
}

function HideLoading() {
    $('body').removeClass("loading");
}

//Access Level Methods
//Start

function HasAdminAccess() {
    var isAdmin = new Boolean();
    $.ajax({
        url: '<%= ResolveUrl("~/Account/HasAdminAccess") %>',
        type: "POST",
        cache: false,
        timeout: 6000000,
        async: false,
        success: function (result) {
            isAdmin = result;
        }
    });
    return isAdmin;
}

function HasAccountManagementAccess() {
    var isAccountMngmt = new Boolean();
    $.ajax({
        url: '<%= ResolveUrl("~/Account/HasAccountManagementAccess") %>',
        type: "POST",
        cache: false,
        timeout: 6000000,
        async: false,
        success: function (result) {
            isAccountMngmt = result;
        }
    });
    return isAccountMngmt;
}

function HasAddPostPayAccess() {
    var isAddPostPay = new Boolean();
    $.ajax({
        url: '/Account/HasAddPostPayAccess',
        type: "POST",
        cache: false,
        timeout: 6000000,
        async: false,
        success: function (result) {
            isAddPostPay = result;
        }
    });
    return isAddPostPay;
}

function HasAddTopupAccess() {
    var isAddTopup = new Boolean();
    $.ajax({
        url: '/Account/HasAddTopupAccess',
        type: "POST",
        cache: false,
        timeout: 6000000,
        async: false,
        success: function (result) {
            isAddTopup = result;
        }
    });
    return isAddTopup;
}

function HasReadOnlyAccess() {
    var isReadOnly = new Boolean();
    $.ajax({
        url: '/Account/HasReadOnlyAccess',
        type: "POST",
        cache: false,
        timeout: 6000000,
        async: false,
        success: function (result) {
            isReadOnly = result;
        }
    });
    return isReadOnly;
}

//End