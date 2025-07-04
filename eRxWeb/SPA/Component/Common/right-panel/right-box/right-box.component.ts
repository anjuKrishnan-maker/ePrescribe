import {Component, ElementRef} from '@angular/core';

import {BaseComponent} from '../../base.component';

import { RightBoxService,EventService, ComponentCommonService } from '../../../../services/service.import.def';
import { RightBoxModel, RightPanelPayload } from '../../../../model/model.import.def';

import { EVENT_COMPONENT_ID, current_context } from '../../../../tools/constants';
@Component({
    selector: 'erx-right-box',
    templateUrl: './right-box.template.html',
    providers: [RightBoxService]
})
export class RightBoxComponent extends BaseComponent {
    rtBxModel: RightBoxModel = new RightBoxModel();
    constructor(cd: ElementRef, private svE: EventService, private compSvc: ComponentCommonService) {
        super(cd, svE);

        this.svE.subscribeEvent(EVENT_COMPONENT_ID.ContentRefreshing, () => {
            this.Hide();
        });

        this.svE.subscribeEvent(EVENT_COMPONENT_ID.RightPanelLoaded, (rightPanelPayload: RightPanelPayload) => {
            if (this.compSvc.CheckVisibility(this.id, current_context.PageName)) {
                this.rtBxModel = rightPanelPayload.RightBoxPayload;
                if (this.rtBxModel != undefined) {
                    this.Show();
                }
            }
        });
    }
}
