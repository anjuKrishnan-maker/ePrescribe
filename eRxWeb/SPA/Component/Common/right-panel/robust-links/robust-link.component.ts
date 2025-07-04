import {Component, ElementRef} from '@angular/core';

import {BaseComponent} from '../../base.component';

import { RobustLinksService, EventService, ComponentCommonService} from '../../../../services/service.import.def';
import { RobustLinkMesssagel, MedicationSelectedPayload } from '../../../../model/model.import.def';
import { EVENT_COMPONENT_ID, current_context } from '../../../../tools/constants';
import ObjectUtil from '../../../../tools/Utils/object.extension';
@Component({
    selector: 'erx-robust-link',
    templateUrl: './robust-link.template.html',
    providers: [RobustLinksService],
    styles: [`.small-text{font-size:smaller;color:blue;text-decoration: underline} 
        .small-text:hover{font-size:smaller;color: red;}
        .lable-text{font-size:12px;color:black;}
        .sponsoredLink a{text-decoration: underline;}`]
})
export class RobustLinksComponent extends BaseComponent {

    links: RobustLinkMesssagel[];
    PendingRequest: any;

    constructor(private sPLSvc: RobustLinksService, cd: ElementRef, private svE: EventService,
        private svC: ComponentCommonService) {
        super(cd, svE);

        this.svE.subscribeEvent(EVENT_COMPONENT_ID.ContentRefreshing,() => {
            this.SetDefaultData();
        });

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

    SetDefaultData() {
        this.links = null;
        if (this.PendingRequest) {
            this.PendingRequest.unsubscribe();
        }
        this.EndLoading();
    }

    LoadData(medLoadedPayload: MedicationSelectedPayload) {
        this.SetDefaultData()
        if (this.svC.CheckVisibility(this.id, current_context.PageName)) {
            try {
                this.links = medLoadedPayload.RobustLinkPayload;
                this.EndLoading();
            } catch (e) {
                this.EndLoading();
            }
        }
       
    }

}
