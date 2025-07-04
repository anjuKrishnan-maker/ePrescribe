import { Injectable ,Inject,forwardRef}     from '@angular/core';
import { BaseService } from '../base.service';
import { ErrorService } from '../service.import.def';
import { ScriptPadModel } from '../../model/model.import.def';
import { HttpClient } from '@angular/common/http';
@Injectable()
export class ScriptPadservice extends BaseService{
    constructor(http: HttpClient, @Inject(forwardRef(() => ErrorService)) svc: ErrorService) {
        super(http,svc);}
    GetCurrentScripts(page: string) {
        return this.InvokeApiMethod<ScriptPadModel[]>('/api/ScriptPAdApi/GetCurrentScriptPad', page);
    }

    DeleteScript(rxId: string) {
        return this.InvokeApiMethod('/api/ScriptPAdApi/RemoveScript', rxId);
    }

    ReviewScriptPad() {
        return this.InvokeApiMethod<ScriptPadModel>('/api/ScriptPAdApi/ReviewScript', null);
    }

    CompleteScriptPadMedHistory(data) {
        return this.InvokeApiMethod<ScriptPadModel>('/api/ScriptPAdApi/CompleteScriptpadMedHistory', data);

    }
}