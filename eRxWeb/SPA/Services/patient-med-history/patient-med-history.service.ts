import { Injectable, Inject, forwardRef } from '@angular/core'
import { BaseService } from '../base.service';
import { ErrorService } from '../service.import.def';
import { PatientMedHistoryModel } from '../../model/model.import.def';
import { HttpClient } from '@angular/common/http';

@Injectable()
export class PatientMedHistoryService extends BaseService {
    constructor(http: HttpClient, @Inject(forwardRef(() => ErrorService)) svc: ErrorService) {
        super(http, svc);
    }

    GetPatientMedHistory() {
        return this.InvokeApiMethod<PatientMedHistoryModel[]>('/api/PatientMedHistoryAPI/GetPatientMedicationHistoryData', null);
    } 
}
