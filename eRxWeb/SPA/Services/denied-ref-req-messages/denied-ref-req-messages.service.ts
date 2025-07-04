import { Injectable, Inject, forwardRef } from '@angular/core';
import { Http } from '@angular/http';
import { ErrorService } from '../service.import.def';

import { BaseService } from '../base.service';
import { DeniedRefReqMessages } from '../../model/model.import.def';
import { HttpClient } from '@angular/common/http';

@Injectable()
export class DeniedRefReqMessagesService extends BaseService {
    constructor(http: HttpClient, @Inject(forwardRef(() => ErrorService)) svc: ErrorService) {
        super(http, svc);
    }
    GetDeniedRefReqMessages() {
        return this.InvokeApiMethod<DeniedRefReqMessages>('/api/FailedRefReqMessagesApi/GetDeniedRefReqMessages', null);
    }
}