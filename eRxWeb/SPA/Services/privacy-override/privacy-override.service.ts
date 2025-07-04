import { Injectable ,Inject,forwardRef} from '@angular/core';
import { BaseService } from '../base.service';
import { ErrorService } from '../service.import.def';
import { PrivacyOverrideModel } from '../../model/model.import.def';
import { HttpClient } from '@angular/common/http';

@Injectable()
export class PrivacyOverrideService extends BaseService {
    constructor(http: HttpClient, @Inject(forwardRef(() => ErrorService)) svc: ErrorService) {
        super(http,svc);
    }

    SaveOverrideReason(overrideText: string, pageName: string) {
        return this.InvokeApiMethod('/api/PrivacyOverrideApi/SaveOverrideReason', {  overrideText,  pageName });
    }

    GetOverrideReason(patientId: string, userId: string, createdUtc: string) {
        return this.InvokeApiMethod<string>('/api/PrivacyOverrideApi/GetOverrideReason', {  patientId,  userId,  createdUtc });
    }

    Cancel(pageName: string) {
        return this.InvokeApiMethod('/api/PrivacyOverrideApi/Cancel', { pageName });
    }

    CheckIsRestrictedUser(patientId: string, userId: string) {
        return this.InvokeApiMethod<PrivacyOverrideModel>('/api/PrivacyOverrideApi/CheckIsRestrictedUser', { patientId, userId });
    }
}