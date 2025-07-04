import { Component } from '@angular/core';
import { BaseComponent } from '../../base.component';

import { RxDetailService, EventService, ComponentCommonService } from '../../../../services/service.import.def';
import { EVENT_COMPONENT_ID, current_context, NAVIGATION_EVENT_ID } from '../../../../tools/constants';
import { RxDetailModel } from '../../../../model/model.import.def';
@Component({
    selector: 'erx-rx-detail',
    templateUrl: './rx-detail.template.html'

})
export class RxDetailComponent extends BaseComponent {
    rxDetails: RxDetailModel;
    constructor(private rxSvc: RxDetailService,
        private svE: EventService, private compSvc: ComponentCommonService
    ) {
        super(null, svE);

        this.svE.subscribeEvent(EVENT_COMPONENT_ID.ContentRefreshing,() => {
                this.Hide();
        });

        this.svE.subscribeEvent(EVENT_COMPONENT_ID.OnHistoryRxSelected, (rxId: string) => {
            if (rxId !== null) {
                this.LoadRxDetails(rxId);
                this.Show();
            }
            else {
                this.rxDetails = null;
                this.Hide();
            }
        });
    }

    LoadRxDetails(rxId: string) {
        this.rxSvc.GetRxDetail(rxId).subscribe(response => {
            this.rxDetails = response;
            }
        )
    }
}
