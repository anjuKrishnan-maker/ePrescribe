import { Injectable, Inject, forwardRef } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { BaseService } from '../base.service';
import { Patient } from '../../model/patient.model';
import {
    SearchPatientStartupParameters, SearchPatientDataRequest, SearchPatientResponse, CheckInPatientDataRequest, CheckInPatientDataResponse,
    SupervisorProviderInfoRequest, SupervisorProviderInfoResponse, DelegateProvider, SetProviderInformationRequest, LoadProvidersForSupervisedPARequest, GetStartupParametersRequest
} from '../../model/model.import.def';
import { API_ROUTE_NAME } from '../../tools/constants';
import { ErrorService } from '../service.import.def';

@Injectable()
export class SelectPatientService extends BaseService {
    constructor(http: HttpClient, @Inject(forwardRef(() => ErrorService)) errorService: ErrorService) {
        super(http, errorService);
    }

    GetStartupParameters(getStartupParametersRequest: GetStartupParametersRequest) {
        return this.InvokeApiMethod<SearchPatientStartupParameters>('/api/SelectPatientApi/GetStartupParameters', getStartupParametersRequest);
    }
    
    SearchPatients(request: SearchPatientDataRequest){
        return this.InvokeApiMethod<SearchPatientResponse>('/api/SelectPatientApi/SearchPatients', request);
    }

    SetPatientInfo(patientId: string) {
        return this.InvokeApiMethod<Patient>(API_ROUTE_NAME.PATIENT_SERVICE__SET_PATIENT_INFO, patientId);
    }

    CheckInPatient(request: CheckInPatientDataRequest){
        return this.InvokeApiMethod<CheckInPatientDataResponse>('/api/SelectPatientApi/CheckInPatient', request);
    }

    SetProviderInformation(request: SetProviderInformationRequest){
        return this.InvokeApiMethod<boolean>('/api/SelectPatientApi/SetProviderInformation', request);
    }

    LoadProvidersForSupervisedPA(request: LoadProvidersForSupervisedPARequest) {
        return this.InvokeApiMethod<DelegateProvider[]>('/api/SelectPatientApi/LoadProvidersForSupervisedPA', request);
    }

    //CheckIfUserIsRestriced(patientId: string) {
    //    return this.InvokeApiMethod<boolean>('/api/SelectPatientApi/CheckIfUserIsRestricted', patientId);
    //}

    SetSupervisingProviderInfo(request: SupervisorProviderInfoRequest){
        return this.InvokeApiMethod<SupervisorProviderInfoResponse>('/api/SelectPatientApi/SetSupervisingProviderInfo', request);
    }
}