import { Injectable ,Inject,forwardRef} from '@angular/core';
import { ErrorService } from '../service.import.def';
import { BaseService } from '../base.service';
import { HttpClient } from '@angular/common/http';

@Injectable()
export class ProgramAlertService extends BaseService {
    constructor(http: HttpClient, @Inject(forwardRef(() => ErrorService)) svc: ErrorService) {
        super(http,svc);
    }
    IsProviderEnrolledInSpecialtyMed() {
        return this.InvokeApiMethod<boolean>('/api/ProgramAlertApi/IsProviderEnrolledInSpecialtyMed', null);
    }
    IsScriptPadHasSpecmed() {
        return this.InvokeApiMethod<boolean>('/api/ProgramAlertApi/IsScriptPadHasSpecmed', null);
    }
}