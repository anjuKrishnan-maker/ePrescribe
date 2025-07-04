import { Component, Input, Output, EventEmitter } from '@angular/core'
import { EVENT_COMPONENT_ID, current_context, NAVIGATION_EVENT_ID } from '../../../tools/constants'
import { EventService, FormularyOverrideService  } from '../../../services/service.import.def';
import { FormsModule, NgForm } from '@angular/forms';
import { FormularyOverrideModel, MedicationModel, SelectedOverrideReasonModel, FormularyOverideArgs, FormularyOverideAction } from '../../../model/model.import.def';

@Component({
    selector: 'erx-select-medication-formulary-override',
    templateUrl: './select-medication-formulary-override.template.html',
    styles: [`
    #frameMdlPopup{
        height: 100%; 
        width: 100%; 
        border: none; 
        overflow-x: hidden;
    }
`]
})

export class SelectMedicationFormularyOverrideComponent {

    public FormularyOverrideResponse: FormularyOverrideModel[];
    public ignoreReasons: string[];
    public medication: MedicationModel[];
    public ShowError = false;
    public selectedOverRideReason: number;
    public rxID: string;
    public selectedMedication: SelectedOverrideReasonModel[];
    callBack: (data: FormularyOverideArgs) => void;

    constructor(private evtSvc: EventService, private foSvc: FormularyOverrideService) {
    }

    loadFormularyOverride(val: any) {
        this.selectedMedication = [];
        if (val.callback != undefined && typeof (val.callback) == 'function') {
            this.callBack = val.callback;
        }
        this.showOverlay();
    }

    showOverlay() {
        this.foSvc.GetFormularyOverideIgnoreReasons().subscribe((response) => {
            this.ignoreReasons = response.IgnoreReasons.IgnoreReasons;
            this.medication = response.Medication;
            this.selectedMedication = [];
            this.ShowError = false;
            $("#mdlPopupFormularyOverride").modal();
        })

    }

    cancel()
    {
        this.foSvc.ClearOverrideRxListFromSession().subscribe(() => {
            this.selectedMedication = [];
            this.ShowError = false;
            $("#mdlPopupFormularyOverride").modal('toggle');
        })
    }

    onChange(overideReason) {
        this.selectedOverRideReason = overideReason.selectedIndex;
        this.rxID = overideReason.value;

        let index = this.selectedMedication.findIndex(x => x.RxID == this.rxID)
        if (index != -1) { this.selectedMedication[index].OverideReason = this.selectedOverRideReason; }
        else { this.selectedMedication.push({ RxID: this.rxID, OverideReason: this.selectedOverRideReason }) }
    }

    process() {
        let selectedMedicationCount = this.selectedMedication.length;
        let MedicationCount = this.medication.length;

        if (selectedMedicationCount < MedicationCount) {
            this.ShowError = true;
        }
        else if(!this.ShowError){
            let index = this.selectedMedication.findIndex(x => x.OverideReason == 0)
            if (index != -1) { this.ShowError = true; return; }
        }
        else {
            this.ShowError = false;
        }

        if (this.ShowError == false) {
            this.foSvc.FormularyOverrideProcessMedication(this.selectedMedication).subscribe((response) => {
                $("#mdlPopupFormularyOverride").modal('toggle');
                this.ShowError = false;
                let foa = new FormularyOverideArgs();
                foa.Action = FormularyOverideAction.Process;
                foa.Data = response;
                this.callBack(foa);
            })
        }
    }
}


