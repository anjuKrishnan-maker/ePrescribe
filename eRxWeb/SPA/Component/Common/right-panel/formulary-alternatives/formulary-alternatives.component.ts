import { Component, ElementRef, OnInit } from '@angular/core';
import { BaseComponent } from '../../base.component';
import { FormularyAlternativeService, EventService, ComponentCommonService } from '../../../../services/service.import.def';
import { FormularyAlternative, MedicationSelectedPayload } from '../../../../model/model.import.def';
import { EVENT_COMPONENT_ID, current_context, COMPONENT_NAME, ROUTE_NAME } from '../../../../tools/constants';
import { Router } from '@angular/router';

@Component({
    selector: 'erx-formulary-alternatives',
    templateUrl: './formulary-alternatives.template.html',
    providers: [FormularyAlternativeService],
    styleUrls: ['./formulary-alternatives.style.css']
})
export class FormularyAlternativesComponent extends BaseComponent implements OnInit {

    FormularyAlternatives: FormularyAlternative[];
    PendingRequest: any;

    constructor(private formularyAlternativeService: FormularyAlternativeService, cd: ElementRef, private svE: EventService,
     private svC: ComponentCommonService, private router : Router) {
        super(cd, svE);

        this.svE.subscribeEvent(EVENT_COMPONENT_ID.MedicationSelecting, () => {
            this.StartLoading();
        });

        this.svE.subscribeEvent(EVENT_COMPONENT_ID.OnMedicationSelected, (val: MedicationSelectedPayload) => {
            this.LoadData(val);
        });        

        this.svE.subscribeEvent(EVENT_COMPONENT_ID.OnMedicationDeSelected,() => {
            this.SetDefaultData();
        });
    }

    ngOnInit() {
        this.SetDefaultData();
    }

    SetDefaultData() {
        this.FormularyAlternatives = null;
        if (this.PendingRequest) {
            this.PendingRequest.unsubscribe();
        }
        this.EndLoading();
    }

    LoadData(medSelectedPayload: MedicationSelectedPayload) {
        this.SetDefaultData()
        if (this.svC.CheckVisibility(this.id, current_context.PageName)) {
            try {
                this.FormularyAlternatives = medSelectedPayload.FormularyAlternativesPayload;
                this.EndLoading();
            } catch (e) {
                this.EndLoading();
            }
        }
    }
    FormularyMedLinkClicked(medName: string) {
        this.router.navigateByUrl(ROUTE_NAME.SelectMedication, {
            state: {
                searchText: medName
            }
        });
    }
}
