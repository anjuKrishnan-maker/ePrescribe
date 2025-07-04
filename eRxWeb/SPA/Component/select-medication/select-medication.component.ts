import { Component, ViewChild, OnInit } from '@angular/core';
import { EVENT_COMPONENT_ID, NAVIGATION_EVENT_ID, PAGE_NAME, MessageModelTag, TASK_TYPES, ROUTE_NAME } from '../../tools/constants';
import { EventService, SelectMedicationService, PatientService, ComponentCommonService, ContentLoadService } from '../../services/service.import.def';
import {
    MessageIcon, MessageModel, PatientCoverageHeader as CoverageModel, PatientCoverageHeaderList, SelectMedicationDataRequest, SelectMedicationRequestType,
    SelectMedicationMedModel, SelectMedicationSelections, SelectMedicationSearchModel, SelectMedicationSearchArgs, PatientRecordStatus, SelectMedicationGridPopulationCompletionArgs,
    PptPlusSummaryRequest, UpdatePatientSelectedCoverageRequest, SelectMedicationGridEvent, SelectMedicationStartUpModel, SetRequestedMedicationAsCurrentMedicationRequest,
    MedicationDTO, SelectedMedicationRows, MedicationSelectedPayload, SelectMedicationNavigationDTO
} from '../../model/model.import.def';
import { SelectMedicationSearchComponent } from './select-medication-search/select-medication-search.component';
import { MessageComponent } from '../common/shared/message/message.component';
import { EventSubscriptionDisposer } from '../common/root.component';
import { EligAndMedHxStatusComponent } from '../common/shared/elig-and-med-hx/elig-and-medhx-status.component';
import Constants = require("../../tools/constants");
import { Subscription } from 'rxjs';
import { SelectMedicationGridComponent } from './select-medication-grid/select-medication-grid.component';
import { PatientMedHistoryComponent } from '../common/shared/patient-medication-history/patient-medication-history.component';
import { Router, NavigationExtras } from '@angular/router';
@Component({
    selector: 'erx-select-medication',
    templateUrl: './select-medication.template.html',
    styleUrls: ['./select-medication.style.css']
})

export class SelectMedicationComponent extends EventSubscriptionDisposer implements OnInit {

    @ViewChild(SelectMedicationSearchComponent) selectMedicationSearchControl: SelectMedicationSearchComponent;
    @ViewChild(MessageComponent) messageComponent: MessageComponent;
    @ViewChild(EligAndMedHxStatusComponent) eligMedHxStatusControl: EligAndMedHxStatusComponent;
    @ViewChild(SelectMedicationGridComponent) selectMedicationGridComponent: SelectMedicationGridComponent;
    @ViewChild(PatientMedHistoryComponent) patientMedHistoryComponent: PatientMedHistoryComponent;

    public IsNavigatingAway: boolean;
    public SelectMedicationMessageStrip: Array<MessageModel> = [];
    public StaticMessageStrip: Array<MessageModel> = [];
    public StaticMessageRxStrip: Array<MessageModel> = [];
    public hasDiagnosis: boolean;
    public diagnosisName: string;
    public isGridDataLoading: boolean = true;
    public parentComponent: string = PAGE_NAME.SelectPatient;

    public SearchText: string;
    public CoverageList: CoverageModel[];
    public CoverageId: string;
    public requestData: SelectMedicationDataRequest;
    MedicationSearchCriterion: SelectMedicationSearchModel;
    PatientRecordStatusFilter: PatientRecordStatus;
    public isMedSelectedFromGrid: boolean = false;
    public selectMedicationModel: SelectMedicationMedModel[];
    public SelectedMedFilter: SelectMedicationRequestType;
    public tasktype: string;
    public freeFormText: string = "Click to Write Free Form Rx";
    public isFreeFormEnabled: boolean = true;
    public currentState: NavigationExtras["state"];
    patientHistory: number = SelectMedicationRequestType.PatientHistory;
    active: number = PatientRecordStatus.Active;
    inActive: number = PatientRecordStatus.InActive;
    both: number = PatientRecordStatus.Both;
    coverageChangeOnly: boolean = false;
    autoRefreshCoverageSubscription: Subscription;
    autoRefreshCoverageTimeout: any;
    isAutoEligiblityCheckInprogress: boolean = false;

    constructor(private fsSvc: SelectMedicationService, private evSvc: EventService,
        private compSvc: ComponentCommonService,
        private contLoadSvc: ContentLoadService, private router: Router) {
        super();
        this.currentState = this.router.getCurrentNavigation()?.extras?.state;
        let thisObj = this;
        this.compSvc.AddWindowFunction('MedicineSelected', function (value) {
            thisObj.medicineSelected(value);
        });
    }

    ngOnInit(): void {
        this.initiateSelectMedicationComponent(this.currentState?.searchText as SelectMedicationNavigationDTO);
    }

    private initiateSelectMedicationComponent(componentParameters: SelectMedicationNavigationDTO): void {
        this.ResetSelectMedicationUI();

        this.SetSelectMedicationStartUpData();
        this.selectMedicationSearchControl.GetStartupData("");
        this.IsNavigatingAway = false;
        if (componentParameters !== undefined && componentParameters !== null) {
            this.tasktype = componentParameters.tasktype;
            if (this.tasktype == TASK_TYPES.NonCSChangeRx) {
                this.DisableFreeForm();
            }
            if (componentParameters.from !== undefined &&
                (componentParameters.from === PAGE_NAME.ApproveRefillTaskPage ||
                    componentParameters.from === PAGE_NAME.PharmRefillSummaryPage)) {
                this.parentComponent = componentParameters.from;
                if (this.tasktype != TASK_TYPES.NonCSChangeRx && this.tasktype != TASK_TYPES.ReconcileNonCS) {
                    this.setUpCSTask();
                }
                else if (this.tasktype == TASK_TYPES.ReconcileNonCS) {
                    this.setUpReconcileRefillTask();
                }
            }
            if (componentParameters.RefillPharmacy !== undefined) {
                this.StaticMessageStrip.push(new MessageModel(
                    "Renewal Request Pharmacy : " + componentParameters.RefillPharmacy,
                    MessageIcon.Information,
                    "selectMedicationStartUpMessages",
                    false));
            } else if (componentParameters.ChangeRxPharmacy !== undefined) {
                this.StaticMessageStrip.push(new MessageModel(
                    "Change Rx Pharmacy : " + componentParameters.ChangeRxPharmacy,
                    MessageIcon.Information,
                    "selectMedicationStartUpMessages",
                    false));
                let request = new SetRequestedMedicationAsCurrentMedicationRequest();
                request.ScriptMessageGuid = componentParameters.ScriptMessageGuid;
                request.RequestedRxDrugDescription = componentParameters.SearchText;
                request.RxDetails = componentParameters.RxDetails;
                this.fsSvc.SetRequestedMedicationAsCurrentMedication(request).subscribe(() => {
                    if (componentParameters.SearchText !== undefined &&
                        componentParameters.SearchText !== null) {
                        this.selectMedicationSearchControl.GetStartupData(componentParameters.SearchText);
                        this.evSvc.invokeEvent(EVENT_COMPONENT_ID.ReloadScriptPadPanel, null);
                    }
                });
            }
            if (componentParameters.RxDetails !== undefined) {
                this.StaticMessageRxStrip.push(new MessageModel("Rx Detail : " + componentParameters.RxDetails,
                    MessageIcon.Information,
                    "selectMedicationStartUpMessages",
                    false));
            }
            if (componentParameters.SearchText !== undefined && componentParameters.SearchText !== null) {
                this.MedicationSearchCriterion = new SelectMedicationSearchModel(componentParameters.SearchText, SelectMedicationRequestType.AllMedication, "");
                this.selectMedicationSearchControl.GetStartupData(componentParameters.SearchText);
            } else {
                this.selectMedicationSearchControl.GetStartupData("");
            }


        } else {
            this.parentComponent = PAGE_NAME.SelectPatient;
            this.selectMedicationSearchControl.GetStartupData("");
        }
    }

    DisableFreeForm() {
        this.freeFormText = "Write Free Form is currently disabled"
        this.isFreeFormEnabled = false;
    }
    enableFreeForm() {
        this.freeFormText = "Click to Write Free Form Rx"
        this.isFreeFormEnabled = true;
    }

    selectSigPressedAction() {
        //Reset the search bar
        this.enableFreeForm();
        //Remove NonCS->CS tasking error message and the 'cannot change med' message and the reconcile med message
        this.SelectMedicationMessageStrip = this.SelectMedicationMessageStrip.filter(x => x.Tag !== MessageModelTag.nonCSTaskToCSMed && x.Tag !== MessageModelTag.CSTaskSearchDisabledMessage && x.Tag !== MessageModelTag.ReconcileRefillSearchDisabledMessage);
        return;
    }

    ResetSelectMedicationUI() {
        this.CoverageList = null;
        this.CoverageId = null;
        this.StopAutoEligbilityCheck();

        this.diagnosisName = "";
        this.hasDiagnosis = false;
        this.StaticMessageStrip = [];
        this.StaticMessageRxStrip = [];
        this.MedicationSearchCriterion = new SelectMedicationSearchModel("", SelectMedicationRequestType.PatientHistory, "");
        this.PatientRecordStatusFilter = PatientRecordStatus.Active;
        this.selectMedicationGridComponent.GridReset();
    }

    StopAutoEligbilityCheck() {
        this.isAutoEligiblityCheckInprogress = false;
        clearTimeout(this.autoRefreshCoverageTimeout);
        if (this.autoRefreshCoverageSubscription !== undefined) {
            this.autoRefreshCoverageSubscription.unsubscribe();
        }
    }

    SetDiagnosisInfo(data: string) {
        this.diagnosisName = data;
        this.hasDiagnosis = (data.length > 0);
    }

    RemoveDiagnosis(isDiagnosisRemoved: boolean) {
        this.hasDiagnosis = !isDiagnosisRemoved;
        this.fsSvc.ClearSelectedDx().subscribe(data => data);
    }

    public NavigateToFreeFormDrug() {
        this.NavigateToUrl(ROUTE_NAME.FreeFormDrug, PAGE_NAME.FreeFormDrug);
    }

    public NavigateToUrl(route: string, page: string) {
        this.IsNavigatingAway = true;
        this.router.navigateByUrl(route, { state: { navigateTo: page } });
    }

    public showPatientMedHistoryPopup() {
        this.patientMedHistoryComponent.showPatientMedHistory();
        return false;
    }

    coverageChange(coverageId: string, shouldUpdateCoverage: boolean = true) {
        this.CoverageId = coverageId;

        let request = new UpdatePatientSelectedCoverageRequest();
        request.CoverageId = coverageId;
        this.coverageChangeOnly = true;
        if (shouldUpdateCoverage) {
            this.fsSvc.UpdatePatientSelectedCoverage(request).subscribe(() => {
                this.PopulateGrid();
            });
        }
        else {
            this.PopulateGrid();
        }
    }

    showEligAndMedHxStatus() {
        this.eligMedHxStatusControl.Show();
    }

    PopulateGrid() {
        this.UpdatePatientRecordStatus();
        this.requestData = new SelectMedicationDataRequest();
        this.requestData.RequestFor = this.MedicationSearchCriterion.MedicationSearchOption;
        this.requestData.SearchText = this.MedicationSearchCriterion.MedSearchText;
        this.requestData.GroupName = this.MedicationSearchCriterion.PreBuiltGroupName;
        this.requestData.PatientRecordStatus = this.PatientRecordStatusFilter;
        this.evSvc.invokeEvent(EVENT_COMPONENT_ID.OnMedicationDeSelected, null);
    }

    OnMedicationSelected(data: SelectMedicationSelections) {
        let allSelectedMeds = data.SelectedMeds.filter(x => this.selectMedicationModel.find(y => y.index === x.index) == undefined);
        this.isMedSelectedFromGrid = data.SelectedMeds.length > 0;
        this.SelectMedicationMessageStrip = this.SelectMedicationMessageStrip.filter(x => x.Tag !== MessageModelTag.gridMessage);
        this.fsSvc.ValidateMedSection(data.SelectedMeds).subscribe((messageModel) => {
            if (messageModel.Message != undefined &&
                this.MedicationSearchCriterion.MedicationSearchOption !== SelectMedicationRequestType.AllMedication) {
                var medInvalidIndex = this.SelectMedicationMessageStrip.findIndex(x => x.Tag === MessageModelTag.medInvalid);
                if (medInvalidIndex >= 0) {
                    this.SelectMedicationMessageStrip[medInvalidIndex] = messageModel;
                } else {
                    this.SelectMedicationMessageStrip.push(messageModel);
                }
            } else {
                this.SelectMedicationMessageStrip = this.SelectMedicationMessageStrip.filter(x => x.Tag !== MessageModelTag.medInvalid);
            }
            let selectedMedicationRows: SelectedMedicationRows = this.getSelectedRows(data);

            if ([SelectMedicationGridEvent.MedSelected, SelectMedicationGridEvent.AllMedSelected, SelectMedicationGridEvent.MedModified].indexOf(data.EventContext.GridEvent) > -1) {
                let pptRequests = this.getPptRequest(data, allSelectedMeds);
                let isSelectedMedicationModified = data.EventContext.GridEvent == SelectMedicationGridEvent.MedModified
                this.onMedicineSelected(data, selectedMedicationRows, pptRequests, isSelectedMedicationModified);
            } else if ([SelectMedicationGridEvent.MedDeselected, SelectMedicationGridEvent.AllMedDeSelected].indexOf(data.EventContext.GridEvent) > -1) {
                this.evSvc.invokeEvent<SelectedMedicationRows | null>(EVENT_COMPONENT_ID.OnMedicationDeSelected, selectedMedicationRows);
            }

            this.selectMedicationModel = data.SelectedMeds;
        });

        //Remove error message if selecting a non-cs med
        const csMedIndex = allSelectedMeds.findIndex((currentMed) => {
            if (currentMed.ControlledSubstanceCode != "") {
                return currentMed;
            }
        })
        //If no cs med found to be selected, remove the NonCS->CS error (if exists)
        if (csMedIndex == -1) {
            this.SelectMedicationMessageStrip = this.SelectMedicationMessageStrip.filter(x => x.Tag !== MessageModelTag.nonCSTaskToCSMed);
        }
    }

    private getPptRequest(fssData: SelectMedicationSelections, currentSelectedMeds: SelectMedicationMedModel[]): PptPlusSummaryRequest[] {
        let requests: PptPlusSummaryRequest[] = [];

        if (fssData.EventContext.GridEvent == SelectMedicationGridEvent.AllMedSelected) {

            for (let i = 0; i < currentSelectedMeds.length; i++) {
                requests.push(this.GetPptPlusSummaryRequest(currentSelectedMeds[i]));
            }
        }
        else if ((fssData.EventContext.GridEvent == SelectMedicationGridEvent.MedSelected) ||
            (fssData.EventContext.GridEvent == SelectMedicationGridEvent.MedModified)) {
            requests.push(this.GetPptPlusSummaryRequest(fssData.EventContext.CurrentMed));
        }
        return requests;
    }

    initiatePptRequest(fssData: SelectMedicationSelections, currentSelectedMeds: SelectMedicationMedModel[]) {
        let requests: PptPlusSummaryRequest[] = [];

        if (fssData.EventContext.GridEvent == SelectMedicationGridEvent.AllMedSelected) {

            for (let i = 0; i < currentSelectedMeds.length; i++) {
                requests.push(this.GetPptPlusSummaryRequest(currentSelectedMeds[i]));
            }
        }
        else if ((fssData.EventContext.GridEvent == SelectMedicationGridEvent.MedSelected) ||
            (fssData.EventContext.GridEvent == SelectMedicationGridEvent.MedModified)) {
            requests.push(this.GetPptPlusSummaryRequest(fssData.EventContext.CurrentMed));
        }
        return requests;
    }

    private onMedicineSelected(fssData: SelectMedicationSelections, selectedMedicationRows: SelectedMedicationRows,
        pptPlusSummaryRequest: PptPlusSummaryRequest[], isSelectedMedicationModified: boolean) {
        let selectedMedication = this.createMedicineSelectedInfo(fssData, selectedMedicationRows, pptPlusSummaryRequest, isSelectedMedicationModified);
        this.medicineSelected(selectedMedication);
    }

    private medicineSelected(selectedMedication: MedicationDTO & SelectedMedicationRows) {
        this.evSvc.invokeEvent(EVENT_COMPONENT_ID.MedicationSelecting, null);
        this.contLoadSvc.RetrieveMedicationLoadedPayload(selectedMedication).subscribe(medLoadedPayload => {
            medLoadedPayload.PptPlusSummaryRequest = selectedMedication.pptPlusSummaryRequest;
            medLoadedPayload.selectedMedicationRowIndex = selectedMedication.selectedMedicationRowIndex;
            this.evSvc.invokeEvent<MedicationSelectedPayload>(EVENT_COMPONENT_ID.OnMedicationSelected, medLoadedPayload);
        });
    }

    public createMedicineSelectedInfo(fssData: SelectMedicationSelections, selectedMedicationRows: SelectedMedicationRows,
        pptPlusSummaryRequest: PptPlusSummaryRequest[], isSelectedMedicationModified: boolean): MedicationDTO & SelectedMedicationRows {
        let isSpecialtyMedication: boolean = fssData.EventContext.CurrentMed.IsSpecialtyMed != null ? fssData.EventContext.CurrentMed.IsSpecialtyMed : false;
        let formularyStatus = fssData.EventContext.CurrentMed.FormularyStatus;
        let selectedMedication = <MedicationDTO & SelectedMedicationRows>{
            DDI: fssData.EventContext.CurrentMed.DDI,
            formularyStatus: formularyStatus != undefined && formularyStatus.toString() != "NaN" ? formularyStatus : 0,
            medName: fssData.EventContext.CurrentMed.DrugName != undefined ? fssData.EventContext.CurrentMed.DrugName : '',
            taskScriptMessageId: "",
            isSpecialtyMedication: isSpecialtyMedication,
            selectedMedicationRowIndex: selectedMedicationRows.selectedMedicationRowIndex,
            pptPlusSummaryRequest: pptPlusSummaryRequest,
            isSelectedMedicationsModified: isSelectedMedicationModified
        };

        return selectedMedication;
    }


    private getSelectedRows(selectedMedication: SelectMedicationSelections): SelectedMedicationRows {
        let selectedMedicationRows = <SelectedMedicationRows>{
            selectedMedicationRowIndex: selectedMedication.SelectedMeds.map(u => u.index)
        };
        return selectedMedicationRows;
    }

    private GetPptPlusSummaryRequest(fsMeds: SelectMedicationMedModel): PptPlusSummaryRequest {
        let pptRequest = new PptPlusSummaryRequest();
        pptRequest.DDI = fsMeds.DDI;
        pptRequest.GPPC = fsMeds.GPPC ? fsMeds.GPPC : "";
        pptRequest.PackUom = fsMeds.PackageUOM ? fsMeds.PackageUOM : "";
        pptRequest.Quantity = fsMeds.Quantity ? String(fsMeds.Quantity) : "0";
        pptRequest.PackSize = fsMeds.PackageSize ? fsMeds.PackageSize : "0";
        pptRequest.PackQuantity = fsMeds.PackageQuantity ? fsMeds.PackageQuantity : "0";
        pptRequest.Refills = fsMeds.Refill ? String(fsMeds.Refill) : "0";
        pptRequest.DaysSupply = fsMeds.DayOfSupply ? String(fsMeds.DayOfSupply) : "0";
        pptRequest.IsDaw = fsMeds.DAW ? String(fsMeds.DAW) : "false";
        pptRequest.MedSearchIndex = String(fsMeds.index);
        pptRequest.Medname = pptRequest.IsDaw.toLowerCase() == "false" ? fsMeds.DrugName : fsMeds.MedicationName;
        pptRequest.Strength = fsMeds.Strength;
        pptRequest.MedExtension = fsMeds.Strength.trim() + ' ' + fsMeds.StrengthUOM.trim() + ' ' + fsMeds.DosageForm.trim();
        pptRequest.RowSelectionStatus = fsMeds.RowStatus;
        pptRequest.Index = fsMeds.index;
        return pptRequest;

    }

    SetSelectMedicationStartUpData() {
        this.fsSvc.GetSelectMedicationStartUpData().subscribe((data: SelectMedicationStartUpModel) => {
            this.SetSelectMedicationStartUpMessages(data.Messages);
            this.SetPatientCoverages(data.PatientCoverages.PatientCoverageHeaders);
            this.CheckAndSetAutoRefreshPatientCoverages(data.PatientCoverages);
            this.SetDiagnosisInfo(data.PatientDiagnosis);
            this.PopulateGrid();
        });
    }

    SetPatientCoverages(data: CoverageModel[]) {
        this.CoverageList = data;
        if (this.CoverageList.length > 0) {
            this.CoverageId = this.CoverageList[0].ID;
        }
    }

    CheckAndSetAutoRefreshPatientCoverages(data: PatientCoverageHeaderList) {
        clearTimeout(this.autoRefreshCoverageTimeout);
        if (data.InvokeAgainDelayMilliseconds > 0) {
            this.isAutoEligiblityCheckInprogress = true;
            this.autoRefreshCoverageTimeout = setTimeout(() => {
                this.AutoRefreshPatientCoverages()
            }, data.InvokeAgainDelayMilliseconds);
        }
        else {
            this.isAutoEligiblityCheckInprogress = false;
        }
    }

    AutoRefreshPatientCoverages() {
        if (this.autoRefreshCoverageSubscription !== undefined) {
            this.autoRefreshCoverageSubscription.unsubscribe();
        }

        this.autoRefreshCoverageSubscription = this.fsSvc.GetPatientCoveragesInfo().subscribe((data: PatientCoverageHeaderList) => {
            this.CoverageList = data.PatientCoverageHeaders;
            let tempCoverage = data.PatientCoverageHeaders.filter(x => x.ID == this.CoverageId);
            if (tempCoverage == undefined || tempCoverage.length == 0) {
                if ((data.PatientCoverageHeaders.length > 0)
                    || ((this.CoverageId != undefined
                        && this.CoverageId != null)
                        && this.CoverageId.length > 0)) {
                    this.SelectMedicationMessageStrip.push(
                        new MessageModel("Due to mismatch of already loaded to recently recieved patient's coverage information, Medication list is auto refreshed",
                            MessageIcon.Information,
                            MessageModelTag.gridMessage, true));

                    if (data.PatientCoverageHeaders.length > 0) {
                        this.coverageChange(data.PatientCoverageHeaders[0].ID, false);
                    }
                    else if (this.CoverageId != undefined
                        && this.CoverageId != null
                        && this.CoverageId.length > 0) {
                        this.coverageChange(null, false);
                    }
                }
            }
            this.CheckAndSetAutoRefreshPatientCoverages(data);
        });
    }

    SetSelectMedicationStartUpMessages(data: MessageModel[]) {
        if (data != undefined) {
            this.SelectMedicationMessageStrip = this.SelectMedicationMessageStrip.filter(x => x.Tag !== MessageModelTag.startUpMessage);
            data.forEach(message => {
                message.Tag = MessageModelTag.startUpMessage;
                this.SelectMedicationMessageStrip.push(message);
            }, this.SelectMedicationMessageStrip);
        }
    }

    MedSearchCriteriaChanged(data: SelectMedicationSearchArgs) {
        let emptySearchErrorMessage = "Please supply search criteria for All Meds search, then click Search.";
        this.SelectMedicationMessageStrip = this.SelectMedicationMessageStrip.filter(x => x.Tag !== MessageModelTag.medInvalid);
        if (data.MedicationSearchCriteria.MedicationSearchOption == SelectMedicationRequestType.AllMedication
            && data.MedicationSearchCriteria.MedSearchText.length <= 0 && data.MedicationSearchCriteria.Validate == true) {
            this.displayMessage(new MessageModel(emptySearchErrorMessage, MessageIcon.Error, "", false));
            this.selectMedicationGridComponent.GridReset();
        }
        else {
            this.SelectMedicationMessageStrip = this.SelectMedicationMessageStrip.filter(item => item.Message !== emptySearchErrorMessage);
            this.MedicationSearchCriterion = data.MedicationSearchCriteria;
            this.SelectedMedFilter = this.MedicationSearchCriterion.MedicationSearchOption;
            this.PopulateGrid();
        }
    }

    SelectMedicationFilters_OnClick(patRecordStatusFilter: PatientRecordStatus) {
        this.PatientRecordStatusFilter = patRecordStatusFilter;
        this.MedicationSearchCriterion.MedSearchText = "";
        this.selectMedicationSearchControl.RefreshControls();
        this.PopulateGrid();
    }

    UpdatePatientRecordStatus() {
        if ((this.MedicationSearchCriterion.MedicationSearchOption === SelectMedicationRequestType.PatientHistory)
            && (this.MedicationSearchCriterion.MedSearchText.trim().length > 0)) {
            this.PatientRecordStatusFilter = PatientRecordStatus.Both;
        }
    }

    GridPopulationComplete(data: SelectMedicationGridPopulationCompletionArgs) {
        this.MedicationSearchCriterion.MedicationSearchOption = data.RequestFor;
        this.selectMedicationSearchControl.RefreshControls();
        this.SelectedMedFilter = data.RequestFor;
        this.PatientRecordStatusFilter = data.PatientRecordStatus;
        this.SelectMedicationMessageStrip = this.SelectMedicationMessageStrip.filter(x => x.Tag !== MessageModelTag.gridMessage);
        data.Messages.forEach(message => {
            message.Tag = MessageModelTag.gridMessage;
            this.SelectMedicationMessageStrip.push(message);
        }, this.SelectMedicationMessageStrip);
        let tempSelectMedication = this.selectMedicationModel;
        this.selectMedicationModel = [];
        this.isMedSelectedFromGrid = false;

        if (this.coverageChangeOnly) {
            this.coverageChangeOnly = false;
            if (tempSelectMedication != null && tempSelectMedication.length > 0) {
                this.selectMedicationGridComponent.AutoSelectItem(tempSelectMedication);
                this.SelectMedicationMessageStrip.push(
                    new MessageModel("Medication information refreshed with new/selected Coverage, Medication list selection retained",
                        MessageIcon.Information,
                        MessageModelTag.gridMessage, true));
            }
            else {
                this.SelectMedicationMessageStrip.push(
                    new MessageModel("Medication information refreshed with new/selected Coverage.",
                        MessageIcon.Information,
                        MessageModelTag.gridMessage, true));
            }
        }

        this.isGridDataLoading = false;
    }

    displayMessage(data: MessageModel) {
        this.SelectMedicationMessageStrip = [];
        if (data != undefined) {
            this.SelectMedicationMessageStrip.push(data);
        }
    }

    setUpCSTask() {
        if (this.SelectMedicationMessageStrip.find(x => x.Tag == MessageModelTag.CSTaskSearchDisabledMessage) === undefined) {
            var message = new MessageModel(
                Constants.MessageModelMessages.CSTaskSerachDisabledMessage,
                MessageIcon.Information,
                Constants.MessageModelTag.CSTaskSearchDisabledMessage,
                false
            );
            this.SelectMedicationMessageStrip.push(message);
        }
        this.DisableFreeForm();
    }

    setUpReconcileRefillTask() {
        if (this.SelectMedicationMessageStrip.find(x => x.Tag == MessageModelTag.ReconcileRefillSearchDisabledMessage) === undefined) {
            var message = new MessageModel(
                Constants.MessageModelMessages.ReconcileRefillSearchDisabledMessage,
                MessageIcon.Information,
                Constants.MessageModelTag.ReconcileRefillSearchDisabledMessage,
                false
            );
            this.SelectMedicationMessageStrip.push(message);
        }
        this.DisableFreeForm();
    }
}