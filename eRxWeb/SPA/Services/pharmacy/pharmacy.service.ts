import { Injectable, Inject, forwardRef } from '@angular/core';
import { BaseService } from '../base.service';
import { ErrorService } from '../service.import.def';
import { PharmacyModel } from '../../model/model.import.def';
import { HttpClient } from '@angular/common/http';
@Injectable()
export class PharmacyService extends BaseService {
    constructor(http: HttpClient, @Inject(forwardRef(() => ErrorService)) svc: ErrorService) {
        super(http, svc);
    }
    GetPharmacy(data) {
        return this.InvokeApiMethod<PharmacyModel>('/api/PharmacyApi/GetPaharmacyByID', data);
    }

    GetSelectedMOPharmacy() {
        return this.InvokeApiMethod<PharmacyModel>('/api/PharmacyApi/GetCurrentMOPharmacyDetails', null);
    }

    GetSelectedRetailPharmacy() {
        return this.InvokeApiMethod<PharmacyModel>('/api/PharmacyApi/GetCurrentRetailPharmacyDetails', null);
    }

    DeleteSelectedMOPharmacy() {
        return this.InvokeApiMethod('/api/PharmacyApi/RemoveMOPharmacyFromPatient', null);
    }

    DeleteSelectedRetailPharmacy() {
        return this.InvokeApiMethod('/api/PharmacyApi/RemoveRetailPharmacyFromPatient', null);
    }

    RequestAddPharmacy(data) {
        return this.InvokeApiMethod<string>('/api/PharmacyApi/NewPharmacyRequest', data);
    }
}