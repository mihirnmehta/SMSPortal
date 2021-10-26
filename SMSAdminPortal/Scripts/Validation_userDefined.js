//Validation for positive integer
//returns true if the value is whole integer
//else false

function isValidName(sName) {
    var regex = /^[a-zA-Z ]+$/;

    if (regex.test(sName)) {
        return true;
    }
    else {
        return false;
    }
}

function ValidateNumericValues(iValue) {

    var reg = /^\s*\d+\s*$/;

    if (!reg.test(iValue)) {
        //alert('Please enter a numeric value without fractions.');
        return false;
    }
    else {
        return true;
    }
}

//Validation for positive floating number
function ValidateDecimalValues(iValue) {
    var reg = /^(([0-9]+)|([0-9]+\.[0-9]{1,2}))$/;

    if (!reg.test(iValue)) {
        return false;
    }
    else {
        return true;
    }
}

//Format number to two decimal if not
function roundOffDecimals(ele) {
    iValue = ele.value;
    var reg = /^(([0-9]+)|([0-9]+\.[0-9]{1,2}))$/;
    if (!reg.test(iValue)) {
        return false;
    }
    else {
        if (iValue.indexOf('.') < 0)
            ele.value = ele.value + ".00";
        else if (iValue.indexOf('.')==iValue.length-1)
            ele.value = ele.value + "00";
        else if (iValue.indexOf('.')==iValue.length-2)
            ele.value = ele.value + "0";
        return true;
    }
}

function isEmailAddress(emailAddress) {

    //var pattern = new RegExp(/^\w+([\.-]?\w+)*@\w+([\.-]?\w+)*(\.\w{2,3})+$/);
    //var pattern = new RegExp(/\w+([-.]\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*/);
    var pattern = new RegExp(/^(([^<>()[\]\\.,;:\s@\"]+(\.[^<>()[\]\\.,;:\s@\"]+)*)|(\".+\"))@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\])|(([a-zA-Z\-0-9]+\.)+[a-zA-Z]{2,}))$/);

    return pattern.test(emailAddress);
};

function check24hrTime(s) {
    var timePattern = new RegExp(/^((0[0-9])|(1[0-9])|(2[0-3])):([0-5][0-9])$/);
    return timePattern.test(s);
}

function checkTimeRange(startTime, endTime) {
    var regExp = /(\d{1,2})\:(\d{1,2})/;

    if (parseInt(endTime.replace(regExp, "$1$2")) > parseInt(startTime.replace(regExp, "$1$2"))) {
        return true
    }
    return false;
}

function validatePhoneNumber(phoneNumber)
{
    var pattern = new RegExp(/^\+?[\d]{9,10}$/);

    if (!pattern.test(phoneNumber)) {
        return false;
    }
    else {
        return true;
    }
}

function checkPasswordComplexity(password)
{
    var strength = 0;

    if (password.match(/([a-zA-Z])/) && password.match(/([0-9])/))
        strength += 1;

    if (password.match(/([!,%,@,#,$,*,_])/))
        strength += 1;

    if (strength < 2)
        return false;
    else
        return true;
}

function checkStartEndDate(sStartDate, sEndDate)
{

    var part = sStartDate.split("/");

    var d1 = part[0];
    var m1 = part[1];
    var y1 = part[2];

    part = sEndDate.split("/");

    var d2 = part[0];
    var m2 = part[1];
    var y2 = part[2];

    var isDateValid = false;

    if (y1 <= y2)
    {
        if (y1 == y2)
        {
            if (m1 <= m2)
            {
                if (m1 == m2)
                {

                    if (d1 <= d2)
                        isDateValid = true;
                    else
                        isDateValid = false;

                }
                else
                {
                    isDateValid = true;
                }
            }
            else {
                isDateValid = false;
            }
            
        }
        else 
            isDateValid = true;        
    }

    return isDateValid;
}