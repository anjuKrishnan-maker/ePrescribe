import { Injectable,Inject,forwardRef }     from '@angular/core';
import { BaseService } from '../base.service';
import { ErrorService } from '../service.import.def';
import { SiteInfo, User } from '../../model/model.import.def';
import { HttpClient } from '@angular/common/http';

@Injectable()
export class UserInfoService extends BaseService {
    constructor(http: HttpClient, @Inject(forwardRef(() => ErrorService)) svc: ErrorService) {
        super(http,svc);
    }

    GetSiteAndUserInfo(){
        return this.InvokeApiMethod<SiteInfo>('/api/AppBuildUpAPI/GetISiteInfo', null);
    }

    GetUserDetails() {
        return this.InvokeApiMethod<User>('/api/AppBuildUpAPI/GetUserDetails', null);

    }
}