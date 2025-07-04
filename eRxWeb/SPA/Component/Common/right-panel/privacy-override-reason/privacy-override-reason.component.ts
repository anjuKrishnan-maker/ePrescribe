import { Component, ChangeDetectorRef } from '@angular/core';
import { BaseComponent } from '../../base.component';
import { EventService, PrivacyOverrideService } from '../../../../services/service.import.def';
import { PrivacyPatientInfoDTO } from '../../../../model/model.import.def';

@Component({
    selector: 'erx-privacy-override-reason',
    templateUrl: './privacyoverridereason.template.html'
})

export class PrivacyOverrideReasonComponent extends BaseComponent {
    public privacyOverrideReason: string;

    constructor(private eventService: EventService,
        private privacyOverrideSvc: PrivacyOverrideService,
        private changeDetector: ChangeDetectorRef) {
        super(null, eventService);
    }

    public loadPrivacyOverrideReason(patientInfo: PrivacyPatientInfoDTO) {
        if (patientInfo.PatientId === '') {
            this.privacyOverrideReason = null;
            this.changeDetector.detectChanges();
        }
        else {
            this.StartLoading();
            this.privacyOverrideSvc
                .GetOverrideReason(patientInfo.PatientId, patientInfo.UserId, patientInfo.CreatedUtc)
                .subscribe((reason: string) => {
                    this.privacyOverrideReason = reason;
                    this.EndLoading();
                    this.changeDetector.detectChanges();
                });
        }
    }
}