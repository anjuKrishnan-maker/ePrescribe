import { Injectable } from '@angular/core';
import { BaseService } from '../base.service';
import {
    StatusFilterEnum, DataRetrievalContext, EieActionRequestModel, ReviewHistoryStartupParameters,
    GetPatientReviewHistoryResponse, FillHistoryTimelineModel, ApiResponse, SupervisingProviderModel
} from '../../model/model.import.def';
import { HttpClient } from '@angular/common/http';

@Injectable()
export class ReviewHistoryService extends BaseService {
    constructor(http: HttpClient) {
        super(http);
    }

    GetPatientReviewHistory(statusFilter: StatusFilterEnum, dataRetrievalContext: DataRetrievalContext) {
        return this.InvokeApiMethod<GetPatientReviewHistoryResponse>('/api/ReviewHistoryApi/GetPatientReviewHistory', { statusFilter: statusFilter, dataRetrievalContext: dataRetrievalContext });
    }

    GetFillHistoryData(rxId: string) {
        return this.InvokeApiMethod<FillHistoryTimelineModel[]>('/api/ReviewHistoryApi/GetFillHistoryData', rxId);
    }

    ExecuteCompleteAction(rxIdList: string[]) {
        return this.InvokeApiMethod<ApiResponse>('/api/ReviewHistoryApi/ExecuteCompleteActionMethod', rxIdList);
    }

    ExecuteDiscontinueAction(rxIdList: string[]) {
        return this.InvokeApiMethod<ApiResponse>('/api/ReviewHistoryApi/ExecuteDiscontinueAction', rxIdList);
    }

    SetNoActiveMedsFlag() {
        return this.InvokeApiMethod('/api/ReviewHistoryApi/SetNoActiveMedsFlag', null);
    }

    ExecuteEieAction(eieItemList: EieActionRequestModel[]) {
        return this.InvokeApiMethod('/api/ReviewHistoryApi/ExecuteEieAction', eieItemList);
    }

    AuditAccessAndGetStartupParameters() {
        return this.InvokeApiMethod<ReviewHistoryStartupParameters>('/api/ReviewHistoryApi/AuditAccessAndGetStartupParameters', null);
    }


    // putting this one here for now. Let's see if dedicated service is warranted
    DoesCurrentPatientHaveInactivatedAllergies() {
        return this.InvokeApiMethod<boolean>('/api/AllergyApi/DoesCurrentPatientHaveInactivatedAllergies', null);
    }

    IsAnyOfSelectedMedsAssociatedWithActiveEpaTask(rxIdList: string[]) {
        return this.InvokeApiMethod<ApiResponse>('/api/ReviewHistoryApi/IsAnyOfSelectedMedsAssociatedWithActiveEpaTask',
            rxIdList);
    }

    GetSupervisingProviders() {
        return this.InvokeApiMethod<SupervisingProviderModel[]>('/api/SupervisingProviderApi/GetSupervisingProviderList', null);
    }
    SetSupervisingProviders(provID: string) {
        return this.InvokeApiMethod('/api/SupervisingProviderApi/SetSupervisingProvider', provID);
    }

    AssignSupervisingProvider(providerId: string) {
        return this.InvokeApiMethod('/api/ReviewHistoryApi/AssignSupervisingProvider', providerId);
    }
}