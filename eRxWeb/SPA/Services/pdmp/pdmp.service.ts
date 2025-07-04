import { Injectable } from '@angular/core';
import { BaseService } from '../base.service';
import { PdmpUISummary, PdmpEnrollmentModel, PdmpEnrollmentSubmissionResponse } from '../../model/model.import.def';
import { HttpClient } from '@angular/common/http';

@Injectable()
export class PDMPService extends BaseService {
    constructor(http: HttpClient) {
        super(http);
    }

    GetPDMPSummary() {
        return this.InvokeApiMethod<PdmpUISummary>('/api/PdmpApi/GetPdmpSummary', null);
    }

    GetCommonUIUrl() {
        return this.InvokeApiMethod<string>('/api/PdmpApi/GetCommonUIUrl', null);
    }
    PDMPUserChanges(userEventName: string) {
        return this.InvokeApiMethod('/api/PdmpApi/PDMPDetailsButtonHandler', userEventName);
    }
    GetPdmpEnrollmentFormInfo() {
        return this.InvokeApiMethod<PdmpEnrollmentModel>('/api/PdmpApi/GetPdmpEnrollmentFormInfo', null);
    }

    SubmitPdmpEnrollmentForm(model: PdmpEnrollmentModel) {
        return this.InvokeApiMethod<PdmpEnrollmentSubmissionResponse>('/api/PdmpApi/PdmpEnrollmentFormSubmit', model);
    }
}