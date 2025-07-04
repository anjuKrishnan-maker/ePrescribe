import { BaseService } from "../base.service";
import { Injectable, Inject, forwardRef } from "@angular/core";
import { ErrorService } from "../service.import.def";
import { HttpClient } from "@angular/common/http";
import { DEALicense, MessageModel, DEALicenseRequest } from "../../model/model.import.def";

@Injectable()
export class DeaLicenseService extends BaseService {
    constructor(public http: HttpClient, @Inject(forwardRef(() => ErrorService)) svc: ErrorService) {
        super(http, svc);
    }

    GetDeaLicenseDetails(deaLicenseRequest: DEALicenseRequest) {
        return this.InvokeApiMethod<DEALicense[]>('/api/UserApi/GetUserDEALicenses', deaLicenseRequest);
    }

    InsertDeaLicenseDetails(deaLicenseRequest: DEALicenseRequest) {        
        return this.InvokeApiMethod<MessageModel>('/api/UserApi/AddUserDEALicense', deaLicenseRequest);
    }

    UpdateDeaLicenseDetails(deaLicenseRequest: DEALicenseRequest) {        
        return this.InvokeApiMethod<MessageModel>('/api/UserApi/UpdateDEALicense', deaLicenseRequest);
    }

    DeleteDeaLicenseDetails(deaLicenseRequest: DEALicenseRequest) {
        return this.InvokeApiMethod<MessageModel>('/api/UserApi/DeleteProviderDEALicense', deaLicenseRequest);
    }
}