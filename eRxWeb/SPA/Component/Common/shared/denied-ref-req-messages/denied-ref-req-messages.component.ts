import { Component, ViewChild } from '@angular/core';
import { DeniedRefReqMessagesService } from '../../../../services/service.import.def';
import { DeniedRefReqMessages } from '../../../../model/model.import.def';
import { EVENT_COMPONENT_ID, current_context } from '../../../../tools/constants';

import { ModalPopupControl } from '../../../shared/controls/modal-popup/modal-popup.control';
@Component({
    selector: 'denied-ref-req-message',
    templateUrl: './denied-ref-req-messages.template.html'
})
export class DeniedRefReqMessageComponent {

    @ViewChild(ModalPopupControl) modalPopup: ModalPopupControl;

    deniedRefReqMessages: DeniedRefReqMessages;
     constructor(private refReqSvc: DeniedRefReqMessagesService) {}
     OpenDeniedRefReqMessagePopup() {
         this.refReqSvc.GetDeniedRefReqMessages().subscribe((response) => {
             this.deniedRefReqMessages = response;
         })
        this.modalPopup.OpenPopup();
    }
}