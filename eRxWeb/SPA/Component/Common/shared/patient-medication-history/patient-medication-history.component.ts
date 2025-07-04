import { Component, Input, Output, EventEmitter } from '@angular/core'
import { EVENT_COMPONENT_ID, current_context } from '../../../../tools/constants'
import { EventService, PatientMedHistoryService } from '../../../../services/service.import.def';
import { PatientMedHistoryModel } from  '../../../../model/model.import.def';

@Component({
    selector: 'erx-patient-med-history',
    templateUrl: './patient-medication-history.template.html',
    styles: [`
    #frameMdlPopup{
        height: 100%; 
        width: 100%; 
        border: none; 
        overflow-x: hidden;
    }` ]
})

export class PatientMedHistoryComponent {
    patHistoryMedDetails: PatientMedHistoryModel[];

    constructor(private evtSvc: EventService, private patMedHistrySvc: PatientMedHistoryService) {
    }

    showPatientMedHistory() {
        this.patMedHistrySvc.GetPatientMedHistory().subscribe((response) => {
            this.patHistoryMedDetails = response;
            this.openPatientMedModal();
        });
    }

    openPatientMedModal() {
        $("#mdlPopupPatientMedHistory").modal();
    }

    closePatientMedModal() {
        $("#mdlPopupPatientMedHistory").modal('toggle');
    }
}