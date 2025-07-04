import { Component, ViewChild, Input } from '@angular/core';
import {
    SearchPatientStartupParameters, DelegateProvider, SearchPatientUserType, PatientSearchBarModel, Patient, SearchPatientUserCategory,
    MessageIcon, MessageModel, PatientMedRecDetailModel, SearchPatientResponse, PatientItemModel, SearchPatientDataRequest,
    CheckInPatientDataRequest, ColumnSortedEvent, SetProviderInformationRequest, LoadProvidersForSupervisedPARequest, NextPage, ActionType,
    RefreshHeaderDTO, PatientContextDTO
} from '../../model/model.import.def';
import {
    PatientService, EventService, PatientMedRecService, SelectMedicationService, SelectPatientService, PrivacyOverrideService, ClientSortService, ContentLoadService
} from '../../services/service.import.def';
import { EVENT_COMPONENT_ID, current_context, ColumnSortDirection, PAGE_NAME, ROUTE_NAME } from "../../tools/constants";


import { WelcomeTourComponent } from '../shared/welcome-tour/welcome-tour.component';
import { DeluxeTeaserAdComponent } from '../shared/deluxe-teaser-ad/deluxe-teaser-ad.component';
import { CreditCardExpiringComponent } from '../common/shared/credit-card-expiring-warning/creditcard-expiring.component';
import { SupervisingProviderComponent } from '../common/shared/supervising-provider-overlay/supervising-provider.component';
import Searchpatientsmodel = require("../../model/search-patients/search-patients.model");
import GetStartupParametersRequest = Searchpatientsmodel.GetStartupParametersRequest;
import PatientDemographics = Searchpatientsmodel.PatientDemographics;
import { PatMedAndMedAllergyReconciliationComponent } from '../common/shared/pat-med-and-med-allergy-reconciliation/pat-med-med-allergy-reconciliation.component';
import { PdmpEnrollmentComponent } from '../pdmp-enrollment/pdmp-enrollment.component';
import { pdmpEpcsTeaserComponent } from '../pdmp-epcs-teaser/pdmp-epcs-teaser.component';
import SelectPatientComponentParameters = Searchpatientsmodel.SelectPatientComponentParameters;
import { Router, NavigationExtras } from '@angular/router';
import Constants = require("../../tools/constants");


@Component({
    selector: 'erx-select-patient',
    templateUrl: './select-patient.template.html',
    styleUrls: ['../../../Style/SearchPatient/Default.css']
})

export class SearchPatientComponent {
    @ViewChild(WelcomeTourComponent) welcomeTourComponent: WelcomeTourComponent;
    @ViewChild(DeluxeTeaserAdComponent) deluxeTeaserAdComponent: DeluxeTeaserAdComponent;
    @ViewChild(CreditCardExpiringComponent) creditcardExpiringComponent: CreditCardExpiringComponent;
    @ViewChild(SupervisingProviderComponent) supervisingProviderComponent: SupervisingProviderComponent;
    @ViewChild(PatMedAndMedAllergyReconciliationComponent) medAllergyRecControl: PatMedAndMedAllergyReconciliationComponent;
    @ViewChild(PdmpEnrollmentComponent) pdmpEnrollment: PdmpEnrollmentComponent;
    @ViewChild(pdmpEpcsTeaserComponent) pdmpTeaser: pdmpEpcsTeaserComponent;
    
    public startupParameters: SearchPatientStartupParameters;
    public SearchPatientUserType: any = SearchPatientUserType;
    public selectedPatientId: string;
    public isInitialLoad: boolean = false;
    public isNavigatingAway: boolean;
    public isMedReconciliationInfoLoaded: boolean;
    public hasInactivePatients: boolean;
    public selectedInactivePatient: boolean;
    public CurrentDelegateProviderSelection: DelegateProvider;
    public Messages: Array<MessageModel>;
    public ColumnSortDirection = ColumnSortDirection;
    public nextPage: NextPage;
    public PatientDemographics: PatientDemographics;
    public PatientMedRecInfo: PatientMedRecDetailModel;
    public isInitialLoading: boolean = true;
    public resetSearchBar: boolean = false;
    public componentParameters: SelectPatientComponentParameters & MessageModel;
    public currentState: NavigationExtras["state"];
    @Input()
    set hideDeluxeTeaserAd(hide: boolean) {
        if (hide)
            this.deluxeTeaserAdComponent.closeModal();
    }

    constructor(private selectPatientSvc: SelectPatientService, private ptSvc: PatientService,
        private evSvc: EventService, private medRecSvc: PatientMedRecService,
        private prSvc: PrivacyOverrideService,
        private fsSvc: SelectMedicationService,
        private clientsSortService: ClientSortService,
        private router: Router, private contentLoadService: ContentLoadService) {
        
        this.isNavigatingAway = false;
        this.isMedReconciliationInfoLoaded = false;

        this.startupParameters = new SearchPatientStartupParameters();
        this.startupParameters.IsAddDiagnosisVisible = false;
        this.startupParameters.UserType = SearchPatientUserType.Provider;
        this.startupParameters.SearchPatientResponse = new SearchPatientResponse();
        this.startupParameters.SearchPatientResponse.Patients = new Array<PatientItemModel>();

        this.selectedPatientId = "";

        this.Messages = new Array<MessageModel>();

        this.CurrentDelegateProviderSelection = new DelegateProvider();
        this.CurrentDelegateProviderSelection.ProviderId = "";

        this.isInitialLoad = true;
        this.hasInactivePatients = false;
        this.selectedInactivePatient = false;
        this.currentState = this.router.getCurrentNavigation()?.extras?.state;
    }

    ngOnInit() {
        if (this.currentState) {
            this.componentParameters = this.currentState.patientInfo;
        }
        this.loadSelectPatientComponent();
        this.resetPatientSearchBar();
        if (Object.entries(this.contentLoadService.initalContentPayload).length != 0) {
            this.processStartupData(this.contentLoadService.initalContentPayload.SelectPatientPayload, false, true);
        }
        else {
            this.getStartupDataAndProcess();
        }
    }

    private loadSelectPatientComponent() {
        current_context.PageName = PAGE_NAME.SelectPatient;
        this.ResetSelectPatientComponent();

        this.evSvc.invokeEvent(EVENT_COMPONENT_ID.OnPatientSelected, false);

        let selectPatientComponentParameters = this.componentParameters;


        if (selectPatientComponentParameters !== null && selectPatientComponentParameters !== undefined) {

            if (selectPatientComponentParameters.MessageText !== null || selectPatientComponentParameters.MessageText !== '') {
                if (selectPatientComponentParameters.MessageIcon === 'Success') {
                    this.Messages.push(new MessageModel(selectPatientComponentParameters.MessageText, MessageIcon.Success, selectPatientComponentParameters.MessageText, true));
                }
                else {
                    if (!((typeof selectPatientComponentParameters.MessageText === 'undefined') || (selectPatientComponentParameters.MessageText === null)) /*Push only if MessageText not undefined or null*/
                        && !this.Messages.some(_ => _.Message === selectPatientComponentParameters.Message)) {
                        this.Messages.push(selectPatientComponentParameters);
                    }
                }
            }

            this.PatientDemographics = selectPatientComponentParameters.PatientDemographics;
        }
    }

    public getStartupDataAndProcess() {
        let getStartupParametersRequest: GetStartupParametersRequest = new GetStartupParametersRequest();
        getStartupParametersRequest.PatientDemographics = this.currentState?.patientInfo?.PatientDemographics;
        this.selectPatientSvc.GetStartupParameters(getStartupParametersRequest).subscribe((response) => {
            let forceResetPatienSelection = this.preSelectedPatient();
            this.processStartupData(response, forceResetPatienSelection);
        });
    }

    public preSelectedPatient(): boolean {
        let forceResetPatienSelection: boolean = false;
        forceResetPatienSelection = this.componentParameters === undefined || this.componentParameters === null || this.componentParameters.PatientId === null;
        return forceResetPatienSelection;
    }

    private resetPatientSelection(): void {
        this.ptSvc.SelectedPatient = new Patient();
        this.evSvc.invokeEvent(EVENT_COMPONENT_ID.RefreshPatientHeader, <RefreshHeaderDTO>{ isToClearHeader: true });
        this.initializeGridNoPatientSelected();
    }

    processStartupData(startupPayload: SearchPatientStartupParameters, forceResetPatienSelection: boolean = false, isInitialLoad: boolean = false) {
        if (startupPayload && Object.entries(startupPayload).length >0) {

            this.startupParameters = startupPayload;

            this.selectedPatientId = this.startupParameters.PatientID;

            if (forceResetPatienSelection == true) {
                this.selectedPatientId = '';
            }

            if (startupPayload.Message  && startupPayload.Message.Message  && !this.Messages.some(_ => _.Message === startupPayload.Message.Message)) {
                this.Messages.push(startupPayload.Message);
            }

            if (this.startupParameters.UserType === SearchPatientUserType.PAwithSupervision) {
                this.setSelectedDelegateProvider();
            }
            if (this.startupParameters.PatientID  && (!forceResetPatienSelection || isInitialLoad)) {
                this.setSelectedPatient(this.selectedPatientId);
                this.ReloadGridWithPatientIdFromPatientService(this.startupParameters.PatientID);
            }
            else if (!isInitialLoad) {
                this.resetPatientSelection();
            }


            // rename IsPrivacyOverrideDisplayed property
            if (this.startupParameters.IsPrivacyOverrideDisplayed) {
                this.ptSvc.SelectedPatient = new Patient();
            }

            this.showTeaserAd();

        } else {
            // todo: do something more appropriate, display a message?
            this.ResetSelectPatientComponent();
        }
    }
    public showTeaserAd() {
        if (this.startupParameters.IsWelcomeTourDisplayed) {
            this.showWelcomeTour();
        } else if (this.startupParameters.IsDeluxeTeaserAdDisplayed) {
            this.showDeluxeTeaserAd();
        } else if (this.startupParameters.IsCreditCardExpiring) {
            this.showCreditCardExpiringWarning();
        }

        if (this.startupParameters.DeluxeTeaserAdModel) {
            if (this.startupParameters.DeluxeTeaserAdModel.Cookie  && this.startupParameters.DeluxeTeaserAdModel.Cookie.length > 0) {
                document.cookie = this.startupParameters.DeluxeTeaserAdModel.Cookie;
            }
        }
    }

    private setSelectedDelegateProvider() {
        if (this.startupParameters.DelegateProviderId !== "") {
            var currentSuper = this.startupParameters.Providers.find(item => item.ProviderId === this.startupParameters.DelegateProviderId);

            if (currentSuper != undefined) {
                this.CurrentDelegateProviderSelection = currentSuper;
            } else {
                this.CurrentDelegateProviderSelection = this.startupParameters.Providers[0];
            }

            this.DelegateProviderSelected(this.CurrentDelegateProviderSelection);
        } else {
            this.CurrentDelegateProviderSelection = this.startupParameters.Providers[0]; // 'select provider'
        }
    }

    private showWelcomeTour() {
        this.welcomeTourComponent.welcomeTourModel = this.startupParameters.WelcomeTourModel;
        this.welcomeTourComponent.OpenModal();
    }

    private showDeluxeTeaserAd() {
        this.deluxeTeaserAdComponent.DeluxeTeaserAdModel = this.startupParameters.DeluxeTeaserAdModel;
        this.deluxeTeaserAdComponent.openModal();
    }

    private showCreditCardExpiringWarning() {
        this.creditcardExpiringComponent.OpenModal();
    }

    public showPDMPEnrollment() {
        this.pdmpEnrollment.ShowPdmpEnrollmentForm();
    }

    public showEpcsTeaser() {
        this.pdmpTeaser.ShowPdmpEnrollmentForm();
    }

    onEPCSTeaserClose(isVisible: boolean) {
        this.isInitialLoading = this.isInitialLoading ? false : this.isInitialLoading;
        this.router.navigateByUrl(ROUTE_NAME.DeluxeFeatures);
    }
    public onPatientSearchSubmit(searchParams: PatientSearchBarModel) {
        // clear selected patient
        this.selectedPatientId = '';
        this.ptSvc.SelectedPatient = new Patient();

        this.isInitialLoad = false;
        this.nextPage = null;

        var request = new SearchPatientDataRequest();
        request.ChartID = searchParams.PatientId;
        request.LastName = searchParams.LastName;
        request.FirstName = searchParams.FirstName;
        request.DateOfBirth = searchParams.DateOfBirth;
        request.UserType = SearchPatientUserType.Provider;
        request.IncludeInactive = searchParams.IncludeInactive;
        request.PatientGuid = null;

        this.GetPatientsFromService(request);
    }

    public GetPatientsFromService(request: SearchPatientDataRequest) {

        this.selectPatientSvc.SearchPatients(request).subscribe((response) => {
            // maybe only delete med rec messages
            let patientsResp: SearchPatientResponse = response;
            this.Messages = new Array<MessageModel>();

            if (patientsResp.Patients.length === 0) {
                this.startupParameters.SearchPatientResponse.Patients = null;
                this.Messages.push(new MessageModel("Your search returned no results. Please change your search criteria.", MessageIcon.Information, "Refine Search Criteria", true));
            } else {
                this.startupParameters.SearchPatientResponse.Patients = patientsResp.Patients;
                var inactivePatients = patientsResp.Patients.filter(p => p.StatusID === '0');
                this.hasInactivePatients = inactivePatients.length > 0;

                if (patientsResp.Patients.length > 50) {
                    this.Messages.push(new MessageModel("Your search returned more than 50 results. Please consider refining your search.", MessageIcon.Information, "Refine Search Criteria", true));
                }
            }
            

            this.evSvc.invokeEvent(EVENT_COMPONENT_ID.RefreshPatientHeader, <RefreshHeaderDTO>{ isToClearHeader: true });
        });
    }

    private setSelectedPatientInGrid(patientId: string) {
        if (this.startupParameters.SearchPatientResponse !== undefined && this.startupParameters.SearchPatientResponse.Patients !== undefined) {
            for (let i = 0; i < this.startupParameters.SearchPatientResponse.Patients.length; i++) {
                if (this.startupParameters.SearchPatientResponse.Patients[i] !== undefined && this.startupParameters.SearchPatientResponse.Patients[i].PatientID === patientId) {
                    if (!this.startupParameters.SearchPatientResponse.Patients[i].Selected) {
                        this.startupParameters.SearchPatientResponse.Patients[i].Selected = true;
                        break;
                    } else {
                        this.startupParameters.SearchPatientResponse.Patients[i].Selected = false;
                    }
                }
            }
        }
        this.evSvc.invokeEvent(EVENT_COMPONENT_ID.OnPatientSelected, <PatientContextDTO>{ containsPatientContext: true });
    }

    //
    // Button Click Events
    //
    public CheckInClicked(providerId: string) {
        var request = new CheckInPatientDataRequest();
        request.ProviderId = providerId;
        this.selectPatientSvc.CheckInPatient(request).subscribe((response) => {
            if (response !== null && response !== undefined) {
                this.Messages.push(new MessageModel(response.CheckedInMessage, MessageIcon.Information, "", response.CheckedInMessageVisibility));
            }
        });
    }

    public ReviewHistoryClicked() {
        this.nextPage = NextPage.ReviewHistory;

        if (this.startupParameters.UserType === SearchPatientUserType.PAwithSupervision) {
            if (this.CurrentDelegateProviderSelection.ProviderId === "") {
                this.AddErrorMessageIfNotExists(new MessageModel(Constants.MessageModelMessages.POBNoProviderSelectedMessage, MessageIcon.Error, "", true));
            } else {
                if (this.ptSvc.SelectedPatient.IsRestrictedPatient) {
                    this.prSvc.CheckIsRestrictedUser(this.selectedPatientId, this.CurrentDelegateProviderSelection.ProviderId).subscribe((response) => {
                        if (response.IsRestrictedUser) {
                            this.showPrivacyOverridePopup();
                        } else {
                            this.navigateToReviewHistory();
                        }
                    });
                } else {
                    this.navigateToReviewHistory();
                }
            }
        } else {
            this.navigateToReviewHistory();
        }
    }

    public NavigateToUrl(route: string, page: string) {
        this.isNavigatingAway = true;
        this.startupParameters.SearchPatientResponse = new SearchPatientResponse();
        this.router.navigateByUrl(route, { state: { navigateTo: page } });
    }

    public NewDiagnosisClicked() {
        let selectDiagnosisUrl = PAGE_NAME.SelectDiagnosis.toLowerCase();
        this.NavigateToUrl(ROUTE_NAME.SelectDiagnosis, selectDiagnosisUrl)
    }

    public NavigateToAddPatientPage() {
        this.NavigateToUrl(ROUTE_NAME.AddPatient, PAGE_NAME.AddPatient_AddMode.toLowerCase());
    }

    public NewRxClicked() {
        this.nextPage = NextPage.SelectMed;
        this.fsSvc.ClearSelectedDx().subscribe();
        if (this.startupParameters.UserType === SearchPatientUserType.PAwithSupervision) {
            if (this.CurrentDelegateProviderSelection.ProviderId === "") {
                this.Messages.push(new MessageModel("Please select a provider whom you are prescribing on behalf of.", MessageIcon.Error, "", true));
            } else {
                if (this.ptSvc.SelectedPatient.IsRestrictedPatient) {
                    this.prSvc.CheckIsRestrictedUser(this.selectedPatientId, this.CurrentDelegateProviderSelection.ProviderId).subscribe((response) => {
                        if (response.IsRestrictedUser) {
                            this.showPrivacyOverridePopup();
                        } else {
                            this.setProviderInformation(this.CurrentDelegateProviderSelection.ProviderId, this.CurrentDelegateProviderSelection.UserCategory)
                                .then(isDelegateProviderSet => {
                                    if (isDelegateProviderSet) {
                                        this.NavigateToSelectMed();
                                    }
                                });
                        }
                    });
                } else {
                    this.setProviderInformation(this.CurrentDelegateProviderSelection.ProviderId, this.CurrentDelegateProviderSelection.UserCategory)
                        .then(isDelegateProviderSet => {
                            if (isDelegateProviderSet) {
                                if (this.CurrentDelegateProviderSelection.UserCategory === SearchPatientUserCategory.PHYSICIAN_ASSISTANT_SUPERVISED) {
                                    this.showSupervisingProviderPopup(this.CurrentDelegateProviderSelection.ProviderId);
                                } else {
                                    this.NavigateToSelectMed();
                                }
                            }
                        });
                }
            }
        } else {
            this.NavigateToSelectMed();
        }
    }

    public setProviderInformation(delegateProviderId: string, userCategory: SearchPatientUserCategory) {
        return new Promise(resolve => {
            let request = new SetProviderInformationRequest();
            request.ProviderId = delegateProviderId;
            this.selectPatientSvc.SetProviderInformation(request).subscribe((response) => {
                if (!response) {
                    this.Messages.push(new MessageModel("Please select a provider with a valid NPI.",
                        MessageIcon.Error,
                        "",
                        true));
                }
                resolve(response);
            });
        });
    }

    DocumentVisitClicked() {
        this.medAllergyRecControl.Show();
    }

    public navigateToReviewHistory() {
        current_context.PageName = PAGE_NAME.ReviewHistory;
        this.router.navigateByUrl(ROUTE_NAME.ReviewHistory, { state: { param: 'null' } });
        if (this.ptSvc.PopupNavigation !== null && this.ptSvc.PopupNavigation !== undefined) {
            this.ptSvc.PopupNavigation.ContentSrc = '';
        }
        this.isMedReconciliationInfoLoaded = false;

    };

    public NavigateToSelectMed() {
        current_context.PageName = PAGE_NAME.SelectMedication;
        if (this.ptSvc.PopupNavigation !== null && this.ptSvc.PopupNavigation !== undefined) {
            this.ptSvc.PopupNavigation.ContentSrc = '';
        }
        this.router.navigateByUrl(ROUTE_NAME.SelectMedication);
    }

    //
    //Grid Action Events
    //
    getSortedPatients(criteria: ColumnSortedEvent) {
        this.startupParameters.SearchPatientResponse.Patients = this.clientsSortService.getSortedResults(criteria, this.startupParameters.SearchPatientResponse.Patients);
    }

    onSorted($event) {
        this.getSortedPatients($event);
    }

    public onPatientSelected(patientItem: PatientItemModel) {
        this.nextPage = null;

        if (this.Messages !== undefined && this.Messages !== null) {
            this.Messages = this.Messages.filter(m => m.Tag !== 'medrec');
        }

        // clear the current select row
        for (let i = 0; i < this.startupParameters.SearchPatientResponse.Patients.length; i++) {
            this.startupParameters.SearchPatientResponse.Patients[i].Selected = false;
        }

        patientItem.Selected = true;    // changing this property will update the css style in the grid row
        this.selectedPatientId = patientItem.PatientID;
        this.selectedInactivePatient = patientItem.StatusID === '0';

        this.evSvc.invokeEvent(EVENT_COMPONENT_ID.OnPatientSelected, <PatientContextDTO>{ containsPatientContext: false });


        this.prSvc.CheckIsRestrictedUser(patientItem.PatientID, this.startupParameters.UserID).subscribe((response) => {
            if (response.IsRestrictedUser) {
                this.showPrivacyOverridePopup();
            } else {
                this.setSelectedPatient(patientItem.PatientID);
            }
        });
    }

    public showSupervisingProviderPopup(delegateProviderId: string) {
        let request = new LoadProvidersForSupervisedPARequest();
        request.ProviderId = delegateProviderId;

        this.selectPatientSvc
            .LoadProvidersForSupervisedPA(request)
            .subscribe((providers: DelegateProvider[]) => {
                if (providers !== undefined) {
                    this.supervisingProviderComponent.providers = providers;
                    this.supervisingProviderComponent.OpenModal();
                } else {
                    this.Messages.push(new MessageModel("Could not load providers.",
                        MessageIcon.Error,
                        "",
                        true));
                }
            });
    }

    showPrivacyOverridePopup() {
        this.evSvc.invokeEvent(EVENT_COMPONENT_ID.OnPatientSelected, <PatientContextDTO>{ containsPatientContext: true, isPrivacyPatient: true });
    }

    setSelectedPatient(patientId: string) {
        this.Messages = this.Messages.filter(_ => _.Tag === 'taskAlert');

        this.selectPatientSvc.SetPatientInfo(patientId).subscribe(response => {
            if (response !== undefined) {
                this.ptSvc.SelectedPatient = response;
                this.GetPatientMedRecInfo(patientId);
            } else {
                this.Messages.push(new MessageModel("Ooops... could not get patient data. Please try later.", MessageIcon.Information, "GetPatientError", true));
            }
            this.evSvc.invokeEvent(EVENT_COMPONENT_ID.OnPatientSelected, <PatientContextDTO>{ containsPatientContext: true });
        });
    }


    DelegateProviderSelected(provider: DelegateProvider) {
        if (provider.ProviderId === "") {
            this.Messages.push(new MessageModel("Please Select valid Provider", MessageIcon.Information, "", true));
        } else {
            // todo: set clinical_msg_delegate_provider session var?
            this.CurrentDelegateProviderSelection = provider;
        }
    }

    PrivacyOverrideSelectionComplete(action: string) {
        if (action === "override") {
            if (this.nextPage === undefined || this.nextPage === null) {
                this.setSelectedPatient(this.selectedPatientId);
            } else {
                if (this.startupParameters.UserType === SearchPatientUserType.PAwithSupervision) {
                    if (this.CurrentDelegateProviderSelection.ProviderId === "") {
                        this.Messages.push(new MessageModel("Please select a provider whom you are prescribing on behalf of.", MessageIcon.Error, "", true));
                    } else {
                        this.setProviderInformation(this.CurrentDelegateProviderSelection.ProviderId, this.CurrentDelegateProviderSelection.UserCategory)
                            .then(returnValue => {
                                if (returnValue) {
                                    if (this.nextPage === NextPage.SelectMed) {
                                        if (this.CurrentDelegateProviderSelection.UserCategory === SearchPatientUserCategory.PHYSICIAN_ASSISTANT_SUPERVISED) {
                                            this.showSupervisingProviderPopup(this.CurrentDelegateProviderSelection.ProviderId);
                                        } else {
                                            this.NavigateToSelectMed();
                                        }
                                    } else if (this.nextPage === NextPage.ReviewHistory) {
                                        this.navigateToReviewHistory();
                                    }
                                }
                            });
                    }
                }
            }
        } else if (action === "cancel") {
            // clear selected patient
            this.selectedPatientId = "";

            // referesh patient header
            this.ptSvc.ResetSelectedPatient();

            for (let i = 0; i < this.startupParameters.SearchPatientResponse.Patients.length; i++) {
                this.startupParameters.SearchPatientResponse.Patients[i].Selected = false;
            }

            this.evSvc.invokeEvent(EVENT_COMPONENT_ID.RefreshPatientHeader, null);
        }
    }

    //
    // Patient Med Allergy Reconciliation events
    //
    GetPatientMedRecInfo(patientId: string) {
        this.medRecSvc.GetEligibilityAndMedHistoryStatus().subscribe((response) => {
            if (response !== undefined && response !== null) {
                this.PatientMedRecInfo = response;
                this.isMedReconciliationInfoLoaded = true;
                if (patientId !== null && patientId !== undefined) {
                    this.PostReconcileMessageToMessageBanner();
                }
            }
        });
    }

    PostReconcileMessageToMessageBanner() {
        if (this.Messages !== null) {
            if (this.Messages.length === 0) {
                this.Messages.push(new MessageModel(this.PatientMedRecInfo.ReconciliationMessage, MessageIcon.Information, "medrec", true));
            }
            else {
                var medRecMessage = this.Messages.find(item => item.Tag === 'medrec');
                if (medRecMessage !== undefined && medRecMessage !== null) {
                    medRecMessage.Message = this.PatientMedRecInfo.ReconciliationMessage;
                } else {
                    this.Messages.push(new MessageModel(this.PatientMedRecInfo.ReconciliationMessage, MessageIcon.Information, "medrec", true));
                }
            }
        }
    }

    PatientMedAllergySelectionCompleted(medRecModel: PatientMedRecDetailModel) {
        this.medRecSvc.UpdatePatientMedRecDetail(medRecModel.Type).subscribe((response) => {
            if (medRecModel.ActionType === ActionType.SavePrescribe) {
                this.NewRxClicked();
            } else {
                this.PatientMedRecInfo = response;
                this.PostReconcileMessageToMessageBanner();
            }
        });
    }

    //
    //Generic UI Methods
    //
    public ResetSelectPatientComponent() {
        this.isNavigatingAway = false;
        this.isMedReconciliationInfoLoaded = false;
        this.Messages = new Array<MessageModel>();
    }

    // coming from some page and we have a patient id in session, or the add/edit patient popup/page
    public ReloadGridWithPatientIdFromPatientService(patientId: string) {
        this.selectedPatientId = patientId;
        let selectedPatient: PatientItemModel =
            this.startupParameters.SearchPatientResponse.Patients.find(p => p.PatientID === patientId);
        this.selectedInactivePatient = selectedPatient.StatusID === '0';

        // sets selected patient row in grid with selected css style, emits patient load complete event
        this.setSelectedPatientInGrid(patientId);
    }

    private resetPatientSearchBar(): void {
        this.resetSearchBar = false;
        setTimeout(() => this.resetSearchBar = true, 0);
    }

    //initializes the patient grid with no selection 
    public initializeGridNoPatientSelected(): void {
        this.resetPatientSearchBar();
        this.setSelectedPatientInGrid(null);
    }




    private AddErrorMessageIfNotExists(targetMessage: MessageModel) {

        var containsMessage = false;

        this.Messages.forEach(function (item) {
            if (item.Message === targetMessage.Message)
                containsMessage = true;
        });

        if (!containsMessage)
            this.Messages.push(targetMessage);
    }
}