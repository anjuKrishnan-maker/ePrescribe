import { Injectable, Inject, forwardRef } from '@angular/core';
import { BaseService } from '../base.service';
import { ErrorService } from '../service.import.def';
import { SamlTokenRefresh} from '../../model/model.import.def';
import { HttpClient } from '@angular/common/http';
@Injectable()
export class UserSessionService extends BaseService {
    constructor(http: HttpClient, @Inject(forwardRef(() => ErrorService)) svc: ErrorService) {
        super(http, svc);
    }
    AttemptSamlTokenRefresh() {
        return this.InvokeApiMethod<SamlTokenRefresh>('/api/UserSessionApi/AttemptSamlTokenRefresh', null);
    }

    RetrieveSessionTimeoutMs() {
        return this.InvokeApiMethod('/api/UserSessionApi/RetrieveSessionTimeoutMs', null);
    }
}