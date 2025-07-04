
var isAngular = true;

if (!window.console || typeof console === "undefined") {
    var console = { log: function (logMsg) { } };
}

// called from a js responseEnd function in pageUtil.js, what is this for? can we remove?
window.ParentLoop = function (msg) {
    if (msg && (msg.EventTarget.indexOf('hiddenSelect') > 0 || msg.EventTarget === 'hiddenSelect'))
        CallAngularInsideFunction(null, 'LoadPatientHeader');
}

// I don't see any call to this
Window.SelectCurrentPatient = function () {
    CallAngularInsideFunction(null, 'LoadPatientHeader');
}

window.Clean = function (arg) {
    if (arg.indexOf('hiddenSelect') > 0 || arg === 'hiddenSelect')
        CallAngularInsideFunction(null, 'CleanPatientHeader');
}

// what is this for
function ParentPopupClose(needRefresh, component) {
    $("#mdlPopupMain").modal('toggle');//('hide');
    if (window['isModalPopup'] !== undefined)
        window['isModalPopup'] = false;
    if (needRefresh) {
        ResetPopupUrl();
    }
}

// saving existing patient from AddPatient.aspx
function SaveEditedPatient(needRefresh, patientId) {
    $("#mdlPopupMain").modal('toggle');//('hide');
    if (window['isModalPopup'] !== undefined)
        window['isModalPopup'] = false;
    if (needRefresh) {
        ResetPopupUrl();
        window.parent.SavePatient(patientId);
    }
}

function UpdateTasksCount(numberOfRemainingTasks) {
    CallAngularInsideFunction(numberOfRemainingTasks, 'UpdateTasksCount');
}
function UpdateActiveMenu(newMenu) {
    CallAngularInsideFunction(newMenu, 'changeAcitiveMenu');
}
// select and load patient when patient selected from TaskSummary/ListSendScripts (refills/approvals) or SpecialtyMedTasks grid
function SelectGridPatient(patientid) {
    CallAngularInsideFunction(patientid, 'SelectPatientFromGrid');
}

function UpdatePatientPharmacy() {
    CallAngularInsideFunction(null, 'UpdatePatientPharmacy');
}

function LoadFeatureComparison() {
    CallAngularInsideFunction(null, 'LoadFeatureComparisonComponent');
}

function SavePatient(patientId) {
    CallAngularInsideFunction(patientId, 'SavePatient');
}

function ReLoadPatientHeader(patientid) {
    CallAngularInsideFunction(patientid, 'LoadPatientHeader');
}

function ParentRedirect(component, targate) {
    ParentPopupClose(true, component);
    CallAngularInsideFunction(targate, 'RedirectContent');
}

function ParentRedirectPatientEdit(component, targate, patientId) {
    ParentPopupClose(true, component);
    CallAngularInsideFunction({ patientId: patientId, targate: targate }, 'UpdateSelectedPatientInfo');
}

var isModalInitialized = false;

window.autoResize = function () {
    var newHeight = 0;
    if (window.navigator.userAgent.indexOf("Trident") > 0) { // checking IE
        newHeight = screen.height - 300;
    }
    else {
        newHeight = $('#frameMdlPopup').contents().find("html").height();
    }
    newHeight = screen.height - 300;
    var mdlSrc = $("#frameMdlPopup").attr('src');
    if (newHeight > 10) {
        $('#frameMdlPopup').height(newHeight);

        if (mdlSrc !== undefined && mdlSrc.length > 2 && mdlSrc !== 'undefined') {
            var x = $("#mdlPopupMain").modal({
                //    backdrop: 'static'
            });
            if (isModalInitialized === false) {
                x.on('hidden.bs.modal', function () {
                    if (window['isModalPopup'] !== undefined)
                        window['isModalPopup'] = false;
                    ResetPopupUrl();
                    if (typeof window.frames[0].reloadPatient === "function") {
                        window.frames[0].reloadPatient();
                    }
                });
            }
            isModalInitialized = true;
            CallAngularInsideFunction(null, 'StopLoading');
        }
    }
    else {
        if (mdlSrc !== undefined && mdlSrc.length > 2 && mdlSrc !== 'undefined')
            window.setTimeout(function () { autoResize(); }, 3000);
    }
}

// called from privacy override component to refresh ??? after saving/canceling popup
function RefreshPatientGrid() {
    if (typeof window.frames[0].reloadPatient === "function") {
        window.frames[0].reloadPatient();
    }
}

/*function to selct event of medication 
usage for bellow user control(replacing)*/
//ucFormularyAlts
function SelectMedication(data) {
    CallAngularInsideFunction(data, 'MedicineSelected');
}

/*Set component on load of page*/
function ContentFrameLoaded(data) {
    if (data.Source === 'content-frame' && data.PageName !== "Login.aspx")
        CallAngularInsideFunction(data.PageName, 'ContentPageLoaded');
    if (data.PageName === "Login.aspx") {
        window.location.href = data.PageName;
    }
}

function LoadStarted(data) {
    if (data.Source === 'content-frame' && data.PageName !== "Login.aspx")
        CallAngularInsideFunction(data.PageName, 'ContentPageLoadStarted');
}

function ResetPopupUrl() {
    CallAngularInsideFunction(null, 'resetModalUrl');
}

function PharmacySelected(data) {
    CallAngularInsideFunction(data, 'PharmacySelected');
}

function TogglePrivacyOverride(patientId, userId, createdUtc, dbId) {
    CallAngularInsideFunction({ patientId: patientId, userId: userId, createdUtc: createdUtc, dbId: dbId }, 'TogglePrivacyOverride');
}

function AuditLogRowSelected(data) {
    CallAngularInsideFunction(data, 'AuditLogRowSelected');
}

function ShowPopupOverlay() {
    CallAngularInsideFunction(null, 'ShowOverlay');
}

function ClosePopupOverlay() {
    CallAngularInsideFunction(null, 'CloseOverlay');
}

function Logout() {
    CallAngularInsideFunction(null, 'Logout');
}

function RedirectAngularContentPage(url) {
    CallAngularInsideFunction(url, 'redirectContent');
}

//function to call zone angular function
function CallAngularInsideFunction(data, fnstring) {
    // find object
    var fn = window[fnstring];
    window.AngularComponent.zone.run(function () {
        if (typeof fn === "function") {
            fn.call(this, data);
        }
    });
}

function setModalStatusIframe() {
    var docIframe = document.getElementById("frameMdlPopup");
    if (docIframe !== undefined && docIframe.contentWindow !== undefined) {
        docIframe.contentWindow.setAngulrModalStatus();
    }
}

function NavigateToAngularComponent(componentName, componentParameters) {
    if (componentName.startsWith("ReviewHistory")) {
        CallAngularInsideFunction(componentParameters, 'ShowReviewHistoryPage');
    }
   else if (componentName.startsWith("SelectMedication")) {
        try {
            componentParameters = JSON.parse(componentParameters);
        } catch (e) { }
        CallAngularInsideFunction(componentParameters, 'ShowSelectMedicationPage');
    } else if (componentName.startsWith("SelectPatient")) {
        CallAngularInsideFunction(componentParameters, 'ShowSelectPatientPage');
    } else if (componentName.toLowerCase().startsWith("reports")) {//From report detail to SPA page.
        CallAngularInsideFunction(componentParameters, 'ShowReportsPage');
    } else if (componentName.toLowerCase().startsWith("settings")) {
        CallAngularInsideFunction(componentParameters, 'ShowSettingsPage');
    } else if (componentName.startsWith("PharmacySigAlert")) {
        CallAngularInsideFunction(componentParameters, 'ShowPharmacySigAlert');
    } else if (componentName.startsWith("DeluxeFeatureSelection")) {
        CallAngularInsideFunction(componentParameters, 'showDeluxeFeatureSelectionPage');
    } else if (componentName.startsWith("EditUser")) {
        CallAngularInsideFunction(componentParameters, 'ShowEditUserPage');
    } else if (componentName.startsWith("HomeAddress")) {
        CallAngularInsideFunction(componentParameters, 'ShowHomeAddressPage');
    }

    else {
        try {
            componentParameters = JSON.parse(componentParameters);
        } catch (e) { }
        CallAngularInsideFunction({ componentName: componentName, componentParameters: componentParameters }, 'LoadComponent');
    }
}

function MessageQueueGetScript(data) {
    CallAngularInsideFunction(data, 'MessageQueueGetScriptData');
}

function OpenUrlInModal(url, title) {
    CallAngularInsideFunction({ src: url, title: title }, "setUrlModalPopup");
}

function showChangePasswordModal() {
    CallAngularInsideFunction(null, 'ChangePasswordPopup');
}

function OpenPdfOverlay(url, title) {
    CallAngularInsideFunction({ PdfUrl: url, PopupTitle: title }, "ShowPdfOverlay");
}

function ShowHtmlOverlay(b64Html, title) {
    CallAngularInsideFunction({ b64Html: b64Html, title: title }, "showHtmlOverlay");
}

function ShowExternalHtmlOverlay(b64Html, title) {
    CallAngularInsideFunction({ b64Html: b64Html, title: title }, "showExternalHtmlOverlay");
}

function ShowPharmacySigAlert() {
    CallAngularInsideFunction(null, "ShowPharmacySigAlert");
}

function ShowLoadingSpinner() {
    CallAngularInsideFunction(null, 'BeginLoading');
}

function HideLoadingSpinner() {
    CallAngularInsideFunction(null, 'StopLoading');
}

function InitiatePricingInquiryAngular(data) {
    CallAngularInsideFunction(data, 'InitiatePricingInquiry');
}

function RetrieveAllScriptPadMedSummaryUi() {
    CallAngularInsideFunction(null, 'RetrieveAllScriptPadMedSummaryUi');
}

function RemoveUnselectedRowsAngular(data) {
    RemoveUnselectedRowsAngular(data, 'RemoveUnselectedRowsAngular');
}

function LoadComponent(component) {
    CallAngularInsideFunction(component, 'LoadComponent');
}

function ShowEcouponUncheckedWarning(checkboxId) {
    CallAngularInsideFunction(checkboxId, "showEcouponUncheckedWarning");
}

function ClearSelectMedicationPrebuiltList(checkboxId) {
    CallAngularInsideFunction(null, "clearSelectMedicationPrebuiltList");
}

function CloseTeaserAd() {
    CallAngularInsideFunction(null, "CloseTeaserAd");
}

function LoadILearnPage() {
    CallAngularInsideFunction(null, "LoadILearnPage");
}

function LoadDeluxeFeatureStatusPage() {
    CallAngularInsideFunction(null, "LoadDeluxeFeatureStatus");
}
