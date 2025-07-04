import { Injectable, Inject,forwardRef } from '@angular/core';
import { BaseService } from '../base.service';
import { ErrorService } from '../service.import.def';
import { AppInfoModel } from '../../model/model.import.def';
import { HttpClient } from '@angular/common/http';

@Injectable()
export class AppInfoService extends BaseService {

    baseUrl = "";

    constructor(http: HttpClient, @Inject(forwardRef(() => ErrorService)) svc: ErrorService) {
        super(http, svc);
    }

    GetAppInfo(){
        return this.InvokeApiMethod<AppInfoModel>('/api/AppBuildUpAPI/GetAppInfo', null);
    }
}