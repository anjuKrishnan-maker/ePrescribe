import { Component, Input, Output, EventEmitter } from '@angular/core';
import { EVENT_COMPONENT_ID,current_context } from '../../../../tools/constants';
import { PrivacyOverrideService, EventService, PatientService } from '../../../../services/service.import.def';
import { PrivacyOverrideModel, PatientContextDTO } from '../../../../model/model.import.def';
import { Patient } from '../../../../model/patient.model';
import { FormsModule, NgForm } from '@angular/forms';

@Component({
    selector: 'privacy-override',
    templateUrl: './privacy-override.template.html',
    styles: [`
        .overlayFooter{
                padding-top: 1px!important;
                padding-right: 15px!important;
                padding-bottom: 1px!important;
                padding-left: 1px!important;
        }
 .form-group{margin-bottom: 0px !important;}
.modal {
  text-align: center;
  padding: 0!important;
}

.modal:before {
  content: '';
  display: inline-block;
  height: 100%;
  vertical-align: middle;
  margin-right: -4px;
}

.modal-dialog {
  display: inline-block;
  text-align: left;
  vertical-align: middle;
}
`]
})

export class PrivacyOverrideComponent {
    @Output() SelectionComplete: EventEmitter<string> = new EventEmitter<string>();

    submited: boolean = false;
    isValid: boolean = false;
    patient: Patient = new Patient();
    privacyOverride: PrivacyOverrideModel = new PrivacyOverrideModel();
    callBack: () => void;

    constructor(private svE: EventService, private privacyOverrideSvc: PrivacyOverrideService, private pSvc: PatientService) {

        this.svE.get(EVENT_COMPONENT_ID.OnPatientSelected).subscribe((patientContext: PatientContextDTO) => {
            this.privacyOverride = new PrivacyOverrideModel();           
            if (patientContext && patientContext.isPrivacyPatient) {
                this.OpenModal();
                if (patientContext.callback != undefined && typeof (patientContext.callback) == 'function') {
                    this.callBack = patientContext.callback;
                }
            }
        });
    }

    validate(d: NgForm) {
        this.isValid = d.valid;
        return;
    }

    SaveOverrideReason() {
        this.submited = true;
        let value = current_context.PageName;

        if (this.isValid)
            this.privacyOverrideSvc.SaveOverrideReason(this.privacyOverride.OverrideText, (value)).subscribe(() => {
                // why are we checking IsRestrictedUser after the user just saved the privacy override reason?
                //if (this.Patient.IsRestrictedUser == true) {
                    this.submited = false;
                    this.privacyOverride = new PrivacyOverrideModel();
                    $("#mdlPrivacyOverride").modal('toggle');

                    if (this.callBack === undefined || this.callBack === null) {
                        // only emit event when popup displayed from selecting patient on Select Patient component
                        this.SelectionComplete.emit('override');
                    } else {
                        // only fire callBack when popup displayed from Task Summary/List Send Scripts/Specialty Med
                        try {
                            this.callBack();
                        }
                        catch (err) {
                        }
                    }
                //}

                this.callBack = undefined;
            });
    }

    public OpenModal() {
        $("#mdlPrivacyOverride").modal({ backdrop: 'static', keyboard: false });
        $("#mdlPrivacyOverride").modal('show');      
    }

    Cancel() {
        let value = current_context.PageName;

        this.privacyOverrideSvc.Cancel((value)).subscribe(() => {
            this.submited = false;
            this.privacyOverride = new PrivacyOverrideModel();
            $("#mdlPrivacyOverride").modal('toggle');

            // only emit event when popup displayed from selecting patient on Select Patient component
            if (this.callBack === undefined || this.callBack === null) {
                this.SelectionComplete.emit("cancel");
            }
        });
    }
}
