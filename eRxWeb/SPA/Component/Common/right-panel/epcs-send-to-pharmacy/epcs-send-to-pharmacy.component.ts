import { Component, ElementRef } from '@angular/core';
import { EpcsSendToPharmacyService, EventService, ComponentCommonService } from '../../../../services/service.import.def';
import { EpcsSendToPharmacyModel, RightPanelPayload } from '../../../../model/model.import.def';
import { BaseComponent } from '../../base.component';
import { EVENT_COMPONENT_ID, current_context } from '../../../../tools/constants';
@Component({
    selector: 'erx-epcs-send-to-pharmacy',
    templateUrl: './epcs-send-to-pharmacy.template.html'
})

export class EpcsSendToPharmacyComponent extends BaseComponent {
    epcsSendToPharmacy: EpcsSendToPharmacyModel = new EpcsSendToPharmacyModel();

    constructor(cd: ElementRef, private svE: EventService, private epcsSendToPharmacySvc: EpcsSendToPharmacyService, private svC: ComponentCommonService) {
        super(cd, svE);

        this.svE.subscribeEvent(EVENT_COMPONENT_ID.ContentRefreshing, () => {
            this.Hide();
        });

        this.svE.subscribeEvent(EVENT_COMPONENT_ID.RightPanelLoaded, (rightPanelPayload: RightPanelPayload) => {
            if (this.svC.CheckVisibility(this.id, current_context.PageName)) {
                this.epcsSendToPharmacy = rightPanelPayload.EpcsSendToPharmPayload;
                if (this.epcsSendToPharmacy != undefined && this.epcsSendToPharmacy.IsGetEpcsSendToPharmacyVisible == true) {
                    this.Show();
                }
                else {
                    this.Hide();
                }
            }
        });

    }
}
