function showSpecialtyRxPharmacyOverlay(hdnButton, ddl) {
    if (ddl.options[ddl.selectedIndex].value === "LDPL") {
        hdnButton.click();
    }

}

function confirmOffer(rxTaskID) {
    $.ajax({
        type: 'POST',
        url: '/JsonGateway.aspx/ConfirmOffer',
        contentType: 'application/json; charset=utf-8',
        dataType: 'json',
        data: '{rxTaskID: "' + rxTaskID + '" }'
    });
}

function detectBrowser() {
    var N = navigator.appName;
    var UA = navigator.userAgent;
    var temp;
    var browserVersion = UA.match(/(opera|chrome|safari|firefox|msie)\/?\s*(\.?\d+(\.\d+)*)/i);
    if (browserVersion && (temp = UA.match(/version\/([\.\d]+)/i)) != null)
        browserVersion[2] = temp[1];
    browserVersion = browserVersion ? [browserVersion[1], browserVersion[2]] : [N, navigator.appVersion, '-?'];
    return browserVersion;
}

function sizeFrame() {
    var browser = detectBrowser();
    if (browser == 'MSIE,7.0') {
        var width = $(window).width(), height = $(window).height();
        if ((width <= 1360) && (height <= 768)) {
            $('#ctl00_ContentPlaceHolder1_ifcDiv').css('height', 495).css('width', 900);
            $('#ctl00_ContentPlaceHolder1_fcFrame').css('height', 440);
        }
        else {
            $('#ctl00_ContentPlaceHolder1_ifcDiv').css('height', 0.90 * $(window).height()).css('width', 0.94 * $(window).width());
            $('#ctl00_ContentPlaceHolder1_fcFrame').css('height', 0.85 * $(window).height()).css('width', 0.90 * $(window).width());
        }
    }
    else {
        $('#ctl00_ContentPlaceHolder1_ifcDiv').css('height', 0.90 * $(window).height()).css('width', 0.94 * $(window).width());
        $('#ctl00_ContentPlaceHolder1_fcFrame').css('height', 0.85 * $(window).height());
        $(window).resize(function () {
            $('#ctl00_ContentPlaceHolder1_ifcDiv').css('height', 0.90 * $(window).height()).css('width', 0.94 * $(window).width());
            $('#ctl00_ContentPlaceHolder1_fcFrame').css('height', 0.85 * $(window).height());
        });
    }
}

function taskSelected(source, eventArgs) {
    taskSelectedRadio(eventArgs.getDataKeyValue("RxTaskID").toString(), eventArgs.getDataKeyValue("PatientGUID").toString());
}