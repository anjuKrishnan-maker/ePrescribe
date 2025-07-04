import { Injectable, Inject, forwardRef } from '@angular/core';
import { ErrorService } from '../service.import.def';
import { BaseService } from '../base.service';
import { EPCSNotice } from '../../model/model.import.def';
import { HttpClient } from '@angular/common/http';

@Injectable()
export class EPCSService extends BaseService {
    constructor(http: HttpClient, @Inject(forwardRef(() => ErrorService)) svc: ErrorService) {
        super(http, svc);
    }
    GetPatientHeader() {
        return this.InvokeApiMethod<EPCSNotice>('/api/EPCSApi/GetAvailableReportsCount', null);
    }
    GetEPCSLink() {
        return this.InvokeApiMethod<Boolean>('/api/EPCSApi/DisplayEpscLink', null);
    }

}