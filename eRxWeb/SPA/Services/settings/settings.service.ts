import { Injectable, Inject, forwardRef } from '@angular/core';
import { BaseService } from '../base.service';
import { ErrorService } from '../service.import.def';
import { SettingsLinkModel } from '../../model/model.import.def';
import { HttpClient } from '@angular/common/http';
import { API_ROUTE_NAME } from '../../tools/constants';
@Injectable()
export class SettingsService extends BaseService {
    constructor(http: HttpClient, @Inject(forwardRef(() => ErrorService)) svc: ErrorService) {
        super(http, svc);
    }
    RetrieveLinks() {
        return this.InvokeApiMethod<SettingsLinkModel[]>(API_ROUTE_NAME.SETTINGS_LIST_FETCHING, null);
    }
}