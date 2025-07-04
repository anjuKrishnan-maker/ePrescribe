import { Injectable, Inject, forwardRef} from '@angular/core';
import { BaseService } from './base.service';
import { HttpClient } from '@angular/common/http';
import { HomeAddressStartUpModel, HomeAddressDetail, HomeAddressResponse } from '../model/home-address.model';
import { API_ROUTE_NAME } from '../tools/constants';
import { ErrorService } from './service.import.def';

@Injectable()
export class HomeAddressService extends BaseService {
    constructor(http: HttpClient, @Inject(forwardRef(() => ErrorService)) svc: ErrorService) {
        super(http, svc);
    }

    getStartUpdata() {
        return this.Get<HomeAddressStartUpModel>(API_ROUTE_NAME.HOME_ADDRESS_GET_START_UP_DATA);
    }

    save(homeAddressSaveRequest: HomeAddressDetail) {
        return this.InvokeApiMethod<HomeAddressResponse>(API_ROUTE_NAME.HOME_ADDRESS_SAVE, homeAddressSaveRequest);
    }   
}