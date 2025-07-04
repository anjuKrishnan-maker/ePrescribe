import { Injectable, Inject, forwardRef } from '@angular/core';
import { BaseService } from '../base.service';
import { ErrorService } from '../service.import.def';
import { HttpClient, HttpHeaders, HttpResponse } from '@angular/common/http';
import { PatientUploadResponse } from '../../model/model.import.def';
@Injectable()
export class PatientUploadService extends BaseService {
    constructor(http: HttpClient, @Inject(forwardRef(() => ErrorService)) svc: ErrorService) {
        super(http, svc);
    }
    UploadFile(PatientData: string) {
        return this.InvokeApiMethod<PatientUploadResponse>('/api/PatientUpload/UploadFile', PatientData);
    }
    CheckJobStatus() {
        return this.Get<PatientUploadResponse>('/api/PatientUpload/GetJobStatus');
    }
    GenerateJobReport(JobID: number) {
        return this.Get<Blob>('/api/PatientUpload/GenerateReport', { 'job': JobID }, 'Blob');
    }
}