import { Injectable, Inject, forwardRef } from '@angular/core';
import { BaseService } from '../base.service';
import { ErrorService } from '../service.import.def';
import { EpcsSendToPharmacyModel } from '../../model/model.import.def';
import { HttpClient } from '@angular/common/http';
@Injectable()

export class EpcsSendToPharmacyService extends BaseService {
    constructor(http: HttpClient, @Inject(forwardRef(() => ErrorService)) svc: ErrorService) {
        super(http,svc);
    }
    GetEpcsSendToPharmacy() {
        return this.InvokeApiMethod<EpcsSendToPharmacyModel>('/api/GetEpcsSendToPharmacyApi/GetEpcsSendToPharmacy', null);
    }
}