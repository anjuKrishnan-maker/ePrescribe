import { Injectable, Inject, forwardRef } from '@angular/core';
import { ErrorService } from '../service.import.def';
import { BaseService } from '../base.service';
import { RightPanelPayload } from '../../model/model.import.def';
import { HttpClient } from '@angular/common/http';
@Injectable()

export class RightPanelService extends BaseService {
    constructor(http: HttpClient, @Inject(forwardRef(() => ErrorService)) svc: ErrorService) {
        super(http, svc);
    }

    RetrievePayload(pageName: any) {
        return this.InvokeApiMethod<RightPanelPayload>('/api/RightPanel/RetrievePayload', pageName);
    }

}