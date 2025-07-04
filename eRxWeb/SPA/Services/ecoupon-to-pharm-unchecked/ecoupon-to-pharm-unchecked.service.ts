import { Injectable } from '@angular/core';
import { BaseService } from '../base.service';
import { HttpClient } from '@angular/common/http';

@Injectable()
export class EcouponToPharmUncheckedService extends BaseService {
    constructor(http: HttpClient) {
        super(http);
    }

    SaveNeverShowPreference() {
        return this.InvokeApiMethod('/api/EcouponToPharmUncheckedApi/SaveNeverShowPreference', null);
    }
}