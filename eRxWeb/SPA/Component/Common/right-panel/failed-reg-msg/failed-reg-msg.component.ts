
import { Component, ElementRef, Output, EventEmitter } from '@angular/core';

import { BaseComponent } from '../../base.component';

import { FailedRegMsgService, EventService } from '../../../../services/service.import.def';
import { FailedRegMsgModel } from '../../../../model/model.import.def';
import { EVENT_COMPONENT_ID, current_context } from '../../../../tools/constants';
@Component({
    selector: 'erx-failed-reg-msg',
    templateUrl: './failed-reg-msg.template.html',
    providers: [FailedRegMsgService]
})
export class FailedRegMsgComponent {

    title: string = '';
    failedRxMsg: FailedRegMsgModel[] = [];
    @Output() FailedRegMessagesChange: EventEmitter<any> = new EventEmitter<any>();
    constructor(private svE: EventService, private faileRxSvc: FailedRegMsgService) {

        this.svE.subscribeEvent(EVENT_COMPONENT_ID.FailedRegistrationMessageModalOpen, (val: any) => {            
            this.LoadData(val);
        });
    }

    Confirm(id: string) {
        this.faileRxSvc.ConfirmFailedRegMessage(id).subscribe((response) => {
            this.failedRxMsg = response;
            if (this.failedRxMsg !== undefined && this.failedRxMsg.length <= 0) {
                $('#mdlFailedRxMsg').modal('hide');
                this.FailedRegMessagesChange.emit(null);
            }
            else
                this.FailedRegMessagesChange.emit(response);
        });
    }
    LoadData(arg: any) {
        this.faileRxSvc.GetFailedRegMessage().subscribe((response) => {
            this.failedRxMsg = response;
            $('#mdlFailedRxMsg').modal();
        });
    }
}
