import { Injectable,Inject,forwardRef } from '@angular/core';
import { BaseService } from '../base.service';
import { ErrorService } from '../service.import.def';
import { PatientMedRecDetailModel } from '../../model/model.import.def';
import { HttpClient } from '@angular/common/http';
@Injectable()
export class PatientMedRecService extends BaseService {
    constructor(http: HttpClient, @Inject(forwardRef(() => ErrorService)) svc: ErrorService) {
        super(http,svc);
    }
    GetEligibilityAndMedHistoryStatus() {
        return this.InvokeApiMethod<PatientMedRecDetailModel>('/api/PatientMedRecApi/GetMedReconciliationInfo', null);
 
    }

        UpdatePatientMedRecDetail(type: string) {
            return this.InvokeApiMethod<PatientMedRecDetailModel>('/api/PatientMedRecApi/UpdatePatientMedRecDetail', type);

        }

}