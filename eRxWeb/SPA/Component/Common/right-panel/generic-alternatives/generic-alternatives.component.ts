import { Component, ElementRef, OnInit } from '@angular/core';
import { BaseComponent } from '../../base.component';
import { GenericAlternativeService, EventService, ComponentCommonService } from '../../../../services/service.import.def';
import { GenericAlternative, MedicationSelectedPayload } from '../../../../model/model.import.def';
import { EVENT_COMPONENT_ID, current_context, ROUTE_NAME } from '../../../../tools/constants';
import { Router } from '@angular/router';

@Component({
    selector: 'erx-generic-alternatives',
    templateUrl: './generic-alternatives.template.html'
})

export class GenericAlternativesComponent extends BaseComponent implements OnInit {

    GenericAlternatives: GenericAlternative[];
    PendingRequest: any;
    IsComponentVisible: boolean;

    constructor(private genericAlternativeService: GenericAlternativeService,
        cd: ElementRef, private svE: EventService, private svC: ComponentCommonService, private router:Router ) {
        super(cd, svE);

        this.svE.subscribeEvent(EVENT_COMPONENT_ID.MedicationSelecting, () => {
            this.StartLoading();
        });

        this.svE.subscribeEvent(EVENT_COMPONENT_ID.OnMedicationSelected, (val: MedicationSelectedPayload) => {
            this.LoadData(val);
        });

        this.svE.subscribeEvent(EVENT_COMPONENT_ID.OnMedicationDeSelected, () => {
            this.SetDefaultData();
        });
    }

    ngOnInit() {
        this.SetDefaultData();
    }

    SetDefaultData() {
        this.GenericAlternatives = null;
        if (this.PendingRequest) {
            this.PendingRequest.unsubscribe();
        }
        this.EndLoading();
    }

    LoadData(medSelectedPayload: MedicationSelectedPayload) {
        this.SetDefaultData()
        if (this.svC.CheckVisibility(this.id, current_context.PageName)) {
            try {
                this.GenericAlternatives = medSelectedPayload.GenericAlternativesPayload;
                this.EndLoading();
            } catch (e) {
                this.EndLoading();
            }
        }
    }
    AlternativeClicked(medName: string) {
        this.router.navigateByUrl(ROUTE_NAME.SelectMedication, {
            state: {
                searchText: medName
            }
        });
    }
}



