import { Injectable, Inject, forwardRef}     from '@angular/core';
import { BaseService } from '../base.service';
import { ErrorService } from '../service.import.def';
import { GenericAlternative } from '../../model/model.import.def';
import { HttpClient } from '@angular/common/http';

@Injectable()


export class GenericAlternativeService extends BaseService {
    constructor(http: HttpClient, @Inject(forwardRef(() => ErrorService)) svc: ErrorService) {
        super(http,svc);
    }
    GetGenericAlternatives(data) {
        return this.InvokeApiMethod<GenericAlternative[]>('/api/GenericAlternativeApi/GetGenericAlternatives', data);
    }
}