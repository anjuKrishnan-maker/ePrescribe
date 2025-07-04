import { Injectable, Inject, forwardRef } from '@angular/core';
import { BaseService } from '../base.service';
import { ErrorService } from '../service.import.def';
import { ChangePasswordModel } from '../../model/model.import.def';
import { HttpClient } from '@angular/common/http';

@Injectable()
export class ChangePasswordService extends BaseService {
    constructor(http: HttpClient, @Inject(forwardRef(() => ErrorService)) svc: ErrorService) {
        super(http, svc);
    }
    ChangePassword(OldPassword: string, NewPassword: string) {        
        return this.InvokeApiMethod<ChangePasswordModel>('/api/ChangePasswordApi/ChangePassword', { OldPassword, NewPassword });
    }
}