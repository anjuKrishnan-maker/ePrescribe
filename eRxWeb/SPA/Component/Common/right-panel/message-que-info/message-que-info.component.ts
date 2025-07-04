import {Component, ElementRef} from '@angular/core';

import {BaseComponent} from '../../base.component';

import { MessageQueRxService, EventService, ComponentCommonService } from '../../../../services/service.import.def';
import { MessageQueInfoModel, MessageQueRequest } from '../../../../model/model.import.def';
import { EVENT_COMPONENT_ID,current_context } from '../../../../tools/constants';
@Component({
    selector: 'erx-message-queue',
    templateUrl: './message-que-info.template.html'
})
export class MessageQueInfoComponent extends BaseComponent {
    messageQueInfo: MessageQueInfoModel = null;
    constructor(private msgQueSvc: MessageQueRxService, cd: ElementRef, private svE: EventService, private _componentCommonService: ComponentCommonService) {
        super(cd);

        this.svE.subscribeEvent(EVENT_COMPONENT_ID.RightPanelLoaded,() => {
            this.visible = this._componentCommonService.CheckVisibility(this.id, current_context.PageName);
            if (!this.visible) {
                this.messageQueInfo = null;
            }
        });
    }
    content: any;

    SetMessageQueInfo(messageQueRequest : MessageQueRequest) {
        if (messageQueRequest) {
            this.LoadInfo(messageQueRequest);            
        }
        else {
            this.messageQueInfo = null;
        }  
        
    }

    LoadInfo(data: MessageQueRequest) {
        this.StartLoading();
        if (data.scriptMessageID == null) {
            this.messageQueInfo = null;
            this.Hide();
        }
        else{
            this.messageQueInfo = new MessageQueInfoModel();
            this.msgQueSvc.GetMessageQueInfo(data).subscribe(response => {
                    this.messageQueInfo = response;
                    this.EndLoading();
                    if (this.messageQueInfo == null)
                        this.Hide();
                    else
                    this.Show();

                }
                );
        }
    }

}
