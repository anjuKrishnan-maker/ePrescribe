import { Injectable, Inject, forwardRef } from '@angular/core';
import { ErrorService } from '../service.import.def';
import { BaseService } from '../base.service';
import { RxDetailModel } from '../../model/model.import.def';
import { HttpClient } from '@angular/common/http';
@Injectable()
export class RxDetailService extends BaseService {
    constructor(http: HttpClient, @Inject(forwardRef(() => ErrorService)) svc: ErrorService) {
        super(http, svc);
    }
    GetRxDetail(rxId: string) {
        return this.InvokeApiMethod<RxDetailModel>('/api/RxDetailApi/GetRxDetail', rxId);
    }


}