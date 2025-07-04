import { Component, OnInit, Directive, ViewChild, Output, EventEmitter } from '@angular/core';
import { DomSanitizer } from '@angular/platform-browser';
import {
    ReviewHistoryItem, ReviewHistory, DataRetrievalContext, StatusFilterEnum,
    SortDirectionEnum, FillHistoryTimelineModel, SelectionGroup, CancelRxItemModel, CancelRxActions,
    CancelRxDialogArgs, EieActionRequestModel, SendCancelRxRequestModel, ReviewHistoryStartupParameters, ReviewHistoryProvider,
    ReviewHistoryUserType, ErrorContextModel, ErrorTypeEnum, ApiResponse, MessageIcon, MessageModel, EligibilityMedHistoryModel, PatientMedRecDetailModel,
    ComponentNavigationEventArgs, ColumnSortedEvent, ActionType
} from '../../model/model.import.def';
import { PatientService, EventService, ReviewHistoryService, CancelRxService, PatientMedRecService } from '../../services/service.import.def';
import { EVENT_COMPONENT_ID, NAVIGATION_EVENT_ID, ERX_CONSTANTS, PAGE_NAME, MessageControlType, current_context, COMPONENT_NAME, ColumnSortDirection, ROUTE_NAME } from "../../tools/constants";
import { CancelRxComponent } from '../common/shared/cancel-rx/cancel-rx.component';
import { PatMedAndMedAllergyReconciliationComponent } from '../common/shared/pat-med-and-med-allergy-reconciliation/pat-med-med-allergy-reconciliation.component';
import { EligAndMedHxStatusComponent } from '../common/shared/elig-and-med-hx/elig-and-medhx-status.component';
import '../../../js/jquery.jqtimeline.js';
import { SupervisingProviderPrompt } from '../common/shared/supervising-provider-prompt/supervising-provider-prompt.component';
import { Router, ActivatedRoute, NavigationExtras } from '@angular/router';

@Component({
    selector: 'erx-review-history',
    templateUrl: './review-history.template.html'
})

export class ReviewHistoryComponent implements OnInit {

    @ViewChild(CancelRxComponent) cancelRxControl: CancelRxComponent;
    @ViewChild(PatMedAndMedAllergyReconciliationComponent) medAllergyRecControl: PatMedAndMedAllergyReconciliationComponent;
    @ViewChild(EligAndMedHxStatusComponent) eligMedHxStatusControl: EligAndMedHxStatusComponent;
    @ViewChild(SupervisingProviderPrompt) supervisingProviderPrompt: SupervisingProviderPrompt;

    @Output() isLeagacyContent = new EventEmitter<boolean>();

    public reviewHistory: ReviewHistory;
    public startupParameters: ReviewHistoryStartupParameters;
    public currentState: NavigationExtras["state"]; 

    public IsRestrictedPatient: boolean;

    // buttons
    public isEnterInErrorEnabled: boolean;
    public isCompleteEnabled: boolean;
    public isMakeActiveEnabled: boolean;
    public isDiscontiueEnabled: boolean;
    public componentParameters: string;
    
    public isNextAvailable: boolean;
    public isPreviousAvailable: boolean;
    public pageSize: number = 50;
    
    public currentDataRetrievalContext: DataRetrievalContext = new DataRetrievalContext("RxDate", SortDirectionEnum.DESC, 0, this.pageSize);

    // selection groups
    private completedAndEieItems: string[];
    private sentToPharmAndSelfRepItems: string[];

    public IsMainDatasetLoading: boolean;
    public CurrentStatusFilter: StatusFilterEnum;
    public CancelRxList: Array<CancelRxItemModel>;
    public CurrentDelegateProviderSelection: ReviewHistoryProvider;
    public ActiveMedsPresent: boolean;

    public IsEligAndHistoryStatusInfoLoaded: boolean;
    public IsMedReconciliationInfoLoaded: boolean;
    public IsNavigatingAway: boolean;

    public Messages: Array<MessageModel>;

    public EligibilityMedHistoryInfo: Array<EligibilityMedHistoryModel> = new Array<EligibilityMedHistoryModel>();
    public FillHistoryItems: Array<FillHistoryTimelineModel> = new Array<FillHistoryTimelineModel>();

    public ColumnSortDirection = ColumnSortDirection;

    constructor(private reviewHistorySvc: ReviewHistoryService,
        private ptSvc: PatientService, private evSvc: EventService,
        private cancelRxSvc: CancelRxService, private medRecSvc: PatientMedRecService,
        private router: Router, private activatedroute: ActivatedRoute) {        
        this.currentState = this.router.getCurrentNavigation().extras.state; 
        this.startupParameters = new ReviewHistoryStartupParameters();

        this.reviewHistory = new ReviewHistory();
        this.reviewHistory.HistoryItems = new Array<ReviewHistoryItem>();

        this.completedAndEieItems = new Array<string>();
        this.sentToPharmAndSelfRepItems = new Array<string>();

        this.Messages = new Array<MessageModel>();
    }

    ngOnInit() {               
        this.InitializeRxHistory(this.currentState.param);                              
    }

    public InitializeRxHistory(componentParameters: string) {        
        this.ResetComponentUI();
        this.CurrentStatusFilter = StatusFilterEnum.Active; // default filter is always active only
        this.currentDataRetrievalContext = new DataRetrievalContext("RxDate", SortDirectionEnum.DESC, 0, this.pageSize);// default context
        this.EligibilityMedHistoryInfo = new Array<EligibilityMedHistoryModel>();
        this.IsMedReconciliationInfoLoaded = false;

        // first must set loading to true. then navigating away to false. Otherwise paginator will try to display what's not there yet.
        this.IsMainDatasetLoading = true;
        this.IsNavigatingAway = false;

        this.Messages = new Array<MessageModel>();
        if (componentParameters !== null && componentParameters !== undefined && componentParameters !== 'null') {
            this.Messages.push(new MessageModel(componentParameters, MessageIcon.Success, "", true));
        }
        this.AuditAccessAndGetStartupData();
        this.evSvc.invokeEvent(EVENT_COMPONENT_ID.RefreshPatientHeader, null);
    }
    
    public onSupervisingProviderSelected(data: any) {
        if (data.ErrorContext != null) {
            this.Messages.push(new MessageModel(data.ErrorContext.Message, MessageIcon.Error, "", true));
        }
        else {
            this.NewRxSubsequentAction();
        }
    }

    public NavigateToSelectPatient() {
        this.IsMedReconciliationInfoLoaded = false;
        let page = PAGE_NAME.SelectPatient.toLowerCase();
        this.NavigateToUrl(ROUTE_NAME.SelectPatient, page);
    }

    public ReloadPatientMedications() {
        this.evSvc.invokeEvent(EVENT_COMPONENT_ID.OnPatientMedicationsUpdated, null);
    }
    public GetReviewHistory() {
        // refresh buttons
        this.isCompleteEnabled = false;
        this.isDiscontiueEnabled = false;
        this.isEnterInErrorEnabled = false;

        this.GetReviewHistoryFromService(this.CurrentStatusFilter, this.currentDataRetrievalContext);
    }

    public GetReviewHistoryFromService(statusFilter: StatusFilterEnum, DataRetrievalContext: DataRetrievalContext) {
        this.IsMainDatasetLoading = true;
        this.reviewHistory = new ReviewHistory();
        this.reviewHistory.HistoryItems = new Array<ReviewHistoryItem>();

        this.reviewHistorySvc.GetPatientReviewHistory(statusFilter, DataRetrievalContext).subscribe((response) => {
            this.reviewHistory.HistoryItems = response.HistoryItems;
            this.isNextAvailable = response.MoreRowsAvailable;
            this.ActiveMedsPresent = response.ActiveMedsPresent;
            this.isPreviousAvailable = this.currentDataRetrievalContext.SkipRows > 0;

            this.IsMainDatasetLoading = false;
        });
    }

    //Place to clean up code when the context changes (whenever EVENT_COMPONENT_ID.LoadReviewHistory is invoked)
    private ResetComponentUI() {
        this.ActiveMedsPresent = true;  //Default value for ActiveMedsPresent is true (this has ui dependency)
    }

    private AuditAccessAndGetStartupData() {
        this.reviewHistorySvc.AuditAccessAndGetStartupParameters().subscribe((response) => {
            this.startupParameters = response;
            this.IsRestrictedPatient = this.startupParameters.IsRestrictedPatient;
            // set provider selection
            if (this.startupParameters.UserType === ReviewHistoryUserType.PAwithSupervision) {
                if (this.startupParameters.DelegateProviderId !== "") {
                    // in some cases supervising provider is not on the list of providers (bug). Adding resiliency to that condition
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
            this.GetReviewHistory();
            this.GetPatientMedRecInfo();
        });
    }

    public GetDataWithFilter(statusFilter: StatusFilterEnum) {
        this.CurrentStatusFilter = statusFilter;
        this.currentDataRetrievalContext.SkipRows = 0;
        this.GetReviewHistory();
    }

    public NavigateToMultipleViewReport() {
        this.NavigateToUrl(ROUTE_NAME.MultipleViewReport, PAGE_NAME.MultipleViewReport);
    }

    public NavigateToSelfReportedMed() {
        this.NavigateToUrl(ROUTE_NAME.SelfReportedMed, PAGE_NAME.SelfReportedMed);
    }

    public NavigateToUrl(route: string, page: string) {
        // clear current data
        this.IsNavigatingAway = true;
        this.reviewHistory = new ReviewHistory();       
        this.router.navigateByUrl(route, { state: { navigateTo: page } });
    }

    showEligAndMedHxStatus() {
        this.eligMedHxStatusControl.Show();
    }

    showPatMedAndMedAllergyReconciliation() {
        this.medAllergyRecControl.Show();
    }

    ShowFillDetailOverlay(item: ReviewHistoryItem) {
        // make the service call 
        this.reviewHistorySvc.GetFillHistoryData(item.RxID).subscribe((response) => {
            this.FillHistoryItems = response;
        });
        $("#mdlFillHistoryDetails").modal('toggle');
    };

    public PopulateFillHistoryDetails(item: ReviewHistoryItem) {

        item.DisplayFillDetails = !item.DisplayFillDetails;

        if (item.FillDetailsHtml == null) {

            item.FillDetailsHtml = "Loading...";

            // make the service call 
            this.reviewHistorySvc.GetFillHistoryData(item.RxID).subscribe((response) => {
                if (response == null) {
                    item.FillDetailsHtml = null;
                    return;
                }

                var points = new Array();
                response.forEach(timelinePoint => {
                    var pointDate = new Date(timelinePoint.FillDate.toString());
                    var point = { name: timelinePoint.TimeLineLabel, on: pointDate };
                    points.push(point);
                });

                var startDate = new Date(item.RxDate.toString());
                var tmpDiv = document.createElement('div');
                $(tmpDiv).jqtimeline({ events: points, numYears: 1, startOn: startDate, gap: 60 });

                item.FillDetailsHtml = tmpDiv.innerHTML;
            });
        }
    }

    public onSorted($event: ColumnSortedEvent) {
        if ($event.sortColumn === this.currentDataRetrievalContext.SortColumnName) {
            // sorting by the same column, change sort direction
            this.currentDataRetrievalContext.ReverseSort(this.pageSize);
        } else {
            // sorting by another column, reset the context
            this.currentDataRetrievalContext = new DataRetrievalContext($event.sortColumn, SortDirectionEnum.ASC, 0, this.pageSize);
        }

        this.GetReviewHistory();
    }

    public selectionChanged(historyItem) {
        historyItem.Selected = !historyItem.Selected;
        this.ShowRxDetails(historyItem);
        this.ApplyEnabledByCurrentSelection();
    }

    public ShowRxDetails(historyItem) {
        let rxId:string = historyItem.Selected ? historyItem.RxID : null;
        this.evSvc.invokeEvent(EVENT_COMPONENT_ID.OnHistoryRxSelected, rxId );
    }
    public ApplyEnabledByCurrentSelection() {

        var numOfSelectedItems = this.reviewHistory.HistoryItems.filter(item => item.Selected).length;

        if (numOfSelectedItems === 0) {
            this.reviewHistory.HistoryItems.forEach(item => item.IsSelectionDisabled = false);
            // disable all action buttons
            this.isDiscontiueEnabled = false;
            this.isCompleteEnabled = false;
            this.isEnterInErrorEnabled = false;
            return; // exit early if nothing selected
        }

        if (numOfSelectedItems > 1) {// we have already applied the disabling logic, just exit
            return;
        }

        // determine selection group
        var currentSelectionGroup = this.reviewHistory.HistoryItems.find(item => item.Selected).SelectionGroupId;

        this.reviewHistory.HistoryItems.forEach(item => {
            if (item.SelectionGroupId !== currentSelectionGroup) {
                item.IsSelectionDisabled = true;
            }

            this.ApplyButtonsEnabledState(currentSelectionGroup);
        });
    }

    private ApplyButtonsEnabledState(selectedGroup: SelectionGroup) {
        switch (selectedGroup)
        {
            case SelectionGroup.Eie:
                {
                    this.isCompleteEnabled = false;
                    this.isDiscontiueEnabled = false;
                    this.isEnterInErrorEnabled = true;
                }
                break;
            case SelectionGroup.DiscontinueComplete:
                {
                    this.isCompleteEnabled = true;
                    this.isDiscontiueEnabled = true;
                    this.isEnterInErrorEnabled = false;
                }
                break;
            case SelectionGroup.DiscontinueEieComplete:
                {
                    this.isCompleteEnabled = true;
                    this.isDiscontiueEnabled = true;
                    this.isEnterInErrorEnabled = true;
                }
                break;
            case SelectionGroup.EieComplete:
                {
                    this.isCompleteEnabled = true;
                    this.isDiscontiueEnabled = false;
                    this.isEnterInErrorEnabled = true;
                }
                break;
            default:
                {
                    this.isCompleteEnabled = false;
                    this.isDiscontiueEnabled = false;
                    this.isEnterInErrorEnabled = false;
                }
        }
    }

    public FetchNextPage() {
        this.currentDataRetrievalContext.SkipRows += this.pageSize;
        this.GetReviewHistory();
    }

    public FetchPreviousPage() {
        this.currentDataRetrievalContext.SkipRows -= this.pageSize;
        this.GetReviewHistory();
    }

    public GetPaginatorVerbiage() {
        if (this.reviewHistory.HistoryItems == undefined)
            return "No Data";

        var totalRetrieved = this.reviewHistory.HistoryItems.length;

        if (totalRetrieved === 0) {
            return "No Prescription History";
        }
        var startRow = this.currentDataRetrievalContext.SkipRows + 1;
        var paginatorText = "Displayed Items " + startRow + " through " + (totalRetrieved + startRow - 1) + "  (" + totalRetrieved + " Items).  ";
        if (this.isNextAvailable) {
            paginatorText += "&nbsp;&nbsp;&nbsp;&nbsp;<b>More Items Available</b>";
        }
        return paginatorText;
    }
    public CurrentlyExecutingActionName: string;

    public ExecuteAction(actionName: string) {
        this.CurrentlyExecutingActionName = actionName;
        this.CancelRxList = this.reviewHistory.HistoryItems.filter(item => item.Selected && item.IsScriptCancelRxEligible).map(item => new CancelRxItemModel(item.RxID, item.Prescription, item.OriginalNewRxTrnsCtrlNo));

        if (this.CancelRxList.length > 0) {
            this.cancelRxControl.PrepareToLaunch(actionName);
            $("#CancelRxTemplate").modal('toggle');
        } else {
            this.ExecuteActionButton(this.CurrentlyExecutingActionName);

        }
    }

    public NewRxClicked() {        
        if (this.CurrentDelegateProviderSelection !== null && this.CurrentDelegateProviderSelection !== undefined) {
            if (this.CurrentDelegateProviderSelection.UserTypeID == 1001) {
                this.supervisingProviderPrompt.retrieveSupervisingProviders();
            }
            else {
                this.NewRxSubsequentAction();
            }
        }
        else {
            this.NewRxSubsequentAction();
        }
    }

    public NewRxSubsequentAction() {
        this.reviewHistorySvc.DoesCurrentPatientHaveInactivatedAllergies().subscribe((response) => {
            if (response === true) {
                this.ShowSelectAllergyDialog();
            }
            else {
                this.router.navigateByUrl(ROUTE_NAME.SelectMedication);
            }
        });
    }

    public NewDiagnosisClicked() {
        this.reviewHistorySvc.DoesCurrentPatientHaveInactivatedAllergies().subscribe((response) => {
            if (response === true) {
                this.ShowSelectAllergyDialog();
            } else {
                this.NavigateToUrl(ROUTE_NAME.SelectDiagnosis, PAGE_NAME.SelectDiagnosis);
            }
        });
    }

    public CancelRxDialogSelectionCompleted(args: CancelRxDialogArgs) {
        $("#CancelRxTemplate").modal('hide');

        if (args.Action === CancelRxActions.Back) { return; } // do nothing 
        this.ExecuteActionButton(this.CurrentlyExecutingActionName);

        if (args.Action === CancelRxActions.ContinueWithCancelRx) {

            var itemsToCancel = args.Items.map(item => new SendCancelRxRequestModel(item.RxID, item.OriginalNewRxTrnsCtrlNo));

            if (itemsToCancel.length > 0) {
                this.cancelRxSvc.SendCancelRx(itemsToCancel).subscribe(() => {
                    this.ReloadPatientMedications();
                });
            }
        }
    }
    private CompleteExecutedActions(actionName: string, response: ApiResponse) {
        var selectedItems = this.reviewHistory.HistoryItems.filter(item => item.Selected);
        switch (actionName) {
            case "Complete":
                {
                    this.GetReviewHistory();
                    if (response.ErrorContext) {
                        this.Messages.push(new MessageModel("Error when executing Complete. " + response.ErrorContext.Message, MessageIcon.Error, "", true));
                    }
                    this.ReloadPatientMedications();
                    this.GetPatientMedRecInfo();
                    break;
                }
            case "Discontinue":
                {
                    this.GetReviewHistory();
                    if (response.ErrorContext) {
                        this.Messages.push(new MessageModel("Error when executing Discontinue. " + response.ErrorContext.Message, MessageIcon.Error, "", true));
                    }
                    this.ReloadPatientMedications();
                    this.GetPatientMedRecInfo();
                    break;
                }
            case "EIE":
                {
                    if (response.ErrorContext) {
                        this.Messages.push(new MessageModel("Action Cancelled. Error when checking Active EPA tasks association. " + response.ErrorContext.Message, MessageIcon.Error, "", true));
                    } else {
                        if (response.Payload !== true) {
                            this.reviewHistorySvc.ExecuteEieAction(selectedItems.map(item => new EieActionRequestModel(item.RxID, item.IsPbmMed))).subscribe(() => {
                                this.GetReviewHistory();
                            });
                        } else {
                            let epaMessage = "This action cannot be performed as one or more of the selected prescription(s) are associated with active prior authorization task.";
                            if (this.Messages.filter(_ => _.Message === epaMessage).length === 0) {
                                this.Messages.push(new MessageModel(epaMessage, MessageIcon.Error, "", true));
                            }
                        }
                    }
                    this.ReloadPatientMedications();
                    this.GetPatientMedRecInfo();
                    break;
                }
        }
    }
    private ExecuteActionButton(actionName: string) {
        var selectedItems = this.reviewHistory.HistoryItems.filter(item => item.Selected);
        switch (actionName) {
            case "Complete":
                {
                    //load current selected patient 
                    this.reviewHistorySvc.ExecuteCompleteAction(selectedItems.map(item => item.RxID)).subscribe((response) => {
                        this.CompleteExecutedActions(actionName, response);
                    });
                    break;
                }
            case "Discontinue":
            {
                    this.reviewHistorySvc.ExecuteDiscontinueAction(selectedItems.map(item => item.RxID)).subscribe((response) => {
                        this.CompleteExecutedActions(actionName, response);
                    });
                    break;
                }
            case "EIE":
            {
                    this.reviewHistorySvc.IsAnyOfSelectedMedsAssociatedWithActiveEpaTask(selectedItems.map(item => item.RxID)).subscribe((response) => {
                        this.CompleteExecutedActions(actionName, response);
                });
                break;
            }
        }
        //DONOT PUT ANYCODE HERE, Instead place it insubscribe of individual methods
    }

    public SetNoActiveMedsFlag() {
        this.reviewHistorySvc.SetNoActiveMedsFlag().subscribe(() => {
            var successMessage = this.Messages.find(item => item.Tag === "NoActMedFlag");
            if (successMessage === undefined || successMessage === null) {
                this.Messages.push(new MessageModel("No Active Medications flag is applied successfully.", MessageIcon.Success, "NoActMedFlag", true));
            }
            this.GetPatientMedRecInfo();
            this.ReloadPatientMedications();    
        });
    }

    ShowSelectAllergyDialog() {
        this.ptSvc.PopupNavigation.PopupSrc = 'PatientAllergy.aspx';
        this.ptSvc.PopupNavigation.PopupTitle = "Some of the patient's Allergies have been inactivated. Please select valid Allergy";
        (<any>window)['isModalPopup'] = true;
    }

    DelegateProviderSelected(provider: ReviewHistoryProvider) {
        if (provider.ProviderId === "") {
            this.Messages.push(new MessageModel("Please Select valid Provider", MessageIcon.Information, "", true));
        } else {
            this.reviewHistorySvc.AssignSupervisingProvider(provider.ProviderId).subscribe();
        }
    }

    NavigateToSelectPatientForPOB() {
        // clear current patient context
        this.ptSvc.GetPatientHeader(ERX_CONSTANTS.EmptyGuid).subscribe((response) => {
            this.ptSvc.SelectedPatient = response
        });

        this.NavigateToUrl(ROUTE_NAME.SelectPatient, PAGE_NAME.SelectPatient);
    }

    public PatientMedRecInfo: PatientMedRecDetailModel = new PatientMedRecDetailModel();

    GetPatientMedRecInfo() {
        this.medRecSvc.GetEligibilityAndMedHistoryStatus().subscribe((response) => {
            this.IsMedReconciliationInfoLoaded = true;
            this.PatientMedRecInfo = new PatientMedRecDetailModel();        
            this.PatientMedRecInfo = response;
            this.PostReconcileMessageToMessageBanner();
        });
    }

    PostReconcileMessageToMessageBanner() {
        if (this.Messages.length === 0) {
            this.Messages.push(new MessageModel(this.PatientMedRecInfo.ReconciliationMessage, MessageIcon.Information, "medrec", true));
        } else {
            var medRecMessage = this.Messages.find(item => item.Tag === 'medrec');
            if (medRecMessage !== undefined && medRecMessage !== null) {
                medRecMessage.Message = this.PatientMedRecInfo.ReconciliationMessage;
            } else {
                this.Messages.push(new MessageModel(this.PatientMedRecInfo.ReconciliationMessage, MessageIcon.Information, "medrec", true));
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
}