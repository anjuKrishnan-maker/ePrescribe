/// <reference path="../../../model/model.import.def.ts" />
import { Component } from '@angular/core';
import { ErrorService } from '../../../services/service.import.def';
import { ErrorContextModel } from '../../../model/model.import.def';
import { current_context } from '../../../tools/constants';

@Component({
    selector: 'erx-message-info',
    templateUrl: './message-info.template.html',
    styleUrls: ['./message-info.style.css']
})
export class MessageInfoComponent {
    error: ErrorContextModel = new ErrorContextModel();
    constructor(private evs: ErrorService) {
        evs.SubscribeError((data: ErrorContextModel) => {
            //ToDo:: Based on type we can redirect, display, warn, info
            //this.error = data;
            if (this.error != undefined)
                console.log(this.error.Message);
        });

    }
    close() {
        this.error.Message = null;
    }
}