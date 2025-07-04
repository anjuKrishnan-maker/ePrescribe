import { Injectable, Inject, forwardRef } from '@angular/core';
import { ErrorService } from '../service.import.def';

import { BaseService } from '../base.service';
import { FailedRxMessage } from '../../model/model.import.def';
import { HttpClient } from '@angular/common/http';

@Injectable()
export class FailedRxMessagesService extends BaseService {
    constructor(http: HttpClient, @Inject(forwardRef(() => ErrorService)) svc: ErrorService) {
        super(http, svc);
    }
    GetFailedRxMessages() {
        return this.InvokeApiMethod<FailedRxMessage>('/api/FailedRxMessagesApi/GetFailedRxMessages', null);
    }

    ConfirmFailedRxMessages(requestId:string) {
        return this.InvokeApiMethod<FailedRxMessage>('/api/FailedRxMessagesApi/Confirm', requestId);
    }
}