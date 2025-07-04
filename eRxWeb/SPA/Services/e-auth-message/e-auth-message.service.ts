import { Injectable, Inject, forwardRef } from '@angular/core';
import { ErrorService } from '../service.import.def';

import { BaseService } from '../base.service';
import { EAuthMessageModel } from '../../model/model.import.def';
import { HttpClient } from '@angular/common/http';

@Injectable()
export class EAuthMessageService extends BaseService {
    constructor(http: HttpClient, @Inject(forwardRef(() => ErrorService)) svc: ErrorService) {
        super(http,svc);
    }
    GetEauthMessage() {
        return this.InvokeApiMethod<EAuthMessageModel>('/api/EAuthMessageApi/GetEauthMessage', null);
    }
}