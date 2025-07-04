import {Component, ElementRef} from '@angular/core';

import {BaseComponent} from '../../base.component';

import { ImportantInformationService,EventService,ComponentCommonService } from '../../../../services/service.import.def';
import { ImportantInfoModel, RightPanelPayload } from '../../../../model/model.import.def';
import { EVENT_COMPONENT_ID,current_context } from '../../../../tools/constants';
@Component({
    selector: 'erx-imp-info',
    templateUrl: './important-information.template.html'
})
export class ImportantInformationComponent extends BaseComponent {
    ipmInfoItems: ImportantInfoModel[];
    constructor(cd: ElementRef, private svE: EventService,private compSvc:ComponentCommonService) {
        super(cd, svE);

        this.svE.subscribeEvent(EVENT_COMPONENT_ID.ContentRefreshing, () => {
            this.Hide();
        });

        this.svE.subscribeEvent(EVENT_COMPONENT_ID.RightPanelLoaded, (rightPanelPayload: RightPanelPayload) => {
            if (this.compSvc.CheckVisibility(this.id, current_context.PageName)) {
                this.ipmInfoItems = rightPanelPayload.ImportantInfoPayload;
                if (this.ipmInfoItems != undefined && this.ipmInfoItems.length > 0) {
                    this.Show();
                } else {
                    this.Hide();
                }
            }
        });
    }
}
