import { Injectable, Inject, forwardRef }     from '@angular/core';
import { ErrorService } from '../service.import.def';
import { BaseService } from '../base.service';
import { FormularyAlternative } from '../../model/model.import.def';
import { HttpClient } from '@angular/common/http';

@Injectable()
export class FormularyAlternativeService extends BaseService{
    constructor(http: HttpClient, @Inject(forwardRef(() => ErrorService)) svc: ErrorService) {
        super(http,svc);
    }
    GetFormularyAlternatives(data) {
        return this.InvokeApiMethod<FormularyAlternative[]>('/api/FormularyAlternativesApi/GetFormularyAlternatives', data);
    }
    GetFormularyAlternativesfromSession() {
        return this.InvokeApiMethod<FormularyAlternative>('/api/FormularyAlternativesApi/GetFormularyAlternativesFromSession', null);
    }
}