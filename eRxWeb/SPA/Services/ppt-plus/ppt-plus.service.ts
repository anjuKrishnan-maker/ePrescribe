import { Injectable } from '@angular/core';
import { BaseService } from '../base.service';
import { PPTPlusSummaryUIResponse, PptPlusDetailsUserChangesResponse as PPTPlusDetailsUserChangesResponse } from '../../Model/model.import.def';
import { HttpClient } from '@angular/common/http';
import { Subject } from 'rxjs';
import { PptDetailContext } from '../../tools/constants';

@Injectable()
export class PptPlusService extends BaseService {
    constructor(http: HttpClient) {
        super(http);
    }

    InitiatePricingInquiry(data) {
        return this.InvokeApiMethod<number>('/api/PptPlusApi/InitiatePricingInquiry', data);
    }
    InitiatePricingInquiryBulk(data) {
        return this.InvokeApiMethod<number>('/api/PptPlusApi/InitiatePricingInquiryBulk', data);
    }

    InitiatePricingInquiryUsingSessionData(data) {
        return this.InvokeApiMethod<number>('/api/PptPlusApi/InitiatePricingInquiryUsingSessionData', data);
    }

    RetrieveSummaryUi(data) {
        return this.InvokeApiMethod<PPTPlusSummaryUIResponse>('/api/PptPlusApi/RetrieveSummaryUi', data);
    }

    RemoveUnselectedRows(data) {
        return this.InvokeApiMethod('/api/PptPlusApi/RemoveUnselectedRows', data);
    }

    RetrieveAllScriptPadMedSummaryUi() {
        return this.InvokeApiMethod<Array<string>>('/api/PptPlusApi/RetrieveAllScriptPadMedSummaryUi', null);
    }

    GetCommonUIUrl() {
        return this.InvokeApiMethod<string>('/api/PptPlusApi/GetCommonUIUrl', null);
    }    

    PPTPlusDetailsUserChanges(userChanges, pageContext) {
        return this.InvokeApiMethod<PPTPlusDetailsUserChangesResponse>('/api/PptPlusApi/PptDetailsHandleOKClick', { userChanges, pageContext });
    } 

    ShouldShowDetail() {
        return this.InvokeApiMethod<boolean>('/api/PptPlusApi/ShouldShowDetail', null);
    }

    IsAutoShowDetailScreen() {
        return this.InvokeApiMethod('/api/PptPlusApi/IsAutoShowDetailScreen', null);
    }
}