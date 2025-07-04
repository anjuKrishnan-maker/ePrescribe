import { Injectable, Inject, forwardRef } from '@angular/core';
import { BaseService } from '../base.service';
import { ErrorService } from '../service.import.def';
import { MessageQueInfoModel } from '../../model/model.import.def';
import { HttpClient } from '@angular/common/http';
@Injectable()
export class MessageQueRxService extends BaseService {
    constructor(http: HttpClient, @Inject(forwardRef(() => ErrorService)) svc: ErrorService) {
        super(http, svc);
    }
    GetMessageQueInfo(data) {
        return this.InvokeApiMethod<MessageQueInfoModel>('/api/MessageQueueApi/GetMessageQueInfo', data);
    }


}