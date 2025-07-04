import { Component, Input, Output, EventEmitter } from '@angular/core';
import { EventService } from '../../../../services/service.import.def';
import { EVENT_COMPONENT_ID, current_context } from '../../../../tools/constants';
import { MessageModel, MessageIcon} from '../../../../model/model.import.def';

@Component({
    selector: 'erx-message',
    templateUrl: './message.template.html',
    inputs: ['Messages']
})

export class MessageComponent {
    @Input() Messages: Array<MessageModel>;
    @Output() MessagesChange: EventEmitter<Array<MessageModel>> = new EventEmitter<Array<MessageModel>>();

    constructor(private evSvc: EventService) {
    }

    public removeMessage(message: MessageModel)
    {
        this.Messages = this.Messages.filter(x => x !== message);
        this.MessagesChange.emit(this.Messages);
    }
}