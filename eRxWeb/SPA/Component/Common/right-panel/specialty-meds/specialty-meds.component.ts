import { Component, ElementRef } from '@angular/core';

import { BaseComponent } from '../../base.component';
import {RightPanelPayload } from '../../../../model/model.import.def';

import { ProgramAlertService, EventService ,ComponentCommonService} from '../../../../services/service.import.def';

import { EVENT_COMPONENT_ID, current_context} from '../../../../tools/constants';
@Component({
    selector: 'erx-spec-med-info',
    templateUrl: './specialty-meds.template.html',
    providers: [ProgramAlertService]
   
})
export class SpecialtyMedsComponent extends BaseComponent {

  
    constructor(private svE: EventService, private paSvc: ProgramAlertService, private compSvc: ComponentCommonService) {
        super(null, svE);

        this.svE.subscribeEvent(EVENT_COMPONENT_ID.ContentRefreshing, () => {
            this.Hide();
        });

        this.svE.subscribeEvent(EVENT_COMPONENT_ID.RightPanelLoaded, (rightPanelPayload: RightPanelPayload) => {
            if (this.compSvc.CheckVisibility(this.id, current_context.PageName)) {
                if (rightPanelPayload.SpecialtyMedInfoPayload === true) {
                    this.Show();
                }
                else {
                    this.Hide();
                }
            }
        });
    }
}
