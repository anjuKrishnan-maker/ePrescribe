import { Component, ViewChild, Output, EventEmitter } from '@angular/core';
import { FailedRxMessagesService } from '../../../../services/service.import.def';
import { FailedRxMessage } from '../../../../model/model.import.def';
import { EVENT_COMPONENT_ID, current_context } from '../../../../tools/constants';

import { ModalPopupControl } from '../../../shared/controls/modal-popup/modal-popup.control';
@Component({
    selector: 'failed-rx-message',
    templateUrl: './failed-rx-message.template.html'
})
export class FailedRxMessageComponent {

    @ViewChild(ModalPopupControl) modalPopup: ModalPopupControl;
    @Output() FailedMessagesChange: EventEmitter<any> = new EventEmitter<any>();
    failedRxMessage: FailedRxMessage;
    constructor(private failRxMsgSvc: FailedRxMessagesService) { }
    OpenFailedRxMessagePopup() {
        this.failRxMsgSvc.GetFailedRxMessages().subscribe((response) => {
            this.failedRxMessage = response;
        })
        this.modalPopup.OpenPopup();
    }

    ConfirmRequest(id: string) {
        this.failRxMsgSvc.ConfirmFailedRxMessages(id).subscribe((response) => {
            this.failedRxMessage = null;
            this.failedRxMessage = response;
            if (this.failedRxMessage != undefined && this.failedRxMessage.FailedRxMessages != undefined && this.failedRxMessage.FailedRxMessages.length > 0)
                this.FailedMessagesChange.emit();
            if (this.failedRxMessage != undefined && this.failedRxMessage.FailedRxMessages != undefined && this.failedRxMessage.FailedRxMessages.length <= 0)
                this.modalPopup.ClosePopup();

        });
     
    }
}