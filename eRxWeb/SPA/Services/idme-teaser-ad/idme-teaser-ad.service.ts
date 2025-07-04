import { Injectable, Inject, forwardRef } from '@angular/core';
import { ErrorService } from '../service.import.def';
import { BaseService } from '../base.service';
import { HttpClient } from '@angular/common/http';

@Injectable()
export class IdmeTeaserAdService extends BaseService {
    constructor(http: HttpClient, @Inject(forwardRef(() => ErrorService)) svc: ErrorService) {
        super(http, svc);
    }
    GetRegistrationLink() {
        return this.InvokeApiMethod<String>('/api/IdmeTeaserAdApi/GetEpcsRegistrationLink', null);
    }
}