import { Component, ElementRef } from '@angular/core';

import { BaseComponent } from '../../base.component';

import { EventService, ComponentCommonService, ProgramAlertService } from '../../../../services/service.import.def';
import { EVENT_COMPONENT_ID, current_context } from '../../../../tools/constants';
import { MedicationDTO } from '../../../../model/model.import.def';
@Component({
    selector: 'erx-program-alert',
    templateUrl: './program-alert.template.html'
})
export class ProgramAlertComponent extends BaseComponent {
    isSpecialtyMed: boolean;
    isProviderEnrolledInSpecialtyMed: boolean;
    PendingRequest: any;

    constructor(private svC: ComponentCommonService, private svE: EventService, cd: ElementRef, private paSvc: ProgramAlertService) {
        super(cd, svE);
        
        this.svE.subscribeEvent(EVENT_COMPONENT_ID.OnMedicationSelected, (val: MedicationDTO) => {
            this.NotifySpecialtyMed(val);
        });

        this.svE.subscribeEvent(EVENT_COMPONENT_ID.RightPanelLoaded, () => {
            this.SetDefaultData();
        });

        this.svE.subscribeEvent(EVENT_COMPONENT_ID.OnMedicationDeSelected, () => {
            this.SetDefaultData();
        });
    }

    SetDefaultData() {
        this.isSpecialtyMed = false;
        this.isProviderEnrolledInSpecialtyMed = false;
        if (this.PendingRequest) {
            this.PendingRequest.unsubscribe();
        }
        this.EndLoading();
    }

    NotifySpecialtyMed(data: MedicationDTO) {
        this.SetDefaultData()
        if (this.svC.CheckVisibility(this.id, current_context.PageName)) {
            try {
                if (data != undefined && data.isSpecialtyMedication) {
                    this.StartLoading();
                    this.PendingRequest = this.paSvc.IsProviderEnrolledInSpecialtyMed().subscribe((val) => {
                        this.isProviderEnrolledInSpecialtyMed = val;
                        this.isSpecialtyMed = true;
                        this.EndLoading();
                    });
                }
            } catch (e) {
                this.EndLoading();
            }
        }
    }
}
