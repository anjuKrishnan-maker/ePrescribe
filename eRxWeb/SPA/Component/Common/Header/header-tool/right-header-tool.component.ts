import { Component, OnInit, Input } from '@angular/core';


import { PatientService, EventService } from '../../../../services/service.import.def';
import { EVENT_COMPONENT_ID, current_context, ROUTE_NAME, PAGE_NAME } from '../../../../tools/constants'
import { ILearnModel } from '../../../../model/model.import.def';
import { Router } from '@angular/router';

@Component({
    selector: 'erx-header-tool',
    templateUrl: './right-header-tool.template.html',

})

export class RightHeaderComponent {
    @Input() isRestrictedMenu: boolean;
    targetUrl: string;
    notificationCount: 0;

    get isException(): boolean {
        return current_context.isException;
    }

    constructor(private evSvc: EventService, private router: Router) {


        this.evSvc.subscribeEvent(EVENT_COMPONENT_ID.ILearnLoaded, (ilearnPayload: ILearnModel) => {
            if (!this.isException && ilearnPayload != undefined) {
                this.targetUrl = ilearnPayload.Url;

                    //Do not delete this code. This is required for a reimplementation at a later date
                    //try {
                    //    let videosCount = 0;
                    //    if (JSON.parse(ilearnPayload.ILearnNotification) instanceof Array) {
                    //    const iLearnPayloadJson = JSON.parse(ilearnPayload.ILearnNotification);
                    //        for (let i = 0; i < iLearnPayloadJson.length; i++){
                    //            videosCount += parseInt(iLearnPayloadJson[i].numberOfVideos);
                    //        }
                    //    } else {
                    //        videosCount = parseInt(JSON.parse(ilearnPayload.ILearnNotification).numberOfVideos);
                    //    }

                    //    this.notificationCount = videosCount;
                    //} catch (e) {
                    //    // need reporting 
                    //    this.notificationCount = 0;
                    //}
                }
            });
    }


    MessageQueueClicked() {       
        this.router.navigateByUrl(ROUTE_NAME.MessageQueueTx,
            {
                state: {
                    navigateTo: PAGE_NAME.MessageQueueTx +"?From="+current_context.PageName
                }
            });


    }
}
