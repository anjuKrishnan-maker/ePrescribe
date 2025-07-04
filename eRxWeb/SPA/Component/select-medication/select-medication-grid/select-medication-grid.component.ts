import { Component, Input, Output, EventEmitter, ViewChild } from '@angular/core';
import { EVENT_COMPONENT_ID, NAVIGATION_EVENT_ID, PAGE_NAME } from '../../../tools/constants';
import { EventService, SelectMedicationService, ErrorService, ComponentCommonService } from '../../../services/service.import.def';
import {
    SelectMedicationGridModel, SelectMedicationMedModel, MedNameDisplayOption, SelectMedicationSelections, SelectMedicationEventContext, SelectMedicationGridEvent, SortOrder,
    ImageModel, SelectMedicationDataRequest, MessageModel, MessageIcon, ErrorContextModel, GridPaginationModel, SelectMedicationGridPopulationCompletionArgs,
    MedCompletionHistoryModel, SelectMedicationRequestType, PatientRecordStatus, GridRowSelectionStatus
} from '../../../model/model.import.def';
import { EventSubscriptionDisposer } from '../../common/root.component';
import { ModalPopupControl } from '../../shared/controls/modal-popup/modal-popup.control';
import { MedSearchComponent } from '../../common/med-search/med-search.component';
import { fail } from 'assert';
import { log } from 'util';
import { elementAt } from 'rxjs/operators';
import { SelectMedicationUrlModalPopupComponent } from '../select-medication-url-modal-popup/select-medication-url-modal-popup.component';

@Component({
    selector: 'erx-select-medication-grid',
    templateUrl: './select-medication-grid.template.html',
    styleUrls: ['./select-medication-grid.style.css']
})

export class SelectMedicationGridComponent extends EventSubscriptionDisposer {
    @ViewChild(ModalPopupControl) modalPopup: ModalPopupControl;
    @ViewChild(SelectMedicationUrlModalPopupComponent) urlModal: SelectMedicationUrlModalPopupComponent;
    @Output() OnMedSelected: EventEmitter<SelectMedicationSelections> = new EventEmitter<SelectMedicationSelections>();
    @Output() OnGridPopulationComplete = new EventEmitter<SelectMedicationGridPopulationCompletionArgs>();
    gridData: SelectMedicationGridModel;
    public SelectAll: boolean = false;

    get MultiSelect(): boolean {
        return this.gridData != undefined && this.gridData.IsAllMedSearch == false;
    }
    public _src_delux_ad: any;     
    private Request: SelectMedicationDataRequest;

    @Input()
    set SelectMedDataRequest(value: SelectMedicationDataRequest) {
        if (value != undefined) {
            this.Request = value;
            this.GetData();
        }
    }
    SortOrder: SortOrder = SortOrder.Ascending;
    constructor(private evSvc: EventService, private fsSvc: SelectMedicationService, private erSvc: ErrorService, private compSvc: ComponentCommonService) {
        super();
        this._src_delux_ad = PAGE_NAME.DeluxAdContainer;
        
        this.evSvc.subscribeEvent(EVENT_COMPONENT_ID.OnScriptPadMedicationAdded, () => {
            this.ResetSelection();
        });
        this.compSvc.AddWindowFunction('CloseDeluxeAdPopup', () => {
            this.closeModalPopup();
        });
    }

    AutoSelectItem(data: SelectMedicationMedModel[]) {
        let lastMedIndex;
        let lastMed;
        data.forEach((previousSelectedMed) => {
            this.gridData.Meds.forEach((gridMed) => {
                if (previousSelectedMed.index == gridMed.index
                    && previousSelectedMed.DDI == gridMed.DDI) {
                    gridMed.Selected = previousSelectedMed.Selected;
                    gridMed.RowStatus = previousSelectedMed.RowStatus;
                    gridMed.DAW = previousSelectedMed.DAW;
                    gridMed.DayOfSupply = previousSelectedMed.DayOfSupply;
                    gridMed.Quantity = previousSelectedMed.Quantity;
                    gridMed.Refill = previousSelectedMed.Refill;

                    if (gridMed.RowStatus == GridRowSelectionStatus.SelectActive) {
                        lastMedIndex = gridMed.index;
                        lastMed = gridMed;
                    }
                }
            });
        });

        if (lastMedIndex == undefined) {
            var selectedRows = this.gridData.Meds.filter(x => x.RowStatus == GridRowSelectionStatus.Selected);
            if (selectedRows != undefined && selectedRows.length > 0) {
                lastMedIndex = selectedRows[selectedRows.length - 1].index;
                this.gridData.Meds.forEach((gridMed) => {
                    if (lastMedIndex == gridMed.index) {
                        gridMed.RowStatus == GridRowSelectionStatus.SelectActive;
                        lastMed = gridMed;
                    }
                });
            }
        }

        if (lastMedIndex != undefined) {
            let SelectMedicationSelections;
            SelectMedicationSelections = this.createSelectionObject(lastMedIndex, lastMed);
            SelectMedicationSelections.EventContext.GridEvent = SelectMedicationGridEvent.AllMedSelected;
            this.OnMedSelected.emit(SelectMedicationSelections);
        }
    }

    IsValid(img: ImageModel): boolean {
        return img !== null && img !== undefined
            && img.ImageUrl !== null
            && img.ImageUrl !== undefined
            && img.ImageUrl.length > 2;
    }
    IsDURImgValid(tooltip: string): boolean {
        if (tooltip !== null)
            return true;
        else
            return false;
    }

    GridReset() {
        if (this.gridData != undefined && this.gridData.Meds != undefined) {
            this.gridData.Meds = [];
        }
        this.SelectAll = false;
    }

    private GetData() {
        this.GridReset();
        //retseting the grid med array
        if (this.gridData !== null && this.gridData !== undefined)
            this.gridData = null;
        this.fsSvc.GetSelectMedicationData(this.Request).subscribe(response => {
            this.gridData = response;
            for (var idx = 0; idx < this.gridData.Meds.length; idx++) {
                this.gridData.Meds[idx].index = idx;
            }
            let grd = new SelectMedicationGridPopulationCompletionArgs();
            grd.Messages = this.gridData.Messages;
            grd.RequestFor = this.gridData.RequestFor;
            grd.PatientRecordStatus = this.gridData.PatientRecordStatus;
            this.OnGridPopulationComplete.emit(grd);
        }
        )
    }
    Refresh() {     // Review and remove if not required (might be required when pagination is removed)
        this.gridData = new SelectMedicationGridModel();
        this.GetData();
    }
    ResetSelection() {
        if (this.gridData != undefined && this.gridData.Meds != undefined)
            for (let i = 0; i < this.gridData.Meds.length; i++) {
                this.gridData.Meds[i].Selected = false;
                this.gridData.Meds[i].RowStatus = GridRowSelectionStatus.NotSelected;
            }
        let SelectMedicationSelections = this.createSelectionObject(-1, null);
        SelectMedicationSelections.EventContext.GridEvent = SelectMedicationGridEvent.AllMedDeSelected;
        this.OnMedSelected.emit(SelectMedicationSelections);
    }

    OnGridItemSelected(item: SelectMedicationMedModel, medIndex: number, e: any): void {
        if (!this.MultiSelect) {
            for (let i = 0; i < this.gridData.Meds.length; i++) {
                this.gridData.Meds[i].Selected = false;
                this.gridData.Meds[i].RowStatus = GridRowSelectionStatus.NotSelected;
            }
        }

        if (item.Selected != true) {
            item.Selected = true;
            item.RowStatus = GridRowSelectionStatus.Selected
        }


        if (e.target !== null && (<any>e.target).checked == false) {
            item.Selected = false;
            item.RowStatus = GridRowSelectionStatus.NotSelected;
        }
        else {
            let activeRow = this.gridData.Meds.filter(x => x.RowStatus == GridRowSelectionStatus.SelectActive);
            for (let i = 0; i < activeRow.length; i++) {
                if (activeRow[i].index != item.index)
                    activeRow[i].RowStatus =
                        activeRow[i].RowStatus == GridRowSelectionStatus.SelectActive
                            || activeRow[i].RowStatus == GridRowSelectionStatus.Selected
                            ? GridRowSelectionStatus.Selected : GridRowSelectionStatus.NotSelected;
            }
        }
        if (item.Selected === true) {
            item.RowStatus = GridRowSelectionStatus.SelectActive
        }

        let SelectMedicationSelections = this.createSelectionObject(medIndex, item);
        SelectMedicationSelections.EventContext.GridEvent = item.Selected ? SelectMedicationGridEvent.MedSelected : SelectMedicationGridEvent.MedDeselected;

        this.OnMedSelected.emit(SelectMedicationSelections);
    }

    OnGridItemModified(item: SelectMedicationMedModel, medIndex: number, e: Event) {
        if (item.Selected == true) {
            for (let i = 0; i < this.gridData.Meds.length; i++) {
                if (this.gridData.Meds[i].Selected == true) {
                    this.gridData.Meds[i].RowStatus = GridRowSelectionStatus.Selected;
                };

            }
            item.RowStatus = GridRowSelectionStatus.SelectActive;
            let SelectMedicationSelections = this.createSelectionObject(medIndex, item);
            SelectMedicationSelections.EventContext.GridEvent = SelectMedicationGridEvent.MedModified;
            this.OnMedSelected.emit(SelectMedicationSelections);
        }
    }
    DawChanged(e: any, item: SelectMedicationMedModel) {
        if (e.target !== null && (e.target).checked == true) {
            item.DAW = true;
        }
        else {
            item.DAW = false;
        }
        this.OnGridItemModified(item, item.index, e);
    }
    private createSelectionObject(medIndex: number, item: SelectMedicationMedModel): SelectMedicationSelections {
        let fss = new SelectMedicationSelections();
        fss.EventContext = new SelectMedicationEventContext();
        fss.SelectedMeds = this.getSelectedMeds(this.gridData);
        fss.EventContext.CurrentMedIndex = medIndex;
        fss.EventContext.CurrentMed = item;
        return fss;
    }

    private getSelectedMeds(fsgm: SelectMedicationGridModel): SelectMedicationMedModel[] {
        return fsgm.Meds.filter(x => x.Selected == true);
    }

    OnGridSelectAllChecked(e: Event) {
        this.SelectAll = !this.SelectAll;
        let lastMedIndex = this.gridData.Meds.length - 1;
        let lastMed = this.gridData.Meds[lastMedIndex];
        this.gridData.Meds.forEach((x, index) => {
            x.Selected = this.SelectAll;
            if (this.SelectAll == true) {
                if (x.RowStatus == GridRowSelectionStatus.SelectActive) {
                    lastMedIndex = x.index;
                    lastMed = x;
                }
                else {
                    x.RowStatus = GridRowSelectionStatus.Selected;
                }
            }
            else if (this.SelectAll == false) {
                x.RowStatus = GridRowSelectionStatus.NotSelected;
            }
        });
        let SelectMedicationSelections;
        if (this.SelectAll === true) {
            SelectMedicationSelections = this.createSelectionObject(lastMedIndex, lastMed);
            SelectMedicationSelections.EventContext.GridEvent = SelectMedicationGridEvent.AllMedSelected;
            lastMed.RowStatus = GridRowSelectionStatus.SelectActive;
        }
        else {
            SelectMedicationSelections = this.createSelectionObject(-1, null);
            SelectMedicationSelections.EventContext.GridEvent = SelectMedicationGridEvent.AllMedDeSelected;
        }
        this.OnMedSelected.emit(SelectMedicationSelections);
    }

    MedNameClicked(med: SelectMedicationMedModel) {


        if (med.DrugNameDisplayOption == MedNameDisplayOption.DisplayLinkWithLibraryLink) {
            this.urlModal.openPopupModal({ src: med.LibraryUrl, title: '' });
            //this.evSvc.invokeEvent(EVENT_COMPONENT_ID.OpenModalPopup, { src: med.LibraryUrl, title: '' });
            this.fsSvc.InsertLibraryAudit().subscribe();
        }
        else {
            this._src_delux_ad = PAGE_NAME.DeluxAdContainer;
            this.modalPopup.OpenPopup();
        }
    }



    Sort() {
        if (this.SortOrder == SortOrder.Ascending) {
            this.SortOrder = SortOrder.Descending;
            this.gridData.Meds.sort((a, b) => {
                var nameA = a.DrugName.toLowerCase(), nameB = b.DrugName.toLowerCase()
                if (nameA < nameB) //sort string descending
                    return 1
                if (nameA > nameB)
                    return -1
                return 0 //default return value (no sorting)
            });
        }
        else if (this.SortOrder == SortOrder.Descending) {

            this.SortOrder = SortOrder.Ascending;
            this.gridData.Meds.sort((a, b) => {
                var nameA = a.DrugName.toLowerCase(), nameB = b.DrugName.toLowerCase()
                if (nameA < nameB) //sort string ascending
                    return -1
                if (nameA > nameB)
                    return 1
                return 0 //default return value (no sorting)
            });
        }

    }
    TextBoxSelected(e: Event) {
        e.stopPropagation();
    }

    closeModalPopup() {
        this.modalPopup.ClosePopup();
    }
} 