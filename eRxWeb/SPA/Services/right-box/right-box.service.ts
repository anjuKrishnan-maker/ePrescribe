import { Injectable,Inject,forwardRef }     from '@angular/core';
import { ErrorService } from '../service.import.def';
import { BaseService } from '../base.service';
import { RightBoxModel } from '../../model/model.import.def';
import { HttpClient } from '@angular/common/http';
@Injectable()
export class RightBoxService extends BaseService  {
    constructor(http: HttpClient, @Inject(forwardRef(() => ErrorService)) svc: ErrorService) {
        super(http,svc);
    }
    GetRighBoxData() {
        return this.InvokeApiMethod<RightBoxModel>('/api/RightBoxApi/GetRightBoxData', null);
    }
}