import { UserPreferenceModel } from '../model/model.import.def';

let EVENT_COMPONENT_ID = {
    ContentLoadEvent: 'content-load',
    ContentLoadStart: 'content-load-start',
    MedicationSelected: 'med-selected',
    PharmacySelected: 'phrma-selected',
    //Navigation Related    
    RightControlLoad: 'right-pnl-load',
    CallFrameContentJavaScript: 'CallFrameContentJavaScript',
    //Work flow related
    OnMedicationSelected: 'on-medication-selected',
    OnMedicationDeSelected: 'on-medication-deselected',
    OnHistoryRxSelected: 'on-history-rx-selected',
    OnScriptPadMedicationAdded: 'on-script-pad-med-added',
    OnPatientSelected: "on-patient-selected",
    OnPatientMedicationsUpdated: "on-patient-medications-updated",
    ErrorEvent: 'error-event',
    MessaQSelectEvent: 'msg-q-select',
    //Custom behaviour for the right panel inheriting base component.
    ColapsableVisible: 'colapsable-visable',
    ColapsableCollapse: 'colapsable-colapse',
    //Component communication missing direct relationship in DOM tree.
    ReviewScriptOpenModal: 'review-script-open-modal',
    FailedRegistrationMessageModalOpen: 'failed-registration-message-model-open',
    RefreshPatientHeader: 'refresh-patient-header',
    PptPlusSummaryUpdate: 'ppt-plus-summary-update',
    PptPlusDetailsUpdate: 'ppt-plus-details-update',
    ReloadScriptPadPanel: "ReloadScriptPadPanel",
    ToggleReviewScriptPadDisplay: "toggle-review-scriptpad-display",
    CompletedRxCancelRxOpen: 'CompletedRxCancelRxOpen',
    ClosePopover: "ClosePopover",
    //Grouped API call related-  
    RightPanelLoaded: 'RightPanelLoaded',
    ContentRefreshing: 'ContentRefreshing',
    ILearnLoaded: 'ILearnLoaded',
    MedicationSelecting: 'MedicationSelecting'
};

let PAGE_NAME = {
    GetEPCS: "Get EPCS",
    AddPatient_AddMode: "AddPatient.aspx?Mode=Add",
    ApproveRefillTaskPage: "ApproveRefillTask.aspx",
    ApproveScriptMessagePatient: 'approvescriptmessagepatient.aspx',
    DeluxAdContainer: 'DeluxAdContainer.aspx',
    DeluxeAccountManagement: 'DeluxeAccountManagement.aspx',
    DeluxeBillingPage: 'DeluxeBillingPage.aspx',
    DeluxeFeatureSelectionPage: 'DeluxeFeatureSelectionPage',
    EditUser: 'EditUser',
    EditUsers: 'EditUsers.aspx',
    EpcsDailyActivityReport: 'epcsdailyactivityreport.aspx',
    EpcsRegManageMode: 'EPCSRegistration.aspx?Mode=Manage',
    ExceptionPage: 'Exception.aspx',
    FreeFormDrug: 'FreeFormDrug.aspx',
    IntegrationSolutionsList: 'integrationsolutionslist.aspx',
    Library: 'library.aspx',
    ListSendScripts: 'listsendscripts.aspx',
    MessageQueueTx: 'messagequeuetx.aspx',
    MultiViewReport: 'multiviewreport',
    MultipleViewReport: 'MultipleViewReport.aspx?ReportID=PatientCurrentMeds&To=ReviewHistory',
    MyProfile: 'MyProfile.aspx',
    MyEpcsReports: 'myepcsreports',
    NurseSig: 'NurseSig.aspx',
    Patient: 'patient',
    PatientUpload: 'PatientUpload',
    PdmpEnrollmentForm: 'PdmpEnrollmentForm',
    PdmpEpcsTeaser: 'PdmpEpcsTeaser',
    Pharmacy: 'Pharmacy.aspx',
    PharmRefillSummaryPage: "PharmRefillSummary.aspx",
    RedirectToAngular: 'RedirectToAngular.aspx',
    ReportDetail: 'ReportDetail',
    ReportList: 'ReportList',
    Reports: 'Reports',
    ReviewHistory: 'ReviewHistory',
    RxFavourites: 'FullScriptEdit.aspx',
    SelectDiagnosis: "SelectDx.aspx",
    ScriptPad: 'ScriptPad.aspx',
    SelectMedication: 'SelectMedication',
    DeluxeFeatureSelection: 'featureselection',
    SelectPatient: 'SelectPatient',
    SelfReportedMed: 'SelfReportedMed.aspx',
    Settings: 'Settings',
    SigPage: 'Sig.aspx',
    SpecialtyMedTasks: "specialtymedtasks.aspx",
    Tasks: 'DocRefillMenu.aspx',
    ForcePasswordSetup: 'forcepasswordsetup.aspx',
    ManageAccount: 'ManageAccount.aspx',
    PDFInPage: 'PDF_InPage.aspx',
    HomeAddress: 'HomeAddress',
    LogOut: 'LogOut.aspx'
};

let ROUTE_NAME = {
    //Patient - Medication Workflow
    SelectPatient: 'patient/selectpatient',
    SelectMedication: 'patient/medication/selectmedication',
    SelectDiagnosis: 'patient/diagnosis',
    Scriptpad: 'patient/scriptpad',
    FreeFormDrug: 'patient/freeformdrug',
    ReviewHistory: 'patient/reviewhistory',
    AddPatient: 'patient/addpatient',
    Sig: 'patient/sig',
    NurseSig: 'patient/nursesig',
    SelfReportedMed: 'patient/selfreportedmed',

    //Deluxe - Billing Workflow
    ManageAccount: 'deluxe/manageaccount',
    DeluxeFeatures: 'deluxe/featureselection',
    GetEpcs: 'deluxe/featureselection/getepcs',
    DeluxeBilling: 'deluxe/billing',

    //Reports
    Reports: 'general/reports',
    MultipleViewReport: 'report/multiviewreport',

    //General Workflow
    Settings: 'general/settings',
    Library: 'general/library',
    Tasks: 'general/tasks',
    MyProfile: 'general/myprofile',
    MessageQueueTx: 'general/messagequeuetx',
    EPCSDailyReport: 'general/epcsdailyreport',
    MyEPCSReport: 'general/myepcsreport',
    PatientUpload: 'general/settings/patientupload',
    ChartExtractDownload: 'general/settings/chartextractdownload',
    ForcePasswordSetup: 'general/forcepasswordsetup',
    PdfInPage: 'user/activationpdf',
    EditUsers: 'general/editusers',

    //User
    EditUser: 'user/edituser',
    RxFavourites: 'user/rxfavourites',
    HomeAddress: 'user/homeaddress',

    //Exception
    Exception: 'exception'
}

let API_ROUTE_NAME = {
    //Refresh data for the page
    RETRIEVE_REFRESH_PAYLOAD: '/api/contentload/retrieverefreshpayload',
    //PatientService
    PATIENT_SERVICE__SET_PATIENT_INFO: '/api/selectpatientapi/setpatientinfo',
    //Report Service
    REPORT_LIST_FETCHING: '/api/reportsapi/getreportslist',
    //Setting Links
    SETTINGS_LIST_FETCHING: '/api/settingsapi/retrievelinks',
    //SelectMedicationService
    SELECT_MEDICATION__GET_SELECT_MEDICATION_DATA: '/api/selectmedicationapi/getselectmedicationdata',
    //Edit User
    EDIT_USER_INIT: '/api/userapi/userinit',
    EDIT_USER_SAVE: '/api/userapi/saveuser',
    HOME_ADDRESS_GET_START_UP_DATA: '/api/homeaddressapi/getstartupdata',
    HOME_ADDRESS_SAVE: '/api/homeaddressapi/savehomeaddress',
};

let ROUTES_WITH_SPINNER = [
    API_ROUTE_NAME.RETRIEVE_REFRESH_PAYLOAD,
    //Patient Service
    API_ROUTE_NAME.PATIENT_SERVICE__SET_PATIENT_INFO,
    //Report Service
    API_ROUTE_NAME.REPORT_LIST_FETCHING,
    API_ROUTE_NAME.SETTINGS_LIST_FETCHING,
    //Medication Service
    API_ROUTE_NAME.SELECT_MEDICATION__GET_SELECT_MEDICATION_DATA,
    //Edit User Init
    API_ROUTE_NAME.EDIT_USER_INIT,
    API_ROUTE_NAME.EDIT_USER_SAVE
];

let NAVIGATION_EVENT_ID = {
    LogoutInvoked: 'log-out',
    SessionTimeout: 'SessionTimeout',
    NavigateToComponent: 'navigate-to-component',
    SessionReAuthRequired: 'SessionReAuthRequired'
};

let MODAL_DISMISS_EVENT = {
    AddDiagnosisModal: 'diagnosisModal',
    AddAllergyModal: 'allergyModal'
};

let COMPONENT_NAME = {
    SelectMedication: 'select-medication',
    ReviewHistory: 'review-hx',
    SelectPatient: 'select-patient'
};

let TASK_TYPES = {
    NonCSChangeRx: 'RXCHG',
    NonCSRefill: 'REFRES',
    ReconcileNonCS: 'RECONCILE_NON_CS'
};

export enum MessageControlType {
    Success = 0,
    Information = 1,
    Error = 2
};

let current_context = {
    PageName: '',
    isException: false,
    isTieEnabled: false,
    Version: (<any>window).version,
    UserPreference: new UserPreferenceModel()
};

let PAGES_TO_HIDE_MENU = [
    'addpatient.aspx',
    PAGE_NAME.DeluxeAccountManagement,
    PAGE_NAME.DeluxeFeatureSelectionPage,
    'deluxebillingpage.aspx',
    'deluxefeatureselectionpage.aspx',
    PAGE_NAME.EditUser,
    'editusers.aspx',
    'privacypatientsearch.aspx',
    'epcsregistration.aspx',
    'epcsdailyactivityreport.aspx',
    'forcepasswordsetup.aspx',
    'epcsreport.aspx',
    PAGE_NAME.HomeAddress,
    'nurseselectmedication',
    'sig.aspx',
    'nursesig.aspx',
    'medication.aspx',
    'rxdurreviewmultiselect.aspx',
    'selectdx.aspx',
    'freeformdrug.aspx',
    'nursemed.aspx',
    'nursefreeformdrug.aspx',
    'messagequeuetx.aspx',
    'selectmedication',
    'featureselection',
    'getepcs'
];


let RIGHT_PANEL_COMPS_TO_SHOW_GIF = [
    'formulary-alternatives',
    'copay-coverage',
    'generic-alternatives'
];

let ERX_CONSTANTS = {
    EmptyGuid: "00000000-0000-0000-0000-000000000000"
}

export class LicenseConstants {
    public static OhioStateCode = "OH";
    public static CertificateToPrescribeCode = "CTP";
}

export class MessageModelTag {
    public static medInvalid: string = "MedicationInvalid";
    public static startUpMessage: string = "StartUpMessage";
    public static gridMessage: string = "GridMessage";
    public static nonCSTaskToCSMed: string = "NonCSTaskToCSMed";
    public static CSTaskSearchDisabledMessage: string = "CSTaskSearchDisabledMessage";
    public static ReconcileRefillSearchDisabledMessage: string = "ReconcileTaskSearchDisabledMessage";
}

export class MessageModelMessages {
    public static CSTaskSerachDisabledMessage: string = "Please select medication from the search results below.  If a change is needed, please deny the task and write a new prescription.";
    public static ReconcileRefillSearchDisabledMessage: string = "Please select medication from the search results below.  If a change is needed, please deny the task and write a new prescription.";
    public static POBNoProviderSelectedMessage: string = "Please select a provider whom you are prescribing on behalf of.";
}

export enum PptDetailContext {
    Unknown = 0,
    ScriptPadMed = 1,
    CandidateMed = 2
}

export enum PPTPlusDetailsUserChangeStatus {
    Fail,
    MedChange,
    NoChange,
    PharmacyChangeOnly,
    PharmacyMissing,
    PriorAuthChangeOnly
}

export enum ColumnSortDirection {
    None = 0,
    Up = 1,
    Down = 2
}

export enum SamlRefreshResult {
    NoChange = 0,
    SessionExtended = 1,
    ForceLogout = 2,
    ReauthenticationRequired = 3
}

export enum Visibility {
    Hidden = 0,
    Visible = 1
}

export enum SelectMedicationReturnAction {
    Nothing = 0,
    ShowFormulary = 1
}

export { EVENT_COMPONENT_ID, current_context, NAVIGATION_EVENT_ID, PAGE_NAME, ROUTE_NAME, API_ROUTE_NAME, ROUTES_WITH_SPINNER, PAGES_TO_HIDE_MENU, ERX_CONSTANTS, RIGHT_PANEL_COMPS_TO_SHOW_GIF, COMPONENT_NAME, MODAL_DISMISS_EVENT, TASK_TYPES };