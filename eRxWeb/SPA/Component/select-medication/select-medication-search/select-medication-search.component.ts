import { Component, ElementRef, Input, Output, EventEmitter, AfterViewInit } from '@angular/core';
import { EVENT_COMPONENT_ID, current_context, NAVIGATION_EVENT_ID } from '../../../tools/constants';
import { EventService, SelectMedicationSearchService, ComponentCommonService } from '../../../services/service.import.def';
import { SelectMedicationSearchModel, SelectMedicationSearchArgs, ApiResponse, SelectMedicationSearchStartUpParameters, SelectMedicationRequestType, RetrieveTaskTypeParameters, PrescriptionTaskType } from '../../../model/model.import.def';

@Component({
    selector: 'erx-select-medication-search',
    templateUrl: './select-medication-search.template.html',
    styleUrls: ['./select-medication-search.style.css']
})

export class SelectMedicationSearchComponent {
    private medicationSearch: SelectMedicationSearchModel;
    private readonly searchMedicationDefault: string = "Select Medication";
    private readonly searchGroupDefault: string = "Select Group";
    @Input()
    set MedicationSearch(data: SelectMedicationSearchModel) {
        if (typeof (data) === 'undefined') {
            this.medicationSearch = new SelectMedicationSearchModel("", SelectMedicationRequestType.PatientHistory, "");
        } else {
            this.medicationSearch = data;
        }
        this.RefreshControls();
    }
    get MedicationSearch() {
        return this.medicationSearch;
    }

    @Output()
    OnMedicationSearchCriteriaChanged: EventEmitter<SelectMedicationSearchArgs> = new EventEmitter<SelectMedicationSearchArgs>();

    patientHistory: number = SelectMedicationRequestType.PatientHistory;
    providerHistory: number = SelectMedicationRequestType.ProviderHistory;
    allMedication: number = SelectMedicationRequestType.AllMedication;
    preBuiltGroup: number = SelectMedicationRequestType.PreBuiltGroup;

    TaskType: RetrieveTaskTypeParameters;
    StartupParameters: SelectMedicationSearchStartUpParameters;
    PreBuiltGroupList: string[];
    PreBuiltGroupDropDownItems: string[];
    PreBuiltGroupName: string;
    PreBuiltGroupDropDownCurrentItemName: string;
    PreBuiltGroupDropDownCurrentItemIndex: number;
    PreBuiltGroupTimeOut: any;

    SearchMedicationNameRequest: any;
    MedicationList: string[];
    MedicationDropDownItems: string[];
    MedicationName: string;
    MedicationDropDownCurrentItemName: string;
    MedicationDropDownCurrentItemIndex: number;
    MedicationDropDownVisible: boolean
    MedicationTimeOut: any;
    showOnlyAllMeds: boolean;


    constructor(private evSvc: EventService, private medSearchService: SelectMedicationSearchService, private componentService: ComponentCommonService) {
        this.MedicationList = null;
        this.PreBuiltGroupList = null;
        this.StartupParameters = null;
        this.TaskType = new RetrieveTaskTypeParameters();
        this.TaskType.PrescriptionTaskType = PrescriptionTaskType.DEFAULT;
        this.TaskType.IsCsRefReqWorkflow = false;
        this.TaskType.IsCsRxChgWorkflow = false;

        this.componentService.AddWindowFunction('clearSelectMedicationPrebuiltList', () => {
            this.PreBuiltGroupList = null;
        });
        /*evSvc.subscribeEvent(EVENT_COMPONENT_ID.LoadSelectMedication, (componentParameters: any) => {
            this.SetStartUpParameters();
        });*/
    }
    GetStartupData(medicationName: string) {
        this.RetrieveTaskType();
        this.SetStartUpParameters();
        this.SetPrebuiltPrescriptionGroupList();
        this.MedicationName = medicationName;
    }

    RetrieveTaskType() {
        this.medSearchService.RetrieveTaskType().subscribe(
            tl => {
                this.TaskType = tl;
                this.showOnlyAllMeds = this.TaskType.PrescriptionTaskType === 8 || this.TaskType.PrescriptionTaskType === 9 || this.TaskType.IsCsRefReqWorkflow || this.TaskType.IsCsRxChgWorkflow || this.TaskType.IsReconcileREFREQNonCS;
            }
        );
    }

    SetStartUpParameters() {
        
        this.medSearchService.GetStartUpParameters().subscribe(
            tl => {
                this.StartupParameters = tl;
            }
        );
        
    }

    SetPrebuiltPrescriptionGroupList() {
        if (this.PreBuiltGroupList == null) {
            this.medSearchService.GetPrebuiltPrescriptionGroupNames().subscribe(
                tl => {
                    this.PreBuiltGroupList = tl;
                }
            );
        }
    }

    SetMedicationDropDownItems() {
        this.MedicationList = new Array();

        if (this.MedicationName.trimLeft().length >= 4) {

            if (this.SearchMedicationNameRequest) {
                this.DiscardPreviousSearchMedicationNameRequest();
            }

            this.SearchMedicationNameRequest = this.medSearchService.SearchMedicationName(this.MedicationName).subscribe(response => {
                if (response != undefined && response.length > 0) {
                    this.MedicationList = response;
                    this.MedicationList.forEach(medicationName => {
                        this.MedicationDropDownItems.push(medicationName);
                    }, this);
                }
            }
            );
        }
    }
    public RefreshControls() {
        this.SetMedicationName();
        this.SetPreBuiltGroupName();
    }
    SetMedicationName() {
        this.MedicationName = this.medicationSearch.MedSearchText == "" ? this.searchMedicationDefault : this.medicationSearch.MedSearchText;
    }

    GetMedicationName() {
        this.medicationSearch.MedSearchText = this.MedicationName == this.searchMedicationDefault ? "" : this.MedicationName;
    }

    MedicationFocus(focus: boolean) {
        clearTimeout(this.MedicationTimeOut);
        this.MedicationTimeOut = setTimeout(() => { this.MedicationFocusChanged(focus) }, focus ? 0 : 500)
    }

    MedicationFocusChanged(focus: boolean) {
        this.MedicationDropDownItems = new Array();
        this.MedicationDropDownCurrentItemIndex = -1;
        this.MedicationDropDownCurrentItemName = "";
        if (focus) {
            this.MedicationName = this.MedicationName != this.searchMedicationDefault ? this.MedicationName : "";
            this.SetMedicationDropDownItems();
        }
        else {
            this.MedicationName = this.MedicationName.trim() != "" ? this.MedicationName : this.searchMedicationDefault;
        }
    }

    MedicationDropDownItemSelected(val: string) {
        this.MedicationName = val;
        this.MedicationSearchCriteriaChanged();
    }


    MedicationKeyDown(e) {
        this.MedicationName = this.MedicationName.replace(this.searchMedicationDefault, '');
        if (this.MedicationDropDownItems.length > 0) {
            if (e.keyCode == 40) {
                /*If the arrow DOWN key is pressed,
                increase the currentFocus variable:*/
                this.MedicationDropDownCurrentItemIndex++;
            } else if (e.keyCode == 38) { //up
                /*If the arrow UP key is pressed,
                decrease the currentFocus variable:*/
                this.MedicationDropDownCurrentItemIndex--;
            } else if (e.keyCode == 13) {
                e.preventDefault();
                if (this.MedicationDropDownCurrentItemIndex > -1) {
                    /*and simulate a click on the "active" item:*/
                    this.MedicationDropDownItemSelected(this.MedicationDropDownCurrentItemName);
                    this.MedicationDropDownItems = new Array();
                    this.MedicationDropDownCurrentItemIndex = -1;
                    this.MedicationDropDownCurrentItemName = "";
                }
                else {
                    this.MedicationSearchCriteriaChanged();
                }
            }

            if (this.MedicationDropDownCurrentItemIndex >= this.MedicationDropDownItems.length)
                this.MedicationDropDownCurrentItemIndex = 0;

            if (this.MedicationDropDownCurrentItemIndex < 0)
                this.MedicationDropDownCurrentItemIndex = (this.MedicationDropDownItems.length - 1);

            this.MedicationDropDownCurrentItemName = this.MedicationDropDownItems[this.MedicationDropDownCurrentItemIndex];
        }
        else {
            if (e.keyCode == 13) {
                e.preventDefault();
                this.MedicationSearchCriteriaChanged();
            }
        }
    }

    SetPreBuiltGroupName() {
        this.PreBuiltGroupName = this.medicationSearch.PreBuiltGroupName == "" ? this.searchGroupDefault : this.medicationSearch.PreBuiltGroupName;
    }

    GetPreBuiltGroupName() {
        this.medicationSearch.PreBuiltGroupName = this.PreBuiltGroupName == this.searchGroupDefault ? "" : this.PreBuiltGroupName;
    }

    PreBuiltGroupFocus(focus: boolean) {
        clearTimeout(this.PreBuiltGroupTimeOut);
        this.PreBuiltGroupTimeOut = setTimeout(() => { this.PreBuiltGroupFocusChanged(focus) }, focus ? 0 : 500)
    }

    PreBuiltGroupFocusChanged(focus: boolean) {
        this.PreBuiltGroupDropDownItems = new Array();
        this.PreBuiltGroupDropDownCurrentItemIndex = -1;
        this.PreBuiltGroupDropDownCurrentItemName = "";
        if (focus) {
            this.PreBuiltGroupName = this.PreBuiltGroupName != "Select Group" ? this.PreBuiltGroupName : "";
            this.PreBuiltGroupList.forEach(groupName => {
                if (this.PreBuiltGroupName == "" || groupName.toLowerCase().indexOf(this.PreBuiltGroupName.trimLeft().toLowerCase()) > -1) {
                    this.PreBuiltGroupDropDownItems.push(groupName);
                }
            }, this);
        }
        else {
            this.PreBuiltGroupName = this.PreBuiltGroupName.trim() != "" ? this.PreBuiltGroupName : "Select Group";
        }
    }

    PreBuiltGroupDropDownItemSelected(val: string) {
        this.PreBuiltGroupName = val;
        this.SetMedicationSearchOption(SelectMedicationRequestType.PreBuiltGroup);
    }


    PreBuiltGroupKeyDown(e) {
        this.PreBuiltGroupName = this.PreBuiltGroupName.replace(this.searchGroupDefault, '');
        if (this.PreBuiltGroupDropDownItems.length > 0) {
            if (e.keyCode == 40) {
                /*If the arrow DOWN key is pressed,
                increase the currentFocus variable:*/
                this.PreBuiltGroupDropDownCurrentItemIndex++;
            } else if (e.keyCode == 38) { //up
                /*If the arrow UP key is pressed,
                decrease the currentFocus variable:*/
                this.PreBuiltGroupDropDownCurrentItemIndex--;
            } else if (e.keyCode == 13) {
                e.preventDefault();
                if (this.PreBuiltGroupDropDownCurrentItemIndex > -1) {
                    /*and simulate a click on the "active" item:*/
                    this.PreBuiltGroupDropDownItemSelected(this.PreBuiltGroupDropDownCurrentItemName);
                    this.PreBuiltGroupDropDownItems = new Array();
                    this.PreBuiltGroupDropDownCurrentItemIndex = -1;
                    this.PreBuiltGroupDropDownCurrentItemName = "";
                }
                else {
                    this.SetMedicationSearchOption(SelectMedicationRequestType.PreBuiltGroup);
                }
            }

            if (this.PreBuiltGroupDropDownCurrentItemIndex >= this.PreBuiltGroupDropDownItems.length)
                this.PreBuiltGroupDropDownCurrentItemIndex = 0;

            if (this.PreBuiltGroupDropDownCurrentItemIndex < 0)
                this.PreBuiltGroupDropDownCurrentItemIndex = (this.PreBuiltGroupDropDownItems.length - 1);

            this.PreBuiltGroupDropDownCurrentItemName = this.PreBuiltGroupDropDownItems[this.PreBuiltGroupDropDownCurrentItemIndex];
        }
        else {
            if (e.keyCode == 13) {
                e.preventDefault();
                this.SetMedicationSearchOption(SelectMedicationRequestType.PreBuiltGroup);
            }
        }
    }
    SelectMedSearchRequestDataType(medSearchOption: SelectMedicationRequestType) {
        this.medicationSearch.MedicationSearchOption = medSearchOption;
        this.medicationSearch.Validate = false;
        this.MedicationSearchCriteriaChanged();
    }

    SetMedicationSearchOption(medSearchOption: SelectMedicationRequestType) {
        this.medicationSearch.MedicationSearchOption = medSearchOption;
        this.MedicationSearchCriteriaChanged();
    }
    SearchMedication() {
        this.DiscardPreviousSearchMedicationNameRequest();
        this.medicationSearch.Validate = true;
        this.MedicationSearchCriteriaChanged();
    }
    MedicationSearchCriteriaChanged() {
        this.GetMedicationName();
        this.GetPreBuiltGroupName();
        this.OnMedicationSearchCriteriaChanged.emit(new SelectMedicationSearchArgs(this.medicationSearch));
        this.MedicationDropDownItems = new Array();
    }

    private DiscardPreviousSearchMedicationNameRequest() {
        if (this.SearchMedicationNameRequest)
            this.SearchMedicationNameRequest.unsubscribe();
    }
};