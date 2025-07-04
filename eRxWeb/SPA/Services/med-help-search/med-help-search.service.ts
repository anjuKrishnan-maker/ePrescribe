import { Injectable, Inject,forwardRef }     from '@angular/core';
import {BaseService} from '../base.service';
import { ErrorService } from '../service.import.def';
import { MedHelpSearchModel } from '../../model/model.import.def';
import { HttpClient } from '@angular/common/http';
@Injectable()
export class MedHelpSearchService extends BaseService{
    constructor(http: HttpClient, @Inject(forwardRef(() => ErrorService)) svc: ErrorService) {
        super(http,svc);
    }
    GetSearchUrl(data) {
        return this.InvokeApiMethod<MedHelpSearchModel>('/api/MedicationHelpSerachAPI/GetSearchUrl', data);
    }
}