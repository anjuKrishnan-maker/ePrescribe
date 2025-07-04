import { Component, Input, Output, EventEmitter, ViewChild } from '@angular/core';
//import { BaseComponent } from '../../base.component';
import { FormGroup, FormBuilder, Validators } from '@angular/forms';
import { } from '../../../../services/service.import.def';
import { NAVIGATION_EVENT_ID, EVENT_COMPONENT_ID, MessageControlType, current_context } from "../../../../tools/constants";
import { PatientMedRecDetailModel, ActionType } from '../../../../model/model.import.def';
import { ModalPopupControl } from '../../../shared/controls/modal-popup/modal-popup.control';

@Component({
    selector: 'patmed-medallergy-reconciliation',
    templateUrl: './pat-med-med-allergy-reconciliation.template.html',
    styleUrls: ['./pat-med-med-allergy-reconciliation.style.css']
})

export class PatMedAndMedAllergyReconciliationComponent {
    @Output() SelectionComplete: EventEmitter<PatientMedRecDetailModel> = new EventEmitter<PatientMedRecDetailModel>();
    @Input() PatientMedRecDetails: PatientMedRecDetailModel;
    @Input() ModalId: string;
    @Input() ShouldShowSaveNPrescribe: boolean = false;
    @ViewChild(ModalPopupControl) modalPopup: ModalPopupControl;
    public DoesHaveValidMedAndAllergy: boolean = false;

    updatePatientMedRecDetail() {
        this.SelectionComplete.emit(this.PatientMedRecDetails);
    }

    Show() {
        this.DoesHaveValidMedAndAllergy = this.PatientMedRecDetails != null && this.PatientMedRecDetails.DoesPatientHaveValidMedAndAllergy;
        this.modalPopup.OpenPopup();
    }

    cancel() {
        this.modalPopup.ClosePopup();
    }

       updateSelectedActionType(actionType: string) {
        this.PatientMedRecDetails.Type = actionType;
    }

    SaveButtonClicked() {
        this.PatientMedRecDetails.ActionType = ActionType.Save;
        this.SelectionComplete.emit(this.PatientMedRecDetails);
        this.modalPopup.ClosePopup();
    }

    SaveAndPrescribeButtonClicked() {
        this.PatientMedRecDetails.ActionType = ActionType.SavePrescribe;
        this.SelectionComplete.emit(this.PatientMedRecDetails);
        this.modalPopup.ClosePopup();
    }

    EnableSaveButtons() {
        return this.PatientMedRecDetails.Type !== 'TC' && this.PatientMedRecDetails.Type !== 'MR';
    }
}
