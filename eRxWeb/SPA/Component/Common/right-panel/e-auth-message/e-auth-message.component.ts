import {Component, ElementRef} from '@angular/core';

import {BaseComponent} from '../../base.component';

import { EAuthMessageService, EventService, ComponentCommonService } from '../../../../services/service.import.def';
import { EAuthMessageModel, RightPanelPayload } from '../../../../model/model.import.def';
import { EVENT_COMPONENT_ID, NAVIGATION_EVENT_ID, current_context, ROUTE_NAME } from '../../../../tools/constants';
import { Router } from '@angular/router';

@Component({
    selector: 'erx-e-auth-msg',
    templateUrl: './e-auth-message.template.html',
    providers: [EAuthMessageService]
})
export class EAuthMessageComponent extends BaseComponent {
    eAuthMsg: EAuthMessageModel;
    constructor(cd: ElementRef, private svE: EventService, private compSvc: ComponentCommonService, private router: Router) {
        super(cd,svE);
        this.eAuthMsg = new EAuthMessageModel();

        this.svE.subscribeEvent(EVENT_COMPONENT_ID.ContentRefreshing, () => {
            this.Hide();
        });

        this.svE.subscribeEvent(EVENT_COMPONENT_ID.RightPanelLoaded, (rightPanelPayload: RightPanelPayload) => {
            if (this.compSvc.CheckVisibility(this.id, current_context.PageName)) {
                this.eAuthMsg = rightPanelPayload.EAuthMessagePayload;
                if (this.eAuthMsg != undefined) {
                    if (this.eAuthMsg.EPAResolvedMsgVisible == false && this.eAuthMsg.EPAOpenMsgVisible == false) {
                        this.Hide();
                    } else {
                        this.Show();
                    }
                }
            }
            else {
                this.Hide();
            }
        });
    }

    RedirectTo(src: string) {
        this.router.navigateByUrl(ROUTE_NAME.Tasks, { state: { navigateTo: src } });
    }

}
