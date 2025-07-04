import { Component, ElementRef } from '@angular/core';
import { BaseComponent } from '../../base.component';
import { RightPanelPayload } from '../../../../model/model.import.def';
import { EventService, EPCSService, ComponentCommonService } from '../../../../services/service.import.def';
import { EVENT_COMPONENT_ID, current_context, PAGE_NAME, ROUTE_NAME } from '../../../../tools/constants';
import { Subscription } from 'rxjs';
import { Router } from '@angular/router';



@Component({
    selector: 'erx-get-epcs',
    templateUrl: './get-epcs.template.html',
    styleUrls: ['./get-epcs.style.css']
})
export class GetEpcs extends BaseComponent {
    public ShowGetEpcsLink: Boolean;
    private summarySubscription: Subscription;
    PendingRequest: any;
    IsComponentVisible: boolean;

    constructor(private cd: ElementRef,
        private svE: EventService,
        private compSvc: ComponentCommonService,
        private epcsSvc: EPCSService,
        private router: Router
        ) {
        super(cd, svE);

        this.svE.subscribeEvent(EVENT_COMPONENT_ID.ContentRefreshing, () => {
            this.ShowGetEpcsLink = false;
            this.StartLoading();
        });

        this.svE.subscribeEvent(EVENT_COMPONENT_ID.RightPanelLoaded, (rightPanelPayload: RightPanelPayload) => {
            if (this.compSvc.CheckVisibility(this.id, current_context.PageName)) {
                this.ShowGetEpcsLink = rightPanelPayload.GetEpcsPayload;
                this.CheckUIVisiblity();
                this.EndLoading();
            }
        });
    }


    private CheckUIVisiblity() {
        
        this.ChangeVisiblity(this.ShowGetEpcsLink);
    }
    private ChangeVisiblity(visible: Boolean) {
        if (visible) {
            this.Show();
        }
        else {
            this.Hide();
        }
    }
    LinkClicked() {
        this.router.navigateByUrl(ROUTE_NAME.DeluxeFeatures);
    }
    
}