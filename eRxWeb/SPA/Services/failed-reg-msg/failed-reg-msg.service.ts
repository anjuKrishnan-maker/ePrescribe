import { Injectable, Inject, forwardRef} from '@angular/core';
import { ErrorService } from '../service.import.def';
import { BaseService } from '../base.service';
import { FailedRegMsgModel } from '../../model/model.import.def';
import { HttpClient } from '@angular/common/http';

@Injectable()
export class FailedRegMsgService extends BaseService {
    constructor(http: HttpClient, @Inject(forwardRef(() => ErrorService)) svc: ErrorService) {
        super(http,svc);
    }
    ConfirmFailedRegMessage(data: string) {
        return this.InvokeApiMethod<FailedRegMsgModel[]>('/api/UrgentMessageApi/ConfirmFilRegMessage?requestID='+data,null);
    }
    GetFailedRegMessage() {
        return this.InvokeApiMethod<FailedRegMsgModel[]>('/api/UrgentMessageApi/GetFailedRxMessage', null);
    }
}