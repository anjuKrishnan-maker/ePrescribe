// Page Utilities
var GLOBAL_CONTEXT = {
    Url: Window.location
}

function backButtonOverride() {
    setTimeout("backButtonOverrideBody()", 0);
}

function backButtonOverrideBody() {
    try {
        history.forward();
        var isIE11 = navigator.userAgent.match(/Trident/);
        if (isIE11 != undefined && (document.documentMode && document.documentMode == 11) && $telerik)
            $telerik.isIE = false;
        //if ($telerik) {
        //    
        //}
    } catch (ex) {
        //Exception, ignore for now.
    }
    //This is a hack to make the back over ride work
    //in Opera, FireFox, and Safari. These browsers 
    //don't always call onLoad event on page load. 
    //These browsers will resume any times when returning to pages. 
    setTimeout("backButtonOverrideBody()", 500);
}

function setInnerText(ctl, content) {
    var isIE = (window.navigator.userAgent.indexOf("MSIE") > 0);

    if (!isIE)
        ctl.textContent = content;
    else
        ctl.innerText = content;
}

function SetCharsRemaining(textBox, charsRemainingSpan, maxLen) {
    if (charsRemainingSpan != null)
        setInnerText(charsRemainingSpan, (maxLen - textBox.value.length).toString());
}

function browserDetect() {
    var agt = navigator.userAgent.toLowerCase();

    if (agt.indexOf("msie") != -1) return 'Internet Explorer';
    if (agt.indexOf("mozilla/5.0") != -1) return 'Mozilla';
    if (agt.indexOf("firefox") != -1) return 'Firefox';
    if (agt.indexOf("opera") != -1) return 'Opera';
    if (agt.indexOf("staroffice") != -1) return 'Star Office';
    if (agt.indexOf("webtv") != -1) return 'WebTV';
    if (agt.indexOf("beonex") != -1) return 'Beonex';
    if (agt.indexOf("chimera") != -1) return 'Chimera';
    if (agt.indexOf("netpositive") != -1) return 'NetPositive';
    if (agt.indexOf("phoenix") != -1) return 'Phoenix';
    if (agt.indexOf("safari") != -1) return 'Safari';
    if (agt.indexOf("skipstone") != -1) return 'SkipStone';
    if (agt.indexOf("netscape") != -1) return 'Netscape';
    if (agt.indexOf('\/') != -1) {
        if (agt.substr(0, agt.indexOf('\/')) != 'mozilla') {
            return navigator.userAgent.substr(0, agt.indexOf('\/'));
        } else {
            return 'Netscape';
        }
    }else if (agt.indexOf(' ') != -1) {
        return navigator.userAgent.substr(0, agt.indexOf(' '));
    } else {
         return navigator.userAgent;
    }
}

function ignoreIeEvents() {
    var isIE11 = navigator.userAgent.match(/Trident/);
    if (isIE11 != undefined && (document.documentMode && document.documentMode == 11)) {
        try {
            if (Sys.Browser) {
                Sys.Browser.agent = null
            }
            if ($telerik) {
                $telerik.isIE = false;
            }
         
        } catch (ex) {
            //console.log(ex);
        }
    }
}

function RegisterBackButton(id) {
    $("#" + id).click(function (ev) {
        //  ev.stopPropagation();
        if (window.parent['isModalPopup'] != undefined && window.parent['isModalPopup'] == true) {
            window.parent.ParentPopupClose(true, 'patient-info');
            return false;
        } else {
            return true;
        }
    });

    setAngulrModalStatus();
}

function RefreshPatientHeader(patientid) {
    window.parent.ReLoadPatientHeader(patientid);
}

// select and load patient when patient selected from TaskSummary/ListSendScripts (refills/approvals) or SpecialtyMedTasks grid
function SelectPatient(patientId) {
    window.parent.SelectGridPatient(patientId);
}

function UpdatePharmacyInPatientHeader() {
    window.parent.UpdatePatientPharmacy();
}

function LoadFeatureComparison() {
    window.parent.LoadFeatureComparison();
}

function responseEnd(sender, eventArgs) {
    var upDateControls = [];
    for (v = 0 ; v < this._ajaxSettings.length; v++) {
        for (updtId in this._ajaxSettings[v].UpdatedControls) {
            upDateControls.push(this._ajaxSettings[v].UpdatedControls[updtId].ControlID);
        }
    }

    window.parent.ParentLoop(eventArgs);
    if (window.parent['isModalPopup'] != undefined && window.parent['isModalPopup'] == true) {
        window.parent.ParentPopupClose(true, '');
    }
}

function requestStart(sender, eventArgs) {

    window.parent.Clean(eventArgs.get_eventTarget());
}

function SelectMedicine(ddi, formularyStatus, medName, taskScriptMessageId) {
    var tsId = taskScriptMessageId != undefined ? taskScriptMessageId : '';
    var medNm = medName != undefined ? medName : '';
    var fSts = formularyStatus != undefined && parseInt(formularyStatus).toString() != "NaN" ? formularyStatus : 0;
    var data = {
        DDI: ddi,
        FormularyStatus: fSts,
        MedName: medNm,
        taskScriptMessageId: tsId
    };
    window.parent.SelectMedication(data);
}

function ShowAllScriptPadMedPPTSummary() {
    window.parent.ShowAllScriptPadMedPPTSummaryUi();
}

function ShowPharmacySigAlert(msg) {
    window.parent.ShowPharmacySigAlert(msg);
}

window.onunload = function () {
    try {
        var str = this.location.href;

        var quryIndex = str.lastIndexOf('?');
        if (quryIndex > -1) {
            str = str.substring(0, quryIndex);
        }
        var index = str.lastIndexOf('/');
        var length = str.length;
        var res = str.substring(index + 1, length);
        if (window.parent.LoadStarted != undefined
            && typeof (window.parent.LoadStarted) == 'function' && window.frameElement) {
            window.parent.LoadStarted({
                PageName: res, Source: window.frameElement.id
            });//to pass the current page
        }
    }
    catch (e) {
    }
};

function pageConetntLoad() {
    try {
        var str = this.location.href;
        var frm = document.getElementsByTagName('form');
        if (frm.length > 0) {
            if (frm[0].action) {
                str = frm[0].action;
            }
        }
        var quryIndex = str.lastIndexOf('?');
        if (quryIndex > -1) {
            str = str.substring(0, quryIndex);
        }
        var index = str.lastIndexOf('/');
        var length = str.length;
        var res = str.substring(index + 1, length);
        if (window.parent.ContentFrameLoaded != undefined
            && typeof (window.parent.ContentFrameLoaded) == 'function' && window.frameElement) {
            window.parent.ContentFrameLoaded({
                PageName: res, Source: window.frameElement.id
            });//to pass the current page
            //window.parent.ContentFrameLoaded({ PageName: res, Source: $(window.frameElement).attr('id') });//to pass the current page
        }

        var mpe = document.getElementById('ctl00_ContentPlaceHolder1_ucMedicationHistoryCompletion_panelHistory');
        if (mpe == undefined || mpe == null) {
            if (window.parent.ClosePopupOverlay)
                window.parent.ClosePopupOverlay();
        }
    }
    catch (e) {}
}

$(document).ready(pageConetntLoad);
// window.onload = pageConetntLoad;

function selectPharmacy(pharmacyId) {
    window.parent.PharmacySelected({
        PharmacyID: pharmacyId
    });
}



function togglePrivacyOverride(patientId, userId, createdUtc, dbId) {
    window.parent.TogglePrivacyOverride({ patientId: patientId, userId: userId, createdUtc: createdUtc, dbId: dbId });
}

if ($('form').live != undefined)
    $('form').live("submit", function () {
        var showInline = true;
        if (typeof Page_IsValid !== 'undefined') {
            if (Page_IsValid !== true) {
                showInline = false;
            }
        }

        if (showInline === true) {
            var dvLoad = document.getElementById("divLoading");
            if (dvLoad != undefined && dvLoad != null)
                dvLoad.style.display = "inline";
        }
    });

var lnkLogoutID = '';
function setLogOutControlId(id) {
    lnkLogoutID = id;
}

function expireSession() {
    var timeoutURL = "logout.aspx?Timeout=YES";
    var lnkLogout = document.getElementById(lnkLogoutID);
    if (lnkLogout != null) {
        var logoutURL = lnkLogout.getAttribute("href")
        if (logoutURL != null && logoutURL != "") {
            if (logoutURL.indexOf('?') >= 0) {
                timeoutURL = logoutURL + "&Timeout=YES"
            }
            else {
                timeoutURL = logoutURL + "?Timeout=YES"
            }
        }
    }
    if (window.parent.Logout) {
        window.parent.Logout();
    }
    else {
        window.location = timeoutURL;
    }
}

function setAngulrModalStatus() {
    try {
        if (window.parent['isModalPopup'] != undefined && window.parent['isModalPopup'] == true) {
            $('#isFromAngularModal').val('true');
        }
    }
    catch (exp) {
        // console.log(exp);
    }
}

function setAngulrStatus() {
    try {
        if (window.parent['isModalPopup'] != undefined && window.parent['isModalPopup'] == true) {
            $('#isInsideAngular').val('true');
        }
    }
    catch (exp) {
        // console.log(exp);
    }
}

function RegisterPopupOvelay(id) {
    Sys.Application.add_load(modalSetup);

    function show_pop() {
        if (!(window.parent['isModalPopup'] != undefined && window.parent['isModalPopup'] == true)) {
            try { window.parent.ShowPopupOverlay(); } catch (e) { }//we need to log
        }
    }

    function hide_pop() {
        if (!(window.parent['isModalPopup'] != undefined && window.parent['isModalPopup'] == true))
            try { window.parent.ClosePopupOverlay(); } catch(e){}//we need to log
    }

    function modalSetup() {
        if (!(window.parent['isModalPopup'] != undefined && window.parent['isModalPopup'] == true) && typeof (window.parent.ClosePopupOverlay) == 'function')
            try { window.parent.ClosePopupOverlay(); } catch(e){}//need log
        var comps = Sys.Application.getComponents();
        for (i = 0; i < comps.length; i++) {
            var cmp = comps[i];
            if (cmp._element != undefined && cmp._element.ModalPopupBehavior) {
                if (cmp != undefined && cmp != null) {
                    cmp._yCoordinate = 5;
                    cmp.add_showing(show_pop);
                    cmp.add_hiding(hide_pop);
                }
            }
        }
    }
}

//MessageQueue: On Script Select

function msgQueScriptLoad(scriptMessageID) {
    window.parent.MessageQueueGetScript({
        scriptMessageID: scriptMessageID
    });
}

function ShowLoadingSpinner() {
    window.parent.ShowLoadingSpinner();
}

function HideLoadingSpinner() {
    window.parent.HideLoadingSpinner();
}

function showDurDetailsOverlay(b64Html)
{
    window.parent.ShowHtmlOverlay(b64Html,'DUR Warning Details');
}

function ClearSelectMedicationPrebuiltList() {
    window.parent.ClearSelectMedicationPrebuiltList();
}