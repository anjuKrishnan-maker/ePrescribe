import { Component, ElementRef, OnInit } from '@angular/core';

import { BaseComponent } from '../../base.component';

import { CopayCoverageServcie, EventService, ComponentCommonService } from '../../../../services/service.import.def';
import { CopayCoverageModel, MedicationSelectedPayload } from '../../../../model/model.import.def';

import { EVENT_COMPONENT_ID, current_context } from '../../../../tools/constants';
import ObjectUtil from '../../../../tools/Utils/object.extension';

@Component({
    selector: 'erx-copay-coverage',
    templateUrl: './copay-coverage.template.html',
    providers: [CopayCoverageServcie]
})
export class CopayCoverageComponent extends BaseComponent implements OnInit {
    CopayCoverages: CopayCoverageModel;
    PendingRequest: any;

    constructor(private copSvc: CopayCoverageServcie, cd: ElementRef, private svE: EventService,
        private svC: ComponentCommonService) {
        super(cd, svE);

        this.svE.subscribeEvent(EVENT_COMPONENT_ID.OnMedicationSelected, (val: MedicationSelectedPayload) => {
            this.LoadCopayCoverageInfo(val);
        });

        this.svE.subscribeEvent(EVENT_COMPONENT_ID.MedicationSelecting, () => {
            this.StartLoading();
        });

        this.svE.subscribeEvent(EVENT_COMPONENT_ID.OnMedicationDeSelected,() => {
            this.SetDefaultData();
        });
    }

    ngOnInit() {
        this.SetDefaultData();
    }

    SetDefaultData() {
        this.CopayCoverages = null;
        if (this.PendingRequest) {
            this.PendingRequest.unsubscribe();
        }
        this.EndLoading();
    }

    LoadCopayCoverageInfo(medSelectedPayload: MedicationSelectedPayload) {
        this.SetDefaultData()
        if (this.svC.CheckVisibility(this.id, current_context.PageName)) {
            try {
                this.CopayCoverages = medSelectedPayload.CopayCoveragePayload;
                this.EndLoading();
            } catch (e) {
                this.EndLoading();
            }
        }
    }
}
