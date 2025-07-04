import { Injectable,Inject,forwardRef }     from '@angular/core';
import { ErrorService } from '../service.import.def';
import { BaseService } from '../base.service';
import { UrgentMessageModel } from '../../model/model.import.def';
import { HttpClient } from '@angular/common/http';

@Injectable()
export class UrgentMessageService extends BaseService {
    constructor(http: HttpClient, @Inject(forwardRef(() => ErrorService)) svc: ErrorService) {
        super(http,svc);
    }
    GetUrgentMessage() {
        return this.InvokeApiMethod<UrgentMessageModel>('/api/UrgentMessageApi/GetUrgentMessage', null);
    }
}