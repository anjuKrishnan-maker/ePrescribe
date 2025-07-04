import { Injectable } from '@angular/core';
import { BaseService } from '../base.service';
import { EligibilityMedHistoryModel } from '../../model/model.import.def';
import { HttpClient } from '@angular/common/http';

@Injectable()
export class EligAndMedHxStatusService extends BaseService {
    constructor(http: HttpClient) {
        super(http);
    }

    GetEligibilityAndMedHistoryStatus() {
        return this.InvokeApiMethod<Array<EligibilityMedHistoryModel>>('/api/EligibilityApi/GetEligibilityAndMedHistoryStatus', null);
    }
}