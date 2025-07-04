import { Injectable, Inject, forwardRef } from '@angular/core'
import { BaseService } from '../base.service';
import { ErrorService } from '../service.import.def';
import { FormularyOverrideModel, ScriptPadModel, FormularyOverrideProcessMedModel } from '../../model/model.import.def';
import { HttpClient } from '@angular/common/http';

@Injectable()
export class FormularyOverrideService extends BaseService {
    constructor(http: HttpClient, @Inject(forwardRef(() => ErrorService)) svc: ErrorService) {
        super(http, svc);
    }

    GetFormularyOverideIgnoreReasons() {
        return this.InvokeApiMethod<FormularyOverrideModel>('/api/FormularyOverrideApi/GetFormularyOverideIgnoreReasons', null);
    }

    ClearOverrideRxListFromSession() {
        return this.InvokeApiMethod('/api/FormularyOverrideApi/ClearOverrideRxListFromSession', null);
    }

    FormularyOverrideProcessMedication(SelectedOverrideReason: any) {
        return this.InvokeApiMethod<FormularyOverrideProcessMedModel>('/api/FormularyOverrideApi/FormularyOverrideProcessMedication', SelectedOverrideReason);
    }
}
